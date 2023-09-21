using Crimsilk.Utilities.Extensions;
using NUnit.Framework;

namespace Testing.EditorTests.Scripts.Extensions
{
    public class IntExtensionsTests
    {
        [Test]
        [TestCase(0, 1)]
        [TestCase(0, -1)]
        [TestCase(1, 0)]
        [TestCase(1, 2)]
        [TestCase(-1, 0)]
        [TestCase(-1, -2)]
        public void IsAdjacent_ShouldReturnTrue_WhenNumberIsAdjacent(int source, int target)
        {
            // Arrange
            // Act
            bool result = source.IsAdjacent(target);

            // Assert
            Assert.True(result);
        }
        
        [Test]
        [TestCase(0, 0)]
        [TestCase(0, 2)]
        [TestCase(0, -2)]
        [TestCase(1, -1)]
        [TestCase(1, 3)]
        [TestCase(-1, 1)]
        [TestCase(-1, -3)]
        public void IsAdjacent_ShouldReturnFalse_WhenNumberIsNotAdjacent(int source, int target)
        {
            // Arrange
            // Act
            bool result = source.IsAdjacent(target);

            // Assert
            Assert.False(result);
        }
    }
}