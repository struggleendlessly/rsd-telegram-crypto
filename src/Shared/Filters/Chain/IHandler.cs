using Shared.Filters.Model;

namespace Shared.Filters.Chain
{
    public interface IHandler
    {
        IHandler SetNext(IHandler handler);

        Task<AddressRequest> Handle(AddressRequest request);
    }
}
