﻿using System;
using System.Collections.Generic;

namespace Models.DatabaseModels.DatabaseEntities
{
    using EntityBase;

    public partial class Products : IEntityBase, IBaseForCreation, IBaseForUpdate
    {
        public Products()
        {
            Orders = new HashSet<Orders>();
        }

        public Guid Id { get; set; }
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public bool Status { get; set; }

        public DateTime CreationDate { get; set; }
        public Nullable<DateTime> UpdateDate { get; set; }

        public virtual Categories Category { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
