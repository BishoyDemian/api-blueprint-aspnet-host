# api-blueprint-aspnet-host
Host your API Blueprints in an ASP.net website.

## Features
+ Scans a terget folder (and all sub-folders) for blueprint markdown files (*.md)
+ Loads all found markdown files and parse them into blueprint api markdown format using [SnowCrash.net](https://github.com/brutski/snowcrash-dot-net-wrapper)
+ a HTTP Module will intercept any matching request to a blueprint's action throught its URI Template and respond with the blueprint's specified first response.

## Mock Server logic

+ All HTTP requests are intercepted using a HttpModule
+ We try to find a matching blueprintbased on the following logic
  + A Request's path must match a blueprint's URITemplate
  + HTTP Method must match (GET, POST, etc.) OR it is an OPTIONS request
    + *OPTIONS response is still in development for now*
  + Find an example (request/response pair) that match the request *Headers* and *Body*
    + incoming request must contain all header keys and exact values as in the blueprint example
    + Body is then matched differently based on *Content-Type*
      + JSON is deserialised and compared as two in-memeory objects (deep equality check using reflection)
      + everything else is stripped off any whitespace and checked as string equality
  + Respond with the matching example's response and stop any further ASP.net pipeline processing.
+ if no matching blueprint found for that request, then the request continue its journey to the ASP.net pipeline to be processed as normal (be it WebForms, or MVC, or even WebAPI).

## Limitations
+ CORS is not fully supported (specifically the OPIONS verb)
+ URL Parameters are not currently supported
+ Model/Schema is not validated
+ will only capture and use the first (ordinal based) valid *matching* request/response (action) pair from the blueprint.

## Known issues
+ SnowCrash.NET is a CLR/Native C++ DLL compiled in 32bit architecture. You will need some tweak to get it to work.
    1. Install Visual C++ Runtime 2012, you can find it in [this MSDN URL](http://www.microsoft.com/en-au/download/details.aspx?id=30679)
    2. Make sure you enable 32bit applications in your IIS APP Pool.
    3. (Not sure if really needed) Grant filesystem access (read + execute) to your App Pool identity to the physical folder of the website.
