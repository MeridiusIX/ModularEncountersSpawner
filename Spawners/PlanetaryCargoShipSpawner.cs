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
	
	public static class PlanetaryCargoShipSpawner{
		
		public static Dictionary<string, List<ImprovedSpawnGroup>> SpawnGroupSublists = new Dictionary<string, List<ImprovedSpawnGroup>>();
		public static Dictionary<string, int> EligibleSpawnsByModId = new Dictionary<string, int>();
		
		public static string AttemptSpawn(Vector3D startCoords, List<string> eligibleNames = null) {
				
			if(Settings.General.UseMaxNpcGrids == true){
				
				var totalNPCs = NPCWatcher.ActiveNPCs.Count;
				
				if(totalNPCs >= Settings.General.MaxGlobalNpcGrids){
					
					return "Spawning Aborted. Max Global NPCs Limit Reached.";
					
				}
				
			}
			
			if(NPCWatcher.ActiveNpcTypeLimitReachedForArea("PlanetaryCargoShip", startCoords, Settings.PlanetaryCargoShips.MaxShipsPerArea, Settings.PlanetaryCargoShips.AreaSize) == true){
				
				return "Too Many Planetary Cargo Ship Grids in Player Area";
				
			}
			
			MyPlanet planet = SpawnResources.GetNearestPlanet(startCoords);
			
			if(SpawnResources.GetDistanceFromSurface(startCoords, planet) > Settings.PlanetaryCargoShips.PlayerSurfaceAltitude){
				
				return "Player Is Too Far From Planet Surface.";
				
			}

			KnownPlayerLocationManager.CleanExpiredLocations();
			var validFactions = new Dictionary<string, List<string>>();
			var spawnGroupList = GetPlanetaryCargoShips(startCoords, eligibleNames, out validFactions);
			
			if(Settings.General.UseModIdSelectionForSpawning == true){
				
				spawnGroupList = SpawnResources.SelectSpawnGroupSublist(SpawnGroupSublists, EligibleSpawnsByModId);
				
			}

			if(spawnGroupList.Count == 0){
				
				return "No Eligible Spawn Groups Could Be Found To Spawn Near Player.";
				
			}
			
			var spawnGroup = spawnGroupList[SpawnResources.rnd.Next(0, spawnGroupList.Count)];
			
			Vector3D startPathCoords = Vector3D.Zero;
			Vector3D endPathCoords = Vector3D.Zero;
			MatrixD startMatrix = MatrixD.CreateWorld(Vector3D.Zero, Vector3D.Forward, Vector3D.Up);
			
			bool successfulPath = CalculateAtmoTravelPath(spawnGroup, startCoords, planet, out startPathCoords, out endPathCoords, out startMatrix);
			
			if(successfulPath == false){
				
				return "Could Not Generate Safe Travel Path For SpawnGroup.";
				
			}
			
			//Get Directions
			var spawnForwardDir = startMatrix.Forward;
			var spawnUpDir = startMatrix.Up;
			var spawnMatrix = startMatrix;
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

					KnownPlayerLocationManager.IncreaseSpawnCountOfLocations(startCoords, randFactionTag);

				}

				var options = SpawnGroupManager.CreateSpawningOptions(spawnGroup, prefab);
				var spawnPosition = Vector3D.Transform((Vector3D)prefab.Position, spawnMatrix);
				var speedL = prefab.Speed;
				var speedA = Vector3.Zero;
				var gridList = new List<IMyCubeGrid>();
				
				
				//Speed Management
				if(Settings.PlanetaryCargoShips.UseMinimumSpeed == true && prefab.Speed < Settings.PlanetaryCargoShips.MinimumSpeed){
					
					speedL = Settings.PlanetaryCargoShips.MinimumSpeed;
					
				}
				
				if(Settings.PlanetaryCargoShips.UseSpeedOverride == true){
					
					speedL = Settings.PlanetaryCargoShips.SpeedOverride;
					
				}
				
				
				
				/*//Character Injection - Start
				MyPrefabDefinition prefabDefB = null;
				
				if(SpawnGroupManager.prefabBackupList.ContainsKey(prefab.SubtypeId) == true){
					
					prefabDefB = MyDefinitionManager.Static.GetPrefabDefinition(prefab.SubtypeId);
					
					if(SpawnGroupManager.prefabBackupList[prefab.SubtypeId].Count == prefabDefB.CubeGrids.Length){
						
						for(int i = 0; i < SpawnGroupManager.prefabBackupList[prefab.SubtypeId].Count; i++){
							
							var clonedGridOb = SpawnGroupManager.prefabBackupList[prefab.SubtypeId][i].Clone();
							prefabDefB.CubeGrids[i] = clonedGridOb as MyObjectBuilder_CubeGrid;
							
						}
						
					}
					
					SpawnGroupManager.prefabBackupList.Remove(prefab.SubtypeId);
					
				}
				
				var injectedCharacters = false;
				if(spawnGroup.FillSeatsWithNpcCrew == true){
					
					injectedCharacters = true;
					
					if(prefabDefB == null){
						
						prefabDefB = MyDefinitionManager.Static.GetPrefabDefinition(prefab.SubtypeId);
						
					}
					
					var backupGridList = new List<MyObjectBuilder_CubeGrid>();
					
					for(int i = 0; i < prefabDefB.CubeGrids.Length; i++){
						
						var clonedGridOb = prefabDefB.CubeGrids[i].Clone();
						backupGridList.Add(clonedGridOb as MyObjectBuilder_CubeGrid);
						GridUtilities.CreateAndAttachCrew(prefabDefB.CubeGrids[i], spawnGroup, gridOwner);
						
					}
					
					if(SpawnGroupManager.prefabBackupList.ContainsKey(prefab.SubtypeId) == false){
						
						SpawnGroupManager.prefabBackupList.Add(prefab.SubtypeId, backupGridList);
						
					}
	
				}
				//Character Injection - End*/
				
				//Grid Manipulation
				GridBuilderManipulation.ProcessPrefabForManipulation(prefab.SubtypeId, spawnGroup, "PlanetaryCargoShip", prefab.Behaviour);
				
				try{
					
					MyAPIGateway.PrefabManager.SpawnPrefab(gridList, prefab.SubtypeId, spawnPosition, spawnForwardDir, spawnUpDir, Vector3.Zero, Vector3.Zero, !string.IsNullOrWhiteSpace(prefab.BeaconText) ? prefab.BeaconText : null, options, gridOwner);
					
				}catch(Exception exc){
					
					
					
				}
				
				var pendingNPC = new ActiveNPC();
				pendingNPC.SpawnGroup = spawnGroup;
				pendingNPC.SpawnGroupName = spawnGroup.SpawnGroupName;
				pendingNPC.InitialFaction = randFactionTag;
				pendingNPC.faction = MyAPIGateway.Session.Factions.TryGetFactionByTag(pendingNPC.InitialFaction);
				pendingNPC.Name = prefab.SubtypeId;
				pendingNPC.GridName = MyDefinitionManager.Static.GetPrefabDefinition(prefab.SubtypeId).CubeGrids[0].DisplayName;
				pendingNPC.StartCoords = startPathCoords;
				pendingNPC.CurrentCoords = startPathCoords;
				pendingNPC.EndCoords = endPathCoords;
				pendingNPC.AutoPilotSpeed = speedL;
				pendingNPC.Planet = planet;
				pendingNPC.SpawnType = "PlanetaryCargoShip";
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
		
		public static bool CalculateAtmoTravelPath(ImprovedSpawnGroup spawnGroup, Vector3D startCoords, MyPlanet planet, out Vector3D startPathCoords, out Vector3D endPathCoords, out MatrixD startMatrix){
			
			startPathCoords = Vector3D.Zero;
			endPathCoords = Vector3D.Zero;
			startMatrix = MatrixD.CreateWorld(Vector3D.Zero, Vector3D.Forward, Vector3D.Up);
			SpawnResources.RefreshEntityLists();
			
			if(planet == null){
				
				return false;
				
			}
			
			var planetEntity = planet as IMyEntity;
			
			for(int i = 0; i < Settings.PlanetaryCargoShips.MaxSpawnAttempts; i++){
				
				//Get Starting Point
				var randDirFromPlayer = SpawnResources.GetRandomCompassDirection(startCoords, planet);
				var pathDist = SpawnResources.GetRandomPathDist(Settings.PlanetaryCargoShips.MinPathDistanceFromPlayer, Settings.PlanetaryCargoShips.MaxPathDistanceFromPlayer);
				var midPointSurface = SpawnResources.GetNearestSurfacePoint(randDirFromPlayer * pathDist + startCoords, planet);
				var upDir = Vector3D.Normalize(midPointSurface - planetEntity.GetPosition());
				var altitudeFromMid = SpawnResources.GetRandomPathDist(Settings.PlanetaryCargoShips.MinSpawningAltitude, Settings.PlanetaryCargoShips.MaxSpawningAltitude);
				var tempStartPath = upDir * altitudeFromMid + midPointSurface;
				
				if(spawnGroup.PlanetRequiresAtmo == true && planet.GetAirDensity(tempStartPath) < Settings.PlanetaryCargoShips.MinAirDensity){
					
					tempStartPath = upDir * Settings.PlanetaryCargoShips.MinSpawningAltitude + midPointSurface;
					
					if(spawnGroup.PlanetRequiresAtmo == true && planet.GetAirDensity(tempStartPath) < Settings.PlanetaryCargoShips.MinAirDensity){
						
						continue;
						
					}
					
				}
				
				if(SpawnResources.IsPositionNearEntities(tempStartPath, Settings.PlanetaryCargoShips.MinSpawnFromGrids) == true){
					
					continue;
					
				}
				
				var startCoordsDistFromCenter = Vector3D.Distance(planetEntity.GetPosition(), tempStartPath);
				
				//Get Ending Point
				var randPathDir = SpawnResources.GetRandomCompassDirection(tempStartPath, planet);
				var randPathDist = SpawnResources.GetRandomPathDist(Settings.PlanetaryCargoShips.MinPathDistance, Settings.PlanetaryCargoShips.MaxPathDistance);
				var endPathA = randPathDir * randPathDist + tempStartPath;
				var endPathB = -randPathDir * randPathDist + tempStartPath;
				var tempEndPath = Vector3D.Zero;
				
				if(Vector3D.Distance(endPathA, startCoords) < Vector3D.Distance(endPathB, startCoords)){
					
					tempEndPath = endPathA;
					
				}else{
					
					tempEndPath = endPathB;
					randPathDir *= -1;
					
				}
				
				//TODO: Set At Same Height From Sealevel As Start
				var endUpDir = Vector3D.Normalize(tempEndPath - planetEntity.GetPosition());
				tempEndPath = endUpDir * startCoordsDistFromCenter + planetEntity.GetPosition();
				
				//Check Path
				var tempMatrix = MatrixD.CreateWorld(tempStartPath, randPathDir, upDir);
				var truePathDir = Vector3D.Normalize(tempEndPath - tempStartPath);
				bool badPath = false;
				
				foreach(var prefab in spawnGroup.SpawnGroup.Prefabs){
					
					var modifiedStart = Vector3D.Transform((Vector3D)prefab.Position, tempMatrix);
					double totalSteps = 0;
					
					while(totalSteps < randPathDist){
						
						var testPath = totalSteps * truePathDir + modifiedStart;
						
						if(SpawnResources.IsPositionInSafeZone(testPath) == true){
							
							badPath = true;
							break;
							
						}
						
						if(SpawnResources.GetDistanceFromSurface(testPath, planet) < Settings.PlanetaryCargoShips.MinPathAltitude){
							
							badPath = true;
							break;
							
						}
												
						totalSteps += Settings.PlanetaryCargoShips.PathStepCheckDistance;
						
					}
					
					if(badPath == true){
						
						break;
						
					}
					
				}
				
				if(badPath == true){
					
					continue;
					
				}
				
				startPathCoords = tempStartPath;
				endPathCoords = tempEndPath;
				startMatrix = tempMatrix;
				return true;
				
			}
			
			return false;
			
		}
				
		public static List<ImprovedSpawnGroup> GetPlanetaryCargoShips(Vector3D playerCoords, List<string> eligibleNames, out Dictionary<string, List<string>> validFactions) {

			string specificGroup = "";
			var planetRestrictions = new List<string>(Settings.General.PlanetSpawnsDisableList.ToList());
			validFactions = new Dictionary<string, List<string>>();
			SpawnGroupSublists.Clear();
			EligibleSpawnsByModId.Clear();
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

			if (!environment.IsOnPlanet) {

				return new List<ImprovedSpawnGroup>();

			}

			if(environment.NearestPlanet == null) {

				return new List<ImprovedSpawnGroup>();

			}
			
			var eligibleGroups = new List<ImprovedSpawnGroup>();
			
			//Filter Eligible Groups To List
			foreach(var spawnGroup in SpawnGroupManager.SpawnGroups){

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

				
				
				if(spawnGroup.AtmosphericCargoShip == false){
					
					continue;
					
				}
				
				if(SpawnResources.CheckCommonConditions(spawnGroup, playerCoords, environment, specificSpawnRequest) == false){
					
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

					if(Settings.PlanetaryCargoShips.UseMaxSpawnGroupFrequency == true && spawnGroup.Frequency > Settings.PlanetaryCargoShips.MaxSpawnGroupFrequency * 10){
						
						spawnGroup.Frequency = (int)Math.Round((double)Settings.PlanetaryCargoShips.MaxSpawnGroupFrequency * 10);
						
					}
					
					for(int i = 0; i < spawnGroup.Frequency; i++){
						
						eligibleGroups.Add(spawnGroup);
						SpawnGroupSublists[modID].Add(spawnGroup);
						
					}
					
				}
				
			}
			
			return eligibleGroups;
			
		}
			
	}
	
}