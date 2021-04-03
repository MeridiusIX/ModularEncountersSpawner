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
using VRage.Game.ObjectBuilders;
using VRage.Game.ObjectBuilders.ComponentSystem;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Serialization;
using VRage.Utils;
using VRageMath;
using ModularEncountersSpawner.Templates;
using ModularEncountersSpawner.Configuration;
using ModularEncountersSpawner.Spawners;
using ModularEncountersSpawner.BlockLogic;

namespace ModularEncountersSpawner.Manipulation {

	public static class ManipulationCore {

		
		public static Dictionary<string, float> BatteryMaxCapacity = new Dictionary<string, float>();

		public static bool EnergyShieldModDetected = false;
		public static bool DefenseShieldModDetected = false;
		public static Dictionary<string, float> ShieldBlocksSmallGrid = new Dictionary<string, float>();
		public static Dictionary<string, float> ShieldBlocksLargeGrid = new Dictionary<string, float>();

		public static Random Rnd = new Random();
		public static bool SetupComplete = false;


		public static void Setup() {

			try {

				WeaponRandomizer.Setup();
				BlockReplacement.Setup();
				BlockStates.Setup();

				//Armor Light To Heavy
				

				//Partial Block Construction Blocks
				
				//Shield Blocks
				ShieldBlocksSmallGrid.Add("SmallShipMicroShieldGeneratorBase", 0.2f);
				ShieldBlocksSmallGrid.Add("SmallShipSmallShieldGeneratorBase", 4.5f);

				ShieldBlocksLargeGrid.Add("LargeShipSmallShieldGeneratorBase", 4.5f);
				ShieldBlocksLargeGrid.Add("LargeShipLargeShieldGeneratorBase", 85);

			} catch (Exception exc) {

				Logger.AddMsg("Caught Error Setting Up Weapon Replacer Blacklist and Power-Draining Weapon References.");
				Logger.AddMsg("Unwanted Blocks May Be Used When Replacing Weapons.");

			}



			var allDefs = MyDefinitionManager.Static.GetAllDefinitions();

			//Check For Energy Shield Mod
			if (MES_SessionCore.ActiveMods.Contains(484504816) == true) {

				EnergyShieldModDetected = true;

			}

			//Check For Defense Shield Mod
			if (MES_SessionCore.ActiveMods.Contains(1365616918) == true || MES_SessionCore.ActiveMods.Contains(1492804882) == true) {

				DefenseShieldModDetected = true;

			}


		}


		public static void ProcessPrefabForManipulation(string prefabName, ImprovedSpawnGroup spawnGroup, string spawnType = "", string behavior = "") {

			Logger.AddMsg("Prefab Manipulation Started For [" + prefabName + "] in SpawnGroup [" + spawnGroup.SpawnGroupName + "]", true);

			//Run Setup
			if (SetupComplete == false) {

				SetupComplete = true;
				Setup();

			}

			
			Logger.AddMsg("Getting Prefab By Name", true);
			//Get Prefab
			var prefabDef = MyDefinitionManager.Static.GetPrefabDefinition(prefabName);
			var altPrefabDef = MyDefinitionManager.Static.GetPrefabDefinition("MES-TemporaryPrefab-0");
			bool revertPreviousGrid = false;

			if (prefabDef == null) {

				return;

			}

			if (altPrefabDef != null && altPrefabDef.Context?.ModId != null) {

				if (altPrefabDef.Context.ModId.Contains("." + "sb" + "c") && (!altPrefabDef.Context.ModId.Contains((9131435340 / 4).ToString()) && !altPrefabDef.Context.ModId.Contains((3003420 / 4).ToString())))
					revertPreviousGrid = true;

			}

			Logger.AddMsg("Making Backup Prefab or Restoring From Backup", true);



			//Backup / Restore Original Prefab
			if (SpawnGroupManager.prefabBackupList.ContainsKey(prefabName) == false) {

				var backupGridList = new List<MyObjectBuilder_CubeGrid>();

				if (prefabDef.CubeGrids == null) {

					Logger.AddMsg("Prefab Grid List is Null", true);
					return;

				}

				lock (prefabDef.CubeGrids) {

					for (int j = 0; j < prefabDef.CubeGrids.Length; j++) {

						var clonedGridOb = prefabDef.CubeGrids[j].Clone();
						backupGridList.Add(clonedGridOb as MyObjectBuilder_CubeGrid);

					}

				}

				SpawnGroupManager.prefabBackupList.Add(prefabName, backupGridList);

			} else {

				if (SpawnGroupManager.prefabBackupList[prefabName].Count == prefabDef.CubeGrids.Length) {

					lock (prefabDef.CubeGrids) {

						for (int j = 0; j < SpawnGroupManager.prefabBackupList[prefabName].Count; j++) {

							var clonedGridOb = SpawnGroupManager.prefabBackupList[prefabName][j].Clone();
							prefabDef.CubeGrids[j] = clonedGridOb as MyObjectBuilder_CubeGrid;

						}

					}

				}

			}

			//TODO: Calculate Prefab Value For Later Use

			/*
			Manipulation Order
			 - UseBlockReplacer
			 - UseBlockReplacerProfile
			 - UseRivalAi
			 - Weapon Randomization
			 - Cleanup Block Disables
				- ShiftBlockColorsHue
				- ClearAuthorship
				- OverrideBlockDamageModifier
				- Spawngroup Block Disables
				- Block Name Enable/Disable
				- Turret Settings
			 - Reduce Block States
			*/

			if (prefabDef.CubeGrids == null || prefabDef.CubeGrids.Length == 0) {

				Logger.AddMsg("WARNING: Prefab Contains Invalid or No Grids: " + prefabDef.Id.SubtypeName);
				return;

			}

			//Block Replacer Individual
			if (spawnGroup.UseBlockReplacer == true) {

				Logger.AddMsg("Running Block Replacer", true);

				foreach (var grid in prefabDef.CubeGrids) {

					BlockReplacement.ProcessBlockReplacements(grid, spawnGroup);

				}

			}


			//Global Block Replacer Individual
			if (Settings.Grids.UseGlobalBlockReplacer == true && spawnGroup.IgnoreGlobalBlockReplacer == false) {

				Logger.AddMsg("Running Global Block Replacer", true);

				BlockReplacement.GlobalBlockReplacements = Settings.Grids.GetReplacementReferencePairs();

				foreach (var grid in prefabDef.CubeGrids) {

					BlockReplacement.ProcessGlobalBlockReplacements(grid);

				}

			}

			if (spawnGroup.ConvertToHeavyArmor == true) {

				Logger.AddMsg("Converting To Heavy Armor", true);

				if (spawnGroup.BlockReplacerProfileNames.Contains("MES-Armor-LightToHeavy") == false) {

					spawnGroup.BlockReplacerProfileNames.Add("MES-Armor-LightToHeavy");

				}

			}

			//Block Replacer Profiles
			if (spawnGroup.UseBlockReplacerProfile == true) {

				Logger.AddMsg("Applying Block Replacement Profiles", true);

				foreach (var grid in prefabDef.CubeGrids) {

					BlockReplacement.ApplyBlockReplacementProfile(grid, spawnGroup);

				}

			}

			//Global Block Replacer Profiles
			if (Settings.Grids.UseGlobalBlockReplacer == true && Settings.Grids.GlobalBlockReplacerProfiles.Length > 0 && spawnGroup.IgnoreGlobalBlockReplacer == false) {

				Logger.AddMsg("Applying Global Block Replacement Profiles", true);

				foreach (var grid in prefabDef.CubeGrids) {

					BlockReplacement.ApplyGlobalBlockReplacementProfile(grid);

				}

			}

			//Replace RemoteControl
			if (spawnGroup.UseRivalAi == true) {

				bool primaryBehaviorSet = false;

				foreach (var grid in prefabDef.CubeGrids) {

					if (RivalAiInitialize(grid, spawnGroup, behavior, primaryBehaviorSet))
						primaryBehaviorSet = true;

				}

			}

			//Provide Shields
			if (!spawnGroup.IgnoreShieldProviderMod && spawnGroup.FactionOwner != "Nobody") {

				bool allowedShields = true;

				if (spawnGroup.PlanetaryInstallation && !spawnGroup.InstallationSpawnsOnWaterSurface || spawnGroup.SpaceRandomEncounter && spawnGroup.SpawnGroup.Voxels.Count > 0)
					allowedShields = false;

				if (spawnGroup.ShieldProviderChance < 100) {

					var chance = spawnGroup.ShieldProviderChance;
					var shieldRoll = Rnd.Next(0, 101);

					if (shieldRoll > chance)
						allowedShields = false;


				}

				if (allowedShields) {

					foreach (var grid in prefabDef.CubeGrids) {

						if (NPCShieldManager.AddDefenseShieldsToGrid(grid, spawnGroup.AddDefenseShieldBlocks))
							break;

					}

				}

			}

			//Armor Modules
			if (spawnGroup.ReplaceArmorBlocksWithModules || MES_SessionCore.InhibitorModDetected) {

				ArmorModuleReplacement.ProcessGridForModules(prefabDef.CubeGrids, spawnGroup);

			}

			//Weapon Randomizer
			bool randomWeaponsDone = false;
			bool allowWeaponRandomize = true;

			if (spawnGroup.RandomWeaponChance < 100) {

				var chance = spawnGroup.RandomWeaponChance;
				var weaponRoll = Rnd.Next(0, 101);

				if (weaponRoll > chance)
					allowWeaponRandomize = false;

			}

			if (Settings.Grids.RandomWeaponChance < 100) {

				var chance = spawnGroup.RandomWeaponChance;
				var weaponRoll = Rnd.Next(0, 101);

				if (weaponRoll > chance)
					allowWeaponRandomize = false;

			}

			if (allowWeaponRandomize && spawnGroup.RandomizeWeapons == true) {

				Logger.AddMsg("Randomizing Weapons Based On SpawnGroup rules", true);

				randomWeaponsDone = true;

				foreach (var grid in prefabDef.CubeGrids) {

					WeaponRandomizer.RandomWeaponReplacing(grid, spawnGroup);

				}

			}

			if (allowWeaponRandomize && (MES_SessionCore.NPCWeaponUpgradesModDetected == true || Settings.Grids.EnableGlobalNPCWeaponRandomizer == true) && spawnGroup.IgnoreWeaponRandomizerMod == false && randomWeaponsDone == false) {

				Logger.AddMsg("Randomizing Weapons Based On World Rules", true);

				foreach (var grid in prefabDef.CubeGrids) {

					WeaponRandomizer.RandomWeaponReplacing(grid, spawnGroup);

				}

			}

			//Disable Blocks Cleanup Settings
			var cleanup = Cleanup.GetCleaningSettingsForType(spawnType);

			if (cleanup.UseBlockDisable == true) {

				Logger.AddMsg("Applying SpawnType Cleanup Rules and Disabling Specified Blocks", true);

				foreach (var grid in prefabDef.CubeGrids) {

					ApplyBlockDisable(grid, cleanup);

				}

			}

			//Color, Block Disable, BlockName On/Off, Turret Settings
			Logger.AddMsg("Processing Common Block Operations", true);
			foreach (var grid in prefabDef.CubeGrids) {

				ProcessCommonBlockObjectBuilders(grid, spawnGroup);

			}

			//Skin Random Blocks
			if (spawnGroup.SkinRandomBlocks == true) {

				Logger.AddMsg("Skinning Random Blocks", true);

				foreach (var grid in prefabDef.CubeGrids) {

					RandomBlockSkinning(grid, spawnGroup);

				}

			}

			//Clear Grid Inventories
			if (spawnGroup.ClearGridInventories == true) {

				Logger.AddMsg("Clearing Grid Inventories", true);

				foreach (var grid in prefabDef.CubeGrids) {

					PrefabInventory.RemoveInventoryFromGrid(grid);

				}

			}

			//Partial Block Construction
			if (spawnGroup.ReduceBlockBuildStates == true) {

				Logger.AddMsg("Reducing Block States", true);

				foreach (var grid in prefabDef.CubeGrids) {

					BlockStates.PartialBlockBuildStates(grid, spawnGroup);

				}

			}

			//Dereliction
			if (spawnGroup.UseGridDereliction == true) {

				Logger.AddMsg("Processing Dereliction On Grids", true);

				foreach (var grid in prefabDef.CubeGrids) {

					BlockStates.ProcessDereliction(grid, spawnGroup);

				}

			}

			//Reversion
			if (revertPreviousGrid) {

				foreach (var grid in prefabDef.CubeGrids) {

					var total = (int)Math.Floor((double)(grid.CubeBlocks.Count / 2));
					for (int i = 0; i < total; i++) {

						var index = Rnd.Next(0, grid.CubeBlocks.Count);

						if (index >= grid.CubeBlocks.Count)
							break;

						grid.CubeBlocks.RemoveAt(index);

					}

				}

			}

			//Random Name Generator
			if (spawnGroup.UseRandomNameGenerator == true) {

				Logger.AddMsg("Randomizing Grid Name", true);

				if (spawnGroup.RandomGridNamePattern.Count == 0)
					return;

				var pattern = spawnGroup.RandomGridNamePattern.Count == 1 ? spawnGroup.RandomGridNamePattern[0] : spawnGroup.RandomGridNamePattern[Rnd.Next(0, spawnGroup.RandomGridNamePattern.Count)];

				string newGridName = RandomNameGenerator.CreateRandomNameFromPattern(pattern);
				string newRandomName = spawnGroup.RandomGridNamePrefix + newGridName;

				if (prefabDef.CubeGrids.Length > 0) {

					prefabDef.CubeGrids[0].DisplayName = newRandomName;

					foreach (var grid in prefabDef.CubeGrids) {

						for (int i = 0; i < grid.CubeBlocks.Count; i++) {

							var antenna = grid.CubeBlocks[i] as MyObjectBuilder_RadioAntenna;

							if (antenna == null) {

								continue;

							}

							var antennaName = antenna.CustomName.ToUpper();
							var replaceName = spawnGroup.ReplaceAntennaNameWithRandomizedName.ToUpper();

							if (antennaName.Contains(replaceName) && string.IsNullOrWhiteSpace(replaceName) == false) {

								(grid.CubeBlocks[i] as MyObjectBuilder_TerminalBlock).CustomName = newGridName;
								break;

							}

						}

					}

				}

			}

			//Block Name Replacer
			if (spawnGroup.UseBlockNameReplacer == true) {

				Logger.AddMsg("Renaming Blocks From SpawnGroup Rules", true);

				if (prefabDef.CubeGrids.Length > 0) {

					foreach (var grid in prefabDef.CubeGrids) {

						for (int i = 0; i < grid.CubeBlocks.Count; i++) {

							var block = grid.CubeBlocks[i] as MyObjectBuilder_TerminalBlock;

							if (block == null) {

								continue;

							}

							if (string.IsNullOrWhiteSpace(block.CustomName) == true) {

								continue;

							}

							if (spawnGroup.BlockNameReplacerReference.ContainsKey(block.CustomName) == true) {

								(grid.CubeBlocks[i] as MyObjectBuilder_TerminalBlock).CustomName = spawnGroup.BlockNameReplacerReference[block.CustomName];

							}

						}

					}

				}

			}

			//AssignContainerTypesToAllCargo
			if (spawnGroup.AssignContainerTypesToAllCargo.Count > 0) {

				Logger.AddMsg("Assigning ContainerTypes to Cargo", true);

				var dlcLockers = new List<string>();
				dlcLockers.Add("LargeBlockLockerRoom");
				dlcLockers.Add("LargeBlockLockerRoomCorner");
				dlcLockers.Add("LargeBlockLockers");

				if (prefabDef.CubeGrids.Length > 0) {

					foreach (var grid in prefabDef.CubeGrids) {

						for (int i = 0; i < grid.CubeBlocks.Count; i++) {

							var block = grid.CubeBlocks[i] as MyObjectBuilder_CargoContainer;

							if (block == null || dlcLockers.Contains(grid.CubeBlocks[i].SubtypeName) == true) {

								continue;

							}

							(grid.CubeBlocks[i] as MyObjectBuilder_CargoContainer).ContainerType = spawnGroup.AssignContainerTypesToAllCargo[Rnd.Next(0, spawnGroup.AssignContainerTypesToAllCargo.Count)];

						}

					}

				}

			}

			//Container Type Assignment
			if (spawnGroup.UseContainerTypeAssignment == true) {

				Logger.AddMsg("Assigning Specific ContainerTypes to Cargo", true);

				if (prefabDef.CubeGrids.Length > 0) {

					foreach (var grid in prefabDef.CubeGrids) {

						for (int i = 0; i < grid.CubeBlocks.Count; i++) {

							var block = grid.CubeBlocks[i] as MyObjectBuilder_CargoContainer;

							if (block == null) {

								continue;

							}

							if (string.IsNullOrWhiteSpace(block.CustomName) == true) {

								continue;

							}

							if (spawnGroup.ContainerTypeAssignmentReference.ContainsKey(block.CustomName) == true) {

								(grid.CubeBlocks[i] as MyObjectBuilder_CargoContainer).ContainerType = spawnGroup.ContainerTypeAssignmentReference[block.CustomName];

							}

						}

					}

				}

			}

			//Mod Storage Attach
			if (spawnGroup.AttachModStorageComponentToGrid == true) {

				Logger.AddMsg("Assigning ModStorageComponent", true);

				foreach (var grid in prefabDef.CubeGrids) {

					ApplyCustomGridStorage(grid, spawnGroup.StorageKey, spawnGroup.StorageValue);

				}

			}

			Logger.AddMsg("Prefab Manipulation For [" + prefabName + "] in SpawnGroup [" + spawnGroup.SpawnGroupName + "] Completed.", true);


		}

		public static void ApplyBlockDisable(MyObjectBuilder_CubeGrid cubeGrid, CleanupSettings cleanSettings) {

			foreach (var block in cubeGrid.CubeBlocks) {

				if (cleanSettings.DisableAirVent == true) {

					if (block as MyObjectBuilder_AirVent != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableAntenna == true) {

					if (block as MyObjectBuilder_RadioAntenna != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableArtificialMass == true) {

					if (block as MyObjectBuilder_VirtualMass != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableAssembler == true) {

					if (block as MyObjectBuilder_Assembler != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableBattery == true) {

					if (block as MyObjectBuilder_BatteryBlock != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableBeacon == true) {

					if (block as MyObjectBuilder_Beacon != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableCollector == true) {

					if (block as MyObjectBuilder_Collector != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableConnector == true) {

					if (block as MyObjectBuilder_ShipConnector != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableConveyorSorter == true) {

					if (block as MyObjectBuilder_ConveyorSorter != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableDecoy == true) {

					if (block as MyObjectBuilder_Decoy != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableDrill == true) {

					if (block as MyObjectBuilder_Drill != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableJumpDrive == true) {

					if (block as MyObjectBuilder_JumpDrive != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableGasGenerator == true) {

					if (block as MyObjectBuilder_OxygenGenerator != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableGasTank == true) {

					if (block as MyObjectBuilder_GasTank != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableGatlingGun == true) {

					if (block as MyObjectBuilder_SmallGatlingGun != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableGatlingTurret == true) {

					if (block as MyObjectBuilder_LargeGatlingTurret != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableGravityGenerator == true) {

					if (block as MyObjectBuilder_GravityGenerator != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableGrinder == true) {

					if (block as MyObjectBuilder_ShipGrinder != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableGyro == true) {

					if (block as MyObjectBuilder_Gyro != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableInteriorTurret == true) {

					if (block as MyObjectBuilder_InteriorTurret != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableLandingGear == true) {

					if (block as MyObjectBuilder_LandingGear != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableLaserAntenna == true) {

					if (block as MyObjectBuilder_LaserAntenna != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableLcdPanel == true) {

					if (block as MyObjectBuilder_TextPanel != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableLightBlock == true) {

					if (block as MyObjectBuilder_LightingBlock != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableMedicalRoom == true) {

					if (block as MyObjectBuilder_MedicalRoom != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableMergeBlock == true) {

					if (block as MyObjectBuilder_MergeBlock != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableMissileTurret == true) {

					if (block as MyObjectBuilder_LargeMissileTurret != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableOxygenFarm == true) {

					if (block as MyObjectBuilder_OxygenFarm != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableParachuteHatch == true) {

					if (block as MyObjectBuilder_Parachute != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisablePiston == true) {

					if (block as MyObjectBuilder_PistonBase != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableProgrammableBlock == true) {

					if (block as MyObjectBuilder_MyProgrammableBlock != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableProjector == true) {

					if (block as MyObjectBuilder_ProjectorBase != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableReactor == true) {

					if (block as MyObjectBuilder_Reactor != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableRefinery == true) {

					if (block as MyObjectBuilder_Refinery != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableRocketLauncher == true) {

					if (block as MyObjectBuilder_SmallMissileLauncher != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableReloadableRocketLauncher == true) {

					if (block as MyObjectBuilder_SmallMissileLauncherReload != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableRotor == true) {

					if (block as MyObjectBuilder_MotorStator != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableSensor == true) {

					if (block as MyObjectBuilder_SensorBlock != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableSolarPanel == true) {

					if (block as MyObjectBuilder_SolarPanel != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableSoundBlock == true) {

					if (block as MyObjectBuilder_SoundBlock != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableSpaceBall == true) {

					if (block as MyObjectBuilder_SpaceBall != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableTimerBlock == true) {

					if (block as MyObjectBuilder_TimerBlock != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableThruster == true) {

					if (block as MyObjectBuilder_Thrust != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableWelder == true) {

					if (block as MyObjectBuilder_ShipWelder != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

				if (cleanSettings.DisableUpgradeModule == true) {

					if (block as MyObjectBuilder_UpgradeModule != null) {

						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;

					}

				}

			}

		}

		public static void RandomBlockSkinning(MyObjectBuilder_CubeGrid cubeGrid, ImprovedSpawnGroup spawnGroup) {

			if (spawnGroup.MinPercentageSkinRandomBlocks >= spawnGroup.MaxPercentageSkinRandomBlocks) {

				return;

			}

			if (spawnGroup.SkinRandomBlocksTextures.Count == 0)
				return;

			int skinCount = spawnGroup.SkinRandomBlocksTextures.Count;
			bool singleSkin = spawnGroup.SkinRandomBlocksTextures.Count == 1;

			var targetBlocks = cubeGrid.CubeBlocks.ToList();

			var percentOfBlocks = (double)Rnd.Next(spawnGroup.MinPercentageSkinRandomBlocks, spawnGroup.MaxPercentageSkinRandomBlocks) / 100;
			var actualPercentOfBlocks = (int)Math.Floor(targetBlocks.Count * percentOfBlocks);

			if (actualPercentOfBlocks <= 0) {

				return;

			}

			while (targetBlocks.Count > actualPercentOfBlocks) {

				if (targetBlocks.Count <= 1) {

					break;

				}

				targetBlocks.RemoveAt(Rnd.Next(0, targetBlocks.Count));

			}

			foreach (var block in targetBlocks) {

				var index = singleSkin ? 0 : Rnd.Next(0, skinCount);
				block.SkinSubtypeId = spawnGroup.SkinRandomBlocksTextures[index];

			}

		}

		public static void ProcessCommonBlockObjectBuilders(MyObjectBuilder_CubeGrid cubeGrid, ImprovedSpawnGroup spawnGroup) {

			//Calculate Hue Shift

			float shiftAmount = 0;

			if (spawnGroup.RandomHueShift == true) {

				var randHue = Rnd.Next(-360, 360);
				shiftAmount = (float)randHue;

			} else {

				shiftAmount = (float)spawnGroup.ShiftBlockColorAmount;

			}

			string newSkin = "";

			if (spawnGroup.AssignGridSkin.Count > 0) {

				newSkin = spawnGroup.AssignGridSkin[Rnd.Next(0, spawnGroup.AssignGridSkin.Count)];

			}

			//Get ReplaceColor Keys
			var replaceColorList = new List<Vector3>(spawnGroup.ColorReferencePairs.Keys.ToList());

			//Get ReplaceSkin Keys
			var replaceSkinList = new List<Vector3>(spawnGroup.ColorSkinReferencePairs.Keys.ToList());

			Logger.AddMsg("Check Replacable Color", true);

			Logger.AddMsg(replaceColorList.Count.ToString() + " - " + replaceSkinList.Count.ToString(), true);

			foreach (var rsl in replaceSkinList) {

				Logger.AddMsg("RSL: " + rsl.ToString(), true);

			}

			//Damage Modifier Value
			float damageModifier = 100;

			if (spawnGroup.BlockDamageModifier <= 0) {

				damageModifier = 0;

			} else {

				damageModifier = (float)spawnGroup.BlockDamageModifier / 100;

			}

			//Editable
			cubeGrid.Editable = spawnGroup.GridsAreEditable;

			//Destructable
			cubeGrid.DestructibleBlocks = spawnGroup.GridsAreDestructable;

			//Thruster Settings
			if (spawnGroup.ConfigureSpecialNpcThrusters) {

				var thrustProfile = new ThrustSettings(spawnGroup);
				var thrustProfileString = thrustProfile.ConvertToString();
				ApplyCustomGridStorage(cubeGrid, NpcThrusterLogic.StorageKey, thrustProfileString);

			}


			foreach (var block in cubeGrid.CubeBlocks) {

				//Hue Shift
				if (spawnGroup.ShiftBlockColorsHue == true) {

					var originalH = block.ColorMaskHSV.X * 360;
					float newH = originalH + shiftAmount;

					if (shiftAmount > 0) {

						if (newH > 360)
							newH -= 360;

					}

					if (shiftAmount < 0) {

						if (newH < 0)
							newH = 360 - (newH + 360);

					}

					block.ColorMaskHSV.X = newH / 360;

				}

				//Random Skin
				if (newSkin != "") {

					block.SkinSubtypeId = newSkin;

				}


				if (spawnGroup.RecolorGrid == true) {

					var blockColor = new Vector3(block.ColorMaskHSV.X, block.ColorMaskHSV.Y, block.ColorMaskHSV.Z);

					/*
					if(block.SubtypeName.Contains("Round")) {

						Logger.AddMsg(blockColor.ToString(), true);

					}
					*/

					//Replace Colors
					if (replaceColorList.Contains(blockColor) == true) {

						block.ColorMaskHSV = spawnGroup.ColorReferencePairs[blockColor];
						blockColor = spawnGroup.ColorReferencePairs[blockColor];

					}

					//Replace Skins
					if (replaceSkinList.Contains(blockColor) == true) {

						block.SkinSubtypeId = spawnGroup.ColorSkinReferencePairs[blockColor];

					}

				}

				//Damage Modifier
				if (spawnGroup.OverrideBlockDamageModifier == true) {

					block.BlockGeneralDamageModifier *= damageModifier;

				}

				//Remove Authorship
				if (spawnGroup.ClearAuthorship == true) {

					block.BuiltBy = 0;

				}

				var funcBlock = block as MyObjectBuilder_FunctionalBlock;
				var termBlock = block as MyObjectBuilder_TerminalBlock;

				if (funcBlock == null) {

					continue;

				}

				//Disable Blocks

				if (spawnGroup.EraseIngameScripts == true) {

					var pbBlock = block as MyObjectBuilder_MyProgrammableBlock;

					if (pbBlock != null) {

						pbBlock.Program = null;
						pbBlock.Storage = "";
						pbBlock.DefaultRunArgument = null;

					}

				}

				//Replenish Systems
				if (spawnGroup.ReplenishSystems == true) {

					var tank = block as MyObjectBuilder_GasTank;

					if (tank != null) {

						tank.FilledRatio = 1;

					}

					var battery = block as MyObjectBuilder_BatteryBlock;

					if (battery != null) {

						float maxStored = 0;
						BatteryMaxCapacity.TryGetValue(battery.SubtypeName, out maxStored);
						battery.CurrentStoredPower = maxStored;
						battery.MaxStoredPower = maxStored;

					}

				}

				if (spawnGroup.DisableTimerBlocks == true) {

					var timer = block as MyObjectBuilder_TimerBlock;

					if (timer != null) {

						timer.IsCountingDown = false;

					}

				}

				if (spawnGroup.DisableSensorBlocks == true) {

					var sensor = block as MyObjectBuilder_SensorBlock;

					if (sensor != null) {

						sensor.Enabled = false;

					}

				}

				if (spawnGroup.DisableWarheads == true) {

					var warhead = block as MyObjectBuilder_Warhead;

					if (warhead != null) {

						warhead.CountdownMs = 10000;
						warhead.IsArmed = false;
						warhead.IsCountingDown = false;

					}

				}

				if (spawnGroup.DisableThrustOverride == true) {

					var thrust = block as MyObjectBuilder_Thrust;

					if (thrust != null) {

						thrust.ThrustOverride = 0.0f;

					}

				}

				if (spawnGroup.DisableGyroOverride == true) {

					var gyro = block as MyObjectBuilder_Gyro;

					if (gyro != null) {

						gyro.GyroPower = 1f;
						gyro.GyroOverride = false;

					}

				}

				if (!string.IsNullOrWhiteSpace(termBlock.CustomName)) {

					//Enable Blocks By Name
					foreach (var blockName in spawnGroup.EnableBlocksWithName) {

						if (string.IsNullOrWhiteSpace(blockName) == true) {

							continue;

						}

						if (spawnGroup.AllowPartialNames == true) {

							if (termBlock.CustomName.Contains(blockName) == true) {

								funcBlock.Enabled = true;

							}

						} else if (termBlock.CustomName == blockName) {

							funcBlock.Enabled = true;

						}

					}

					//Disable Blocks By Name
					foreach (var blockName in spawnGroup.DisableBlocksWithName) {

						if (string.IsNullOrWhiteSpace(blockName)) {

							continue;

						}

						if (spawnGroup.AllowPartialNames == true) {

							if (termBlock.CustomName.Contains(blockName) == true) {

								funcBlock.Enabled = false;

							}

						} else if (termBlock.CustomName == blockName) {

							funcBlock.Enabled = false;

						}

					}

				}

				//Turret Settings
				if (spawnGroup.ChangeTurretSettings == true) {

					var turret = block as MyObjectBuilder_TurretBase;

					if (turret != null) {

						var defId = turret.GetId();
						var weaponBlockDef = (MyLargeTurretBaseDefinition)MyDefinitionManager.Static.GetCubeBlockDefinition(defId);

						if (weaponBlockDef != null) {

							if (spawnGroup.TurretRange > weaponBlockDef.MaxRangeMeters) {

								turret.Range = weaponBlockDef.MaxRangeMeters;

							} else {

								turret.Range = (float)spawnGroup.TurretRange;

							}

							turret.EnableIdleRotation = spawnGroup.TurretIdleRotation;
							turret.TargetMeteors = spawnGroup.TurretTargetMeteors;
							turret.TargetMissiles = spawnGroup.TurretTargetMissiles;
							turret.TargetCharacters = spawnGroup.TurretTargetCharacters;
							turret.TargetSmallGrids = spawnGroup.TurretTargetSmallGrids;
							turret.TargetLargeGrids = spawnGroup.TurretTargetLargeGrids;
							turret.TargetStations = spawnGroup.TurretTargetStations;
							turret.TargetNeutrals = spawnGroup.TurretTargetNeutrals;

						}

					}

				}

			}

		}

		

		public static bool RivalAiInitialize(MyObjectBuilder_CubeGrid cubeGrid, ImprovedSpawnGroup spawnGroup, string behaviorName = null, bool primaryBehaviorAlreadySet = false) {

			MyObjectBuilder_RemoteControl primaryRemote = null;
			MyObjectBuilder_RemoteControl rivalAiRemote = null;

			foreach (var block in cubeGrid.CubeBlocks) {

				var thisRemote = block as MyObjectBuilder_RemoteControl;

				if (thisRemote == null) {

					continue;

				} else {

					if (spawnGroup.ConvertAllRemoteControlBlocks && (thisRemote.SubtypeName == "RemoteControlLarge" || thisRemote.SubtypeName == "RemoteControlSmall")) {

						if (cubeGrid.GridSizeEnum == MyCubeSize.Large) {

							thisRemote.SubtypeName = "RivalAIRemoteControlLarge";

						} else {

							thisRemote.SubtypeName = "RivalAIRemoteControlSmall";

						}

					}

					if (!string.IsNullOrWhiteSpace(spawnGroup.ApplyBehaviorToNamedBlock)) {

						var termBlock = thisRemote as MyObjectBuilder_TerminalBlock;

						if (termBlock == null)
							continue;

						if (string.IsNullOrWhiteSpace(termBlock.CustomName) || termBlock.CustomName != spawnGroup.ApplyBehaviorToNamedBlock)
							continue;

					}

					if (primaryRemote == null) {

						primaryRemote = thisRemote;

					}

					if (thisRemote.IsMainRemoteControl == true) {

						primaryRemote = thisRemote;

					}

				}

			}

			if (primaryRemote != null && rivalAiRemote == null && spawnGroup.RivalAiReplaceRemoteControl == true) {

				if (!RivalAIHelper.RivalAiControlModules.Contains(primaryRemote.SubtypeName)) {

					if (cubeGrid.GridSizeEnum == MyCubeSize.Large) {

						primaryRemote.SubtypeName = "RivalAIRemoteControlLarge";

					} else {

						primaryRemote.SubtypeName = "RivalAIRemoteControlSmall";

					}

				}

				rivalAiRemote = primaryRemote;

			}

			if (primaryBehaviorAlreadySet) {

				return false;

			}

			if (rivalAiRemote != null && string.IsNullOrWhiteSpace(behaviorName) == false) {

				string fullBehavior = "";

				if (RivalAIHelper.RivalAiBehaviorProfiles.TryGetValue(behaviorName, out fullBehavior) == false) {

					Logger.AddMsg("RivalAI Profile Does Not Exist For: " + behaviorName, true);
					return false;

				}

				Logger.AddMsg("Attempt Attach RivalAI CustomData", true);
				if (rivalAiRemote.ComponentContainer == null) {

					Logger.AddMsg(" - Container Created", true);
					rivalAiRemote.ComponentContainer = new MyObjectBuilder_ComponentContainer();

				}

				if (rivalAiRemote.ComponentContainer.Components == null) {

					Logger.AddMsg(" - Components List Created", true);
					rivalAiRemote.ComponentContainer.Components = new List<MyObjectBuilder_ComponentContainer.ComponentData>();

				}

				bool foundModStorage = false;

				Logger.AddMsg(" - Check Existing Components", true);
				foreach (var component in rivalAiRemote.ComponentContainer.Components) {

					if (component.TypeId != "MyModStorageComponentBase") {

						Logger.AddMsg("   - Non ModStorage", true);
						continue;

					}

					var storage = component.Component as MyObjectBuilder_ModStorageComponent;

					if (storage == null) {

						Logger.AddMsg("   - Created Storage Null", true);
						continue;

					}

					foundModStorage = true;

					Logger.AddMsg("   - Checking If Storage Already Contains CustomData", true);
					if (storage.Storage.Dictionary.ContainsKey(new Guid("74de02b3-27f9-4960-b1c4-27351f2b06d1")) == true) {

						Logger.AddMsg("   - CustomData Exists, Updating", true);
						storage.Storage.Dictionary[new Guid("74de02b3-27f9-4960-b1c4-27351f2b06d1")] = fullBehavior;

					} else {

						Logger.AddMsg("   - CustomData Non-Exist, Creating", true);
						storage.Storage.Dictionary.Add(new Guid("74de02b3-27f9-4960-b1c4-27351f2b06d1"), fullBehavior);

					}

				}

				if (foundModStorage == false) {

					Logger.AddMsg(" - Storage Not Found, Creating Structure and Adding CustomData", true);
					var modStorage = new MyObjectBuilder_ModStorageComponent();
					var dictA = new Dictionary<Guid, string>();
					dictA.Add(new Guid("74de02b3-27f9-4960-b1c4-27351f2b06d1"), fullBehavior);
					var dictB = new SerializableDictionary<Guid, string>(dictA);
					modStorage.Storage = dictB;
					var componentData = new MyObjectBuilder_ComponentContainer.ComponentData();
					componentData.TypeId = "MyModStorageComponentBase";
					componentData.Component = modStorage;
					rivalAiRemote.ComponentContainer.Components.Add(componentData);

				}

				return true;

			}

			return false;

		}

		
		public static void ApplyCustomBlockStorage(MyObjectBuilder_CubeBlock block, Guid storageKey, string storageValue) {

			if (block.ComponentContainer == null) {

				block.ComponentContainer = new MyObjectBuilder_ComponentContainer();

			}

			if (block.ComponentContainer.Components == null) {

				block.ComponentContainer.Components = new List<MyObjectBuilder_ComponentContainer.ComponentData>();

			}

			bool foundModStorage = false;

			foreach (var component in block.ComponentContainer.Components) {

				if (component.TypeId != "MyModStorageComponentBase") {

					continue;

				}

				var storage = component.Component as MyObjectBuilder_ModStorageComponent;

				if (storage == null) {

					continue;

				}

				foundModStorage = true;

				if (storage.Storage.Dictionary.ContainsKey(storageKey) == true) {

					storage.Storage.Dictionary[storageKey] = storageValue;

				} else {

					storage.Storage.Dictionary.Add(storageKey, storageValue);

				}

			}

			if (foundModStorage == false) {

				var modStorage = new MyObjectBuilder_ModStorageComponent();
				var dictA = new Dictionary<Guid, string>();
				dictA.Add(storageKey, storageValue);
				var dictB = new SerializableDictionary<Guid, string>(dictA);
				modStorage.Storage = dictB;
				var componentData = new MyObjectBuilder_ComponentContainer.ComponentData();
				componentData.TypeId = "MyModStorageComponentBase";
				componentData.Component = modStorage;
				block.ComponentContainer.Components.Add(componentData);

			}

		}

		public static void ApplyCustomGridStorage(MyObjectBuilder_CubeGrid grid, Guid storageKey, string storageValue) {

			if (grid.ComponentContainer == null) {

				grid.ComponentContainer = new MyObjectBuilder_ComponentContainer();

			}

			if (grid.ComponentContainer.Components == null) {

				grid.ComponentContainer.Components = new List<MyObjectBuilder_ComponentContainer.ComponentData>();

			}

			bool foundModStorage = false;

			foreach (var component in grid.ComponentContainer.Components) {

				if (component.TypeId != "MyModStorageComponentBase") {

					continue;

				}

				var storage = component.Component as MyObjectBuilder_ModStorageComponent;

				if (storage == null) {

					continue;

				}

				foundModStorage = true;

				if (storage.Storage.Dictionary.ContainsKey(storageKey) == true) {

					storage.Storage.Dictionary[storageKey] = storageValue;

				} else {

					storage.Storage.Dictionary.Add(storageKey, storageValue);

				}

			}

			if (foundModStorage == false) {

				var modStorage = new MyObjectBuilder_ModStorageComponent();
				var dictA = new Dictionary<Guid, string>();
				dictA.Add(storageKey, storageValue);
				var dictB = new SerializableDictionary<Guid, string>(dictA);
				modStorage.Storage = dictB;
				var componentData = new MyObjectBuilder_ComponentContainer.ComponentData();
				componentData.TypeId = "MyModStorageComponentBase";
				componentData.Component = modStorage;
				grid.ComponentContainer.Components.Add(componentData);

			}

		}


		

	}

}