using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Post_Office_Management.Data;

namespace Post_Office_Management.Controllers
{
    public class InfoController : Controller
    {
        private readonly ApplicationDbContext _db;
        public InfoController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Services()
        {
            var service = _db.Services.ToList();
            return View(service);
        }

        public IActionResult Charges()
        {
            var charges = _db.Charges.Include(s => s.ServiceType).ToList();
            return View(charges);
        }

        public IActionResult Locations()
        {
            var locations = _db.Locations.ToList();
            return View(locations);
        }
    }
}
