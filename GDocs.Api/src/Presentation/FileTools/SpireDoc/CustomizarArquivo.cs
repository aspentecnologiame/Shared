using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ICE.GDocs.Application.GDocs;
using ICE.GDocs.Common.Core.Exceptions;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.AspNetCore.Http;
using Spire.Doc;
using Spire.Doc.Documents;
using Spire.Doc.Fields;
using Spire.Xls;
using Microsoft.Extensions.Configuration;

namespace ICE.GDocs.Api.FileTools.Ghostscript
{
    public class CustomizarArquivo
    {
        private const string MUSTACHE_NAO_ENCONTRADO = "Você não utilizou a versão correta deste FI. O sistema só aceita a versão padrão deste FI que encontra-se disponível no Portal ICE. Por favor refazer seu documento na versão padrão do FI e reenviar para upload.";
        private const float ALTURA_PADRAO_ASSINATURA = 50f;
        private const float ALTURA_PADRAO_CARIMBO_ICE = 70f;
        private const float ALTURA_PADRAO_CARIMBO_PESSOA = 170f;
        private const int QTDE_PADRAO_POR_LINHA = 2;

        private readonly IAssinaturaUsuarioService _assinaturaUsuarioService;
        private readonly ConfiguracaoCategoriaModel _configCategorias;
        private readonly IConfiguration _configuration;

        public CustomizarArquivo(
            IAssinaturaUsuarioService assinaturaUsuarioService, 
            ConfiguracaoCategoriaModel configCategorias,
            IConfiguration configuration)
        {
            _assinaturaUsuarioService = assinaturaUsuarioService;
            _configCategorias = configCategorias;
            _configuration = configuration;
        }

        public async Task<TryException<MemoryStream>> AjustarDocumentoComNumeracaoAutomaticaEAssinaturaDocumento(IFormFile arquivo, RequisicaoUploadModel requisicao, string extensaoArquivo, CancellationToken cancellationToken)
		{
            if (extensaoArquivo == "XLSX")
                return AjustarDocumentoExcelComNumeracaoAutomatica(arquivo, requisicao);
            else
                return await AjustarDocumentoWordComNumeracaoEAssinaturaDocumento(arquivo, requisicao, extensaoArquivo, cancellationToken);
        }

        private TryException<MemoryStream> AjustarDocumentoExcelComNumeracaoAutomatica(IFormFile arquivo, RequisicaoUploadModel requisicao)
        {
            var ms = new MemoryStream();

			if (_configCategorias == null ||
			   (!_configCategorias.NumeracaoAutomatica.Habilitado))
			{
				arquivo.CopyTo(ms);
				ms.Position = 0;
				return ms;
			}

			using (Workbook workbook = new Workbook())
			{
				workbook.LoadFromStream(arquivo.OpenReadStream());
				CellRange[] cell = workbook.Worksheets.FindAll(_configCategorias.NumeracaoAutomatica.Mustache, FindType.Text, ExcelFindOptions.None);

				if ((cell == null || cell.Length == 0) && requisicao.QtdeDocumentos == 0)
					return new BusinessException(MUSTACHE_NAO_ENCONTRADO);

				workbook.Replace(_configCategorias.NumeracaoAutomatica.Mustache, requisicao.NumeroDocumento.Value.ToString());

				workbook.SaveToStream(ms, Spire.Xls.FileFormat.Version2016);
			}

            ms.Position = 0;
            return ms;
        }

		private async Task<TryException<MemoryStream>> AjustarDocumentoWordComNumeracaoEAssinaturaDocumento(IFormFile arquivo, RequisicaoUploadModel requisicao, string extensaoArquivo, CancellationToken cancellationToken)
		{
			var ms = new MemoryStream();
			var extensoesValidas = new List<(string, Spire.Doc.FileFormat)>
			{
				("DOC", Spire.Doc.FileFormat.Doc),
				("DOCX", Spire.Doc.FileFormat.Docx)
			};

			if (_configCategorias == null ||
			   (!_configCategorias.NumeracaoAutomatica.Habilitado &&
				!requisicao.ListaGuidUsuarioAssinaturaDocumento.Any()) ||
			   !extensoesValidas.Select(s => s.Item1).Contains(extensaoArquivo))
			{
				arquivo.CopyTo(ms);
				ms.Position = 0;
				return ms;
			}

			var formato = extensoesValidas.First(w => w.Item1 == extensaoArquivo).Item2;

			using (Document document = new Document())
			{
				var arq = arquivo.OpenReadStream();
				document.LoadFromStream(arq, formato);
				List<AssinaturaArmazenadaUsuarioModel> listaAssinaturas = new List<AssinaturaArmazenadaUsuarioModel>();

				if (requisicao.ListaGuidUsuarioAssinaturaDocumento.Any())
				{
					var assinaturaDocumentoUsuario = await _assinaturaUsuarioService.ObterAssinaturasDoUsuarioPorListagemDeGuid(requisicao.ListaGuidUsuarioAssinaturaDocumento.Distinct().ToList(), cancellationToken);

					if (assinaturaDocumentoUsuario.IsFailure)
						return assinaturaDocumentoUsuario.Failure;

					listaAssinaturas = assinaturaDocumentoUsuario.Success.ToList();
				}

				var aplicaAssinaturaDocumento = AplicarAssinaturaDocumento(document, listaAssinaturas, requisicao);

				if (aplicaAssinaturaDocumento.IsFailure)
					return aplicaAssinaturaDocumento.Failure;

				var aplicaNumeracaoAutomatica = AplicarNumeracaoAutomatica(document, requisicao);

				if (aplicaNumeracaoAutomatica.IsFailure)
					return aplicaNumeracaoAutomatica.Failure;

				document.SaveToStream(ms, formato);
			}

			ms.Position = 0;
			return ms;
		}

		private TryException<Return> AplicarNumeracaoAutomatica(Document document, RequisicaoUploadModel requisicao)
		{
			if (_configCategorias.NumeracaoAutomatica.Habilitado &&
				requisicao.NumeroDocumento.HasValue &&
				!string.IsNullOrEmpty(_configCategorias.NumeracaoAutomatica.Mustache))
			{
				TextSelection[] listaMustacheNumeroDcoumento = document.FindAllString(_configCategorias.NumeracaoAutomatica.Mustache, false, true);

				if (listaMustacheNumeroDcoumento == null && requisicao.QtdeDocumentos == 0)
					return new BusinessException("upload-mustache-numerodocumento-nao_encontrada",MUSTACHE_NAO_ENCONTRADO);

				document.Replace(_configCategorias.NumeracaoAutomatica.Mustache, requisicao.NumeroDocumento.Value.ToString(), true, true);
			}

			return Return.Empty;
		}

		private TryException<Return> AplicarAssinaturaDocumento(Document document, List<AssinaturaArmazenadaUsuarioModel> assinaturas, RequisicaoUploadModel requisicao)
		{
			if (!string.IsNullOrEmpty(_configCategorias.AssinaturaDocumento.Mustache))
			{
				TextSelection[] listaMustacheAssinaturaDocumento = document.FindAllString(_configCategorias.AssinaturaDocumento.Mustache, false, true);

				if (listaMustacheAssinaturaDocumento == null)
				{
					if (requisicao.QtdeDocumentos == 0)
						return new BusinessException("upload-assinatura-nao_encontrada",MUSTACHE_NAO_ENCONTRADO);
					else
						return Return.Empty;
				}

				foreach (TextSelection mustacheAssinaturaDocumento in listaMustacheAssinaturaDocumento)
				{
					var range = mustacheAssinaturaDocumento.GetAsOneRange();

					if (assinaturas.Count == 0)
						RemoverMustacheAssinatura(range);
					else if (assinaturas.Count == 1)
						IncluirAssinaturaPadrao(assinaturas[0].AssinaturaDocumentoArmazenadaBinario, range);
					else
						IncluirAssinaturasMultiplas(assinaturas, range);
				}
			}

			return Return.Empty;
		}

		private void RemoverMustacheAssinatura(TextRange range)
		{
			range.OwnerParagraph.ChildObjects.Remove(range);
		}

		private void IncluirAssinaturaPadrao(byte[] assinatura, TextRange range)
		{
			var alturaPadraoCarimboPessoa = _configuration.GetValue<float>("AssinaturaDocumento:AlturaPadraoCarimboPessoa", ALTURA_PADRAO_CARIMBO_PESSOA);
			var pic = range.OwnerParagraph.AppendPicture(assinatura);
			var ajuste = (alturaPadraoCarimboPessoa / pic.Height);
			pic.Height = alturaPadraoCarimboPessoa;
			pic.Width = pic.Width * ajuste;
			range.OwnerParagraph.ChildObjects.Remove(range);
		}

		private void IncluirAssinaturasMultiplas(List<AssinaturaArmazenadaUsuarioModel> assinaturas, TextRange range)
		{
			var alturaPadraoAssinatura = _configuration.GetValue<float>("AssinaturaDocumento:AlturaPadrao", ALTURA_PADRAO_ASSINATURA);
			var alturaPadraoCarimboIce = _configuration.GetValue<float>("AssinaturaDocumento:AlturaPadraoCarimboIce", ALTURA_PADRAO_CARIMBO_ICE);
			var limitePorLinha = _configuration.GetValue<int>("AssinaturaDocumento:QtdePorLinha", QTDE_PADRAO_POR_LINHA);

			var cont = 0;

			foreach (var ass in assinaturas)
			{
				cont++;

				if (cont == limitePorLinha + 1)
				{
					range.OwnerParagraph.AppendBreak(BreakType.LineBreak);
					cont = 0;
				}

				var pic = range.OwnerParagraph.AppendPicture(ass.AssinaturaArmazenadaBinario);
				var ajuste = (alturaPadraoAssinatura / pic.Height);
				pic.Height = alturaPadraoAssinatura;
				pic.Width = pic.Width * ajuste;
			}

			var assinaturaBase = Convert.FromBase64String(_configuration.GetValue("Ice:PdfManager:Carimbos:AssinaturaBase", string.Empty));
			range.OwnerParagraph.AppendBreak(BreakType.LineBreak);
			var picBase = range.OwnerParagraph.AppendPicture(assinaturaBase);
			var ajusteBase = (alturaPadraoCarimboIce / picBase.Height);
			picBase.Height = alturaPadraoCarimboIce;
			picBase.Width = picBase.Width * ajusteBase;
			range.OwnerParagraph.ChildObjects.Remove(range);
		}
	}
}