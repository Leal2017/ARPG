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
    public class InventoryItemDBHandler :HandlerBase
    {

        private InventoryItemDBManager inventoryItemDbManager = new InventoryItemDBManager();

        public override void OnHandlerMessage(OperationRequest request, OperationResponse response, ClientPeer peer,SendParameters sendParameters)
        {
            SubCode subcode = ParameterTool.GetParameter<SubCode>(request.Parameters, ParameterCode.SubCode, false);
            ParameterTool.AddParameter(response.Parameters,ParameterCode.SubCode, subcode,false);
            switch (subcode)
            {
                    case SubCode.GetInventoryItemDB:
                        List<InventoryItemDB> list = inventoryItemDbManager.GetInventoryItemDB(peer.LoginRole);
                        foreach (var temp in list)
                        {
                            temp.Role = null;
                        }
                        ParameterTool.AddParameter(response.Parameters,ParameterCode.InventoryItemDBList, list);
                        break;
                    case SubCode.AddInventoryItemDB:
                        InventoryItemDB itemDB = ParameterTool.GetParameter<InventoryItemDB>(request.Parameters,
                            ParameterCode.InventoryItemDB);
                        itemDB.Role = peer.LoginRole;
                        inventoryItemDbManager.AddInventoryItemDB(itemDB);
                        itemDB.Role = null;
                        ParameterTool.AddParameter(response.Parameters,ParameterCode.InventoryItemDB, itemDB);
                        response.ReturnCode = (short) ReturnCode.Success;
                        break;
                    case SubCode.UpdateInventoryItemDB:
                            InventoryItemDB itemDB2 = ParameterTool.GetParameter<InventoryItemDB>(request.Parameters,
                               ParameterCode.InventoryItemDB);
                            itemDB2.Role = peer.LoginRole;
                            inventoryItemDbManager.UpdateInventoryItemDB(itemDB2);
                        break;
                    case SubCode.UpdateInventoryItemDBList:
                    List<InventoryItemDB> list2 = ParameterTool.GetParameter<List<InventoryItemDB>>(request.Parameters,
                        ParameterCode.InventoryItemDBList);
                    foreach (var itemDB3 in list2)
                    {
                        itemDB3.Role = peer.LoginRole;
                    }
                    inventoryItemDbManager.UpdateInventoryItemDBList(list2);
                    break;
                case SubCode.UpgradeEquip:
                    InventoryItemDB itemDB4 = ParameterTool.GetParameter<InventoryItemDB>(request.Parameters,
                        ParameterCode.InventoryItemDB);
                    Role role = ParameterTool.GetParameter<Role>(request.Parameters, ParameterCode.Role);
                    peer.LoginRole = role;
                    role.User = peer.LoginUser;
                    itemDB4.Role = role;
                    inventoryItemDbManager.UpgradeEquip(itemDB4, role);
                    break;
            }
        }

        public override OperationCode OpCode
        {
            get { return OperationCode.InventoryItemDB; }
        }
    }
}
