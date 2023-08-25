CREATE TABLE [dbo].[tb_scit_solicitacao_ciencia_item] (
    [scit_idt]             INT      IDENTITY (1, 1) NOT NULL,
    [soc_idt]              INT      NOT NULL,
    [ssmi_idt]             INT      NOT NULL,
    [scit_flg_ativo]       BIT      CONSTRAINT [df_scit_flg_ativo] DEFAULT ((1)) NOT NULL,
    [scit_dat_criacao]     DATETIME CONSTRAINT [df_scit_dat_criacao] DEFAULT (getdate()) NOT NULL,
    [scit_dat_atualizacao] DATETIME CONSTRAINT [df_scit_dat_atualizacao] DEFAULT (getdate()) NULL,
    CONSTRAINT [pk_scit] PRIMARY KEY CLUSTERED ([scit_idt] ASC),
    CONSTRAINT [fk_scit_soc] FOREIGN KEY ([soc_idt]) REFERENCES [dbo].[tb_soc_solicitacao_ciencia] ([soc_idt]),
    CONSTRAINT [fk_scit_ssmi] FOREIGN KEY ([ssmi_idt]) REFERENCES [dbo].[tb_ssmi_solicitacao_saida_material_item] ([ssmi_idt])
);

