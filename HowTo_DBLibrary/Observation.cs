using System;
using System.Collections.Generic;

namespace HowTo_DBLibrary
{
    public partial class Observation
    {
        public int ObservationId { get; set; }
        public int ProblemId { get; set; }
        public string Observation1 { get; set; } = null!;
        public string Comment { get; set; } = null!;

        public virtual Problem Problem { get; set; } = null!;
    }
}
