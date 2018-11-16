using System;
using System.Collections.Generic;

namespace WALL_R.Models
{
    public partial class Files
    {
        public Files()
        {
            Rooms = new HashSet<Rooms>();
        }

        public int Id { get; set; }
        public string FilePath { get; set; }

        public ICollection<Rooms> Rooms { get; set; }
    }
}
