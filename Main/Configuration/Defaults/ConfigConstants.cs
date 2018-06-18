namespace yt2mp3.Configuration.Defaults
{
    /// <summary>
    /// Default constants. See ConfigFileModel for each field documentation.
    /// </summary>
    class ConfigConstants
    {
        public const string videoFolderPath = @"C:\temp\videoFolder";
        public const string audioFolderPath = @"C:\temp\audioFolder";
        public const string ytLinksCsv = @"C:\temp\ytLinks.csv";
        public const int maxAsyncVideoInfo = 10;
        public const int maxAsyncVideoDownloads = 10;
        public const int maxAsyncConvertions = 10;
        public const bool deleteAfterConvertion = true;
        public const string audioFormat = "mp3";
        public const int kbpsQuality = 320;
    }
}
