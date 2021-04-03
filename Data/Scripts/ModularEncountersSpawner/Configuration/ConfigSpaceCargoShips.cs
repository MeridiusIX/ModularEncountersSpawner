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

	//SpaceCargoShips

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

	public class ConfigSpaceCargoShips : ConfigBase {
		
		public float ModVersion;
		public int FirstSpawnTime; //Time Until Spawn When World Starts
		public int MinSpawnTime; //Min Time Until Next Spawn
		public int MaxSpawnTime; //Max Time Until Next Spawn
		public int MaxShipsPerArea;
		public double AreaSize;
		public int MaxSpawnAttempts; //Number Of Attempts To Spawn Ship(s)
		public double MinPathDistanceFromPlayer;
		public double MaxPathDistanceFromPlayer;
		public double MinLunarSpawnHeight;
		public double MaxLunarSpawnHeight;
		public double MinSpawnDistFromEntities;
		public double MinPathDistance; //Minimum Path Distance Of Cargo Ship
		public double MaxPathDistance; //Maximum Path Distance Of Cargo Ship
		public double PathCheckStep;
		public double DespawnDistanceFromEndPath; // Ship Will Despawn If Within This Distance Of Path End Coordinates
		public double DespawnDistanceFromPlayer;
		public bool UseMinimumSpeed;
		public float MinimumSpeed;
		public bool UseSpeedOverride; //If True, The Cargo Ship Will Use Override Speed Instead Of Prefab Speed
		public float SpeedOverride; //Override Speed Value For Cargo Ship (If Used)
		
		public bool UseMaxSpawnGroupFrequency;
		public int MaxSpawnGroupFrequency;

		public bool EnableWaveSpawner;
		public string[] UseSpecificRandomGroups;
		public int MinWaveSpawnTime;
		public int MaxWaveSpawnTime;
		public int TotalSpawnEventsPerCluster;
		public int TimeBetweenWaveSpawns;
		public double PlayerClusterDistance;
		
		public ConfigSpaceCargoShips(){
			
			ModVersion = MES_SessionCore.ModVersion;
			FirstSpawnTime = 300;
			MinSpawnTime = 780;
			MaxSpawnTime = 1020;
			MaxShipsPerArea = 15;
			AreaSize = 15000;
			MaxSpawnAttempts = 10;
			MinPathDistanceFromPlayer = 2000;
			MaxPathDistanceFromPlayer = 4000;
			MinLunarSpawnHeight = 3500;
			MaxLunarSpawnHeight = 4500;
			MinSpawnDistFromEntities = 1000;
			MinPathDistance = 10000;
			MaxPathDistance = 15000;
			PathCheckStep = 150;
			DespawnDistanceFromEndPath = 1000;
			DespawnDistanceFromPlayer = 1000;
			UseMinimumSpeed = false;
			MinimumSpeed = 10;
			UseSpeedOverride = false;
			SpeedOverride = 20;
			
			UseMaxSpawnGroupFrequency = false;
			MaxSpawnGroupFrequency = 5;

			UseTimeout = true;
			TimeoutDuration = 900;
			TimeoutRadius = 10000;
			TimeoutSpawnLimit = 4;

			EnableWaveSpawner = false;
			UseSpecificRandomGroups = new string[]{"SomeSpawnGroupNameHere", "AnotherSpawnGroupNameHere", "EtcEtcEtc"};
			MinWaveSpawnTime = 1980;
			MaxWaveSpawnTime = 3600;
			TotalSpawnEventsPerCluster = 6;
			TimeBetweenWaveSpawns = 8;
			PlayerClusterDistance = 15000;
			
			UseCleanupSettings = true;
			CleanupUseDistance = true;
			CleanupUseTimer = false;
			CleanupUseBlockLimit = false;
			CleanupDistanceStartsTimer = false;
			CleanupResetTimerWithinDistance = false;
			CleanupDistanceTrigger = 30000;
			CleanupTimerTrigger = 1800;
			CleanupBlockLimitTrigger = 0;
			CleanupIncludeUnowned = true;
			CleanupUnpoweredOverride = true;
			CleanupUnpoweredDistanceTrigger = 20000;
			CleanupUnpoweredTimerTrigger = 900;

		}
		
		public ConfigSpaceCargoShips LoadSettings(){
			
			if(MyAPIGateway.Utilities.FileExistsInWorldStorage("Config-SpaceCargoShips.xml", typeof(ConfigSpaceCargoShips)) == true){
				
				try{
					
					ConfigSpaceCargoShips config = null;
					var reader = MyAPIGateway.Utilities.ReadFileInWorldStorage("Config-SpaceCargoShips.xml", typeof(ConfigSpaceCargoShips));
					string configcontents = reader.ReadToEnd();
					config = MyAPIGateway.Utilities.SerializeFromXML<ConfigSpaceCargoShips>(configcontents);
					Logger.AddMsg("Loaded Existing Settings From Config-SpaceCargoShips.xml");
					return config;
					
				}catch(Exception exc){
					
					Logger.AddMsg("ERROR: Could Not Load Settings From Config-SpaceCargoShips.xml. Using Default Configuration.");
					var defaultSettings = new ConfigSpaceCargoShips();
					return defaultSettings;
					
				}
				
			}
			
			var settings = new ConfigSpaceCargoShips();
			
			try{
				
				using (var writer = MyAPIGateway.Utilities.WriteFileInWorldStorage("Config-SpaceCargoShips.xml", typeof(ConfigSpaceCargoShips))){
				
					writer.Write(MyAPIGateway.Utilities.SerializeToXML<ConfigSpaceCargoShips>(settings));
				
				}
				
			}catch(Exception exc){
				
				Logger.AddMsg("ERROR: Could Not Create Config-SpaceCargoShips.xml. Default Settings Will Be Used.");
				
			}
			
			return settings;
			
		}
		
		public string SaveSettings(ConfigSpaceCargoShips settings){
			
			try{
				
				using (var writer = MyAPIGateway.Utilities.WriteFileInWorldStorage("Config-SpaceCargoShips.xml", typeof(ConfigSpaceCargoShips))){
					
					writer.Write(MyAPIGateway.Utilities.SerializeToXML<ConfigSpaceCargoShips>(settings));
				
				}
				
				Logger.AddMsg("Settings In Config-SpaceCargoShips.xml Updated Successfully!");
				return "Settings Updated Successfully.";
				
			}catch(Exception exc){
				
				Logger.AddMsg("ERROR: Could Not Save To Config-SpaceCargoShips.xml. Changes Will Be Lost On World Reload.");
				
			}
			
			return "Settings Changed, But Could Not Be Saved To XML. Changes May Be Lost On Session Reload.";
			
		}
		
	}
	
}