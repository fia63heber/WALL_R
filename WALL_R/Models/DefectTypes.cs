using System;
using System.Collections.Generic;

namespace WALL_R.Models
{
    public partial class DefectTypes
    {
        public DefectTypes()
        {
            Defects = new HashSet<Defects>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Defects> Defects { get; set; }
    }
}
