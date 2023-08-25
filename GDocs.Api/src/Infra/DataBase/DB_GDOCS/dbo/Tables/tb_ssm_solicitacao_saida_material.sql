CREATE TABLE [dbo].[tb_ssm_solicitacao_saida_material] (
    [ssm_idt]                  INT              IDENTITY (1, 1) NOT NULL,
    [ssm_num]                  INT              NOT NULL,
    [ssm_flg_retorno]          BIT              DEFAULT ((0)) NOT NULL,
    [ssm_guid_ad_responsavel]  UNIQUEIDENTIFIER NOT NULL,
    [ssm_guid_ad_autor]        UNIQUEIDENTIFIER NOT NULL,
    [ssm_nm_setor_responsavel] VARCHAR (50)     NOT NULL,
    [ssm_des_origem]           VARCHAR (10)     NOT NULL,
    [ssm_des_destino]          VARCHAR (70)     NOT NULL,
    [ssm_dat_retorno]          SMALLDATETIME    NULL,
    [ssm_des_motivo]           VARCHAR (256)    NOT NULL,
    [ssm_des_observacao]       VARCHAR (255)    NULL,
    [ssm_dat_cricao]           DATETIME         DEFAULT (getdate()) NOT NULL,
    [ssm_dat_atualizacao]      DATETIME         DEFAULT (getdate()) NULL,
    [ssm_flg_ativo]            BIT              DEFAULT ((1)) NOT NULL,
    [bin_idt]                  BIGINT           NULL,
    [stsm_idt]                 INT              NOT NULL,
    [ssm_ret_parcial]          BIT              NULL, 
    CONSTRAINT [pk_ssm] PRIMARY KEY CLUSTERED ([ssm_idt] ASC),
    CONSTRAINT [fk_bin_idt] FOREIGN KEY ([bin_idt]) REFERENCES [dbo].[tb_bin_binario] ([bin_idt]),
    CONSTRAINT [uk_ssm_1] UNIQUE NONCLUSTERED ([ssm_num] ASC)
);





