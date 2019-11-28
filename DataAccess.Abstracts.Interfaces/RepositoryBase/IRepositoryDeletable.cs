namespace DataAccess.Abstracts.Interfaces.RepositoryBase
{
    public interface IRepositoryDeletable<T> where T : class
    {
        bool TemporaryDelete(object id);

        bool PermanentDelete(object id);
    }
}
