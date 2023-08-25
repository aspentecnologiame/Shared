CREATE TABLE [dbo].[tb_stsm_status_saida_material] (
    [stsm_idt]             INT          NOT NULL,
    [stsm_des]             VARCHAR (50) NOT NULL,
    [stsm_flg_ativo]       BIT          CONSTRAINT [df_stsm_flg_ativo] DEFAULT ((1)) NOT NULL,
    [stsm_dat_criacao]     DATETIME     CONSTRAINT [df_stsm_dat_criacao] DEFAULT (getdate()) NOT NULL,
    [stsm_dat_atualizacao] DATETIME     CONSTRAINT [df_stsm_dat_atualizacao] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [pk_stsm] PRIMARY KEY CLUSTERED ([stsm_idt] ASC),
    CONSTRAINT [uk_stsm_des] UNIQUE NONCLUSTERED ([stsm_des] ASC)
);

