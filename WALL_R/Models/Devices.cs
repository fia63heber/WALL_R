using System;
using System.Collections.Generic;

namespace WALL_R.Models
{
    public partial class Devices
    {
        public int Id { get; set; }
        public int? RoomId { get; set; }
        public int? DeviceTypeId { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
    }
}
