



/*
	STATUS DA CIENCIA
*/

if not exists (select * from tb_sci_status_ciencia where sci_idt = 1) begin

	insert into tb_sci_status_ciencia values (1, 'Pendente', 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_sci_status_ciencia where sci_idt = 2) begin

	insert into tb_sci_status_ciencia values (2, 'Concluido', 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_sci_status_ciencia where sci_idt = 3) begin

	insert into tb_sci_status_ciencia values (3, 'Cancelado', 1, GETDATE(), GETDATE())

end




/*
	TIPO DE CIENCIA
*/

if not exists (select * from tb_tci_tipo_ciencia where tci_idt = 1) begin

	insert into tb_tci_tipo_ciencia values (1, 'Baixa sem retorno', 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_tci_tipo_ciencia where tci_idt = 2) begin

	insert into tb_tci_tipo_ciencia values (2, 'Prorrogação', 1, GETDATE(), GETDATE())

end




/*
	NOVOS PERFIS
*/

if not exists (select * from tb_per_perfil where per_idt = 8) begin

	insert into tb_per_perfil values (8, 'Diretoria', 1, GETDATE(), GETDATE(), NULL)

end

if not exists (select * from tb_per_perfil where per_idt = 9) begin

	insert into tb_per_perfil values (9, 'Portaria', 1, GETDATE(), GETDATE(), NULL)

end

if not exists (select * from tb_per_perfil where per_idt = 10) begin

	insert into tb_per_perfil values (10, 'Saída de Material', 1, GETDATE(), GETDATE(), NULL)

end




/*
	NOVAS FUNCIONALIDADES
*/

if not exists (select * from tb_func_funcionalidade where func_idt = 15) begin

	insert into tb_func_funcionalidade values (15, 'Solicitar', 'Solicitações', 'Menu de Solicitações', 'fa-address-card', null, 1, null, 15, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_func_funcionalidade where func_idt = 16) begin

	insert into tb_func_funcionalidade values (16, 'fi347:consultar', 'Saída de Materiais', 'Menu de Saida de Materiais', null, '/fi347/consultar', 1, 15, 100, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_func_funcionalidade where func_idt = 17) begin

	insert into tb_func_funcionalidade values (17, 'assinatura:aba:assinados', 'Assinar Aba Assinados', 'Habilita aba Assinados', null, null, 3, 8, null, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_func_funcionalidade where func_idt = 18) begin

	insert into tb_func_funcionalidade values (18, 'fi347:ciencia:pendente', 'Assinar Aba Ciência', 'Habilita aba Ciência', null, null, 3, 8, null, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_func_funcionalidade where func_idt = 19) begin

	insert into tb_func_funcionalidade values (19, 'fi347:adicionar', 'Permite cadastrar documento FI347', 'Permite cadastrar documento FI347', null, null, 3, 1, null, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_func_funcionalidade where func_idt = 20) begin

	insert into tb_func_funcionalidade values (20, 'fi347:consultar:todosusuarios', 'Permite consultar saida material de todos os usuários', 'Habilita a opção para consultar saida material de todos os usuários', null, null, 3, 1, null, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_func_funcionalidade where func_idt = 21) begin

	insert into tb_func_funcionalidade values (21, 'fi347:enviar:assinatura', 'Permite acesso a enviar documento para assinatura', 'Permite acesso a enviar documento para assinatura', null, null, 3, 1, null, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_func_funcionalidade where func_idt = 22) begin

	insert into tb_func_funcionalidade values (22, 'fi347:consultar:todosusuarios:cancelartodos', 'Permite cancelar saida de material de todos os usuarios', 'Habilita a ação para cancelar saida de mateiral de todos os usuários', null, null, 2, 20, null, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_func_funcionalidade where func_idt = 23) begin

	insert into tb_func_funcionalidade values (23, 'fi347:acoes:saida', 'Permite acesso a saida de material', 'Permite acesso a saida de material', null, null, 3, 16, null, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_func_funcionalidade where func_idt = 24) begin

	insert into tb_func_funcionalidade values (24, 'fi347:acoes:retorno', 'Permite acesso ao retorno de material', 'Permite acesso ao retorno de material', null, null, 3, 16, null, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_func_funcionalidade where func_idt = 25) begin

	insert into tb_func_funcionalidade values (25, 'fi347:acoes:prorrogacao', 'Permite acesso a função de prorrogação', 'Permite acesso a função de prorrogação', null, null, 3, 16, null, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_func_funcionalidade where func_idt = 26) begin

	insert into tb_func_funcionalidade values (26, 'fi347:acoes:baixasemretorno', 'Permite acesso a função baixa sem retorno', 'Permite acesso a função baixa sem retorno', null, null, 3, 16, null, 1, GETDATE(), GETDATE())

end

if exists (select * from tb_func_funcionalidade where func_idt = 1) begin

	update tb_func_funcionalidade set func_idt_pai = 15 where func_idt = 1

end


/*
	NOVAS FUNCIONALIDADES X PERFILS NOVOS
*/

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 1 and func_idt = 15) begin

	insert into tb_pfu_perfil_funcionalidade values (1 ,15 ,0, getdate(), getdate())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 2 and func_idt = 15) begin

	insert into tb_pfu_perfil_funcionalidade values (2 ,15 ,1, getdate(), getdate())

end


if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 2 and func_idt = 21) begin

	insert into tb_pfu_perfil_funcionalidade values (2 ,21 ,1, getdate(), getdate())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 2 and func_idt = 23) begin

	insert into tb_pfu_perfil_funcionalidade values (2 ,23 ,1, getdate(), getdate())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 2 and func_idt = 24) begin

	insert into tb_pfu_perfil_funcionalidade values (2 ,24 ,1, getdate(), getdate())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 2 and func_idt = 25) begin

	insert into tb_pfu_perfil_funcionalidade values (2 ,25 ,1, getdate(), getdate())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 2 and func_idt = 19) begin

	insert into tb_pfu_perfil_funcionalidade values (2 ,19 ,1, getdate(), getdate())

end


if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 3 and func_idt = 15) begin

	insert into tb_pfu_perfil_funcionalidade values (3 ,15 ,1, getdate(), getdate())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 4 and func_idt = 15) begin

	insert into tb_pfu_perfil_funcionalidade values (4 ,15 ,1, getdate(), getdate())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 5 and func_idt = 15) begin

	insert into tb_pfu_perfil_funcionalidade values (5 ,15 ,1, getdate(), getdate())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 6 and func_idt = 15) begin

	insert into tb_pfu_perfil_funcionalidade values (6 ,15 ,1, getdate(), getdate())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 1 and func_idt = 16) begin

	insert into tb_pfu_perfil_funcionalidade values (1 ,16 ,0, getdate(), getdate())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 2 and func_idt = 16) begin

	insert into tb_pfu_perfil_funcionalidade values (2 ,16 ,1, getdate(), getdate())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 3 and func_idt = 16) begin

	insert into tb_pfu_perfil_funcionalidade values (3 ,16 ,1, getdate(), getdate())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 4 and func_idt = 16) begin

	insert into tb_pfu_perfil_funcionalidade values (4 ,16 ,1, getdate(), getdate())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 5 and func_idt = 16) begin

	insert into tb_pfu_perfil_funcionalidade values (5 ,16 ,1, getdate(), getdate())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 6 and func_idt = 16) begin

	insert into tb_pfu_perfil_funcionalidade values (6 ,16 ,1, getdate(), getdate())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 8 and func_idt = 1) begin

	insert into tb_pfu_perfil_funcionalidade values (8, 1, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 8 and func_idt = 2) begin

	insert into tb_pfu_perfil_funcionalidade values (8, 2, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 8 and func_idt = 4) begin

	insert into tb_pfu_perfil_funcionalidade values (8, 4, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 8 and func_idt = 8) begin

	insert into tb_pfu_perfil_funcionalidade values (8, 8, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 8 and func_idt = 9) begin

	insert into tb_pfu_perfil_funcionalidade values (8, 9, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 8 and func_idt = 10) begin

	insert into tb_pfu_perfil_funcionalidade values (8, 10, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 8 and func_idt = 11) begin

	insert into tb_pfu_perfil_funcionalidade values (8, 11, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 8 and func_idt = 12) begin

	insert into tb_pfu_perfil_funcionalidade values (8, 12, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 8 and func_idt = 13) begin

	insert into tb_pfu_perfil_funcionalidade values (8, 13, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 8 and func_idt = 14) begin

	insert into tb_pfu_perfil_funcionalidade values (8, 14, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 8 and func_idt = 17) begin

	insert into tb_pfu_perfil_funcionalidade values (8, 17, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 8 and func_idt = 18) begin

	insert into tb_pfu_perfil_funcionalidade values (8, 18, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 9 and func_idt = 15) begin

	insert into tb_pfu_perfil_funcionalidade values (9, 15, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 9 and func_idt = 16) begin

	insert into tb_pfu_perfil_funcionalidade values (9, 16, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 9 and func_idt = 20) begin

	insert into tb_pfu_perfil_funcionalidade values (9, 20, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 9 and func_idt = 23) begin

	insert into tb_pfu_perfil_funcionalidade values (9, 23, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 9 and func_idt = 24) begin

	insert into tb_pfu_perfil_funcionalidade values (9, 24, 1, GETDATE(), GETDATE())

end


if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 10 and func_idt = 8) begin

	insert into tb_pfu_perfil_funcionalidade values (10, 8, 1, GETDATE(), GETDATE())

end


if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 10 and func_idt = 10) begin

	insert into tb_pfu_perfil_funcionalidade values (10, 10, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 10 and func_idt = 15) begin

	insert into tb_pfu_perfil_funcionalidade values (10, 15, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 10 and func_idt = 16) begin

	insert into tb_pfu_perfil_funcionalidade values (10, 16, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 10 and func_idt = 19) begin

	insert into tb_pfu_perfil_funcionalidade values (10, 19, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 10 and func_idt = 20) begin

	insert into tb_pfu_perfil_funcionalidade values (10, 20, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 10 and func_idt = 21) begin

	insert into tb_pfu_perfil_funcionalidade values (10, 21, 1, GETDATE(), GETDATE())

end


if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 10 and func_idt = 22) begin

	insert into tb_pfu_perfil_funcionalidade values (10, 22, 1, GETDATE(), GETDATE())

end


if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 10 and func_idt = 23) begin

	insert into tb_pfu_perfil_funcionalidade values (10, 23, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 10 and func_idt = 25) begin

	insert into tb_pfu_perfil_funcionalidade values (10, 25, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 2 and func_idt = 26) begin

	insert into tb_pfu_perfil_funcionalidade values (2, 26, 1, GETDATE(), GETDATE())

end





DECLARE @SERVIDOR AS VARCHAR(50)

set @SERVIDOR = @@SERVERNAME

IF (@SERVIDOR <> 'ICEBD01') BEGIN

	--USUARIO TESTE 1 (DIRETORIA TST/HOM)
	update tb_usp_usuario_perfil set usp_flg_ativo = 0, usp_dat_alteracao = GETDATE() where usp_guid_ad = '3b2726b6-0221-4131-bb38-fe1f8eb1c45a' and usp_flg_ativo = 1 --teste1

	if exists (select * from tb_usp_usuario_perfil where usp_guid_ad = '3b2726b6-0221-4131-bb38-fe1f8eb1c45a' and per_idt = 8) begin

		update tb_usp_usuario_perfil set usp_flg_ativo = 1, usp_dat_alteracao = GETDATE() where usp_guid_ad = '3b2726b6-0221-4131-bb38-fe1f8eb1c45a' and per_idt = 8

	end else begin

		insert into tb_usp_usuario_perfil (usp_guid_ad,per_idt,usp_flg_ativo,usp_dat_inclusao,usp_dat_alteracao) values ('3b2726b6-0221-4131-bb38-fe1f8eb1c45a', 8, 1, GETDATE(), GETDATE())

	end

	--USUARIO TESTE 2 (DIRETORIA TST/HOM)
	update tb_usp_usuario_perfil set usp_flg_ativo = 0, usp_dat_alteracao = GETDATE() where usp_guid_ad = '413db42b-c3f4-4faa-92ff-42e3241e9ddd' and usp_flg_ativo = 1 --teste1

	if exists (select * from tb_usp_usuario_perfil where usp_guid_ad = '413db42b-c3f4-4faa-92ff-42e3241e9ddd' and per_idt = 8) begin

		update tb_usp_usuario_perfil set usp_flg_ativo = 1, usp_dat_alteracao = GETDATE() where usp_guid_ad = '413db42b-c3f4-4faa-92ff-42e3241e9ddd' and per_idt = 8

	end else begin

		insert into tb_usp_usuario_perfil (usp_guid_ad,per_idt,usp_flg_ativo,usp_dat_inclusao,usp_dat_alteracao) values ('413db42b-c3f4-4faa-92ff-42e3241e9ddd', 8, 1, GETDATE(), GETDATE())

	end

END

--USUARIO PAULO (DIRETORIA)
update tb_usp_usuario_perfil set usp_flg_ativo = 0, usp_dat_alteracao = GETDATE() where usp_guid_ad = '5bb1bd6c-651f-456b-9039-c4673d0fac82' and usp_flg_ativo = 1 --teste1

if exists (select * from tb_usp_usuario_perfil where usp_guid_ad = '5bb1bd6c-651f-456b-9039-c4673d0fac82' and per_idt = 8) begin

	update tb_usp_usuario_perfil set usp_flg_ativo = 1, usp_dat_alteracao = GETDATE() where usp_guid_ad = '5bb1bd6c-651f-456b-9039-c4673d0fac82' and per_idt = 8

end else begin

	insert into tb_usp_usuario_perfil (usp_guid_ad,per_idt,usp_flg_ativo,usp_dat_inclusao,usp_dat_alteracao) values ('5bb1bd6c-651f-456b-9039-c4673d0fac82', 8, 1, GETDATE(), GETDATE())

end

--USUARIO IGNACIO (DIRETORIA)
update tb_usp_usuario_perfil set usp_flg_ativo = 0, usp_dat_alteracao = GETDATE() where usp_guid_ad = 'b014f744-29de-4447-9d84-9bdc9501ff9d' and usp_flg_ativo = 1 --teste1

if exists (select * from tb_usp_usuario_perfil where usp_guid_ad = 'b014f744-29de-4447-9d84-9bdc9501ff9d' and per_idt = 8) begin

	update tb_usp_usuario_perfil set usp_flg_ativo = 1, usp_dat_alteracao = GETDATE() where usp_guid_ad = 'b014f744-29de-4447-9d84-9bdc9501ff9d' and per_idt = 8

end else begin

	insert into tb_usp_usuario_perfil (usp_guid_ad,per_idt,usp_flg_ativo,usp_dat_inclusao,usp_dat_alteracao) values ('b014f744-29de-4447-9d84-9bdc9501ff9d', 8, 1, GETDATE(), GETDATE())

end


if exists (select * from tb_tci_tipo_ciencia where tci_idt = 1) begin

	update tb_tci_tipo_ciencia set tci_des = 'Baixa sem retorno',  tci_flg_ativo = 1, tci_dat_alteracao = GETDATE() where tci_idt = 1

end else begin

	insert into tb_tci_tipo_ciencia values (1, 'Baixa sem retorno', 1, GETDATE(), GETDATE())

end

if exists (select * from tb_tci_tipo_ciencia where tci_idt = 2) begin

	update tb_tci_tipo_ciencia set tci_des = 'Prorrogação',  tci_flg_ativo = 1, tci_dat_alteracao = GETDATE() where tci_idt = 2

end else begin

	insert into tb_tci_tipo_ciencia values (2, 'Prorrogação', 1, GETDATE(), GETDATE())

end


if exists (select * from tb_tci_tipo_ciencia where tci_idt = 3) begin

	update tb_tci_tipo_ciencia set tci_des = 'Cancelamento',  tci_flg_ativo = 1, tci_dat_alteracao = GETDATE() where tci_idt = 3

end else begin

	insert into tb_tci_tipo_ciencia values (3, 'Cancelamento', 1, GETDATE(), GETDATE())

end



if exists (select * from tb_sci_status_ciencia where sci_idt = 1) begin

	update tb_sci_status_ciencia set sci_des = 'Pendente',  sci_flg_ativo = 1, sci_dat_alteracao = GETDATE() where sci_idt = 1

end else begin

	insert into tb_sci_status_ciencia values (1, 'Pendente', 1, GETDATE(), GETDATE())

end

if exists (select * from tb_sci_status_ciencia where sci_idt = 2) begin

	update tb_sci_status_ciencia set sci_des = 'Concluido',  sci_flg_ativo = 1, sci_dat_alteracao = GETDATE() where sci_idt = 2

end else begin

	insert into tb_sci_status_ciencia values (2, 'Concluido', 1, GETDATE(), GETDATE())

end

if exists (select * from tb_sci_status_ciencia where sci_idt = 3) begin

	update tb_sci_status_ciencia set sci_des = 'Cancelado',  sci_flg_ativo = 1, sci_dat_alteracao = GETDATE() where sci_idt = 3

end else begin

	insert into tb_sci_status_ciencia values (3, 'Cancelado', 1, GETDATE(), GETDATE())

end

if not exists (SELECT rel_idt FROM [dbo].[tb_rel_relatorio] WHERE rel_idt = 1) begin

	INSERT INTO [dbo].[tb_rel_relatorio]
           ([rel_nome]
           ,[rel_url]
           ,[rel_parametros]
           ,[rel_ativo]
           ,[rel_dat_criacao]
           ,[rel_dat_atualizacao])
     VALUES
           ('AUTORIZAÇÕES DE SAÍDA DE MATERIAL'
           ,'/DW/GDocs/ICE+FI+2396_email'
           ,'nome=null&tipo_relatorio=D'
		   ,1
           ,GETDATE()
           ,GETDATE())

end

UPDATE [dbo].[tb_func_funcionalidade] SET func_texto = 'Aprovações' where func_chave = 'assinatura'
UPDATE [dbo].[tb_func_funcionalidade] SET func_texto = 'Aprovar' where func_chave = 'assinatura:pendencias'

/*
	TIPO DA ACAO NA SAIDA DE MATERIAL
*/

if not exists (select * from tb_smta_saida_material_tipo_acao where smta_idt = 1) begin

	insert into tb_smta_saida_material_tipo_acao (smta_idt, smta_des, tci_idt, smta_flg_ativo, smta_dat_criacao, smta_dat_atualizacao) values (1, 'Registro de saída', null, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_smta_saida_material_tipo_acao where smta_idt = 2) begin

	insert into tb_smta_saida_material_tipo_acao (smta_idt, smta_des, tci_idt, smta_flg_ativo, smta_dat_criacao, smta_dat_atualizacao) values (2, 'Registro de retorno', null, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_smta_saida_material_tipo_acao where smta_idt = 3) begin

	insert into tb_smta_saida_material_tipo_acao (smta_idt, smta_des, tci_idt, smta_flg_ativo, smta_dat_criacao, smta_dat_atualizacao) values (3, 'Solicitação de prorrogação', 2, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_smta_saida_material_tipo_acao where smta_idt = 4) begin

	insert into tb_smta_saida_material_tipo_acao (smta_idt, smta_des, tci_idt, smta_flg_ativo, smta_dat_criacao, smta_dat_atualizacao) values (4, 'Solicitação de cancelamento', null, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_smta_saida_material_tipo_acao where smta_idt = 5) begin

	insert into tb_smta_saida_material_tipo_acao (smta_idt, smta_des, tci_idt, smta_flg_ativo, smta_dat_criacao, smta_dat_atualizacao) values (5, 'Baixa de material sem retorno', null, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_smta_saida_material_tipo_acao where smta_idt = 6) begin
	insert into tb_smta_saida_material_tipo_acao (smta_idt, smta_des, tci_idt, smta_flg_ativo, smta_dat_criacao, smta_dat_atualizacao) values (6, 'Solicitaçao Baixa de material sem retorno', 1, 1, GETDATE(), GETDATE())
end

/*
	STATUS DA SAIDA DE MATERIAL
*/

if not exists (select * from tb_stsm_status_saida_material where stsm_idt = 0) begin

	insert into tb_stsm_status_saida_material values (0, 'Em construção', 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_stsm_status_saida_material where stsm_idt = 1) begin

	insert into tb_stsm_status_saida_material values (1, 'Em Aprovação', 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_stsm_status_saida_material where stsm_idt = 2) begin

	insert into tb_stsm_status_saida_material values (2, 'Saída Pendente', 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_stsm_status_saida_material where stsm_idt = 3) begin

	insert into tb_stsm_status_saida_material values (3, 'Reprovado', 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_stsm_status_saida_material where stsm_idt = 4) begin

	insert into tb_stsm_status_saida_material values (4, 'Cancelado', 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_stsm_status_saida_material where stsm_idt = 5) begin

	insert into tb_stsm_status_saida_material values (5, 'Em Aberto', 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_stsm_status_saida_material where stsm_idt = 6) begin

	insert into tb_stsm_status_saida_material values (6, 'Concluída', 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_stsm_status_saida_material where stsm_idt = 7) begin

	insert into tb_stsm_status_saida_material values (7, 'Em Aprovação de Ciência', 1, GETDATE(), GETDATE())

end


ALTER TABLE tb_ssm_solicitacao_saida_material ALTER COLUMN ssm_des_observacao VARCHAR (255);

ALTER TABLE tb_ssm_solicitacao_saida_material ALTER COLUMN ssm_des_motivo VARCHAR (200);
