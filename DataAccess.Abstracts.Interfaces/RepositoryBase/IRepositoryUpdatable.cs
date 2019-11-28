namespace DataAccess.Abstracts.Interfaces.RepositoryBase
{
    public interface IRepositoryUpdatable<T> where T : class
    {
        T UpdateItem(T updateItem);
    }
}
