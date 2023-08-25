using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class DocumentoFI1548CienciaModel
    {
        public int CienciaId { get; set; }
        public int DocumentoFI1548Id { get; set; }
        public int DocumentoFI1548Numero { get; set; }
        public int IdTipoCiencia { get; set; }
        public string TipoCiencia { get; set; }
        public int IdStatusCiencia { get; set; }
        public string StatusCiencia { get; set; }
        public Guid IdUsuario { get; set; }
        public string NomeUsuario { get; set; }
        public string Observacao { get; set; }
        public bool FlgAtivo { get; set; }
        public bool Ciente { get; set; } = false;
        public string MotivoSolicitacao { get; set; }
        public string Justificativa { get; set; }
        public string Fornecedor { get; set; }
        public string MoedaSimbolo { get; set; }
        public decimal ValorTotal { get; set; }
        public string Pagamento { get; set; }
        public string MotivoRejeicao { get; set; }
        public IEnumerable<CienciaUsuarioAprovacaoModel> CienciaUsuarioAprovacao { get; set; }
    }
}
