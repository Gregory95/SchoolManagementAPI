using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using OpenIddict.Server.AspNetCore;
using SchoolManagementAPI.ViewModels.User;

using static OpenIddict.Abstractions.OpenIddictConstants;

namespace SchoolManagementAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _db;
        private readonly OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> _applicationManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IOpenIddictScopeManager _scopeManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public AuthController(
            IConfiguration configuration,
            ApplicationDbContext db,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication> applicationManager,
            IHttpContextAccessor httpContextAccessor,
            IMapper mapper
        )
        {
            _configuration = configuration;
            _db = db;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _applicationManager = applicationManager;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        [
            HttpPost("~/token"),
            IgnoreAntiforgeryToken,
            Produces("application/json")
        ]

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest();

            if (request == null || !request.IsPasswordGrantType())
            {
                throw new NotImplementedException("The specified grant is not implemented.");
            }

            // Create a new ClaimsIdentity containing the claims that
            // will be used to create an id_token, a token or a code.
            // Create the claims-based identity that will be used by OpenIddict to generate tokens.
            var identity =
                new ClaimsIdentity(authenticationType: TokenValidationParameters
                        .DefaultAuthenticationType,
                    nameType: Claims.Name,
                    roleType: Claims.Role);

            // Add a "sub" claim containing the user identifier, and attach
            // the "access_token" destination to allow OpenIddict to store it
            // in the access token, so it can be retrieved from your controllers.
            var user = await _userManager.FindByNameAsync(request.Username);

            // var officeClaim =  new Claim("office", user.Id, ClaimValueTypes.Integer);
            // await _userManager.AddClaimAsync(user, officeClaim);

            // identity.AddClaim(officeClaim);

            // ... add other claims, if necessary.
            var principal = new ClaimsPrincipal(identity);

            return SignIn(principal,
            OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserAddVM model)
        {
            _logger.Info("Register new User");

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    ApplicationUser newUser = new ApplicationUser();
                    ErrorList error = new ErrorList();

                    if (string.IsNullOrEmpty(model.Password) && string.IsNullOrEmpty(model.UserName))
                    {
                        throw new Exception("Please complete username and password");
                    }

                    if (string.IsNullOrEmpty(model.Role))
                    {
                        throw new Exception("User must be assigned to a role.");
                    }

                    if (await _userManager.FindByEmailAsync(model.Email) != null)
                    {
                        throw new Exception("An other user with the same email address exists.");
                    }

                    if (await _userManager.FindByNameAsync(model.UserName) == null)
                    {
                        IdentityResult result;

                        var applicationUser = new ApplicationUser
                        {
                            UserName = model.UserName,
                            Email = model.Email,
                            NormalizedUserName = model.UserName.ToUpper(),
                            NormalizedEmail =
                                    !string.IsNullOrEmpty(model.Email)
                                        ? model.Email.ToUpper()
                                        : string.Empty,
                            PhoneNumber = model.PhoneNumber ?? string.Empty,
                            Created = DateTime.UtcNow
                        };

                        if (string.IsNullOrEmpty(model.Password))
                        {
                            result = await _userManager.CreateAsync(applicationUser);
                        }
                        else
                        {
                            result = await _userManager.CreateAsync(applicationUser, model.Password);
                        }

                        if (result.Succeeded)
                        {
                            // Save user to database
                            await _db.SaveChangesAsync();

                            await _userManager.AddToRoleAsync(await _userManager.FindByNameAsync(applicationUser.UserName), model.Role);

                            // Save User Role
                            await _db.SaveChangesAsync();

                            var returnObject = _mapper.Map<ApplicationUser, UserVM>(applicationUser);

                            transaction.Commit();

                            return Ok(returnObject);
                        }
                    }
                    else
                    {
                        error = new ErrorList
                        {
                            Title = "Bad Request",
                            Description = $"User - {model.UserName} already exists.",
                            StatusCode = 400
                        };

                        return BadRequest(error);
                    }

                    error = new ErrorList
                    {
                        Title = "Bad Request",
                        Description = "Something went wrong.",
                        StatusCode = 400
                    };

                    return BadRequest(error);
                }
                catch (System.Exception e)
                {
                    _logger.Error($"Error on registration: {e.Message}");
                    _logger.Error($"Error Stack Trace: {e.StackTrace}");
                    _logger.Error($"Error Inner Exception: {e.InnerException}");

                    transaction.Rollback();

                    ErrorList error = new ErrorList
                    {
                        Title = "Bad Request",
                        Description = e.Message.ToString(),
                        StatusCode = 400
                    };

                    return BadRequest(error);
                }
            }
        }

        #region Auth Functions

        private TokenVM CreateToken(ApplicationUser user)
        {
            List<Claim> claims =
                new List<Claim> { new Claim(ClaimTypes.Name, user.UserName) };

            var key =
                new SymmetricSecurityKey(System
                        .Text
                        .Encoding
                        .UTF8
                        .GetBytes(_configuration
                            .GetSection("AppSettings:Token")
                            .Value));

            var cred =
                new SigningCredentials(key,
                    SecurityAlgorithms.HmacSha512Signature);

            var token =
                new JwtSecurityToken(claims: claims,
                    expires: DateTime.Now.AddHours(5),
                    signingCredentials: cred);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            var tokenReturn =
                new TokenVM
                {
                    Access_Token = jwt,
                    Expires_In = token.Payload.Exp
                };

            return tokenReturn;
        }

        private void CreatePasswordHash(
            string password,
            out byte[] passwordHash,
            out byte[] passwordSalt
        )
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash =
                    hmac
                        .ComputeHash(System
                            .Text
                            .Encoding
                            .UTF8
                            .GetBytes(password));
            }
        }

        private bool
        VerifyPasswordHash(
            string password,
            byte[] passwordHash,
            byte[] passwordSalt
        )
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computeHash =
                    hmac
                        .ComputeHash(System
                            .Text
                            .Encoding
                            .UTF8
                            .GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }


        #endregion
    }
}
