CREATE TABLE [dbo].[tb_tcinf_tipo_ciencia_nota_fiscal](
	[tcinf_idt] [int] NOT NULL,
	[tcinf_des] [varchar](50) NOT NULL,
	[tcinf_flg_ativo] [bit] NOT NULL DEFAULT (1),
	[tcinf_dat_inclusao] [datetime] NOT NULL DEFAULT GETDATE(),
	[tcinf_dat_alteracao] [datetime] NOT NULL DEFAULT GETDATE(),
 CONSTRAINT [pk_tcinf] PRIMARY KEY CLUSTERED 
(
	[tcinf_idt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [uk_tcinf_1] UNIQUE NONCLUSTERED 
(
	[tcinf_des] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]