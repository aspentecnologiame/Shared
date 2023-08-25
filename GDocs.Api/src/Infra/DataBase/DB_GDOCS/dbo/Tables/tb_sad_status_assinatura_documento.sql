CREATE TABLE [dbo].[tb_sad_status_assinatura_documento]
(
    [sad_idt] INT NOT NULL,
    [sad_descricao] VARCHAR(50) NOT NULL,

    [sad_flg_ativo] BIT NOT NULL CONSTRAINT [df_sad_flg_ativo]  DEFAULT (1),
	[sad_dat_criacao] DATETIME NOT NULL CONSTRAINT [df_sad_dat_criacao]  DEFAULT getdate(),
	[sad_dat_atualizacao] DATETIME NOT NULL CONSTRAINT [df_sad_dat_atualizacao]  DEFAULT getdate(),

    CONSTRAINT [pk_sad] PRIMARY KEY CLUSTERED ([sad_idt] ASC),
    CONSTRAINT [uk_sad_descricao] UNIQUE ([sad_descricao])
)
