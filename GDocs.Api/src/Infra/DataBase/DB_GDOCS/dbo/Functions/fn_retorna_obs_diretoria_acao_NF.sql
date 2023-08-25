CREATE FUNCTION [dbo].[fn_retorna_obs_diretoria_acao_NF]
(
	@idSolicitacaoCienciaNf int
)
RETURNS [varchar](max)WITH EXECUTE AS CALLER
AS 
BEGIN

	declare 
		@htmlRetorno varchar(max) = '',
		@nome varchar(100),
		@observacao varchar(200),
		@htmlPadrao varchar(max) = '<div><strong> #NOME </strong><pre><b>Observação DIR: </b>#RESPOSTA</pre></div>'

	DECLARE itens_cursor CURSOR FOR   
	SELECT
	  (select top 1  usp_nome_ad 
	  from tb_usp_usuario_perfil as u 
	  where usp_guid_ad = a.scunfa_usu_guid_ad and usp_nome_ad is not null) as nome ,
	  a.scunfa_des_observacao as observacao 
	FROM
		tb_socnf_solicitacao_ciencia_nota_fiscal  s  
		join tb_scunfa_solicitacao_ciencia_usuario_nota_fiscal_aprovacao a on a.socnf_idt = s.socnf_idt
	WHERE 
	 a.socnf_idt = @idSolicitacaoCienciaNf
	 and scunfa_flg_ativo = 1	
	 and a.scunfa_flg_rejeitado = 1
	ORDER BY 
		a.scunfa_dat_atualizacao
		 
	OPEN itens_cursor  
  
	FETCH NEXT FROM itens_cursor INTO @nome, @observacao 
  
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
	  IF @observacao is not null
	    BEGIN
		 SET @htmlRetorno = @htmlRetorno + replace(replace(@htmlPadrao, '#NOME', convert(varchar(100),@nome)),'#RESPOSTA', @observacao)
        END
		
		FETCH NEXT FROM itens_cursor INTO @nome, @observacao 

	END

	CLOSE itens_cursor;  
	DEALLOCATE itens_cursor;  

	return @htmlRetorno;

END
