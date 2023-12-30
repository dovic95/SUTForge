# SUT Forge

<p align="center">
    <img src="favicon.png" alt="Icon" />
</p>

Simplifying .NET unit testing and Test-Driven Development with an intuitive builder pattern, classical approach, and high extensibility for effortless creation and customization of System Under Test instances.

## Introduction

Experience unit testing and Test-Driven Development (TDD) in a whole new light with `SUTForge`, a simple yet powerful .NET package meticulously designed to streamline the creation of System Under Test (SUT) instances. 

`SUTForge` follows the Chicago School to unit testing (also referred to as the Classical School of unit testing), offering a clean and efficient alternative to the over-mocking issues associated with the London approach. 

What sets `SUTForge` apart is its high degree of extensibility, allowing developers to craft personalized extension methods and tailor SUTs to their specific testing needs.


**Key Features**:

- **Effortless TDD Integration**: `SUTForge` seamlessly integrates with Test-Driven Development (TDD), enabling developers to write tests first and then effortlessly construct and configure their SUTs using the intuitive builder pattern provided by `SUTForge`.

- **Chicago School of Unit Testing**: Embrace the Chicago approach of unit testing, emphasizing simplicity and readability. `SUTForge` encourages clean and maintainable tests without succumbing to over-mocking pitfalls, ensuring your unit tests remain valuable assets throughout the development lifecycle.

- **Intuitive Builder Pattern**: Constructing SUTs becomes a breeze with `SUTForge`'s intuitive builder pattern. Say goodbye to the complexities of manual setup and embrace a syntax that makes unit test code concise, expressive, and easy to understand.

- **Highly Extensible**: `SUTForge` goes beyond the basics, offering developers a high degree of extensibility. Write your own extension methods to customize SUTs according to your unique testing requirements. Tailor `SUTForge` to fit seamlessly into your specific development workflow.

💪 Elevate your unit testing and TDD experience with `SUTForge`—a library that not only embraces the Chicago approach but also empowers you to extend and customize your testing capabilities. Your journey to efficient and maintainable tests starts here!

## Getting Started

To get started with `SUTForge`, you need to install the package in your .NET project. Once installed, you can start writing your unit tests using the `SUTForge`'s intuitive builder pattern.

### Installation

Install `SUTForge` via NuGet package manager:

```shell
Install-Package SUTForge
```

### Writing Your First Test

Here's an example of how you can write a unit test using `SUTForge`. This test checks whether a service is correctly built by the builder.

```csharp
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
```

In the above test, we're using the `SutBuilder.Create` method to create a new instance of the SUT builder. We then configure the services by adding a singleton service of type `ISomeInterface` with an implementation of `SomeImplementation`. After building the SUT, we assert that the SUT is not null and that the service of type `ISomeInterface` is not null and is of type `SomeImplementation`.

### Resolving Configuration for Services

`SUTForge` also allows you to resolve configurations for services. Here's an example:

Considering the following class that make use of `IConfiguration`:

```csharp
class ClassWithConfiguration
{   
    private readonly IConfiguration _configuration;

    public ClassWithConfiguration(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // ...
}
```

In this test, we're adding a service of type `ClassWithConfiguration` that requires configuration:

```csharp
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
```

 After building the SUT, we assert that the service of type `ClassWithConfiguration` is not null, implying that the configuration was resolved correctly.

### Customizing Configuration

`SUTForge` also allows you to customize the configuration. Here's an example:

```csharp
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
        }))
        .BuildAsync();

    // Assert
    var service = sut.GetService<ClassWithConfiguration>();
    service.Should().NotBeNull();
    service.Configuration["Country"].Should().Be("France");
}
```

In this test, we're customizing the configuration by adding an in-memory collection with a key-value pair of "Country" and "France". After building the SUT, we assert that the service of type `ClassWithConfiguration` is not null and that the configuration value for "Country" is "France".

### Extending SUTForge

`SUTForge` is highly extensible, allowing you to use existing extension methods on `IServiceCollection` or to write your own extension methods to customize SUTs according to your unique testing requirements. Here's an example:

```csharp   

public static class SutBuilderExtensions
{
    public static SutBuilder WithCurrentTime(this SutBuilder builder, DateTime time)
    {
        return builder.ConfigureServices(services => { services.AddSingleton<ITimeProvider(new DeterministicTimeProvider(time)); });
    }
}   
```

In this example, we're writing an extension method that allows us to customize the SUT by adding a singleton service of type `ITimeProvider` with an implementation of `DeterministicTimeProvider` to control the time of the SUT. This extension method can then be used in our tests as follows:

```csharp   
[Test]
public async Task SUT_can_be_customized()
{
    // Arrange
    var time = 14.January(2021).At(12, 0, 0).AsUtc(); // Using FluentAssertions
    
    var sut = await SutBuilder.Create
        .AddMyApplicationServices() // 💡 Add the application services (the same method you'll use in your Startup class)
        .WithCurrentTime(time)
        .BuildAsync();
    
    var service = sut.GetRequiredService<ISomeFancyService>();
    
    // Act
    var result = service.DoSomething();

    // Assert
    // Assert something
}
```

With `SUTForge`, writing unit tests becomes a breeze. Embrace the Chicago approach of unit testing and elevate your TDD experience with `SUTForge`. Happy testing!

## References

- [Unit Testing Principles, Practices, and Patterns](https://www.goodreads.com/en/book/show/48927138) (Vladimir Khorikov) 
- [🚀 TDD, Where Did It All Go Wrong (Ian Cooper)](https://www.youtube.com/watch?v=EZ05e7EMOLM)
- Uncle Bob (Robert C. Martin), TDD Harms Architecture https://blog.cleancoder.com/uncle-bob/2017/03/03/TDD-Harms-Architecture.html (On a shift from London to Chicago School of TDD; About fallacy of <class>Test file pattern)
- [TDD Revisited - Ian Cooper - NDC London 2021](https://www.youtube.com/watch?v=vOO3hulIcsY) (on testing “ports&adapters” architecture with Chicago School of TDD)