using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExtractor;
using yt2mp3.ProcessingTasks.Models;
using yt2mp3.ProcessingTasks.SimpleTasks.Interface;

namespace yt2mp3.ProcessingTasks
{
    internal class VideoInfoRetrieverTask : ISimpleTask
    {
        public TaskElementModel Process(TaskElementModel tem)
        {
            tem.IsVideoInfoTried = true;

            if (tem.IsRawUrlCorrect())
            {
                IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(tem.link);

                if (videoInfos.Count<VideoInfo>() == 0)
                    tem.info = null;
                VideoInfo video = videoInfos
                    .FirstOrDefault(info => info.VideoType == VideoType.Mp4 && info.Resolution == 360);
                tem.info = video;
            }
            return tem;
        }
    }
}
