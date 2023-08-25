CREATE TABLE [dbo].[tb_tci_tipo_ciencia] (
    [tci_idt]           INT          NOT NULL,
    [tci_des]           VARCHAR (50) NOT NULL,
    [tci_flg_ativo]     BIT          CONSTRAINT [df_tci_flg_ativo] DEFAULT ((1)) NOT NULL,
    [tci_dat_inclusao]  DATETIME     CONSTRAINT [df_tci_dat_inclusao] DEFAULT (getdate()) NOT NULL,
    [tci_dat_alteracao] DATETIME     CONSTRAINT [df_tci_dat_alteracao] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [pk_tci] PRIMARY KEY CLUSTERED ([tci_idt] ASC),
    CONSTRAINT [uk_tci_1] UNIQUE NONCLUSTERED ([tci_des] ASC)
);

