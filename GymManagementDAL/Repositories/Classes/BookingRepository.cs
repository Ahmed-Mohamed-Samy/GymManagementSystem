using GymManagementDAL.Data.Contexts;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Repositories.Classes
{
    public class BookingRepository : GenericRepostitory<MemberSession> , IBookingRepository
    {
        private readonly GymDbContext _dbContext;

        public BookingRepository(GymDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }



        public async Task<IEnumerable<int>> GetMembersIdsAsync(Expression<Func<MemberSession, bool>>? condition = null)
        => await _dbContext.MemberSessions.Where(condition ?? (_ => true)).Select(ms => ms.MemberId).ToListAsync();

        public async Task<IEnumerable<MemberSession>> GetSessionByIdAsync(int sessionId)
        => await _dbContext.MemberSessions.Where(MS => MS.SessionId == sessionId)
                                    .Include(MS => MS.Member)
                                    .ToListAsync();

    }
}
