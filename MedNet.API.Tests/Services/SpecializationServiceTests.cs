using FluentAssertions;
using MedNet.API.Models.Domain;
using MedNet.API.Models.DTO;
using MedNet.API.Repositories.Interface;
using MedNet.API.Services.Implementation;
using Microsoft.Extensions.Logging;
using Moq;

namespace MedNet.API.Tests.Services;

public class SpecializationServiceTests
{
    private readonly Mock<ISpecializationRepository> _mockRepository;
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<SpecializationService>> _mockLogger;
    private readonly SpecializationService _service;

    public SpecializationServiceTests()
    {
        _mockRepository = new Mock<ISpecializationRepository>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILogger<SpecializationService>>();
        _service = new SpecializationService(_mockRepository.Object, _mockLogger.Object, _mockUnitOfWork.Object);
    }

    #region CreateSpecializationAsync Tests

    [Fact]
    public async Task CreateSpecializationAsync_WithValidRequest_ShouldCreateAndReturnSpecializationDto()
    {
        // Arrange
        var request = new CreateSpecializationRequestDto
        {
            Name = "Cardiology",
            Description = "Heart and cardiovascular system"
        };

        _mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<Specialization>()))
            .ReturnsAsync((Specialization s) => s);

        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateSpecializationAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Cardiology");
        result.Description.Should().Be("Heart and cardiovascular system");
        result.Id.Should().NotBeEmpty();

        _mockRepository.Verify(r => r.CreateAsync(It.Is<Specialization>(s =>
            s.Name == "Cardiology" &&
            s.Description == "Heart and cardiovascular system"
        )), Times.Once);

        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateSpecializationAsync_ShouldGenerateNewGuid()
    {
        // Arrange
        var request = new CreateSpecializationRequestDto
        {
            Name = "Neurology",
            Description = "Nervous system"
        };

        _mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<Specialization>()))
            .ReturnsAsync((Specialization s) => s);

        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.CreateSpecializationAsync(request);

        // Assert
        result.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task CreateSpecializationAsync_ShouldLogInformation()
    {
        // Arrange
        var request = new CreateSpecializationRequestDto
        {
            Name = "Orthopedics",
            Description = "Bones and joints"
        };

        _mockRepository
            .Setup(r => r.CreateAsync(It.IsAny<Specialization>()))
            .ReturnsAsync((Specialization s) => s);

        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _service.CreateSpecializationAsync(request);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Creating specialization")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    #endregion

    #region GetAllSpecializationsAsync Tests

    [Fact]
    public async Task GetAllSpecializationsAsync_WhenSpecializationsExist_ShouldReturnAllSpecializationDtos()
    {
        // Arrange
        var specializations = new List<Specialization>
        {
            new() { Id = Guid.NewGuid(), Name = "Cardiology", Description = "Heart" },
            new() { Id = Guid.NewGuid(), Name = "Neurology", Description = "Brain" },
            new() { Id = Guid.NewGuid(), Name = "Pediatrics", Description = "Children" }
        };

        _mockRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(specializations);

        // Act
        var result = await _service.GetAllSpecializationsAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(s => s.Name == "Cardiology");
        result.Should().Contain(s => s.Name == "Neurology");
        result.Should().Contain(s => s.Name == "Pediatrics");

        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllSpecializationsAsync_WhenNoSpecializations_ShouldReturnEmptyList()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Specialization>());

        // Act
        var result = await _service.GetAllSpecializationsAsync();

        // Assert
        result.Should().BeEmpty();
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllSpecializationsAsync_ShouldLogInformation()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Specialization>());

        // Act
        await _service.GetAllSpecializationsAsync();

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Retrieving all specializations")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region GetSpecializationByIdAsync Tests

    [Fact]
    public async Task GetSpecializationByIdAsync_WhenSpecializationExists_ShouldReturnSpecializationDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var specialization = new Specialization
        {
            Id = id,
            Name = "Dermatology",
            Description = "Skin conditions"
        };

        _mockRepository
            .Setup(r => r.GetById(id))
            .ReturnsAsync(specialization);

        // Act
        var result = await _service.GetSpecializationByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Name.Should().Be("Dermatology");
        result.Description.Should().Be("Skin conditions");

        _mockRepository.Verify(r => r.GetById(id), Times.Once);
    }

    [Fact]
    public async Task GetSpecializationByIdAsync_WhenSpecializationDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.GetById(id))
            .ReturnsAsync((Specialization?)null);

        // Act
        var result = await _service.GetSpecializationByIdAsync(id);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetById(id), Times.Once);
    }

    [Fact]
    public async Task GetSpecializationByIdAsync_WhenNotFound_ShouldLogWarning()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.GetById(id))
            .ReturnsAsync((Specialization?)null);

        // Act
        await _service.GetSpecializationByIdAsync(id);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Specialization not found")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region GetSpecializationsByDoctorIdAsync Tests

    [Fact]
    public async Task GetSpecializationsByDoctorIdAsync_WhenDoctorHasSpecializations_ShouldReturnSpecializationDtos()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var specializations = new List<Specialization>
        {
            new() { Id = Guid.NewGuid(), Name = "Cardiology", Description = "Heart" },
            new() { Id = Guid.NewGuid(), Name = "Neurology", Description = "Brain" }
        };

        _mockRepository
            .Setup(r => r.GetAllByDoctorIdAsync(doctorId))
            .ReturnsAsync(specializations);

        // Act
        var result = await _service.GetSpecializationsByDoctorIdAsync(doctorId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(s => s.Name == "Cardiology");
        result.Should().Contain(s => s.Name == "Neurology");

        _mockRepository.Verify(r => r.GetAllByDoctorIdAsync(doctorId), Times.Once);
    }

    [Fact]
    public async Task GetSpecializationsByDoctorIdAsync_WhenDoctorHasNoSpecializations_ShouldReturnEmptyList()
    {
        // Arrange
        var doctorId = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.GetAllByDoctorIdAsync(doctorId))
            .ReturnsAsync(new List<Specialization>());

        // Act
        var result = await _service.GetSpecializationsByDoctorIdAsync(doctorId);

        // Assert
        result.Should().BeEmpty();
        _mockRepository.Verify(r => r.GetAllByDoctorIdAsync(doctorId), Times.Once);
    }

    #endregion

    #region UpdateSpecializationAsync Tests

    [Fact]
    public async Task UpdateSpecializationAsync_WhenSpecializationExists_ShouldUpdateAndReturnDto()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingSpecialization = new Specialization
        {
            Id = id,
            Name = "Cardiology",
            Description = "Heart conditions"
        };

        var updateRequest = new UpdateSpecializationRequestDto
        {
            Name = "Advanced Cardiology",
            Description = "Advanced heart conditions"
        };

        _mockRepository
            .Setup(r => r.GetById(id))
            .ReturnsAsync(existingSpecialization);

        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Specialization>()))
            .ReturnsAsync((Specialization s) => s);

        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.UpdateSpecializationAsync(id, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Name.Should().Be("Advanced Cardiology");
        result.Description.Should().Be("Advanced heart conditions");

        _mockRepository.Verify(r => r.GetById(id), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<Specialization>(s =>
            s.Id == id &&
            s.Name == "Advanced Cardiology" &&
            s.Description == "Advanced heart conditions"
        )), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateSpecializationAsync_WhenSpecializationDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateRequest = new UpdateSpecializationRequestDto
        {
            Name = "Non-existent",
            Description = "Does not exist"
        };

        _mockRepository
            .Setup(r => r.GetById(id))
            .ReturnsAsync((Specialization?)null);

        // Act
        var result = await _service.UpdateSpecializationAsync(id, updateRequest);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetById(id), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Specialization>()), Times.Never);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateSpecializationAsync_WhenUpdateFails_ShouldReturnNull()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingSpecialization = new Specialization
        {
            Id = id,
            Name = "Cardiology",
            Description = "Heart conditions"
        };

        var updateRequest = new UpdateSpecializationRequestDto
        {
            Name = "Advanced Cardiology",
            Description = "Advanced heart conditions"
        };

        _mockRepository
            .Setup(r => r.GetById(id))
            .ReturnsAsync(existingSpecialization);

        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<Specialization>()))
            .ReturnsAsync((Specialization?)null);

        // Act
        var result = await _service.UpdateSpecializationAsync(id, updateRequest);

        // Assert
        result.Should().BeNull();
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task UpdateSpecializationAsync_WhenNotFound_ShouldLogWarning()
    {
        // Arrange
        var id = Guid.NewGuid();
        var updateRequest = new UpdateSpecializationRequestDto
        {
            Name = "Test",
            Description = "Test"
        };

        _mockRepository
            .Setup(r => r.GetById(id))
            .ReturnsAsync((Specialization?)null);

        // Act
        await _service.UpdateSpecializationAsync(id, updateRequest);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Specialization not found for update")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region DeleteSpecializationAsync Tests

    [Fact]
    public async Task DeleteSpecializationAsync_WhenSpecializationExists_ShouldDeleteAndReturnSuccessMessage()
    {
        // Arrange
        var id = Guid.NewGuid();
        var specialization = new Specialization
        {
            Id = id,
            Name = "Oncology",
            Description = "Cancer treatment"
        };

        _mockRepository
            .Setup(r => r.DeleteAsync(id))
            .ReturnsAsync(specialization);

        _mockUnitOfWork
            .Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _service.DeleteSpecializationAsync(id);

        // Assert
        result.Should().NotBeNull();
        result.Should().Contain("Oncology");
        result.Should().Contain(id.ToString());
        result.Should().Contain("deleted successfully");

        _mockRepository.Verify(r => r.DeleteAsync(id), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteSpecializationAsync_WhenSpecializationDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.DeleteAsync(id))
            .ReturnsAsync((Specialization?)null);

        // Act
        var result = await _service.DeleteSpecializationAsync(id);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.DeleteAsync(id), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteSpecializationAsync_WhenNotFound_ShouldLogWarning()
    {
        // Arrange
        var id = Guid.NewGuid();

        _mockRepository
            .Setup(r => r.DeleteAsync(id))
            .ReturnsAsync((Specialization?)null);

        // Act
        await _service.DeleteSpecializationAsync(id);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Specialization not found for deletion")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion

    #region ValidateSpecializationsAsync Tests

    [Fact]
    public async Task ValidateSpecializationsAsync_WhenAllIdsAreValid_ShouldReturnDictionary()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var specializationIds = new List<Guid> { id1, id2 };

        var specializations = new List<Specialization>
        {
            new() { Id = id1, Name = "Cardiology", Description = "Heart" },
            new() { Id = id2, Name = "Neurology", Description = "Brain" }
        };

        _mockRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(specializations);

        // Act
        var result = await _service.ValidateSpecializationsAsync(specializationIds);

        // Assert
        result.Should().HaveCount(2);
        result.Should().ContainKey(id1);
        result.Should().ContainKey(id2);
        result[id1].Should().Be("Cardiology");
        result[id2].Should().Be("Neurology");
    }

    [Fact]
    public async Task ValidateSpecializationsAsync_WhenSomeIdsAreInvalid_ShouldThrowArgumentException()
    {
        // Arrange
        var validId = Guid.NewGuid();
        var invalidId = Guid.NewGuid();
        var specializationIds = new List<Guid> { validId, invalidId };

        var specializations = new List<Specialization>
        {
            new() { Id = validId, Name = "Cardiology", Description = "Heart" }
        };

        _mockRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(specializations);

        // Act
        Func<Task> act = async () => await _service.ValidateSpecializationsAsync(specializationIds);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("One or more specialization IDs are invalid.");
    }

    [Fact]
    public async Task ValidateSpecializationsAsync_WhenAllIdsAreInvalid_ShouldThrowArgumentException()
    {
        // Arrange
        var invalidId1 = Guid.NewGuid();
        var invalidId2 = Guid.NewGuid();
        var specializationIds = new List<Guid> { invalidId1, invalidId2 };

        _mockRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Specialization>());

        // Act
        Func<Task> act = async () => await _service.ValidateSpecializationsAsync(specializationIds);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("One or more specialization IDs are invalid.");
    }

    [Fact]
    public async Task ValidateSpecializationsAsync_WhenInvalid_ShouldLogWarning()
    {
        // Arrange
        var invalidId = Guid.NewGuid();
        var specializationIds = new List<Guid> { invalidId };

        _mockRepository
            .Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Specialization>());

        // Act
        try
        {
            await _service.ValidateSpecializationsAsync(specializationIds);
        }
        catch (ArgumentException)
        {
            // Expected exception
        }

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Specialization validation failed")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    #endregion
}