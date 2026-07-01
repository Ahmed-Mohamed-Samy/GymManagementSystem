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
        Task<IEnumerable<MemberShipViewModel>> GetAllMemberShipsAsync();
        Task<bool> CreateMembershipAsync(CreateMembershipViewModel createMembership);
        Task<IEnumerable<MemberSelectViewModel>> GetAllMembersForDropdownAsync();
        Task<IEnumerable<PlanSelectViewModel>> GetAllActivePlansForDropdownAsync();
        Task<bool> DeleteMemberShipAsync(int memberId);

    }
}
