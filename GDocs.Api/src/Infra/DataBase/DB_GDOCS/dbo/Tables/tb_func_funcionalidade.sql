CREATE TABLE [dbo].[tb_func_funcionalidade] (
	[func_idt] INT NOT NULL CONSTRAINT [pk_func] PRIMARY KEY,
	[func_chave] [varchar](255) NOT NULL,
	[func_texto] [varchar](100) NOT NULL,
	[func_desc] [varchar](1000) NULL,
	[func_icone] [varchar](50) NULL,
	[func_rota] [varchar](150) NULL,	
	[tfu_idt] [int] NULL,
	[func_idt_pai] INT NULL,
	[func_ordem] INT NULL,
	[func_flg_ativo] BIT CONSTRAINT [df_func_flg_ativo] DEFAULT(1) NOT NULL,
	[func_dat_inclusao] DATETIME CONSTRAINT df_func_dat_inclusao DEFAULT(GETDATE()) NOT NULL,
	[func_dat_alteracao] DATETIME CONSTRAINT df_func_dat_alteracao DEFAULT(GETDATE()) NOT NULL,

	CONSTRAINT [fk_func_tfu] FOREIGN KEY ([tfu_idt]) REFERENCES [dbo].[tb_tfu_tipo_funcionalidade] ([tfu_idt]),
	CONSTRAINT [fk_func_func] FOREIGN KEY ([func_idt_pai]) REFERENCES [dbo].[tb_func_funcionalidade] ([func_idt])
);
