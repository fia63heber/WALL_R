using System;
using System.Collections.Generic;

namespace WALL_R.Models
{
    public partial class Rights
    {
        public Rights()
        {
            RightGroupsRights = new HashSet<RightGroupsRights>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<RightGroupsRights> RightGroupsRights { get; set; }
    }
}
