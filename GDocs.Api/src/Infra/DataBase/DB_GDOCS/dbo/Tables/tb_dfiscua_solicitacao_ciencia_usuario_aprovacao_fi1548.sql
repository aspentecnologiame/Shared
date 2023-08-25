CREATE TABLE [dbo].[tb_dfiscua_solicitacao_ciencia_usuario_aprovacao_fi1548] (
    [dfiscua_idt]             INT              IDENTITY (1, 1) NOT NULL,
    [dfisoc_idt]              INT              NOT NULL,
    [dfiscua_usu_guid_ad]     UNIQUEIDENTIFIER NOT NULL,
    [dfiscua_dat_aprovacao]   DATETIME         NULL,
    [dfiscua_des_observacao]  VARCHAR (200)    NULL,
    [dfiscua_flg_ativo]       BIT              CONSTRAINT [df_dfiscua_flg_ativo] DEFAULT ((1)) NOT NULL,
    [dfiscua_dat_criacao]     DATETIME         CONSTRAINT [df_dfiscua_dat_criacao] DEFAULT (getdate()) NOT NULL,
    [dfiscua_dat_atualizacao] DATETIME         CONSTRAINT [df_dfiscua_dat_atualizacao] DEFAULT (getdate()) NULL,
    [dfiscua_flg_rejeitado]   BIT              NULL,
    CONSTRAINT [pk_dfiscua] PRIMARY KEY CLUSTERED ([dfiscua_idt] ASC),
    CONSTRAINT [fk_dfiscua_dfisoc] FOREIGN KEY ([dfisoc_idt]) REFERENCES [dbo].[tb_dfisoc_solicitacao_ciencia_fi1548] ([dfisoc_idt]),
    CONSTRAINT [uk_dfiscua] UNIQUE NONCLUSTERED ([dfisoc_idt] ASC, [dfiscua_usu_guid_ad] ASC)
);
