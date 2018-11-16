using System;
using System.Collections.Generic;

namespace WALL_R.Models
{
    public partial class RightGroups
    {
        public RightGroups()
        {
            Accounts = new HashSet<Accounts>();
            RightGroupsRights = new HashSet<RightGroupsRights>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Accounts> Accounts { get; set; }
        public ICollection<RightGroupsRights> RightGroupsRights { get; set; }
    }
}
