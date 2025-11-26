using GymManagementDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementDAL.Repositories.Interfaces
{
    public interface IMemberShipRepository : IGenericRepostitory<MemberShip>
    {
        IEnumerable<MemberShip> GetMemberShipsWithMemberAndPlan(Func<MemberShip,bool>? filter = null);

        MemberShip? GetFirstMemberShip(Func<MemberShip, bool>? filter = null);
    }
}
