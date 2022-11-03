using System;
using System.Collections.Generic;

namespace HowTo_DBLibrary
{
    public partial class Tree
    {
        public Tree()
        {
            Infos = new HashSet<Info>();
            Keys = new HashSet<Key>();
            Nodes = new HashSet<Node>();
        }

        public short TreeId { get; set; }
        public string Heading { get; set; } = null!;
        public short TypeId { get; set; }

        public virtual Type Type { get; set; } = null!;
        public virtual ICollection<Info> Infos { get; set; }
        public virtual ICollection<Key> Keys { get; set; }
        public virtual ICollection<Node> Nodes { get; set; }
    }
}
