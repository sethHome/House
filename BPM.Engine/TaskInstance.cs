using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BPM.Engine
{
    public abstract class TaskInstance : ITask
    {
        public string ID { get; set; }

        public string SourceID { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Name { get; set; }

        public int UserID { get; set; }

        public bool IsDone { get; set; }

        public DateTime? ExecuteDate { get; set; }

        public DateTime? TurnDate { get; set; }

        public TaskStatus Status { get; set; }

        public TaskType Type { get; set; }

        private IToken _Token;

        public IToken Token
        {
            get {
                return this._Token;
            }
            set {
                this._Token = value;
                value.CurrentTask = this;
            }
        }

        public virtual async Task<string> Excute(ProcessInstance pi)
        {
            //Thread.Sleep(1000);
            //Console.WriteLine(Name + "is excute");

            //// 异步更新任务状态
            //BPMDBService.TaskDone(this.ID);

            return await Task<string>.Factory.StartNew(() =>
            {
                return pi.Sequences[this.To].To;
            });
            
        }
    }
}
