using System;
using System.Collections.Generic;

namespace HowTo_DBLibrary
{
    public partial class Task
    {
        public Task()
        {
            Attempts = new HashSet<Attempt>();
        }

        public int TaskId { get; set; }
        public int NodeId { get; set; }
        public string RequestSystem { get; set; } = null!;
        public int RequestNo { get; set; }
        public DateTime StartedOn { get; set; }
        public DateTime CompletedOn { get; set; }
        public string WhereAt { get; set; } = null!;
        public string Client { get; set; } = null!;
        public string Instructions { get; set; } = null!;

        public virtual Node Node { get; set; } = null!;
        public virtual ICollection<Attempt> Attempts { get; set; }
    }
}
