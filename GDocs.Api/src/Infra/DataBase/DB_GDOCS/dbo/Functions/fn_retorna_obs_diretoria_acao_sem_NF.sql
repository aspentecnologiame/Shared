CREATE FUNCTION [dbo].[fn_retorna_obs_diretoria_acao_sem_NF]
(
	@idSolicitacaoCienciaSemNf int
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
	  where usp_guid_ad = a.scua_usu_guid_ad and usp_nome_ad is not null) as nome ,
	  a.scua_des_observacao as observacao 
	FROM
		tb_soc_solicitacao_ciencia  s  
		join tb_scua_solicitacao_ciencia_usuario_aprovacao a on a.soc_idt = s.soc_idt
	WHERE 
	 a.soc_idt = @idSolicitacaoCienciaSemNf
	 and a.scua_flg_ativo = 1	
	 and a.scua_flg_rejeitado = 1
	ORDER BY 
		a.scua_dat_atualizacao
		 

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