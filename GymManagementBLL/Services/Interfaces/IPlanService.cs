using GymManagementBLL.ViewModels.PlanViewModels;
using GymManagementDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagementBLL.Services.Interfaces
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync();

        Task<PlanViewModel?> GetPlanDetailsAsync(int id);

        Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planId);

        Task<bool> UpdatePlanAsync(int planId, UpdatePlanViewModel updatePlan);

        Task<bool> ToggleStatusAsync(int planId);

    }
}
