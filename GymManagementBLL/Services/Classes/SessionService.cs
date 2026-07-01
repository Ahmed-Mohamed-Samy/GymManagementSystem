using AutoMapper;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.SessionViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SessionService(IUnitOfWork unitOfWork ,IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> CreateSessionAsync(CreateSessionViewModel createSession)
        {
            try
            {
                if (createSession is null) return false;


                if (! await IsTainerExistsAsync(createSession.TrainerId) || ! await IsCategoryExistsAsync(createSession.CategoryId) || !IsValidDateRange(createSession.StartDate, createSession.EndDate)) return false;


                var session = _mapper.Map<Session>(createSession);

                await _unitOfWork.SessionRepository.AddAsync(session);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync()
        {
            var sessionRepo = _unitOfWork.SessionRepository;

            var sessions = await sessionRepo.GetAllSessionsWithTrainersAndCategoriesAsync();

            if (sessions is null || !sessions.Any()) return [];

            var MappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);

            foreach (var session in MappedSessions)
                session.AvailableSlots = session.Capacity - await sessionRepo.GetCountOfBookedSlotsAsync(session.Id);
            
            return MappedSessions;

        }

        public async Task<SessionViewModel?> GetSessionByIdAsync(int sessionId)
        {
            
            var sessionRepo = _unitOfWork.SessionRepository;
            
            var session = await sessionRepo.GetSessionWithTrainerAndCategoryAsync(sessionId);


            if(session is null) return null;

            var sessionMapped =  _mapper.Map<SessionViewModel>(session);

            sessionMapped.AvailableSlots = await sessionRepo.GetCountOfBookedSlotsAsync(sessionId);

            return sessionMapped;

        }


        public async Task<SessionToUpdateViewModel?> GetSessionToUpdateAsync(int sessionId)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId);
            if(session is null || ! await IsSessionAvailableForUpdatingAsync(session)) return null;

            return _mapper.Map<SessionToUpdateViewModel>(session);
        }

        public async Task<bool> UpdateSessionAsync(int sessionId, SessionToUpdateViewModel updateSession)
        {
            try
            {
                var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId);

                if (session is null) return false;

                if (! await IsTainerExistsAsync(updateSession.TrainerId) || !await IsSessionAvailableForUpdatingAsync(session) || !IsValidDateRange(session.StartDate,session.EndDate)) return false;


                _mapper.Map(updateSession,session);
                session.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.SessionRepository.Update(session);

                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveSessionAsync(int sessionId)
        {
            try
            {
                var Repo = _unitOfWork.GetRepository<Session>();

                var session = await Repo.GetByIdAsync(sessionId);

                if (session is null || ! await IsSessionAvailableForRemovingAsync(session)) return false;

                Repo.Delete(session);

                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch 
            { 
                return false; 
            }


        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetAllTrainersForDropDownAsync()
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync();

            if (trainers is null) return [];

            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(trainers);
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetAllCategoriesForDropDownAsync()
        {
            var Categries = await _unitOfWork.GetRepository<Category>().GetAllAsync();

            if (Categries is null) return [];

            return _mapper.Map<IEnumerable<CategorySelectViewModel>>(Categries);
        }

        #region Helper Methods


        private async Task<bool> IsTainerExistsAsync(int trainerId)
        {
            return await _unitOfWork.GetRepository<Trainer>().GetFirstOrDefaultAsync(T => T.Id == trainerId) is not null;
        }
        private async Task<bool> IsCategoryExistsAsync(int CategoryId)
        {
            return await _unitOfWork.GetRepository<Category>().GetFirstOrDefaultAsync(C => C.Id == CategoryId) is not null;
        }

        private bool IsValidDateRange(DateTime strartDate , DateTime endDate)
        {
            return strartDate < endDate && strartDate > DateTime.Now;
        }

        private async Task<bool> IsSessionAvailableForUpdatingAsync(Session session)
        {
            if(session is null) return false;

            if(session.EndDate < DateTime.UtcNow)
                return false;

            if(session.StartDate <= DateTime.UtcNow && session.EndDate > DateTime.UtcNow)
                return false;

            var HasActiveBooking = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id) > 0;

            if (HasActiveBooking) return false;


            return true;
        }
        private async Task<bool> IsSessionAvailableForRemovingAsync(Session session)
        {
            if(session is null) return false;

            if(session.StartDate > DateTime.UtcNow)
                return false;

            if(session.StartDate <= DateTime.UtcNow && session.EndDate > DateTime.UtcNow)
                return false;

            var HasActiveBooking = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(session.Id) > 0;

            if (HasActiveBooking) return false;


            return true;
        }




        #endregion


    }
}
