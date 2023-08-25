CREATE TABLE [dbo].[tb_tdnf_tipo_documento_nota_fiscal](
	[tdnf_idt] [int] IDENTITY(1,1) NOT NULL,
	[tdnf_des] [varchar](64),
	[tdnf_flg_ativo] [bit] NOT NULL DEFAULT (1),
	[tdnf_dat_inclusao] [datetime] NOT NULL DEFAULT GETDATE(),
	[tdnf_dat_alteracao] [datetime] NOT NULL DEFAULT GETDATE(),
 CONSTRAINT [pk_tdnf_idt] PRIMARY KEY CLUSTERED 
(
	[tdnf_idt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [uk_tdnf_1] UNIQUE NONCLUSTERED 
(
	[tdnf_des] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]