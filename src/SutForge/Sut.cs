namespace SutForge;

/// <summary>
/// Represents a System Under Test. This SUT implements <see cref="IServiceProvider"/> to allow to retrieve services from the <see cref="IServiceProvider"/> used to build the SUT.
/// </summary>
public class Sut : IServiceProvider
{
    private IServiceProvider _serviceProviderImplementation;

    internal Sut(IServiceProvider serviceProviderImplementation)
    {
        _serviceProviderImplementation = serviceProviderImplementation;
    }

    /// <inheritdoc />
    public object? GetService(Type serviceType)
    {
        return _serviceProviderImplementation.GetService(serviceType);
    }
}