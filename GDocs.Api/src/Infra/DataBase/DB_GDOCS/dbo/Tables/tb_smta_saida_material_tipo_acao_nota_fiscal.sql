CREATE TABLE [dbo].[tb_smtanf_saida_material_tipo_acao_nota_fiscal](
	[smtanf_idt] [int] NOT NULL,
	[smtanf_des] [varchar](50) NOT NULL,
	[smtanf_flg_ativo] [bit] NOT NULL,
	[smtanf_dat_criacao] [datetime] NOT NULL,
	[smtanf_dat_atualizacao] [datetime] NOT NULL,
	[tcinf_idt] [int] NULL,
 CONSTRAINT [pk_smtanf] PRIMARY KEY CLUSTERED 
(
	[smtanf_idt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [uk_smtanf_des] UNIQUE NONCLUSTERED 
(
	[smtanf_des] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY], 
    CONSTRAINT [fk_smtanf_tcinf] FOREIGN KEY ([tcinf_idt]) REFERENCES [tb_tcinf_tipo_ciencia_nota_fiscal]([tcinf_idt])
) ON [PRIMARY]
