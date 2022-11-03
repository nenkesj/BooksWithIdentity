using System;
using System.Collections.Generic;

namespace HowTo_DBLibrary
{
    public partial class Key
    {
        public int KeyId { get; set; }
        public short TreeId { get; set; }
        public int NodeId { get; set; }
        public short TypeId { get; set; }
        public string KeyText { get; set; } = null!;

        public virtual Node Node { get; set; } = null!;
        public virtual Tree Tree { get; set; } = null!;
        public virtual Type Type { get; set; } = null!;
    }
}
