CREATE TABLE [dbo].[tb_dfisoc_solicitacao_ciencia_fi1548] (
    [dfisoc_idt]          		INT              IDENTITY (1, 1) NOT NULL,
    [dfi_idt]             		INT              NOT NULL,
    [dfitci_idt]          		INT              NOT NULL,
    [dfisci_idt]          		INT              NOT NULL,
    [dfisoc_flg_ativo]    		BIT              CONSTRAINT [df_dfisoc_flg_ativo] DEFAULT ((1)) NOT NULL,
    [dfisoc_usu_guid_ad]  		UNIQUEIDENTIFIER NOT NULL,
    [dfisoc_des_observacao] 	VARCHAR (200)    NULL,
    [dfisoc_dat_criacao]    	DATETIME         CONSTRAINT [df_dfisoc_dat_criacao] DEFAULT (getdate()) NOT NULL,
    [dfisoc_dat_atualizacao] 	DATETIME         CONSTRAINT [df_dfisoc_dat_atualizacao] DEFAULT (getdate()) NULL,
    CONSTRAINT [pk_dfisoc] PRIMARY KEY CLUSTERED ([dfisoc_idt] ASC),
    CONSTRAINT [fk_dfisoc_sci] FOREIGN KEY ([dfisci_idt]) REFERENCES [dbo].[tb_dfisci_status_ciencia_fi1548] ([dfisci_idt]),
    CONSTRAINT [fk_dfisoc_dfi] FOREIGN KEY ([dfi_idt]) REFERENCES [dbo].[tb_dfi_documento_fi1548] ([dfi_idt]),
    CONSTRAINT [fk_dfisoc_tci] FOREIGN KEY ([dfitci_idt]) REFERENCES [dbo].[tb_dfitci_tipo_ciencia_fi1548] ([dfitci_idt])
);
