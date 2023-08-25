CREATE TABLE [dbo].[tb_sadpu_status_assinatura_documento_passo_usuario]
(
    [sadpu_idt] INT NOT NULL,
    [sadpu_descricao] VARCHAR(50) NOT NULL,

    [sadpu_flg_ativo] BIT NOT NULL CONSTRAINT [df_sadpu_flg_ativo]  DEFAULT (1),
	[sadpu_dat_criacao] DATETIME NOT NULL CONSTRAINT [df_sadpu_dat_criacao]  DEFAULT getdate(),
	[sadpu_dat_atualizacao] DATETIME NOT NULL CONSTRAINT [df_sadpu_dat_atualizacao]  DEFAULT getdate(),

    CONSTRAINT [pk_sadpu] PRIMARY KEY CLUSTERED ([sadpu_idt] ASC),
    CONSTRAINT [uk_sadpu_descricao] UNIQUE ([sadpu_descricao])
)

