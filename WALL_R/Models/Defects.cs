using System;
using System.Collections.Generic;

namespace WALL_R.Models
{
    public partial class Defects
    {
        public int Id { get; set; }
        public int? ComponentId { get; set; }
        public int? DefectTypeId { get; set; }
        public int? StateId { get; set; }
        public int? PriorityId { get; set; }
        public int? WriterId { get; set; }
        public string Name { get; set; }
        public string EntryComment { get; set; }
        public string OwnerComment { get; set; }
    }
}
