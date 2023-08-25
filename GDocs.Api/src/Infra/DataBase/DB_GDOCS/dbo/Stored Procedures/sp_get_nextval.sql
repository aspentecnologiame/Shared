CREATE PROCEDURE [dbo].[sp_get_nextval] (
	@nome VARCHAR(30), 
	@valor INT OUTPUT
)
AS
BEGIN
	BEGIN TRAN

		if not exists (select top 1 1 from [dbo].[tb_seq_sequencia] where seq_nom = @nome) begin
			RAISERROR('Sequence não encontrada!', 16, 1) --change to > 10
			RETURN
		end

		UPDATE [dbo].[tb_seq_sequencia] SET @valor = seq_val = seq_val + seq_inc WHERE seq_nom = @nome
	COMMIT
END