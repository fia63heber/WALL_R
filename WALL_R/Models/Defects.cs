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
        public string WriterComment { get; set; }

        public Components Component { get; set; }
        public DefectTypes DefectType { get; set; }
        public Priorities Priority { get; set; }
        public States State { get; set; }
        public Accounts Writer { get; set; }
    }
}
