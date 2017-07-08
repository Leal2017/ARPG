using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using log4net;
using log4net.Config;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaidouCommon;
using TaidouServer.Handlers;

namespace TaidouServer
{
    public class TaidouApplication:ApplicationBase {

        private static TaidouApplication _instance;

        private static readonly ILogger log = ExitGames.Logging.LogManager.GetCurrentClassLogger();

        public Dictionary<byte, HandlerBase> handlers = new Dictionary<byte, HandlerBase>();

        public static TaidouApplication Instance {
            get { return _instance; }
        }

        public List<ClientPeer> clientPeerListForTeam = new List<ClientPeer>(); 

        public TaidouApplication() {
            _instance = this;
            RegisteHandlers();
        }

        protected override PeerBase CreatePeer(InitRequest initRequest) {
            return new ClientPeer(initRequest.Protocol, initRequest.PhotonPeer);
        }

        protected override void Setup() {

            ExitGames.Logging.LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
            GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(this.ApplicationRootPath, "log");
            GlobalContext.Properties["LogFileName"] = "TD" + this.ApplicationName;
            XmlConfigurator.ConfigureAndWatch(new FileInfo(Path.Combine(this.BinaryPath, "log4net.config")));

            log.Debug("Application setup complete.");
        }

        void RegisteHandlers() {
            //handlers.Add((byte)OperationCode.Login, new LoginHandler());//把LoginHandler交给taidouapplication进行管理
            //handlers.Add((byte) OperationCode.GetServer,new ServerHandler());
            //handlers.Add((byte) OperationCode.Register,new RegisterHandler());

            Type[] types = Assembly.GetAssembly(typeof (HandlerBase)).GetTypes();
            foreach (var type in types)
            {
                if (type.FullName.EndsWith("Handler"))
                {
                    Activator.CreateInstance(type);
                }
            }
        }

        protected override void TearDown() {
            log.Debug("Application tear down.");
        }
    }
}
