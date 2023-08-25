using ICE.GDocs.Common.Core.Exceptions;
using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.GDocs.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Domain.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs
{
    internal class AssinaturaUsuarioService : DomainService, IAssinaturaUsuarioService
    {
        private readonly IAssinaturaArmazenadaUsuarioRepository _assinaturaArmazenadaUsuarioRepository;
        private readonly IBinarioRepository _binarioRepository;
        private readonly ILogService _logService;
        private const string UPLOAD_ASSINATURA_NAO_ENCONTRADA = "Não foi encontrada assinatura do usuário responsável pelo documento.";
        private const string UPLOAD_LISTA_ASSINATURA_NAO_ENCONTRADA = "Não foi encontrada uma ou mais assinaturas do(s) usuário(s) informado(s).";

        public AssinaturaUsuarioService
        (
            IAssinaturaArmazenadaUsuarioRepository assinaturaArmazenadaUsuarioRepository,
            IBinarioRepository binarioRepository,
            ILogService logService,
            IUnitOfWork unitOfWork
            ) : base(unitOfWork)
        {
            _logService = logService;
            _assinaturaArmazenadaUsuarioRepository = assinaturaArmazenadaUsuarioRepository;
            _binarioRepository = binarioRepository;
        }

        public async Task<TryException<Return>> SalvarArquivoBase64(string arquivoBase64AssinaturaUsuario, Guid usuarioGuid, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(arquivoBase64AssinaturaUsuario))
            {
                return new BusinessException("nenhum-arquivo-selecionado", $"Nenhum arquivo selecionado.");
            }

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                await _logService.AdicionarRastreabilidade(new
                {
                    usuarioGuid = usuarioGuid
                }, "SalvarArquivoBase64 iniciado.");

                var arquivoSalvo = await SalvarArquivo(Convert.FromBase64String(arquivoBase64AssinaturaUsuario), usuarioGuid, cancellationToken);
                if (arquivoSalvo.IsFailure)
                    return arquivoSalvo.Failure;

                transaction.Commit();
            }

            return Return.Empty;
        }

        public async Task<TryException<List<AssinaturaArmazenadaUsuarioModel>>> ObterAssinaturasDoUsuarioPorListagemDeGuid(List<Guid> listaGuid, CancellationToken cancellationToken)
		{
            var assinaturaArmazenadaUsuario = await _assinaturaArmazenadaUsuarioRepository.ListarPorListagemDeUsuario(listaGuid, cancellationToken);

            if (assinaturaArmazenadaUsuario.IsFailure)
                return assinaturaArmazenadaUsuario.Failure;

            var assinaturas = assinaturaArmazenadaUsuario.Success;

            if (assinaturas == null || (assinaturas.Count() == 1 && !assinaturas.First().BinarioAssinaturaDocumentoId.HasValue))
                return new BusinessException("upload-assinatura-nao-encontrada", UPLOAD_ASSINATURA_NAO_ENCONTRADA);

            if (listaGuid.Count != assinaturas.Count())
                return new BusinessException("upload-assinatura-nao-encontrada", UPLOAD_LISTA_ASSINATURA_NAO_ENCONTRADA);

            return assinaturas.OrderBy(o => listaGuid.IndexOf(o.Guid)).ToList();
        }

        private async Task<TryException<Return>> SalvarArquivo(byte[] arquivo, Guid usuarioGuid, CancellationToken cancellationToken)
        {
            var binarioSalvo = await _binarioRepository.Inserir(arquivo, cancellationToken);
            if (binarioSalvo.IsFailure)
                return binarioSalvo.Failure;

            await _logService.AdicionarRastreabilidade(new
            {
                Id = binarioSalvo.Success
            }, "Binário salvo com sucesso.");

            var assinaturaArmazenadaUsuario = await _assinaturaArmazenadaUsuarioRepository.Salvar(binarioSalvo.Success, usuarioGuid, cancellationToken);
            if (assinaturaArmazenadaUsuario.IsFailure)
                return assinaturaArmazenadaUsuario.Failure;

            await _logService.AdicionarRastreabilidade(new
            {
                UsuarioGuid = usuarioGuid
            }, "Arquivo salvo com sucesso.");

            return Return.Empty;
        }
    }
}
