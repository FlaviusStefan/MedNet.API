using FluentAssertions;
using MedNet.API.Exceptions;
using MedNet.API.Models.DTO;
using MedNet.API.Models.DTO.Auth;
using MedNet.API.Models.Enums;
using MedNet.API.Services.Implementation;
using MedNet.API.Services.Interface;
using MedNet.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.IdentityModel.Tokens.Jwt;

namespace MedNet.API.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
    private readonly Mock<SignInManager<IdentityUser>> _mockSignInManager;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IPatientService> _mockPatientService;
    private readonly Mock<IDoctorService> _mockDoctorService;
    private readonly Mock<IHospitalService> _mockHospitalService;
    private readonly Mock<ILogger<AuthService>> _mockLogger;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        // Mock UserManager (requires complex setup)
        var userStoreMock = new Mock<IUserStore<IdentityUser>>();
        _mockUserManager = new Mock<UserManager<IdentityUser>>(
            userStoreMock.Object, null, null, null, null, null, null, null, null);

        // Mock SignInManager
        var contextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var userPrincipalFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
        _mockSignInManager = new Mock<SignInManager<IdentityUser>>(
            _mockUserManager.Object,
            contextAccessor.Object,
            userPrincipalFactory.Object,
            null, null, null, null);

        _mockConfiguration = new Mock<IConfiguration>();
        _mockPatientService = new Mock<IPatientService>();
        _mockDoctorService = new Mock<IDoctorService>();
        _mockHospitalService = new Mock<IHospitalService>();
        _mockLogger = new Mock<ILogger<AuthService>>();

        // Setup configuration for JWT
        _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("this-is-a-very-long-secret-key-for-jwt-token-generation-minimum-32-characters");
        _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
        _mockConfiguration.Setup(c => c["Jwt:ExpiryMinutes"]).Returns("60");

        _service = new AuthService(
            _mockUserManager.Object,
            _mockSignInManager.Object,
            _mockConfiguration.Object,
            _mockPatientService.Object,
            _mockDoctorService.Object,
            _mockHospitalService.Object,
            _mockLogger.Object);
    }

    #region RegisterPatientAsync Tests

    [Fact]
    public async Task RegisterPatientAsync_WithValidData_ShouldReturnJwtToken()
    {
        // Arrange
        var registerDto = new RegisterPatientDto
        {
            Email = "newpatient@test.com",
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

        var createdPatient = new CreatedPatientDto
        {
            Id = Guid.NewGuid(),
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            UserId = "user-id-123",
            DateOfBirth = registerDto.DateOfBirth,
            Gender = registerDto.Gender,
            Height = registerDto.Height,
            Weight = registerDto.Weight,
            Address = new AddressDto { Id = Guid.NewGuid(), Street = "Main St", StreetNr = 123, City = "City", State = "State", Country = "Country", PostalCode = "12345" },
            Contact = new ContactDto { Id = Guid.NewGuid(), Email = registerDto.Email, Phone = registerDto.PhoneNumber }
        };

        _mockUserManager.Setup(um => um.FindByEmailAsync(registerDto.Email))
            .ReturnsAsync((IdentityUser)null);

        _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), registerDto.Password))
            .ReturnsAsync(IdentityResult.Success)
            .Callback<IdentityUser, string>((user, pwd) => user.Id = "user-id-123");

        _mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<IdentityUser>(), "Patient"))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager.Setup(um => um.GetRolesAsync(It.IsAny<IdentityUser>()))
            .ReturnsAsync(new List<string> { "Patient" });

        _mockPatientService.Setup(ps => ps.CreatePatientAsync(It.IsAny<CreatePatientRequestDto>()))
            .ReturnsAsync(createdPatient);

        // Act
        var result = await _service.RegisterPatientAsync(registerDto);

        // Assert
        result.Should().NotBeNullOrEmpty();
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result);
        token.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == "Patient");

        _mockUserManager.Verify(um => um.CreateAsync(It.IsAny<IdentityUser>(), registerDto.Password), Times.Once);
        _mockUserManager.Verify(um => um.AddToRoleAsync(It.IsAny<IdentityUser>(), "Patient"), Times.Once);
        _mockPatientService.Verify(ps => ps.CreatePatientAsync(It.IsAny<CreatePatientRequestDto>()), Times.Once);
    }

    [Fact]
    public async Task RegisterPatientAsync_WhenEmailExists_ShouldThrowCustomException()
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

        var existingUser = new IdentityUser { Email = registerDto.Email };
        _mockUserManager.Setup(um => um.FindByEmailAsync(registerDto.Email))
            .ReturnsAsync(existingUser);

        // Act
        Func<Task> act = async () => await _service.RegisterPatientAsync(registerDto);

        // Assert
        await act.Should().ThrowAsync<CustomException>()
            .WithMessage($"An account with email '{registerDto.Email}' already exists.");

        _mockUserManager.Verify(um => um.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task RegisterPatientAsync_WhenUserCreationFails_ShouldThrowCustomException()
    {
        // Arrange
        var registerDto = new RegisterPatientDto
        {
            Email = "newpatient@test.com",
            Password = "weak",
            ConfirmPassword = "weak",
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

        _mockUserManager.Setup(um => um.FindByEmailAsync(registerDto.Email))
            .ReturnsAsync((IdentityUser)null);

        var errors = new[]
        {
            new IdentityError { Description = "Password too weak" }
        };
        _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), registerDto.Password))
            .ReturnsAsync(IdentityResult.Failed(errors));

        // Act
        Func<Task> act = async () => await _service.RegisterPatientAsync(registerDto);

        // Assert
        await act.Should().ThrowAsync<CustomException>()
            .WithMessage("An error occurred while registering the patient: Password too weak");
    }

    #endregion

    #region RegisterPatientByAdminAsync Tests

    [Fact]
    public async Task RegisterPatientByAdminAsync_WithValidData_ShouldReturnSuccessMessage()
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

        var createdPatient = new CreatedPatientDto
        {
            Id = Guid.NewGuid(),
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            UserId = "user-id-456",
            DateOfBirth = registerDto.DateOfBirth,
            Gender = registerDto.Gender,
            Height = registerDto.Height,
            Weight = registerDto.Weight,
            Address = new AddressDto { Id = Guid.NewGuid(), Street = "Admin St", StreetNr = 456, City = "City", State = "State", Country = "Country", PostalCode = "54321" },
            Contact = new ContactDto { Id = Guid.NewGuid(), Email = registerDto.Email, Phone = registerDto.PhoneNumber }
        };

        _mockUserManager.Setup(um => um.FindByEmailAsync(registerDto.Email))
            .ReturnsAsync((IdentityUser)null);

        _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), registerDto.Password))
            .ReturnsAsync(IdentityResult.Success)
            .Callback<IdentityUser, string>((user, pwd) => user.Id = "user-id-456");

        _mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<IdentityUser>(), "Patient"))
            .ReturnsAsync(IdentityResult.Success);

        _mockPatientService.Setup(ps => ps.CreatePatientAsync(It.IsAny<CreatePatientRequestDto>()))
            .ReturnsAsync(createdPatient);

        // Act
        var result = await _service.RegisterPatientByAdminAsync(registerDto);

        // Assert
        result.Should().Be("Patient account created successfully.");
        _mockUserManager.Verify(um => um.CreateAsync(It.IsAny<IdentityUser>(), registerDto.Password), Times.Once);
        _mockPatientService.Verify(ps => ps.CreatePatientAsync(It.IsAny<CreatePatientRequestDto>()), Times.Once);
    }

    [Fact]
    public async Task RegisterPatientByAdminAsync_WhenEmailExists_ShouldThrowCustomException()
    {
        // Arrange
        var registerDto = new RegisterPatientByAdminDto
        {
            Email = "existing@test.com",
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

        var existingUser = new IdentityUser { Email = registerDto.Email };
        _mockUserManager.Setup(um => um.FindByEmailAsync(registerDto.Email))
            .ReturnsAsync(existingUser);

        // Act
        Func<Task> act = async () => await _service.RegisterPatientByAdminAsync(registerDto);

        // Assert
        await act.Should().ThrowAsync<CustomException>()
            .WithMessage($"An account with email '{registerDto.Email}' already exists.");
    }

    #endregion

    #region LoginAsync Tests

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnJwtToken()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "user@test.com",
            Password = "Test@123"
        };

        var user = new IdentityUser
        {
            Id = "user-123",
            Email = loginDto.Email,
            UserName = loginDto.Email
        };

        _mockSignInManager.Setup(sm => sm.PasswordSignInAsync(
                loginDto.Email,
                loginDto.Password,
                false,
                true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        _mockUserManager.Setup(um => um.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        _mockUserManager.Setup(um => um.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "Patient" });

        // Act
        var result = await _service.LoginAsync(loginDto);

        // Assert
        result.Should().NotBeNullOrEmpty();
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(result);
        token.Claims.Should().Contain(c => c.Type == System.Security.Claims.ClaimTypes.Role && c.Value == "Patient");

        _mockSignInManager.Verify(sm => sm.PasswordSignInAsync(loginDto.Email, loginDto.Password, false, true), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenAccountIsLockedOut_ShouldThrowCustomException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "locked@test.com",
            Password = "Test@123"
        };

        _mockSignInManager.Setup(sm => sm.PasswordSignInAsync(
                loginDto.Email,
                loginDto.Password,
                false,
                true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);

        // Act
        Func<Task> act = async () => await _service.LoginAsync(loginDto);

        // Assert
        await act.Should().ThrowAsync<CustomException>()
            .WithMessage("Account is locked due to multiple failed login attempts. Please try again later.");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidCredentials_ShouldThrowException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "user@test.com",
            Password = "WrongPassword"
        };

        _mockSignInManager.Setup(sm => sm.PasswordSignInAsync(
                loginDto.Email,
                loginDto.Password,
                false,
                true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

        // Act
        Func<Task> act = async () => await _service.LoginAsync(loginDto);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("Invalid login attempt.");
    }

    [Fact]
    public async Task LoginAsync_WhenUserNotFoundAfterSignIn_ShouldThrowException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "ghost@test.com",
            Password = "Test@123"
        };

        _mockSignInManager.Setup(sm => sm.PasswordSignInAsync(
                loginDto.Email,
                loginDto.Password,
                false,
                true))
            .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

        _mockUserManager.Setup(um => um.FindByEmailAsync(loginDto.Email))
            .ReturnsAsync((IdentityUser)null);

        // Act
        Func<Task> act = async () => await _service.LoginAsync(loginDto);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("User not found.");
    }

    #endregion

    #region Logging Tests

    [Fact]
    public async Task RegisterPatientAsync_ShouldLogInformation()
    {
        // Arrange
        var registerDto = new RegisterPatientDto
        {
            Email = "log-test@test.com",
            Password = "Test@123",
            ConfirmPassword = "Test@123",
            FirstName = "Log",
            LastName = "Test",
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

        _mockUserManager.Setup(um => um.FindByEmailAsync(registerDto.Email))
            .ReturnsAsync((IdentityUser)null);

        _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<IdentityUser>(), registerDto.Password))
            .ReturnsAsync(IdentityResult.Success)
            .Callback<IdentityUser, string>((user, pwd) => user.Id = "user-id");

        _mockUserManager.Setup(um => um.AddToRoleAsync(It.IsAny<IdentityUser>(), "Patient"))
            .ReturnsAsync(IdentityResult.Success);

        _mockUserManager.Setup(um => um.GetRolesAsync(It.IsAny<IdentityUser>()))
            .ReturnsAsync(new List<string> { "Patient" });

        _mockPatientService.Setup(ps => ps.CreatePatientAsync(It.IsAny<CreatePatientRequestDto>()))
            .ReturnsAsync(new CreatedPatientDto
            {
                Id = Guid.NewGuid(),
                UserId = "user-id",
                FirstName = "Log",
                LastName = "Test",
                DateOfBirth = registerDto.DateOfBirth,
                Gender = registerDto.Gender,
                Height = registerDto.Height,
                Weight = registerDto.Weight,
                Address = new AddressDto { Id = Guid.NewGuid(), Street = "Main St", StreetNr = 123, City = "City", State = "State", Country = "Country", PostalCode = "12345" },
                Contact = new ContactDto { Id = Guid.NewGuid(), Email = registerDto.Email, Phone = registerDto.PhoneNumber }
            });

        // Act
        await _service.RegisterPatientAsync(registerDto);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Patient self-registration attempt")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.AtLeastOnce);
    }

    #endregion
}
