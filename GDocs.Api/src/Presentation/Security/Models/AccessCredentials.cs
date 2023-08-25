namespace ICE.GDocs.Api.Security
{
    public class AccessCredentials
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RefreshToken { get; set; }
        public string GuidSignature { get; set; }
        public string GrantType { get; set; }
    }
}
