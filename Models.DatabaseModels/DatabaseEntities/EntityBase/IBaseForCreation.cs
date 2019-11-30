using System;
using System.Collections.Generic;
using System.Text;

namespace Models.DatabaseModels.DatabaseEntities.EntityBase
{
    public interface IBaseForCreation
    {
        DateTime CreationDate { get; set; }
    }
}
