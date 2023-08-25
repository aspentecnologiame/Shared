CREATE TABLE [dbo].[tb_lre_log_requisicao] (
    [lre_idt] UNIQUEIDENTIFIER NOT NULL,	
	[lre_recurso] VARCHAR(500) NOT NULL,
	[lre_metadados] XML NOT NULL,
	[lre_xml_requisicao] XML NOT NULL,
	[lre_status] VARCHAR(50) NOT NULL,
	[lre_origem] VARCHAR(100) NOT NULL,
	[lre_dat_criacao] DATETIME NOT NULL CONSTRAINT [df_lre_dat_criacao] DEFAULT getdate(),

	CONSTRAINT [pk_lre] PRIMARY KEY CLUSTERED ([lre_idt] ASC)
);
