using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox.Common;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Common.ObjectBuilders.Definitions;
using Sandbox.Definitions;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.EntityComponents;
using Sandbox.Game.GameSystems;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI;
using ProtoBuf;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;
using VRageMath;
using ModularEncountersSpawner;

namespace ModularEncountersSpawner.Templates{
	
	[ProtoContract]
	public class SaleTracker{

        [ProtoMember(1)]
        public Dictionary<long, long> Transactions;

        public SaleTracker(){

            Transactions = new Dictionary<long, long>();

        }

        public string ToString() {

            try {

                var byteData = MyAPIGateway.Utilities.SerializeToBinary<SaleTracker>(this);
                var stringData = Convert.ToBase64String(byteData);
                return stringData;

            } catch(Exception exc) {

                Logger.AddMsg("Failed To Save SaleTracker Data to String");

            }

            return "";

        }
		
	}
	
}