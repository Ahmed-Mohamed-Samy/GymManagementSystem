using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.MemberShipViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GymManagementPL.Controllers
{
    public class MemberShipController : Controller
    {
        private readonly IMemberShipService _memberShipService;

        public MemberShipController(IMemberShipService memberShipService)
        {
            _memberShipService = memberShipService;
        }

        public async Task<ActionResult> Index()
        {
            var MemberShips = await _memberShipService.GetAllMemberShipsAsync();
            return View(MemberShips);
        }

        public async Task<ActionResult> Create()
        {

            await LoadMembersDropDown();
            await LoadPlansDropDown();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateMembershipViewModel createMembership)
        {
            if(!ModelState.IsValid)
            {
               await LoadMembersDropDown();
               await LoadPlansDropDown();
                TempData["ErrorMessage"] = "Membership Can Not be Created Check Your Data";
                return View(createMembership);
            }

            var Result = await _memberShipService.CreateMembershipAsync(createMembership);

            if(Result)
            {
                TempData["SuccessMessage"] = "Membership Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Membership Created Failed";
                await LoadMembersDropDown();
                await LoadPlansDropDown();
                return View(createMembership);
            }
        }
        [HttpPost]
        public async Task<ActionResult> Cancel(int id)
        {
            var Result = await _memberShipService.DeleteMemberShipAsync(id);

            if(Result)
                TempData["SuccessMessage"] = "Membership Cancelled Successfully";
           
            else
                TempData["ErrorMessage"] = "Membership Cancelled Failed";

            return RedirectToAction(nameof(Index));
            
        }

        #region Helper Methods

        private async Task LoadMembersDropDown()
        {
            var Members = await _memberShipService.GetAllMembersForDropdownAsync();
            ViewBag.Members = new SelectList(Members, "Id", "Name");
        }
        private async Task LoadPlansDropDown()
        {
            var Plans = await _memberShipService.GetAllActivePlansForDropdownAsync();
            ViewBag.Plans = new SelectList(Plans, "Id", "Name");
        }

        #endregion
    }
}
