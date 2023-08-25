CREATE TABLE [dbo].[tb_tpl_tipo_log] (
    [tpl_idt] INT NOT NULL,
    [tpl_nom] VARCHAR (50) NOT NULL,
    [tpl_ativo] BIT NOT NULL CONSTRAINT [df_tpl_ativo] DEFAULT (1),
    [tpl_dat_criacao] DATETIME NOT NULL CONSTRAINT [df_tpl_dat_criacao] DEFAULT getdate(),
    [tpl_dat_atualizacao] DATETIME NOT NULL CONSTRAINT [df_tpl_dat_atualizacao] DEFAULT getdate(),

    CONSTRAINT [pk_tpl] PRIMARY KEY CLUSTERED ([tpl_idt] ASC)
);
