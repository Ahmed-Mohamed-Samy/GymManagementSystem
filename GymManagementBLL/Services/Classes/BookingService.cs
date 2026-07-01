using AutoMapper;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.BookingViewModels;
using GymManagementBLL.ViewModels.MemberShipViewModels;
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
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsWithTrainerAndCategoryAsync()
        {
            var sessionRepo = _unitOfWork.SessionRepository;
            var sessions = await sessionRepo.GetAllSessionsWithTrainersAndCategoriesAsync();

            if (sessions is null) return [];

            var MappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);
            foreach (var session in MappedSessions)
                session.AvailableSlots = session.Capacity - await sessionRepo.GetCountOfBookedSlotsAsync(session.Id);

            return MappedSessions;
        }
        public async Task<IEnumerable<MemberForSessionViewModel>> GetAllMembersForSessionAsync(int id)
        {
            var BookingRepo = _unitOfWork.BookingRepository;
            var MembersForSession = await BookingRepo.GetSessionByIdAsync(id);
            if (MembersForSession is null) return [];


            return _mapper.Map<IEnumerable<MemberForSessionViewModel>>(MembersForSession);
        }

        public async Task<bool> CreateBookingAsync(CreateBookingViewModel createBooking)
        {
            try
            {
                var SessionRepo = _unitOfWork.SessionRepository;
                var Session = await SessionRepo.GetByIdAsync(createBooking.SessionId);

                if (Session is null || Session.StartDate <= DateTime.UtcNow) return false;

                var ActiveMembershipForMember = await _unitOfWork.MemberShipRepository.GetFirstMemberShipAsync(m => m.EndDate > DateTime.UtcNow && m.MemberId == createBooking.MemberId);

                if (ActiveMembershipForMember is null) return false;

                var BookedSlots = await SessionRepo.GetCountOfBookedSlotsAsync(createBooking.SessionId);

                var AvailableSlots = Session.Capacity - BookedSlots;

                if (AvailableSlots == 0) return false;

                var Booking = _mapper.Map<MemberSession>(createBooking);


                await _unitOfWork.BookingRepository.AddAsync(Booking);

                return await _unitOfWork.SaveChangesAsync() > 0;
            }catch (Exception ex)
            {
                return false;
            }

        }
        public async Task<bool> CancelBookingAsync(MemberAttendOrCancelViewModel model)
        {
            try
            {
                var Session = await _unitOfWork.SessionRepository.GetByIdAsync(model.SessionId);
                if (Session is null || Session.StartDate <= DateTime.UtcNow || Session.EndDate < DateTime.UtcNow) return false;

                var Booking = await _unitOfWork.BookingRepository.GetFirstOrDefaultAsync(B => B.SessionId == model.SessionId && B.MemberId == model.MemberId);
                if (Booking is null) return false;

                _unitOfWork.BookingRepository.Delete(Booking);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<bool> MemberAttendAsync(MemberAttendOrCancelViewModel model)
        {
            try
            {
                var Session = await _unitOfWork.SessionRepository.GetByIdAsync(model.SessionId);
                if (Session is null) return false;

                var Booking = await _unitOfWork.BookingRepository.GetFirstOrDefaultAsync(B => B.SessionId == model.SessionId && B.MemberId == model.MemberId);
                if (Booking is null) return false;

                Booking.IsAttended = true;
                Booking.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.BookingRepository.Update(Booking);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #region Helper Methods

        public async Task<IEnumerable<MemberSelectViewModel>> GetMembersForDropDownAsync(int id)
        {
            var BookingRepo = _unitOfWork.BookingRepository;
            var bookMemberIds = await BookingRepo.GetMembersIdsAsync(S => S.SessionId == id);

            var MembersAvailableToBook = await _unitOfWork.GetRepository<Member>().GetAllAsync(M => !bookMemberIds.Contains(M.Id));

            return _mapper.Map<IEnumerable<MemberSelectViewModel>>(MembersAvailableToBook);

        }


        #endregion
    }
}
