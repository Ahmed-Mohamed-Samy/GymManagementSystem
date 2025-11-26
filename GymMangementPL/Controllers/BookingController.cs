using GymManagementBLL.Services.Interfaces;
using GymManagementBLL.ViewModels.BookingViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GymManagementPL.Controllers
{
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public ActionResult Index()
        {
            var sessions = _bookingService.GetAllSessionsWithTrainerAndCategory();
            
            return View(sessions);
        }

        public ActionResult GetMembersForUpcomingSession(int id)
        {
            var members = _bookingService.GetAllMembersForSession(id);
            return View(members);
        }

        public ActionResult GetMembersForOngoingSession(int id)
        {
            var members = _bookingService.GetAllMembersForSession(id);
            return View(members);
        }

        public ActionResult Create(int id)
        {
            var Members = _bookingService.GetMembersForDropDown(id);
            ViewBag.Members = new SelectList(Members, "Id", "Name"); 

            return View();
        }
        [HttpPost]
        public ActionResult Create(CreateBookingViewModel createBooking)
        {
            var Result = _bookingService.CreateBooking(createBooking);

            if(Result)
                TempData["SuccessMessage"] = "Booking Created Successfully";
            else
                TempData["ErrorMessage"] = "Booking Created Failed";

            return RedirectToAction(nameof(GetMembersForUpcomingSession),new {id = createBooking.SessionId});
        }

        [HttpPost]
        public ActionResult Cancel(MemberAttendOrCancelViewModel model)
        {
            var Result =  _bookingService.CancelBooking(model);


            if (Result)
                TempData["SuccessMessage"] = "Booking Cancelled Successfully";
            else
                TempData["ErrorMessage"] = "Booking Cancelled Failed";

            return RedirectToAction(nameof(GetMembersForUpcomingSession), new { id = model.SessionId });
        }
        [HttpPost]
        public ActionResult Attended(MemberAttendOrCancelViewModel model)
        {
            var Result = _bookingService.MemberAttend(model);


            if (Result)
                TempData["SuccessMessage"] = "Booking Cancelled Successfully";
            else
                TempData["ErrorMessage"] = "Booking Cancelled Failed";

            return RedirectToAction(nameof(GetMembersForOngoingSession), new { id = model.SessionId });
        }
    }
}
