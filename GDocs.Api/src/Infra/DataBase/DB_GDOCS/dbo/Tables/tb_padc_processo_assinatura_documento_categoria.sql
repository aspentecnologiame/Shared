CREATE TABLE [dbo].[tb_padc_processo_assinatura_documento_categoria]
(
    [padc_idt] INT NOT NULL,
    [padc_descricao] VARCHAR(50) NOT NULL,

    [padc_flg_ativo] BIT NOT NULL CONSTRAINT [df_padc_flg_ativo]  DEFAULT (1),
	[padc_dat_criacao] DATETIME NOT NULL CONSTRAINT [df_padc_dat_criacao]  DEFAULT getdate(),
	[padc_dat_atualizacao] DATETIME NOT NULL CONSTRAINT [df_padc_dat_atualizacao]  DEFAULT getdate(),

    CONSTRAINT [pk_padc] PRIMARY KEY CLUSTERED ([padc_idt] ASC),
    CONSTRAINT [uk_padc_descricao] UNIQUE ([padc_descricao])
)
