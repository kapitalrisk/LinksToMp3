using System;
using System.Collections.Generic;
using YoutubeExtractor;
using yt2mp3.Configuration.Defaults;
using yt2mp3.FileHandler;
using yt2mp3.FileHandler.Models;
using yt2mp3.Tools;

namespace yt2mp3.Configuration
{
    internal class ConfigManager
    {
        #region Properties & ctor
        private ConfigModel configFields;
        private IEnumerable<string> videoLinks;

        public ConfigModel ConfigFields { get => configFields; set => configFields = value; }
        public IEnumerable<string> VideoLinks { get => videoLinks; set => videoLinks = value; }

        public ConfigManager()
        {
            // FIXME Logs
        }
        #endregion

        #region ConfigSetup
        internal void SetupConfiguration()
        {
            var defaultConfig = GetConfigurationFromDefaults();

            try
            {
                ConfigFields = GetConfigurationFromFile();
            }
            catch (Exception e)
            {
                // FIXME log
                throw e;
            }

            // EVOLUTION
            // find a better way to set default values to properties into a loop instead of comparing field by field with temp model class
            if (String.IsNullOrWhiteSpace(ConfigFields.audioFolderPath))
                ConfigFields.audioFolderPath = defaultConfig.audioFolderPath;
            if (String.IsNullOrWhiteSpace(ConfigFields.videoFolderPath))
                ConfigFields.videoFolderPath = defaultConfig.videoFolderPath;
            if (String.IsNullOrWhiteSpace(ConfigFields.ytLinksCsv))
                ConfigFields.ytLinksCsv = defaultConfig.ytLinksCsv;
            if (ConfigFields.maxAsyncVideoInfo <= 0 || ConfigFields.maxAsyncVideoInfo > 100)
                ConfigFields.maxAsyncVideoInfo = defaultConfig.maxAsyncVideoInfo;
            if (ConfigFields.maxAsyncVideoDownloads <= 0 || ConfigFields.maxAsyncVideoDownloads > 100)
                ConfigFields.maxAsyncVideoDownloads = defaultConfig.maxAsyncVideoDownloads;
            if (ConfigFields.maxAsyncConvertions <= 0 || ConfigFields.maxAsyncConvertions > 100)
                ConfigFields.maxAsyncConvertions = defaultConfig.maxAsyncConvertions;
            if (!ConfigFields.convertionOptions.audioFormat.Equals("mp3"))
                ConfigFields.convertionOptions.audioFormat = defaultConfig.convertionOptions.audioFormat;
            if (ConfigFields.convertionOptions.kbpsQuality < 96 || ConfigFields.convertionOptions.kbpsQuality > 320)
                ConfigFields.convertionOptions.kbpsQuality = defaultConfig.convertionOptions.kbpsQuality;
            SetupVideoLinksList();
        }
        #endregion

        #region VideoLinksSetup
        private void SetupVideoLinksList()
        {
            if (ConfigFields == null || String.IsNullOrWhiteSpace(ConfigFields.ytLinksCsv))
                throw new ArgumentNullException();
            string rawCsv = string.Empty;

            try
            {
                rawCsv = FileManager.GetFileRawContent(ConfigFields.ytLinksCsv);
            }
            catch (Exception e)
            {
                // FIXME Logs
                throw e;
            }
            if (String.IsNullOrWhiteSpace(rawCsv)) // FIXME LOGS
                throw new Exception("Links file content null or empty");

            try
            {
                VideoLinks = DataFormat.TryParseVideoLinksList(rawCsv); 
            }
            catch (Exception e)
            {
                // FIXME LOGS
            }
        }
        #endregion

        #region Internal process
        private ConfigModel GetConfigurationFromFile(string configFilePath = null)
        {
            if (String.IsNullOrWhiteSpace(configFilePath))
                configFilePath = ApplicationConstants.ConfigFilePath;
            string rawConfig = string.Empty;

            try
            {
                rawConfig = FileManager.GetFileRawContent(configFilePath);
            }
            catch (Exception e)
            {
                // FIXME log
                throw e;
            }

            if (String.IsNullOrWhiteSpace(rawConfig)) // FIXME Logs
                throw new Exception("Configuration file empty !");
            ConfigModel result = new ConfigModel();

            try
            {
                result = Serializor.DeSerialize<ConfigModel>(rawConfig);
            }
            catch (Exception e)
            {
                // FIXME log
                throw e;
            }
            return result;
        }

        private ConfigModel GetConfigurationFromDefaults()
        {
            return new ConfigModel
            {
                audioFolderPath = ConfigConstants.audioFolderPath,
                videoFolderPath = ConfigConstants.videoFolderPath,
                ytLinksCsv = ConfigConstants.ytLinksCsv,
                maxAsyncVideoInfo = ConfigConstants.maxAsyncVideoInfo,
                maxAsyncVideoDownloads = ConfigConstants.maxAsyncVideoDownloads,
                maxAsyncConvertions = ConfigConstants.maxAsyncConvertions,
                convertionOptions = new ConvertionOptions
                {
                    deleteAfterConvertion = ConfigConstants.deleteAfterConvertion,
                    audioFormat = ConfigConstants.audioFormat,
                    kbpsQuality = ConfigConstants.kbpsQuality
                }
            };
        }
        #endregion
    }
}
