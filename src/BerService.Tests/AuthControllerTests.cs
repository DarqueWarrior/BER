namespace BerService.Tests
{
   using Microsoft.AspNetCore.Mvc;
   using Microsoft.Extensions.Logging;
   using Microsoft.Extensions.Options;
   using NSubstitute;
   using BerService.Model.Contracts;
   using BerService.Controllers;
   using System;
   using Xunit;
    using Microsoft.IdentityModel.Logging;

    public class AuthControllerTests
   {
      [Fact]
      [Trait("Category", "WebAppTests")]
      public void CreateToken_CheckConfig_Web()
      {
         // Arrange
         var options = Substitute.For<IOptions<TokenData>>();

         options.Value.Returns<TokenData>(new TokenData());

         var logger = Substitute.For<ILogger<AuthController>>();

         var model = new CredentialModel
         {
            UserAgent = "UnitTesting"
         };

         using (var target = new AuthController(logger, options))
         {

            // Act
            var result = target.CreateToken(model);

            // Assert
            logger.Received().Log(
               LogLevel.Warning,
               Arg.Any<EventId>(),
               Arg.Is<object>(o => o.ToString() == "Check server configuration"),
               null,
               Arg.Any<Func<object, Exception, string>>());

            Assert.IsType<BadRequestResult>(result);
         }
      }

      [Fact]
      [Trait("Category", "WebAppTests")]
      public void CreateToken_CheckConfig_Mobile()
      {
         // Arrange
         var options = Substitute.For<IOptions<TokenData>>();

         options.Value.Returns<TokenData>(new TokenData
         {
            Web = "Web"
         });

         var logger = Substitute.For<ILogger<AuthController>>();

         var model = new CredentialModel
         {
            UserAgent = "UnitTesting"
         };

         using (var target = new AuthController(logger, options))
         {

            // Act
            var result = target.CreateToken(model);

            // Assert
            logger.Received().Log(
               LogLevel.Warning,
               Arg.Any<EventId>(),
               Arg.Is<object>(o => o.ToString() == "Check server configuration"),
               null,
               Arg.Any<Func<object, Exception, string>>());

            Assert.IsType<BadRequestResult>(result);
         }
      }

      [Fact]
      [Trait("Category", "WebAppTests")]
      public void CreateToken()
      {
         // Arrange
         IdentityModelEventSource.ShowPII = true;
         var options = Substitute.For<IOptions<TokenData>>();

         options.Value.Returns<TokenData>(new TokenData
         {
            Web = "Web",
            Mobile = "Mobile",
            Key = "ThisMustBeAtLeast16Characters",
            Issuer = "Issuer",
            Audience = "Audience"
         });

         var logger = Substitute.For<ILogger<AuthController>>();

         var model = new CredentialModel
         {
            UserAgent = "Web"
         };

         using (var target = new AuthController(logger, options))
         {
            // Act
            var result = target.CreateToken(model);

            // Assert
            Assert.IsType<OkObjectResult>(result);
         }
      }

      [Fact]
      [Trait("Category", "WebAppTests")]
      public void CreateToken_Throws()
      {
         // Arrange
         IdentityModelEventSource.ShowPII = true;
         var options = Substitute.For<IOptions<TokenData>>();

         options.Value.Returns<TokenData>(new TokenData
         {
            Web = "Web",
            Mobile = "Mobile",
            Key = "ToShort",
            Issuer = "Issuer",
            Audience = "Audience"
         });

         var logger = Substitute.For<ILogger<AuthController>>();

         var model = new CredentialModel
         {
            UserAgent = "Web"
         };

         using (var target = new AuthController(logger, options))
         {
            // Act
            var result = target.CreateToken(model);

            // Assert
            // Just test that the error was logged
            logger.Received().Log(
               LogLevel.Error,
               Arg.Any<EventId>(),
               Arg.Any<object>(),
               null,
               Arg.Any<Func<object, Exception, string>>());

            Assert.IsType<BadRequestResult>(result);
         }
      }
   }
}
