using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ChronoBot.Services
{
    public class HttpService
    {
        public HttpClient Client { get; set; }

        public HttpService()
        {
            Client = new HttpClient();
        }
    }
}
