CREATE TABLE [dbo].[tb_sdo_status_documento]
(
    [sdo_idt] INT NOT NULL,
    [sdo_des] VARCHAR(50) NOT NULL,

    [sdo_flg_ativo] BIT NOT NULL CONSTRAINT [df_sdo_flg_ativo]  DEFAULT (1),
	[sdo_dat_criacao] DATETIME NOT NULL CONSTRAINT [df_sdo_dat_criacao]  DEFAULT getdate(),
	[sdo_dat_atualizacao] DATETIME NOT NULL CONSTRAINT [df_sdo_dat_atualizacao]  DEFAULT (getdate())

    CONSTRAINT [pk_sdo] PRIMARY KEY CLUSTERED ([sdo_idt] ASC),
    CONSTRAINT [uk_sdo_des] UNIQUE ([sdo_des])
)
