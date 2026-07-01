using GymManagementBLL.ViewModels.SessionViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Interfaces
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync();

        Task<SessionViewModel?> GetSessionByIdAsync(int sessionId);

        Task<bool> CreateSessionAsync(CreateSessionViewModel createSession);

        Task<SessionToUpdateViewModel?> GetSessionToUpdateAsync(int sessionId);

        Task<bool> UpdateSessionAsync(int sessionId, SessionToUpdateViewModel updateSession);

        Task<bool> RemoveSessionAsync(int sessionId);

        Task<IEnumerable<TrainerSelectViewModel>> GetAllTrainersForDropDownAsync();
        Task<IEnumerable<CategorySelectViewModel>> GetAllCategoriesForDropDownAsync();

    }
}
