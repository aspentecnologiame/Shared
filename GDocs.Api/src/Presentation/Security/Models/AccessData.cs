namespace ICE.GDocs.Api.Security
{
    public class AccessData
    {
        public string AccessToken { get; set; }
        public long ExpiresInSeconds { get; set; }
        public string RefreshToken { get; set; }

        public UserData UserData { get; set; }
    }
}
