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

	//BossEncounters

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

	public class ConfigBossEncounters : ConfigBase {
		
		public int PlayerSpawnCooldown;
		public int SpawnTimerTrigger;
		public int SignalActiveTimer;
		public int MaxShipsPerArea;
		public double AreaSize;
		public double TriggerDistance;
		public int PathCalculationAttempts;
		public double MinCoordsDistanceSpace;
		public double MaxCoordsDistanceSpace;
		public double MinCoordsDistancePlanet;
		public double MaxCoordsDistancePlanet;
		public double PlayersWithinDistance;
		public double MinPlanetAltitude;
		public double MinSignalDistFromOtherEntities;
		public double MinSpawnDistFromCoords;
		public double MaxSpawnDistFromCoords;
		public float MinAirDensity;
		
		public bool UseMaxSpawnGroupFrequency;
		public int MaxSpawnGroupFrequency;
		
		public double DespawnDistanceFromPlayer;

		
		
		public ConfigBossEncounters(){
			
			PlayerSpawnCooldown = 600;
			SpawnTimerTrigger = 1200;
			SignalActiveTimer = 1200;
			MaxShipsPerArea = 6;
			AreaSize = 25000;
			TriggerDistance = 300;
			PathCalculationAttempts = 25;
			MinCoordsDistanceSpace = 6000;
			MaxCoordsDistanceSpace = 8000;
			MinCoordsDistancePlanet = 6000;
			MaxCoordsDistancePlanet = 8000;
			PlayersWithinDistance = 25000;
			MinPlanetAltitude = 1000;
			MinSignalDistFromOtherEntities = 2000;
			MinSpawnDistFromCoords = 2500;
			MaxSpawnDistFromCoords = 4000;
			MinAirDensity = 0.65f;
			
			UseMaxSpawnGroupFrequency = false;
			MaxSpawnGroupFrequency = 5;
			
			DespawnDistanceFromPlayer = 1000;

			UseTimeout = false;
			TimeoutDuration = 900;
			TimeoutRadius = 10000;
			TimeoutSpawnLimit = 4;

			UseCleanupSettings = true;
			CleanupUseDistance = true;
			CleanupUseTimer = false;
			CleanupUseBlockLimit = false;
			CleanupDistanceStartsTimer = false;
			CleanupResetTimerWithinDistance = false;
			CleanupDistanceTrigger = 50000;
			CleanupTimerTrigger = 1800;
			CleanupBlockLimitTrigger = 0;
			CleanupIncludeUnowned = true;
			CleanupUnpoweredOverride = true;
			CleanupUnpoweredDistanceTrigger = 25000;
			CleanupUnpoweredTimerTrigger = 900;
			
		}
		
		public ConfigBossEncounters LoadSettings(){
			
			if(MyAPIGateway.Utilities.FileExistsInWorldStorage("Config-BossEncounters.xml", typeof(ConfigBossEncounters)) == true){
				
				try{
					
					ConfigBossEncounters config = null;
					var reader = MyAPIGateway.Utilities.ReadFileInWorldStorage("Config-BossEncounters.xml", typeof(ConfigBossEncounters));
					string configcontents = reader.ReadToEnd();
					config = MyAPIGateway.Utilities.SerializeFromXML<ConfigBossEncounters>(configcontents);
					Logger.AddMsg("Loaded Existing Settings From Config-BossEncounters.xml");
					return config;
					
				}catch(Exception exc){
					
					Logger.AddMsg("ERROR: Could Not Load Settings From Config-BossEncounters.xml. Using Default Configuration.");
					var defaultSettings = new ConfigBossEncounters();
					return defaultSettings;
					
				}
				
			}
			
			var settings = new ConfigBossEncounters();
			
			try{
				
				using (var writer = MyAPIGateway.Utilities.WriteFileInWorldStorage("Config-BossEncounters.xml", typeof(ConfigBossEncounters))){
				
					writer.Write(MyAPIGateway.Utilities.SerializeToXML<ConfigBossEncounters>(settings));
				
				}
				
			}catch(Exception exc){
				
				Logger.AddMsg("ERROR: Could Not Create Config-BossEncounters.xml. Default Settings Will Be Used.");
				
			}
			
			return settings;
			
		}
		
		public string SaveSettings(ConfigBossEncounters settings){
			
			try{
				
				using (var writer = MyAPIGateway.Utilities.WriteFileInWorldStorage("Config-BossEncounters.xml", typeof(ConfigBossEncounters))){
					
					writer.Write(MyAPIGateway.Utilities.SerializeToXML<ConfigBossEncounters>(settings));
				
				}
				
				Logger.AddMsg("Settings In Config-BossEncounters.xml Updated Successfully!");
				return "Settings Updated Successfully.";
				
			}catch(Exception exc){
				
				Logger.AddMsg("ERROR: Could Not Save To Config-BossEncounters.xml. Changes Will Be Lost On World Reload.");
				
			}
			
			return "Settings Changed, But Could Not Be Saved To XML. Changes May Be Lost On Session Reload.";
			
		}
		
	}
		
}