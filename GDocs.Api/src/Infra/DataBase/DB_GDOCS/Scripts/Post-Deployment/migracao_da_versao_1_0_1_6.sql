--OS 1398
DECLARE @smta_idt INT
SELECT @smta_idt = smta_idt FROM tb_smta_saida_material_tipo_acao WHERE smta_des = 'Solicitação de cancelamento'
IF (@smta_idt <> 0)
BEGIN
	UPDATE tb_smta_saida_material_tipo_acao SET tci_idt = 3 WHERE smta_idt = @smta_idt
END
ELSE
BEGIN
	INSERT INTO tb_smta_saida_material_tipo_acao VALUES (4, 'Solicitação de cancelamento', 1, GETDATE(), GETDATE(), 3)
END

IF (SELECT tnnf_idt FROM tb_tnnf_tipo_natureza_nota_fiscal WHERE tnnf_des = 'Outras Saídas') IS NULL
BEGIN
	insert into tb_tnnf_tipo_natureza_nota_fiscal values ('Outras Saídas', 1, getdate(), getdate())
END

IF (SELECT tnnf_idt FROM tb_tnnf_tipo_natureza_nota_fiscal WHERE tnnf_des = 'Demonstração') IS NULL
BEGIN
	insert into tb_tnnf_tipo_natureza_nota_fiscal values ('Demonstração', 1, getdate(), getdate())
END

IF (SELECT dfitci_idt FROM tb_dfitci_tipo_ciencia_fi1548 WHERE dfitci_des = 'Cancelamento') IS NULL
BEGIN
	INSERT INTO tb_dfitci_tipo_ciencia_fi1548 VALUES (1, 'Cancelamento', 1, GETDATE(), GETDATE())
END

IF (SELECT dfisci_idt FROM tb_dfisci_status_ciencia_fi1548 WHERE dfisci_des = 'Pendente') IS NULL
BEGIN
	INSERT INTO tb_dfisci_status_ciencia_fi1548 VALUES (1, 'Pendente', 1, GETDATE(), GETDATE())
END

IF (SELECT dfisci_idt FROM tb_dfisci_status_ciencia_fi1548 WHERE dfisci_des = 'Concluído') IS NULL
BEGIN
	INSERT INTO tb_dfisci_status_ciencia_fi1548 VALUES (2, 'Concluído', 1, GETDATE(), GETDATE())
END

IF (SELECT dfisci_idt FROM tb_dfisci_status_ciencia_fi1548 WHERE dfisci_des = 'Rejeitado') IS NULL
BEGIN
	INSERT INTO tb_dfisci_status_ciencia_fi1548 VALUES (3, 'Rejeitado', 1, GETDATE(), GETDATE())
END

IF (SELECT sdo_idt FROM tb_sdo_status_documento WHERE sdo_des = 'Em aprovação de ciência') IS NULL
BEGIN
	INSERT INTO tb_sdo_status_documento VALUES (6, 'Em aprovação de ciência', 1, GETDATE(), GETDATE())
END

IF NOT EXISTS (SELECT * FROM tb_func_funcionalidade WHERE func_idt = 43) 
BEGIN
	INSERT INTO tb_func_funcionalidade VALUES (43, 'fi1548:ciencia:pendente', 'Assinar Aba Ciência', 'Habilita aba Ciência', null, null, 3, 8, null, 1, GETDATE(), GETDATE())
END

IF NOT EXISTS (SELECT * FROM tb_pfu_perfil_funcionalidade where per_idt = 8 AND func_idt = 43)
BEGIN
	INSERT INTO tb_pfu_perfil_funcionalidade VALUES (8, 43, 1, GETDATE(), GETDATE())
END


if not exists (select * from tb_pfu_perfil_funcionalidade where per_idt = 9 and func_idt = 34) begin
	insert into tb_pfu_perfil_funcionalidade values (9 ,34 ,1, getdate(), getdate())
end

update tb_per_perfil set per_peso = 1 where per_idt = 12