using System.Reflection;
using System.Xml;

namespace ICE.GDocs.Common.Core.Domain.ValueObjects
{
    public class MetadadosTrace : Metadados
    {
        protected override string NomeMetadados => "Trace";

        public MetadadosTrace(dynamic metadados) : base(metadados as object)
        {
        }

        public MetadadosTrace(XmlDocument metadadosXml) : base(metadadosXml)
        {
        }

        public MetadadosTrace(string metadadosXmlSerializado) : base(metadadosXmlSerializado)
        {
        }
    }

    public class Metadados : Metadados<Metadados>
    {
        protected override string NomeMetadados => "Metadados";

        public Metadados(dynamic metadados) : base(metadados as object)
        {
        }

        public Metadados(XmlDocument metadadosXml) : base(metadadosXml)
        {
        }

        public Metadados(string metadadosXmlSerializado) : base(metadadosXmlSerializado)
        {
        }

        public static implicit operator Metadados(XmlDocument metadadosXml) =>
            Criar(metadadosXml);

        public static implicit operator Metadados(string metadadosXmlSerializado) =>
            Criar(metadadosXmlSerializado);
    }

    public abstract class Metadados<TMetadados>
        where TMetadados : Metadados<TMetadados>
    {
        protected abstract string NomeMetadados { get; }

        public dynamic Dados { get; protected set; }

        public XmlDocument DadosXml { get; protected set; }

        protected Metadados()
        {
        }

        protected Metadados(dynamic metadados) : this()
        {
            Dados = metadados;
            DadosXml = SerializarMetadadosParaXml(metadados, NomeMetadados);
        }

        protected Metadados(XmlDocument metadadosXml) : this()
        {
            DadosXml = metadadosXml;
            Dados = DeserializarMetadadosDeXml(metadadosXml);
        }

        protected Metadados(string metadadosXmlSerializado) : this()
        {
            DadosXml = CarregarXmlDeString(metadadosXmlSerializado);
            Dados = DeserializarMetadadosDeXml(DadosXml);
        }

        public bool TryParse<T>(out T @object)
        {
            try
            {
                @object = DeserializarMetadadosDeXml<T>(DadosXml);
                return true;
            }
            catch
            {
                @object = default;
                return false;
            }
        }

        public static TMetadados Criar(dynamic metadados) =>
            CriarDinamicamente(metadados);

        public static TMetadados Criar(XmlDocument metadadosXml) =>
            CriarDinamicamente(metadadosXml);

        public static TMetadados Criar(string metadadosXmlSerializado) =>
            CriarDinamicamente(metadadosXmlSerializado);

        private static TMetadados CriarDinamicamente<T>(T metadados) =>
            typeof(TMetadados).GetConstructor(new[] { typeof(T) })
                .Invoke(new object[] { metadados }) as TMetadados;

        public T ExtrairValor<T>(string propertyNamePath) =>
            ReflectionExtensions.GetValueByPropertyNamePath<T>(Dados, propertyNamePath);

        protected static XmlDocument CarregarXmlDeString(string metadadosXmlSerializado)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(metadadosXmlSerializado);
            return xmlDocument;
        }

        protected static XmlDocument SerializarMetadadosParaXml(dynamic metadados, string nomeMetadados)
        {
            try
            {

                var metadadosJson = Newtonsoft.Json.JsonConvert.SerializeObject(
                    metadados
                    , new Newtonsoft.Json.Converters.ExpandoObjectConverter()
                    , new Newtonsoft.Json.Converters.StringEnumConverter()
                );

                return Newtonsoft.Json.JsonConvert.DeserializeXmlNode(
                    metadadosJson
                    , nomeMetadados
                );
            }
            catch
            {
                var metadadosJsonWithObjct = Newtonsoft.Json.JsonConvert.SerializeObject(
                      new { obj = metadados }
                    , new Newtonsoft.Json.Converters.ExpandoObjectConverter()
                    , new Newtonsoft.Json.Converters.StringEnumConverter()
                );

                return Newtonsoft.Json.JsonConvert.DeserializeXmlNode(
                    metadadosJsonWithObjct
                    , nomeMetadados
                );
            }
        }

        protected static dynamic DeserializarMetadadosDeXml(XmlDocument metadadosXml)
        {
            var metadadosJson = Newtonsoft.Json.JsonConvert.SerializeXmlNode(
                metadadosXml
                , Newtonsoft.Json.Formatting.None
                , true
            );

            return Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(
                metadadosJson
                , new Newtonsoft.Json.Converters.ExpandoObjectConverter()
                , new Newtonsoft.Json.Converters.StringEnumConverter()
            );
        }

        protected static T DeserializarMetadadosDeXml<T>(XmlDocument metadadosXml)
        {
            var metadadosJson = Newtonsoft.Json.JsonConvert.SerializeXmlNode(
                metadadosXml
                , Newtonsoft.Json.Formatting.None
                , true
            );

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(
                metadadosJson
                , new Newtonsoft.Json.Converters.StringEnumConverter()
            );
        }

        public override string ToString() =>
            DadosXml?.InnerXml;
    }
}
