namespace DataAccess.Abstracts.Interfaces.RepositoryBase
{
    public interface IRepositoryInsertable<T> where T : class
    {
        T InsertItem(T insertItem);
    }
}
