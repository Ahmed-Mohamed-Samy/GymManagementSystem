using GymManagementBLL.Services.Classes;
using GymManagementBLL.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagementPL.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IAnalyticsService _analyticsService;

        public HomeController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        public async Task<IActionResult> Index()
        {

            var analtyics = await _analyticsService.GetAnalyticsDataAsync();
            return View(analtyics);
        }
    }
}
