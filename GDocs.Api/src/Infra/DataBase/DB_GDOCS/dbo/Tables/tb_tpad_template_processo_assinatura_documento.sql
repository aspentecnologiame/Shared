CREATE TABLE [dbo].[tb_tpad_template_processo_assinatura_documento]
(
    [tpad_idt] INT NOT NULL,
    [tpad_nome] VARCHAR(255) NOT NULL,
    [tpad_flg_ativo] BIT NOT NULL CONSTRAINT [df_tpad_flg_ativo]  DEFAULT (1),
	[tpad_dat_criacao] DATETIME NOT NULL CONSTRAINT [df_tpad_dat_criacao]  DEFAULT getdate(),
	[tpad_dat_atualizacao] DATETIME NOT NULL CONSTRAINT [df_tpad_dat_atualizacao]  DEFAULT getdate(),

    CONSTRAINT [pk_tpad] PRIMARY KEY CLUSTERED ([tpad_idt] ASC)    
)
