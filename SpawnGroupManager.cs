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
using ModularEncountersSpawner.Spawners;

namespace ModularEncountersSpawner{
	
	public static class SpawnGroupManager{
		
		public static List<ImprovedSpawnGroup> SpawnGroups = new List<ImprovedSpawnGroup>();
		public static List<string> PlanetNames = new List<string>();
		
		public static Dictionary<string, Vector2> SpaceCargoShipFrequencyRange = new Dictionary<string, Vector2>();
		public static Dictionary<string, Vector2> RandomEncounterFrequencyRange = new Dictionary<string, Vector2>();
		public static Dictionary<string, Vector2> PlanetaryCargoShipFrequencyRange = new Dictionary<string, Vector2>();
		public static Dictionary<string, Vector2> PlanetaryInstallationFrequencyRange = new Dictionary<string, Vector2>();
		public static Dictionary<string, Vector2> BossEncounterFrequencyRange = new Dictionary<string, Vector2>();
		
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

		public static bool CheckSpawnGroupPlanetLists(ImprovedSpawnGroup spawnGroup, MyPlanet planet){
			
			string planetName = "";
				
			if(planet != null){
				
				planetName = planet.Generator.Id.SubtypeId.ToString();
				
			}else{
				
				if(spawnGroup.AtmosphericCargoShip == true){
					
					return false;
					
				}
				
				return true;
				
			}
			
			if(spawnGroup.PlanetBlacklist.Count > 0 && Settings.General.IgnorePlanetBlacklists == false){
				
				if(spawnGroup.PlanetBlacklist.Contains(planetName) == true){
					
					return false;
					
				}
				
			}
			
			if(spawnGroup.PlanetWhitelist.Count > 0 && Settings.General.IgnorePlanetWhitelists == false){
				
				if(spawnGroup.PlanetWhitelist.Contains(planetName) == false){
					
					return false;
					
				}
				
			}
			
			var planetEntity = planet as IMyEntity;
			var sealevel = Vector3D.Up * (double)planet.MinimumRadius + planetEntity.GetPosition();

			if(spawnGroup.PlanetRequiresVacuum == true && planet.GetAirDensity(sealevel) > 0){
				
				return false;
				
			}

			if(spawnGroup.PlanetRequiresAtmo == true && planet.GetAirDensity(sealevel) == 0){
				
				return false;
				
			}

			if(spawnGroup.PlanetRequiresOxygen == true && planet.GetOxygenForPosition(sealevel) == 0){
				
				return false;
				
			}

			if(spawnGroup.PlanetMinimumSize > 0 && planet.MinimumRadius * 2 < spawnGroup.PlanetMinimumSize){
				
				return false;
				
			}

			if(spawnGroup.PlanetMaximumSize > 0 && planet.MaximumRadius * 2 < spawnGroup.PlanetMaximumSize){
				
				return false;
				
			}
			
			return true;
			
		}
		
		public static bool DistanceFromCenterCheck(ImprovedSpawnGroup spawnGroup, Vector3D checkCoords){
			
			if(spawnGroup.MinSpawnFromWorldCenter > 0){
				
				if(Vector3D.Distance(Vector3D.Zero, checkCoords) < spawnGroup.MinSpawnFromWorldCenter){
					
					return false;
					
				}
				
			}
			
			if(spawnGroup.MaxSpawnFromWorldCenter > 0){
				
				if(Vector3D.Distance(Vector3D.Zero, checkCoords) > spawnGroup.MaxSpawnFromWorldCenter){
					
					return false;
					
				}
				
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
						
			foreach(var tag in descSplit){

				//SpawnGroupEnabled
				if(tag.Contains("[SpawnGroupEnabled") == true){

					improveSpawnGroup.SpawnGroupEnabled = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
										
				}
				
				//SpaceCargoShip
				if(tag.Contains("[SpaceCargoShip") == true){

					improveSpawnGroup.SpaceCargoShip = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
										
				}
				
				//LunarCargoShip
				if(tag.Contains("[LunarCargoShip") == true){

					improveSpawnGroup.LunarCargoShip = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//AtmosphericCargoShip
				if(tag.Contains("[AtmosphericCargoShip") == true){

					improveSpawnGroup.AtmosphericCargoShip = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//SpaceRandomEncounter
				if(tag.Contains("[SpaceRandomEncounter") == true){

					improveSpawnGroup.SpaceRandomEncounter = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//PlanetaryInstallation
				if(tag.Contains("[PlanetaryInstallation:") == true){

					improveSpawnGroup.PlanetaryInstallation = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//PlanetaryInstallationType
				if(tag.Contains("[PlanetaryInstallationType") == true){

					improveSpawnGroup.PlanetaryInstallationType = TagStringCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
					
					if(improveSpawnGroup.PlanetaryInstallationType == ""){
						
						improveSpawnGroup.PlanetaryInstallationType = "Small";
						
					}
					
				}
				
				//SkipTerrainCheck
				if(tag.Contains("[SkipTerrainCheck:") == true){

					improveSpawnGroup.SkipTerrainCheck = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}

                //RotateInstallations
                if(tag.Contains("[RotateInstallations") == true) {

                    improveSpawnGroup.RotateInstallations = TagVector3DListCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //ReverseForwardDirections
                if(tag.Contains("[ReverseForwardDirections") == true) {

                    improveSpawnGroup.ReverseForwardDirections = TagBoolListCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //CutVoxelsAtAirtightCells
                if(tag.Contains("[CutVoxelsAtAirtightCells:") == true){

					improveSpawnGroup.CutVoxelsAtAirtightCells = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//BossEncounterSpace
				if(tag.Contains("[BossEncounterSpace") == true){

					improveSpawnGroup.BossEncounterSpace = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//BossEncounterAtmo
				if(tag.Contains("[BossEncounterAtmo") == true){

					improveSpawnGroup.BossEncounterAtmo = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//BossEncounterAny
				if(tag.Contains("[BossEncounterAny") == true){

					improveSpawnGroup.BossEncounterAny = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}

                //RivalAiSpaceSpawn
                if(tag.Contains("[RivalAiSpaceSpawn") == true) {

                    improveSpawnGroup.RivalAiSpaceSpawn = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //RivalAiAtmosphericSpawn
                if(tag.Contains("[RivalAiAtmosphericSpawn") == true) {

                    improveSpawnGroup.RivalAiAtmosphericSpawn = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //RivalAiAnySpawn
                if(tag.Contains("[RivalAiAnySpawn") == true) {

                    improveSpawnGroup.RivalAiAnySpawn = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //Frequency
                improveSpawnGroup.Frequency = (int)Math.Round((double)spawnGroup.Frequency * 10);
				
				//UniqueEncounter
				if(tag.Contains("[UniqueEncounter") == true){

					improveSpawnGroup.UniqueEncounter = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//FactionOwner
				if(tag.Contains("[FactionOwner") == true){

					improveSpawnGroup.FactionOwner = TagStringCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
					
					if(improveSpawnGroup.FactionOwner == ""){
						
						improveSpawnGroup.FactionOwner = "SPRT";
						
					}

				}

                //UseRandomMinerFaction
                if(tag.Contains("[UseRandomMinerFaction") == true) {

                    improveSpawnGroup.UseRandomMinerFaction = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //UseRandomBuilderFaction
                if(tag.Contains("[UseRandomBuilderFaction") == true) {

                    improveSpawnGroup.UseRandomBuilderFaction = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //UseRandomTraderFaction
                if(tag.Contains("[UseRandomTraderFaction") == true) {

                    improveSpawnGroup.UseRandomTraderFaction = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //IgnoreCleanupRules
                if(tag.Contains("[IgnoreCleanupRules") == true){

					improveSpawnGroup.IgnoreCleanupRules = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//ReplenishSystems
				if(tag.Contains("[ReplenishSystems") == true){

					improveSpawnGroup.ReplenishSystems = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}

                //UseNonPhysicalAmmo
                if(tag.Contains("[UseNonPhysicalAmmo") == true) {

                    improveSpawnGroup.UseNonPhysicalAmmo = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //RemoveContainerContents
                if(tag.Contains("[RemoveContainerContents") == true) {

                    improveSpawnGroup.RemoveContainerContents = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //InitializeStoreBlocks
                if(tag.Contains("[InitializeStoreBlocks") == true) {

                    improveSpawnGroup.InitializeStoreBlocks = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //ContainerTypesForStoreOrders
                if(tag.Contains("[ContainerTypesForStoreOrders") == true) {

                    improveSpawnGroup.ContainerTypesForStoreOrders = TagStringListCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //ForceStaticGrid
                if(tag.Contains("[ForceStaticGrid") == true){

					improveSpawnGroup.ForceStaticGrid = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
					setForceStatic = true;
					
				}
				
				//AdminSpawnOnly
				if(tag.Contains("[AdminSpawnOnly") == true){

					improveSpawnGroup.AdminSpawnOnly = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}

				//SandboxVariables
				if(tag.Contains("[SandboxVariables") == true){

					improveSpawnGroup.SandboxVariables = TagStringListCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}

                //FalseSandboxVariables
                if(tag.Contains("[FalseSandboxVariables") == true) {

                    improveSpawnGroup.FalseSandboxVariables = TagStringListCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //RandomNumberRoll
                if(tag.Contains("[RandomNumberRoll") == true){

					improveSpawnGroup.RandomNumberRoll = TagIntCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.RandomNumberRoll, out badParse);
						
				}

                //UseCommonConditions
                if(tag.Contains("[UseCommonConditions") == true) {

                    improveSpawnGroup.UseCommonConditions = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //UseAutoPilotInSpace
                if(tag.Contains("[UseAutoPilotInSpace") == true) {

                    improveSpawnGroup.UseAutoPilotInSpace = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //PauseAutopilotAtPlayerDistance
                if(tag.Contains("[PauseAutopilotAtPlayerDistance") == true) {

                    improveSpawnGroup.PauseAutopilotAtPlayerDistance = TagDoubleCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.PauseAutopilotAtPlayerDistance, out badParse);

                }

                //PreventOwnershipChange
                if(tag.Contains("[PreventOwnershipChange") == true) {

                    improveSpawnGroup.PreventOwnershipChange = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //RandomizeWeapons
                if(tag.Contains("[RandomizeWeapons") == true){

					improveSpawnGroup.RandomizeWeapons = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//IgnoreWeaponRandomizerMod
				if(tag.Contains("[IgnoreWeaponRandomizerMod") == true){

					improveSpawnGroup.IgnoreWeaponRandomizerMod = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//IgnoreWeaponRandomizerTargetGlobalBlacklist
				if(tag.Contains("[IgnoreWeaponRandomizerTargetGlobalBlacklist") == true){

					improveSpawnGroup.IgnoreWeaponRandomizerTargetGlobalBlacklist = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//IgnoreWeaponRandomizerTargetGlobalWhitelist
				if(tag.Contains("[IgnoreWeaponRandomizerTargetGlobalWhitelist") == true){

					improveSpawnGroup.IgnoreWeaponRandomizerTargetGlobalWhitelist = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//IgnoreWeaponRandomizerGlobalBlacklist
				if(tag.Contains("[IgnoreWeaponRandomizerGlobalBlacklist") == true){

					improveSpawnGroup.IgnoreWeaponRandomizerGlobalBlacklist = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//IgnoreWeaponRandomizerGlobalWhitelist
				if(tag.Contains("[IgnoreWeaponRandomizerGlobalWhitelist") == true){

					improveSpawnGroup.IgnoreWeaponRandomizerGlobalWhitelist = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//WeaponRandomizerTargetBlacklist
				if(tag.Contains("[WeaponRandomizerTargetBlacklist") == true){

					improveSpawnGroup.WeaponRandomizerTargetBlacklist = TagStringListCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//WeaponRandomizerTargetWhitelist
				if(tag.Contains("[WeaponRandomizerTargetWhitelist") == true){

					improveSpawnGroup.WeaponRandomizerTargetWhitelist = TagStringListCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//WeaponRandomizerBlacklist
				if(tag.Contains("[WeaponRandomizerBlacklist") == true){

					improveSpawnGroup.WeaponRandomizerBlacklist = TagStringListCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//WeaponRandomizerWhitelist
				if(tag.Contains("[WeaponRandomizerWhitelist") == true){

					improveSpawnGroup.WeaponRandomizerWhitelist = TagStringListCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//UseBlockReplacerProfile
				if(tag.Contains("[UseBlockReplacerProfile") == true){

					improveSpawnGroup.UseBlockReplacerProfile = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//BlockReplacerProfileNames
				if(tag.Contains("[BlockReplacerProfileNames") == true){

					improveSpawnGroup.BlockReplacerProfileNames = TagStringListCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
					
				}
				
				//UseBlockReplacer
				if(tag.Contains("[UseBlockReplacer") == true){

					improveSpawnGroup.UseBlockReplacer = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//RelaxReplacedBlocksSize
				if(tag.Contains("[RelaxReplacedBlocksSize") == true){

					improveSpawnGroup.RelaxReplacedBlocksSize = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//AlwaysRemoveBlock
				if(tag.Contains("[AlwaysRemoveBlock") == true){

					improveSpawnGroup.AlwaysRemoveBlock = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}

                //IgnoreGlobalBlockReplacer
                if(tag.Contains("[IgnoreGlobalBlockReplacer") == true) {

                    improveSpawnGroup.IgnoreGlobalBlockReplacer = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //ReplaceBlockReference
                if(tag.Contains("[ReplaceBlockReference") == true){

					improveSpawnGroup.ReplaceBlockReference = TagMDIDictionaryCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//ConvertToHeavyArmor
				if(tag.Contains("[ConvertToHeavyArmor") == true){

					improveSpawnGroup.ConvertToHeavyArmor = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}

                //UseRandomNameGenerator
                if(tag.Contains("[UseRandomNameGenerator") == true) {

                    improveSpawnGroup.UseRandomNameGenerator = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //RandomGridNamePrefix
                if(tag.Contains("[RandomGridNamePrefix") == true) {

                    improveSpawnGroup.RandomGridNamePrefix = TagStringCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //RandomGridNamePattern
                if(tag.Contains("[RandomGridNamePattern") == true) {

                    improveSpawnGroup.RandomGridNamePattern = TagStringCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //ReplaceAntennaNameWithRandomizedName
                if(tag.Contains("[ReplaceAntennaNameWithRandomizedName") == true) {

                    improveSpawnGroup.ReplaceAntennaNameWithRandomizedName = TagStringCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //UseBlockNameReplacer
                if(tag.Contains("[UseBlockNameReplacer") == true) {

                    improveSpawnGroup.UseBlockNameReplacer = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //BlockNameReplacerReference
                if(tag.Contains("[BlockNameReplacerReference") == true) {

                    improveSpawnGroup.BlockNameReplacerReference = TagStringDictionaryCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //AssignContainerTypesToAllCargo
                if(tag.Contains("[AssignContainerTypesToAllCargo") == true) {

                    improveSpawnGroup.AssignContainerTypesToAllCargo = TagStringListCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //UseContainerTypeAssignment
                if(tag.Contains("[UseContainerTypeAssignment") == true) {

                    improveSpawnGroup.UseContainerTypeAssignment = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //ContainerTypeAssignmentReference
                if(tag.Contains("[ContainerTypeAssignmentReference") == true) {

                    improveSpawnGroup.ContainerTypeAssignmentReference = TagStringDictionaryCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //OverrideBlockDamageModifier
                if(tag.Contains("[OverrideBlockDamageModifier") == true){

					improveSpawnGroup.OverrideBlockDamageModifier = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//BlockDamageModifier
				if(tag.Contains("[BlockDamageModifier") == true){

					improveSpawnGroup.BlockDamageModifier = TagDoubleCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.BlockDamageModifier, out badParse);
						
				}

                //GridsAreEditable
                if(tag.Contains("[GridsAreEditable") == true) {

                    improveSpawnGroup.GridsAreEditable = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //GridsAreDestructable
                if(tag.Contains("[GridsAreDestructable") == true) {

                    improveSpawnGroup.GridsAreDestructable = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //ShiftBlockColorsHue
                if(tag.Contains("[ShiftBlockColorsHue") == true){

					improveSpawnGroup.ShiftBlockColorsHue = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//RandomHueShift
				if(tag.Contains("[RandomHueShift") == true){

					improveSpawnGroup.RandomHueShift = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}

                //ShiftBlockColorAmount
                if(tag.Contains("[ShiftBlockColorAmount") == true) {

                    improveSpawnGroup.ShiftBlockColorAmount = TagDoubleCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.ShiftBlockColorAmount, out badParse);

                }

                //AssignGridSkin
                if(tag.Contains("[AssignGridSkin") == true){

					improveSpawnGroup.AssignGridSkin = TagStringListCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}

                //RecolorGrid
                if(tag.Contains("[RecolorGrid") == true) {

                    improveSpawnGroup.RecolorGrid = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //ColorReferencePairs
                if(tag.Contains("[ColorReferencePairs") == true) {

                    improveSpawnGroup.ColorReferencePairs = TagVector3DictionaryCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //ColorSkinReferencePairs
                if(tag.Contains("[ColorSkinReferencePairs") == true) {

                    improveSpawnGroup.ColorSkinReferencePairs = TagVector3StringDictionaryCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //ReduceBlockBuildStates
                if(tag.Contains("[ReduceBlockBuildStates") == true){

					improveSpawnGroup.ReduceBlockBuildStates = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//MinimumBlocksPercent
				if(tag.Contains("[MinimumBlocksPercent") == true){

					improveSpawnGroup.MinimumBlocksPercent = TagIntCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.MinimumBlocksPercent, out badParse);
						
				}
				
				//MaximumBlocksPercent
				if(tag.Contains("[MaximumBlocksPercent") == true){

					improveSpawnGroup.MaximumBlocksPercent = TagIntCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.MaximumBlocksPercent, out badParse);
						
				}
				
				//MinimumBuildPercent
				if(tag.Contains("[MinimumBuildPercent") == true){

					improveSpawnGroup.MinimumBuildPercent = TagIntCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.MinimumBuildPercent, out badParse);
						
				}
				
				//MaximumBuildPercent
				if(tag.Contains("[MaximumBuildPercent") == true){

					improveSpawnGroup.MaximumBuildPercent = TagIntCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.MaximumBuildPercent, out badParse);
						
				}
				
				//UseRivalAi
				if(tag.Contains("[UseRivalAi") == true){

					improveSpawnGroup.UseRivalAi = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}

                //RivalAiReplaceRemoteControl
                if(tag.Contains("[RivalAiReplaceRemoteControl") == true){

					improveSpawnGroup.RivalAiReplaceRemoteControl = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//EraseIngameScripts
				if(tag.Contains("[EraseIngameScripts") == true){

					improveSpawnGroup.EraseIngameScripts = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//DisableTimerBlocks
				if(tag.Contains("[DisableTimerBlocks") == true){

					improveSpawnGroup.DisableTimerBlocks = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//DisableSensorBlocks
				if(tag.Contains("[DisableSensorBlocks") == true){

					improveSpawnGroup.DisableSensorBlocks = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//DisableWarheads
				if(tag.Contains("[DisableWarheads") == true){

					improveSpawnGroup.DisableWarheads = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//DisableThrustOverride
				if(tag.Contains("[DisableThrustOverride") == true){

					improveSpawnGroup.DisableThrustOverride = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//DisableGyroOverride
				if(tag.Contains("[DisableGyroOverride") == true){

					improveSpawnGroup.DisableGyroOverride = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//EraseLCDs
				if(tag.Contains("[EraseLCDs") == true){

					improveSpawnGroup.EraseLCDs = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//UseTextureLCD
				if(tag.Contains("[UseTextureLCD") == true){

					improveSpawnGroup.UseTextureLCD = TagStringListCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//EraseLCDs
				if(tag.Contains("[EraseLCDs") == true){

					improveSpawnGroup.EraseLCDs = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//EnableBlocksWithName
				if(tag.Contains("[EnableBlocksWithName") == true){

					improveSpawnGroup.EnableBlocksWithName = TagStringListCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//DisableBlocksWithName
				if(tag.Contains("[DisableBlocksWithName") == true){

					improveSpawnGroup.DisableBlocksWithName = TagStringListCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//AllowPartialNames
				if(tag.Contains("[AllowPartialNames") == true){

					improveSpawnGroup.AllowPartialNames = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//ChangeTurretSettings
				if(tag.Contains("[ChangeTurretSettings") == true){

					improveSpawnGroup.ChangeTurretSettings = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//TurretRange
				if(tag.Contains("[TurretRange") == true){

					improveSpawnGroup.TurretRange = TagDoubleCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.TurretRange, out badParse);
						
				}
				
				//TurretIdleRotation
				if(tag.Contains("[TurretIdleRotation") == true){

					improveSpawnGroup.TurretIdleRotation = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//TurretTargetMeteors
				if(tag.Contains("[TurretTargetMeteors") == true){

					improveSpawnGroup.TurretTargetMeteors = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//TurretTargetMissiles
				if(tag.Contains("[TurretTargetMissiles") == true){

					improveSpawnGroup.TurretTargetMissiles = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//TurretTargetCharacters
				if(tag.Contains("[TurretTargetCharacters") == true){

					improveSpawnGroup.TurretTargetCharacters = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//TurretTargetSmallGrids
				if(tag.Contains("[TurretTargetSmallGrids") == true){

					improveSpawnGroup.TurretTargetSmallGrids = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//TurretTargetLargeGrids
				if(tag.Contains("[TurretTargetLargeGrids") == true){

					improveSpawnGroup.TurretTargetLargeGrids = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//TurretTargetStations
				if(tag.Contains("[TurretTargetStations") == true){

					improveSpawnGroup.TurretTargetStations = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//TurretTargetNeutrals
				if(tag.Contains("[TurretTargetNeutrals") == true){

					improveSpawnGroup.TurretTargetNeutrals = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//ClearAuthorship
				if(tag.Contains("[ClearAuthorship") == true){

					improveSpawnGroup.ClearAuthorship = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//MinSpawnFromWorldCenter
				if(tag.Contains("[MinSpawnFromWorldCenter") == true){

					improveSpawnGroup.MinSpawnFromWorldCenter = TagDoubleCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.MinSpawnFromWorldCenter, out badParse);
						
				}
				
				//MaxSpawnFromWorldCenter
				if(tag.Contains("[MaxSpawnFromWorldCenter") == true){

					improveSpawnGroup.MaxSpawnFromWorldCenter = TagDoubleCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.MaxSpawnFromWorldCenter, out badParse);
						
				}
				
				//PlanetBlacklist
				if(tag.Contains("[PlanetBlacklist") == true){

					improveSpawnGroup.PlanetBlacklist = TagStringListCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//PlanetWhitelist
				if(tag.Contains("[PlanetWhitelist") == true){

					improveSpawnGroup.PlanetWhitelist = TagStringListCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//PlanetRequiresVacuum
				if(tag.Contains("[PlanetRequiresVacuum") == true){

					improveSpawnGroup.PlanetRequiresVacuum = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//PlanetRequiresAtmo
				if(tag.Contains("[PlanetRequiresAtmo") == true){

					improveSpawnGroup.PlanetRequiresAtmo = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
					setAtmoRequired = true;
						
				}
				
				//PlanetRequiresOxygen
				if(tag.Contains("[PlanetRequiresOxygen") == true){

					improveSpawnGroup.PlanetRequiresOxygen = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//PlanetMinimumSize
				if(tag.Contains("[PlanetMinimumSize") == true){

					improveSpawnGroup.PlanetMinimumSize = TagDoubleCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.PlanetMinimumSize, out badParse);
						
				}
				
				//PlanetMaximumSize
				if(tag.Contains("[PlanetMaximumSize") == true){

					improveSpawnGroup.PlanetMaximumSize = TagDoubleCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.PlanetMaximumSize, out badParse);
						
				}

                //UsePlayerCountCheck
                if(tag.Contains("[UsePlayerCountCheck") == true) {

                    improveSpawnGroup.UsePlayerCountCheck = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //PlayerCountCheckRadius
                if(tag.Contains("[PlayerCountCheckRadius") == true) {

                    improveSpawnGroup.PlayerCountCheckRadius = TagDoubleCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.PlayerCountCheckRadius, out badParse);

                }

                //MinimumPlayers
                if(tag.Contains("[MinimumPlayers") == true) {

                    improveSpawnGroup.MinimumPlayers = TagIntCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.MinimumPlayers, out badParse);

                }

                //MaximumPlayers
                if(tag.Contains("[MaximumPlayers") == true) {

                    improveSpawnGroup.MaximumPlayers = TagIntCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.MaximumPlayers, out badParse);

                }

                //UseThreatLevelCheck
                if(tag.Contains("[UseThreatLevelCheck") == true){

					improveSpawnGroup.UseThreatLevelCheck = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//ThreatLevelCheckRange
				if(tag.Contains("[ThreatLevelCheckRange") == true){

					improveSpawnGroup.ThreatLevelCheckRange = TagDoubleCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.ThreatLevelCheckRange, out badParse);
						
				}
				
				//ThreatIncludeOtherNpcOwners
				if(tag.Contains("[ThreatIncludeOtherNpcOwners") == true){

					improveSpawnGroup.ThreatIncludeOtherNpcOwners = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//ThreatScoreMinimum
				if(tag.Contains("[ThreatScoreMinimum") == true){

					improveSpawnGroup.ThreatScoreMinimum = TagIntCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.ThreatScoreMinimum, out badParse);
						
				}
				
				//ThreatScoreMaximum
				if(tag.Contains("[ThreatScoreMaximum") == true){

					improveSpawnGroup.ThreatScoreMaximum = TagIntCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.ThreatScoreMaximum, out badParse);
				
				}
				
				//UsePCUCheck
				if(tag.Contains("[UsePCUCheck") == true){

					improveSpawnGroup.UsePCUCheck = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//PCUCheckRadius
				if(tag.Contains("[PCUCheckRadius") == true){

					improveSpawnGroup.PCUCheckRadius = TagDoubleCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.PCUCheckRadius, out badParse);
						
				}
				
				//PCUMinimum
				if(tag.Contains("[PCUMinimum") == true){

					improveSpawnGroup.PCUMinimum = TagIntCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.PCUMinimum, out badParse);
						
				}
				
				//PCUMaximum
				if(tag.Contains("[PCUMaximum") == true){

					improveSpawnGroup.PCUMaximum = TagIntCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.PCUMaximum, out badParse);
						
				}
				
				//UsePlayerCredits
				if(tag.Contains("[UsePlayerCredits") == true){

					improveSpawnGroup.UsePlayerCredits = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}

                //IncludeAllPlayersInRadius
                if(tag.Contains("[IncludeAllPlayersInRadius") == true) {

                    improveSpawnGroup.IncludeAllPlayersInRadius = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //IncludeFactionBalance
                if(tag.Contains("[IncludeFactionBalance") == true) {

                    improveSpawnGroup.IncludeFactionBalance = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //PlayerCreditsCheckRadius
                if(tag.Contains("[PlayerCreditsCheckRadius") == true){

					improveSpawnGroup.PlayerCreditsCheckRadius = TagDoubleCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.PlayerCreditsCheckRadius, out badParse);
						
				}
				
				//MinimumPlayerCredits
				if(tag.Contains("[MinimumPlayerCredits") == true){

					improveSpawnGroup.MinimumPlayerCredits = TagIntCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.MinimumPlayerCredits, out badParse);
						
				}
				
				//MaximumPlayerCredits
				if(tag.Contains("[MaximumPlayerCredits") == true){

					improveSpawnGroup.MaximumPlayerCredits = TagIntCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.MaximumPlayerCredits, out badParse);
						
				}
				
				//UsePlayerFactionReputation
				if(tag.Contains("[UsePlayerFactionReputation") == true){

					improveSpawnGroup.UsePlayerFactionReputation = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//PlayerReputationCheckRadius
				if(tag.Contains("[PlayerReputationCheckRadius") == true){

					improveSpawnGroup.PlayerReputationCheckRadius = TagDoubleCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.PlayerReputationCheckRadius, out badParse);
				
				}
				
				//CheckReputationAgainstOtherNPCFaction
				if(tag.Contains("[CheckReputationAgainstOtherNPCFaction") == true){

					improveSpawnGroup.CheckReputationAgainstOtherNPCFaction = TagStringCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
				
				}
				
				//MinimumReputation
				if(tag.Contains("[MinimumReputation") == true){
				
					improveSpawnGroup.MinimumReputation = TagIntCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.MinimumReputation, out badParse);
				
				}
				
				//MaximumReputation
				if(tag.Contains("[MaximumReputation") == true){

					improveSpawnGroup.MaximumReputation = TagIntCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.MaximumReputation, out badParse);
						
				}
				
				//RequireAllMods
				if(tag.Contains("[RequiredMods") == true || tag.Contains("[RequireAllMods") == true){

					improveSpawnGroup.RequireAllMods = TagUlongListCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//ExcludeAnyMods
				if(tag.Contains("[ExcludedMods") == true || tag.Contains("[ExcludeAnyMods") == true){

					improveSpawnGroup.ExcludeAnyMods = TagUlongListCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//RequireAnyMods
				if(tag.Contains("[RequireAnyMods") == true){

					improveSpawnGroup.RequireAnyMods = TagUlongListCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//ExcludeAllMods
				if(tag.Contains("[ExcludeAllMods") == true){

					improveSpawnGroup.ExcludeAllMods = TagUlongListCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//RequiredPlayersOnline
				if(tag.Contains("[RequiredPlayersOnline") == true){

					improveSpawnGroup.RequiredPlayersOnline = TagUlongListCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//AttachModStorageComponentToGrid
				if(tag.Contains("[AttachModStorageComponentToGrid") == true){

					improveSpawnGroup.AttachModStorageComponentToGrid = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//StorageKey
				if(tag.Contains("[StorageKey") == true){

					var storageKey = TagStringCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
					
					try{
						
						improveSpawnGroup.StorageKey = new Guid(storageKey);
						
					}catch(Exception e){
						
						Logger.AddMsg("Spawngroup Tag [StorageKey] Invalid for SpawnGroup" + improveSpawnGroup.SpawnGroupName);
						badParse = true;
						
					}
					
				
				}
				
				//StorageValue
				if(tag.Contains("[StorageValue") == true){

					improveSpawnGroup.StorageValue = TagStringCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
				
				}

                //UseKnownPlayerLocations
                if(tag.Contains("[UseKnownPlayerLocations") == true) {

                    improveSpawnGroup.UseKnownPlayerLocations = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //KnownPlayerLocationMustMatchFaction
                if(tag.Contains("[KnownPlayerLocationMustMatchFaction") == true) {

                    improveSpawnGroup.KnownPlayerLocationMustMatchFaction = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //KnownPlayerLocationMinSpawnedEncounters
                if(tag.Contains("[KnownPlayerLocationMinSpawnedEncounters") == true) {

                    improveSpawnGroup.KnownPlayerLocationMinSpawnedEncounters = TagIntCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.KnownPlayerLocationMinSpawnedEncounters, out badParse);

                }

                //KnownPlayerLocationMaxSpawnedEncounters
                if(tag.Contains("[KnownPlayerLocationMaxSpawnedEncounters") == true) {

                    improveSpawnGroup.KnownPlayerLocationMaxSpawnedEncounters = TagIntCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.KnownPlayerLocationMaxSpawnedEncounters, out badParse);

                }

                //Territory
                if(tag.Contains("[Territory") == true){

					improveSpawnGroup.Territory = TagStringCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//MinDistanceFromTerritoryCenter
				if(tag.Contains("[MinDistanceFromTerritoryCenter") == true){

					improveSpawnGroup.MinDistanceFromTerritoryCenter = TagDoubleCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.MinDistanceFromTerritoryCenter, out badParse);
						
				}
				
				//MaxDistanceFromTerritoryCenter
				if(tag.Contains("[MaxDistanceFromTerritoryCenter") == true){

					improveSpawnGroup.MaxDistanceFromTerritoryCenter = TagDoubleCheck(tag, spawnGroup.Id.SubtypeName, improveSpawnGroup.MaxDistanceFromTerritoryCenter, out badParse);
						
				}
				
				//RotateFirstCockpitToForward
				if(tag.Contains("[RotateFirstCockpitToForward") == true){

					improveSpawnGroup.RotateFirstCockpitToForward = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//PositionAtFirstCockpit
				if(tag.Contains("[PositionAtFirstCockpit") == true){

					improveSpawnGroup.PositionAtFirstCockpit = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//SpawnRandomCargo
				if(tag.Contains("[SpawnRandomCargo") == true){

					improveSpawnGroup.SpawnRandomCargo = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//DisableDampeners
				if(tag.Contains("[DisableDampeners") == true){

					improveSpawnGroup.DisableDampeners = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
					setDampeners = true;
						
				}
				
				//ReactorsOn
				if(tag.Contains("[ReactorsOn") == true){

					improveSpawnGroup.ReactorsOn = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}

                //RemoveVoxelsIfGridRemoved
                if(tag.Contains("[RemoveVoxelsIfGridRemoved") == true) {

                    improveSpawnGroup.RemoveVoxelsIfGridRemoved = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);

                }

                //BossCustomAnnounceEnable
                if(tag.Contains("[BossCustomAnnounceEnable") == true){

					improveSpawnGroup.BossCustomAnnounceEnable = TagBoolCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//BossCustomAnnounceAuthor
				if(tag.Contains("[BossCustomAnnounceAuthor") == true){

					improveSpawnGroup.BossCustomAnnounceAuthor = TagStringCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//BossCustomAnnounceMessage
				if(tag.Contains("[BossCustomAnnounceMessage") == true){

					improveSpawnGroup.BossCustomAnnounceMessage = TagStringCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
				
				//BossCustomGPSLabel
				if(tag.Contains("[BossCustomGPSLabel") == true){

					improveSpawnGroup.BossCustomGPSLabel = TagStringCheck(tag, spawnGroup.Id.SubtypeName, out badParse);
						
				}
								
			}
			
			if(improveSpawnGroup.SpaceCargoShip == true && setDampeners == false){
				
				improveSpawnGroup.DisableDampeners = true;
				
			}
				
			if(improveSpawnGroup.AtmosphericCargoShip == true && setAtmoRequired == false){
				
				improveSpawnGroup.PlanetRequiresAtmo = true;
				
			}
			
			if(improveSpawnGroup.PlanetaryInstallation == true && setForceStatic == false){
				
				improveSpawnGroup.ForceStaticGrid = true;
				
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
		
		public static bool ModRestrictionCheck(ImprovedSpawnGroup spawnGroup){
			
			//Require All
			if(spawnGroup.RequireAllMods.Count > 0){
				
				foreach(var item in spawnGroup.RequireAllMods){
				
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
			
			var thisTag = tag;
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
                Logger.AddMsg("MultipColonSplit - " + b);

            }

			return tagSplit;
			
		}
		
		public static bool TagBoolCheck(string tag, string spawnGroupName, out bool badParse){
			
			bool result = false;
			badParse = false;
			var tagSplit = ProcessTag(tag);
					
			if(tagSplit.Length == 2){
				
				if(bool.TryParse(tagSplit[1], out result) == false){
					
					Logger.AddMsg("Could not process Bool tag " + tag + " from SpawnGroup " + spawnGroupName);
					badParse = true;
					
				}
				
			}else{
				
				Logger.AddMsg("Could not process Bool tag " + tag + " from SpawnGroup " + spawnGroupName);
				badParse = true;
				
			}
			
			return result;
			
		}
		
		public static double TagDoubleCheck(string tag, string spawnGroupName, double defaultValue, out bool badParse){
			
			double result = defaultValue;
			badParse = false;
			var tagSplit = ProcessTag(tag);
					
			if(tagSplit.Length == 2){
				
				if(double.TryParse(tagSplit[1], out result) == false){
					
					Logger.AddMsg("Could not process Double tag " + tag + " from SpawnGroup " + spawnGroupName);
					badParse = true;
					
				}
				
			}else{
				
				Logger.AddMsg("Could not process Double tag " + tag + " from SpawnGroup " + spawnGroupName);
				badParse = true;
				
			}
			
			return result;
			
		}
		
		public static int TagIntCheck(string tag, string spawnGroupName, int defaultValue, out bool badParse){
			
			int result = defaultValue;
			badParse = false;
			var tagSplit = ProcessTag(tag);
					
			if(tagSplit.Length == 2){
				
				if(int.TryParse(tagSplit[1], out result) == false){
					
					Logger.AddMsg("Could not process Int tag " + tag + " from SpawnGroup " + spawnGroupName);
					badParse = true;
					
				}
				
			}else{
				
				Logger.AddMsg("Could not process Int tag " + tag + " from SpawnGroup " + spawnGroupName);
				badParse = true;
				
			}
			
			return result;
			
		}
		
		public static MyDefinitionId TagMyDefIdCheck(string tag, string spawnGroupName, out bool badParse){
			
			MyDefinitionId result = new MyDefinitionId();
			badParse = false;
			var tagSplit = ProcessTag(tag);
			
			if(tagSplit.Length == 2){
				
				if(MyDefinitionId.TryParse(tagSplit[1], out result) == false){
					
					Logger.AddMsg("Could not parse MyDefinitionId tag " + tag + " from SpawnGroup " + spawnGroupName);
					badParse = true;
					
				}
				
			}else{
				
				Logger.AddMsg("Could not process MyDefinitionId tag " + tag + " from SpawnGroup " + spawnGroupName + ". Array Length Is: " + tagSplit.Length.ToString());
				badParse = true;
				
			}
			
			return result;
			
		}
		
		public static string TagStringCheck(string tag, string spawnGroupName, out bool badParse){
			
			string result = "";
			badParse = false;
			var tagSplit = ProcessTag(tag);
			
			if(tagSplit.Length == 2){
				
				result = tagSplit[1];
				
			}else{
				
				Logger.AddMsg("Could not process String tag " + tag + " from SpawnGroup " + spawnGroupName + ". Array Length Is: " + tagSplit.Length.ToString());
				badParse = true;
				
			}
			
			return result;
			
		}
		
		public static List<string> TagStringListCheck(string tag, string spawnGroupName, out bool badParse){
			
			List<string> result = new List<string>();
			badParse = false;
			var tagSplit = ProcessTag(tag);
			
			if(tagSplit.Length == 2){
				
				var array = tagSplit[1].Split(',');
				
				foreach(var item in array){
					
					if(item == "" || item == " " || item == null){
						
						continue;
						
					}
					
					result.Add(item);
					
				}

			}else{
				
				Logger.AddMsg("Could not process String List tag " + tag + " from SpawnGroup " + spawnGroupName);
				badParse = true;
				
			}
			
			return result;
			
		}
		
		public static Dictionary<MyDefinitionId, MyDefinitionId> TagMDIDictionaryCheck(string tag, string spawnGroupName, out bool badParse){
			
			Dictionary<MyDefinitionId, MyDefinitionId> result = new Dictionary<MyDefinitionId, MyDefinitionId>();
			badParse = false;
			var tagSplit = ProcessTag(tag);
			
			if(tagSplit.Length == 2){
				
				var array = tagSplit[1].Split(',');
				
				foreach(var item in array){
					
					if(item == "" || item == " " || item == null){
						
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

			}else{
				
				Logger.AddMsg("Could not process MyDefinitionId Dictionary tag " + tag + " from SpawnGroup " + spawnGroupName);
				badParse = true;
				
			}
			
			return result;
			
		}

        public static Dictionary<string, string> TagStringDictionaryCheck(string tag, string spawnGroupName, out bool badParse) {

            Dictionary<string, string> result = new Dictionary<string, string>();
            badParse = false;
            var tagSplit = ProcessTag(tag);

            if(tagSplit.Length == 2) {

                var array = tagSplit[1].Split(',');

                foreach(var item in array) {

                    if(string.IsNullOrWhiteSpace(item) == true) {

                        continue;

                    }

                    var secondSplit = item.Split('|');

                    string key = secondSplit[0];
                    string val = secondSplit[1];

                    if(string.IsNullOrWhiteSpace(key) == false && string.IsNullOrWhiteSpace(val) == false && result.ContainsKey(val) == false) {

                        result.Add(key, val);

                    }

                }

            } else {

                Logger.AddMsg("Could not process MyDefinitionId Dictionary tag " + tag + " from SpawnGroup " + spawnGroupName);
                badParse = true;

            }

            return result;

        }

        public static Dictionary<Vector3, Vector3> TagVector3DictionaryCheck(string tag, string spawnGroupName, out bool badParse) {

            Dictionary<Vector3, Vector3> result = new Dictionary<Vector3, Vector3>();
            badParse = false;
            var tagSplit = ProcessTag(tag);

            if(tagSplit.Length == 2) {

                var array = tagSplit[1].Split(',');

                foreach(var item in array) {

                    if(string.IsNullOrWhiteSpace(item) == true) {

                        continue;

                    }

                    var secondSplit = item.Split('|');

                    string key = secondSplit[0];
                    string val = secondSplit[1];

                    if(string.IsNullOrWhiteSpace(key) == true || string.IsNullOrWhiteSpace(val) == true) {

                        continue;

                    }

                    Vector3D keyVector = Vector3D.Zero;
                    Vector3D valVector = Vector3D.Zero;

                    if(Vector3D.TryParse(key, out keyVector) == false || Vector3D.TryParse(val, out valVector) == false) {

                        continue;

                    }

                    result.Add(keyVector, valVector);

                }

            } else {

                Logger.AddMsg("Could not process Vector3-Vector3 Dictionary tag " + tag + " from SpawnGroup " + spawnGroupName);
                badParse = true;

            }

            return result;

        }

        public static Vector3D TagVector3DCheck(string tag, string spawnGroupName, out bool badParse) {

            Vector3D result = Vector3D.Zero;
            badParse = false;
            var tagSplit = ProcessTag(tag);

            if(tagSplit.Length == 2) {

                if(Vector3D.TryParse(tagSplit[1], out result) == false) {

                    Logger.AddMsg("Could not process Vector3D tag " + tag + " from SpawnGroup " + spawnGroupName);
                    badParse = true;

                }

            } else {

                Logger.AddMsg("Could not process Vector3D tag " + tag + " from SpawnGroup " + spawnGroupName);
                badParse = true;

            }

            return result;

        }

        public static List<bool> TagBoolListCheck(string tag, string spawnGroupName, out bool badParse) {

            List<bool> result = new List<bool>();
            badParse = false;
            var tagSplit = ProcessTag(tag);

            if(tagSplit.Length == 2) {

                var array = tagSplit[1].Split(',');

                foreach(var item in array) {

                    if(string.IsNullOrWhiteSpace(item) == true) {

                        continue;

                    }

                    bool entry = false;

                    if(bool.TryParse(item, out entry) == false) {

                        continue;

                    }

                    result.Add(entry);

                }

            } else {

                Logger.AddMsg("Could not process bool List tag " + tag + " from SpawnGroup " + spawnGroupName);
                badParse = true;

            }

            return result;

        }

        public static List<Vector3D> TagVector3DListCheck(string tag, string spawnGroupName, out bool badParse) {

            List<Vector3D> result = new List<Vector3D>();
            badParse = false;
            var tagSplit = ProcessTag(tag);

            if(tagSplit.Length == 2) {

                var array = tagSplit[1].Split(',');

                foreach(var item in array) {

                    if(string.IsNullOrWhiteSpace(item) == true) {

                        continue;

                    }

                    Vector3D entry = Vector3D.Zero;

                    if(Vector3D.TryParse(item, out entry) == false) {

                        continue;

                    }

                    result.Add(entry);

                }

            } else {

                Logger.AddMsg("Could not process Vector3D List tag " + tag + " from SpawnGroup " + spawnGroupName);
                badParse = true;

            }

            return result;

        }

        public static Dictionary<Vector3, string> TagVector3StringDictionaryCheck(string tag, string spawnGroupName, out bool badParse) {

            Dictionary<Vector3, string> result = new Dictionary<Vector3, string>();
            badParse = false;
            var tagSplit = ProcessTag(tag);

            if(tagSplit.Length == 2) {

                var array = tagSplit[1].Split(',');

                foreach(var item in array) {

                    if(string.IsNullOrWhiteSpace(item) == true) {

                        continue;

                    }

                    var secondSplit = item.Split('|');

                    string key = secondSplit[0];
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

            } else {

                Logger.AddMsg("Could not process Vector3-Vector3 Dictionary tag " + tag + " from SpawnGroup " + spawnGroupName);
                badParse = true;

            }

            return result;

        }

        public static List<ulong> TagUlongListCheck(string tag, string spawnGroupName, out bool badParse){
			
			List<ulong> result = new List<ulong>();
			badParse = false;
			var tagSplit = ProcessTag(tag);
			
			if(tagSplit.Length == 2){
				
				var array = tagSplit[1].Split(',');
				
				foreach(var item in array){
					
					if(item == "" || item == " " || item == null){
						
						continue;
						
					}
					
					ulong modId = 0;
					
					if(ulong.TryParse(item, out modId) == false){
						
						Logger.AddMsg("Could not parse ulong List item " + item + " from SpawnGroup " + spawnGroupName);
						badParse = true;
						
					}
					
					result.Add(modId);
					
				}

			}else{
				
				Logger.AddMsg("Could not process ulong List tag " + tag + " from SpawnGroup " + spawnGroupName);
				badParse = true;
				
			}
			result.RemoveAll(item => item == 0);
			return result;
			
		}
		
	}
	
}