MERGE INTO [dbo].[tb_padc_processo_assinatura_documento_categoria] AS Target
USING (
	VALUES 
		(1	,'Status pagamento',1),
		(2	,'LH',1),
		(3	,'LI',1),
		(4	,'LP',1),
		(5	,'OS',1),
		(6	,'RH',1),
		(7	,'RE',1),
		(8	,'Lista de Pedidos TOTVS (D-1) - Pagto. Único',1),
		(9	,'Lista de Pedidos TOTVS (D-1) - Pagto. Parcelado',1),
		(10	,'Lista de Pedidos TOTVS (D-1) – Pedidos Pontuais',1),
		(11	,'Solicitações do RH',1),
		(12	,'Movimentações de Colaborador',1),
		(13	,'Processo de Recrutamento',1),
		(14	,'Movimentações de Arquivo Morto',1),
		(15	,'Autorização de Saída de Materiais',1),
		(16	,'Comunicação Interna',1),
		(17	,'LD',1),
		(18	,'Reajustes de Contratos',1),		    
		(19, 'Solicitações de NN - Jurídico',1),
		(20, 'Demandas PAGCORP',1),
		(21, 'Artefatos Desenvolvimento',1),
		(22, 'Solicitações da SEC',1),
		(23, 'Solicitações da Qualidade',1),
		(24, 'Projetos: Aprovação de Plantas e Layouts',1)
) AS Source 
(padc_idt,padc_descricao,padc_flg_ativo) ON Target.[padc_idt] = Source.[padc_idt]
WHEN MATCHED AND (Target.[padc_descricao] <> Source.[padc_descricao] OR Target.[padc_flg_ativo] <> Source.[padc_flg_ativo]) THEN
	UPDATE SET 
		padc_descricao = Source.[padc_descricao],
		padc_flg_ativo = Source.[padc_flg_ativo],
		padc_dat_atualizacao = GETDATE()
WHEN NOT MATCHED BY TARGET THEN
	INSERT ([padc_idt],[padc_descricao]) 
	VALUES (Source.[padc_idt],Source.[padc_descricao]);
