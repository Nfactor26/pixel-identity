---
title: "Authorization code with pkce"
permalink: /docs/authorization-code-with-pkce/
---

Samples.Blazor.App is a blazor based application that uses **Authorization Code with pkce** to authorize users. We need to register a new application on Pixel Identity -> Application management page for onboarding Samples.Blazor.App as a known application.

- Login to Pixel Identity with a user having appropriate access to manage applications and scopes
- Goto Applications page and click the + button to add a new application
- Pick "Authorization Code Flow" preset
- Fill in required details as shown below and add new application
  {% include figure image_path="/assets/add-application-authorization-code-with-pkce.jpg" %}{: .full}
- Add "service-api-scope" custom scope so that Samples.Blazor.App is allowed to access Sample.Service.Api service.
  {% include figure image_path="/assets/assign-scope-to-app.jpg" %}{: .full}