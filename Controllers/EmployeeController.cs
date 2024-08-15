using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using Post_Office_Management.Data;
using Post_Office_Management.Models;
using SQLitePCL;

namespace Post_Office_Management.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("Dashboard/[controller]/[action]")]

    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public EmployeeController(
            ILogger<EmployeeController> logger,
            ApplicationDbContext db,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<IdentityUser> signInManager)
        {
            _logger = logger;
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public IActionResult List()
        {
            return View(_db.Employees.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Employee model)
        {
            if (ModelState.IsValid)
            {
                if (_db.Users.Any(u => u.Email == model.Email) || _db.Employees.Any(e => e.Email == model.Email))
                {
                    // Email already exists in the database for a user or an employee
                    ModelState.AddModelError("Email", "Email already exists.");
                    return View(model);
                }
                else
                {
                    var loginIdentity = Guid.NewGuid().ToString();
                    var user = new IdentityUser { UserName = loginIdentity, Email = model.Email };
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "employee");
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        result = await _userManager.ConfirmEmailAsync(user, token);
                        if (result.Succeeded)
                        {
                            // Email address confirmed
                        }

                        // Save the employee data to the database
                        var employee = new Employee
                        {
                            LoginIdentity = loginIdentity,
                            Name = model.Name,
                            Email = model.Email,
                            PhoneNumber = model.PhoneNumber,
                            Password = model.Password,
                            Address = model.Address,
                            CreatedBy = User.Identity?.Name,
                            CreatedAt = DateTime.Now
                        };
                        _db.Employees.Add(employee);
                        await _db.SaveChangesAsync();
                        return RedirectToAction("List");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _db.Employees.FindAsync(id);
            var user = await _userManager.FindByEmailAsync(employee.Email);

            if (employee == null || user == null)
            {
                return NotFound("Employee or user not found");
            }

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Employee model)
        {
            // Update the employee and user data
            var employee = _db.Employees.Find(model.EmployeeID);
            var user = await _userManager.FindByEmailAsync(employee.Email);

            if (employee == null || user == null)
            {
                return NotFound("Employee or user not found");
            }

            if (ModelState.IsValid)
            {
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.PhoneNumber = model.PhoneNumber;
                employee.Address = model.Address;
                employee.UpdatedAt = DateTime.Now;
                employee.UpdatedBy = "Admin";

                // Update the user data
                user.Email = model.Email;

                if (!string.IsNullOrEmpty(Request.Form["OldPassword"]) && !string.IsNullOrEmpty(Request.Form["Password"]))
                {
                    var passwordResult = await _userManager.ChangePasswordAsync(user, Request.Form["OldPassword"], Request.Form["Password"]);
                    if (!passwordResult.Succeeded)
                    {
                        foreach (var error in passwordResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(employee);
                    }
                }

                await _userManager.UpdateAsync(user);
                _db.Employees.Update(employee);
                await _db.SaveChangesAsync();
                return RedirectToAction("List");
            }
            return View(employee);
        }

        public IActionResult Delete(int id)
        {
            var employee = _db.Employees.Find(id);

            if ( employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Employee employee)
        {
            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            var users = await _userManager.GetUsersInRoleAsync("employee");
            var usersWithEmail = users.Where(u => u.Email == employee.Email);

            var user = usersWithEmail.FirstOrDefault();
            if (user != null)
            {
                _db.Employees.Remove(employee);
                await _userManager.DeleteAsync(user);
                await _db.SaveChangesAsync();
            }
            else
            {
                _db.Employees.Remove(employee);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("List");
        }

    }
}
