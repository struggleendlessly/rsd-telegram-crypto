namespace api_alchemy
{
    public class ApiAlchemy
    {
        private readonly HttpClient httpClient;

        public ApiAlchemy(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
    }
}
