CREATE TABLE [dbo].[tb_pada_processo_assinatura_documento_arquivo]
(
    [pada_idt] BIGINT NOT NULL IDENTITY (1, 1),
    [pad_idt] INT NOT NULL,
    [pada_ordem] INT NOT NULL,
    [bin_idt]   BIGINT NULL,
    [pada_flg_final] BIT NOT NULL CONSTRAINT [df_pada_flg_final] DEFAULT (0),
    [pada_flg_ativo] BIT NOT NULL CONSTRAINT [df_pada_flg_ativo]  DEFAULT (1),
	[pada_dat_criacao] DATETIME NOT NULL CONSTRAINT [df_pada_dat_criacao]  DEFAULT getdate(),
	[pada_dat_atualizacao] DATETIME NOT NULL CONSTRAINT [df_pada_dat_atualizacao]  DEFAULT getdate(),
    CONSTRAINT [pk_pada] PRIMARY KEY CLUSTERED ([pada_idt] ASC),
    CONSTRAINT [fk_pada_pad] FOREIGN KEY ([pad_idt]) references [dbo].[tb_pad_processo_assinatura_documento] ([pad_idt]),
    CONSTRAINT [fk_pada_bin] FOREIGN KEY ([bin_idt]) references [dbo].[tb_bin_binario] ([bin_idt])
)
