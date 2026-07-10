using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using Moq;
using sports_api.Services;
using Xunit;

namespace Tests;

public class UploadServiceTests
{
    private readonly Mock<ICloudinaryUploadApi> _cloudinaryMock;
    private readonly Mock<IConfiguration> _configMock;

    public UploadServiceTests()
    {
        _cloudinaryMock = new Mock<ICloudinaryUploadApi>();
        _configMock = new Mock<IConfiguration>();
    }

    private UploadService CreateSut() => new(_cloudinaryMock.Object, _configMock.Object);

    [Fact]
    public async Task UploadImageAsync_WhenUploadSucceeds_ReturnsSecureUrl()
    {
        // Arrange
        var secureUrl = "https://res.cloudinary.com/demo/image/upload/avatar.jpg";
        _cloudinaryMock
            .Setup(c => c.UploadAsync(It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken?>()))
            .ReturnsAsync(new ImageUploadResult { SecureUrl = new Uri(secureUrl) });

        var sut = CreateSut();

        // Act
        var result = await sut.UploadImageAsync(new MemoryStream(), "avatar.jpg", "avatars");

        // Assert
        Assert.Equal(secureUrl, result);
    }

    [Fact]
    public async Task UploadImageAsync_WhenCloudinaryReturnsError_ReturnsNull()
    {
        // Arrange
        _cloudinaryMock
            .Setup(c => c.UploadAsync(It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken?>()))
            .ReturnsAsync(new ImageUploadResult { Error = new Error { Message = "upload failed" } });

        var sut = CreateSut();

        // Act
        var result = await sut.UploadImageAsync(new MemoryStream(), "avatar.jpg", "avatars");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UploadImageAsync_WhenFolderIsConfigured_UsesConfiguredFolderAndSubFolder()
    {
        // Arrange
        _configMock.Setup(c => c["Cloudinary:Folder"]).Returns("custom-root");

        ImageUploadParams? capturedParams = null;
        _cloudinaryMock
            .Setup(c => c.UploadAsync(It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken?>()))
            .Callback<ImageUploadParams, CancellationToken?>((p, _) => capturedParams = p)
            .ReturnsAsync(new ImageUploadResult { SecureUrl = new Uri("https://res.cloudinary.com/demo/image/upload/x.jpg") });

        var sut = CreateSut();

        // Act
        await sut.UploadImageAsync(new MemoryStream(), "avatar.jpg", "avatars");

        // Assert
        Assert.Equal("custom-root/avatars", capturedParams?.Folder);
    }

    [Fact]
    public async Task UploadImageAsync_WhenFolderNotConfigured_UsesDefaultFolder()
    {
        // Arrange
        _configMock.Setup(c => c["Cloudinary:Folder"]).Returns((string?)null);

        ImageUploadParams? capturedParams = null;
        _cloudinaryMock
            .Setup(c => c.UploadAsync(It.IsAny<ImageUploadParams>(), It.IsAny<CancellationToken?>()))
            .Callback<ImageUploadParams, CancellationToken?>((p, _) => capturedParams = p)
            .ReturnsAsync(new ImageUploadResult { SecureUrl = new Uri("https://res.cloudinary.com/demo/image/upload/x.jpg") });

        var sut = CreateSut();

        // Act
        await sut.UploadImageAsync(new MemoryStream(), "logo.jpg", "team-logos");

        // Assert
        Assert.Equal("matchers/production/team-logos", capturedParams?.Folder);
    }

    [Fact]
    public async Task DeleteImageAsync_WhenResultIsOk_ReturnsTrue()
    {
        // Arrange
        _cloudinaryMock
            .Setup(c => c.DestroyAsync(It.IsAny<DeletionParams>()))
            .ReturnsAsync(new DeletionResult { Result = "ok" });

        var sut = CreateSut();

        // Act
        var result = await sut.DeleteImageAsync("some-public-id");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteImageAsync_WhenResultIsNotOk_ReturnsFalse()
    {
        // Arrange
        _cloudinaryMock
            .Setup(c => c.DestroyAsync(It.IsAny<DeletionParams>()))
            .ReturnsAsync(new DeletionResult { Result = "not found" });

        var sut = CreateSut();

        // Act
        var result = await sut.DeleteImageAsync("missing-public-id");

        // Assert
        Assert.False(result);
    }
}
