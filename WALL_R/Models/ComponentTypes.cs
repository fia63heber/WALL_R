using System;
using System.Collections.Generic;

namespace WALL_R.Models
{
    public partial class ComponentTypes
    {
        public ComponentTypes()
        {
            Components = new HashSet<Components>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Components> Components { get; set; }
    }
}
