CREATE TABLE [dbo].[tb_ssma_solicitacao_saida_material_acao] (
    [ssma_idt]               INT           IDENTITY (1, 1) NOT NULL,
    [ssm_idt]                INT           NOT NULL,
    [smta_idt]               INT           NOT NULL,
    [ssma_nom_conferente]    VARCHAR (50)  NULL,
    [ssma_dat_acao]          DATETIME      NULL,
    [ssma_nom_portador]      VARCHAR (50)  NULL,
    [ssma_des_setor_empresa] VARCHAR (50)  NULL,
    [ssma_des_observacao]    VARCHAR (200) NULL,
    [ssma_flg_ativo]         BIT           CONSTRAINT [df_ssma_flg_ativo] DEFAULT ((1)) NOT NULL,
    [ssma_dat_criacao]       DATETIME      CONSTRAINT [df_ssma_dat_criacao] DEFAULT (getdate()) NOT NULL,
    [ssma_dat_atualizacao]   DATETIME      CONSTRAINT [df_ssma_dat_atualizacao] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [pk_ssma] PRIMARY KEY CLUSTERED ([ssma_idt] ASC),
    CONSTRAINT [fk_ssma_smm] FOREIGN KEY ([ssm_idt]) REFERENCES [dbo].[tb_ssm_solicitacao_saida_material] ([ssm_idt]),
    CONSTRAINT [fk_ssma_smta] FOREIGN KEY ([smta_idt]) REFERENCES [dbo].[tb_smta_saida_material_tipo_acao] ([smta_idt])
);





