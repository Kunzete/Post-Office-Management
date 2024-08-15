using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Post_Office_Management.Data;

namespace Post_Office_Management.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public DashboardController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var users = _userManager.GetUsersInRoleAsync("user").Result.Count();
            ViewBag.userCount = users;

            var employee = _userManager.GetUsersInRoleAsync("employee").Result.Count();
            ViewBag.employeeCount = employee;

            var deliverable = _db.Deliverables;
            ViewBag.deliverableCount = deliverable.Count();

            var service = _db.Services;
            ViewBag.serviceCount = service.Count();

            var deliveries = _db.Deliverables.ToList();

            return View(deliveries);
        }
    }
}
