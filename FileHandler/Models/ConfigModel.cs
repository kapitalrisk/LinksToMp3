namespace yt2mp3.FileHandler.Models
{

    public class ConfigModel
    {
        /// <summary>
        /// Full path to video folder where they will be downloaded.
        /// Default "C:/temp/videoFolder"
        /// </summary>
        public string videoFolderPath { get; set; }

        /// <summary>
        /// Full path to audio folder where they will be saved.
        /// Default "C:/temp/audioFolder"
        /// </summary>
        public string audioFolderPath { get; set; }

        /// <summary>
        /// Path to file containing YouTube video links to download in CSV format.
        /// Default "C:/temp/ytLinks.csv"
        /// </summary>
        public string ytLinksCsv { get; set; }

        /// <summary>
        /// Maximum number of Tasks launched asynchronously to retrieve informations on video before download.
        /// Light ressource operation.
        /// Default 10. Min 1. Max 100.
        /// </summary>
        public int maxAsyncVideoInfo { get; set; }

        /// <summary>
        /// Maximum number of Tasks launched asynchronously to download video file.
        /// Default 10. Min 1. Max 100.
        /// Heavy ressource operation.
        /// </summary>
        public int maxAsyncVideoDownloads { get; set; }

        /// <summary>
        /// Maximum number of Tasks launched asynchronously to convert video file to audio
        /// Default 10. Min 1. Max 100.
        /// Medium ressource operation.
        /// </summary>
        public int maxAsyncConvertions { get; set; }

        /// <summary>
        /// Set of parameters for convertion handling.
        /// </summary>
        public ConvertionOptions convertionOptions { get; set; }
    }

    public class ConvertionOptions
    {
        /// <summary>
        /// Determine if videos files needs to be deleted after convertion.
        /// Might be usefull to set to false if multiple run are done for different audio format / quality / ...
        /// Default true.
        /// </summary>
        public bool deleteAfterConvertion { get; set; }

        /// <summary>
        /// Audio file format retrieve from video.
        /// Default mp3.
        /// </summary>
        public string audioFormat { get; set; }

        /// <summary>
        /// Audio file quality in dskfjsdlkjfsidljg
        /// Default 320 (high). Between 96 and 320 kbps.
        /// </summary>
        public int kbpsQuality { get; set; }
    }

}
