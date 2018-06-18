using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yt2mp3.FileHandler.Models;
using yt2mp3.ProcessingTasks.Models;
using yt2mp3.ProcessingTasks.SimpleTasks.Interface;

namespace yt2mp3.ProcessingTasks
{
    public class ProcessingTasksManager
    {
        #region properties & ctor
        private ConfigModel config;
        private IList<LinksModel> links;
        private IList<TaskElementModel> taskElements;
        private IList<Task> taskList;

        public ProcessingTasksManager(ConfigModel config, IList<LinksModel> links)
        {
            this.config = config;
            this.links = links;
            taskList = new List<Task>();
        }
        #endregion

        #region processing
        public async Task<bool> Start()
        {
            MakeTaskElementList();
            MakeVideoInfoRetrieverTaskList();
            await ProcessTasks();
            MakeDownloadVideoTaskList();
            await ProcessTasks();
            MakeConvertionTaskList();
            await ProcessTasks();

            return true;
        }

        private async Task<bool> ProcessTasks()
        {
            foreach (var t in taskList)
            {
                t.Start();
                await t;
            }
            return true;
        }
        #endregion

        #region taskElementList
        private void MakeTaskElementList()
        {
            taskElements = new List<TaskElementModel>();

            foreach (var l in links)
                taskElements.Add(new TaskElementModel(l.rawUrl));
        }
        #endregion

        #region videoInfo
        private void MakeVideoInfoRetrieverTaskList()
        {
            taskList.Clear();

            for (int i = 0; i < config.maxAsyncVideoDownloads && i < taskElements.Count; i++)
                if (taskElements[i].IsRawUrlCorrect())
                    AddNextVideoInfoTask();
        }

        private void AddNextVideoInfoTask()
        {
            TaskElementModel temp = null;
            
            foreach (var e in taskElements)
                if (!e.IsVideoInfoTried && !e.IsVideoInfoComplete())
                    temp = e;
            if (temp != null)
            {
                var parentTask = new Task(() =>
                {
                    var tsk = new VideoInfoRetrieverTask();
                    temp = tsk.Process(temp);
                });
                parentTask.ContinueWith((previous) =>
                {
                    AddNextVideoInfoTask();
                }); 
                taskList.Add(parentTask);
            }
        }
        #endregion

        #region downloadVideo
        private void MakeDownloadVideoTaskList()
        {
            taskList.Clear();

            for (int i = 0; i < config.maxAsyncVideoDownloads && i < taskElements.Count; i++)
                if (taskElements[i].IsVideoInfoComplete())
                    AddNextVideoDownloadTask();
        }
        
        private void AddNextVideoDownloadTask()
        {
            TaskElementModel temp = null;

            foreach (var e in taskElements)
                if (e.IsVideoInfoComplete() && !e.IsDownloadTried && !e.IsDownloaded())
                    temp = e;
            if (temp != null)
            {
                var parentTask = new Task(() =>
                {
                    var tsk = new DownloadVideoFileTask();
                    tsk.Process(out temp);
                });
                parentTask.ContinueWith((previous) => 
                {
                    AddNextVideoDownloadTask();
                });
                taskList.Add(parentTask);
            }
        }
        #endregion

        #region convertion
        private void MakeConvertionTaskList()
        {
            taskList.Clear();

            for (int i = 0; i < config.maxAsyncConvertions && i < taskElements.Count; i++)
                if (taskElements[i].IsDownloaded())
                    AddNextConvertionTask();
        }

        private void AddNextConvertionTask()
        {
            TaskElementModel temp = null;

            foreach (var e in taskElements)
                if (e.IsVideoInfoComplete() && !e.IsDownloadTried && !e.IsDownloaded())
                    temp = e;
            if (temp != null)
            {
                var parentTask = new Task(() =>
                {
                    var tsk = new ConvertVideoToAudioTask();
                    tsk.Process(out temp);
                });
                parentTask.ContinueWith((previous) =>
                {
                    AddNextVideoDownloadTask();
                });
                taskList.Add(parentTask);
            }
        }
        #endregion
    }
}
