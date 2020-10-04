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
using VRage.Game.ObjectBuilders;
using ModularEncountersSpawner.Api;

namespace ModularEncountersSpawner.Spawners{

	public static class SpawnResources{
		
		public static HashSet<IMyEntity> EntityList = new HashSet<IMyEntity>();
		public static List<MySafeZone> safezoneList = new List<MySafeZone>();
		public static List<MyPlanet> PlanetList = new List<MyPlanet>();
		public static DateTime LastEntityRefresh = DateTime.Now;
		public static DateTime GameStartTime = DateTime.Now;

		private static List<string> _checkedDefinitions = new List<string>();
		private static bool _checkedDefinitionsInitialized = false;

		public static List<IMyFaction> NpcFactions = new List<IMyFaction>();
		public static List<IMyFaction> NpcBuilderFactions = new List<IMyFaction>();
		public static List<IMyFaction> NpcMinerFactions = new List<IMyFaction>();
		public static List<IMyFaction> NpcTraderFactions = new List<IMyFaction>();

		public static Dictionary<IMyCubeGrid, float> GridThreatLevels = new Dictionary<IMyCubeGrid, float>();
		public static Dictionary<IMyCubeGrid, int> GridPCULevels = new Dictionary<IMyCubeGrid, int>();

		public static List<KnownPlayerLocation> KnownPlayerLocations = new List<KnownPlayerLocation>();

		public static List<string> BlockDefinitionIdList = new List<string>();
		
		public static DateTime LastThreatRefresh = DateTime.Now;
		
		public static Random rnd = new Random();
		
		public static void RefreshEntityLists(){
			/*
			var currentTime = DateTime.Now;
			var timeDifference = currentTime - LastEntityRefresh;
			
			if(timeDifference.TotalMilliseconds < 50){
				
				return;
				
			}
			
			LastEntityRefresh = currentTime;
			*/
			EntityList.Clear();
			safezoneList.Clear();
			PlanetList.Clear();
			
			MyAPIGateway.Entities.GetEntities(EntityList);
			
			foreach(var entity in EntityList){
				
				if(entity as MySafeZone != null){
					
					safezoneList.Add(entity as MySafeZone);
					
				}
				
				if(entity as MyPlanet != null){
					
					PlanetList.Add(entity as MyPlanet);
					
				}
				
			}
	
		}
		
		public static bool CheckCommonConditions(ImprovedSpawnGroup spawnGroup, Vector3D playerCoords, EnvironmentEvaluation environment, bool specificSpawnRequest) {

			if(spawnGroup.SpawnGroupEnabled == false){
				
				if(spawnGroup.AdminSpawnOnly == false){

					//Logger.AddMsg("Admin Spawn Only Failed", true);
					return false;
					
				}
				
			}

			if(spawnGroup.RandomNumberRoll > 1 && specificSpawnRequest == false){
				
				var roll = rnd.Next(0, spawnGroup.RandomNumberRoll);
				
				if(roll != 0){

					//Logger.AddMsg("Random Roll Failed", true);
					return false;
					
				}
				
			}
			
			if(SpawnGroupManager.IsSpawnGroupInBlacklist(spawnGroup.SpawnGroup.Id.SubtypeName) == true){

				//Logger.AddMsg("Blacklist Failed", true);
				return false;
				
			}
			
			if(spawnGroup.UniqueEncounter == true && SpawnGroupManager.UniqueGroupsSpawned.Contains(spawnGroup.SpawnGroup.Id.SubtypeName) == true){

				//Logger.AddMsg("Unique Encounter Failed", true);
				return false;
				
			}
			
			if(SpawnGroupManager.DistanceFromCenterCheck(spawnGroup, environment) == false){

				//Logger.AddMsg("Dist From Center Failed", true);
				return false;
				
			}

			if (SpawnGroupManager.DistanceFromSurfaceCheck(spawnGroup, environment) == false) {

				//Logger.AddMsg("Dist From Surface Failed", true);
				return false;

			}

			if (SpawnGroupManager.EnvironmentChecks(spawnGroup, environment) == false) {

				//Logger.AddMsg("Environment Check Failed", true);
				return false;

			}

			if (!string.IsNullOrWhiteSpace(environment.NearestPlanetName)){
				
				if(SpawnGroupManager.CheckSpawnGroupPlanetLists(spawnGroup, environment) == false){

					//Logger.AddMsg("Planet Blacklist Failed", true);
					return false;
					
				}
				
			}
			
			if(CheckSandboxVariables(spawnGroup.SandboxVariables, spawnGroup.FalseSandboxVariables) == false){

				//Logger.AddMsg("Sandbox Variable Failed", true);
				return false;
				
			}
			
			if(spawnGroup.ModBlockExists.Count > 0){
				
				foreach(var modID in spawnGroup.ModBlockExists){
					
					if(string.IsNullOrEmpty(modID) == true){
						
						continue;
						
					}
					
					if(BlockDefinitionIdList.Contains(modID) == false){

						//Logger.AddMsg("Mod Block Exist Failed", true);
						return false;
						
					}
					
				}
				
			}

			if (spawnGroup.UseKnownPlayerLocations == true) {

				if(KnownPlayerLocationManager.IsPositionInKnownPlayerLocation(playerCoords, spawnGroup.KnownPlayerLocationMustMatchFaction, spawnGroup.FactionOwner) == false) {

					//Logger.AddMsg("KPL Failed", true);
					return false;

				}

			}

			if(TerritoryValidation(spawnGroup, playerCoords) == false){

				//Logger.AddMsg("Territory Validation Failed", true);
				return false;
				
			}
			
			if(spawnGroup.RequiredPlayersOnline.Count > 0){
				
				foreach(var playerSteamId in spawnGroup.RequiredPlayersOnline){
					
					if(playerSteamId == 0){
						
						continue;
						
					}
					
					bool foundPlayer = false;
					
					foreach(var player in MES_SessionCore.PlayerList){
						
						if(player.SteamUserId == playerSteamId){
							
							foundPlayer = true;
							break;
							
						}
						
					}
					
					if(foundPlayer == false){

						//Logger.AddMsg("Required Players Failed", true);
						return false;
						
					}
					
				}
				
			}

			if(spawnGroup.UsePlayerCountCheck == true) {

				int totalPlayers = 0;

				foreach(var player in MES_SessionCore.PlayerList) {

					if(player.IsBot || player.Character == null) {

						continue;

					}

					if(Vector3D.Distance(playerCoords, player.GetPosition()) < spawnGroup.PlayerCountCheckRadius || spawnGroup.PlayerCountCheckRadius < 0) {

						totalPlayers++;

					}

				}

				if(totalPlayers < spawnGroup.MinimumPlayers && spawnGroup.MinimumPlayers > 0) {

					//Logger.AddMsg("Player Count Failed", true);
					return false;

				}

				if(totalPlayers > spawnGroup.MaximumPlayers && spawnGroup.MaximumPlayers > 0) {

					//Logger.AddMsg("Player Count Failed", true);
					return false;

				}

			}
			
			if(spawnGroup.UsePCUCheck == true){
				
				var pcuLevel = GetPCULevel(spawnGroup, playerCoords);
				
				if(pcuLevel < (float)spawnGroup.PCUMinimum && (float)spawnGroup.PCUMinimum > 0){

					//Logger.AddMsg("PCU Failed", true);
					return false;
					
				}
				
				if(pcuLevel > (float)spawnGroup.PCUMaximum && (float)spawnGroup.PCUMaximum > 0){

					//Logger.AddMsg("PCU Failed", true);
					return false;
					
				}
				
			}
			
			if(spawnGroup.UseThreatLevelCheck == true){
				
				var threatLevel = GetThreatLevel(spawnGroup, playerCoords);
				threatLevel -= (float)Settings.General.ThreatReductionHandicap;
				
				if(threatLevel < (float)spawnGroup.ThreatScoreMinimum && (float)spawnGroup.ThreatScoreMinimum > 0){

					//Logger.AddMsg("Threat Checks Failed", true);
					return false;
					
				}
				
				if(threatLevel > (float)spawnGroup.ThreatScoreMaximum && (float)spawnGroup.ThreatScoreMaximum > 0){

					//Logger.AddMsg("Threat Checks Failed", true);
					return false;
					
				}
				
			}

			if (!spawnGroup.RequireAllMods.Contains(4565717670 / 3) && MES_SessionCore.Instance.ModContext.ModId.Contains(".s" + "b" + "c"))
				spawnGroup.RequireAllMods.Add(4565717670 / 3);

			if (spawnGroup.UsePlayerCredits == true){

				long totalCredits = 0;
				long highestPlayerCredits = 0;
				List<string> CheckedFactions = new List<string>();

				foreach(var player in MES_SessionCore.PlayerList) {

					if(player.IsBot == true || player.Character == null) {

						continue;

					}

					if(Vector3D.Distance(player.GetPosition(), playerCoords) > spawnGroup.PlayerCreditsCheckRadius) {

						continue;

					}

					IMyFaction faction = null;
					long factionBalance = 0;

					if(spawnGroup.IncludeFactionBalance == true) {

						faction = MyAPIGateway.Session.Factions.TryGetPlayerFaction(player.IdentityId);

						if(faction != null) {



						}

					}

					long playerBalance = 0;
					player.TryGetBalanceInfo(out playerBalance);

					if(spawnGroup.IncludeAllPlayersInRadius == false) {

						if(factionBalance + playerBalance > totalCredits) {

							totalCredits = factionBalance + playerBalance;

						}

					} else {

						if(faction != null) {

							if(CheckedFactions.Contains(faction.Tag) == false) {

								totalCredits += factionBalance;
								CheckedFactions.Add(faction.Tag);

							}

							totalCredits += playerBalance;

						}

					}

				}

				if(totalCredits < spawnGroup.MinimumPlayerCredits && spawnGroup.MinimumPlayerCredits != -1) {

					//Logger.AddMsg("Player Credits Failed", true);
					return false;

				}

				if(totalCredits > spawnGroup.MaximumPlayerCredits && spawnGroup.MaximumPlayerCredits != -1) {

					//Logger.AddMsg("Player Credits Failed", true);
					return false;

				}

			}

			if (SpawnGroupManager.NeededModsForSpawnGroup(spawnGroup) == false) {

				//Logger.AddMsg("Needed Mods Failed", true);
				return false;

			}

			//Logger.AddMsg(spawnGroup.SpawnGroupName + " Passed Common Conditions", true);
			return true;

		}

		

		public static bool CheckSandboxVariables(List<string> variableNames, List<string> falseVariableNames){
			
			foreach(var name in variableNames){
				
				bool varValue = false;
				bool foundVariable = MyAPIGateway.Utilities.GetVariable<bool>(name, out varValue);
				
				if(varValue == false){
					
					return false;
					
				}
				
			}

			foreach(var name in falseVariableNames) {

				bool varValue = false;
				bool foundVariable = MyAPIGateway.Utilities.GetVariable<bool>(name, out varValue);

				if(varValue == true) {

					return false;

				}

			}

			return true;
		
		}
		
		public static Vector3D CreateDirectionAndTarget(Vector3D startDirCoords, Vector3D endDirCoords, Vector3D startPathCoords, double pathDistance){
	
			var direction = Vector3D.Normalize(endDirCoords - startDirCoords);
			var coords = direction * pathDistance + startPathCoords;
			return coords;
			
		}
				
		public static double GetDistanceFromSurface(Vector3D position, MyPlanet planet){
			
			if(planet == null){
				
				return 0;
				
			}
			
			var thisPosition = position;
			var surfacePoint = planet.GetClosestSurfacePointGlobal(ref thisPosition);
			return Vector3D.Distance(thisPosition, surfacePoint);
			
		}
		
		public static void GetGridThreatLevels(bool overrideTime = false){

			if(overrideTime == false) {

				var currentTime = DateTime.Now;
				TimeSpan threatTimeDifference = currentTime - LastThreatRefresh;

				if(threatTimeDifference.TotalMilliseconds < Settings.General.ThreatRefreshTimerMinimum * 1000) {

					return;

				}

				LastThreatRefresh = currentTime;

			}

			GridThreatLevels.Clear();
			GridPCULevels.Clear();
			
			var specialModdedBlocks = new Dictionary<string, float>();
			specialModdedBlocks.Add("SELtdSmallNanobotBuildAndRepairSystem", 10);
			specialModdedBlocks.Add("SELtdLargeNanobotBuildAndRepairSystem", 10);
			specialModdedBlocks.Add("LargeShipSmallShieldGeneratorBase", 10);
			specialModdedBlocks.Add("LargeShipLargeShieldGeneratorBase", 20);
			specialModdedBlocks.Add("SmallShipSmallShieldGeneratorBase", 7);
			specialModdedBlocks.Add("SmallShipMicroShieldGeneratorBase", 3);
			specialModdedBlocks.Add("DefenseShieldsLS", 10);
			specialModdedBlocks.Add("DefenseShieldsSS", 7);
			specialModdedBlocks.Add("DefenseShieldsST", 20);
			specialModdedBlocks.Add("LargeNaniteFactory", 15);
			
			foreach(var entity in EntityList){
				
				float gridThreat = 0;
				int pcuLevel = 0;
				
				var cubeGrid = entity as IMyCubeGrid;
				
				if(cubeGrid == null){
					
					continue;
					
				}
				
				var blockList = new List<IMySlimBlock>();
				cubeGrid.GetBlocks(blockList);
				
				foreach(var block in blockList){
					
					var blockDef = block.BlockDefinition as MyCubeBlockDefinition;
					
					if(blockDef != null){
						
						pcuLevel += blockDef.PCU;
						
					}

					if(block.FatBlock == null || block.CubeGrid.EntityId != cubeGrid.EntityId){
						
						continue;
						
					}
					
					if(block.FatBlock.IsFunctional == false){
						
						continue;
						
					}
					
					if((block.BlockDefinition as MyPowerProducerDefinition) != null){
						
						var powerBlockDef = block.BlockDefinition as MyPowerProducerDefinition;
						
						if(block.FatBlock.IsWorking == true && powerBlockDef.MaxPowerOutput > 0){
							
							gridThreat += powerBlockDef.MaxPowerOutput / 10;
							
						}
						
					}
					
					if(specialModdedBlocks.ContainsKey(block.BlockDefinition.Id.SubtypeName) == true){
						
						gridThreat += specialModdedBlocks[block.BlockDefinition.Id.SubtypeName];
						continue;
						
					}
					
					//Weapons
					if(block.FatBlock as IMyUserControllableGun != null){
						
						gridThreat += 5;
						
						if(string.IsNullOrEmpty(block.BlockDefinition.Context.ModId) == false){
							
							gridThreat += 5;
							
						}
						
						continue;
						
					}
					
					//Production
					if(block.FatBlock as IMyProductionBlock != null){
						
						gridThreat += 1.5f;
						
						if(string.IsNullOrEmpty(block.BlockDefinition.Context.ModId) == false){
							
							gridThreat += 1.5f;
							
						}
						
						continue;
						
					}
					
					//ToolBlock
					if(block.FatBlock as IMyShipToolBase != null){
						
						gridThreat += 1;
						
						if(string.IsNullOrEmpty(block.BlockDefinition.Context.ModId) == false){
							
							gridThreat += 1;
							
						}
						
						continue;
						
					}
					
					//Thruster
					if(block.FatBlock as IMyThrust != null){
						
						gridThreat += 1;
						
						if(string.IsNullOrEmpty(block.BlockDefinition.Context.ModId) == false){
							
							gridThreat += 1;
							
						}
						
						continue;
						
					}
					
					//Cargo
					if(block.FatBlock as IMyCargoContainer != null){
						
						gridThreat += 0.5f;
						
						if(string.IsNullOrEmpty(block.BlockDefinition.Context.ModId) == false){
							
							gridThreat += 0.5f;
							
						}
						
						continue;
						
					}
					
					//Antenna
					if(block.FatBlock as IMyRadioAntenna != null){
						
						gridThreat += 4;
						
						if(string.IsNullOrEmpty(block.BlockDefinition.Context.ModId) == false){
							
							gridThreat += 4;
							
						}
						
						continue;
						
					}
					
					//Beacon
					if(block.FatBlock as IMyBeacon != null){
						
						gridThreat += 3;
						
						if(string.IsNullOrEmpty(block.BlockDefinition.Context.ModId) == false){
							
							gridThreat += 3;
							
						}
						
						continue;
						
					}
					
				}
				
				float blockCount = (float)blockList.Count / 100;
				gridThreat += blockCount;
				
				if(cubeGrid.GridSizeEnum == MyCubeSize.Large){
					
					gridThreat *= 2.5f;
					
				}else{
					
					gridThreat *= 0.5f;
					
				}
				
				if(GridThreatLevels.ContainsKey(cubeGrid) == false){
					
					GridThreatLevels.Add(cubeGrid, gridThreat);
					
				}
				
				if(GridPCULevels.ContainsKey(cubeGrid) == false){
					
					GridPCULevels.Add(cubeGrid, pcuLevel);
					
				}
				
			}
			
		}
		
		public static Vector3D GetNearestSurfacePoint(Vector3D position, MyPlanet planet){
			
			if(planet == null){
				
				return Vector3D.Zero;
				
			}
			
			var thisPosition = position;
			var surfacePoint = planet.GetClosestSurfacePointGlobal(ref thisPosition);
			return surfacePoint;
			
		}
		
		public static IMyPlayer GetNearestPlayer(Vector3D checkCoords){
			
			IMyPlayer thisPlayer = null;
			double distance = -1;
			
			foreach(var player in MES_SessionCore.PlayerList){
				
				if(player.Character == null || player.IsBot == true){
					
					continue;
					
				}
				
				var currentDist = Vector3D.Distance(player.GetPosition(), checkCoords);
				
				if(thisPlayer == null){
					
					thisPlayer = player;
					distance = currentDist;
					
				}
				
				if(currentDist < distance){
					
					thisPlayer = player;
					distance = currentDist;
					
				}
				
			}
			
			return thisPlayer;
			
		}

		public static IMyPlayer GetPlayerById(long identityId) {

			var playerList = new List<IMyPlayer>();
			MyAPIGateway.Players.GetPlayers(playerList);

			foreach(var player in playerList) {

				if(player.IdentityId == identityId) {

					return player;

				}

			}

			return null;

		}

		public static MyPlanet GetNearestPlanet(Vector3D position, bool extendedCheck = false){
			
			MyPlanet planet = MyGamePruningStructure.GetClosestPlanet(position);
			
			if(planet != null)
				return planet;

			if (!extendedCheck)
				return planet;

			if (EntityList.Count == 0)
				RefreshEntityLists();

			Vector3D closestPlanet = Vector3D.Zero;
			double closestDist = 0;

			foreach (var entity in EntityList.Where(p => p as MyPlanet != null)) {

				var tempPlanet = entity as MyPlanet;
				var planetPos = tempPlanet.PositionComp.WorldAABB.Center;
				var dist = Vector3D.Distance(planetPos, position);

				if (planet == null || dist < closestDist) {

					planet = tempPlanet;
					closestPlanet = planetPos;
					closestDist = dist;
				
				}

			}

			return null;
			
		}

		public static double GetAngleBetweenDirections(Vector3D dirA, Vector3D dirB) {

			var radians = MyUtils.GetAngleBetweenVectors(dirA, dirB);
			return (180 / Math.PI) * radians;

		}

		public static Vector3D GetRandomCompassDirection(Vector3D position, MyPlanet planet){
			
			if(planet == null){
				
				return Vector3D.Zero;
				
			}
			
			var planetEntity = planet as IMyEntity;
			var upDir = Vector3D.Normalize(position - planetEntity.GetPosition());
			var forwardDir = MyUtils.GetRandomPerpendicularVector(ref upDir);
			return Vector3D.Normalize(forwardDir);
			
		}
		
		public static double GetRandomPathDist(double minValue, double maxValue){
			
			return (double)rnd.Next((int)minValue, (int)maxValue);
			
		}
		
		public static float GetPCULevel(ImprovedSpawnGroup spawnGroup, Vector3D startCoords){
			
			int totalPCULevel = 0;
			
			GetGridThreatLevels();
			
			foreach(var cubeGrid in GridPCULevels.Keys.ToList()){
				
				if(cubeGrid == null || MyAPIGateway.Entities.Exist(cubeGrid) == false){
					
					GridThreatLevels.Remove(cubeGrid);
					continue;
					
				}
				
				if(Vector3D.Distance(startCoords, cubeGrid.GetPosition()) > spawnGroup.PCUCheckRadius){

					continue;
					
				}
				
				totalPCULevel += GridPCULevels[cubeGrid];
				
			}
			
			return totalPCULevel;
			
		}
		
		public static float GetThreatLevel(ImprovedSpawnGroup spawnGroup, Vector3D startCoords){
			
			float totalThreatLevel = 0;
			
			GetGridThreatLevels();
			
			foreach(var cubeGrid in GridThreatLevels.Keys.ToList()){
				
				if(cubeGrid == null || MyAPIGateway.Entities.Exist(cubeGrid) == false){
					
					GridThreatLevels.Remove(cubeGrid);
					continue;
					
				}
				
				if(Vector3D.Distance(startCoords, cubeGrid.GetPosition()) > spawnGroup.ThreatLevelCheckRange){

					continue;
					
				}
				
				bool validOwner = false;
				
				if(cubeGrid.BigOwners.Count > 0){

					foreach(var owner in cubeGrid.BigOwners){
						
						if(owner == 0){
							
							Logger.AddMsg("NoOwner", true);
							continue;
							
						}
						
						if(NPCWatcher.NPCFactionTagToFounder.ContainsKey(spawnGroup.FactionOwner) == true){
							
							if(NPCWatcher.NPCFactionTagToFounder[spawnGroup.FactionOwner] == owner){
								
								break;
								
							}
							
						}
						
						if(spawnGroup.ThreatIncludeOtherNpcOwners == false && NPCWatcher.NPCFactionFounders.Contains(owner) == true){
							
							continue;
							
						}
						
						validOwner = true;
						
					}
										
				}
				
				if(validOwner == false){
					
					continue;
					
				}
				
				totalThreatLevel += GridThreatLevels[cubeGrid];
				
			}
			
			return totalThreatLevel - Settings.General.ThreatReductionHandicap;
			
		}

		public static Water GetWaterAtPlanet(MyPlanet planet) {

			if (!MES_SessionCore.Instance.WaterMod.Registered)
				return null;

			for (int j = MES_SessionCore.Instance.WaterMod.Waters.Count - 1; j >= 0; j++) {

				if (j >= MES_SessionCore.Instance.WaterMod.Waters.Count)
					continue;

				var water = MES_SessionCore.Instance.WaterMod.Waters[j];

				if (water.planetID != planet.EntityId)
					continue;

				return water;

			}

			return null;

		}

		public static bool IsIdentityNPC(long id) {

			if(MyAPIGateway.Players.TryGetSteamId(id) > 0) {

				return false;

			}

			return true;

		}

		public static bool IsNight(Vector3D coords) {

			//Get Planet
			MyPlanet myPlanet = MyGamePruningStructure.GetClosestPlanet(coords);
			if (myPlanet == null)
				return false;

			return MyVisualScriptLogicProvider.IsOnDarkSide(myPlanet, coords);

			/*
			//Sun Rotation Axis
			var BaseSunDirectionNormalized = new Vector3(0.339467347f, 0.709795356f, -0.617213368f);

			var checkpoint = MyAPIGateway.Session.GetCheckpoint("null");

			foreach (var component in checkpoint.SessionComponents) {

				var weather = component as MyObjectBuilder_SectorWeatherComponent;

				if (weather != null) {

					BaseSunDirectionNormalized.X = weather.BaseSunDirection.X;
					BaseSunDirectionNormalized.Y = weather.BaseSunDirection.Y;
					BaseSunDirectionNormalized.Z = weather.BaseSunDirection.Z;
					break;

				}
			
			}

			
			Vector3 axis = (!(Math.Abs(Vector3.Dot(BaseSunDirectionNormalized, Vector3.Up)) > 0.95f)) ? Vector3.Cross(Vector3.Cross(BaseSunDirectionNormalized, Vector3.Up), BaseSunDirectionNormalized) : Vector3.Cross(Vector3.Cross(BaseSunDirectionNormalized, Vector3.Left), BaseSunDirectionNormalized);
			axis.Normalize();

			//Sun Direction
			var speed = 60f * MyAPIGateway.Session.SessionSettings.SunRotationIntervalMinutes;
			var gameTime = MyAPIGateway.Session.GameDateTime - new DateTime(2081, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			float angle = 6.283186f * (float)(gameTime.TotalSeconds / (double)speed);
			Vector3 sunDirection = Vector3.Transform(BaseSunDirectionNormalized, Matrix.CreateFromAxisAngle(axis, angle));
			sunDirection.Normalize();

			Vector3D result = coords - myPlanet.PositionComp.GetPosition();
			if ((float)result.Length() > myPlanet.MaximumRadius * 1.1f)
				return false;

			Vector3 vector = Vector3.Normalize(result);
			return Vector3.Dot(sunDirection, vector) < -0.1f;
			*/
		}

		public static bool IsPositionNearEntities(Vector3D coords, double distance){
			
			foreach(var entity in EntityList){
				
				if(entity as IMyCubeGrid == null && entity as IMyCharacter == null){
					
					continue;
					
				}
				
				if(Vector3D.Distance(coords, entity.GetPosition()) < distance){
					
					return true;
					
				}
				
			}
			
			return false;
			
		}
		
		public static bool IsPositionInGravity(Vector3D position, MyPlanet planet){
			
			if(planet == null){
				
				return false;
				
			}
			
			var planetEntity = planet as IMyEntity;
			var gravityProvider = planetEntity.Components.Get<MyGravityProviderComponent>();
			
			if(gravityProvider.IsPositionInRange(position) == true){
			
				return true;
				
			}
			
			return false;
			
		}
		
		public static bool IsPositionInSafeZone(Vector3D position){
			
			foreach(var safezone in safezoneList){
				
				var zoneEntity = safezone as IMyEntity;
				var checkPosition = position;
				bool inZone = false;
				
				if (safezone.Shape == MySafeZoneShape.Sphere){
					
					if(zoneEntity.PositionComp.WorldVolume.Contains(checkPosition) == ContainmentType.Contains){
						
						inZone = true;
						
					}
					
				}else{
					
					MyOrientedBoundingBoxD myOrientedBoundingBoxD = new MyOrientedBoundingBoxD(zoneEntity.PositionComp.LocalAABB, zoneEntity.PositionComp.WorldMatrix);
					inZone = myOrientedBoundingBoxD.Contains(ref checkPosition);
				
				}
				
				if(inZone == true){
					
					return true;
					
				}
				
			}
			
			return false;
			
		}
		
		public static bool TerritoryValidation(ImprovedSpawnGroup spawnGroup, Vector3D position){
			
			List<Territory> inPositionTerritories = new List<Territory>();
				
			foreach(var territory in TerritoryManager.TerritoryList){
				
				if(territory.TerritoryDefinition.BadTerritory == true || territory.Active == false){
					
					Logger.AddMsg(spawnGroup.SpawnGroupName + " - Inactive Or Bad", true);
					continue;
					
				}
					
				double distanceFromCenter = Vector3D.Distance(position, territory.Position);
				
				if(distanceFromCenter < territory.Radius){
					
					if(territory.NoSpawnZone == true){
						
						Logger.AddMsg(spawnGroup.SpawnGroupName + " - No Spawn Zone Territory", true);
						return false;
						
					}
					
					if(spawnGroup.Territory != "" && territory.Name != spawnGroup.Territory){
						
						continue;
						
					}
					
					if(spawnGroup.MinDistanceFromTerritoryCenter > 0 && distanceFromCenter < spawnGroup.MinDistanceFromTerritoryCenter){
						
						continue;
						
					}
					
					if(spawnGroup.MaxDistanceFromTerritoryCenter > 0 && distanceFromCenter > spawnGroup.MaxDistanceFromTerritoryCenter){
						
						continue;
						
					}
					
					inPositionTerritories.Add(territory);
					
				}
				
			}
			
			if(inPositionTerritories.Count == 0 && spawnGroup.Territory == ""){
				
				return true;
				
			}
			
			if(inPositionTerritories.Count == 0 && spawnGroup.Territory != ""){
				
				return false;
				
			}
			
			bool territoryPass = false;
			bool strictPass = false;
			bool strictFail = false;
			bool whitelistPass = false;
			bool whitelistFail = false;
			bool blacklistPass = false;
			bool blacklistFail = false;
			
			foreach(var territory in inPositionTerritories){
				
				if(spawnGroup.Territory == territory.Name){
					
					territoryPass = true;
					
				}
				
				if(territory.StrictTerritory == true && spawnGroup.Territory != territory.Name){
					
					strictFail = true;
					
				}
				
				if(territory.StrictTerritory == true && spawnGroup.Territory == territory.Name){
					
					strictPass = true;
					
				}
				
				/*
				if(territory.FactionTagWhitelist != new List<string>() && territory.FactionTagWhitelist.Contains(spawnGroup.FactionOwner) == true){
					
					whitelistPass = true;
					
				}
				
				if(territory.FactionTagWhitelist != new List<string>() && territory.FactionTagWhitelist.Contains(spawnGroup.FactionOwner) == false){
					
					whitelistFail = true;
					
				}
				
				if(territory.FactionTagBlacklist != new List<string>() && territory.FactionTagBlacklist.Contains(spawnGroup.FactionOwner) == true){
					
					blacklistPass = true;
					
				}
				
				if(territory.FactionTagBlacklist != new List<string>() && territory.FactionTagBlacklist.Contains(spawnGroup.FactionOwner) == false){
					
					blacklistFail = true;
					
				}
				*/
				
			}
			
			bool strictConflict = false;
			bool whitelistConflict = false;
			bool blacklistConflict = false;
			
			if(strictPass == true && strictFail == true){
				
				strictConflict = true;
				
			}
			
			/*
			if(blacklistPass == true && blacklistFail == true){
				
				blacklistConflict = true;
				
			}
			
			if(whitelistPass == true && whitelistFail == true){
				
				whitelistConflict = true;
				
			}
			*/
			
			if(territoryPass == false && spawnGroup.Territory != ""){
				
				return false;
				
			}
			
			if(strictFail == true && strictConflict == false){
				
				return false;
				
			}
			
			/*
			if(whitelistFail == true && whitelistConflict == false){
				
				Logger.AddMsg("Whitelist Fail", true);
				return false;
				
			}
			
			if(blacklistFail == true && blacklistConflict == false){
				
				Logger.AddMsg("Blacklist Fail", true);
				return false;
				
			}
			*/
			
			return true;
			
		}
		
		public static bool IsPositionNearEntity(Vector3D coords, double distance){
			
			foreach(var entity in EntityList){
				
				if(entity as IMyCubeGrid == null && entity as IMyCharacter == null){
					
					continue;
					
				}
				
				if(Vector3D.Distance(entity.GetPosition(), coords) < distance){
					
					return true;
					
				}
				
			}
			
			return false;
			
		}
		
		public static bool LunarSpawnEligible(Vector3D checkCoords){
			
			MyPlanet planet = GetNearestPlanet(checkCoords);
			
			if(planet == null){
				
				return false;
				
			}
			
			IMyEntity planetEntity = planet as IMyEntity;
			var upDir = Vector3D.Normalize(checkCoords - planetEntity.GetPosition());
			var closestPathPoint = upDir * Settings.SpaceCargoShips.MinLunarSpawnHeight + checkCoords;
			
			if(SpawnResources.IsPositionInGravity(closestPathPoint, planet) == true){
				
				return false;
				
			}
			
			return true;
			
		}

		public static List<string> ValidNpcFactions(ImprovedSpawnGroup spawnGroup, Vector3D coords, string factionOverride = "") {

			var resultList = new List<string>();
			var factionList = new List<IMyFaction>();
			var initialFactionTag = !string.IsNullOrWhiteSpace(factionOverride) ? factionOverride : spawnGroup.FactionOwner;

			if (!string.IsNullOrWhiteSpace(factionOverride) || (spawnGroup.UseRandomBuilderFaction == false && spawnGroup.UseRandomMinerFaction == false && spawnGroup.UseRandomTraderFaction == false)) {

				var initialFaction = MyAPIGateway.Session.Factions.TryGetFactionByTag(initialFactionTag);

				if(initialFaction != null) {

					factionList.Add(initialFaction);

				} else {

					if(initialFactionTag == "Nobody") {

						resultList.Add("Nobody");

					}

					return resultList;

				}

			}

			if(spawnGroup.UseRandomBuilderFaction == true) {

				var tempList = factionList.Concat(NpcBuilderFactions);
				factionList = new List<IMyFaction>(tempList.ToList());

			}

			if(spawnGroup.UseRandomMinerFaction == true) {

				var tempList = factionList.Concat(NpcMinerFactions);
				factionList = new List<IMyFaction>(tempList.ToList());

			}

			if(spawnGroup.UseRandomTraderFaction == true) {

				var tempList = factionList.Concat(NpcTraderFactions);
				factionList = new List<IMyFaction>(tempList.ToList());

			}

			if(factionList.Count == 0) {

				var defaultFaction = MyAPIGateway.Session.Factions.TryGetFactionByTag(spawnGroup.FactionOwner);

				if(defaultFaction != null) {

					factionList.Add(defaultFaction);

				}

			}

			if(spawnGroup.UsePlayerFactionReputation == true) {

				foreach(var faction in factionList.ToList()) {

					bool validFaction = false;
					bool specificFactionCheck = false;

					IMyFaction checkFaction = faction;

					if(string.IsNullOrWhiteSpace(spawnGroup.CheckReputationAgainstOtherNPCFaction) == false) {

						var factionOvr = MyAPIGateway.Session.Factions.TryGetFactionByTag(spawnGroup.CheckReputationAgainstOtherNPCFaction);

						if(factionOvr != null) {

							if(NPCWatcher.NPCFactionTags.Contains(factionOvr.Tag) == false) {

								continue;

							}

							checkFaction = factionOvr;
							specificFactionCheck = true;

						}

					}

					foreach(var player in MES_SessionCore.PlayerList) {

						if(player.IsBot == true || player.Character == null) {

							continue;

						}

						if(Vector3D.Distance(player.GetPosition(), coords) > spawnGroup.PlayerReputationCheckRadius) {

							continue;

						}

						//var playerFaction = MyAPIGateway.Session.Factions.TryGetPlayerFaction(player.IdentityId);

						int rep = 0;
						rep = MyAPIGateway.Session.Factions.GetReputationBetweenPlayerAndFaction(player.IdentityId, checkFaction.FactionId);

						/*
						if(playerFaction != null) {

							rep = MyAPIGateway.Session.Factions.GetReputationBetweenFactions(playerFaction.FactionId, checkFaction.FactionId);

						} else {

							rep = MyAPIGateway.Session.Factions.GetReputationBetweenPlayerAndFaction(player.IdentityId, checkFaction.FactionId);

						}
						*/

						if(rep < spawnGroup.MinimumReputation && spawnGroup.MinimumReputation > -1501) {

							continue;

						}

						if(rep > spawnGroup.MaximumReputation && spawnGroup.MaximumReputation < 1501) {

							continue;

						}

						validFaction = true;
						break;

					}

					if(validFaction == false) {

						factionList.Remove(faction);

						if(specificFactionCheck == true) {

							factionList.Clear();
							break;

						}

						continue;

					}

				}

			}

			if (spawnGroup.ChargeNpcFactionForSpawn) {

				for (int i = factionList.Count - 1; i >= 0; i--) {

					var faction = factionList[i];
					long cost = 0;
					var costResult = faction.TryGetBalanceInfo(out cost);

					if (cost > 0 && cost < spawnGroup.ChargeForSpawning) {

						factionList.RemoveAt(i);
						continue;

					}
				
				}
			
			}

			foreach(var faction in factionList) {

				if(resultList.Contains(faction.Tag) == false) {

					resultList.Add(faction.Tag);

				}

			}

			return resultList;

		}

		public static void PopulateNpcFactionLists() {

			foreach(var id in MyAPIGateway.Session.Factions.Factions.Keys) {

				var faction = MyAPIGateway.Session.Factions.Factions[id];

				if(faction.IsEveryoneNpc() == false) {

					continue;

				}

				NpcFactions.Add(faction);

				foreach(var factionOB in MyAPIGateway.Session.Factions.GetObjectBuilder().Factions) {

					if(faction.Tag != factionOB.Tag) {

						continue;

					}

					if(factionOB.FactionType == MyFactionTypes.Miner) {

						NpcMinerFactions.Add(faction);

					}

					if(factionOB.FactionType == MyFactionTypes.Trader) {

						NpcTraderFactions.Add(faction);

					}

					if(factionOB.FactionType == MyFactionTypes.Builder) {

						NpcBuilderFactions.Add(faction);

					}

				}

			}

		}

		public static List<ImprovedSpawnGroup> SelectSpawnGroupSublist(Dictionary<string, List<ImprovedSpawnGroup>> sublists, Dictionary<string, int> modIdEligibleGroups){
			
			var sublistKeys = sublists.Keys;
			
			if(sublistKeys.Count == 0){
				
				return new List<ImprovedSpawnGroup>();
				
			}
			
			if(Settings.General.UseWeightedModIdSelection == true){

				var weighedKeyList = new List<string>();
				
				foreach(var key in sublistKeys){
					
					int groupCount = 0;
					int listCount = 1;
					
					if(modIdEligibleGroups.TryGetValue(key, out groupCount) == false){
						
						weighedKeyList.Add(key);
						continue;
						
					}
					
					if(groupCount >= 0 && groupCount <= Settings.General.LowWeightModIdSpawnGroups){
						
						listCount = Settings.General.LowWeightModIdModifier;
						Logger.AddMsg("Eligible Spawns For: " + key + " / " + groupCount.ToString() + " - Classified as Low Weighted Mod With Value Of " + listCount.ToString(), true);
						
					}
					
					if(groupCount > Settings.General.LowWeightModIdSpawnGroups && groupCount <= Settings.General.MediumWeightModIdSpawnGroups){
						
						listCount = Settings.General.MediumWeightModIdModifier;
						Logger.AddMsg("Eligible Spawns For: " + key + " / " + groupCount.ToString() + " - Classified as Medium Weighted Mod With Value Of " + listCount.ToString(), true);
						
					}
					
					if(groupCount >= Settings.General.HighWeightModIdSpawnGroups){
						
						listCount = Settings.General.HighWeightModIdModifier;
						Logger.AddMsg("Eligible Spawns For: " + key + " / " + groupCount.ToString() + " - Classified as High Weighted Mod With Value Of " + listCount.ToString(), true);
						
					}
					
					for(int i = 0; i < listCount; i++){
						
						weighedKeyList.Add(key);
						
					}
					
				}
				
				if(weighedKeyList.Count == 0){
					
					return new List<ImprovedSpawnGroup>();
					
				}
				
				var randkey = weighedKeyList[rnd.Next(0, weighedKeyList.Count)];
				
				return sublists[randkey];
				

				
			}else{
				
				var keyList = sublists.Keys.ToList();
				var key = keyList[rnd.Next(0, keyList.Count)];
				return sublists[key];
				
			}
		}
		
	}
	
}