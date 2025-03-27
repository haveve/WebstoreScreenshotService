# Website Screenshot Service

The Website Screenshot Service is a .NET 8 application with a React frontend that allows you to capture screenshots of web pages programmatically. It provides a simple API to take screenshots in various formats and configurations.

## Features

- Capture screenshots in PNG and JPEG formats.
- Specify the quality of JPEG screenshots.
- Capture full-page screenshots or specific regions.
- Manage user subscriptions and track usage.

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node v20.0.9

### Installation

1. **Clone the repository**:
    ```git clone https://github.com/your-repo/website-screenshot-service.git```
2. **Navigate to the project directory**:
    ```cd website-screenshot-service```
4. **Run Back End**:
    ``` 
        dotnet restore
        dotnet run
    ```
5. **Run Front End**:
    ```
        cd FrontEnd
    ```
    Create .cert folder:
    ```
        mkdir .cert
    ```
    For starting the front end you need to generate SSL certificates locally. You can use for this purposes [mkcert](https://github.com/FiloSottile/mkcert). 
    ```
        mkcert -cert-file .cert/cert.pem -key-file .cert/key.pem localhost
    ```
    Run the following commands to install dependencies and run the application:
    ```
        npm install
        npm start
    ```

### Changing Development Configuration

To change the development configuration, you need to update both the `.env` file and the `appsettings.Development.json` file.

#### Update the `.env` File

1. Open the `.env` file in your Front End root directory.
2. Add or update the necessary environment variable

#### Update the `appsettings.Development.json` File

1. Open the `appsettings.Development.json` file in Back End root directory.
2. Add or update the necessary configuration settings.

## Project Structure

- **Controllers**: Handle HTTP requests and responses.
- **Model**: Defines data models used in the application.
- **Services**: Contains business logic and operations.
- **Extensions**: Provides extension methods.
- **Utils**: Contains utility classes and validation attributes.
- **Repositories**: Manages data access and storage.
- **Entities**: Defines entities representing data.
- **ServiceExtensions**: Contains service configuration extensions.
- **Properties**: Contains launch and publish settings.
- **docs**: Contains project documentation.

## Documentation

For documentation generation, the 'DocFX' library has been used. Refer to the [API Documentation](WebsiteScreenshotService/docs/index.md).

**Start Documentation Project**:
```
    docfx WebsiteScreenshotService\docs\docfx.json --serve
```

For more information, see [Docfx documentation](https://dotnet.github.io/docfx/).

## Code Practices

Refer to the [Code Practices](WebsiteScreenshotService/docs/docs/code-practices.md) document for best practices on writing and maintaining code in this project.

## Swagger

To view the Swagger documentation, see [Swagger](WebsiteScreenshotService/swagger/swagger.json).

For more information, see [Swagger documentation](https://swagger.io/tools/open-source/getting-started/).

## Storybook

Storybook is used for developing UI components in isolation, see [StoryBook](WebsiteScreenshotService/FrontEnd/.storybook).

**Start Storybook**:
```
    npm install
    npm run storybook
```

For more information, see [StoryBook documentation](https://storybook.js.org/docs).

## Author

- **Name**: Ivan Pohoidash
- **Email**: haveveq@gmail.com
- **GitHub**: [haveveq](https://github.com/haveve)

## GDPR Consent

This project complies with GDPR regulations by obtaining user consent before storing or processing any personal data. Users are informed about the type of data collected and its intended use. For details on GDPR compliance, refer to the GDPR [Compliance document](GDPR.md).

## License

This project is licensed under the **'CC BY-NC-ND 4.0'** License. See the [LICENSE](LICENSE) file for details.
