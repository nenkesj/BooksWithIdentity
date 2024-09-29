using HowTo_DBLibrary;
using Xunit;
namespace Books.Tests
{
    public class BookTests
    {
        [Fact]
        public void CanChangeNodeHeading()
        {
            // Arrange
            var n = new Node();
            // Act
            n.Heading = "Test";
            //Assert
            Assert.Equal("Test", n.Heading);
        }
    }
}