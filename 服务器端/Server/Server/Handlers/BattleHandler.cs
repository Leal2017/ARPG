using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ExitGames.Logging;
using NHibernate.Loader;
using Photon.SocketServer;
using TaidouCommon;
using TaidouCommon.Model;
using TaidouCommon.Tools;

namespace TaidouServer.Handlers {
    class BattleHandler : HandlerBase {
        private static readonly ILogger log = ExitGames.Logging.LogManager.GetCurrentClassLogger();
        public override void OnHandlerMessage(OperationRequest request, OperationResponse response, ClientPeer peer,SendParameters sendParameters)
        {
            SubCode subcode = ParameterTool.GetSubcode(request.Parameters);
            ParameterTool.AddSubcode(response.Parameters,subcode);
            switch (subcode)
            {
                case SubCode.SendTeam:
                    if (TaidouApplication.Instance.clientPeerListForTeam.Count >= 2)
                    {
                        //取得list中的前两个peer  跟当前的peer进行组队
                        ClientPeer peer1 = TaidouApplication.Instance.clientPeerListForTeam[0];
                        ClientPeer peer2 = TaidouApplication.Instance.clientPeerListForTeam[1];
                        Team t = new Team(peer1,peer2,peer);
                        TaidouApplication.Instance.clientPeerListForTeam.RemoveRange(0,2);
                        List<Role> roleList = new List<Role>();
                        foreach (var clientPeer in t.clientPeers)
                        {
                            roleList.Add(clientPeer.LoginRole);
                        }
                        ParameterTool.AddParameter(response.Parameters,ParameterCode.RoleList, roleList);
                        ParameterTool.AddParameter(response.Parameters,ParameterCode.MasterRoleID,t.masterRoleId,false);
                        response.ReturnCode = (short) ReturnCode.GetTeam;

                        SendEventByPeer(peer1,(OperationCode) response.OperationCode, SubCode.GetTeam, roleList,t.masterRoleId);
                        SendEventByPeer(peer2,(OperationCode) response.OperationCode,  SubCode.GetTeam ,roleList,t.masterRoleId);
                    }
                    else
                    {
                        //当当前服务器可供组队的客户端不足的时候，把自身添加到集合中 等待组队
                        TaidouApplication.Instance.clientPeerListForTeam.Add(peer);
                        response.ReturnCode = (short) ReturnCode.WartingTeam;
                    }
                    break;
                case SubCode.CancelTeam:
                    TaidouApplication.Instance.clientPeerListForTeam.Remove(peer);
                    response.ReturnCode = (short) ReturnCode.Success;
                    break;
                case SubCode.SyncPositionAndRotation:
                    object posObj = null;
                    request.Parameters.TryGetValue((byte) ParameterCode.Position, out posObj);
                    object eulerAnglesObj = null;
                    request.Parameters.TryGetValue((byte) ParameterCode.EulerAngles, out eulerAnglesObj);
                    foreach (ClientPeer temp in peer.Team.clientPeers)
                    {
                        if (temp != peer)
                        {
                            SendEventByPeer(temp,OpCode,SubCode.SyncPositionAndRotation, peer.LoginRole.ID,posObj,eulerAnglesObj);
                        }
                    }
                    break;
                case SubCode.SyncMoveAnimation:
                    foreach (ClientPeer temp in peer.Team.clientPeers) {
                        if (temp != peer) {
                            SendMoveAnimationEvent(temp,OpCode,SubCode.SyncMoveAnimation,peer.LoginRole.ID,request.Parameters);
                        }
                    }
                    break;
                case SubCode.SyncAnimation:
                    request.Parameters.Add((byte) ParameterCode.RoleID,peer.LoginRole.ID);
                    RequestTool.TransmitRequest(peer,request,OpCode);
                    break;
                case SubCode.SendGameState:
                    RequestTool.TransmitRequest(peer,request,OpCode);
                    peer.Team.Dismiss();//解散队伍
                    break;
            }
        }

        void SendMoveAnimationEvent(ClientPeer peer,OperationCode opCode,SubCode subCode,int roleID,Dictionary<byte,object> parameters  )
        {
            EventData data = new EventData();
            data.Parameters = parameters;
            ParameterTool.AddOperationcodeSubcodeRoleID(parameters,opCode,roleID);
            peer.SendEvent(data, new SendParameters());
        }

        //向客户端发送 位置和旋转的数据 进行同步
        void SendEventByPeer(ClientPeer peer,OperationCode opCode,SubCode subCode,int roleId,object posObj,object eulerAnglesObj)
        {
            EventData data = new EventData();
            data.Parameters = new Dictionary<byte, object>();
            ParameterTool.AddParameter(data.Parameters,ParameterCode.OperationCode,opCode,false);
            ParameterTool.AddParameter(data.Parameters,ParameterCode.SubCode,subCode,false);
            data.Parameters.Add((byte) ParameterCode.RoleID,roleId);
            data.Parameters.Add((byte) ParameterCode.Position,posObj.ToString());
            data.Parameters.Add((byte) ParameterCode.EulerAngles,eulerAnglesObj.ToString());
            peer.SendEvent(data, new SendParameters());
        }
        void SendEventByPeer( ClientPeer peer,OperationCode opCode, SubCode subcode ,List<Role> rolelist ,int masterRoleID)
        {
            //OperationResponse response  = new OperationResponse();
            //response.Parameters = new Dictionary<byte, object>();
            //ParameterTool.AddSubcode(response.Parameters, subcode);
            //ParameterTool.AddParameter(response.Parameters,ParameterCode.RoleList, rolelist);
            //response.ReturnCode = (short) ReturnCode.GetTeam;
            //peer.SendOperationResponse(response, sendParameters);

            EventData eventData = new EventData();
            eventData.Parameters = new Dictionary<byte, object>();
            ParameterTool.AddParameter(eventData.Parameters,ParameterCode.OperationCode,opCode,false);
            ParameterTool.AddSubcode(eventData.Parameters,subcode);
            ParameterTool.AddParameter(eventData.Parameters,ParameterCode.RoleList,rolelist);
            ParameterTool.AddParameter(eventData.Parameters,ParameterCode.MasterRoleID, masterRoleID,false);

            peer.SendEvent(eventData, new SendParameters());
        }

        public override OperationCode OpCode
        {
            get { return OperationCode.Battle;}
        }
    }
}
