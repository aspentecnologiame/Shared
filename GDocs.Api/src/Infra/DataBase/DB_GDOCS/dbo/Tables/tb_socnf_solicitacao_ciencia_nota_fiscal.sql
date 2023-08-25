CREATE TABLE [dbo].[tb_socnf_solicitacao_ciencia_nota_fiscal](
	[socnf_idt] [int] IDENTITY(1,1) NOT NULL,
	[smnf_idt] [int] NOT NULL,
	[tcinf_idt] [int] NOT NULL,
	[scinf_idt] [int] NOT NULL,
	[smnfa_idt] [int] NOT NULL,
	[socnf_flg_ativo] [bit] NOT NULL DEFAULT (1),
	[socnf_usu_guid_ad] [uniqueidentifier] NOT NULL,
	[socnf_des_observacao] [varchar](200) NULL,
	[socnf_dat_prorrogacao] [datetime] NULL,
	[socnf_dat_criacao] [datetime] NOT NULL DEFAULT GETDATE(),
	[socnf_dat_atualizacao] [datetime] NULL DEFAULT GETDATE(),
 CONSTRAINT [pk_socnf] PRIMARY KEY CLUSTERED 
(
	[socnf_idt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY], 
    CONSTRAINT [fk_socnf_smnf] FOREIGN KEY ([smnf_idt]) REFERENCES [tb_smnf_saida_material_nota_fiscal]([smnf_idt]), 
    CONSTRAINT [fk_socnf_smnfa] FOREIGN KEY ([smnfa_idt]) REFERENCES [tb_smnfa_saida_material_nota_fiscal_acao]([smnfa_idt]), 
    CONSTRAINT [fk_socnf_tcinf] FOREIGN KEY ([tcinf_idt]) REFERENCES [tb_tcinf_tipo_ciencia_nota_fiscal]([tcinf_idt])
) ON [PRIMARY]
