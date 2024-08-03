using BG.Repo.EF;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using PrjBase.SecurityBase;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BG.Api.Controllers.Security;

//[Route("api/[controller]")]
[Route("api/[controller]/[action]")]
[ApiController]
public class AppUsersController : ControllerBase
{
    private readonly BGDbContext _context;
    private readonly ILogger<AppUsersController> _logger;
    private readonly IConfiguration _configuration;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public AppUsersController(
        BGDbContext context,
        ILogger<AppUsersController> logger,
        IConfiguration configuration,
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager)
    {
        _context = context;
        _logger = logger;
        _configuration = configuration;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="userRegisterRequest">An object containing the user data.</param>
    /// <returns>A 201 – Created Status Code in case of success.</returns>
    /// <response code="201">User has been registered</response>
    /// <response code="400">Invalid data</response>
    /// <response code="500">An error occurred</response>
    [HttpPost]
    [ResponseCache(CacheProfileName = "NoCache")]
    [ProducesResponseType(typeof(string), 201)]
    [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 500)]
    public async Task<ActionResult> Register(UserRegisterRequest userRegisterRequest)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var newUser = new AppUser();
                newUser.UserName = userRegisterRequest.UserName;
                newUser.Email = userRegisterRequest.Email;
                IdentityResult? result = await _userManager
                                .CreateAsync(newUser, userRegisterRequest.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation(
                        "User {userName} ({email}) has been created.",
                        newUser.UserName, newUser.Email
                        );

                    return StatusCode(201,
                        $"User '{newUser.UserName}' has been created.");
                }
                else
                    throw new Exception(
                        string.Format(
                            "Error: {0}"
                            , string.Join(" ", result.Errors.Select(e => e.Description))));
            }
            else
            {
                var details = new ValidationProblemDetails(ModelState);
                details.Type =
                        "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                details.Status = StatusCodes.Status400BadRequest;
                return new BadRequestObjectResult(details);
            }
        }
        catch (Exception e)
        {
            var exceptionDetails = new ProblemDetails();
            exceptionDetails.Detail = e.Message;
            exceptionDetails.Status = StatusCodes.Status500InternalServerError;
            exceptionDetails.Type =
                                "https://tools.ietf.org/html/rfc7231#section-6.6.1";
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                exceptionDetails);
        }
    }

    /// <summary>
    /// Performs a user login.
    /// </summary>
    /// <param name="userLoginRequest">
    ///     An object containing the user's credentials.</param>
    /// <returns>The Bearer Token (in JWT format).</returns>
    /// <response code="200">User has been logged in</response>
    /// <response code="400">Login failed (bad request)</response>
    /// <response code="401">Login failed (unauthorized)</response>
    [HttpPost]
    [ResponseCache(CacheProfileName = "NoCache")]
    [ProducesResponseType(typeof(string), 200)]
    [ProducesResponseType(typeof(BadRequestObjectResult), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 401)]
    public async Task<ActionResult> Login(UserLoginRequest userLoginRequest)
    {
        try
        {
            if (ModelState.IsValid)
            {
                bool isUserFound = false;

                AppUser? user = await _userManager
                                .FindByNameAsync(userLoginRequest.UserName);

                if (user is not null)
                    isUserFound = true;

                bool isUserAuthenticated = false;

                if (isUserFound)
                {
                    isUserAuthenticated = await _userManager
                                   .CheckPasswordAsync(user!, userLoginRequest.Password);
                }
                
                if(isUserAuthenticated)
                {
                    byte[] encodedKey = System.Text.Encoding.UTF8.GetBytes(
                                _configuration["JWT:SigningKey"]
                                ?? "ChangeThisInAppSettings123!@#");

                    var signingCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(encodedKey),
                        SecurityAlgorithms.HmacSha256);

                    List<Claim> claims = [];

                    claims.Add(new Claim(
                                ClaimTypes.Name, user!.UserName!));

                    //claims.AddRange(
                    //    (await _userManager.GetRolesAsync(user))
                    //        .Select(r => new Claim(ClaimTypes.Role, r)));

                    IList<string> roleList = await _userManager.GetRolesAsync(user);
                    IEnumerable<Claim> roleClaims = roleList
                                            .Select(r => new Claim(ClaimTypes.Role, r));

                    claims.AddRange(roleClaims);

                    var jwtObject = new JwtSecurityToken(
                        issuer: _configuration["JWT:Issuer"],
                        audience: _configuration["JWT:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddSeconds(300),
                        signingCredentials: signingCredentials);

                    var jwtString = new JwtSecurityTokenHandler()
                                    .WriteToken(jwtObject);

                    return StatusCode(
                        StatusCodes.Status200OK,
                        jwtString);
                }
                else
                {
                    throw new Exception("Invalid login attempt.");
                }
            }
            else
            {
                var details = new ValidationProblemDetails(ModelState);
                details.Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
                details.Status = StatusCodes.Status400BadRequest;
                return new BadRequestObjectResult(details);
            }
        }
        catch (Exception e)
        {
            var exceptionDetails = new ProblemDetails();
            exceptionDetails.Detail = e.Message;
            exceptionDetails.Status = StatusCodes.Status401Unauthorized;
            exceptionDetails.Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1";
            return StatusCode(StatusCodes.Status401Unauthorized, exceptionDetails);
        }
    }
}
