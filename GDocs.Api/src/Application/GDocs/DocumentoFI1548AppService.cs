using ICE.GDocs.Common.Core.Exceptions;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.GDocs.Services.SolicitacaoSaidaMaterial;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Domain.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.DocumentoFI1548.Enum;
using ICE.GDocs.Infra.CrossCutting.Models.DocumentoFI1548.ViewModel;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs
{
    internal class DocumentoFI1548AppService : IDocumentoFI1548AppService
    {
        private const string CANCELAR_CODIGOERRO = "documentofi1548:cancelar";
        private const string LIQUIDAR_CODIGOERRO = "documentofi1548:liquidar";
        private const string PDF_NAO_ENCONTRADO = "documentofi1548:obterbase64dopdf:pdf:naoencontrado";
        private const string DOCUMENTO_NAO_ENCONTRADO = "documentofi1548:obterbase64dopdf:documento:naoencontrado";

        private readonly ILogService _logService;
        private readonly IConfiguration _configuration;
        private readonly IDocumentoFI1548Service _documentoFI1548Service;
        private readonly IDocumentoFI1548Repository _documentoFI1548Repository;
        private readonly IProcessoAssinaturaDocumentoOrigemRepository _processoAssinaturaDocumentoOrigemRepository;
        private readonly IActiveDirectoryExternalService _activeDirectoryExternalService;
        private readonly IBinarioRepository _binarioRepository;

        public DocumentoFI1548AppService(
                ILogService logService,
                IConfiguration configuration,
                IDocumentoFI1548Service documentoFI1548Service,
                IDocumentoFI1548Repository documentoFI1548Repository,
                IProcessoAssinaturaDocumentoOrigemRepository processoAssinaturaDocumentoOrigemRepository,
                IActiveDirectoryExternalService activeDirectoryExternalService,
                IBinarioRepository binarioRepository
        )
        {
            _logService = logService;
            _configuration = configuration;
            _documentoFI1548Service = documentoFI1548Service;
            _documentoFI1548Repository = documentoFI1548Repository;
            _processoAssinaturaDocumentoOrigemRepository = processoAssinaturaDocumentoOrigemRepository;
            _activeDirectoryExternalService = activeDirectoryExternalService;
            _binarioRepository = binarioRepository;
        }

        public async Task<TryException<DocumentoFI1548Model>> Adicionar(DocumentoFI1548Model documento, UsuarioModel usuarioModel, CancellationToken cancellationToken)
        {
            await _logService.AdicionarRastreabilidade(documento, "Iniciando cadastro de documento.");

            documento?.DefinirAutor(usuarioModel.ActiveDirectoryId, usuarioModel.Nome);

            await _logService.AdicionarRastreabilidade(documento, "Autor definido.");

            var adicionarDocumento = await _documentoFI1548Service.Adicionar(documento, cancellationToken);

            if (adicionarDocumento.IsFailure)
                return adicionarDocumento.Failure;

            await _logService.AdicionarRastreabilidade(documento, "Documento cadastrado.");

            return adicionarDocumento.Success;
        }

        public async Task<TryException<DocumentoFI1548Model>> CancelarOuSolicitarCiencia(int documentoId, string motivo, UsuarioModel usuarioModel, bool permiteCancelarTodos, CancellationToken cancellationToken)
        {
            await _logService.AdicionarRastreabilidade(documentoId, "Iniciando cancelamento de documento.");

            var documento = await ObterPorId(documentoId, cancellationToken);

            if (documento.IsFailure)
                return documento.Failure;

            if (documento.Success == null)
                return new BusinessException(CANCELAR_CODIGOERRO, "Documento não encontrado.");

            await _logService.AdicionarRastreabilidade(documento, "Documento encontrado.");

            if (documento.Success.Status == Infra.CrossCutting.Models.Enums.DocumentoFI1548Status.EmAberto)
                return await EnviarParaCiencia(documento.Success, motivo, usuarioModel, cancellationToken);
            else
                return await Cancelar(documento.Success, usuarioModel, permiteCancelarTodos, cancellationToken);
        }

        private async Task<TryException<DocumentoFI1548Model>> EnviarParaCiencia(DocumentoFI1548Model documento, string motivo, UsuarioModel usuarioModel, CancellationToken cancellationToken)
        {
            var documentoFI1548CienciaModel = new DocumentoFI1548CienciaModel
            {
                DocumentoFI1548Id = documento.Id,
                IdUsuario = usuarioModel.ActiveDirectoryId,
                FlgAtivo = true,
                Observacao = motivo,
                IdStatusCiencia = (int)Infra.CrossCutting.Models.Enums.DocumentoFI1548StatusCiencia.Pendente,
                IdTipoCiencia = (int)Infra.CrossCutting.Models.Enums.DocumentoFI1548TipoCiencia.Cancelamento
            };

            var registroCiencia = await _documentoFI1548Service.InserirSolicitacaoCiencia(documentoFI1548CienciaModel, cancellationToken);

            if (registroCiencia.IsFailure) return registroCiencia.Failure;

            documento.Status = Infra.CrossCutting.Models.Enums.DocumentoFI1548Status.AprovacaoCiencia;
            var trocaStatus = await _documentoFI1548Service.AtualizarStatusDoDocumento(documento, cancellationToken);

            if (trocaStatus.IsFailure) return trocaStatus.Failure;

            return await Task.FromResult(documento);
        }

        public async Task<TryException<DocumentoFI1548Model>> Cancelar(DocumentoFI1548Model documento, UsuarioModel usuarioModel, bool permiteCancelarTodos, CancellationToken cancellationToken)
        {
            byte[] arquivo = default;
            if (documento.Status == Infra.CrossCutting.Models.Enums.DocumentoFI1548Status.EmConstrucao || documento.Status == Infra.CrossCutting.Models.Enums.DocumentoFI1548Status.PendenteAprovacao)
            {
                var binarioObterPorId = await _binarioRepository.ObterPorId(documento.BinarioId.Value, cancellationToken);
                if (binarioObterPorId.IsFailure)
                    return binarioObterPorId.Failure;

                arquivo = binarioObterPorId.Success;

                var carimboCancelado = Convert.FromBase64String(_configuration.GetValue("Ice:PdfManager:Carimbos:Cancelado", string.Empty));
                arquivo = Framework.Pdf.PdfManager.CarimbarDocumento(arquivo, carimboCancelado);
            }

            var documentoCancelado = await _documentoFI1548Service.Cancelar(documento, usuarioModel, permiteCancelarTodos, cancellationToken, arquivo);

            if (documentoCancelado.IsFailure)
                return documentoCancelado.Failure;

            await _logService.AdicionarRastreabilidade(documento, "Documento foi salvo.");

            return documentoCancelado.Success;
        }

        public async Task<TryException<DocumentoFI1548Model>> Liquidar(DocumentoFI1548Model model, bool permiteLiquidar, string uploadPathBase, CancellationToken cancellationToken)
        {
            await _logService.AdicionarRastreabilidade(model, "Iniciando liquidação do documento.");

            var documentoExiste = await ObterPorNumero(model.Numero, cancellationToken);

            if (documentoExiste.IsFailure)
                return documentoExiste.Failure;

            if (documentoExiste.Success == null)
                return new BusinessException(LIQUIDAR_CODIGOERRO, "Documento não encontrado.");

            documentoExiste.Success.DefinirNomeArquivoSalvo(model.NomeSalvo);

            await _logService.AdicionarRastreabilidade(documentoExiste.Success, "Documento encontrado.");

            var arquivo = File.ReadAllBytes(Path.Combine(uploadPathBase, documentoExiste.Success.NomeSalvo));

            var carimboLiquidado = Convert.FromBase64String(_configuration.GetValue("Ice:PdfManager:Carimbos:Liquidado", string.Empty));
            arquivo = Framework.Pdf.PdfManager.CarimbarDocumento(arquivo, carimboLiquidado);

            var documentoLiquidado = await _documentoFI1548Service.Liquidar(documentoExiste.Success, permiteLiquidar, arquivo, cancellationToken);

            if (documentoLiquidado.IsFailure)
                return documentoLiquidado.Failure;

            await _logService.AdicionarRastreabilidade(documentoExiste, "Documento foi salvo.");

            return documentoLiquidado.Success;
        }

        public async Task<TryException<DocumentoFI1548Model>> ObterPorId(int id, CancellationToken cancellationToken)
        => await this.Listar(false, id, cancellationToken);

        public async Task<TryException<DocumentoFI1548Model>> ObterPorNumero(int numero, CancellationToken cancellationToken)
        => await this.Listar(true, numero, cancellationToken);

        public async Task<TryException<IEnumerable<DocumentoFI1548Model>>> ObterPorNumeroTipoPagamentoAutorPeriodo(
            DocumentoFI1548FilterModel filtro,
            CancellationToken cancellationToken
        )
        {
            await _logService.AdicionarRastreabilidade(filtro, "Iniciando a busca de documento com o filtro Selecionado.");

            filtro.Ordenar = await this.DefinirOrdenacaoRelatorioDocumentoFI1548(filtro.Ordenar);
            var response = await _documentoFI1548Repository.Listar(filtro, cancellationToken);

            if (response.IsFailure)
                return response.Failure;

            await _logService.AdicionarRastreabilidade(filtro, $"{response.Success.Count()} resultado encontrado.");

            var usuarios = _activeDirectoryExternalService.GetActiveDirectoryUsers(string.Empty, response.Success.Select(x => x.AutorId).Distinct().AsList());

            if (usuarios.IsFailure)
                return usuarios.Failure;

            var results = response.Success.Join(usuarios.Success,
                      d => d.AutorId,
                      u => u.Guid,
                      (doc, usu) => doc.DefinirAutor(usu.Nome)
                    );

            await _logService.AdicionarRastreabilidade(filtro, $"Total de {results.Count()} documenos após junção com os autores ad.");

            return  results?.ToCollection();
        }

        public async Task<TryException<IEnumerable<DocumentoFI1548StatusModel>>> ListarStatusPagamento(CancellationToken cancellationToken)
            => await _documentoFI1548Repository.ListarStatusPagamento(cancellationToken);

        public async Task<TryException<IEnumerable<DropDownCustonModel>>> ListarReferenciaSubstitutos(Guid usuarioLogadoAd, CancellationToken cancellationToken)
        {
            var resultListaSubistitutos = await _documentoFI1548Repository.ListarSubstitutoRejeitadoAssinatura(usuarioLogadoAd, cancellationToken);
            if (resultListaSubistitutos.IsFailure)
                return resultListaSubistitutos.Failure;

            var resultAprovadoPorCiencia = await _documentoFI1548Repository.ListarSubstitutoAprovadosPorCiencia(usuarioLogadoAd, cancellationToken);
            if (resultAprovadoPorCiencia.IsFailure)
                return resultAprovadoPorCiencia.Failure;

            var resultAguardandoCiencia = await _documentoFI1548Repository.ListarSubstitutoAguardandoCiencia(usuarioLogadoAd, cancellationToken);
            if (resultAguardandoCiencia.IsFailure)
                return resultAguardandoCiencia.Failure;

            return ProcessaListas(resultListaSubistitutos, resultAprovadoPorCiencia, resultAguardandoCiencia).ToCollection();  
        }

        private IEnumerable<DropDownCustonModel> ProcessaListas(TryException<IEnumerable<DropDownCustonModel>> resultListaSubistitutos, TryException<IEnumerable<DropDownCustonModel>> resultAprovadoPorCiencia, TryException<IEnumerable<DropDownCustonModel>> resultAguardandoCiencia)
        {
            var ListaOficial = new List<DropDownCustonModel>();
            ListaOficial.AddRange(resultListaSubistitutos.Success.ToList());
            ListaOficial.AddRange(resultAprovadoPorCiencia.Success.ToList());
            ListaOficial.AddRange(resultAguardandoCiencia.Success.ToList());

            return ListaOficial;
        }

        public async Task<TryException<IEnumerable<AssinaturaArquivoModel>>> EnviarParaAssinatura(DocumentoEhAssinaturaPassosModel documentoEhAssinaturaPassosModel, CancellationToken cancellationToken)
        {
            await _logService.AdicionarRastreabilidade(documentoEhAssinaturaPassosModel.DocumentoModel, $"Iniciando envio da FI {documentoEhAssinaturaPassosModel.DocumentoModel.Id} para assinatura.");

            var processoAssinaturaDocumentoOrigemNome = _configuration.GetValue("DocumentoFI1548:ProcessoAssinaturaDocumentoOrigemNome", string.Empty);

            var processoAssinaturaDocumentoOrigem = await _documentoFI1548Service.ListarPorProcessoAssinaturaDocumentoOrigemIdENome(documentoEhAssinaturaPassosModel.DocumentoModel.Id.ToString(), processoAssinaturaDocumentoOrigemNome, cancellationToken);
            if (processoAssinaturaDocumentoOrigem.IsFailure)
                return processoAssinaturaDocumentoOrigem.Failure;

            if (processoAssinaturaDocumentoOrigem.Success != null)
            {
                var inativarFluxo = await _documentoFI1548Service.InativarFluxoAssinatura(processoAssinaturaDocumentoOrigem.Success.ProcessoAssinaturaDocumentoId, cancellationToken);

                if (inativarFluxo.IsFailure)
                  return inativarFluxo.Failure;

            }

            var titulo = string.Format(_configuration.GetValue("DocumentoFI1548:ProcessoAssinaturaTitulo", string.Empty), documentoEhAssinaturaPassosModel.DocumentoModel.Numero);
            var nomeDocumento = _configuration.GetValue("DocumentoFI1548:ProcessoAssinaturaNomeDocumento", string.Empty);
            var categoriaId = _configuration.GetValue("DocumentoFI1548:ProcessoAssinaturaCategoriaId", 0);

            var enviarParaAssinatura = await _documentoFI1548Service.EnviarParaAssinatura(documentoEhAssinaturaPassosModel, processoAssinaturaDocumentoOrigemNome, titulo, nomeDocumento, categoriaId, cancellationToken);
            if (enviarParaAssinatura.IsFailure)
                return enviarParaAssinatura.Failure;


            var listaArquivoUpload =  await _documentoFI1548Service.ListarArquivosUploadPorPadId(enviarParaAssinatura.Success, cancellationToken, true);
            if (listaArquivoUpload.IsFailure)
                return listaArquivoUpload.Failure;


            await _logService.AdicionarRastreabilidade(documentoEhAssinaturaPassosModel.DocumentoModel, $"Finalizou envio da FI {documentoEhAssinaturaPassosModel.DocumentoModel.Id} para assinatura. ProcessoAssinaturaDocumentoId: {enviarParaAssinatura.Success}");

            return listaArquivoUpload.Success.ToCollection();
        }

        public async Task<TryException<string>> ObterBase64DoPdf(int documentoId, CancellationToken cancellationToken)
        {
            var documentoObterPorId = await ObterPorId(documentoId, cancellationToken);

            if (documentoObterPorId.IsFailure)
                return documentoObterPorId.Failure;

            if (!documentoObterPorId.Success.ExisteBinario())
                return new BusinessException(DOCUMENTO_NAO_ENCONTRADO, $"Documento não está mais disponível para visualização. Temporalidade expirada.");

            if (documentoObterPorId.Success == null)
                return new BusinessException(DOCUMENTO_NAO_ENCONTRADO, $"Não foi encontrado nenhum documento com o id {documentoId}.");

            var documento = documentoObterPorId.Success;

            var binarioObterPorId = await _binarioRepository.ObterPorId(documento.BinarioId.Value, cancellationToken);

            if (binarioObterPorId.IsFailure)
                return binarioObterPorId.Failure;

            if (binarioObterPorId.Success == null)
                return new BusinessException(PDF_NAO_ENCONTRADO, "PDF não encontrado.");

            return Convert.ToBase64String(binarioObterPorId.Success);
        }

        private async Task<TryException<DocumentoFI1548Model>> Listar(bool obterPorNumero, int value, CancellationToken cancellationToken)
        {
            var filter = obterPorNumero ? new DocumentoFI1548FilterModel { Numero = value } : new DocumentoFI1548FilterModel { Id = value };
            filter.Ordenar = await this.DefinirOrdenacaoRelatorioDocumentoFI1548(filter.Ordenar);
            var response = await _documentoFI1548Repository.Listar(filter, cancellationToken);

            if (response.IsFailure)
                return response.Failure;

            return response.Success?.FirstOrDefault();
        }

        private async Task<DocumentoFI1548OrdenacaoModel> DefinirOrdenacaoRelatorioDocumentoFI1548(DocumentoFI1548OrdenacaoModel ordenacao)
        {
            ordenacao = ordenacao ?? new DocumentoFI1548OrdenacaoModel();
            switch (ordenacao.Active)
            {
                case "dataCriacao":
                    ordenacao.OrdenarPor = $"dfi_dat_criacao {ordenacao.Direction}, dfi.[dfi_numero] asc";
                    break;

                case "numero":
                    ordenacao.OrdenarPor = $"dfi.[dfi_numero] {ordenacao.Direction}";
                    break;

                case "referencia":
                    ordenacao.OrdenarPor = $"dfi.[dfi_referencia] {ordenacao.Direction}, dfi.[dfi_numero] asc";
                    break;

                case "tipoPagamento":
                    ordenacao.OrdenarPor = $"dfi.[dfi_pagamento] {ordenacao.Direction}, dfi.[dfi_numero] asc";
                    break;

                case "valor":
                    ordenacao.OrdenarPor = $"dfi.[dfi_valor_total] {ordenacao.Direction}, dfi.[dfi_numero] asc";
                    break;

                case "status":
                    ordenacao.OrdenarPor = $"sdo.[sdo_des] {ordenacao.Direction}, dfi.[dfi_numero] asc";
                    break;

                default:
                    ordenacao.Direction = "Desc";
                    ordenacao.OrdenarPor = $"dfi_dat_criacao {ordenacao.Direction}, dfi.[dfi_numero] asc";
                    break;
            }

            return await Task.FromResult(ordenacao);
        }

        public async Task<TryException<IEnumerable<AssinaturaPassoItemModel>>> ObterPassosEhUsuariosPorId(int documentoId, CancellationToken cancellationToken)
        {
             var documentoOrigem = _configuration.GetValue("DocumentoFI1548:ProcessoAssinaturaDocumentoOrigemNome", string.Empty);
             var result = await  _documentoFI1548Service.ObterPassosEhUsuariosPorId(documentoId,documentoOrigem, cancellationToken);

            if (result.IsFailure)
                return result.Failure;


            var usuarios = _activeDirectoryExternalService.GetActiveDirectoryUsers(string.Empty, result.Success.SelectMany(x => x.Usuarios).Select(x => x.Guid).ToList());

            if (usuarios.IsFailure)
                return usuarios.Failure;

            result.Success.SelectMany(x => x.Usuarios).Join(usuarios.Success,
                   d => d.Guid,
                   u => u.Guid,
                   (doc, usu) => doc.DefinirNome(usu.Nome)
                 ).ToCollection();

            return result;
        }  

        public async Task<TryException<ProcessoAssinaturaDocumentoOrigemModel>> ObterOrigemDocumento(int documentoId, CancellationToken cancellationToken)
        {
            var processoAssinaturaDocumentoOrigemNome = _configuration.GetValue("DocumentoFI1548:ProcessoAssinaturaDocumentoOrigemNome", string.Empty);

            var processoAssinaturaDocumentoOrigem = await _processoAssinaturaDocumentoOrigemRepository.ListarPorProcessoAssinaturaDocumentoOrigemIdENome(documentoId.ToString(), processoAssinaturaDocumentoOrigemNome, cancellationToken);
            if (processoAssinaturaDocumentoOrigem.IsFailure)
                return processoAssinaturaDocumentoOrigem.Failure;


            return processoAssinaturaDocumentoOrigem;

        }

        public async Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarCienciasPendentesDeAprovacaoPorUsuario(Guid activeDirectoryId, CancellationToken cancellationToken)
        => await _documentoFI1548Service.ListarCienciasPendentesDeAprovacaoPorUsuario(activeDirectoryId, cancellationToken);

        public async Task<TryException<DocumentoFI1548CienciaModel>> ObterCienciaPorId(int idSolicitacaoCiencia, CancellationToken cancellationToken)
        {
            var solicitacaoCiencia = await _documentoFI1548Service.ObterCienciaPorId(idSolicitacaoCiencia, cancellationToken);

            if (solicitacaoCiencia.IsFailure)
                return solicitacaoCiencia.Failure;

            var usuarios = _activeDirectoryExternalService.GetActiveDirectoryUsers(string.Empty, new List<Guid> { solicitacaoCiencia.Success.IdUsuario });

            if (usuarios.IsFailure)
                return usuarios.Failure;

            solicitacaoCiencia.Success.NomeUsuario = usuarios.Success.First()?.Nome;

            var result = await _documentoFI1548Service.ListarObsUsuarioPorCiencia(idSolicitacaoCiencia, cancellationToken);

            if (result.IsFailure)
                return result.Failure;

            var usuarioAd = _activeDirectoryExternalService.GetActiveDirectoryUsers(string.Empty, result.Success.Select(x => x.UsuarioGuid).AsList());

            if (usuarioAd.IsFailure)
                return usuarioAd.Failure;

            var usuariosAprovacao = result.Success.Join(usuarioAd.Success,
                r => r.UsuarioGuid,
                u => u.Guid,
                (result, usuAd) => result.DefinirNome(usuAd.Nome));


            solicitacaoCiencia.Success.CienciaUsuarioAprovacao = usuariosAprovacao.ToCollection();

            return solicitacaoCiencia;
        }

        public async Task<TryException<SoclicitacaoCienciaAprovadoresModel>> ObterAprovadoresCienciaCancelamento(int documentoId, CancellationToken cancellationToken)
        {
         var result = await _documentoFI1548Service.ObterAprovadoresCienciaCancelamento(documentoId, cancellationToken);

            if (result.IsFailure)
                return result.Failure;

            if (result.Success == null)
                return result;

            var usuarioAd = _activeDirectoryExternalService.GetActiveDirectoryUsers(string.Empty, result.Success.CienciaUsuariosAprovacao.Select(x => x.UsuarioGuid).AsList());

            if (usuarioAd.IsFailure)
                return usuarioAd.Failure;

            result.Success.CienciaUsuariosAprovacao.Join(usuarioAd.Success,
               r => r.UsuarioGuid,
               u => u.Guid,
               (result, usuAd) => result.DefinirNome(usuAd.Nome)).ToCollection();

            return result;
        }

        public async Task<TryException<Return>> RegistroCienciaPorUsuario(DocumentoFI1548CienciaModel documentoFI1548CienciaModel, UsuarioModel usuarioModel, CancellationToken cancellationToken)
        {
            var destinatario = _activeDirectoryExternalService.GetActiveDirectoryUsers(documentoFI1548CienciaModel.NomeUsuario, new List<Guid> { documentoFI1548CienciaModel.IdUsuario });

            if (destinatario.IsFailure)
                return destinatario.Failure;

            usuarioModel.Email = destinatario.Success?.First()?.Email;
            var result = await _documentoFI1548Service.RegistroCienciaPorUsuario(documentoFI1548CienciaModel, usuarioModel, cancellationToken);

            if (result.IsFailure)
                return result.Failure;

            if (result.Success && documentoFI1548CienciaModel.IdTipoCiencia == (int)TipoCienciaFi1548.Cancelamento)
                await CancelarOuSolicitarCiencia(documentoFI1548CienciaModel.DocumentoFI1548Id, null, usuarioModel, true, cancellationToken);

            return Return.Empty;
        }


        public async Task<TryException<IEnumerable<AssinaturaInformacoesModel>>> ObterReferenciaSubstitutoOrigemDocumento(long documentoSubstitutoId, CancellationToken cancellationToken)
        => await _documentoFI1548Service.ObterReferenciaSubstitutoOrigemDocumento(documentoSubstitutoId, cancellationToken);

        public async Task<TryException<IEnumerable<ObterMotivoCancelamentoModel>>> ObterMotivoCancelamento(int documentoId, CancellationToken cancellationToken)
        => await _documentoFI1548Service.ObterMotivoCancelamento(documentoId, cancellationToken);

    }
}
