CREATE TABLE [dbo].[tb_smnfa_saida_material_nota_fiscal_acao](
	[smnfa_idt] [int] IDENTITY(1,1) NOT NULL,
	[smnf_idt] [int] NOT NULL,
	[smtanf_idt] [int] NOT NULL,
	[smnfa_nom_conferente] [varchar](50) NULL,
	[smnfa_dat_acao] [datetime] NULL,
	[smnfa_nom_portador] [varchar](50) NULL,
	[smnfa_des_setor_empresa] [varchar](50) NULL,
	[smnfa_des_observacao] [varchar](200) NULL,
	[smnfa_flg_ativo] [bit] NOT NULL DEFAULT (1),
	[smnfa_dat_criacao] [datetime] NOT NULL DEFAULT GETDATE(),
	[smnfa_dat_atualizacao] [datetime] NOT NULL DEFAULT GETDATE(),
 CONSTRAINT [pk_smnfa] PRIMARY KEY CLUSTERED 
(
	[smnfa_idt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY], 
    CONSTRAINT [fk_smnfa_smnf] FOREIGN KEY ([smnf_idt]) REFERENCES [tb_smnf_saida_material_nota_fiscal]([smnf_idt]), 
    CONSTRAINT [fk_smnfa_smtanf] FOREIGN KEY ([smtanf_idt]) REFERENCES [tb_smtanf_saida_material_tipo_acao_nota_fiscal]([smtanf_idt])
) ON [PRIMARY]
