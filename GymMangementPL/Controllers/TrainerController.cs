using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.TrainerViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagementPL.Controllers
{
    [Authorize (Roles ="SuperAdmin")]
    public class TrainerController : Controller
    {
        private readonly ITrainerService _trainerService;

        public TrainerController(ITrainerService trainerService)
        {
            _trainerService = trainerService;
        }

        public async Task<ActionResult> Index()
        {

            var trainers = await _trainerService.GetAllTrainersAsync();


            return View(trainers);
        }

        public async Task<ActionResult> TrainerDetails(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Id Of Trainer can Not be 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var trainer = await _trainerService.GetTrainerDetailsAsync(id);

            if (trainer == null)
            {
                TempData["ErrorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }


            return View(trainer);


        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> CreateTrainer(CreateTrainerViewModel createTrainer)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("DataInvalid", "Check Data And Mising Fields");
                return View(nameof(Create), createTrainer);
            }

            bool Result = await _trainerService.CreateTrainerAsync(createTrainer);

            if (Result)
            {
                TempData["SuccessMessage"] = "Trainer Created Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Trainer Created Failed , check Phone and Email";
                return RedirectToAction(nameof(Index));
            }

        }



        public async Task<ActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Id Of Trainer can Not be 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var TrainerToUpdate = await _trainerService.GetTrainerToUpdateAsync(id);

            if (TrainerToUpdate is null)
            {
                TempData["ErrorMessage"] = "Trainer not found";
                return RedirectToAction(nameof(Index));
            }

            return View(TrainerToUpdate);

        }

        [HttpPost]
        public async Task<ActionResult> Edit([FromRoute] int id, TrianerToUpdateViewModel trianer)
        {
            if (!ModelState.IsValid)
                return View(trianer);

            bool Result = await _trainerService.UpdateTrainerAsync(id, trianer);


            if (Result)
            {
                TempData["SuccessMessage"] = "Trainer Updated Successfully";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Trainer Updated Failed , check Phone and Email";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Id Of Trainer can Not be 0 or Negative Number";
                return RedirectToAction(nameof(Index));
            }

            var trainer = await _trainerService.GetTrainerDetailsAsync(id);

            if (trainer is null)
            {
                TempData["ErrorMessage"] = "Trainer Not Found";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.trainerId = id;
            return View();
        }


        [HttpPost]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var Result = await _trainerService.RemoveTrainerAsync(id);

            if (Result)
                TempData["SuccessMessage"] = "Trainer Deleted Successfully";


            else
                TempData["ErrorMessage"] = "Trainer Deleted Failed";

            return RedirectToAction(nameof(Index));

        }


    }
}
