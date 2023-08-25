CREATE TABLE [dbo].[tb_aus_assinatura_usuario]
(
    [aus_guid_ad] UNIQUEIDENTIFIER NOT NULL,
    [bin_idt]   BIGINT NOT NULL,
    [aus_dat_criacao]   DATETIME CONSTRAINT [df_aus_dat_criacao] DEFAULT getdate() NOT NULL,
    [aus_dat_atualizacao] DATETIME NOT NULL CONSTRAINT [df_aus_dat_atualizacao]  DEFAULT getdate(),
    [bin_idt_assinaturadocumento]    BIGINT           NULL,
    CONSTRAINT [pk_aus] PRIMARY KEY CLUSTERED ([aus_guid_ad] ASC),
    CONSTRAINT [fk_aus_bin] FOREIGN KEY ([bin_idt]) references [dbo].[tb_bin_binario] ([bin_idt]),
    CONSTRAINT [fk_aus_bin_assinaturadocumento] FOREIGN KEY ([bin_idt_assinaturadocumento]) REFERENCES [dbo].[tb_bin_binario] ([bin_idt]),
    CONSTRAINT [uk_aus_1] UNIQUE ([bin_idt])
)
