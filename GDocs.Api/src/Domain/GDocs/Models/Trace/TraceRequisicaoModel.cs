using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace ICE.GDocs.Domain.Models.Trace
{
    public class TraceRequisicaoModel
    {
        public TraceRequisicaoModel(ICollection<KeyValuePair<string, StringValues>> headers)
        {
            Headers = headers;
        }

        public string Method { get; set; }

        public string BaseUrl { get; set; }

        public string Resource { get; set; }

        public string QueryString { get; set; }

        public ICollection<KeyValuePair<string, StringValues>> Headers { get; }

        public string Body { get; set; }
    }
}
