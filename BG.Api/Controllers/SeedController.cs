using BG.Repo.EF;
using Infrastructure.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PrjBase.SecurityBase;

namespace BG.Api.Controllers;

//[Authorize(Roles = RoleName.ADMIN)]
//[Authorize]
[Route("api/[controller]/[action]")]
//[Route("api/[controller]")]
[ApiController]
public class SeedController : ControllerBase
{
    private readonly BGDbContext _dbContext;

    private readonly IWebHostEnvironment _env;

    private readonly ILogger<SeedController> _logger;

    private readonly RoleManager<IdentityRole> _roleManager;

    private readonly UserManager<AppUser> _userManager;

    public SeedController(
        BGDbContext dbContext,
        IWebHostEnvironment env,
        ILogger<SeedController> logger,
        RoleManager<IdentityRole> roleManager,
        UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _env = env;
        _logger = logger;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [HttpPost]
    [ResponseCache(NoStore = true)]
    public async Task<IActionResult> AuthData()
    {
        int rolesCreated = 0;
        int usersAddedToRoles = 0;

        if (!await _roleManager.RoleExistsAsync(RoleName.MODERATOR))
        {
            await _roleManager.CreateAsync(
                new IdentityRole(RoleName.MODERATOR));
            rolesCreated++;
        }
        if (!await _roleManager.RoleExistsAsync(RoleName.ADMIN))
        {
            await _roleManager.CreateAsync(
                new IdentityRole(RoleName.ADMIN));
            rolesCreated++;
        }

        AppUser? testModerator = await _userManager.FindByNameAsync("TestModerator");

        if (testModerator is not null)
        {
            bool isUserModerator = await _userManager
                                        .IsInRoleAsync(testModerator, RoleName.MODERATOR);

            if (!isUserModerator)
            {
                await _userManager.AddToRoleAsync(testModerator, RoleName.MODERATOR);
                usersAddedToRoles++;
            }
        }

        //var testAdministrator = await _userManager.FindByNameAsync("TestAdministrator");
        AppUser? testAdmin = await _userManager.FindByNameAsync("testAdmin");

        if (testAdmin != null
            && !await _userManager.IsInRoleAsync(testAdmin, RoleName.ADMIN))
        {
            await _userManager.AddToRoleAsync(testAdmin, RoleName.MODERATOR);
            await _userManager.AddToRoleAsync(testAdmin, RoleName.ADMIN);
            usersAddedToRoles++;
        }

        return new JsonResult(new
        {
            RolesCreated = rolesCreated,
            UsersAddedToRoles = usersAddedToRoles
        });
    }
}
