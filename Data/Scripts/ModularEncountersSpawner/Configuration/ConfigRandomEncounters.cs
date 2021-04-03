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

namespace ModularEncountersSpawner.Configuration{

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

	public class ConfigRandomEncounters : ConfigBase {
		
		public float ModVersion;
		public int PlayerSpawnCooldown;
		public int SpawnTimerTrigger;
		public double PlayerTravelDistance;
		public int MaxShipsPerArea;
		public double AreaSize;
		public double MinSpawnDistanceFromPlayer;
		public double MaxSpawnDistanceFromPlayer;
		public double MinDistanceFromOtherEntities;
		public bool RemoveVoxelsIfGridRemoved;
		public int SpawnAttempts;
		
		public bool UseMaxSpawnGroupFrequency;
		public int MaxSpawnGroupFrequency;
		
		public double DespawnDistanceFromPlayer;

		public ConfigRandomEncounters(){
			
			ModVersion = MES_SessionCore.ModVersion;
			PlayerSpawnCooldown = 300;
			SpawnTimerTrigger = 60;
			PlayerTravelDistance = 15000;
			MaxShipsPerArea = 10;
			AreaSize = 25000;
			MinSpawnDistanceFromPlayer = 8000;
			MaxSpawnDistanceFromPlayer = 12000;
			MinDistanceFromOtherEntities = 3000;
			RemoveVoxelsIfGridRemoved = true;
			SpawnAttempts = 10;
			
			UseMaxSpawnGroupFrequency = false;
			MaxSpawnGroupFrequency = 5;
			
			DespawnDistanceFromPlayer = 1000;

			UseTimeout = true;
			TimeoutDuration = 900;
			TimeoutRadius = 25000;
			TimeoutSpawnLimit = 2;

			UseCleanupSettings = true;
			CleanupUseDistance = true;
			CleanupUseTimer = true;
			CleanupUseBlockLimit = false;
			CleanupDistanceStartsTimer = true;
			CleanupResetTimerWithinDistance = false;
			CleanupDistanceTrigger = 50000;
			CleanupTimerTrigger = 1800;
			CleanupBlockLimitTrigger = 0;
			CleanupIncludeUnowned = true;
			CleanupUnpoweredOverride = false;
			CleanupUnpoweredDistanceTrigger = 25000;
			CleanupUnpoweredTimerTrigger = 900;
			
		}
		
		public ConfigRandomEncounters LoadSettings(){
			
			if(MyAPIGateway.Utilities.FileExistsInWorldStorage("Config-RandomEncounters.xml", typeof(ConfigRandomEncounters)) == true){
				
				try{
					
					ConfigRandomEncounters config = null;
					var reader = MyAPIGateway.Utilities.ReadFileInWorldStorage("Config-RandomEncounters.xml", typeof(ConfigRandomEncounters));
					string configcontents = reader.ReadToEnd();
					config = MyAPIGateway.Utilities.SerializeFromXML<ConfigRandomEncounters>(configcontents);
					Logger.AddMsg("Loaded Existing Settings From Config-RandomEncounters.xml");
					return config;
					
				}catch(Exception exc){
					
					Logger.AddMsg("ERROR: Could Not Load Settings From Config-RandomEncounters.xml. Using Default Configuration.");
					var defaultSettings = new ConfigRandomEncounters();
					return defaultSettings;
					
				}
				
			}
			
			var settings = new ConfigRandomEncounters();
			
			try{
				
				using (var writer = MyAPIGateway.Utilities.WriteFileInWorldStorage("Config-RandomEncounters.xml", typeof(ConfigRandomEncounters))){
				
					writer.Write(MyAPIGateway.Utilities.SerializeToXML<ConfigRandomEncounters>(settings));
				
				}
				
			}catch(Exception exc){
				
				Logger.AddMsg("ERROR: Could Not Create Config-RandomEncounters.xml. Default Settings Will Be Used.");
				
			}
			
			return settings;
			
		}
		
		public string SaveSettings(ConfigRandomEncounters settings){
			
			try{
				
				using (var writer = MyAPIGateway.Utilities.WriteFileInWorldStorage("Config-RandomEncounters.xml", typeof(ConfigRandomEncounters))){
					
					writer.Write(MyAPIGateway.Utilities.SerializeToXML<ConfigRandomEncounters>(settings));
				
				}
				
				Logger.AddMsg("Settings In Config-RandomEncounters.xml Updated Successfully!");
				return "Settings Updated Successfully.";
				
			}catch(Exception exc){
				
				Logger.AddMsg("ERROR: Could Not Save To Config-RandomEncounters.xml. Changes Will Be Lost On World Reload.");
				
			}
			
			return "Settings Changed, But Could Not Be Saved To XML. Changes May Be Lost On Session Reload.";
			
		}
		
	}
		
}