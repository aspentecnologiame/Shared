using ICE.GDocs.Domain.Core.Services;
using ICE.GDocs.Domain.Core.Uow;
using ICE.GDocs.Domain.Repositories;
using System;
using System.Threading.Tasks;

namespace ICE.GDocs.Domain.Services
{
    internal class SequencialService : DomainService, ISequencialService
    {
        private const string CHAVE_FI1548_STATUS_PAGAMENTO = "FI1548StatusPagamento";
        private const string CHAVE_FI347_SAIDA_MATERIAS = "SaidaMateriais";

        private readonly ISequencialRepository _sequencialRepository;

        public SequencialService(
            IUnitOfWork unitOfWork,
            ISequencialRepository sequencialRepository
        ) : base(unitOfWork)
        {
            _sequencialRepository = sequencialRepository;
        }

        public async Task<TryException<int>> ObterProximoSequencialStatusPagamento()
            => await _sequencialRepository.ObterProximo(CHAVE_FI1548_STATUS_PAGAMENTO);

        public async Task<TryException<int>> ObterProximoSequencialSaidaMaterias() 
            => await _sequencialRepository.ObterProximo(CHAVE_FI347_SAIDA_MATERIAS);

        public async Task<TryException<int>> ObterProximoSequencialPorChaveCategoria(string chaveCategoria)
            => await _sequencialRepository.ObterProximo(chaveCategoria);
    }
}
