using AutoMapper;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.PlanViewModels;
using GymManagementDAL.Entities;
using GymManagementDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PlanService(IUnitOfWork unitOfWork , IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync()
        {
            var Plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync();

            if (Plans == null || !Plans.Any()) return [];

            var PlansMapped = _mapper.Map<IEnumerable<PlanViewModel>>(Plans);

            return PlansMapped;


        }

        public async Task<PlanViewModel?> GetPlanDetailsAsync(int id)
        {
            var Plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(id);
            if (Plan == null) return null;

            var PlanMapped = _mapper.Map<PlanViewModel>(Plan);

            return PlanMapped;
        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planId)
        {
            
            
            
            
            var Plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(planId);

            if (Plan is null ||await HasActiveMemberShipsAsync(planId)) return null;

          
            var PlanMappedToUpdate = _mapper.Map<UpdatePlanViewModel>(Plan);

            return PlanMappedToUpdate;    

        }

        public async Task<bool> ToggleStatusAsync(int planId)
        {


            var planRepo = _unitOfWork.GetRepository<Plan>();
            var plan = await planRepo.GetByIdAsync(planId);

            if (plan is null || await HasActiveMemberShipsAsync(planId)) return false;

            
            plan.IsActive = !plan.IsActive;

            try
            {
                planRepo.Update(plan);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }




        }

        public async Task<bool> UpdatePlanAsync(int planId, UpdatePlanViewModel updatePlan)
        {

            try
            {
                var planRepo = _unitOfWork.GetRepository<Plan>();


                var Plan = await planRepo.GetByIdAsync(planId);


                if (Plan is null || await HasActiveMemberShipsAsync(planId)) return false;

                _mapper.Map(updatePlan,Plan);
             



                planRepo.Update(Plan);

                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch
            {
                return false;
            }

        }


        #region Helper Methods


        private async Task<bool> HasActiveMemberShipsAsync(int planId)
        {
            return await _unitOfWork.GetRepository<MemberShip>().GetFirstOrDefaultAsync(MS => MS.PlanId == planId && MS.EndDate > DateTime.UtcNow) is not null;
        } 

        #endregion
    }
}
