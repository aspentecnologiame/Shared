CREATE TABLE [dbo].[tb_tpadp_template_processo_assinatura_documento_passo]
(
    [tpadp_idt] INT NOT NULL,
    [tpad_idt] INT NOT NULL,
    [tpadp_ordem] INT NOT NULL,    
    [tpadp_flg_ativo] BIT NOT NULL CONSTRAINT [df_tpadp_flg_ativo]  DEFAULT (1),
	[tpadp_dat_criacao] DATETIME NOT NULL CONSTRAINT [df_tpadp_dat_criacao]  DEFAULT getdate(),
	[tpadp_dat_atualizacao] DATETIME NOT NULL CONSTRAINT [df_tpadp_dat_atualizacao]  DEFAULT getdate(),
    [tpadp_flg_aguardar_todos_usuarios] BIT NOT NULL CONSTRAINT [df_tpadp_flg_aguardar_todos_usuarios]  DEFAULT (0),
    CONSTRAINT [pk_tpadp] PRIMARY KEY CLUSTERED ([tpadp_idt] ASC),    
    CONSTRAINT [fk_tpadp_tpad] FOREIGN KEY ([tpad_idt]) references [dbo].[tb_tpad_template_processo_assinatura_documento] ([tpad_idt]),
    CONSTRAINT [uk_tpadp_1] UNIQUE ([tpad_idt],[tpadp_ordem])
)
