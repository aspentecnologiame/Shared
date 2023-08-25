CREATE TABLE [dbo].[tb_smdnf_saida_material_documento_nota_fiscal](
	[smdnf_idt] [int] IDENTITY(1,1) NOT NULL,
	[smnf_idt] [int] NOT NULL,
	[tdnf_idt] [int] NOT NULL,
	[smnf_numero_documento] [int] NULL,
	[bin_idt] [bigint] NULL,
	[smnf_motivo] [varchar](255) NULL,
	[smdnf_guid_ad_autor] [uniqueidentifier] NOT NULL,
	[smdnf_flg_ativo] [bit] NOT NULL DEFAULT (1),
	[smdnf_dat_inclusao] [datetime] NOT NULL DEFAULT GETDATE(),
	[smdnf_dat_alteracao] [datetime] NOT NULL DEFAULT GETDATE(),
 CONSTRAINT [pk_smdnf_idt] PRIMARY KEY CLUSTERED 
(
	[smdnf_idt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY], 
    CONSTRAINT [fk_smnf_idt] FOREIGN KEY ([smnf_idt]) REFERENCES [tb_smnf_saida_material_nota_fiscal]([smnf_idt]), 
    CONSTRAINT [fk_tdnf_idt] FOREIGN KEY ([tdnf_idt]) REFERENCES [tb_tdnf_tipo_documento_nota_fiscal]([tdnf_idt]), 
    CONSTRAINT [fk_hspnf_bin_idt] FOREIGN KEY ([bin_idt]) REFERENCES [tb_bin_binario]([bin_idt])
) ON [PRIMARY]
