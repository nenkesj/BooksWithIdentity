using System;
using System.Collections.Generic;

namespace HowTo_DBLibrary
{
    public partial class Problem
    {
        public Problem()
        {
            Observations = new HashSet<Observation>();
        }

        public int ProblemId { get; set; }
        public int NodeId { get; set; }
        public string ProblemSystem { get; set; } = null!;
        public int ProblemNo { get; set; }
        public string Title { get; set; } = null!;
        public DateTime Occurred { get; set; }
        public string Impacts { get; set; } = null!;
        public string Details { get; set; } = null!;
        public string Client { get; set; } = null!;
        public string Lpar { get; set; } = null!;

        public virtual Node Node { get; set; } = null!;
        public virtual ICollection<Observation> Observations { get; set; }
    }
}
