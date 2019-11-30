using System;
using System.Collections.Generic;

namespace Models.DatabaseModels.DatabaseEntities
{
    public partial class Users
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

        public virtual ICollection<Orders> Orders { get; set; }
    }
}
