using System;
using System.Collections.Generic;
using System.Text;

namespace Models.DatabaseModels.DatabaseEntities.EntityBase
{
    public interface IBaseForUpdate
    {
        Nullable<DateTime> UpdateDate { get; set; }
    }
}
