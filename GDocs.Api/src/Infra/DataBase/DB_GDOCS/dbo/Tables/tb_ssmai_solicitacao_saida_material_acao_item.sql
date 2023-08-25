CREATE TABLE [dbo].[tb_ssmai_solicitacao_saida_material_acao_item] (
    [ssmai_idt]             INT      IDENTITY (1, 1) NOT NULL,
    [ssma_idt]              INT      NOT NULL,
    [ssmi_idt]              INT      NOT NULL,
    [ssmai_flg_ativo]       BIT      CONSTRAINT [df_ssmai_flg_ativo] DEFAULT ((1)) NOT NULL,
    [ssmai_dat_criacao]     DATETIME CONSTRAINT [df_ssmai_dat_criacao] DEFAULT (getdate()) NOT NULL,
    [ssmai_dat_atualizacao] DATETIME CONSTRAINT [df_ssmai_dat_atualizacao] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [pk_ssmai] PRIMARY KEY CLUSTERED ([ssmai_idt] ASC),
    CONSTRAINT [fk_ssmmi_ssmi] FOREIGN KEY ([ssmi_idt]) REFERENCES [dbo].[tb_ssmi_solicitacao_saida_material_item] ([ssmi_idt]),
    CONSTRAINT [fk_ssmmi_ssmm] FOREIGN KEY ([ssma_idt]) REFERENCES [dbo].[tb_ssma_solicitacao_saida_material_acao] ([ssma_idt])
);

