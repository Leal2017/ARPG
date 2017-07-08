using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using TaidouCommon;
using TaidouCommon.Tools;

namespace TaidouServer.Handlers {
    class BossHandler :HandlerBase{
        public override void OnHandlerMessage(OperationRequest request, OperationResponse response, ClientPeer peer,
            SendParameters sendParameters)
        {
            SubCode subcode = ParameterTool.GetSubcode(request.Parameters);
            switch (subcode)
            {
                case SubCode.SyncBossAnimation:
                    RequestTool.TransmitRequest(peer,request,OpCode);
                    break;
            }
        }

        public override OperationCode OpCode
        {
            get { return OperationCode.Boss;}
        }
    }
}
