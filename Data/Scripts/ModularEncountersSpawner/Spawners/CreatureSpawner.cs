using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox.Definitions;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.Utils;
using VRageMath;
using ModularEncountersSpawner.Configuration;
using ModularEncountersSpawner.Templates;
using Sandbox.Game;
using VRage.Game;

namespace ModularEncountersSpawner.Spawners {

	public static class CreatureSpawner{
		
		public static Dictionary<string, List<ImprovedSpawnGroup>> SpawnGroupSublists = new Dictionary<string, List<ImprovedSpawnGroup>>();
		public static Dictionary<string, int> EligibleSpawnsByModId = new Dictionary<string, int>();

		public static string AttemptSpawn(Vector3D startCoords, List<string> eligibleNames = null, bool ignoreAltitudeRequirement = false){

			if (!MES_SessionCore.SpawnerEnabled) {

				return "Spawner Disabled Due To Incompatible World Settings (Selective Physics Updates)";

			}

			KnownPlayerLocationManager.CleanExpiredLocations();
			var validFactions = new Dictionary<string, List<string>>();
			var spawnGroupList = GetCreatureSpawnGroups(startCoords, eligibleNames, ignoreAltitudeRequirement, out validFactions);
			
			if(Settings.General.UseModIdSelectionForSpawning == true){
				
				spawnGroupList = SpawnResources.SelectSpawnGroupSublist(SpawnGroupSublists, EligibleSpawnsByModId);
				
			}

			if(spawnGroupList.Count == 0){

				Logger.AddMsg("No Creature SpawnGroups", true);
				return "No Eligible Creature Spawn Groups Could Be Found To Spawn Near Player.";
				
			}
			
			var spawnGroup = spawnGroupList[SpawnResources.rnd.Next(0, spawnGroupList.Count)];

			int count = 0;

			if (spawnGroup.MinCreatureCount == spawnGroup.MaxCreatureCount)
				count = spawnGroup.MinCreatureCount;

			else
				count = SpawnResources.rnd.Next(spawnGroup.MinCreatureCount, spawnGroup.MaxCreatureCount);

			var planet = MyGamePruningStructure.GetClosestPlanet(startCoords);
			var up = Vector3D.Normalize(startCoords - planet.PositionComp.WorldAABB.Center);
			SpawnResources.RefreshEntityLists();
			var cells = new List<Vector3I>();

			for (int i = 0; i < count; i++) {

				for (int j = 0; j < 11; j++) {

					var forward = MyUtils.GetRandomPerpendicularVector(ref up);
					var dist = SpawnResources.rnd.Next(spawnGroup.MinCreatureDistance, spawnGroup.MaxCreatureDistance);
					var roughcoords = forward * dist + startCoords;
					var coords = planet.GetClosestSurfacePointGlobal(roughcoords) + up;
					var upCoords = up * 100 + coords;
					bool badCoords = false;

					foreach (var entity in SpawnResources.EntityList) {

						if (Vector3D.Distance(entity.GetPosition(), coords) > 500)
							continue;

						var grid = entity as IMyCubeGrid;

						if (grid == null)
							continue;

						if (grid.WorldAABB.Contains(coords) == ContainmentType.Disjoint)
							continue;

						cells.Clear();
						grid.RayCastCells(upCoords, coords, cells);

						foreach (var cell in cells) {

							var block = grid.GetCubeBlock(cell);

							if (block != null) {

								badCoords = true;
								break;
							
							}
						
						}

						if (badCoords)
							break;
					
					}

					if (badCoords)
						continue;

					//Spawn Bot Here
					string botType = "";

					if (spawnGroup.CreatureIds.Count == 1) {

						botType = spawnGroup.CreatureIds[0];

					} else {

						botType = spawnGroup.CreatureIds[SpawnResources.rnd.Next(0, spawnGroup.CreatureIds.Count)];
					
					}

					Logger.AddMsg("Spawning Creature: " + botType, true);
					MyVisualScriptLogicProvider.SpawnBot(botType, coords, -forward, up, botType);
					break;

				}
			
			}

			Logger.SkipNextMessage = false;
			return "Spawning Group - " + spawnGroup.SpawnGroup.Id.SubtypeName;
			
		}
		
		public static List<ImprovedSpawnGroup> GetCreatureSpawnGroups(Vector3D playerCoords, List<string> eligibleNames, bool ignoreAltitude, out Dictionary<string, List<string>> validFactions){

			string specificGroup = "";
			var planetRestrictions = new List<string>(Settings.General.PlanetSpawnsDisableList.ToList());
			validFactions = new Dictionary<string, List<string>>();
			SpawnGroupSublists.Clear();
			EligibleSpawnsByModId.Clear();
			var environment = new EnvironmentEvaluation(playerCoords);

			if (environment.NearestPlanet != null) {

				if (planetRestrictions.Contains(environment.NearestPlanetName) && environment.IsOnPlanet) {

					Logger.SpawnGroupDebug(Logger.DebugSpawnGroup, "Restricted Planet, No Spawns Allowed");
					return new List<ImprovedSpawnGroup>();

				}

			}

			bool specificSpawnRequest = false;
			
			if(SpawnGroupManager.AdminSpawnGroup != ""){
				
				specificGroup = SpawnGroupManager.AdminSpawnGroup;
				SpawnGroupManager.AdminSpawnGroup = "";
				specificSpawnRequest = true;
				
			}
			
			if(!environment.IsOnPlanet || (environment.AltitudeAtPosition > Settings.Creatures.MaxPlayerAltitudeForSpawn && !ignoreAltitude)) {

				return new List<ImprovedSpawnGroup>();

			}
			
			var eligibleGroups = new List<ImprovedSpawnGroup>();
			SpawnResources.SandboxVariableCache.Clear();

			//Filter Eligible Groups To List
			foreach (var spawnGroup in SpawnGroupManager.SpawnGroups){
				
				if (eligibleNames != null) {

					if (!eligibleNames.Contains(spawnGroup.SpawnGroupName)) {

						Logger.SpawnGroupDebug(spawnGroup.SpawnGroup.Id.SubtypeName, "Was not included in Groups From API Request");
						continue;

					}
				
				} else {

					if (specificGroup != "" && spawnGroup.SpawnGroupName != specificGroup) {

						Logger.SpawnGroupDebug(spawnGroup.SpawnGroupName, "Doesn't Match Admin Spawn");
						continue;

					}

					if (specificGroup == "" && spawnGroup.AdminSpawnOnly == true) {

						Logger.SpawnGroupDebug(spawnGroup.SpawnGroupName, "Only Admin Spawn Allowed");
						continue;

					}

				}

				if(spawnGroup.CreatureSpawn == false || spawnGroup.CreatureIds.Count == 0){

					continue;
					
				}

				/*
				if (spawnGroup.LunarCargoShip && spawnGroup.LunarCargoShipChance < spawnGroup.ChanceCeiling && allowLunar && !specificSpawnRequest) {

					var roll = SpawnResources.rnd.Next(0, spawnGroup.ChanceCeiling + 1);

					if (roll > spawnGroup.LunarCargoShipChance)
						continue;

				}

				if (spawnGroup.SpaceCargoShip && spawnGroup.SpaceCargoShipChance < spawnGroup.ChanceCeiling && !specificSpawnRequest) {

					var roll = SpawnResources.rnd.Next(0, spawnGroup.ChanceCeiling + 1);

					if (roll > spawnGroup.SpaceCargoShipChance)
						continue;

				}
				*/

				if (SpawnResources.CheckCommonConditions(spawnGroup, playerCoords, environment, specificSpawnRequest) == false){

					Logger.SpawnGroupDebug(spawnGroup.SpawnGroup.Id.SubtypeName, "Common Conditions Failed");
					continue;
					
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

					for(int i = 0; i < spawnGroup.Frequency; i++){
						
						eligibleGroups.Add(spawnGroup);
						SpawnGroupSublists[modID].Add(spawnGroup);
						
					}
					
				}
				
			}
			
			return eligibleGroups;
			
		}

		public static void BotOverrideConfig() {

			//Scan Planets and Generate Temp SpawnGroups
			var allPlanets = MyDefinitionManager.Static.GetPlanetsGeneratorsDefinitions();

			var bots = MyDefinitionManager.Static.GetBotDefinitions();
			var botIds = new List<MyPlanetAnimal>();

			MySpawnGroupDefinition dummyGroup = null;
			var spawnGroups = MyDefinitionManager.Static.GetSpawnGroupDefinitions();

			foreach (var group in spawnGroups) {

				if (group.Id.SubtypeName != "MES-CreatureDummySpawnGroup")
					continue;

				dummyGroup = group;
				break;
			
			}

			foreach (var bot in bots) {

				if (bot as MyAnimalBotDefinition == null)
					continue;

				var animal = new MyPlanetAnimal();
				animal.AnimalType = bot.Id.SubtypeName;
				botIds.Add(animal);

			}

			var botArray = botIds.ToArray();

			foreach (var planet in allPlanets) {

				//If No Planet Animal Info, Create it, Add Spider, and Skip
				if (planet.AnimalSpawnInfo == null && planet.NightAnimalSpawnInfo == null) {

					AddAllBotsToPlanetGenerator(planet, botArray);
					continue;

				}

				if (planet.AnimalSpawnInfo?.Animals != null && planet.AnimalSpawnInfo.Animals.Length > 0) {

					var spawnGroup = new ImprovedSpawnGroup();
					spawnGroup.SpawnGroupName = "CreatureSpawn-" + planet.Id.SubtypeName + "-InternalDaySpawns";
					spawnGroup.CreatureSpawn = true;
					spawnGroup.MinCreatureCount = planet.AnimalSpawnInfo.WaveCountMin;
					spawnGroup.MaxCreatureCount = planet.AnimalSpawnInfo.WaveCountMax;
					spawnGroup.MinCreatureDistance = (int)planet.AnimalSpawnInfo.SpawnDistMin;
					spawnGroup.MaxCreatureDistance = (int)planet.AnimalSpawnInfo.SpawnDistMax;
					spawnGroup.SpawnGroup = dummyGroup;
					spawnGroup.PlanetWhitelist.Add(planet.Id.SubtypeName);
					spawnGroup.Frequency = 30;

					foreach (var animal in planet.AnimalSpawnInfo.Animals) {

						spawnGroup.CreatureIds.Add(animal.AnimalType);
						
					}

					SpawnGroupManager.AddSpawnGroup(spawnGroup);

				}

				if (planet.NightAnimalSpawnInfo?.Animals != null && planet.NightAnimalSpawnInfo.Animals.Length > 0) {

					var spawnGroup = new ImprovedSpawnGroup();
					spawnGroup.SpawnGroupName = "CreatureSpawn-" + planet.Id.SubtypeName + "-InternalNightSpawns";
					spawnGroup.CreatureSpawn = true;
					spawnGroup.MinCreatureCount = planet.NightAnimalSpawnInfo.WaveCountMin;
					spawnGroup.MaxCreatureCount = planet.NightAnimalSpawnInfo.WaveCountMax;
					spawnGroup.MinCreatureDistance = (int)planet.NightAnimalSpawnInfo.SpawnDistMin;
					spawnGroup.MaxCreatureDistance = (int)planet.NightAnimalSpawnInfo.SpawnDistMax;
					spawnGroup.SpawnGroup = dummyGroup;
					spawnGroup.UseDayOrNightOnly = true;
					spawnGroup.SpawnOnlyAtNight = true;
					spawnGroup.PlanetWhitelist.Add(planet.Id.SubtypeName);
					spawnGroup.Frequency = 30;

					foreach (var animal in planet.NightAnimalSpawnInfo.Animals) {

						spawnGroup.CreatureIds.Add(animal.AnimalType);

					}

					SpawnGroupManager.AddSpawnGroup(spawnGroup);

				}

				AddAllBotsToPlanetGenerator(planet, botArray);

			}

			//Disable Wolves and Spiders

		}

		public static void AddAllBotsToPlanetGenerator(MyPlanetGeneratorDefinition planet, MyPlanetAnimal[] bots) {

			planet.AnimalSpawnInfo = new MyPlanetAnimalSpawnInfo();
			planet.AnimalSpawnInfo.SpawnDelayMin = 600000;
			planet.AnimalSpawnInfo.SpawnDelayMax = 3600000;
			planet.AnimalSpawnInfo.SpawnDistMin = 100;
			planet.AnimalSpawnInfo.SpawnDistMax = 1000;
			planet.AnimalSpawnInfo.WaveCountMin = 1;
			planet.AnimalSpawnInfo.WaveCountMax = 4;
			planet.AnimalSpawnInfo.Animals = bots;

		}

	}

}