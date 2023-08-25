using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.GDocs.Repositories;
using ICE.GDocs.Domain.ExternalServices.Model;
using Mustache;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Collections.Generic;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Domain.Services;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using System.Threading;
using ICE.GDocs.Common.Core.Exceptions;
using SixLabors.ImageSharp;

namespace ICE.GDocs.Infra.ExternalServices.Email
{
    public class EmailService : IEmailExternalService
    {
        private readonly IEmailRepository _emailRepository;
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsuarioService _usuarioService;


        public EmailService(
            IEmailRepository emailRepository,
            ILogger<EmailService> logger,
            IConfiguration configuration,
            IUsuarioService usuarioService)
        {
            _emailRepository = emailRepository;
            _usuarioService = usuarioService;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<TryException<Return>> Enviar(EmailModel emailModel)
        {
            FormatCompiler compiler = new FormatCompiler();
            if (String.IsNullOrEmpty(emailModel.Template))
                _logger.LogWarning("Não foi encontrado Template para o E-mail.");
            Generator generator = compiler.Compile(emailModel.Template);
            var mensagem = generator.Render(emailModel);

            if (emailModel?.ArquivoBinario?.Length > 0) await this.CriarAnexo(emailModel);

            return await _emailRepository.Enviar(emailModel, mensagem);
        }

      

        public async Task<TryException<Return>> EnviarEmailPorPerfil(EmailModel emailModel, Perfil perfil, CancellationToken cancellationToken)
        {
            var obterUsuariosPorPerfil = await _usuarioService.ListarUsuariosPorPerfil(perfil, cancellationToken);

            if (obterUsuariosPorPerfil.IsFailure)
                return obterUsuariosPorPerfil.Failure;

            if (obterUsuariosPorPerfil.Success == null)
                _logger.LogWarning($"Não foi encontrado usuario para o perfil:{perfil.ToInt32()} - {perfil.GetDescription()}.");

            foreach (var usuario in obterUsuariosPorPerfil.Success)
            {
                emailModel.Destinatario = usuario.Email;
                emailModel.Nome = usuario.Nome;

                var enviouEmail = await Enviar(emailModel);
                if (enviouEmail.IsFailure)
                    return enviouEmail.Failure;
            }
            return Return.Empty;
        }

        private async Task<TryException<Return>> CriarAnexo(EmailModel emailModel)
        {
            var pastaPublicaAnexo = _configuration.GetValue("PastaPublicaAnexo", "");

            if (string.IsNullOrEmpty(pastaPublicaAnexo))
            {
                _logger.LogWarning("A configuração [PastaPublicaAnexo] não existe ou está vazia. Arquivo não anexado!");
            }

            if (!Directory.Exists(pastaPublicaAnexo))
            {
                _logger.LogWarning($"A pasta publica para anexo [{pastaPublicaAnexo}] não existe ou não foi dado acesso.");
            }

            emailModel.CaminhoArquivoAnexo = Path.Combine(pastaPublicaAnexo, $"NFSaida_{emailModel.Numero}_{DateTime.Now: yyyyMMddHHmm}.pdf");
            File.WriteAllBytes(emailModel.CaminhoArquivoAnexo, emailModel.ArquivoBinario);

            return await Task.FromResult(Return.Empty);
        }
    }
}
