namespace SutForge;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Allows to build a <see cref="Sut"/> (System Under Test) with a custom configuration.
/// </summary>
public class SutBuilder
{
    private Action<IServiceCollection> configureServices = _ => { };
    private Action<ConfigurationBuilder> configurationBuilders = _ => { };
    private Func<Sut, Task> onCreated = _ => Task.CompletedTask;

    protected SutBuilder()
    {
    }

    public static SutBuilder Create => new();

    /// <summary>
    /// Builds the <see cref="Sut"/> (System Under Test) with the provided configuration.
    /// </summary>
    /// <returns>An instance of <see cref="Sut"/> ready to be tested</returns>
    public async Task<Sut> BuildAsync()
    {
        // Create the empty service collection and the configuration
        var services = new ServiceCollection();
        var configurationBuilder = new ConfigurationBuilder();
        
        // Execute the registered configuration builders
        this.configurationBuilders(configurationBuilder);
        
        // Add the configuration to the service collection
        services.AddSingleton<IConfiguration>(configurationBuilder.Build());

        // Execute the registered services configuration
        this.configureServices(services);

        // Build the service provider
        var serviceProvider = services.BuildServiceProvider();

        // Create the SUT and execute the registered operations
        var sut =  new Sut(serviceProvider);
        await this.onCreated(sut);
        
        return sut; 
    }

    /// <summary>
    /// Registers an action to configure the services of the <see cref="Sut"/> (System Under Test).
    /// </summary>
    public SutBuilder ConfigureServices(Action<IServiceCollection> configureServices)
    {
        this.configureServices += configureServices;

        return this;
    }

    /// <summary>
    /// Registers an action to configure a <see cref="IConfiguration"/> which will be passed to the <see cref="OnCreatedAsync"/> methods.
    /// </summary>
    public SutBuilder OnSetupConfiguration(Action<ConfigurationBuilder> builder)
    {
        this.configurationBuilders += builder;
        
        return this;
    }

    /// <summary>
    /// Registers an action to execute when the <see cref="Sut"/> (System Under Test) is created.
    /// </summary>
    public SutBuilder OnCreatedAsync(Func<Sut, Task> operation)
    {
        this.onCreated += operation;
        
        return this;
    }
}