using System;

#region Custom Usings
using System.Linq;
using System.Linq.Expressions;
#endregion Custom Usings

namespace DataAccess.Abstracts.Interfaces.RepositoryBase
{
    public interface IRepositorySelectable<T> where T : class
    {
        /// <summary>
        /// Herhangi bir sart olmadan tum kayitlari listeler.
        /// </summary>
        /// <returns></returns>
        IQueryable<T> GetAllItems();

        /// <summary>
        /// Verilen Where Case degeri / degerlerine gore tum kayitlari listeler.
        /// </summary>
        /// <param name="predicate">Filtrele yapmak icin gerekli deger / degerler</param>
        /// <returns></returns>
        IQueryable<T> GetAllItems(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Verilen Id degerine gore tek bir kayidi listeler
        /// </summary>
        /// <param name="id">Listelenmek istenilen kayita ait Id bilgisi</param>
        /// <returns></returns>
        T GetItem(object id);

        /// <summary>
        /// Verilen Where Case degeri / degerlerine gore tek bir kayidi listeler
        /// </summary>
        /// <param name="predicate">Filtrele yapmak icin gerekli deger / degerler</param>
        /// <returns></returns>
        T GetItem(Expression<Func<T, bool>> predicate);
    }
}
