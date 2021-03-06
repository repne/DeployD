using System;
using System.Linq;
using Nancy;
using Nancy.ViewEngines;

namespace Deployd.Agent.WebUi
{
    public static class CustomFormatterExtensions
    {
        public static Response AsNegotiated<TModel>(this IResponseFormatter formatter, TModel model, HttpStatusCode statusCode = HttpStatusCode.OK, Tuple<Func<Response>, string> defaultResponse = null)
        {

            if (defaultResponse == null)
                defaultResponse = new Tuple<Func<Response>, string>(() => new Response
                                                                              {
                                                                                  ContentType = null,
                                                                                  StatusCode = HttpStatusCode.UnsupportedMediaType
                                                                              }, null);

            var defaultResponseDelegate = defaultResponse.Item1;
            var defaultContentType = defaultResponse.Item2;

            if (formatter.Context.Request == null || formatter.Context.Request.Headers == null || formatter.Context.Request.Headers.Accept == null)
                return defaultResponseDelegate.Invoke();

            var accept = formatter.Context.Request.Headers.Accept;
            var weightedContentTypes = accept.Select(x => x.Item1).DefaultIfEmpty();

            foreach (var contentType in weightedContentTypes)
            {
                if (defaultContentType == contentType || contentType == "*/*") return defaultResponseDelegate.Invoke();

                var serializer = formatter.Serializers.FirstOrDefault(x => x.CanSerialize(contentType));
                if (serializer != null && !(contentType.EndsWith("xml") && model.IsAnonymousType()))
                {
                    return new Response
                               {
                                   Contents = stream => serializer.Serialize(contentType, model, stream),
                                   ContentType = contentType,
                                   StatusCode = statusCode
                               };
                }
            }
            return defaultResponseDelegate.Invoke();
        }

    }
}