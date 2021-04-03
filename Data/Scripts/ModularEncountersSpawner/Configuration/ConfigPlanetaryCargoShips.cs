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

	//PlanetaryCargoShips

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

	public class ConfigPlanetaryCargoShips : ConfigBase {
		
		public float ModVersion;
		public int FirstSpawnTime; //Time Until Spawn When World Starts
		public int MinSpawnTime; //Min Time Until Next Spawn
		public int MaxSpawnTime; //Max Time Until Next Spawn
		public int MaxShipsPerArea;
		public double AreaSize;
		public int MaxSpawnAttempts; //Number Of Attempts To Spawn Ship(s)
		public double PlayerSurfaceAltitude; //Player Must Be Less Than This Altitude From Surface For Spawn Attempt
		public double MinPathDistanceFromPlayer;
		public double MaxPathDistanceFromPlayer;
		public double MinSpawnFromGrids;
		public float MinAirDensity; //Acts As A Dynamic Max Altitude For Spawning
		public double MinSpawningAltitude; //Minimum Distance From The Surface For Spawning
		public double MaxSpawningAltitude;
		public double MinPathAltitude; //Minimum Path Altitude From Start to End
		public double MinPathDistance; //Minimum Path Distance Of Cargo Ship
		public double MaxPathDistance; //Maximum Path Distance Of Cargo Ship
		public double PathStepCheckDistance; //Distance Between Altitude Checks Of Path (Used To Ensure Path Isn't Obstructed By Terrain)
		public double DespawnDistanceFromEndPath; // Ship Will Despawn If Within This Distance Of Path End Coordinates
		public double DespawnDistanceFromPlayer;
		public double DespawnAltitude;
		public bool UseMinimumSpeed;
		public float MinimumSpeed;
		public bool UseSpeedOverride; //If True, The Cargo Ship Will Use Override Speed Instead Of Prefab Speed
		public float SpeedOverride; //Override Speed Value For Cargo Ship (If Used)
		
		public bool UseMaxSpawnGroupFrequency;
		public int MaxSpawnGroupFrequency;

		public bool UseTimeout;
		public double TimeoutRadius;
		public int TimeoutSpawnLimit;
		public int TimeoutDuration;

		public bool UseCleanupSettings;
		public bool CleanupUseDistance;
		public bool CleanupUseTimer;
		public bool CleanupUseBlockLimit;
		public bool CleanupDistanceStartsTimer;
		public bool CleanupResetTimerWithinDistance;
		public double CleanupDistanceTrigger;
		public int CleanupTimerTrigger;
		public int CleanupBlockLimitTrigger;
		public bool CleanupIncludeUnowned;
		public bool CleanupUnpoweredOverride;
		public double CleanupUnpoweredDistanceTrigger;
		public int CleanupUnpoweredTimerTrigger;
		
		public bool UseBlockDisable;
		public bool DisableAirVent;
		public bool DisableAntenna;
		public bool DisableArtificialMass;
		public bool DisableAssembler;
		public bool DisableBattery;
		public bool DisableBeacon;
		public bool DisableCollector;
		public bool DisableConnector;
		public bool DisableConveyorSorter;
		public bool DisableDecoy;
		public bool DisableDrill;
		public bool DisableJumpDrive;
		public bool DisableGasGenerator;
		public bool DisableGasTank;
		public bool DisableGatlingGun;
		public bool DisableGatlingTurret;
		public bool DisableGravityGenerator;
		public bool DisableGrinder;
		public bool DisableGyro;
		public bool DisableInteriorTurret;
		public bool DisableLandingGear;
		public bool DisableLaserAntenna;
		public bool DisableLcdPanel;
		public bool DisableLightBlock;
		public bool DisableMedicalRoom;
		public bool DisableMergeBlock;
		public bool DisableMissileTurret;
		public bool DisableOxygenFarm;
		public bool DisableParachuteHatch;
		public bool DisablePiston;
		public bool DisableProgrammableBlock;
		public bool DisableProjector;
		public bool DisableReactor;
		public bool DisableRefinery;
		public bool DisableRocketLauncher;
		public bool DisableReloadableRocketLauncher;
		public bool DisableRotor;
		public bool DisableSensor;
		public bool DisableSolarPanel;
		public bool DisableSoundBlock;
		public bool DisableSpaceBall;
		public bool DisableTimerBlock;
		public bool DisableThruster;
		public bool DisableWelder;
		public bool DisableUpgradeModule;
		
		public ConfigPlanetaryCargoShips(){
			
			ModVersion = MES_SessionCore.ModVersion;
			FirstSpawnTime = 300;
			MinSpawnTime = 780;
			MaxSpawnTime = 1020;
			MaxShipsPerArea = 2;
			AreaSize = 20000;
			MaxSpawnAttempts = 25;
			PlayerSurfaceAltitude = 6000;
			MinPathDistanceFromPlayer = 3000;
			MaxPathDistanceFromPlayer = 5000;
			MinSpawnFromGrids = 1200;
			MinAirDensity = 0.70f;
			MinSpawningAltitude = 1500;
			MaxSpawningAltitude = 2000;
			MinPathAltitude = 300;
			MinPathDistance = 10000;
			MaxPathDistance = 15000;
			PathStepCheckDistance = 100;
			DespawnDistanceFromEndPath = 750;
			DespawnDistanceFromPlayer = 1000;
			DespawnAltitude = 5000;
			UseMinimumSpeed = false;
			MinimumSpeed = 10;
			UseSpeedOverride = false;
			SpeedOverride = 20;
			
			UseMaxSpawnGroupFrequency = false;
			MaxSpawnGroupFrequency = 5;

			UseTimeout = true;
			TimeoutDuration = 900;
			TimeoutRadius = 10000;
			TimeoutSpawnLimit = 2;

			UseCleanupSettings = true;
			CleanupUseDistance = true;
			CleanupUseTimer = false;
			CleanupUseBlockLimit = false;
			CleanupDistanceStartsTimer = false;
			CleanupResetTimerWithinDistance = false;
			CleanupDistanceTrigger = 25000;
			CleanupTimerTrigger = 1800;
			CleanupBlockLimitTrigger = 0;
			CleanupIncludeUnowned = true;
			CleanupUnpoweredOverride = true;
			CleanupUnpoweredDistanceTrigger = 25000;
			CleanupUnpoweredTimerTrigger = 900;
			
		}
		
		public ConfigPlanetaryCargoShips LoadSettings(){
			
			if(MyAPIGateway.Utilities.FileExistsInWorldStorage("Config-PlanetaryCargoShips.xml", typeof(ConfigPlanetaryCargoShips)) == true){
				
				try{
					
					ConfigPlanetaryCargoShips config = null;
					var reader = MyAPIGateway.Utilities.ReadFileInWorldStorage("Config-PlanetaryCargoShips.xml", typeof(ConfigPlanetaryCargoShips));
					string configcontents = reader.ReadToEnd();
					config = MyAPIGateway.Utilities.SerializeFromXML<ConfigPlanetaryCargoShips>(configcontents);
					Logger.AddMsg("Loaded Existing Settings From Config-PlanetaryCargoShips.xml");
					return config;
					
				}catch(Exception exc){
					
					Logger.AddMsg("ERROR: Could Not Load Settings From Config-PlanetaryCargoShips.xml. Using Default Configuration.");
					var defaultSettings = new ConfigPlanetaryCargoShips();
					return defaultSettings;
					
				}
				
			}
			
			var settings = new ConfigPlanetaryCargoShips();
			
			try{
				
				using (var writer = MyAPIGateway.Utilities.WriteFileInWorldStorage("Config-PlanetaryCargoShips.xml", typeof(ConfigPlanetaryCargoShips))){
				
					writer.Write(MyAPIGateway.Utilities.SerializeToXML<ConfigPlanetaryCargoShips>(settings));
				
				}
				
			}catch(Exception exc){
				
				Logger.AddMsg("ERROR: Could Not Create Config-PlanetaryCargoShips.xml. Default Settings Will Be Used.");
				
			}
			
			return settings;
			
		}
		
		public string SaveSettings(ConfigPlanetaryCargoShips settings){
			
			try{
				
				using (var writer = MyAPIGateway.Utilities.WriteFileInWorldStorage("Config-PlanetaryCargoShips.xml", typeof(ConfigPlanetaryCargoShips))){
					
					writer.Write(MyAPIGateway.Utilities.SerializeToXML<ConfigPlanetaryCargoShips>(settings));
				
				}
				
				Logger.AddMsg("Settings In Config-PlanetaryCargoShips.xml Updated Successfully!");
				return "Settings Updated Successfully.";
				
			}catch(Exception exc){
				
				Logger.AddMsg("ERROR: Could Not Save To Config-PlanetaryCargoShips.xml. Changes Will Be Lost On World Reload.");
				
			}
			
			return "Settings Changed, But Could Not Be Saved To XML. Changes May Be Lost On Session Reload.";
			
		}
		
	}
	
}