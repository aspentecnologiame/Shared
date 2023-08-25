using ICE.Framework.Pdf;
using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.GDocs.Enums;
using ICE.GDocs.Domain.GDocs.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Domain.Validation;
using ICE.GDocs.Domain.Validation.ProcessoAssinatura;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using Microsoft.Extensions.Configuration;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Services
{
    internal class ProcessoAssinaturaDocumentoService : DomainService, IProcessoAssinaturaDocumentoService
    {
        private readonly IBinarioRepository _binarioRepository;
        private readonly IInformacaoRepository _informacaoRepository;
        private readonly IPassoRepository _passoRepository;
        private readonly IPassoUsuarioRepository _passoUsuarioRepository;
        private readonly IArquivoRepository _arquivoRepository;
        private readonly IGdocsWorkerExternalService _gdocsWorkerExternalService;
        private readonly ILogService _logService;
        private readonly IConfiguration _configuration;

        private const float FONT_SIZE_NUMERO_DOCUMENTO = 12f;
        private const int POSITION_X_NUMERO_DOCUMENTO = 5;
        private const int POSITION_Y_NUMERO_DOCUMENTO = 5;

        public ProcessoAssinaturaDocumentoService(
            IUnitOfWork unitOfWork,
            IProcessoAssinaturaArquivoRepository processoAssinaturaArquivoRepository,
            IInformacaoRepository informacaoRepository,
            IPassoRepository passoRepository,
            IPassoUsuarioRepository passoUsuarioRepository,
            IGdocsWorkerExternalService gdocsWorkerExternalService,
            ILogService logService,
            IConfiguration configuration) : base(unitOfWork)
        {
            _binarioRepository = processoAssinaturaArquivoRepository.BinarioRepository;
            _informacaoRepository = informacaoRepository;
            _passoRepository = passoRepository;
            _passoUsuarioRepository = passoUsuarioRepository;
            _arquivoRepository = processoAssinaturaArquivoRepository.ArquivoRepository;
            _gdocsWorkerExternalService = gdocsWorkerExternalService;
            _logService = logService;
            _configuration = configuration;
        }

        public async Task<TryException<AssinaturaModel>> Salvar(AssinaturaModel assinatura, string uploadBasePath, List<ConfiguracaoCategoriaModel> configCategorias, CancellationToken cancellationToken)
        {
            var adicionarTemplate = await AdicionarPassosDaDir(assinatura);
            if (adicionarTemplate.IsFailure)
                return adicionarTemplate.Failure;

            AtualizarTipoDeAssinatura(ref assinatura);

            var salvarValidation = await new SalvarValidation(assinatura, configCategorias).ValidateAsync(assinatura);

            if (!salvarValidation.IsValid)
                return salvarValidation.Errors.GroupBy(e => e.ErrorMessage).Select(g => g.First()).ToValidationExceptionCollection();

            var assinaturaDocumentoSalvar = await _informacaoRepository.Salvar(assinatura.Informacoes, cancellationToken);
            if (assinaturaDocumentoSalvar.IsFailure)
                return assinaturaDocumentoSalvar.Failure;
            assinatura.Informacoes = assinaturaDocumentoSalvar.Success;

            await _logService.AdicionarRastreabilidade(assinatura.Informacoes, "aprovação informações foram salvas com sucesso.");

            var incluirPassosRetorno = await IncluirPassos(assinatura, cancellationToken);
            if (incluirPassosRetorno.IsFailure)
                return incluirPassosRetorno.Failure;

            var criarBinariosEIncluirArquivos = await CriarBinariosEIncluirArquivos(assinatura, uploadBasePath, cancellationToken);
            if (criarBinariosEIncluirArquivos.IsFailure)
                return criarBinariosEIncluirArquivos.Failure;

            return assinatura;
        }

        private void AtualizarTipoDeAssinatura(ref AssinaturaModel assinatura)
        {
            assinatura.Informacoes.CertificadoDigital = false;
            assinatura.Informacoes.AssinadoNoDocumento = false;

            if (assinatura.Passos.Itens.SelectMany(item => item.Usuarios).Any(usuario => usuario.AssinarDigitalmente))
                assinatura.Informacoes.CertificadoDigital = true;

            if (assinatura.Passos.Itens.SelectMany(item => item.Usuarios).Any(usuario => usuario.AssinarFisicamente))
                assinatura.Informacoes.AssinadoNoDocumento = true;
        }

        private async Task<TryException<AssinaturaModel>> AdicionarPassosDaDir(AssinaturaModel assinatura)
        {
            if (!assinatura.Passos.AdicionarDir)
                return assinatura;

            var passosDir = assinatura.Passos.ItensDir;

            foreach (var passo in passosDir.OrderBy(passo => passo.Ordem))
            {
                passo.Ordem = assinatura.Passos.Itens.Count() + 1;
                assinatura.Passos.Itens.Add(passo);
            }

            await _logService.AdicionarRastreabilidade(assinatura.Passos.ItensDir, "Passo de aprovação DIR foi adicionado com sucesso.");
            return assinatura;
        }

        private async Task<TryException<AssinaturaModel>> IncluirPassos(AssinaturaModel assinatura, CancellationToken cancellationToken)
        {
            if (!assinatura.Passos.Itens.Any())
                return assinatura;

            var passoSalvar = await _passoRepository.Salvar(assinatura.Informacoes.Id, assinatura.Passos.Itens, cancellationToken);
            if (passoSalvar.IsFailure)
                return passoSalvar.Failure;

            await _logService.AdicionarRastreabilidade(passoSalvar.Success, "Passos de aprovação foram adicionados com sucesso.");

            var passoUsuarioSalvar = await _passoUsuarioRepository.Salvar(passoSalvar.Success, cancellationToken);
            if (passoUsuarioSalvar.IsFailure)
                return passoUsuarioSalvar.Failure;

            assinatura.Passos.DefinirItens(passoUsuarioSalvar.Success.ToList());

            await _logService.AdicionarRastreabilidade(assinatura.Passos, "Aprovadores dos passos de aprovação foram adicionados com sucesso.");

            return assinatura;
        }

        public async Task<TryException<AssinaturaModel>> CriarBinariosEIncluirArquivos(AssinaturaModel assinatura, string uploadBasePath, CancellationToken cancellationToken)
        {
            byte[] binarioArquivoFinal = await ExecutarMergePdf(
                assinatura.Arquivos,
                uploadBasePath,
                cancellationToken,
                (pdfBytes, origemBytesPdf) =>
                {
                    var categoriasEscreverNumeroNoPdf = _configuration.GetSection("CategoriasEscreverNumeroNoPdf")?.Get<int[]>();

                    if (origemBytesPdf == OrigemBytesPdf.BancoDeDados || !categoriasEscreverNumeroNoPdf.Contains(assinatura.Informacoes.CategoriaId))
                    {
                        return pdfBytes;
                    }

                    var font = new Font("Arial", FONT_SIZE_NUMERO_DOCUMENTO, FontStyle.Bold, GraphicsUnit.Point);

                    var drawTextInPdfOptions = DrawTextInPdfOptions
                        .Create(font, new Point(POSITION_X_NUMERO_DOCUMENTO, POSITION_Y_NUMERO_DOCUMENTO));

                    pdfBytes = PdfManager.DrawTextInPdf(pdfBytes, assinatura.Informacoes.NumeroDocumento?.ToString(), drawTextInPdfOptions);

                    return pdfBytes;
                }
            );

            var binarioMergeSalvo = await _binarioRepository.Inserir(binarioArquivoFinal, cancellationToken);
            if (binarioMergeSalvo.IsFailure)
                return binarioMergeSalvo.Failure;

            await _logService.AdicionarRastreabilidade(assinatura, "Binário do processo de aprovação foi adicionado com sucesso.");

            var arquivo = assinatura.Arquivos.First();

            arquivo.BinarioId = binarioMergeSalvo.Success;
            arquivo.Id = 0;
            arquivo.Ordem = 0;

            assinatura.Arquivos = new List<AssinaturaArquivoModel>() { arquivo };

            var arquivoSalvo = await _arquivoRepository.Salvar(assinatura.Informacoes.Id, assinatura.Arquivos, cancellationToken);
            if (arquivoSalvo.IsFailure)
                return arquivoSalvo.Failure;

            assinatura.Arquivos = arquivoSalvo.Success;

            await _logService.AdicionarRastreabilidade(assinatura, "O vinculo do binário do processo de aprovação foi salvo com sucesso.");

            return assinatura;
        }

        public async Task<TryException<AssinaturaInformacoesModel>> CancelarProcesso(AssinaturaInformacoesModel model, CancellationToken cancellationToken)
        {
            var atualizarStatusProcesso = await AtualizarStatusProcessos(new List<int> { model.Id }, StatusAssinaturaDocumento.Cancelado, cancellationToken);

            if (atualizarStatusProcesso.IsFailure)
                return atualizarStatusProcesso.Failure;

            if (atualizarStatusProcesso.Success?.Any() != null)
            {
                await _logService.AdicionarRastreabilidade(model, "Iniciando emissão de eventos de cancelamento de processo de aprovação.");

                foreach (var pad in atualizarStatusProcesso.Success)
                {
                    var emitirAlteracaoStatusDocumento = await _gdocsWorkerExternalService.EmitirAlteracaoStatusDocumento(pad.Id, cancellationToken: cancellationToken);
                    if (emitirAlteracaoStatusDocumento.IsFailure)
                        return emitirAlteracaoStatusDocumento.Failure;

                    await _logService.AdicionarRastreabilidade(pad, "Evento emitido com sucesso.");
                }
            }

            var informacao = atualizarStatusProcesso.Success?.FirstOrDefault();

            informacao?.DefinirStatus(StatusAssinaturaDocumento.Cancelado);

            return informacao;
        }

        public async Task<TryException<IEnumerable<AssinaturaInformacoesModel>>> AtualizarStatusProcessos(IEnumerable<int> processoListId, StatusAssinaturaDocumento status, CancellationToken cancellationToken)
        {
            var atualizarStatusProcesso = await _informacaoRepository.AtualizarStatusProcessos(processoListId, status, cancellationToken);

            if (atualizarStatusProcesso.IsFailure)
                return atualizarStatusProcesso.Failure;

            await _logService.AdicionarRastreabilidade(processoListId, "Processos foram cancelados no banco de dados.");

            return atualizarStatusProcesso;
        }

        public async Task<TryException<Return>> AssinarRejeitar(Guid usuarioAdGuid, IEnumerable<int> listaDeprocessoAssinaturaDocumentoId, StatusAssinaturaDocumentoPassoUsuario status, string justificativaRejeicao, CancellationToken cancellationToken)
           => await _passoUsuarioRepository.AssinarRejeitar(usuarioAdGuid, listaDeprocessoAssinaturaDocumentoId, status, justificativaRejeicao, cancellationToken);

        public async Task<TryException<IEnumerable<AssinaturaPassoAssinanteModel>>> ListarPorPadId(IEnumerable<int> processoAssinaturaDocumentoId, CancellationToken cancellationToken)
            => await _passoUsuarioRepository.ListarPorPadId(processoAssinaturaDocumentoId, cancellationToken);

        private async Task<byte[]> ExecutarMergePdf(
            IEnumerable<AssinaturaArquivoModel> listaArquivosPdf,
            string uploadBasePath,
            CancellationToken cancellationToken,
            Func<byte[], OrigemBytesPdf, byte[]> beforeAddPdfPages = null
        )
        {
            byte[] arquivoBinarioPdf = null;

            using (var pdfMergeStream = new MemoryStream())
            using (var pdfMergeDocument = new PdfDocument())
            {
                foreach (var arquivo in listaArquivosPdf.OrderBy(order => order.Ordem))
                {
                    byte[] pdfBytes = null;
                    var origemBytesPdf = OrigemBytesPdf.Arquivo;

                    if (arquivo.BinarioId == 0)
                        pdfBytes = File.ReadAllBytes(Path.Combine(uploadBasePath, arquivo.NomeSalvo));
                    else
                    {
                        pdfBytes = (await _binarioRepository.ObterPorId(arquivo.BinarioId, cancellationToken)).Success;
                        origemBytesPdf = OrigemBytesPdf.BancoDeDados;
                    }

                    if (beforeAddPdfPages != null)
                        pdfBytes = beforeAddPdfPages.Invoke(pdfBytes, origemBytesPdf);

                    using (var pdfStream = new MemoryStream(pdfBytes))
                    using (var pdfDocument = PdfReader.Open(pdfStream, PdfDocumentOpenMode.Import))
                    {
                        AddPdfPages(pdfDocument, pdfMergeDocument);
                    }
                }

                pdfMergeDocument.Save(pdfMergeStream);

                arquivoBinarioPdf = pdfMergeStream.ToArray();
            }

            return arquivoBinarioPdf;
        }

        private void AddPdfPages(PdfDocument pdfFrom, PdfDocument pdfTo)
        {
            for (int i = 0; i < pdfFrom.PageCount; i++)
                pdfTo.AddPage(pdfFrom.Pages[i]);
        }

        public async Task<TryException<Return>> InativarPassos(int processoAssinaturaDocumentoId, CancellationToken cancellationToken) => 
            await _passoRepository.InativarPassos(processoAssinaturaDocumentoId,cancellationToken);

        public async Task<TryException<Return>> InativarUsuariosDosPassos(int processoAssinaturaDocumentoId, CancellationToken cancellationToken) =>
            await _passoUsuarioRepository.InativarUsuariosDosPassos(processoAssinaturaDocumentoId, cancellationToken);

        public async Task<TryException<IEnumerable<AssinaturaPassoItemModel>>> ObterPassosEhUsuariosPorId(int processoAssinaturaDocumentoId, CancellationToken cancellationToken) =>
            await _passoRepository.ObterPassosEhUsuariosPorId(processoAssinaturaDocumentoId, cancellationToken);
    }
}
