using GymManagementDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Repositories.Interfaces
{
    public interface IBookingRepository : IGenericRepostitory<MemberSession>
    {
        Task<IEnumerable<MemberSession>> GetSessionByIdAsync(int sessionId);
        Task<IEnumerable<int>> GetMembersIdsAsync(Expression<Func<MemberSession, bool>>? condition = null);

    }
}
