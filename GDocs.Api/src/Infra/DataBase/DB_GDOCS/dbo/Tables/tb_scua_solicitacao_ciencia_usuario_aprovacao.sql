CREATE TABLE [dbo].[tb_scua_solicitacao_ciencia_usuario_aprovacao] (
    [scua_idt]             INT              IDENTITY (1, 1) NOT NULL,
    [soc_idt]              INT              NOT NULL,
    [scua_usu_guid_ad]     UNIQUEIDENTIFIER NOT NULL,
    [scua_dat_aprovacao]   DATETIME         NULL,
    [scua_des_observacao]  VARCHAR (200)    NULL,
    [scua_flg_ativo]       BIT              CONSTRAINT [df_scua_flg_ativo] DEFAULT ((1)) NOT NULL,
    [scua_dat_criacao]     DATETIME         CONSTRAINT [df_scua_dat_criacao] DEFAULT (getdate()) NOT NULL,
    [scua_dat_atualizacao] DATETIME         CONSTRAINT [df_scua_dat_atualizacao] DEFAULT (getdate()) NULL,
    [scua_flg_rejeitado]   BIT              NULL,
    CONSTRAINT [pk_scua] PRIMARY KEY CLUSTERED ([scua_idt] ASC),
    CONSTRAINT [fk_scua_soc] FOREIGN KEY ([soc_idt]) REFERENCES [dbo].[tb_soc_solicitacao_ciencia] ([soc_idt]),
    CONSTRAINT [uk_scua_1] UNIQUE NONCLUSTERED ([soc_idt] ASC, [scua_usu_guid_ad] ASC)
);



