using GymManagementBLL.ViewModels.BookingViewModels;
using GymManagementBLL.ViewModels.MemberShipViewModels;
using GymManagementBLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IBookingService
    {
        Task<IEnumerable<SessionViewModel>> GetAllSessionsWithTrainerAndCategoryAsync();
        Task<IEnumerable<MemberForSessionViewModel>> GetAllMembersForSessionAsync(int id);
        Task<bool> CreateBookingAsync(CreateBookingViewModel createBooking);
        Task<IEnumerable<MemberSelectViewModel>> GetMembersForDropDownAsync(int id);
        Task<bool> CancelBookingAsync(MemberAttendOrCancelViewModel model);
        Task<bool> MemberAttendAsync(MemberAttendOrCancelViewModel model);
    }
}
