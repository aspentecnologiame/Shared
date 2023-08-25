CREATE TABLE [dbo].[tb_smnf_saida_material_nota_fiscal](
	[smnf_idt] [int] IDENTITY(1,1) NOT NULL,
	[smnf_num] [int] NULL,
	[tnnf_idt] [int] NOT NULL,
	[mfnf_idt] [int] NOT NULL,
	[stsmnf_idt] [int] NOT NULL,
	[smnf_flg_retorno] [bit] NOT NULL,
	[smnf_guid_ad_autor] [uniqueidentifier] NOT NULL,
	[smnf_setor] [varchar](4) NOT NULL,
	[smnf_origem] [varchar](50) NOT NULL,
	[smnf_transportador] [varchar](256) NOT NULL,
	[smnf_dat_saida] [smalldatetime] NOT NULL,
	[smnf_destino] [varchar](60) NOT NULL,
	[smnf_documento] [varchar](19) NULL,
	[smnf_codigo_totvs] [varchar](32) NULL,
	[smnf_motivo] [varchar](255) NULL,
	[smnf_volume] [varchar](255) NOT NULL,
	[smnf_peso] [varchar](255) NULL,
	[smnf_dat_retorno] [smalldatetime] NULL,
	[smnf_dat_criacao] [datetime] NOT NULL DEFAULT GETDATE(),
	[smnf_dat_atualizacao] [datetime] NULL DEFAULT GETDATE(),
	[smnf_flg_ativo] [bit] NOT NULL DEFAULT (1),
	[smnf_ret_parcial] BIT NULL, 
    CONSTRAINT [pk_smnf] PRIMARY KEY CLUSTERED 
(
	[smnf_idt] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY], 
    CONSTRAINT [fk_mfnf_idt] FOREIGN KEY ([mfnf_idt]) REFERENCES [tb_mfnf_modalidade_frete_nota_fiscal]([mfnf_idt]), 
    CONSTRAINT [fk_tnnf_idt] FOREIGN KEY ([tnnf_idt]) REFERENCES [tb_tnnf_tipo_natureza_nota_fiscal]([tnnf_idt]), 
    CONSTRAINT [fk_stsmnf_idt] FOREIGN KEY ([stsmnf_idt]) REFERENCES [tb_stsmnf_status_saida_material_nota_fiscal]([stsmnf_idt])
) ON [PRIMARY]