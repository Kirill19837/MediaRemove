using System;
using System.Collections.Generic;

namespace MediaRemove.Models.Nexu
{
    public class NexuRelationDisplayModel
    {
        public NexuRelationDisplayModel()
        {
            Properties = new List<NexuRelationPropertyDisplay>();
        }

        public int Id { get; set; }

        public Guid Key { get; set; }

        public string Name { get; set; }

        public string Culture { get; set; }

        public bool IsPublished { get; set; }

        public bool IsTrashed { get; set; }

        public IList<NexuRelationPropertyDisplay> Properties { get; set; }
    }
}
