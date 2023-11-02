### .Net Core 3.1 API with SQL server Code First

This is base framework for .Net Core 3.1 API with SQL server Code First. The JWT based authentication and Swagger is already implemented in it.

#### NuGet Dependancies 

 - Automapper: Used to transform model object.
 - FluentValidation: Used to validate the data from requests
 - SendGrid: Used as mail client
 - SharpRaven: Used for communicating with Sentry for tracking exceptions
 - IdentityModel.Tokens.Jwt: Token management library
 - Swashbukle.AspNetCore: Swagger documentation

#### Appsettings Configuration
 - Signing Key: Signing key used to sign JSON Web Tokens
 - Connection String: SQL Server connection string
 - JWT Issuer Options: JWT Issuer Options section
     - Issuer: Issuers of the JWT
     - Audience: Audience of the JWT

Example:

```
{
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
      "Default": "Debug",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "RabbitMqOptions": {
	"HostName": "<Rabbit MQ Host Name>"
  },
  "SigningKey": "<Signing Key",
  "ConnectionString": "<SQL Server String>",
  "JwtIssuerOptions": {
    "Issuer": "<Issuer Identity>",
    "Audience": "http://localhost:5001/"
  },
  "SentryCredentials": {
    "SentryUri": "<SentryUrl>"
    "ServiceName": "JetDevs API"
  },
  "EmailSettings": {
    "FromAddress": "support@JetDevs.com",
    "FromName": "JetDevs Portal",
    "SendGridKey": "<SendGridKey>"
  }
}
```