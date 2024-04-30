namespace ConsoleApp1.BaseScanModels
{
    public interface IHandler
    {
        IHandler SetNext(IHandler handler);

        Task<AddressRequest> Handle(AddressRequest request);
    }
}
