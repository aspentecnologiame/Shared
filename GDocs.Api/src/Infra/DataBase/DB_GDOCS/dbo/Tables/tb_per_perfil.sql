CREATE TABLE [dbo].[tb_per_perfil] (
    [per_idt] INT NOT NULL,
    [per_des] varchar(50) NOT NULL,
    [per_flg_ativo] BIT CONSTRAINT df_per_flg_ativo DEFAULT(1) NOT NULL,
    [per_dat_inclusao] DATETIME CONSTRAINT df_per_dat_inclusao DEFAULT(GETDATE()) NOT NULL,
    [per_dat_alteracao] DATETIME CONSTRAINT df_per_dat_alteracao DEFAULT(GETDATE()) NOT NULL,

    [per_peso] NCHAR(10) NULL, 
    CONSTRAINT [pk_per] PRIMARY KEY ([per_idt]),
    CONSTRAINT [uk_per_1] UNIQUE ([per_des])
);
