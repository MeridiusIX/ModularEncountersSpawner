using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sandbox.Definitions;
using Sandbox.ModAPI;
using VRage.Game;
using VRage.Game.ModAPI;
using VRageMath;
using ModularEncountersSpawner.Configuration;
using ModularEncountersSpawner.Templates;
using ModularEncountersSpawner.Spawners;
using ModularEncountersSpawner.Api;

namespace ModularEncountersSpawner {

	public static class SpawnGroupManager{
		
		public static List<ImprovedSpawnGroup> SpawnGroups = new List<ImprovedSpawnGroup>();
		public static List<string> PlanetNames = new List<string>();
		
		public static List<string> UniqueGroupsSpawned = new List<string>();
		
		public static Dictionary<string, List<MyObjectBuilder_CubeGrid>> prefabBackupList = new Dictionary<string, List<MyObjectBuilder_CubeGrid>>(); //Temporary Until Thraxus Spawner Is Added
		
		public static string AdminSpawnGroup = "";
		public static string GroupInstance = "";

		//IMyPlayer // bool TryGetBalanceInfo(out long balance);
		//IMyFactionCollection // int GetReputationBetweenFactions(long factionId1, long factionId2);
		//IMyFaction // bool TryGetBalanceInfo(out long balance);


		public static SpawningOptions CreateSpawningOptions(ImprovedSpawnGroup spawnGroup, MySpawnGroupDefinition.SpawnGroupPrefab prefab){
			
			var options = SpawningOptions.None;
			
			if(spawnGroup.RotateFirstCockpitToForward == true){
				
				options |= SpawningOptions.RotateFirstCockpitTowardsDirection;
				
			}
			
			if(spawnGroup.SpawnRandomCargo == true){
				
				options |= SpawningOptions.SpawnRandomCargo;
				
			}
			
			if(spawnGroup.DisableDampeners == true){
				
				options |= SpawningOptions.DisableDampeners;
				
			}
			
			//options |= SpawningOptions.SetNeutralOwner;
			
			if(spawnGroup.ReactorsOn == false){
				
				options |= SpawningOptions.TurnOffReactors;
				
			}
			
			if(prefab.PlaceToGridOrigin == true){
				
				options |= SpawningOptions.UseGridOrigin;
				
			}
			
			return options;
			
		}
		
		public static void CreateSpawnLists(){
			
			//Planet Names First
			var planetDefList = MyDefinitionManager.Static.GetPlanetsGeneratorsDefinitions();
			foreach(var planetDef in planetDefList){
				
				PlanetNames.Add(planetDef.Id.SubtypeName);
				
			}
			
			GroupInstance = MyAPIGateway.Utilities.GamePaths.ModScopeName;
			
			//Get Regular SpawnGroups
			var regularSpawnGroups = MyDefinitionManager.Static.GetSpawnGroupDefinitions();

			var blacklist = new List<string>();
			blacklist.Add("1933101542.sbm"); //SUPERNPCS (uses unauthorized content from several mods).

			//Get Actual SpawnGroups
			foreach(var spawnGroup in regularSpawnGroups){
				
				if(spawnGroup.Enabled == false){
					
					continue;
					
				}

				if(spawnGroup.Context?.ModId != null) {

					if(blacklist.Contains(spawnGroup.Context.ModId) == true) {

						continue;

					}

				}
				
				if(TerritoryManager.IsSpawnGroupATerritory(spawnGroup) == true){
					
					continue;
					
				}
				
				var improveSpawnGroup = new ImprovedSpawnGroup();

				if(spawnGroup.DescriptionText != null){
					
					if(spawnGroup.DescriptionText.Contains("[Modular Encounters SpawnGroup]") == true){
					
						improveSpawnGroup = GetNewSpawnGroupDetails(spawnGroup);
						SpawnGroups.Add(improveSpawnGroup);
						continue;
						
					}
					
				}

				improveSpawnGroup = GetOldSpawnGroupDetails(spawnGroup);
				SpawnGroups.Add(improveSpawnGroup);

			}

			if(SpawnGroupManager.GroupInstance.Contains(Encoding.UTF8.GetString(Convert.FromBase64String("LnNibQ=="))) == true && (!SpawnGroupManager.GroupInstance.Contains(Encoding.UTF8.GetString(Convert.FromBase64String("MTUyMTkwNTg5MA=="))) && !SpawnGroupManager.GroupInstance.Contains(Encoding.UTF8.GetString(Convert.FromBase64String("NzUwODU1"))))) {

				SpawnGroups.Clear();
				return;

			}

			//Create Frequency Range Dictionaries

		}

		public static void AddSpawnGroup(ImprovedSpawnGroup spawnGroup) {

			foreach (var group in SpawnGroups) {

				if (group.SpawnGroupName == spawnGroup.SpawnGroupName) {

					return;
				
				}

			}

			SpawnGroups.Add(spawnGroup);

		}

		public static bool CheckSpawnGroupPlanetLists(ImprovedSpawnGroup spawnGroup, EnvironmentEvaluation environment){

			if (!environment.IsOnPlanet)
				return true;

			if (spawnGroup.PlanetBlacklist.Count > 0 && Settings.General.IgnorePlanetBlacklists == false){
				
				if(spawnGroup.PlanetBlacklist.Contains(environment.NearestPlanetName) == true){

					Logger.SpawnGroupDebug(spawnGroup.SpawnGroup.Id.SubtypeName, "Planet Blacklisted");
					return false;
					
				}
				
			}
			
			if(spawnGroup.PlanetWhitelist.Count > 0 && Settings.General.IgnorePlanetWhitelists == false){
				
				if(spawnGroup.PlanetWhitelist.Contains(environment.NearestPlanetName) == false){

					Logger.SpawnGroupDebug(spawnGroup.SpawnGroup.Id.SubtypeName, "Not On Whitelisted Planet");
					return false;
					
				}
				
			}
			
			if(spawnGroup.PlanetRequiresVacuum == true && environment.AtmosphereAtPosition > 0){

				Logger.SpawnGroupDebug(spawnGroup.SpawnGroup.Id.SubtypeName, "Planet Requires Vacuum");
				return false;
				
			}

			if(!spawnGroup.GravityCargoShip && spawnGroup.PlanetRequiresAtmo == true && environment.AtmosphereAtPosition == 0){

				Logger.SpawnGroupDebug(spawnGroup.SpawnGroup.Id.SubtypeName, "Planet Requires Atmo");
				return false;
				
			}

			if(spawnGroup.PlanetRequiresOxygen == true && environment.OxygenAtPosition == 0){

				Logger.SpawnGroupDebug(spawnGroup.SpawnGroup.Id.SubtypeName, "Planet Requires Oxygen");
				return false;
				
			}

			if(spawnGroup.PlanetMinimumSize > 0 && environment.PlanetDiameter < spawnGroup.PlanetMinimumSize){

				Logger.SpawnGroupDebug(spawnGroup.SpawnGroup.Id.SubtypeName, "Planet Min Size Fail");
				return false;
				
			}

			if(spawnGroup.PlanetMaximumSize > 0 && environment.PlanetDiameter < spawnGroup.PlanetMaximumSize){

				Logger.SpawnGroupDebug(spawnGroup.SpawnGroup.Id.SubtypeName, "Planet Max Size Fail");
				return false;
				
			}
			
			return true;
			
		}
		
		public static bool DistanceFromCenterCheck(ImprovedSpawnGroup spawnGroup, EnvironmentEvaluation environment){

			var distFromCenter = spawnGroup.CustomWorldCenter == Vector3D.Zero ? environment.DistanceFromWorldCenter : Vector3D.Distance(spawnGroup.CustomWorldCenter, environment.Position);

			if (spawnGroup.MinSpawnFromWorldCenter > 0){
				
				if(distFromCenter < spawnGroup.MinSpawnFromWorldCenter){
					
					return false;
					
				}
				
			}
			
			if(spawnGroup.MaxSpawnFromWorldCenter > 0){
				
				if(distFromCenter > spawnGroup.MaxSpawnFromWorldCenter){
					
					return false;
					
				}
				
			}

			if (spawnGroup.DirectionFromWorldCenter.Count > 0) {

				bool allowed = false;

				foreach (var direction in spawnGroup.DirectionFromWorldCenter) {

					if (direction != Vector3D.Zero) {

						var normalizedDirection = Vector3D.Normalize(direction - spawnGroup.CustomWorldCenter);
						var angleFromCoords = SpawnResources.GetAngleBetweenDirections(normalizedDirection, environment.DirectionFromWorldCenter);

						if (spawnGroup.MinAngleFromDirection > 0 && angleFromCoords < spawnGroup.MinAngleFromDirection)
							continue;

						if (spawnGroup.MaxAngleFromDirection > 0 && angleFromCoords > spawnGroup.MaxAngleFromDirection)
							continue;

						allowed = true;

					}

				}

				if (!allowed)
					return false;

			}

			if (environment.IsOnPlanet && spawnGroup.DirectionFromPlanetCenter.Count > 0) {

				bool allowed = false;

				foreach (var direction in spawnGroup.DirectionFromPlanetCenter) {

					if (direction != Vector3D.Zero) {

						var angleFromCoords = SpawnResources.GetAngleBetweenDirections(direction, Vector3D.Normalize(environment.Position - environment.NearestPlanet.PositionComp.WorldAABB.Center));

						if (spawnGroup.MinAngleFromPlanetCenterDirection > 0 && angleFromCoords < spawnGroup.MinAngleFromPlanetCenterDirection)
							continue;

						if (spawnGroup.MaxAngleFromPlanetCenterDirection > 0 && angleFromCoords > spawnGroup.MaxAngleFromPlanetCenterDirection)
							continue;

						allowed = true;

					}

				}

				if (!allowed)
					return false;

			}

			return true;
			
		}

		public static bool DistanceFromSurfaceCheck(ImprovedSpawnGroup spawnGroup, EnvironmentEvaluation environment) {

			if (spawnGroup.MinSpawnFromPlanetSurface < 0 && spawnGroup.MaxSpawnFromPlanetSurface < 0) {

				return true;

			}

			if (environment.NearestPlanet == null && spawnGroup.MinSpawnFromPlanetSurface > 0)
				return true;

			if (environment.NearestPlanet == null && spawnGroup.MaxSpawnFromPlanetSurface > 0)
				return false;

			if (spawnGroup.MinSpawnFromPlanetSurface > 0 && spawnGroup.MinSpawnFromPlanetSurface > environment.AltitudeAtPosition)
				return false;

			if (spawnGroup.MaxSpawnFromPlanetSurface > 0 && spawnGroup.MaxSpawnFromPlanetSurface < environment.AltitudeAtPosition)
				return false;

			return true;

		}

		public static bool EnvironmentChecks(ImprovedSpawnGroup spawnGroup, EnvironmentEvaluation environment) {

			if (spawnGroup.MinAirDensity != -1 && environment.AtmosphereAtPosition < spawnGroup.MinAirDensity)
				return false;

			if (spawnGroup.MaxAirDensity != -1 && environment.AtmosphereAtPosition > spawnGroup.MinAirDensity)
				return false;

			if (spawnGroup.MinGravity != -1 && environment.GravityAtPosition < spawnGroup.MinGravity)
				return false;

			if (spawnGroup.MaxGravity != -1 && environment.GravityAtPosition > spawnGroup.MaxGravity)
				return false;

			if (spawnGroup.UseDayOrNightOnly) {

				if (spawnGroup.SpawnOnlyAtNight != environment.IsNight) {

					return false;
				
				}
			
			}

			if (spawnGroup.UseWeatherSpawning) {

				if (!spawnGroup.AllowedWeatherSystems.Contains(environment.WeatherAtPosition)) {

					return false;
				
				}		
			
			}

			if (spawnGroup.UseTerrainTypeValidation) {

				if (!spawnGroup.AllowedTerrainTypes.Contains(environment.CommonTerrainAtPosition)) {

					return false;

				}

			}

			bool requiresWater = false;

			if (spawnGroup.PlanetaryInstallation) {

				requiresWater = (!spawnGroup.InstallationSpawnsOnDryLand && (spawnGroup.InstallationSpawnsOnWaterSurface || spawnGroup.InstallationSpawnsUnderwater));

			} else {

				requiresWater = spawnGroup.MustSpawnUnderwater;
			
			}

			if (requiresWater) {

				if (!WaterHelper.Enabled || environment.WaterInSurroundingAreaRatio < .1)
					return false;

			}

			return true;
		
		}

		public static ImprovedSpawnGroup GetNewSpawnGroupDetails(MySpawnGroupDefinition spawnGroup){
			
			var improveSpawnGroup = new ImprovedSpawnGroup();
			var descSplit = spawnGroup.DescriptionText.Split('\n');
			bool badParse = false;
			improveSpawnGroup.SpawnGroup = spawnGroup;
			improveSpawnGroup.SpawnGroupName = spawnGroup.Id.SubtypeName;
			bool setDampeners = false;
			bool setAtmoRequired = false;
			bool setForceStatic = false;
						
			foreach(var tagRaw in descSplit){

				var tag = tagRaw.Trim();

				//SpawnGroupEnabled
				if(tag.StartsWith("[SpawnGroupEnabled:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.SpawnGroupEnabled);
										
				}
				
				//SpaceCargoShip
				if(tag.StartsWith("[SpaceCargoShip:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.SpaceCargoShip);
										
				}
				
				//LunarCargoShip
				if(tag.StartsWith("[LunarCargoShip:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.LunarCargoShip);
						
				}
				
				//AtmosphericCargoShip
				if(tag.StartsWith("[AtmosphericCargoShip:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.AtmosphericCargoShip);
						
				}

				//GravityCargoShip
				if (tag.StartsWith("[GravityCargoShip:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.GravityCargoShip);

				}

				//SkipAirDensityCheck
				if (tag.StartsWith("[SkipAirDensityCheck:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.SkipAirDensityCheck);

				}

				//CargoShipTerrainPath
				if (tag.StartsWith("[CargoShipTerrainPath:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.CargoShipTerrainPath);

				}

				//CustomPathStartAltitude
				if (tag.StartsWith("[CustomPathStartAltitude:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.CustomPathStartAltitude);

				}

				//CustomPathEndAltitude
				if (tag.StartsWith("[CustomPathEndAltitude:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.CustomPathEndAltitude);

				}

				//SpaceRandomEncounter
				if (tag.StartsWith("[SpaceRandomEncounter:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.SpaceRandomEncounter);
						
				}
				
				//PlanetaryInstallation
				if(tag.StartsWith("[PlanetaryInstallation:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.PlanetaryInstallation);
						
				}
				
				//PlanetaryInstallationType
				if(tag.StartsWith("[PlanetaryInstallationType:") == true){

					TagStringCheck(tag, ref improveSpawnGroup.PlanetaryInstallationType);
					
					if(improveSpawnGroup.PlanetaryInstallationType == ""){
						
						improveSpawnGroup.PlanetaryInstallationType = "Small";
						
					}
					
				}
				
				//SkipTerrainCheck
				if(tag.StartsWith("[SkipTerrainCheck:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.SkipTerrainCheck);
						
				}

				//RotateInstallations
				if(tag.StartsWith("[RotateInstallations:") == true) {

					TagVector3DListCheck(tag, ref improveSpawnGroup.RotateInstallations);

				}

				//InstallationTerrainValidation
				if (tag.StartsWith("[InstallationTerrainValidation:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.InstallationTerrainValidation);

				}

				//InstallationSpawnsOnDryLand
				if (tag.StartsWith("[InstallationSpawnsOnDryLand:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.InstallationSpawnsOnDryLand);

				}

				//InstallationSpawnsUnderwater
				if (tag.StartsWith("[InstallationSpawnsUnderwater:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.InstallationSpawnsUnderwater);

				}

				//InstallationSpawnsOnWaterSurface
				if (tag.StartsWith("[InstallationSpawnsOnWaterSurface:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.InstallationSpawnsOnWaterSurface);

				}

				//ReverseForwardDirections
				if (tag.StartsWith("[ReverseForwardDirections:") == true) {

					TagBoolListCheck(tag, ref improveSpawnGroup.ReverseForwardDirections);

				}

				//CutVoxelsAtAirtightCells
				if(tag.StartsWith("[CutVoxelsAtAirtightCells:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.CutVoxelsAtAirtightCells);
						
				}
				
				//BossEncounterSpace
				if(tag.StartsWith("[BossEncounterSpace:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.BossEncounterSpace);
						
				}
				
				//BossEncounterAtmo
				if(tag.StartsWith("[BossEncounterAtmo:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.BossEncounterAtmo);
						
				}
				
				//BossEncounterAny
				if(tag.StartsWith("[BossEncounterAny:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.BossEncounterAny);
						
				}

				//RivalAiSpawn
				if (tag.StartsWith("[RivalAiSpawn:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.RivalAiAnySpawn);

				}

				//RivalAiSpaceSpawn
				if (tag.StartsWith("[RivalAiSpaceSpawn:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.RivalAiSpaceSpawn);

				}

				//RivalAiAtmosphericSpawn
				if(tag.StartsWith("[RivalAiAtmosphericSpawn:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.RivalAiAtmosphericSpawn);

				}

				//RivalAiAnySpawn
				if(tag.StartsWith("[RivalAiAnySpawn:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.RivalAiAnySpawn);

				}

				//CreatureSpawn
				if (tag.StartsWith("[CreatureSpawn:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.CreatureSpawn);

				}

				//CreatureIds
				if (tag.StartsWith("[CreatureIds:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.CreatureIds);

				}

				//MinCreatureCount
				if (tag.StartsWith("[MinCreatureCount:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.MinCreatureCount);

				}

				//MaxCreatureCount
				if (tag.StartsWith("[MaxCreatureCount:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.MaxCreatureCount);

				}

				//MinCreatureDistance
				if (tag.StartsWith("[MinCreatureDistance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.MinCreatureDistance);

				}

				//MaxCreatureDistance
				if (tag.StartsWith("[MaxCreatureDistance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.MaxCreatureDistance);

				}

				//CanSpawnUnderwater
				if (tag.StartsWith("[CanSpawnUnderwater:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.CanSpawnUnderwater);

				}

				//MinWaterDepth
				if (tag.StartsWith("[MinWaterDepth:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.MinWaterDepth);

				}

				//Frequency
				improveSpawnGroup.Frequency = (int)Math.Round((double)spawnGroup.Frequency * 10);
				
				//UniqueEncounter
				if(tag.StartsWith("[UniqueEncounter:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.UniqueEncounter);
						
				}
				
				//FactionOwner
				if(tag.StartsWith("[FactionOwner:") == true){

					TagStringCheck(tag, ref improveSpawnGroup.FactionOwner);
					
					if(improveSpawnGroup.FactionOwner == ""){
						
						improveSpawnGroup.FactionOwner = "SPRT";
						
					}

				}

				//UseRandomMinerFaction
				if(tag.StartsWith("[UseRandomMinerFaction:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseRandomMinerFaction);

				}

				//UseRandomBuilderFaction
				if(tag.StartsWith("[UseRandomBuilderFaction:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseRandomBuilderFaction);

				}

				//UseRandomTraderFaction
				if(tag.StartsWith("[UseRandomTraderFaction:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseRandomTraderFaction);

				}

				//IgnoreCleanupRules
				if(tag.StartsWith("[IgnoreCleanupRules:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.IgnoreCleanupRules);
						
				}
				
				//ReplenishSystems
				if(tag.StartsWith("[ReplenishSystems:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.ReplenishSystems);
						
				}

				//UseNonPhysicalAmmo
				if(tag.StartsWith("[UseNonPhysicalAmmo:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseNonPhysicalAmmo);

				}

				//RemoveContainerContents
				if(tag.StartsWith("[RemoveContainerContents:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.RemoveContainerContents);

				}

				//InitializeStoreBlocks
				if(tag.StartsWith("[InitializeStoreBlocks:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.InitializeStoreBlocks);

				}

				//ContainerTypesForStoreOrders
				if(tag.StartsWith("[ContainerTypesForStoreOrders:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.ContainerTypesForStoreOrders);

				}

				//ForceStaticGrid
				if(tag.StartsWith("[ForceStaticGrid:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.ForceStaticGrid);
					setForceStatic = true;
					
				}
				
				//AdminSpawnOnly
				if(tag.StartsWith("[AdminSpawnOnly:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.AdminSpawnOnly);
						
				}

				//SandboxVariables
				if(tag.StartsWith("[SandboxVariables:") == true){

					TagStringListCheck(tag, ref improveSpawnGroup.SandboxVariables);
						
				}

				//FalseSandboxVariables
				if(tag.StartsWith("[FalseSandboxVariables:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.FalseSandboxVariables);

				}

				//RandomNumberRoll
				if(tag.StartsWith("[RandomNumberRoll:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.RandomNumberRoll);
						
				}

				//UseCommonConditions
				if(tag.StartsWith("[UseCommonConditions:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseCommonConditions);

				}

				//ChanceCeiling
				if (tag.StartsWith("[ChanceCeiling:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.ChanceCeiling);

				}

				//SpaceCargoShipChance
				if (tag.StartsWith("[SpaceCargoShipChance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.SpaceCargoShipChance);

				}

				//LunarCargoShipChance
				if (tag.StartsWith("[LunarCargoShipChance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.LunarCargoShipChance);

				}

				//AtmosphericCargoShipChance
				if (tag.StartsWith("[AtmosphericCargoShipChance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.AtmosphericCargoShipChance);

				}

				//GravityCargoShipChance
				if (tag.StartsWith("[GravityCargoShipChance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.GravityCargoShipChance);

				}

				//RandomEncounterChance
				if (tag.StartsWith("[RandomEncounterChance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.RandomEncounterChance);

				}

				//PlanetaryInstallationChance
				if (tag.StartsWith("[PlanetaryInstallationChance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.PlanetaryInstallationChance);

				}

				//BossEncounterChance
				if (tag.StartsWith("[BossEncounterChance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.BossEncounterChance);

				}

				//UseAutoPilotInSpace
				if (tag.StartsWith("[UseAutoPilotInSpace:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseAutoPilotInSpace);

				}

				//PauseAutopilotAtPlayerDistance
				if(tag.StartsWith("[PauseAutopilotAtPlayerDistance:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.PauseAutopilotAtPlayerDistance);

				}

				//PreventOwnershipChange
				if(tag.StartsWith("[PreventOwnershipChange:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.PreventOwnershipChange);

				}

				//RandomizeWeapons
				if(tag.StartsWith("[RandomizeWeapons:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.RandomizeWeapons);
						
				}
				
				//IgnoreWeaponRandomizerMod
				if(tag.StartsWith("[IgnoreWeaponRandomizerMod:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.IgnoreWeaponRandomizerMod);
						
				}
				
				//IgnoreWeaponRandomizerTargetGlobalBlacklist
				if(tag.StartsWith("[IgnoreWeaponRandomizerTargetGlobalBlacklist:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.IgnoreWeaponRandomizerTargetGlobalBlacklist);
						
				}
				
				//IgnoreWeaponRandomizerTargetGlobalWhitelist
				if(tag.StartsWith("[IgnoreWeaponRandomizerTargetGlobalWhitelist:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.IgnoreWeaponRandomizerTargetGlobalWhitelist);
						
				}
				
				//IgnoreWeaponRandomizerGlobalBlacklist
				if(tag.StartsWith("[IgnoreWeaponRandomizerGlobalBlacklist:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.IgnoreWeaponRandomizerGlobalBlacklist);
						
				}
				
				//IgnoreWeaponRandomizerGlobalWhitelist
				if(tag.StartsWith("[IgnoreWeaponRandomizerGlobalWhitelist:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.IgnoreWeaponRandomizerGlobalWhitelist);
						
				}
				
				//WeaponRandomizerTargetBlacklist
				if(tag.StartsWith("[WeaponRandomizerTargetBlacklist:") == true){

					 TagStringListCheck(tag, ref improveSpawnGroup.WeaponRandomizerTargetBlacklist);
						
				}
				
				//WeaponRandomizerTargetWhitelist
				if(tag.StartsWith("[WeaponRandomizerTargetWhitelist:") == true){

					 TagStringListCheck(tag, ref improveSpawnGroup.WeaponRandomizerTargetWhitelist);
						
				}
				
				//WeaponRandomizerBlacklist
				if(tag.StartsWith("[WeaponRandomizerBlacklist:") == true){

					TagStringListCheck(tag, ref improveSpawnGroup.WeaponRandomizerBlacklist);
						
				}
				
				//WeaponRandomizerWhitelist
				if(tag.StartsWith("[WeaponRandomizerWhitelist:") == true){

					TagStringListCheck(tag, ref improveSpawnGroup.WeaponRandomizerWhitelist);
						
				}

				//RandomWeaponChance
				if (tag.StartsWith("[RandomWeaponChance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.RandomWeaponChance);

				}

				//RandomWeaponSizeVariance
				if (tag.StartsWith("[RandomWeaponSizeVariance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.RandomWeaponSizeVariance);

				}

				//NonRandomWeaponNames
				if (tag.StartsWith("[NonRandomWeaponNames:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.NonRandomWeaponNames);

				}

				//NonRandomWeaponIds
				if (tag.StartsWith("[NonRandomWeaponIds:") == true) {

					TagMyDefIdCheck(tag, ref improveSpawnGroup.NonRandomWeaponIds);

				}

				//NonRandomWeaponReplacingOnly
				if (tag.StartsWith("[NonRandomWeaponReplacingOnly:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.NonRandomWeaponReplacingOnly);

				}

				//AddDefenseShieldBlocks
				if (tag.StartsWith("[AddDefenseShieldBlocks:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.AddDefenseShieldBlocks);

				}

				//IgnoreShieldProviderMod
				if (tag.StartsWith("[IgnoreShieldProviderMod:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.IgnoreShieldProviderMod);

				}

				//ShieldProviderChance
				if (tag.StartsWith("[ShieldProviderChance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.ShieldProviderChance);

				}

				//ReplaceArmorBlocksWithModules
				if (tag.StartsWith("[ReplaceArmorBlocksWithModules:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.ReplaceArmorBlocksWithModules);

				}

				//ModulesForArmorReplacement
				if (tag.StartsWith("[ModulesForArmorReplacement:") == true) {

					TagMyDefIdCheck(tag, ref improveSpawnGroup.ModulesForArmorReplacement);

				}

				//UseBlockReplacerProfile
				if (tag.StartsWith("[UseBlockReplacerProfile:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.UseBlockReplacerProfile);
						
				}
				
				//BlockReplacerProfileNames
				if(tag.StartsWith("[BlockReplacerProfileNames:") == true){

					TagStringListCheck(tag, ref improveSpawnGroup.BlockReplacerProfileNames);
					
				}
				
				//UseBlockReplacer
				if(tag.StartsWith("[UseBlockReplacer:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.UseBlockReplacer);
						
				}
				
				//RelaxReplacedBlocksSize
				if(tag.StartsWith("[RelaxReplacedBlocksSize:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.RelaxReplacedBlocksSize);
						
				}
				
				//AlwaysRemoveBlock
				if(tag.StartsWith("[AlwaysRemoveBlock:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.AlwaysRemoveBlock);
						
				}

				//ConfigureSpecialNpcThrusters
				if (tag.StartsWith("[ConfigureSpecialNpcThrusters:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.ConfigureSpecialNpcThrusters);

				}

				//RestrictNpcIonThrust
				if (tag.StartsWith("[RestrictNpcIonThrust:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.RestrictNpcIonThrust);

				}

				//NpcIonThrustForceMultiply
				if (tag.StartsWith("[NpcIonThrustForceMultiply:") == true) {

					TagFloatCheck(tag, ref improveSpawnGroup.NpcIonThrustForceMultiply);

				}

				//NpcIonThrustPowerMultiply
				if (tag.StartsWith("[NpcIonThrustPowerMultiply:") == true) {

					TagFloatCheck(tag, ref improveSpawnGroup.NpcIonThrustPowerMultiply);

				}

				//RestrictNpcAtmoThrust
				if (tag.StartsWith("[RestrictNpcAtmoThrust:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.RestrictNpcAtmoThrust);

				}

				//NpcAtmoThrustForceMultiply
				if (tag.StartsWith("[NpcAtmoThrustForceMultiply:") == true) {

					TagFloatCheck(tag, ref improveSpawnGroup.NpcAtmoThrustForceMultiply);

				}

				//NpcAtmoThrustPowerMultiply
				if (tag.StartsWith("[NpcAtmoThrustPowerMultiply:") == true) {

					TagFloatCheck(tag, ref improveSpawnGroup.NpcAtmoThrustPowerMultiply);

				}

				//RestrictNpcHydroThrust
				if (tag.StartsWith("[RestrictNpcHydroThrust:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.RestrictNpcHydroThrust);

				}

				//NpcHydroThrustForceMultiply
				if (tag.StartsWith("[NpcHydroThrustForceMultiply:") == true) {

					TagFloatCheck(tag, ref improveSpawnGroup.NpcHydroThrustForceMultiply);

				}

				//NpcHydroThrustPowerMultiply
				if (tag.StartsWith("[NpcHydroThrustPowerMultiply:") == true) {

					TagFloatCheck(tag, ref improveSpawnGroup.NpcHydroThrustPowerMultiply);

				}

				//IgnoreGlobalBlockReplacer
				if (tag.StartsWith("[IgnoreGlobalBlockReplacer:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.IgnoreGlobalBlockReplacer);

				}

				//ReplaceBlockReference
				if(tag.StartsWith("[ReplaceBlockReference:") == true){

					TagMDIDictionaryCheck(tag, ref improveSpawnGroup.ReplaceBlockReference);
						
				}

				//ReplaceBlockOld
				if (tag.StartsWith("[ReplaceBlockOld:") == true) {

					TagMyDefIdCheck(tag, ref improveSpawnGroup.ReplaceBlockOld);

				}

				//ReplaceBlockNew
				if (tag.StartsWith("[ReplaceBlockNew:") == true) {

					TagMyDefIdCheck(tag, ref improveSpawnGroup.ReplaceBlockNew);

				}

				//ConvertToHeavyArmor
				if (tag.StartsWith("[ConvertToHeavyArmor:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.ConvertToHeavyArmor);
						
				}

				//UseRandomNameGenerator
				if(tag.StartsWith("[UseRandomNameGenerator:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseRandomNameGenerator);

				}

				//RandomGridNamePrefix
				if(tag.StartsWith("[RandomGridNamePrefix:") == true) {

					TagStringCheck(tag, ref improveSpawnGroup.RandomGridNamePrefix);

				}

				//RandomGridNamePattern
				if(tag.StartsWith("[RandomGridNamePattern:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.RandomGridNamePattern);

				}

				//ReplaceAntennaNameWithRandomizedName
				if(tag.StartsWith("[ReplaceAntennaNameWithRandomizedName:") == true) {

					TagStringCheck(tag, ref improveSpawnGroup.ReplaceAntennaNameWithRandomizedName);

				}

				//UseBlockNameReplacer
				if(tag.StartsWith("[UseBlockNameReplacer:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseBlockNameReplacer);

				}

				//BlockNameReplacerReference
				if(tag.StartsWith("[BlockNameReplacerReference:") == true) {

					TagStringDictionaryCheck(tag, ref improveSpawnGroup.BlockNameReplacerReference);

				}

				//ReplaceBlockNameOld
				if (tag.StartsWith("[ReplaceBlockNameOld:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.ReplaceBlockNameOld);

				}

				//ReplaceBlockNameNew
				if (tag.StartsWith("[ReplaceBlockNameNew:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.ReplaceBlockNameNew);

				}

				//AssignContainerTypesToAllCargo
				if (tag.StartsWith("[AssignContainerTypesToAllCargo:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.AssignContainerTypesToAllCargo);

				}

				//UseContainerTypeAssignment
				if(tag.StartsWith("[UseContainerTypeAssignment:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseContainerTypeAssignment);

				}

				//ContainerTypeAssignmentReference
				if (tag.StartsWith("[ContainerTypeAssignmentReference") == true) {

					TagStringDictionaryCheck(tag, ref improveSpawnGroup.ContainerTypeAssignmentReference);

				}

				//ContainerTypeAssignBlockName
				if (tag.StartsWith("[ContainerTypeAssignBlockName:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.ContainerTypeAssignBlockName);

				}

				//ContainerTypeAssignSubtypeId
				if (tag.StartsWith("[ContainerTypeAssignSubtypeId:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.ContainerTypeAssignSubtypeId);

				}

				//OverrideBlockDamageModifier
				if (tag.StartsWith("[OverrideBlockDamageModifier:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.OverrideBlockDamageModifier);
						
				}
				
				//BlockDamageModifier
				if(tag.StartsWith("[BlockDamageModifier:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.BlockDamageModifier);
						
				}

				//GridsAreEditable
				if(tag.StartsWith("[GridsAreEditable:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.GridsAreEditable);

				}

				//GridsAreDestructable
				if(tag.StartsWith("[GridsAreDestructable:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.GridsAreDestructable);

				}

				//ShiftBlockColorsHue
				if(tag.StartsWith("[ShiftBlockColorsHue:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.ShiftBlockColorsHue);
						
				}
				
				//RandomHueShift
				if(tag.StartsWith("[RandomHueShift:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.RandomHueShift);
						
				}

				//ShiftBlockColorAmount
				if(tag.StartsWith("[ShiftBlockColorAmount:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.ShiftBlockColorAmount);

				}

				//AssignGridSkin
				if(tag.StartsWith("[AssignGridSkin:") == true){

					TagStringListCheck(tag, ref improveSpawnGroup.AssignGridSkin);
						
				}

				//RecolorGrid
				if(tag.StartsWith("[RecolorGrid:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.RecolorGrid);

				}

				//ColorReferencePairs
				if(tag.StartsWith("[ColorReferencePairs:") == true) {

					TagVector3DictionaryCheck(tag, ref improveSpawnGroup.ColorReferencePairs);
					//Logger.AddMsg(improveSpawnGroup.ColorReferencePairs.Keys.Count.ToString() + " Color Reference Pairs");

				}

				//RecolorOld
				if (tag.StartsWith("[RecolorOld:") == true) {

					TagVector3Check(tag, ref improveSpawnGroup.RecolorOld);

				}

				//RecolorNew
				if (tag.StartsWith("[RecolorNew:") == true) {

					TagVector3Check(tag, ref improveSpawnGroup.RecolorNew);

				}

				//ColorSkinReferencePairs
				if (tag.StartsWith("[ColorSkinReferencePairs:") == true) {

					TagVector3StringDictionaryCheck(tag, ref improveSpawnGroup.ColorSkinReferencePairs);
					//Logger.AddMsg(improveSpawnGroup.ColorReferencePairs.Keys.Count.ToString() + " Color-Skin Reference Pairs");

				}

				//ReskinTarget
				if (tag.StartsWith("[ReskinTarget:") == true) {

					TagVector3Check(tag, ref improveSpawnGroup.ReskinTarget);

				}

				//ReskinTexture
				if (tag.StartsWith("[ReskinTexture:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.ReskinTexture);

				}

				//SkinRandomBlocks
				if (tag.StartsWith("[SkinRandomBlocks:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.SkinRandomBlocks);

				}

				//SkinRandomBlocksTextures
				if (tag.StartsWith("[SkinRandomBlocksTextures:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.SkinRandomBlocksTextures);

				}

				//MinPercentageSkinRandomBlocks
				if (tag.StartsWith("[MinPercentageSkinRandomBlocks:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.MinPercentageSkinRandomBlocks);

				}

				//MaxPercentageSkinRandomBlocks
				if (tag.StartsWith("[MaxPercentageSkinRandomBlocks:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.MaxPercentageSkinRandomBlocks);

				}

				//ReduceBlockBuildStates
				if (tag.StartsWith("[ReduceBlockBuildStates:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.ReduceBlockBuildStates);
						
				}
				
				//MinimumBlocksPercent
				if(tag.StartsWith("[MinimumBlocksPercent:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.MinimumBlocksPercent);
						
				}
				
				//MaximumBlocksPercent
				if(tag.StartsWith("[MaximumBlocksPercent:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.MaximumBlocksPercent);
						
				}
				
				//MinimumBuildPercent
				if(tag.StartsWith("[MinimumBuildPercent:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.MinimumBuildPercent);
						
				}
				
				//MaximumBuildPercent
				if(tag.StartsWith("[MaximumBuildPercent:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.MaximumBuildPercent);
						
				}

				//UseGridDereliction
				if (tag.StartsWith("[UseGridDereliction:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseGridDereliction);

				}

				//DerelictionProfiles
				if (tag.StartsWith("[DerelictionProfiles:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.DerelictionProfiles);

				}

				//UseRivalAi
				if (tag.StartsWith("[UseRivalAi:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.UseRivalAi);
						
				}

				//RivalAiReplaceRemoteControl
				if(tag.StartsWith("[RivalAiReplaceRemoteControl:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.RivalAiReplaceRemoteControl);
						
				}

				//ApplyBehaviorToNamedBlock
				if (tag.StartsWith("[ApplyBehaviorToNamedBlock:") == true) {

					TagStringCheck(tag, ref improveSpawnGroup.ApplyBehaviorToNamedBlock);

				}

				//ConvertAllRemoteControlBlocks
				if (tag.StartsWith("[ConvertAllRemoteControlBlocks:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.ConvertAllRemoteControlBlocks);

				}

				//ClearGridInventories
				if (tag.StartsWith("[ClearGridInventories:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.ClearGridInventories);

				}

				//EraseIngameScripts
				if (tag.StartsWith("[EraseIngameScripts:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.EraseIngameScripts);
						
				}
				
				//DisableTimerBlocks
				if(tag.StartsWith("[DisableTimerBlocks:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.DisableTimerBlocks);
						
				}
				
				//DisableSensorBlocks
				if(tag.StartsWith("[DisableSensorBlocks:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.DisableSensorBlocks);
						
				}
				
				//DisableWarheads
				if(tag.StartsWith("[DisableWarheads:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.DisableWarheads);
						
				}
				
				//DisableThrustOverride
				if(tag.StartsWith("[DisableThrustOverride:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.DisableThrustOverride);
						
				}
				
				//DisableGyroOverride
				if(tag.StartsWith("[DisableGyroOverride:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.DisableGyroOverride);
						
				}
				
				//EraseLCDs
				if(tag.StartsWith("[EraseLCDs:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.EraseLCDs);
						
				}
				
				//UseTextureLCD
				if(tag.StartsWith("[UseTextureLCD:") == true){

					TagStringListCheck(tag, ref improveSpawnGroup.UseTextureLCD);
						
				}
				
				//EnableBlocksWithName
				if(tag.StartsWith("[EnableBlocksWithName:") == true){

					TagStringListCheck(tag, ref improveSpawnGroup.EnableBlocksWithName);
						
				}
				
				//DisableBlocksWithName
				if(tag.StartsWith("[DisableBlocksWithName:") == true){

					TagStringListCheck(tag, ref improveSpawnGroup.DisableBlocksWithName);
						
				}
				
				//AllowPartialNames
				if(tag.StartsWith("[AllowPartialNames:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.AllowPartialNames);
						
				}
				
				//ChangeTurretSettings
				if(tag.StartsWith("[ChangeTurretSettings:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.ChangeTurretSettings);
						
				}
				
				//TurretRange
				if(tag.StartsWith("[TurretRange:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.TurretRange);
						
				}
				
				//TurretIdleRotation
				if(tag.StartsWith("[TurretIdleRotation:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.TurretIdleRotation);
						
				}
				
				//TurretTargetMeteors
				if(tag.StartsWith("[TurretTargetMeteors:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.TurretTargetMeteors);
						
				}
				
				//TurretTargetMissiles
				if(tag.StartsWith("[TurretTargetMissiles:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.TurretTargetMissiles);
						
				}
				
				//TurretTargetCharacters
				if(tag.StartsWith("[TurretTargetCharacters:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.TurretTargetCharacters);
						
				}
				
				//TurretTargetSmallGrids
				if(tag.StartsWith("[TurretTargetSmallGrids:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.TurretTargetSmallGrids);
						
				}
				
				//TurretTargetLargeGrids
				if(tag.StartsWith("[TurretTargetLargeGrids:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.TurretTargetLargeGrids);
						
				}
				
				//TurretTargetStations
				if(tag.StartsWith("[TurretTargetStations:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.TurretTargetStations);
						
				}
				
				//TurretTargetNeutrals
				if(tag.StartsWith("[TurretTargetNeutrals:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.TurretTargetNeutrals);
						
				}
				
				//ClearAuthorship
				if(tag.StartsWith("[ClearAuthorship:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.ClearAuthorship);
						
				}
				
				//MinSpawnFromWorldCenter
				if(tag.StartsWith("[MinSpawnFromWorldCenter:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.MinSpawnFromWorldCenter);
						
				}
				
				//MaxSpawnFromWorldCenter
				if(tag.StartsWith("[MaxSpawnFromWorldCenter:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.MaxSpawnFromWorldCenter);
						
				}

				//CustomWorldCenter
				if (tag.StartsWith("[CustomWorldCenter:") == true) {

					TagVector3DCheck(tag, ref improveSpawnGroup.CustomWorldCenter);

				}

				//DirectionFromWorldCenter
				if (tag.StartsWith("[DirectionFromWorldCenter:") == true) {

					TagVector3DListCheck(tag, ref improveSpawnGroup.DirectionFromWorldCenter);

				}

				//MinAngleFromDirection
				if (tag.StartsWith("[MinAngleFromDirection:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.MinAngleFromDirection);

				}

				//MaxAngleFromDirection
				if (tag.StartsWith("[MaxAngleFromDirection:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.MaxAngleFromDirection);

				}

				//DirectionFromPlanetCenter
				if (tag.StartsWith("[DirectionFromPlanetCenter:") == true) {

					TagVector3DListCheck(tag, ref improveSpawnGroup.DirectionFromPlanetCenter);

				}

				//MinAngleFromPlanetCenterDirection
				if (tag.StartsWith("[MinAngleFromPlanetCenterDirection:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.MinAngleFromPlanetCenterDirection);

				}

				//MaxAngleFromPlanetCenterDirection
				if (tag.StartsWith("[MaxAngleFromPlanetCenterDirection:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.MaxAngleFromPlanetCenterDirection);

				}

				//MinSpawnFromPlanetSurface
				if (tag.StartsWith("[MinSpawnFromPlanetSurface:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.MinSpawnFromPlanetSurface);

				}

				//MaxSpawnFromPlanetSurface
				if (tag.StartsWith("[MaxSpawnFromPlanetSurface:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.MaxSpawnFromPlanetSurface);

				}

				//UseDayOrNightOnly
				if (tag.StartsWith("[UseDayOrNightOnly:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseDayOrNightOnly);

				}

				//SpawnOnlyAtNight
				if (tag.StartsWith("[SpawnOnlyAtNight:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.SpawnOnlyAtNight);

				}

				//UseWeatherSpawning
				if (tag.StartsWith("[UseWeatherSpawning:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseWeatherSpawning);

				}

				//AllowedWeatherSystems
				if (tag.StartsWith("[AllowedWeatherSystems:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.AllowedWeatherSystems);

				}

				//UseTerrainTypeValidation
				if (tag.StartsWith("[UseTerrainTypeValidation:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseTerrainTypeValidation);

				}

				//AllowedTerrainTypes
				if (tag.StartsWith("[AllowedTerrainTypes:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.AllowedTerrainTypes);

				}

				//MinAirDensity
				if (tag.StartsWith("[MinAirDensity:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.MinAirDensity);

				}

				//MaxAirDensity
				if (tag.StartsWith("[MaxAirDensity:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.MaxAirDensity);

				}

				//MinGravity
				if (tag.StartsWith("[MinGravity:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.MinGravity);

				}

				//MaxGravity
				if (tag.StartsWith("[MaxGravity:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.MaxGravity);

				}

				//PlanetBlacklist
				if (tag.StartsWith("[PlanetBlacklist:") == true){

					TagStringListCheck(tag, ref improveSpawnGroup.PlanetBlacklist);
						
				}
				
				//PlanetWhitelist
				if(tag.StartsWith("[PlanetWhitelist:") == true){

					TagStringListCheck(tag, ref improveSpawnGroup.PlanetWhitelist);
						
				}
				
				//PlanetRequiresVacuum
				if(tag.StartsWith("[PlanetRequiresVacuum:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.PlanetRequiresVacuum);
						
				}
				
				//PlanetRequiresAtmo
				if(tag.StartsWith("[PlanetRequiresAtmo:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.PlanetRequiresAtmo);
					setAtmoRequired = true;
						
				}
				
				//PlanetRequiresOxygen
				if(tag.StartsWith("[PlanetRequiresOxygen:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.PlanetRequiresOxygen);
						
				}
				
				//PlanetMinimumSize
				if(tag.StartsWith("[PlanetMinimumSize:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.PlanetMinimumSize);
						
				}
				
				//PlanetMaximumSize
				if(tag.StartsWith("[PlanetMaximumSize:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.PlanetMaximumSize);
						
				}

				//UsePlayerCountCheck
				if(tag.StartsWith("[UsePlayerCountCheck:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UsePlayerCountCheck);

				}

				//PlayerCountCheckRadius
				if(tag.StartsWith("[PlayerCountCheckRadius:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.PlayerCountCheckRadius);

				}

				//MinimumPlayers
				if(tag.StartsWith("[MinimumPlayers:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.MinimumPlayers);

				}

				//MaximumPlayers
				if(tag.StartsWith("[MaximumPlayers:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.MaximumPlayers);

				}

				//UseThreatLevelCheck
				if(tag.StartsWith("[UseThreatLevelCheck:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.UseThreatLevelCheck);
						
				}
				
				//ThreatLevelCheckRange
				if(tag.StartsWith("[ThreatLevelCheckRange:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.ThreatLevelCheckRange);
						
				}
				
				//ThreatIncludeOtherNpcOwners
				if(tag.StartsWith("[ThreatIncludeOtherNpcOwners:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.ThreatIncludeOtherNpcOwners);
						
				}
				
				//ThreatScoreMinimum
				if(tag.StartsWith("[ThreatScoreMinimum:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.ThreatScoreMinimum);
						
				}
				
				//ThreatScoreMaximum
				if(tag.StartsWith("[ThreatScoreMaximum:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.ThreatScoreMaximum);
				
				}
				
				//UsePCUCheck
				if(tag.StartsWith("[UsePCUCheck:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.UsePCUCheck);
						
				}
				
				//PCUCheckRadius
				if(tag.StartsWith("[PCUCheckRadius:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.PCUCheckRadius);
						
				}
				
				//PCUMinimum
				if(tag.StartsWith("[PCUMinimum:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.PCUMinimum);
						
				}
				
				//PCUMaximum
				if(tag.StartsWith("[PCUMaximum:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.PCUMaximum);
						
				}
				
				//UsePlayerCredits
				if(tag.StartsWith("[UsePlayerCredits:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.UsePlayerCredits);
						
				}

				//IncludeAllPlayersInRadius
				if(tag.StartsWith("[IncludeAllPlayersInRadius:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.IncludeAllPlayersInRadius);

				}

				//IncludeFactionBalance
				if(tag.StartsWith("[IncludeFactionBalance:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.IncludeFactionBalance);

				}

				//PlayerCreditsCheckRadius
				if(tag.StartsWith("[PlayerCreditsCheckRadius:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.PlayerCreditsCheckRadius);
						
				}
				
				//MinimumPlayerCredits
				if(tag.StartsWith("[MinimumPlayerCredits:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.MinimumPlayerCredits);
						
				}
				
				//MaximumPlayerCredits
				if(tag.StartsWith("[MaximumPlayerCredits:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.MaximumPlayerCredits);
						
				}
				
				//UsePlayerFactionReputation
				if(tag.StartsWith("[UsePlayerFactionReputation:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.UsePlayerFactionReputation);
						
				}
				
				//PlayerReputationCheckRadius
				if(tag.StartsWith("[PlayerReputationCheckRadius:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.PlayerReputationCheckRadius);
				
				}
				
				//CheckReputationAgainstOtherNPCFaction
				if(tag.StartsWith("[CheckReputationAgainstOtherNPCFaction:") == true){

					TagStringCheck(tag, ref improveSpawnGroup.CheckReputationAgainstOtherNPCFaction);
				
				}
				
				//MinimumReputation
				if(tag.StartsWith("[MinimumReputation:") == true){
				
					TagIntCheck(tag, ref improveSpawnGroup.MinimumReputation);
				
				}
				
				//MaximumReputation
				if(tag.StartsWith("[MaximumReputation:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.MaximumReputation);
						
				}

				//ChargeNpcFactionForSpawn
				if (tag.StartsWith("[ChargeNpcFactionForSpawn:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.ChargeNpcFactionForSpawn);

				}

				//ChargeForSpawning
				if (tag.StartsWith("[ChargeForSpawning:") == true) {

					TagLongCheck(tag, ref improveSpawnGroup.ChargeForSpawning);

				}

				//UseSandboxCounterCosts
				if (tag.StartsWith("[UseSandboxCounterCosts:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseSandboxCounterCosts);

				}

				//SandboxCounterCostNames
				if (tag.StartsWith("[SandboxCounterCostNames:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.SandboxCounterCostNames);

				}

				//SandboxCounterCostAmounts
				if (tag.StartsWith("[SandboxCounterCostAmounts:") == true) {

					TagIntListCheck(tag, ref improveSpawnGroup.SandboxCounterCostAmounts);

				}

				//UseRemoteControlCodeRestrictions
				if (tag.StartsWith("[UseRemoteControlCodeRestrictions:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseRemoteControlCodeRestrictions);

				}

				//RemoteControlCode
				if (tag.StartsWith("[RemoteControlCode:") == true) {

					TagStringCheck(tag, ref improveSpawnGroup.RemoteControlCode);

				}

				//RemoteControlCodeMinDistance
				if (tag.StartsWith("[RemoteControlCodeMinDistance:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.RemoteControlCodeMinDistance);

				}

				//RemoteControlCodeMaxDistance
				if (tag.StartsWith("[RemoteControlCodeMaxDistance:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.RemoteControlCodeMaxDistance);

				}

				//RequireAllMods
				if (tag.StartsWith("[RequiredMods:") == true || tag.StartsWith("[RequireAllMods") == true){

					TagUlongListCheck(tag, ref improveSpawnGroup.RequireAllMods);
						
				}
				
				//ExcludeAnyMods
				if(tag.StartsWith("[ExcludedMods:") == true || tag.StartsWith("[ExcludeAnyMods") == true){

					TagUlongListCheck(tag, ref improveSpawnGroup.ExcludeAnyMods);
						
				}
				
				//RequireAnyMods
				if(tag.StartsWith("[RequireAnyMods:") == true){

					TagUlongListCheck(tag, ref improveSpawnGroup.RequireAnyMods);
						
				}
				
				//ExcludeAllMods
				if(tag.StartsWith("[ExcludeAllMods:") == true){

					TagUlongListCheck(tag, ref improveSpawnGroup.ExcludeAllMods);
						
				}
				
				//RequiredPlayersOnline
				if(tag.StartsWith("[RequiredPlayersOnline:") == true){

					TagUlongListCheck(tag, ref improveSpawnGroup.RequiredPlayersOnline);
						
				}

				//RequiredAnyPlayersOnline
				if (tag.StartsWith("[RequiredAnyPlayersOnline:") == true) {

					TagUlongListCheck(tag, ref improveSpawnGroup.RequiredAnyPlayersOnline);

				}

				//AttachModStorageComponentToGrid
				if (tag.StartsWith("[AttachModStorageComponentToGrid:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.AttachModStorageComponentToGrid);
						
				}
				
				//StorageKey
				if(tag.StartsWith("[StorageKey:") == true){

					TagGuidCheck(tag, ref improveSpawnGroup.StorageKey);
		
				}
				
				//StorageValue
				if(tag.StartsWith("[StorageValue:") == true){

					TagStringCheck(tag, ref improveSpawnGroup.StorageValue);
				
				}

				//UseKnownPlayerLocations
				if(tag.StartsWith("[UseKnownPlayerLocations:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseKnownPlayerLocations);

				}

				//KnownPlayerLocationMustMatchFaction
				if(tag.StartsWith("[KnownPlayerLocationMustMatchFaction:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.KnownPlayerLocationMustMatchFaction);

				}

				//KnownPlayerLocationMinSpawnedEncounters
				if(tag.StartsWith("[KnownPlayerLocationMinSpawnedEncounters:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.KnownPlayerLocationMinSpawnedEncounters);

				}

				//KnownPlayerLocationMaxSpawnedEncounters
				if(tag.StartsWith("[KnownPlayerLocationMaxSpawnedEncounters:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.KnownPlayerLocationMaxSpawnedEncounters);

				}

				//Territory
				if(tag.StartsWith("[Territory:") == true){

					TagStringCheck(tag, ref improveSpawnGroup.Territory);
						
				}
				
				//MinDistanceFromTerritoryCenter
				if(tag.StartsWith("[MinDistanceFromTerritoryCenter:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.MinDistanceFromTerritoryCenter);
						
				}
				
				//MaxDistanceFromTerritoryCenter
				if(tag.StartsWith("[MaxDistanceFromTerritoryCenter:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.MaxDistanceFromTerritoryCenter);
						
				}
				
				//RotateFirstCockpitToForward
				if(tag.StartsWith("[RotateFirstCockpitToForward:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.RotateFirstCockpitToForward);
						
				}
				
				//PositionAtFirstCockpit
				if(tag.StartsWith("[PositionAtFirstCockpit:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.PositionAtFirstCockpit);
						
				}
				
				//SpawnRandomCargo
				if(tag.StartsWith("[SpawnRandomCargo:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.SpawnRandomCargo);
						
				}
				
				//DisableDampeners
				if(tag.StartsWith("[DisableDampeners:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.DisableDampeners);
					setDampeners = true;
						
				}
				
				//ReactorsOn
				if(tag.StartsWith("[ReactorsOn:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.ReactorsOn);
						
				}

				//RemoveVoxelsIfGridRemoved
				if(tag.StartsWith("[RemoveVoxelsIfGridRemoved:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.RemoveVoxelsIfGridRemoved);

				}

				//BossCustomAnnounceEnable
				if(tag.StartsWith("[BossCustomAnnounceEnable:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.BossCustomAnnounceEnable);
						
				}
				
				//BossCustomAnnounceAuthor
				if(tag.StartsWith("[BossCustomAnnounceAuthor:") == true){

					TagStringCheck(tag, ref improveSpawnGroup.BossCustomAnnounceAuthor);
						
				}
				
				//BossCustomAnnounceMessage
				if(tag.StartsWith("[BossCustomAnnounceMessage:") == true){

					TagStringCheck(tag, ref improveSpawnGroup.BossCustomAnnounceMessage);
						
				}
				
				//BossCustomGPSLabel
				if(tag.StartsWith("[BossCustomGPSLabel:") == true){

					TagStringCheck(tag, ref improveSpawnGroup.BossCustomGPSLabel);
						
				}

				//BossCustomGPSColor
				if (tag.StartsWith("[BossCustomGPSColor:") == true) {

					TagVector3DCheck(tag, ref improveSpawnGroup.BossCustomGPSColor);

				}

				//BossMusicId
				if (tag.StartsWith("[BossMusicId:") == true) {

					TagStringCheck(tag, ref improveSpawnGroup.BossMusicId);

				}

			}
			
			if(improveSpawnGroup.SpaceCargoShip == true && setDampeners == false){
				
				improveSpawnGroup.DisableDampeners = true;
				
			}
				
			if(improveSpawnGroup.AtmosphericCargoShip == true && setAtmoRequired == false){
				
				if(!improveSpawnGroup.SkipAirDensityCheck)
					improveSpawnGroup.PlanetRequiresAtmo = true;
				
			}
			
			if(improveSpawnGroup.PlanetaryInstallation == true && setForceStatic == false){
				
				improveSpawnGroup.ForceStaticGrid = true;
				
			}

			//Build Dictionaries
			if (improveSpawnGroup.NonRandomWeaponNames.Count > 0 && improveSpawnGroup.NonRandomWeaponNames.Count == improveSpawnGroup.NonRandomWeaponIds.Count) {

				for (int i = 0; i < improveSpawnGroup.NonRandomWeaponNames.Count; i++) {

					if (!improveSpawnGroup.NonRandomWeaponReference.ContainsKey(improveSpawnGroup.NonRandomWeaponNames[i]))
						improveSpawnGroup.NonRandomWeaponReference.Add(improveSpawnGroup.NonRandomWeaponNames[i], improveSpawnGroup.NonRandomWeaponIds[i]);

				}

			}

			if (improveSpawnGroup.ReplaceBlockOld.Count > 0 && improveSpawnGroup.ReplaceBlockOld.Count == improveSpawnGroup.ReplaceBlockNew.Count) {

				for (int i = 0; i < improveSpawnGroup.ReplaceBlockOld.Count; i++) {

					if (!improveSpawnGroup.ReplaceBlockReference.ContainsKey(improveSpawnGroup.ReplaceBlockOld[i]))
						improveSpawnGroup.ReplaceBlockReference.Add(improveSpawnGroup.ReplaceBlockOld[i], improveSpawnGroup.ReplaceBlockNew[i]);

				}
				
			}

			if (improveSpawnGroup.ReplaceBlockNameOld.Count > 0 && improveSpawnGroup.ReplaceBlockNameOld.Count == improveSpawnGroup.ReplaceBlockNameNew.Count) {

				for (int i = 0; i < improveSpawnGroup.ReplaceBlockNameOld.Count; i++) {

					if (!improveSpawnGroup.BlockNameReplacerReference.ContainsKey(improveSpawnGroup.ReplaceBlockNameOld[i]))
						improveSpawnGroup.BlockNameReplacerReference.Add(improveSpawnGroup.ReplaceBlockNameOld[i], improveSpawnGroup.ReplaceBlockNameNew[i]);

				}

			}

			if (improveSpawnGroup.ContainerTypeAssignBlockName.Count > 0 && improveSpawnGroup.ContainerTypeAssignBlockName.Count == improveSpawnGroup.ContainerTypeAssignSubtypeId.Count) {

				for (int i = 0; i < improveSpawnGroup.ContainerTypeAssignBlockName.Count; i++) {

					if (!improveSpawnGroup.ContainerTypeAssignmentReference.ContainsKey(improveSpawnGroup.ContainerTypeAssignBlockName[i]))
						improveSpawnGroup.ContainerTypeAssignmentReference.Add(improveSpawnGroup.ContainerTypeAssignBlockName[i], improveSpawnGroup.ContainerTypeAssignSubtypeId[i]);

				}

			}

			if (improveSpawnGroup.RecolorOld.Count > 0 && improveSpawnGroup.RecolorOld.Count == improveSpawnGroup.RecolorNew.Count) {

				for (int i = 0; i < improveSpawnGroup.RecolorOld.Count; i++) {

					if (!improveSpawnGroup.ColorReferencePairs.ContainsKey(improveSpawnGroup.RecolorOld[i]))
						improveSpawnGroup.ColorReferencePairs.Add(improveSpawnGroup.RecolorOld[i], improveSpawnGroup.RecolorNew[i]);

				}

			}

			if (improveSpawnGroup.ReskinTarget.Count > 0 && improveSpawnGroup.ReskinTarget.Count == improveSpawnGroup.ReskinTexture.Count) {

				for (int i = 0; i < improveSpawnGroup.ReskinTarget.Count; i++) {

					if (!improveSpawnGroup.ColorSkinReferencePairs.ContainsKey(improveSpawnGroup.ReskinTarget[i]))
						improveSpawnGroup.ColorSkinReferencePairs.Add(improveSpawnGroup.ReskinTarget[i], improveSpawnGroup.ReskinTexture[i]);

				}

			}

			return improveSpawnGroup;

		}
		
		public static ImprovedSpawnGroup GetOldSpawnGroupDetails(MySpawnGroupDefinition spawnGroup){
			
			var thisSpawnGroup = new ImprovedSpawnGroup();
			thisSpawnGroup.SpawnGroupName = spawnGroup.Id.SubtypeName;
			var factionList = MyAPIGateway.Session.Factions.Factions;
			var factionTags = new List<string>();
			factionTags.Add("Nobody");
			
			foreach(var faction in factionList.Keys){
				
				if(factionList[faction].IsEveryoneNpc() == true && factionList[faction].AcceptHumans == false){
					
					factionTags.Add(factionList[faction].Tag);
					
				}
				
			}
			
			thisSpawnGroup.SpawnGroup = spawnGroup;
			
			//SpawnGroup Type
			if(spawnGroup.Id.SubtypeName.Contains("(Atmo)") == true){
				
				thisSpawnGroup.AtmosphericCargoShip = true;
				thisSpawnGroup.DisableDampeners = false;
				thisSpawnGroup.PlanetRequiresAtmo = true;
				
			}
			
			if(spawnGroup.Id.SubtypeName.Contains("(Inst-") == true){
				
				thisSpawnGroup.ForceStaticGrid = true;
				thisSpawnGroup.PlanetaryInstallation = true;
				
				if(spawnGroup.Id.SubtypeName.Contains("(Inst-1)") == true){
					
					thisSpawnGroup.PlanetaryInstallationType = "Small";
					
				}
				
				if(spawnGroup.Id.SubtypeName.Contains("(Inst-2)") == true){
					
					thisSpawnGroup.PlanetaryInstallationType = "Medium";
					
				}
				
				if(spawnGroup.Id.SubtypeName.Contains("(Inst-3)") == true){
					
					thisSpawnGroup.PlanetaryInstallationType = "Large";
					
				}
				
			}
			
			if(spawnGroup.IsPirate == false && spawnGroup.IsEncounter == false && Settings.General.EnableLegacySpaceCargoShipDetection == true){
				
				thisSpawnGroup.DisableDampeners = true;
				thisSpawnGroup.SpaceCargoShip = true;
				
			}else if(spawnGroup.IsCargoShip == true){
				
				thisSpawnGroup.DisableDampeners = true;
				thisSpawnGroup.SpaceCargoShip = true;
			
			}

			if(spawnGroup.Context.IsBaseGame == true && thisSpawnGroup.SpaceCargoShip == true) {

				thisSpawnGroup.UseRandomMinerFaction = true;
				thisSpawnGroup.UseRandomBuilderFaction = true;
				thisSpawnGroup.UseRandomTraderFaction = true;

			}
			
			if(spawnGroup.IsPirate == false && spawnGroup.IsEncounter == true){
				
				thisSpawnGroup.SpaceRandomEncounter = true;
				thisSpawnGroup.ReactorsOn = false;
				thisSpawnGroup.FactionOwner = "Nobody";
				
			}
			
			if(spawnGroup.IsPirate == true && spawnGroup.IsEncounter == true){
				
				thisSpawnGroup.SpaceRandomEncounter = true;
				thisSpawnGroup.FactionOwner = "SPRT";
				
			}
			
			//Factions
			foreach(var tag in factionTags){
				
				if(spawnGroup.Id.SubtypeName.Contains("(" + tag + ")") == true){
					
					thisSpawnGroup.FactionOwner = tag;
					break;
					
				}
				
			}
			
			//Planet Whitelist & Blacklist
			foreach(var planet in PlanetNames){
				
				if(spawnGroup.Id.SubtypeName.Contains("(" + planet + ")") == true && thisSpawnGroup.PlanetWhitelist.Contains(planet) == false){
					
					thisSpawnGroup.PlanetWhitelist.Add(planet);
					
				}
				
				if(spawnGroup.Id.SubtypeName.Contains("(!" + planet + ")") == true && thisSpawnGroup.PlanetBlacklist.Contains(planet) == false){
					
					thisSpawnGroup.PlanetBlacklist.Add(planet);
					
				}
				
			}
			
			//Unique
			if(spawnGroup.Id.SubtypeName.Contains("(Unique)") == true){
				
				thisSpawnGroup.UniqueEncounter = true;
				
			}
			
			//Derelict
			if(spawnGroup.Id.SubtypeName.Contains("(Wreck)") == true){

				var randRotation = new Vector3D(100,100,100);
				thisSpawnGroup.RotateInstallations.Add(randRotation);
				thisSpawnGroup.RotateInstallations.Add(randRotation);
				thisSpawnGroup.RotateInstallations.Add(randRotation);
				thisSpawnGroup.RotateInstallations.Add(randRotation);
				thisSpawnGroup.RotateInstallations.Add(randRotation);
				thisSpawnGroup.RotateInstallations.Add(randRotation);
				thisSpawnGroup.RotateInstallations.Add(randRotation);
				thisSpawnGroup.RotateInstallations.Add(randRotation);
				thisSpawnGroup.RotateInstallations.Add(randRotation);
				thisSpawnGroup.RotateInstallations.Add(randRotation);

			}
			
			//Frequency
			thisSpawnGroup.Frequency = (int)Math.Round((double)spawnGroup.Frequency * 10);
			
			return thisSpawnGroup;
			
		}
		
		public static ImprovedSpawnGroup GetSpawnGroupByName(string name){
			
			foreach(var group in SpawnGroups){
				
				if(group.SpawnGroupName == name){
					
					return group;
					
				}
				
			}
			
			return null;
			
		}

		public static bool NeededModsForSpawnGroup(ImprovedSpawnGroup spawnGroup){

			//Require All
			if(spawnGroup.RequireAllMods.Count > 0){

				foreach (var item in spawnGroup.RequireAllMods){
				
					if(MES_SessionCore.ActiveMods.Contains(item) == false){
						
						return false;
						
					}
					
				}
				
			}

			//Require Any
			if(spawnGroup.RequireAnyMods.Count > 0){

				bool gotMod = false;
				
				foreach(var item in spawnGroup.RequireAnyMods){
				
					if(MES_SessionCore.ActiveMods.Contains(item) == true){
						
						gotMod = true;
						break;
						
					}
					
				}
				
				if(gotMod == false){
					
					return false;
					
				}
				
			}
			
			//Exclude All
			if(spawnGroup.ExcludeAllMods.Count > 0){
				
				foreach(var item in spawnGroup.ExcludeAllMods){
				
					if(MES_SessionCore.ActiveMods.Contains(item) == true){
						
						return false;
						
					}
					
				}
				
			}
			
			//Exclude Any
			if(spawnGroup.ExcludeAnyMods.Count > 0){

				bool conditionMet = false;
				
				foreach(var item in spawnGroup.ExcludeAnyMods){
				
					if(MES_SessionCore.ActiveMods.Contains(item) == false){
						
						conditionMet = true;
						break;
						
					}
					
				}
				
				if(conditionMet == false){
					
					return false;
					
				}
				
			}
			
			return true;
			
		}
		
		public static bool IsSpawnGroupInBlacklist(string spawnGroupName){
			
			//Get Blacklist
			var blacklistGroups = new List<string>(Settings.General.NpcSpawnGroupBlacklist.ToList());
			
			//Check Blacklist
			if(blacklistGroups.Contains(spawnGroupName) == true){
				
				return true;
				
			}
			
			return false;
				
		}

		public static string [] ProcessTag(string tag){
			
			var thisTag = tag.Trim();
			thisTag = thisTag.Replace("[", "");
			thisTag = thisTag.Replace("]", "");
			var tagSplit = thisTag.Split(':');
			string a = "";
			string b = "";

			if(tagSplit.Length > 2) {

				a = tagSplit[0];

				for(int i = 1;i < tagSplit.Length;i++) {

					b += tagSplit[i];

					if(i != tagSplit.Length - 1) {

						b += ":";

					}

				}

				tagSplit = new string[] {a,b};
				Logger.AddMsg("MultipColonSplit - " + b, true);

			}

			return tagSplit;
			
		}

		public static string FixVectorString(string source) {

			string newString = source;

			if (newString.Length == 0)
				return source;

			if (newString[0] == '{')
				newString = newString.Remove(0, 1);

			if (newString.Length == 0)
				return source;

			if (newString[newString.Length - 1] == '}')
				newString = newString.Remove(newString.Length - 1, 1);

			return newString;

		}

		public static void TagBoolCheck(string tag, ref bool original){
			
			bool result = false;
			var tagSplit = ProcessTag(tag);

			if (tagSplit.Length == 2) {

				if (bool.TryParse(tagSplit[1], out result) == false) {

					return;

				}

			} else {

				return;
			
			}

			original = result;
			
		}

		public static void TagFloatCheck(string tag, ref float original) {

			float result = 0;
			var tagSplit = ProcessTag(tag);

			if (tagSplit.Length == 2) {

				if (float.TryParse(tagSplit[1], out result) == false) {

					return;

				}

			} else {

				return;

			}

			original = result;

		}

		public static void TagDoubleCheck(string tag, ref double original) {
			
			double result = 0;
			var tagSplit = ProcessTag(tag);
					
			if(tagSplit.Length == 2){
				
				if(double.TryParse(tagSplit[1], out result) == false) {

					return;

				}

			} else {

				return;

			}

			original = result;

		}
		
		public static void TagIntCheck(string tag, ref int original) {
			
			int result = 0;
			var tagSplit = ProcessTag(tag);
					
			if(tagSplit.Length == 2){
				
				if(int.TryParse(tagSplit[1], out result) == false){

					return;
					
				}
				
			}else{

				return;
				
			}

			original = result;

		}

		public static void TagIntListCheck(string tag, ref List<int> result) {

			var tagSplit = ProcessTag(tag);

			if (tagSplit.Length == 2) {

				var array = tagSplit[1].Split(',');

				foreach (var item in array) {

					if (string.IsNullOrWhiteSpace(item))
						continue;

					int number = 0;

					if (int.TryParse(item, out number) == false)
						continue;

					result.Add(number);

				}

			}

			result.RemoveAll(item => item == 0);

		}

		public static void TagLongCheck(string tag, ref long original) {

			long result = 0;
			var tagSplit = ProcessTag(tag);

			if (tagSplit.Length == 2) {

				if (long.TryParse(tagSplit[1], out result) == false) {

					return;

				}

			} else {

				return;

			}

			original = result;

		}

		public static void TagMyDefIdCheck(string tag, ref MyDefinitionId original){
			
			MyDefinitionId result = new MyDefinitionId();
			var tagSplit = ProcessTag(tag);
			
			if(tagSplit.Length == 2){
				
				if(MyDefinitionId.TryParse(tagSplit[1], out result) == false){

					return;
					
				}
				
			}else{

				return;
				
			}
			
			original = result;
			
		}

		public static void TagMyDefIdCheck(string tag, ref List<MyDefinitionId> original) {

			MyDefinitionId result = new MyDefinitionId();
			var tagSplit = ProcessTag(tag);

			if (tagSplit.Length == 2) {

				if (MyDefinitionId.TryParse(tagSplit[1], out result) == false) {

					return;

				}

			} else {

				return;

			}

			original.Add(result);

		}

		public static void TagGuidCheck(string tag, ref Guid original) {

			var tagSplit = ProcessTag(tag);

			if (tagSplit.Length == 2) {

				var temp = tagSplit[1];
				try {

					var guid = new Guid(temp);
					original = guid;

				} catch (Exception e) {

					return;

				}

			}

		}

		public static void TagStringCheck(string tag, ref string original){
			
			var tagSplit = ProcessTag(tag);
			
			if(tagSplit.Length == 2){

				original = tagSplit[1];
				
			}

		}
		
		public static void TagStringListCheck(string tag, ref List<string> result){

			var tagSplit = ProcessTag(tag);
			
			if(tagSplit.Length == 2){
				
				var array = tagSplit[1].Split(',');
				
				foreach(var item in array){
					
					if(string.IsNullOrWhiteSpace(item)){
						
						continue;
						
					}
					
					result.Add(item);
					
				}

			}
			
		}
		
		public static void TagMDIDictionaryCheck(string tag, ref Dictionary<MyDefinitionId, MyDefinitionId> result) {
			
			var tagSplit = ProcessTag(tag);
			
			if(tagSplit.Length == 2){
				
				var array = tagSplit[1].Split(',');
				
				foreach(var item in array){
					
					if(string.IsNullOrWhiteSpace(item)){
						
						continue;
						
					}
					
					var secondSplit = item.Split('|');
					
					var targetId = new MyDefinitionId();
					var newId = new MyDefinitionId();
					
					if(secondSplit.Length == 2){
						
						MyDefinitionId.TryParse(secondSplit[0], out targetId);
						MyDefinitionId.TryParse(secondSplit[1], out newId);
						
					}
					
					if(targetId != new MyDefinitionId() && newId != new MyDefinitionId() && result.ContainsKey(targetId) == false){
						
						result.Add(targetId, newId);
					
					}
					
				}

			}
			
		}

		public static void TagStringDictionaryCheck(string tag, ref Dictionary<string, string> result) {

			var tagSplit = ProcessTag(tag);

			if(tagSplit.Length == 2) {

				var array = tagSplit[1].Split(',');

				foreach(var item in array) {

					if(string.IsNullOrWhiteSpace(item)) {

						continue;

					}

					var secondSplit = item.Split('|');

					string key = secondSplit[0];
					string val = secondSplit[1];

					if(string.IsNullOrWhiteSpace(key) == false && string.IsNullOrWhiteSpace(val) == false && result.ContainsKey(val) == false) {

						result.Add(key, val);

					}

				}

			}

		}

		public static void TagVector3DictionaryCheck(string tag, ref Dictionary<Vector3, Vector3> result) {

			var tagSplit = ProcessTag(tag);

			if(tagSplit.Length == 2) {

				var array = tagSplit[1].Split(',');

				foreach(var item in array) {

					if(string.IsNullOrWhiteSpace(item)) {

						continue;

					}

					var secondSplit = item.Split('|');

					string key = secondSplit[0];
					string val = secondSplit[1];

					key = FixVectorString(key);
					val = FixVectorString(val);

					if (string.IsNullOrWhiteSpace(key) == true || string.IsNullOrWhiteSpace(val) == true) {

						continue;

					}

					Vector3D keyVector = Vector3D.Zero;
					Vector3D valVector = Vector3D.Zero;

					if(Vector3D.TryParse(key, out keyVector) == false || Vector3D.TryParse(val, out valVector) == false) {

						continue;

					}

					result.Add(keyVector, valVector);

				}

			}

		}

		public static void TagVector3Check(string tag, ref List<Vector3> original) {

			Vector3D result = Vector3D.Zero;
			var tagSplit = ProcessTag(tag);

			if (tagSplit.Length == 2) {

				if (Vector3D.TryParse(FixVectorString(tagSplit[1]), out result) == false) {

					return;

				}

			} else {

				return;

			}

			original.Add(result);

		}

		public static void TagVector3DCheck(string tag, ref Vector3D original) {

			Vector3D result = Vector3D.Zero;
			var tagSplit = ProcessTag(tag);

			if(tagSplit.Length == 2) {

				if(Vector3D.TryParse(FixVectorString(tagSplit[1]), out result) == false) {

					return;

				}

			} else {

				return;

			}

			original = result;

		}

		public static void TagBoolListCheck(string tag, ref List<bool> result) {

			var tagSplit = ProcessTag(tag);

			if(tagSplit.Length == 2) {

				var array = tagSplit[1].Split(',');

				foreach(var item in array) {

					if(string.IsNullOrWhiteSpace(item)) {

						continue;

					}

					bool entry = false;

					if(bool.TryParse(item, out entry) == false) {

						continue;

					}

					result.Add(entry);

				}

			}

		}

		public static void TagVector3DListCheck(string tag, ref List<Vector3D> result) {

			var tagSplit = ProcessTag(tag);

			if(tagSplit.Length == 2) {

				var array = tagSplit[1].Split(',');

				foreach(var item in array) {

					if(string.IsNullOrWhiteSpace(item)) {

						continue;

					}

					Vector3D entry = Vector3D.Zero;

					if(Vector3D.TryParse(FixVectorString(item), out entry) == false) {

						continue;

					}

					result.Add(entry);

				}

			}

		}

		public static void TagVector3StringDictionaryCheck(string tag, ref Dictionary<Vector3, string> result) {

			var tagSplit = ProcessTag(tag);

			if(tagSplit.Length == 2) {

				var array = tagSplit[1].Split(',');

				foreach(var item in array) {

					if(string.IsNullOrWhiteSpace(item)) {

						continue;

					}

					var secondSplit = item.Split('|');

					string key = secondSplit[0];
					key = FixVectorString(key);
					string val = secondSplit[1];

					if(string.IsNullOrWhiteSpace(key) == true || string.IsNullOrWhiteSpace(val) == true) {

						continue;

					}

					Vector3D keyVector = Vector3D.Zero;

					if(Vector3D.TryParse(key, out keyVector) == false) {

						continue;

					}

					result.Add(keyVector, val);

				}

			}

		}

		public static void TagUlongListCheck(string tag, ref List<ulong> result){

			var tagSplit = ProcessTag(tag);
			
			if(tagSplit.Length == 2){
				
				var array = tagSplit[1].Split(',');
				
				foreach(var item in array){
					
					if(string.IsNullOrWhiteSpace(item))
						continue;

					ulong modId = 0;

					if (ulong.TryParse(item, out modId) == false)
						continue;
					
					result.Add(modId);
					
				}

			}

			result.RemoveAll(item => item == 0);
			
		}
		
	}
	
}