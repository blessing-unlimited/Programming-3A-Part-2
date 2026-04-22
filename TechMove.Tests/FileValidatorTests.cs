using TechMove.Services;
using Microsoft.AspNetCore.Http;
using Moq; 
using Xunit;

namespace TechMove.Tests
{
    public class FileValidatorTests
    {
        private readonly IFileValidator _validator;

        public FileValidatorTests()
        {
            _validator = new FileValidator();
        }

        [Fact]
        public void IsPdf_ValidPdf_ReturnsTrue()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("contract.pdf");

            // Act
            var result = _validator.IsPdf(fileMock.Object);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("virus.exe")]
        [InlineData("script.js")]
        [InlineData("image.png")]
        public void IsPdf_InvalidExtension_ReturnsFalse(string fileName)
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns(fileName);

            // Act
            var result = _validator.IsPdf(fileMock.Object);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsPdf_NullFile_ReturnsFalse()
        {
            // Act
            var result = _validator.IsPdf(null);

            // Assert
            Assert.False(result);
        }
    }
}