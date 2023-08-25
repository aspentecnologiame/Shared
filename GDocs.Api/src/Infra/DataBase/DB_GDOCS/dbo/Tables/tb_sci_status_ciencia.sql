CREATE TABLE [dbo].[tb_sci_status_ciencia] (
    [sci_idt]           INT          NOT NULL,
    [sci_des]           VARCHAR (50) NOT NULL,
    [sci_flg_ativo]     BIT          CONSTRAINT [df_sci_flg_ativo] DEFAULT ((1)) NOT NULL,
    [sci_dat_inclusao]  DATETIME     CONSTRAINT [df_sci_dat_inclusao] DEFAULT (getdate()) NOT NULL,
    [sci_dat_alteracao] DATETIME     CONSTRAINT [df_sci_dat_alteracao] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [pk_sci] PRIMARY KEY CLUSTERED ([sci_idt] ASC),
    CONSTRAINT [uk_sci_1] UNIQUE NONCLUSTERED ([sci_des] ASC)
);

