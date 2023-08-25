CREATE TABLE [dbo].[tb_tpadpu_template_processo_assinatura_documento_passo_usuario]
(
    [tpadpu_idt] BIGINT NOT NULL,
    [tpadp_idt] INT NOT NULL,
    [tpadpu_guid_ad] UNIQUEIDENTIFIER NOT NULL,  
    [tpadpu_flg_notificar_finalizacao] BIT NOT NULL CONSTRAINT [df_tpadpu_flg_notificar_finalizacao] DEFAULT (0),
    [tpadpu_flg_ativo] BIT NOT NULL CONSTRAINT [df_tpadpu_flg_ativo]  DEFAULT (1),
	[tpadpu_dat_criacao] DATETIME NOT NULL CONSTRAINT [df_tpadpu_dat_criacao]  DEFAULT getdate(),
	[tpadpu_dat_atualizacao] DATETIME NOT NULL CONSTRAINT [df_tpadpu_dat_atualizacao]  DEFAULT getdate(),
    [tpadpu_notificar_finalizacao_fluxo] VARCHAR(255) NOT NULL CONSTRAINT [df_tpadpu_notificar_finalizacao_fluxo]  DEFAULT 'Padrao',
    CONSTRAINT [pk_tpadpu] PRIMARY KEY CLUSTERED ([tpadpu_idt] ASC),
    CONSTRAINT [fk_tpadpu_padp] FOREIGN KEY ([tpadp_idt]) references [dbo].[tb_tpadp_template_processo_assinatura_documento_passo] ([tpadp_idt]),    
    CONSTRAINT [uk_tpadpu_1] UNIQUE ([tpadp_idt],[tpadpu_guid_ad])
)
