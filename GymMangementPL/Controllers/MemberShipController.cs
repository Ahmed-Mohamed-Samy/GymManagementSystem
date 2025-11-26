using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.MemberShipViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementPL.Controllers
{
    public class MemberShipController : Controller
    {
        private readonly IMemberShipService _memberShipService;

        public MemberShipController(IMemberShipService memberShipService)
        {
            _memberShipService = memberShipService;
        }

        public ActionResult Index()
        {
            var MemberShips = _memberShipService.GetAllMemberShips();
            return View(MemberShips);
        }

        public ActionResult Create()
        {

            LoadMembersDropDown();
            LoadPlansDropDown();
            return View();
        }

        [HttpPost]
        public ActionResult Create(CreateMembershipViewModel createMembership)
        {
            if(!ModelState.IsValid)
            {
                LoadMembersDropDown();
                LoadPlansDropDown();
                TempData["ErrorMessage"] = "Membership Can Not be Created Check Your Data";
                return View(createMembership);
            }

            var Result = _memberShipService.CreateMembership(createMembership);

            if(Result)
            {
                TempData["SuccessMessage"] = "Membership Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Membership Created Failed";
                LoadMembersDropDown();
                LoadPlansDropDown();
                return View(createMembership);
            }
        }
        [HttpPost]
        public ActionResult Cancel(int id)
        {
            var Result = _memberShipService.DeleteMemberShip(id);

            if(Result)
                TempData["SuccessMessage"] = "Membership Cancelled Successfully";
           
            else
                TempData["ErrorMessage"] = "Membership Cancelled Failed";

            return RedirectToAction(nameof(Index));
            
        }

        #region Helper Methods

        private void LoadMembersDropDown()
        {
            var Members = _memberShipService.GetAllMembersForDropdown();
            ViewBag.Members = new SelectList(Members, "Id", "Name");
        }
        private void LoadPlansDropDown()
        {
            var Plans = _memberShipService.GetAllActivePlansForDropdown();
            ViewBag.Plans = new SelectList(Plans, "Id", "Name");
        }

        #endregion
    }
}
