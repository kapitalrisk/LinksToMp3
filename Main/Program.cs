using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExtractor;
using yt2mp3.Configuration;
using yt2mp3.ProcessingTasks;

namespace yt2mp3
{
    class Program
    {
        public static string mp4Folder = @"C:\temp\videoFolder";
        public static string mp3Folder = @"C:\temp\audioFolder";
        public static string ffmpegBinPath = @"C:\Users\Marjorie\Documents\ffmpeg\bin\ffmpeg.exe";
        public static string[] videoList;

        static void Main(string[] args)
        {
            
            //videoList = ToProcess.videoList.Except(AlreadyProcess.videoList).ToArray();

            //if (videoList.Length > 0)
            //    processAll();
            //else
            //    Console.WriteLine("No video to process");
            Console.WriteLine("END");
            Console.ReadKey();
        }

        static async Task MainAsync(string[] args)
        {
            ConfigManager cm = new ConfigManager();

            cm.SetupConfiguration();
            var ptm = new ProcessingTasksManager(cm);
            await ptm.Start();

        }

        static VideoInfo getVideoInfo(string videoLink)
        {
            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(videoLink);

            if (videoInfos.Count<VideoInfo>() == 0)
                return null;
            VideoInfo video = videoInfos
                .FirstOrDefault(info => info.VideoType == VideoType.Mp4 && info.Resolution == 360);
            return video;
        }

        static void download(VideoInfo video, string mp4Path)
        {
            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }
            var videoDownloader = new VideoDownloader(video, @Path.Combine(mp4Folder, video.Title + video.VideoExtension));
            //videoDownloader.DownloadProgressChanged += (sender, args) => Console.WriteLine(args.ProgressPercentage);
            videoDownloader.Execute();
        }

        static void convert(string input, string output)
        {
            var ffmpegOutput = "";
            var ffmpegProcess = new Process();
            ffmpegProcess.StartInfo.UseShellExecute = false;
            ffmpegProcess.StartInfo.RedirectStandardInput = true;
            ffmpegProcess.StartInfo.RedirectStandardOutput = true;
            ffmpegProcess.StartInfo.RedirectStandardError = true;
            ffmpegProcess.StartInfo.CreateNoWindow = true;
            ffmpegProcess.StartInfo.FileName = ffmpegBinPath;
            ffmpegProcess.StartInfo.Arguments = " -i " + "\"" + input + "\"" + " -vn -f mp3 -ab 320k " + "\"" + output + "\"";
            ffmpegProcess.Start();
            //ffmpegProcess.StandardOutput.ReadToEnd();
            ffmpegOutput = ffmpegProcess.StandardError.ReadToEnd();
            ffmpegProcess.WaitForExit();

            if (!ffmpegProcess.HasExited)
                ffmpegProcess.Kill();
            Console.WriteLine(ffmpegOutput);
        }

        static void processAll()
        {
            foreach (var l in videoList)
            {
                string mp4File = string.Empty;
                string mp3File = string.Empty;
                string mp4Path = string.Empty;
                string mp3Path = string.Empty;
                VideoInfo video;

                Console.WriteLine($"Processing {l}...");

                try
                {
                    video = getVideoInfo(l);
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occur trying to retrieve information on video - goto next");
                    continue;
                }
                if (video != null)
                {
                    mp4File = video.Title + video.VideoExtension;
                    mp3File = video.Title + ".mp3";
                    mp4Path = Path.Combine(mp4Folder, mp4File);
                    mp3Path = Path.Combine(mp3Folder, mp3File);
                    Console.WriteLine("Successfully retrieve video infos");
                    Console.WriteLine("video path : " + mp4Path);
                    Console.WriteLine("audio path : " + mp3Path);
                }
                else
                {
                    Console.WriteLine("An error occur trying to retrieve video info - goto next one");
                    continue;
                }

                if (!File.Exists(Path.Combine(mp4Folder, mp4File)) && !File.Exists(Path.Combine(mp3Folder, mp3File)))
                {
                    Console.WriteLine($"mp4 file does not exists nor mp3");
                    
                    if (video != null)
                    {
                        Console.WriteLine("Start download");

                        try
                        {
                            download(video, mp4Path);
                            Console.WriteLine("Download ended");
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine("An exception occur trying to download video - goto next one" + e.ToString());
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("An error occur while retrieving video info - check link");
                    }
                }
                else if (!File.Exists(Path.Combine(mp4Folder, mp4File)) && File.Exists(Path.Combine(mp3Folder, mp3File)))
                {
                    Console.WriteLine("Apparently mp3 already exists no need to download nor convert");
                    continue;
                }
                else
                {
                    Console.WriteLine("mp4 file exists but no mp3");
                }
                if (!File.Exists(Path.Combine(mp3Folder, mp3File)) && File.Exists(Path.Combine(mp4Folder, mp4File)))
                {
                    Console.WriteLine("Converting to mp3...");
                    convert(mp4Path, mp3Path);
                    Console.WriteLine("Convert ended - deleting mp4 file");
                    File.Delete(Path.Combine(mp4Folder, mp4File));
                }
                else if (!File.Exists(Path.Combine(mp3Folder, mp3File)) && !File.Exists(Path.Combine(mp4Folder, mp4File)))
                {
                    Console.WriteLine("An error must have occur with mp4 file - FileNotFound");
                }
                else
                {
                    Console.WriteLine("mp3 already exists - deleting mp4 file");
                    File.Delete(Path.Combine(mp4Folder, mp4File));
                } 
            }
        }
    }
}
