CREATE TABLE [dbo].[tb_sadp_status_assinatura_documento_passo]
(
    [sadp_idt] INT NOT NULL,
    [sadp_descricao] VARCHAR(50) NOT NULL,

    [sadp_flg_ativo] BIT NOT NULL CONSTRAINT [df_sadp_flg_ativo]  DEFAULT (1),
	[sadp_dat_criacao] DATETIME NOT NULL CONSTRAINT [df_sadp_dat_criacao]  DEFAULT getdate(),
	[sadp_dat_atualizacao] DATETIME NOT NULL CONSTRAINT [df_sadp_dat_atualizacao]  DEFAULT getdate(),

    CONSTRAINT [pk_sadp] PRIMARY KEY CLUSTERED ([sadp_idt] ASC),
    CONSTRAINT [uk_sadp_descricao] UNIQUE ([sadp_descricao])
)
