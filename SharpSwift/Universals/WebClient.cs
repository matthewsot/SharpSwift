using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SharpSwift.Universals
{
    public class WebClient
    {
        private HttpClient _client = new HttpClient();

        public void GetAsyncString(string URL, Action<string> handler)
        {
            _client.GetStringAsync(URL).ContinueWith(resp => handler(resp.Result));
        }
    }
}
