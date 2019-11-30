//using Models.DatabaseModels.DatabaseEntities.EntityBase;

namespace DataAccess.Abstracts.Interfaces.RepositoryBase
{
    public interface IRepositoryUpdatable<T> where T : class //, IBaseForUpdate
    {
        /// <summary>
        /// Herhangi bir tablodaki kayiti guncelleyen fonksiyon
        /// </summary>
        /// <param name="updateItem">Guncellenecek olan kayita ait "YENI" bilgiler</param>
        /// <returns></returns>
        T UpdateItem(T updateItem);
    }
}
