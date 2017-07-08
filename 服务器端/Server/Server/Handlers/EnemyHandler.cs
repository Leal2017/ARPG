using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using TaidouCommon;
using TaidouCommon.Tools;

namespace TaidouServer.Handlers {
    class EnemyHandler : HandlerBase {
        public override void OnHandlerMessage(OperationRequest request, OperationResponse response, ClientPeer peer,
            SendParameters sendParameters) {
            SubCode subcode = ParameterTool.GetSubcode(request.Parameters);
            switch (subcode) {
                case SubCode.CreateEnemy:
                    TransmitRequest(peer,request);
                    break;
                case SubCode.SyncPositionAndRotation:
                    TransmitRequest(peer, request);
                    break;
                case SubCode.SyncAnimation:
                    TransmitRequest(peer, request);
                    break;
            }
        }
        //这个方法用来转发请求
        void TransmitRequest(ClientPeer peer,OperationRequest request)
        {
            foreach (ClientPeer temp in peer.Team.clientPeers) {
                if (temp != peer) {
                    EventData data = new EventData();
                    data.Parameters = request.Parameters;
                    ParameterTool.AddOperationcodeSubcodeRoleID(data.Parameters, OpCode, peer.LoginRole.ID);
                    temp.SendEvent(data, new SendParameters());
                }
            }
        }

        public override OperationCode OpCode {
            get { return OperationCode.Enemy; }
        }
    }
}
