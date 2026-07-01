using GymManagementDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Repositories.Interfaces
{
    public interface IMemberShipRepository : IGenericRepostitory<MemberShip>
    {
        Task<IEnumerable<MemberShip>> GetMemberShipsWithMemberAndPlanAsync(Expression<Func<MemberShip, bool>>? filter = null);

        Task<MemberShip?> GetFirstMemberShipAsync(Expression<Func<MemberShip, bool>>? filter = null);
    }
}
