CREATE TABLE [dbo].[tb_rel_relatorio](
	[rel_idt] [int] IDENTITY(1,1) NOT NULL,
	[rel_nome] [varchar](64) NOT NULL,
	[rel_url] [varchar](512) NOT NULL,
	[rel_parametros] [varchar](256) NULL,
	[rel_ativo] [bit] DEFAULT 1 NOT NULL,
	[rel_dat_criacao] [datetime] DEFAULT GETDATE() NOT NULL,
	[rel_dat_atualizacao] [datetime] DEFAULT GETDATE() NOT NULL,
 CONSTRAINT [PK_tb_rel_relatorio] PRIMARY KEY CLUSTERED 
(
	[rel_idt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
