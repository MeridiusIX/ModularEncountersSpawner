using ModularEncountersSpawner.Configuration;
using ModularEncountersSpawner.Manipulation;
using ModularEncountersSpawner.Templates;
using ModularEncountersSpawner.World;
using ModularEncountersSpawner.Zones;
using Sandbox.Definitions;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.Utils;
using VRageMath;

namespace ModularEncountersSpawner.Spawners {

	public static class RandomEncounterSpawner{
		
		public static Dictionary<string, List<ImprovedSpawnGroup>> SpawnGroupSublists = new Dictionary<string, List<ImprovedSpawnGroup>>();
		public static Dictionary<string, int> EligibleSpawnsByModId = new Dictionary<string, int>();
		
		public static string AttemptSpawn(Vector3D startCoords, List<string> eligibleNames = null) {

			if (!MES_SessionCore.SpawnerEnabled) {

				return "Spawner Disabled Due To Incompatible World Settings (Selective Physics Updates)";

			}

			if (Settings.General.UseMaxNpcGrids == true){
				
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
			var spawnGroupList = GetRandomEncounters(startCoords, eligibleNames, out validFactions);
			
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
				IMyVoxelMap voxelSpawn = null;

				try {

					voxelSpawn = MyAPIGateway.Session.VoxelMaps.CreateVoxelMapFromStorageName(voxel.StorageName, voxel.StorageName, voxelSpawningPosition);

					if (Settings.RandomEncounters.RemoveVoxelsIfGridRemoved == true && spawnGroup.RemoveVoxelsIfGridRemoved == true) {

						NPCWatcher.SpawnedVoxels.Add(voxelSpawn.EntityId.ToString(), voxelSpawn as IMyEntity);

					}

					successfulVoxelSpawn = true;

				} catch (Exception exc) {

					Logger.AddMsg("Voxel Spawning For " + voxel.StorageName + " Failed");
					continue;

				}

				if (voxel.CenterOffset && !centerVoxelOffset) {

					centerVoxelOffset = true;
					var oldVoxelTranslation = voxelSpawn.WorldMatrix.Translation;
					var center = voxelSpawn.PositionComp.WorldAABB.Center;
					var corner = voxelSpawn.PositionLeftBottomCorner;
					var centerCornerHalf = Vector3D.Normalize(corner - center) * (Vector3D.Distance(corner, center) / 2) + center;
					var newMatrix = MatrixD.CreateTranslation(centerCornerHalf/* + voxelSpawn.Storage.Size / 2*/);
					voxelSpawn.SetWorldMatrix(newMatrix);
					var newSpawnCoords = Vector3D.Normalize(centerCornerHalf - spawnMatrix.Translation) * (Vector3D.Distance(centerCornerHalf, spawnMatrix.Translation) / 4) + spawnMatrix.Translation;
					spawnMatrix = MatrixD.CreateWorld(newSpawnCoords, Vector3D.Forward, Vector3D.Up);
					

					Logger.CreateDebugGPS("Old Voxel Translation", oldVoxelTranslation);
					Logger.CreateDebugGPS("Center", center);
					Logger.CreateDebugGPS("Corner", corner);
					Logger.CreateDebugGPS("Center-Corner-Half", centerCornerHalf);
					Logger.CreateDebugGPS("NewMatrix", newMatrix.Translation);
					Logger.CreateDebugGPS("SpawnMatrix", spawnMatrix.Translation);

					//spawnMatrix.Translation = voxelSpawn.PositionLeftBottomCorner;

					//spawnMatrix.Translation = center;

				}

			}

			if (successfulVoxelSpawn == true){
				
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


			SpawnResources.ApplySpawningCosts(spawnGroup, randFactionTag);


			foreach (var prefab in spawnGroup.SpawnGroup.Prefabs){

				if (spawnGroup.UseKnownPlayerLocations) {

					KnownPlayerLocationManager.IncreaseSpawnCountOfLocations(startCoords, randFactionTag);

				}

				var options = SpawnGroupManager.CreateSpawningOptions(spawnGroup, prefab);
				var prefabPos = prefab.Position;
				prefabPos.Z *= centerVoxelOffset ? 1 : 1;
				var spawnPosition = Vector3D.Transform((Vector3D)prefabPos, spawnMatrix);

				Logger.CreateDebugGPS("Prefab Spawn Coords", spawnPosition);
				var speedL = Vector3.Zero;
				var speedA = Vector3.Zero;
				var gridList = new List<IMyCubeGrid>();
				
				//Grid Manipulation
				ManipulationCore.ProcessPrefabForManipulation(prefab.SubtypeId, spawnGroup, "RandomEncounter", prefab.Behaviour);
				
				try{
					
					MyAPIGateway.PrefabManager.SpawnPrefab(gridList, prefab.SubtypeId, spawnPosition, spawnMatrix.Forward, spawnMatrix.Up, speedL, speedA, !string.IsNullOrWhiteSpace(prefab.BeaconText) ? prefab.BeaconText : null, options, gridOwner);
					
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
				pendingNPC.EndCoords = Vector3D.Zero;
				pendingNPC.SpawnType = "RandomEncounter";
				pendingNPC.CleanupIgnore = spawnGroup.IgnoreCleanupRules;
				pendingNPC.ForceStaticGrid = spawnGroup.ForceStaticGrid;
				pendingNPC.KeenAiName = prefab.Behaviour;
				pendingNPC.KeenAiTriggerDistance = prefab.BehaviourActivationDistance;

				TimeoutManagement.ApplySpawnTimeoutToZones(SpawnType.RandomEncounter, spawnPosition);

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

			if (spawnGroup.UniqueEncounter == true) {

				SpawnGroupManager.UniqueGroupsSpawned.Add(spawnGroup.SpawnGroup.Id.SubtypeName);
				string[] uniqueSpawnedArray = SpawnGroupManager.UniqueGroupsSpawned.ToArray();
				MyAPIGateway.Utilities.SetVariable<string[]>("MES-UniqueGroupsSpawned", uniqueSpawnedArray);

			}

			Logger.SkipNextMessage = false;
			return "Spawning Group - " + spawnGroup.SpawnGroup.Id.SubtypeName;
			
		}
		
		public static List<ImprovedSpawnGroup> GetRandomEncounters(Vector3D playerCoords, List<string> eligibleNames, out Dictionary<string, List<string>> validFactions) {
			
			MyPlanet planet = SpawnResources.GetNearestPlanet(playerCoords);
			string specificGroup = "";
			validFactions = new Dictionary<string, List<string>>();
			SpawnGroupSublists.Clear();
			EligibleSpawnsByModId.Clear();
			var environment = new EnvironmentEvaluation(playerCoords);

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
			SpawnResources.SandboxVariableCache.Clear();

			//Filter Eligible Groups To List
			foreach (var spawnGroup in SpawnGroupManager.SpawnGroups){

				if (eligibleNames != null) {

					if (!eligibleNames.Contains(spawnGroup.SpawnGroupName)) {

						continue;

					}

				} else {

					if (specificGroup != "" && spawnGroup.SpawnGroup.Id.SubtypeName != specificGroup) {

						continue;

					}

					if (specificGroup == "" && spawnGroup.AdminSpawnOnly == true) {

						continue;

					}

				}

				if (spawnGroup.SpaceRandomEncounter == false){
					
					continue;
					
				}

				if (spawnGroup.RandomEncounterChance < spawnGroup.ChanceCeiling && !specificSpawnRequest) {

					var roll = SpawnResources.rnd.Next(0, spawnGroup.ChanceCeiling + 1);

					if (roll > spawnGroup.RandomEncounterChance)
						continue;

				}

				if (SpawnResources.CheckCommonConditions(spawnGroup, playerCoords, environment, specificSpawnRequest, SpawnType.RandomEncounter) == false){
					
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