CREATE TABLE [dbo].[tb_usp_usuario_perfil]
(
    [usp_idt] INT IDENTITY(1,1) NOT NULL,
    [usp_guid_ad] UNIQUEIDENTIFIER NOT NULL,
    [per_idt] INT NOT NULL,
    [usp_flg_ativo] bit CONSTRAINT df_usp_flg_ativo DEFAULT(1) NOT NULL,
    [usp_dat_inclusao] DATETIME CONSTRAINT df_usp_dat_inclusao DEFAULT(GETDATE()) NOT NULL,
    [usp_dat_alteracao] DATETIME CONSTRAINT df_usp_dat_alteracao DEFAULT(GETDATE()) NOT NULL,
    [usp_nome_ad] varchar(100) NULL,
    [usp_email_ad]  varchar(100) NULL,

    CONSTRAINT [pk_usp] PRIMARY KEY CLUSTERED ([usp_idt] ASC),
    CONSTRAINT [fk_usp_per] FOREIGN KEY ([per_idt]) references [dbo].[tb_per_perfil] ([per_idt]),
    CONSTRAINT [uk_usp_1] UNIQUE ([usp_guid_ad],[per_idt])
);
