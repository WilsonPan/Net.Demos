using Xunit;

namespace Demo.StreamWriter.UnitTests
{
    public class BytesHelperTest
    {
        [Fact]
        public void BytesEqualsTest_Equals_ReturnTrue()
        {
            // Arrange
            var a = new byte[] { 23, 19, 39, 29 };
            var b = new byte[] { 23, 19, 39, 29 };
            var c = a;

            // Act
            var bothNull = BytesHelper.BytesEquals(null, null);
            var valueEquals = BytesHelper.BytesEquals(a, b);
            var referenceEquals = BytesHelper.BytesEquals(a, c);

            // Assert
            Assert.True(bothNull, nameof(bothNull));
            Assert.True(valueEquals, nameof(valueEquals));
            Assert.True(referenceEquals, nameof(referenceEquals));
        }

        [Fact]
        public void BytesEqualsTest_NotEquals_ReturnFalse()
        {
            // Arrange
            var a = new byte[] { 23, 19, 39, 29 };
            var b = new byte[] { 23, 19, 39, 23 };
            var c = new byte[] { 23, 19, 39, 29, 56 };

            // Act
            var rightIsNull = BytesHelper.BytesEquals(a, null);
            var leftIsNull = BytesHelper.BytesEquals(null, b);

            var valueNotEquals = BytesHelper.BytesEquals(a, b);

            var lengthNotEquals = BytesHelper.BytesEquals(a, c);

            // Assert
            Assert.False(rightIsNull, nameof(rightIsNull));
            Assert.False(leftIsNull, nameof(leftIsNull));

            Assert.False(valueNotEquals, nameof(valueNotEquals));

            Assert.False(lengthNotEquals, nameof(lengthNotEquals));
        }
    }
}