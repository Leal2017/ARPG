using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using Photon.SocketServer;
using TaidouServer;

namespace TaidouCommon.Tools {
    public class RequestTool {
        public static void TransmitRequest(ClientPeer peer, OperationRequest request,OperationCode opCode) {
            foreach (ClientPeer temp in peer.Team.clientPeers) {
                if (temp != peer) {
                    EventData data = new EventData();
                    data.Parameters = request.Parameters;
                    ParameterTool.AddOperationcodeSubcodeRoleID(data.Parameters, opCode, peer.LoginRole.ID);
                    temp.SendEvent(data, new SendParameters());
                }
            }
        }
    }
}
