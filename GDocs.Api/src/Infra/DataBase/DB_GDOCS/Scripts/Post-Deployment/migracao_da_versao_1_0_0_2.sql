-- Consolidado a parte do tb_pfu_perfil_funcionalidade no "migracao_da_versao_1_0_0_3

MERGE INTO [dbo].[tb_sad_status_assinatura_documento] AS Target
USING (
	VALUES 
		(0, 'Em construção'),  
		(1, @StatusNaoIniciado),
		(2, 'Em andamento'),
		(3, 'Concluído'),
		(4, 'Concluído com rejeição'),
		(5, 'Cancelado')
) AS Source 
(sad_idt,sad_descricao) ON Target.[sad_idt] = Source.[sad_idt]
WHEN MATCHED THEN
	UPDATE SET 
		sad_descricao = Source.sad_descricao
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([sad_idt],[sad_descricao]) 
	VALUES (Source.sad_idt, Source.sad_descricao);

MERGE INTO [dbo].[tb_sadp_status_assinatura_documento_passo] AS Target
USING (
	VALUES 
		(1, @StatusNaoIniciado),
		(2, 'Em andamento'),
		(3, 'Concluído'),
		(4, 'Concluído com rejeição')
) AS Source 
(sadp_idt,sadp_descricao) ON Target.[sadp_idt] = Source.[sadp_idt]
WHEN MATCHED THEN
	UPDATE SET 
		sadp_descricao = Source.sadp_descricao
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([sadp_idt],[sadp_descricao]) 
	VALUES (Source.sadp_idt, Source.sadp_descricao);

MERGE INTO [dbo].[tb_sadpu_status_assinatura_documento_passo_usuario] AS Target
USING (
	VALUES 
		(1, @StatusNaoIniciado),
		(2, 'Aguardando assinatura'),
		(3, 'Assinado'),
		(4, 'Rejeitado')
) AS Source 
(sadpu_idt,sadpu_descricao) ON Target.[sadpu_idt] = Source.[sadpu_idt]
WHEN MATCHED THEN
	UPDATE SET 
		sadpu_descricao = Source.sadpu_descricao
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([sadpu_idt],[sadpu_descricao]) 
	VALUES (Source.sadpu_idt, Source.sadpu_descricao);
