using System;
using System.Collections.Generic;
using System.Text;

namespace ModularEncountersSpawner.Configuration {
	public abstract class ConfigBase {

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

		public ConfigBase() {

			UseTimeout = true;
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

	}

}
