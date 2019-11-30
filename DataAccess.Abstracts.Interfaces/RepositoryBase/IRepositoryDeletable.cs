using Models.DatabaseModels.DatabaseEntities.EntityBase;

namespace DataAccess.Abstracts.Interfaces.RepositoryBase
{
    public interface IRepositoryDeletable<T> where T : class //, IBaseForUpdate
    {
        /// <summary>
        /// Tablolardaki herhangi bir kayiti gecici olarak silmeye (UPDATE) yarayan fonksiyon
        /// </summary>
        /// <param name="deleteEntity"></param>
        /// <returns></returns>
        T TemporaryDelete(T deleteEntity);

        //Todo : bir fonksiyonun ozellikle bir interface'den miras almasini nasil saglarim
        //Todo : summary ekle
        //bool PermanentDelete<T>(object id) where T : IBaseForDelete;
        bool PermanentDelete(object id);
    }
}
