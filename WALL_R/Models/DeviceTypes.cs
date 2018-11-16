﻿using System;
using System.Collections.Generic;

namespace WALL_R.Models
{
    public partial class DeviceTypes
    {
        public DeviceTypes()
        {
            Devices = new HashSet<Devices>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Devices> Devices { get; set; }
    }
}
