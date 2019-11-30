using System;
using System.Collections.Generic;

namespace Models.DatabaseModels.DatabaseEntities
{
    using EntityBase;

    public partial class Users : IEntityBase, IBaseForCreation
    {
        public Users()
        {
            Orders = new HashSet<Orders>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Status { get; set; }

        public DateTime CreationDate { get; set; }

        public virtual ICollection<Orders> Orders { get; set; }
    }
}
