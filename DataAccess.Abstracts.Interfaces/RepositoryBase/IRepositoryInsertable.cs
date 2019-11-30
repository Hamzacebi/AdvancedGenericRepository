//using Models.DatabaseModels.DatabaseEntities.EntityBase;

namespace DataAccess.Abstracts.Interfaces.RepositoryBase
{
    public interface IRepositoryInsertable<T> where T : class //, IBaseForCreation
    {
        /// <summary>
        /// Herhangi bir tabloya kayit eklemek icin kullanilan fonksiyon
        /// </summary>
        /// <param name="insertItem">Eklenmek istenilen kayita ait bilgiler</param>
        /// <returns></returns>
        T InsertItem(T insertItem);
    }
}
