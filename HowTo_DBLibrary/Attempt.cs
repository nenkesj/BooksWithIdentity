using System;
using System.Collections.Generic;

namespace HowTo_DBLibrary
{
    public partial class Attempt
    {
        public int AttemptId { get; set; }
        public int TaskId { get; set; }
        public DateTime CompletedOn { get; set; }
        public bool? Succeeded { get; set; }
        public string Attempt1 { get; set; } = null!;
        public string Outcome { get; set; } = null!;

        public virtual Task Task { get; set; } = null!;
    }
}
