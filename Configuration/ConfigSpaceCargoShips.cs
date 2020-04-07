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

	public class ConfigSpaceCargoShips{
		
		public float ModVersion {get; set;}
		public int FirstSpawnTime {get; set;} //Time Until Spawn When World Starts
		public int MinSpawnTime {get; set;} //Min Time Until Next Spawn
		public int MaxSpawnTime {get; set;} //Max Time Until Next Spawn
		public int MaxShipsPerArea {get; set;}
		public double AreaSize {get; set;}
		public int MaxSpawnAttempts {get; set;} //Number Of Attempts To Spawn Ship(s)
		public double MinPathDistanceFromPlayer {get; set;}
		public double MaxPathDistanceFromPlayer {get; set;}
		public double MinLunarSpawnHeight {get; set;}
		public double MaxLunarSpawnHeight {get; set;}
		public double MinSpawnDistFromEntities {get; set;}
		public double MinPathDistance {get; set;} //Minimum Path Distance Of Cargo Ship
		public double MaxPathDistance {get; set;} //Maximum Path Distance Of Cargo Ship
		public double PathCheckStep {get; set;}
		public double DespawnDistanceFromEndPath {get; set;} // Ship Will Despawn If Within This Distance Of Path End Coordinates
		public double DespawnDistanceFromPlayer {get; set;}
		public bool UseMinimumSpeed {get; set;}
		public float MinimumSpeed {get; set;}
		public bool UseSpeedOverride {get; set;} //If True, The Cargo Ship Will Use Override Speed Instead Of Prefab Speed
		public float SpeedOverride {get; set;} //Override Speed Value For Cargo Ship (If Used)
		
		public bool UseMaxSpawnGroupFrequency {get; set;}
		public int MaxSpawnGroupFrequency {get; set;}
		
		public bool EnableWaveSpawner {get; set;}
		public string[] UseSpecificRandomGroups {get; set;}
		public int MinWaveSpawnTime {get; set;}
		public int MaxWaveSpawnTime {get; set;}
		public int TotalSpawnEventsPerCluster {get; set;}
		public int TimeBetweenWaveSpawns {get; set;}
		public double PlayerClusterDistance {get; set;}
		
		public bool UseCleanupSettings {get; set;}
		public bool CleanupUseDistance {get; set;}
		public bool CleanupUseTimer {get; set;}
		public bool CleanupUseBlockLimit {get; set;}
		public bool CleanupDistanceStartsTimer {get; set;}
		public bool CleanupResetTimerWithinDistance {get; set;}
		public double CleanupDistanceTrigger {get; set;}
		public int CleanupTimerTrigger {get; set;}
		public int CleanupBlockLimitTrigger {get; set;}
		public bool CleanupIncludeUnowned {get; set;}
		public bool CleanupUnpoweredOverride {get; set;}
		public double CleanupUnpoweredDistanceTrigger {get; set;}
		public int CleanupUnpoweredTimerTrigger {get; set;}

		public bool UseBlockDisable {get; set;}
		public bool DisableAirVent {get; set;}
		public bool DisableAntenna {get; set;}
		public bool DisableArtificialMass {get; set;}
		public bool DisableAssembler {get; set;}
		public bool DisableBattery {get; set;}
		public bool DisableBeacon {get; set;}
		public bool DisableCollector {get; set;}
		public bool DisableConnector {get; set;}
		public bool DisableConveyorSorter {get; set;}
		public bool DisableDecoy {get; set;}
		public bool DisableDrill {get; set;}
		public bool DisableJumpDrive {get; set;}
		public bool DisableGasGenerator {get; set;}
		public bool DisableGasTank {get; set;}
		public bool DisableGatlingGun {get; set;}
		public bool DisableGatlingTurret {get; set;}
		public bool DisableGravityGenerator {get; set;}
		public bool DisableGrinder {get; set;}
		public bool DisableGyro {get; set;}
		public bool DisableInteriorTurret {get; set;}
		public bool DisableLandingGear {get; set;}
		public bool DisableLaserAntenna {get; set;}
		public bool DisableLcdPanel {get; set;}
		public bool DisableLightBlock {get; set;}
		public bool DisableMedicalRoom {get; set;}
		public bool DisableMergeBlock {get; set;}
		public bool DisableMissileTurret {get; set;}
		public bool DisableOxygenFarm {get; set;}
		public bool DisableParachuteHatch {get; set;}
		public bool DisablePiston {get; set;}
		public bool DisableProgrammableBlock {get; set;}
		public bool DisableProjector {get; set;}
		public bool DisableReactor {get; set;}
		public bool DisableRefinery {get; set;}
		public bool DisableRocketLauncher {get; set;}
		public bool DisableReloadableRocketLauncher {get; set;}
		public bool DisableRotor {get; set;}
		public bool DisableSensor {get; set;}
		public bool DisableSolarPanel {get; set;}
		public bool DisableSoundBlock {get; set;}
		public bool DisableSpaceBall {get; set;}
		public bool DisableTimerBlock {get; set;}
		public bool DisableThruster {get; set;}
		public bool DisableWelder {get; set;}
		public bool DisableUpgradeModule {get; set;}

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

			UseBlockDisable = false;
			DisableAirVent = false;
			DisableAntenna = false;
			DisableArtificialMass = false;
			DisableAssembler = false;
			DisableBattery = false;
			DisableBeacon = false;
			DisableCollector = false;
			DisableConnector = false;
			DisableConveyorSorter = false;
			DisableDecoy = false;
			DisableDrill = false;
			DisableJumpDrive = false;
			DisableGasGenerator = false;
			DisableGasTank = false;
			DisableGatlingGun = false;
			DisableGatlingTurret = false;
			DisableGravityGenerator = false;
			DisableGrinder = false;
			DisableGyro = false;
			DisableInteriorTurret = false;
			DisableLandingGear = false;
			DisableLaserAntenna = false;
			DisableLcdPanel = false;
			DisableLightBlock = false;
			DisableMedicalRoom = false;
			DisableMergeBlock = false;
			DisableMissileTurret = false;
			DisableOxygenFarm = false;
			DisableParachuteHatch = false;
			DisablePiston = false;
			DisableProgrammableBlock = false;
			DisableProjector = false;
			DisableReactor = false;
			DisableRefinery = false;
			DisableRocketLauncher = false;
			DisableReloadableRocketLauncher = false;
			DisableRotor = false;
			DisableSensor = false;
			DisableSolarPanel = false;
			DisableSoundBlock = false;
			DisableSpaceBall = false;
			DisableTimerBlock = false;
			DisableThruster = false;
			DisableWelder = false;
			DisableUpgradeModule = false;

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