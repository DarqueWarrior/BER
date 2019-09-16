namespace BerService.Controllers
{
   using Microsoft.AspNetCore.Authorization;
   using Microsoft.AspNetCore.Mvc;
   using Microsoft.Extensions.Logging;
   using Microsoft.Extensions.Options;
   using Microsoft.IdentityModel.Tokens;
   using BerService.Model.Contracts;
   using BerService.Filters;
   using System;
   using System.IdentityModel.Tokens.Jwt;
   using System.Security.Claims;
   using System.Text;

   /// <summary>
   /// This controller is used to give out JWTs to use to auth calls to the
   /// Record controller.
   /// </summary>
   [ValidateModel]
   public class AuthController : Controller
   {
      private readonly IOptions<TokenData> _tokenData;
      private readonly ILogger<AuthController> _logger;

      public AuthController(ILogger<AuthController> logger, IOptions<TokenData> tokenData)
      {
         _logger = logger;
         _tokenData = tokenData;
      }

      [HttpPost("api/auth/token")]
      [AllowAnonymous]
      public IActionResult CreateToken([FromBody] CredentialModel model)
      {
         try
         {
            _logger.LogInformation("Trying to create Token");

            // For production this needs to be much more sophisticated.
            // For demo purposes we are just making sure the model.UserAgent
            // matches values stored in configuration. The configuration values
            // are stored in appsettings.json.
            // I wanted to control the tokens for the mobile app separate from 
            // the web app. You can reduce to one or add more to test here. 
            // If the strings match a token is created. If not return a bad request.
            if (string.Compare(model.UserAgent, _tokenData.Value.Web) != 0 &&
                string.Compare(model.UserAgent, _tokenData.Value.Mobile) != 0)
            {
               if (string.IsNullOrEmpty(_tokenData.Value.Web) ||
                   string.IsNullOrEmpty(_tokenData.Value.Mobile))
               {
                  _logger.LogWarning("Check server configuration");
               }

               return BadRequest();
            }

            var claims = new[]
            {
               new Claim(JwtRegisteredClaimNames.Sub, model.UserAgent),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenData.Value.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
               issuer: _tokenData.Value.Issuer,
               audience: _tokenData.Value.Audience,
               claims: claims,
               expires: DateTime.UtcNow.AddMinutes(15),
               signingCredentials: creds);

            return Ok(new
            {
               token = new JwtSecurityTokenHandler().WriteToken(token),
               expiration = token.ValidTo
            });
         }
         catch (Exception e)
         {
            _logger.LogError(e.Message);
         }

         return BadRequest();
      }
   }
}