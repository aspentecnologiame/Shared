CREATE TABLE [dbo].[tb_hspnf_historico_solicitacao_prorogacao_nota_fiscal](
	[hspnf_idt] [int] IDENTITY(1,1) NOT NULL,
	[smnf_idt] [int] NOT NULL,
	[smnfa_idt] [int] NOT NULL,
	[hspnf_dat_de] [datetime] NOT NULL,
	[hspnf_dat_para] [datetime] NULL,
	[hspnf_dat_criacao] [datetime] NULL DEFAULT GETDATE(),
 CONSTRAINT [pk_hspnf] PRIMARY KEY CLUSTERED 
(
	[hspnf_idt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY], 
    CONSTRAINT [fk_hspnf_smnfa] FOREIGN KEY ([smnfa_idt]) REFERENCES [tb_smnfa_saida_material_nota_fiscal_acao]([smnfa_idt])
) ON [PRIMARY]
