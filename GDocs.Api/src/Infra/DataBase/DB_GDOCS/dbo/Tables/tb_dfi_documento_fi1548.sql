CREATE TABLE [dbo].[tb_dfi_documento_fi1548] (
    [dfi_idt]                        INT              IDENTITY (1, 1) NOT NULL,
    [dfi_descricao]                  VARCHAR (640)    NOT NULL,
    [dfi_referencia]                 VARCHAR (100)    NOT NULL,
    [dfi_valor_total]                DECIMAL (10, 2)  NOT NULL,
    [dfi_pagamento]                  VARCHAR (100)    NOT NULL,
    [dfi_quantidade_parcelas]        INT              NOT NULL,
    [sdo_idt]                        INT              CONSTRAINT [df_dfi_sdo_idt] DEFAULT (1) NOT NULL,
    [dfi_guid_usuario_ad]            UNIQUEIDENTIFIER NOT NULL,
    [dfi_flg_ativo]                  BIT              CONSTRAINT [df_dfi_flg_ativo] DEFAULT (1) NOT NULL,
    [dfi_dat_criacao]                DATETIME         CONSTRAINT [df_dfi_dat_criacao] DEFAULT (getdate()) NOT NULL,
    [dfi_dat_atualizacao]            DATETIME         CONSTRAINT [df_dfi_dat_atualizacao] DEFAULT (getdate()) NOT NULL,
    [bin_idt]                        BIGINT           NULL,
    [dfi_dat_liquidacao]             DATETIME         NULL,
    [dfi_numero]                     INT              NOT NULL,
    [dfi_moeda_simbolo]              VARCHAR (5)      DEFAULT ('R$') NOT NULL,
    [dfi_fornecedor]                 VARCHAR (100)    NULL,
    [dfi_prazo_entrega]              VARCHAR (255)    NULL,
    [dfi_vencimento_pagamento_unico] INT              NULL,
    [dfi_referencia_substituto]      INT              NULL,
    CONSTRAINT [pk_dfi] PRIMARY KEY CLUSTERED ([dfi_idt] ASC),
    CONSTRAINT [fk_dfi_bin] FOREIGN KEY ([bin_idt]) REFERENCES [dbo].[tb_bin_binario] ([bin_idt]),
    CONSTRAINT [fk_dfi_sdo] FOREIGN KEY ([sdo_idt]) REFERENCES [dbo].[tb_sdo_status_documento] ([sdo_idt]),
    CONSTRAINT [fk_dfi_dfi] FOREIGN KEY ([dfi_referencia_substituto]) REFERENCES [dbo].[tb_dfi_documento_fi1548] ([dfi_idt])
);


