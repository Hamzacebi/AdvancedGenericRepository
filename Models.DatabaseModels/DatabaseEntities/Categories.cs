using System;
using System.Collections.Generic;

namespace Models.DatabaseModels.DatabaseEntities
{
    using EntityBase;

    public partial class Categories : IEntityBase, IBaseForCreation, IBaseForUpdate
    {
        public Categories()
        {
            Products = new HashSet<Products>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; }
        public DateTime CreationDate { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }

        public virtual ICollection<Products> Products { get; set; }
    }
}
