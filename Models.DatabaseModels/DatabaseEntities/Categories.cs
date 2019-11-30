using System;
using System.Collections.Generic;

namespace Models.DatabaseModels.DatabaseEntities
{
    public partial class Categories
    {
        public Categories()
        {
            Products = new HashSet<Products>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<Products> Products { get; set; }
    }
}
