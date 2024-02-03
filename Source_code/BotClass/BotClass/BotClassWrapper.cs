using Avalonia.Controls;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tmds.DBus.Protocol;
using static System.Net.WebRequestMethods;

namespace BotClass
{
    internal static class BotClassWrapper
    {
        private static readonly HttpClient client = new HttpClient();
        static string URI_API = "https://cloud.botclass.ru/api/message";
        static string sessionId = "dc132d1f-937f-4a39-b466-117e087dd0c3";
        public static event EventHandler<ResponseEventArgs>? ResponseReceived;
        public class ResponseEventArgs : EventArgs
        {
            public ResponseEventArgs(string r)
            {
                Response = r;
            }
            public string Response { get; set; } = "";
        }

        static string RaiseEvent_GotResponse(string response)
        {
            if (ResponseReceived != null)
            {
                ResponseReceived(null, new ResponseEventArgs(response));
            }
            return response;
        }
        
        public static async Task PushMessage(string botToken, string message)
        {
            //Flurl
            try
            {
                var resp = await URI_API
                .PostJsonAsync(new
                {
                    sessionId = sessionId,
                    botToken = botToken,
                    message = message
                })
                .ReceiveString();
                JObject json = JObject.Parse(resp);
#pragma warning disable 8600
                string resultMessage = (string)json["message"] ?? "";
                _ = RaiseEvent_GotResponse(resultMessage);
            }
            catch (Exception e)
            {
                _ = RaiseEvent_GotResponse(e.Message);
            }
        }
    }
}
