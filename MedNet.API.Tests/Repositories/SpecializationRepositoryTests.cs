using FluentAssertions;
using MedNet.API.Data;
using MedNet.API.Models.Domain;
using MedNet.API.Repositories.Implementation;
using Microsoft.EntityFrameworkCore;

namespace MedNet.API.Tests.Repositories;

public class SpecializationRepositoryTests : IDisposable
{
    private readonly ApplicationDbContext _dbContext;
    private readonly SpecializationRepository _repository;

    public SpecializationRepositoryTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _repository = new SpecializationRepository(_dbContext);
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WithValidSpecialization_ShouldAddToDatabase()
    {
        // Arrange
        var specialization = new Specialization
        {
            Id = Guid.NewGuid(),
            Name = "Cardiology",
            Description = "Heart and cardiovascular system"
        };

        // Act
        var result = await _repository.CreateAsync(specialization);
        await _dbContext.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(specialization.Id);
        result.Name.Should().Be("Cardiology");

        var savedSpecialization = await _dbContext.Specializations.FindAsync(specialization.Id);
        savedSpecialization.Should().NotBeNull();
        savedSpecialization!.Name.Should().Be("Cardiology");
    }

    [Fact]
    public async Task CreateAsync_WithValidSpecialization_ShouldReturnSameInstance()
    {
        // Arrange
        var specialization = new Specialization
        {
            Id = Guid.NewGuid(),
            Name = "Neurology",
            Description = "Nervous system"
        };

        // Act
        var result = await _repository.CreateAsync(specialization);

        // Assert
        result.Should().BeSameAs(specialization);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WhenSpecializationsExist_ShouldReturnAllSpecializations()
    {
        // Arrange
        var specializations = new List<Specialization>
        {
            new() { Id = Guid.NewGuid(), Name = "Cardiology", Description = "Heart" },
            new() { Id = Guid.NewGuid(), Name = "Neurology", Description = "Brain" },
            new() { Id = Guid.NewGuid(), Name = "Pediatrics", Description = "Children" }
        };

        await _dbContext.Specializations.AddRangeAsync(specializations);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCount(3);
        result.Should().Contain(s => s.Name == "Cardiology");
        result.Should().Contain(s => s.Name == "Neurology");
        result.Should().Contain(s => s.Name == "Pediatrics");
    }

    [Fact]
    public async Task GetAllAsync_WhenNoSpecializations_ShouldReturnEmptyList()
    {
        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_ShouldUseAsNoTracking()
    {
        // Arrange
        var specialization = new Specialization
        {
            Id = Guid.NewGuid(),
            Name = "Orthopedics",
            Description = "Bones"
        };

        await _dbContext.Specializations.AddAsync(specialization);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();
        var firstItem = result.First();

        // Assert
        var entry = _dbContext.Entry(firstItem);
        entry.State.Should().Be(EntityState.Detached);
    }

    #endregion

    #region GetById Tests

    [Fact]
    public async Task GetById_WhenSpecializationExists_ShouldReturnSpecialization()
    {
        // Arrange
        var id = Guid.NewGuid();
        var specialization = new Specialization
        {
            Id = id,
            Name = "Dermatology",
            Description = "Skin conditions"
        };

        await _dbContext.Specializations.AddAsync(specialization);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetById(id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Name.Should().Be("Dermatology");
        result.Description.Should().Be("Skin conditions");
    }

    [Fact]
    public async Task GetById_WhenSpecializationDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.GetById(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetById_ShouldUseAsNoTracking()
    {
        // Arrange
        var id = Guid.NewGuid();
        var specialization = new Specialization
        {
            Id = id,
            Name = "Psychiatry",
            Description = "Mental health"
        };

        await _dbContext.Specializations.AddAsync(specialization);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetById(id);

        // Assert
        result.Should().NotBeNull();
        var entry = _dbContext.Entry(result!);
        entry.State.Should().Be(EntityState.Detached);
    }

    #endregion

    #region GetAllByDoctorIdAsync Tests

    [Fact]
    public async Task GetAllByDoctorIdAsync_WhenDoctorHasSpecializations_ShouldReturnSpecializations()
    {
        // Arrange
        var doctorId = Guid.NewGuid();
        var specialization1 = new Specialization
        {
            Id = Guid.NewGuid(),
            Name = "Cardiology",
            Description = "Heart"
        };
        var specialization2 = new Specialization
        {
            Id = Guid.NewGuid(),
            Name = "Neurology",
            Description = "Brain"
        };

        await _dbContext.Specializations.AddRangeAsync(specialization1, specialization2);

        var doctorSpecializations = new List<DoctorSpecialization>
        {
            new() { DoctorId = doctorId, SpecializationId = specialization1.Id, Specialization = specialization1 },
            new() { DoctorId = doctorId, SpecializationId = specialization2.Id, Specialization = specialization2 }
        };

        await _dbContext.DoctorSpecializations.AddRangeAsync(doctorSpecializations);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllByDoctorIdAsync(doctorId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().Contain(s => s.Name == "Cardiology");
        result.Should().Contain(s => s.Name == "Neurology");
    }

    [Fact]
    public async Task GetAllByDoctorIdAsync_WhenDoctorHasNoSpecializations_ShouldReturnEmptyList()
    {
        // Arrange
        var doctorId = Guid.NewGuid();

        // Act
        var result = await _repository.GetAllByDoctorIdAsync(doctorId);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllByDoctorIdAsync_ShouldNotReturnOtherDoctorsSpecializations()
    {
        // Arrange
        var doctorId1 = Guid.NewGuid();
        var doctorId2 = Guid.NewGuid();

        var specialization1 = new Specialization
        {
            Id = Guid.NewGuid(),
            Name = "Cardiology",
            Description = "Heart"
        };
        var specialization2 = new Specialization
        {
            Id = Guid.NewGuid(),
            Name = "Neurology",
            Description = "Brain"
        };

        await _dbContext.Specializations.AddRangeAsync(specialization1, specialization2);

        var doctorSpecializations = new List<DoctorSpecialization>
        {
            new() { DoctorId = doctorId1, SpecializationId = specialization1.Id, Specialization = specialization1 },
            new() { DoctorId = doctorId2, SpecializationId = specialization2.Id, Specialization = specialization2 }
        };

        await _dbContext.DoctorSpecializations.AddRangeAsync(doctorSpecializations);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllByDoctorIdAsync(doctorId1);

        // Assert
        result.Should().HaveCount(1);
        result.Should().Contain(s => s.Name == "Cardiology");
        result.Should().NotContain(s => s.Name == "Neurology");
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WhenSpecializationExists_ShouldUpdateAndReturnSpecialization()
    {
        // Arrange
        var id = Guid.NewGuid();
        var existingSpecialization = new Specialization
        {
            Id = id,
            Name = "Cardiology",
            Description = "Heart conditions"
        };

        await _dbContext.Specializations.AddAsync(existingSpecialization);
        await _dbContext.SaveChangesAsync();

        _dbContext.ChangeTracker.Clear();

        var updatedSpecialization = new Specialization
        {
            Id = id,
            Name = "Advanced Cardiology",
            Description = "Advanced heart conditions"
        };

        // Act
        var result = await _repository.UpdateAsync(updatedSpecialization);
        await _dbContext.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Advanced Cardiology");
        result.Description.Should().Be("Advanced heart conditions");

        var savedSpecialization = await _dbContext.Specializations.FindAsync(id);
        savedSpecialization!.Name.Should().Be("Advanced Cardiology");
        savedSpecialization.Description.Should().Be("Advanced heart conditions");
    }

    [Fact]
    public async Task UpdateAsync_WhenSpecializationDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentSpecialization = new Specialization
        {
            Id = Guid.NewGuid(),
            Name = "Non-existent",
            Description = "Does not exist"
        };

        // Act
        var result = await _repository.UpdateAsync(nonExistentSpecialization);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenSpecializationExists_ShouldRemoveAndReturnSpecialization()
    {
        // Arrange
        var id = Guid.NewGuid();
        var specialization = new Specialization
        {
            Id = id,
            Name = "Oncology",
            Description = "Cancer treatment"
        };

        await _dbContext.Specializations.AddAsync(specialization);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _repository.DeleteAsync(id);
        await _dbContext.SaveChangesAsync();

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.Name.Should().Be("Oncology");

        var deletedSpecialization = await _dbContext.Specializations.FindAsync(id);
        deletedSpecialization.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenSpecializationDoesNotExist_ShouldReturnNull()
    {
        // Arrange
        var nonExistentId = Guid.NewGuid();

        // Act
        var result = await _repository.DeleteAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotAffectOtherSpecializations()
    {
        // Arrange
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();

        var specialization1 = new Specialization
        {
            Id = id1,
            Name = "Cardiology",
            Description = "Heart"
        };
        var specialization2 = new Specialization
        {
            Id = id2,
            Name = "Neurology",
            Description = "Brain"
        };

        await _dbContext.Specializations.AddRangeAsync(specialization1, specialization2);
        await _dbContext.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(id1);
        await _dbContext.SaveChangesAsync();

        // Assert
        var deletedSpecialization = await _dbContext.Specializations.FindAsync(id1);
        deletedSpecialization.Should().BeNull();

        var remainingSpecialization = await _dbContext.Specializations.FindAsync(id2);
        remainingSpecialization.Should().NotBeNull();
        remainingSpecialization!.Name.Should().Be("Neurology");
    }

    #endregion
}