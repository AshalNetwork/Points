using SimpleCrm.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using SimpleCrm.Repositories;
using SimpleCrm.Contexts;


namespace SimpleCrm
{
    public class UnitOfWork(PointsDbContext context) : IUnitOfWork
    {
        private readonly PointsDbContext _context = context;
        private Hashtable _repositories;


        private Dictionary<object, DbSet<object>> addedEntities = new Dictionary<object, DbSet<object>>();
        private Dictionary<object, DbSet<object>> modifiedEntities = new Dictionary<object, DbSet<object>>();
        private List<object> deletedEntities = new List<object>();

        public IGenericRepo<TEntity> Repository<TEntity>() where TEntity : class
        {
            
            if (_repositories == null)
                _repositories = new Hashtable();

    
            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repository = new GenericRepo<TEntity>(_context);
                _repositories.Add(type, repository);
            }
    
            return _repositories[type] as IGenericRepo<TEntity>;
        }

        public async Task<int> Complete()
        {
            try
            {

                foreach (var entity in deletedEntities)
                {
                    _context.Entry(entity).State = EntityState.Deleted;
                }
                var result = await _context.SaveChangesAsync();

                ClearTracking();

                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public void Rollback()
        {
            ClearTracking();
        }

        public async ValueTask DisposeAsync()
          => await _context.DisposeAsync();

        public void RegisterRemoved<TEntity>(TEntity entity) where TEntity : class
        {
            var set = _context.Set<TEntity>();
            if (addedEntities.ContainsKey(entity))
            {
                addedEntities.Remove(entity);
                return;
            }

            if (modifiedEntities.ContainsKey(entity))
            {
                modifiedEntities.Remove(entity);
                return;
            }

            deletedEntities.Add(entity);
        }

        private void ClearTracking()
        {
            addedEntities.Clear();
            modifiedEntities.Clear();
            deletedEntities.Clear();
        }
    }
}
