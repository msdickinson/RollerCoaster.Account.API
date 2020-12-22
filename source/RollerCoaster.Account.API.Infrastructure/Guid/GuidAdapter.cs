using DickinsonBros.Guid.Abstractions;
using RollerCoaster.Account.API.UseCases.InterfaceAdapters.GuidFactory;

namespace RollerCoaster.Account.API.Infrastructure.Guid
{
    public class GuidAdapter : IGuidFactory
    {
        internal readonly IGuidService _guidService;
        public GuidAdapter
        (
            IGuidService guidService
        )
        {
            _guidService = guidService;
        }

        public System.Guid NewGuid()
        {
            return _guidService.NewGuid();
        }
    }
}
