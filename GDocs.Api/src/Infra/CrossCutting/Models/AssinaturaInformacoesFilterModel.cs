using System;
using System.Collections.Generic;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class AssinaturaInformacoesFilterModel
    {
        public AssinaturaInformacoesFilterModel()
        {
            Status = new List<int>();
            UsuariosGuidAd = new List<Guid>();
            ListaDeprocessoAssinaturaDocumentoId = new List<int>();
            CategoriaId = new List<int>();
        }

        public int NumeroDocumento { get; set; }
        public string NomeDocumento { get; set; }
        public IEnumerable<int> Status { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataTermino { get; set; }
        public IEnumerable<Guid> UsuariosGuidAd { get; set; }
        public IEnumerable<int> ListaDeprocessoAssinaturaDocumentoId { get; set; }
        public IEnumerable<int> CategoriaId { get; set; }
        public AssinaturaInformacoesOrdenacaoFilterModel Ordenacao { get; set; }
    }

    public class AssinaturaInformacoesOrdenacaoFilterModel
    {
        public string Campo { get; set; }
        public string Direcao { get; set; }
    }
 }
