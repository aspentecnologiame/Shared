using System.Collections.Generic;

namespace ICE.GDocs.Infra.CrossCutting.Models
{
    /// <summary>
    /// Padrão de Response Body de erro do serviço.
    /// </summary>
    public class ResponseError : ResponseError<object>
    {
        /// <summary>
        /// Construtor padrão de Response Body de erro do serviço.
        /// </summary>
        public ResponseError()
        {
            Content = null;
        }
    }

    /// <summary>
    /// Padrão de Response Body de erro do serviço.
    /// </summary>
    public class ResponseError<T>
    {
        /// <summary>
        /// Lista de erros encontrados com seu respectivo código e mensagem.
        /// </summary>
        public IEnumerable<Error> Errors { get; set; }

        /// <summary>
        /// Objeto de retorno do serviço que varia de acordo com o método chamado.
        /// </summary>
        public T Content { get; set; }
    }

    /// <summary>
    /// Informações de erros.
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Código do erro.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Mensagem descritiva do erro.
        /// </summary>
        public string Message { get; set; }
    }
}
