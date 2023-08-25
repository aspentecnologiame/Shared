using AutoMapper;

namespace ICE.GDocs.Api.Mappers
{
    internal class ConfiguracaoMappingProfile : Profile
    {
        public ConfiguracaoMappingProfile()
        {
            CreateMapping();
        }

        private void CreateMapping()
        {
            /* Atenção ao criar uma versão de uma features,
             * ajustar os demais mappers para evitar problemas nas versões anteriores
             */

            // Mapeamento para V1
            CreateMap<Infra.CrossCutting.Models.ConfiguracaoModel, V1.Models.Configuracao>();
        }
    }
}
