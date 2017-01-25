using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace Api.Framework.Core.Chat
{
    public class WSHandler
    {
        private WebSocket _Ws = null;

        public WSHandler()
        {
            // 
            var address = ConfigurationManager.AppSettings["WSUrl"];
            _Ws = new WebSocket(string.Format("{0}?identity=0", address));
            _Ws.Connect();
        }

        ~WSHandler()
        {
            _Ws.Close();

            _Ws = null;
        }

        public void Send(object Message)
        {
            if (_Ws.ReadyState != WebSocketState.Open)
            {
                _Ws.Connect();
            }

            _Ws.Send(JsonConvert.SerializeObject(Message));
        }
    }
}
