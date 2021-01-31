﻿using ModularEncountersSpawner.Configuration;
using ModularEncountersSpawner.Templates;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Definitions;
using Sandbox.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage.Game;
using VRage.ObjectBuilders;
using VRageMath;

namespace ModularEncountersSpawner.Manipulation {
	public static class WeaponRandomizer {

		public struct WeaponProfile {

			public MyCubeBlockDefinition BlockDefinition;
			public bool IsWeaponCore;
			public bool IsWeaponStatic;
			public bool IsWeaponTurret;

		}

		public static Dictionary<string, MyCubeBlockDefinition> BlockDirectory = new Dictionary<string, MyCubeBlockDefinition>();
		public static Dictionary<string, WeaponProfile> WeaponProfiles = new Dictionary<string, WeaponProfile>();
		public static Dictionary<string, float> PowerProviderBlocks = new Dictionary<string, float>();
		public static List<string> ForwardGunIDs = new List<string>();
		public static List<string> TurretIDs = new List<string>();

		public static List<MyDefinitionId> AllWeaponCoreIDs = new List<MyDefinitionId>();
		public static List<MyDefinitionId> AllWeaponCoreStaticIDs = new List<MyDefinitionId>();
		public static List<MyDefinitionId> AllWeaponCoreTurretIDs = new List<MyDefinitionId>();

		public static List<string> BlacklistedWeaponSubtypes = new List<string>();
		public static List<string> WhitelistedWeaponSubtypes = new List<string>();
		public static List<string> BlacklistedWeaponTargetSubtypes = new List<string>();
		public static List<string> WhitelistedWeaponTargetSubtypes = new List<string>();
		public static Dictionary<string, float> PowerDrainingWeapons = new Dictionary<string, float>();

		public static List<MyDefinitionId> DefaultPublicBlocks = new List<MyDefinitionId>();

		public static Random Rnd = new Random();

		public static void Setup() {

			var errorDebugging = new StringBuilder();

			try {

				//Setup Power Hogging Weapons Reference
				PowerDrainingWeapons.Add("NovaTorpedoLauncher_Large", 20); //Nova Heavy Plasma Torpedo
				PowerDrainingWeapons.Add("LargeDualBeamGTFBase_Large", 1050); //GTF Large Dual Beam Laser Turret
				PowerDrainingWeapons.Add("LargeStaticLBeamGTF_Small", 787.50f); //GTF Large Heavy Beam Laser
				PowerDrainingWeapons.Add("LargeStaticLBeamGTF_Large", 787.50f); //GTF Large Heavy Beam Laser
				PowerDrainingWeapons.Add("AegisSmallBeamBase_Large", 828); //Aegis Small Multi-Laser
				PowerDrainingWeapons.Add("AegisMarauderBeamStatic_Large", 18400); //Aegis Gungnir Large Beam Cannon
				PowerDrainingWeapons.Add("MediumQuadBeamGTFBase_Large", 330); //GTF Medium Quad Beam Turret
				PowerDrainingWeapons.Add("MPulseLaserBase_Small", 225); //GTF Medium Pulse Turret
				PowerDrainingWeapons.Add("MPulseLaserBase_Large", 225); //GTF Medium Pulse Turret
				PowerDrainingWeapons.Add("AegisLargeBeamStatic_Large", 7820); //Aegis Large Static Beam Laser
				PowerDrainingWeapons.Add("AegisMediumBeamStaticS_Small", 1797.45f); //Aegis Medium Static Beam Laser
				PowerDrainingWeapons.Add("AegisMediumBeamStatic_Large", 1797.45f); //Aegis Medium Static Beam Laser
				PowerDrainingWeapons.Add("AegisSmallBeamStatic_Large", 828); //Aegis Small Static Beam Laser
				PowerDrainingWeapons.Add("SDualPlasmaBase_Large", 6); //GTF Small Dual Blaster Turret
				PowerDrainingWeapons.Add("LSDualPlasmaStatic_Small", 12); //GTF Dual Static Blaster
				PowerDrainingWeapons.Add("LSDualPlasmaStatic_Large", 12); //GTF Dual Static Blaster
				PowerDrainingWeapons.Add("MDualPlasmaBase_Large", 12); //GTF Medium Dual Blaster Turret
				PowerDrainingWeapons.Add("ThorStatic_Small", 10); //Thor Plasma Cannon
				PowerDrainingWeapons.Add("ThorStatic_Large", 10); //Thor Plasma Cannon
				PowerDrainingWeapons.Add("LPlasmaTriBlasterBase_Large", 24); //GTF Large Tri Blaster Turret
				PowerDrainingWeapons.Add("ThorTurretBase_Large", 10); //Thor Dual Plasma Cannon
				PowerDrainingWeapons.Add("XLCitadelPlasmaCannonBarrel_Large", 48); //GTF XL Citadel Plasma Cannon Turret
				PowerDrainingWeapons.Add("SmallBeamBaseGTF_Large", 15); //GTF Small Beam Turret
				PowerDrainingWeapons.Add("SSmallBeamStaticGTF_Small", 15); //
				PowerDrainingWeapons.Add("Interior_Pulse_Laser_Base_Large", 15); //GTF Pulse Laser Interior Turret
				PowerDrainingWeapons.Add("SmallPulseLaser_Base_Large", 15); //GTF Small Pulse Turret
				PowerDrainingWeapons.Add("MediumStaticLPulseGTF_Small", 241.50f); //GTF Medium Static Pulse Laser
				PowerDrainingWeapons.Add("MediumStaticLPulseGTF_Large", 241.50f); //GTF Medium Static Pulse Laser

				var allDefs = MyDefinitionManager.Static.GetAllDefinitions();

				foreach (MyDefinitionBase definition in allDefs.Where(x => x is MyCubeBlockDefinition)) {

					errorDebugging.Clear();
					errorDebugging.Append("Start Weapon Profile Creation For Definition").AppendLine();
					errorDebugging.Append("Get DefinitionId").AppendLine();
					errorDebugging.Append(definition.Id.ToString()).AppendLine();

					if (BlockDirectory.ContainsKey(definition.Id.ToString()) == false) {

						BlockDirectory.Add(definition.Id.ToString(), definition as MyCubeBlockDefinition);

					} else {

						Logger.AddMsg("Block Reference Setup Found Duplicate DefinitionId Detected And Skipped: " + definition.Id.ToString());
						continue;

					}

					errorDebugging.Append("Skip If Blacklisted").AppendLine();

					errorDebugging.Append("If Power Block, Get MaxOutput").AppendLine();

					if (definition as MyPowerProducerDefinition != null) {

						var powerBlock = definition as MyPowerProducerDefinition;

						if (PowerProviderBlocks.ContainsKey(definition.Id.ToString()) == false) {

							PowerProviderBlocks.Add(definition.Id.ToString(), powerBlock.MaxPowerOutput);

						}

						var batteryDef = powerBlock as MyBatteryBlockDefinition;

						if (batteryDef != null) {

							if (ManipulationCore.BatteryMaxCapacity.ContainsKey(batteryDef.Id.SubtypeName) == false) {

								ManipulationCore.BatteryMaxCapacity.Add(batteryDef.Id.SubtypeName, batteryDef.MaxStoredPower);

							}

						}

					}

					errorDebugging.Append("Check if Weapon Block, Continue if Not").AppendLine();

					MyCubeBlockDefinition weaponBlock = null;
					bool isWeaponCore = false;
					bool isTurret = false;
					bool isStatic = false;

					if (AllWeaponCoreIDs.Contains(definition.Id)) {

						weaponBlock = definition as MyCubeBlockDefinition;
						isWeaponCore = true;
						isStatic = AllWeaponCoreStaticIDs.Contains(definition.Id);
						isTurret = AllWeaponCoreTurretIDs.Contains(definition.Id);

					} else {

						if (definition as MyWeaponBlockDefinition != null) {

							weaponBlock = definition as MyCubeBlockDefinition;

							if (weaponBlock as MyLargeTurretBaseDefinition != null) {

								isTurret = true;

							} else {

								isStatic = true;

							}

						}

					}

					if (weaponBlock == null) {

						continue;

					}

					if (!definition.Public) {

						if (!DefaultPublicBlocks.Contains(definition.Id))
							continue;

					}

					errorDebugging.Append("Set Defintions To WeaponProfile class object").AppendLine();

					WeaponProfile weaponProfile;
					weaponProfile.BlockDefinition = weaponBlock;
					weaponProfile.IsWeaponCore = isWeaponCore;
					weaponProfile.IsWeaponStatic = isStatic;
					weaponProfile.IsWeaponTurret = isTurret;

					Logger.AddMsg("Potential Weapon Profile: " + weaponBlock.Id.ToString() + " / WC: " + weaponProfile.IsWeaponCore.ToString() + " / TW: " + weaponProfile.IsWeaponTurret.ToString() + " / SW: " + weaponProfile.IsWeaponStatic.ToString(), true);

					bool goodSize = false;

					errorDebugging.Append("Check if weapon grid X,Y,Z size is valid").AppendLine();

					/*
					if(weaponProfile.IsWeaponTurret) {
						
						if(weaponBlock.Size.X == weaponBlock.Size.Z && weaponBlock.Size.X % 2 != 0){
							
							goodSize = true;
							//Logger.AddMsg("Turret Profile: " + weaponBlock.Id.ToString(), true);
							TurretIDs.Add(weaponBlock.Id.ToString());
							
						}
			
					} 
					*/

					//Experimental-Start
					if (weaponProfile.IsWeaponTurret) {

						if (weaponBlock.Size.X % 2 != 0 && weaponBlock.Size.Z % 2 != 0) {

							goodSize = true;
							//Logger.AddMsg("Turret Profile: " + weaponBlock.Id.ToString(), true);
							TurretIDs.Add(weaponBlock.Id.ToString());

						}

					}
					//Experimental-End

					if (weaponProfile.IsWeaponStatic) {

						if (weaponBlock.Size.X == weaponBlock.Size.Y && weaponBlock.Size.X % 2 != 0) {

							goodSize = true;
							//Logger.AddMsg("Static Profile: " + weaponBlock.Id.ToString(), true);
							ForwardGunIDs.Add(weaponBlock.Id.ToString());

						}

					}

					if (goodSize == false) {

						continue;

					}

					errorDebugging.Append("Add weapon to profile").AppendLine();

					if (WeaponProfiles.ContainsKey(weaponBlock.Id.ToString()) == false) {

						WeaponProfiles.Add(weaponBlock.Id.ToString(), weaponProfile);


					} else {

						Logger.AddMsg("Weapon Profile Already Exists And Is Being Skipped For: " + weaponBlock.Id.ToString());

					}

				}

			} catch (Exception e) {

				MyVisualScriptLogicProvider.ShowNotificationToAll("Modular Encounters Spawner + NPC Weapon Replacer Encountered An Error.", 10000, "Red");
				MyVisualScriptLogicProvider.ShowNotificationToAll("Please Submit A Copy Of The Game Log To The Mod Author.", 10000, "Red");
				Logger.AddMsg("Error While Handling Weapon Replacer Setup");
				Logger.AddMsg(errorDebugging.ToString());
				Logger.AddMsg(e.ToString());

			}
		
		}

		public static void RandomWeaponReplacing(MyObjectBuilder_CubeGrid cubeGrid, ImprovedSpawnGroup spawnGroup) {

			Logger.AddMsg("Getting Weapon Randomizer Blacklist/Whitelist from Global Settings", true);
			//Update Lists
			BlacklistedWeaponSubtypes = Settings.Grids.WeaponReplacerBlacklist.ToList();
			WhitelistedWeaponSubtypes = Settings.Grids.WeaponReplacerWhitelist.ToList();
			BlacklistedWeaponTargetSubtypes = Settings.Grids.WeaponReplacerTargetBlacklist.ToList();
			WhitelistedWeaponTargetSubtypes = Settings.Grids.WeaponReplacerTargetWhitelist.ToList();


			var errorDebugging = new StringBuilder();
			bool allowRandomizeWeapons = true; //Backwards compatibility laziness
			Dictionary<Vector3I, MyObjectBuilder_CubeBlock> blockMap = new Dictionary<Vector3I, MyObjectBuilder_CubeBlock>();
			List<MyObjectBuilder_CubeBlock> weaponBlocks = new List<MyObjectBuilder_CubeBlock>();
			List<MyObjectBuilder_CubeBlock> replaceBlocks = new List<MyObjectBuilder_CubeBlock>();
			float availablePower = 0;
			float gridBlockCount = 0;
			bool shieldBlocksDetected = false;

			if (cubeGrid?.CubeBlocks == null) {

				Logger.AddMsg("Provided CubeGrid for Weapon Replacing Was Somehow Null or Has No Blocks.");
				return;

			}

			//Build blockMap - This is used to determine which blocks occupy cells.
			foreach (var block in cubeGrid.CubeBlocks) {

				if (block == null)
					continue;

				gridBlockCount++;
				string defIdString = block.GetId().ToString(); //Get MyDefinitionId from ObjectBuilder
				MyCubeBlockDefinition blockDefinition = null;

				//Check if block directory has block.
				if (BlockDirectory.ContainsKey(defIdString) == true) {

					blockDefinition = BlockDirectory[defIdString];

				} else {

					//If Block Definition Could Not Be Found, It 
					//Likely Means The Target Grid Is Using Modded 
					//Blocks And That Mod Is Not Loaded In The Game 
					//World.

					//Logger("Block Definition Could Not Be Found For [" + defIdString + "]. Weapon Randomizer May Produce Unexpected Results.");
					continue;

				}

				if (PowerProviderBlocks.ContainsKey(defIdString) == true) {

					availablePower += PowerProviderBlocks[defIdString];

				}

				//Returns a list of all cells the block occupies
				var cellList = GetBlockCells(block.Min, blockDefinition.Size, block.BlockOrientation);

				//Adds to map. Throws warning if a cell was already occupied, since it should not be.
				foreach (var cell in cellList) {

					if (blockMap.ContainsKey(cell) == false) {

						blockMap.Add(cell, block);

					} else {

						//Logger("Cell for "+ defIdString +" Already Occupied By Another Block. This May Cause Issues.");

					}

				}

				//If block was a weapon, add it to the list of weapons we'll be replacing
				if (block as MyObjectBuilder_UserControllableGun != null) {

					weaponBlocks.Add(block);

				}

				//TODO: Check CustomData For MES-Replace-Block Tag

			}

			availablePower *= 0.666f; //We only want to allow 2/3 of grid power to be used by weapons - this should be ok for most NPCs

			//Now Process Existing Weapon Blocks

			if (allowRandomizeWeapons == true) {

				foreach (var weaponBlock in weaponBlocks) {

					//Get details of weapon block being replaced
					string defIdString = weaponBlock.GetId().ToString();
					errorDebugging.Append("Processing Grid Weapon: ").Append(defIdString).AppendLine();
					MyCubeBlockDefinition blockDefinition = BlockDirectory[defIdString];
					MyWeaponBlockDefinition targetWeaponBlockDef = (MyWeaponBlockDefinition)blockDefinition;

					//Do Blacklist/Whitelist Check on Target Block
					if (targetWeaponBlockDef == null) {

						continue;

					} else if (IsTargetWeaponAllowed(targetWeaponBlockDef, spawnGroup) == false) {

						continue;

					}

					string oldWeaponId = defIdString;
					var weaponIds = WeaponProfiles.Keys.ToList();
					bool isTurret = false;
					bool targetNeutralSetting = false;

					if (weaponBlock as MyObjectBuilder_TurretBase != null) {

						var tempTurretOb = weaponBlock as MyObjectBuilder_TurretBase;
						targetNeutralSetting = tempTurretOb.TargetNeutrals;
						isTurret = true;
						weaponIds = new List<string>(TurretIDs);

					}

					//Get Additional Details From Old Block.
					var oldBlocksCells = GetBlockCells(weaponBlock.Min, blockDefinition.Size, weaponBlock.BlockOrientation);
					var likelyMountingCell = GetLikelyBlockMountingPoint((MyWeaponBlockDefinition)blockDefinition, cubeGrid, blockMap, weaponBlock);
					var oldOrientation = (MyBlockOrientation)weaponBlock.BlockOrientation;
					var oldColor = (Vector3)weaponBlock.ColorMaskHSV;
					var oldLocalForward = GetLocalGridDirection(weaponBlock.BlockOrientation.Forward);
					var oldLocalUp = GetLocalGridDirection(weaponBlock.BlockOrientation.Up);

					var oldMatrix = new MatrixI(ref likelyMountingCell, ref oldLocalForward, ref oldLocalUp);

					//Check if Block Is Named Replacement
					string blockName = (weaponBlock as MyObjectBuilder_TerminalBlock)?.CustomName;
					string restrictedId = "";
					bool onlyNamedReplacements = false;

					if (!string.IsNullOrWhiteSpace(blockName)) {

						MyDefinitionId id = new MyDefinitionId();

						if (spawnGroup.NonRandomWeaponReference.TryGetValue(blockName, out id)) {

							if (weaponIds.Contains(id.ToString())) {

								restrictedId = id.ToString();

							} else {

								if (spawnGroup.NonRandomWeaponReplacingOnly)
									onlyNamedReplacements = true;


							}

						} else {

							onlyNamedReplacements = spawnGroup.NonRandomWeaponReplacingOnly;

						}

					}

					if (onlyNamedReplacements)
						continue;

					//Remove The Old Block
					cubeGrid.CubeBlocks.Remove(weaponBlock);

					foreach (var cell in oldBlocksCells) {

						blockMap.Remove(cell);

					}

					//Loop through weapon IDs and choose one at random each run of the loop
					while (weaponIds.Count > 0) {

						if (weaponIds.Count == 0) {

							errorDebugging.Append(" - No further weapons available to process.").AppendLine();
							break;

						}

						var randIndex = Rnd.Next(0, weaponIds.Count);
						var randId = weaponIds[randIndex];
						weaponIds.RemoveAt(randIndex);

						if (!string.IsNullOrWhiteSpace(restrictedId) && randId != restrictedId)
							continue;

						errorDebugging.Append(" - Attempting to replace with: ").Append(randId).AppendLine();

						if (WeaponProfiles.ContainsKey(randId) == false) {

							errorDebugging.Append(" - No weapon profile for .").AppendLine();
							continue;

						}

						var weaponProfile = WeaponProfiles[randId];

						if (IsWeaponSizeAllowed(blockDefinition.Size, weaponProfile.BlockDefinition.Size, spawnGroup.RandomWeaponSizeVariance)) {

							errorDebugging.Append(" - Weapon Size Variance Outside Limit").AppendLine();

						}

						if (IsWeaponStaticOrTurret(isTurret, weaponProfile) == false) {

							errorDebugging.Append(" - Did not match Turret vs Static").AppendLine();
							continue;

						}

						if (IsRandomWeaponAllowed(weaponProfile.BlockDefinition, spawnGroup) == false) {

							errorDebugging.Append(" - Did not pass Blacklist/Whitelist").AppendLine();
							continue;

						}

						if (weaponProfile.BlockDefinition.CubeSize != cubeGrid.GridSizeEnum) {

							errorDebugging.Append(" - Block not same grid size").AppendLine();
							continue;

						}


						bool isPowerHog = false;
						float powerDrain = 0;

						//Check against manually maintained list of Subtypes that draw energy for ammo generation.
						if (PowerDrainingWeapons.ContainsKey(weaponProfile.BlockDefinition.Id.SubtypeName) == true) {

							if (PowerDrainingWeapons[weaponProfile.BlockDefinition.Id.SubtypeName] > availablePower) {

								continue;

							}

							isPowerHog = true;
							powerDrain = PowerDrainingWeapons[weaponProfile.BlockDefinition.Id.SubtypeName];

						}

						//Calculate Min and Get Block Cells of where new weapon would be placed.
						var estimatedMin = CalculateMinPosition(weaponProfile.BlockDefinition.Size, likelyMountingCell, oldMatrix, isTurret);
						var newBlocksCells = GetBlockCells(estimatedMin, weaponProfile.BlockDefinition.Size, oldOrientation);
						bool foundOccupiedCell = false;

						//Check each cell against blockMap - skip weapon if a cell is occupied 
						foreach (var cell in newBlocksCells) {

							if (blockMap.ContainsKey(cell) == true) {

								foundOccupiedCell = true;
								break;

							}

						}

						if (foundOccupiedCell == true) {

							errorDebugging.Append(" - Grid cell occupied in proposed position.").AppendLine();
							continue;

						}

						//TODO: Learn How Mount Points Work And Try To Add That Check As Well
						//Existing Method Should Work in Most Cases Though

						//Create Object Builder From DefinitionID
						var newBlockBuilder = MyObjectBuilderSerializer.CreateNewObject(weaponProfile.BlockDefinition.Id);

						//Determine If Weapon Is Turret or Gun. Build Object For That Type
						if (isTurret == true && !weaponProfile.IsWeaponCore) {

							var turretBuilder = newBlockBuilder as MyObjectBuilder_TurretBase;
							turretBuilder.EntityId = 0;
							turretBuilder.SubtypeName = weaponProfile.BlockDefinition.Id.SubtypeName;
							turretBuilder.Min = estimatedMin;
							turretBuilder.BlockOrientation = oldOrientation;
							turretBuilder.ColorMaskHSV = oldColor;

							var turretDef = (MyLargeTurretBaseDefinition)weaponProfile.BlockDefinition;

							if (turretDef.MaxRangeMeters <= 800) {

								turretBuilder.Range = turretDef.MaxRangeMeters;


							} else if (gridBlockCount <= 800) {

								if (turretDef.MaxRangeMeters <= 800) {

									turretBuilder.Range = turretDef.MaxRangeMeters;

								} else {

									turretBuilder.Range = 800;

								}

							} else {

								var randRange = (float)Rnd.Next(800, (int)gridBlockCount);

								if (randRange > turretDef.MaxRangeMeters) {

									randRange = turretDef.MaxRangeMeters;

								}

								turretBuilder.Range = randRange;


							}

							turretBuilder.TargetMissiles = true;
							turretBuilder.TargetCharacters = true;
							turretBuilder.TargetSmallGrids = true;
							turretBuilder.TargetLargeGrids = true;
							turretBuilder.TargetStations = true;
							turretBuilder.TargetNeutrals = targetNeutralSetting;

							cubeGrid.CubeBlocks.Add(turretBuilder as MyObjectBuilder_CubeBlock);

						} else {

							var gunBuilder = newBlockBuilder as MyObjectBuilder_CubeBlock;
							gunBuilder.EntityId = 0;
							gunBuilder.SubtypeName = weaponProfile.BlockDefinition.Id.SubtypeName;
							gunBuilder.Min = estimatedMin;
							gunBuilder.BlockOrientation = oldOrientation;
							gunBuilder.ColorMaskHSV = oldColor;

							cubeGrid.CubeBlocks.Add(gunBuilder as MyObjectBuilder_CubeBlock);

						}

						if (isPowerHog == true) {

							availablePower -= powerDrain;

						}

						foreach (var cell in newBlocksCells) {

							if (blockMap.ContainsKey(cell) == false) {

								blockMap.Add(cell, (MyObjectBuilder_CubeBlock)newBlockBuilder);

							}

						}

						Logger.AddMsg("Replaced " + oldWeaponId + " with new weapon " + weaponProfile.BlockDefinition.Id.ToString(), true);
						break;

					}

				}

			}

			if (Logger.LoggerDebugMode == true) {

				Logger.AddMsg(errorDebugging.ToString(), true);

			}

		}

		public static bool IsWeaponSizeAllowed(Vector3I originalSize, Vector3I replacementSize, int spawnGroupVariance) {

			var allowedVariance = spawnGroupVariance > -1 ? spawnGroupVariance : Settings.Grids.RandomWeaponSizeVariance;

			if (allowedVariance == -1)
				return true;

			if (SizeDifference(originalSize.X, replacementSize.X) > allowedVariance)
				return false;

			if (SizeDifference(originalSize.Y, replacementSize.Y) > allowedVariance)
				return false;

			if (SizeDifference(originalSize.Z, replacementSize.Z) > allowedVariance)
				return false;

			return true;

		}

		public static int SizeDifference(int a, int b) {

			if (a >= b)
				return a - b;

			return b - a;

		}

		public static bool IsWeaponStaticOrTurret(bool turret, WeaponProfile weapon) {

			if (weapon.IsWeaponTurret) {

				if (!turret)
					return false;

			}

			if (weapon.IsWeaponStatic) {

				if (turret)
					return false;

			}

			return true;

		}


		public static bool IsRandomWeaponAllowed(MyCubeBlockDefinition weaponDefinition, ImprovedSpawnGroup spawnGroup) {

			//Check SpawnGroup First
			if (spawnGroup.WeaponRandomizerBlacklist.Count > 0) {

				if (spawnGroup.WeaponRandomizerBlacklist.Contains(weaponDefinition.Id.SubtypeName) == true || spawnGroup.WeaponRandomizerBlacklist.Contains(weaponDefinition.Id.ToString()) == true) {

					return false;

				}

				if (weaponDefinition.Context.ModId != "0" && string.IsNullOrEmpty(weaponDefinition.Context.ModId) == false) {

					foreach (var item in spawnGroup.WeaponRandomizerBlacklist) {

						if (weaponDefinition.Context.ModId.Contains(item) == true) {

							return false;

						}

					}

				}

			}

			if (spawnGroup.WeaponRandomizerWhitelist.Count > 0) {

				bool passWhitelist = false;

				if (spawnGroup.WeaponRandomizerWhitelist.Contains(weaponDefinition.Id.SubtypeName) == true || spawnGroup.WeaponRandomizerWhitelist.Contains(weaponDefinition.Id.ToString()) == true) {

					passWhitelist = true;

				}

				if (weaponDefinition.Context.ModId != "0" && string.IsNullOrEmpty(weaponDefinition.Context.ModId) == false) {

					foreach (var item in spawnGroup.WeaponRandomizerWhitelist) {

						if (weaponDefinition.Context.ModId.Contains(item) == true) {

							passWhitelist = true;

						}

					}

				}

				if (passWhitelist == false) {

					return false;

				}

			}

			//Check Settings After
			if (BlacklistedWeaponSubtypes.Count > 0 && spawnGroup.IgnoreWeaponRandomizerGlobalBlacklist == false) {

				if (BlacklistedWeaponSubtypes.Contains(weaponDefinition.Id.SubtypeName) == true || BlacklistedWeaponSubtypes.Contains(weaponDefinition.Id.ToString()) == true) {

					return false;

				}

				if (weaponDefinition.Context.ModId != "0" && string.IsNullOrEmpty(weaponDefinition.Context.ModId) == false) {

					foreach (var item in BlacklistedWeaponSubtypes) {

						if (weaponDefinition.Context.ModId.Contains(item) == true) {

							return false;

						}

					}

				}

			}

			if (WhitelistedWeaponSubtypes.Count > 0 && spawnGroup.IgnoreWeaponRandomizerGlobalWhitelist == false) {

				bool passWhitelist = false;

				if (WhitelistedWeaponSubtypes.Contains(weaponDefinition.Id.SubtypeName) == true || WhitelistedWeaponSubtypes.Contains(weaponDefinition.Id.ToString()) == true) {

					passWhitelist = true;

				}

				if (weaponDefinition.Context.ModId != "0" && string.IsNullOrEmpty(weaponDefinition.Context.ModId) == false) {

					foreach (var item in WhitelistedWeaponSubtypes) {

						if (weaponDefinition.Context.ModId.Contains(item) == true) {

							passWhitelist = true;

						}

					}

				}

				if (passWhitelist == false) {

					return false;

				}

			}

			return true;

		}

		public static bool IsTargetWeaponAllowed(MyWeaponBlockDefinition weaponDefinition, ImprovedSpawnGroup spawnGroup) {

			//Check SpawnGroup First
			if (spawnGroup.WeaponRandomizerTargetBlacklist.Count > 0) {

				if (spawnGroup.WeaponRandomizerTargetBlacklist.Contains(weaponDefinition.Id.SubtypeName) == true || spawnGroup.WeaponRandomizerTargetBlacklist.Contains(weaponDefinition.Id.ToString()) == true) {

					return false;

				}

				if (weaponDefinition.Context.ModId != "0" && string.IsNullOrEmpty(weaponDefinition.Context.ModId) == false) {

					foreach (var item in spawnGroup.WeaponRandomizerTargetBlacklist) {

						if (weaponDefinition.Context.ModId.Contains(item) == true) {

							return false;

						}

					}

				}

			}

			if (spawnGroup.WeaponRandomizerTargetWhitelist.Count > 0) {

				bool passWhitelist = false;

				if (spawnGroup.WeaponRandomizerTargetWhitelist.Contains(weaponDefinition.Id.SubtypeName) == true || spawnGroup.WeaponRandomizerTargetWhitelist.Contains(weaponDefinition.Id.ToString()) == true) {

					passWhitelist = true;

				}

				if (weaponDefinition.Context.ModId != "0" && string.IsNullOrEmpty(weaponDefinition.Context.ModId) == false) {

					foreach (var item in spawnGroup.WeaponRandomizerTargetWhitelist) {

						if (weaponDefinition.Context.ModId.Contains(item) == true) {

							passWhitelist = true;

						}

					}

				}

				if (passWhitelist == false) {

					return false;

				}

			}

			//Check Settings After
			if (BlacklistedWeaponTargetSubtypes.Count > 0 && spawnGroup.IgnoreWeaponRandomizerTargetGlobalBlacklist == false) {

				if (BlacklistedWeaponTargetSubtypes.Contains(weaponDefinition.Id.SubtypeName) == true || BlacklistedWeaponTargetSubtypes.Contains(weaponDefinition.Id.ToString()) == true) {

					return false;

				}

				if (weaponDefinition.Context.ModId != "0" && string.IsNullOrEmpty(weaponDefinition.Context.ModId) == false) {

					foreach (var item in BlacklistedWeaponTargetSubtypes) {

						if (weaponDefinition.Context.ModId.Contains(item) == true) {

							return false;

						}

					}

				}

			}

			if (WhitelistedWeaponTargetSubtypes.Count > 0 && spawnGroup.IgnoreWeaponRandomizerTargetGlobalWhitelist == false) {

				bool passWhitelist = false;

				if (WhitelistedWeaponTargetSubtypes.Contains(weaponDefinition.Id.SubtypeName) == true || WhitelistedWeaponTargetSubtypes.Contains(weaponDefinition.Id.ToString()) == true) {

					passWhitelist = true;

				}

				if (weaponDefinition.Context.ModId != "0" && string.IsNullOrEmpty(weaponDefinition.Context.ModId) == false) {

					foreach (var item in WhitelistedWeaponTargetSubtypes) {

						if (weaponDefinition.Context.ModId.Contains(item) == true) {

							passWhitelist = true;

						}

					}

				}

				if (passWhitelist == false) {

					return false;

				}

			}

			return true;

		}


		public static Vector3I CalculateMinPosition(Vector3I size, Vector3I mountingCell, MatrixI mountingMatrix, bool isTurret) {

			Vector3I minPosition = Vector3I.Zero;

			if (isTurret == true) {

				var cellList = new List<Vector3I>();

				//Move Cells Distance
				/*
				int moveCellDist = (int)Math.Floor((double)size.X / 2);
				//TODO: Try Creating moveCellDistX and moveCellDistZ - See If that Allows Placement of X != Z Turrets
				cellList.Add(Vector3I.Transform(new Vector3I(moveCellDist, 0, moveCellDist), mountingMatrix));
				cellList.Add(Vector3I.Transform(new Vector3I(moveCellDist * -1, 0, moveCellDist), mountingMatrix));
				cellList.Add(Vector3I.Transform(new Vector3I(moveCellDist, 0, moveCellDist * -1), mountingMatrix));
				cellList.Add(Vector3I.Transform(new Vector3I(moveCellDist * -1, 0, moveCellDist * -1), mountingMatrix));
				
				cellList.Add(Vector3I.Transform(new Vector3I(moveCellDist, size.Y - 1, moveCellDist), mountingMatrix));
				cellList.Add(Vector3I.Transform(new Vector3I(moveCellDist * -1, size.Y - 1, moveCellDist), mountingMatrix));
				cellList.Add(Vector3I.Transform(new Vector3I(moveCellDist, size.Y - 1, moveCellDist * -1), mountingMatrix));
				cellList.Add(Vector3I.Transform(new Vector3I(moveCellDist * -1, size.Y - 1, moveCellDist * -1), mountingMatrix));
				*/

				//Experimental-Start
				int moveCellDistX = (int)Math.Floor((double)size.X / 2);
				int moveCellDistZ = (int)Math.Floor((double)size.Z / 2);
				int y = size.Y - 1;
				cellList.Add(Vector3I.Transform(new Vector3I(moveCellDistX, 0, moveCellDistZ), mountingMatrix));
				cellList.Add(Vector3I.Transform(new Vector3I(-moveCellDistX, 0, moveCellDistZ), mountingMatrix));
				cellList.Add(Vector3I.Transform(new Vector3I(moveCellDistX, 0, -moveCellDistZ), mountingMatrix));
				cellList.Add(Vector3I.Transform(new Vector3I(-moveCellDistX, 0, -moveCellDistZ), mountingMatrix));

				cellList.Add(Vector3I.Transform(new Vector3I(moveCellDistX, y, moveCellDistZ), mountingMatrix));
				cellList.Add(Vector3I.Transform(new Vector3I(-moveCellDistX, y, moveCellDistZ), mountingMatrix));
				cellList.Add(Vector3I.Transform(new Vector3I(moveCellDistX, y, -moveCellDistZ), mountingMatrix));
				cellList.Add(Vector3I.Transform(new Vector3I(-moveCellDistX, y, -moveCellDistZ), mountingMatrix));
				//Experimental-End

				for (int i = 0; i < cellList.Count; i++) {

					if (i == 0) {

						minPosition = cellList[i];
						continue;

					}

					minPosition = Vector3I.Min(minPosition, cellList[i]);

				}


			} else {

				var forwardDist = size.Z - 1;
				Vector3I otherEnd = mountingMatrix.ForwardVector * forwardDist + mountingCell;
				minPosition = Vector3I.Min(mountingCell, otherEnd);

			}

			return minPosition;

		}

		//This is used to calculate the 'center' position of the block that is used when you get 
		//VRage.Game.ModAPI.Ingame.IMySlimBlock.Position;
		//I didn't write this.. I lifted it from MySlimBlock, since that's the only place it seems
		//to exist.
		public static Vector3I ComputePositionInGrid(MatrixI localMatrix, Vector3I blockCenter, Vector3I blockSize, Vector3I min) {

			Vector3I center = blockCenter;
			Vector3I vector3I = blockSize - 1;
			Vector3I value;
			Vector3I.TransformNormal(ref vector3I, ref localMatrix, out value);
			Vector3I a;
			Vector3I.TransformNormal(ref center, ref localMatrix, out a);
			Vector3I vector3I2 = Vector3I.Abs(value);
			Vector3I result = a + min;

			if (value.X != vector3I2.X) {

				result.X += vector3I2.X;

			}

			if (value.Y != vector3I2.Y) {

				result.Y += vector3I2.Y;

			}

			if (value.Z != vector3I2.Z) {

				result.Z += vector3I2.Z;

			}

			return result;

		}

		//This returns a list of cells occupied by a block. Useful to get
		//blocks that occupy multiple cells.
		public static List<Vector3I> GetBlockCells(Vector3I Min, Vector3I Size, MyBlockOrientation blockOrientation) {

			var cellList = new List<Vector3I>();
			cellList.Add(Min);

			var localMatrix = new MatrixI(blockOrientation);

			for (int i = 0; i < Size.X; i++) {

				for (int j = 0; j < Size.Y; j++) {

					for (int k = 0; k < Size.Z; k++) {

						var stepSize = new Vector3I(i, j, k);
						var transformedSize = Vector3I.TransformNormal(stepSize, ref localMatrix);
						Vector3I.Abs(ref transformedSize, out transformedSize);
						var cell = Min + transformedSize;

						if (cellList.Contains(cell) == false) {

							cellList.Add(cell);

						}

					}

				}

			}

			return cellList;

		}

		//
		public static Vector3I GetLikelyBlockMountingPoint(MyWeaponBlockDefinition blockDefinition, MyObjectBuilder_CubeGrid cubeGrid, Dictionary<Vector3I, MyObjectBuilder_CubeBlock> blockMap, MyObjectBuilder_CubeBlock block) {

			var direction = Vector3I.Zero;
			Vector3I likelyPosition = ComputePositionInGrid(new MatrixI(block.BlockOrientation), blockDefinition.Center, blockDefinition.Size, block.Min);

			if (TurretIDs.Contains(blockDefinition.Id.ToString()) == true) {

				direction = Vector3I.Down;

			} else {

				direction = Vector3I.Backward;

			}

			var blockForward = GetLocalGridDirection(block.BlockOrientation.Forward);
			var blockUp = GetLocalGridDirection(block.BlockOrientation.Up);
			var blockLocalMatrix = new MatrixI(ref likelyPosition, ref blockForward, ref blockUp);
			bool loopBreak = false;

			while (loopBreak == false) {

				var checkCell = Vector3I.Transform(direction, blockLocalMatrix);
				blockLocalMatrix = new MatrixI(ref checkCell, ref blockForward, ref blockUp);

				if (blockMap.ContainsKey(checkCell) == true) {

					if (blockMap[checkCell] == block) {

						likelyPosition = checkCell;

					} else {

						break;

					}

				} else {

					break;

				}

			}

			return likelyPosition;

		}

		//Translates a Base6Directions direction into a Vector3I
		public static Vector3I GetLocalGridDirection(Base6Directions.Direction Direction) {

			if (Direction == Base6Directions.Direction.Forward) {

				return new Vector3I(0, 0, -1);

			}

			if (Direction == Base6Directions.Direction.Backward) {

				return new Vector3I(0, 0, 1);

			}

			if (Direction == Base6Directions.Direction.Up) {

				return new Vector3I(0, 1, 0);

			}

			if (Direction == Base6Directions.Direction.Down) {

				return new Vector3I(0, -1, 0);

			}

			if (Direction == Base6Directions.Direction.Left) {

				return new Vector3I(-1, 0, 0);

			}

			if (Direction == Base6Directions.Direction.Right) {

				return new Vector3I(1, 0, 0);

			}

			return Vector3I.Zero;

		}


	}

}
