---
title: "Sample Applications"
permalink: /docs/sample-applications/
---

[Pixel Identity](https://github.com/Nfactor26/pixel-identity) repository contains few sample applications to demonstrate authentication and authorization using Pixel Identity as OpenId Connect service.

- Sample.Service.Api :
    This is a Asp.Net based wep api project. This is configured to use [introspection](docs/introspection) for authorization. Only users having a "read-weather" claim with value "true" should be able to access the api.   

- Samples.Blazor.App and Samples.Blazor.App.Host :
    Samples.Blazor.App is a blazor based client side application that use [Authorization Code with pkce](docs/authorization-code-with-pkce) for authorization.
    Samples.Blazor.App.Host acts as a host which will serve the blazor application. Only users having a "read-weather" claim with value "true" should be able to 
    access the data on "Fetch Data" page. The FetchData page makes call to "api/WeatherForecase" endpoint provided by **Sample.Service.Api** service which uses introspection to validate the token passed by Blazor client.


### Register applications and scopes with OpenIddict

See [authorization code with pkce](/docs/authorization-code-with-pkce) and [introspection](/docs/introspection) to create required scope and onboard applications.


### Create users with and without "read-weather" claim with value of "true"

Register new user accounts if required. Claims can be added directly to users or claims can be added to roles which can be assigned to users.

### Launch the sample application to see authentication and authorization in action

- Launch Sample.Service.Api service
- Launch Samples.Blazor.App.Host host for blazor client
- Navigate to https://localhost:5239
- Page will be redirected to login page on Pixel Identity service
- Login with user email and password
- Page will be redirected to https://localhost:5239 on successful login
- Click Fetch data page from side navigation bar
- If user doesn't have "read-weather" claim with value of "true", user will see unauthorized message.
  ![](/assets/blazor-fetch-data-not-authorized.PNG) 
- If user has "read-weather" claim with value of "true", user will see weather forecase data retrieved from Sample.Service.Api service.
  ![](/assets/data-available-when-authorized.PNG)

### Notes
- Ensure that the sample applications are running on https if pixel identity service is running over https
- If running pixel identity service in docker, you need to make an entry in host file so that pixel.docker.localhost can be resolved correctly. Browsers are able to resolve 
  pixel.docker.localhost without any modification in host file. However, pixel.docker.localhost is not accessible from dotnet applications running outside container.

