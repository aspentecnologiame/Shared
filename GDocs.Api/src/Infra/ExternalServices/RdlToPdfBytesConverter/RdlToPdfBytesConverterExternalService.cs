using ICE.GDocs.Common.Core.Exceptions;
using ICE.GDocs.Domain.ExternalServices;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace ICE.GDocs.Infra.ExternalServices.RdlToPdfBytesConverter
{
    internal class RdlToPdfBytesConverterExternalService : IRdlToPdfBytesConverterExternalService
    {
        const string REGEX_IDENTIFICADOR_ERRO = @"(?<title>Reporting Services Error).*?<ul>(?<message>.*?)<\/ul>";

        private readonly IConfiguration _configuration;
        public RdlToPdfBytesConverterExternalService(
          IConfiguration configuration

        )
        {
            _configuration = configuration;
        }

        public TryException<byte[]> Converter(string rdlPath, Dictionary<string, string> queryStringParameters = null)
        {
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                wc.Credentials = CredentialCache.DefaultCredentials;

                var baseUrl = _configuration.GetValue("Infra:ExternalServices:RdlToPdfBytesConverter:BaseUrl", string.Empty);
                var fileName = $"?{rdlPath}&rs:format=PDF&{ConvertDictonaryToQueryString(queryStringParameters)}";

                byte[] data = wc.DownloadData($"{baseUrl}{fileName}");
                return ValidarSeTemErroNoRetorno(data);
            }
        }

        private TryException<byte[]> ValidarSeTemErroNoRetorno(byte[] data)
        {
            var dataString = System.Text.Encoding.UTF8
                .GetString(data)
                .Replace("\r", "")
                .Replace("\n", "")
                .Replace("\t", "");

            if (Regex.IsMatch(
                dataString,
                REGEX_IDENTIFICADOR_ERRO,
                RegexOptions.IgnoreCase | RegexOptions.Multiline,
                TimeSpan.FromMinutes(1)
            ))
            {
                var matches = Regex.Match(
                    dataString,
                    REGEX_IDENTIFICADOR_ERRO,
                    RegexOptions.IgnoreCase | RegexOptions.Multiline,
                    TimeSpan.FromSeconds(1)
                );

                var titulo = matches.Result("${title}");
                var mensagem = matches.Result("${message}")
                    .Replace("<li>", "")
                    .Replace("<ul>", "")
                    .Replace("</ul>", "")
                    .Replace("</li>", ". ");

                return new BusinessException("RdlToPdfBytesConverterExternalService-Converter", $"{titulo} - {mensagem}");
            }

            return data;
        }

        private string ConvertDictonaryToQueryString(Dictionary<string, string> queryStringParameters)
        {
            if (queryStringParameters == null || !queryStringParameters.Any())
                return "";

            var queryString = string.Join("&", queryStringParameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));

            return queryString;
        }
    }
}
