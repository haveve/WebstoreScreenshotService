# How to Add a New Endpoint

This guide provides step-by-step instructions on how to add a new endpoint to the Website Screenshot Service.

## Step 1: Create a New Controller (if needed)

If your new endpoint belongs to a new category of functionality, you may need to create a new controller. Otherwise, you can add the endpoint to an existing controller.

1. **Create a new controller file** in the `Controllers` directory. For example, `NewFeatureController.cs`.
```csharp
1. using Microsoft.AspNetCore.Mvc;

namespace WebsiteScreenshotService.Controllers
{
    /// <summary>
    /// Controller for handling new feature endpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NewFeatureController : ControllerBase
    {
        // Add your endpoint methods here
    }
}
```

## Step 2: Define the Endpoint Method

Add a new method to your controller to handle the endpoint. Use attributes to specify the HTTP method and route.
```csharp
using Microsoft.AspNetCore.Mvc;

namespace WebsiteScreenshotService.Controllers
{
    /// <summary>
    /// Controller for handling new feature endpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NewFeatureController : ControllerBase
    {
        /// <summary>
        /// Gets the new feature.
        /// </summary>
        /// <returns>A response with the new feature.</returns>
        [HttpGet("new-endpoint")]
        public IActionResult GetNewFeature()
        {
            // Implement your logic here
            return Ok("New feature response");
        }
    }
}
```

## Step 3: Add Any Required Models

If your endpoint requires input or output models, define them in the `Model` directory.
```csharp
namespace WebsiteScreenshotService.Model
{
    /// <summary>
    /// Model for the new feature.
    /// </summary>
    public class NewFeatureModel
    {
        /// <summary>
        /// Gets or sets the first property.
        /// </summary>
        public string Property1 { get; set; }

        /// <summary>
        /// Gets or sets the second property.
        /// </summary>
        public int Property2 { get; set; }
    }
}
```

## Step 4: Implement Business Logic in Services

If your endpoint requires business logic, implement it in a service class in the `Services` directory. Define an interface for the service if needed.
```csharp
namespace WebsiteScreenshotService.Services
{
    /// <summary>
    /// Interface for the new feature service.
    /// </summary>
    public interface INewFeatureService
    {
        /// <summary>
        /// Gets the new feature asynchronously.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. 
        /// The task result contains the new feature response.
        /// </returns>
        Task<string> GetNewFeatureAsync();
    }

    /// <summary>
    /// Service for handling new feature business logic.
    /// </summary>
    public class NewFeatureService : INewFeatureService
    {
        /// <summary>
        /// Gets the new feature asynchronously.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. 
        /// The task result contains the new feature response.
        /// </returns>
        public async Task<string> GetNewFeatureAsync()
        {
            // Implement your business logic here
            return await Task.FromResult("New feature response");
        }
    }
}
```

## Step 5: Register the Service

Register your new service in the `ControllerServicesExtension.cs` file.
```csharp
using Microsoft.Extensions.DependencyInjection;
using WebsiteScreenshotService.Services;

namespace WebsiteScreenshotService.ServiceExtensions
{
    /// <summary>
    /// Extension methods for configuring controller services.
    /// </summary>
    public static class ControllerServicesExtension
    {
        /// <summary>
        /// Adds controller services to the specified service collection.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        public static void AddControllerServices(this IServiceCollection services)
        {
            // Register your new service here
            services.AddScoped<INewFeatureService, NewFeatureService>();
        }
    }
}
```

## Step 6: Update the Controller to Use the Service

Inject the service into your controller and use it in your endpoint method.
```csharp
using Microsoft.AspNetCore.Mvc;
using WebsiteScreenshotService.Services;

namespace WebsiteScreenshotService.Controllers
{
    /// <summary>
    /// Controller for handling new feature endpoints.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class NewFeatureController : ControllerBase
    {
        private readonly INewFeatureService _newFeatureService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewFeatureController"/> class.
        /// </summary>
        /// <param name="newFeatureService">The new feature service.</param>
        public NewFeatureController(INewFeatureService newFeatureService)
        {
            _newFeatureService = newFeatureService;
        }

        /// <summary>
        /// Gets the new feature.
        /// </summary>
        /// <returns>A response with the new feature.</returns>
        [HttpGet("new-endpoint")]
        public async Task<IActionResult> GetNewFeature()
        {
            var result = await _newFeatureService.GetNewFeatureAsync();
            return Ok(result);
        }
    }
}
```


## Step 7: Test the Endpoint

- Run the application(see [distribution](./../index.md#distribution))
- Open your web browser and navigate to http://localhost:5000/swagger (or the appropriate URL for your application). This will open the Swagger UI.

## Conclusion

You have successfully added a new endpoint to the Website Screenshot Service. Follow these steps for any additional endpoints you need to create.
