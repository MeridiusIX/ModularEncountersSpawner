using ModularEncountersSpawner.Configuration;
using ModularEncountersSpawner.Helpers;
using ModularEncountersSpawner.Manipulation;
using ModularEncountersSpawner.Spawners;
using ModularEncountersSpawner.Templates;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Definitions;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage;
using VRage.Game;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Utils;
using VRageMath;

namespace ModularEncountersSpawner.World {

	public static class NPCWatcher {

		//NPC Faction and Founder Data
		public static List<long> NPCFactionFounders = new List<long>();
		public static List<string> NPCFactionTags = new List<string>();
		public static Dictionary<long, string> NPCFactionFounderToTag = new Dictionary<long, string>();
		public static Dictionary<string, long> NPCFactionTagToFounder = new Dictionary<string, long>();

		//NPC Remote Controls and Codes
		public static Dictionary<IMyRemoteControl, string> RemoteControlCodes = new Dictionary<IMyRemoteControl, string>();

		//Drop Container Names
		public static List<string> DropContainerNames = new List<string>();
		public static List<string> EconomyStationNames = new List<string>();

		//NPC Parameter GUIDs
		public static Guid GuidStartCoords = new Guid("CC27ADFD-A121-477A-94B1-FB1B4E2E3046");
		public static Guid GuidEndCoords = new Guid("513F6C90-E0D9-4A8F-972E-09757FE32C19");
		public static Guid GuidSpawnType = new Guid("C9D22735-C76B-4DB4-AFB5-51D1E1516A05");
		public static Guid GuidCleanupTimer = new Guid("8E5E70C9-9C7B-429A-9D5D-036465948175");
		public static Guid GuidIgnoreCleanup = new Guid("7ADDED32-4069-4C52-891C-25F52478B2EB");
		public static Guid GuidWeaponsReplaced = new Guid("C0CD2D13-AA56-466E-BA44-D840658A772B");
		public static Guid GuidActiveNpcData = new Guid("AD4DBD09-359D-48F5-9F48-54D352B59171");

		//Pending Boss Encounters
		public static List<BossEncounter> BossEncounters = new List<BossEncounter>();

		//Pending NPC Deletion
		public static bool DeleteGrids = false;
		public static int DeletionTimer = 0;
		public static List<IMyCubeGrid> DeleteGridList = new List<IMyCubeGrid>();
		public static IMyCubeGrid LastDeletedGrid = null;

		//Pending NPC Spawns
		public static List<ActiveNPC> PendingNPCs = new List<ActiveNPC>();
		public static int PreviousPendingSpawns = 0;
		public static int PendingNPCTimeout = 0;

		//Active Ships and Stations
		public static Dictionary<IMyCubeGrid, ActiveNPC> ActiveNPCs = new Dictionary<IMyCubeGrid, ActiveNPC>();

		//Spawned Voxels
		public static Dictionary<string, IMyEntity> SpawnedVoxels = new Dictionary<string, IMyEntity>();

		//Watcher Timers
		public static int NpcDistanceCheckTimer = 1;
		public static int NpcOwnershipCheckTimer = 10;
		public static int NpcCleanupCheckTimer = 60;
		public static int NpcBlacklistCheckTimer = 5;
		public static int NpcBossSignalCheckTimer = 10;
		public static int SpawnedVoxelCheckTimer = 900;

		//Active Boss Encounter
		public static Vector3D bossCoords = new Vector3D(0, 0, 0);
		public static IMyGps bossGps = null;

		public static bool ActiveNpcTypeLimitReachedForArea(string spawnType, Vector3D checkArea, int maxCount, double areaDistance) {

			var count = 0;

			foreach (var cubeGrid in ActiveNPCs.Keys.ToList()) {

				if (cubeGrid == null || MyAPIGateway.Entities.Exist(cubeGrid as IMyEntity) == false) {

					continue;

				}

				if (ActiveNPCs.ContainsKey(cubeGrid) == false) {

					continue;

				}
				if (Vector3D.
				Distance(cubeGrid.GetPosition(), checkArea) < areaDistance && ActiveNPCs[cubeGrid].SpawnType == spawnType) {

					count++;

				}

			}

			if (count >= maxCount) {

				return true;

			}

			return false;

		}

		public static void ActiveNpcMonitor() {

			if (PendingNPCs.Count > 0) {

				if (PreviousPendingSpawns == 0) {

					PendingNPCTimeout = 30;

				}

				PreviousPendingSpawns = PendingNPCs.Count;
				PendingNPCTimeout--;

				if (PendingNPCTimeout <= 0) {

					PendingNPCs.Clear();

				}

			} else {

				PreviousPendingSpawns = 0;

			}

			NpcDistanceCheckTimer--;
			NpcOwnershipCheckTimer--;
			NpcCleanupCheckTimer--;
			SpawnedVoxelCheckTimer--;

			if (NpcBlacklistCheckTimer >= 0) {

				NpcBlacklistCheckTimer--;

			}

			if (SpawnedVoxelCheckTimer <= 0) {

				SpawnedVoxelCheck();

			}

			var grids = new List<IMyCubeGrid>(ActiveNPCs.Keys.ToList());

			foreach (var cubeGrid in grids) {

				if (ActiveNPCs.ContainsKey(cubeGrid) == false) {

					continue;

				}

				if (cubeGrid == null || MyAPIGateway.Entities.Exist(cubeGrid) == false) {

					if (ActiveNPCs.ContainsKey(cubeGrid) == true) {

						ActiveNPCs.Remove(cubeGrid);

					}

					continue;

				}

				var gridEntity = cubeGrid as IMyEntity;

				if (MyAPIGateway.Entities.Exist(gridEntity) == false) {

					if (ActiveNPCs.ContainsKey(cubeGrid) == true) {

						ActiveNPCs.Remove(cubeGrid);

					}

					continue;

				}

				//NPC Ownership Check
				if (NpcOwnershipCheckTimer <= 0) {

					if (NpcOwnershipCheck(cubeGrid) == false) {

						Logger.AddMsg("NPC Grid: " + cubeGrid.CustomName + " / " + cubeGrid.EntityId.ToString() + " ownership no longer meets NPC Requirements and will not be monitored by Spawner.");
						ActiveNPCs[cubeGrid].FullyNPCOwned = false;
						ActiveNPCs.Remove(cubeGrid);
						SetThrusterPowerConsumption(cubeGrid, 1);
						RemoveGUIDs(cubeGrid);
						continue;

					}

					//Economy Stations
					if (ActiveNPCs[cubeGrid].EconomyStationCheck == false) {

						ActiveNPCs[cubeGrid].EconomyStationCheck = true;

						if (ActiveNPCs[cubeGrid].SpawnType == "Other") {

							if (IsEconomyStation(cubeGrid) == true) {

								Logger.AddMsg(cubeGrid.CustomName + " is Keen Economy Station and will not be cleaned by MES.", true);
								ActiveNPCs[cubeGrid].CleanupIgnore = true;


							}

						}

					}



					//Keen AI Handler
					if (ActiveNPCs[cubeGrid].KeenBehaviorCheck == false) {

						var myCubeGrid = cubeGrid as MyCubeGrid;
						Logger.AddMsg(cubeGrid.CustomName + " Dampeners Second Check: " + myCubeGrid.DampenersEnabled, true);

						ActiveNPCs[cubeGrid].KeenBehaviorCheck = true;

						if (string.IsNullOrWhiteSpace(ActiveNPCs[cubeGrid].KeenAiName) == false) {

							if (RivalAIHelper.RivalAiBehaviorProfiles.ContainsKey(ActiveNPCs[cubeGrid].KeenAiName) == false) {

								//TODO: Attach AI Here.
								if (string.IsNullOrEmpty(cubeGrid.Name) == true) {

									MyVisualScriptLogicProvider.SetName(cubeGrid.EntityId, cubeGrid.EntityId.ToString());

								}

								MyVisualScriptLogicProvider.SetDroneBehaviourFull(cubeGrid.EntityId.ToString(), ActiveNPCs[cubeGrid].KeenAiName, true, false, null, false, null, 10, ActiveNPCs[cubeGrid].KeenAiTriggerDistance);
								ActiveNPCs[cubeGrid].KeenBehaviorCheck = false;

							}

						} else {

							Logger.AddMsg("Encounter Has No Stock AI Defined", true);

						}

					}

					//Defense Shield Provider Activation
					if (ActiveNPCs[cubeGrid].DefenseShieldActivationCheck == false) {

						ActiveNPCs[cubeGrid].DefenseShieldActivationCheck = true;
						NPCShieldManager.ActivateShieldsForNPC(cubeGrid);


					}

					//Remove Container Inventory From NPCs
					if (ActiveNPCs[cubeGrid].EmptyInventoryCheck == false) {

						ActiveNPCs[cubeGrid].EmptyInventoryCheck = true;

						if (ActiveNPCs[cubeGrid].SpawnGroup != null) {

							if (ActiveNPCs[cubeGrid].SpawnGroup.RemoveContainerContents == true || Settings.Grids.RemoveContainerInventoryFromNPCs == true) {

								GridUtilities.RemoveGridContainerComponents(cubeGrid);

							}

						}

					}

					//Store Blocks
					if (ActiveNPCs[cubeGrid].StoreBlocksInit == false) {

						ActiveNPCs[cubeGrid].StoreBlocksInit = true;

						if (ActiveNPCs[cubeGrid].SpawnGroup != null) {

							if (ActiveNPCs[cubeGrid].SpawnGroup.InitializeStoreBlocks == true) {

								EconomyHelper.InitNpcStoreBlock(cubeGrid, ActiveNPCs[cubeGrid].SpawnGroup);

							}

						}

					}

					//Ammo Fill Check
					if (ActiveNPCs[cubeGrid].ReplenishedSystems == false) {

						Logger.AddMsg("Restocking Grid Inventories", true);
						ActiveNPCs[cubeGrid].ReplenishedSystems = true;
						GridUtilities.ReplenishGridSystems(cubeGrid, ActiveNPCs[cubeGrid].ReplacedWeapons);

					}

					//Non Physical Ammo Check
					if (ActiveNPCs[cubeGrid].NonPhysicalAmmoCheck == false) {

						ActiveNPCs[cubeGrid].NonPhysicalAmmoCheck = true;

						if (Settings.Grids.UseNonPhysicalAmmoForNPCs == true || ActiveNPCs[cubeGrid].SpawnGroup.UseNonPhysicalAmmo == true) {

							Logger.AddMsg("Processing Non Physical Ammo For Grid: " + cubeGrid.CustomName, true);
							GridUtilities.NonPhysicalAmmoProcessing(cubeGrid);

						}

					}

					//Voxel Cut Check
					if (ActiveNPCs[cubeGrid].VoxelCut == false && ActiveNPCs[cubeGrid].SpawnGroup != null) {

						ActiveNPCs[cubeGrid].VoxelCut = true;

						if (ActiveNPCs[cubeGrid].SpawnGroup.CutVoxelsAtAirtightCells == true) {

							try {

								Logger.AddMsg("Attempting Voxel Cutting", true);
								CutVoxelsAtAirtightPositions(cubeGrid);

							} catch (Exception exc) {

								Logger.AddMsg("Error While Cutting Installation Voxels");
								Logger.AddMsg(exc.ToString(), true);

							}

						}

					}

					if (ActiveNPCs[cubeGrid].SpawnType == "PlanetaryCargoShip") {

						for (int i = ActiveNPCs[cubeGrid].GasGenerators.Count - 1; i >= 0; i--) {

							var generator = ActiveNPCs[cubeGrid].GasGenerators[i];

							if (generator == null) {

								ActiveNPCs[cubeGrid].HydrogenTanks.RemoveAt(i);
								continue;

							}

							if (generator.IsFunctional == false || generator.IsWorking == false || (float)generator.GetInventory(0).CurrentVolume > (float)generator.GetInventory(0).MaxVolume / 2) {

								continue;

							}

							var invToFill = generator.GetInventory(0).MaxVolume - generator.GetInventory(0).CurrentVolume;
							invToFill *= 1000;
							invToFill -= 10;
							MyDefinitionId defId = new MyDefinitionId(typeof(MyObjectBuilder_Ore), "Ice");
							var content = (MyObjectBuilder_PhysicalObject)MyObjectBuilderSerializer.CreateNewObject(defId);
							MyObjectBuilder_InventoryItem inventoryItem = new MyObjectBuilder_InventoryItem { Amount = invToFill, Content = content };
							generator.GetInventory(0).AddItems(invToFill, inventoryItem.Content);

						}

					}

				}

				//NPC Blacklist Check
				if (NpcBlacklistCheckTimer == 0) {

					if (BlacklistCheck(cubeGrid) == true) {

						continue;

					}

				}

			}

			//NPC Distance Check
			if (NpcDistanceCheckTimer <= 0) {

				DistanceChecker();

			}

			if (NpcDistanceCheckTimer <= 0) {

				NpcDistanceCheckTimer = Settings.General.NpcDistanceCheckTimerTrigger;

			}

			if (NpcOwnershipCheckTimer <= 0) {

				NpcOwnershipCheckTimer = Settings.General.NpcOwnershipCheckTimerTrigger;

			}

			if (NpcBlacklistCheckTimer == 0) {

				Cleanup.CleanupProcess(true);

			}

			if (NpcCleanupCheckTimer <= 0) {

				Logger.AddMsg("Running Cleanup", true);
				NpcCleanupCheckTimer = Settings.General.NpcCleanupCheckTimerTrigger;
				Cleanup.CleanupProcess();

			}

		}

		public static bool BlacklistCheck(IMyCubeGrid cubeGrid) {

			var blacklistNames = new List<string>(Settings.General.NpcGridNameBlacklist.ToList());

			if (blacklistNames.Contains(cubeGrid.CustomName) == true) {

				Logger.AddMsg("Blacklisted NPC Ship Found and Removed: " + cubeGrid.CustomName);
				ActiveNPCs.Remove(cubeGrid);
				DeleteGrid(cubeGrid);
				return true;

			}

			return false;

		}

		public static void BossSignalWatcher() {

			if (BossEncounters.Count == 0) {

				return;

			}

			bool listChange = false;

			foreach (var player in MES_SessionCore.PlayerList) {

				if (player.IsBot == true || player.Character == null) {

					continue;

				}

				for (int i = BossEncounters.Count - 1; i >= 0; i--) {

					if (BossEncounters[i].CheckPlayerDistance(player) == true) {

						BossEncounters[i].SpawnAttempts++;
						Logger.AddMsg("Player " + player.DisplayName + " Is Within Signal Distance Of Boss Encounter. Attempting Spawn.");
						SpawnResources.RefreshEntityLists();

						if (BossEncounterSpawner.SpawnBossEncounter(BossEncounters[i]) == true || BossEncounters[i].SpawnAttempts > 5) {

							Logger.AddMsg("Removing Boss Encounter GPS", true);
							BossEncounters[i].RemoveGpsForPlayers();
							BossEncounters.RemoveAt(i);
							listChange = true;
							continue;

						}

					}

				}

			}

			for (int i = BossEncounters.Count - 1; i >= 0; i--) {

				BossEncounters[i].Timer--;

				if (BossEncounters[i].Timer <= 0) {

					Logger.AddMsg("Boss Encounter Timer Expired. Removing GPS.", true);
					BossEncounters[i].RemoveGpsForPlayers();
					BossEncounters.RemoveAt(i);
					listChange = true;
					continue;

				}

			}

			if (listChange == true) {

				try {

					if (BossEncounters.Count > 0) {

						BossEncounter[] encounterArray = BossEncounters.ToArray();
						var byteArray = MyAPIGateway.Utilities.SerializeToBinary(encounterArray);
						var storedBossData = Convert.ToBase64String(byteArray);
						MyAPIGateway.Utilities.SetVariable("MES-ActiveBossEncounters", storedBossData);

					} else {

						MyAPIGateway.Utilities.SetVariable("MES-ActiveBossEncounters", "");

					}

				} catch (Exception e) {

					Logger.AddMsg("Something went wrong while updating Boss Encounter Data to Storage.");
					Logger.AddMsg(e.ToString(), true);

				}

			}

		}

		public static void DeleteGrid(IMyCubeGrid cubeGrid) {

			if (cubeGrid == null || MyAPIGateway.Entities.Exist(cubeGrid) == false) {

				return;

			}

			if (NpcOwnershipCheck(cubeGrid, true, true) == false) {

				Logger.AddMsg("Despawning Aborted For Grid : " + cubeGrid.CustomName + " / " + cubeGrid.EntityId.ToString() + ". Ownership Discrepancy Detected.");
				return;

			}

			Logger.AddMsg("Despawning Grid : " + cubeGrid.CustomName + " / " + cubeGrid.EntityId.ToString());

			try {

				var gridGroups = MyAPIGateway.GridGroups.GetGroup(cubeGrid, GridLinkTypeEnum.Physical);

				foreach (var grid in gridGroups) {

					if (DeleteGridList.Contains(grid) == false) {

						DeleteGridList.Add(grid);

					}

				}

				DeleteGrids = true;

				/*if(cubeGrid != null && MyAPIGateway.Entities.Exist(cubeGrid) == true){

					if(cubeGrid.MarkedForClose == false) {

						cubeGrid.Close();

					}

				}*/

			} catch (Exception exc) {

				Logger.AddMsg("Failed To Despawn Grid With ID: " + cubeGrid.EntityId.ToString());
				Logger.AddMsg(exc.ToString());

			}

		}

		public static void DeleteGridsProcessing() {

			if (DeleteGridList.Count == 0) {

				DeleteGrids = false;
				return;

			}

			if (MyAPIGateway.Entities.Exist(LastDeletedGrid) == true) {

				return;

			}

			MyAPIGateway.Utilities.InvokeOnGameThread(() => {

				for (int i = DeleteGridList.Count - 1; i >= 0; i--) {

					if (MyAPIGateway.Entities.Exist(DeleteGridList[i]) == true && !DeleteGridList[i].MarkedForClose) {

						var cubeGrid = DeleteGridList[i] as MyCubeGrid;

						using (cubeGrid.Pin()) {

							Logger.AddMsg(" - Closing Grid...", true);
							DeleteGridList[i].Close();
							DeleteGridList.RemoveAt(i);
							Logger.AddMsg(" - Closed Grid.", true);
							return;

						}

					}

					DeleteGridList.RemoveAt(i);

				}

			});

		}

		public static void DistanceChecker() {

			var grids = new List<IMyCubeGrid>(ActiveNPCs.Keys.ToList());

			foreach (var cubeGrid in grids) {

				if (ActiveNPCs.ContainsKey(cubeGrid) == false) {

					continue;

				}

				if (cubeGrid == null || MyAPIGateway.Entities.Exist(cubeGrid) == false) {

					ActiveNPCs.Remove(cubeGrid);
					continue;

				}

				var activeNPC = ActiveNPCs[cubeGrid];


				if (activeNPC.FixTurrets == false && activeNPC.SpawnType != "Other") {

					activeNPC.FixTurrets = true;

					try {

						var gts = MyAPIGateway.TerminalActionsHelper.GetTerminalSystemForGrid(cubeGrid);
						var blockList = new List<IMyLargeTurretBase>();
						gts.GetBlocksOfType(blockList);

						var doorList = new List<IMyDoor>();
						gts.GetBlocksOfType(doorList);

						foreach (var turret in blockList) {

							if (turret.CubeGrid.EntityId != cubeGrid.EntityId) {

								continue;

							}

							var blockColor = turret.SlimBlock.ColorMaskHSV;
							cubeGrid.ColorBlocks(turret.Min, turret.Min, new Vector3(42, 41, 40));
							turret.SlimBlock.UpdateVisual();
							cubeGrid.ColorBlocks(turret.Min, turret.Min, blockColor);
							turret.SlimBlock.UpdateVisual();


						}

						foreach (var door in doorList) {

							if (door.CubeGrid.EntityId != cubeGrid.EntityId) {

								continue;

							}

							var blockColor = door.SlimBlock.ColorMaskHSV;
							cubeGrid.ColorBlocks(door.Min, door.Min, new Vector3(42, 41, 40));
							door.SlimBlock.UpdateVisual();
							cubeGrid.ColorBlocks(door.Min, door.Min, blockColor);
							door.SlimBlock.UpdateVisual();

						}

					} catch (Exception exc) {

						Logger.AddMsg("Unexpected exception while applying subpart fix script");

					}

				}

				//Space / Lunar Cargo Ships
				if (activeNPC.SpawnType == "SpaceCargoShip" || activeNPC.SpawnType == "LunarCargoShip") {

					try {

						if (Vector3D.Distance(cubeGrid.GetPosition(), activeNPC.EndCoords) < Settings.SpaceCargoShips.DespawnDistanceFromEndPath == activeNPC.FlagForDespawn == false) {

							activeNPC.FlagForDespawn = true;
							activeNPC.DespawnReason = "EndPath";

						}

						if (activeNPC.FlagForDespawn == true) {

							var player = SpawnResources.GetNearestPlayer(cubeGrid.GetPosition());

							if (player == null || Vector3D.Distance(cubeGrid.GetPosition(), player.GetPosition()) > Settings.SpaceCargoShips.DespawnDistanceFromPlayer) {

								Logger.AddMsg("NPC Cargo Ship " + cubeGrid.CustomName + " Has Reached End Of Travel Path And Has Been Despawned.");
								activeNPC.ActivateDespawnActions();
								ActiveNPCs.Remove(cubeGrid);
								DeleteGrid(cubeGrid);
								continue;

							}

						} else {

							if (activeNPC.SpawnGroup.UseAutoPilotInSpace == true) {

								if (activeNPC.SpawnGroup.PauseAutopilotAtPlayerDistance > 0) {

									double closestPlayerDistance = -1;

									foreach (var player in MES_SessionCore.PlayerList) {

										if (player == null) {

											continue;

										}

										if (player.IsBot || player.Character == null) {

											continue;

										}

										var dist = Vector3D.Distance(cubeGrid.GetPosition(), player.GetPosition());

										if (dist < closestPlayerDistance || closestPlayerDistance == -1) {

											if (activeNPC.faction != null) {

												if (MyAPIGateway.Session.Factions.GetReputationBetweenPlayerAndFaction(player.IdentityId, activeNPC.faction.FactionId) <= -500) {

													continue;

												}

											}

											closestPlayerDistance = dist;

										}

									}

									if (activeNPC.RemoteControl != null) {

										if (activeNPC.RemoteControl.IsAutoPilotEnabled == false && activeNPC.SpawnGroup.PauseAutopilotAtPlayerDistance < closestPlayerDistance || closestPlayerDistance == -1) {

											activeNPC.RemoteControl.SetAutoPilotEnabled(true);

										}

										if (activeNPC.RemoteControl.IsAutoPilotEnabled == true && activeNPC.SpawnGroup.PauseAutopilotAtPlayerDistance > closestPlayerDistance && closestPlayerDistance > -1) {

											activeNPC.RemoteControl.SetAutoPilotEnabled(false);

										}

									}

								}

							}

						}

					} catch (Exception exc) {

						Logger.AddMsg("Unexpected exception in Space Cargo Ship Distance Checker");

					}

				}

				//Atmo Cargo Ships
				if (activeNPC.SpawnType == "PlanetaryCargoShip") {

					var errorLogger = new StringBuilder();

					try {

						errorLogger.Clear();
						errorLogger.Append("Checking Planetary Cargo Ship").AppendLine();
						errorLogger.Append(cubeGrid.CustomName).AppendLine();

						bool skip = activeNPC.FlagForDespawn;


						if (activeNPC.Planet == null) {

							errorLogger.Append("Planet Missing").AppendLine();
							Logger.AddMsg("Planet For Planetary Cargo Ship " + cubeGrid.CustomName + " / " + cubeGrid.EntityId + " No Longer Exists. The NPC Ship Will Be Despawned.");
							activeNPC.FlagForDespawn = true;
							skip = true;

						}

						if (activeNPC.RemoteControl == null && skip == false) {

							errorLogger.Append("Remote Missing").AppendLine();
							Logger.AddMsg("Planetary Cargo Ship " + cubeGrid.CustomName + " Remote Control Damaged, Missing, Or Inactive. Ship Now Identified As \"Other\" NPC.");
							activeNPC.SpawnType = "Other";

							if (cubeGrid.Storage != null) {

								cubeGrid.Storage[GuidActiveNpcData] = activeNPC.ToString();

							}

							continue;

						}

						if (activeNPC.RemoteControl != null && skip == false) {

							if (activeNPC.RemoteControl.IsFunctional == false) {

								Logger.AddMsg("Planetary Cargo Ship " + cubeGrid.CustomName + " Remote Control Damaged, Missing, Or Inactive. Ship Now Identified As \"Other\" NPC.");
								activeNPC.SpawnType = "Other";

								if (cubeGrid.Storage != null) {

									cubeGrid.Storage[GuidActiveNpcData] = activeNPC.ToString();

								}

								continue;

							}

							if (activeNPC.SpawnGroup.PauseAutopilotAtPlayerDistance > 0) {

								double closestPlayerDistance = -1;

								foreach (var player in MES_SessionCore.PlayerList) {

									if (player == null) {

										continue;

									}

									if (player.IsBot || player.Character == null) {

										continue;

									}

									var dist = Vector3D.Distance(cubeGrid.GetPosition(), player.GetPosition());

									if (dist < closestPlayerDistance || closestPlayerDistance == -1) {

										if (activeNPC.faction != null) {

											if (MyAPIGateway.Session.Factions.GetReputationBetweenPlayerAndFaction(player.IdentityId, activeNPC.faction.FactionId) <= -500) {

												continue;

											}

										}

										closestPlayerDistance = dist;

									}

								}

								if (activeNPC.RemoteControl.IsAutoPilotEnabled == false && activeNPC.SpawnGroup.PauseAutopilotAtPlayerDistance < closestPlayerDistance || closestPlayerDistance == -1) {

									activeNPC.RemoteControl.SetAutoPilotEnabled(true);

								}

								if (activeNPC.RemoteControl.IsAutoPilotEnabled == true && activeNPC.SpawnGroup.PauseAutopilotAtPlayerDistance > closestPlayerDistance && closestPlayerDistance > -1) {

									activeNPC.RemoteControl.SetAutoPilotEnabled(false);

								}

							} else {

								if (activeNPC.RemoteControl.IsAutoPilotEnabled == false && !activeNPC.CargoShipOverride) {

									Logger.AddMsg("Planetary Cargo Ship " + cubeGrid.CustomName + " Remote Control Autopilot Disabled. Ship Now Identified As \"Other\" NPC.");
									activeNPC.SpawnType = "Other";

									if (cubeGrid.Storage != null) {

										cubeGrid.Storage[GuidActiveNpcData] = activeNPC.ToString();

									}

								}

							}

						}

						if (skip == false) {

							double elevation = SpawnResources.GetDistanceFromSurface(cubeGrid.PositionComp.WorldAABB.Center, activeNPC.Planet);

							/*
							if(elevation > Settings.PlanetaryCargoShips.DespawnAltitude && skip == false){
								
								errorLogger.Append("Too High From Ground").AppendLine();
								Logger.AddMsg("Planetary Cargo Ship " + cubeGrid.CustomName + " Has Ascended Too High From Its Path And Will Be Despawned.");
								activeNPC.FlagForDespawn = true; 
								skip = true;
								
							}
							*/

							if (elevation < Settings.PlanetaryCargoShips.MinPathAltitude && skip == false/* && getElevation == true*/) {

								errorLogger.Append("Too Close To Ground").AppendLine();
								Logger.AddMsg("Planetary Cargo Ship " + cubeGrid.CustomName + " Altitude Lower Than Allowed Threshold. Ship Now Identified As \"Other\" NPC.");
								activeNPC.SpawnType = "Other";

								if (cubeGrid.Storage != null) {

									cubeGrid.Storage[GuidActiveNpcData] = activeNPC.ToString();

								}

								continue;

							}

						}

						var planetEntity = activeNPC.Planet as IMyEntity;
						var shipUpDir = Vector3D.Normalize(cubeGrid.GetPosition() - planetEntity.GetPosition());
						var coreDist = Vector3D.Distance(activeNPC.EndCoords, planetEntity.GetPosition());
						var pathCheckCoords = shipUpDir * coreDist + planetEntity.GetPosition();

						if (Vector3D.Distance(pathCheckCoords, activeNPC.EndCoords) < Settings.PlanetaryCargoShips.DespawnDistanceFromEndPath && skip == false) {

							errorLogger.Append("Reached Path End").AppendLine();
							Logger.AddMsg("Planetary Cargo Ship " + cubeGrid.CustomName + " Has Reached End Of Path And Will Be Despawned.");
							activeNPC.FlagForDespawn = true;
							activeNPC.DespawnReason = "EndPath";
							skip = true;

						}

						if (activeNPC.FlagForDespawn == true) {

							var player = SpawnResources.GetNearestPlayer(cubeGrid.GetPosition());

							if (player == null || Vector3D.Distance(cubeGrid.GetPosition(), player.GetPosition()) > Settings.PlanetaryCargoShips.DespawnDistanceFromPlayer) {

								errorLogger.Append("Despawning Grid").AppendLine();
								Logger.AddMsg("NPC Cargo Ship " + cubeGrid.CustomName + " Has Been Despawned.");
								activeNPC.ActivateDespawnActions();
								ActiveNPCs.Remove(cubeGrid);
								DeleteGrid(cubeGrid);
								continue;

							}

						}

					} catch (Exception exc) {

						Logger.AddMsg("Unexpected exception in Planetary Cargo Ship Distance Checker");

					}

				}

				//Random Encounters
				if (activeNPC.SpawnType == "RandomEncounter") {

					try {

						if (activeNPC.FlagForDespawn == true) {

							var player = SpawnResources.GetNearestPlayer(cubeGrid.GetPosition());

							if (player == null || Vector3D.Distance(cubeGrid.GetPosition(), player.GetPosition()) > Settings.RandomEncounters.DespawnDistanceFromPlayer) {

								Logger.AddMsg("NPC Random Encounter " + cubeGrid.CustomName + " Has Been Despawned.");
								activeNPC.ActivateDespawnActions();
								ActiveNPCs.Remove(cubeGrid);
								DeleteGrid(cubeGrid);
								continue;

							}

						}

					} catch (Exception exc) {

						Logger.AddMsg("Unexpected exception in Random Encounters Distance Checker");

					}

				}

				//Boss Encounters
				if (activeNPC.SpawnType == "BossEncounter") {

					try {

						if (activeNPC.FlagForDespawn == true) {

							var player = SpawnResources.GetNearestPlayer(cubeGrid.GetPosition());

							if (player == null || Vector3D.Distance(cubeGrid.GetPosition(), player.GetPosition()) > Settings.BossEncounters.DespawnDistanceFromPlayer) {

								Logger.AddMsg("NPC Boss Encounter " + cubeGrid.CustomName + " Has Been Despawned.");
								activeNPC.ActivateDespawnActions();
								ActiveNPCs.Remove(cubeGrid);
								DeleteGrid(cubeGrid);
								continue;

							}

						}

					} catch (Exception exc) {

						Logger.AddMsg("Unexpected exception in Boss Encounters Distance Checker");

					}

				}

				//Planetary Installations
				if (activeNPC.SpawnType == "PlanetaryInstallation") {

					try {

						if (activeNPC.FlagForDespawn == true) {

							var player = SpawnResources.GetNearestPlayer(cubeGrid.GetPosition());

							if (player == null || Vector3D.Distance(cubeGrid.GetPosition(), player.GetPosition()) > Settings.PlanetaryInstallations.DespawnDistanceFromPlayer) {

								Logger.AddMsg("NPC Planetary Installation " + cubeGrid.CustomName + " Has Been Despawned.");
								activeNPC.ActivateDespawnActions();
								ActiveNPCs.Remove(cubeGrid);
								DeleteGrid(cubeGrid);
								continue;

							}

						}

					} catch (Exception exc) {

						Logger.AddMsg("Unexpected exception in Planetary Installations Distance Checker");

					}

				}

				//Other NPCs
				if (activeNPC.SpawnType == "Other") {

					try {

						if (activeNPC.FlagForDespawn == true) {

							var player = SpawnResources.GetNearestPlayer(cubeGrid.GetPosition());

							if (player == null || Vector3D.Distance(cubeGrid.GetPosition(), player.GetPosition()) > Settings.OtherNPCs.DespawnDistanceFromPlayer) {

								Logger.AddMsg("NPC Grid " + cubeGrid.CustomName + " Has Been Despawned.");
								activeNPC.ActivateDespawnActions();
								ActiveNPCs.Remove(cubeGrid);
								DeleteGrid(cubeGrid);
								continue;

							}

						}

					} catch (Exception exc) {

						Logger.AddMsg("Unexpected exception in Other NPCs Distance Checker");

					}

				}

				ActiveNPCs[cubeGrid] = activeNPC;

			}

		}

		public static void InitFactionData() {

			//Get NPC Faction Data
			var allFactions = MyAPIGateway.Session.Factions.Factions;

			foreach (var faction in allFactions.Keys) {

				var thisFaction = allFactions[faction];
				bool foundHuman = false;

				foreach (var id in thisFaction.Members.Keys) {

					if (MyAPIGateway.Players.TryGetSteamId(thisFaction.Members[id].PlayerId) > 0) {

						Logger.AddMsg("Faction " + thisFaction.Tag + " has human with Steam ID " + MyAPIGateway.Players.TryGetSteamId(thisFaction.Members[id].PlayerId).ToString(), true);
						foundHuman = true;
						break;

					}

				}

				if (foundHuman == true) {

					continue;

				}

				NPCFactionFounders.Add(thisFaction.FounderId);
				NPCFactionTags.Add(thisFaction.Tag);

				if (NPCFactionTagToFounder.ContainsKey(thisFaction.Tag) == false) {

					NPCFactionTagToFounder.Add(thisFaction.Tag, thisFaction.FounderId);

				}

				if (NPCFactionFounderToTag.ContainsKey(thisFaction.FounderId) == false) {

					NPCFactionFounderToTag.Add(thisFaction.FounderId, thisFaction.Tag);

				}

			}

			NPCFactionFounders.Add(0);
			NPCFactionTags.Add("Nobody");

			if (NPCFactionFounderToTag.ContainsKey(0) == false) {

				NPCFactionFounderToTag.Add(0, "Nobody");

			}

			if (NPCFactionTagToFounder.ContainsKey("Nobody") == false) {

				NPCFactionTagToFounder.Add("Nobody", 0);

			}

		}

		public static bool IsEconomyStation(IMyCubeGrid cubeGrid) {

			if (cubeGrid == null || MyAPIGateway.Entities.Exist(cubeGrid) == false) {

				return false;

			}

			if (SpawnResources.IsPositionInSafeZone(cubeGrid.GetPosition()) == false) {

				return false;

			}

			var blockList = new List<IMySlimBlock>();
			cubeGrid.GetBlocks(blockList);
			var gotStore = false;
			var gotContract = false;

			foreach (var block in blockList) {

				if (block.BlockDefinition.Id.TypeId == typeof(MyObjectBuilder_StoreBlock) == true) {

					gotStore = true;
					continue;

				}

				if (block.BlockDefinition.Id.TypeId == typeof(MyObjectBuilder_ContractBlock) == true) {

					gotContract = true;

				}

			}

			if (gotStore == false || gotContract == false) {

				return false;

			}

			return true;

		}

		public static bool IsGridNameEconomyPattern(string gridName) {

			if (gridName.StartsWith("Economy_MiningStation_") == true) {

				return true;

			}

			if (gridName.StartsWith("Economy_OrbitalStation_") == true) {

				return true;

			}

			if (gridName.StartsWith("Economy_Outpost_") == true) {

				return true;

			}

			if (gridName.StartsWith("Economy_SpaceStation_") == true) {

				return true;

			}

			var nameSplit = gridName.Split(' ');
			var stationTypes = new List<string>();
			stationTypes.Add("MiningStation");
			stationTypes.Add("OrbitalStation");
			stationTypes.Add("Outpost");
			stationTypes.Add("SpaceStation");

			if (nameSplit.Length < 3) {

				return false;

			}

			if (NPCFactionTagToFounder.ContainsKey(nameSplit[0]) == false) {

				return false;

			}

			if (stationTypes.Contains(nameSplit[1]) == false) {

				return false;

			}

			long stationId = 0;

			if (long.TryParse(nameSplit[2], out stationId) == false) {

				return false;

			}

			return true;

		}

		public static IMyEntity MakeDummyContainer() {

			var randomDir = MyUtils.GetRandomVector3D();
			var randomSpawn = randomDir * 10000000;
			var prefab = MyDefinitionManager.Static.GetPrefabDefinition("MES-Dummy-Container");
			var gridOB = prefab.CubeGrids[0];
			gridOB.PositionAndOrientation = new MyPositionAndOrientation(randomSpawn, Vector3.Forward, Vector3.Up);
			MyAPIGateway.Entities.RemapObjectBuilder(gridOB);
			var entity = MyAPIGateway.Entities.CreateFromObjectBuilderAndAdd(gridOB);
			return entity;

		}

		/*public static void TestOwnerChange(IMyCubeGrid cubeGrid){
			
			Logger.AddMsg("Grid Ownership Change: " + cubeGrid.CustomName, true);
				
			foreach(var owner in cubeGrid.BigOwners){
				
				Logger.AddMsg(owner.ToString(), true);
				
			}
			
		}*/

		public static void NewEntityDetected(IMyEntity entity) {

			var cubeGrid = entity as IMyCubeGrid;

			if (cubeGrid == null) {

				return;

			}

			if (cubeGrid.Physics == null) {

				return;

			}

			if (Logger.LoggerDebugMode == true) {

				/*
				Logger.AddMsg("Grid Ownership At Spawn: " + cubeGrid.CustomName, true);
				
				foreach(var owner in cubeGrid.BigOwners){
					
					Logger.AddMsg(owner.ToString(), true);
					
				}
				
				cubeGrid.OnBlockOwnershipChanged += TestOwnerChange;
				*/
			}

			Logger.AddMsg("New Grid Detected. Name: " + cubeGrid.CustomName + ". Static: " + cubeGrid.IsStatic.ToString(), true);

			int closestIndex = -1;
			double closestDist = -1;

			//Container Check (Unknown Signal) - Do Something More Robust Later
			if (DropContainerNames.Contains(cubeGrid.CustomName) == true) {

				return;

			}

			for (int i = 0; i < PendingNPCs.Count; i++) {

				//Named Grid - No Previous
				if (PendingNPCs[i].GridName == cubeGrid.CustomName && closestDist == -1) {

					closestDist = Vector3D.Distance(PendingNPCs[i].CurrentCoords, cubeGrid.GetPosition());
					closestIndex = i;
					continue;

				}

				//Named Grid - Closer Eligible
				if (PendingNPCs[i].GridName == cubeGrid.CustomName && Vector3D.Distance(PendingNPCs[i].CurrentCoords, cubeGrid.GetPosition()) < closestDist) {

					closestDist = Vector3D.Distance(PendingNPCs[i].CurrentCoords, cubeGrid.GetPosition());
					closestIndex = i;
					continue;

				}

				//Mismatch Grid Names - Lookin' at you, Keen + Default Cargo Ships >:(


			}

			//Temp Debug
			var myCubeGrid = cubeGrid as MyCubeGrid;
			Logger.AddMsg(cubeGrid.CustomName + " Dampeners First Check: " + myCubeGrid.DampenersEnabled, true);


			if (closestIndex >= 0) {

				PendingNPCs[closestIndex].CubeGrid = cubeGrid;

				/*
				if(PendingNPCs[closestIndex].SpawnType == "PlanetaryCargoShip" && string.IsNullOrWhiteSpace(PendingNPCs[closestIndex].KeenAiName) == false) {

					PendingNPCs[closestIndex].SpawnType = "Other";

				}
				*/

				if (ActiveNPCs.ContainsKey(cubeGrid) == false) {

					if (PendingNPCs[closestIndex].SpawnType == "PlanetaryCargoShip" || PendingNPCs[closestIndex].SpawnGroup.UseAutoPilotInSpace == true) {

						SetThrusterPowerConsumption(cubeGrid, 0.1f);

						try {

							var gts = MyAPIGateway.TerminalActionsHelper.GetTerminalSystemForGrid(cubeGrid);
							var blockList = new List<IMyRemoteControl>();
							gts.GetBlocksOfType(blockList);
							IMyRemoteControl remoteControl = null;

							foreach (var block in blockList) {

								if (block.IsFunctional == true) {

									remoteControl = block;

									if (block.IsMainCockpit == true || RivalAIHelper.RivalAiControlModules.Contains(block.SlimBlock.BlockDefinition.Id.SubtypeName)) {

										remoteControl = block;
										break;

									}

								}

							}

							if (remoteControl == null) {

								if (PendingNPCs[closestIndex].SpawnType != "SpaceCargoShip") {

									PendingNPCs[closestIndex].SpawnType = "Other";

								}

							} else {

								remoteControl.ClearWaypoints();
								remoteControl.AddWaypoint(PendingNPCs[closestIndex].EndCoords, "Destination");
								remoteControl.SpeedLimit = PendingNPCs[closestIndex].AutoPilotSpeed;
								remoteControl.FlightMode = Sandbox.ModAPI.Ingame.FlightMode.OneWay;
								remoteControl.SetAutoPilotEnabled(true);
								PendingNPCs[closestIndex].RemoteControl = remoteControl;
								gts.GetBlocksOfType(PendingNPCs[closestIndex].HydrogenTanks);
								gts.GetBlocksOfType(PendingNPCs[closestIndex].GasGenerators);

							}

						} catch (Exception exc) {

							Logger.AddMsg("Something went wrong with Planetary Cargo Ship Spawn.");

						}

					} else {

						//Planetary Cargo Ships Cannot Be Static
						if (cubeGrid.IsStatic == false && PendingNPCs[closestIndex].ForceStaticGrid == true) {

							cubeGrid.IsStatic = true;

						}

					}

					if (IsGridNameEconomyPattern(cubeGrid.CustomName) == true) {

						Logger.AddMsg(cubeGrid.CustomName + " is Keen Economy Station and will not be cleaned by MES.");
						PendingNPCs[closestIndex].CleanupIgnore = true;

					}

					ActiveNPCs.Add(cubeGrid, PendingNPCs[closestIndex]);

				}

				PendingNPCs.RemoveAt(closestIndex);


			} else {

				var activeNPC = CheckIfGridWasActiveNPC(cubeGrid);

				if (activeNPC != new ActiveNPC()) {

					activeNPC.CubeGrid = cubeGrid;


				} else {

					activeNPC.Name = cubeGrid.CustomName;
					activeNPC.CubeGrid = cubeGrid;
					activeNPC.StartCoords = cubeGrid.GetPosition();
					activeNPC.EndCoords = cubeGrid.GetPosition();
					activeNPC.SpawnType = "UnknownSource";

					var planet = SpawnResources.GetNearestPlanet(cubeGrid.GetPosition());

					if (planet != null) {

						if (SpawnResources.IsPositionInGravity(cubeGrid.GetPosition(), planet) == true) {

							activeNPC.Planet = planet;

						}

					}

				}

				if (ActiveNPCs.ContainsKey(cubeGrid) == false) {

					if (IsGridNameEconomyPattern(cubeGrid.CustomName) == true) {

						Logger.AddMsg(cubeGrid.CustomName + " is Keen Economy Station and will not be cleaned by MES.");
						activeNPC.CleanupIgnore = true;

					}

					ActiveNPCs.Add(cubeGrid, activeNPC);

				}

			}

			//Init Entity Storage
			if (cubeGrid.Storage == null) {

				cubeGrid.Storage = new MyModStorageComponent();

			}

			//Active Npc Data
			if (cubeGrid.Storage.ContainsKey(GuidActiveNpcData) == false) {

				cubeGrid.Storage.Add(GuidActiveNpcData, ActiveNPCs[cubeGrid].ToString());

			} else {

				cubeGrid.Storage[GuidActiveNpcData] = ActiveNPCs[cubeGrid].ToString();

			}

			if (ActiveNPCs[cubeGrid].SpawnType == "SpaceCargoShip") {

				CargoShipWatcher.NewlySpawnedGridSpeedCheck.Add(cubeGrid);

			}

			NpcOwnershipCheckTimer = 2;
			NpcBlacklistCheckTimer = Settings.General.NpcBlacklistCheckTimerTrigger;

		}

		public static bool NpcOwnershipCheck(IMyCubeGrid cubeGrid, bool printOwnersToLog = false, bool skipActiveNPCList = false) {

			if (cubeGrid == null) {

				return false;

			}

			if (ActiveNPCs.ContainsKey(cubeGrid) == false && skipActiveNPCList == false) {

				return false;

			}

			string type = "";

			if (cubeGrid.Storage != null) {

				if (cubeGrid.Storage.ContainsKey(GuidSpawnType) == true) {

					type = cubeGrid.Storage[GuidSpawnType];

				}

			}

			var gridGroups = MyAPIGateway.GridGroups.GetGroup(cubeGrid, GridLinkTypeEnum.Physical);

			if (gridGroups.Contains(cubeGrid) == false) {

				gridGroups.Add(cubeGrid);

			}

			var ownerList = new List<long>();

			foreach (var grid in gridGroups) {

				foreach (var owner in grid.BigOwners) {

					if (ownerList.Contains(owner) == false) {

						ownerList.Add(owner);

					}

				}

				foreach (var owner in grid.SmallOwners) {

					if (ownerList.Contains(owner) == false) {

						ownerList.Add(owner);

					}

				}

			}

			bool foundNpcOwner = false;
			bool foundHumanOwner = false;

			var ownerSB = new StringBuilder();
			ownerSB.Append("Owners Of Grid (Includes Subgrids): ").Append(cubeGrid.CustomName).Append(" / ").Append(cubeGrid.EntityId.ToString()).Append(" /// ");

			foreach (var owner in ownerList) {

				if (NPCFactionFounders.Contains(owner) == true && owner != 0) {

					if (printOwnersToLog == true) {

						if (NPCFactionFounderToTag.ContainsKey(owner) == true) {

							ownerSB.Append("[NPC Owner: ").Append(NPCFactionFounderToTag[owner]).Append("/").Append(owner.ToString()).Append("] /// ");

						} else {

							ownerSB.Append("[NPC Owner: ").Append("UnknownFaction").Append("/").Append(owner.ToString()).Append("] /// ");

						}

					}

					foundNpcOwner = true;
					continue;

				}

				if (NPCFactionFounders.Contains(owner) == false && owner != 0) {

					if (printOwnersToLog == true) {

						var faction = MyAPIGateway.Session.Factions.TryGetPlayerFaction(owner);

						if (faction != null) {

							ownerSB.Append("[Human Owner: ").Append(faction.Tag).Append("/").Append(owner.ToString()).Append("] /// ");

						} else {

							ownerSB.Append("[Human Owner: ").Append("NoFaction").Append("/").Append(owner.ToString()).Append("] /// ");

						}

					}

					foundHumanOwner = true;
					break;

				}

				if (owner == 0 && printOwnersToLog == true) {

					ownerSB.Append("[No Owner: ").Append("NoFaction").Append("/").Append("0").Append("] /// ");

				}

			}

			if (printOwnersToLog == true) {

				Logger.AddMsg(ownerSB.ToString());

			}

			if (foundHumanOwner == true) {

				return false;

			}

			if (foundNpcOwner == false && foundHumanOwner == false && type == "UnknownSource") {

				return false;

			}

			if (type == "UnknownSource") {

				cubeGrid.Storage[GuidSpawnType] = "Other";
				ActiveNPCs[cubeGrid].SpawnType = "Other";

			}

			return true;

		}

		public static void RefreshBlockSubparts(IMyCubeGrid cubeGrid) {

			cubeGrid.IsStatic = true;

			var gts = MyAPIGateway.TerminalActionsHelper.GetTerminalSystemForGrid(cubeGrid);
			var blockList = new List<IMyTerminalBlock>();
			gts.GetBlocksOfType<IMyTerminalBlock>(blockList);

			foreach (var block in blockList) {

				if (block as IMyLargeTurretBase != null) {

					//block.Init();
					/*
					var turret = block as IMyLargeTurretBase;
					var enabledState = turret.Enabled;
					turret.Enabled = true;
					turret.SyncAzimuth();
					turret.SyncElevation();
					turret.Enabled = enabledState;
					*/

				}

				if (block as IMyDoor != null) {

					//block.Init();
					/*
					var door = block as IMyDoor;
					var enabledState = door.Enabled;
					door.Enabled = true;
					door.ToggleDoor();
					door.ToggleDoor();
					door.Enabled = enabledState;
					*/

				}

			}

			cubeGrid.IsStatic = false;

		}

		public static void RemoveGUIDs(IMyCubeGrid cubeGrid) {

			if (cubeGrid == null) {

				return;

			}

			var gridGroups = MyAPIGateway.GridGroups.GetGroup(cubeGrid, GridLinkTypeEnum.Mechanical);

			if (gridGroups.Contains(cubeGrid) == false) {

				gridGroups.Add(cubeGrid);

			}

			foreach (var grid in gridGroups) {

				if (grid.Storage == null) {

					continue;

				}

				if (grid.Storage.ContainsKey(GuidStartCoords) == true) {

					grid.Storage.Remove(GuidStartCoords);

				}

				if (grid.Storage.ContainsKey(GuidEndCoords) == true) {

					grid.Storage.Remove(GuidEndCoords);

				}

				if (grid.Storage.ContainsKey(GuidSpawnType) == true) {

					grid.Storage.Remove(GuidSpawnType);

				}

				if (grid.Storage.ContainsKey(GuidCleanupTimer) == true) {

					grid.Storage.Remove(GuidCleanupTimer);

				}

				if (grid.Storage.ContainsKey(GuidIgnoreCleanup) == true) {

					grid.Storage.Remove(GuidIgnoreCleanup);

				}

				if (grid.Storage.ContainsKey(GuidActiveNpcData) == true) {

					grid.Storage.Remove(GuidActiveNpcData);

				}

			}

		}

		public static void CutVoxelsAtAirtightPositions(IMyCubeGrid cubeGrid) {

			Logger.VoxelCutLogging("Beginning Voxel Cut Process At CubeGrid:", true);
			var min = cubeGrid.Min;
			var max = cubeGrid.Max;
			var sphere = new BoundingSphereD(cubeGrid.GetPosition(), 1000);
			var airtightCells = new List<Vector3I>();

			Logger.VoxelCutLogging(" - Collecting List Of AirTight Cells");
			for (int x = min.X; x <= max.X; x++) {

				for (int y = min.Y; y <= max.Y; y++) {

					for (int z = min.Z; z <= max.Z; z++) {

						var checkCell = new Vector3I(x, y, z);

						if (cubeGrid.IsRoomAtPositionAirtight(checkCell) == true) {

							airtightCells.Add(checkCell);

						}

					}

				}

			}

			Logger.VoxelCutLogging(" - Total Airtight Cells: " + airtightCells.ToString());
			Logger.VoxelCutLogging(" - Queueing Airtight Cells That Require Removal");
			foreach (var cell in airtightCells) {

				var cellWorldSpace = cubeGrid.GridIntegerToWorld(cell);
				var voxelTool = MyAPIGateway.Session.VoxelMaps.GetBoxVoxelHand();
				voxelTool.Boundaries = new BoundingBoxD(new Vector3D(-1.3, -1.3, -1.3), new Vector3D(1.3, 1.3, 1.3));
				MES_SessionCore.RemoveVoxels.Add(MatrixD.CreateWorld(cellWorldSpace, cubeGrid.WorldMatrix.Forward, cubeGrid.WorldMatrix.Up));

			}

			Logger.VoxelCutLogging(" - Cells Queued, Removal Activated.");
			MES_SessionCore.VoxelsToRemove = true;

		}

		public static void SpawnedVoxelCheck() {

			SpawnedVoxelCheckTimer = Settings.General.SpawnedVoxelCheckTimerTrigger;
			bool listModified = false;
			SpawnResources.RefreshEntityLists();

			foreach (var voxelId in SpawnedVoxels.Keys.ToList()) {

				if (SpawnedVoxels[voxelId] == null || MyAPIGateway.Entities.Exist(SpawnedVoxels[voxelId]) == false) {

					listModified = true;
					SpawnedVoxels.Remove(voxelId);
					continue;

				}

				bool closeGrid = false;

				foreach (var entity in SpawnResources.EntityList) {

					if (entity as IMyCubeGrid == null && entity as IMyCharacter == null) {

						continue;

					}

					if (Vector3D.Distance(entity.GetPosition(), SpawnedVoxels[voxelId].GetPosition()) < Settings.General.SpawnedVoxelMinimumGridDistance) {

						closeGrid = true;
						break;

					}

				}

				if (closeGrid == true) {

					continue;

				}

				Logger.AddMsg("Removed Voxels Spawned From NPC At Coords " + SpawnedVoxels[voxelId].GetPosition().ToString() + ". No Grids Within Range.");
				SpawnedVoxels[voxelId].Delete();
				SpawnedVoxels.Remove(voxelId);
				listModified = true;

			}

			if (listModified == true) {

				var voxelIdList = new List<string>(SpawnedVoxels.Keys.ToList());
				string[] voxelIdArray = voxelIdList.ToArray();
				MyAPIGateway.Utilities.SetVariable("MES-SpawnedVoxels", voxelIdArray);

			}

		}

		public static void StartupScan() {

			SpawnResources.RefreshEntityLists();

			foreach (var entity in SpawnResources.EntityList) {

				var cubeGrid = entity as IMyCubeGrid;

				if (cubeGrid == null) {

					continue;

				}

				if (cubeGrid.Physics == null) {

					continue;

				}

				if (DropContainerNames.Contains(cubeGrid.CustomName) == true) {

					continue;

				}

				if (ActiveNPCs.ContainsKey(cubeGrid) == true) {

					continue;

				}

				//Check For NPC by Spawner Tags
				var activeNPC = CheckIfGridWasActiveNPC(cubeGrid);
				activeNPC.VoxelCut = true; //No Matter What, This Check Shouldn't Happen At Startup

				if (activeNPC.StartupScanValid == true) {

					if (NpcOwnershipCheck(cubeGrid, true, true) == true) {

						Logger.SkipNextMessage = true;
						Logger.AddMsg(cubeGrid.CustomName + " / " + cubeGrid.EntityId.ToString() + " Identified as an NPC Grid: " + activeNPC.SpawnType);

						if (IsGridNameEconomyPattern(cubeGrid.CustomName) == true) {

							Logger.AddMsg(cubeGrid.CustomName + " is Keen Economy Station and will not be cleaned by MES.");
							activeNPC.CleanupIgnore = true;

						}

						ActiveNPCs.Add(cubeGrid, activeNPC);

						if (activeNPC.SpawnType == "PlanetaryCargoShip") {

							SetThrusterPowerConsumption(cubeGrid, 0.1f);

						}

						continue;

					} else {

						Logger.SkipNextMessage = true;
						Logger.AddMsg(cubeGrid.CustomName + " / " + cubeGrid.EntityId.ToString() + " Identified as a Former NPC Grid, but Failed Ownership Check.");
						RemoveGUIDs(cubeGrid);

					}

					continue;

				}

				if (NpcOwnershipCheck(cubeGrid, true, true) == true) {

					if (IsGridNameEconomyPattern(cubeGrid.CustomName) == true) {

						Logger.AddMsg(cubeGrid.CustomName + " is Keen Economy Station and will not be cleaned by MES.");
						activeNPC.CleanupIgnore = true;

					}

					Logger.SkipNextMessage = true;
					Logger.AddMsg(cubeGrid.CustomName + " / " + cubeGrid.EntityId.ToString() + " Identified as an UnknownSource NPC Grid");
					activeNPC = new ActiveNPC();
					activeNPC.Name = cubeGrid.CustomName;
					activeNPC.SpawnType = "UnknownSource";
					activeNPC.GridName = cubeGrid.CustomName;
					activeNPC.CubeGrid = cubeGrid;
					activeNPC.CleanupTime = 0;
					ActiveNPCs.Add(cubeGrid, activeNPC);
					SetThrusterPowerConsumption(cubeGrid, 0.1f);

				}

			}

		}

		public static void SetThrusterPowerConsumption(IMyCubeGrid cubeGrid, float newPowerRatio) {

			try {

				var gts = MyAPIGateway.TerminalActionsHelper.GetTerminalSystemForGrid(cubeGrid);
				var blockList = new List<IMyThrust>();
				gts.GetBlocksOfType(blockList);

				foreach (var thrust in blockList) {

					thrust.PowerConsumptionMultiplier = newPowerRatio;

				}

			} catch (Exception exc) {

				Logger.AddMsg("Error Detected In SetThrusterPowerConsumption Method:");
				Logger.AddMsg(exc.ToString());

			}


		}

		public static ActiveNPC CheckIfGridWasActiveNPC(IMyCubeGrid cubeGrid) {

			var activeNPC = new ActiveNPC();
			bool gotNpcDataStorage = false;

			if (cubeGrid.Storage != null) {

				string activeNpcDataString = "";

				if (cubeGrid.Storage.TryGetValue(GuidActiveNpcData, out activeNpcDataString) == true) {

					activeNPC = new ActiveNPC(activeNpcDataString);

					if (activeNPC.ModStorageRetrieveFail == false && string.IsNullOrWhiteSpace(activeNpcDataString) == false) {

						Logger.AddMsg("Got Serialized NPC Data From " + cubeGrid.CustomName);
						Logger.AddMsg("SpawnType: " + activeNPC.SpawnType);
						activeNPC.StartupScanValid = true;
						gotNpcDataStorage = true;

					}


				}

				if (cubeGrid.Storage.ContainsKey(GuidSpawnType) == true || gotNpcDataStorage == true) {

					if (gotNpcDataStorage == false) {

						activeNPC.StartupScanValid = true;
						activeNPC.Name = cubeGrid.CustomName;
						activeNPC.GridName = cubeGrid.CustomName;
						activeNPC.CubeGrid = cubeGrid;
						activeNPC.SpawnType = cubeGrid.Storage[GuidSpawnType];
						activeNPC.KeenBehaviorCheck = true;
						activeNPC.FixTurrets = true;
						activeNPC.DisabledBlocks = true;
						activeNPC.ReplacedWeapons = true;
						activeNPC.AddedCrew = true;
						activeNPC.ReplenishedSystems = true;
						activeNPC.StoreBlocksInit = true;

						if (cubeGrid.Storage.ContainsKey(GuidStartCoords) == true) {

							var StartCoords = cubeGrid.GetPosition();
							Vector3D.TryParse(cubeGrid.Storage[GuidStartCoords], out StartCoords);
							activeNPC.StartCoords = StartCoords;

						}

						if (cubeGrid.Storage.ContainsKey(GuidEndCoords) == true) {

							var EndCoords = cubeGrid.GetPosition();
							Vector3D.TryParse(cubeGrid.Storage[GuidEndCoords], out EndCoords);
							activeNPC.EndCoords = EndCoords;

						}

						if (cubeGrid.Storage.ContainsKey(GuidCleanupTimer) == true) {

							int timer = 0;
							int.TryParse(cubeGrid.Storage[GuidCleanupTimer], out timer);
							activeNPC.CleanupTime = timer;

						}

						if (cubeGrid.Storage.ContainsKey(GuidIgnoreCleanup) == true) {

							var cleanIgnore = false;
							bool.TryParse(cubeGrid.Storage[GuidIgnoreCleanup], out cleanIgnore);
							activeNPC.CleanupIgnore = cleanIgnore;

						}

						if (cubeGrid.Storage.ContainsKey(GuidWeaponsReplaced) == true) {

							var weaponsReplaced = false;
							bool.TryParse(cubeGrid.Storage[GuidWeaponsReplaced], out weaponsReplaced);
							activeNPC.ReplacedWeapons = weaponsReplaced;

						}

					}

					if (activeNPC.SpawnType == "SpaceCargoShip") {

						if (activeNPC.SpawnGroup != null) {

							if (activeNPC.SpawnGroup.UseAutoPilotInSpace == true) {

								var gts = MyAPIGateway.TerminalActionsHelper.GetTerminalSystemForGrid(cubeGrid);
								var blockList = new List<IMyRemoteControl>();
								gts.GetBlocksOfType(blockList);

								foreach (var block in blockList) {

									if (block.IsFunctional == true) {

										if (block.IsAutoPilotEnabled == true) {

											activeNPC.RemoteControl = block;

										} else {

											var ob = (MyObjectBuilder_RemoteControl)block.SlimBlock.GetObjectBuilder();

											if (ob != null) {

												if (ob.CurrentWaypointIndex > -1 && ob.Waypoints.Count - 1 >= ob.CurrentWaypointIndex) {

													if (Vector3D.Distance(activeNPC.EndCoords, ob.Waypoints[ob.CurrentWaypointIndex].Coords) <= 5) {

														activeNPC.RemoteControl = block;
														break;

													}

												}

											}

										}

									}

								}

							}

						}

					}

					if (activeNPC.SpawnType == "PlanetaryCargoShip") {

						var gts = MyAPIGateway.TerminalActionsHelper.GetTerminalSystemForGrid(cubeGrid);
						var blockList = new List<IMyRemoteControl>();
						gts.GetBlocksOfType(blockList);

						foreach (var block in blockList) {

							if (block.IsFunctional == true) {

								if (block.IsAutoPilotEnabled == true) {

									activeNPC.RemoteControl = block;
									break;

								} else {

									var ob = (MyObjectBuilder_RemoteControl)block.SlimBlock.GetObjectBuilder();

									if (ob != null) {

										if (ob.CurrentWaypointIndex > -1 && ob.Waypoints.Count - 1 >= ob.CurrentWaypointIndex) {

											if (Vector3D.Distance(activeNPC.EndCoords, ob.Waypoints[ob.CurrentWaypointIndex].Coords) <= 5) {

												activeNPC.RemoteControl = block;
												break;

											}

										}

									}

								}

							}

						}

						gts.GetBlocksOfType(activeNPC.HydrogenTanks);
						gts.GetBlocksOfType(activeNPC.GasGenerators);

					}

					if (activeNPC.SpawnType != "Other") {

						var planet = SpawnResources.GetNearestPlanet(cubeGrid.GetPosition());

						if (planet != null) {

							if (SpawnResources.IsPositionInGravity(cubeGrid.GetPosition(), planet) == true) {

								activeNPC.Planet = planet;

							}

						}

					}

				}

			}

			return activeNPC;

		}

	}

}