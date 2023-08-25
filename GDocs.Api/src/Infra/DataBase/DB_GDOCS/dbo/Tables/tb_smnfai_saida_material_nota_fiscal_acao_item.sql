CREATE TABLE [dbo].[tb_smnfai_saida_material_nota_fiscal_acao_item](
	[smnfai_idt] [int] IDENTITY(1,1) NOT NULL,
	[smnf_idt] [int] NOT NULL,
	[smnfi_idt] [int] NOT NULL,
	[smnfa_idt] [int] NOT NULL,
	[smnfai_flg_ativo] [bit] NOT NULL DEFAULT (1),
	[smnfai_dat_criacao] [datetime] NOT NULL DEFAULT GETDATE(),
	[smnfai_dat_atualizacao] [datetime] NOT NULL DEFAULT GETDATE(),
 CONSTRAINT [pk_smnfai] PRIMARY KEY CLUSTERED 
(
	[smnfai_idt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY], 
    CONSTRAINT [fk_smnfai_smnfi] FOREIGN KEY ([smnfi_idt]) REFERENCES [tb_smnfi_saida_material_Nota_fiscal_item]([smnfi_idt]), 
    CONSTRAINT [fk_smnfai_smnfa] FOREIGN KEY ([smnfa_idt]) REFERENCES [tb_smnfa_saida_material_nota_fiscal_acao]([smnfa_idt])
) ON [PRIMARY]
