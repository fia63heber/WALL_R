using System;
using System.Collections.Generic;

namespace WALL_R.Models
{
    public partial class Rooms
    {
        public Rooms()
        {
            Devices = new HashSet<Devices>();
        }

        public int Id { get; set; }
        public int? OwnerId { get; set; }
        public int? PictureFileId { get; set; }
        public string RoomNumber { get; set; }

        public Accounts Owner { get; set; }
        public Files PictureFile { get; set; }
        public ICollection<Devices> Devices { get; set; }
    }
}
