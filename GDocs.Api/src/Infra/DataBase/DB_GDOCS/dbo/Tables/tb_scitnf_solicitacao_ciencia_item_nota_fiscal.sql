CREATE TABLE [dbo].[tb_scitnf_solicitacao_ciencia_item_nota_fiscal](
	[scitnf_idt] [int] IDENTITY(1,1) NOT NULL,
	[socnf_idt] [int] NOT NULL,
	[smnfi_idt] [int] NOT NULL,
	[scitnf_flg_ativo] [bit] NOT NULL DEFAULT (1),
	[scitnf_dat_criacao] [datetime] NOT NULL DEFAULT GETDATE(),
	[scitnf_dat_atualizacao] [datetime] NULL DEFAULT GETDATE(),
 CONSTRAINT [pk_scitnf] PRIMARY KEY CLUSTERED 
(
	[scitnf_idt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY], 
    CONSTRAINT [fk_scitnf_socnf] FOREIGN KEY ([socnf_idt]) REFERENCES [tb_socnf_solicitacao_ciencia_nota_fiscal]([socnf_idt]), 
    CONSTRAINT [fk_scitnf_smnfi] FOREIGN KEY ([smnfi_idt]) REFERENCES [tb_smnfi_saida_material_nota_fiscal_item]([smnfi_idt])
) ON [PRIMARY]
