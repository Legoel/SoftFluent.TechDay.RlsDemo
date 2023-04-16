using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
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
		public ActionResult<UserTokenDto> GetToken(string role)
		{
			var expiresOn = DateTime.UtcNow.AddMinutes(_configuration.GetValue("Authentication:JwtToken:Expiration", 60));
			var login = $"Jean-Michel {role.ToUpper()}";

			var userToken = new UserTokenDto
			{
				Login = login,
				ExpiresOn = expiresOn,
				Roles = GetRoles(login)
			};

			var tokenHandler = new JwtSecurityTokenHandler();

			var secret = Encoding.ASCII.GetBytes(_configuration.GetValue<string>("Authentication:JwtToken:Secret", "TheB€stKept_secretIn-the*World")!);
			var descriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(GetClaims(login)),
				Expires = expiresOn,
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256Signature)
			};

			SecurityToken token = tokenHandler.CreateToken(descriptor);
			userToken.Token = tokenHandler.WriteToken(token);
			return Ok(userToken);
		}

		private static IEnumerable<string> GetRoles(string login)
		{
			yield return login.Contains("Admin", StringComparison.OrdinalIgnoreCase) ? "Administrator" : "Contributor";
		}

		private static IEnumerable<Claim> GetClaims(string login)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.NameIdentifier, login),
				new Claim(ClaimTypes.Name, login)
			};

			claims.AddRange(GetRoles(login).Select(role => new Claim(ClaimTypes.Role, role)));

			return claims;
		}
	}
}
