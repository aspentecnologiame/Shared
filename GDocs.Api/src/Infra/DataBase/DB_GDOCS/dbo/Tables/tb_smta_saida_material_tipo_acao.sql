CREATE TABLE [dbo].[tb_smta_saida_material_tipo_acao] (
    [smta_idt]             INT          NOT NULL,
    [smta_des]             VARCHAR (50) NOT NULL,
    [smta_flg_ativo]       BIT          CONSTRAINT [df_smta_flg_ativo] DEFAULT ((1)) NOT NULL,
    [smta_dat_criacao]     DATETIME     CONSTRAINT [df_smta_dat_criacao] DEFAULT (getdate()) NOT NULL,
    [smta_dat_atualizacao] DATETIME     CONSTRAINT [df_smta_dat_atualizacao] DEFAULT (getdate()) NOT NULL,
    [tci_idt]              INT          NULL,
    CONSTRAINT [pk_smta] PRIMARY KEY CLUSTERED ([smta_idt] ASC),
    CONSTRAINT [fk_smta_tci] FOREIGN KEY ([tci_idt]) REFERENCES [dbo].[tb_tci_tipo_ciencia] ([tci_idt]),
    CONSTRAINT [uk_smta_des] UNIQUE NONCLUSTERED ([smta_des] ASC)
);



