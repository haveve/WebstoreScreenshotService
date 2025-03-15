---
_layout: landing
---

# Welcome to the Website Screenshot Service Documentation

This documentation provides information on how to develop the Website Screenshot Service. It includes details on the available features, setup instructions, project organization, code writing practices, and distribution methods.

## Features

- Capture screenshots in PNG and JPEG formats.
- Specify the quality of JPEG screenshots.
- Capture full-page screenshots or specific regions.
- Manage user subscriptions and track usage.

## Getting Started

1. **Add a new endpoint**: [create a new endpoint](docs/create-a-new-endpoint.md)

## Project Organization

The project is organized into several key components (see [Documentation](api/WebsiteScreenshotService.Controllers.yml) for more information):

- **Controllers**: Handle HTTP requests and responses.
- **Model**: Defines data models used in the application.
- **Services**: Contains business logic and operations.
- **Extensions**: Provides extension methods.
- **Utils**: Contains utility classes and validation attributes.
- **Repositories**: Manages data access and storage.
- **Entities**: Defines entities representing data.
- **ServiceExtensions**: Contains service configuration extensions.

## Code Writing Practices

Details about core practices you can find in [Code Practices](docs/code-practices.md)

- **Asynchronous Methods**: Use async methods for better performance.
- **Extension Methods**: Use extension methods for cleaner code.
- **Validation Attributes**: Use custom validation attributes for data validation.
- **Interfaces**: Use interfaces for flexibility and testability.

## Distribution
To get started with the Website Screenshot Service, follow these steps:

1. **Clone the repository**:
```
git clone https://github.com/your-repo/website-screenshot-service.git
```

2. **Navigate to the project directory**:
```
cd website-screenshot-service
```

3. **Install dependencies**:
```
dotnet restore
```

4. **Run the application**:
```
dotnet run
```

To publish the project, use the following command:
This will create a build of the project in the `./publish` directory, ready for deployment.

## License

This project is licensed under the MIT License. See the LICENSE file for details.
