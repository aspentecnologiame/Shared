INSERT [dbo].[tb_tpl_tipo_log] (tpl_idt,tpl_nom)
VALUES 
    (1,	'Erro'),
    (2,	'Atenção'),
    (3,	'Informação'),
    (4,	'Rastreamento');

-- Movido para o arquivo configuracoes.sql para evitar duplicações no sonar

INSERT INTO tb_per_perfil (per_idt,per_des,per_flg_ativo,per_dat_inclusao,per_dat_alteracao)
VALUES 
    (1, 'Convidado', 1, GETDATE(), GETDATE()),
    (2, 'Administrador', 1, GETDATE(), GETDATE()),
    (3, 'Financeiro', 1, GETDATE(), GETDATE()),
    (4, 'Consulta Completa - Status-Pagamentos', 1, GETDATE(), GETDATE()),
    (5, 'Aprovação em lote', 1, GETDATE(), GETDATE()),
    (6, 'Acesso básico', 1, GETDATE(), GETDATE()),
    (7, 'Consulta Completa - Assinatura', 1, GETDATE(), GETDATE());

INSERT tb_tfu_tipo_funcionalidade ([tfu_idt],[tfu_des])
VALUES (1,'Menu'),
       (2, 'Acao'),
       (3,'Acesso');

INSERT [tb_func_funcionalidade] (func_idt, func_chave, func_texto, func_desc, func_icone, func_rota, tfu_idt, func_idt_pai, func_ordem)
VALUES 
    -- Menu
    (1, 'fi1548:consultar', 'Status - Pagamentos','Menu para o documento Status pagamentos','fa-file-alt','/fi1548/consultar',1, null,10),
        
    (5, 'usuario', 'Usuários','Menu da página para listar os usuários com permissões','fa-user','/usuario',1, null,100),
    (8, 'assinatura','Assinatura', 'Menu de assinatura de documentos','fa-file-signature',null,1,null, 20),

    (9, 'assinatura:gerenciardocumentos','Consultar', 'Menu para listar documentos adicionados para fazer o gerenciamento deles.',null,'/assinatura/gerenciamento',1,8, 10),
    (10, 'assinatura:pendencias','Assinar', 'Menu para consultar os documentos pendentes de assinatura/rejeição',null,'/assinatura/pendencias',1,8, 50),  
    (12, 'assinatura:gerenciardocumentos:adicionar', 'Criar fluxo','Habilita o botão para adicionar novos processos de assinatura',null,'/assinatura/adicionar',1, 8, 5),
    
    -- Acesso
    (4, 'fi1548:consultar:todosusuarios', 'Permite consultar de todos os usuários','Habilita a opção para consultar de todos os usuários',null,null,3, 1, null),
	(2, 'fi1548:adicionar', 'Permite adicionar documento','Habilita a opção de adicionar FIxxxx',null,null,3, 1,10),    
    (13, 'assinatura:gerenciardocumentos:todosusuarios', 'Permite consultar de todos os usuários','Habilita a opção para consultar de todos os usuários',null,null,3, 9, null),
    
    -- Acao;
    (6, 'fi1548:consultar:liquidar', 'Permite mudar o status para liquidado','Habilita a ação para que altera o status para liquidado',null,null,3, 1, null),
    (7, 'fi1548:consultar:todosusuarios:cancelartodos', 'Permite cancelar de todos os usuários','Habilita a ação para cancelar de todos os usuários',null,null,2, 4, null),   
    (11, 'assinatura:pendencias:assinaremlote', 'Permite o usuário assinar documentos em lote','Habilita a ação para assinar em lote',null,null,2, 10, null),
    (14, 'assinatura:pendencias:exibirstatuspasso', 'Permite o usuário vizualizar o status do passo ativo.','Habilita a visualização do status do passo ativo.',null,null,2, 10, null),
	(3, 'fi1548:consultar:todosusuarios:enviarparaassinatura', 'Permite enviar para assinatura de todos os usuários','Habilita a ação para enviar para assinatura',null,null,3, 4,null);    
       

INSERT [dbo].[tb_pfu_perfil_funcionalidade] (func_idt,per_idt)
VALUES 
    -- Menu
    (1,6), (1,2), (1,3), (1,4), -- fixxxx (todos perfis)
    (2,6), (2,2), (2,3), (2,4), -- fixxxx:adicionar (todos perfis)         
    (5,2), -- usuario (apenas administrador)    
    (8,1), (8,6), (8,2), (8,3), (8,4), (8,5), (8,7), -- assinatura (todos perfis)
    (9,6), (9,2), (9,3), (9,4),(9,7),  -- assinatura:gerenciardocumentos (todos perfis)
    (10,1), (10,6), (10,2), (10,3), (10,4), (10,5),  -- assinatura:pendencias (todos perfis)
    -- Acesso
    (4,2), (4,3), (4,4), -- fixxxx:consulta:todosusuarios (administrador, financeiro, Consulta Completa)
    (12,6), (12,2), (12,3), (12,4), -- assinatura:gerenciardocumentos:adicionar (todos perfis)
    (13,2),(13,7), -- assinatura:gerenciardocumentos:todosusuarios(administrador)
    -- Acao;
    (6,3),(6,2), -- fi1548:consultar:liquidar (Financeiro e administrador)
    (7,2), -- fi1548:consultar:todosusuarios:cancelartodos (apenas administrador)
    (11,5), -- assinatura:pendencias:assinaremlote (apenas Aprovação em lote)
	(3,2), -- fi1548:consultar:todosusuarios:enviarparaassinatura (apenas administrador)  
    (14,6), (14,2), (14,3), (14,4), (14,5);  -- assinatura:pendencias:exibirstatuspasso (todos perfis)


INSERT [dbo].[tb_sdo_status_documento] (sdo_idt,sdo_des)
VALUES 
    (0, 'Em construção'),
    (1, 'Pendente de aprovação'),
    (2, 'Em Aberto'),
    (3, 'Reprovado'),
    (4, 'Cancelado'),
    (5, 'Liquidado');

INSERT [dbo].[tb_sad_status_assinatura_documento] ([sad_idt],[sad_descricao])
VALUES 
    (0, 'Em construção'), -- Será utilizado quando criado por algum processo automático e enviado para editar (quando salvar irá para o status 1)
    (1, @StatusNaoIniciado), -- Job ainda não pegou o processo para enviar os emails aos usuarios do primeiro passo (quando ocorrer o evento de inicialização das assinaturas irá para o status 2)
    (2, 'Em andamento'), -- Processo de assinatura está em andamento, aguardando todos assinarem (quando ocorrer esse evento irá para o status 3) ou uma rejeição (quando esse evento ocorrer irá para o status 4)
    (3, 'Concluído'), -- Todos os usuarios assinaram (fim do processo).
    (4, 'Concluído com rejeição'), -- Houve uma ou mais rejeição (fim do processo).
    (5, 'Cancelado') -- Usuário com permissão cancelou o processo (fim do processo).

INSERT [dbo].[tb_sadp_status_assinatura_documento_passo] ([sadp_idt], [sadp_descricao])
VALUES
    (1, @StatusNaoIniciado), -- Job ainda não iniciou o processo de assinatura do passo (permanace neste status até o evento de iniciar passo mude para o status 2)
    (2, 'Em andamento'), -- Processo de assinatura está em andamento, aguardando todos do passo assinarem (quando ocorrer esse evento irá para o status 3) ou uma rejeição (quando esse evento ocorrer irá para o status 4)
    (3, 'Concluído'), -- Todos os usuarios assinaram (fim do processo).
    (4, 'Concluído com rejeição') -- Houve uma ou mais rejeição (fim do processo).

INSERT [dbo].[tb_sadpu_status_assinatura_documento_passo_usuario] ([sadpu_idt],[sadpu_descricao])
VALUES
    (1, @StatusNaoIniciado), -- Job ainda não iniciou o processo de assinatura do usuário  (permanace neste status até o evento de iniciar assinatura mude para o status 2)
    (2, 'Aguardando assinatura'), -- Usuário ainda não assinou (quando ocorrer esse evento irá para o status 3) ou rejeitou o documento (quando esse evento ocorrer irá para o status 4)
    (3, 'Assinado'), -- Usuario assinou (fim do processo).
    (4, 'Rejeitado') -- Usuario rejeitou (fim do processo).

INSERT INTO tb_tpad_template_processo_assinatura_documento (tpad_idt, tpad_nome) 
VALUES 
    (1, 'DIR'),
    (2, 'EnvioEmailFimProcesso')

INSERT INTO tb_tpadp_template_processo_assinatura_documento_passo (tpadp_idt, tpad_idt, tpadp_ordem, tpadp_flg_aguardar_todos_usuarios) 
VALUES
    (1,1,0,1),
    (2,2,0,0),
    (3,1,1,1)

INSERT INTO tb_tpadpu_template_processo_assinatura_documento_passo_usuario (tpadpu_idt,tpadp_idt,tpadpu_guid_ad, tpadpu_flg_notificar_finalizacao, tpadpu_notificar_finalizacao_fluxo) 
VALUES
    (1, 1,'5bb1bd6c-651f-456b-9039-c4673d0fac82',1,@NomeFluxoDir),
    (2, 3,'b014f744-29de-4447-9d84-9bdc9501ff9d',1,@NomeFluxoDir),
    (3, 2,'0d7b2ea6-d4ec-4bb7-b418-beeb791cc837',1,@NomeFluxoPadrao),
    (4, 2,'5d93a6ef-8cd0-4193-b153-210abebdadbf',1,@NomeFluxoPadrao),
	(5, 2,'641fe8ef-88ea-403f-9c0f-7082ed2f8d56',1,@NomeFluxoPadrao)

INSERT INTO tb_tpadt_template_processo_assinatura_documento_tag (tpad_idt,tpadt_tag) 
VALUES 
    (1,'DIR-NOTIFICACAO'),
	(2,'EnvioEmailFimProcesso-LI'),
	(2,'EnvioEmailFimProcesso-RE'),
	(2,'EnvioEmailFimProcesso-LH'),
	(2,'EnvioEmailFimProcesso-RH'),
	(2,'EnvioEmailFimProcesso-LP'),
	(2,'EnvioEmailFimProcesso-LD'),
	(2,'EnvioEmailFimProcesso-OS')

INSERT INTO tb_padc_processo_assinatura_documento_categoria (padc_idt,padc_descricao) 
VALUES 
    (1	,'Status pagamento'),
    (2	,'LH'),
    (3	,'LI'),
    (4	,'LP'),
    (5	,'OS'),
    (6	,'RH'),
    (7	,'RE'),
    (8	,'Lista de Pedidos TOTVS (D-1) - Pagto. Único'),
    (9	,'Lista de Pedidos TOTVS (D-1) - Pagto. Parcelado'),
    (10	,'Lista de Pedidos TOTVS (D-1) – Pedidos Pontuais'),
    (11	,'Solicitações do RH'),
    (12	,'Movimentações de Colaborador'),
    (13	,'Processo de Recrutamento'),
    (14	,'Movimentações de Arquivo Morto'),
    (15	,'Autorização de Saída de Materiais'),
    (16	,'Comunicação Interna'),
    (17	,'LD'),
    (18	,'Reajustes de Contratos'),
    (19, 'Solicitações de NN - Jurídico'),
    (20, 'Demandas PAGCORP'),
    (21, 'Artefatos Desenvolvimento'),
    (22, 'Solicitações da SEC'),
    (23, 'Solicitações da Qualidade'),
    (24, 'Projetos: Aprovação de Plantas e Layouts'),
    (25, 'Inventário Estoque Trimestral ALM'),
	(26, 'Inventário'),
	(27, 'LIC'),
	(28, 'Solicitação de Horas Extras'),
	(29, 'Ofício'),
	(30, 'Ata de Reunião')

INSERT INTO [dbo].[tb_seq_sequencia] (seq_nom, seq_val, seq_inc)
VALUES
    ('FI1548StatusPagamento', 0,	1),
	('Oficio', 0,	1),
	('AtaReuniao', 0,	1),
	('SaidaMateriais', 0,	1),
	('ComunicacaoInterna', 0,	1),
	('ArquivoMorto', 0,	1)