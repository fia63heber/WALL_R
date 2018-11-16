using System;
using System.Collections.Generic;

namespace WALL_R.Models
{
    public partial class RightGroupsRights
    {
        public int Id { get; set; }
        public int? RightGroupId { get; set; }
        public int? RightId { get; set; }

        public Rights Right { get; set; }
        public RightGroups RightGroup { get; set; }
    }
}
