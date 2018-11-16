using System;
using System.Collections.Generic;

namespace WALL_R.Models
{
    public partial class Accounts
    {
        public Accounts()
        {
            Defects = new HashSet<Defects>();
            Rooms = new HashSet<Rooms>();
        }

        public int Id { get; set; }
        public int? RightGroupId { get; set; }
        public string Ldapid { get; set; }
        public string Prename { get; set; }
        public string Surname { get; set; }

        public RightGroups RightGroup { get; set; }
        public ICollection<Defects> Defects { get; set; }
        public ICollection<Rooms> Rooms { get; set; }
    }
}
