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


        public IEnumerable<SessionViewModel> GetAllSessionsWithTrainerAndCategory()
        {
            var sessionRepo = _unitOfWork.SessionRepository;
            var sessions = sessionRepo.GetAllSessionsWithTrainersAndCategories();

            if (sessions is null) return [];

            var MappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(sessions);
            foreach (var session in MappedSessions)
                session.AvailableSlots = session.Capacity - sessionRepo.GetCountOfBookedSlots(session.Id);

            return MappedSessions;
        }
        public IEnumerable<MemberForSessionViewModel> GetAllMembersForSession(int id)
        {
            var BookingRepo = _unitOfWork.BookingRepository;
            var MembersForSession = BookingRepo.GetSessionById(id);
            if (MembersForSession is null) return [];


            return _mapper.Map<IEnumerable<MemberForSessionViewModel>>(MembersForSession);
        }

        public bool CreateBooking(CreateBookingViewModel createBooking)
        {
            try
            {
                var SessionRepo = _unitOfWork.SessionRepository;
                var Session = SessionRepo.GetById(createBooking.SessionId);

                if (Session is null || Session.StartDate <= DateTime.UtcNow) return false;

                var ActiveMembershipForMember = _unitOfWork.MemberShipRepository.GetFirstMemberShip(m => m.Status.ToLower() == "active" && m.MemberId == createBooking.MemberId);

                if (ActiveMembershipForMember is null) return false;

                var BookedSlots = SessionRepo.GetCountOfBookedSlots(createBooking.SessionId);

                var AvailableSlots = Session.Capacity - BookedSlots;

                if (AvailableSlots == 0) return false;

                var Booking = _mapper.Map<MemberSession>(createBooking);


                _unitOfWork.BookingRepository.Add(Booking);

                return _unitOfWork.SaveChanges() > 0;
            }catch (Exception ex)
            {
                return false;
            }

        }
        public bool CancelBooking(MemberAttendOrCancelViewModel model)
        {
            try
            {
                var Session = _unitOfWork.SessionRepository.GetById(model.SessionId);
                if (Session is null || Session.StartDate <= DateTime.UtcNow || Session.EndDate < DateTime.UtcNow) return false;

                var Booking = _unitOfWork.BookingRepository.GetAll(B => B.SessionId == model.SessionId && B.MemberId == model.MemberId).FirstOrDefault();
                if (Booking is null) return false;

                _unitOfWork.BookingRepository.Delete(Booking);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public bool MemberAttend(MemberAttendOrCancelViewModel model)
        {
            try
            {
                var Session = _unitOfWork.SessionRepository.GetById(model.SessionId);
                if (Session is null) return false;

                var Booking = _unitOfWork.BookingRepository.GetAll(B => B.SessionId == model.SessionId && B.MemberId == model.MemberId).FirstOrDefault();
                if (Booking is null) return false;

                Booking.IsAttended = true;
                Booking.UpdatedAt = DateTime.UtcNow;

                _unitOfWork.BookingRepository.Update(Booking);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #region Helper Methods

        public IEnumerable<MemberSelectViewModel> GetMembersForDropDown(int id)
        {
            var BookingRepo = _unitOfWork.BookingRepository;
            var bookMemberIds = BookingRepo.GetAll(S => S.Id == id).Select(ms => ms.MemberId).ToList();

            var MembersAvailableToBook = _unitOfWork.GetRepository<Member>().GetAll(M => !bookMemberIds.Contains(M.Id));

            return _mapper.Map<IEnumerable<MemberSelectViewModel>>(MembersAvailableToBook);

        }


        #endregion
    }
}
