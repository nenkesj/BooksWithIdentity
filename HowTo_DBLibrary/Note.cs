using System;
using System.Collections.Generic;

namespace HowTo_DBLibrary
{
    public partial class Note
    {
        public int NoteId { get; set; }
        public int NodeId { get; set; }
        public string Details { get; set; } = null!;
        public string Text { get; set; } = null!;

        public virtual Node Node { get; set; } = null!;
    }
}
