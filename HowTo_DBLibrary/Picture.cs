using System;
using System.Collections.Generic;

namespace HowTo_DBLibrary
{
    public partial class Picture
    {
        public int PictureId { get; set; }
        public int NodeId { get; set; }
        public short TypeId { get; set; }
        public string Picture1 { get; set; } = null!;
        public string Title { get; set; } = null!;
        public int PictureSize { get; set; }
        public short DisplayAt { get; set; }
        public short DisplayStopAt { get; set; }
        public int InfoId { get; set; }

        public virtual Node Node { get; set; } = null!;
        public virtual Type Type { get; set; } = null!;
    }
}
