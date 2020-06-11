using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
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
using ModularEncountersSpawner.Spawners;
using ModularEncountersSpawner.Templates;

namespace ModularEncountersSpawner{
	
	public static class Logger{

		public static bool LoggerDebugMode { get { return Settings.General.DebugMode; } set { Settings.General.DebugMode = value; } }
		public static bool SkipNextMessage = false;
		public static string LogDefaultIdentifier = "Modular Encounters Spawner: ";
		public static Stopwatch PerformanceTimer = new Stopwatch();

		public static string DebugSpawnGroup = "(NPC-SDE) SuperDuperEasyCargoShipNo";
		public static StringBuilder VoxelCutDebugLog = new StringBuilder();

		public static void AddMsg(string message, bool debugOnly = false, string identifier = ""){
			
			if(LoggerDebugMode == false && debugOnly == true){
				
				return;
				
			}
			
			if(LoggerDebugMode == false && SkipNextMessage == true){
				
				SkipNextMessage = false;
				return;
				
			}
			
			SkipNextMessage = false;
			
			string thisIdentifier = "";
			
			if(identifier == ""){
				
				thisIdentifier = LogDefaultIdentifier;
				
			}
			
			MyLog.Default.WriteLineAndConsole(thisIdentifier + message);
			return;

			if(LoggerDebugMode == true){
				
				foreach(var player in MES_SessionCore.PlayerList){
					
					if(player.PromoteLevel == MyPromoteLevel.Owner || player.PromoteLevel == MyPromoteLevel.Admin){
						
						MyVisualScriptLogicProvider.ShowNotification(message, 5000, "White", player.IdentityId);
						
					}
					
				}

			}
			
		}

		public static void SpawnGroupDebug(string spawnGroup, string msg) {

			if (spawnGroup != DebugSpawnGroup)
				return;

			Logger.AddMsg(spawnGroup + ": " + msg, true);
		
		}

		public static void VoxelCutLogging(string msg, bool reset = false) {

			if (!LoggerDebugMode)
				return;

			if (reset)
				VoxelCutDebugLog.Clear();

			VoxelCutDebugLog.Append(msg).AppendLine();

		}

		public static void StartTimer(){
			
			if(LoggerDebugMode == false){
				
				return;
				
			}
			
			PerformanceTimer = Stopwatch.StartNew();
			
		}
		
		public static string StopTimer(){
			
			if(LoggerDebugMode == false){
				
				return "";
				
			}
			
			PerformanceTimer.Stop();
			return PerformanceTimer.Elapsed.ToString();
			
		}
		
		public static void CreateDebugGPS(string name, Vector3D coords){
			
			if(LoggerDebugMode == false){
				
				return;
				
			}
			
			var gps = MyAPIGateway.Session.GPS.Create(name, "", coords, true);
			MyAPIGateway.Session.GPS.AddLocalGps(gps);
			
		}
		
		public static string GetActiveNPCs(){
			
			var sb = new StringBuilder();
			
			if(NPCWatcher.ActiveNPCs.Keys.Count == 0){
				
				return "";
				
			}
			
			foreach(var npc in NPCWatcher.ActiveNPCs.Keys){
				
				sb.Append("Name: ").Append(NPCWatcher.ActiveNPCs[npc].Name).AppendLine();
				
				if(npc != null && MyAPIGateway.Entities.Exist(npc) == true){
					
					sb.Append("EntityId: ").Append(npc.EntityId.ToString()).AppendLine();
					
				}
				
				sb.Append("Start: ").Append(NPCWatcher.ActiveNPCs[npc].StartCoords.ToString()).AppendLine();
				sb.Append("End: ").Append(NPCWatcher.ActiveNPCs[npc].EndCoords.ToString()).AppendLine();
				
				if(NPCWatcher.ActiveNPCs[npc].SpawnType.Contains("CargoShip") == true && npc != null && MyAPIGateway.Entities.Exist(npc) == true){
					
					var distance = Vector3D.Distance(npc.GetPosition(), NPCWatcher.ActiveNPCs[npc].EndCoords);
					sb.Append("Distance Remaining: ").Append(distance.ToString()).AppendLine();
					
				}
				
				sb.Append("Type: ").Append(NPCWatcher.ActiveNPCs[npc].SpawnType).AppendLine();
				sb.Append("Clean Ignore: ").Append(NPCWatcher.ActiveNPCs[npc].CleanupIgnore.ToString()).AppendLine();
				sb.Append("Clean Timer: ").Append(NPCWatcher.ActiveNPCs[npc].CleanupTime.ToString()).AppendLine();
				sb.Append("NPC Owned: ").Append(NPCWatcher.ActiveNPCs[npc].FullyNPCOwned.ToString()).AppendLine();
				
				sb.AppendLine();
				
			}
			
			return sb.ToString();
			
		}

		public static string GetBlockDefinitionInfo() {

			var sb = new StringBuilder();

			foreach(var def in ChatCommand.BlockDefinitionList) {

				if(def == null) {

					continue;

				}

				sb.Append("Block Name: ").Append(def.DisplayNameText).AppendLine();
				sb.Append("Block ID:   ").Append(def.Id.ToString()).AppendLine();

				if(def.Context?.ModId != null) {

					if(string.IsNullOrWhiteSpace(def.Context.ModId) == false) {

						sb.Append("Mod ID:     ").Append(def.Context.ModId).AppendLine();

					}

				}
				
				sb.Append("Is Public:  ").Append(def.Public.ToString()).AppendLine();
				sb.Append("Size:       ").Append(def.CubeSize.ToString()).AppendLine();

				sb.AppendLine();

			}

			return sb.ToString();

		}
		
		public static string GetColorListFromGrid(IMyPlayer player){
			
			var sb = new StringBuilder();
			
			if(player == null || player?.Character == null){

				sb.Append("Player Does Not Exist On Server. Congrats, you've reached a state I thought not possible!");
				return sb.ToString();
				
			}
			
			if(player?.Controller?.ControlledEntity?.Entity == null){

				MyVisualScriptLogicProvider.ShowNotification("Player Must Be Seated On Grid To Get Color List.", 5000, "White", player.IdentityId);
				sb.Append(" ");
				return sb.ToString();
				
			}
			
			var seat = player.Controller.ControlledEntity.Entity as IMyShipController;
			
			if(seat == null){

				MyVisualScriptLogicProvider.ShowNotification("Player Using Invalid Ship Controller / Seat.", 5000, "White", player.IdentityId);
				sb.Append(" ");
				return sb.ToString();
				
			}
			
			var blockList = new List<IMySlimBlock>();
			seat.SlimBlock.CubeGrid.GetBlocks(blockList);
			var colorDict = new Dictionary<Vector3, int>();
			
			foreach(var block in blockList){
				
				if(colorDict.ContainsKey(block.ColorMaskHSV) == true){
					
					colorDict[block.ColorMaskHSV]++;
					
				}else{
					
					colorDict.Add(block.ColorMaskHSV, 1);
					
				}
				
			}
			
			foreach(var color in colorDict.Keys){
				
				sb.Append("ColorMaskHSV:      ").Append(color.ToString()).AppendLine();
				sb.Append("Blocks With Color: ").Append(colorDict[color].ToString()).AppendLine().AppendLine();
				
			}

			MyVisualScriptLogicProvider.ShowNotification("Grid Color List Sent To Clipboard", 5000, "White", player.IdentityId);
			return sb.ToString();
			
		}
		
		public static string GetPlayerWatcherData(){
			
			var sb = new StringBuilder();
			
			if(MES_SessionCore.playerWatchList.Keys.Count == 0){
				
				return "";
				
			}
			
			foreach(var player in MES_SessionCore.playerWatchList.Keys){
				
				if(player == null){
					
					continue;
					
				}
				
				if(player.Character == null){
					
					continue;
					
				}
				
				sb.Append("Player Name: ").Append(player.DisplayName).AppendLine();
				sb.Append("Space Cargo Ship Timer: ").Append(MES_SessionCore.playerWatchList[player].SpaceCargoShipTimer.ToString()).AppendLine();
				sb.Append("Atmo Cargo Ship Timer: ").Append(MES_SessionCore.playerWatchList[player].AtmoCargoShipTimer.ToString()).AppendLine();
				sb.Append("Random Encounter Timer: ").Append(MES_SessionCore.playerWatchList[player].RandomEncounterCheckTimer.ToString()).AppendLine();
				sb.Append("Random Encounter Cooldown Timer: ").Append(MES_SessionCore.playerWatchList[player].RandomEncounterCoolDownTimer.ToString()).AppendLine();
				sb.Append("Random Encounter Travel Distance: ").Append(Vector3D.Distance(player.GetPosition(), MES_SessionCore.playerWatchList[player].RandomEncounterDistanceCoordCheck).ToString()).AppendLine();
				sb.Append("Planetary Installation Timer: ").Append(MES_SessionCore.playerWatchList[player].PlanetaryInstallationCheckTimer.ToString()).AppendLine();
				sb.Append("Planetary Installation Cooldown: ").Append(MES_SessionCore.playerWatchList[player].PlanetaryInstallationCooldownTimer.ToString()).AppendLine();
				sb.Append("Planetary Installation Travel Distance: ").Append(Vector3D.Distance(player.GetPosition(), MES_SessionCore.playerWatchList[player].InstallationDistanceCoordCheck).ToString()).AppendLine();
				sb.Append("Boss Encounter Timer: ").Append(MES_SessionCore.playerWatchList[player].BossEncounterCheckTimer.ToString()).AppendLine();
				sb.Append("Boss Encounter Cooldown Timer: ").Append(MES_SessionCore.playerWatchList[player].BossEncounterCooldownTimer.ToString()).AppendLine();
				sb.Append("Boss Encounter Active: ").Append(MES_SessionCore.playerWatchList[player].BossEncounterActive.ToString()).AppendLine();
				sb.AppendLine();
				
			}
			
			return sb.ToString();
			
		}
		
		public static string GetSpawnedUniqueEncounters(){
			
			var sb = new StringBuilder();
			
			if(SpawnGroupManager.UniqueGroupsSpawned.Count == 0){
				
				return "";
				
			}
			
			foreach(var groupName in SpawnGroupManager.UniqueGroupsSpawned){
				
				sb.Append(groupName).AppendLine();
				
			}
			
			return sb.ToString();
			
		}
		
		public static string SpawnGroupResults(){
			
			var sb = new StringBuilder();
			sb.Append("======SPAWN GROUPS======").AppendLine();
			sb.AppendLine();
			sb.AppendLine();
			
			if(SpawnGroupManager.SpawnGroups.Count == 0 && TerritoryManager.Territories.Count == 0){
				
				return "";
				
			}
			
			foreach(var spawnGroup in SpawnGroupManager.SpawnGroups){
				
				sb.Append("Spawn Group Name: ").Append(spawnGroup.SpawnGroup.Id.SubtypeName).AppendLine();
				sb.Append("SpaceCargoShip: ").Append(spawnGroup.SpaceCargoShip.ToString()).AppendLine();
				sb.Append("LunarCargoShip: ").Append(spawnGroup.LunarCargoShip.ToString()).AppendLine();
				sb.Append("AtmosphericCargoShip: ").Append(spawnGroup.AtmosphericCargoShip.ToString()).AppendLine();
				sb.Append("SpaceRandomEncounter: ").Append(spawnGroup.SpaceRandomEncounter.ToString()).AppendLine();
				sb.Append("PlanetaryInstallation: ").Append(spawnGroup.PlanetaryInstallation.ToString()).AppendLine();
				sb.Append("PlanetaryInstallationType: ").Append(spawnGroup.PlanetaryInstallationType).AppendLine();
				sb.Append("BossEncounterSpace: ").Append(spawnGroup.BossEncounterSpace.ToString()).AppendLine();
				sb.Append("BossEncounterAtmo: ").Append(spawnGroup.BossEncounterAtmo.ToString()).AppendLine();
				sb.Append("BossEncounterAny: ").Append(spawnGroup.BossEncounterAny.ToString()).AppendLine();
				sb.Append("Frequency: ").Append(spawnGroup.Frequency.ToString()).AppendLine();
				sb.Append("UniqueEncounter: ").Append(spawnGroup.UniqueEncounter.ToString()).AppendLine();
				sb.Append("FactionOwner: ").Append(spawnGroup.FactionOwner).AppendLine();
				sb.Append("IgnoreCleanupRules: ").Append(spawnGroup.IgnoreCleanupRules.ToString()).AppendLine();
				sb.Append("ReplenishSystems: ").Append(spawnGroup.ReplenishSystems.ToString()).AppendLine();
				sb.Append("ForceStaticGrid: ").Append(spawnGroup.ForceStaticGrid.ToString()).AppendLine();
				sb.Append("AdminSpawnOnly: ").Append(spawnGroup.AdminSpawnOnly.ToString()).AppendLine();
				

				sb.Append("SandboxVariables: ");
				foreach(var variable in spawnGroup.SandboxVariables){
					
					sb.Append(variable).Append(", ");
					
				}
				sb.AppendLine();
				
				sb.Append("RandomizeWeapons: ").Append(spawnGroup.RandomizeWeapons.ToString()).AppendLine();
				sb.Append("IgnoreWeaponRandomizerMod: ").Append(spawnGroup.IgnoreWeaponRandomizerMod.ToString()).AppendLine();
				
				sb.Append("WeaponRandomizerBlacklist: ");
				foreach(var item in spawnGroup.WeaponRandomizerBlacklist){
					
					sb.Append(item).Append(", ");
					
				}
				sb.AppendLine();
				
				sb.Append("WeaponRandomizerWhitelist: ");
				foreach(var item in spawnGroup.WeaponRandomizerWhitelist){
					
					sb.Append(item).Append(", ");
					
				}
				sb.AppendLine();

				sb.Append("UseBlockReplacer: ").Append(spawnGroup.UseBlockReplacer.ToString()).AppendLine();
				sb.Append("UseBlockReplacerProfile: ").Append(spawnGroup.UseBlockReplacerProfile.ToString()).AppendLine();
				sb.Append("RelaxReplacedBlocksSize: ").Append(spawnGroup.AlwaysRemoveBlock.ToString()).AppendLine();
				sb.Append("ConvertToHeavyArmor: ").Append(spawnGroup.ConvertToHeavyArmor.ToString()).AppendLine();
				sb.Append("OverrideBlockDamageModifier: ").Append(spawnGroup.OverrideBlockDamageModifier.ToString()).AppendLine();
				sb.Append("BlockDamageModifier: ").Append(spawnGroup.BlockDamageModifier.ToString()).AppendLine();
				sb.Append("ShiftBlockColorsHue: ").Append(spawnGroup.ShiftBlockColorsHue.ToString()).AppendLine();
				sb.Append("RandomHueShift: ").Append(spawnGroup.RandomHueShift.ToString()).AppendLine();
				sb.Append("ShiftBlockColorAmount: ").Append(spawnGroup.ShiftBlockColorAmount.ToString()).AppendLine();
				sb.Append("RecolorGrid: ").Append(spawnGroup.RecolorGrid.ToString()).AppendLine();
				sb.Append("ReduceBlockBuildStates: ").Append(spawnGroup.ReduceBlockBuildStates.ToString()).AppendLine();
				sb.Append("MinimumBlocksPercent: ").Append(spawnGroup.MinimumBlocksPercent.ToString()).AppendLine();
				sb.Append("MaximumBlocksPercent: ").Append(spawnGroup.MaximumBlocksPercent.ToString()).AppendLine();
				sb.Append("MinimumBuildPercent: ").Append(spawnGroup.MinimumBuildPercent.ToString()).AppendLine();
				sb.Append("MaximumBuildPercent: ").Append(spawnGroup.MaximumBuildPercent.ToString()).AppendLine();
				sb.Append("UseRivalAi: ").Append(spawnGroup.UseRivalAi.ToString()).AppendLine();
				sb.Append("MinSpawnFromWorldCenter: ").Append(spawnGroup.MinSpawnFromWorldCenter.ToString()).AppendLine();
				sb.Append("MaxSpawnFromWorldCenter: ").Append(spawnGroup.MaxSpawnFromWorldCenter.ToString()).AppendLine();
				sb.Append("PlanetBlackList: ").Append(string.Join(",", spawnGroup.PlanetBlacklist)).AppendLine();
				sb.Append("PlanetWhiteList: ").Append(string.Join(",", spawnGroup.PlanetWhitelist)).AppendLine();
				sb.Append("PlanetRequiresVacuum: ").Append(spawnGroup.PlanetRequiresVacuum.ToString()).AppendLine();
				sb.Append("PlanetRequiresAtmo: ").Append(spawnGroup.PlanetRequiresAtmo.ToString()).AppendLine();
				sb.Append("PlanetRequiresOxygen: ").Append(spawnGroup.PlanetRequiresOxygen.ToString()).AppendLine();
				sb.Append("PlanetMinimumSize: ").Append(spawnGroup.PlanetMinimumSize.ToString()).AppendLine();
				sb.Append("PlanetMaximumSize: ").Append(spawnGroup.PlanetMaximumSize.ToString()).AppendLine();
				
				sb.Append("RequiredAllMods: ");
				foreach(var mod in spawnGroup.RequireAllMods){
					
					sb.Append(mod.ToString()).Append(", ");
					
				}
				sb.AppendLine();
				
				sb.Append("RequiredAnyMods: ");
				foreach(var mod in spawnGroup.RequireAnyMods){
					
					sb.Append(mod.ToString()).Append(", ");
					
				}
				sb.AppendLine();
				
				sb.Append("ExcludedAllMods: ");
				foreach(var mod in spawnGroup.ExcludeAllMods){
					
					sb.Append(mod.ToString()).Append(", ");
					
				}
				sb.AppendLine();
				
				sb.Append("ExcludedAnyMods: ");
				foreach(var mod in spawnGroup.ExcludeAnyMods){
					
					sb.Append(mod.ToString()).Append(", ");
					
				}
				sb.AppendLine();
				
				sb.Append("Replace Block References: ");
				foreach(var id in spawnGroup.ReplaceBlockReference.Keys){
					
					sb.Append(id.ToString()).Append(", ").Append(spawnGroup.ReplaceBlockReference[id].ToString()).Append(" /// ");
					
				}
				sb.AppendLine();
				
				sb.Append("Territory: ").Append(spawnGroup.Territory).AppendLine();
				sb.Append("MinDistanceFromTerritoryCenter: ").Append(spawnGroup.MinDistanceFromTerritoryCenter.ToString()).AppendLine();
				sb.Append("MaxDistanceFromTerritoryCenter: ").Append(spawnGroup.MaxDistanceFromTerritoryCenter.ToString()).AppendLine();
				sb.Append("RotateFirstCockpitToForward: ").Append(spawnGroup.RotateFirstCockpitToForward.ToString()).AppendLine();
				sb.Append("SpawnRandomCargo: ").Append(spawnGroup.SpawnRandomCargo.ToString()).AppendLine();
				sb.Append("DisableDampeners: ").Append(spawnGroup.DisableDampeners.ToString()).AppendLine();
				sb.Append("ReactorsOn: ").Append(spawnGroup.ReactorsOn.ToString()).AppendLine();
				sb.AppendLine();
				
			}
			
			sb.AppendLine();
			sb.Append("======TERRITORIES======").AppendLine();
			sb.AppendLine();
			
			foreach(var territory in TerritoryManager.Territories){
				
				sb.Append("Name: ").Append(territory.Name).AppendLine();
				sb.Append("TagOld: ").Append(territory.TagOld).AppendLine();
				sb.Append("Position: ").Append(territory.Position.ToString()).AppendLine();
				sb.Append("Type: ").Append(territory.Type).AppendLine();
				sb.Append("Radius: ").Append(territory.Radius.ToString()).AppendLine();
				sb.Append("ScaleRadiusWithPlanetSize: ").Append(territory.ScaleRadiusWithPlanetSize.ToString()).AppendLine();
				sb.Append("AnnounceArriveDepart: ").Append(territory.AnnounceArriveDepart.ToString()).AppendLine();
				sb.Append("CustomArriveMessage: ").Append(territory.CustomArriveMessage).AppendLine();
				sb.Append("CustomDepartMessage: ").Append(territory.CustomDepartMessage).AppendLine();
				sb.Append("PlanetGeneratorName: ").Append(territory.PlanetGeneratorName).AppendLine();
				sb.Append("BadTerritory: ").Append(territory.BadTerritory.ToString()).AppendLine();
				sb.AppendLine();
				
			}
			
			sb.AppendLine();
			sb.Append("Total Territory Definitions Active: ").Append(TerritoryManager.TerritoryList.Count.ToString()).AppendLine();
			sb.Append("Total Territories Active: ").Append(TerritoryManager.Territories.Count.ToString()).AppendLine();
			
			if(LoggerDebugMode == true){
				
				sb.AppendLine();
				
				foreach(var terr in TerritoryManager.TerritoryList){
					
					sb.Append(terr.Position.ToString()).AppendLine();
					
				}
				
			}
			
				
			
			return sb.ToString();
			
		}
		
		public static string EligibleSpawnGroupsAtPosition(Vector3D coords){
			
			var sb = new StringBuilder();
			var validFactions = new Dictionary<string, List<string>>();

			//Spawn Data Near Player
			var selectedSpawnGroup = new ImprovedSpawnGroup();
			SpawnResources.RefreshEntityLists();
			SpawnResources.LastThreatRefresh = SpawnResources.GameStartTime;
			var threatLevel = SpawnResources.GetThreatLevel(selectedSpawnGroup, coords);
			var pcuLevel = SpawnResources.GetPCULevel(selectedSpawnGroup, coords);

			sb.Append("::: Spawn Data Near Player :::").AppendLine();
			sb.Append(" - Threat Score: ").Append(threatLevel.ToString()).AppendLine();
			sb.Append(" - PCU Score:    ").Append(pcuLevel.ToString()).AppendLine();

			sb.AppendLine();

			//Environment Data Near Player
			var environment = new EnvironmentEvaluation(coords);
			sb.Append("::: Environment Data Near Player :::").AppendLine();
			sb.Append(" - Distance From World Center:  ").Append(environment.DistanceFromWorldCenter.ToString()).AppendLine();
			sb.Append(" - Direction From World Center: ").Append(environment.DirectionFromWorldCenter.ToString()).AppendLine();
			sb.Append(" - Is On Planet:                ").Append(environment.IsOnPlanet.ToString()).AppendLine();
			sb.Append(" - Planet Name:                 ").Append(environment.IsOnPlanet ? environment.NearestPlanetName : "N/A").AppendLine();
			sb.Append(" - Planet Diameter:             ").Append(environment.IsOnPlanet ? environment.PlanetDiameter.ToString() : "N/A").AppendLine();
			sb.Append(" - Oxygen At Position:          ").Append(environment.IsOnPlanet ? environment.OxygenAtPosition.ToString() : "N/A").AppendLine();
			sb.Append(" - Atmosphere At Position:      ").Append(environment.IsOnPlanet ? environment.AtmosphereAtPosition.ToString() : "N/A").AppendLine();
			sb.Append(" - Gravity At Position:         ").Append(environment.IsOnPlanet ? environment.GravityAtPosition.ToString() : "N/A").AppendLine();
			sb.Append(" - Altitude At Position:        ").Append(environment.IsOnPlanet ? environment.AltitudeAtPosition.ToString() : "N/A").AppendLine();
			sb.Append(" - Is Night At Position:        ").Append(environment.IsOnPlanet ? environment.IsNight.ToString() : "N/A").AppendLine();
			sb.Append(" - Weather At Position:         ").Append(environment.IsOnPlanet && !string.IsNullOrWhiteSpace(environment.WeatherAtPosition) ? environment.WeatherAtPosition.ToString() : "N/A").AppendLine();
			sb.Append(" - Common Terrain At Position:  ").Append(environment.IsOnPlanet ? environment.CommonTerrainAtPosition.ToString() : "N/A").AppendLine();

			sb.AppendLine();

			//Space Cargo Ship
			sb.Append("::: Space / Lunar Cargo Ship Eligible Spawns :::").AppendLine();
			var spaceCargoList = SpaceCargoShipSpawner.GetSpaceCargoShips(coords, null, out validFactions);
			
			foreach(var sgroup in spaceCargoList.Distinct().ToList()){
				
				sb.Append(" - ").Append(sgroup.SpawnGroupName).AppendLine();
				
			}
			
			sb.AppendLine();
			
			//Random Encounter
			sb.Append("::: Random Encounter Eligible Spawns :::").AppendLine().AppendLine();
			var randEncounterList = RandomEncounterSpawner.GetRandomEncounters(coords, null, out validFactions);
			
			foreach(var sgroup in randEncounterList.Distinct().ToList()){
				
				sb.Append(" - ").Append(sgroup.SpawnGroupName).AppendLine();
				
			}
			
			sb.AppendLine();
			
			//Planetary Cargo Ship
			sb.Append("::: Planetary Cargo Ship Eligible Spawns :::").AppendLine().AppendLine();
			var planetCargoList = PlanetaryCargoShipSpawner.GetPlanetaryCargoShips(coords, null, out validFactions);
			
			foreach(var sgroup in planetCargoList.Distinct().ToList()){
				
				sb.Append(" - ").Append(sgroup.SpawnGroupName).AppendLine();
				
			}
			
			sb.AppendLine();
			
			//Planetary Installation
			sb.Append("::: Planetary Installation Eligible Spawns :::").AppendLine().AppendLine();
			var smallList = new List<ImprovedSpawnGroup>();
			var mediumList = new List<ImprovedSpawnGroup>();
			var largeList = new List<ImprovedSpawnGroup>();
			var planetStationList = PlanetaryInstallationSpawner.GetPlanetaryInstallations(coords, null, out smallList, out mediumList, out largeList, out validFactions);
			
			foreach(var sgroup in planetStationList.Distinct().ToList()){
				
				sb.Append(" - ").Append(sgroup.SpawnGroupName).AppendLine();
				
			}
			
			sb.AppendLine();
			
			//Boss Encounter
			sb.Append("::: Boss Encounter Eligible Spawns :::").AppendLine().AppendLine();
			
			var spawnCoords = Vector3D.Zero;
			
			if(BossEncounterSpawner.GetInitialSpawnCoords(coords, out spawnCoords) == true){
				
				var bossList = BossEncounterSpawner.GetBossEncounters(coords, spawnCoords, null);
			
				foreach(var sgroup in bossList.Distinct().ToList()){
					
					sb.Append(" - ").Append(sgroup.SpawnGroupName).AppendLine();
					
				}
				
			}
			
			return sb.ToString();
			
			
		}
		
	}
	
}