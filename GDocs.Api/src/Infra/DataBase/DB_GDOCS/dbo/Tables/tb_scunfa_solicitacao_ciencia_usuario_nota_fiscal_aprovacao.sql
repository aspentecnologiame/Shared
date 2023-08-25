CREATE TABLE [dbo].[tb_scunfa_solicitacao_ciencia_usuario_nota_fiscal_aprovacao](
	[scunfa_idt] [int] IDENTITY(1,1) NOT NULL,
	[socnf_idt] [int] NOT NULL,
	[scunfa_usu_guid_ad] [uniqueidentifier] NOT NULL,
	[scunfa_dat_aprovacao] [datetime] NULL,
	[scunfa_des_observacao] [varchar](200) NULL,
	[scunfa_flg_ativo] [bit] NOT NULL DEFAULT (1),
	[scunfa_dat_criacao] [datetime] NOT NULL DEFAULT GETDATE(),
	[scunfa_dat_atualizacao] [datetime] NULL DEFAULT GETDATE(),
	[scunfa_flg_rejeitado] [bit] NULL,
 CONSTRAINT [pk_scunfa] PRIMARY KEY CLUSTERED 
(
	[scunfa_idt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [uk_scunfa_1] UNIQUE NONCLUSTERED 
(
	[socnf_idt] ASC,
	[scunfa_usu_guid_ad] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY], 
    CONSTRAINT [fk_scunfa_socnf] FOREIGN KEY ([socnf_idt]) REFERENCES [tb_socnf_solicitacao_ciencia_nota_fiscal]([socnf_idt])
) ON [PRIMARY]
