using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.AnalyticsViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Classes
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AnalyticsViewModel> GetAnalyticsDataAsync()
        {
            return new AnalyticsViewModel()
            {
                TotalMembers = await _unitOfWork.GetRepository<Member>().CountAsync(),
                ActiveMembers = await _unitOfWork.GetRepository<MemberShip>().CountAsync(X => X.EndDate > DateTime.UtcNow),
                TotalTrainers = await _unitOfWork.GetRepository<Trainer>().CountAsync(),
                UpcomingSessions = await _unitOfWork.SessionRepository.CountAsync(X => X.StartDate > DateTime.UtcNow),
                OngoingSessions = await _unitOfWork.SessionRepository.CountAsync(X => X.StartDate <= DateTime.UtcNow && X.EndDate > DateTime.UtcNow),
                CompletedSessions = await _unitOfWork.SessionRepository.CountAsync(X => X.EndDate < DateTime.UtcNow)
            };
        }
    }
}
