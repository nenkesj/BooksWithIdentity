using System;
using System.Collections.Generic;

namespace HowTo_DBLibrary
{
    public partial class Summary
    {
        public int SummaryId { get; set; }
        public int NodeId { get; set; }
        public string Summary1 { get; set; } = null!;

        public virtual Node? Node { get; set; } = null!;
    }
}
