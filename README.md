# api-blueprint-aspnet-host
Host your API Blueprints in an ASP.net website.

## Features
+ Scans a terget folder (and all sub-folders) for blueprint markdown files (*.md)
+ Loads all found markdown files and parse them into blueprint api markdown format using [SnowCrash.net](https://github.com/brutski/snowcrash-dot-net-wrapper)
+ a HTTP Module will intercept any matching request to a blueprint's action throught its URI Template and respond with the blueprint's specified first response.
+ Matching is done based on the following
  + Request's path must match a blueprint's URITemplate
  + HTTP Method must match (GET, POST, etc.) OR it is an OPTIONS request
    + OPTIONS response is still in development for now
  + find an example (request/response pair) that match the request *Headers* and *Body*
    + incoming request must contain all header keys and exact values as in the blueprint example
    + Body is then matched differently based on *Content-Type*
      + JSON is deserialised and compared as two in-memeory objects (deep equality check using reflection)
      + everything else is stripped off any whitespace and checked as string equality
  + Respond with the matching example's response and stop any further ASP.net pipeline processing.


## Limitations
+ CORS is not fully supported (specifically the OPIONS verb)
+ URL Parameters are not currently supported
+ Model/Schema is not validated
+ will only capture and use the first (ordinal based) valid *matching* request/response (action) pair from the blueprint.

## Known issues
+ Loading a native DLL in an ASP.net host process (SnowCrash dependency) might fail under IIS.
  + Currently Under investigation, but a look at [this article](http://blogs.msdn.com/b/jorman/archive/2007/08/31/loading-c-assemblies-in-asp-net.aspx) might be usefull
