CREATE TABLE [dbo].[tb_dfitci_tipo_ciencia_fi1548] (
    [dfitci_idt]           INT          NOT NULL,
    [dfitci_des]           VARCHAR (50) NOT NULL,
    [dfitci_flg_ativo]     BIT          CONSTRAINT [df_dfitci_flg_ativo] DEFAULT ((1)) NOT NULL,
    [dfitci_dat_inclusao]  DATETIME     CONSTRAINT [df_dfitci_dat_inclusao] DEFAULT (getdate()) NOT NULL,
    [dfitci_dat_alteracao] DATETIME     CONSTRAINT [df_dfitci_dat_alteracao] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [pk_dfitci] PRIMARY KEY CLUSTERED ([dfitci_idt] ASC),
    CONSTRAINT [uk_dfitci] UNIQUE NONCLUSTERED ([dfitci_des] ASC)
);
