namespace SutForge.Tests;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class SutBuilderTest
{
    [Test]
    public async Task Services_are_built_by_the_builder()
    {
        // Arrange
        // Act
        var sut = await SutBuilder.Create
            .ConfigureServices(services => { services.AddSingleton<ISomeInterface, SomeImplementation>(); })
            .BuildAsync();

        // Assert
        sut.Should().NotBeNull();
        sut.GetService<ISomeInterface>().Should().NotBeNull().And.BeOfType<SomeImplementation>();
    }

    [Test]
    public async Task Configuration_is_resolved_for_services()
    {
        // Arrange
        // Act
        var sut = await SutBuilder.Create
            .ConfigureServices(services => { services.AddSingleton<ClassWithConfiguration>(); })
            .BuildAsync();

        // Assert
        sut.GetService<ClassWithConfiguration>().Should().NotBeNull();
    }
    
    [Test]
    public async Task Configuration_can_be_customized()
    {
        // Arrange
        // Act
        var sut = await SutBuilder.Create
            .ConfigureServices(services => { services.AddSingleton<ClassWithConfiguration>(); })
            .OnSetupConfiguration(builder => builder.AddInMemoryCollection(new Dictionary<string, string>
            {
                {"Country", "France"}
            }!))
            .BuildAsync();

        var classWithConfiguration = sut.GetRequiredService<ClassWithConfiguration>();
        
        // Assert
        classWithConfiguration.Country.Should().Be("France");
    }

    [Test]
    public async Task Operations_can_be_executed_when_SUT_is_created()
    {
        // Arrange
        // Act
        var sut = await SutBuilder.Create
            .ConfigureServices(services => { services.AddSingleton<ClassWithConfiguration>(); })
            .OnCreatedAsync(sut => sut.GetRequiredService<ClassWithConfiguration>().SetCurrentValue(212))
            .BuildAsync();

        // Assert
        sut.GetRequiredService<ClassWithConfiguration>().CurrentValue.Should().Be(212);
    }
    
    [Test]
    public async Task Exception_thrown_when_configuring_services_is_propagated()
    {
        // Arrange
        // Act
        Func<Task> act = async () => await SutBuilder.Create
            .ConfigureServices(services => throw new InvalidOperationException())
            .BuildAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
    
    [Test]
    public async Task Exception_thrown_when_configuring_configuration_is_propagated()
    {
        // Arrange
        // Act
        Func<Task> act = async () => await SutBuilder.Create
            .OnSetupConfiguration(builder => throw new InvalidOperationException())
            .BuildAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }
    
    [Test]
    public async Task Exception_thrown_when_executing_operations_is_propagated()
    {
        // Arrange
        // Act
        Func<Task> act = async () => await SutBuilder.Create
            .OnCreatedAsync(sut => throw new InvalidOperationException())
            .BuildAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    #region Simple class
    interface ISomeInterface
    {
    }
    
    class SomeImplementation : ISomeInterface
    {
    }

    #endregion
    
    #region Class with dependencies
    class ClassWithConfiguration
    {   
        private readonly IConfiguration _configuration;

        public ClassWithConfiguration(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        public string? Country => _configuration["Country"];

        public int CurrentValue { get; private set; }
        
        public async Task SetCurrentValue(int value)
        {
            await Task.Yield();
            
            CurrentValue = value;
        }
    }
    
    #endregion
}