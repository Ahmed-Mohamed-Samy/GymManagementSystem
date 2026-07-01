using GymManagementBLL.ViewModels.MemberViewModel;
using GymManagementDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberViewModel>> GetAllMembersAsync();

        Task<bool> CreateMemberAsync(CreateMemberViewModel createMember);

        Task<MemberDetailsViewModel?> GetMemberDetailsAsync(int memberId);

        Task<HealthRecordViewModel?> GetMemberHealthRecordDetailsAsync(int memberId);

        Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int id);

        Task<bool> UpdateMemberAsync(int id, MemberToUpdateViewModel memberToUpdate);

        Task<bool> RemoveMemberAsync(int memberId);

    }
}
