using System;
using System.IO;
using IWshRuntimeLibrary;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using Microsoft.EntityFrameworkCore;
using DesktopLearningAssistant.TomatoClock.Model;

namespace DesktopLearningAssistant.TomatoClock.SQLite
{
    class TaskService
    {
        #region 接口函数
        public void AddTask(TaskInfo taskInfo)
        {
            using (var context = Context)
            {
                var task = new TaskList()
                {
                    Name = taskInfo.Name,
                    Notes = taskInfo.Notes,
                    StartTime = taskInfo.StartTime,
                    Deadline = taskInfo.Deadline,
                    TomatoNum = taskInfo.TomatoNum,
                    TomatoCount = 0,
                    State = taskInfo.TaskState
                };
                context.Tasks.Add(task);
                context.SaveChanges();
            }
        }
        public void DeletTask(int iTaskID)
        {
            using (var context = Context)
            {
                var task = context.Tasks.Include(t => t.TaskTomatoLists).FirstOrDefault(tt => tt.TaskID == iTaskID);
                if (task != null)
                {
                    context.Tasks.Remove(task);
                    context.SaveChanges();
                }
            }
        }
        public void ModifyTask(TaskInfo taskInfo)
        {
            using (var context = Context)
            {
                var task = new TaskList()
                {
                    TaskID = taskInfo.TaskID,
                    Name = taskInfo.Name,
                    Notes = taskInfo.Notes,
                    StartTime = taskInfo.StartTime,
                    Deadline = taskInfo.Deadline,
                    TomatoNum = taskInfo.TomatoNum,
                    TomatoCount = taskInfo.TomatoCount,
                    State = taskInfo.TaskState
                };
                context.Entry(task).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
        public TaskInfo ReadTask(int iTaskID)
        {
            using (var context = Context)
            {
                TaskInfo taskInfo = new TaskInfo();
                var task = context.Tasks.SingleOrDefault(t => t.TaskID == iTaskID);
                if (task != null)
                {
                    taskInfo.Name = task.Name;
                    taskInfo.Notes = task.Notes;
                    taskInfo.StartTime = task.StartTime;
                    taskInfo.Deadline = task.Deadline;
                    taskInfo.TomatoCount = task.TomatoCount;
                    taskInfo.TomatoNum = task.TomatoNum;
                    taskInfo.TaskState = task.State;
                }
                return taskInfo;
            }
        }
        private void ChangeTaskToFinishState(int iTaskID)
        {
            using (var context = Context)
            {
                var task = new TaskList() { TaskID = iTaskID, State = 1 };
                context.Entry(task).State = EntityState.Modified;
                context.SaveChanges();
            }
        }
        public int AddTomatoStartTime(int iTaskID)
        {
            using (var context = Context)
            {
                var tomato = new TaskTomatoList() { BeginTime = DateTime.Now, TaskID = iTaskID };
                context.Entry(tomato).State = EntityState.Added;
                context.SaveChanges();
                return tomato.TomatoID;
            }
        }
        public void AddTomatoEndTime(int iTaskID, int iTomatoID)
        {
            using (var context =Context)
            {
                var tomato = new TaskTomatoList() { TaskID = iTaskID, TomatoID = iTomatoID, EndTime = DateTime.Now };
                context.Entry(tomato).State = EntityState.Modified;
                var task = context.Tasks.FirstOrDefault(t => t.TaskID == iTaskID);
                if (task != null)
                {
                    task.TomatoCount = AddTomatoNum(tomato.BeginTime, tomato.EndTime, task.TomatoCount);
                }
                var taskfile = context.TaskFileLists.FirstOrDefault(f => f.TaskID == iTaskID);
                if (taskfile != null)
                {
                    GetFilePath(tomato.BeginTime, tomato.EndTime, iTaskID);
                }
                context.SaveChanges();
            }
        }
        public List<Tomato> ReadTomato(int iTaskID)    //提供id为TaskID的所有番茄钟信息（起止时间）以查找时段内的应用程序信息
        {
            List<Tomato> TomatoList = new List<Tomato>();
            using (var context = Context)
            {
                var query = context.TaskTomatoes.Where(tt => tt.TaskID == iTaskID).OrderBy(tt => tt.TomatoID);
                foreach (var tt in query)
                {
                    Tomato tomato = new Tomato
                    {
                        TomatoID = tt.TomatoID,
                        StartTime = tt.BeginTime,
                        EndTime = tt.EndTime
                    };
                    TomatoList.Add(tomato);
                }
                return TomatoList;
            }
        }
        public List<string> RecentTenApp(DateTime iTime)
        {
            List<string> AppList = new List<string>();

            Dictionary<string, DateTime> RecentOpen = new Dictionary<string, DateTime>();

            // Get Dictionary<TaskName,EachToamtoEndTime> to order
            using (var context = Context)
            {
                var query = context.Tasks.Where(t => t.State == 1).OrderBy(t => t.TaskID);
                foreach (var t in query)
                {
                    var tquery = context.TaskTomatoes.Where(tt => tt.TaskID == t.TaskID).OrderBy(tt => tt.TomatoID);
                    foreach (var tt in tquery)
                    {
                        if (DateTime.Compare(iTime, tt.EndTime) > 0)
                        {
                            if (RecentOpen.ContainsKey(t.Name) && DateTime.Compare(RecentOpen[t.Name], tt.EndTime) > 0)
                                RecentOpen[t.Name] = tt.EndTime;
                            else
                                RecentOpen.Add(t.Name, tt.EndTime);
                        }
                    }
                }
            }

            Dictionary<string,DateTime> RecentOpenDesc = RecentOpen.OrderByDescending(p => p.Value).ToDictionary(p => p.Key, p => p.Value);
            foreach (var key in RecentOpenDesc.Keys)
            {
                if (AppList.Count <= 10)
                    AppList.Add(key);
            }

            return AppList;
        }
        #endregion

        #region 私有函数
        private int AddTomatoNum(DateTime iBeginTime, DateTime iEndTime, int iTomatoCount)
        {
            TimeSpan ts1 = new TimeSpan(iBeginTime.Ticks);
            TimeSpan ts2 = new TimeSpan(iEndTime.Ticks);
            TimeSpan ts = ts2.Subtract(ts1);
            int TimeSpanSecond = int.Parse(ts.TotalSeconds.ToString());
            if (TimeSpanSecond >= 25 * 60) { return iTomatoCount++; }
            else { return iTomatoCount; }
        }
        private void GetFilePath(DateTime iBeginTime, DateTime iEndTime, int iTaskID)
        {
            string appdata = Environment.GetEnvironmentVariable("AppData");
            string RecentFilePath = $"{appdata}\\Microsoft\\Windows\\Rencent";
            DirectoryInfo directoryInfo = new DirectoryInfo(RecentFilePath);
            List<FileInfo> fileInfos = new List<FileInfo>(directoryInfo.GetFiles());
            //List<string> FilePathList = new List<string>();
            foreach (var fileInfo in fileInfos)
            {
                if (DateTime.Compare(fileInfo.CreationTime,iBeginTime)>0 && DateTime.Compare(fileInfo.CreationTime,iEndTime)<0)
                {
                    if (!fileInfo.Exists)
                    {
                        WshShell shell = new WshShell();
                        IWshShortcut lnkPath = (IWshShortcut)shell.CreateShortcut(fileInfo.FullName);
                        string FileRealPath = lnkPath.TargetPath;
                        using (var context = Context)
                        {
                            var file = new TaskFileList() { FilePath = FileRealPath, TaskID = iTaskID };
                            context.TaskFileLists.Add(file);
                            context.SaveChanges();
                        }

                    }
                }
            }
            
        }
        #endregion

        //private readonly TaskTomatoContext context;
        /// <summary>
        /// 用于操作数据库的 DbContext
        /// </summary>
        private TaskTomatoContext Context
        {
            get
            {
                var builder = new DbContextOptionsBuilder<TaskTomatoContext>();
                builder.UseSqlite($"Data Source={TaskTomatoConfig.DbPath}");
                TaskTomatoContext context = new TaskTomatoContext(builder.Options);
                context.Database.EnsureCreated();
                return context;
            }
        }

        class TaskTomatoConfig
        {
            public static string DbPath { get; } = "TaskTomatoDB.db";
        }
    }
}
