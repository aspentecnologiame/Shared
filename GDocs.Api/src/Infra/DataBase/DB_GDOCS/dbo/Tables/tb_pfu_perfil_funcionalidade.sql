CREATE TABLE [dbo].[tb_pfu_perfil_funcionalidade] (
	[pfu_idt] INT IDENTITY(1,1) NOT NULL,
	[per_idt] INT NOT NULL,
	[func_idt] INT NOT NULL,
	[pfu_flg_ativo] BIT CONSTRAINT [df_pfu_flg_ativo] DEFAULT(1) NOT NULL,
	[pfu_dat_inclusao] DATETIME CONSTRAINT df_pfu_dat_inclusao DEFAULT(GETDATE()) NOT NULL,
	[pfu_dat_alteracao] DATETIME CONSTRAINT df_pfu_dat_alteracao DEFAULT(GETDATE()) NOT NULL,

	CONSTRAINT [pk_pfu] PRIMARY KEY CLUSTERED ([pfu_idt] ASC),
	CONSTRAINT [fk_pfu_per] FOREIGN KEY ([per_idt]) REFERENCES [dbo].[tb_per_perfil] ([per_idt]),
	CONSTRAINT [fk_pfu_func] FOREIGN KEY ([func_idt]) REFERENCES [dbo].[tb_func_funcionalidade] ([func_idt]),
	CONSTRAINT [uk_pfu_1] UNIQUE ([per_idt],[func_idt])
);
