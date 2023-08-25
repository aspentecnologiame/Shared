using ICE.GDocs.Common.Core.Domain.ValueObjects;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    public class LogModel
    {
        public TipoLog Tipo { get; private set; }
        public Metadados Metadados { get; private set; }
        public string Dados { get; private set; }
        public DateTime DataCriacao { get; private set; }

        public LogModel()
        {
            DataCriacao = DateTime.Now;
        }

        public LogModel DefinirTipo(TipoLog tipo)
        {
            if (Tipo == default(TipoLog))
                Tipo = tipo;

            return this;
        }

        public LogModel DefinirMetadados(Metadados metadados)
        {
            if (Metadados == default(Metadados))
                Metadados = metadados;

            return this;
        }

        public LogModel DefinirDados<T>(T dados)
        {
            if (Dados == default(string))
            {
                Dados = dados is string ? dados.ToString() : TraceSerializeToXml(dados);
            }

            return this;
        }

        private static string TraceSerializeToXml<T>(T traceObject)
        {
            var objectJson = Newtonsoft.Json.JsonConvert.SerializeObject(
                traceObject
                , new Newtonsoft.Json.Converters.ExpandoObjectConverter()
                , new Newtonsoft.Json.Converters.StringEnumConverter()
            );

            return Newtonsoft.Json.JsonConvert.DeserializeXmlNode(objectJson, typeof(T).Name)?.InnerXml;
        }
    }
}
