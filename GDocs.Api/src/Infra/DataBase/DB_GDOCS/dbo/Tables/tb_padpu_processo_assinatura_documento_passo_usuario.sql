CREATE TABLE [dbo].[tb_padpu_processo_assinatura_documento_passo_usuario] (
    [padpu_idt]                             BIGINT           IDENTITY (1, 1) NOT NULL,
    [padp_idt]                              INT              NOT NULL,
    [padpu_guid_ad]                         UNIQUEIDENTIFIER NOT NULL,
    [padpu_justificativa]                   VARCHAR (MAX)    NULL,
    [sadpu_idt]                             INT              NOT NULL,
    [padpu_flg_notificar_finalizacao]       BIT              CONSTRAINT [df_padpu_flg_notificar_finalizacao] DEFAULT ((0)) NOT NULL,
    [padpu_dat_assinatura]                  DATETIME         NULL,
    [padpu_guid_ad_representante]           UNIQUEIDENTIFIER NULL,
    [padpu_flg_ativo]                       BIT              CONSTRAINT [df_padpu_flg_ativo] DEFAULT ((1)) NOT NULL,
    [padpu_dat_criacao]                     DATETIME         CONSTRAINT [df_padpu_dat_criacao] DEFAULT (getdate()) NOT NULL,
    [padpu_dat_atualizacao]                 DATETIME         CONSTRAINT [df_padpu_dat_atualizacao] DEFAULT (getdate()) NOT NULL,
    [padpu_notificar_finalizacao_fluxo]     VARCHAR (255)    CONSTRAINT [df_padpu_notificar_finalizacao_fluxo] DEFAULT ('Padrao') NOT NULL,
    [padpu_flg_assinar_certificado_digital] BIT              CONSTRAINT [df_padpu_flg_assinar_certificado_digital] DEFAULT ((0)) NOT NULL,
    [padpu_flg_assinar_fisicamente]         BIT              CONSTRAINT [df_padpu_flg_assinar_fisicamente] DEFAULT ((0)) NOT NULL,
    [padpu_flg_farei_envio] BIT NOT NULL DEFAULT ((0)),
    CONSTRAINT [pk_padpu] PRIMARY KEY CLUSTERED ([padpu_idt] ASC),
    CONSTRAINT [fk_padpu_padp] FOREIGN KEY ([padp_idt]) REFERENCES [dbo].[tb_padp_processo_assinatura_documento_passo] ([padp_idt]),
    CONSTRAINT [fk_padpu_sadpu] FOREIGN KEY ([sadpu_idt]) REFERENCES [dbo].[tb_sadpu_status_assinatura_documento_passo_usuario] ([sadpu_idt]),
    CONSTRAINT [uk_padpu_1] UNIQUE NONCLUSTERED ([padp_idt] ASC, [padpu_guid_ad] ASC)
);


