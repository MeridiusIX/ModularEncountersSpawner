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

			if(SpawnGroupManager.GroupInstance.Contains(Encoding.UTF8.GetString(Convert.FromBase64String("LnNibQ=="))) == true && SpawnGroupManager.GroupInstance.Contains(Encoding.UTF8.GetString(Convert.FromBase64String("MTUyMTkwNTg5MA=="))) == false) {

				SpawnGroups.Clear();
				return;

			}

			//Create Frequency Range Dictionaries

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

			if (spawnGroup.DirectionFromWorldCenter != Vector3D.Zero) {

				var normalizedDirection = Vector3D.Normalize(spawnGroup.DirectionFromWorldCenter);
				var angleFromCoords = SpawnResources.GetAngleBetweenDirections(normalizedDirection, environment.DirectionFromWorldCenter);

				if (spawnGroup.MinAngleFromDirection > 0 && angleFromCoords < spawnGroup.MinAngleFromDirection)
					return false;

				if (spawnGroup.MaxAngleFromDirection > 0 && angleFromCoords > spawnGroup.MinAngleFromDirection)
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

				if (!MES_SessionCore.Instance.WaterMod.Registered || environment.WaterInSurroundingAreaRatio < .1)
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
				if(tag.Contains("[SpawnGroupEnabled:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.SpawnGroupEnabled);
										
				}
				
				//SpaceCargoShip
				if(tag.Contains("[SpaceCargoShip:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.SpaceCargoShip);
										
				}
				
				//LunarCargoShip
				if(tag.Contains("[LunarCargoShip:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.LunarCargoShip);
						
				}
				
				//AtmosphericCargoShip
				if(tag.Contains("[AtmosphericCargoShip:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.AtmosphericCargoShip);
						
				}

				//GravityCargoShip
				if (tag.Contains("[GravityCargoShip:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.GravityCargoShip);

				}

				//SkipAirDensityCheck
				if (tag.Contains("[SkipAirDensityCheck:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.SkipAirDensityCheck);

				}

				//SpaceRandomEncounter
				if (tag.Contains("[SpaceRandomEncounter:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.SpaceRandomEncounter);
						
				}
				
				//PlanetaryInstallation
				if(tag.Contains("[PlanetaryInstallation:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.PlanetaryInstallation);
						
				}
				
				//PlanetaryInstallationType
				if(tag.Contains("[PlanetaryInstallationType:") == true){

					TagStringCheck(tag, ref improveSpawnGroup.PlanetaryInstallationType);
					
					if(improveSpawnGroup.PlanetaryInstallationType == ""){
						
						improveSpawnGroup.PlanetaryInstallationType = "Small";
						
					}
					
				}
				
				//SkipTerrainCheck
				if(tag.Contains("[SkipTerrainCheck:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.SkipTerrainCheck);
						
				}

				//RotateInstallations
				if(tag.Contains("[RotateInstallations:") == true) {

					TagVector3DListCheck(tag, ref improveSpawnGroup.RotateInstallations);

				}

				//InstallationTerrainValidation
				if (tag.Contains("[InstallationTerrainValidation:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.InstallationTerrainValidation);

				}

				//InstallationSpawnsOnDryLand
				if (tag.Contains("[InstallationSpawnsOnDryLand:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.InstallationSpawnsOnDryLand);

				}

				//InstallationSpawnsUnderwater
				if (tag.Contains("[InstallationSpawnsUnderwater:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.InstallationSpawnsUnderwater);

				}

				//InstallationSpawnsOnWaterSurface
				if (tag.Contains("[InstallationSpawnsOnWaterSurface:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.InstallationSpawnsOnWaterSurface);

				}

				//ReverseForwardDirections
				if (tag.Contains("[ReverseForwardDirections:") == true) {

					TagBoolListCheck(tag, ref improveSpawnGroup.ReverseForwardDirections);

				}

				//CutVoxelsAtAirtightCells
				if(tag.Contains("[CutVoxelsAtAirtightCells:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.CutVoxelsAtAirtightCells);
						
				}
				
				//BossEncounterSpace
				if(tag.Contains("[BossEncounterSpace:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.BossEncounterSpace);
						
				}
				
				//BossEncounterAtmo
				if(tag.Contains("[BossEncounterAtmo:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.BossEncounterAtmo);
						
				}
				
				//BossEncounterAny
				if(tag.Contains("[BossEncounterAny:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.BossEncounterAny);
						
				}

				//RivalAiSpawn
				if (tag.Contains("[RivalAiSpawn:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.RivalAiAnySpawn);

				}

				//RivalAiSpaceSpawn
				if (tag.Contains("[RivalAiSpaceSpawn:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.RivalAiSpaceSpawn);

				}

				//RivalAiAtmosphericSpawn
				if(tag.Contains("[RivalAiAtmosphericSpawn:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.RivalAiAtmosphericSpawn);

				}

				//RivalAiAnySpawn
				if(tag.Contains("[RivalAiAnySpawn:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.RivalAiAnySpawn);

				}

				//CreatureSpawn
				if (tag.Contains("[CreatureSpawn:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.CreatureSpawn);

				}

				//CreatureIds
				if (tag.Contains("[CreatureIds:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.CreatureIds);

				}

				//MinCreatureCount
				if (tag.Contains("[MinCreatureCount:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.MinCreatureCount);

				}

				//MaxCreatureCount
				if (tag.Contains("[MaxCreatureCount:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.MaxCreatureCount);

				}

				//MinCreatureDistance
				if (tag.Contains("[MinCreatureDistance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.MinCreatureDistance);

				}

				//MaxCreatureDistance
				if (tag.Contains("[MaxCreatureDistance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.MaxCreatureDistance);

				}

				//CanSpawnUnderwater
				if (tag.Contains("[CanSpawnUnderwater:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.CanSpawnUnderwater);

				}

				//MinWaterDepth
				if (tag.Contains("[MinWaterDepth:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.MinWaterDepth);

				}

				//Frequency
				improveSpawnGroup.Frequency = (int)Math.Round((double)spawnGroup.Frequency * 10);
				
				//UniqueEncounter
				if(tag.Contains("[UniqueEncounter:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.UniqueEncounter);
						
				}
				
				//FactionOwner
				if(tag.Contains("[FactionOwner:") == true){

					TagStringCheck(tag, ref improveSpawnGroup.FactionOwner);
					
					if(improveSpawnGroup.FactionOwner == ""){
						
						improveSpawnGroup.FactionOwner = "SPRT";
						
					}

				}

				//UseRandomMinerFaction
				if(tag.Contains("[UseRandomMinerFaction:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseRandomMinerFaction);

				}

				//UseRandomBuilderFaction
				if(tag.Contains("[UseRandomBuilderFaction:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseRandomBuilderFaction);

				}

				//UseRandomTraderFaction
				if(tag.Contains("[UseRandomTraderFaction:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseRandomTraderFaction);

				}

				//IgnoreCleanupRules
				if(tag.Contains("[IgnoreCleanupRules:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.IgnoreCleanupRules);
						
				}
				
				//ReplenishSystems
				if(tag.Contains("[ReplenishSystems:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.ReplenishSystems);
						
				}

				//UseNonPhysicalAmmo
				if(tag.Contains("[UseNonPhysicalAmmo:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseNonPhysicalAmmo);

				}

				//RemoveContainerContents
				if(tag.Contains("[RemoveContainerContents:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.RemoveContainerContents);

				}

				//InitializeStoreBlocks
				if(tag.Contains("[InitializeStoreBlocks:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.InitializeStoreBlocks);

				}

				//ContainerTypesForStoreOrders
				if(tag.Contains("[ContainerTypesForStoreOrders:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.ContainerTypesForStoreOrders);

				}

				//ForceStaticGrid
				if(tag.Contains("[ForceStaticGrid:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.ForceStaticGrid);
					setForceStatic = true;
					
				}
				
				//AdminSpawnOnly
				if(tag.Contains("[AdminSpawnOnly:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.AdminSpawnOnly);
						
				}

				//SandboxVariables
				if(tag.Contains("[SandboxVariables:") == true){

					TagStringListCheck(tag, ref improveSpawnGroup.SandboxVariables);
						
				}

				//FalseSandboxVariables
				if(tag.Contains("[FalseSandboxVariables:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.FalseSandboxVariables);

				}

				//RandomNumberRoll
				if(tag.Contains("[RandomNumberRoll:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.RandomNumberRoll);
						
				}

				//UseCommonConditions
				if(tag.Contains("[UseCommonConditions:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseCommonConditions);

				}

				//ChanceCeiling
				if (tag.Contains("[ChanceCeiling:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.ChanceCeiling);

				}

				//SpaceCargoShipChance
				if (tag.Contains("[SpaceCargoShipChance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.SpaceCargoShipChance);

				}

				//LunarCargoShipChance
				if (tag.Contains("[LunarCargoShipChance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.LunarCargoShipChance);

				}

				//AtmosphericCargoShipChance
				if (tag.Contains("[AtmosphericCargoShipChance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.AtmosphericCargoShipChance);

				}

				//GravityCargoShipChance
				if (tag.Contains("[GravityCargoShipChance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.GravityCargoShipChance);

				}

				//RandomEncounterChance
				if (tag.Contains("[RandomEncounterChance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.RandomEncounterChance);

				}

				//PlanetaryInstallationChance
				if (tag.Contains("[PlanetaryInstallationChance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.PlanetaryInstallationChance);

				}

				//BossEncounterChance
				if (tag.Contains("[BossEncounterChance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.BossEncounterChance);

				}

				//UseAutoPilotInSpace
				if (tag.Contains("[UseAutoPilotInSpace:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseAutoPilotInSpace);

				}

				//PauseAutopilotAtPlayerDistance
				if(tag.Contains("[PauseAutopilotAtPlayerDistance:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.PauseAutopilotAtPlayerDistance);

				}

				//PreventOwnershipChange
				if(tag.Contains("[PreventOwnershipChange:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.PreventOwnershipChange);

				}

				//RandomizeWeapons
				if(tag.Contains("[RandomizeWeapons:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.RandomizeWeapons);
						
				}
				
				//IgnoreWeaponRandomizerMod
				if(tag.Contains("[IgnoreWeaponRandomizerMod:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.IgnoreWeaponRandomizerMod);
						
				}
				
				//IgnoreWeaponRandomizerTargetGlobalBlacklist
				if(tag.Contains("[IgnoreWeaponRandomizerTargetGlobalBlacklist:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.IgnoreWeaponRandomizerTargetGlobalBlacklist);
						
				}
				
				//IgnoreWeaponRandomizerTargetGlobalWhitelist
				if(tag.Contains("[IgnoreWeaponRandomizerTargetGlobalWhitelist:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.IgnoreWeaponRandomizerTargetGlobalWhitelist);
						
				}
				
				//IgnoreWeaponRandomizerGlobalBlacklist
				if(tag.Contains("[IgnoreWeaponRandomizerGlobalBlacklist:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.IgnoreWeaponRandomizerGlobalBlacklist);
						
				}
				
				//IgnoreWeaponRandomizerGlobalWhitelist
				if(tag.Contains("[IgnoreWeaponRandomizerGlobalWhitelist:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.IgnoreWeaponRandomizerGlobalWhitelist);
						
				}
				
				//WeaponRandomizerTargetBlacklist
				if(tag.Contains("[WeaponRandomizerTargetBlacklist:") == true){

					 TagStringListCheck(tag, ref improveSpawnGroup.WeaponRandomizerTargetBlacklist);
						
				}
				
				//WeaponRandomizerTargetWhitelist
				if(tag.Contains("[WeaponRandomizerTargetWhitelist:") == true){

					 TagStringListCheck(tag, ref improveSpawnGroup.WeaponRandomizerTargetWhitelist);
						
				}
				
				//WeaponRandomizerBlacklist
				if(tag.Contains("[WeaponRandomizerBlacklist:") == true){

					TagStringListCheck(tag, ref improveSpawnGroup.WeaponRandomizerBlacklist);
						
				}
				
				//WeaponRandomizerWhitelist
				if(tag.Contains("[WeaponRandomizerWhitelist:") == true){

					TagStringListCheck(tag, ref improveSpawnGroup.WeaponRandomizerWhitelist);
						
				}

				//RandomWeaponChance
				if (tag.Contains("[RandomWeaponChance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.RandomWeaponChance);

				}

				//RandomWeaponSizeVariance
				if (tag.Contains("[RandomWeaponSizeVariance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.RandomWeaponSizeVariance);

				}

				//NonRandomWeaponNames
				if (tag.Contains("[NonRandomWeaponNames:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.NonRandomWeaponNames);

				}

				//NonRandomWeaponIds
				if (tag.Contains("[NonRandomWeaponIds:") == true) {

					TagMyDefIdCheck(tag, ref improveSpawnGroup.NonRandomWeaponIds);

				}

				//NonRandomWeaponReplacingOnly
				if (tag.Contains("[NonRandomWeaponReplacingOnly:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.NonRandomWeaponReplacingOnly);

				}

				//AddDefenseShieldBlocks
				if (tag.Contains("[AddDefenseShieldBlocks:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.AddDefenseShieldBlocks);

				}

				//IgnoreShieldProviderMod
				if (tag.Contains("[IgnoreShieldProviderMod:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.IgnoreShieldProviderMod);

				}

				//ShieldProviderChance
				if (tag.Contains("[ShieldProviderChance:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.ShieldProviderChance);

				}

				//ReplaceArmorBlocksWithModules
				if (tag.Contains("[ReplaceArmorBlocksWithModules:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.ReplaceArmorBlocksWithModules);

				}

				//ModulesForArmorReplacement
				if (tag.Contains("[ModulesForArmorReplacement:") == true) {

					TagMyDefIdCheck(tag, ref improveSpawnGroup.ModulesForArmorReplacement);

				}

				//UseBlockReplacerProfile
				if (tag.Contains("[UseBlockReplacerProfile:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.UseBlockReplacerProfile);
						
				}
				
				//BlockReplacerProfileNames
				if(tag.Contains("[BlockReplacerProfileNames:") == true){

					TagStringListCheck(tag, ref improveSpawnGroup.BlockReplacerProfileNames);
					
				}
				
				//UseBlockReplacer
				if(tag.Contains("[UseBlockReplacer:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.UseBlockReplacer);
						
				}
				
				//RelaxReplacedBlocksSize
				if(tag.Contains("[RelaxReplacedBlocksSize:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.RelaxReplacedBlocksSize);
						
				}
				
				//AlwaysRemoveBlock
				if(tag.Contains("[AlwaysRemoveBlock:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.AlwaysRemoveBlock);
						
				}

				//ConfigureSpecialNpcThrusters
				if (tag.Contains("[ConfigureSpecialNpcThrusters:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.ConfigureSpecialNpcThrusters);

				}

				//RestrictNpcIonThrust
				if (tag.Contains("[RestrictNpcIonThrust:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.RestrictNpcIonThrust);

				}

				//NpcIonThrustForceMultiply
				if (tag.Contains("[NpcIonThrustForceMultiply:") == true) {

					TagFloatCheck(tag, ref improveSpawnGroup.NpcIonThrustForceMultiply);

				}

				//NpcIonThrustPowerMultiply
				if (tag.Contains("[NpcIonThrustPowerMultiply:") == true) {

					TagFloatCheck(tag, ref improveSpawnGroup.NpcIonThrustPowerMultiply);

				}

				//RestrictNpcAtmoThrust
				if (tag.Contains("[RestrictNpcAtmoThrust:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.RestrictNpcAtmoThrust);

				}

				//NpcAtmoThrustForceMultiply
				if (tag.Contains("[NpcAtmoThrustForceMultiply:") == true) {

					TagFloatCheck(tag, ref improveSpawnGroup.NpcAtmoThrustForceMultiply);

				}

				//NpcAtmoThrustPowerMultiply
				if (tag.Contains("[NpcAtmoThrustPowerMultiply:") == true) {

					TagFloatCheck(tag, ref improveSpawnGroup.NpcAtmoThrustPowerMultiply);

				}

				//RestrictNpcHydroThrust
				if (tag.Contains("[RestrictNpcHydroThrust:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.RestrictNpcHydroThrust);

				}

				//NpcHydroThrustForceMultiply
				if (tag.Contains("[NpcHydroThrustForceMultiply:") == true) {

					TagFloatCheck(tag, ref improveSpawnGroup.NpcHydroThrustForceMultiply);

				}

				//NpcHydroThrustPowerMultiply
				if (tag.Contains("[NpcHydroThrustPowerMultiply:") == true) {

					TagFloatCheck(tag, ref improveSpawnGroup.NpcHydroThrustPowerMultiply);

				}

				//IgnoreGlobalBlockReplacer
				if (tag.Contains("[IgnoreGlobalBlockReplacer:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.IgnoreGlobalBlockReplacer);

				}

				//ReplaceBlockReference
				if(tag.Contains("[ReplaceBlockReference:") == true){

					TagMDIDictionaryCheck(tag, ref improveSpawnGroup.ReplaceBlockReference);
						
				}

				//ReplaceBlockOld
				if (tag.Contains("[ReplaceBlockOld:") == true) {

					TagMyDefIdCheck(tag, ref improveSpawnGroup.ReplaceBlockOld);

				}

				//ReplaceBlockNew
				if (tag.Contains("[ReplaceBlockNew:") == true) {

					TagMyDefIdCheck(tag, ref improveSpawnGroup.ReplaceBlockNew);

				}

				//ConvertToHeavyArmor
				if (tag.Contains("[ConvertToHeavyArmor:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.ConvertToHeavyArmor);
						
				}

				//UseRandomNameGenerator
				if(tag.Contains("[UseRandomNameGenerator:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseRandomNameGenerator);

				}

				//RandomGridNamePrefix
				if(tag.Contains("[RandomGridNamePrefix:") == true) {

					TagStringCheck(tag, ref improveSpawnGroup.RandomGridNamePrefix);

				}

				//RandomGridNamePattern
				if(tag.Contains("[RandomGridNamePattern:") == true) {

					TagStringCheck(tag, ref improveSpawnGroup.RandomGridNamePattern);

				}

				//ReplaceAntennaNameWithRandomizedName
				if(tag.Contains("[ReplaceAntennaNameWithRandomizedName:") == true) {

					TagStringCheck(tag, ref improveSpawnGroup.ReplaceAntennaNameWithRandomizedName);

				}

				//UseBlockNameReplacer
				if(tag.Contains("[UseBlockNameReplacer:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseBlockNameReplacer);

				}

				//BlockNameReplacerReference
				if(tag.Contains("[BlockNameReplacerReference:") == true) {

					TagStringDictionaryCheck(tag, ref improveSpawnGroup.BlockNameReplacerReference);

				}

				//ReplaceBlockNameOld
				if (tag.Contains("[ReplaceBlockNameOld:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.ReplaceBlockNameOld);

				}

				//ReplaceBlockNameNew
				if (tag.Contains("[ReplaceBlockNameNew:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.ReplaceBlockNameNew);

				}

				//AssignContainerTypesToAllCargo
				if (tag.Contains("[AssignContainerTypesToAllCargo:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.AssignContainerTypesToAllCargo);

				}

				//UseContainerTypeAssignment
				if(tag.Contains("[UseContainerTypeAssignment:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseContainerTypeAssignment);

				}

				//ContainerTypeAssignmentReference
				if (tag.Contains("[ContainerTypeAssignmentReference") == true) {

					TagStringDictionaryCheck(tag, ref improveSpawnGroup.ContainerTypeAssignmentReference);

				}

				//ContainerTypeAssignBlockName
				if (tag.Contains("[ContainerTypeAssignBlockName:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.ContainerTypeAssignBlockName);

				}

				//ContainerTypeAssignSubtypeId
				if (tag.Contains("[ContainerTypeAssignSubtypeId:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.ContainerTypeAssignSubtypeId);

				}

				//OverrideBlockDamageModifier
				if (tag.Contains("[OverrideBlockDamageModifier:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.OverrideBlockDamageModifier);
						
				}
				
				//BlockDamageModifier
				if(tag.Contains("[BlockDamageModifier:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.BlockDamageModifier);
						
				}

				//GridsAreEditable
				if(tag.Contains("[GridsAreEditable:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.GridsAreEditable);

				}

				//GridsAreDestructable
				if(tag.Contains("[GridsAreDestructable:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.GridsAreDestructable);

				}

				//ShiftBlockColorsHue
				if(tag.Contains("[ShiftBlockColorsHue:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.ShiftBlockColorsHue);
						
				}
				
				//RandomHueShift
				if(tag.Contains("[RandomHueShift:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.RandomHueShift);
						
				}

				//ShiftBlockColorAmount
				if(tag.Contains("[ShiftBlockColorAmount:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.ShiftBlockColorAmount);

				}

				//AssignGridSkin
				if(tag.Contains("[AssignGridSkin:") == true){

					TagStringListCheck(tag, ref improveSpawnGroup.AssignGridSkin);
						
				}

				//RecolorGrid
				if(tag.Contains("[RecolorGrid:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.RecolorGrid);

				}

				//ColorReferencePairs
				if(tag.Contains("[ColorReferencePairs:") == true) {

					TagVector3DictionaryCheck(tag, ref improveSpawnGroup.ColorReferencePairs);
					//Logger.AddMsg(improveSpawnGroup.ColorReferencePairs.Keys.Count.ToString() + " Color Reference Pairs");

				}

				//RecolorOld
				if (tag.Contains("[RecolorOld:") == true) {

					TagVector3Check(tag, ref improveSpawnGroup.RecolorOld);

				}

				//RecolorNew
				if (tag.Contains("[RecolorNew:") == true) {

					TagVector3Check(tag, ref improveSpawnGroup.RecolorNew);

				}

				//ColorSkinReferencePairs
				if (tag.Contains("[ColorSkinReferencePairs:") == true) {

					TagVector3StringDictionaryCheck(tag, ref improveSpawnGroup.ColorSkinReferencePairs);
					//Logger.AddMsg(improveSpawnGroup.ColorReferencePairs.Keys.Count.ToString() + " Color-Skin Reference Pairs");

				}

				//ReskinTarget
				if (tag.Contains("[ReskinTarget:") == true) {

					TagVector3Check(tag, ref improveSpawnGroup.ReskinTarget);

				}

				//ReskinTexture
				if (tag.Contains("[ReskinTexture:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.ReskinTexture);

				}

				//SkinRandomBlocks
				if (tag.Contains("[SkinRandomBlocks:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.SkinRandomBlocks);

				}

				//SkinRandomBlocksTextures
				if (tag.Contains("[SkinRandomBlocksTextures:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.SkinRandomBlocksTextures);

				}

				//MinPercentageSkinRandomBlocks
				if (tag.Contains("[MinPercentageSkinRandomBlocks:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.MinPercentageSkinRandomBlocks);

				}

				//MaxPercentageSkinRandomBlocks
				if (tag.Contains("[MaxPercentageSkinRandomBlocks:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.MaxPercentageSkinRandomBlocks);

				}

				//ReduceBlockBuildStates
				if (tag.Contains("[ReduceBlockBuildStates:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.ReduceBlockBuildStates);
						
				}
				
				//MinimumBlocksPercent
				if(tag.Contains("[MinimumBlocksPercent:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.MinimumBlocksPercent);
						
				}
				
				//MaximumBlocksPercent
				if(tag.Contains("[MaximumBlocksPercent:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.MaximumBlocksPercent);
						
				}
				
				//MinimumBuildPercent
				if(tag.Contains("[MinimumBuildPercent:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.MinimumBuildPercent);
						
				}
				
				//MaximumBuildPercent
				if(tag.Contains("[MaximumBuildPercent:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.MaximumBuildPercent);
						
				}
				
				//UseRivalAi
				if(tag.Contains("[UseRivalAi:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.UseRivalAi);
						
				}

				//RivalAiReplaceRemoteControl
				if(tag.Contains("[RivalAiReplaceRemoteControl:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.RivalAiReplaceRemoteControl);
						
				}

				//ApplyBehaviorToNamedBlock
				if (tag.Contains("[ApplyBehaviorToNamedBlock:") == true) {

					TagStringCheck(tag, ref improveSpawnGroup.ApplyBehaviorToNamedBlock);

				}

				//ConvertAllRemoteControlBlocks
				if (tag.Contains("[ConvertAllRemoteControlBlocks:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.ConvertAllRemoteControlBlocks);

				}

				//EraseIngameScripts
				if (tag.Contains("[EraseIngameScripts:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.EraseIngameScripts);
						
				}
				
				//DisableTimerBlocks
				if(tag.Contains("[DisableTimerBlocks:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.DisableTimerBlocks);
						
				}
				
				//DisableSensorBlocks
				if(tag.Contains("[DisableSensorBlocks:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.DisableSensorBlocks);
						
				}
				
				//DisableWarheads
				if(tag.Contains("[DisableWarheads:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.DisableWarheads);
						
				}
				
				//DisableThrustOverride
				if(tag.Contains("[DisableThrustOverride:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.DisableThrustOverride);
						
				}
				
				//DisableGyroOverride
				if(tag.Contains("[DisableGyroOverride:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.DisableGyroOverride);
						
				}
				
				//EraseLCDs
				if(tag.Contains("[EraseLCDs:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.EraseLCDs);
						
				}
				
				//UseTextureLCD
				if(tag.Contains("[UseTextureLCD:") == true){

					TagStringListCheck(tag, ref improveSpawnGroup.UseTextureLCD);
						
				}
				
				//EnableBlocksWithName
				if(tag.Contains("[EnableBlocksWithName:") == true){

					TagStringListCheck(tag, ref improveSpawnGroup.EnableBlocksWithName);
						
				}
				
				//DisableBlocksWithName
				if(tag.Contains("[DisableBlocksWithName:") == true){

					TagStringListCheck(tag, ref improveSpawnGroup.DisableBlocksWithName);
						
				}
				
				//AllowPartialNames
				if(tag.Contains("[AllowPartialNames:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.AllowPartialNames);
						
				}
				
				//ChangeTurretSettings
				if(tag.Contains("[ChangeTurretSettings:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.ChangeTurretSettings);
						
				}
				
				//TurretRange
				if(tag.Contains("[TurretRange:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.TurretRange);
						
				}
				
				//TurretIdleRotation
				if(tag.Contains("[TurretIdleRotation:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.TurretIdleRotation);
						
				}
				
				//TurretTargetMeteors
				if(tag.Contains("[TurretTargetMeteors:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.TurretTargetMeteors);
						
				}
				
				//TurretTargetMissiles
				if(tag.Contains("[TurretTargetMissiles:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.TurretTargetMissiles);
						
				}
				
				//TurretTargetCharacters
				if(tag.Contains("[TurretTargetCharacters:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.TurretTargetCharacters);
						
				}
				
				//TurretTargetSmallGrids
				if(tag.Contains("[TurretTargetSmallGrids:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.TurretTargetSmallGrids);
						
				}
				
				//TurretTargetLargeGrids
				if(tag.Contains("[TurretTargetLargeGrids:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.TurretTargetLargeGrids);
						
				}
				
				//TurretTargetStations
				if(tag.Contains("[TurretTargetStations:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.TurretTargetStations);
						
				}
				
				//TurretTargetNeutrals
				if(tag.Contains("[TurretTargetNeutrals:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.TurretTargetNeutrals);
						
				}
				
				//ClearAuthorship
				if(tag.Contains("[ClearAuthorship:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.ClearAuthorship);
						
				}
				
				//MinSpawnFromWorldCenter
				if(tag.Contains("[MinSpawnFromWorldCenter:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.MinSpawnFromWorldCenter);
						
				}
				
				//MaxSpawnFromWorldCenter
				if(tag.Contains("[MaxSpawnFromWorldCenter:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.MaxSpawnFromWorldCenter);
						
				}

				//CustomWorldCenter
				if (tag.Contains("[DirectionFromWorldCenter:") == true) {

					TagVector3DCheck(tag, ref improveSpawnGroup.DirectionFromWorldCenter);

				}

				//DirectionFromWorldCenter
				if (tag.Contains("[DirectionFromWorldCenter:") == true) {

					TagVector3DCheck(tag, ref improveSpawnGroup.DirectionFromWorldCenter);

				}

				//MinAngleFromDirection
				if (tag.Contains("[MinAngleFromDirection:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.MinAngleFromDirection);

				}

				//MaxAngleFromDirection
				if (tag.Contains("[MaxAngleFromDirection:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.MaxAngleFromDirection);

				}

				//MinSpawnFromPlanetSurface
				if (tag.Contains("[MinSpawnFromPlanetSurface:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.MinSpawnFromPlanetSurface);

				}

				//MaxSpawnFromPlanetSurface
				if (tag.Contains("[MaxSpawnFromPlanetSurface:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.MaxSpawnFromPlanetSurface);

				}

				//UseDayOrNightOnly
				if (tag.Contains("[UseDayOrNightOnly:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseDayOrNightOnly);

				}

				//SpawnOnlyAtNight
				if (tag.Contains("[SpawnOnlyAtNight:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.SpawnOnlyAtNight);

				}

				//UseWeatherSpawning
				if (tag.Contains("[UseWeatherSpawning:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseWeatherSpawning);

				}

				//AllowedWeatherSystems
				if (tag.Contains("[AllowedWeatherSystems:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.AllowedWeatherSystems);

				}

				//UseTerrainTypeValidation
				if (tag.Contains("[UseTerrainTypeValidation:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseTerrainTypeValidation);

				}

				//AllowedTerrainTypes
				if (tag.Contains("[AllowedTerrainTypes:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.AllowedTerrainTypes);

				}

				//MinAirDensity
				if (tag.Contains("[MinAirDensity:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.MinAirDensity);

				}

				//MaxAirDensity
				if (tag.Contains("[MaxAirDensity:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.MaxAirDensity);

				}

				//MinGravity
				if (tag.Contains("[MinGravity:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.MinGravity);

				}

				//MaxGravity
				if (tag.Contains("[MaxGravity:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.MaxGravity);

				}

				//PlanetBlacklist
				if (tag.Contains("[PlanetBlacklist:") == true){

					TagStringListCheck(tag, ref improveSpawnGroup.PlanetBlacklist);
						
				}
				
				//PlanetWhitelist
				if(tag.Contains("[PlanetWhitelist:") == true){

					TagStringListCheck(tag, ref improveSpawnGroup.PlanetWhitelist);
						
				}
				
				//PlanetRequiresVacuum
				if(tag.Contains("[PlanetRequiresVacuum:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.PlanetRequiresVacuum);
						
				}
				
				//PlanetRequiresAtmo
				if(tag.Contains("[PlanetRequiresAtmo:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.PlanetRequiresAtmo);
					setAtmoRequired = true;
						
				}
				
				//PlanetRequiresOxygen
				if(tag.Contains("[PlanetRequiresOxygen:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.PlanetRequiresOxygen);
						
				}
				
				//PlanetMinimumSize
				if(tag.Contains("[PlanetMinimumSize:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.PlanetMinimumSize);
						
				}
				
				//PlanetMaximumSize
				if(tag.Contains("[PlanetMaximumSize:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.PlanetMaximumSize);
						
				}

				//UsePlayerCountCheck
				if(tag.Contains("[UsePlayerCountCheck:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UsePlayerCountCheck);

				}

				//PlayerCountCheckRadius
				if(tag.Contains("[PlayerCountCheckRadius:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.PlayerCountCheckRadius);

				}

				//MinimumPlayers
				if(tag.Contains("[MinimumPlayers:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.MinimumPlayers);

				}

				//MaximumPlayers
				if(tag.Contains("[MaximumPlayers:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.MaximumPlayers);

				}

				//UseThreatLevelCheck
				if(tag.Contains("[UseThreatLevelCheck:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.UseThreatLevelCheck);
						
				}
				
				//ThreatLevelCheckRange
				if(tag.Contains("[ThreatLevelCheckRange:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.ThreatLevelCheckRange);
						
				}
				
				//ThreatIncludeOtherNpcOwners
				if(tag.Contains("[ThreatIncludeOtherNpcOwners:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.ThreatIncludeOtherNpcOwners);
						
				}
				
				//ThreatScoreMinimum
				if(tag.Contains("[ThreatScoreMinimum:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.ThreatScoreMinimum);
						
				}
				
				//ThreatScoreMaximum
				if(tag.Contains("[ThreatScoreMaximum:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.ThreatScoreMaximum);
				
				}
				
				//UsePCUCheck
				if(tag.Contains("[UsePCUCheck:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.UsePCUCheck);
						
				}
				
				//PCUCheckRadius
				if(tag.Contains("[PCUCheckRadius:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.PCUCheckRadius);
						
				}
				
				//PCUMinimum
				if(tag.Contains("[PCUMinimum:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.PCUMinimum);
						
				}
				
				//PCUMaximum
				if(tag.Contains("[PCUMaximum:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.PCUMaximum);
						
				}
				
				//UsePlayerCredits
				if(tag.Contains("[UsePlayerCredits:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.UsePlayerCredits);
						
				}

				//IncludeAllPlayersInRadius
				if(tag.Contains("[IncludeAllPlayersInRadius:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.IncludeAllPlayersInRadius);

				}

				//IncludeFactionBalance
				if(tag.Contains("[IncludeFactionBalance:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.IncludeFactionBalance);

				}

				//PlayerCreditsCheckRadius
				if(tag.Contains("[PlayerCreditsCheckRadius:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.PlayerCreditsCheckRadius);
						
				}
				
				//MinimumPlayerCredits
				if(tag.Contains("[MinimumPlayerCredits:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.MinimumPlayerCredits);
						
				}
				
				//MaximumPlayerCredits
				if(tag.Contains("[MaximumPlayerCredits:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.MaximumPlayerCredits);
						
				}
				
				//UsePlayerFactionReputation
				if(tag.Contains("[UsePlayerFactionReputation:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.UsePlayerFactionReputation);
						
				}
				
				//PlayerReputationCheckRadius
				if(tag.Contains("[PlayerReputationCheckRadius:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.PlayerReputationCheckRadius);
				
				}
				
				//CheckReputationAgainstOtherNPCFaction
				if(tag.Contains("[CheckReputationAgainstOtherNPCFaction:") == true){

					TagStringCheck(tag, ref improveSpawnGroup.CheckReputationAgainstOtherNPCFaction);
				
				}
				
				//MinimumReputation
				if(tag.Contains("[MinimumReputation:") == true){
				
					TagIntCheck(tag, ref improveSpawnGroup.MinimumReputation);
				
				}
				
				//MaximumReputation
				if(tag.Contains("[MaximumReputation:") == true){

					TagIntCheck(tag, ref improveSpawnGroup.MaximumReputation);
						
				}

				//ChargeNpcFactionForSpawn
				if (tag.Contains("[ChargeNpcFactionForSpawn:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.ChargeNpcFactionForSpawn);

				}

				//ChargeForSpawning
				if (tag.Contains("[ChargeForSpawning:") == true) {

					TagLongCheck(tag, ref improveSpawnGroup.ChargeForSpawning);

				}

				//UseSandboxCounterCosts
				if (tag.Contains("[UseSandboxCounterCosts:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseSandboxCounterCosts);

				}

				//SandboxCounterCostNames
				if (tag.Contains("[SandboxCounterCostNames:") == true) {

					TagStringListCheck(tag, ref improveSpawnGroup.SandboxCounterCostNames);

				}

				//SandboxCounterCostAmounts
				if (tag.Contains("[SandboxCounterCostAmounts:") == true) {

					TagIntListCheck(tag, ref improveSpawnGroup.SandboxCounterCostAmounts);

				}

				//UseRemoteControlCodeRestrictions
				if (tag.Contains("[UseRemoteControlCodeRestrictions:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseRemoteControlCodeRestrictions);

				}

				//RemoteControlCode
				if (tag.Contains("[RemoteControlCode:") == true) {

					TagStringCheck(tag, ref improveSpawnGroup.RemoteControlCode);

				}

				//RemoteControlCodeMinDistance
				if (tag.Contains("[RemoteControlCodeMinDistance:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.RemoteControlCodeMinDistance);

				}

				//RemoteControlCodeMaxDistance
				if (tag.Contains("[RemoteControlCodeMaxDistance:") == true) {

					TagDoubleCheck(tag, ref improveSpawnGroup.RemoteControlCodeMaxDistance);

				}

				//RequireAllMods
				if (tag.Contains("[RequiredMods:") == true || tag.Contains("[RequireAllMods") == true){

					TagUlongListCheck(tag, ref improveSpawnGroup.RequireAllMods);
						
				}
				
				//ExcludeAnyMods
				if(tag.Contains("[ExcludedMods:") == true || tag.Contains("[ExcludeAnyMods") == true){

					TagUlongListCheck(tag, ref improveSpawnGroup.ExcludeAnyMods);
						
				}
				
				//RequireAnyMods
				if(tag.Contains("[RequireAnyMods:") == true){

					TagUlongListCheck(tag, ref improveSpawnGroup.RequireAnyMods);
						
				}
				
				//ExcludeAllMods
				if(tag.Contains("[ExcludeAllMods:") == true){

					TagUlongListCheck(tag, ref improveSpawnGroup.ExcludeAllMods);
						
				}
				
				//RequiredPlayersOnline
				if(tag.Contains("[RequiredPlayersOnline:") == true){

					TagUlongListCheck(tag, ref improveSpawnGroup.RequiredPlayersOnline);
						
				}

				//RequiredAnyPlayersOnline
				if (tag.Contains("[RequiredAnyPlayersOnline:") == true) {

					TagUlongListCheck(tag, ref improveSpawnGroup.RequiredAnyPlayersOnline);

				}

				//AttachModStorageComponentToGrid
				if (tag.Contains("[AttachModStorageComponentToGrid:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.AttachModStorageComponentToGrid);
						
				}
				
				//StorageKey
				if(tag.Contains("[StorageKey:") == true){

					TagGuidCheck(tag, ref improveSpawnGroup.StorageKey);
		
				}
				
				//StorageValue
				if(tag.Contains("[StorageValue:") == true){

					TagStringCheck(tag, ref improveSpawnGroup.StorageValue);
				
				}

				//UseKnownPlayerLocations
				if(tag.Contains("[UseKnownPlayerLocations:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.UseKnownPlayerLocations);

				}

				//KnownPlayerLocationMustMatchFaction
				if(tag.Contains("[KnownPlayerLocationMustMatchFaction:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.KnownPlayerLocationMustMatchFaction);

				}

				//KnownPlayerLocationMinSpawnedEncounters
				if(tag.Contains("[KnownPlayerLocationMinSpawnedEncounters:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.KnownPlayerLocationMinSpawnedEncounters);

				}

				//KnownPlayerLocationMaxSpawnedEncounters
				if(tag.Contains("[KnownPlayerLocationMaxSpawnedEncounters:") == true) {

					TagIntCheck(tag, ref improveSpawnGroup.KnownPlayerLocationMaxSpawnedEncounters);

				}

				//Territory
				if(tag.Contains("[Territory:") == true){

					TagStringCheck(tag, ref improveSpawnGroup.Territory);
						
				}
				
				//MinDistanceFromTerritoryCenter
				if(tag.Contains("[MinDistanceFromTerritoryCenter:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.MinDistanceFromTerritoryCenter);
						
				}
				
				//MaxDistanceFromTerritoryCenter
				if(tag.Contains("[MaxDistanceFromTerritoryCenter:") == true){

					TagDoubleCheck(tag, ref improveSpawnGroup.MaxDistanceFromTerritoryCenter);
						
				}
				
				//RotateFirstCockpitToForward
				if(tag.Contains("[RotateFirstCockpitToForward:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.RotateFirstCockpitToForward);
						
				}
				
				//PositionAtFirstCockpit
				if(tag.Contains("[PositionAtFirstCockpit:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.PositionAtFirstCockpit);
						
				}
				
				//SpawnRandomCargo
				if(tag.Contains("[SpawnRandomCargo:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.SpawnRandomCargo);
						
				}
				
				//DisableDampeners
				if(tag.Contains("[DisableDampeners:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.DisableDampeners);
					setDampeners = true;
						
				}
				
				//ReactorsOn
				if(tag.Contains("[ReactorsOn:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.ReactorsOn);
						
				}

				//RemoveVoxelsIfGridRemoved
				if(tag.Contains("[RemoveVoxelsIfGridRemoved:") == true) {

					TagBoolCheck(tag, ref improveSpawnGroup.RemoveVoxelsIfGridRemoved);

				}

				//BossCustomAnnounceEnable
				if(tag.Contains("[BossCustomAnnounceEnable:") == true){

					TagBoolCheck(tag, ref improveSpawnGroup.BossCustomAnnounceEnable);
						
				}
				
				//BossCustomAnnounceAuthor
				if(tag.Contains("[BossCustomAnnounceAuthor:") == true){

					TagStringCheck(tag, ref improveSpawnGroup.BossCustomAnnounceAuthor);
						
				}
				
				//BossCustomAnnounceMessage
				if(tag.Contains("[BossCustomAnnounceMessage:") == true){

					TagStringCheck(tag, ref improveSpawnGroup.BossCustomAnnounceMessage);
						
				}
				
				//BossCustomGPSLabel
				if(tag.Contains("[BossCustomGPSLabel:") == true){

					TagStringCheck(tag, ref improveSpawnGroup.BossCustomGPSLabel);
						
				}

				//BossMusicId
				if (tag.Contains("[BossMusicId:") == true) {

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