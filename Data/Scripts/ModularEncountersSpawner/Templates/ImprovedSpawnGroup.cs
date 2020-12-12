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

namespace ModularEncountersSpawner.Templates{

	public class ImprovedSpawnGroup{
		
		public bool SpawnGroupEnabled;
		public string SpawnGroupName;
		public MySpawnGroupDefinition SpawnGroup;
		
		public bool SpaceCargoShip;
		public bool LunarCargoShip;
		public bool AtmosphericCargoShip;
		public bool GravityCargoShip;

		public bool SkipAirDensityCheck;
		
		public bool SpaceRandomEncounter;
		
		public bool PlanetaryInstallation;
		public string PlanetaryInstallationType;
		public bool SkipTerrainCheck;
		public List<Vector3D> RotateInstallations;
		public List<bool> ReverseForwardDirections;
		public bool InstallationTerrainValidation;
		public bool InstallationSpawnsOnDryLand;
		public bool InstallationSpawnsUnderwater;
		public bool InstallationSpawnsOnWaterSurface;

		public bool CutVoxelsAtAirtightCells;
		
		public bool BossEncounterSpace;
		public bool BossEncounterAtmo;
		public bool BossEncounterAny;

		public bool RivalAiSpawn;
		public bool RivalAiSpaceSpawn;
		public bool RivalAiAtmosphericSpawn;
		public bool RivalAiAnySpawn;

		public bool CanSpawnUnderwater;
		public bool MustSpawnUnderwater;
		public double MinWaterDepth;

		public bool StaticEncounter;
		public Vector3D StaticEncounterCoords;
		public bool StaticEncounterPlanetary;
		public bool StaticEncounterDistanceFromSurface;
		public double StaticEncounterMinTriggerDistance;
		public double StaticEncounterMaxTriggerDistance;

		public int Frequency;
		public bool UniqueEncounter;
		public string FactionOwner;
		public bool UseRandomMinerFaction;
		public bool UseRandomBuilderFaction;
		public bool UseRandomTraderFaction;
		public bool IgnoreCleanupRules;
		public bool ReplenishSystems;
		public bool UseNonPhysicalAmmo;
		public bool RemoveContainerContents;
		public bool InitializeStoreBlocks;
		public List<string> ContainerTypesForStoreOrders;
		public bool ForceStaticGrid;
		public bool AdminSpawnOnly;
		public List<string> SandboxVariables;
		public List<string> FalseSandboxVariables;
		public bool UseCommonConditions;

		public bool SettingsAutoHeal;
		public bool SettingsAutoRespawn;
		public bool SettingsBountyContracts;
		public bool SettingsDestructibleBlocks;
		public bool SettingsCopyPaste;
		public bool SettingsContainerDrops;
		public bool SettingsEconomy;
		public bool SettingsEnableDrones;
		public bool SettingsIngameScripts;
		public bool SettingsJetpack;
		public bool SettingsOxygen;
		public bool SettingsResearch;
		public bool SettingsSpawnWithTools;
		public bool SettingsSpiders;
		public bool SettingsSubgridDamage;
		public bool SettingsSunRotation;
		public bool SettingsSupergridding;
		public bool SettingsThrusterDamage;
		public bool SettingsVoxelDestruction;
		public bool SettingsWeaponsEnabled;
		public bool SettingsWeather;
		public bool SettingsWolves;

		public int RandomNumberRoll;
		public int ChanceCeiling;
		public int SpaceCargoShipChance;
		public int LunarCargoShipChance;
		public int AtmosphericCargoShipChance;
		public int GravityCargoShipChance;
		public int RandomEncounterChance;
		public int PlanetaryInstallationChance;
		public int BossEncounterChance;

		public bool UseAutoPilotInSpace; 
		public double PauseAutopilotAtPlayerDistance; 

		public bool PreventOwnershipChange; //Implement / Doc
		
		public bool RandomizeWeapons;
		public bool IgnoreWeaponRandomizerMod;
		public bool IgnoreWeaponRandomizerTargetGlobalBlacklist; 
		public bool IgnoreWeaponRandomizerTargetGlobalWhitelist; 
		public bool IgnoreWeaponRandomizerGlobalBlacklist;
		public bool IgnoreWeaponRandomizerGlobalWhitelist;
		public List<string> WeaponRandomizerTargetBlacklist; 
		public List<string> WeaponRandomizerTargetWhitelist; 
		public List<string> WeaponRandomizerBlacklist;
		public List<string> WeaponRandomizerWhitelist;
		public int RandomWeaponChance;
		public int RandomWeaponSizeVariance;
		public List<string> NonRandomWeaponNames;
		public List<MyDefinitionId> NonRandomWeaponIds;
		public Dictionary<string, MyDefinitionId> NonRandomWeaponReference;
		public bool NonRandomWeaponReplacingOnly;

		public bool AddDefenseShieldBlocks = false;
		public bool IgnoreShieldProviderMod = false;
		public int ShieldProviderChance;

		public bool UseBlockReplacer;
		public Dictionary<MyDefinitionId, MyDefinitionId> ReplaceBlockReference;
		public List<MyDefinitionId> ReplaceBlockOld;
		public List<MyDefinitionId> ReplaceBlockNew;

		public bool UseBlockReplacerProfile;
		public List<string> BlockReplacerProfileNames;
		
		public bool RelaxReplacedBlocksSize;
		public bool AlwaysRemoveBlock;

		public bool ConfigureSpecialNpcThrusters;

		public bool RestrictNpcIonThrust;
		public float NpcIonThrustForceMultiply;
		public float NpcIonThrustPowerMultiply;

		public bool RestrictNpcAtmoThrust;
		public float NpcAtmoThrustForceMultiply;
		public float NpcAtmoThrustPowerMultiply;

		public bool RestrictNpcHydroThrust;
		public float NpcHydroThrustForceMultiply;
		public float NpcHydroThrustPowerMultiply;

		public bool IgnoreGlobalBlockReplacer;

		public bool ConvertToHeavyArmor;

		public bool UseRandomNameGenerator; 
		public string RandomGridNamePrefix; 
		public string RandomGridNamePattern; 
		public string ReplaceAntennaNameWithRandomizedName;

		public bool UseBlockNameReplacer; 
		public Dictionary<string, string> BlockNameReplacerReference;
		public List<string> ReplaceBlockNameOld;
		public List<string> ReplaceBlockNameNew;

		public List<string> AssignContainerTypesToAllCargo; 

		public bool UseContainerTypeAssignment;
		public Dictionary<string, string> ContainerTypeAssignmentReference;
		public List<string> ContainerTypeAssignBlockName;
		public List<string> ContainerTypeAssignSubtypeId;

		public bool OverrideBlockDamageModifier;
		public double BlockDamageModifier;

		public bool GridsAreEditable; 
		public bool GridsAreDestructable; 

		public bool ShiftBlockColorsHue;
		public bool RandomHueShift;
		public double ShiftBlockColorAmount;

		public List<string> AssignGridSkin;

		public bool RecolorGrid;
		public Dictionary<Vector3, Vector3> ColorReferencePairs;
		public List<Vector3> RecolorOld;
		public List<Vector3> RecolorNew;
		public Dictionary<Vector3, string> ColorSkinReferencePairs;
		public List<Vector3> ReskinTarget;
		public List<string> ReskinTexture;

		public bool SkinRandomBlocks;
		public List<string> SkinRandomBlocksTextures;
		public int MinPercentageSkinRandomBlocks;
		public int MaxPercentageSkinRandomBlocks;

		public bool ReduceBlockBuildStates;
		public bool AffectNonFunctionalBlock;
		public bool AffectFunctionalBlock;
		public int MinimumBlocksPercent;
		public int MaximumBlocksPercent;
		public int MinimumBuildPercent;
		public int MaximumBuildPercent;

		public List<string> ReduceBlockStateByType;

		public bool UseRivalAi;
		public bool RivalAiReplaceRemoteControl;
		public string ApplyBehaviorToNamedBlock;
		public bool ConvertAllRemoteControlBlocks;
		
		public bool EraseIngameScripts;
		public bool DisableTimerBlocks;
		public bool DisableSensorBlocks;
		public bool DisableWarheads;
		public bool DisableThrustOverride;
		public bool DisableGyroOverride;
		public bool EraseLCDs;
		public List<string> UseTextureLCD;
		
		public List<string> EnableBlocksWithName;
		public List<string> DisableBlocksWithName;
		public bool AllowPartialNames;
		
		public bool ChangeTurretSettings;
		public double TurretRange;
		public bool TurretIdleRotation;
		public bool TurretTargetMeteors;
		public bool TurretTargetMissiles;
		public bool TurretTargetCharacters;
		public bool TurretTargetSmallGrids;
		public bool TurretTargetLargeGrids;
		public bool TurretTargetStations;
		public bool TurretTargetNeutrals;
		
		public bool ClearAuthorship;
		
		public double MinSpawnFromWorldCenter;
		public double MaxSpawnFromWorldCenter;
		public Vector3D CustomWorldCenter;
		public Vector3D DirectionFromWorldCenter;
		public double MinAngleFromDirection;
		public double MaxAngleFromDirection;

		public double MinSpawnFromPlanetSurface;
		public double MaxSpawnFromPlanetSurface;

		public bool UseDayOrNightOnly;
		public bool SpawnOnlyAtNight;

		public bool UseWeatherSpawning;
		public List<string> AllowedWeatherSystems;

		public bool UseTerrainTypeValidation;
		public List<string> AllowedTerrainTypes;

		public double MinAirDensity;
		public double MaxAirDensity;
		public double MinGravity;
		public double MaxGravity;

		public List<string> PlanetBlacklist;
		public List<string> PlanetWhitelist;
		public bool PlanetRequiresVacuum;
		public bool PlanetRequiresAtmo;
		public bool PlanetRequiresOxygen;
		public double PlanetMinimumSize;
		public double PlanetMaximumSize;

		public bool UsePlayerCountCheck;
		public double PlayerCountCheckRadius;
		public int MinimumPlayers;
		public int MaximumPlayers;

		public bool UseThreatLevelCheck;
		public double ThreatLevelCheckRange;
		public bool ThreatIncludeOtherNpcOwners;
		public int ThreatScoreMinimum;
		public int ThreatScoreMaximum;
		
		public bool UsePCUCheck;
		public double PCUCheckRadius;
		public int PCUMinimum;
		public int PCUMaximum;
		
		public bool UsePlayerCredits;
		public bool IncludeAllPlayersInRadius;
		public bool IncludeFactionBalance;
		public double PlayerCreditsCheckRadius;
		public int MinimumPlayerCredits;
		public int MaximumPlayerCredits;

		public bool UsePlayerFactionReputation;
		public double PlayerReputationCheckRadius;
		public string CheckReputationAgainstOtherNPCFaction;
		public int MinimumReputation;
		public int MaximumReputation;

		public bool ChargeNpcFactionForSpawn;
		public long ChargeForSpawning;

		public bool UseSandboxCounterCosts;
		public List<string> SandboxCounterCostNames;
		public List<int> SandboxCounterCostAmounts;

		public bool UseRemoteControlCodeRestrictions;
		public string RemoteControlCode;
		public double RemoteControlCodeMinDistance;
		public double RemoteControlCodeMaxDistance;

		public List<ulong> RequireAllMods;
		public List<ulong> RequireAnyMods;
		public List<ulong> ExcludeAllMods;
		public List<ulong> ExcludeAnyMods;
		
		public List<string> ModBlockExists;
		
		public List<ulong> RequiredPlayersOnline;
		public List<ulong> RequiredAnyPlayersOnline;

		public bool AttachModStorageComponentToGrid; 
		public Guid StorageKey; 
		public string StorageValue;

		public bool UseKnownPlayerLocations;
		public bool KnownPlayerLocationMustMatchFaction;
		public int KnownPlayerLocationMinSpawnedEncounters;
		public int KnownPlayerLocationMaxSpawnedEncounters;

		public string Territory;
		public double MinDistanceFromTerritoryCenter;
		public double MaxDistanceFromTerritoryCenter;
		
		public bool BossCustomAnnounceEnable;
		public string BossCustomAnnounceAuthor;
		public string BossCustomAnnounceMessage;
		public string BossCustomGPSLabel;
		public string BossMusicId;
		
		public bool RotateFirstCockpitToForward;
		public bool PositionAtFirstCockpit;
		public bool SpawnRandomCargo;
		public bool DisableDampeners;
		public bool ReactorsOn;
		public bool UseBoundingBoxCheck;
		public bool RemoveVoxelsIfGridRemoved;
		
		public ImprovedSpawnGroup(){
			
			SpawnGroupEnabled = true;
			SpawnGroupName = "";
			SpawnGroup = null;
			
			SpaceCargoShip = false;
			LunarCargoShip = false;
			AtmosphericCargoShip = false;
			GravityCargoShip = false;

			SkipAirDensityCheck = false;

			SpaceRandomEncounter = false;
			
			PlanetaryInstallation = false;
			PlanetaryInstallationType = "Small";
			SkipTerrainCheck = false;
			RotateInstallations = new List<Vector3D>();
			ReverseForwardDirections = new List<bool>();
			InstallationTerrainValidation = false;
			InstallationSpawnsOnDryLand = true;
			InstallationSpawnsUnderwater = false;
			InstallationSpawnsOnWaterSurface = false;

			CutVoxelsAtAirtightCells = false;
			
			BossEncounterSpace = false;
			BossEncounterAtmo = false;
			BossEncounterAny = false;

			RivalAiSpawn = false;
			RivalAiSpaceSpawn = false;
			RivalAiAtmosphericSpawn = false;
			RivalAiAnySpawn = false;

			CanSpawnUnderwater = false;
			MustSpawnUnderwater = false;
			MinWaterDepth = 0;

			Frequency = 0; 
			UniqueEncounter = false;
			FactionOwner = "SPRT";
			UseRandomMinerFaction = false;
			UseRandomBuilderFaction = false;
			UseRandomTraderFaction = false;
			IgnoreCleanupRules = false;
			ReplenishSystems = false;
			UseNonPhysicalAmmo = false;
			RemoveContainerContents = false;
			InitializeStoreBlocks = false;
			ContainerTypesForStoreOrders = new List<string>();
			ForceStaticGrid = false;
			AdminSpawnOnly = false;
			SandboxVariables = new List<string>();
			FalseSandboxVariables = new List<string>();
			RandomNumberRoll = 1;
			UseCommonConditions = true;

			ChanceCeiling = 100;
			SpaceCargoShipChance = 100;
			LunarCargoShipChance = 100;
			AtmosphericCargoShipChance = 100;
			GravityCargoShipChance = 100;
			RandomEncounterChance = 100;
			PlanetaryInstallationChance = 100;
			BossEncounterChance = 100;

			UseAutoPilotInSpace = false;
			PauseAutopilotAtPlayerDistance = -1;

			PreventOwnershipChange = false;

			RandomizeWeapons = false;
			IgnoreWeaponRandomizerMod = false;
			IgnoreWeaponRandomizerTargetGlobalBlacklist = false;
			IgnoreWeaponRandomizerTargetGlobalWhitelist = false;
			IgnoreWeaponRandomizerGlobalBlacklist = false;
			IgnoreWeaponRandomizerGlobalWhitelist = false;
			WeaponRandomizerTargetBlacklist = new List<string>();
			WeaponRandomizerTargetWhitelist = new List<string>();
			WeaponRandomizerBlacklist = new List<string>();
			WeaponRandomizerWhitelist = new List<string>();
			RandomWeaponChance = 100;
			RandomWeaponSizeVariance = -1;
			NonRandomWeaponNames = new List<string>();
			NonRandomWeaponIds = new List<MyDefinitionId>();
			NonRandomWeaponReference = new Dictionary<string, MyDefinitionId>();
			NonRandomWeaponReplacingOnly = false;

			AddDefenseShieldBlocks = false;
			IgnoreShieldProviderMod = false;
			ShieldProviderChance = 100;

			UseBlockReplacer = false;
			ReplaceBlockReference = new Dictionary<MyDefinitionId, MyDefinitionId>();
			ReplaceBlockOld = new List<MyDefinitionId>();
			ReplaceBlockNew = new List<MyDefinitionId>();
			
			UseBlockReplacerProfile = false;
			BlockReplacerProfileNames = new List<string>();

			RelaxReplacedBlocksSize = false;
			AlwaysRemoveBlock = false;

			ConfigureSpecialNpcThrusters = false;

			RestrictNpcIonThrust = false;
			NpcIonThrustForceMultiply = 1;
			NpcIonThrustPowerMultiply = 1;

			RestrictNpcAtmoThrust = false;
			NpcAtmoThrustForceMultiply = 1;
			NpcAtmoThrustPowerMultiply = 1;

			RestrictNpcHydroThrust = false;
			NpcHydroThrustForceMultiply = 1;
			NpcHydroThrustPowerMultiply = 1;

			IgnoreGlobalBlockReplacer = false;

			ConvertToHeavyArmor = false;

			UseRandomNameGenerator = false;
			RandomGridNamePrefix = "";
			RandomGridNamePattern = "";
			ReplaceAntennaNameWithRandomizedName = "";

			UseBlockNameReplacer = false;
			BlockNameReplacerReference = new Dictionary<string, string>();
			ReplaceBlockNameOld = new List<string>();
			ReplaceBlockNameNew = new List<string>();

			AssignContainerTypesToAllCargo = new List<string>();

			UseContainerTypeAssignment = false;
			ContainerTypeAssignmentReference = new Dictionary<string, string>();
			ContainerTypeAssignBlockName = new List<string>();
			ContainerTypeAssignSubtypeId = new List<string>();

			OverrideBlockDamageModifier = false;
			BlockDamageModifier = 1;

			GridsAreEditable = true;
			GridsAreDestructable = true;

			ShiftBlockColorsHue = false;
			RandomHueShift = false;
			ShiftBlockColorAmount = 0;

			AssignGridSkin = new List<string>();

			RecolorGrid = false;
			ColorReferencePairs = new Dictionary<Vector3, Vector3>();
			RecolorOld = new List<Vector3>();
			RecolorNew = new List<Vector3>();
			ColorSkinReferencePairs = new Dictionary<Vector3, string>();
			ReskinTarget = new List<Vector3>();
			ReskinTexture = new List<string>();

			SkinRandomBlocks = false;
			SkinRandomBlocksTextures = new List<string>();
			MinPercentageSkinRandomBlocks = 10;
			MaxPercentageSkinRandomBlocks = 40;

			ReduceBlockBuildStates = false;
			AffectNonFunctionalBlock = true;
			AffectFunctionalBlock = false;
			MinimumBlocksPercent = 10;
			MaximumBlocksPercent = 40;
			MinimumBuildPercent = 10;
			MaximumBuildPercent = 75;
			
			UseRivalAi = false;
			RivalAiReplaceRemoteControl = false;
			ApplyBehaviorToNamedBlock = "";
			ConvertAllRemoteControlBlocks = false;

			EraseIngameScripts = false;
			DisableTimerBlocks = false;
			DisableSensorBlocks = false;
			DisableWarheads = false;
			DisableThrustOverride = false;
			DisableGyroOverride = false;
			EraseLCDs = false;
			UseTextureLCD = new List<string>();
			
			EnableBlocksWithName = new List<string>();
			DisableBlocksWithName = new List<string>();
			AllowPartialNames = false;
			
			ChangeTurretSettings = false;
			TurretRange = 800;
			TurretIdleRotation = false;
			TurretTargetMeteors = true;
			TurretTargetMissiles = true;
			TurretTargetCharacters = true;
			TurretTargetSmallGrids = true;
			TurretTargetLargeGrids = true;
			TurretTargetStations = true;
			TurretTargetNeutrals = true;
			
			ClearAuthorship = false;
			
			MinSpawnFromWorldCenter = -1;
			MaxSpawnFromWorldCenter = -1;
			CustomWorldCenter = Vector3D.Zero;
			DirectionFromWorldCenter = Vector3D.Zero;
			MinAngleFromDirection = -1;
			MaxAngleFromDirection = -1;

			MinSpawnFromPlanetSurface = -1;
			MaxSpawnFromPlanetSurface = -1;

			UseDayOrNightOnly = false;
			SpawnOnlyAtNight = false;

			UseWeatherSpawning = false;
			AllowedWeatherSystems = new List<string>();

			UseTerrainTypeValidation = false;
			AllowedTerrainTypes = new List<string>();

			MinAirDensity = -1;
			MaxAirDensity = -1;
			MinGravity = -1;
			MaxGravity = -1;

			PlanetBlacklist = new List<string>();
			PlanetWhitelist = new List<string>();
			PlanetRequiresVacuum = false;
			PlanetRequiresAtmo = false;
			PlanetRequiresOxygen = false;
			PlanetMinimumSize = -1;
			PlanetMaximumSize = -1;

			UsePlayerCountCheck = false;
			PlayerCountCheckRadius = -1;
			MinimumPlayers = -1;
			MaximumPlayers = -1;

			UseThreatLevelCheck = false;
			ThreatLevelCheckRange = 5000;
			ThreatIncludeOtherNpcOwners = false;
			ThreatScoreMinimum = -1;
			ThreatScoreMaximum = -1;
			
			UsePCUCheck = false;
			PCUCheckRadius = 5000;
			PCUMinimum = -1;
			PCUMaximum = -1;
			
			UsePlayerCredits = false;
			IncludeAllPlayersInRadius = false;
			IncludeFactionBalance = false;
			PlayerCreditsCheckRadius = 15000;
			MinimumPlayerCredits = -1;
			MaximumPlayerCredits = -1;
			
			UsePlayerFactionReputation = false;
			PlayerReputationCheckRadius = 15000;
			CheckReputationAgainstOtherNPCFaction = "";
			MinimumReputation = -1501;
			MaximumReputation = 1501;

			ChargeNpcFactionForSpawn = false;
			ChargeForSpawning = 0;

			UseSandboxCounterCosts = false;
			SandboxCounterCostNames = new List<string>();
			SandboxCounterCostAmounts = new List<int>();

			UseRemoteControlCodeRestrictions = false;
			RemoteControlCode = "";
			RemoteControlCodeMinDistance = -1;
			RemoteControlCodeMaxDistance = -1;

			RequireAllMods = new List<ulong>();
			RequireAnyMods = new List<ulong>();
			ExcludeAllMods = new List<ulong>();
			ExcludeAnyMods = new List<ulong>();
			
			ModBlockExists = new List<string>();
			
			RequiredPlayersOnline = new List<ulong>();
			RequiredAnyPlayersOnline = new List<ulong>();

			AttachModStorageComponentToGrid = false;
			StorageKey = new Guid("00000000-0000-0000-0000-000000000000");
			StorageValue = "";

			UseKnownPlayerLocations = false;
			KnownPlayerLocationMustMatchFaction = false;
			KnownPlayerLocationMinSpawnedEncounters = -1;
			KnownPlayerLocationMaxSpawnedEncounters = -1;

			Territory = "";
			MinDistanceFromTerritoryCenter = -1;
			MaxDistanceFromTerritoryCenter = -1;
			
			BossCustomAnnounceEnable = false;
			BossCustomAnnounceAuthor = "";
			BossCustomAnnounceMessage = "";
			BossCustomGPSLabel = "Dangerous Encounter";
			BossMusicId = "";

			RotateFirstCockpitToForward = true;
			PositionAtFirstCockpit = false;
			SpawnRandomCargo = true;
			DisableDampeners = false;
			ReactorsOn = true;
			UseBoundingBoxCheck = false;
			RemoveVoxelsIfGridRemoved = true;

		}
	
	}

}