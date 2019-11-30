using System;
using System.Collections.Generic;
using System.Text;

namespace Models.DatabaseModels.DatabaseEntities.EntityBase
{
    public interface IBaseForDelete
    {
        Nullable<DateTime> DeleteDate { get; set; }
    }
}
