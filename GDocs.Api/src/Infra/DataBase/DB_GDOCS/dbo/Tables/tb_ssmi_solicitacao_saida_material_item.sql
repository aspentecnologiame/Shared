CREATE TABLE [dbo].[tb_ssmi_solicitacao_saida_material_item] (
    [ssm_idt]              INT          NOT NULL,
    [ssmi_idt]             INT          IDENTITY (1, 1) NOT NULL,
    [ssmi_qtd_item]        INT          NOT NULL,
    [ssmi_unidade]         VARCHAR (5)  NOT NULL,
    [ssm_num_patrimonio]   VARCHAR (50) NULL,
    [ssmi_descricao]       VARCHAR (150) NOT NULL,
    [ssmi_dat_criacao]     DATETIME     DEFAULT (getdate()) NOT NULL,
    [ssmi_flg_ativo]       BIT          DEFAULT ((1)) NOT NULL,
    [ssmi_dat_atualizacao] DATETIME     NULL,
    CONSTRAINT [pk_ssmi] PRIMARY KEY CLUSTERED ([ssmi_idt] ASC),
    CONSTRAINT [fk_ssm_ssmi] FOREIGN KEY ([ssm_idt]) REFERENCES [dbo].[tb_ssm_solicitacao_saida_material] ([ssm_idt])
);

