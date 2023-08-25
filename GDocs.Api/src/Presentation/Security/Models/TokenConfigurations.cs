using System;

namespace ICE.GDocs.Api.Security
{
    public class TokenConfigurations
    {
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public TimeSpan Validade { get; set; }
        public TimeSpan ValidadeRefresh { get; set; }
    }
}
