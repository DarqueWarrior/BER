namespace PeopleTracker.BerService.Tests
{
   using Microsoft.AspNetCore.Mvc;
   using Microsoft.Extensions.Logging;
   using Microsoft.Extensions.Options;
   using NSubstitute;
   using PeopleTracker.BerService.Contracts;
   using PeopleTracker.BerService.Controllers;
   using System;
   using Xunit;

   public class AuthControllerTests
   {
      [Fact]
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

         var target = new AuthController(logger, options);

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

      [Fact]
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

         var target = new AuthController(logger, options);

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

      [Fact]
      public void CreateToken()
      {
         // Arrange
         var options = Substitute.For<IOptions<TokenData>>();

         options.Value.Returns<TokenData>(new TokenData
         {
            Web = "Web",
            Mobile = "Mobile",
            Key = "ThisMustBeALongKeyValueForItToWork",
            Issuer = "Issuer",
            Audience = "Audience"
         });

         var logger = Substitute.For<ILogger<AuthController>>();

         var model = new CredentialModel
         {
            UserAgent = "Web"
         };

         var target = new AuthController(logger, options);

         // Act
         var result = target.CreateToken(model);

         // Assert
         Assert.IsType<OkObjectResult>(result);
      }

      [Fact]
      public void CreateToken_Throws()
      {
         // Arrange
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

         var target = new AuthController(logger, options);

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
