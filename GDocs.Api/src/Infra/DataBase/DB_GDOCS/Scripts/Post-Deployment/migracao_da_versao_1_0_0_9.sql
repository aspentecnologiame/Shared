MERGE INTO [dbo].[tb_func_funcionalidade] AS Target
USING (
	VALUES         
        (1, 'fi1548:consultar', 'Status - Pagamentos','Menu para o documento Status pagamentos','fa-file-alt','/fi1548/consultar',1, null,10),
        (2, 'fi1548:adicionar', 'Permite adicionar documento','Habilita a opção de adicionar FIxxxx',null,null,3, 1,10),
        (3, 'fi1548:consultar:todosusuarios:enviarparaassinatura', 'Permite enviar para assinatura de todos os usuários','Habilita a ação para enviar para assinatura',null,null,3, 4,null),
        (4, 'fi1548:consultar:todosusuarios', 'Permite consultar de todos os usuários','Habilita a opção para consultar de todos os usuários',null,null,3, 1, null),
        (5, 'usuario', 'Usuários','Menu da página para listar os usuários com permissões','fa-user','/usuario',1, null,100),
        (6, 'fi1548:consultar:liquidar', 'Permite mudar o status para liquidado','Habilita a ação para que altera o status para liquidado',null,null,3, 1, null), 
        (7, 'fi1548:consultar:todosusuarios:cancelartodos', 'Permite cancelar de todos os usuários','Habilita a ação para cancelar de todos os usuários',null,null,2, 4, null),   
        (8, 'assinatura','Assinatura', 'Menu de assinatura de documentos','fa-file-signature',null,1,null, 20),
        (9, 'assinatura:gerenciardocumentos','Consultar', 'Menu para listar documentos adicionados para fazer o gerenciamento deles.',null,'/assinatura/gerenciamento',1,8, 10),
        (10, 'assinatura:pendencias','Assinar', 'Menu para consultar os documentos pendentes de assinatura/rejeição',null,'/assinatura/pendencias',1,8, 50),
        (11, 'assinatura:pendencias:assinaremlote', 'Permite o usuário assinar documentos em lote','Habilita a ação para assinar em lote',null,null,2, 10, null),	
        (12, 'assinatura:gerenciardocumentos:adicionar', 'Criar fluxo','Habilita o botão para adicionar novos processos de assinatura',null,'/assinatura/adicionar',1, 8, 5),
        (13, 'assinatura:gerenciardocumentos:todosusuarios', 'Permite consultar de todos os usuários','Habilita a opção para consultar de todos os usuários',null,null,3, 9, null),
		(14, 'assinatura:pendencias:exibirstatuspasso', 'Permite o usuário vizualizar o status do passo ativo.','Habilita a visualização do status do passo ativo.',null,null,2, 10, null)
) AS Source 
(func_idt, func_chave, func_texto, func_desc, func_icone, func_rota, tfu_idt, func_idt_pai, func_ordem) ON Target.[func_idt] = Source.[func_idt]
WHEN MATCHED AND (
	Target.[func_chave] <> Source.[func_chave]
	OR Target.[func_texto] <> Source.[func_texto] 
    OR Target.[func_desc] <> Source.[func_desc]
    OR Target.[func_icone] <> Source.[func_icone]
    OR Target.[func_rota] <> Source.[func_rota]
    OR Target.[tfu_idt] <> Source.[tfu_idt]
    OR Target.[func_idt_pai] <> Source.[func_idt_pai]
    OR Target.[func_ordem] <> Source.[func_ordem]
) THEN
	UPDATE SET 
		func_chave = Source.[func_chave],
        func_texto = Source.[func_texto], 
        func_desc = Source.[func_desc], 
        func_icone = Source.[func_icone], 
        func_rota = Source.[func_rota], 
        tfu_idt = Source.[tfu_idt], 
        func_idt_pai = Source.[func_idt_pai], 
        func_ordem = Source.[func_ordem],
        func_flg_ativo = 1,
		func_dat_alteracao = GETDATE()
WHEN NOT MATCHED BY Source AND (Target.[func_flg_ativo] <> 0) THEN
    UPDATE SET 
        func_flg_ativo = 0,
        func_dat_alteracao = GETDATE()
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([func_idt], [func_chave], [func_texto], [func_desc], [func_icone], [func_rota], [tfu_idt], [func_idt_pai], [func_ordem]) 
	VALUES (Source.[func_idt], Source.[func_chave], Source.[func_texto], Source.[func_desc], Source.[func_icone], Source.[func_rota], Source.[tfu_idt], Source.[func_idt_pai], Source.[func_ordem]);

--Consolidado a parte do [dbo].[tb_pfu_perfil_funcionalidade] no "migracao_da_versao_1_0_1_0

MERGE INTO [dbo].[tb_tpadp_template_processo_assinatura_documento_passo] AS Target
USING (
	VALUES
        (1,1,0,1),
        (2,2,0,0)
) AS Source 
(tpadp_idt, tpad_idt, tpadp_ordem, tpadp_flg_aguardar_todos_usuarios) ON Target.[tpadp_idt] = Source.[tpadp_idt]
WHEN MATCHED AND (
	Target.[tpad_idt] <> Source.[tpad_idt]
	OR Target.[tpadp_ordem] <> Source.[tpadp_ordem] 
    OR Target.[tpadp_flg_aguardar_todos_usuarios] <> Source.[tpadp_flg_aguardar_todos_usuarios]     
) THEN
	UPDATE SET 
		tpad_idt = Source.[tpad_idt],
        tpadp_ordem = Source.[tpadp_ordem], 
        tpadp_flg_aguardar_todos_usuarios = Source.[tpadp_flg_aguardar_todos_usuarios],         
        tpadp_flg_ativo = 1,
		tpadp_dat_atualizacao = GETDATE()
WHEN NOT MATCHED BY Source AND (Target.[tpadp_flg_ativo] <> 0) THEN
    UPDATE SET 
        tpadp_flg_ativo = 0,
        tpadp_dat_atualizacao = GETDATE()
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([tpadp_idt], [tpad_idt], [tpadp_ordem], [tpadp_flg_aguardar_todos_usuarios]) 
	VALUES (Source.[tpadp_idt], Source.[tpad_idt], Source.[tpadp_ordem], Source.[tpadp_flg_aguardar_todos_usuarios]);
