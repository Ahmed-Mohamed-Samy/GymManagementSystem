using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.BookingViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace GymManagementPL.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<ActionResult> Index()
        {
            var sessions = await _bookingService.GetAllSessionsWithTrainerAndCategoryAsync();
            
            return View(sessions);
        }

        public async Task<ActionResult> GetMembersForUpcomingSession(int id)
        {
            var members = await _bookingService.GetAllMembersForSessionAsync(id);
            return View(members);
        }

        public async Task<ActionResult> GetMembersForOngoingSession(int id)
        {
            var members = await _bookingService.GetAllMembersForSessionAsync(id);
            return View(members);
        }

        public async Task<ActionResult> Create(int id)
        {
            var Members = await _bookingService.GetMembersForDropDownAsync(id);
            ViewBag.Members = new SelectList(Members, "Id", "Name"); 

            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Create(CreateBookingViewModel createBooking)
        {
            var Result = await _bookingService.CreateBookingAsync(createBooking);

            if(Result)
                TempData["SuccessMessage"] = "Booking Created Successfully";
            else
                TempData["ErrorMessage"] = "Booking Created Failed";

            return RedirectToAction(nameof(GetMembersForUpcomingSession),new {id = createBooking.SessionId});
        }

        [HttpPost]
        public async Task<ActionResult> Cancel(MemberAttendOrCancelViewModel model)
        {
            var Result = await  _bookingService.CancelBookingAsync(model);


            if (Result)
                TempData["SuccessMessage"] = "Booking Cancelled Successfully";
            else
                TempData["ErrorMessage"] = "Booking Cancelled Failed";

            return RedirectToAction(nameof(GetMembersForUpcomingSession), new { id = model.SessionId });
        }
        [HttpPost]
        public async Task<ActionResult> Attended(MemberAttendOrCancelViewModel model)
        {
            var Result = await _bookingService.MemberAttendAsync(model);


            if (Result)
                TempData["SuccessMessage"] = "Booking Cancelled Successfully";
            else
                TempData["ErrorMessage"] = "Booking Cancelled Failed";

            return RedirectToAction(nameof(GetMembersForOngoingSession), new { id = model.SessionId });
        }
    }
}
