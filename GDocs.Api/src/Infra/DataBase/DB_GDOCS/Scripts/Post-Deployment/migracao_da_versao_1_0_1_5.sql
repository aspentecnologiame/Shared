 --AJUSTE 

if  exists (select * from tb_tnnf_tipo_natureza_nota_fiscal where tnnf_idt = 3 and tnnf_des = 'Concerto') begin
	update  tb_tnnf_tipo_natureza_nota_fiscal  set tnnf_des ='Conserto' where  tnnf_idt = 3
end
 
 


-- Tipo de Natureza
if not exists (select * from tb_tnnf_tipo_natureza_nota_fiscal where tnnf_des = 'Amostra') begin
	insert into tb_tnnf_tipo_natureza_nota_fiscal values ('Amostra', 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_tnnf_tipo_natureza_nota_fiscal where tnnf_des = 'Beneficiamento') begin
	insert into tb_tnnf_tipo_natureza_nota_fiscal values ('Beneficiamento', 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_tnnf_tipo_natureza_nota_fiscal where tnnf_des = 'Conserto') begin
	insert into tb_tnnf_tipo_natureza_nota_fiscal values ('Conserto', 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_tnnf_tipo_natureza_nota_fiscal where tnnf_des = 'Devolução') begin
	insert into tb_tnnf_tipo_natureza_nota_fiscal values ('Devolução', 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_tnnf_tipo_natureza_nota_fiscal where tnnf_des = 'Doação') begin
	insert into tb_tnnf_tipo_natureza_nota_fiscal values ('Doação', 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_tnnf_tipo_natureza_nota_fiscal where tnnf_des = 'Transferência') begin
	insert into tb_tnnf_tipo_natureza_nota_fiscal values ('Transferência', 1, GETDATE(), GETDATE())
end

-- Modalidade de Frete
if not exists (select * from tb_mfnf_modalidade_frete_nota_fiscal where mfnf_des = 'Por conta do remetente') begin
	insert into tb_mfnf_modalidade_frete_nota_fiscal values ('Por conta do remetente', 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_mfnf_modalidade_frete_nota_fiscal where mfnf_des = 'Por conta do Destinatário') begin
	insert into tb_mfnf_modalidade_frete_nota_fiscal values ('Por conta do Destinatário', 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_mfnf_modalidade_frete_nota_fiscal where mfnf_des = 'Sem Frete') begin
	insert into tb_mfnf_modalidade_frete_nota_fiscal values ('Sem Frete', 1, GETDATE(), GETDATE())
end	

--Tipo de documento
if not exists (select * from tb_tdnf_tipo_documento_nota_fiscal where tdnf_des = 'Nota fiscal de saída') begin
	insert into tb_tdnf_tipo_documento_nota_fiscal values ('Nota fiscal de saída', 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_tdnf_tipo_documento_nota_fiscal where tdnf_des = 'Nota fiscal de retorno') begin
	insert into tb_tdnf_tipo_documento_nota_fiscal values ('Nota fiscal de retorno', 1, GETDATE(), GETDATE())
end




/*
	STATUS DA CIENCIA
*/
if not exists (select * from tb_scinf_status_ciencia_nota_fiscal where scinf_idt = 1) begin
	insert into tb_scinf_status_ciencia_nota_fiscal values (1, 'Pendente', 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_scinf_status_ciencia_nota_fiscal where scinf_idt = 2) begin
	insert into tb_scinf_status_ciencia_nota_fiscal values (2, 'Concluído', 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_scinf_status_ciencia_nota_fiscal where scinf_idt = 3) begin
	insert into tb_scinf_status_ciencia_nota_fiscal values (3, 'Rejeitado', 1, GETDATE(), GETDATE())
end


/*
	STATUS DA CIENCIA sem NF -padronizando nome
*/
if not exists (select * from tb_sci_status_ciencia where sci_idt = 3 and sci_des = 'Cancelado') begin
	update  tb_sci_status_ciencia  set sci_des ='Rejeitado' where  sci_idt = 3
end

if not exists (select * from tb_sci_status_ciencia where sci_idt = 2 and sci_des = 'Concluido') begin
	update  tb_sci_status_ciencia  set sci_des ='Concluído' where  sci_idt = 2
end




/*
	TIPO DE CIENCIA
*/

if not exists (select * from tb_tcinf_tipo_ciencia_nota_fiscal where tcinf_idt = 1) begin

	insert into tb_tcinf_tipo_ciencia_nota_fiscal values (1, 'Baixa sem retorno', 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_tcinf_tipo_ciencia_nota_fiscal where tcinf_idt = 2) begin

	insert into tb_tcinf_tipo_ciencia_nota_fiscal values (2, 'Prorrogação', 1, GETDATE(), GETDATE())

end



/*
	TIPO DA ACAO NA SAIDA DE MATERIAL
*/

if not exists (select * from tb_smtanf_saida_material_tipo_acao_nota_fiscal where smtanf_idt = 1) begin

	insert into tb_smtanf_saida_material_tipo_acao_nota_fiscal (smtanf_idt, smtanf_des, tcinf_idt, smtanf_flg_ativo, smtanf_dat_criacao, smtanf_dat_atualizacao) values (1, 'Registro de saída', null, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_smtanf_saida_material_tipo_acao_nota_fiscal where smtanf_idt = 2) begin

	insert into tb_smtanf_saida_material_tipo_acao_nota_fiscal (smtanf_idt, smtanf_des, tcinf_idt, smtanf_flg_ativo, smtanf_dat_criacao, smtanf_dat_atualizacao) values (2, 'Registro de retorno', null, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_smtanf_saida_material_tipo_acao_nota_fiscal where smtanf_idt = 3) begin

	insert into tb_smtanf_saida_material_tipo_acao_nota_fiscal (smtanf_idt, smtanf_des, tcinf_idt, smtanf_flg_ativo, smtanf_dat_criacao, smtanf_dat_atualizacao) values (3, 'Solicitação de prorrogação', 2, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_smtanf_saida_material_tipo_acao_nota_fiscal where smtanf_idt = 4) begin

	insert into tb_smtanf_saida_material_tipo_acao_nota_fiscal (smtanf_idt, smtanf_des, tcinf_idt, smtanf_flg_ativo, smtanf_dat_criacao, smtanf_dat_atualizacao) values (4, 'Solicitação de cancelamento', null, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_smtanf_saida_material_tipo_acao_nota_fiscal where smtanf_idt = 5) begin

	insert into tb_smtanf_saida_material_tipo_acao_nota_fiscal (smtanf_idt, smtanf_des, tcinf_idt, smtanf_flg_ativo, smtanf_dat_criacao, smtanf_dat_atualizacao) values (5, 'Baixa de material sem retorno', null, 1, GETDATE(), GETDATE())

end

if not exists (select * from tb_smtanf_saida_material_tipo_acao_nota_fiscal where smtanf_idt = 6) begin
	insert into tb_smtanf_saida_material_tipo_acao_nota_fiscal (smtanf_idt, smtanf_des, tcinf_idt, smtanf_flg_ativo, smtanf_dat_criacao, smtanf_dat_atualizacao) values (6, 'Solicitaçao Baixa de material sem retorno', 1, 1, GETDATE(), GETDATE())
end



-- status nota fiscal 
if not exists (select * from tb_stsmnf_status_saida_material_nota_fiscal where stsmnf_des = 'Pendente NF Saída') begin
	insert into tb_stsmnf_status_saida_material_nota_fiscal values (1,'Pendente NF Saída', 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_stsmnf_status_saida_material_nota_fiscal where stsmnf_des = 'Saída Pendente') begin
	insert into tb_stsmnf_status_saida_material_nota_fiscal values (2,'Saída Pendente', 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_stsmnf_status_saida_material_nota_fiscal where stsmnf_des = 'Em aberto') begin
	insert into tb_stsmnf_status_saida_material_nota_fiscal values (3,'Em aberto', 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_stsmnf_status_saida_material_nota_fiscal where stsmnf_des = 'Pendente NF Retorno') begin
	insert into tb_stsmnf_status_saida_material_nota_fiscal values (4,'Pendente NF Retorno', 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_stsmnf_status_saida_material_nota_fiscal where stsmnf_des = 'Em aprovação de ciência') begin
	insert into tb_stsmnf_status_saida_material_nota_fiscal values (5,'Em aprovação de ciência', 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_stsmnf_status_saida_material_nota_fiscal where stsmnf_des = 'Pendente NF Cancelamento') begin
	insert into tb_stsmnf_status_saida_material_nota_fiscal values (6,'Pendente NF Cancelamento', 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_stsmnf_status_saida_material_nota_fiscal where stsmnf_des = 'Cancelada') begin
	insert into tb_stsmnf_status_saida_material_nota_fiscal values (7,'Cancelada', 1, GETDATE(), GETDATE())
end

 if not exists (select * from tb_stsmnf_status_saida_material_nota_fiscal where stsmnf_des = 'Concluída') begin
	insert into tb_stsmnf_status_saida_material_nota_fiscal values (8,'Concluída', 1, GETDATE(), GETDATE())
end
 

-- Perfil

if not exists (select * from tb_per_perfil where per_idt = 12) begin
	insert into tb_per_perfil values (12, 'Contabilidade', 1, GETDATE(), GETDATE(), NULL)
end


/*
	NOVAS FUNCIONALIDADES
*/

if not exists (select * from tb_func_funcionalidade where func_idt = 27) begin
	insert into tb_func_funcionalidade values
	(27, 'SaidaMaterialNF:consulta', 'Saída de Material - com NF', 'Saída de Material Com Nota Fiscal', null, '/SaidaMaterialNotaFiscal/consulta', 1, 15, 99, 1, GETDATE(), GETDATE())
end
else
begin
	update tb_func_funcionalidade set func_texto = 'Saída de Material - com NF', func_desc = 'Menu de Saida de Material com Nota Fiscal' Where func_idt = 27
end


if not exists (select * from tb_func_funcionalidade where func_idt = 28) begin
	insert into tb_func_funcionalidade 
	values (28, 'SaidaMaterialNF:acao:saida', 'Permite acesso a saida de material com NF', 'Permite acesso a saida de materialcom NF', null, null, 3, 27, null, 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_func_funcionalidade where func_idt = 29) begin
	insert into tb_func_funcionalidade 
	values (29, 'SaidaMaterialNF:acao:retorno', 'Permite acesso a retorno de material com NF', 'Permite acesso a retorno de materialcom NF', null, null, 3, 27, null, 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_func_funcionalidade where func_idt = 30) begin
	insert into tb_func_funcionalidade 
	values (30, 'SaidaMaterialNF:consulta:todosusuario', 'Permite consultar saida material de todos os usuários', 'Permite consultar saida material de todos os usuários', null, null, 3, 27, null, 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_func_funcionalidade where func_idt = 31) begin
	insert into tb_func_funcionalidade 
	values (31, 'SaidaMaterialNF:acao:prorrogacao', 'Permite acesso a função de prorrogação', 'Permite acesso a função de prorrogação', null, null, 3, 27, null, 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_func_funcionalidade where func_idt = 32) begin
	insert into tb_func_funcionalidade 
	values (32, 'SaidaMaterialNF:acao:baixasemretorno', 'Permite acesso a função baixa sem retorno', 'Permite acesso a função baixa sem retorno', null, null, 3, 27, null, 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_func_funcionalidade where func_idt = 33) begin
	insert into tb_func_funcionalidade 
	values (33, 'SaidaMaterialNF:acao:adiciona', 'Permite cadastrar documento', 'Permite cadastrar documento', null, null, 3, 27, null, 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_func_funcionalidade where func_idt = 34) begin
	insert into tb_func_funcionalidade 
	values (34, 'SaidaMaterialNF:acao:historico', 'Permite ver historico', 'Permite ver historico', null, null, 3, 27, null, 1, GETDATE(), GETDATE())
end


if not exists (select * from tb_func_funcionalidade where func_idt = 35) begin
	insert into tb_func_funcionalidade 
	values (35, 'SaidaMaterialNF:acao:uploadNotaSaida', 'Permite ver historico', 'Permite ver historico', null, null, 3, 27, null, 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_func_funcionalidade where func_idt = 36) begin
	insert into tb_func_funcionalidade 
	values (36, 'SaidaMaterialNF:acao:verDetalhes', 'Permite ver detalhes', 'habilita ver detalhe', null, null, 3, 27, null, 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_func_funcionalidade where func_idt = 37) begin
	insert into tb_func_funcionalidade 
	values (37, 'SaidaMaterialNF:acao:trocarNotaFiscal', 'Permite alterar a nota fiscal', 'Habilita alterar a nota fiscal', null, null, 3, 27, null, 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_func_funcionalidade where func_idt = 38) begin
	insert into tb_func_funcionalidade 
	values (38, 'SaidaMaterialNF:acao:efetivarCancelamento', 'Permite ver historico', 'Permite ver historico', null, null, 3, 27, null, 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_func_funcionalidade where func_idt = 39) begin
	insert into tb_func_funcionalidade 
	values (39, 'SaidaMaterialNF:acao:cancelar', 'Permite cancelar uma solicitacao ou solicitar cancelamento', 'Permite cancelar uma solicitacao ou solicitar cancelamento', null, null, 3, 27, null, 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_func_funcionalidade where func_idt = 40) begin
	insert into tb_func_funcionalidade 
	values (40, 'SaidaMaterialNF:ciencia:pendente', 'Assinar Aba Ciência', 'Habilita aba Ciência', null, null, 3, 27, null, 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_func_funcionalidade where func_idt = 41) begin
	insert into tb_func_funcionalidade 
	values (41, 'SaidaMaterialNF:acao:retorno:uploadNf', 'Permite acesso a função de uploda nf de retorno', 'Habilita retorno parcial, upload nota fiscal de retorno', null, null, 3, 27, null, 1, GETDATE(), GETDATE())
end

/*
	NOVAS FUNCIONALIDADES X PERFILS NOVOS
*/

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 6 and func_idt = 27) begin
		insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao)
	    values (6,27,1,GETDATE(),GETDATE())
end


if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 6 and func_idt = 31) begin
		insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao)
	    values (6,31,1,GETDATE(),GETDATE())
end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 6 and func_idt = 32) begin
		insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao)
	    values (6,32,1,GETDATE(),GETDATE())
end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 6 and func_idt = 33) begin
		insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao)
	    values (6,33,1,GETDATE(),GETDATE())
end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 6 and func_idt = 34) begin
		insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao)
	    values (6,34,1,GETDATE(),GETDATE())
end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 6 and func_idt = 36) begin
		insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao)
	    values (6,36,1,GETDATE(),GETDATE())
end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 6 and func_idt = 39) begin
		insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao)
	    values (6,39,1,GETDATE(),GETDATE())
end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 6 and func_idt = 41) begin
		insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao)
	    values (6,41,1,GETDATE(),GETDATE())
end

--DIRETORIA
if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 8 and func_idt = 40) begin
		insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao)
	    values (8,40,1,GETDATE(),GETDATE())
end

--segurança
if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 9 and func_idt = 27) begin
		insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao)
	    values (9,27,1,GETDATE(),GETDATE())
end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 9 and func_idt = 28) begin
		insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao)
	    values (9,28,1,GETDATE(),GETDATE())
end


if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 9 and func_idt = 29) begin
		insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao)
	    values (9,29,1,GETDATE(),GETDATE())
end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 9 and func_idt = 30) begin
		insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao)
	    values (9,30,1,GETDATE(),GETDATE())
end
 
 --- contabilidade
 if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 12 and func_idt = 15) begin
		insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao)
	    values (12,15,1,GETDATE(),GETDATE())
end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 12 and func_idt = 27) begin
		insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao)
	    values (12,27,1,GETDATE(),GETDATE())
end

 if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 12 and func_idt = 35) begin
		insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao)
	    values (12,35,1,GETDATE(),GETDATE())
end
 
 if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 12 and func_idt = 36) begin
		insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao)
	    values (12,36,1,GETDATE(),GETDATE())
end

 if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 12 and func_idt = 37) begin
		insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao)
	    values (12,37,1,GETDATE(),GETDATE())
end

  if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 12 and func_idt = 38) begin
		insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao)
	    values (12,38,1,GETDATE(),GETDATE())
end
 
   if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 12 and func_idt = 27) begin
		insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao)
	    values (12,27,1,GETDATE(),GETDATE())
end

  if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 12 and func_idt = 30) begin
		insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao)
	    values (12,30,1,GETDATE(),GETDATE())
 end

 if exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 2 and func_idt = 20) begin

	delete from tb_pfu_perfil_funcionalidade where per_idt = 2 and func_idt = 20

end

if exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 6 and func_idt = 30) begin
		delete from tb_pfu_perfil_funcionalidade where per_idt = 6 and func_idt = 30
end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 2 and func_idt = 27) begin

	insert into tb_pfu_perfil_funcionalidade values (2 ,27 ,1, getdate(), getdate())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 2 and func_idt = 33) begin

	insert into tb_pfu_perfil_funcionalidade values (2 ,33 ,1, getdate(), getdate())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 2 and func_idt = 28) begin

	insert into tb_pfu_perfil_funcionalidade values (2 ,28 ,1, getdate(), getdate())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 2 and func_idt = 29) begin

	insert into tb_pfu_perfil_funcionalidade values (2 ,29 ,1, getdate(), getdate())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 2 and func_idt = 31) begin

	insert into tb_pfu_perfil_funcionalidade values (2 ,31 ,1, getdate(), getdate())

end

if exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 2 and func_idt = 4) begin
		delete from tb_pfu_perfil_funcionalidade where per_idt = 2 and func_idt = 4
end

if exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 2 and func_idt = 13) begin
		delete from tb_pfu_perfil_funcionalidade where per_idt = 2 and func_idt = 13
end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 7 and func_idt = 4) begin

	insert into tb_pfu_perfil_funcionalidade values (7 ,4 ,1, getdate(), getdate())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 7 and func_idt = 13) begin

	insert into tb_pfu_perfil_funcionalidade values (7 ,13 ,1, getdate(), getdate())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 7 and func_idt = 20) begin

	insert into tb_pfu_perfil_funcionalidade values (7 ,20 ,1, getdate(), getdate())

end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 7 and func_idt = 30) begin

	insert into tb_pfu_perfil_funcionalidade values (7 ,30 ,1, getdate(), getdate())

end

--Atualização de menu
declare @FuncIdt INT = (select func_idt from tb_func_funcionalidade where func_texto = 'Saída de Materiais' OR func_texto = 'Saída de Materiais - sem NF')
if @FuncIdt is not null
begin
	update tb_func_funcionalidade set func_texto = 'Saída de Material - sem NF', func_desc = 'Menu de Saida de Material sem Nota Fiscal' Where func_idt = @FuncIdt
end


IF NOT EXISTS (SELECT rel_idt FROM tb_rel_relatorio WHERE rel_idt = 1)
BEGIN
	INSERT INTO [tb_rel_relatorio]
           ([rel_nome]
           ,[rel_url]
           ,[rel_parametros]
           ,[rel_ativo]
           ,[rel_dat_criacao]
           ,[rel_dat_atualizacao])
     VALUES
           ('Autorizações de Saída de Material - sem NF'
           ,'/DW/GDocs/ICE+FI+2396_email'
           ,'nome=null&tipo_relatorio=D'
           ,1
           ,GETDATE()
           ,GETDATE())
END
ELSE
BEGIN
	UPDATE [tb_rel_relatorio] SET [rel_nome] = 'Autorizações de Saída de Material - sem NF' WHERE [rel_idt] = 1
END

IF NOT EXISTS (SELECT rel_idt FROM tb_rel_relatorio WHERE rel_idt = 2)
BEGIN
	INSERT INTO [tb_rel_relatorio]
           ([rel_nome]
           ,[rel_url]
           ,[rel_parametros]
           ,[rel_ativo]
           ,[rel_dat_criacao]
           ,[rel_dat_atualizacao])
     VALUES
           ('Autorizações de Saída de Material - com NF'
           ,'/DW/GDocs/ICE+FI+2405_email'
           ,'nome=null&tipo_relatorio=D'
           ,1
           ,GETDATE()
           ,GETDATE())
END
ELSE
BEGIN
	UPDATE [tb_rel_relatorio] SET [rel_nome] = 'Autorizações de Saída de Material - com NF' WHERE [rel_idt] = 2
END

IF EXISTS (SELECT 1 FROM sys.columns 
          WHERE Name = N'rel_idt'
          AND Object_ID = Object_ID(N'tb_ntu_notificacao_usuario'))
BEGIN
    UPDATE [tb_ntu_notificacao_usuario] SET [rel_idt] = 1
END

if not exists (select * from tb_func_funcionalidade where func_idt = 42) begin
	insert into tb_func_funcionalidade 
	values (42, 'Notificacoes:Relatorio', 'Permite acesso a notificações de reltários', 'Permite acesso a notificações de reltários não lidos', null, null, 3, null, null, 1, GETDATE(), GETDATE())
end

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 6 and func_idt = 42) begin
		insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao)
	    values (6,42,1,GETDATE(),GETDATE())
end


if exists (select * from tb_padc_processo_assinatura_documento_categoria where padc_idt = 31) begin
 update  tb_padc_processo_assinatura_documento_categoria  set padc_descricao = 'Autorização de Saída de Material - Com Retorno'  , padc_flg_ativo = 1 where padc_idt = 31
end

if exists (select * from tb_padc_processo_assinatura_documento_categoria where padc_idt = 32) begin
 update  tb_padc_processo_assinatura_documento_categoria  set padc_flg_ativo = 1  where padc_idt = 32
end
 
  -- categoria  deleta categoria
if   exists (select * from tb_padc_processo_assinatura_documento_categoria where padc_descricao = 'Saída de Material com nota fiscal' and padc_idt = 35 ) begin
	delete tb_padc_processo_assinatura_documento_categoria where padc_idt = 35 
end

-- Perfil
if not exists (select * from tb_per_perfil where per_idt = 13) begin 
	insert into tb_per_perfil values (13, 'Almoxarifado', 1, GETDATE(), GETDATE(), NULL)
end 

--NOVAS FUNCIONALIDADES X PERFILS NOVOS
if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 13 and func_idt = 20) begin 
	insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao) values (13,20,1,GETDATE(),GETDATE())
end 

if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 13 and func_idt = 30) begin 
	insert into tb_pfu_perfil_funcionalidade (per_idt, func_idt, pfu_flg_ativo, pfu_dat_inclusao, pfu_dat_alteracao) values (13,30,1,GETDATE(),GETDATE())
end