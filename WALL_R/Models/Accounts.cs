using System;
using System.Collections.Generic;

namespace WALL_R.Models
{
    public partial class Accounts
    {
        public int Id { get; set; }
        public int? RightGroupId { get; set; }
        public string Ldapid { get; set; }
        public string Prename { get; set; }
        public string Surname { get; set; }
    }
}
