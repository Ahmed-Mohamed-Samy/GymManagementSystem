using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Repositories.Classes
{
    public class SessionRepository : GenericRepostitory<Session>, ISessionRepository
    {
        private readonly GymDbContext _dbContext;

        public SessionRepository(GymDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }



        public async Task<IEnumerable<Session>> GetAllSessionsWithTrainersAndCategoriesAsync() => await _dbContext.Sessions.Include(S => S.SessionTrainer)
                                                                                                    .Include(S => S.SessionCategory)
                                                                                                    .AsNoTracking().ToListAsync();

        public async Task<int> GetCountOfBookedSlotsAsync(int sessionId) => await _dbContext.MemberSessions.CountAsync(S => S.SessionId == sessionId);

        public async Task<Session?> GetSessionWithTrainerAndCategoryAsync(int sessionId)
        {
            return await _dbContext.Sessions.Include(S => S.SessionTrainer).Include(S => S.SessionCategory).FirstOrDefaultAsync(S => S.Id == sessionId);
        }
    }
}
