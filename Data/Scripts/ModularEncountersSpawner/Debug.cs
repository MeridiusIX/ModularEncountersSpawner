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
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;
using VRageMath;
using ModularEncountersSpawner;
using ModularEncountersSpawner.Configuration;
using ModularEncountersSpawner.Templates;
using ModularEncountersSpawner.Spawners;

namespace ModularEncountersSpawner{
	
	public static class Debug{
		
		public static bool DebugMode = true;
		
		public static void CheckZone(){
			
			if(DebugMode == false){
				
				return;
				
			}
			
			var player = MyAPIGateway.Session.LocalHumanPlayer;
			SpawnResources.RefreshEntityLists();
			var check = SpawnResources.IsPositionInSafeZone(player.GetPosition());
			
			if(check == true){
				
				Logger.AddMsg("In Zone!", true);
				
			}
			
		}

        public static void InitStoreBlocks() {

            var entityList = new HashSet<IMyEntity>();
            MyAPIGateway.Entities.GetEntities(entityList);

            foreach(var entity in entityList) {

                var cubeGrid = entity as IMyCubeGrid;

                if(cubeGrid == null) {

                    continue;

                }

                //EconomyHelper.InitNpcStoreBlock(cubeGrid);

            }

        }

        public static void ReputationCheck(SyncData receivedData) {

            var stringSplit = receivedData.ChatMessage.Split('.');

            if(stringSplit.Length != 4) {

                return;

            }

            var npcFaction = MyAPIGateway.Session.Factions.TryGetFactionByTag(stringSplit[3]);

            if(npcFaction == null) {

                return;

            }

            var myFaction = MyAPIGateway.Session.Factions.TryGetPlayerFaction(receivedData.PlayerId);
            int rep = 0;

            if(myFaction != null) {

                rep = MyAPIGateway.Session.Factions.GetReputationBetweenFactions(myFaction.FactionId, npcFaction.FactionId);

            } else {

                rep = MyAPIGateway.Session.Factions.GetReputationBetweenPlayerAndFaction(receivedData.PlayerId, npcFaction.FactionId);

            }

            MyVisualScriptLogicProvider.ShowNotification("Reputation To Faction: " + rep.ToString(), 5000, "White", receivedData.PlayerId);

        }
		
	}
	
}