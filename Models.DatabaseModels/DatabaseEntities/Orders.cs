using System;
using System.Collections.Generic;

namespace Models.DatabaseModels.DatabaseEntities
{
    public partial class Orders
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public DateTime? DeleteDate { get; set; }
        public bool? Status { get; set; }

        public virtual Products Product { get; set; }
        public virtual Users User { get; set; }
    }
}
