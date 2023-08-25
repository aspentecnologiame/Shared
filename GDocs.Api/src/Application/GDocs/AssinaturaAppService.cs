using ICE.GDocs.Common.Core.Exceptions;
using ICE.GDocs.Domain.ExternalServices;
using ICE.GDocs.Domain.ExternalServices.Model;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Domain.Services;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Application.GDocs
{
    internal class AssinaturaAppService : IAssinaturaAppService
    {
        private readonly IAssinaturaAgregationAppService _assinaturaAgregationService;
        private readonly IUsuarioService _usuarioService;
        private readonly IActiveDirectoryExternalService _activeDirectoryExternalService;
        private readonly ILogService _logService;
        private readonly IInformacaoRepository _informacaoRepository;
        private readonly IPassoUsuarioRepository _passoUsuarioRepository;
        private readonly IConfiguration _configuration;
        private const string DIR_FLUXO_MESMO_PASSO = "DIR-MESMO-PASSO";
        private const string CAMPO_FILTRO_PADRAO = "DataCriacao";
        private const string CAMPO_FILTRO_SECUNDARIO = "NumeroDocumento";

        public AssinaturaAppService
        (
            IAssinaturaAgregationAppService assinaturaAgregationService,
            IUsuarioService usuarioService,
            IActiveDirectoryExternalService activeDirectoryExternalService,
            ILogService logService,
            IInformacaoRepository informacaoRepository,
            IPassoUsuarioRepository passoUsuarioRepository,
            IConfiguration configuration)
        {
            _assinaturaAgregationService = assinaturaAgregationService;
            _usuarioService = usuarioService;
            _activeDirectoryExternalService = activeDirectoryExternalService;
            _logService = logService;
            _informacaoRepository = informacaoRepository;
            _passoUsuarioRepository = passoUsuarioRepository;
            _configuration = configuration;
        }

        public async Task<TryException<ArquivoModel>> Converter(string extensao, ArquivoModel arquivo, CancellationToken cancellationToken)
            => await _assinaturaAgregationService.AssinaturaService.Converter(extensao, arquivo, cancellationToken);

        public async Task<TryException<IEnumerable<UsuarioActiveDirectory>>> ListarUsuariosActiveDirectory(string nome, CancellationToken cancellationToken)
        {
            var usersAD = await _usuarioService.ListarUsuariosActiveDirectory(nome, cancellationToken);

            if (usersAD.IsFailure)
                return usersAD.Failure;

            var usuariosAd = usersAD.Success;

            var usuariosADBloqueadosNaLista = _configuration.GetSection("UsuariosAdBloqueadosNaLista")?.Get<string[]>();

            if (usuariosADBloqueadosNaLista != null)
                return usuariosAd.Where(usuAd => !usuariosADBloqueadosNaLista.ToList().Exists(usuBloqueado => usuBloqueado == usuAd.Guid.ToString())).AsList();

            return usuariosAd.ToList();
        }

        public async Task<TryException<AssinaturaModel>> SalvarProcesso(AssinaturaModel assinatura, string uploadBasePath, CancellationToken cancellationToken)
            => await _assinaturaAgregationService.AssinaturaService.SalvarProcesso(assinatura, uploadBasePath, cancellationToken);

        public async Task<TryException<Return>> Assinar(Guid usuarioAdGuid, AssinaturaPassoItemAssinarRejeitarModel assinaturaPassoItemAssinarModel, string uploadPathBase, CancellationToken cancellationToken)
            => await _assinaturaAgregationService.AssinaturaService.Assinar(usuarioAdGuid, assinaturaPassoItemAssinarModel, uploadPathBase, cancellationToken);

        public async Task<TryException<Return>> Rejeitar(Guid usuarioAdGuid, AssinaturaPassoItemAssinarRejeitarModel assinaturaPassoItemRejeitarModel, CancellationToken cancellationToken)
            => await _assinaturaAgregationService.AssinaturaService.Rejeitar(usuarioAdGuid, assinaturaPassoItemRejeitarModel, cancellationToken);

        public async Task<TryException<Return>> ValidarSeOUsuarioJaAssinouAlgumDosProcessos(Guid usuarioAdGuid, IEnumerable<int> listaDeprocessoAssinaturaDocumentoId, StatusAssinaturaDocumentoPassoUsuario status, CancellationToken cancellationToken)
            => await _assinaturaAgregationService.AssinaturaService.ValidarSeOUsuarioJaAssinouAlgumDosProcessos(usuarioAdGuid, listaDeprocessoAssinaturaDocumentoId, status, cancellationToken);

        public async Task<TryException<AssinaturaInformacoesModel>> CancelarProcesso(AssinaturaInformacoesModel model, CancellationToken cancellationToken, DocumentoFI1548FilterModel filtro, UsuarioModel usuario, bool permiteCancelar)
        {
            var retorno = await _assinaturaAgregationService.AssinaturaService.CancelarProcesso(model, cancellationToken);

            var documento = await _assinaturaAgregationService.DocumentoFI1548AppService.ObterPorNumero(model.NumeroDocumento??0, cancellationToken);

            if(documento.Success != null && retorno.IsSuccess)
                await _assinaturaAgregationService.DocumentoFI1548AppService.Cancelar(documento.Success, usuario, permiteCancelar, cancellationToken);

            return retorno;
        }
            
        public async Task<TryException<Return>> SalvarArquivos(IEnumerable<AssinaturaArquivoModel> assinaturaArquivoModel, string uploadBasePath, CancellationToken cancellationToken)
            => await _assinaturaAgregationService.AssinaturaService.SalvarArquivos(assinaturaArquivoModel, uploadBasePath, cancellationToken);

        public async Task<TryException<Return>> SalvarFareiEnvio(AssinaturaModel assinaturaModel,Guid usuarioId, bool fareiEnvio,CancellationToken cancellationToken)
           => await _assinaturaAgregationService.AssinaturaService.SalvarFareiEnvio(assinaturaModel, usuarioId , fareiEnvio, cancellationToken);

        public async Task<TryException<IEnumerable<AssinaturaInformacoesModel>>> ObterInformacoesPorNumeroNomeStatusAutorPeriodo(AssinaturaInformacoesFilterModel filtro, CancellationToken cancellationToken)
        {
            await _logService.AdicionarRastreabilidade(filtro, "Iniciando a busca de aprovação informações com o filtro Selecionado.");

            var response = await _informacaoRepository.Listar(filtro, cancellationToken);

            if (response.IsFailure)
                return response.Failure;

            await _logService.AdicionarRastreabilidade(filtro, $"{response.Success.Count()} resultado encontrado.");

            var usuarios = _activeDirectoryExternalService.GetActiveDirectoryUsers(string.Empty, response.Success.Select(x => x.UsuarioGuidAd).Distinct().AsList());

            if (usuarios.IsFailure)
                return usuarios.Failure;

            var results = response.Success.Join(usuarios.Success,
                      d => d.UsuarioGuidAd,
                      u => u.Guid,
                      (info, usu) => info.DefinirAutor(usu.Nome)
                    );

            await _logService.AdicionarRastreabilidade(filtro, $"Total de {results.Count()} documenos após junção com os autores ad.");

            results = AplicarOrdenacao(filtro, results);

            return results?.ToCollection();
        }

        public async Task<TryException<IEnumerable<AssinaturaNomeDocumento>>> ListarNomesDocumentos(string termo, CancellationToken cancellationToken)
        {
            termo = termo.Trim();
            if (string.IsNullOrWhiteSpace(termo))
                return new List<AssinaturaNomeDocumento>();

            await _logService.AdicionarRastreabilidade(termo, "Iniciando listagem de nomes de documentos por termo.");

            var response = await _informacaoRepository.ListarNomesDocumentos(termo, cancellationToken);

            if (response.IsFailure)
                return response.Failure;

            await _logService.AdicionarRastreabilidade(termo, "Listagem de nomes de documentos por termo foi executada com sucesso.");

            return response.Success?.ToCollection();
        }

        public async Task<TryException<IEnumerable<AssinaturaPassoAssinanteRepresentanteModel>>> ListarAssinantesProcesso(long processoId, bool todosStatus, CancellationToken cancellationToken)
        {
            await _logService.AdicionarRastreabilidade(processoId, "Iniciando listagem de assinante do processo.");

            var passoAssinanteList = await _passoUsuarioRepository.ListarAssinantesERepresentantesPorPadIdEStatusNaoIniciadoEmAndamento(processoId, todosStatus, cancellationToken);

            if (passoAssinanteList.IsFailure)
                return passoAssinanteList.Failure;

            if (!passoAssinanteList.Success.Any())
                return passoAssinanteList;

            var usuarioAssinanteList = _activeDirectoryExternalService.GetActiveDirectoryUsers(string.Empty, passoAssinanteList.Success.Select(x => x.UsuarioAdAssinanteGuid).Distinct().AsList());

            if (usuarioAssinanteList.IsFailure)
                return usuarioAssinanteList.Failure;

            var passoassinanteDefinidoList = passoAssinanteList.Success.Join(usuarioAssinanteList.Success,
                      p => p.UsuarioAdAssinanteGuid,
                      u => u.Guid,
                      (passo, usuAd) => passo.DefinirAssinante(usuAd.Nome)
                    );

            var usuarioRepresentanteList = _activeDirectoryExternalService.GetActiveDirectoryUsers(string.Empty, passoAssinanteList.Success.Where(p => p.UsuarioAdRepresentanteGuid.HasValue).Select(x => x.UsuarioAdRepresentanteGuid.Value).Distinct().AsList());

            if (usuarioRepresentanteList.IsFailure)
                return usuarioRepresentanteList.Failure;

            var assinanteRepresentanteDefinidoList = (from passo in passoassinanteDefinidoList
                                                      join usuarioRepresentanteAd in usuarioRepresentanteList.Success
                                                            on passo.UsuarioAdRepresentanteGuid equals usuarioRepresentanteAd.Guid into ur
                                                      from urLeft in ur.DefaultIfEmpty()
                                                      select new AssinaturaPassoAssinanteRepresentanteModel
                                                      {
                                                          PassoId = passo.PassoId,
                                                          UsuarioAdAssinanteGuid = passo.UsuarioAdAssinanteGuid,
                                                          UsuarioAdAssinate = passo.UsuarioAdAssinate,
                                                          FareiEnvio = passo.FareiEnvio,
                                                          UsuarioAdRepresentanteGuid = urLeft?.Guid,
                                                          UsuarioAdRepresentante = urLeft?.Nome
                                                      }).ToList();

            await _logService.AdicionarRastreabilidade(processoId, "Listagem foi executada com sucesso.");

            return ValidarSeOUsuarioPodeSerRepresentado(assinanteRepresentanteDefinidoList).ToCollection();
        }

        private IEnumerable<AssinaturaPassoAssinanteRepresentanteModel> ValidarSeOUsuarioPodeSerRepresentado(IEnumerable<AssinaturaPassoAssinanteRepresentanteModel> assinanteRepresentanteDefinidoList)
        {
            var listaUsuariosQueNaoPodemSerRepresentados = _configuration.GetSection("UsuariosQueNaoPodemSerRepresentados")?.Get<Guid[]>();

            assinanteRepresentanteDefinidoList
                .Where(assinantes => listaUsuariosQueNaoPodemSerRepresentados.ToList().Exists(guidAd => guidAd == assinantes.UsuarioAdAssinanteGuid))
                .ForEach(assinantes => assinantes.PodeSerRepresentado = false);

            return assinanteRepresentanteDefinidoList;
        }

        public async Task<TryException<IEnumerable<AssinaturaPassoAssinanteRepresentanteModel>>> AtribuirRepresentantes(IEnumerable<AssinaturaPassoAssinanteRepresentanteModel> passosList, CancellationToken cancellationToken)
        {
            await _logService.AdicionarRastreabilidade(passosList, "Iniciando atribuição de representantes para aprovadores.");

            if (!passosList.Any())
                return passosList.ToCollection();

            passosList = ValidarSeOUsuarioPodeSerRepresentado(passosList);

            if (passosList.Any(assinador => !assinador.PodeSerRepresentado && assinador.UsuarioAdRepresentanteGuid.HasValue && assinador.UsuarioAdRepresentanteGuid.Value != Guid.Empty))
            {
                return new ExceptionReadOnlyCollection(
                    passosList
                        .Where(assinador => !assinador.PodeSerRepresentado && assinador.UsuarioAdRepresentanteGuid.HasValue && assinador.UsuarioAdRepresentanteGuid.Value != Guid.Empty)
                        .ConvertAll(assinador => new BusinessException("usuario-nao-pode-ser-representado", $"{assinador.UsuarioAdAssinate} não pode ser representado.")));
            }

            var usuarioRepresentanteList = _activeDirectoryExternalService.GetActiveDirectoryUsers(string.Empty, passosList.Where(p => p.UsuarioAdRepresentanteGuid.HasValue).Select(x => x.UsuarioAdRepresentanteGuid.Value).Distinct().AsList());

            if (usuarioRepresentanteList.IsFailure)
                return usuarioRepresentanteList.Failure;

            var passoRepresentanteDefinidoList = passosList.Join(usuarioRepresentanteList.Success,
                      p => p.UsuarioAdRepresentanteGuid,
                      u => u.Guid,
                      (passo, usuAd) => passo
                    )
                .Union(passosList.Where(p => !p.UsuarioAdRepresentanteGuid.HasValue));

            await _passoUsuarioRepository.AtribuirRepresentantes(passoRepresentanteDefinidoList, cancellationToken);

            await _logService.AdicionarRastreabilidade(passoRepresentanteDefinidoList, "Representantes definidos com sucesso.");

            return passoRepresentanteDefinidoList.ToCollection();
        }


        public async Task<TryException<bool>> VerificarProcessoCompleto(int processoId, Guid usuarioId , CancellationToken cancellationToken)
        {
            var processo = await _informacaoRepository.ObterProcessoComPassosParaAssinatura(processoId, cancellationToken);

             if (processo.IsFailure)
                return processo.Failure;

            if (!processo.Success.Passos.Itens.SelectMany(passo => passo.Usuarios).Any(x => x.Status != StatusAssinaturaDocumentoPassoUsuario.Assinado) &&
                processo.Success.Passos.Itens.SelectMany(passo => passo.Usuarios).Any(x => x.FlagFareiEnvio && x.Guid == usuarioId))
                return true;
            else
                return false;
        }

        public async Task<TryException<AssinaturaModel>> ObterProcessoComPassosParaAssinatura(long processoId, CancellationToken cancellationToken)
        {
            await _logService.AdicionarRastreabilidade(processoId, "Iniciando busca por processo de aprovação com respectivos passos.");

            var processo = await _informacaoRepository.ObterProcessoComPassosParaAssinatura(processoId, cancellationToken);

            if (processo.IsFailure)
                return processo.Failure;

            await _logService.AdicionarRastreabilidade(processoId, "Busca por processo de aprovação com respectivos passos foi executada com sucesso.");

            var guidsAd = processo.Success.Passos.Itens.SelectMany(u => u.Usuarios).Select(a => a.Guid).AsList();
            guidsAd.AddRange(processo.Success.Passos.Itens.SelectMany(u => u.Usuarios).Select(a => a.GuidRepresentante).Where(guid => guid != Guid.Empty).AsList());

            var usuariosAd = _activeDirectoryExternalService.GetActiveDirectoryUsers(string.Empty, guidsAd.Distinct().AsList());
            if (usuariosAd.IsFailure)
                return usuariosAd.Failure;

            processo.Success.Passos.Itens.ForEach(passo =>
            {
                var usuarioDefinidoList = passo.Usuarios.Join(usuariosAd.Success,
                    a => a.Guid,
                    uad => uad.Guid,
                    (assinante, usuarioAd) =>
                    {
                        assinante.Nome = usuarioAd.Nome;

                        if (assinante.GuidRepresentante != Guid.Empty)
                        {
                            var usuarioRepresentante = usuariosAd.Success.FirstOrDefault(ad => ad.Guid == assinante.GuidRepresentante);
                            if (usuarioRepresentante != null)
                            {
                                assinante.NomeRepresentante = usuarioRepresentante.Nome;
                            }
                        }

                        return assinante;
                    });

                passo.DefinirUsuarios(usuarioDefinidoList.OrderBy(u => u.Nome).ToList());
            });

            return processo;
        }

        public async Task<TryException<IEnumerable<AssinaturaArquivoModel>>> ListarArquivosUploadPorPadId(int processoAssinaturaDocumentoId, CancellationToken cancellationToken)
        {
            var responseListaArquivos = await _assinaturaAgregationService.ArquivoRepository.ListarArquivosUploadPorPadId(processoAssinaturaDocumentoId, cancellationToken);

            if (responseListaArquivos.IsFailure)
                return responseListaArquivos.Failure;

            var listaArquivos = responseListaArquivos.Success;

            var usuarios = _activeDirectoryExternalService.GetActiveDirectoryUsers(string.Empty, listaArquivos.Select(x => x.AutorId).Distinct().AsList());

            if (usuarios.IsFailure)
                return usuarios.Failure;

            var listaArquivosComAutor = listaArquivos.Join(usuarios.Success,
                     d => d.AutorId,
                     u => u.Guid,
                     (doc, usu) => doc.DefinirAutor(usu.Nome)
                   );

            return listaArquivosComAutor?.ToCollection();
        }

        public async Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarDocumentosPendentesDeAssinaturaOuJaAssinadoPeloUsuario(Guid activeDirectoryId, CancellationToken cancellationToken)
        {
            var response = await _assinaturaAgregationService.AssinaturaDocumentoRepository.ListarDocumentosPendentesDeAssinaturaOuJaAssinadoPeloUsuario(activeDirectoryId, cancellationToken);

            if (response.IsFailure)
                return response.Failure;

            var listaDocumentosPendentes = await ProcessaListaDocumentosPendentesDeAssinatura(response.Success, activeDirectoryId, cancellationToken);

            return listaDocumentosPendentes;
        }

        public async Task<TryException<Return>> AtualizarDestaquePorPadId(int processoAssinaturaDocumentoId, bool destaque, CancellationToken cancellationToken)
        {
            var informacao = await ObterInformacaoPorId(processoAssinaturaDocumentoId, cancellationToken);

            if (informacao.IsFailure)
                return informacao.Failure;

            var assinaturaInformacao = informacao.Success;
            assinaturaInformacao.Destaque = destaque;

            var response = await _assinaturaAgregationService.AssinaturaService.AtualizarDestaqueProcesso(assinaturaInformacao, cancellationToken);

            if (response.IsFailure)
                return response.Failure;

            return Return.Empty;
        }

        public async Task<TryException<AssinaturaInformacoesModel>> ObterInformacaoPorId(int processoAssinaturaDocumentoId, CancellationToken cancellationToken)
        {
            var filtro = new AssinaturaInformacoesFilterModel { ListaDeprocessoAssinaturaDocumentoId = new List<int> { processoAssinaturaDocumentoId } };

            var response = await ObterInformacoesPorNumeroNomeStatusAutorPeriodo(filtro, cancellationToken);

            if (response.IsFailure)
                return response.Failure;

            return response.Success.FirstOrDefault();
        }

        public async Task<TryException<DocumentoFI1548Model>> ObterDocumentoFI1548PorNumero(DocumentoFI1548FilterModel documento, CancellationToken cancellationToken)
     => await _assinaturaAgregationService.DocumentoFI1548AppService.ObterPorNumero(documento.Numero, cancellationToken);

        public async Task<TryException<IEnumerable<AssinaturaPassoItemModel>>> ListarPassosEUsuariosPorTag(string tag, CancellationToken cancellationToken)
        {
            var passos = await _assinaturaAgregationService.TemplateProcessoAssinaturaDocumentoRepository.ListarPassosEUsuariosPorTag(tag, cancellationToken);

            if (passos.IsFailure)
                return passos.Failure;

            foreach (var passo in passos.Success)
            {
                foreach (var usuario in passo.Usuarios)
                {
                    var usuarioAd = await _usuarioService.ObterUsuarioActiveDirectoryPorId(usuario.Guid, cancellationToken);
                    
                    if (usuarioAd.IsFailure)
                        return usuarioAd.Failure;

                    usuario.Nome = usuarioAd.Success.Nome;
                }
            }

            return passos;
        }

        private async Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ProcessaListaDocumentosPendentesDeAssinatura(IEnumerable<ProcessoAssinaturaDocumentoModel> listaDocumentosPendentes, Guid activeDirectoryId, CancellationToken cancellationToken)
        {
            var pendenciasDistintas = listaDocumentosPendentes.GroupBy(pendencia => pendencia.Id)
                .Select(group => group.FirstOrDefault());

            foreach (var processoAssinaturaDocumento in pendenciasDistintas)
            {
                var processo = await _informacaoRepository.ObterProcessoComPassosParaAssinatura(processoAssinaturaDocumento.Id, cancellationToken);
                processoAssinaturaDocumento.StatusPassoAtualId = ObterStatusPassoAtual(processo.Success);
                processoAssinaturaDocumento.UsuarioDaConsultaAssinou = VerificarSeFoiAssinadoPeloUsuarioDaConsulta(processo.Success, activeDirectoryId);
            }

            return pendenciasDistintas?.ToCollection();
        }

        private bool VerificarSeFoiAssinadoPeloUsuarioDaConsulta(AssinaturaModel processoAssinatura, Guid activeDirectoryId)
        {
            var passoDoUsuario = ObterPassoDoUsuarioConsulta(processoAssinatura, activeDirectoryId);
            var passoDoUsuarioFiltro = passoDoUsuario.SelectMany(s => s.Usuarios).Where(usuario => usuario.Guid == activeDirectoryId || usuario.GuidRepresentante == activeDirectoryId);
            return passoDoUsuarioFiltro.Any(usuario => (usuario.Status == StatusAssinaturaDocumentoPassoUsuario.Assinado || (usuario.Status == StatusAssinaturaDocumentoPassoUsuario.Rejeitado && usuario.FluxoNotificacao == DIR_FLUXO_MESMO_PASSO)));
        }

        private StatusAssinaturaDocumentoPassoAtual ObterStatusPassoAtual(AssinaturaModel processoAssinatura)
        {
            var passoAtual = ObterPassoAtual(processoAssinatura);

            if (VerificarSeEstaRejeitado(passoAtual))
                return StatusAssinaturaDocumentoPassoAtual.Rejeitado;

            if (VerificarSeAssinadoComObservacao(passoAtual))
                return StatusAssinaturaDocumentoPassoAtual.AssinadoComObservacao;
            
            if (VerificarSeAssinado(passoAtual))
                return StatusAssinaturaDocumentoPassoAtual.Assinado;

            return StatusAssinaturaDocumentoPassoAtual.NaoIniciado;
        }

        private IEnumerable<AssinaturaPassoItemModel> ObterPassoAtual(AssinaturaModel processo)
            => processo.Passos.Itens.Where(passo => passo.Status == StatusAssinaturaDocumentoPasso.EmAndamento ||
                                                    passo.Status == StatusAssinaturaDocumentoPasso.ConcluidoComRejeicao ||
                                                    passo.Status == StatusAssinaturaDocumentoPasso.Concluido);

        private IEnumerable<AssinaturaPassoItemModel> ObterPassoDoUsuarioConsulta(AssinaturaModel processo, Guid activeDirectoryId)
            => processo.Passos.Itens.Where(passo => passo.Usuarios.Any(usuario => usuario.Guid == activeDirectoryId || usuario.GuidRepresentante == activeDirectoryId));

        private bool VerificarSeEstaRejeitado(IEnumerable<AssinaturaPassoItemModel> passoAtual)
            => passoAtual.Any(item => item.Usuarios.Any(usu => usu.Status == StatusAssinaturaDocumentoPassoUsuario.Rejeitado));

        private static bool VerificarSeAssinadoComObservacao(IEnumerable<AssinaturaPassoItemModel> passoAtual)
            => passoAtual.Any(item => item.Usuarios.Any(usu => usu.Status == StatusAssinaturaDocumentoPassoUsuario.Assinado && !string.IsNullOrEmpty(usu.Justificativa)));
        
        private static bool VerificarSeAssinado(IEnumerable<AssinaturaPassoItemModel> passoAtual) 
            => passoAtual.Any(item =>   item.Status == StatusAssinaturaDocumentoPasso.EmAndamento && item.Usuarios.Any(usu => usu.Status == StatusAssinaturaDocumentoPassoUsuario.Assinado));

        private object ObterColunaParaOrdenacao(AssinaturaInformacoesModel filtro, string nome)
        {
            Type type = typeof(AssinaturaInformacoesModel);
            PropertyInfo propInfo = type.GetProperty(FirstLetterToUpper(nome));
            return propInfo.GetValue(filtro, null);
        }

        public string FirstLetterToUpper(string texto)
        {
            if (texto == null)
                return null;

            if (texto.Length > 1)
                return char.ToUpper(texto[0]) + texto.Substring(1);

            return texto.ToUpper();
        }

        private IEnumerable<AssinaturaInformacoesModel> AplicarOrdenacao(AssinaturaInformacoesFilterModel filtro, IEnumerable<AssinaturaInformacoesModel> results)
        {
            if (filtro.Ordenacao != null &&
                !string.IsNullOrEmpty(filtro.Ordenacao.Campo) &&
                !string.IsNullOrEmpty(filtro.Ordenacao.Direcao))
            {
                filtro.Ordenacao.Campo = FirstLetterToUpper(filtro.Ordenacao.Campo);

                if (filtro.Ordenacao.Direcao.ToUpper() == "DESC")
                    return results.OrderByDescending(w => ObterColunaParaOrdenacao(w, filtro.Ordenacao.Campo)).ThenBy(w => (filtro.Ordenacao.Campo == CAMPO_FILTRO_SECUNDARIO) ? 0 : w.NumeroDocumento);
                else
                    return results.OrderBy(w => ObterColunaParaOrdenacao(w, filtro.Ordenacao.Campo)).ThenBy(w => (filtro.Ordenacao.Campo == CAMPO_FILTRO_SECUNDARIO) ? 0 : w.NumeroDocumento);
            }
            else
                return results.OrderByDescending(w => ObterColunaParaOrdenacao(w, CAMPO_FILTRO_PADRAO)).ThenBy(w => w.NumeroDocumento);
        }

        public async Task<TryException<IEnumerable<AssinaturaArquivoModel>>> ListarArquivoExpurgoPorPad(int processoAssinaturaDocumentoId, CancellationToken cancellationToken, bool listarTodos = false)
        {
            var result = await _assinaturaAgregationService.ArquivoRepository.ListarArquivoExpurgoPorPad(processoAssinaturaDocumentoId, cancellationToken, listarTodos);

            if (result.IsFailure)
                return result.Failure;

            if (result.Success.All(x => x.ArquivoBinario == null))
                return new BusinessException("Documento não está mais disponível para visualização. Temporalidade expirada.");

            return result;
        }
    }
}
