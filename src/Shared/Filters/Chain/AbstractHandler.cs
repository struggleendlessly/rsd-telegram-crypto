using Shared.Filters.Model;

namespace Shared.Filters.Chain
{
    public abstract class AbstractHandler : IHandler
    {
        private IHandler _nextHandler;

        public IHandler SetNext(IHandler handler)
        {
            _nextHandler = handler;

            // Returning a handler from here will let us link handlers in a
            // convenient way like this:
            // monkey.SetNext(squirrel).SetNext(dog);
            return handler;
        }

        public async virtual Task<AddressRequest> Handle(AddressRequest request)
        {
            var hadlerName = GetType().Name;
            Console.WriteLine(hadlerName);

            if (!request.IsValid && string.IsNullOrEmpty(request.TokenInfo.ErrorType))
            {
                request.TokenInfo.ErrorType = hadlerName;
            }

            if (_nextHandler != null)
            {
                return await _nextHandler.Handle(request);
            }
            else
            {
                return request;
            }
        }
    }
}
