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

	public class ConfigGrids{
		
		public float ModVersion {get; set;}

		public bool EnableGlobalNPCWeaponRandomizer;
		public bool EnableGlobalNPCShieldProvider;

		public string[] WeaponReplacerBlacklist;
		public string[] WeaponReplacerWhitelist;
		public string[] WeaponReplacerTargetBlacklist;
		public string[] WeaponReplacerTargetWhitelist;
		public int RandomWeaponChance;
		public int RandomWeaponSizeVariance;

		public bool UseGlobalBlockReplacer;
		public string[] GlobalBlockReplacerReference;
		public string[] GlobalBlockReplacerProfiles;

		public bool UseNonPhysicalAmmoForNPCs;
		public bool RemoveContainerInventoryFromNPCs;

		public int ReplenishedAmmoMaxAmount;
		public int ReplenishedFuelMaxAmount;

		public ConfigGrids(){
			
			ModVersion = MES_SessionCore.ModVersion;

			EnableGlobalNPCWeaponRandomizer = false;
			EnableGlobalNPCShieldProvider = false;

			WeaponReplacerBlacklist = new string[]{"1380830774", "Large_SC_LaserDrill_HiddenStatic", "Large_SC_LaserDrill_HiddenTurret", "Large_SC_LaserDrill", "Large_SC_LaserDrillTurret", "Spotlight_Turret_Large", "Spotlight_Turret_Light_Large", "Spotlight_Turret_Small", "SmallSpotlight_Turret_Small", "ShieldChargerBase_Large", "LDualPulseLaserBase_Large", "AegisLargeBeamBase_Large", "AegisMediumeamBase_Large", "XLGigaBeamGTFBase_Large", "XLDualPulseLaserBase_Large", "1817300677"};
			WeaponReplacerWhitelist = new string[]{};
			WeaponReplacerTargetBlacklist = new string[]{};
			WeaponReplacerTargetWhitelist = new string[]{};
			RandomWeaponChance = 100;
			RandomWeaponSizeVariance = -1;

			UseGlobalBlockReplacer = false;
			GlobalBlockReplacerReference = new string[]{};
			GlobalBlockReplacerProfiles = new string[]{};

			UseNonPhysicalAmmoForNPCs = false;
			RemoveContainerInventoryFromNPCs = false;

			ReplenishedAmmoMaxAmount = 30;
			ReplenishedFuelMaxAmount = 100;

		}
		
		public ConfigGrids LoadSettings(){
			
			if(MyAPIGateway.Utilities.FileExistsInWorldStorage("Config-Grids.xml", typeof(ConfigGrids)) == true){
				
				try{
					
					ConfigGrids config = null;
					var reader = MyAPIGateway.Utilities.ReadFileInWorldStorage("Config-Grids.xml", typeof(ConfigGrids));
					string configcontents = reader.ReadToEnd();
					config = MyAPIGateway.Utilities.SerializeFromXML<ConfigGrids>(configcontents);
					Logger.AddMsg("Loaded Existing Settings From Config-Grids.xml");
					return config;
					
				}catch(Exception exc){
					
					Logger.AddMsg("ERROR: Could Not Load Settings From Config-Grids.xml. Using Default Configuration.");
					var defaultSettings = new ConfigGrids();
					return defaultSettings;
					
				}
				
			}
			
			var settings = new ConfigGrids();
			
			try{
				
				using (var writer = MyAPIGateway.Utilities.WriteFileInWorldStorage("Config-Grids.xml", typeof(ConfigGrids))){
				
					writer.Write(MyAPIGateway.Utilities.SerializeToXML<ConfigGrids>(settings));
				
				}
				
			}catch(Exception exc){
				
				Logger.AddMsg("ERROR: Could Not Create Config-Grids.xml. Default Settings Will Be Used.");
				
			}
			
			return settings;
			
		}
		
		public string SaveSettings(ConfigGrids settings){
			
			try{
				
				using (var writer = MyAPIGateway.Utilities.WriteFileInWorldStorage("Config-Grids.xml", typeof(ConfigGrids))){
					
					writer.Write(MyAPIGateway.Utilities.SerializeToXML<ConfigGrids>(settings));
				
				}
				
				Logger.AddMsg("Settings In Config-Grids.xml Updated Successfully!");
				return "Settings Updated Successfully.";
				
			}catch(Exception exc){
				
				Logger.AddMsg("ERROR: Could Not Save To Config-Grids.xml. Changes Will Be Lost On World Reload.");
				
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