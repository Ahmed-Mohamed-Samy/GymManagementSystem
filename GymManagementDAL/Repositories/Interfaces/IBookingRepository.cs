using GymManagementDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Repositories.Interfaces
{
    public interface IBookingRepository : IGenericRepostitory<MemberSession>
    {
        IEnumerable<MemberSession> GetSessionById(int sessionId);
    }
}
