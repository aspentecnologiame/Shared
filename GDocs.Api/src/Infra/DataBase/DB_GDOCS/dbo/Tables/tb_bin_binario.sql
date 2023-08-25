CREATE TABLE [dbo].[tb_bin_binario]
(
    [bin_idt]       BIGINT IDENTITY (1, 1) NOT NULL,
    [bin_val]       VARBINARY (MAX) NOT NULL,
    [bin_hash_sha1] VARCHAR(255) NOT NULL,
    [bin_dat_criacao]   DATETIME CONSTRAINT [df_bin_dat_criacao] DEFAULT getdate() NOT NULL,    
    CONSTRAINT [pk_bin] PRIMARY KEY CLUSTERED ([bin_idt] ASC)
)
