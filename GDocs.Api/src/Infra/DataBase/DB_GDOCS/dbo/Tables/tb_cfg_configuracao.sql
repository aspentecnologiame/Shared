CREATE TABLE [dbo].[tb_cfg_configuracao] (
	[cfg_idt] INT IDENTITY(1,1) NOT NULL,
	[cfg_nom_key] VARCHAR(200) NOT NULL,
	[cfg_des_value] VARCHAR(max) NOT NULL,
	[cfg_des_descricao] VARCHAR(400) NULL,
	[cfg_flg_ativo] BIT NOT NULL CONSTRAINT [df_cfg_flg_ativo]  DEFAULT (1),
	[cfg_dat_criacao] DATETIME NOT NULL CONSTRAINT [df_cfg_dat_criacao]  DEFAULT getdate(),
	[cfg_dat_atualizacao] DATETIME NOT NULL CONSTRAINT [df_cfg_dat_atualizacao]  DEFAULT (getdate()),

	CONSTRAINT [pk_cfg] PRIMARY KEY CLUSTERED ([cfg_idt] ASC),
	CONSTRAINT [uk_cfg_nom_key] UNIQUE ([cfg_nom_key])
);