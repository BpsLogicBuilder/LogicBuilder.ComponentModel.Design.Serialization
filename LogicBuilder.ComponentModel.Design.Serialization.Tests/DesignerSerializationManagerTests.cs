using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using Moq;

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
            using var container = new Container();

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
            using var container = new Container();
            using (manager.CreateSession())
            {
                // Act & Assert
                Assert.Throws<InvalidOperationException>(() => manager.Container = container);
            }
        }

        [Fact]
        public void Container_GetsFromDesignerHostWhenNull()
        {
            // Arrange
            using var container = new Container();
            var mockHost = new Mock<IDesignerHost>();
            mockHost.Setup(h => h.Container).Returns(container);
            
            var mockProvider = new Mock<IServiceProvider>();
            mockProvider.Setup(p => p.GetService(typeof(IDesignerHost))).Returns(mockHost.Object);
            
            var manager = new DesignerSerializationManager(mockProvider.Object);

            // Act
            var result = manager.Container;

            // Assert
            Assert.Same(container, result);
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
        public void PropertyProvider_ClearsPropertiesWhenChanged()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();
            var provider1 = new TestPropertyProvider();
            var provider2 = new TestPropertyProvider();
            
            ((DesignerSerializationManager)manager).PropertyProvider = provider1;
            var props1 = manager.Properties;

            // Act
            ((DesignerSerializationManager)manager).PropertyProvider = provider2;
            var props2 = manager.Properties;

            // Assert
            Assert.NotSame(props1, props2);
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
        public void Properties_ReturnsWrappedPropertiesFromProvider()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();
            ((DesignerSerializationManager)manager).PropertyProvider = new TestPropertyProvider();

            // Act
            var properties = manager.Properties;

            // Assert
            Assert.NotNull(properties);
            Assert.True(properties.Count > 0);
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
        public void CreateInstance_RecyclesInstanceWhenRecycleInstancesIsTrue()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager
            {
                RecycleInstances = true
            };

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var instance1 = manager.CreateInstance(typeof(TestClass), null, "test", false);
                Assert.Same(instance1, instance1);
            }
        }

        [Fact]
        public void CreateInstance_DoesNotRecycleInstanceWhenTypeMismatchAndValidateRecycledTypesIsTrue()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager
            {
                RecycleInstances = true,
                ValidateRecycledTypes = true
            };

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var instance1 = manager.CreateInstance(typeof(TestClass), null, "test1", false);
                var instance2 = manager.CreateInstance(typeof(TestClass2), null, "test2", false);
                Assert.NotSame(instance1, instance2);
            }
        }

        [Fact]
        public void CreateInstance_RecyclesFromContainerWhenAvailable()
        {
            // Arrange
            using var container = new Container();
            var existingComponent = new TestComponent();
            container.Add(existingComponent, "test");

            var manager = (IDesignerSerializationManager)new DesignerSerializationManager
            {
                RecycleInstances = true,
                Container = container
            };

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var instance = manager.CreateInstance(typeof(TestComponent), null, "test", true);
                Assert.Same(existingComponent, instance);
            }
        }

        [Fact]
        public void CreateInstance_RecyclesFromContainerWhenAvailable_ObjectTypeDoesNotMatch()
        {
            // Arrange
            using var container = new Container();
            var existingComponent = new TestComponent();
            container.Add(existingComponent, "test");

            var manager = (IDesignerSerializationManager)new DesignerSerializationManager
            {
                RecycleInstances = true,
                Container = container
            };

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var instance = manager.CreateInstance(typeof(TestClass), null, "test", true);
                Assert.NotSame(existingComponent, instance);
            }
        }

        [Fact]
        public void CreateInstance_UsesDesignerHostToCreateComponent()
        {
            // Arrange
            using var container = new Container();
            var mockHost = new Mock<IDesignerHost>();
            mockHost.Setup(h => h.Container).Returns(container);
            mockHost.Setup(h => h.CreateComponent(typeof(TestComponent), "test"))
                .Returns(new TestComponent());

            var mockProvider = new Mock<IServiceProvider>();
            mockProvider.Setup(p => p.GetService(typeof(IDesignerHost))).Returns(mockHost.Object);

            var manager = (IDesignerSerializationManager)new DesignerSerializationManager(mockProvider.Object)
            {
                Container = container
            };

            // Act
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var instance = manager.CreateInstance(typeof(TestComponent), null, "test", true);

                // Assert
                Assert.NotNull(instance);
                mockHost.Verify(h => h.CreateComponent(typeof(TestComponent), "test"), Times.Once);
            }
        }

        [Fact]
        public void CreateInstance_UsesDesignerHostWithoutNameWhenPreserveNamesIsFalseAndNameExists()
        {
            // Arrange
            using var container = new Container();
            container.Add(new TestComponent(), "test");
            
            var mockHost = new Mock<IDesignerHost>();
            mockHost.Setup(h => h.Container).Returns(container);
            mockHost.Setup(h => h.CreateComponent(typeof(TestComponent)))
                .Returns(new TestComponent());

            var mockProvider = new Mock<IServiceProvider>();
            mockProvider.Setup(p => p.GetService(typeof(IDesignerHost))).Returns(mockHost.Object);

            var manager = (IDesignerSerializationManager)new DesignerSerializationManager(mockProvider.Object)
            {
                Container = container,
                PreserveNames = false
            };

            // Act
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var instance = manager.CreateInstance(typeof(TestComponent), null, "test", true);

                // Assert
                Assert.NotNull(instance);
                mockHost.Verify(h => h.CreateComponent(typeof(TestComponent)), Times.Once);
            }
        }

        [Fact]
        public void CreateInstance_AddsComponentToContainerWhenNotCreatedByHost()
        {
            // Arrange
            using var container = new Container();
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager
            {
                Container = container
            };

            // Act
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                manager.CreateInstance(typeof(TestComponent), null, "test", true);

                // Assert
                Assert.NotNull(container.Components["test"]);
            }
        }

        [Fact]
        public void CreateInstance_AddsComponentWithoutNameWhenPreserveNamesIsFalseAndNameExists()
        {
            // Arrange
            using var container = new Container();
            container.Add(new TestComponent(), "test");
            
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager
            {
                Container = container,
                PreserveNames = false
            };

            // Act
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var instance = manager.CreateInstance(typeof(TestComponent), null, "test", true);

                // Assert
                Assert.NotNull(instance);
                Assert.True(container.Components.Count >= 2);
            }
        }

        [Fact]
        public void CreateInstance_ThrowsSerializationExceptionForMissingConstructor()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var exception = Assert.Throws<System.Runtime.Serialization.SerializationException>(() =>
                    manager.CreateInstance(typeof(TestClass), new object[] { "arg1", 123, true }, "test", false));
                
                Assert.Contains("TestClass", exception.Message);
            }
        }

        [Fact]
        public void CreateInstance_ConvertsParametersForConstructor()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var instance = manager.CreateInstance(typeof(TestClassWithConstructor), new object[] { 42 }, "test", false);

                // Assert
                Assert.NotNull(instance);
                var testInstance = instance as TestClassWithConstructor;
                Assert.Equal("42", testInstance?.Value);
            }
        }

        [Fact]
        public void CreateInstance_ConvertsParametersForConstructor_WithConstructorParameterNotConvertable()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                // Assert
                Assert.Throws<System.Runtime.Serialization.SerializationException>(() =>
                    manager.CreateInstance(typeof(TestClassWithConstructorWithNotConvertibleParameter), new object[] { 42, 43, new TestClass { Name = "Test" } }, "test", false));
            }
        }

        [Fact]
        public void CreateInstance_ConvertsParametersForConstructor_WithConstructorParameterThrowingInvalidCastException()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                // Assert
                Assert.Throws<System.Runtime.Serialization.SerializationException>(() =>
                    manager.CreateInstance(typeof(TestClassWithConstructorWithParameteInvalidCastException), new object[] { 42, 43, "44" }, "test", false));
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
        public void GetInstance_ReturnsFromContainerWhenPreserveNamesIsTrue()
        {
            // Arrange
            using var container = new Container();
            var component = new TestComponent();
            container.Add(component, "test");

            var manager = (IDesignerSerializationManager)new DesignerSerializationManager
            {
                Container = container,
                PreserveNames = true
            };

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var retrieved = manager.GetInstance("test");
                Assert.Same(component, retrieved);
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
        public void GetName_ReturnsNameFromComponentSite()
        {
            // Arrange
            using var container = new Container();
            var component = new TestComponent();
            container.Add(component, "test");

            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var name = manager.GetName(component);
                Assert.Equal("test", name);
            }
        }

        [Fact]
        public void GetName_ReturnsFullNameFromNestedSite()
        {
            // Arrange
            var mockSite = new Mock<INestedSite>();
            mockSite.Setup(s => s.FullName).Returns("parent.child");
            
            var component = new TestComponent();
            typeof(Component).GetProperty("Site")!.SetValue(component, mockSite.Object);

            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var name = manager.GetName(component);
                Assert.Equal("parent.child", name);
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
        public void ReportError_DoesNothingForNullError()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                manager.ReportError(null!);
                Assert.Empty(((DesignerSerializationManager)manager).Errors);
            }
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
        public void GetType_HandlesPlusSignAsNestedType()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var type = manager.GetType("LogicBuilder.ComponentModel.Design.Serialization.Tests.DesignerSerializationManagerTests+TestClass, LogicBuilder.ComponentModel.Design.Serialization.Tests, Version=1.0.0.0, Culture=neutral, PublicKeyToken=646893bec0268535");
                Assert.Equal(typeof(TestClass), type);
            }
        }

        [Fact]
        public void GetType_ReturnsNullForUnsupportedType()
        {
            // Arrange
            var mockProvider = new Mock<TypeDescriptionProvider>();
            mockProvider.Setup(p => p.IsSupportedType(It.IsAny<Type>())).Returns(false);

            var mockProviderService = new Mock<TypeDescriptionProviderService>();
            mockProviderService.Setup(s => s.GetProvider(It.IsAny<Type>())).Returns(mockProvider.Object);

            var mockServiceProvider = new Mock<IServiceProvider>();
            mockServiceProvider.Setup(p => p.GetService(typeof(TypeDescriptionProviderService)))
                .Returns(mockProviderService.Object);

            var manager = (IDesignerSerializationManager)new DesignerSerializationManager(mockServiceProvider.Object);

            // Act & Assert
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var type = manager.GetType("System.String");
                Assert.Null(type);
            }
        }

        [Fact]
        public void GetRuntimeType_UsesTypeResolutionService()
        {
            // Arrange
            var mockTypeResolver = new Mock<ITypeResolutionService>();
            mockTypeResolver.Setup(t => t.GetType("CustomType")).Returns(typeof(TestClass));

            var mockProvider = new Mock<IServiceProvider>();
            mockProvider.Setup(p => p.GetService(typeof(ITypeResolutionService))).Returns(mockTypeResolver.Object);

            var manager = new DesignerSerializationManager(mockProvider.Object);

            // Act
            var type = manager.GetRuntimeType("CustomType");

            // Assert
            Assert.Equal(typeof(TestClass), type);
        }

        [Fact]
        public void GetRuntimeType_FallsBackToTypeGetType()
        {
            // Arrange
            var manager = new DesignerSerializationManager();

            // Act
            var type = manager.GetRuntimeType("System.String");

            // Assert
            Assert.Equal(typeof(string), type);
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
        public void GetSerializer_ReturnsSerializerFromAttribute()
        {
            // Arrange
            var manager = new DesignerSerializationManager();

            // Act
            using (manager.CreateSession())
            {
                var serializer = manager.GetSerializer(typeof(TestClassWithSerializer), typeof(TestSerializer));

                // Assert
                Assert.NotNull(serializer);
                Assert.IsType<TestSerializer>(serializer);
            }
        }

        [Fact]
        public void GetSerializer_CachesSerializerDuringSession()
        {
            // Arrange
            var manager = new DesignerSerializationManager();

            // Act
            using (manager.CreateSession())
            {
                var serializer1 = manager.GetSerializer(typeof(TestClassWithSerializer), typeof(TestSerializer));
                var serializer2 = manager.GetSerializer(typeof(TestClassWithSerializer), typeof(TestSerializer));

                // Assert
                Assert.Same(serializer1, serializer2);
            }
        }

        [Fact]
        public void GetSerializer_CachesSerializerDuringSession_WithUnAssignableSubclasss()
        {
            // Arrange
            var manager = new DesignerSerializationManager();

            // Act
            using (manager.CreateSession())
            {
                var serializer1 = manager.GetSerializer(typeof(TestClassWithSerializer), typeof(TestSerializer));
                var serializer2 = manager.GetSerializer(typeof(TestClassWithSerializer), typeof(TestSerializerSubclass));

                // Assert
                Assert.NotSame(serializer1, serializer2);
            }
        }

        [Fact]
        public void GetSerializer_UsesCustomSerializationProvider()
        {
            // Arrange
            var manager = (IDesignerSerializationManager)new DesignerSerializationManager();
            var customSerializer = new TestSerializer();
            var provider = new CustomSerializationProvider(customSerializer);
            manager.AddSerializationProvider(provider);

            // Act
            using (((DesignerSerializationManager)manager).CreateSession())
            {
                var serializer = manager.GetSerializer(typeof(TestClass), typeof(TestSerializer));

                // Assert
                Assert.Same(customSerializer, serializer);
            }
        }

        [Fact]
        public void GetSerializer_UsesDefaultSerializationProvider()
        {
            // Arrange
            var manager = new DesignerSerializationManager();

            // Act
            using (manager.CreateSession())
            {
                var serializer = manager.GetSerializer(typeof(TestClassWithDefaultProvider), typeof(TestSerializerWithDefaultProvider));

                // Assert
                Assert.NotNull(serializer);
            }
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

        [Fact]
        public void IServiceProvider_GetService_ReturnsContainer()
        {
            // Arrange
            using var container = new Container();
            var manager = (IServiceProvider)new DesignerSerializationManager
            {
                Container = container
            };

            // Act
            var service = manager.GetService(typeof(IContainer));

            // Assert
            Assert.Same(container, service);
        }

        [Fact]
        public void IServiceProvider_GetService_DelegatesToProvider()
        {
            // Arrange
            var mockService = new object();
            var mockProvider = new Mock<IServiceProvider>();
            mockProvider.Setup(p => p.GetService(typeof(object))).Returns(mockService);

            var manager = (IServiceProvider)new DesignerSerializationManager(mockProvider.Object);

            // Act
            var service = manager.GetService(typeof(object));

            // Assert
            Assert.Same(mockService, service);
        }

        #endregion

        #region Helper Classes

        private class TestClass
        {
            public string Name { get; set; } = string.Empty;
        }

        private class TestClass2
        {
            public string Name { get; set; } = string.Empty;
        }

        private class TestClass3 : TestClass
        {
        }

        private class TestClassWithConstructor(string value)
        {
            public string Value { get; } = value;
        }

        private class TestClassWithConstructorWithNotConvertibleParameter(string value1, int value2, TestClass3 value3)
        {
            public string Value1 { get; } = value1;
            public int Value2 { get; } = value2;
            public TestClass Value3 { get; } = value3;
        }

        private class TestClassWithConstructorWithParameteInvalidCastException(string value1, int value2, DateTime value3)
        {
            public string Value1 { get; } = value1;
            public int Value2 { get; } = value2;
            public DateTime Value4 { get; } = value3;
        }

        private class TestComponent : Component
        {
        }

        private class TestPropertyProvider
        {
            public string TestProperty { get; set; } = "test";
        }

        [DesignerSerializer(typeof(TestSerializer), typeof(TestSerializer))]
        private class TestClassWithSerializer
        {
        }

        [DesignerSerializer(typeof(TestClass), typeof(TestClass))]
        private class TestClassWithIncorrectSerializer
        {
        }

        [DefaultSerializationProvider(typeof(TestDefaultSerializationProvider))]
        private class TestSerializerWithDefaultProvider
        {
        }

        private class TestClassWithDefaultProvider
        {
        }

        private class TestSerializer
        {
        }

        private class TestSerializerSubclass : TestSerializer
        {
        }

        private class TestDefaultSerializationProvider : IDesignerSerializationProvider
        {
            public object? GetSerializer(IDesignerSerializationManager manager, object? currentSerializer, Type? objectType, Type serializerType)
            {
                if (serializerType == typeof(TestSerializerWithDefaultProvider))
                    return new TestSerializerWithDefaultProvider();
                return null;
            }
        }

        private class CustomSerializationProvider(object serializer) : IDesignerSerializationProvider
        {
            private readonly object serializer = serializer;

            public object? GetSerializer(IDesignerSerializationManager manager, object? currentSerializer, Type? objectType, Type serializerType)
            {
                return serializer;
            }
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