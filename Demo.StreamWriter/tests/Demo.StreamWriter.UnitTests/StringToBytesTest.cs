using Xunit;

namespace Demo.StreamWriter.UnitTests
{
    public class StringToBytesTest
    {
        [Fact]
        public void StringToBytes_Equals_ReturnTrue()
        {
            // Arrange

            var stringValue = "Hello World";

            // Act
            var bytesWithStream = StringToBytesWithStream.StringToBytes(stringValue);
            var bytesWithEncoding = StringToBytesWithEncoding.StringToBytes(stringValue);

            var compareResult = BytesHelper.BytesEquals(bytesWithStream, bytesWithEncoding);

            // Assert 

            Assert.True(compareResult, "CompareResult for Stream vs Encoding");
        }
    }
}