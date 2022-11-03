using System;
using System.Collections.Generic;

namespace HowTo_DBLibrary
{
    public partial class Code
    {
        public int CodeId { get; set; }
        public int NodeId { get; set; }
        public short TypeId { get; set; }
        public string Code1 { get; set; } = null!;

        public virtual Node Node { get; set; } = null!;
    }
}
