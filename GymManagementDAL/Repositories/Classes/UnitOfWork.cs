using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly GymDbContext _dbContext;

        public UnitOfWork(GymDbContext dbContext,ISessionRepository sessionRepository , IMemberShipRepository memberShipRepository) 
        {
            _dbContext = dbContext;
            SessionRepository = sessionRepository;
            MemberShipRepository = memberShipRepository;
        }



        private readonly Dictionary<Type,object> _repositories = new();

        public ISessionRepository SessionRepository { get; }
        public IMemberShipRepository MemberShipRepository { get; }

        public IGenericRepostitory<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            
            var EntityType = typeof(TEntity);
            if (_repositories.ContainsKey(EntityType))
                return (IGenericRepostitory<TEntity>)_repositories[EntityType];

            var newRepo = new GenericRepostitory<TEntity>(_dbContext);

            _repositories.Add(EntityType, newRepo);

            return newRepo;
            
        }

        public int SaveChanges()
        {
            return _dbContext.SaveChanges();
        }


    }
}
