using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.MemberViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading.Tasks;

namespace GymManagementPL.Controllers
{
    [Authorize (Roles ="SuperAdmin")]
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        #region Get All Members
        public async Task<ActionResult> Index()
        {

            var Members = await _memberService.GetAllMembersAsync();
            return View(Members);
        }
        #endregion


        #region Get Member Data 

        public async Task<ActionResult> MemberDetails(int id)
        {

            if (id <= 0)
            {

                TempData["ErrorMessage"] = "Id Of Member can Not be 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var Member = await _memberService.GetMemberDetailsAsync(id);
            
            if (Member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }


            
            return View(Member);
        }

        public async Task<ActionResult> HealthRecordDetails([FromRoute]int id)
        {
            if(id <= 0)
            {

                TempData["ErrorMessage"] = "Id Of Health Record can Not be 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var HealthRecord = await _memberService.GetMemberHealthRecordDetailsAsync(id);

            if (HealthRecord is null)
            {
                TempData["ErrorMessage"] = "Health Record Not Found";
                return RedirectToAction(nameof(Index));
            }

            return View(HealthRecord);
        }
        #endregion


        #region Add Member

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateMember(CreateMemberViewModel createMember)
        {
            if(!ModelState.IsValid)
            {
                ModelState.AddModelError("DataInvalid", "Check Data And Mising Fields");
                return View(nameof(Create),createMember);
            }

            bool Result = await _memberService.CreateMemberAsync(createMember);

            if(Result)
            {
                TempData["SuccessMessage"] = "Member Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Member Failed Create , check Phone and Email";
                return RedirectToAction(nameof(Index));
            }
        }


        #endregion

        #region Update Member


        public async Task<ActionResult> Edit(int id)
        {

            if (id <= 0)
            {

                TempData["ErrorMessage"] = "Id Of Member can Not be 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var Member = await _memberService.GetMemberToUpdateAsync(id);

            if (Member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View(Member);
        }


        [HttpPost]
        public async Task<ActionResult> Edit([FromRoute]int id,MemberToUpdateViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);


            var Result = await _memberService.UpdateMemberAsync(id, viewModel);


            if (Result)
            {
                TempData["SuccessMessage"] = "Member Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Member Failed to Update , check Phone and Email";
                return RedirectToAction(nameof(Index));
            }

        }

        #endregion

        #region Delete Member

        public async Task<ActionResult> Delete(int id)
        {
            if(id <= 0)
            {

                TempData["ErrorMessage"] = "Id Of Member can Not be 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }


            var Member = await _memberService.GetMemberDetailsAsync(id);

            if (Member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.MemberId = id;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> DeleteConfirm(int id)
        {
            var Result = await _memberService.RemoveMemberAsync(id);

            if (Result)
                TempData["SuccessMessage"] = "Member Deleted Successfully";


            else
                TempData["ErrorMessage"] = "Member Failed to Delete ";



            return RedirectToAction(nameof(Index));

        }

        #endregion

    }
}
