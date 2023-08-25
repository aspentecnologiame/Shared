using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;

namespace ICE.GDocs.Api.Security
{
    public class UserData
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public IEnumerable<FuncionalidadeModel> Menu { get; set; }
    }
}
