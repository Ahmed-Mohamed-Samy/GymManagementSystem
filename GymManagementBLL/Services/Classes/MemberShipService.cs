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


        public async Task<IEnumerable<MemberShipViewModel>> GetAllMemberShipsAsync()
        {
            var MemberShips = await _unitOfWork.MemberShipRepository.GetMemberShipsWithMemberAndPlanAsync(MS => MS.EndDate > DateTime.UtcNow);

            if (MemberShips is null || !MemberShips.Any()) return [];

            var MappedMemberShips = _mapper.Map<IEnumerable<MemberShipViewModel>>(MemberShips);

            return MappedMemberShips;
        }
        public async Task<bool> CreateMembershipAsync(CreateMembershipViewModel createMembership)
        {
            if(! await IsMemberExistsAsync(createMembership.MemberId) ||! await IsPlanExistsAsync(createMembership.PlanId) || await HasActiveMembershipsAsync(createMembership.MemberId)) 
                return false;

            var membershipRepo = _unitOfWork.MemberShipRepository;
            var membershipToCreate = _mapper.Map<MemberShip>(createMembership);
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(createMembership.PlanId);
            membershipToCreate.EndDate = DateTime.UtcNow.AddDays(plan!.DurationDays);

            await membershipRepo.AddAsync(membershipToCreate);
            return await _unitOfWork.SaveChangesAsync() > 0;

        }

        public async Task<IEnumerable<MemberSelectViewModel>> GetAllMembersForDropdownAsync()
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync();
            if (members is null) return [];

            return _mapper.Map<IEnumerable<MemberSelectViewModel>>(members);
        }

        public async Task<IEnumerable<PlanSelectViewModel>> GetAllActivePlansForDropdownAsync()
        {
            var activePlans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(P => P.IsActive);
            if(activePlans is null) return [];

            return _mapper.Map<IEnumerable<PlanSelectViewModel>>(activePlans);

        }

        public async Task<bool> DeleteMemberShipAsync(int memberId)
        {
            var membershipRepo = _unitOfWork.MemberShipRepository;

            var membershipToDelete = await membershipRepo.GetFirstMemberShipAsync(MS => MS.MemberId == memberId && MS.EndDate > DateTime.UtcNow);

            if(membershipToDelete is null) return false;

            membershipRepo.Delete(membershipToDelete);

            return await _unitOfWork.SaveChangesAsync() > 0;

        }

        #region Helper Methods

        private async Task<bool> IsMemberExistsAsync(int memberId) 
        => await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId) is not null;
        private async Task<bool> IsPlanExistsAsync(int planId) 
        => await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId) is not null;
        private async Task<bool> HasActiveMembershipsAsync(int memberId)
        => await _unitOfWork.MemberShipRepository
           .GetFirstOrDefaultAsync(MS => MS.MemberId == memberId && MS.EndDate > DateTime.UtcNow) is not null;




        #endregion
    }
}
