CREATE TABLE [dbo].[tb_seq_sequencia]
(
	[seq_nom] VARCHAR(30) NOT NULL,
	[seq_val] INT NOT NULL DEFAULT 1,
	[seq_inc] INT NOT NULL DEFAULT 1,	
	CONSTRAINT [pk_seq] PRIMARY KEY CLUSTERED ([seq_nom] ASC)
)