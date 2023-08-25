using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.GDocs.Repositories.SaidaMaterialNotaFiscal;
using ICE.GDocs.Domain.GDocs.Repositories.SolicitacaoSaidaMaterial;
using ICE.GDocs.Domain.GDocs.Services.SaidaMaterialNotaFiscal.Interface;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Domain.Repositories;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ICE.GDocs.Domain.GDocs.Enums;
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.Drawing;
using ICE.Framework.Pdf;
using ICE.GDocs.Common.Core.Exceptions;
using ICE.GDocs.Infra.CrossCutting.Models.SaidaMaterialNotaFiscal.Enums;
using Microsoft.Extensions.Configuration;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.ExternalServices.Model;
using ICE.GDocs.Domain.Services;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using FluentValidation;
using ICE.GDocs.Domain.GDocs.Validation.SaidaMaterialNotaFiscal;
using ICE.GDocs.Domain.Validation;
using ICE.GDocs.Domain.Models;

namespace ICE.GDocs.Domain.GDocs.Services.SaidaMaterialNotaFiscal.Service
{
    internal class SaidaMaterialNotaFiscalService : DomainService, ISaidaMateriaNotaFiscalService
    {
        private readonly ISaidaMaterialNotaFiscalRepository _saidaMaterialNotaFiscalRepository;
        private readonly ISaidaMaterialNotaFiscalItemRepository _saidaMaterialItemNotaFiscalRepository;
        private readonly ISaidaMaterialAnexoRepository _saidaMaterialAnexoRepository;
        private readonly IBinarioRepository _binarioRepository;
        private readonly IConfiguration _configuration;
        private readonly IEmailExternalService _emailExternalService;
        private readonly IUsuarioService _usuarioService;



        private const string REGISTRO_NAO_ENCONTRADO = "saidaMaterialNf:obterbase64dopdf:registrosaidamaterial:naoencontrado";
        private const string PDF_NAO_ENCONTRADO = "saidaMaterialNf:obterbase64dopdf:pdf:naoencontrado";

        public SaidaMaterialNotaFiscalService(
            ISaidaMaterialNotaFiscalRepository saidaMaterialNotaFiscalRepository,
            IUnitOfWork unitOfWork,
            ISaidaMaterialNotaFiscalItemRepository saidaMaterialItemNotaFiscalRepository,
            IBinarioRepository binarioRepository,
            ISaidaMaterialAnexoRepository saidaMaterialAnexoRepository,
            IConfiguration configuration,
            IEmailExternalService emailExternalService,
            IUsuarioService usuarioService) : base(unitOfWork)
        {
            _saidaMaterialNotaFiscalRepository = saidaMaterialNotaFiscalRepository;
            _saidaMaterialItemNotaFiscalRepository = saidaMaterialItemNotaFiscalRepository;
            _binarioRepository = binarioRepository;
            _saidaMaterialAnexoRepository = saidaMaterialAnexoRepository;
            _configuration = configuration;
            _emailExternalService = emailExternalService;
            _usuarioService = usuarioService;
        }

        public async Task<TryException<IEnumerable<SaidaMaterialNotaFiscalModel>>> ConsultaMaterialNotaFiscalPorFiltro(
            SaidaMaterialNotaFiscalFilterModel saidaMaterialNotaFiscalFiltroModel, CancellationToken cancellationToken, bool buscarNfCancelada = false) =>
            await _saidaMaterialNotaFiscalRepository.ConsultaMaterialNotaFiscalPorFiltro(saidaMaterialNotaFiscalFiltroModel, cancellationToken, buscarNfCancelada);


        public async Task<TryException<SaidaMaterialNotaFiscalModel>> Inserir(SaidaMaterialNotaFiscalModel saidaMaterialNotaFiscalModel, CancellationToken cancellationToken)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var cadastroValidadtion = await new CadastroValidation(saidaMaterialNotaFiscalModel).ValidateAsync(saidaMaterialNotaFiscalModel);
                if (!cadastroValidadtion.IsValid)
                    return cadastroValidadtion.Errors.ToValidationExceptionCollection();

                var result = await _saidaMaterialNotaFiscalRepository.Inserir(saidaMaterialNotaFiscalModel, cancellationToken);
                if (result.IsFailure)
                    return result.Failure;


                if (saidaMaterialNotaFiscalModel.Fornecedor != null)
                {

                    var fornecedorValidadtion = await new FornecedorValidator(saidaMaterialNotaFiscalModel.Fornecedor).ValidateAsync(saidaMaterialNotaFiscalModel.Fornecedor);
                    if (!fornecedorValidadtion.IsValid)
                        return fornecedorValidadtion.Errors.ToValidationExceptionCollection();

                    var resultFornecedor = await _saidaMaterialNotaFiscalRepository.InserirFornecedor(saidaMaterialNotaFiscalModel, cancellationToken);
                    if (resultFornecedor.IsFailure)
                        return result.Failure;
                }

                foreach (var ItemMaterialNotaFiscal in saidaMaterialNotaFiscalModel.ItemMaterialNf)
                {
                    ItemMaterialNotaFiscal.IdSolicitacaoSaidaMaterialNF = saidaMaterialNotaFiscalModel.Id;

                    var itemValitadtion = await new SaidaMaterialNotaFiscalItemValidation(ItemMaterialNotaFiscal).ValidateAsync(ItemMaterialNotaFiscal);
                    if (!itemValitadtion.IsValid)
                        return itemValitadtion.Errors.ToValidationExceptionCollection();

                    var resultItem = await _saidaMaterialItemNotaFiscalRepository.Inserir(ItemMaterialNotaFiscal, cancellationToken);
                    if (resultItem.IsFailure)
                        return resultItem.Failure;
                }

                var envioEmail = await _emailExternalService.EnviarEmailPorPerfil(new EmailModel()
                {
                    Assunto = $"Pendência de Emissão de NF",
                    Template = _configuration.GetSection("TemplatesEmail:SaidaMaterialNotaFiscal-Cadastro").Value,
                    Link = $@"{_configuration.GetSection("Site:BaseUrl").Value}/SaidaMaterialNotaFiscal/uploadNotaFiscal/{saidaMaterialNotaFiscalModel.Id}",
                }, Perfil.Contabilidade, cancellationToken);

                if (envioEmail.IsFailure)
                    return envioEmail.Failure;

                _unitOfWork.Commit();

                return result.Success;
            }
        }

        public async Task<TryException<IEnumerable<DropDownModel>>> ListarModalidadeFrete(CancellationToken cancellationToken) =>
            await _saidaMaterialNotaFiscalRepository.ListarModalidadeFrete(cancellationToken);


        public async Task<TryException<IEnumerable<DropDownModel>>> ListarNatureza(CancellationToken cancellationToken) =>
            await _saidaMaterialNotaFiscalRepository.ListarNatureza(cancellationToken);

        public async Task<TryException<SaidaMaterialNotaFiscalModel>> ObterMaterialNotaFiscalPorId(int id, CancellationToken cancellationToken)
        {

            var resultMaterial = await _saidaMaterialNotaFiscalRepository.ObterMaterialNotaFiscalPorId(id, cancellationToken);


            var nomeResponsavel = await _usuarioService.ObterUsuarioActiveDirectoryPorId(resultMaterial.Success.GuidAutor, cancellationToken);

            if (nomeResponsavel.IsFailure)
                return nomeResponsavel.Failure;

            resultMaterial.Success.DefinirNomeAutor(nomeResponsavel.Success.Nome);
            resultMaterial.Success.DefinirEmailAutor(nomeResponsavel.Success.Email);


            return resultMaterial;

        }

        public async Task<TryException<IEnumerable<DropDownModel>>> ListarStatusMaterial(CancellationToken cancellationToken) =>
            await _saidaMaterialNotaFiscalRepository.ListarStatusMaterial(cancellationToken);

        public async Task<TryException<IEnumerable<DropDownModel>>> ListarStatusMaterialFiltro(UsuarioModel usuario, CancellationToken cancellationToken)
        {
            List<DropDownModel> retornoListaStatusMaterial = new List<DropDownModel>();
            var perfisUsuario = await _usuarioService.ListarPerfisUsuarioPorGuid(usuario.ActiveDirectoryId, cancellationToken);
            var perfisUsuarioFiltro = perfisUsuario.Success.Perfis.Where(x => x.Peso > 0).ToList();
            var configuracaoPerfilStatusSaidaMaterial = _configuration.GetSection("ConfiguracaoPerfilStatusSaidaMaterial")?.Get<ConfiguracaoPerfilStatusSaidaMaterialModel[]>();
            int pesoPerfil = 0;
            int perfilPrioritario = 0;
            foreach (var p in perfisUsuarioFiltro)
            {
                if ((p.Peso > 0 && p.Peso < pesoPerfil) || (p.Peso > 0 && pesoPerfil == 0))
                {
                    pesoPerfil = p.Peso;
                    perfilPrioritario = p.Id;
                }
            }

            if (perfilPrioritario != 0)
            {
                var configuracaoFiltroPadraoPerfil = configuracaoPerfilStatusSaidaMaterial.Where(p => p.Perfil == perfilPrioritario).ToList();
                var listaStatusMaterial = await _saidaMaterialNotaFiscalRepository.ListarStatusMaterial(cancellationToken);

                foreach (var c in configuracaoFiltroPadraoPerfil)
                {
                    retornoListaStatusMaterial.Add(listaStatusMaterial.Success.FirstOrDefault(i => i.Id == c.Filtro));

                }

            }

            return retornoListaStatusMaterial;

        }



        public async Task<TryException<Return>> SalvarArquivosUploadSaida(IEnumerable<SaidaMaterialArquivoModel> saidaMaterialArquivoModel, string uploadBasePath, Guid usuarioLogado, CancellationToken cancellationToken)
        {
            if (!saidaMaterialArquivoModel.Any()) return Return.Empty;

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var jaExisteNotaDeSaidaComEsseNumero = await _saidaMaterialAnexoRepository.CheckarDuplicidade(saidaMaterialArquivoModel.First().SaidaMaterialNfId, saidaMaterialArquivoModel.First().Numero, cancellationToken);

                if (jaExisteNotaDeSaidaComEsseNumero.IsFailure)
                    return jaExisteNotaDeSaidaComEsseNumero.Failure;

                if (jaExisteNotaDeSaidaComEsseNumero.Success > 0)
                    return new BusinessException($"Já existe uma nota de saída com o número {saidaMaterialArquivoModel.First().Numero} para essa saída de material.");

                var resultUpload = await CriarBinariosEIncluirArquivosSaida(saidaMaterialArquivoModel, uploadBasePath, usuarioLogado, cancellationToken);

                if (resultUpload.IsFailure)
                    return resultUpload.Failure;

                var resultStatus = await _saidaMaterialNotaFiscalRepository.AtualizarStatusPendenteSaidaEhNumero(saidaMaterialArquivoModel.First(), cancellationToken);
                if (resultStatus.IsFailure)
                    return resultStatus.Failure;

                _unitOfWork.Commit();
                return Return.Empty;
            }

        }

        public async Task<TryException<Return>> SalvarArquivosUploadRetorno(SaidaMaterialNotaFiscalModel saidaMaterialNotaFiscalModel, IEnumerable<SaidaMaterialArquivoModel> saidaMaterialArquivoModel, string uploadBasePath, Guid usuarioLogado, CancellationToken cancellationToken)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var resultUpload = await CriarBinariosEIncluirArquivosRetorno(saidaMaterialArquivoModel, uploadBasePath, usuarioLogado, cancellationToken);

                if (resultUpload.IsFailure)
                    return resultUpload.Failure;

                var resultStatus = await AtualizarStatusRegistroRetorno(saidaMaterialNotaFiscalModel, cancellationToken);
                if (resultStatus.IsFailure)
                    return resultStatus.Failure;

                _unitOfWork.Commit();
                return Return.Empty;
            }
        }


        private async Task<TryException<Return>> AtualizarStatusRegistroRetorno(
                 SaidaMaterialNotaFiscalModel saidaMaterialNotaFiscalModel,
                 CancellationToken cancellationToken)
        {
            var solicitacaoSaidaMaterialItemRepository = await _saidaMaterialItemNotaFiscalRepository.ObterPorIdSolicitacaoSaidaMaterial(saidaMaterialNotaFiscalModel.Id, cancellationToken);
            if (solicitacaoSaidaMaterialItemRepository.IsFailure)
                return solicitacaoSaidaMaterialItemRepository.Failure;

            var totalItemMaterialBaixado = await _saidaMaterialItemNotaFiscalRepository.ObterQuantidadeItemComBaixa(saidaMaterialNotaFiscalModel.Id, cancellationToken);
            if (totalItemMaterialBaixado.IsFailure) return totalItemMaterialBaixado.Failure;

            if (solicitacaoSaidaMaterialItemRepository.Success.Count() == totalItemMaterialBaixado.Success)
                await _saidaMaterialNotaFiscalRepository.AtualizarStatus(saidaMaterialNotaFiscalModel, SaidaMaterialNotaFiscalStatus.Concluida, cancellationToken);
            else
                await _saidaMaterialNotaFiscalRepository.AtualizarStatus(saidaMaterialNotaFiscalModel, SaidaMaterialNotaFiscalStatus.EmAberto, cancellationToken, true);

            return Return.Empty;
        }


        public async Task<TryException<Return>> CriarBinariosEIncluirArquivosRetorno(IEnumerable<SaidaMaterialArquivoModel> saidaMaterialArquivoModel, string uploadBasePath, Guid usuarioLogado, CancellationToken cancellationToken)
        {
            var anexoRetorno = await _saidaMaterialAnexoRepository.ObterAnexoPorIdEhTipo(saidaMaterialArquivoModel.First().SaidaMaterialNfId, TipoDocAnexo.Retorno, cancellationToken);

            if (anexoRetorno.IsFailure)
                return anexoRetorno.Failure;

            if (anexoRetorno.Success != null)
            {
                var novaListaAnexo = saidaMaterialArquivoModel.ToList();
                novaListaAnexo.Add(anexoRetorno.Success);

                saidaMaterialArquivoModel = novaListaAnexo.ToCollection();
                anexoRetorno.Success.DefinirAutor(usuarioLogado);
                var documentoInativo = await _saidaMaterialAnexoRepository.InativaAnexo(anexoRetorno.Success, cancellationToken);
                if (documentoInativo.IsFailure)
                    return documentoInativo.Failure;

            }

            byte[] BinarioRetorno = await ExecutarMergePdf(
                saidaMaterialArquivoModel,
                uploadBasePath,
                cancellationToken);


            var resultBinario = await _binarioRepository.Inserir(BinarioRetorno, cancellationToken);
            if (resultBinario.IsFailure)
                return resultBinario.Failure;

            var saidaMaterialArquivoFinal = saidaMaterialArquivoModel.First();
            saidaMaterialArquivoFinal.DefinirBinarioId(resultBinario.Success);
            saidaMaterialArquivoFinal.DefinirAutor(usuarioLogado);

            var salvarAnexo = await _saidaMaterialAnexoRepository.Salvar(saidaMaterialArquivoFinal, cancellationToken);
            if (salvarAnexo.IsFailure)
                return salvarAnexo.Failure;

            return Return.Empty;

        }

        public async Task<TryException<Return>> CriarBinariosEIncluirArquivosSaida(IEnumerable<SaidaMaterialArquivoModel> saidaMaterialArquivoModel, string uploadBasePath, Guid UsuarioLogado, CancellationToken cancellationToken)
        {

            byte[] binarioArquivoFinal = await ExecutarMergePdf(
             saidaMaterialArquivoModel,
             uploadBasePath,
             cancellationToken);

            var binarioMergeSalvo = await _binarioRepository.Inserir(binarioArquivoFinal, cancellationToken);
            if (binarioMergeSalvo.IsFailure)
                return binarioMergeSalvo.Failure;

            var saidaMaterialArquivoFinal = saidaMaterialArquivoModel.First();
            saidaMaterialArquivoFinal.DefinirBinario(binarioArquivoFinal);
            saidaMaterialArquivoFinal.DefinirBinarioId(binarioMergeSalvo.Success);
            saidaMaterialArquivoFinal.DefinirAutor(UsuarioLogado);


            if (saidaMaterialArquivoFinal.PodeEditar())
            {
                var documentoEditar = await _saidaMaterialAnexoRepository.ObterAnexoPorIdEhTipo(saidaMaterialArquivoFinal.SaidaMaterialNfId, TipoDocAnexo.Saida, cancellationToken);

                if (documentoEditar.IsFailure)
                    return documentoEditar.Failure;

                documentoEditar.Success.DefinirMotivo(saidaMaterialArquivoFinal.Motivo);
                documentoEditar.Success.DefinirAutor(UsuarioLogado);

                var documentoInativo = await _saidaMaterialAnexoRepository.InativaAnexo(documentoEditar.Success, cancellationToken);
                if (documentoInativo.IsFailure)
                    return documentoInativo.Failure;

            }


            var salvarAnexo = await _saidaMaterialAnexoRepository.Salvar(saidaMaterialArquivoFinal, cancellationToken);
            if (salvarAnexo.IsFailure)
                return salvarAnexo.Failure;

            var saidaMaterialNf = await ObterMaterialNotaFiscalPorId(saidaMaterialArquivoFinal.SaidaMaterialNfId, cancellationToken);

            if (saidaMaterialNf.IsFailure)
                return saidaMaterialNf.Failure;


            var emailModel = new EmailModel
            {
                Destinatario = saidaMaterialNf.Success.EmailAutor,
                Assunto = $"Nota Fiscal {saidaMaterialArquivoFinal.Numero} disponível",
                Nome = saidaMaterialNf.Success.NomeAutor,
                Numero = saidaMaterialArquivoFinal.Numero.ToString(),
                Destino = saidaMaterialNf.Success.Destino,
                DataSaida = saidaMaterialNf.Success.Saida.ToString("dd/MM/yyyy"),
                Template = _configuration.GetSection("TemplatesEmail:SaidaMaterialNotaFiscal-UploadNfSaida").Value,
                ArquivoBinario = salvarAnexo.Success.ArquivoBinario
            };

            var enviouEmail = await _emailExternalService.Enviar(emailModel);
            if (enviouEmail.IsFailure)
                return enviouEmail.Failure;

            return Return.Empty;
        }

        private async Task<byte[]> ExecutarMergePdf(
          IEnumerable<SaidaMaterialArquivoModel> listaArquivosPdf,
          string uploadBasePath,
          CancellationToken cancellationToken
      )
        {
            byte[] arquivoBinario = null;

            using (var MergeStream = new MemoryStream())
            using (var MergeDocument = new PdfDocument())
            {
                foreach (var arquivo in listaArquivosPdf)
                {
                    byte[] pdfBytes = null;

                    if (arquivo.BinarioId == 0)
                        pdfBytes = File.ReadAllBytes(Path.Combine(uploadBasePath, arquivo.NomeSalvo));
                    else
                        pdfBytes = (await _binarioRepository.ObterPorId(arquivo.BinarioId, cancellationToken)).Success;


                    using (var Stream = new MemoryStream(pdfBytes))
                    using (var Document = PdfReader.Open(Stream, PdfDocumentOpenMode.Import))
                    {
                        AdicionaPaginaPdf(Document, MergeDocument);
                    }
                }
                MergeDocument.Save(MergeStream);
                arquivoBinario = MergeStream.ToArray();
            }

            return arquivoBinario;
        }

        private void AdicionaPaginaPdf(PdfDocument documentPdfFrom, PdfDocument documentPdfTo)
        {
            for (int i = 0; i < documentPdfFrom.PageCount; i++)
                documentPdfTo.AddPage(documentPdfFrom.Pages[i]);
        }

        public async Task<TryException<IEnumerable<SaidaMaterialArquivoModel>>> ObterBase64DoPdf(int saidaMaterialNfId, CancellationToken cancellationToken)
        {

            var saidaMaterialNf = await _saidaMaterialNotaFiscalRepository.ObterMaterialNotaFiscalPorId(saidaMaterialNfId, cancellationToken);

            if (saidaMaterialNf.IsFailure)
                return saidaMaterialNf.Failure;

            if (saidaMaterialNf.Success == null)
                return new BusinessException(REGISTRO_NAO_ENCONTRADO, $"Não foi encontrado nenhuma saída de material com nota fiscal com o id {saidaMaterialNfId}.");

            var listBinario = await _saidaMaterialAnexoRepository.ListarAnexo(saidaMaterialNfId, cancellationToken);

            if (listBinario.IsFailure)
                return listBinario.Failure;
             
            if (listBinario.Success.All(x => x.ArquivoBinario == null))
                return new BusinessException(PDF_NAO_ENCONTRADO, "Documento não está mais disponível para visualização. Temporalidade expirada.");

            if (listBinario.Success == null)
                return new BusinessException(PDF_NAO_ENCONTRADO, "PDF não encontrado.");

            listBinario.Success.ForEach(x => x.Arquivo = Convert.ToBase64String(x.ArquivoBinario));


            return listBinario.Success.ToCollection();

        }

        public async Task<TryException<Return>> CancelarOuSolicitarCancelamento(SaidaMaterialNotaFiscalModel saidaMaterialNotaFiscalModel, string motivo, CancellationToken cancellationToken)
        {
            var statusParaAtualizar = SaidaMaterialNotaFiscalStatus.Cancelada;

            var anexoDeSaida = await _saidaMaterialAnexoRepository.ObterAnexoPorIdEhTipo(saidaMaterialNotaFiscalModel.Id, TipoDocAnexo.Saida, cancellationToken);

            if (anexoDeSaida.IsFailure)
                return anexoDeSaida.Failure;

            if (anexoDeSaida.Success != null)
                statusParaAtualizar = SaidaMaterialNotaFiscalStatus.PendenteNFCancelamento;

            using (var transaction = _unitOfWork.BeginTransaction())
            {
                var atualizarStatus = await _saidaMaterialNotaFiscalRepository.AtualizarStatus(saidaMaterialNotaFiscalModel, statusParaAtualizar, cancellationToken);
                if (atualizarStatus.IsFailure)
                    return atualizarStatus.Failure;

                var inserirMotivo = await _saidaMaterialNotaFiscalRepository.InserirMotivoCancelar(saidaMaterialNotaFiscalModel, motivo, cancellationToken);

                if (inserirMotivo.IsFailure)
                    return inserirMotivo.Failure;



                if (statusParaAtualizar == SaidaMaterialNotaFiscalStatus.PendenteNFCancelamento)
                {
                    var obterUsuariosPorPerfil = await _usuarioService.ListarUsuariosPorPerfil(Perfil.Contabilidade, cancellationToken);

                    if (obterUsuariosPorPerfil.IsFailure)
                        return obterUsuariosPorPerfil.Failure;


                    foreach (var usuario in obterUsuariosPorPerfil.Success)
                    {

                        var emailModel = new EmailModel
                        {
                            Destinatario = usuario.Email,
                            Assunto = $"Cancelar NF {anexoDeSaida.Success.Numero.ToString()}",
                            Nome = usuario.Nome,
                            Numero = anexoDeSaida.Success.Numero.ToString(),
                            Template = _configuration.GetSection("TemplatesEmail:SaidaMaterialNotaFiscal-SolicitarCancelarNfSaida").Value,
                        };

                        var enviouEmail = await _emailExternalService.Enviar(emailModel);
                        if (enviouEmail.IsFailure)
                            return enviouEmail.Failure;
                    }
                }

                _unitOfWork.Commit();
            }

            return Return.Empty;
        }

        public async Task<TryException<Return>> EfetivarCancelamento(SaidaMaterialNotaFiscalModel saidaMaterialNotaFiscalModel, CancellationToken cancellationToken)
        {
            var obterAnexoSaida = await _saidaMaterialAnexoRepository.ObterAnexoPorIdEhTipo(saidaMaterialNotaFiscalModel.Id, TipoDocAnexo.Saida, cancellationToken);

            if (obterAnexoSaida.IsFailure)
                return obterAnexoSaida.Failure;

            if (obterAnexoSaida.Success == null)
                throw new BusinessException("Não encontrado anexo para a saída de material com nota fiscal");

            var anexoDeSaida = obterAnexoSaida.Success;

            var binario = await _binarioRepository.ObterPorId(anexoDeSaida.BinarioId, cancellationToken);

            if (binario.IsFailure)
                return binario.Failure;

            byte[] arquivoParaCarimbo = binario.Success;

            var carimboCancelado = Convert.FromBase64String(_configuration.GetValue("Ice:PdfManager:Carimbos:Cancelado", string.Empty));
            var arquivoCarimbado = Framework.Pdf.PdfManager.CarimbarDocumento(arquivoParaCarimbo, carimboCancelado);

            using (var transaction = _unitOfWork.BeginTransaction())
            {

                var InsertBinario = await _binarioRepository.Inserir(arquivoCarimbado, cancellationToken);
                if (InsertBinario.IsFailure)
                    return InsertBinario.Failure;

                var anexoCarimbado = obterAnexoSaida.Success;

                anexoCarimbado.DefinirBinarioId(InsertBinario.Success);

                var documentoInativo = await _saidaMaterialAnexoRepository.InativaAnexo(anexoDeSaida, cancellationToken);
                if (documentoInativo.IsFailure)
                    return documentoInativo.Failure;

                var salvarAnexo = await _saidaMaterialAnexoRepository.Salvar(anexoCarimbado, cancellationToken);
                if (salvarAnexo.IsFailure)
                    return salvarAnexo.Failure;

                var atualizarStatusMaterial = await _saidaMaterialNotaFiscalRepository.AtualizarStatus(saidaMaterialNotaFiscalModel, SaidaMaterialNotaFiscalStatus.Cancelada, cancellationToken);
                if (atualizarStatusMaterial.IsFailure)
                    return atualizarStatusMaterial.Failure;

                _unitOfWork.Commit();
            }


            return Return.Empty;
        }

    }
}
