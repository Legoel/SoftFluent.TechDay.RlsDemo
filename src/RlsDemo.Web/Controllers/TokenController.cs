using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RlsDemo.Context.Model;

namespace RlsDemo.Web.Controllers
{
	[ApiController, Route("api/[controller]")]
	public class TokenController : ControllerBase
	{
		private readonly IConfiguration _configuration;

		public TokenController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		[AllowAnonymous]
		[HttpGet]
		public ActionResult<UserTokenDto> GetToken(string name)
		{
			var expiresOn = DateTime.UtcNow.AddMinutes(_configuration.GetValue("Authentication:JwtToken:Expiration", 60));
			var role = GetRole(name);
			var login = $"{name} {role.ToUpper()}";
			var tenant = GetTenant(name);

			var userToken = new UserTokenDto
			{
				Login = login,
				ExpiresOn = expiresOn,
				Roles = new[] { role },
				TenantId = tenant,
			};

			var tokenHandler = new JwtSecurityTokenHandler();

			var secret = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("Authentication:JwtToken:Secret", "TheB€stKept_secretIn-the*World")!);
			var descriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(GetClaims(login, role, tenant)),
				Expires = expiresOn,
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature)
			};

			SecurityToken token = tokenHandler.CreateToken(descriptor);
			userToken.Token = tokenHandler.WriteToken(token);
			return Ok(userToken);
		}

		private static string GetRole(string name)
		{
			return name switch
			{
				"Thomas" => "Administrator",
				_ => "User",
			};
		}

		private static int GetTenant(string name)
		{
			return name switch
			{
				"Thomas" => 1,
				"Maxime" => 2,
				_ => 3,
			};
		}

		private static IEnumerable<Claim> GetClaims(string login, string role, int tenant)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, login),
				new Claim(ClaimTypes.Name, login),
				new Claim(ClaimTypes.Role, role),
				new Claim("Tenant", tenant.ToString()),
			};

			return claims;
		}
	}
}
