CREATE TABLE [dbo].[tb_tnnf_tipo_natureza_nota_fiscal](
	[tnnf_idt] [int] IDENTITY(1,1) NOT NULL,
	[tnnf_des] [varchar](50) NOT NULL,
	[tnnf_flg_ativo] [bit] NOT NULL DEFAULT (1),
	[tnnf_dat_inclusao] [datetime] NOT NULL DEFAULT GETDATE(),
	[tnnf_dat_alteracao] [datetime] NOT NULL DEFAULT GETDATE(),
 CONSTRAINT [pk_tnnf_idt] PRIMARY KEY CLUSTERED 
(
	[tnnf_idt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [uk_tnopnf_1] UNIQUE NONCLUSTERED 
(
	[tnnf_des] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]