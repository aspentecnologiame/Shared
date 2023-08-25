CREATE TABLE [dbo].[tb_pad_processo_assinatura_documento]
(
    [pad_idt] INT NOT NULL IDENTITY (1, 1),
    [pad_titulo] VARCHAR(255) NOT NULL,
    [pad_descricao] VARCHAR(MAX) NOT NULL,
    [pad_nome_documento] VARCHAR(255) NOT NULL,
    [pad_numero_documento] INT NULL,
    [pad_flg_destaque] BIT NOT NULL CONSTRAINT [df_pad_flg_destaque]  DEFAULT (0),
    [pad_guid_ad] UNIQUEIDENTIFIER NOT NULL,
    [sad_idt] INT NOT NULL,
    [pad_flg_ativo] BIT NOT NULL CONSTRAINT [df_pad_flg_ativo] DEFAULT (1),
	[pad_dat_criacao] DATETIME NOT NULL CONSTRAINT [df_pad_dat_criacao] DEFAULT getdate(),
	[pad_dat_atualizacao] DATETIME NOT NULL CONSTRAINT [df_pad_dat_atualizacao] DEFAULT getdate(),
    [padc_idt] INT NOT NULL, 
    [pad_chave_validacao] UNIQUEIDENTIFIER NULL,
    [pad_flg_gerar_numero_documento] BIT NOT NULL CONSTRAINT [df_pad_flg_gerar_numero] DEFAULT (0),
    [pad_flg_certificado_digital] BIT NOT NULL CONSTRAINT [df_pad_flg_certificado_digital] DEFAULT (0), 
    [pad_flg_assinado_fisicamente] BIT NOT NULL CONSTRAINT [df_pad_flg_assinado_fisicamente] DEFAULT (0), 
    CONSTRAINT [pk_pad] PRIMARY KEY CLUSTERED ([pad_idt] ASC),
    CONSTRAINT [fk_pad_sad] FOREIGN KEY ([sad_idt]) references [dbo].[tb_sad_status_assinatura_documento] ([sad_idt]),
    CONSTRAINT [fk_pad_padc] FOREIGN KEY ([padc_idt]) references [dbo].[tb_padc_processo_assinatura_documento_categoria] ([padc_idt])
)
