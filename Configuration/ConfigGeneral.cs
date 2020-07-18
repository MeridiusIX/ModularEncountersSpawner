using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
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

namespace ModularEncountersSpawner.Configuration{

	//General

	/*
	  
	  Hello Stranger!
	 
	  If you are in here because you want to change settings
	  for how this mod behaves, you are in the wrong place.

	  All the settings in this file, along with the other
	  configuration files, are created as XML files in the
	  \Storage\1521905890.sbm_ModularEncountersSpawner folder
	  of your Save File. This means you do not need to edit
	  the mod files here to tune the settings to your liking.

	  The workshop page for this mod also has a link to a
	  guide that explains what all the configuration options
	  do, along with how to activate them in-game via chat
	  commands if desired.
	  
	  If you plan to edit the values here anyway, I ask that
	  you do not reupload this mod to the Steam Workshop. If
	  this is not respected and I find out about it, I'll
	  exercise my rights as the creator and file a DMCA
	  takedown on any infringing copies. This warning can be
	  found on the workshop page for this mod as well.

	  Thank you.
		 
	*/

	public class ConfigGeneral{
		
		public float ModVersion {get; set;}
		public bool DebugMode { get; set; }
		public bool EnableSpaceCargoShips {get; set;}
		public bool EnablePlanetaryCargoShips {get; set;}
		public bool EnableRandomEncounters {get; set;}
		public bool EnablePlanetaryInstallations {get; set;}
		public bool EnableBossEncounters {get; set;}
		
		public bool EnableGlobalNPCWeaponRandomizer {get; set;}
		
		//[XmlIgnoreAttribute]
		public bool EnableGlobalNPCShieldProvider { get; set; }
		
		public bool EnableLegacySpaceCargoShipDetection {get; set;}
		
		public bool UseModIdSelectionForSpawning {get; set;}
		public bool UseWeightedModIdSelection {get; set;}
		public int LowWeightModIdSpawnGroups {get; set;}
		public int LowWeightModIdModifier {get; set;}
		public int MediumWeightModIdSpawnGroups {get; set;}
		public int MediumWeightModIdModifier {get; set;}
		public int HighWeightModIdSpawnGroups {get; set;}
		public int HighWeightModIdModifier {get; set;}
		
		public bool UseMaxNpcGrids {get; set;}
		public bool UseGlobalEventsTimers {get; set;}
		
		public bool IgnorePlanetWhitelists {get; set;}
		public bool IgnorePlanetBlacklists {get; set;}
		
		public int ThreatRefreshTimerMinimum {get; set;}
		public int ThreatReductionHandicap {get; set;}
		
		public int MaxGlobalNpcGrids {get; set;}
		public int PlayerWatcherTimerTrigger {get; set;}
		public int NpcDistanceCheckTimerTrigger {get; set;}
		public int NpcOwnershipCheckTimerTrigger {get; set;}
		public int NpcCleanupCheckTimerTrigger {get; set;}
		public int NpcBlacklistCheckTimerTrigger {get; set;}
		public int SpawnedVoxelCheckTimerTrigger {get; set;}
		public double SpawnedVoxelMinimumGridDistance {get; set;}
		public string[] PlanetSpawnsDisableList {get; set;}
		public string[] NpcGridNameBlacklist {get; set;}
		public string[] NpcSpawnGroupBlacklist {get; set;}
		
		public string[] WeaponReplacerBlacklist {get; set;}
		public string[] WeaponReplacerWhitelist {get; set;}
		public string[] WeaponReplacerTargetBlacklist {get; set;}
		public string[] WeaponReplacerTargetWhitelist {get; set;}

        public bool WeaponReplacerUseMaxRangeOverride {get; set;}
        public float WeaponReplacerMaxRangeOverride {get; set;}
        public bool UseGlobalBlockReplacer {get; set;}
		public string[] GlobalBlockReplacerReference {get; set;}
		public string[] GlobalBlockReplacerProfiles {get; set;}
		
		public bool UseNonPhysicalAmmoForNPCs { get; set;} 

		public bool RemoveContainerInventoryFromNPCs { get; set; }

		public bool UseEconomyBuyingReputationIncrease { get; set; }
		public long EconomyBuyingReputationCostAmount { get; set; }

		public ConfigGeneral(){
			
			ModVersion = MES_SessionCore.ModVersion;
			DebugMode = false;
			EnableSpaceCargoShips = true;
			EnablePlanetaryCargoShips = true;
			EnableRandomEncounters = true;
			EnablePlanetaryInstallations = true;
			EnableBossEncounters = true;
			EnableGlobalNPCWeaponRandomizer = false;
			EnableGlobalNPCShieldProvider = false;
			EnableLegacySpaceCargoShipDetection = true;
			UseModIdSelectionForSpawning = true;
			UseWeightedModIdSelection = true;
			LowWeightModIdSpawnGroups = 10;
			LowWeightModIdModifier = 1;
			MediumWeightModIdSpawnGroups = 19;
			MediumWeightModIdModifier = 2;
			HighWeightModIdSpawnGroups = 20;
			HighWeightModIdModifier = 3;
			UseMaxNpcGrids = false;
			UseGlobalEventsTimers = true;
			IgnorePlanetWhitelists = false;
			IgnorePlanetBlacklists = false;
			ThreatRefreshTimerMinimum = 20;
			ThreatReductionHandicap = 0;
			MaxGlobalNpcGrids = 50;
			PlayerWatcherTimerTrigger = 10;
			NpcDistanceCheckTimerTrigger = 1;
			NpcOwnershipCheckTimerTrigger = 10;
			NpcCleanupCheckTimerTrigger = 60;
			NpcBlacklistCheckTimerTrigger = 5;
			SpawnedVoxelCheckTimerTrigger = 900;
			SpawnedVoxelMinimumGridDistance = 1000;
			PlanetSpawnsDisableList = new string[]{"Planet_SubtypeId_Here", "Planet_SubtypeId_Here"};
			NpcGridNameBlacklist = new string[]{"BlackList_Grid_Name_Here", "BlackList_Grid_Name_Here"};
			NpcSpawnGroupBlacklist = new string[]{"BlackList_SpawnGroup_Here", "BlackList_SpawnGroup_Here"};
			WeaponReplacerBlacklist = new string[]{"1380830774", "Large_SC_LaserDrill_HiddenStatic", "Large_SC_LaserDrill_HiddenTurret", "Large_SC_LaserDrill", "Large_SC_LaserDrillTurret", "Spotlight_Turret_Large", "Spotlight_Turret_Light_Large", "Spotlight_Turret_Small", "SmallSpotlight_Turret_Small", "ShieldChargerBase_Large", "LDualPulseLaserBase_Large", "AegisLargeBeamBase_Large", "AegisMediumeamBase_Large", "XLGigaBeamGTFBase_Large", "XLDualPulseLaserBase_Large", "1817300677"};
			WeaponReplacerWhitelist = new string[]{};
			WeaponReplacerTargetBlacklist = new string[]{};
			WeaponReplacerTargetWhitelist = new string[]{};
			WeaponReplacerUseMaxRangeOverride = false;
			WeaponReplacerMaxRangeOverride = 800;
			UseGlobalBlockReplacer = false;
			GlobalBlockReplacerReference = new string[]{};
			GlobalBlockReplacerProfiles = new string[]{};
			UseNonPhysicalAmmoForNPCs = false;
			RemoveContainerInventoryFromNPCs = false;
			UseEconomyBuyingReputationIncrease = true;
			EconomyBuyingReputationCostAmount = 500000;

		}
		
		public ConfigGeneral LoadSettings(){
			
			if(MyAPIGateway.Utilities.FileExistsInWorldStorage("Config-General.xml", typeof(ConfigGeneral)) == true){
				
				try{
					
					ConfigGeneral config = null;
					var reader = MyAPIGateway.Utilities.ReadFileInWorldStorage("Config-General.xml", typeof(ConfigGeneral));
					string configcontents = reader.ReadToEnd();
					config = MyAPIGateway.Utilities.SerializeFromXML<ConfigGeneral>(configcontents);
					Logger.AddMsg("Loaded Existing Settings From Config-General.xml");
					return config;
					
				}catch(Exception exc){
					
					Logger.AddMsg("ERROR: Could Not Load Settings From Config-General.xml. Using Default Configuration.");
					var defaultSettings = new ConfigGeneral();
					return defaultSettings;
					
				}
				
			}
			
			var settings = new ConfigGeneral();
			
			try{
				
				using (var writer = MyAPIGateway.Utilities.WriteFileInWorldStorage("Config-General.xml", typeof(ConfigGeneral))){
				
					writer.Write(MyAPIGateway.Utilities.SerializeToXML<ConfigGeneral>(settings));
				
				}
				
			}catch(Exception exc){
				
				Logger.AddMsg("ERROR: Could Not Create Config-General.xml. Default Settings Will Be Used.");
				
			}
			
			return settings;
			
		}
		
		public string SaveSettings(ConfigGeneral settings){
			
			try{
				
				using (var writer = MyAPIGateway.Utilities.WriteFileInWorldStorage("Config-General.xml", typeof(ConfigGeneral))){
					
					writer.Write(MyAPIGateway.Utilities.SerializeToXML<ConfigGeneral>(settings));
				
				}
				
				Logger.AddMsg("Settings In Config-General.xml Updated Successfully!");
				return "Settings Updated Successfully.";
				
			}catch(Exception exc){
				
				Logger.AddMsg("ERROR: Could Not Save To Config-General.xml. Changes Will Be Lost On World Reload.");
				
			}
			
			return "Settings Changed, But Could Not Be Saved To XML. Changes May Be Lost On Session Reload.";
			
		}

		public Dictionary<MyDefinitionId, MyDefinitionId> GetReplacementReferencePairs() {

			var result = new Dictionary<MyDefinitionId, MyDefinitionId>();

			if(this.GlobalBlockReplacerReference.Length == 0) {

				Logger.AddMsg("Global Block Replacement References 0", true);
				return result;

			}

			foreach(var pair in this.GlobalBlockReplacerReference) {

				var split = pair.Split('|');

				if(split.Length != 2) {

					Logger.AddMsg("Global Replace Bad Split: " + pair, true);
					continue;

				}

				var idA = new MyDefinitionId();
				var idB = new MyDefinitionId();

				if(MyDefinitionId.TryParse(split[0], out idA) == false) {

					Logger.AddMsg("Could Not Parse: " + split[0], true);
					continue;

				}

				if(MyDefinitionId.TryParse(split[1], out idB) == false) {

					Logger.AddMsg("Could Not Parse: " + split[1], true);
					continue;

				}

				if(result.ContainsKey(idA) == true) {

					Logger.AddMsg("MyDefinitionId already present: " + split[0], true);
					continue;

				}

				result.Add(idA, idB);

			}

			return result;

		}
		
	}
	
}