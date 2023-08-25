CREATE TABLE [dbo].[tb_log_log] (
	[log_idt] BIGINT NOT NULL IDENTITY (1, 1),
	[tpl_idt] INT NOT NULL,
	[log_metadados] XML NULL,
	[log_dados]	 VARCHAR(MAX) NOT NULL,
	[log_dat_criacao] DATETIME NOT NULL CONSTRAINT [df_log_dat_criacao] DEFAULT (getdate()),	
	CONSTRAINT [pk_log] PRIMARY KEY CLUSTERED ([log_idt] ASC),
	CONSTRAINT [fk_log_tpl] FOREIGN KEY ([tpl_idt]) REFERENCES [dbo].[tb_tpl_tipo_log] ([tpl_idt])	
)
GO

CREATE NONCLUSTERED INDEX [ix_log_01]
	ON [dbo].[tb_log_log]([tpl_idt] ASC);
GO

CREATE PRIMARY XML INDEX [ix_log_02]
	ON [dbo].[tb_log_log]([log_metadados]);
GO

CREATE NONCLUSTERED INDEX [ix_log_03]
	ON [dbo].[tb_log_log]([log_dat_criacao] DESC, [tpl_idt] ASC);
GO