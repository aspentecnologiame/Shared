CREATE TABLE [dbo].[tb_scinf_status_ciencia_nota_fiscal](
	[scinf_idt] [int] NOT NULL,
	[scinf_des] [varchar](50) NOT NULL,
	[scinf_flg_ativo] [bit] NOT NULL DEFAULT (1),
	[scinf_dat_inclusao] [datetime] NOT NULL DEFAULT GETDATE(),
	[scinf_dat_alteracao] [datetime] NOT NULL DEFAULT GETDATE(),
 CONSTRAINT [pk_scinf] PRIMARY KEY CLUSTERED 
(
	[scinf_idt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [uk_scinf_1] UNIQUE NONCLUSTERED 
(
	[scinf_des] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
