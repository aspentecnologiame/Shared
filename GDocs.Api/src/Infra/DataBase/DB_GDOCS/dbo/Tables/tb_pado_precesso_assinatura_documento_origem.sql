CREATE TABLE [dbo].[tb_pado_processo_assinatura_documento_origem]
(    
    [pad_idt] INT NOT NULL,
    [pado_origem_idt] VARCHAR(36) NOT NULL,
    [pado_origem_nome] VARCHAR(36) NOT NULL,
    [pado_flg_ativo] BIT NOT NULL CONSTRAINT [df_pado_flg_ativo]  DEFAULT (1),
	[pado_dat_criacao] DATETIME NOT NULL CONSTRAINT [df_pado_dat_criacao]  DEFAULT getdate(),
	[pado_dat_atualizacao] DATETIME NOT NULL CONSTRAINT [df_pado_dat_atualizacao]  DEFAULT getdate(),

    CONSTRAINT [pk_pado] PRIMARY KEY CLUSTERED ([pad_idt] ASC),
    CONSTRAINT [fk_pado_pad] FOREIGN KEY ([pad_idt]) references [dbo].[tb_pad_processo_assinatura_documento] ([pad_idt])
)
