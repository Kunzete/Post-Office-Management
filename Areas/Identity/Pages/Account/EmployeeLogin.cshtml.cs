using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;

namespace Post_Office_Management.Areas.Identity.Pages.Account
{
    public class EmployeeLoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<EmployeeLoginModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public EmployeeLoginModel(
            SignInManager<IdentityUser> signInManager,
            ILogger<EmployeeLoginModel> logger,
            UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }
        public async Task OnGetAsync(string returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User))
            {
                Response.Redirect("/Dashboard");
                return;
            }
            else
            {
                if (!string.IsNullOrEmpty(ErrorMessage))
                {
                    ModelState.AddModelError(string.Empty, ErrorMessage);
                }

                returnUrl ??= Url.Content("~/");

                // Clear the existing external cookie to ensure a clean login process
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

                ReturnUrl = returnUrl;
            }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/Dashboard");

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.FindByNameAsync(Input.Username.Trim());
                    if (user != null && await _userManager.CheckPasswordAsync(user, Input.Password))
                    {
                        await _signInManager.SignInAsync(user, Input.RememberMe);
                        _logger.LogInformation("User logged in.");
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        _logger.LogWarning("Invalid login attempt.");
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        return Page();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error logging in user.");
                    ModelState.AddModelError(string.Empty, "An error occurred while logging in.");
                    return Page();
                }
            }
            return Page();
        }
    }
}
