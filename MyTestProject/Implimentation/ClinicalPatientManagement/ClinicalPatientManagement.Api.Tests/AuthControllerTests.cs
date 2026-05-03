using Microsoft.AspNetCore.Identity;using Microsoft.AspNetCore.Http;using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using ClinicalPatientManagement.Api.Controllers;
using ClinicalPatientManagement.Api.DTOs;
using ClinicalPatientManagement.Api.Models;

namespace ClinicalPatientManagement.Api.Tests;

public class AuthControllerTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);
        _configurationMock = new Mock<IConfiguration>();

        // Setup configuration
        _configurationMock.Setup(c => c["Jwt:Key"]).Returns("your-super-secret-key-change-in-production-min-32-chars");
        _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("ClinicalPatientManagement");
        _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("ClinicalPatientManagementClient");
        _configurationMock.Setup(c => c["Jwt:ExpirationMinutes"]).Returns("60");

        _controller = new AuthController(
            _userManagerMock.Object,
            _configurationMock.Object);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "doctor", Password = "Password123!" };
        var user = new ApplicationUser { UserName = "doctor", Id = "1" };

        _userManagerMock.Setup(um => um.FindByNameAsync(loginDto.Username))
            .ReturnsAsync(user);
        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, loginDto.Password))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = okResult.Value;
        Assert.NotNull(response);
        // Check that Token property exists
        var tokenProperty = response.GetType().GetProperty("Token");
        Assert.NotNull(tokenProperty);
        var token = tokenProperty.GetValue(response) as string;
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task Login_InvalidUsername_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "invalid", Password = "Password123!" };

        _userManagerMock.Setup(um => um.FindByNameAsync(loginDto.Username))
            .ReturnsAsync((ApplicationUser)null);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid username or password", unauthorizedResult.Value);
    }

    [Fact]
    public async Task Login_InvalidPassword_ReturnsUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto { Username = "doctor", Password = "wrongpassword" };
        var user = new ApplicationUser { UserName = "doctor" };

        _userManagerMock.Setup(um => um.FindByNameAsync(loginDto.Username))
            .ReturnsAsync(user);
        _userManagerMock.Setup(um => um.CheckPasswordAsync(user, loginDto.Password))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Invalid username or password", unauthorizedResult.Value);
    }
}