using System;
using System.Collections.Generic;

namespace HowTo_DBLibrary
{
    public partial class Type
    {
        public Type()
        {
            Infos = new HashSet<Info>();
            Keys = new HashSet<Key>();
            Nodes = new HashSet<Node>();
            Pictures = new HashSet<Picture>();
            Trees = new HashSet<Tree>();
        }

        public short TypeId { get; set; }
        public string Label { get; set; } = null!;
        public string Category { get; set; } = null!;

        public virtual ICollection<Info> Infos { get; set; }
        public virtual ICollection<Key> Keys { get; set; }
        public virtual ICollection<Node> Nodes { get; set; }
        public virtual ICollection<Picture> Pictures { get; set; }
        public virtual ICollection<Tree> Trees { get; set; }
    }
}
