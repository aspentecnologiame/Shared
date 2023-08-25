CREATE TABLE [dbo].[tb_tpadt_template_processo_assinatura_documento_tag]
(
    [tpad_idt] INT NOT NULL,
    [tpadt_tag] VARCHAR(255) NOT NULL,
    [tpadt_flg_ativo] bit CONSTRAINT df_tpadt_flg_ativo DEFAULT(1) NOT NULL,
    [tpadt_dat_inclusao] DATETIME CONSTRAINT df_tpadt_dat_inclusao DEFAULT(GETDATE()) NOT NULL,
    [tpadt_dat_alteracao] DATETIME CONSTRAINT df_tpadt_dat_alteracao DEFAULT(GETDATE()) NOT NULL,
    CONSTRAINT [uk_padt_1] UNIQUE ([tpad_idt],[tpadt_tag]),
    CONSTRAINT [fk_tpad_tpad] FOREIGN KEY ([tpad_idt]) references [dbo].[tb_tpad_template_processo_assinatura_documento] ([tpad_idt])
)
