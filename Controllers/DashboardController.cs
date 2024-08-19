using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

            var OurServices = _db.Services.ToList();
            ViewBag.OurServices = OurServices;

            var OurCharges = _db.Charges.Include(s => s.ServiceType).ToList();
            ViewBag.OurCharges = OurCharges;

            var OurLocations = _db.Locations.ToList();
            ViewBag.OurLocations = OurLocations;

            var deliveries = _db.Deliverables.Include(f => f.FromOffice).Include(t => t.ToOffice).Include(s => s.ServiceType).ToList();

            var topServices = _db.Deliverables
                .GroupBy(d => d.ServiceType)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => new { ServiceName = g.Key.Name, ServiceCharge = g.Key.BaseCharge, VPPStatus = g.Key.IsVPP, ServiceCount = g.Count() })
                .ToList();


            ViewBag.topServices = topServices;

            return View(deliveries);
        }
    }
}
