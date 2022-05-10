---
title: "Introspection"
permalink: /docs/introspection/
---


Sample.Service.Api is a dotnet web api application that uses "Introspection" to validate tokens passed by clients that tries to access service endpoints in order to authorize access. We need to register a new application on Pixel Identity -> Application management page for onboarding Samples.Service.Api as a known application. 

A custom scope also needs to be created. The custom scope should have a resource with same name as client-id used while onboarding application. These custom scopes must be assigned to clients that want to consume this service.

### Creating a custom scope

- Login to Pixel Identity with a user having appropriate access to manage applications and scopes
- Goto Scopes page and click the + button to add a custom scope
- Fill in required details as shown below
  ![](/assets/add-scope-for-api-service.PNG)
- Add scope

### Onboarding Sample.Serice.Api application

- Goto Applications page and click the + button to add a new application
- Pick "Introspection" preset
- Fill in required details as shown below 
  ![](/assets/add-application-with-introspection.PNG)
- Add application 