using System;
using System.Collections.Generic;

namespace WALL_R.Models
{
    public partial class Components
    {
        public int Id { get; set; }
        public int? ComponentTypeId { get; set; }
        public string Name { get; set; }
        public int? DeviceId { get; set; }
    }
}
