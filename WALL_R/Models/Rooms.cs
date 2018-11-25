using System;
using System.Collections.Generic;

namespace WALL_R.Models
{
    public partial class Rooms
    {
        public int Id { get; set; }
        public int? OwnerId { get; set; }
        public int? PictureFileId { get; set; }
        public string RoomNumber { get; set; }
    }
}
