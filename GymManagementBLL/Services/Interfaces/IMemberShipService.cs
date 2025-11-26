using GymManagementBLL.ViewModels.MemberShipViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IMemberShipService 
    {
        IEnumerable<MemberShipViewModel> GetAllMemberShips();
        bool CreateMembership(CreateMembershipViewModel createMembership);
        IEnumerable<MemberSelectViewModel> GetAllMembersForDropdown();
        IEnumerable<PlanSelectViewModel> GetAllActivePlansForDropdown();
        bool DeleteMemberShip(int memberId);

    }
}
