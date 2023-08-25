GO

DECLARE 
    @StatusNaoIniciado VARCHAR(255) = 'Não iniciado',
    @NomeFluxoPadrao VARCHAR(255) = 'Padrao',
    @NomeFluxoDir VARCHAR(255) = 'Dir';

DECLARE @type_version NVARCHAR(64) = ''
SELECT @type_version = type_version FROM msdb.dbo.sysdac_instances_internal WHERE instance_name = DB_NAME()

IF @type_version = ''
BEGIN 
    GOTO CreateDataBase
END
ELSE IF @type_version = '1.0.0.0'
    GOTO MigracaoDaVersao1000
ELSE IF @type_version = '1.0.0.2'
    GOTO MigracaoDaVersao1002
ELSE IF @type_version = '1.0.0.3' OR @type_version = '1.0.0.4'
    GOTO MigracaoDaVersao1003e1004
ELSE IF @type_version = '1.0.0.5'
    GOTO MigracaoDaVersao1005
ELSE IF @type_version = '1.0.0.7'
    GOTO MigracaoDaVersao1007
ELSE IF @type_version = '1.0.0.9'
    GOTO MigracaoDaVersao1009
ELSE IF @type_version = '1.0.1.0'
    GOTO MigracaoDaVersao1010
ELSE IF @type_version = '1.0.1.2'
    GOTO MigracaoDaVersao1012
ELSE IF @type_version = '1.0.1.3'
    GOTO MigracaoDaVersao1013
ELSE IF @type_version = '1.0.1.4'
    GOTO MigracaoDaVersao1015
ELSE IF @type_version = '1.0.1.5'
    GOTO MigracaoDaVersao1016
ELSE
BEGIN 
    GOTO Sair
END

CreateDataBase:
:r .\Post-Deployment\CargaInicial.sql
GOTO Sair

MigracaoDaVersao1000:
:r .\Post-Deployment\migracao_da_versao_1_0_0_0.sql

MigracaoDaVersao1002:
:r .\Post-Deployment\migracao_da_versao_1_0_0_2.sql

MigracaoDaVersao1003e1004:
:r .\Post-Deployment\migracao_da_versao_1_0_0_3_e_1_0_0_4.sql

MigracaoDaVersao1005:
:r .\Post-Deployment\migracao_da_versao_1_0_0_5.sql

MigracaoDaVersao1007:
:r .\Post-Deployment\migracao_da_versao_1_0_0_7.sql

MigracaoDaVersao1009:
:r .\Post-Deployment\migracao_da_versao_1_0_0_9.sql

MigracaoDaVersao1010:
:r .\Post-Deployment\migracao_da_versao_1_0_1_0.sql

MigracaoDaVersao1012:
:r .\Post-Deployment\migracao_da_versao_1_0_1_2.sql

MigracaoDaVersao1013_ExecutadoManualmente:
:r .\Post-Deployment\migracao_da_versao_1_0_1_3.sql

MigracaoDaVersao1013:
:r .\Post-Deployment\migracao_da_versao_1_0_1_4.sql

MigracaoDaVersao1015:
:r .\Post-Deployment\migracao_da_versao_1_0_1_5.sql

MigracaoDaVersao1016:
:r .\Post-Deployment\migracao_da_versao_1_0_1_6.sql

Sair:
:r .\Post-Deployment\configuracoes.sql

Print 'Migrate complete'

GO
