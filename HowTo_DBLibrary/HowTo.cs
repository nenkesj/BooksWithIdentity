using System;
using System.Collections.Generic;

namespace HowTo_DBLibrary
{
    public partial class HowTo
    {
        public int HowToId { get; set; }
        public int NodeId { get; set; }
        public string Topic { get; set; } = null!;
        public string Client { get; set; } = null!;

        public virtual Node Node { get; set; } = null!;
    }
}
