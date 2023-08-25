CREATE TABLE [dbo].[tb_soc_solicitacao_ciencia] (
    [soc_idt]             INT              IDENTITY (1, 1) NOT NULL,
    [ssm_idt]             INT              NOT NULL,
    [tci_idt]             INT              NOT NULL,
    [sci_idt]             INT              NOT NULL,
    [ssma_idt]            INT              NOT NULL,
    [soc_flg_ativo]       BIT              CONSTRAINT [df_soc_flg_ativo] DEFAULT ((1)) NOT NULL,
    [soc_usu_guid_ad]     UNIQUEIDENTIFIER NOT NULL,
    [soc_des_observacao]  VARCHAR (200)    NULL,
    [soc_dat_prorrogacao] DATETIME         NULL,
    [soc_dat_criacao]     DATETIME         CONSTRAINT [df_soc_dat_criacao] DEFAULT (getdate()) NOT NULL,
    [soc_dat_atualizacao] DATETIME         CONSTRAINT [df_soc_dat_atualizacao] DEFAULT (getdate()) NULL,
    CONSTRAINT [pk_soc] PRIMARY KEY CLUSTERED ([soc_idt] ASC),
    CONSTRAINT [fk_soc_sci] FOREIGN KEY ([sci_idt]) REFERENCES [dbo].[tb_sci_status_ciencia] ([sci_idt]),
    CONSTRAINT [fk_soc_ssm] FOREIGN KEY ([ssm_idt]) REFERENCES [dbo].[tb_ssm_solicitacao_saida_material] ([ssm_idt]),
    CONSTRAINT [fk_soc_tci] FOREIGN KEY ([tci_idt]) REFERENCES [dbo].[tb_tci_tipo_ciencia] ([tci_idt]),
    CONSTRAINT [fk_soc_ssma] FOREIGN KEY ([ssma_idt]) REFERENCES [dbo].[tb_ssma_solicitacao_saida_material_acao] ([ssma_idt])
);

