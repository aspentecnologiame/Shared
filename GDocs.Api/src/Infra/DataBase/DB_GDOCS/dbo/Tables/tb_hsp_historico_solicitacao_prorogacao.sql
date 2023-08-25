CREATE TABLE [dbo].[tb_hsp_historico_solicitacao_prorogacao]
(
	[hsp_idt]  INT NOT NULL  IDENTITY (1,1), 
    [ssm_idt]  INT NOT NULL, 
    [ssma_idt] INT NOT NULL,
    [hsp_dat_de] DATETIME NOT NULL, 
    [hsp_dat_para] DATETIME NULL, 
    [hsp_dat_criacao] DATETIME NULL DEFAULT GetDate(), 

    CONSTRAINT [pk_hsp] PRIMARY KEY CLUSTERED ([hsp_idt] ASC),
    CONSTRAINT [fk_hsp_ssma] FOREIGN KEY ([ssma_idt]) REFERENCES [dbo].[tb_ssma_solicitacao_saida_material_acao] ([ssma_idt])
);
