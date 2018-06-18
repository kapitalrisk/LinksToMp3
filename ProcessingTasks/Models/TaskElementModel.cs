using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YoutubeExtractor;
using yt2mp3.FileHandler.Models;

namespace yt2mp3.ProcessingTasks.Models
{
    internal class TaskElementModel
    {
        public string link;
        public VideoInfo info;
        public string videoFilePath;
        public string audioFilePath;
        public bool IsVideoInfoTried;
        public bool IsDownloadTried;
        public bool IsConvertionTried;

        public TaskElementModel(LinksModel l)
        {
            this.link = l.rawUrl;
        }

        public TaskElementModel(string rawUrl)
        {
            this.link = rawUrl;
            IsVideoInfoTried = false;
            IsDownloadTried = false;
            IsConvertionTried = false;
        }
        public bool IsRawUrlCorrect()
        {
            if (!Uri.IsWellFormedUriString(link, UriKind.Absolute))
                return false;
            return true;
        }

        public bool IsVideoInfoComplete()
        {
            if (info == null)
                return false;
            if (String.IsNullOrWhiteSpace(info.Title))
                return false;
            if (String.IsNullOrWhiteSpace(info.VideoExtension))
                return false;
            return true;
        }

        public bool IsDownloaded()
        {
            if (String.IsNullOrWhiteSpace(videoFilePath))
                return false;
            if (!File.Exists(videoFilePath))
                return false;
            return true;
        }

        public bool IsConverted()
        {
            if (String.IsNullOrWhiteSpace(audioFilePath))
                return false;
            if (!File.Exists(audioFilePath))
                return false;
            return true;
        }
    }
}
