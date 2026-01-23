using FluentAssertions;
using MedNet.API.Controllers;
using MedNet.API.Exceptions;
using MedNet.API.Models.DTO;
using MedNet.API.Models.DTO.Auth;
using MedNet.API.Models.Enums;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace MedNet.API.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _mockAuthService;
    private readonly Mock<ILogger<AuthController>> _mockLogger;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockLogger = new Mock<ILogger<AuthController>>();
        _controller = new AuthController(_mockAuthService.Object, _mockLogger.Object);

        // Setup controller context for admin tests
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, "Admin")
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    #region RegisterPatient Tests

    [Fact]
    public async Task RegisterPatient_WithValidData_ShouldReturnCreatedAtActionWithToken()
    {
        // Arrange
        var registerDto = new RegisterPatientDto
        {
            Email = "test@test.com",
            Password = "Test@123",
            ConfirmPassword = "Test@123",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "+1234567890",
            DateOfBirth = DateTime.Now.AddYears(-30),
            Gender = Gender.Male,
            Height = 180,
            Weight = 75,
            Address = new CreateAddressRequestDto
            {
                Street = "Main St",
                StreetNr = 123,
                City = "City",
                State = "State",
                Country = "Country",
                PostalCode = "12345"
            }
        };

        var expectedToken = "fake-jwt-token";
        _mockAuthService.Setup(s => s.RegisterPatientAsync(registerDto)).ReturnsAsync(expectedToken);

        // Act
        var result = await _controller.RegisterPatient(registerDto);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var createdResult = (CreatedAtActionResult)result;
        createdResult.Value.Should().BeEquivalentTo(new { message = "Registration successful", token = expectedToken });
        _mockAuthService.Verify(s => s.RegisterPatientAsync(registerDto), Times.Once);
    }

    [Fact]
    public async Task RegisterPatient_WhenEmailAlreadyExists_ShouldReturn500WithCustomException()
    {
        // Arrange
        var registerDto = new RegisterPatientDto
        {
            Email = "existing@test.com",
            Password = "Test@123",
            ConfirmPassword = "Test@123",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "+1234567890",
            DateOfBirth = DateTime.Now.AddYears(-30),
            Gender = Gender.Male,
            Height = 180,
            Weight = 75,
            Address = new CreateAddressRequestDto
            {
                Street = "Main St",
                StreetNr = 123,
                City = "City",
                State = "State",
                Country = "Country",
                PostalCode = "12345"
            }
        };

        _mockAuthService.Setup(s => s.RegisterPatientAsync(registerDto))
            .ThrowsAsync(new CustomException("An account with email 'existing@test.com' already exists."));

        // Act
        var result = await _controller.RegisterPatient(registerDto);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result;
        objectResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task RegisterPatient_WhenUnexpectedError_ShouldReturn500()
    {
        // Arrange
        var registerDto = new RegisterPatientDto
        {
            Email = "test@test.com",
            Password = "Test@123",
            ConfirmPassword = "Test@123",
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "+1234567890",
            DateOfBirth = DateTime.Now.AddYears(-30),
            Gender = Gender.Male,
            Height = 180,
            Weight = 75,
            Address = new CreateAddressRequestDto
            {
                Street = "Main St",
                StreetNr = 123,
                City = "City",
                State = "State",
                Country = "Country",
                PostalCode = "12345"
            }
        };

        _mockAuthService.Setup(s => s.RegisterPatientAsync(registerDto))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.RegisterPatient(registerDto);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().BeEquivalentTo(new { error = "An unexpected error occurred." });
    }

    #endregion

    #region RegisterPatientByAdmin Tests

    [Fact]
    public async Task RegisterPatientByAdmin_WithValidData_ShouldReturnOkWithMessage()
    {
        // Arrange
        var registerDto = new RegisterPatientByAdminDto
        {
            Email = "admin-patient@test.com",
            Password = "Test@123",
            FirstName = "Jane",
            LastName = "Doe",
            PhoneNumber = "+1234567890",
            DateOfBirth = DateTime.Now.AddYears(-25),
            Gender = Gender.Female,
            Height = 165,
            Weight = 60,
            Address = new CreateAddressRequestDto
            {
                Street = "Admin St",
                StreetNr = 456,
                City = "City",
                State = "State",
                Country = "Country",
                PostalCode = "54321"
            }
        };

        var expectedMessage = "Patient account created successfully.";
        _mockAuthService.Setup(s => s.RegisterPatientByAdminAsync(registerDto)).ReturnsAsync(expectedMessage);

        // Act
        var result = await _controller.RegisterPatientByAdmin(registerDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().BeEquivalentTo(new { message = expectedMessage });
        _mockAuthService.Verify(s => s.RegisterPatientByAdminAsync(registerDto), Times.Once);
    }

    [Fact]
    public async Task RegisterPatientByAdmin_WhenServiceThrows_ShouldReturn500()
    {
        // Arrange
        var registerDto = new RegisterPatientByAdminDto
        {
            Email = "error@test.com",
            Password = "Test@123",
            FirstName = "Error",
            LastName = "Test",
            PhoneNumber = "+1234567890",
            DateOfBirth = DateTime.Now.AddYears(-25),
            Gender = Gender.Male,
            Height = 175,
            Weight = 70,
            Address = new CreateAddressRequestDto
            {
                Street = "Error St",
                StreetNr = 999,
                City = "City",
                State = "State",
                Country = "Country",
                PostalCode = "99999"
            }
        };

        _mockAuthService.Setup(s => s.RegisterPatientByAdminAsync(registerDto))
            .ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _controller.RegisterPatientByAdmin(registerDto);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result;
        objectResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region RegisterDoctorByAdmin Tests

    [Fact]
    public async Task RegisterDoctorByAdmin_WithValidData_ShouldReturnOkWithMessage()
    {
        // Arrange
        var registerDto = new RegisterDoctorByAdminDto
        {
            Email = "doctor@test.com",
            Password = "Test@123",
            FirstName = "Dr. John",
            LastName = "Smith",
            PhoneNumber = "+1234567890",
            DateOfBirth = DateTime.Now.AddYears(-40),
            Gender = Gender.Male,
            LicenseNumber = "DOC12345",
            YearsOfExperience = 15,
            Qualifications = new List<CreateQualificationDto>
            {
                new() { Degree = "MD", Institution = "Harvard", StudiedYears = 6, YearOfCompletion = 2005 }
            },
            SpecializationIds = new List<Guid> { Guid.NewGuid() },
            Address = new CreateAddressRequestDto
            {
                Street = "Hospital St",
                StreetNr = 789,
                City = "City",
                State = "State",
                Country = "Country",
                PostalCode = "11111"
            }
        };

        var expectedMessage = "Doctor account created successfully.";
        _mockAuthService.Setup(s => s.RegisterDoctorByAdminAsync(registerDto)).ReturnsAsync(expectedMessage);

        // Act
        var result = await _controller.RegisterDoctorByAdmin(registerDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().BeEquivalentTo(new { message = expectedMessage });
        _mockAuthService.Verify(s => s.RegisterDoctorByAdminAsync(registerDto), Times.Once);
    }

    [Fact]
    public async Task RegisterDoctorByAdmin_WhenServiceThrows_ShouldReturn500()
    {
        // Arrange
        var registerDto = new RegisterDoctorByAdminDto
        {
            Email = "doctor-error@test.com",
            Password = "Test@123",
            FirstName = "Dr. Error",
            LastName = "Test",
            PhoneNumber = "+1234567890",
            DateOfBirth = DateTime.Now.AddYears(-40),
            Gender = Gender.Male,
            LicenseNumber = "ERR12345",
            YearsOfExperience = 10,
            Qualifications = new List<CreateQualificationDto>
            {
                new() { Degree = "MD", Institution = "Test", StudiedYears = 6, YearOfCompletion = 2010 }
            },
            SpecializationIds = new List<Guid> { Guid.NewGuid() },
            Address = new CreateAddressRequestDto
            {
                Street = "Error St",
                StreetNr = 999,
                City = "City",
                State = "State",
                Country = "Country",
                PostalCode = "99999"
            }
        };

        _mockAuthService.Setup(s => s.RegisterDoctorByAdminAsync(registerDto))
            .ThrowsAsync(new CustomException("Registration failed"));

        // Act
        var result = await _controller.RegisterDoctorByAdmin(registerDto);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result;
        objectResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region RegisterHospitalByAdmin Tests

    [Fact]
    public async Task RegisterHospitalByAdmin_WithValidData_ShouldReturnOkWithMessage()
    {
        // Arrange
        var registerDto = new RegisterHospitalByAdminDto
        {
            Name = "Test Hospital",
            Email = "hospital@test.com",
            Password = "Test@123",
            PhoneNumber = "+1234567890",
            Address = new CreateAddressRequestDto
            {
                Street = "Hospital Ave",
                StreetNr = 100,
                City = "City",
                State = "State",
                Country = "Country",
                PostalCode = "22222"
            }
        };

        var expectedMessage = "Hospital account created successfully.";
        _mockAuthService.Setup(s => s.RegisterHospitalByAdminAsync(registerDto)).ReturnsAsync(expectedMessage);

        // Act
        var result = await _controller.RegisterHospitalByAdmin(registerDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().BeEquivalentTo(new { message = expectedMessage });
        _mockAuthService.Verify(s => s.RegisterHospitalByAdminAsync(registerDto), Times.Once);
    }

    [Fact]
    public async Task RegisterHospitalByAdmin_WhenServiceThrows_ShouldReturn500()
    {
        // Arrange
        var registerDto = new RegisterHospitalByAdminDto
        {
            Name = "Error Hospital",
            Email = "hospital-error@test.com",
            Password = "Test@123",
            PhoneNumber = "+1234567890",
            Address = new CreateAddressRequestDto
            {
                Street = "Error Ave",
                StreetNr = 999,
                City = "City",
                State = "State",
                Country = "Country",
                PostalCode = "99999"
            }
        };

        _mockAuthService.Setup(s => s.RegisterHospitalByAdminAsync(registerDto))
            .ThrowsAsync(new Exception("Hospital creation failed"));

        // Act
        var result = await _controller.RegisterHospitalByAdmin(registerDto);

        // Assert
        result.Should().BeOfType<ObjectResult>();
        var objectResult = (ObjectResult)result;
        objectResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnOkWithToken()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "user@test.com",
            Password = "Test@123"
        };

        var expectedToken = "valid-jwt-token";
        _mockAuthService.Setup(s => s.LoginAsync(loginDto)).ReturnsAsync(expectedToken);

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = (OkObjectResult)result;
        okResult.Value.Should().BeEquivalentTo(new { token = expectedToken });
        _mockAuthService.Verify(s => s.LoginAsync(loginDto), Times.Once);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "invalid@test.com",
            Password = "WrongPassword"
        };

        _mockAuthService.Setup(s => s.LoginAsync(loginDto))
            .ThrowsAsync(new Exception("Invalid credentials"));

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
        var unauthorizedResult = (UnauthorizedObjectResult)result;
        unauthorizedResult.Value.Should().BeEquivalentTo(new { message = "Invalid credentials." });
    }

    [Fact]
    public async Task Login_WhenAccountLocked_ShouldReturnUnauthorized()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "locked@test.com",
            Password = "Test@123"
        };

        _mockAuthService.Setup(s => s.LoginAsync(loginDto))
            .ThrowsAsync(new CustomException("Account is locked"));

        // Act
        var result = await _controller.Login(loginDto);

        // Assert
        result.Should().BeOfType<UnauthorizedObjectResult>();
    }

    #endregion
}
