CREATE TABLE [dbo].[tb_tfu_tipo_funcionalidade]
(
	[tfu_idt] INT NOT NULL CONSTRAINT [pk_tfu] PRIMARY KEY,
	[tfu_des] VARCHAR(50) NOT NULL,
	[tfu_flg_ativo] BIT CONSTRAINT [df_tfu_flg_ativo] DEFAULT(1) NOT NULL,
	[tfu_dat_inclusao] DATETIME CONSTRAINT df_tfu_dat_inclusao DEFAULT(GETDATE()) NOT NULL,
	[tfu_dat_alteracao] DATETIME CONSTRAINT df_tfu_dat_alteracao DEFAULT(GETDATE()) NOT NULL,
);
