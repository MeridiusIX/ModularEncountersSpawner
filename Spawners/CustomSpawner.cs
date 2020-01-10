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
	
	public static class CustomSpawner {
		
		public static bool CustomSpawnRequest(List<string> spawnGroups, MatrixD spawningMatrix, Vector3 velocity, bool ignoreSafetyCheck) {

			Logger.AddMsg("Custom Spawn Request Received", true);

			if(Settings.General.UseMaxNpcGrids == true){
				
				var totalNPCs = NPCWatcher.ActiveNPCs.Count;
				
				if(totalNPCs >= Settings.General.MaxGlobalNpcGrids){

					//return "Spawning Aborted. Max Global NPCs Limit Reached.";
					return false;
					
				}
				
			}

			KnownPlayerLocationManager.CleanExpiredLocations();
			var validFactions = new Dictionary<string, List<string>>();
			var spawnGroupList = GetSpawnGroups(spawnGroups, spawningMatrix.Translation, out validFactions);
			
			if(spawnGroupList.Count == 0){

				return false;
				//return "No Eligible Spawn Groups Could Be Found To Spawn Near Player.";
				
			}
			
			var spawnGroup = spawnGroupList[SpawnResources.rnd.Next(0, spawnGroupList.Count)];
			bool badSpawnArea = false;

			if (ignoreSafetyCheck == false) {



			}

			if (badSpawnArea)
				return false;

			var startPathCoords = Vector3D.Zero;
			var endPathCoords = Vector3D.Zero;
			bool successfulPath = false;
			
			//Get Directions
			var spawnForwardDir = spawningMatrix.Forward;
			var spawnUpDir = spawningMatrix.Up;
			var spawnMatrix = spawningMatrix;
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

					KnownPlayerLocationManager.IncreaseSpawnCountOfLocations(spawnMatrix.Translation);


				}

				var options = SpawnGroupManager.CreateSpawningOptions(spawnGroup, prefab);
				var spawnPosition = Vector3D.Transform((Vector3D)prefab.Position, spawningMatrix);
				var speedL = velocity;
				var speedA = Vector3.Zero;
				var gridList = new List<IMyCubeGrid>();
				
				//Grid Manipulation
				GridBuilderManipulation.ProcessPrefabForManipulation(prefab.SubtypeId, spawnGroup, "SpaceCargoShip", prefab.Behaviour);

				try{
					
					MyAPIGateway.PrefabManager.SpawnPrefab(gridList, prefab.SubtypeId, spawnPosition, spawnForwardDir, spawnUpDir, speedL, speedA, prefab.BeaconText, options, gridOwner);
					
				}catch(Exception exc){
					
					Logger.AddMsg("Something Went Wrong With Prefab Spawn Manager.", true);
					
				}
				
				var pendingNPC = new ActiveNPC();
				pendingNPC.SpawnGroupName = spawnGroup.SpawnGroupName;
				pendingNPC.SpawnGroup = spawnGroup;
				pendingNPC.InitialFaction = randFactionTag;
				pendingNPC.faction = MyAPIGateway.Session.Factions.TryGetFactionByTag(pendingNPC.InitialFaction);
				pendingNPC.Name = prefab.SubtypeId;
				pendingNPC.GridName = MyDefinitionManager.Static.GetPrefabDefinition(prefab.SubtypeId).CubeGrids[0].DisplayName;
				pendingNPC.StartCoords = startPathCoords;
				pendingNPC.CurrentCoords = startPathCoords;
				pendingNPC.EndCoords = endPathCoords;
				pendingNPC.SpawnType = "CustomSpawn";
				pendingNPC.AutoPilotSpeed = speedL.Length();
				pendingNPC.CleanupIgnore = spawnGroup.IgnoreCleanupRules;
				pendingNPC.ForceStaticGrid = spawnGroup.ForceStaticGrid;
				pendingNPC.KeenAiName = prefab.Behaviour;
				pendingNPC.KeenAiTriggerDistance = prefab.BehaviourActivationDistance;
				
				if(string.IsNullOrEmpty(pendingNPC.KeenAiName) == false){
					
					Logger.AddMsg("AI Detected In Prefab: " + prefab.SubtypeId + " in SpawnGroup: " + spawnGroup.SpawnGroup.Id.SubtypeName);
					
				}
				
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
			//return "Spawning Group - " + spawnGroup.SpawnGroup.Id.SubtypeName;
			return true;
			
		}
		
		public static List<ImprovedSpawnGroup> GetSpawnGroups(List<string> spawnGroups, Vector3D coords, out Dictionary<string, List<string>> validFactions){
			

			MyPlanet planet = SpawnResources.GetNearestPlanet(coords);
			var planetRestrictions = new List<string>(Settings.General.PlanetSpawnsDisableList.ToList());
			validFactions = new Dictionary<string, List<string>>();
			
			if(planet != null){
				
				if(planetRestrictions.Contains(planet.Generator.Id.SubtypeName) == true){
					
					return new List<ImprovedSpawnGroup>();
					
				}
				
			}
			
			string planetName = "";
			
			if(planet != null){
				
				planetName = planet.Generator.Id.SubtypeId.ToString();
				
			}
			
			var eligibleGroups = new List<ImprovedSpawnGroup>();
			
			//Filter Eligible Groups To List
			foreach(var spawnGroup in SpawnGroupManager.SpawnGroups){

				if(!spawnGroups.Contains(spawnGroup.SpawnGroupName)) {

					continue;

				}

				if(spawnGroup.RivalAiAnySpawn == false) {

					if(planet == null && spawnGroup.RivalAiSpaceSpawn == false) {

						continue;

					}

					if(planet != null) {

						var airDensity = planet.GetAirDensity(coords);

						if(spawnGroup.RivalAiAtmosphericSpawn == false || planet.HasAtmosphere == false || airDensity < 0.4f) {

							continue;

						}

					}

				}

				if(SpawnResources.CheckCommonConditions(spawnGroup, coords, planet, false) == false){
					
					continue;
					
				}

				var validFactionsList = SpawnResources.ValidNpcFactions(spawnGroup, coords);

				if(validFactionsList.Count == 0) {

					continue;

				}

				if(validFactions.ContainsKey(spawnGroup.SpawnGroupName) == false) {

					validFactions.Add(spawnGroup.SpawnGroupName, validFactionsList);

				}
				
				if(spawnGroup.Frequency > 0){
					
					if(Settings.SpaceCargoShips.UseMaxSpawnGroupFrequency == true && spawnGroup.Frequency > Settings.SpaceCargoShips.MaxSpawnGroupFrequency * 10){
						
						spawnGroup.Frequency = (int)Math.Round((double)Settings.SpaceCargoShips.MaxSpawnGroupFrequency * 10);
						
					}
					
					for(int i = 0; i < spawnGroup.Frequency; i++){
						
						eligibleGroups.Add(spawnGroup);
						
					}
					
				}
				
			}
			
			return eligibleGroups;
			
		}
			
	}
	
}