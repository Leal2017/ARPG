using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using TaidouCommon;
using TaidouCommon.Model;
using TaidouCommon.Tools;
using TaidouServer.DB.Manager;

namespace TaidouServer.Handlers {
    public class RoleHandler:HandlerBase
    {


        private RoleManager roleManager = null;

        public RoleHandler()
        {
            roleManager = new RoleManager();
        }

        public override void OnHandlerMessage(Photon.SocketServer.OperationRequest request, OperationResponse response, ClientPeer peer,SendParameters sendParameters)
        {
            //先得到子操作代码，根据子操作代码，分别进行不同的处理
            SubCode subcode = ParameterTool.GetParameter<SubCode>(request.Parameters, ParameterCode.SubCode, false);


            Dictionary<byte, object> parameters = response.Parameters;
            parameters.Add((byte)ParameterCode.SubCode, subcode);
            response.OperationCode = request.OperationCode;
            switch (subcode)
            {
                case SubCode.AddRole:
                    Role role = ParameterTool.GetParameter<Role>(request.Parameters, ParameterCode.Role);
                    role.User = peer.LoginUser;
                    roleManager.AddRole(role);
                    role.User = null;
                    ParameterTool.AddParameter(response.Parameters,ParameterCode.Role,role);
                    break;
                case SubCode.GetRole:
                    List<Role> roleList = roleManager.GetRoleListByUser(peer.LoginUser);
                    foreach (var role1 in roleList)
                    {
                        role1.User = null;
                    }
                    ParameterTool.AddParameter(parameters,ParameterCode.RoleList,roleList);
                    break;
                case SubCode.SelectRole:
                    peer.LoginRole = ParameterTool.GetParameter<Role>(request.Parameters, ParameterCode.Role);
                    break;
                case SubCode.UpdateRole:
                    Role role2 = ParameterTool.GetParameter<Role>(request.Parameters, ParameterCode.Role);
                    role2.User = peer.LoginUser;
                    roleManager.UpdateRole(role2);
                    role2.User = null;
                    response.ReturnCode = (short) ReturnCode.Success;
                    break;
            }
        }

        public override TaidouCommon.OperationCode OpCode {
            get {return
                OperationCode.Role;
            }
        }
    }
}
