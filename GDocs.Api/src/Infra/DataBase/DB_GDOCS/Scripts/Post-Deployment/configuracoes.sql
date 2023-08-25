-- Esse arquivo sempre será executado.

MERGE INTO [dbo].[tb_cfg_configuracao] AS Target
USING (
	VALUES         
		(
			'appsettings.api',
			'{
              "AssinaturaAdicionarCategoriaExclude": [
                1,
                31,
                32,
                35
              ],
              "UsuariosQueNaoPodemSerRepresentados": [
                "5bb1bd6c-651f-456b-9039-c4673d0fac82",
                "b014f744-29de-4447-9d84-9bdc9501ff9d"
              ],
              "ConfiguracaoPerfilStatusSaidaMaterial": [
                {
                  "perfil": 12,
                  "filtro": 1
                }
              ],
              "UsuariosAdBloqueadosNaLista": [
                "5bb1bd6c-651f-456b-9039-c4673d0fac82",
                "b014f744-29de-4447-9d84-9bdc9501ff9d"
              ],
              "CategoriasEscreverNumeroNoPdf": [
                8,
                9,
                10
              ],

              "AbaAssinadosPorMim": {
                "TempoVisivel": "06:00:00"
              },
              "TemplatesEmail": {
                    "SaidaMaterialNotaFiscal-UploadNfSaida": "<!DOCTYPE html><html xmlns lang=\"pt-br\"><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=9\"><style type=\"text/css\">body{font-family:Tahoma;font-size:14px}.warning{color:#f30}.highlight{color:#244061}p{margin:0;margin-bottom:15px;padding:0;width:600px;text-align:justify}.box-title{color: #fff;background-color:#244061;border:solid windowtext 2.2pt;text-align:center;font-size:15px;font-weight:700}.box-info{color:#000;background-color:#fff;border:solid windowtext 2.25pt;padding:10px}</style></head><body><table align=\"center\" style=\"width:650px\"><tr><td class=\"box-title\">Nota Fiscal {{Numero}} disponível</td></tr><tr><td class=\"box-info\"><p>A nota fiscal {{Numero}} referente a saída de material – com NF, com destino para {{Destino}} e com data de saída em {{DataSaida}} foi emitida e está disponível em anexo.</p><p>Atenciosamente,</p><p><img src=\"http://www.icecards.com.br/media/img/logo_ice_preto.png\" width=\"95\" alt=\"\"/></p></td></tr></table></body></html>",
                    "SaidaMaterialNotaFiscal-SolicitarCancelarNfSaida": "<!DOCTYPE html><html xmlns lang=\"pt-br\"><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=9\"><style type=\"text/css\">body{font-family:Tahoma;font-size:14px}.warning{color:#f30}.highlight{color:#244061}p{margin:0;margin-bottom:15px;padding:0;width:600px;text-align:justify}.box-title{color: #fff;background-color:#244061;border:solid windowtext 2.2pt;text-align:center;font-size:15px;font-weight:700}.box-info{color:#000;background-color:#fff;border:solid windowtext 2.25pt;padding:10px}</style></head><body><table align=\"center\" style=\"width:650px\"><tr><td class=\"box-title\">Cancelar NF {{Numero}}</td></tr><tr><td class=\"box-info\"><p>Por favor proceda ao efetivo cancelamento da NF {{Numero}} no sistema de emissão original, pois ela não será mais utilizada.</p><p>Atenciosamente,</p><p><img src=\"http://www.icecards.com.br/media/img/logo_ice_preto.png\" width=\"95\" alt=\"\"/></p></td></tr></table></body></html>",
                    "SaidaMaterialNotaFiscal-Cadastro":"<!DOCTYPE html><html xmlns lang=\"pt-br\"><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=9\"><style type=\"text/css\">body{font-family:Tahoma;font-size:14px}.warning{color:#f30}.highlight{color:#244061}p{margin:0;margin-bottom:15px;padding:0;width:600px;text-align:justify}.box-title{color: #fff;background-color:#244061;border:solid windowtext 2.2pt;text-align:center;font-size:15px;font-weight:700}.box-info{color:#000;background-color:#fff;border:solid windowtext 2.25pt;padding:10px}</style></head><body><table align=\"center\" style=\"width:650px\"><tr><td class=\"box-title\">Pendência de Emissão de NF</td></tr><tr><td class=\"box-info\"><p>Foi solicitada a emissão de NF no GDocs referente a saída de material – com NF.</p><p>Por favor acesse o GDocs para prosseguir com o atendimento da solicitação. </p><p><a href=\"{{Link}}\">Clique aqui para acessar o GDocs</a></p><p>Atenciosamente,</p><p><img src=\"http://www.icecards.com.br/media/img/logo_ice_preto.png\" width=\"95\" alt=\"\"/></p></td></tr></table></body></html>",
                    "SolicitacaoSaidaMaterial-CancelamentoAceito": "<!DOCTYPE html><html xmlns lang=\"pt-br\"><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=9\"><style type=\"text/css\">body{font-family:Tahoma;font-size:14px}.warning{color:#f30}.highlight{color:#244061}p{margin:0;margin-bottom:15px;padding:0;width:600px;text-align:justify}.box-title{color: #fff;background-color:#244061;border:solid windowtext 2.2pt;text-align:center;font-size:15px;font-weight:700}.box-info{color:#000;background-color:#fff;border:solid windowtext 2.25pt;padding:10px}</style></head><body><table align=\"center\" style=\"width:650px\"><tr><td class=\"box-title\">Cancelamento aceito</td></tr><tr><td class=\"box-info\"><p>Prezado(a) Sr.(a) {{Responsavel}},</p><p>Seu pedido de cancelamento foi aceito e o documento de Saída de Material - sem NF - {{Numero}} foi cancelado.</p><p>Atenciosamente,</p><p><img src=\"http://www.icecards.com.br/media/img/logo_ice_preto.png\" width=\"95\" alt=\"\"/></p></td></tr></table></body></html>",
                    "SolicitacaoSaidaMaterial-CancelamentoRejeitado": "<!DOCTYPE html><html xmlns lang=\"pt-br\"><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=9\"><style type=\"text/css\">body{font-family:Tahoma;font-size:14px}.warning{color:#f30}.highlight{color:#244061}p{margin:0;margin-bottom:15px;padding:0;width:600px;text-align:justify}.box-title{color: #fff;background-color:#f03434;border:solid windowtext 2.2pt;text-align:center;font-size:15px;font-weight:700}.box-info{color:#000;background-color:#fff;border:solid windowtext 2.25pt;padding:10px}</style></head><body><table align=\"center\" style=\"width:650px\"><tr><td class=\"box-title\">Cancelamento rejeitado</td></tr><tr><td class=\"box-info\"><p>Prezado(a) Sr.(a) {{Responsavel}},</p><p>Seu pedido de cancelamento foi rejeitado e o documento de Saída de Material - sem NF - {{Numero}} continua com a situação \"Saída Pendente\".</p><br><p><b>Observações de rejeição:</b></p><ul>{{#each this.Texto}}<li><b>{{this.NomeDiretor}} - {{this.DataRejeicao}}</b> - {{this.Observacao}}</li>{{/each}}</ul><p>Atenciosamente,</p><p><img src=\"http://www.icecards.com.br/media/img/logo_ice_preto.png\" width=\"95\" alt=\"\"/></p></td></tr></table></body></html>",
                    "StatusPagamento-CancelamentoAceito": "<!DOCTYPE html><html xmlns lang=\"pt-br\"><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=9\"><style type=\"text/css\">body{font-family:Tahoma;font-size:14px}.warning{color:#f30}.highlight{color:#244061}p{margin:0;margin-bottom:15px;padding:0;width:600px;text-align:justify}.box-title{color: #fff;background-color:#244061;border:solid windowtext 2.2pt;text-align:center;font-size:15px;font-weight:700}.box-info{color:#000;background-color:#fff;border:solid windowtext 2.25pt;padding:10px}</style></head><body><table align=\"center\" style=\"width:650px\"><tr><td class=\"box-title\">Cancelamento aceito</td></tr><tr><td class=\"box-info\"><p>Prezado(a) Sr.(a) {{Responsavel}},</p><p>Seu pedido de cancelamento foi aceito e o documento de Status de Pagamento - {{Numero}} foi cancelado.</p><p>Atenciosamente,</p><p><img src=\"http://www.icecards.com.br/media/img/logo_ice_preto.png\" width=\"95\" alt=\"\"/></p></td></tr></table></body></html>",
                    "StatusPagamento-CancelamentoRejeitado": "<!DOCTYPE html><html xmlns lang=\"pt-br\"><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=9\"><style type=\"text/css\">body{font-family:Tahoma;font-size:14px}.warning{color:#f30}.highlight{color:#244061}p{margin:0;margin-bottom:15px;padding:0;width:600px;text-align:justify}.box-title{color: #fff;background-color:#f03434;border:solid windowtext 2.2pt;text-align:center;font-size:15px;font-weight:700}.box-info{color:#000;background-color:#fff;border:solid windowtext 2.25pt;padding:10px}</style></head><body><table align=\"center\" style=\"width:650px\"><tr><td class=\"box-title\">Cancelamento rejeitado</td></tr><tr><td class=\"box-info\"><p>Prezado(a) Sr.(a) {{Responsavel}},</p><p>Seu pedido de cancelamento foi rejeitado e o documento de Status de Pagamento - {{Numero}} continua com a situação \"Em Aberto\".</p><br><p><b>Observações de rejeição:</b></p><ul>{{#each this.Texto}}<li><b>{{this.NomeDiretor}} - {{this.DataRejeicao}}</b> - {{this.Observacao}}</li>{{/each}}</ul><p>Atenciosamente,</p><p><img src=\"http://www.icecards.com.br/media/img/logo_ice_preto.png\" width=\"95\" alt=\"\"/></p></td></tr></table></body></html>"
              }
            }',
			'Configuração complementar do appsettings.json da api, faz override parcial na configuração fisica.'
		),
		(
			'appsettings.site',
			'{
              "configuracaoAbasPendenciaAssinatura": [
                1
              ],
              "moedas": [
                {
                  "titulo": "Dolar",
                  "simbolo": "$"
                },
                {
                  "titulo": "Real",
                  "simbolo": "R$"
                },
                {
                  "titulo": "Euro",
                  "simbolo": "€"
                }
              ],
               
              "featureToggles": [
                {
                  "nome": "selecionarApenasArquivosPendentesDeAssinaturaJaVisualizados",
                  "ativo": true
                }
              ],
              "tempoAtualizacaoTelaPendenciaAssinatura": "00:03:00",
              "statusPagamento": {
                "maxColsDescricao": 150,
                "maxRowsDescricao": 6
              }
            }',
			'Configuração para o angular.'
		),
		(
			'appsettings.worker',
			'{
                "TemplatesManifesto": {
                    "Assinatura": "<table class=\"clsTabelaAssinatura\">    <tbody>     <tr>      <td style=\"width: 100%;\">       <ul class=\"clsLista\">        <li class=\"clsListaItem\">Assinado eletronicamente por {{Assinatura.Nome}}</li>        <li>Data: {{Assinatura.Data}}</li>       </ul>      </td>     </tr>     <tr>      <td class=\"clsColunaAssinatura\">       <img class=\"clsImgAssinatura\" style=\"float: left; height: 90px;\" src=\"data:image/png;base64, {{Assinatura.Imagem}}\" alt=\"\" />      </td>     </tr>    </tbody>   </table>",
                    "AssinaturaRepresentante": "<table class=\"clsTabelaAssinatura\">    <tbody>     <tr>      <td style=\"width: 100%;\">       <ul class=\"clsLista\">        <li class=\"clsListaItem\">Assinado eletronicamente por {{Assinatura.NomeRepresentante}}, representando {{Assinatura.Nome}}</li>        <li>Data: {{Assinatura.Data}}</li>       </ul>      </td>     </tr>     <tr>      <td class=\"clsColunaAssinatura\">       <img class=\"clsImgAssinatura\" style=\"float: left; height: 90px;\" src=\"data:image/png;base64, {{Assinatura.Imagem}}\" alt=\"\" />      </td>     </tr>    </tbody>   </table>   ",
                    "LinhaAssinatura": "<tr>    <td style=\"width: 48%;\">#PAINELASSINATURA1</td>    <td style=\"width: 4%;\">&nbsp;</td>    <td style=\"width: 48%;\">#PAINELASSINATURA2</td>   </tr>",
                    "Manifesto": "<html>   <head> <meta charset=\"UTF-8\">    <style>     .clsTabelaPrincipal {      border-collapse: separate;      width: 100%;      border-spacing: 0 15px;      border: none;     }       .clsTabelaAssinatura {      border-collapse: collapse;      width: 100%;      border: 2px solid;     }       .clsLista {      height: 45px;      margin-top: 5px;     }       .clsListaItem {      padding-right: 20px;     }       .clsColunaAssinatura {      width: 100%;      height: 17px;      padding-left: 40px;      padding-right: 40px;     }       .clsImgAssinatura {      float: left;      height: 90px;     }    </style>   </head>   <body>    <table class=\"clsTabelaPrincipal\">     <tbody>      #CONTEUDOMANIFESTO     </tbody>    </table>   </body>   </html>",
                    "Observacao": "<html> <head> <head> <meta charset=\"UTF-8\"> <style> .clsTabelaPrincipal { border-collapse: separate; width: 100%; border-spacing: 0 15px; border: none; } </style> </head> <body> <table class=\"clsTabelaPrincipal\"> <tbody> <tr> <td><strong> Observações </strong></td> </tr> {{#each this}} <tr> <td> <strong>{{Nome}} - {{Data}}</strong><br>{{Justificativa}} <td> </tr> {{/each}} </tbody> </table> </body> </html>"
                },
                "TemplatesEmail": {
                    "AssinaturaPendenteDocumento": "<!DOCTYPE html><html xmlns lang=\"pt-br\"><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=9\"><style type=\"text/css\">body{font-family:Tahoma;font-size:14px}.warning{color:#f30}.highlight{color:#244061}p{margin:0;margin-bottom:15px;padding:0;width:600px;text-align:justify}.box-title{color:#fff;background-color:#244061;border:solid windowtext 2.2pt;text-align:center;font-size:15px;font-weight:700}.box-info{color:#000;background-color:#fff;border:solid windowtext 2.25pt;padding:10px}</style></head><body><table align=\"center\" style=\"width:650px\"><tr><td class=\"box-title\">Documento aguardando sua assinatura</td></tr><tr><td class=\"box-info\"><p>Prezado(a) Sr.(a) {{Usuario.Nome}},</p><p>O documento \"{{ProcessoAssinatura.Titulo}}\" aguarda sua assinatura.</p><p>Para ver todos os seus documentos pendentes <a href=\"{{Sistema.UrlPendentes}}\" class=\"highlight\">clique aqui.</a></p><p>Atenciosamente,</p><p><img src=\"http://www.icecards.com.br/media/img/logo_ice_preto.png\" width=\"95\" alt=\"\"></p></td></tr></table></body></html>",
                    "AssinaturaRejeicaoDocumento":"<!DOCTYPE html><html xmlns lang=\"pt-br\"><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=9\"><style type=\"text/css\">body{font-family:Tahoma;font-size:14px}.warning{color:#f30}.highlight{color:#244061}p{margin:0;margin-bottom:15px;padding:0;width:600px;text-align:justify}.box-title{color:#fff;background-color:#f03434;border:solid windowtext 2.2pt;text-align:center;font-size:15px;font-weight:700}.box-info{color:#000;background-color:#fff;border:solid windowtext 2.25pt;padding:10px}</style></head><body><table align=\"center\" style=\"width:650px\"><tr><td class=\"box-title\">Documento rejeitado</td></tr><tr><td class=\"box-info\"><p>Prezado(a) Sr.(a) {{Usuario.Nome}},</p><p>O documento \"{{ProcessoAssinatura.Titulo}}\" foi rejeitado.</p><p>Número do Documento: \"{{ProcessoAssinatura.NumeroDocumento}}\"</p><p>Categoria: \"{{ProcessoAssinatura.Categoria}}\"</p>{{#if Observacoes}}<p><strong>Observações:</strong></p><ul>{{#each Observacoes}}<li style=\"margin-bottom:5px\"><strong>{{Nome}} - {{Data}}</strong>- {{Observacao}}</li>{{/each}}</ul>{{/if}}<p>Atenciosamente,</p><p><img src=\"http://www.icecards.com.br/media/img/logo_ice_preto.png\" width=\"95\" alt=\"\"></p></td></tr></table></body></html>",
                    "AssinaturaCancelamentoDocumento": "<!DOCTYPE html><html xmlns lang=\"pt-br\"><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=9\"><style type=\"text/css\">body{font-family:Tahoma;font-size:14px}.warning{color:#f30}.highlight{color:#244061}p{margin:0;margin-bottom:15px;padding:0;width:600px;text-align:justify}.box-title{color:#fff;background-color:#244061;border:solid windowtext 2.2pt;text-align:center;font-size:15px;font-weight:700}.box-info{color:#000;background-color:#fff;border:solid windowtext 2.25pt;padding:10px}</style></head><body><table align=\"center\" style=\"width:650px\"><tr><td class=\"box-title\">Documento cancelado</td></tr><tr><td class=\"box-info\"><p>Prezado(a) Sr.(a) {{Usuario.Nome}},</p><p>O documento \"{{ProcessoAssinatura.Titulo}}\" foi cancelado.</p><p>Atenciosamente,</p><p><img src=\"http://www.icecards.com.br/media/img/logo_ice_preto.png\" width=\"95\" alt=\"\"></p></td></tr></table></body></html>",
                    "AssinaturaConcluidoDocumento": "<!DOCTYPE html><html xmlns lang=\"pt-br\"><head><meta http-equiv=\"X-UA-Compatible\" content=\"IE=9\"><style type=\"text/css\">body{font-family:Tahoma;font-size:14px}.warning{color:#f30}.highlight{color:#244061}p{margin:0;margin-bottom:15px;padding:0;width:600px;text-align:justify}.box-title{color: {{Tema.CorTextoTitulo}};background-color:{{Tema.CorFundoTitulo}};border:solid windowtext 2.2pt;text-align:center;font-size:15px;font-weight:700}.box-info{color:#000;background-color:#fff;border:solid windowtext 2.25pt;padding:10px}</style></head><body><table align=\"center\" style=\"width:650px\"><tr><td class=\"box-title\">Documento assinado</td></tr><tr><td class=\"box-info\"><p>Prezado(a) Sr.(a) {{Usuario.Nome}},</p><p>O processo de assinatura do documento \"{{ProcessoAssinatura.Titulo}}\" foi concluído {{#if Observacoes}}com{{#else}}sem{{/if}} observação.</p>{{#if FareiEnvio.Flg}}<p>O envio do documento será feito por: {{FareiEnvio.Nome}}</p>{{/if}} {{#if Observacoes}}<p><strong>Observações:</strong></p><ul>{{#each Observacoes}}<li style=\"margin-bottom:5px\"><strong>{{Nome}}</strong>- {{Observacao}}</li>{{/each}}</ul>{{/if}}<p>Atenciosamente,</p><p><img src=\"http://www.icecards.com.br/media/img/logo_ice_preto.png\" width=\"95\" alt=\"\"></p></td></tr></table></body></html>",
                    "TituloDocumentoConcluido": {
                        "Padrao": {
                            "CorFonte": "#fff",
                            "CorFundo": "#244061"
                        },
                        "ConcluidoComObservacoes": {
                            "CorFonte": "#000",
                            "CorFundo": "#f0ed34"
                        }
                    }
                }
            }',
			'Configuração complementar do appsettings.json da api, faz override parcial na configuração fisica.'
	),
    (
        'configCategorias',
        '[
          {
            "codigo": 1,
            "extensoesPermitidas": "pdf|doc|docx|xls|xlsx|txt|rtf",
            "numeracaoAutomatica": {
              "habilitado": true,
              "mustache": "",
              "chaveSequence": "FI1548StatusPagamento"
            },
            "assinaturaDocumento": {
              "habilitado": false,
              "mustache": ""
            },
            "certificadoDigital": {
              "habilitado": false
            }
          },
          {
            "codigo": 29,
            "extensoesPermitidas": "doc|docx",
            "numeracaoAutomatica": {
              "habilitado": true,
              "mustache": "{{nr_documento}}",
              "chaveSequence": "Oficio"
            },
            "assinaturaDocumento": {
              "habilitado": true,
              "mustache": "{{assinatura}}"
            },
            "certificadoDigital": {
              "habilitado": true
            },
            "flagFareiEnvio": {
              "habilitado": true
            },
            "templateDir": "DIR-MESMO-PASSO"
          },
          {
            "codigo": 30,
            "extensoesPermitidas": "doc|docx",
            "numeracaoAutomatica": {
              "habilitado": true,
              "mustache": "{{nr_documento}}",
              "chaveSequence": "AtaReuniao"
            },
            "assinaturaDocumento": {
              "habilitado": false,
              "mustache": ""
            },
            "certificadoDigital": {
              "habilitado": false
            }
          },
          {
            "codigo": 16,
            "extensoesPermitidas": "pdf|doc|docx|xls|xlsx|txt|rtf",
            "numeracaoAutomatica": {
              "habilitado": true,
              "mustache": "{{nr_documento}}",
              "chaveSequence": "ComunicacaoInterna"
            },
            "assinaturaDocumento": {
              "habilitado": false,
              "mustache": ""
            },
            "certificadoDigital": {
              "habilitado": false
            }
          },
          {
            "codigo": 14,
            "extensoesPermitidas": "xlsx",
            "numeracaoAutomatica": {
              "habilitado": true,
              "mustache": "{{nr_documento}}",
              "chaveSequence": "ArquivoMorto"
            },
            "assinaturaDocumento": {
              "habilitado": false,
              "mustache": ""
            },
            "certificadoDigital": {
              "habilitado": false
            }
          },
          {
            "codigo": 31,
            "extensoesPermitidas": "doc|docx",
            "numeracaoAutomatica": {
              "habilitado": true,
              "mustache": "{{nr_documento}}",
              "chaveSequence": "SaidaMateriais"
            },
            "assinaturaDocumento": {
              "habilitado": false,
              "mustache": ""
            },
            "certificadoDigital": {
              "habilitado": false
            }
          },
          {
            "codigo": 32,
            "extensoesPermitidas": "doc|docx",
            "numeracaoAutomatica": {
              "habilitado": true,
              "mustache": "{{nr_documento}}",
              "chaveSequence": "SaidaMateriais"
            },
            "assinaturaDocumento": {
              "habilitado": false,
              "mustache": ""
            },
            "certificadoDigital": {
              "habilitado": false
            }
          }
        ]',
        'Configuração das categorias - tabela PADC'
    )      
) AS Source 
(cfg_nom_key,cfg_des_value,cfg_des_descricao) ON Target.cfg_nom_key = Source.cfg_nom_key
WHEN MATCHED AND (Target.[cfg_des_value] <> Source.[cfg_des_value] OR Target.[cfg_des_descricao] <> Source.[cfg_des_descricao]) THEN
	UPDATE SET 
		[cfg_des_value] = Source.[cfg_des_value],
		[cfg_des_descricao] = Source.[cfg_des_descricao],
		[cfg_dat_atualizacao] = GETDATE()

WHEN NOT MATCHED BY TARGET THEN
	INSERT (cfg_nom_key,cfg_des_value,cfg_des_descricao) 
	VALUES (Source.[cfg_nom_key],Source.[cfg_des_value],Source.[cfg_des_descricao]);
