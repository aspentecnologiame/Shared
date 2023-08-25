CREATE TABLE [dbo].[tb_fnf_fornecedor_nota_fiscal](
	[fnf_idt] [int] IDENTITY(1,1) NOT NULL,
	[smnf_idt] [int] NOT NULL,
	[fnf_endereco] [varchar](255) NOT NULL,
	[fnf_bairro] [varchar](64) NULL,
	[fnf_cep] [varchar](9) NOT NULL,
	[fnf_cidade] [varchar](64) NOT NULL,
	[fnf_estado] [varchar](2) NOT NULL,
	[socnf_flg_ativo] [bit] NOT NULL DEFAULT (1) ,
	[socnf_dat_criacao] [datetime] NOT NULL DEFAULT GETDATE(),
	[socnf_dat_atualizacao] [datetime] NULL DEFAULT GETDATE(),
 CONSTRAINT [pk_fnf] PRIMARY KEY CLUSTERED 
(
	[fnf_idt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
