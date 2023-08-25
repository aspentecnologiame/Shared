using System;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class RefreshTokenModel
    {
        public Guid ActiveDirectoryId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string RefreshToken { get; set; }
    }
}
