namespace HRKošarka.UI.Services.Base
{
    public partial class Client : IClient
    {
        public HttpClient HttpClient
        {
            get
            {
                return _httpClient;
            }
            set { }
        }
    }
}
