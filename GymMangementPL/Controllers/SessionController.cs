using GymManagementBLL.Services.Classes;
using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.SessionViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GymManagementPL.Controllers
{
    [Authorize]
    public class SessionController : Controller
    {
        private readonly ISessionService _sessionService;

        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
        }

        public async Task<ActionResult> Index()
        {

            var Sessions = await _sessionService.GetAllSessionsAsync();

            return View(Sessions);
        }

        public async Task<ActionResult> Details(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "id must be valid";
                return RedirectToAction(nameof(Index));
            }


            var Session = await _sessionService.GetSessionByIdAsync(id);

            if (Session is null)
            {
                TempData["ErrorMessage"] = "Session not found";
                return RedirectToAction(nameof(Index));
            }

            return View(Session);

        }


        public async Task<ActionResult> Create()
        {

            await LoadTrainersDropDowns();
            await LoadCategoriesDropDowns();


            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateSessionViewModel createSession)
        {
            if (!ModelState.IsValid)
            {

                await LoadTrainersDropDowns();
                await LoadCategoriesDropDowns();
                return View(createSession);
            }

            var Result = await _sessionService.CreateSessionAsync(createSession);

            if (Result)
            {
                TempData["SuccessMessage"] = "Session Created Successfully";

                return RedirectToAction(nameof(Index));

            }
            else
            {
                TempData["ErrorMessage"] = "Session Created Failed";
                await LoadTrainersDropDowns();
                await LoadCategoriesDropDowns();
                return View(createSession);
            }

        }


        public async Task<ActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "id must be valid";
                return RedirectToAction(nameof(Index));
            }

            var Session = await _sessionService.GetSessionToUpdateAsync(id);
            

            if (Session is null)
            {
                TempData["ErrorMessage"] = "Session not found";
                return RedirectToAction(nameof(Index));
            }

            await LoadTrainersDropDowns();

            return View(Session);

        }

        [HttpPost]

        public async Task<ActionResult> Edit([FromRoute] int id , SessionToUpdateViewModel sessionToUpdate)
        {
            if (!ModelState.IsValid)
            {

                await LoadTrainersDropDowns();
                return View(sessionToUpdate);
            }

            var Result = await _sessionService.UpdateSessionAsync(id, sessionToUpdate);

            if (Result)
            {
                TempData["SuccessMessage"] = "Session Updated Successfully";

                return RedirectToAction(nameof(Index));

            }
            else
            {
                TempData["ErrorMessage"] = "Session Updated Failed";
                await LoadTrainersDropDowns();
                return View(sessionToUpdate);
            }

        }

        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "id must be valid";
                return RedirectToAction(nameof(Index));
            }


            var Session = await _sessionService.GetSessionByIdAsync(id);

            if (Session is null)
            {
                TempData["ErrorMessage"] = "Session not found";
                return RedirectToAction(nameof(Index));
            }



            ViewBag.SessionId = id;
            return View();

        }

        [HttpPost]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var Result = await _sessionService.RemoveSessionAsync(id);
            
            if (Result)
                TempData["SuccessMessage"] = "Session Deleted Successfully";

            else
                TempData["ErrorMessage"] = "Session Deleted Failed";

            return RedirectToAction(nameof(Index));

        }


        #region Helper 


        private async Task LoadTrainersDropDowns()
        {

            var Trainers = await _sessionService.GetAllTrainersForDropDownAsync();
            ViewBag.Trainers = new SelectList(Trainers, "Id", "Name");
        }
        private async Task LoadCategoriesDropDowns()
        {

            var Categories = await _sessionService.GetAllCategoriesForDropDownAsync();
            ViewBag.Categories = new SelectList(Categories, "Id", "Name");
        }

        #endregion
    }
}
