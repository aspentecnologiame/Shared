CREATE TABLE [dbo].[tb_stsmnf_status_saida_material_nota_fiscal](
	[stsmnf_idt] [int] NOT NULL,
	[stsmnf_des] [varchar](50) NOT NULL,
	[stsmnf_flg_ativo] [bit] NOT NULL DEFAULT (1),
	[stsmnf_dat_criacao] [datetime] NOT NULL DEFAULT GETDATE(),
	[stsmnf_dat_atualizacao] [datetime] NOT NULL DEFAULT GETDATE(),
 CONSTRAINT [pk_stsmnf] PRIMARY KEY CLUSTERED 
(
	[stsmnf_idt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [uk_stsmnf_des] UNIQUE NONCLUSTERED 
(
	[stsmnf_des] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]