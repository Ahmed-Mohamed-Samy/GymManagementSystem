using AutoMapper;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.MemberShipViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Classes
{
    public class MemberShipService : IMemberShipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MemberShipService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public IEnumerable<MemberShipViewModel> GetAllMemberShips()
        {
            var MemberShips = _unitOfWork.MemberShipRepository.GetMemberShipsWithMemberAndPlan(MS => MS.Status.ToLower() == "active");

            if (MemberShips is null || !MemberShips.Any()) return [];

            var MappedMemberShips = _mapper.Map<IEnumerable<MemberShipViewModel>>(MemberShips);

            return MappedMemberShips;
        }
        public bool CreateMembership(CreateMembershipViewModel createMembership)
        {
            if(!IsMemberExists(createMembership.MemberId) || !IsPlanExists(createMembership.PlanId) || HasActiveMemberships(createMembership.MemberId)) 
                return false;

            var membershipRepo = _unitOfWork.MemberShipRepository;
            var membershipToCreate = _mapper.Map<MemberShip>(createMembership);
            var plan = _unitOfWork.GetRepository<Plan>().GetById(createMembership.PlanId);
            membershipToCreate.EndDate = DateTime.UtcNow.AddDays(plan!.DurationDays);

            membershipRepo.Add(membershipToCreate);
            return _unitOfWork.SaveChanges() > 0;

        }

        public IEnumerable<MemberSelectViewModel> GetAllMembersForDropdown()
        {
            var members = _unitOfWork.GetRepository<Member>().GetAll();
            if (members is null) return [];

            return _mapper.Map<IEnumerable<MemberSelectViewModel>>(members);
        }

        public IEnumerable<PlanSelectViewModel> GetAllActivePlansForDropdown()
        {
            var activePlans = _unitOfWork.GetRepository<Plan>().GetAll(P => P.IsActive);
            if(activePlans is null) return [];

            return _mapper.Map<IEnumerable<PlanSelectViewModel>>(activePlans);

        }


        #region Helper Methods

        private bool IsMemberExists(int memberId) 
        => _unitOfWork.GetRepository<Member>().GetById(memberId) is not null;
        private bool IsPlanExists(int planId) 
        => _unitOfWork.GetRepository<Plan>().GetById(planId) is not null;
        private bool HasActiveMemberships(int memberId)
        => _unitOfWork.MemberShipRepository
           .GetMemberShipsWithMemberAndPlan(MS => MS.MemberId == memberId && MS.Status.ToLower() == "active").Any();

        public bool DeleteMemberShip(int memberId)
        {
            var membershipRepo = _unitOfWork.MemberShipRepository;

            var membershipToDelete = membershipRepo.GetFirstMemberShip(MS => MS.MemberId == memberId && MS.Status.ToLower() == "active");

            if(membershipToDelete is null) return false;

            membershipRepo.Delete(membershipToDelete);

            return _unitOfWork.SaveChanges() > 0;

        }



        #endregion
    }
}
