using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using Blueprint.Aspnet.Host.Extensions;
using DeepEqual.Syntax;
using Newtonsoft.Json;
using snowcrashCLR;

namespace Blueprint.Aspnet.Host.Modules
{
    public class BlueprintModule: IHttpModule 
    {
        IEnumerable<snowcrashCLR.Blueprint> _blueprints;

        public String ModuleName
        {
            get { return "BlueprintModule"; }
        }

        public void Init(HttpApplication application)
        {
            LoadBlueprints();
            application.BeginRequest += Application_BeginRequest;
            application.EndRequest += Application_EndRequest;
        }

        private void LoadBlueprints()
        {
            var blueprintPath = ConfigurationManager.AppSettings["BlueprintPath"];
            if (_blueprints == null)
            {
                _blueprints = GetBlueprints(blueprintPath).ToList();
            }
        }

        private void Application_BeginRequest(Object source, EventArgs e)
        {
            // Create HttpApplication and HttpContext objects to access
            // request and response properties.
            var application = (HttpApplication)source;
            var context = application.Context;
            ProcessRequest(context);
        }

        private IEnumerable<snowcrashCLR.Blueprint> GetBlueprints(string path)
        {
            if (!Directory.Exists(path))
                yield break;

            var files = Directory.EnumerateFiles(path, "*.md", SearchOption.AllDirectories).ToList();

            foreach (var file in files)
            {
                using (var reader = new StreamReader(file))
                {
                    var contents = reader.ReadToEnd();
                    snowcrashCLR.Blueprint blueprint;
                    snowcrashCLR.Result result;
                    SnowCrashCLR.parse(contents, out blueprint, out result);
                    if (blueprint != null)
                    {
                        yield return blueprint;
                    }
                }
            }
        }

        private void HandleRequest(HttpRequest actualRequest, HttpResponse actualResponse, Resource resource)
        {
            var action = resource.GetActionsCs().FirstOrDefault(a => a.method.EqualsIgnoreCase(actualRequest.HttpMethod));
            if (action != null)
            {
                var examples = action.GetTransactionExamplesCs();
                foreach (var example in examples)
                {
                    var match = MatchExample(actualRequest, example);
                    if (match)
                    {
                        WriteResponse(actualResponse, example);
                        actualResponse.End();
                        break;
                    }
                }
            }
        }

        private void WriteResponse(HttpResponse actualResponse, TransactionExample example)
        {
            actualResponse.Clear();

            var exampleResponse = example.GetResponsesCs().First();

            // write status code
            if (!string.IsNullOrWhiteSpace(exampleResponse.name))
            {
                int statusCode;
                if(int.TryParse(exampleResponse.name, out statusCode))
                {
                    actualResponse.StatusCode = statusCode;
                }
            }

            // write headers
            exampleResponse
                .GetHeadersCs()
                .ForEach(h => actualResponse.AppendHeader(h.Item1, h.Item2));

            //write body
            actualResponse.Write(exampleResponse.body);
        }

        private bool MatchExample(HttpRequest request, TransactionExample example)
        {
            var payloads = example.GetRequestsCs();
            if (!payloads.Any() || request.HttpMethod == "GET")
                return true;

            var matchingPayloads = 
                from payload in payloads
                let isMatch = MatchPayload(request, payload)
                where isMatch
                select payload;

            return matchingPayloads.Any();
        }

        private bool MatchPayload(HttpRequest actualRequest, Payload payload)
        {
            if (!MatchHeaders(actualRequest.Headers, payload.Headers()))
                return false;

            if (!MatchBody(actualRequest, payload))
                return false;

            return true;
        }

        private bool MatchHeaders(NameValueCollection actualRequestHeaders, NameValueCollection payloadHeaders)
        {
            return actualRequestHeaders.Contains(payloadHeaders);
        }

        private bool MatchBody(HttpRequest request, Payload payload)
        {
            // check if content type matches, only when blueprint has a content type header.
            var payloadContentTypeHeader = payload
                .Headers()
                .Get("content-type");

            if (!string.IsNullOrWhiteSpace(payloadContentTypeHeader))
                if (!request.ContentType.EqualsIgnoreCase(payloadContentTypeHeader))
                    return false;

            if (request.ContentType.EqualsIgnoreCase("application/json"))
                return MatchJson(request.GetBodyString(), payload.body);

            // compare body as a string ignoring whitespace (space, tab, line ending, carriage return)
            return request
                .GetBodyString()
                .EqualsIgnoreWhitespace(payload.body, StringComparison.OrdinalIgnoreCase);
        }

        private bool MatchJson(string requestBody, string payloadBody)
        {
            var requestObject = JsonConvert.DeserializeObject(requestBody);
            var payloadObject = JsonConvert.DeserializeObject(payloadBody);

            return requestObject.IsDeepEqual(payloadObject);
        }

        private void Application_EndRequest(Object source, EventArgs e)
        {
            var application = (HttpApplication)source;
            var context = application.Context;
            context.Request.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public void Dispose() { }

        private void ProcessRequest(HttpContext context)
        {
            LoadBlueprints();
            var request = context.Request;
            var response = context.Response;

            var matchingResources = from b in _blueprints
                from g in b.GetResourceGroupsCs()
                from r in g.GetResourcesCs()
                where r.uriTemplate == request.Path
                select r;

            // we will just take the first resource that matches the path
            var resource = matchingResources.FirstOrDefault();

            if (resource != null)
            {
                HandleRequest(request, response, resource);
            }
        }
    }
}