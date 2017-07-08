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
using TaidouServer.Tools;

namespace TaidouServer.Handlers {
    public class LoginHandler:HandlerBase
    {

        private UserManager manager;

        public LoginHandler()
        {
            manager = new UserManager();
        }

        public override void OnHandlerMessage(Photon.SocketServer.OperationRequest request,OperationResponse response, ClientPeer peer,SendParameters sendParameters)
        {
            Dictionary<byte, object> parameters = request.Parameters;
            object jsonObject = null;
            parameters.TryGetValue((byte) ParameterCode.User, out jsonObject);
            User user = JsonMapper.ToObject<User>(jsonObject.ToString());
            User userDB = manager.GetUserByUsername(user.Username);
            if (userDB != null && userDB.Password == MD5Tool.GetMD5(user.Password))
            {
                //用户名和密码正确 登录成功
                response.ReturnCode = (short) ReturnCode.Success;
                peer.LoginUser = userDB;
            }
            else
            {
                response.ReturnCode = (short) ReturnCode.Fail;
                response.DebugMessage = "用户名或密码错误!";

            }
        }

        public override OperationCode OpCode {
            get { return OperationCode.Login; }
        }
    }
}
