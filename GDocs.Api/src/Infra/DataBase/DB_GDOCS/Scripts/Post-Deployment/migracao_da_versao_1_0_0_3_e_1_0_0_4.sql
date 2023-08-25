--Consolidado a parte do [dbo].[tb_per_perfil] no "migracao_da_versao_1_0_1_0

MERGE INTO [dbo].[tb_sdo_status_documento] AS Target
USING (
	VALUES 
		(0, 'Em construção'),
		(1, 'Pendente de aprovação'),
		(2, 'Em Aberto'),
		(3, 'Reprovado'),
		(4, 'Cancelado'),
		(5, 'Liquidado')
) AS Source 
(sdo_idt,sdo_des) ON Target.[sdo_idt] = Source.[sdo_idt]
WHEN MATCHED AND (Target.[sdo_des] <> Source.[sdo_des]) THEN
	UPDATE SET 
		sdo_des = Source.[sdo_des],
		sdo_dat_atualizacao = GETDATE()
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([sdo_idt],[sdo_des]) 
	VALUES (Source.[sdo_idt],Source.[sdo_des]);


--Consolidado a parte do [tb_func_funcionalidade] no "migracao_da_versao_1_0_0_7

--Consolidado a parte do [tb_pfu_perfil_funcionalidade] no "migracao_da_versao_1_0_0_7
