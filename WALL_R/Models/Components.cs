using System;
using System.Collections.Generic;

namespace WALL_R.Models
{
    public partial class Components
    {
        public Components()
        {
            Defects = new HashSet<Defects>();
        }

        public int Id { get; set; }
        public int? ComponentTypeId { get; set; }
        public string Name { get; set; }
        public int? DeviceId { get; set; }

        public ComponentTypes ComponentType { get; set; }
        public Devices Device { get; set; }
        public ICollection<Defects> Defects { get; set; }
    }
}
