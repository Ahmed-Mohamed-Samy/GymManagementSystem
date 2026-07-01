using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Repositories.Classes
{
    public class GenericRepostitory<TEntity> : IGenericRepostitory<TEntity> where TEntity : BaseEntity, new()
    {
        private readonly GymDbContext _dbContext;

        public GenericRepostitory(GymDbContext dbContext)
        {
            _dbContext = dbContext;
        }



        public async Task AddAsync(TEntity entity) => await _dbContext.Set<TEntity>().AddAsync(entity);
       
            
        

        public void Delete(TEntity entity) => _dbContext.Set<TEntity>().Remove(entity);
           
        

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? condition = null)
        {
            if (condition is null)
                return await _dbContext.Set<TEntity>().AsNoTracking().ToListAsync();

            return await _dbContext.Set<TEntity>().AsNoTracking().Where(condition).ToListAsync();
        }



        public async Task<TEntity?> GetByIdAsync(int id) => await _dbContext.Set<TEntity>().FindAsync(id);



        public void Update(TEntity entity) => _dbContext.Set<TEntity>().Update(entity);

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? condition = null) => await _dbContext.Set<TEntity>().CountAsync(condition ?? (_ => true));

        public async Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>>? condition = null)
        => await _dbContext.Set<TEntity>().FirstOrDefaultAsync(condition ?? (_ => true));

    }
}
