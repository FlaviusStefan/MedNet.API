using FluentAssertions;
using MedNet.API.Controllers;
using MedNet.API.Models.DTO;
using MedNet.API.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace MedNet.API.Tests.Controllers;

public class SpecializationControllerTests
{
    private readonly Mock<ISpecializationService> _mockService;
    private readonly Mock<ILogger<SpecializationsController>> _mockLogger;
    private readonly SpecializationsController _controller;

    public SpecializationControllerTests()
    {
        _mockService = new Mock<ISpecializationService>();
        _mockLogger = new Mock<ILogger<SpecializationsController>>();
        _controller = new SpecializationsController(_mockService.Object, _mockLogger.Object);

        // Provide a ClaimsPrincipal so controller.User is not null in tests
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

    #region Create Tests

    [Fact]
    public async Task CreateSpecialization_ShouldReturnCreatedAtAction_WithDto()
    {
        // Arrange
        var request = new CreateSpecializationRequestDto { Name = "Cardiology", Description = "Heart" };
        var dto = new SpecializationDto { Id = Guid.NewGuid(), Name = "Cardiology", Description = "Heart" };

        _mockService.Setup(s => s.CreateSpecializationAsync(It.Is<CreateSpecializationRequestDto>(r => r.Name == request.Name && r.Description == request.Description)))
                    .ReturnsAsync(dto);

        // Act
        var result = await _controller.CreateSpecialization(request);

        // Assert
        result.Should().BeOfType<CreatedAtActionResult>();
        var created = (CreatedAtActionResult)result;
        created.Value.Should().BeEquivalentTo(dto);
        _mockService.Verify(s => s.CreateSpecializationAsync(It.IsAny<CreateSpecializationRequestDto>()), Times.Once);
    }

    #endregion

    #region GetAll Tests

    [Fact]
    public async Task GetAllSpecializations_WhenExist_ShouldReturnOkWithList()
    {
        // Arrange
        var list = new List<SpecializationDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Cardiology", Description = "Heart" },
            new() { Id = Guid.NewGuid(), Name = "Neurology", Description = "Brain" }
        };

        _mockService.Setup(s => s.GetAllSpecializationsAsync()).ReturnsAsync(list);

        // Act
        var result = await _controller.GetAllSpecializations();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var ok = (OkObjectResult)result;
        ok.Value.Should().BeEquivalentTo(list);
        _mockService.Verify(s => s.GetAllSpecializationsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllSpecializations_WhenEmpty_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        _mockService.Setup(s => s.GetAllSpecializationsAsync()).ReturnsAsync(new List<SpecializationDto>());

        // Act
        var result = await _controller.GetAllSpecializations();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var ok = (OkObjectResult)result;
        ((IEnumerable<SpecializationDto>)ok.Value!).Should().BeEmpty();
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetSpecializationById_WhenFound_ShouldReturnOkWithDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var dto = new SpecializationDto { Id = id, Name = "Dermatology", Description = "Skin" };

        _mockService.Setup(s => s.GetSpecializationByIdAsync(id)).ReturnsAsync(dto);

        // Act
        var result = await _controller.GetSpecializationById(id);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var ok = (OkObjectResult)result;
        ok.Value.Should().BeEquivalentTo(dto);
        _mockService.Verify(s => s.GetSpecializationByIdAsync(id), Times.Once);
    }

    [Fact]
    public async Task GetSpecializationById_WhenNotFound_ShouldReturnNotFound_AndLogWarning()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.GetSpecializationByIdAsync(id)).ReturnsAsync((SpecializationDto?)null);

        // Act
        var result = await _controller.GetSpecializationById(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("not found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    #endregion

    #region GetByDoctorId Tests

    [Fact]
    public async Task GetSpecializationsByDoctorId_WhenExist_ShouldReturnOkWithList()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var list = new List<SpecializationDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Cardiology", Description = "Heart" }
        };

        _mockService.Setup(s => s.GetSpecializationsByDoctorIdAsync(doctorId)).ReturnsAsync(list);

        // Act
        var result = await _controller.GetSpecializationsByDoctorId(doctorId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var ok = (OkObjectResult)result;
        ok.Value.Should().BeEquivalentTo(list);
        _mockService.Verify(s => s.GetSpecializationsByDoctorIdAsync(doctorId), Times.Once);
    }

    [Fact]
    public async Task GetSpecializationsByDoctorId_WhenNone_ShouldReturnOkWithEmptyList()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        _mockService.Setup(s => s.GetSpecializationsByDoctorIdAsync(doctorId)).ReturnsAsync(new List<SpecializationDto>());

        // Act
        var result = await _controller.GetSpecializationsByDoctorId(doctorId);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var ok = (OkObjectResult)result;
        ((IEnumerable<SpecializationDto>)ok.Value!).Should().BeEmpty();
    }

    #endregion

    #region Update Tests

    [Fact]
    public async Task UpdateSpecialization_WhenExists_ShouldReturnOkWithDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateSpecializationRequestDto { Name = "Advanced Cardiology", Description = "Advanced" };
        var updated = new SpecializationDto { Id = id, Name = request.Name, Description = request.Description };

        _mockService.Setup(s => s.UpdateSpecializationAsync(id, request)).ReturnsAsync(updated);

        // Act
        var result = await _controller.UpdateSpecialization(id, request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var ok = (OkObjectResult)result;
        ok.Value.Should().BeEquivalentTo(updated);
        _mockService.Verify(s => s.UpdateSpecializationAsync(id, request), Times.Once);
    }

    [Fact]
    public async Task UpdateSpecialization_WhenNotFound_ShouldReturnNotFound_AndLogWarning()
    {
        // Arrange
        var id = Guid.NewGuid();
        var request = new UpdateSpecializationRequestDto { Name = "X", Description = "Y" };
        _mockService.Setup(s => s.UpdateSpecializationAsync(id, request)).ReturnsAsync((SpecializationDto?)null);

        // Act
        var result = await _controller.UpdateSpecialization(id, request);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("not found for update")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    #endregion

    #region Delete Tests

    [Fact]
    public async Task DeleteSpecialization_WhenExists_ShouldReturnOkWithMessage()
    {
        // Arrange
        var id = Guid.NewGuid();
        var serviceMessage = $"Specialization 'Oncology' (ID: {id}) deleted successfully!";
        _mockService.Setup(s => s.DeleteSpecializationAsync(id)).ReturnsAsync(serviceMessage);

        // Act
        var result = await _controller.DeleteSpecialization(id);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var ok = (OkObjectResult)result;
        ok.Value.Should().BeEquivalentTo(new { message = serviceMessage });
        _mockService.Verify(s => s.DeleteSpecializationAsync(id), Times.Once);
    }

    [Fact]
    public async Task DeleteSpecialization_WhenNotFound_ShouldReturnNotFound_AndLogWarning()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockService.Setup(s => s.DeleteSpecializationAsync(id)).ReturnsAsync((string?)null);

        // Act
        var result = await _controller.DeleteSpecialization(id);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("not found for deletion")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    #endregion
}
