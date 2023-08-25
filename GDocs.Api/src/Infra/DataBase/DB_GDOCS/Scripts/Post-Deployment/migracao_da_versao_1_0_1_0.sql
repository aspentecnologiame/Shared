MERGE INTO [dbo].[tb_per_perfil] AS Target
USING (
	VALUES 
		(1, 'Convidado', 1),
		(2, 'Administrador',1),
		(3, 'Financeiro', 1),
		(4, 'Consulta Completa - Status-Pagamentos', 1),
		(5, 'Aprovação em lote', 1),
		(6, 'Acesso básico', 1),
		(7, 'Consulta Completa - Assinatura', 1)
) AS Source 
(per_idt,per_des,per_flg_ativo) ON Target.[per_idt] = Source.[per_idt]
WHEN MATCHED AND (Target.[per_des] <> Source.[per_des] OR Target.[per_flg_ativo] <> Source.[per_flg_ativo]) THEN
	UPDATE SET 
		per_des = Source.[per_des],
		per_flg_ativo = Source.[per_flg_ativo],
		per_dat_alteracao = GETDATE()
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([per_idt],[per_des],[per_flg_ativo],[per_dat_inclusao],[per_dat_alteracao]) 
	VALUES (Source.[per_idt],Source.[per_des],Source.[per_flg_ativo],GETDATE(),GETDATE());


MERGE INTO [dbo].[tb_pfu_perfil_funcionalidade] AS Target
USING (
	VALUES         		
		(1,6), (1,2), (1,3), (1,4), (2,6), (2,2), (2,3), (2,4), 
		(5,2), (8,1), (8,6), (8,2), (8,3), (8,4), (8,5),(8,7), (9,6), (9,2), (9,3), (9,4), (9,7),  
		(10,1), (10,6), (10,2), (10,3), (10,4), (10,5),  (4,2), (4,3), (4,4), (12,6), (12,2), (12,3), (12,4),
		(13,2),(13,7), (6,3),(6,2), (7,2), (11,5), (3,2), (14,6), (14,2), (14,3), (14,4), (14,5)
) AS Source 
(func_idt,per_idt) ON Target.[func_idt] = Source.[func_idt] AND Target.[per_idt] = Source.[per_idt]
WHEN MATCHED AND (Target.[pfu_flg_ativo] <> 1) THEN
	UPDATE SET 
		pfu_flg_ativo = 1,
		pfu_dat_alteracao = GETDATE()
WHEN NOT MATCHED BY Source AND (Target.[pfu_flg_ativo] <> 0) THEN
	UPDATE SET 
		pfu_flg_ativo = 0,
		pfu_dat_alteracao = GETDATE()
WHEN NOT MATCHED BY TARGET THEN
	INSERT (func_idt,per_idt) 
	VALUES (Source.[func_idt],Source.[per_idt]);

--Consolidado a parte do [dbo].[tb_padc_processo_assinatura_documento_categoria] no "migracao_da_versao_1_0_1_2
	
-- Atribuindo a permissão "Acesso básico" para todos os usuários que já criaram documentos
INSERT [dbo].[tb_usp_usuario_perfil]  (usp_guid_ad,per_idt)
SELECT DISTINCT pad_guid_ad, 6 per_idt FROM [dbo].[tb_pad_processo_assinatura_documento] PAD
LEFT JOIN [dbo].[tb_usp_usuario_perfil] USP ON USP.usp_guid_ad = PAD.pad_guid_ad AND per_idt = 6
WHERE USP.usp_guid_ad IS NULL

INSERT [dbo].[tb_usp_usuario_perfil]  (usp_guid_ad,per_idt)
SELECT DISTINCT dfi_guid_usuario_ad, 6 per_idt FROM [dbo].[tb_dfi_documento_fi1548] DFI
LEFT JOIN [dbo].[tb_usp_usuario_perfil] USP ON USP.usp_guid_ad = DFI.dfi_guid_usuario_ad AND per_idt = 6
WHERE USP.usp_guid_ad IS NULL

-- Atribuindo a permissão "Acesso básico" para a DIR
IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[tb_usp_usuario_perfil] WHERE usp_guid_ad = '5BB1BD6C-651F-456B-9039-C4673D0FAC82' AND per_idt = 6)
BEGIN
	INSERT [dbo].[tb_usp_usuario_perfil] (usp_guid_ad,per_idt)
	VALUES ('5BB1BD6C-651F-456B-9039-C4673D0FAC82',6)
END

IF NOT EXISTS (SELECT TOP 1 1 FROM [dbo].[tb_usp_usuario_perfil] WHERE usp_guid_ad = 'B014F744-29DE-4447-9D84-9BDC9501FF9D' AND per_idt = 6)
BEGIN
	INSERT [dbo].[tb_usp_usuario_perfil] (usp_guid_ad,per_idt)
	VALUES ('B014F744-29DE-4447-9D84-9BDC9501FF9D',6)
END

UPDATE [dbo].[tb_tpadpu_template_processo_assinatura_documento_passo_usuario] 
SET tpadpu_notificar_finalizacao_fluxo = @NomeFluxoDir, tpadpu_dat_atualizacao = GETDATE() 
WHERE tpadpu_guid_ad IN ('5bb1bd6c-651f-456b-9039-c4673d0fac82','b014f744-29de-4447-9d84-9bdc9501ff9d')
