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
using ModularEncountersSpawner.Configuration;
using ModularEncountersSpawner.Templates;
using ModularEncountersSpawner.Manipulation;
using ModularEncountersSpawner.World;

namespace ModularEncountersSpawner.Spawners {

	public static class OtherNPCSpawner{
		
		public static void AttemptSpawn(SpawnRequestMES spawnData){
			
			if(Settings.General.UseMaxNpcGrids == true){
				
				var totalNPCs = NPCWatcher.ActiveNPCs.Count;
				
				if(totalNPCs >= Settings.General.MaxGlobalNpcGrids){
					
					Logger.AddMsg("Spawning Aborted. Max Global NPCs Limit Reached.", true);
					return;
					
				}
				
			}
			
			ImprovedSpawnGroup spawnGroup = null;
			
			foreach(var spawngroupItem in SpawnGroupManager.SpawnGroups){
				
				if(spawngroupItem.SpawnGroupName == spawnData.SpawnGroupName){
					
					spawnGroup = spawngroupItem;
					break;
					
				}
				
			}
			
			if(spawnGroup == null){
				
				Logger.AddMsg("SpawnGroup with Name: " + spawnData.SpawnGroupName + " Not Found.", true);
				return;
				
			}
			
			//Get Directions
			var spawnForwardDir = spawnData.SpawnDirectionForward;
			var spawnUpDir = spawnData.SpawnDirectionUp;
			var spawnMatrix = MatrixD.CreateWorld(spawnData.SpawnCoordinates, spawnForwardDir, spawnUpDir);
			string factionTag = spawnGroup.FactionOwner;
			long gridOwner = 0;
			
			if(spawnData.FactionTagOverride != ""){
				
				factionTag = spawnData.FactionTagOverride;
				
			}

			if(NPCWatcher.NPCFactionTagToFounder.ContainsKey(factionTag) == true){
				
				gridOwner = NPCWatcher.NPCFactionTagToFounder[factionTag];
				
			}else{
				
				Logger.AddMsg("Could Not Find Faction Founder For: " + factionTag);
				
			}

			foreach(var prefab in spawnGroup.SpawnGroup.Prefabs){
				
				var options = SpawnGroupManager.CreateSpawningOptions(spawnGroup, prefab);
				var spawnPosition = Vector3D.Transform((Vector3D)prefab.Position, spawnMatrix);
				var speedL = (Vector3)spawnData.LinearVelocity;
				var speedA = Vector3.Zero;
				var gridList = new List<IMyCubeGrid>();
				
				//Grid Manipulation
				ManipulationCore.ProcessPrefabForManipulation(prefab.SubtypeId, spawnGroup, "OtherNPC", prefab.Behaviour);

				try{
					
					MyAPIGateway.PrefabManager.SpawnPrefab(gridList, prefab.SubtypeId, spawnPosition, spawnForwardDir, spawnUpDir, speedL, speedA, prefab.BeaconText, options, gridOwner);
					
				}catch(Exception exc){
					
					Logger.AddMsg("Something Went Wrong With Prefab Spawn Manager.", true);
					
				}
				
				var pendingNPC = new ActiveNPC();
				pendingNPC.SpawnGroup = spawnGroup;
				pendingNPC.Name = prefab.SubtypeId;
				pendingNPC.InitialFaction = factionTag;
				pendingNPC.faction = MyAPIGateway.Session.Factions.TryGetFactionByTag(pendingNPC.InitialFaction);
				pendingNPC.GridName = MyDefinitionManager.Static.GetPrefabDefinition(prefab.SubtypeId).CubeGrids[0].DisplayName;
				pendingNPC.SpawnType = "OtherNPC";
				pendingNPC.CleanupIgnore = spawnGroup.IgnoreCleanupRules;
				pendingNPC.ForceStaticGrid = spawnGroup.ForceStaticGrid;
				pendingNPC.KeenAiName = prefab.Behaviour;
				pendingNPC.KeenAiTriggerDistance = prefab.BehaviourActivationDistance;

				TimeoutManagement.ApplySpawnTimeoutToZones(SpawnType.OtherNPC, spawnPosition);

				if (string.IsNullOrEmpty(pendingNPC.KeenAiName) == false) {

					if (RivalAIHelper.RivalAiBehaviorProfiles.ContainsKey(pendingNPC.KeenAiName) && spawnGroup.UseRivalAi) {

						Logger.AddMsg("RivalAI Behavior Detected In Prefab: " + prefab.SubtypeId + " in SpawnGroup: " + spawnGroup.SpawnGroup.Id.SubtypeName);

					} else {

						Logger.AddMsg("Stock AI Detected In Prefab: " + prefab.SubtypeId + " in SpawnGroup: " + spawnGroup.SpawnGroup.Id.SubtypeName);

					}


				}

				if (spawnGroup.RandomizeWeapons == true){
						
					pendingNPC.ReplenishedSystems = false;
					pendingNPC.ReplacedWeapons = true;
					
				}else if((MES_SessionCore.NPCWeaponUpgradesModDetected == true || Settings.Grids.EnableGlobalNPCWeaponRandomizer == true) && spawnGroup.IgnoreWeaponRandomizerMod == false){
				
					pendingNPC.ReplenishedSystems = false;
					pendingNPC.ReplacedWeapons = true;
					
				}else if(spawnGroup.ReplenishSystems == true){
					
					pendingNPC.ReplenishedSystems = false;
					
				}
				
				NPCWatcher.PendingNPCs.Add(pendingNPC);
				
			}
			
			Logger.AddMsg("Spawning Group - " + spawnGroup.SpawnGroup.Id.SubtypeName);
			return;
			
		}
		
	}
	
}