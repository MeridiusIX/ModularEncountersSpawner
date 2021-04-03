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
	
	//DisableBlocks
	
	public class ConfigDisableBlocks{
		
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
		
		public ConfigDisableBlocks(){
			
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
		
		public ConfigDisableBlocks LoadSettings(){
	
			var settings = new ConfigDisableBlocks();
			
			return settings;
			
		}
				
	}
	
}