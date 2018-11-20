using System;
using System.Collections.Generic;

namespace WALL_R.Models
{
    public partial class Sessions
    {
        public int Id { get; set; }
        public int? AccountId { get; set; }
        public DateTime? ExpiringDate { get; set; }
        public string Token { get; set; }
    }
}
