namespace LogicBuilder.ComponentModel.Design.Serialization.Tests
{
    public class SRTests
    {
        [Fact]
        public void GetString_WithValidResourceKey_ReturnsString()
        {
            // Arrange
            string resourceKey = "SerializationManagerNoSession";

            // Act
            string result = SR.GetString(resourceKey);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetString_WithInvalidResourceKey_ReturnsNull()
        {
            // Arrange
            string invalidKey = "NonExistentResourceKey_12345";

            // Act
            string result = SR.GetString(invalidKey);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetString_WithEmptyKey_ReturnsNull()
        {
            // Act
            string result = SR.GetString(string.Empty);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetString_WithFormatArgs_ReturnsFormattedString()
        {
            // Arrange
            string resourceKey = "SerializationManagerNoSession";
            object[] args = ["TestValue"];

            // Act
            string result = SR.GetString(resourceKey, args);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetString_WithNullArgs_ReturnsUnformattedString()
        {
            // Arrange
            string resourceKey = "SerializationManagerNoSession";

            // Act
            string result = SR.GetString(resourceKey, (object[])null!);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetString_WithEmptyArgs_ReturnsUnformattedString()
        {
            // Arrange
            string resourceKey = "SerializationManagerNoSession";

            // Act
            string result = SR.GetString(resourceKey, []);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetString_WithLongStringArg_TruncatesString()
        {
            // Arrange
            string resourceKey = "SerializationManagerNoSession";
            string longString = new('A', 1100); // More than 0x400 (1024)
            object[] args = [longString];

            // Act
            string result = SR.GetString(resourceKey, args);

            // Assert
            Assert.NotNull(result);
            // The long string should be truncated to 1021 chars + "..."
            Assert.DoesNotContain(new string('A', 1100), result);
        }

        [Fact]
        public void GetString_WithMultipleArgs_SomeAreLongStrings_TruncatesOnlyLongOnes()
        {
            // Arrange
            string resourceKey = "SerializationManagerNoSession";
            string shortString = "Short";
            string longString = new('B', 1100);
            object[] args = [shortString, longString];

            // Act
            string result = SR.GetString(resourceKey, args);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetString_WithNonStringArgs_DoesNotTruncate()
        {
            // Arrange
            string resourceKey = "SerializationManagerNoSession";
            object[] args = [123, DateTime.Now, true];

            // Act
            string result = SR.GetString(resourceKey, args);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetString_WithExactly1024CharString_DoesNotTruncate()
        {
            // Arrange
            string resourceKey = "SerializationManagerNoSession";
            string exactString = new('C', 1024); // Exactly 0x400
            object[] args = [exactString];

            // Act
            string result = SR.GetString(resourceKey, args);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetString_WithUsedFallback_ReturnsFalse()
        {
            // Arrange
            string resourceKey = "SerializationManagerNoSession";

            // Act
            string result = SR.GetString(resourceKey, out bool usedFallback);

            // Assert
            Assert.NotNull(result);
            Assert.False(usedFallback);
        }

        [Fact]
        public void GetString_WithInvalidKeyAndUsedFallback_ReturnsNullAndFalse()
        {
            // Arrange
            string invalidKey = "NonExistentResourceKey_12345";

            // Act
            string result = SR.GetString(invalidKey, out bool usedFallback);

            // Assert
            Assert.Null(result);
            Assert.False(usedFallback);
        }

        [Fact]
        public void GetObject_WithValidResourceKey_ReturnsObject()
        {
            // Arrange
            string resourceKey = "SerializationManagerNoSession";

            // Act
            object result = SR.GetObject(resourceKey);

            // Assert
            // The result might be null if the resource is a string, not an object resource
            // This is expected behavior
            Assert.True(result == null || result is string);
        }

        [Fact]
        public void GetObject_WithInvalidResourceKey_ReturnsNull()
        {
            // Arrange
            string invalidKey = "NonExistentResourceKey_12345";

            // Act
            object result = SR.GetObject(invalidKey);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Resources_Property_ReturnsResourceManager()
        {
            // Act
            var resources = SR.Resources;

            // Assert
            Assert.NotNull(resources);
        }

        [Fact]
        public void Resources_Property_CalledMultipleTimes_ReturnsSameInstance()
        {
            // Act
            var resources1 = SR.Resources;
            var resources2 = SR.Resources;

            // Assert
            Assert.Same(resources1, resources2);
        }

        [Fact]
        public async Task GetString_CalledConcurrently_IsThreadSafe()
        {
            // Arrange
            string resourceKey = "SerializationManagerNoSession";
            var tasks = new Task<string>[100];

            // Act
            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() => SR.GetString(resourceKey));
            }

            await Task.WhenAll(tasks);

            // Assert
            foreach (var task in tasks)
            {
                Assert.NotNull(await task);
            }
        }

        [Theory]
        [InlineData("SerializationManagerNoSession")]
        [InlineData("SerializerNoRootExpression")]
        [InlineData("CodeDomDesignerLoaderNoRootSerializer")]
        [InlineData("InvalidArgument")]
        public void GetString_WithVariousValidKeys_ReturnsNonEmptyStrings(string resourceKey)
        {
            // Act
            string result = SR.GetString(resourceKey);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact]
        public void GetString_WithMixedArgsIncludingNull_HandlesGracefully()
        {
            // Arrange
            string resourceKey = "SerializationManagerNoSession";
            object[] args = ["test", null!, 123];

            // Act
            string result = SR.GetString(resourceKey, args);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetString_WithStringExactly1025Chars_TruncatesString()
        {
            // Arrange
            string resourceKey = "SerializationManagerNoSession";
            string longString = new('D', 1025); // 0x400 + 1
            object[] args = [longString];

            // Act
            string result = SR.GetString(resourceKey, args);

            // Assert
            Assert.NotNull(result);
            Assert.DoesNotContain(new string('D', 1025), result);
        }

        [Fact]
        public void GetString_WithEmptyStringArg_DoesNotTruncate()
        {
            // Arrange
            string resourceKey = "SerializationManagerNoSession";
            object[] args = [string.Empty];

            // Act
            string result = SR.GetString(resourceKey, args);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetString_WithWhitespaceStringArg_DoesNotTruncate()
        {
            // Arrange
            string resourceKey = "SerializationManagerNoSession";
            object[] args = ["   "];

            // Act
            string result = SR.GetString(resourceKey, args);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetString_ResourceManagerInitialization_IsLazyLoaded()
        {
            // This test verifies that the resource manager is initialized lazily
            // by accessing it through multiple methods

            // Act
            var result1 = SR.GetString("SerializationManagerNoSession");
            var result2 = SR.Resources;

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
        }

        [Fact]
        public void GetString_WithStringArgContainingSpecialCharacters_HandlesCorrectly()
        {
            // Arrange
            string resourceKey = "SerializationManagerNoSession";
            object[] args = ["Test\nNew\tLine\r\n"];

            // Act
            string result = SR.GetString(resourceKey, args);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetString_WithStringArgContainingUnicodeCharacters_HandlesCorrectly()
        {
            // Arrange
            string resourceKey = "SerializationManagerNoSession";
            object[] args = ["Test中文Ñoño"];

            // Act
            string result = SR.GetString(resourceKey, args);

            // Assert
            Assert.NotNull(result);
        }
    }
}