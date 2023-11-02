using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using JetDevs.Api.Context;
using JetDevs.Api.Controllers;
using JetDevs.Api.Models.DbEntities;
using JetDevs.Api.Models.ViewModels;
using JetDevs.Common.Web.Email;
using JetDevs.Common.Web.Options;
using JetDevs.Common.Web.Security;
using System.Collections.Generic;
using Xunit;

namespace AuthUnitTests
{
	public class AccountControllerUnitTests
	{
		[Fact]
		public async void CreateUser()
		{
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(databaseName: "Unit_Testing_Database")
				.Options;

			var userStore = new Mock<IUserStore<User>>();
			var passwordManager = userStore.As<IUserPasswordStore<User>>();
			var ioptions = new Mock<IOptions<IdentityOptions>>();
			var identityOptions = new IdentityOptions();
			identityOptions.Lockout.AllowedForNewUsers = false;
			ioptions.Setup(o => o.Value).Returns(identityOptions);
			var userValidators = new List<IUserValidator<User>>();
			var validator = new Mock<IUserValidator<User>>();
			userValidators.Add(validator.Object);
			var pwdValidators = new List<PasswordValidator<User>>
			{
				new PasswordValidator<User>()
			};

			var userManager = new UserManager<User>(userStore.Object, ioptions.Object, new PasswordHasher<User>(),
				userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
				new IdentityErrorDescriber(), null,
				new Mock<ILogger<UserManager<User>>>().Object
			);

			var roleStore = new Mock<IRoleStore<IdentityRole>>();
			var appDbContext = new Mock<ApplicationDbContext>(options);
			var jwtFactory = new Mock<IJWTFactory>();
			var jwtIssuerOptions = new Mock<IOptions<JwtIssuerOptions>>();
			var emailSender = new Mock<IEmailSender>();
			var roleManager = new RoleManager<IdentityRole>(roleStore.Object, null, null, null, null);
			var logger = new Mock<ILogger<AccountController>>();

			var mapperConfig = new MapperConfiguration(config =>
			{
				config.CreateMap<RegistrationUserViewModel, User>().ForMember(au => au.UserName, map => map.MapFrom(vm => vm.Email));
			});
			var mapper = mapperConfig.CreateMapper();

			var accountController = new AccountController(
					appDbContext.Object,
					userManager,
					mapper,
					jwtFactory.Object,
					jwtIssuerOptions.Object,
					emailSender.Object,
					roleManager,
					logger.Object
				);

			var registrationUser = new RegistrationUserViewModel()
			{
				Email = "testing@localhost",
				Password = "testingPassword12345@",
				FirstName = "Testing",
				LastName = "McTesterson",
				Roles = new List<string> { "Administrator", "User" }
			};

			var result = await accountController.CreateUser(registrationUser);

			var okRequestObject = Assert.IsType<OkObjectResult>(result);

			Assert.Equal("Success", okRequestObject.Value);

		}
	}
}
