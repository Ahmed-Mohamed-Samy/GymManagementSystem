using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.PlanViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagementPL.Controllers
{
    [Authorize]
    public class PlanController : Controller
    {
        private readonly IPlanService _planService;

        public PlanController(IPlanService planService)
        {
            _planService = planService;
        }


        public async Task<ActionResult> Index()
        {
            
            var Plans = await _planService.GetAllPlansAsync();
            
            return View(Plans);
        }


        public async Task<ActionResult> Details(int id)
        {
            if(id <= 0)
            {
                TempData["ErrorMessage"] = "id must be valid";
                return RedirectToAction(nameof(Index));
            }

            var plan = await _planService.GetPlanDetailsAsync(id);

            if(plan is null)
            {
                TempData["ErrorMessage"] = "plan not found";
                return RedirectToAction(nameof(Index));
            }

            return View(plan);

        }

        public async Task<ActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "id must be valid";
                return RedirectToAction(nameof(Index));
            }

            var plan = await _planService.GetPlanToUpdateAsync(id);

            if(plan is null)
            {
                TempData["ErrorMessage"] = "plan not found";
                return RedirectToAction(nameof(Index));
            }

            return View(plan);

        }


        [HttpPost]
        public async Task<ActionResult> Edit([FromRoute]int id,UpdatePlanViewModel updatePlan)
        {
            if (!ModelState.IsValid)
                return View(updatePlan);

            var Result = await _planService.UpdatePlanAsync(id, updatePlan);

            if(Result)
            {
                TempData["SuccessMessage"] = "Plan Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Plan Failed to Update";
                return RedirectToAction(nameof(Index));
            }
        }
        [HttpPost]
        public async Task<ActionResult> Activate(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "id must be valid";
                return RedirectToAction(nameof(Index));
            }

            var Result = await _planService.ToggleStatusAsync(id);


            if(Result)
                TempData["SuccessMessage"] = "Plan Updated Successfully";
            else
                TempData["ErrorMessage"] = "Plan Failed to Update";


            return RedirectToAction(nameof(Index));
        }
    }
}
