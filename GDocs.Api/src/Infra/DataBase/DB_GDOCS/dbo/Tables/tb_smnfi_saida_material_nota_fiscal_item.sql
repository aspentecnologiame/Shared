CREATE TABLE [dbo].[tb_smnfi_saida_material_nota_fiscal_item](
	[smnfi_idt] [int] IDENTITY(1,1) NOT NULL,
	[smnf_idt] [int] NOT NULL,
	[smnfi_qtd] [int] NOT NULL,
	[smnfi_unidade] [varchar](50) NOT NULL,
	[smnfi_des] [varchar](150) NOT NULL,
	[smnfi_valor_unitario] [decimal](9, 2) NOT NULL,
	[smnfi_codigo] [varchar](10) NULL,
	[smnfi_tag_servico] [varchar](10) NULL,
	[smnfi_patrimonio] [varchar](10) NULL,
	[smnfi_dat_criacao] [datetime] NOT NULL DEFAULT GETDATE(),
	[smnfi_flg_ativo] [bit] NOT NULL DEFAULT (1),
	[smnfi_dat_atualizacao] [datetime] NULL DEFAULT GETDATE(),
 CONSTRAINT [pk_smnfi] PRIMARY KEY CLUSTERED 
(
	[smnfi_idt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY], 
    CONSTRAINT [fk_smnf_smnfi] FOREIGN KEY ([smnf_idt]) REFERENCES [tb_smnf_saida_material_nota_fiscal]([smnf_idt])
) ON [PRIMARY]