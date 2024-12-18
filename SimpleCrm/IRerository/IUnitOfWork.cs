using SimpleCrm.Abstraction;

namespace SimpleCrm.IRepository
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        IGenericRepo<TEntity> Repository<TEntity>() where TEntity : class;
        Task<int> Complete();
        public void Rollback();
    }
}
