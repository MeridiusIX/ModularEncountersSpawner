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

	public class ConfigPlanetaryInstallations : ConfigBase {
		
		public float ModVersion;
		public int PlayerSpawnCooldown;
		public int SpawnTimerTrigger;
		public double PlayerDistanceSpawnTrigger;
		public int MaxShipsPerArea;
		public double AreaSize;
		public double PlayerMaximumDistanceFromSurface;
		public double MinimumSpawnDistanceFromPlayers;
		public double MaximumSpawnDistanceFromPlayers;
		public bool AggressivePathCheck;
		public double SearchPathIncrement;
		
		public double MinimumSpawnDistanceFromOtherGrids;
		public double MinimumTerrainVariance;
		public double MaximumTerrainVariance;
		public bool AggressiveTerrainCheck;
		public double TerrainCheckIncrementDistance;
		
		public double SmallTerrainCheckDistance;
		
		public int MediumSpawnChanceBaseValue;
		public int MediumSpawnChanceIncrement;
		public double MediumSpawnDistanceIncrement;
		public double MediumTerrainCheckDistance;
		
		public int LargeSpawnChanceBaseValue;
		public int LargeSpawnChanceIncrement;
		public double LargeSpawnDistanceIncrement;
		public double LargeTerrainCheckDistance;

		public bool RemoveVoxelsIfGridRemoved;

		public bool UseMaxSpawnGroupFrequency;
		public int MaxSpawnGroupFrequency;
		
		public double DespawnDistanceFromPlayer;

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
		
		public ConfigPlanetaryInstallations(){
			
			ModVersion = MES_SessionCore.ModVersion;
			PlayerSpawnCooldown = 300;
			SpawnTimerTrigger = 60;
			PlayerDistanceSpawnTrigger = 6000;
			MaxShipsPerArea = 10;
			AreaSize = 15000;
			PlayerMaximumDistanceFromSurface = 6000;
			MinimumSpawnDistanceFromPlayers = 3000;
			MaximumSpawnDistanceFromPlayers = 6000;
			AggressivePathCheck = true;
			SearchPathIncrement = 150;
			
			MinimumSpawnDistanceFromOtherGrids = 2500;
			MinimumTerrainVariance = -2.5;
			MaximumTerrainVariance = 2.5;
			AggressiveTerrainCheck = true;
			TerrainCheckIncrementDistance = 10;
			
			SmallTerrainCheckDistance = 40;
			
			MediumSpawnChanceBaseValue = 15;
			MediumSpawnChanceIncrement = 15;
			MediumSpawnDistanceIncrement = 2000;
			MediumTerrainCheckDistance = 70;
			
			LargeSpawnChanceBaseValue = 5;
			LargeSpawnChanceIncrement = 15;
			LargeSpawnDistanceIncrement = 4000;
			LargeTerrainCheckDistance = 100;

			RemoveVoxelsIfGridRemoved = true;

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
			CleanupResetTimerWithinDistance = true;
			CleanupDistanceTrigger = 50000;
			CleanupTimerTrigger = 1800;
			CleanupBlockLimitTrigger = 0;
			CleanupIncludeUnowned = true;
			CleanupUnpoweredOverride = true;
			CleanupUnpoweredDistanceTrigger = 25000;
			CleanupUnpoweredTimerTrigger = 900;
			
		}
		
		public ConfigPlanetaryInstallations LoadSettings(){
			
			if(MyAPIGateway.Utilities.FileExistsInWorldStorage("Config-PlanetaryInstallations.xml", typeof(ConfigPlanetaryInstallations)) == true){
				
				try{
					
					ConfigPlanetaryInstallations config = null;
					var reader = MyAPIGateway.Utilities.ReadFileInWorldStorage("Config-PlanetaryInstallations.xml", typeof(ConfigPlanetaryInstallations));
					string configcontents = reader.ReadToEnd();
					config = MyAPIGateway.Utilities.SerializeFromXML<ConfigPlanetaryInstallations>(configcontents);
					Logger.AddMsg("Loaded Existing Settings From Config-PlanetaryInstallations.xml");
					return config;
					
				}catch(Exception exc){
					
					Logger.AddMsg("ERROR: Could Not Load Settings From Config-PlanetaryInstallations.xml. Using Default Configuration.");
					var defaultSettings = new ConfigPlanetaryInstallations();
					return defaultSettings;
					
				}
				
			}
			
			var settings = new ConfigPlanetaryInstallations();
			
			try{
				
				using (var writer = MyAPIGateway.Utilities.WriteFileInWorldStorage("Config-PlanetaryInstallations.xml", typeof(ConfigPlanetaryInstallations))){
				
					writer.Write(MyAPIGateway.Utilities.SerializeToXML<ConfigPlanetaryInstallations>(settings));
				
				}
				
			}catch(Exception exc){
				
				Logger.AddMsg("ERROR: Could Not Create Config-PlanetaryInstallations.xml. Default Settings Will Be Used.");
				
			}
			
			return settings;
			
		}
		
		public string SaveSettings(ConfigPlanetaryInstallations settings){
			
			try{
				
				using (var writer = MyAPIGateway.Utilities.WriteFileInWorldStorage("Config-PlanetaryInstallations.xml", typeof(ConfigPlanetaryInstallations))){
					
					writer.Write(MyAPIGateway.Utilities.SerializeToXML<ConfigPlanetaryInstallations>(settings));
				
				}
				
				Logger.AddMsg("Settings In Config-PlanetaryInstallations.xml Updated Successfully!");
				return "Settings Updated Successfully.";
				
			}catch(Exception exc){
				
				Logger.AddMsg("ERROR: Could Not Save To Config-PlanetaryInstallations.xml. Changes Will Be Lost On World Reload.");
				
			}
			
			return "Settings Changed, But Could Not Be Saved To XML. Changes May Be Lost On Session Reload.";
			
		}
		
	}

}