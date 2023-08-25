CREATE TABLE [dbo].[tb_csmnf_cancelar_saida_material_nota_fiscal]
(
	[csmnf_idt] [int] IDENTITY(1,1) NOT NULL,
	[smnf_idt] [int] NOT NULL, 
	[csmnf_motivo] varchar(256) NOT NULL,
	[csmnf_dat_criacao] [datetime] NULL DEFAULT GETDATE(), 
    CONSTRAINT [pk_csmnf] PRIMARY KEY ([csmnf_idt]), 
    CONSTRAINT [fk_csmnf_smnf] FOREIGN KEY ([smnf_idt]) REFERENCES [tb_smnf_saida_material_nota_fiscal]([smnf_idt])
)