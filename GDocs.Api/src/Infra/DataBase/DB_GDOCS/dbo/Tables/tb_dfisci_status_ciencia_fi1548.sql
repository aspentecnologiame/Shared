CREATE TABLE [dbo].[tb_dfisci_status_ciencia_fi1548] (
    [dfisci_idt]           INT          NOT NULL,
    [dfisci_des]           VARCHAR (50) NOT NULL,
    [dfisci_flg_ativo]     BIT          CONSTRAINT [df_dfisci_flg_ativo] DEFAULT ((1)) NOT NULL,
    [dfisci_dat_inclusao]  DATETIME     CONSTRAINT [df_dfisci_dat_inclusao] DEFAULT (getdate()) NOT NULL,
    [dfisci_dat_alteracao] DATETIME     CONSTRAINT [df_dfisci_dat_alteracao] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [pk_dfisci] PRIMARY KEY CLUSTERED ([dfisci_idt] ASC),
    CONSTRAINT [uk_dfisci] UNIQUE NONCLUSTERED ([dfisci_des] ASC)
);
