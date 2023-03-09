using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace SecurityForAssessmentStudent.Pages.RoleManager
{
    public class AssignModel : PageModel
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AssignModel(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public SelectList Roles { get; set; }
        public SelectList Users { get; set; }
        [BindProperty, Required, Display(Name = "Role")]

        public string SelectedRole { get; set; }
        [BindProperty, Required, Display(Name = "User")]
        public string SelectedUser { get; set; }
        public async Task OnGet()
        {
            await GetOptions();
        }

        public async Task GetOptions()
        {
            Roles = new SelectList(await _roleManager.Roles.ToListAsync(), nameof(IdentityRole));
            Users = new SelectList(await _userManager.Users.ToListAsync(), nameof(IdentityUser));
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(SelectedUser);
                await _userManager.AddToRoleAsync(user, SelectedRole);
                return RedirectToPage("/RoleManager/Index");
            }
            await GetOptions(); return Page();
        }
    }
}
