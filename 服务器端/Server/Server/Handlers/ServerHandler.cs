using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LitJson;
using Photon.SocketServer;
using TaidouCommon;
using TaidouCommon.Model;
using TaidouServer.DB.Manager;

namespace TaidouServer.Handlers {
    class ServerHandler :HandlerBase
    {
        private ServerPropertyManager manager;

        public ServerHandler()
        {
            manager=new ServerPropertyManager();
        }

        public override void OnHandlerMessage(Photon.SocketServer.OperationRequest request, OperationResponse response, ClientPeer peer,SendParameters sendParameters)
        {
            List<ServerProperty> list = manager.GetServerList();
            string json = JsonMapper.ToJson(list);
            Dictionary<byte, object> parameters = response.Parameters;
            parameters.Add((byte) ParameterCode.ServerList,json);

            response.OperationCode = request.OperationCode;

        }

        public override OperationCode OpCode {
            get { return OperationCode.GetServer;}
        }
    }
}
