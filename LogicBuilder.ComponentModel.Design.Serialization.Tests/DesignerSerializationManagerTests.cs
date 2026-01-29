using System.ComponentModel;
using System.ComponentModel.Design.Serialization;

namespace LogicBuilder.ComponentModel.Design.Serialization.Tests
{
    public class DesignerSerializationManagerTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_WithoutProvider_InitializesSuccessfully()
        {
            // Arrange & Act
            var manager = new DesignerSerializationManager();

            // Assert
            Assert.NotNull(manager);
            Assert.True(manager.PreserveNames);
            Assert.True(manager.ValidateRecycledTypes);
            Assert.False(manager.RecycleInstances);
        }

        [Fact]
        public void Constructor_WithProvider_InitializesSuccessfully()
        {
            // Arrange
            var provider = new MockServiceProvider();

            // Act
            var manager = new DesignerSerializationManager(provider);

            // Assert
            Assert.NotNull(manager);
            Assert.True(manager.PreserveNames);
            Assert.True(manager.ValidateRecycledTypes);
        }

        [Fact]
        public void Constructor_WithNullProvider_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            Assert.Throws<ArgumentNullException>(() => new DesignerSerializationManager(null));
        }

        #endregion

        #region Property Tests

        [Fact]
        public void PreserveNames_CanSetAndGet()
        {
            // Arrange
            var manager = new DesignerSerializationManager
            {
                // Act
                PreserveNames = false
            };

            // Assert
            Assert.False(manager.PreserveNames);
        }

        [Fact]
        public void PreserveNames_ThrowsExceptionDuringSession()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            using (manager.CreateSession())
            {
                // Act & Assert
                Assert.Throws<InvalidOperationException>(() => manager.PreserveNames = false);
            }
        }

        [Fact]
        public void RecycleInstances_CanSetAndGet()
        {
            // Arrange
            var manager = new DesignerSerializationManager
            {
                // Act
                RecycleInstances = true
            };

            // Assert
            Assert.True(manager.RecycleInstances);
        }

        [Fact]
        public void RecycleInstances_ThrowsExceptionDuringSession()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            using (manager.CreateSession())
            {
                // Act & Assert
                Assert.Throws<InvalidOperationException>(() => manager.RecycleInstances = true);
            }
        }

        [Fact]
        public void ValidateRecycledTypes_CanSetAndGet()
        {
            // Arrange
            var manager = new DesignerSerializationManager
            {
                // Act
                ValidateRecycledTypes = false
            };

            // Assert
            Assert.False(manager.ValidateRecycledTypes);
        }

        [Fact]
        public void ValidateRecycledTypes_ThrowsExceptionDuringSession()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            using (manager.CreateSession())
            {
                // Act & Assert
                Assert.Throws<InvalidOperationException>(() => manager.ValidateRecycledTypes = false);
            }
        }

        [Fact]
        public void Container_CanSetAndGet()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var container = new Container();

            // Act
            manager.Container = container;

            // Assert
            Assert.Same(container, manager.Container);
        }

        [Fact]
        public void Container_ThrowsExceptionDuringSession()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var container = new Container();
            using (manager.CreateSession())
            {
                // Act & Assert
                Assert.Throws<InvalidOperationException>(() => manager.Container = container);
            }
        }

        [Fact]
        public void PropertyProvider_CanSetAndGet()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            var provider = new object();

            // Act
            manager.PropertyProvider = provider;

            // Assert
            Assert.Same(provider, manager.PropertyProvider);
        }

        [Fact]
        public void Errors_ThrowsExceptionOutsideSession()
        {
            // Arrange
            var manager = new DesignerSerializationManager();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => manager.Errors);
        }

        [Fact]
        public void Errors_ReturnsListDuringSession()
        {
            // Arrange
            var manager = new DesignerSerializationManager();

            // Act & Assert
            using (manager.CreateSession())
            {
                var errors = manager.Errors;
                Assert.NotNull(errors);
                Assert.Empty(errors);
            }
        }

        #endregion

        #region Session Tests

        [Fact]
        public void CreateSession_CreatesNewSession()
        {
            // Arrange
            var manager = new DesignerSerializationManager();

            // Act
            using var session = manager.CreateSession();
            // Assert
            Assert.NotNull(session);
        }

        [Fact]
        public void CreateSession_ThrowsExceptionWhenSessionAlreadyExists()
        {
            // Arrange
            var manager = new DesignerSerializationManager();

            // Act & Assert
            using (manager.CreateSession())
            {
                Assert.Throws<InvalidOperationException>(() => manager.CreateSession());
            }
        }

        [Fact]
        public void SessionCreated_EventRaised()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            bool eventRaised = false;
            manager.SessionCreated += (sender, e) => eventRaised = true;

            // Act
            using (manager.CreateSession())
            {
                // Assert
                Assert.True(eventRaised);
            }
        }

        [Fact]
        public void SessionDisposed_EventRaised()
        {
            // Arrange
            var manager = new DesignerSerializationManager();
            bool eventRaised = false;
            manager.SessionDisposed += (sender, e) => eventRaised = true;

            // Act
            using (manager.CreateSession())
            {
            }

            // Assert
            Assert.True(eventRaised);
        }

        #endregion

        #region IDesignerSerializationManager Tests

        [Fact]
        public void Context_ThrowsExceptionOutsideSession()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => manager.Context);
        }

        [Fact]
        public void Context_ReturnsContextStackDuringSession()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var context = manager.Context;
                Assert.NotNull(context);
            }
        }

        [Fact]
        public void Properties_ReturnsEmptyCollectionWhenNoProvider()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act
            var properties = manager.Properties;

            // Assert
            Assert.NotNull(properties);
            Assert.Empty(properties);
        }

        [Fact]
        public void AddSerializationProvider_AddsProvider()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();
            var provider = new MockSerializationProvider();

            // Act
            manager.AddSerializationProvider(provider);

            // Assert
            var providers = ((DesignerSerializationManager)manager).SerializationProviders.OfType<IDesignerSerializationProvider>();
            Assert.Contains(provider, providers);
        }

        [Fact]
        public void RemoveSerializationProvider_RemovesProvider()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();
            var provider = new MockSerializationProvider();
            manager.AddSerializationProvider(provider);

            // Act
            manager.RemoveSerializationProvider(provider);

            // Assert
            var providers = ((DesignerSerializationManager)manager).SerializationProviders.OfType<IDesignerSerializationProvider>();
            Assert.DoesNotContain(provider, providers);
        }

        [Fact]
        public void CreateInstance_CreatesInstanceDuringSession()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var instance = manager.CreateInstance(typeof(TestClass), null, "test", false);
                Assert.NotNull(instance);
                Assert.IsType<TestClass>(instance);
            }
        }

        [Fact]
        public void CreateInstance_ThrowsExceptionOutsideSession()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                manager.CreateInstance(typeof(TestClass), null, "test", false));
        }

        [Fact]
        public void CreateInstance_ThrowsExceptionForDuplicateName()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                manager.CreateInstance(typeof(TestClass), null, "test", false);
                Assert.Throws<System.Runtime.Serialization.SerializationException>(() =>
                    manager.CreateInstance(typeof(TestClass), null, "test", false));
            }
        }

        [Fact]
        public void GetInstance_ReturnsInstanceByName()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var created = manager.CreateInstance(typeof(TestClass), null, "test", false);
                var retrieved = manager.GetInstance("test");
                Assert.Same(created, retrieved);
            }
        }

        [Fact]
        public void GetInstance_ThrowsExceptionOutsideSession()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => manager.GetInstance("test"));
        }

        [Fact]
        public void GetInstance_ThrowsExceptionForNullName()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                Assert.Throws<ArgumentNullException>(() => manager.GetInstance(null!));
            }
        }

        [Fact]
        public void GetName_ReturnsNameForInstance()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var instance = manager.CreateInstance(typeof(TestClass), null, "test", false);
                var name = manager.GetName(instance);
                Assert.Equal("test", name);
            }
        }

        [Fact]
        public void GetName_ThrowsExceptionOutsideSession()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => manager.GetName(new object()));
        }

        [Fact]
        public void GetName_ThrowsExceptionForNullValue()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                Assert.Throws<ArgumentNullException>(() => manager.GetName(null!));
            }
        }

        [Fact]
        public void SetName_SetsNameForInstance()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var instance = new TestClass();
                manager.SetName(instance, "test");
                var name = manager.GetName(instance);
                Assert.Equal("test", name);
            }
        }

        [Fact]
        public void SetName_ThrowsExceptionOutsideSession()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                manager.SetName(new object(), "test"));
        }

        [Fact]
        public void SetName_ThrowsExceptionForNullInstance()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                Assert.Throws<ArgumentNullException>(() => manager.SetName(null!, "test"));
            }
        }

        [Fact]
        public void SetName_ThrowsExceptionForNullName()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                Assert.Throws<ArgumentNullException>(() => manager.SetName(new object(), null!));
            }
        }

        [Fact]
        public void SetName_ThrowsExceptionForDuplicateName()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var instance1 = new TestClass();
                var instance2 = new TestClass();
                manager.SetName(instance1, "test");
                Assert.Throws<ArgumentException>(() => manager.SetName(instance2, "test"));
            }
        }

        [Fact]
        public void SetName_ThrowsExceptionWhenInstanceAlreadyHasName()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var instance = new TestClass();
                manager.SetName(instance, "test1");
                Assert.Throws<ArgumentException>(() => manager.SetName(instance, "test2"));
            }
        }

        [Fact]
        public void ReportError_AddsErrorToList()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var error = "Test error";
                manager.ReportError(error);
                var errors = ((DesignerSerializationManager)manager).Errors.OfType<string>();
                Assert.Contains(error, errors);
            }
        }

        [Fact]
        public void ReportError_ThrowsExceptionOutsideSession()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => manager.ReportError("error"));
        }

        [Fact]
        public void GetType_ThrowsExceptionOutsideSession()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => manager.GetType("System.String"));
        }

        [Fact]
        public void GetType_ReturnsTypeDuringSession()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var type = manager.GetType("System.String");
                Assert.Equal(typeof(string), type);
            }
        }

        [Fact]
        public void GetSerializer_ReturnsNullForNullObjectType()
        {
            // Arrange
            var manager = new DesignerSerializationManager();

            // Act
            var serializer = manager.GetSerializer(null, typeof(object));

            // Assert
            Assert.Null(serializer);
        }

        [Fact]
        public void GetSerializer_ThrowsExceptionForNullSerializerType()
        {
            // Arrange
            var manager = new DesignerSerializationManager();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                manager.GetSerializer(typeof(TestClass), null));
        }

        [Fact]
        public void ResolveName_EventRaisedWhenInstanceNotFound()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();
            bool eventRaised = false;
            void handler(object? sender, ResolveNameEventArgs e)
            {
                eventRaised = true;
                e.Value = new TestClass();
            }

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                manager.ResolveName += handler;
                var instance = manager.GetInstance("test");
                Assert.True(eventRaised);
                Assert.NotNull(instance);
            }
        }

        [Fact]
        public void SerializationComplete_EventRaisedAfterSessionDisposed()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();
            bool eventRaised = false;

            // Act
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                manager.SerializationComplete += (sender, e) => eventRaised = true;
            }

            // Assert
            Assert.True(eventRaised);
        }

        #endregion

        #region Helper Classes

        private class TestClass
        {
            public string Name { get; set; } = string.Empty;
        }

        private class MockServiceProvider : IServiceProvider
        {
            public object? GetService(Type serviceType)
            {
                return null;
            }
        }

        private class MockSerializationProvider : IDesignerSerializationProvider
        {
            public object GetSerializer(IDesignerSerializationManager manager, object? currentSerializer,
                Type? objectType, Type serializerType)
            {
                return null!;
            }
        }

        #endregion
    }
}