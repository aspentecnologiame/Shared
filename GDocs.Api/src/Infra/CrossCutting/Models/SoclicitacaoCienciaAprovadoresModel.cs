using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class SoclicitacaoCienciaAprovadoresModel
    {
        public int IdSolicitacaoCiencia { get; set; }
        public DateTime DataAtualizacao { get; set; }
        public StatusCiencia StatusCiencia { get; set; }
        public IList<CienciaUsuarioAprovacaoModel> CienciaUsuariosAprovacao { get; private set; } = new List<CienciaUsuarioAprovacaoModel>();

        public void AddCienciaUsuario(CienciaUsuarioAprovacaoModel usuario)
        {
            CienciaUsuariosAprovacao.Add(usuario);
        }
    }
}
