CREATE TABLE [dbo].[tb_ntu_notificacao_usuario](
	[ntu_idt] [int] IDENTITY(1,1) NOT NULL,
	[rel_idt] [int] NULL,
	[ntu_id_usuario] [uniqueidentifier] NOT NULL,
	[ntu_lido] [bit] DEFAULT 0 NOT NULL,
	[ntu_ativo] [bit] DEFAULT 1 NOT NULL,
	[ntu_dat_leitura] [datetime] NULL,
	[ntu_dat_criacao] [datetime] DEFAULT GETDATE() NOT NULL,
	[ntu_dat_atualizacao] [datetime] DEFAULT GETDATE() NOT NULL,
 CONSTRAINT [PK_tb_ntu_notificacao_usuario] PRIMARY KEY CLUSTERED 
(
	[ntu_idt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY], 
    CONSTRAINT [FK_ntu_rel_idt] FOREIGN KEY ([rel_idt]) REFERENCES [tb_rel_relatorio]([rel_idt])
) ON [PRIMARY]
GO