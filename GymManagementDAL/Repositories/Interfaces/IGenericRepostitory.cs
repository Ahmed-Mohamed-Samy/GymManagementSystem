using GymManagementDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Repositories.Interfaces
{
    public interface IGenericRepostitory<TEntity> where TEntity : BaseEntity , new()  
    {

        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? condition = null);
        Task<TEntity?> GetByIdAsync(int id);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? condition = null);
        Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>>? condition = null);



    }
}
