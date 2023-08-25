CREATE TABLE [dbo].[tb_padp_processo_assinatura_documento_passo]
(
    [padp_idt] INT NOT NULL IDENTITY (1, 1),
    [pad_idt] INT NOT NULL,
    [padp_ordem] INT NOT NULL,
    [sadp_idt] INT NOT NULL,    
    [padp_flg_ativo] BIT NOT NULL CONSTRAINT [df_padp_flg_ativo]  DEFAULT (1),
	[padp_dat_criacao] DATETIME NOT NULL CONSTRAINT [df_padp_dat_criacao]  DEFAULT getdate(),
	[padp_dat_atualizacao] DATETIME NOT NULL CONSTRAINT [df_padp_dat_atualizacao]  DEFAULT getdate(),
    [padp_flg_aguardar_todos_usuarios] BIT NOT NULL CONSTRAINT [df_padp_flg_aguardar_todos_usuarios]  DEFAULT (0),
    CONSTRAINT [pk_padp] PRIMARY KEY CLUSTERED ([padp_idt] ASC),
    CONSTRAINT [fk_padp_sadp] FOREIGN KEY ([sadp_idt]) references [dbo].[tb_sadp_status_assinatura_documento_passo] ([sadp_idt]),
    CONSTRAINT [fk_padp_pad] FOREIGN KEY ([pad_idt]) references [dbo].[tb_pad_processo_assinatura_documento] ([pad_idt]),
    CONSTRAINT [uk_padp_1] UNIQUE ([pad_idt],[padp_ordem])
)
