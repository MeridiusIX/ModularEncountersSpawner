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

namespace ModularEncountersSpawner.Spawners{
	
	public static class RandomEncounterSpawner{
		
		public static Dictionary<string, List<ImprovedSpawnGroup>> SpawnGroupSublists = new Dictionary<string, List<ImprovedSpawnGroup>>();
		public static Dictionary<string, int> EligibleSpawnsByModId = new Dictionary<string, int>();
		
		public static string AttemptSpawn(Vector3D startCoords){
			
			if(Settings.General.UseMaxNpcGrids == true){
				
				var totalNPCs = NPCWatcher.ActiveNPCs.Count;
				
				if(totalNPCs >= Settings.General.MaxGlobalNpcGrids){
					
					return "Spawning Aborted. Max Global NPCs Limit Reached.";
					
				}
				
			}
			
			if(NPCWatcher.ActiveNpcTypeLimitReachedForArea("RandomEncounter", startCoords, Settings.RandomEncounters.MaxShipsPerArea, Settings.RandomEncounters.AreaSize) == true){
				
				return "Too Many Random Encounter Grids in Player Area";
				
			}

			KnownPlayerLocationManager.CleanExpiredLocations();
			var validFactions = new Dictionary<string, List<string>>();
			var spawnGroupList = GetRandomEncounters(startCoords, out validFactions);
			
			if(Settings.General.UseModIdSelectionForSpawning == true){
				
				spawnGroupList = SpawnResources.SelectSpawnGroupSublist(SpawnGroupSublists, EligibleSpawnsByModId);
				
			}

			if(spawnGroupList.Count == 0){
				
				return "No Eligible Spawn Groups Could Be Found To Spawn Near Player.";
				
			}
			
			var spawnGroup = spawnGroupList[SpawnResources.rnd.Next(0, spawnGroupList.Count)];
			Vector3D spawnCoords = Vector3D.Zero;
			
			if(GetSpawnCoords(spawnGroup, startCoords, out spawnCoords) == false){
				
				return "Could Not Find Safe Position To Spawn Encounter";
				
			}

			//Get Directions
			var spawnMatrix = MatrixD.CreateWorld(spawnCoords);
			var successfulVoxelSpawn = false;
			var centerVoxelOffset = false;
			
			foreach(var voxel in spawnGroup.SpawnGroup.Voxels){
				
				spawnGroup.RotateFirstCockpitToForward = false;
				var voxelSpawningPosition = Vector3D.Transform((Vector3D)voxel.Offset, spawnMatrix);
				
				
				if(voxel.CenterOffset == true){
					
					voxelSpawningPosition = spawnCoords;
					
					try{

						var offsetCoords = Vector3D.Transform((Vector3D)voxel.Offset, spawnMatrix);
						var voxelSpawn = MyAPIGateway.Session.VoxelMaps.CreateVoxelMapFromStorageName(voxel.StorageName, voxel.StorageName, offsetCoords);
						var newVoxelMatrix = voxelSpawn.WorldMatrix;
						spawnMatrix.Translation = voxelSpawn.PositionComp.WorldAABB.Center;
						//newVoxelMatrix.Translation = voxelSpawningPosition;
						//voxelSpawn.SetWorldMatrix(newVoxelMatrix);

						Logger.CreateDebugGPS("Original SpawnCoords", voxelSpawningPosition);
						Logger.CreateDebugGPS("Original Translation", newVoxelMatrix.Translation);
						Logger.CreateDebugGPS("Original BottomCorner", voxelSpawn.PositionLeftBottomCorner);
						Logger.CreateDebugGPS("Original BBCenter", voxelSpawn.PositionComp.WorldAABB.Center);




						if(Settings.RandomEncounters.RemoveVoxelsIfGridRemoved == true && spawnGroup.RemoveVoxelsIfGridRemoved == true) {

							NPCWatcher.SpawnedVoxels.Add(voxelSpawn.EntityId.ToString(), voxelSpawn as IMyEntity);

						}

						successfulVoxelSpawn = true;
						centerVoxelOffset = true;
						
						
					}catch(Exception exc){
						
						Logger.AddMsg("Manual Voxel Spawning For " + voxel.StorageName + " Failed");
						
					}
					
				}else{
					
					try{
						
						var voxelSpawn = MyAPIGateway.Session.VoxelMaps.CreateVoxelMapFromStorageName(voxel.StorageName, voxel.StorageName, voxelSpawningPosition);

						if(Settings.RandomEncounters.RemoveVoxelsIfGridRemoved == true && spawnGroup.RemoveVoxelsIfGridRemoved == true) {

							NPCWatcher.SpawnedVoxels.Add(voxelSpawn.EntityId.ToString(), voxelSpawn as IMyEntity);

						}

						successfulVoxelSpawn = true;
						
					}catch(Exception exc){
						
						Logger.AddMsg("Voxel Spawning For " + voxel.StorageName + " Failed");
						
					}
					
				}
			
			}
			
			if(successfulVoxelSpawn == true){
				
				var voxelIdList = new List<string>(NPCWatcher.SpawnedVoxels.Keys.ToList());
				string[] voxelIdArray = voxelIdList.ToArray();
				MyAPIGateway.Utilities.SetVariable<string[]>("MES-SpawnedVoxels", voxelIdArray);
				
			}

			long gridOwner = 0;
			var randFactionTag = spawnGroup.FactionOwner;

			if(validFactions.ContainsKey(spawnGroup.SpawnGroupName)) {

				randFactionTag = validFactions[spawnGroup.SpawnGroupName][SpawnResources.rnd.Next(0, validFactions[spawnGroup.SpawnGroupName].Count)];

			}

			if(NPCWatcher.NPCFactionTagToFounder.ContainsKey(randFactionTag) == true) {

				gridOwner = NPCWatcher.NPCFactionTagToFounder[randFactionTag];

			} else {

				Logger.AddMsg("Could Not Find Faction Founder For: " + randFactionTag);

			}

			foreach(var prefab in spawnGroup.SpawnGroup.Prefabs){

				if (spawnGroup.UseKnownPlayerLocations) {

					KnownPlayerLocationManager.IncreaseSpawnCountOfLocations(startCoords);

				}

				var options = SpawnGroupManager.CreateSpawningOptions(spawnGroup, prefab);
				var spawnPosition = Vector3D.Transform((Vector3D)prefab.Position, spawnMatrix);
				Logger.CreateDebugGPS("Prefab Spawn Coords", spawnPosition);
				var speedL = Vector3.Zero;
				var speedA = Vector3.Zero;
				var gridList = new List<IMyCubeGrid>();
				
				//Grid Manipulation
				GridBuilderManipulation.ProcessPrefabForManipulation(prefab.SubtypeId, spawnGroup, "RandomEncounter", prefab.Behaviour);
				
				try{
					
					MyAPIGateway.PrefabManager.SpawnPrefab(gridList, prefab.SubtypeId, spawnPosition, spawnMatrix.Forward, spawnMatrix.Up, speedL, speedA, prefab.BeaconText, options, gridOwner);
					
				}catch(Exception exc){
					
					
					
				}
				
				var pendingNPC = new ActiveNPC();
				pendingNPC.SpawnGroupName = spawnGroup.SpawnGroupName;
				pendingNPC.SpawnGroup = spawnGroup;
				pendingNPC.InitialFaction = randFactionTag;
				pendingNPC.faction = MyAPIGateway.Session.Factions.TryGetFactionByTag(pendingNPC.InitialFaction);
				pendingNPC.Name = prefab.SubtypeId;
				pendingNPC.GridName = MyDefinitionManager.Static.GetPrefabDefinition(prefab.SubtypeId).CubeGrids[0].DisplayName;
				pendingNPC.StartCoords = spawnCoords;
				pendingNPC.CurrentCoords = spawnCoords;
				pendingNPC.EndCoords = spawnCoords;
				pendingNPC.SpawnType = "RandomEncounter";
				pendingNPC.CleanupIgnore = spawnGroup.IgnoreCleanupRules;
				pendingNPC.ForceStaticGrid = spawnGroup.ForceStaticGrid;
				pendingNPC.KeenAiName = prefab.Behaviour;
				pendingNPC.KeenAiTriggerDistance = prefab.BehaviourActivationDistance;
				
				if(spawnGroup.RandomizeWeapons == true){
						
					pendingNPC.ReplenishedSystems = false;
					pendingNPC.ReplacedWeapons = true;
					
				}else if((MES_SessionCore.NPCWeaponUpgradesModDetected == true || Settings.General.EnableGlobalNPCWeaponRandomizer == true) && spawnGroup.IgnoreWeaponRandomizerMod == false){
				
					pendingNPC.ReplenishedSystems = false;
					pendingNPC.ReplacedWeapons = true;
					
				}else if(spawnGroup.ReplenishSystems == true){
					
					pendingNPC.ReplenishedSystems = false;
					
				}
				
				NPCWatcher.PendingNPCs.Add(pendingNPC);
				
			}
			
			Logger.SkipNextMessage = false;
			return "Spawning Group - " + spawnGroup.SpawnGroup.Id.SubtypeName;
			
		}
		
		public static List<ImprovedSpawnGroup> GetRandomEncounters(Vector3D playerCoords, out Dictionary<string, List<string>> validFactions) {
			
			MyPlanet planet = SpawnResources.GetNearestPlanet(playerCoords);
			string specificGroup = "";
			validFactions = new Dictionary<string, List<string>>();
			SpawnGroupSublists.Clear();
			EligibleSpawnsByModId.Clear();
			
			bool specificSpawnRequest = false;
			
			if(SpawnGroupManager.AdminSpawnGroup != ""){
				
				specificGroup = SpawnGroupManager.AdminSpawnGroup;
				SpawnGroupManager.AdminSpawnGroup = "";
				specificSpawnRequest = true;
				
			}
			
			if(SpawnResources.IsPositionInGravity(playerCoords, planet) == true){
				
				return new List<ImprovedSpawnGroup>();
				
			}
			
			var eligibleGroups = new List<ImprovedSpawnGroup>();
			
			//Filter Eligible Groups To List
			foreach(var spawnGroup in SpawnGroupManager.SpawnGroups){
				
				if(specificGroup != "" && spawnGroup.SpawnGroup.Id.SubtypeName != specificGroup){
					
					continue;
					
				}
				
				if(specificGroup == "" && spawnGroup.AdminSpawnOnly == true){
					
					continue;
					
				}
				
				if(spawnGroup.SpaceRandomEncounter == false){
					
					continue;
					
				}
				
				if(SpawnResources.CheckCommonConditions(spawnGroup, playerCoords, planet, specificSpawnRequest) == false){
					
					continue;
					
				}

				var validFactionsList = SpawnResources.ValidNpcFactions(spawnGroup, playerCoords);

				if(validFactionsList.Count == 0) {

					continue;

				}

				if(validFactions.ContainsKey(spawnGroup.SpawnGroupName) == false) {

					validFactions.Add(spawnGroup.SpawnGroupName, validFactionsList);

				}

				if(spawnGroup.Frequency > 0){
					
					string modID = spawnGroup.SpawnGroup.Context.ModId;
					
					if(string.IsNullOrEmpty(modID) == true){
						
						modID = "KeenSWH";
						
					}
					
					if(SpawnGroupSublists.ContainsKey(modID) == false){
						
						SpawnGroupSublists.Add(modID, new List<ImprovedSpawnGroup>());
						
					}
					
					if(EligibleSpawnsByModId.ContainsKey(modID) == false){
						
						EligibleSpawnsByModId.Add(modID, 1);
						
					}else{
						
						EligibleSpawnsByModId[modID] += 1;
						
					}
					
					if(Settings.RandomEncounters.UseMaxSpawnGroupFrequency == true && spawnGroup.Frequency > Settings.RandomEncounters.MaxSpawnGroupFrequency * 10){
						
						spawnGroup.Frequency = (int)Math.Round((double)Settings.RandomEncounters.MaxSpawnGroupFrequency * 10);
						
					}
					
					for(int i = 0; i < spawnGroup.Frequency; i++){
						
						eligibleGroups.Add(spawnGroup);
						SpawnGroupSublists[modID].Add(spawnGroup);
						
					}
					
				}
				
			}
			
			return eligibleGroups;
			
		}
		
		public static bool GetSpawnCoords(ImprovedSpawnGroup spawnGroup, Vector3D startCoords, out Vector3D spawnCoords){
			
			spawnCoords = Vector3D.Zero;
			SpawnResources.RefreshEntityLists();
			MyPlanet planet = SpawnResources.GetNearestPlanet(spawnCoords);
			
			for(int i = 0; i < Settings.RandomEncounters.SpawnAttempts; i++){
				
				var spawnDir = Vector3D.Normalize(MyUtils.GetRandomVector3D());
				var randDist = (double)SpawnResources.rnd.Next((int)Settings.RandomEncounters.MinSpawnDistanceFromPlayer, (int)Settings.RandomEncounters.MaxSpawnDistanceFromPlayer);
				var tempSpawnCoords = spawnDir * randDist + startCoords;
				
				if(SpawnResources.IsPositionInGravity(tempSpawnCoords, planet) == true){
					
					spawnDir *= -1;
					tempSpawnCoords = spawnDir * randDist + startCoords;
					
					if(SpawnResources.IsPositionInGravity(tempSpawnCoords, planet) == true){
						
						continue;
						
					}
					
				}
				
				var tempMatrix = MatrixD.CreateWorld(tempSpawnCoords);
				var badPath = false;
				
				foreach(var prefab in spawnGroup.SpawnGroup.Prefabs){
										
					var prefabCoords = Vector3D.Transform((Vector3D)prefab.Position, tempMatrix);
					planet = SpawnResources.GetNearestPlanet(prefabCoords);
					
					foreach(var entity in SpawnResources.EntityList){
						
						if(Vector3D.Distance(entity.GetPosition(), prefabCoords) < Settings.RandomEncounters.MinDistanceFromOtherEntities){
							
							badPath = true;
							break;
							
						}
						
					}

					if(SpawnResources.IsPositionInSafeZone(prefabCoords) == true || SpawnResources.IsPositionInGravity(prefabCoords, planet) == true){
						
						badPath = true;
						break;
						
					}
					
					if(badPath == true){
							
						break;
						
					}
					
				}
				
				if(badPath == true){
					
					continue;
					
				}
				
				spawnCoords = tempSpawnCoords;
				return true;
				
			}

			return false;
			
		}
			
	}
	
}