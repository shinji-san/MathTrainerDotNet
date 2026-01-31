using MathTrainerDotNet.Services.Id;

namespace MathTrainerDotNetTest.Services.Id;

public class PublicIdServiceTest
{
    private readonly PublicIdService publicIdService;

    public PublicIdServiceTest()
    {
        this.publicIdService = new PublicIdService();
    }

    [Fact]
    public void GeneratePublicId_ShouldReturnStringOfLengthSix()
    {
        // Act
        var result = this.publicIdService.GeneratePublicId();

        // Assert
        Assert.Equal(6, result.Length);
    }

    [Fact]
    public void GeneratePublicId_ShouldOnlyContainAllowedCharacters()
    {
        // Arrange
        const string allowedChars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            
        // Act
        var result = this.publicIdService.GeneratePublicId();

        // Assert
        foreach (var c in result)
        {
            Assert.Contains(c, allowedChars);
        }
    }

    [Fact]
    public void GeneratePublicId_ShouldGenerateDifferentIds()
    {
        // Act
        var id1 = this.publicIdService.GeneratePublicId();
        var id2 = this.publicIdService.GeneratePublicId();

        // Assert
        Assert.NotEqual(id1, id2);
    }

    [Fact]
    public void GeneratePublicId_ShouldNotReturnNullOrEmpty()
    {
        // Act
        var result = this.publicIdService.GeneratePublicId();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(result));
    }
}