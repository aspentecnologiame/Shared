CREATE FUNCTION [dbo].[fn_retorna_itens_solicitacao_saida_material_nota_fiscal]
(
@idSolicitacaoSaidaMaterialNotaFiscal int
)
RETURNS [varchar](max) WITH EXECUTE AS CALLER
AS 
BEGIN

	declare 
		@htmlRetorno varchar(max) = '',
		@descricao varchar(50),
		@patrimonio varchar(50),
		@qtde int,
		@unidade varchar(10),
		@contador int = 1,
		@htmlPadrao varchar(max) = '<div><strong>ITEM #NUMITEM</strong><pre>#DESCRICAO (#PATRIMONIO) - Qtde: #QTDE (#UNIDADE)</pre></div>'

	DECLARE itens_cursor CURSOR FOR   
	SELECT
		smnfi_des,
		case when isnull(smnfi_patrimonio,'') = '' then 'N/A' else smnfi_patrimonio end as num_patrimonio,
		smnfi_qtd,
		smnfi_unidade
	FROM
		tb_smnfi_saida_material_nota_fiscal_item (NOLOCK)
	WHERE 
		smnf_idt = @idSolicitacaoSaidaMaterialNotaFiscal
	and smnfi_flg_ativo = 1	
	ORDER BY 
		smnfi_idt

	OPEN itens_cursor  
  
	FETCH NEXT FROM itens_cursor INTO @descricao, @patrimonio, @qtde, @unidade  
  
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		SET @htmlRetorno = @htmlRetorno + replace(replace(replace(replace(replace(@htmlPadrao, '#NUMITEM', convert(varchar(10),@contador)), '#DESCRICAO', @descricao), '#PATRIMONIO', @patrimonio), '#QTDE', convert(varchar(10),@qtde)), '#UNIDADE', @unidade)

		SET @contador = @contador + 1

		FETCH NEXT FROM itens_cursor INTO @descricao, @patrimonio, @qtde, @unidade  
	END

	CLOSE itens_cursor;  
	DEALLOCATE itens_cursor;  

	return @htmlRetorno;

END
