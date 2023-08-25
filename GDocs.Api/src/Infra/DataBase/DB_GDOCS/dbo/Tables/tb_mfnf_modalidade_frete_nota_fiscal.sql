CREATE TABLE [dbo].[tb_mfnf_modalidade_frete_nota_fiscal](
	[mfnf_idt] [int] IDENTITY(1,1) NOT NULL,
	[mfnf_des] [varchar](50) NOT NULL,
	[mfnf_flg_ativo] [bit] NOT NULL DEFAULT (1),
	[mfnf_dat_inclusao] [datetime] NOT NULL DEFAULT GETDATE(),
	[mfnf_dat_alteracao] [datetime] NOT NULL DEFAULT GETDATE(),
 CONSTRAINT [pk_mfnf_idt] PRIMARY KEY CLUSTERED 
(
	[mfnf_idt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [uk_mfnf_1] UNIQUE NONCLUSTERED 
(
	[mfnf_des] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
