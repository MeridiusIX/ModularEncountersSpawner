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
using ModularEncountersSpawner.Api;

namespace ModularEncountersSpawner.Spawners{
	
	public static class PlanetaryInstallationSpawner{
		
		public static Dictionary<string, List<ImprovedSpawnGroup>> SpawnGroupSublists = new Dictionary<string, List<ImprovedSpawnGroup>>();
		public static Dictionary<string, List<ImprovedSpawnGroup>> SmallSpawnGroupSublists = new Dictionary<string, List<ImprovedSpawnGroup>>();
		public static Dictionary<string, List<ImprovedSpawnGroup>> MediumSpawnGroupSublists = new Dictionary<string, List<ImprovedSpawnGroup>>();
		public static Dictionary<string, List<ImprovedSpawnGroup>> LargeSpawnGroupSublists = new Dictionary<string, List<ImprovedSpawnGroup>>();
		
		public static Dictionary<string, int> EligibleSpawnsByModId = new Dictionary<string, int>();
		public static Dictionary<string, int> EligibleSmallSpawnsByModId = new Dictionary<string, int>();
		public static Dictionary<string, int> EligibleMediumSpawnsByModId = new Dictionary<string, int>();
		public static Dictionary<string, int> EligibleLargeSpawnsByModId = new Dictionary<string, int>();
		
		public static string AttemptSpawn(Vector3D startCoords, IMyPlayer player, List<string> eligibleNames = null) {

			if (!MES_SessionCore.SpawnerEnabled) {

				return "Spawner Disabled Due To Incompatible World Settings (Selective Physics Updates)";

			}

			if (Settings.General.UseMaxNpcGrids == true){
				
				var totalNPCs = NPCWatcher.ActiveNPCs.Count;
				
				if(totalNPCs >= Settings.General.MaxGlobalNpcGrids){
					
					return "Spawning Aborted. Max Global NPCs Limit Reached.";
					
				}
				
			}
			
			if(NPCWatcher.ActiveNpcTypeLimitReachedForArea("PlanetaryInstallation", startCoords, Settings.PlanetaryInstallations.MaxShipsPerArea, Settings.PlanetaryInstallations.AreaSize) == true){
				
				return "Too Many Planetary Installation Grids in Player Area";
				
			}
			
			MyPlanet planet = SpawnResources.GetNearestPlanet(startCoords);
			
			if(planet == null){
				
				return "No Planets In Game World Found.";
				
			}else{
				
				if(SpawnResources.GetDistanceFromSurface(startCoords, planet) > Settings.PlanetaryInstallations.PlayerMaximumDistanceFromSurface || SpawnResources.IsPositionInGravity(startCoords, planet) == false){
					
					return "Player Not In Planet Gravity Or Too Far From Surface.";
					
				}
				
			}
			
			var planetEntity = planet as IMyEntity;

			if (player != null) {

				if (MES_SessionCore.playerWatchList.ContainsKey(player) == true) {

					var playerSurface = SpawnResources.GetNearestSurfacePoint(player.GetPosition(), planet);

					if (MES_SessionCore.playerWatchList[player].InstallationDistanceCoordCheck == Vector3D.Zero) {

						MES_SessionCore.playerWatchList[player].InstallationDistanceCoordCheck = playerSurface;
						return "New Player Detected. Storing Position On Planet.";

					}

					if (Vector3D.Distance(MES_SessionCore.playerWatchList[player].InstallationDistanceCoordCheck, playerSurface) < Settings.PlanetaryInstallations.PlayerDistanceSpawnTrigger) {

						Logger.AddMsg("Player Travelled: " + Vector3D.Distance(MES_SessionCore.playerWatchList[player].InstallationDistanceCoordCheck, playerSurface) + " Distance From Last Saved Position.");
						return "Player Hasn't Traveled Far Enough Yet.";

					}

					MES_SessionCore.playerWatchList[player].InstallationDistanceCoordCheck = playerSurface;

				} else {

					return "Player Not In Watcher List... Although They Probably Should Be If The Script Got Here.";

				}

			}

			KnownPlayerLocationManager.CleanExpiredLocations();
			var smallStations = new List<ImprovedSpawnGroup>();
			var mediumStations = new List<ImprovedSpawnGroup>();
			var largeStations = new List<ImprovedSpawnGroup>();
			var validFactions = new Dictionary<string, List<string>>();
			var spawnGroupList = GetPlanetaryInstallations(startCoords, eligibleNames, out smallStations, out mediumStations, out largeStations, out validFactions);
			
			if(Settings.General.UseModIdSelectionForSpawning == true){
				
				spawnGroupList = SpawnResources.SelectSpawnGroupSublist(SpawnGroupSublists, EligibleSpawnsByModId);
				smallStations = SpawnResources.SelectSpawnGroupSublist(SmallSpawnGroupSublists, EligibleSmallSpawnsByModId);
				mediumStations = SpawnResources.SelectSpawnGroupSublist(MediumSpawnGroupSublists, EligibleMediumSpawnsByModId);
				largeStations = SpawnResources.SelectSpawnGroupSublist(LargeSpawnGroupSublists, EligibleLargeSpawnsByModId);
				
			}

			if(spawnGroupList.Count == 0){
				
				return "No Eligible Spawn Groups Could Be Found To Spawn Near Player.";
				
			}
			
			Logger.AddMsg("Found " + (spawnGroupList.Count / 10).ToString() + " Potential Spawn Groups. Small: " + (smallStations.Count / 10).ToString() + " // Medium: " + (mediumStations.Count / 10).ToString() + " // Large: " + (largeStations.Count / 10).ToString(), true);
			
			string stationSize = "Small";
			spawnGroupList = smallStations;
			
			bool skippedAbsentSmall = false;
			bool skippedAbsentMedium = false;
			bool skippedAbsentLarge = false;
			
			//Start With Small Station Always, Try Chance For Medium.
			if(stationSize == "Small" && smallStations.Count == 0){
				
				//No Small Stations Available For This Area, So Try Medium.
				skippedAbsentSmall = true;
				stationSize = "Medium";
				spawnGroupList = mediumStations;
				
			}else if(stationSize == "Small" && smallStations.Count != 0){
				
				int mediumChance = 0;
				string varName = "MES-" + planetEntity.EntityId.ToString() + "-Medium";
				
				if(MyAPIGateway.Utilities.GetVariable<int>(varName, out mediumChance) == false){
					
					mediumChance = Settings.PlanetaryInstallations.MediumSpawnChanceBaseValue;
					MyAPIGateway.Utilities.SetVariable<int>(varName, mediumChance);
					
				}
				
				if(SpawnResources.rnd.Next(0, 100) < mediumChance){
					
					stationSize = "Medium";
					spawnGroupList = mediumStations;
					
				}
				
			}
			
			if(stationSize == "Medium" && mediumStations.Count == 0){
				
				//No Medium Stations Available For This Area, So Try Large.
				skippedAbsentMedium = true;
				stationSize = "Large";
				spawnGroupList = largeStations;
				
			}else if(stationSize == "Medium" && mediumStations.Count != 0){
				
				int largeChance = 0;
				string varName = "MES-" + planetEntity.EntityId.ToString() + "-Large";
				
				if(MyAPIGateway.Utilities.GetVariable<int>(varName, out largeChance) == false){
					
					largeChance = Settings.PlanetaryInstallations.LargeSpawnChanceBaseValue;
					MyAPIGateway.Utilities.SetVariable<int>(varName, largeChance);
					
				}
				
				if(SpawnResources.rnd.Next(0, 100) < largeChance){
					
					stationSize = "Large";
					spawnGroupList = largeStations;
					
				}
				
			}
			
			if(stationSize == "Large" && largeStations.Count == 0){
				
				skippedAbsentLarge = true;
				stationSize = "Medium";
				spawnGroupList = mediumStations;
				
				if(mediumStations.Count == 0){
					
					skippedAbsentMedium = true;
					stationSize = "Small";
					spawnGroupList = smallStations;
					
				}
				
			}
			
			if(spawnGroupList.Count == 0){
				
				return "Could Not Find Station Of Suitable Size For This Spawn Instance.";
				
			}
			
			var spawnGroup = spawnGroupList[SpawnResources.rnd.Next(0, spawnGroupList.Count)];
			Vector3D spawnCoords = Vector3D.Zero;
			Logger.StartTimer();
			
			if(GetSpawnCoords(spawnGroup, startCoords, out spawnCoords) == false){
				
				Logger.AddMsg("Planetary Installation Spawn Coord Calculation Time: " + Logger.StopTimer(), true);
				return "Could Not Find Safe Position To Spawn " + stationSize + " Installation.";
				
			}
			
			Logger.AddMsg("Planetary Installation Spawn Coord Calculation Time: " + Logger.StopTimer(), true);

			//Get Directions
			var upDir = Vector3D.Normalize(spawnCoords - planetEntity.GetPosition());
			var forwardDir = Vector3D.CalculatePerpendicularVector(upDir);
			var spawnMatrix = MatrixD.CreateWorld(spawnCoords, forwardDir, upDir);
			var successfulVoxelSpawn = false;
			
			foreach(var voxel in spawnGroup.SpawnGroup.Voxels){
				
				var voxelSpawningPosition = Vector3D.Transform((Vector3D)voxel.Offset, spawnMatrix);
				
				try{
					
					var voxelSpawn = MyAPIGateway.Session.VoxelMaps.CreateVoxelMapFromStorageName(voxel.StorageName, voxel.StorageName, voxelSpawningPosition);

					if(Settings.PlanetaryInstallations.RemoveVoxelsIfGridRemoved == true && spawnGroup.RemoveVoxelsIfGridRemoved == true) {

						NPCWatcher.SpawnedVoxels.Add(voxelSpawn.EntityId.ToString(), voxelSpawn as IMyEntity);

					}

					successfulVoxelSpawn = true;
					
				}catch(Exception exc){
					
					Logger.AddMsg("Voxel Spawning For " + voxel.StorageName + " Failed");
					
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

			SpawnResources.ApplySpawningCosts(spawnGroup, randFactionTag);

			for (int i = 0; i < spawnGroup.SpawnGroup.Prefabs.Count; i++){

				if (spawnGroup.UseKnownPlayerLocations) {

					KnownPlayerLocationManager.IncreaseSpawnCountOfLocations(startCoords, randFactionTag);

				}

				var prefab = spawnGroup.SpawnGroup.Prefabs[i];
				var options = SpawnGroupManager.CreateSpawningOptions(spawnGroup, prefab);				
				var spawnPosition = Vector3D.Transform((Vector3D)prefab.Position, spawnMatrix);
				
				//Realign to Terrain
				var offsetSurfaceCoords = SpawnResources.GetNearestSurfacePoint(spawnPosition, planet);

				if (MES_SessionCore.Instance.WaterMod.Registered && spawnGroup.InstallationSpawnsOnWaterSurface) {

					for (int j = MES_SessionCore.Instance.WaterMod.Waters.Count - 1; j >= 0; j++) {

						if (j >= MES_SessionCore.Instance.WaterMod.Waters.Count)
							continue;

						var water = MES_SessionCore.Instance.WaterMod.Waters[j];

						if (water.planetID != planet.EntityId)
							continue;

						if (water.IsUnderwater(offsetSurfaceCoords)) {

							offsetSurfaceCoords = water.GetClosestSurfacePoint(offsetSurfaceCoords);

						}

						break;

					}

				}
				
				var offsetSurfaceMatrix = MatrixD.CreateWorld(offsetSurfaceCoords, forwardDir, upDir);
				var finalCoords = Vector3D.Transform(new Vector3D(0, (double)prefab.Position.Y, 0), offsetSurfaceMatrix);
				
				var newForward = offsetSurfaceMatrix.Forward;
				var newUp = offsetSurfaceMatrix.Up;

				GetReversedForwardDirections(spawnGroup, i, ref newForward);
				GetDerelictDirections(spawnGroup, i, finalCoords, ref newForward, ref newUp);
				
				var speedL = Vector3.Zero;
				var speedA = Vector3.Zero;
				var gridList = new List<IMyCubeGrid>();
				
				//Grid Manipulation
				GridBuilderManipulation.ProcessPrefabForManipulation(prefab.SubtypeId, spawnGroup, "PlanetaryInstallation", prefab.Behaviour);
				
				try{
					
					MyAPIGateway.PrefabManager.SpawnPrefab(gridList, prefab.SubtypeId, finalCoords, newForward, newUp, speedL, speedA, !string.IsNullOrWhiteSpace(prefab.BeaconText) ? prefab.BeaconText : null, options, gridOwner);
					
				}catch(Exception exc){
					
					
					
				}

				Logger.AddMsg("Installation Forward Vector: " + newForward.ToString(), true);
				var pendingNPC = new ActiveNPC();
				pendingNPC.SpawnGroup = spawnGroup;
				pendingNPC.SpawnGroupName = spawnGroup.SpawnGroupName;
				pendingNPC.InitialFaction = randFactionTag;
				pendingNPC.faction = MyAPIGateway.Session.Factions.TryGetFactionByTag(pendingNPC.InitialFaction);
				pendingNPC.Name = prefab.SubtypeId;
				pendingNPC.GridName = MyDefinitionManager.Static.GetPrefabDefinition(prefab.SubtypeId).CubeGrids[0].DisplayName;
				pendingNPC.StartCoords = finalCoords;
				pendingNPC.CurrentCoords = finalCoords;
				pendingNPC.EndCoords = finalCoords;
				pendingNPC.SpawnType = "PlanetaryInstallation";
				pendingNPC.CleanupIgnore = spawnGroup.IgnoreCleanupRules;
				pendingNPC.ForceStaticGrid = spawnGroup.ForceStaticGrid;
				pendingNPC.KeenAiName = prefab.Behaviour;
				pendingNPC.KeenAiTriggerDistance = prefab.BehaviourActivationDistance;
				
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
			
			if(spawnGroup.PlanetaryInstallationType == "Small"){
				
				int mediumChance = 0;
				string varName = "MES-" + planetEntity.EntityId.ToString() + "-Medium";
				if(MyAPIGateway.Utilities.GetVariable<int>(varName, out mediumChance) == true){
					
					mediumChance += Settings.PlanetaryInstallations.MediumSpawnChanceIncrement;
					MyAPIGateway.Utilities.SetVariable<int>(varName, mediumChance);
					
				}
				
				Logger.AddMsg("Medium Installation Spawning Chance Now Set To: " + mediumChance.ToString() + " / 100", true);
				
			}
			
			if(spawnGroup.PlanetaryInstallationType == "Medium" || skippedAbsentMedium == true){
				
				int mediumChance = 0;
				string varName = "MES-" + planetEntity.EntityId.ToString() + "-Medium";
				if(MyAPIGateway.Utilities.GetVariable<int>(varName, out mediumChance) == true){
					
					mediumChance = Settings.PlanetaryInstallations.MediumSpawnChanceBaseValue;
					MyAPIGateway.Utilities.SetVariable<int>(varName, mediumChance);
					
				}
				
				Logger.AddMsg("Medium Installation Spawning Chance Now Set To: " + mediumChance.ToString() + " / 100", true);
				
				int largeChance = 0;
				varName = "MES-" + planetEntity.EntityId.ToString() + "-Large";
				if(MyAPIGateway.Utilities.GetVariable<int>(varName, out largeChance) == true){
					
					largeChance += Settings.PlanetaryInstallations.LargeSpawnChanceIncrement;
					MyAPIGateway.Utilities.SetVariable<int>(varName, largeChance);
					
				}
				
				Logger.AddMsg("Large Installation Spawning Chance Now Set To: " + largeChance.ToString() + " / 100", true);
				
			}
			
			if(spawnGroup.PlanetaryInstallationType == "Large" || skippedAbsentLarge == true){
				
				int largeChance = 0;
				string varName = "MES-" + planetEntity.EntityId.ToString() + "-Large";
				if(MyAPIGateway.Utilities.GetVariable<int>(varName, out largeChance) == true){
					
					largeChance = Settings.PlanetaryInstallations.LargeSpawnChanceBaseValue;
					MyAPIGateway.Utilities.SetVariable<int>(varName, largeChance);
					
				}
				
				Logger.AddMsg("Large Installation Spawning Chance Now Set To: " + largeChance.ToString() + " / 100", true);
				
			}
			
			if(spawnGroup.UniqueEncounter == true){
				
				SpawnGroupManager.UniqueGroupsSpawned.Add(spawnGroup.SpawnGroup.Id.SubtypeName);
				string[] uniqueSpawnedArray = SpawnGroupManager.UniqueGroupsSpawned.ToArray();
				MyAPIGateway.Utilities.SetVariable<string[]>("MES-UniqueGroupsSpawned", uniqueSpawnedArray);
				
			}
			
			Logger.SkipNextMessage = false;
			return "Spawning Group - " + spawnGroup.SpawnGroup.Id.SubtypeName;
			
		}
		
		public static void GetDerelictDirections(ImprovedSpawnGroup spawnGroup, int index, Vector3D coords, ref Vector3D forward, ref Vector3D up){
			
			if(index >= spawnGroup.RotateInstallations.Count) {

				Logger.AddMsg("Installation Prefab Index Higher Than Rotation List Count", true);
				return;

			}

			var rotationData = spawnGroup.RotateInstallations[index];

			//Do Not Rotate
			if(rotationData == Vector3D.Zero) {

				Logger.AddMsg("Installation Rotation Data is Zero", true);
				return;

			}

			//Produce Random Rotation
			if(rotationData == new Vector3D(100, 100, 100)) {

				Logger.AddMsg("Installation Rotation Randomized", true);
				double x = SpawnResources.rnd.Next(-100, 110);
				double y = SpawnResources.rnd.Next(-100, 110);
				double z = SpawnResources.rnd.Next(-100, 110);
				var rotationVector = new Vector3D(x / 10.0, y / 10.0, z / 10.0);
				var tempMatrix = MatrixD.CreateWorld(Vector3D.Zero, forward, up);
				var rotationMatrix = CalculateDerelictSpawnMatrix(tempMatrix, rotationVector);
				forward = rotationMatrix.Forward;
				up = rotationMatrix.Up;
				return;

			} else {

				Logger.AddMsg("Installation Rotation Set To Preset", true);
				var tempMatrix = MatrixD.CreateWorld(Vector3D.Zero, forward, up);
				var rotationMatrix = CalculateDerelictSpawnMatrix(tempMatrix, rotationData);
				forward = rotationMatrix.Forward;
				up = rotationMatrix.Up;

			}


		}

		public static void GetReversedForwardDirections(ImprovedSpawnGroup spawnGroup, int index, ref Vector3D forward) {

			if(index >= spawnGroup.ReverseForwardDirections.Count) {

				Logger.AddMsg("Installation Prefab Index Higher Than Rotation List Count", true);
				return;

			}

			if(spawnGroup.ReverseForwardDirections[index] == true) {

				forward *= -1;

			}

		}

		public static List<ImprovedSpawnGroup> GetPlanetaryInstallations(Vector3D playerCoords, List<string> eligibleNames, out List<ImprovedSpawnGroup> smallStations, out List<ImprovedSpawnGroup> mediumStations, out List<ImprovedSpawnGroup> largeStations, out Dictionary<string, List<string>> validFactions) {
			
			smallStations = new List<ImprovedSpawnGroup>();
			mediumStations = new List<ImprovedSpawnGroup>();
			largeStations = new List<ImprovedSpawnGroup>();
			validFactions = new Dictionary<string, List<string>>();

			string specificGroup = "";
			var planetRestrictions = new List<string>(Settings.General.PlanetSpawnsDisableList.ToList());
			SpawnGroupSublists.Clear();
			SmallSpawnGroupSublists.Clear();
			MediumSpawnGroupSublists.Clear();
			LargeSpawnGroupSublists.Clear();
			EligibleSpawnsByModId.Clear();
			EligibleSmallSpawnsByModId.Clear();
			EligibleMediumSpawnsByModId.Clear();
			EligibleLargeSpawnsByModId.Clear();

			var environment = new EnvironmentEvaluation(playerCoords);

			if (environment.NearestPlanet != null && environment.IsOnPlanet) {

				if (planetRestrictions.Contains(environment.NearestPlanetName) == true) {

					return new List<ImprovedSpawnGroup>();

				}

			}

			bool specificSpawnRequest = false;
			
			if(SpawnGroupManager.AdminSpawnGroup != ""){
				
				specificGroup = SpawnGroupManager.AdminSpawnGroup;
				SpawnGroupManager.AdminSpawnGroup = "";
				specificSpawnRequest = true;
				
			}
			
			if(!environment.IsOnPlanet){
				
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

				if (spawnGroup.PlanetaryInstallation == false){
					
					continue;
					
				}
				
				if(spawnGroup.PlanetaryInstallationType != "Small" && spawnGroup.PlanetaryInstallationType != "Medium" && spawnGroup.PlanetaryInstallationType != "Large"){
					
					continue;
					
				}

				if (spawnGroup.PlanetaryInstallationChance < spawnGroup.ChanceCeiling && !specificSpawnRequest) {

					var roll = SpawnResources.rnd.Next(0, spawnGroup.ChanceCeiling + 1);

					if (roll > spawnGroup.PlanetaryInstallationChance)
						continue;

				}

				if (SpawnResources.CheckCommonConditions(spawnGroup, playerCoords, environment, specificSpawnRequest) == false){
					
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
					
					if(spawnGroup.PlanetaryInstallationType == "Small"){

						if(SmallSpawnGroupSublists.ContainsKey(modID) == false){
							
							SmallSpawnGroupSublists.Add(modID, new List<ImprovedSpawnGroup>());
							
						}
						
						if(EligibleSmallSpawnsByModId.ContainsKey(modID) == false){
							
							EligibleSmallSpawnsByModId.Add(modID, 1);
							
						}else{
							
							EligibleSmallSpawnsByModId[modID] += 1;
							
						}
		
					}
					
					if(spawnGroup.PlanetaryInstallationType == "Medium"){
	
						if(MediumSpawnGroupSublists.ContainsKey(modID) == false){
							
							MediumSpawnGroupSublists.Add(modID, new List<ImprovedSpawnGroup>());
							
						}
						
						if(EligibleMediumSpawnsByModId.ContainsKey(modID) == false){
							
							EligibleMediumSpawnsByModId.Add(modID, 1);
							
						}else{
							
							EligibleMediumSpawnsByModId[modID] += 1;
							
						}
	
					}
					
					if(spawnGroup.PlanetaryInstallationType == "Large"){

						if(LargeSpawnGroupSublists.ContainsKey(modID) == false){
							
							LargeSpawnGroupSublists.Add(modID, new List<ImprovedSpawnGroup>());
							
						}
						
						if(EligibleLargeSpawnsByModId.ContainsKey(modID) == false){
							
							EligibleLargeSpawnsByModId.Add(modID, 1);
							
						}else{
							
							EligibleLargeSpawnsByModId[modID] += 1;
							
						}
	
					}
						
					if(Settings.PlanetaryInstallations.UseMaxSpawnGroupFrequency == true && spawnGroup.Frequency > Settings.PlanetaryInstallations.MaxSpawnGroupFrequency * 10){
						
						spawnGroup.Frequency = (int)Math.Round((double)Settings.PlanetaryInstallations.MaxSpawnGroupFrequency * 10);
						
					}

					for(int i = 0; i < spawnGroup.Frequency; i++){
						
						eligibleGroups.Add(spawnGroup);
						SpawnGroupSublists[modID].Add(spawnGroup);
						
						if(spawnGroup.PlanetaryInstallationType == "Small"){
					
							smallStations.Add(spawnGroup);
							SmallSpawnGroupSublists[modID].Add(spawnGroup);
					
						}
						
						if(spawnGroup.PlanetaryInstallationType == "Medium"){
					
							mediumStations.Add(spawnGroup);
							MediumSpawnGroupSublists[modID].Add(spawnGroup);
					
						}
						
						if(spawnGroup.PlanetaryInstallationType == "Large"){
					
							largeStations.Add(spawnGroup);
							LargeSpawnGroupSublists[modID].Add(spawnGroup);
					
						}
						
					}
					
				}
		
			}
			
			return eligibleGroups;
			
		}
		
		public static bool GetSpawnCoords(ImprovedSpawnGroup spawnGroup, Vector3D startCoords, out Vector3D spawnCoords){
			
			spawnCoords = Vector3D.Zero;
			SpawnResources.RefreshEntityLists();
			MyPlanet planet = SpawnResources.GetNearestPlanet(startCoords);
			var planetEntity = planet as IMyEntity;
			double extraDistance = 0;
			double terrainVarianceCheckTarget = Settings.PlanetaryInstallations.SmallTerrainCheckDistance;
			
			if(planetEntity == null || planet == null){
				
				Logger.AddMsg("Planet Somehow Doesn't Exist... Even Though It Should If Script Got Here...", true);
				return false;
				
			}
			
			if(spawnGroup.PlanetaryInstallationType == "Medium"){
				
				extraDistance = Settings.PlanetaryInstallations.MediumSpawnDistanceIncrement;
				terrainVarianceCheckTarget = Settings.PlanetaryInstallations.MediumTerrainCheckDistance;
				
			}
			
			if(spawnGroup.PlanetaryInstallationType == "Large"){
				
				extraDistance = Settings.PlanetaryInstallations.LargeSpawnDistanceIncrement;
				terrainVarianceCheckTarget = Settings.PlanetaryInstallations.LargeTerrainCheckDistance;
				
			}
			
			var startDist = Settings.PlanetaryInstallations.MinimumSpawnDistanceFromPlayers + extraDistance;
			var endDist = Settings.PlanetaryInstallations.MaximumSpawnDistanceFromPlayers + extraDistance;
			var upDir = Vector3D.Normalize(startCoords - planetEntity.GetPosition());
			var forwardDir = Vector3D.Normalize(MyUtils.GetRandomPerpendicularVector(ref upDir));
			var searchMatrix = MatrixD.CreateWorld(startCoords, forwardDir, upDir);
			
			//Searches in 8 directions from the player position
			var searchDirections = new List<Vector3D>();
			searchDirections.Add(searchMatrix.Forward);
			searchDirections.Add(searchMatrix.Backward);
			searchDirections.Add(searchMatrix.Left);
			searchDirections.Add(searchMatrix.Right);
			
			if(Settings.PlanetaryInstallations.AggressivePathCheck == true){
				
				searchDirections.Add(Vector3D.Normalize(searchMatrix.Forward + searchMatrix.Left));
				searchDirections.Add(Vector3D.Normalize(searchMatrix.Forward + searchMatrix.Right));
				searchDirections.Add(Vector3D.Normalize(searchMatrix.Backward + searchMatrix.Left));
				searchDirections.Add(Vector3D.Normalize(searchMatrix.Backward + searchMatrix.Right));
				
			}

			int debugSpawnPointAttempts = 0;
			int searchDirectionAttempts = 0;

			bool doWaterChecks = false;
			Water localWater = null;

			if (MES_SessionCore.Instance.WaterMod.Registered) {

				for (int i = MES_SessionCore.Instance.WaterMod.Waters.Count - 1; i >= 0; i--) {

					if (i >= MES_SessionCore.Instance.WaterMod.Waters.Count)
						continue;

					var water = MES_SessionCore.Instance.WaterMod.Waters[i];

					if (water.planetID != planet.EntityId)
						continue;

					doWaterChecks = true;
					localWater = water;
					break;

				}

			}
			
			foreach(var searchDirection in searchDirections){
				
				searchDirectionAttempts++;
				double searchIncrement = startDist;
				
				while(searchIncrement < endDist){
					
					debugSpawnPointAttempts++;
					var checkCoords = searchDirection * searchIncrement + startCoords;
					var surfaceCoords = SpawnResources.GetNearestSurfacePoint(checkCoords, planet);

					if (spawnGroup.InstallationTerrainValidation) {

						var terrain = planet.GetMaterialAt(ref surfaceCoords);

						if (terrain != null) {

							if (!spawnGroup.AllowedTerrainTypes.Contains(terrain.MaterialTypeName)) {

								searchIncrement += Settings.PlanetaryInstallations.SearchPathIncrement;
								continue;

							}
				
						} else {

							searchIncrement += Settings.PlanetaryInstallations.SearchPathIncrement;
							continue;

						}

					}

					

					if (SpawnResources.IsPositionNearEntity(surfaceCoords, Settings.PlanetaryInstallations.MinimumSpawnDistanceFromOtherGrids) == true || SpawnResources.IsPositionInSafeZone(surfaceCoords) == true){
						
						searchIncrement += Settings.PlanetaryInstallations.SearchPathIncrement;
						continue;
						
					}

					bool foundWaterSurfacePosition = false;

					if (doWaterChecks) {

						var surfaceUnderwater = localWater.IsUnderwater(surfaceCoords);

						if (surfaceUnderwater && !spawnGroup.InstallationSpawnsOnWaterSurface && !spawnGroup.InstallationSpawnsUnderwater) {

							searchIncrement += Settings.PlanetaryInstallations.SearchPathIncrement;
							continue;

						}

						if (!surfaceUnderwater && !spawnGroup.InstallationSpawnsOnDryLand) {

							searchIncrement += Settings.PlanetaryInstallations.SearchPathIncrement;
							continue;

						}

						if (surfaceUnderwater && spawnGroup.InstallationSpawnsUnderwater && spawnGroup.MinWaterDepth > 0) {

							var waterSurfaceCoords = localWater.GetClosestSurfacePoint(surfaceCoords);
							var surfaceToWaterDist = Vector3D.Distance(surfaceCoords, waterSurfaceCoords);

							if (surfaceToWaterDist < spawnGroup.MinWaterDepth) {

								searchIncrement += Settings.PlanetaryInstallations.SearchPathIncrement;
								continue;

							}

						}

						if (!surfaceUnderwater && spawnGroup.InstallationSpawnsOnWaterSurface) {

							searchIncrement += Settings.PlanetaryInstallations.SearchPathIncrement;
							continue;

						}

						if (surfaceUnderwater && spawnGroup.InstallationSpawnsOnWaterSurface) {

							//TODO: Account For Minimum Depth in Surface Checks
							foundWaterSurfacePosition = true;

						}

					}

					var checkUpDir = Vector3D.Normalize(surfaceCoords - planetEntity.GetPosition());
					var checkForwardDir = Vector3D.Normalize(MyUtils.GetRandomPerpendicularVector(ref checkUpDir));
					var checkMatrix = MatrixD.CreateWorld(surfaceCoords, checkForwardDir, checkUpDir);
					
					var checkDirections = new List<Vector3D>();
					checkDirections.Add(checkMatrix.Forward);
					checkDirections.Add(checkMatrix.Backward);
					checkDirections.Add(checkMatrix.Left);
					checkDirections.Add(checkMatrix.Right);
					
					if(Settings.PlanetaryInstallations.AggressiveTerrainCheck == true){
						
						checkDirections.Add(Vector3D.Normalize(checkMatrix.Forward + checkMatrix.Left));
						checkDirections.Add(Vector3D.Normalize(checkMatrix.Forward + checkMatrix.Right));
						checkDirections.Add(Vector3D.Normalize(checkMatrix.Backward + checkMatrix.Left));
						checkDirections.Add(Vector3D.Normalize(checkMatrix.Backward + checkMatrix.Right));
						
					}
					
					var distToCore = Vector3D.Distance(surfaceCoords, planetEntity.GetPosition());
					bool badPosition = false;
					
					if(spawnGroup.SkipTerrainCheck == false || foundWaterSurfacePosition) {
						
						foreach(var checkDirection in checkDirections){
							
							double terrainCheckIncrement = 0;
							
							while(terrainCheckIncrement < terrainVarianceCheckTarget){

								var checkTerrainCoords = checkDirection * terrainCheckIncrement + surfaceCoords;
								var checkTerrainSurfaceCoords = SpawnResources.GetNearestSurfacePoint(checkTerrainCoords, planet);

								if (!foundWaterSurfacePosition) {

									var checkDistToCore = Vector3D.Distance(checkTerrainSurfaceCoords, planetEntity.GetPosition());
									var elevationDiff = checkDistToCore - distToCore;

									if (elevationDiff < Settings.PlanetaryInstallations.MinimumTerrainVariance || elevationDiff > Settings.PlanetaryInstallations.MaximumTerrainVariance) {

										badPosition = true;
										break;

									}

								} else {

									var pointUnderwater = localWater.IsUnderwater(checkTerrainSurfaceCoords);

									if (!pointUnderwater) {

										badPosition = true;
										break;

									}

									var pointSurfaceWater = localWater.GetClosestSurfacePoint(checkTerrainSurfaceCoords);

									if (Vector3D.Distance(pointSurfaceWater, checkTerrainSurfaceCoords) < spawnGroup.MinWaterDepth) {

										badPosition = true;
										break;

									}

								}

								terrainCheckIncrement += Settings.PlanetaryInstallations.TerrainCheckIncrementDistance;

							}

							if (badPosition == true){
								
								break;
								
							}
							
						}
						
					}
					
					
					
					if(badPosition == false){
						
						spawnCoords = surfaceCoords;
						Logger.AddMsg("Found Installation Site After: " + debugSpawnPointAttempts.ToString() + " Attempts", true);
						Logger.AddMsg("Search Directions Used: " + searchDirectionAttempts.ToString(), true);
						return true;
						
					}
					
					searchIncrement += Settings.PlanetaryInstallations.SearchPathIncrement;
					
				}
				
			}
			
			Logger.AddMsg("Could Not Find Installation Site After: " + debugSpawnPointAttempts.ToString() + " Attempts", true);
			return false;
			
		}

		public static MatrixD CalculateDerelictSpawnMatrix(MatrixD existingMatrix, Vector3D rotationValues) {

			//X: Pitch - Up/Forward | +Up -Down
			//Y: Yaw   - Forward/Up | +Right -Left
			//Z: Roll  - Up/Forward | +Right -Left

			var resultMatrix = existingMatrix;

			if(rotationValues.X != 0) {

				var translation = resultMatrix.Translation;
				var fowardPos = resultMatrix.Forward * 45;
				var upPos = resultMatrix.Up * 45;
				var pitchForward = Vector3D.Normalize(resultMatrix.Up * rotationValues.X + fowardPos);
				var pitchUp = Vector3D.Normalize(resultMatrix.Backward * rotationValues.X + upPos);
				resultMatrix = MatrixD.CreateWorld(translation, pitchForward, pitchUp);

			}

			if(rotationValues.Y != 0) {

				var translation = resultMatrix.Translation;
				var fowardPos = resultMatrix.Forward * 45;
				var upPos = resultMatrix.Up * 45;
				var yawForward = Vector3D.Normalize(resultMatrix.Right * rotationValues.Y + fowardPos);
				var yawUp = resultMatrix.Up;
				resultMatrix = MatrixD.CreateWorld(translation, yawForward, yawUp);

			}

			if(rotationValues.Z != 0) {

				var translation = resultMatrix.Translation;
				var fowardPos = resultMatrix.Forward * 45;
				var upPos = resultMatrix.Up * 45;
				var rollForward = resultMatrix.Forward;
				var rollUp = Vector3D.Normalize(resultMatrix.Right * rotationValues.Z + upPos);
				resultMatrix = MatrixD.CreateWorld(translation, rollForward, rollUp);

			}

			return resultMatrix;

		}
			
	}
	
}