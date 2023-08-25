using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Net;

namespace ICE.GDocs.Domain.Models.Trace
{
    public class TraceRespostaModel
    {
        public TraceRespostaModel(ICollection<KeyValuePair<string, StringValues>> headers)
        {
            Headers = headers;
        }

        public ICollection<KeyValuePair<string, StringValues>> Headers { get; }

        public string ContentType { get; set; }

        public string ContentEncoding { get; set; }

        public long ContentLength { get; set; }

        public HttpStatusCode? StatusCode { get; set; }

        public string ResponseStatus { get; set; }

        public string ErrorMessage { get; set; }

        public string Content { get; set; }
    }
}
