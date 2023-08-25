using System;
using System.Runtime.Serialization;

namespace ICE.GDocs.Common.Core.Exceptions
{
    [Serializable]
    public class BusinessException : Exception
    {
        private const string CODIGO_ERRO_NEGOCIO_GENERICO = "ERRO_NEGOCIO_GENERICO";

        public virtual new int HResult { get { return 1; } }
        public string Code { get; }

        public BusinessException()
        {
            Code = CODIGO_ERRO_NEGOCIO_GENERICO;
        }

        public BusinessException(string message)
            : base(message)
        {
            Code = CODIGO_ERRO_NEGOCIO_GENERICO;
        }

        public BusinessException(string message, Exception innerException)
            : base(message, innerException)
        {
            Code = CODIGO_ERRO_NEGOCIO_GENERICO;
        }

        protected BusinessException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }


        public BusinessException(string code, string message)
            : this(message)
        {
            Code = code;
        }

        public BusinessException(string code, string message, Exception innerException)
            : base(message, innerException)
        {
            Code = code;
        }

    }
}
