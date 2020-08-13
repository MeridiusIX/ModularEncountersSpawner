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

	public class ConfigPlanetaryCargoShips{
		
		public float ModVersion {get; set;}
		public int FirstSpawnTime {get; set;} //Time Until Spawn When World Starts
		public int MinSpawnTime {get; set;} //Min Time Until Next Spawn
		public int MaxSpawnTime {get; set;} //Max Time Until Next Spawn
		public int MaxShipsPerArea {get; set;}
		public double AreaSize {get; set;}
		public int MaxSpawnAttempts {get; set;} //Number Of Attempts To Spawn Ship(s)
		public double PlayerSurfaceAltitude {get; set;} //Player Must Be Less Than This Altitude From Surface For Spawn Attempt
		public double MinPathDistanceFromPlayer {get; set;}
		public double MaxPathDistanceFromPlayer {get; set;}
		public double MinSpawnFromGrids {get; set;}
		public float MinAirDensity {get; set;} //Acts As A Dynamic Max Altitude For Spawning
		public double MinSpawningAltitude {get; set;} //Minimum Distance From The Surface For Spawning
		public double MaxSpawningAltitude {get; set;}
		public double MinPathAltitude {get; set;} //Minimum Path Altitude From Start to End
		public double MinPathDistance {get; set;} //Minimum Path Distance Of Cargo Ship
		public double MaxPathDistance {get; set;} //Maximum Path Distance Of Cargo Ship
		public double PathStepCheckDistance {get; set;} //Distance Between Altitude Checks Of Path (Used To Ensure Path Isn't Obstructed By Terrain)
		public double DespawnDistanceFromEndPath {get; set;} // Ship Will Despawn If Within This Distance Of Path End Coordinates
		public double DespawnDistanceFromPlayer {get; set;}
		public double DespawnAltitude {get; set;}
		public bool UseMinimumSpeed {get; set;}
		public float MinimumSpeed {get; set;}
		public bool UseSpeedOverride {get; set;} //If True, The Cargo Ship Will Use Override Speed Instead Of Prefab Speed
		public float SpeedOverride {get; set;} //Override Speed Value For Cargo Ship (If Used)
		
		public bool UseMaxSpawnGroupFrequency {get; set;}
		public int MaxSpawnGroupFrequency {get; set;}
		
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
		
		public ConfigPlanetaryCargoShips(){
			
			ModVersion = MES_SessionCore.ModVersion;
			FirstSpawnTime = 300;
			MinSpawnTime = 780;
			MaxSpawnTime = 1020;
			MaxShipsPerArea = 1;
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