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
	
	public static class BossEncounterSpawner{
		
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
			
			if(NPCWatcher.ActiveNpcTypeLimitReachedForArea("BossEncounter", startCoords, Settings.BossEncounters.MaxShipsPerArea, Settings.BossEncounters.AreaSize) == true){
				
				return "Too Many Boss Encounter Grids in Player Area";
				
			}
			
			var spawnCoords = Vector3D.Zero;
			
			if(GetInitialSpawnCoords(startCoords, out spawnCoords) == false){
				
				return "Could Not Find Valid Coords For Boss Encounter Signal Generation.";
				
			}
			
			var spawnGroupList = GetBossEncounters(startCoords, spawnCoords, eligibleNames);
			
			if(Settings.General.UseModIdSelectionForSpawning == true){
				
				spawnGroupList = SpawnResources.SelectSpawnGroupSublist(SpawnGroupSublists, EligibleSpawnsByModId);
				
			}

			if(spawnGroupList.Count == 0){
				
				return "No Eligible Spawn Groups Could Be Found To Spawn Near Player.";
				
			}
			
			var spawnGroup = spawnGroupList[SpawnResources.rnd.Next(0, spawnGroupList.Count)];
			
			var bossEncounter = new BossEncounter();
			bossEncounter.SpawnGroup = spawnGroup;
			bossEncounter.SpawnGroupName = spawnGroup.SpawnGroupName;
			bossEncounter.Position = spawnCoords;
			
			foreach(var player in MES_SessionCore.PlayerList){
				
				if(player.IsBot == true || player.Character == null || IsPlayerInBossEncounter(player.IdentityId) == true){
					
					continue;
					
				}
				
				if(Vector3D.Distance(player.GetPosition(), spawnCoords) < Settings.BossEncounters.PlayersWithinDistance){
					
					bossEncounter.PlayersInEncounter.Add(player.IdentityId);
					
				}else{
					
					continue;
					
				}
				
				if(spawnGroup.BossCustomAnnounceEnable == true){
					
					MyVisualScriptLogicProvider.SendChatMessage(spawnGroup.BossCustomAnnounceMessage, spawnGroup.BossCustomAnnounceAuthor, player.IdentityId, "Red");
					
				}
				
				/*
				var syncData = new SyncData();
				syncData.Instruction = "MESBossGPSCreate";
				syncData.GpsName = spawnGroup.BossCustomGPSLabel;
				syncData.GpsCoords = spawnCoords;
				var sendData = MyAPIGateway.Utilities.SerializeToBinary<SyncData>(syncData);
				bool sendStatus = MyAPIGateway.Multiplayer.SendMessageTo(8877, sendData, player.SteamUserId);
				*/

			}

			bossEncounter.CreateGpsForPlayers();
			NPCWatcher.BossEncounters.Add(bossEncounter);

			try {

				if(NPCWatcher.BossEncounters.Count > 0) {

					BossEncounter[] encounterArray = NPCWatcher.BossEncounters.ToArray();
					var byteArray = MyAPIGateway.Utilities.SerializeToBinary<BossEncounter[]>(encounterArray);
					var storedBossData = Convert.ToBase64String(byteArray);
					MyAPIGateway.Utilities.SetVariable<string>("MES-ActiveBossEncounters", storedBossData);

				} else {

					MyAPIGateway.Utilities.SetVariable<string>("MES-ActiveBossEncounters", "");

				}

			} catch(Exception e) {

				Logger.AddMsg("Something went wrong while getting Boss Encounter Data from Storage.");
				Logger.AddMsg(e.ToString(), true);

			}

		   

			Logger.SkipNextMessage = false;
			return "Boss Encounter GPS Created with Spawngroup: " + spawnGroup.SpawnGroup.Id.SubtypeName;
		
		}
		
		public static List<ImprovedSpawnGroup> GetBossEncounters(Vector3D playerCoords, Vector3D spawnCoords, List<string> eligibleNames){

			bool spaceSpawn = false;
			bool planetSpawn = false;
			string specificGroup = "";
			var planetRestrictions = new List<string>(Settings.General.PlanetSpawnsDisableList.ToList());
			SpawnGroupSublists.Clear();
			EligibleSpawnsByModId.Clear();
			var environment = new EnvironmentEvaluation(playerCoords);
			
			if(environment.NearestPlanet != null){
				
				if(planetRestrictions.Contains(environment.NearestPlanetName) && environment.IsOnPlanet){
					
					return new List<ImprovedSpawnGroup>();
					
				}
				
			}
			
			bool specificSpawnRequest = false;
			
			if(SpawnGroupManager.AdminSpawnGroup != ""){
				
				specificGroup = SpawnGroupManager.AdminSpawnGroup;
				SpawnGroupManager.AdminSpawnGroup = "";
				specificSpawnRequest = true;
				
			}
			
			if(environment.IsOnPlanet) {
				
				if(environment.AtmosphereAtPosition > Settings.BossEncounters.MinAirDensity){
					
					planetSpawn = true;
					
				}

			}else{
				
				spaceSpawn = true;
				
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

				bool eligibleGroup = false;
				
				if(spawnGroup.BossEncounterSpace == true && spaceSpawn == true){
					
					eligibleGroup = true;
					
				}
				
				if(spawnGroup.BossEncounterAtmo == true && planetSpawn == true){
					
					eligibleGroup = true;
					
				}
				
				if(spawnGroup.BossEncounterAny == true){
					
					eligibleGroup = true;
					
				}
				
				if(eligibleGroup == false){
					
					continue;
					
				}
				
				if(SpawnResources.CheckCommonConditions(spawnGroup, playerCoords, environment, specificSpawnRequest) == false){
					
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

					if(Settings.BossEncounters.UseMaxSpawnGroupFrequency == true && spawnGroup.Frequency > Settings.BossEncounters.MaxSpawnGroupFrequency * 10){
						
						spawnGroup.Frequency = (int)Math.Round((double)Settings.BossEncounters.MaxSpawnGroupFrequency * 10);
						
					}
					
					for(int i = 0; i < spawnGroup.Frequency; i++){
						
						eligibleGroups.Add(spawnGroup);
						SpawnGroupSublists[modID].Add(spawnGroup);
						
					}
					
				}
				
			}
			
			return eligibleGroups;
			
		}
		
		public static void RemoveGPSFromEncounter(BossEncounter encounter){
			
			foreach(var player in MES_SessionCore.PlayerList){
				
				if(player.IsBot == true || player.Character == null){
					
					continue;
					
				}
				
				if(encounter.PlayersInEncounter.Contains(player.IdentityId) == true){
					
					var sendData = MyAPIGateway.Utilities.SerializeToBinary<string>("MESBossGPSRemove\nNa\nNa");
					bool sendStatus = MyAPIGateway.Multiplayer.SendMessageTo(8877, sendData, player.SteamUserId);
					
				}
				
			}
			
		}
		
		public static bool SpawnBossEncounter(BossEncounter encounter){
			
			MyPlanet planet = SpawnResources.GetNearestPlanet(encounter.Position);
			var inGravity = SpawnResources.IsPositionInGravity(encounter.Position, planet);
			
			for(int i = 0; i < Settings.BossEncounters.PathCalculationAttempts; i++){
				
				bool gotMatrix = false;
				var tempMatrix = MatrixD.CreateWorld(Vector3D.Zero, Vector3D.Forward, Vector3D.Up);
				
				if(inGravity == false){
					
					var randDir = Vector3D.Normalize(MyUtils.GetRandomVector3D());
					var randDist = (double)SpawnResources.rnd.Next((int)Settings.BossEncounters.MinSpawnDistFromCoords, (int)Settings.BossEncounters.MaxSpawnDistFromCoords);
					var spawnCoords = randDir * randDist + encounter.Position;
					
					if(SpawnResources.IsPositionInGravity(spawnCoords, planet) == true){
						
						randDir *= -1;
						spawnCoords = randDir * randDist + encounter.Position;
						
						if(SpawnResources.IsPositionInGravity(spawnCoords, planet) == true){
							
							continue;
							
						}
						
					}
					
					var forwardDir = Vector3D.Normalize(encounter.Position - spawnCoords);
					var upDir = Vector3D.CalculatePerpendicularVector(forwardDir);
					tempMatrix = MatrixD.CreateWorld(spawnCoords, forwardDir, upDir);
					gotMatrix = true;
					
				}else{
					
					var planetEntity = planet as IMyEntity;
					var upDir = Vector3D.Normalize(encounter.Position - planetEntity.GetPosition());
					var randDir = SpawnResources.GetRandomCompassDirection(encounter.Position, planet);
					var randDist = (double)SpawnResources.rnd.Next((int)Settings.BossEncounters.MinSpawnDistFromCoords, (int)Settings.BossEncounters.MaxSpawnDistFromCoords);
					var roughCoords = randDir * randDist + encounter.Position;
					var surfaceCoords = SpawnResources.GetNearestSurfacePoint(roughCoords, planet);
					var spawnCoords = upDir * Settings.BossEncounters.MinPlanetAltitude + surfaceCoords;
					tempMatrix = MatrixD.CreateWorld(spawnCoords, randDir * -1, upDir);
					gotMatrix = true;
					
				}
				
				if(gotMatrix == false){
					
					continue;
					
				}
				
				bool badCoords = false;
				
				foreach(var prefab in encounter.SpawnGroup.SpawnGroup.Prefabs){
					
					var offsetCoords = Vector3D.Transform((Vector3D)prefab.Position, tempMatrix);
					
					foreach(var entity in SpawnResources.EntityList){
						
						if(Vector3D.Distance(offsetCoords, entity.GetPosition()) < Settings.BossEncounters.MinSignalDistFromOtherEntities){
							
							badCoords = true;
							break;
							
						}
						
					}
					
					if(badCoords == false){
						
						if(SpawnResources.IsPositionInSafeZone(offsetCoords) == true){
							
							badCoords = true;
							break;
							
						}
						
					}
					
					if(SpawnResources.IsPositionInGravity(offsetCoords, planet) == true){
						
						if(SpawnResources.GetDistanceFromSurface(offsetCoords, planet) < Settings.BossEncounters.MinPlanetAltitude / 4){
							
							badCoords = true;
							break;
							
						}
						
					}
					
				}
				
				if(badCoords == true){
					
					continue;
					
				}
				
				//Spawn the things!
				Logger.SkipNextMessage = false;
				Logger.AddMsg("Boss Encounter SpawnGroup " + encounter.SpawnGroup.SpawnGroup.Id.SubtypeName + " Now Spawning.");

				if (encounter.SpawnGroup.FactionOwner != "Nobody" && encounter.SpawnGroup.ChargeNpcFactionForSpawn) {

					var faction = MyAPIGateway.Session.Factions.TryGetFactionByTag(encounter.SpawnGroup.FactionOwner);

					if (faction != null) {

						long currentBalance = 0;
						faction.TryGetBalanceInfo(out currentBalance);
						faction.RequestChangeBalance(encounter.SpawnGroup.ChargeForSpawning > currentBalance ? -currentBalance : -encounter.SpawnGroup.ChargeForSpawning);

					}

				}

				foreach (var prefab in encounter.SpawnGroup.SpawnGroup.Prefabs){

					var options = SpawnGroupManager.CreateSpawningOptions(encounter.SpawnGroup, prefab);
					var spawnPosition = Vector3D.Transform((Vector3D)prefab.Position, tempMatrix);
					var speedL = prefab.Speed * (Vector3)tempMatrix.Forward;
					var speedA = Vector3.Zero;
					var gridList = new List<IMyCubeGrid>();
					long gridOwner = 0;
					
					//Speed Management
					if(Settings.SpaceCargoShips.UseMinimumSpeed == true && prefab.Speed < Settings.SpaceCargoShips.MinimumSpeed){
						
						speedL = Settings.SpaceCargoShips.MinimumSpeed * (Vector3)tempMatrix.Forward;
						
					}
					
					if(Settings.SpaceCargoShips.UseSpeedOverride == true){
						
						speedL = Settings.SpaceCargoShips.SpeedOverride * (Vector3)tempMatrix.Forward;
						
					}
					
					if(NPCWatcher.NPCFactionTagToFounder.ContainsKey(encounter.SpawnGroup.FactionOwner) == true){
						
						gridOwner = NPCWatcher.NPCFactionTagToFounder[encounter.SpawnGroup.FactionOwner];
						
					}else{
						
						Logger.AddMsg("Could Not Find Faction Founder For: " + encounter.SpawnGroup.FactionOwner);
						
					}


					if (encounter.SpawnGroup.UseKnownPlayerLocations) {

						KnownPlayerLocationManager.IncreaseSpawnCountOfLocations(encounter.Position, gridOwner != 0 ? encounter.SpawnGroup.FactionOwner : null);

					}


					//Grid Manipulation
					GridBuilderManipulation.ProcessPrefabForManipulation(prefab.SubtypeId, encounter.SpawnGroup, "BossEncounter", prefab.Behaviour);
					
					try{
						
						MyAPIGateway.PrefabManager.SpawnPrefab(gridList, prefab.SubtypeId, spawnPosition, tempMatrix.Forward, tempMatrix.Up, speedL, speedA, !string.IsNullOrWhiteSpace(prefab.BeaconText) ? prefab.BeaconText : null, options, gridOwner);
						
					}catch(Exception exc){
						
						
						
					}
					
					var pendingNPC = new ActiveNPC();
					pendingNPC.SpawnGroup = encounter.SpawnGroup;
					pendingNPC.SpawnGroupName = encounter.SpawnGroup.SpawnGroupName;
					pendingNPC.InitialFaction = encounter.SpawnGroup.FactionOwner;
					pendingNPC.faction = MyAPIGateway.Session.Factions.TryGetFactionByTag(pendingNPC.InitialFaction);
					pendingNPC.Name = prefab.SubtypeId;
					pendingNPC.GridName = MyDefinitionManager.Static.GetPrefabDefinition(prefab.SubtypeId).CubeGrids[0].DisplayName;
					pendingNPC.StartCoords = spawnPosition;
					pendingNPC.CurrentCoords = spawnPosition;
					pendingNPC.EndCoords = spawnPosition;
					pendingNPC.SpawnType = "BossEncounter";
					pendingNPC.CleanupIgnore = encounter.SpawnGroup.IgnoreCleanupRules;
					pendingNPC.ForceStaticGrid = encounter.SpawnGroup.ForceStaticGrid;
					pendingNPC.KeenAiName = prefab.Behaviour;
					pendingNPC.KeenAiTriggerDistance = prefab.BehaviourActivationDistance;
					
					if(encounter.SpawnGroup.RandomizeWeapons == true){
						
						pendingNPC.ReplenishedSystems = false;
						pendingNPC.ReplacedWeapons = true;
						
					}else if((MES_SessionCore.NPCWeaponUpgradesModDetected == true || Settings.General.EnableGlobalNPCWeaponRandomizer == true) && encounter.SpawnGroup.IgnoreWeaponRandomizerMod == false){
					
						pendingNPC.ReplenishedSystems = false;
						pendingNPC.ReplacedWeapons = true;
						
					}else if(encounter.SpawnGroup.ReplenishSystems == true){
						
						pendingNPC.ReplenishedSystems = false;
						
					}
					
					if(inGravity == true){
						
						pendingNPC.Planet = planet;
						
					}
					
					NPCWatcher.PendingNPCs.Add(pendingNPC);
					
				}

				if (!string.IsNullOrWhiteSpace(encounter.SpawnGroup.BossMusicId)) {

					MyVisualScriptLogicProvider.MusicPlayMusicCue(encounter.SpawnGroup.BossMusicId);
				
				}
				
				return true;
				
			}
			
			Logger.AddMsg("Could Not Find Safe Area To Spawn Boss Encounter");
			return false;
			
		}
		
		public static bool GetInitialSpawnCoords(Vector3D startCoords, out Vector3D spawnCoords){
			
			spawnCoords = Vector3D.Zero;
			MyPlanet planet = SpawnResources.GetNearestPlanet(startCoords);
			var inGravity = SpawnResources.IsPositionInGravity(startCoords, planet);
			
			for(int i = 0; i < Settings.BossEncounters.PathCalculationAttempts; i++){
				
				var testCoords = Vector3D.Zero;
				
				if(inGravity == false){
					
					var randDir = Vector3D.Normalize(MyUtils.GetRandomVector3D());
					var randDist = (double)SpawnResources.rnd.Next((int)Settings.BossEncounters.MinCoordsDistanceSpace, (int)Settings.BossEncounters.MaxCoordsDistanceSpace);
					spawnCoords = randDir * randDist + startCoords;
					
					if(SpawnResources.IsPositionInGravity(spawnCoords, planet) == true){
						
						randDir *= -1;
						spawnCoords = randDir * randDist + startCoords;
						
						if(SpawnResources.IsPositionInGravity(spawnCoords, planet) == true){
							
							continue;
							
						}
						
					}
				
				}else{
					
					var planetEntity = planet as IMyEntity;
					var upDir = Vector3D.Normalize(startCoords - planetEntity.GetPosition());
					var randDir = SpawnResources.GetRandomCompassDirection(startCoords, planet);
					var randDist = (double)SpawnResources.rnd.Next((int)Settings.BossEncounters.MinCoordsDistancePlanet, (int)Settings.BossEncounters.MaxCoordsDistancePlanet);
					var roughCoords = randDir * randDist + startCoords;
					var surfaceCoords = SpawnResources.GetNearestSurfacePoint(roughCoords, planet);
					spawnCoords = upDir * Settings.BossEncounters.MinPlanetAltitude + surfaceCoords;
					
					if(planet.GetAirDensity(spawnCoords) < Settings.BossEncounters.MinAirDensity){
						
						spawnCoords = Vector3D.Zero;
						continue;
						
					}
					
				}
				
				if(spawnCoords == Vector3D.Zero){
					
					continue;
					
				}
				
				bool badCoords = false;
				
				foreach(var entity in SpawnResources.EntityList){
					
					if(Vector3D.Distance(spawnCoords, entity.GetPosition()) < Settings.BossEncounters.MinSignalDistFromOtherEntities){
						
						badCoords = true;
						break;
						
					}
					
				}
				
				if(badCoords == false){
					
					if(SpawnResources.IsPositionInSafeZone(spawnCoords) == true){
						
						badCoords = true;
						
					}
					
				}
				
				if(badCoords == false){
					
					return true;
					
				}
				
			}
			
			spawnCoords = Vector3D.Zero;
			return false;
			
		}
		
		public static bool IsPlayerInBossEncounter(long playerId){
			
			foreach(var bossEncounter in NPCWatcher.BossEncounters){
				
				if(bossEncounter.PlayersInEncounter.Contains(playerId) == true){
					
					return true;
					
				}
				
			}
			
			return false;
			
		}
		
	}
	
}
