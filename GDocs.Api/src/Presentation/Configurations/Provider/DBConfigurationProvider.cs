using Dapper;
using ICE.GDocs.Infra.CrossCutting.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace ICE.GDocs.Api.Configurations.Provider
{
    public class DBConfigurationProvider : ConfigurationProvider
    {
        private readonly string _configurationKey;
        private readonly string _connectionString;

        public DBConfigurationProvider(
            string configurationKey,
            string connectionString
        )
        {
            _configurationKey = configurationKey;
            _connectionString = connectionString;
        }

        public override void Load()
        {
            ConfiguracaoModel configuracaoModel = null;
            Data.Clear();

            using (var connection = new System.Data.SqlClient.SqlConnection(_connectionString))
            {
                configuracaoModel = connection.Query<ConfiguracaoModel>(@"
					SELECT TOP 1
						cfg_idt as [Id],
						cfg_nom_key as [Chave],
						cfg_des_value as [Valor],
						cfg_des_descricao as [Descricao],
						cfg_flg_ativo as [Ativo]
					FROM
						dbo.tb_cfg_configuracao
					WHERE
						cfg_nom_key = @key"
                    ,
                    new { key = _configurationKey })
                .ToCollection()
                .FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(configuracaoModel?.Valor))
                Data = JsonConfigurationParser.Parse(configuracaoModel.Valor);
        }
    }

    public static class DBConfigurationExtensions
    {
        public static IConfigurationBuilder AddDBConfiguration(
            this IConfigurationBuilder builder,
            string connectionStringName,
            string configurationKey
        )
            => builder.Add(new DBConfigurationSource(configurationKey, builder.Build().GetConnectionString(connectionStringName)));
    }

    public class DBConfigurationSource : IConfigurationSource
    {
        private readonly string _chaveConfiguracao;
        private readonly string _connectionString;

        public DBConfigurationSource(
            string configurationKey,
            string connectionString
        )
        {
            _chaveConfiguracao = configurationKey;
            _connectionString = connectionString;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
            => new DBConfigurationProvider(_chaveConfiguracao, _connectionString);
    }
}
