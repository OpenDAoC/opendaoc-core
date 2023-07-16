


 //Created by Loki 2020

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DOL.GS
{


    class DolWebHook
    {
        private HttpClient Client;
        private string Url;

        public string Name { get; set; }


        public DolWebHook(string webhookUrl)
        {
            Client = new HttpClient();
            Url = webhookUrl;
        }

        public bool SendMessage(string content)
        {
            MultipartFormDataContent data = new MultipartFormDataContent();


            data.Add(new StringContent(content), "content");



            var resp = Client.PostAsync(Url, data).Result;

            return resp.StatusCode == HttpStatusCode.NoContent;
        }
    }

}