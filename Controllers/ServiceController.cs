using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Post_Office_Management.Data;
using Post_Office_Management.Models;

namespace Post_Office_Management.Controllers
{
    [Authorize(Roles = "admin, employee")]
    [Route("Dashboard/[controller]/[action]")]
    public class ServiceController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ServiceController> _logger;

        public ServiceController(ApplicationDbContext db, ILogger<ServiceController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public IActionResult List()
        {
            return View(_db.Services.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(ServiceType service)
        {
            if (ModelState.IsValid)
            {
                _db.Services.Add(service);
                _db.SaveChanges();
                return RedirectToAction("List");
            }
            return View(service);
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var service = _db.Services.Find(id);
            if (service != null)
            {
                return View(service);
            }
            return NotFound("Service not found");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ServiceType service)
        {
            if (ModelState.IsValid)
            {
                _db.Services.Update(service);
                await _db.SaveChangesAsync();
                return RedirectToAction("List");
            }
            return View(service);
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var service = _db.Services.Find(id);
            if (service != null)
            {
                return View(service);
            }
            return NotFound("Service not found");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ServiceType service)
        {
            _db.Remove(service);
            await _db.SaveChangesAsync();
            return RedirectToAction("List");
        }
    }
}
