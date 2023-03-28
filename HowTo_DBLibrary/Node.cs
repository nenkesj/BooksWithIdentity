using System;
using System.Collections.Generic;

namespace HowTo_DBLibrary
{
    public partial class Node
    {
        public Node()
        {
            Codes = new HashSet<Code>();
            HowTos = new HashSet<HowTo>();
            Infos = new HashSet<Info>();
            Keys = new HashSet<Key>();
            Pictures = new HashSet<Picture>();
            Problems = new HashSet<Problem>();
            Summaries = new HashSet<Summary>();
            Tasks = new HashSet<Task>();
        }

        public int NodeId { get; set; }
        public short TreeId { get; set; }
        public short TypeId { get; set; }
        public int ParentNodeId { get; set; }
        public short TreeLevel { get; set; }
        public string Heading { get; set; } = null!;
        public string NodeText { get; set; } = null!;

        public virtual Tree? Tree { get; set; } = null!;
        public virtual Type? Type { get; set; } = null!;
        public virtual ICollection<Code> Codes { get; set; }
        public virtual ICollection<HowTo> HowTos { get; set; }
        public virtual ICollection<Info> Infos { get; set; }
        public virtual ICollection<Key> Keys { get; set; }
        public virtual ICollection<Picture> Pictures { get; set; }
        public virtual ICollection<Problem> Problems { get; set; }
        public virtual ICollection<Summary> Summaries { get; set; }
        public virtual ICollection<Task> Tasks { get; set; }
    }
}
