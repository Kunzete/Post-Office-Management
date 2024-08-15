using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Post_Office_Management.Data;
using Post_Office_Management.Models;
using System.Security.Claims;

namespace Post_Office_Management.Controllers
{
    [Authorize(Roles = "admin, employee")]
    [Route("Dashboard/[controller]/[action]")]
    public class ChargeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<ChargeController> _logger;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public ChargeController(ApplicationDbContext db, ILogger<ChargeController> logger, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult List()
        {
            var chargeDetails = _db.Charges
                                   .Include(cd => cd.ServiceType)
                                   .ToList();
            return View(chargeDetails);
        }

        public IActionResult Create()
        { 
            ViewBag.ServiceTypes = _db.Services.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(ChargeDetail model)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }
            if (ModelState.IsValid)
            {
                var serviceType = await _db.Services.FindAsync(model.ServiceTypeId);
                if (serviceType == null)
                {
                    ModelState.AddModelError("ServiceTypeId", "Service Type not found");
                    ViewBag.ServiceTypes = _db.Services.ToList();
                    return View(model);
                }

                model.CreatedDate = DateTime.Now;
                model.LastModifiedDate = DateTime.Now;

                try
                {
                    _db.Charges.Add(model);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("List");
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Error creating charge: " + ex.Message);
                }
            }

            ViewBag.ServiceTypes = _db.Services.ToList();
            return View(model);
        }

        public IActionResult Edit(int id)
        {
            var chargeDetail = _db.Charges.Find(id);
            if (chargeDetail == null)
            {
                return NotFound("Charge detail not found.");
            }

            ViewBag.ServiceTypes = _db.Services.ToList();
            return View(chargeDetail);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ChargeDetail charges)
        {
            var existingCharge = await _db.Charges.FindAsync(charges.Id);
            if (existingCharge == null)
            {
                return NotFound("Charge detail not found.");
            }

            if (ModelState.IsValid)
            {
                // Retain original values for CreatedBy and CreatedDate
                charges.CreatedBy = existingCharge.CreatedBy;
                charges.CreatedDate = existingCharge.CreatedDate;

                // Update necessary fields
                charges.LastModifiedDate = DateTime.Now;
                charges.LastModifiedBy = (await _userManager.GetUserAsync(User)).Email;

                try
                {
                    _db.Entry(existingCharge).CurrentValues.SetValues(charges);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("List");
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Error updating charge: " + ex.Message);
                }
            }

            ViewBag.ServiceTypes = _db.Services.ToList();
            return View(charges);
        }

    }
}
