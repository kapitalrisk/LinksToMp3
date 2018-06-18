using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yt2mp3.ProcessingTasks.Models;

namespace yt2mp3.ProcessingTasks.SimpleTasks.Interface
{
    interface ISimpleTask
    {
        TaskElementModel Process(TaskElementModel tem);
    }
}
