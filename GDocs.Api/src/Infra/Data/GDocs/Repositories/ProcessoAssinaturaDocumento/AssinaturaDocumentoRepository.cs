using Dapper;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Database;
using ICE.GDocs.Domain.Repositories.ProcessoAssinaturaDocumento;
using ICE.GDocs.Infra.CrossCutting.Models;
using ICE.GDocs.Infra.CrossCutting.Models.Enums;
using ICE.GDocs.Infra.Data.Core.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ICE.GDocs.Infra.Data.Repositories.ProcessoAssinaturaDocumento
{
    internal class AssinaturaDocumentoRepository : Repository, IAssinaturaDocumentoRepository
    {
        private readonly IConfiguration _configuration;

        public AssinaturaDocumentoRepository(
            IGDocsDatabase db,
            IUnitOfWork unitOfWork,
            IConfiguration configuration
        ) : base(db, unitOfWork)
        {
            _configuration = configuration;
        }
        public async Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarAssinadosRecentemente(Guid activeDirectoryId, List<ProcessoAssinaturaDocumentoModel> processoAssinaturaDocumentoModel, CancellationToken cancellationToken)
        {

            var tempoVisivel = _configuration.GetSection("AbaAssinadosPorMim:TempoVisivel")?.Get<TimeSpan>();
            var data = (tempoVisivel.HasValue ? DateTime.Now.Subtract(tempoVisivel.Value) : DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");

            var listaSadIdt = new List<int>();
            listaSadIdt.Add(StatusAssinaturaDocumentoPasso.EmAndamento.ToInt32());
            listaSadIdt.Add(StatusAssinaturaDocumentoPasso.Concluido.ToInt32());
            listaSadIdt.Add(StatusAssinaturaDocumentoPasso.ConcluidoComRejeicao.ToInt32());
            listaSadIdt.Add(StatusAssinaturaDocumentoPasso.Cancelado.ToInt32());



            var result = await _db.Connection.QueryAsync<ProcessoAssinaturaDocumentoModel>(
                    new CommandDefinition(
                        commandText: $@"
                                        select
                                        PAD.pad_idt AS [Id],
                                        PAD.pad_titulo AS [Titulo],
                                        PAD.pad_descricao AS [Descricao],
                                        PAD.pad_nome_documento AS [NomeDocumento],
                                        PAD.pad_flg_destaque AS [Destaque],                                     
                                        PAD.pad_guid_ad AS [AutorId],                                     
                                        PAD.sad_idt AS [StatusId],                                         
                                        PAD.pad_dat_criacao AS [DataCriacao],
                                        PAD.pad_numero_documento AS [Numero],
                                        PAD.padc_idt AS CategoriaId,                                       
                                        PADC.padc_descricao AS DescricaoCategoria,
                                        PADPU.padpu_flg_assinar_certificado_digital [AssinarCertificadoDigital],
                                        PADPU.padpu_flg_assinar_fisicamente [AssinarFisicamente],                                 
                                        convert(bit, case when PADPU.sadpu_idt = 3 and PADPU.padpu_dat_assinatura >= '{data}' OR (PADPU.sadpu_idt = 4 and PADPU.padpu_dat_assinatura >= '{data}') then 1 else 0 end) AssinadoPorMimVisivel
                                        from tb_pad_processo_assinatura_documento pad
                                        INNER JOIN [dbo].[tb_padc_processo_assinatura_documento_categoria] PADC ON PADC.padc_idt = PAD.padc_idt
                                        INNER JOIN [dbo].[tb_padp_processo_assinatura_documento_passo] PADP ON PADP.pad_idt = PAD.pad_idt
                                        INNER JOIN [dbo].[tb_padpu_processo_assinatura_documento_passo_usuario] PADPU ON PADPU.padp_idt = PADP.padp_idt
                                        INNER JOIN [dbo].[tb_sadpu_status_assinatura_documento_passo_usuario] SADPU ON SADPU.sadpu_idt = PADPU.sadpu_idt
                                        WHERE
                                        PAD.pad_flg_ativo = 1
                                        AND PADP.padp_flg_ativo = 1
                                        AND PADPU.padpu_flg_ativo = 1
                                        AND PAD.sad_idt in @listaSadIdt
                                        AND (PADPU.sadpu_idt = 3 OR PADPU.sadpu_idt = 4)
                                        AND (PADPU.padpu_guid_ad = @activeDirectoryId OR PADPU.padpu_guid_ad_representante = @activeDirectoryId)
                                        AND (PADPU.padpu_dat_assinatura >= '{data}')
                                        ORDER BY PAD.pad_numero_documento",
                        parameters: new
                        {
                            activeDirectoryId,
                            listaSadIdt,
                        },
                        cancellationToken: cancellationToken
                    )
                );


            processoAssinaturaDocumentoModel.ForEach(pendentes => {
                if (result.Any(x => x.Id == pendentes.Id))
                   pendentes.AssinadoPorMimVisivel = true;
            });

           processoAssinaturaDocumentoModel.AddRange(result.ToList());
   

            return processoAssinaturaDocumentoModel.ToList();
        }

        public async Task<TryException<IEnumerable<ProcessoAssinaturaDocumentoModel>>> ListarDocumentosPendentesDeAssinaturaOuJaAssinadoPeloUsuario(Guid activeDirectoryId, CancellationToken cancellationToken)
        {

            var result = await _db.Connection.QueryAsync<ProcessoAssinaturaDocumentoModel>(
                    new CommandDefinition(
                        commandText: $@"SELECT 
                                            PAD.pad_idt AS [Id],
                                            PAD.pad_titulo AS [Titulo],
                                            PAD.pad_descricao AS [Descricao],
                                            PAD.pad_nome_documento AS [NomeDocumento],
                                            PAD.pad_flg_destaque AS [Destaque],
                                            PAD.pad_guid_ad AS [AutorId],
                                            PAD.sad_idt AS [StatusId],
                                            PAD.pad_dat_criacao AS [DataCriacao],
                                            PAD.pad_numero_documento AS [Numero],
                                            PAD.padc_idt AS CategoriaId,
                                            PADC.padc_descricao AS DescricaoCategoria,
                                            PADPU.padpu_flg_assinar_certificado_digital [AssinarCertificadoDigital], 
                                            PADPU.padpu_flg_assinar_fisicamente [AssinarFisicamente],
											Case
											when PAD.padc_idt = 1 
											then (select COUNT(dfi_idt) from tb_dfi_documento_fi1548 dfi where  dfi.dfi_numero = PAD.pad_numero_documento and dfi.dfi_referencia_substituto >0) 
											end 
											as ReferenciaSubstituto
                                        FROM tb_pad_processo_assinatura_documento PAD 
                                            INNER JOIN [dbo].[tb_padc_processo_assinatura_documento_categoria] PADC ON PADC.padc_idt = PAD.padc_idt 
                                            INNER JOIN [dbo].[tb_padp_processo_assinatura_documento_passo] PADP ON PADP.pad_idt = PAD.pad_idt 
                                            INNER JOIN [dbo].[tb_padpu_processo_assinatura_documento_passo_usuario] PADPU ON PADPU.padp_idt = PADP.padp_idt 
                                            INNER JOIN [dbo].[tb_sadpu_status_assinatura_documento_passo_usuario] SADPU ON SADPU.sadpu_idt = PADPU.sadpu_idt 
                                        WHERE
                                            PAD.pad_flg_ativo = 1 
                                              AND PADP.padp_flg_ativo = 1 
                                              AND PADPU.padpu_flg_ativo = 1 
                                              AND PAD.sad_idt = @sadIdt 
                                              AND (PADP.sadp_idt = @sadpIdt OR PADPU.sadpu_idt = @sadpuIdt)
                                            AND (PADPU.padpu_guid_ad = @activeDirectoryId OR PADPU.padpu_guid_ad_representante = @activeDirectoryId)
                                        ORDER BY PAD.pad_numero_documento",
                        parameters: new
                        {
                            activeDirectoryId,
                            sadIdt = (int)StatusAssinaturaDocumento.EmAndamento,
                            sadpIdt = (int)StatusAssinaturaDocumentoPasso.EmAndamento,
                            sadpuIdt = (int)StatusAssinaturaDocumentoPassoUsuario.Assinado
                        },
                        cancellationToken: cancellationToken
                    )
                );

            var listaComAprovadosRecentimente = await ListarAssinadosRecentemente(activeDirectoryId, result.ToList(), cancellationToken);
            if (listaComAprovadosRecentimente.IsFailure)
                return listaComAprovadosRecentimente.Failure;

            return listaComAprovadosRecentimente.Success.ToList();
        }
    }
}
