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
using ModularEncountersSpawner.Templates;
using ModularEncountersSpawner.Spawners;

namespace ModularEncountersSpawner{

	public static class Cleanup{

		public static void CleanupProcess(bool singleCheckParameters = false){
			
			MES_SessionCore.PlayerList.Clear();
			MyAPIGateway.Players.GetPlayers(MES_SessionCore.PlayerList);
			var grids = new List<IMyCubeGrid>(NPCWatcher.ActiveNPCs.Keys.ToList());
			
			foreach(var cubeGrid in grids){
				
				if(NPCWatcher.ActiveNPCs.ContainsKey(cubeGrid) == false){
					
					continue;
					
				}
				
				if(cubeGrid == null){
					
					NPCWatcher.ActiveNPCs.Remove(cubeGrid);
					
				}

				if(NPCWatcher.ActiveNPCs[cubeGrid].CleanupIgnore == true) {

					continue;

				} else {

					var gridGroups = MyAPIGateway.GridGroups.GetGroup(cubeGrid, GridLinkTypeEnum.Mechanical);
					bool ignore = false;

					foreach(var grid in gridGroups) {

						if(NPCWatcher.ActiveNPCs.ContainsKey(grid) == true) {

							if(NPCWatcher.ActiveNPCs[grid].CleanupIgnore == true) {

								ignore = true;
								break;

							}

						}

					}

					if(ignore == true) {

						continue;

					}

				}

				if(HasLegacyIgnoreTag(cubeGrid) == true){
						
					continue;
						
				}
				
				if(NPCWatcher.ActiveNPCs[cubeGrid].SpawnType == "Other"){
					
					if(IsSubgridForNormalEncounter(cubeGrid) == true){
						
						continue;
						
					}
					
				}
				
				var cleanSettings = GetCleaningSettingsForType(NPCWatcher.ActiveNPCs[cubeGrid].SpawnType);
				
				if(cleanSettings.UseCleanupSettings == false){
					
					continue;
					
				}
				
				var powered = IsGridPowered(cubeGrid);
				
				if(singleCheckParameters == false){
					
					var outsideDistance = IsDistanceFurtherThanPlayers(cleanSettings, cubeGrid.GetPosition(), powered);
					
					if(outsideDistance == true && cleanSettings.CleanupDistanceStartsTimer == false){
						
						Logger.AddMsg("Cleanup: " + NPCWatcher.ActiveNPCs[cubeGrid].SpawnType + "/" + cubeGrid.CustomName + " Is Further Than Allowed Distance From Player. Grid Marked For Despawn.");
						NPCWatcher.ActiveNPCs[cubeGrid].FlagForDespawn = true;
						continue;
						
					}
					
					var timerIsExpired = TimerExpired(cubeGrid, cleanSettings, outsideDistance, powered);
										
					if(timerIsExpired == true){
						
						Logger.AddMsg("Cleanup: " + NPCWatcher.ActiveNPCs[cubeGrid].SpawnType + "/" + cubeGrid.CustomName + " Timer Has Expired. Grid Marked For Despawn.");
						NPCWatcher.ActiveNPCs[cubeGrid].FlagForDespawn = true;
						continue;
						
					}else{
						
						if(cleanSettings.CleanupDistanceStartsTimer == true && cleanSettings.CleanupResetTimerWithinDistance == true && outsideDistance == false){
							
							NPCWatcher.ActiveNPCs[cubeGrid].CleanupTime = 0;
							
							if(cubeGrid.Storage != null){
								
								if(cubeGrid.Storage.ContainsKey(NPCWatcher.GuidCleanupTimer) == false){
				
									cubeGrid.Storage.Add(NPCWatcher.GuidCleanupTimer, NPCWatcher.ActiveNPCs[cubeGrid].CleanupTime.ToString());
									
								}else{
									
									cubeGrid.Storage[NPCWatcher.GuidCleanupTimer] = NPCWatcher.ActiveNPCs[cubeGrid].CleanupTime.ToString();
									
								}
								
							}
							
						}
						
					}
					
				}
				
				if(singleCheckParameters == true){
					
					if(NPCWatcher.ActiveNPCs[cubeGrid].CheckedBlockCount == false && cleanSettings.CleanupUseBlockLimit == true){
						
						NPCWatcher.ActiveNPCs[cubeGrid].CheckedBlockCount = true;
						var blockList = new List<IMySlimBlock>();
						cubeGrid.GetBlocks(blockList);
						
						if(blockList.Count > cleanSettings.CleanupBlockLimitTrigger){
							
							NPCWatcher.ActiveNPCs[cubeGrid].FlagForDespawn = true;
							
						}
						
					}
					
					if(NPCWatcher.ActiveNPCs[cubeGrid].DisabledBlocks == false && cleanSettings.UseBlockDisable == true){
						
						NPCWatcher.ActiveNPCs[cubeGrid].DisabledBlocks = true;
						/*
						var gts = MyAPIGateway.TerminalActionsHelper.GetTerminalSystemForGrid(cubeGrid);
						List<IMyFunctionalBlock> blockList = new List<IMyFunctionalBlock>();
						gts.GetBlocksOfType<IMyFunctionalBlock>(blockList);
						
						foreach(var block in blockList){
							
							if(cleanSettings.DisableAirVent == true){
								
								if(block as IMyAirVent != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableAntenna == true){
								
								if(block as IMyRadioAntenna != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableArtificialMass == true){
								
								if(block as IMyArtificialMassBlock != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableAssembler == true){
								
								if(block as IMyAssembler != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableBattery == true){
								
								if(block as IMyBatteryBlock != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableBeacon == true){
								
								if(block as IMyBeacon != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableCollector == true){
								
								if(block as IMyCollector != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableConnector == true){
								
								if(block as IMyShipConnector != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableConveyorSorter == true){
								
								if(block as IMyConveyorSorter != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableDecoy == true){
								
								if(block as IMyDecoy != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableDrill == true){
								
								if(block as IMyShipDrill != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableJumpDrive == true){
								
								if(block as IMyJumpDrive != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableGasGenerator == true){
								
								if(block as IMyGasGenerator != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableGasTank == true){
								
								if(block as IMyGasTank != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableGatlingGun == true){
								
								if(block as IMySmallGatlingGun != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableGatlingTurret == true){
								
								if(block as IMyLargeGatlingTurret != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableGravityGenerator == true){
								
								if(block as IMyGravityGeneratorBase != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableGrinder == true){
								
								if(block as IMyShipGrinder != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableGyro == true){
								
								if(block as IMyGyro != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableInteriorTurret == true){
								
								if(block as IMyLargeInteriorTurret != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableLandingGear == true){
								
								if(block as IMyLandingGear != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableLaserAntenna == true){
								
								if(block as IMyLaserAntenna != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableLcdPanel == true){
								
								if(block as IMyTextPanel != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableLightBlock == true){
								
								if(block as IMyLightingBlock != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableMedicalRoom == true){
								
								if(block as IMyMedicalRoom != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableMergeBlock == true){
								
								if(block as IMyShipMergeBlock != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableMissileTurret == true){
								
								if(block as IMyLargeMissileTurret != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableOxygenFarm == true){
								
								if(block as IMyOxygenFarm != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableParachuteHatch == true){
								
								if(block as IMyParachute != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisablePiston == true){
								
								if(block as IMyPistonBase != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableProgrammableBlock == true){
								
								if(block as IMyProgrammableBlock != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableProjector == true){
								
								if(block as IMyProjector != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableReactor == true){
								
								if(block as IMyReactor != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableRefinery == true){
								
								if(block as IMyRefinery != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableRocketLauncher == true){
								
								if(block as IMySmallMissileLauncher != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableReloadableRocketLauncher == true){
								
								if(block as IMySmallMissileLauncherReload != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableRotor == true){
								
								if(block as IMyMotorStator != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableSensor == true){
								
								if(block as IMySensorBlock != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableSolarPanel == true){
								
								if(block as IMySolarPanel != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableSoundBlock == true){
								
								if(block as IMySoundBlock != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableSpaceBall == true){
								
								if(block as IMySpaceBall != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableTimerBlock == true){
								
								if(block as IMyTimerBlock != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableThruster == true){
								
								if(block as IMyThrust != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableWelder == true){
								
								if(block as IMyShipWelder != null){
									
									block.Enabled = false;
									
								}
								
							}
							
							if(cleanSettings.DisableUpgradeModule == true){
								
								if(block as IMyUpgradeModule != null){
									
									block.Enabled = false;
									
								}
								
							}
							
						}
						*/
					}
					
					continue;
					
				}
				
			}
			
		}
		
		public static bool IsDistanceFurtherThanPlayers(CleanupSettings cleanSettings, Vector3D coords, bool powered){
			
			if(cleanSettings.CleanupUseDistance == false){
				
				return false;
				
			}
			
			var player = SpawnResources.GetNearestPlayer(coords);
			
			if(player == null){
				
				return false;
				
			}
			
			double distanceToCheck = 0;
			
			if(cleanSettings.CleanupUnpoweredOverride == true && powered == false){
				
				distanceToCheck = cleanSettings.CleanupUnpoweredDistanceTrigger;
				
			}else{
				
				distanceToCheck = cleanSettings.CleanupDistanceTrigger;
				
			}
			
			if(Vector3D.Distance(player.GetPosition(), coords) > distanceToCheck){
				
				return true;
				
			}
			
			return false;
			
		}
		
		public static bool TimerExpired(IMyCubeGrid cubeGrid, CleanupSettings cleanSettings, bool distanceCheck, bool powered){
			
			if(cleanSettings.CleanupUseTimer == false){
				
				return false;
				
			}
			
			int timeTriggerToUse = 0;
			
			if(cleanSettings.CleanupUnpoweredOverride == true && powered == false){
				
				timeTriggerToUse = cleanSettings.CleanupUnpoweredTimerTrigger;
				
			}else{
				
				timeTriggerToUse = cleanSettings.CleanupTimerTrigger;
				
			}
			
			if(cleanSettings.CleanupDistanceStartsTimer == true && distanceCheck == false){
				
				return false;
				
			}
			
			NPCWatcher.ActiveNPCs[cubeGrid].CleanupTime += Settings.General.NpcCleanupCheckTimerTrigger;
			
			if(cubeGrid.Storage != null){
								
				if(cubeGrid.Storage.ContainsKey(NPCWatcher.GuidCleanupTimer) == false){

					cubeGrid.Storage.Add(NPCWatcher.GuidCleanupTimer, NPCWatcher.ActiveNPCs[cubeGrid].CleanupTime.ToString());
					
				}else{
					
					cubeGrid.Storage[NPCWatcher.GuidCleanupTimer] = NPCWatcher.ActiveNPCs[cubeGrid].CleanupTime.ToString();
					
				}
				
			}
			
			if(NPCWatcher.ActiveNPCs[cubeGrid].CleanupTime >= timeTriggerToUse){
				
				return true;
				
			}
			
			return false;
			
		}
		
		public static CleanupSettings GetCleaningSettingsForType(string spawnType){
		
			var thisSettings = new CleanupSettings();
			
			if(spawnType == "SpaceCargoShip"){
				
				thisSettings.UseCleanupSettings = Settings.SpaceCargoShips.UseCleanupSettings;
				thisSettings.CleanupUseDistance = Settings.SpaceCargoShips.CleanupUseDistance;
				thisSettings.CleanupUseTimer = Settings.SpaceCargoShips.CleanupUseTimer;
				thisSettings.CleanupDistanceStartsTimer = Settings.SpaceCargoShips.CleanupDistanceStartsTimer;
				thisSettings.CleanupResetTimerWithinDistance = Settings.SpaceCargoShips.CleanupResetTimerWithinDistance;
				thisSettings.CleanupDistanceTrigger = Settings.SpaceCargoShips.CleanupDistanceTrigger;
				thisSettings.CleanupTimerTrigger = Settings.SpaceCargoShips.CleanupTimerTrigger;
				thisSettings.CleanupIncludeUnowned = Settings.SpaceCargoShips.CleanupIncludeUnowned;
				thisSettings.CleanupUnpoweredOverride = Settings.SpaceCargoShips.CleanupUnpoweredOverride;
				thisSettings.CleanupUnpoweredDistanceTrigger = Settings.SpaceCargoShips.CleanupUnpoweredDistanceTrigger;
				thisSettings.CleanupUnpoweredTimerTrigger = Settings.SpaceCargoShips.CleanupUnpoweredTimerTrigger;
				thisSettings.UseBlockDisable = Settings.SpaceCargoShips.UseBlockDisable;
				thisSettings.DisableAirVent = Settings.SpaceCargoShips.DisableAirVent;
				thisSettings.DisableAntenna = Settings.SpaceCargoShips.DisableAntenna;
				thisSettings.DisableArtificialMass = Settings.SpaceCargoShips.DisableArtificialMass;
				thisSettings.DisableAssembler = Settings.SpaceCargoShips.DisableAssembler;
				thisSettings.DisableBattery = Settings.SpaceCargoShips.DisableBattery;
				thisSettings.DisableBattery = Settings.SpaceCargoShips.DisableBattery;
				thisSettings.DisableCollector = Settings.SpaceCargoShips.DisableCollector;
				thisSettings.DisableCollector = Settings.SpaceCargoShips.DisableCollector;
				thisSettings.DisableCollector = Settings.SpaceCargoShips.DisableCollector;
				thisSettings.DisableDecoy = Settings.SpaceCargoShips.DisableDecoy;
				thisSettings.DisableDecoy = Settings.SpaceCargoShips.DisableDecoy;
				thisSettings.DisableJumpDrive = Settings.SpaceCargoShips.DisableJumpDrive;
				thisSettings.DisableGasGenerator = Settings.SpaceCargoShips.DisableGasGenerator;
				thisSettings.DisableGasGenerator = Settings.SpaceCargoShips.DisableGasGenerator;
				thisSettings.DisableGatlingGun = Settings.SpaceCargoShips.DisableGatlingGun;
				thisSettings.DisableGatlingTurret = Settings.SpaceCargoShips.DisableGatlingTurret;
				thisSettings.DisableGravityGenerator = Settings.SpaceCargoShips.DisableGravityGenerator;
				thisSettings.DisableGrinder = Settings.SpaceCargoShips.DisableGrinder;
				thisSettings.DisableGyro = Settings.SpaceCargoShips.DisableGyro;
				thisSettings.DisableInteriorTurret = Settings.SpaceCargoShips.DisableInteriorTurret;
				thisSettings.DisableLandingGear = Settings.SpaceCargoShips.DisableLandingGear;
				thisSettings.DisableLaserAntenna = Settings.SpaceCargoShips.DisableLaserAntenna;
				thisSettings.DisableLcdPanel = Settings.SpaceCargoShips.DisableLcdPanel;
				thisSettings.DisableLightBlock = Settings.SpaceCargoShips.DisableLightBlock;
				thisSettings.DisableMedicalRoom = Settings.SpaceCargoShips.DisableMedicalRoom;
				thisSettings.DisableMergeBlock = Settings.SpaceCargoShips.DisableMergeBlock;
				thisSettings.DisableMissileTurret = Settings.SpaceCargoShips.DisableMissileTurret;
				thisSettings.DisableOxygenFarm = Settings.SpaceCargoShips.DisableOxygenFarm;
				thisSettings.DisableParachuteHatch = Settings.SpaceCargoShips.DisableParachuteHatch;
				thisSettings.DisablePiston = Settings.SpaceCargoShips.DisablePiston;
				thisSettings.DisableProgrammableBlock = Settings.SpaceCargoShips.DisableProgrammableBlock;
				thisSettings.DisableProjector = Settings.SpaceCargoShips.DisableProjector;
				thisSettings.DisableReactor = Settings.SpaceCargoShips.DisableReactor;
				thisSettings.DisableRefinery = Settings.SpaceCargoShips.DisableRefinery;
				thisSettings.DisableRocketLauncher = Settings.SpaceCargoShips.DisableRocketLauncher;
				thisSettings.DisableReloadableRocketLauncher = Settings.SpaceCargoShips.DisableReloadableRocketLauncher;
				thisSettings.DisableRotor = Settings.SpaceCargoShips.DisableRotor;
				thisSettings.DisableSensor = Settings.SpaceCargoShips.DisableSensor;
				thisSettings.DisableSolarPanel = Settings.SpaceCargoShips.DisableSolarPanel;
				thisSettings.DisableSoundBlock = Settings.SpaceCargoShips.DisableSoundBlock;
				thisSettings.DisableSpaceBall = Settings.SpaceCargoShips.DisableSpaceBall;
				thisSettings.DisableTimerBlock = Settings.SpaceCargoShips.DisableTimerBlock;
				thisSettings.DisableThruster = Settings.SpaceCargoShips.DisableThruster;
				thisSettings.DisableWelder = Settings.SpaceCargoShips.DisableWelder;
				thisSettings.DisableUpgradeModule = Settings.SpaceCargoShips.DisableUpgradeModule;
				
			}
			
			if(spawnType == "RandomEncounter"){
				
				thisSettings.UseCleanupSettings = Settings.RandomEncounters.UseCleanupSettings;
				thisSettings.CleanupUseDistance = Settings.RandomEncounters.CleanupUseDistance;
				thisSettings.CleanupUseTimer = Settings.RandomEncounters.CleanupUseTimer;
				thisSettings.CleanupDistanceStartsTimer = Settings.RandomEncounters.CleanupDistanceStartsTimer;
				thisSettings.CleanupResetTimerWithinDistance = Settings.RandomEncounters.CleanupResetTimerWithinDistance;
				thisSettings.CleanupDistanceTrigger = Settings.RandomEncounters.CleanupDistanceTrigger;
				thisSettings.CleanupTimerTrigger = Settings.RandomEncounters.CleanupTimerTrigger;
				thisSettings.CleanupIncludeUnowned = Settings.RandomEncounters.CleanupIncludeUnowned;
				thisSettings.CleanupUnpoweredOverride = Settings.RandomEncounters.CleanupUnpoweredOverride;
				thisSettings.CleanupUnpoweredDistanceTrigger = Settings.RandomEncounters.CleanupUnpoweredDistanceTrigger;
				thisSettings.CleanupUnpoweredTimerTrigger = Settings.RandomEncounters.CleanupUnpoweredTimerTrigger;
				thisSettings.UseBlockDisable = Settings.RandomEncounters.UseBlockDisable;
				thisSettings.DisableAirVent = Settings.RandomEncounters.DisableAirVent;
				thisSettings.DisableAntenna = Settings.RandomEncounters.DisableAntenna;
				thisSettings.DisableArtificialMass = Settings.RandomEncounters.DisableArtificialMass;
				thisSettings.DisableAssembler = Settings.RandomEncounters.DisableAssembler;
				thisSettings.DisableBattery = Settings.RandomEncounters.DisableBattery;
				thisSettings.DisableBattery = Settings.RandomEncounters.DisableBattery;
				thisSettings.DisableCollector = Settings.RandomEncounters.DisableCollector;
				thisSettings.DisableCollector = Settings.RandomEncounters.DisableCollector;
				thisSettings.DisableCollector = Settings.RandomEncounters.DisableCollector;
				thisSettings.DisableDecoy = Settings.RandomEncounters.DisableDecoy;
				thisSettings.DisableDecoy = Settings.RandomEncounters.DisableDecoy;
				thisSettings.DisableJumpDrive = Settings.RandomEncounters.DisableJumpDrive;
				thisSettings.DisableGasGenerator = Settings.RandomEncounters.DisableGasGenerator;
				thisSettings.DisableGasGenerator = Settings.RandomEncounters.DisableGasGenerator;
				thisSettings.DisableGatlingGun = Settings.RandomEncounters.DisableGatlingGun;
				thisSettings.DisableGatlingTurret = Settings.RandomEncounters.DisableGatlingTurret;
				thisSettings.DisableGravityGenerator = Settings.RandomEncounters.DisableGravityGenerator;
				thisSettings.DisableGrinder = Settings.RandomEncounters.DisableGrinder;
				thisSettings.DisableGyro = Settings.RandomEncounters.DisableGyro;
				thisSettings.DisableInteriorTurret = Settings.RandomEncounters.DisableInteriorTurret;
				thisSettings.DisableLandingGear = Settings.RandomEncounters.DisableLandingGear;
				thisSettings.DisableLaserAntenna = Settings.RandomEncounters.DisableLaserAntenna;
				thisSettings.DisableLcdPanel = Settings.RandomEncounters.DisableLcdPanel;
				thisSettings.DisableLightBlock = Settings.RandomEncounters.DisableLightBlock;
				thisSettings.DisableMedicalRoom = Settings.RandomEncounters.DisableMedicalRoom;
				thisSettings.DisableMergeBlock = Settings.RandomEncounters.DisableMergeBlock;
				thisSettings.DisableMissileTurret = Settings.RandomEncounters.DisableMissileTurret;
				thisSettings.DisableOxygenFarm = Settings.RandomEncounters.DisableOxygenFarm;
				thisSettings.DisableParachuteHatch = Settings.RandomEncounters.DisableParachuteHatch;
				thisSettings.DisablePiston = Settings.RandomEncounters.DisablePiston;
				thisSettings.DisableProgrammableBlock = Settings.RandomEncounters.DisableProgrammableBlock;
				thisSettings.DisableProjector = Settings.RandomEncounters.DisableProjector;
				thisSettings.DisableReactor = Settings.RandomEncounters.DisableReactor;
				thisSettings.DisableRefinery = Settings.RandomEncounters.DisableRefinery;
				thisSettings.DisableRocketLauncher = Settings.RandomEncounters.DisableRocketLauncher;
				thisSettings.DisableReloadableRocketLauncher = Settings.RandomEncounters.DisableReloadableRocketLauncher;
				thisSettings.DisableRotor = Settings.RandomEncounters.DisableRotor;
				thisSettings.DisableSensor = Settings.RandomEncounters.DisableSensor;
				thisSettings.DisableSolarPanel = Settings.RandomEncounters.DisableSolarPanel;
				thisSettings.DisableSoundBlock = Settings.RandomEncounters.DisableSoundBlock;
				thisSettings.DisableSpaceBall = Settings.RandomEncounters.DisableSpaceBall;
				thisSettings.DisableTimerBlock = Settings.RandomEncounters.DisableTimerBlock;
				thisSettings.DisableThruster = Settings.RandomEncounters.DisableThruster;
				thisSettings.DisableWelder = Settings.RandomEncounters.DisableWelder;
				thisSettings.DisableUpgradeModule = Settings.RandomEncounters.DisableUpgradeModule;
			}
			
			if(spawnType == "BossEncounter"){
				
				thisSettings.UseCleanupSettings = Settings.BossEncounters.UseCleanupSettings;
				thisSettings.CleanupUseDistance = Settings.BossEncounters.CleanupUseDistance;
				thisSettings.CleanupUseTimer = Settings.BossEncounters.CleanupUseTimer;
				thisSettings.CleanupDistanceStartsTimer = Settings.BossEncounters.CleanupDistanceStartsTimer;
				thisSettings.CleanupResetTimerWithinDistance = Settings.BossEncounters.CleanupResetTimerWithinDistance;
				thisSettings.CleanupDistanceTrigger = Settings.BossEncounters.CleanupDistanceTrigger;
				thisSettings.CleanupTimerTrigger = Settings.BossEncounters.CleanupTimerTrigger;
				thisSettings.CleanupIncludeUnowned = Settings.BossEncounters.CleanupIncludeUnowned;
				thisSettings.CleanupUnpoweredOverride = Settings.BossEncounters.CleanupUnpoweredOverride;
				thisSettings.CleanupUnpoweredDistanceTrigger = Settings.BossEncounters.CleanupUnpoweredDistanceTrigger;
				thisSettings.CleanupUnpoweredTimerTrigger = Settings.BossEncounters.CleanupUnpoweredTimerTrigger;
				thisSettings.UseBlockDisable = Settings.BossEncounters.UseBlockDisable;
				thisSettings.DisableAirVent = Settings.BossEncounters.DisableAirVent;
				thisSettings.DisableAntenna = Settings.BossEncounters.DisableAntenna;
				thisSettings.DisableArtificialMass = Settings.BossEncounters.DisableArtificialMass;
				thisSettings.DisableAssembler = Settings.BossEncounters.DisableAssembler;
				thisSettings.DisableBattery = Settings.BossEncounters.DisableBattery;
				thisSettings.DisableBattery = Settings.BossEncounters.DisableBattery;
				thisSettings.DisableCollector = Settings.BossEncounters.DisableCollector;
				thisSettings.DisableCollector = Settings.BossEncounters.DisableCollector;
				thisSettings.DisableCollector = Settings.BossEncounters.DisableCollector;
				thisSettings.DisableDecoy = Settings.BossEncounters.DisableDecoy;
				thisSettings.DisableDecoy = Settings.BossEncounters.DisableDecoy;
				thisSettings.DisableJumpDrive = Settings.BossEncounters.DisableJumpDrive;
				thisSettings.DisableGasGenerator = Settings.BossEncounters.DisableGasGenerator;
				thisSettings.DisableGasGenerator = Settings.BossEncounters.DisableGasGenerator;
				thisSettings.DisableGatlingGun = Settings.BossEncounters.DisableGatlingGun;
				thisSettings.DisableGatlingTurret = Settings.BossEncounters.DisableGatlingTurret;
				thisSettings.DisableGravityGenerator = Settings.BossEncounters.DisableGravityGenerator;
				thisSettings.DisableGrinder = Settings.BossEncounters.DisableGrinder;
				thisSettings.DisableGyro = Settings.BossEncounters.DisableGyro;
				thisSettings.DisableInteriorTurret = Settings.BossEncounters.DisableInteriorTurret;
				thisSettings.DisableLandingGear = Settings.BossEncounters.DisableLandingGear;
				thisSettings.DisableLaserAntenna = Settings.BossEncounters.DisableLaserAntenna;
				thisSettings.DisableLcdPanel = Settings.BossEncounters.DisableLcdPanel;
				thisSettings.DisableLightBlock = Settings.BossEncounters.DisableLightBlock;
				thisSettings.DisableMedicalRoom = Settings.BossEncounters.DisableMedicalRoom;
				thisSettings.DisableMergeBlock = Settings.BossEncounters.DisableMergeBlock;
				thisSettings.DisableMissileTurret = Settings.BossEncounters.DisableMissileTurret;
				thisSettings.DisableOxygenFarm = Settings.BossEncounters.DisableOxygenFarm;
				thisSettings.DisableParachuteHatch = Settings.BossEncounters.DisableParachuteHatch;
				thisSettings.DisablePiston = Settings.BossEncounters.DisablePiston;
				thisSettings.DisableProgrammableBlock = Settings.BossEncounters.DisableProgrammableBlock;
				thisSettings.DisableProjector = Settings.BossEncounters.DisableProjector;
				thisSettings.DisableReactor = Settings.BossEncounters.DisableReactor;
				thisSettings.DisableRefinery = Settings.BossEncounters.DisableRefinery;
				thisSettings.DisableRocketLauncher = Settings.BossEncounters.DisableRocketLauncher;
				thisSettings.DisableReloadableRocketLauncher = Settings.BossEncounters.DisableReloadableRocketLauncher;
				thisSettings.DisableRotor = Settings.BossEncounters.DisableRotor;
				thisSettings.DisableSensor = Settings.BossEncounters.DisableSensor;
				thisSettings.DisableSolarPanel = Settings.BossEncounters.DisableSolarPanel;
				thisSettings.DisableSoundBlock = Settings.BossEncounters.DisableSoundBlock;
				thisSettings.DisableSpaceBall = Settings.BossEncounters.DisableSpaceBall;
				thisSettings.DisableTimerBlock = Settings.BossEncounters.DisableTimerBlock;
				thisSettings.DisableThruster = Settings.BossEncounters.DisableThruster;
				thisSettings.DisableWelder = Settings.BossEncounters.DisableWelder;
				thisSettings.DisableUpgradeModule = Settings.BossEncounters.DisableUpgradeModule;
			}
			
			if(spawnType == "PlanetaryCargoShip"){
				
				thisSettings.UseCleanupSettings = Settings.PlanetaryCargoShips.UseCleanupSettings;
				thisSettings.CleanupUseDistance = Settings.PlanetaryCargoShips.CleanupUseDistance;
				thisSettings.CleanupUseTimer = Settings.PlanetaryCargoShips.CleanupUseTimer;
				thisSettings.CleanupDistanceStartsTimer = Settings.PlanetaryCargoShips.CleanupDistanceStartsTimer;
				thisSettings.CleanupResetTimerWithinDistance = Settings.PlanetaryCargoShips.CleanupResetTimerWithinDistance;
				thisSettings.CleanupDistanceTrigger = Settings.PlanetaryCargoShips.CleanupDistanceTrigger;
				thisSettings.CleanupTimerTrigger = Settings.PlanetaryCargoShips.CleanupTimerTrigger;
				thisSettings.CleanupIncludeUnowned = Settings.PlanetaryCargoShips.CleanupIncludeUnowned;
				thisSettings.CleanupUnpoweredOverride = Settings.PlanetaryCargoShips.CleanupUnpoweredOverride;
				thisSettings.CleanupUnpoweredDistanceTrigger = Settings.PlanetaryCargoShips.CleanupUnpoweredDistanceTrigger;
				thisSettings.CleanupUnpoweredTimerTrigger = Settings.PlanetaryCargoShips.CleanupUnpoweredTimerTrigger;
				thisSettings.UseBlockDisable = Settings.PlanetaryCargoShips.UseBlockDisable;
				thisSettings.DisableAirVent = Settings.PlanetaryCargoShips.DisableAirVent;
				thisSettings.DisableAntenna = Settings.PlanetaryCargoShips.DisableAntenna;
				thisSettings.DisableArtificialMass = Settings.PlanetaryCargoShips.DisableArtificialMass;
				thisSettings.DisableAssembler = Settings.PlanetaryCargoShips.DisableAssembler;
				thisSettings.DisableBattery = Settings.PlanetaryCargoShips.DisableBattery;
				thisSettings.DisableBattery = Settings.PlanetaryCargoShips.DisableBattery;
				thisSettings.DisableCollector = Settings.PlanetaryCargoShips.DisableCollector;
				thisSettings.DisableCollector = Settings.PlanetaryCargoShips.DisableCollector;
				thisSettings.DisableCollector = Settings.PlanetaryCargoShips.DisableCollector;
				thisSettings.DisableDecoy = Settings.PlanetaryCargoShips.DisableDecoy;
				thisSettings.DisableDecoy = Settings.PlanetaryCargoShips.DisableDecoy;
				thisSettings.DisableJumpDrive = Settings.PlanetaryCargoShips.DisableJumpDrive;
				thisSettings.DisableGasGenerator = Settings.PlanetaryCargoShips.DisableGasGenerator;
				thisSettings.DisableGasGenerator = Settings.PlanetaryCargoShips.DisableGasGenerator;
				thisSettings.DisableGatlingGun = Settings.PlanetaryCargoShips.DisableGatlingGun;
				thisSettings.DisableGatlingTurret = Settings.PlanetaryCargoShips.DisableGatlingTurret;
				thisSettings.DisableGravityGenerator = Settings.PlanetaryCargoShips.DisableGravityGenerator;
				thisSettings.DisableGrinder = Settings.PlanetaryCargoShips.DisableGrinder;
				thisSettings.DisableGyro = Settings.PlanetaryCargoShips.DisableGyro;
				thisSettings.DisableInteriorTurret = Settings.PlanetaryCargoShips.DisableInteriorTurret;
				thisSettings.DisableLandingGear = Settings.PlanetaryCargoShips.DisableLandingGear;
				thisSettings.DisableLaserAntenna = Settings.PlanetaryCargoShips.DisableLaserAntenna;
				thisSettings.DisableLcdPanel = Settings.PlanetaryCargoShips.DisableLcdPanel;
				thisSettings.DisableLightBlock = Settings.PlanetaryCargoShips.DisableLightBlock;
				thisSettings.DisableMedicalRoom = Settings.PlanetaryCargoShips.DisableMedicalRoom;
				thisSettings.DisableMergeBlock = Settings.PlanetaryCargoShips.DisableMergeBlock;
				thisSettings.DisableMissileTurret = Settings.PlanetaryCargoShips.DisableMissileTurret;
				thisSettings.DisableOxygenFarm = Settings.PlanetaryCargoShips.DisableOxygenFarm;
				thisSettings.DisableParachuteHatch = Settings.PlanetaryCargoShips.DisableParachuteHatch;
				thisSettings.DisablePiston = Settings.PlanetaryCargoShips.DisablePiston;
				thisSettings.DisableProgrammableBlock = Settings.PlanetaryCargoShips.DisableProgrammableBlock;
				thisSettings.DisableProjector = Settings.PlanetaryCargoShips.DisableProjector;
				thisSettings.DisableReactor = Settings.PlanetaryCargoShips.DisableReactor;
				thisSettings.DisableRefinery = Settings.PlanetaryCargoShips.DisableRefinery;
				thisSettings.DisableRocketLauncher = Settings.PlanetaryCargoShips.DisableRocketLauncher;
				thisSettings.DisableReloadableRocketLauncher = Settings.PlanetaryCargoShips.DisableReloadableRocketLauncher;
				thisSettings.DisableRotor = Settings.PlanetaryCargoShips.DisableRotor;
				thisSettings.DisableSensor = Settings.PlanetaryCargoShips.DisableSensor;
				thisSettings.DisableSolarPanel = Settings.PlanetaryCargoShips.DisableSolarPanel;
				thisSettings.DisableSoundBlock = Settings.PlanetaryCargoShips.DisableSoundBlock;
				thisSettings.DisableSpaceBall = Settings.PlanetaryCargoShips.DisableSpaceBall;
				thisSettings.DisableTimerBlock = Settings.PlanetaryCargoShips.DisableTimerBlock;
				thisSettings.DisableThruster = Settings.PlanetaryCargoShips.DisableThruster;
				thisSettings.DisableWelder = Settings.PlanetaryCargoShips.DisableWelder;
				thisSettings.DisableUpgradeModule = Settings.PlanetaryCargoShips.DisableUpgradeModule;
			}
			
			if(spawnType == "PlanetaryInstallation"){

				thisSettings.UseCleanupSettings = Settings.PlanetaryInstallations.UseCleanupSettings;
				thisSettings.CleanupUseDistance = Settings.PlanetaryInstallations.CleanupUseDistance;
				thisSettings.CleanupUseTimer = Settings.PlanetaryInstallations.CleanupUseTimer;
				thisSettings.CleanupDistanceStartsTimer = Settings.PlanetaryInstallations.CleanupDistanceStartsTimer;
				thisSettings.CleanupResetTimerWithinDistance = Settings.PlanetaryInstallations.CleanupResetTimerWithinDistance;
				thisSettings.CleanupDistanceTrigger = Settings.PlanetaryInstallations.CleanupDistanceTrigger;
				thisSettings.CleanupTimerTrigger = Settings.PlanetaryInstallations.CleanupTimerTrigger;
				thisSettings.CleanupIncludeUnowned = Settings.PlanetaryInstallations.CleanupIncludeUnowned;
				thisSettings.CleanupUnpoweredOverride = Settings.PlanetaryInstallations.CleanupUnpoweredOverride;
				thisSettings.CleanupUnpoweredDistanceTrigger = Settings.PlanetaryInstallations.CleanupUnpoweredDistanceTrigger;
				thisSettings.CleanupUnpoweredTimerTrigger = Settings.PlanetaryInstallations.CleanupUnpoweredTimerTrigger;
				thisSettings.UseBlockDisable = Settings.PlanetaryInstallations.UseBlockDisable;
				thisSettings.DisableAirVent = Settings.PlanetaryInstallations.DisableAirVent;
				thisSettings.DisableAntenna = Settings.PlanetaryInstallations.DisableAntenna;
				thisSettings.DisableArtificialMass = Settings.PlanetaryInstallations.DisableArtificialMass;
				thisSettings.DisableAssembler = Settings.PlanetaryInstallations.DisableAssembler;
				thisSettings.DisableBattery = Settings.PlanetaryInstallations.DisableBattery;
				thisSettings.DisableBattery = Settings.PlanetaryInstallations.DisableBattery;
				thisSettings.DisableCollector = Settings.PlanetaryInstallations.DisableCollector;
				thisSettings.DisableCollector = Settings.PlanetaryInstallations.DisableCollector;
				thisSettings.DisableCollector = Settings.PlanetaryInstallations.DisableCollector;
				thisSettings.DisableDecoy = Settings.PlanetaryInstallations.DisableDecoy;
				thisSettings.DisableDecoy = Settings.PlanetaryInstallations.DisableDecoy;
				thisSettings.DisableJumpDrive = Settings.PlanetaryInstallations.DisableJumpDrive;
				thisSettings.DisableGasGenerator = Settings.PlanetaryInstallations.DisableGasGenerator;
				thisSettings.DisableGasGenerator = Settings.PlanetaryInstallations.DisableGasGenerator;
				thisSettings.DisableGatlingGun = Settings.PlanetaryInstallations.DisableGatlingGun;
				thisSettings.DisableGatlingTurret = Settings.PlanetaryInstallations.DisableGatlingTurret;
				thisSettings.DisableGravityGenerator = Settings.PlanetaryInstallations.DisableGravityGenerator;
				thisSettings.DisableGrinder = Settings.PlanetaryInstallations.DisableGrinder;
				thisSettings.DisableGyro = Settings.PlanetaryInstallations.DisableGyro;
				thisSettings.DisableInteriorTurret = Settings.PlanetaryInstallations.DisableInteriorTurret;
				thisSettings.DisableLandingGear = Settings.PlanetaryInstallations.DisableLandingGear;
				thisSettings.DisableLaserAntenna = Settings.PlanetaryInstallations.DisableLaserAntenna;
				thisSettings.DisableLcdPanel = Settings.PlanetaryInstallations.DisableLcdPanel;
				thisSettings.DisableLightBlock = Settings.PlanetaryInstallations.DisableLightBlock;
				thisSettings.DisableMedicalRoom = Settings.PlanetaryInstallations.DisableMedicalRoom;
				thisSettings.DisableMergeBlock = Settings.PlanetaryInstallations.DisableMergeBlock;
				thisSettings.DisableMissileTurret = Settings.PlanetaryInstallations.DisableMissileTurret;
				thisSettings.DisableOxygenFarm = Settings.PlanetaryInstallations.DisableOxygenFarm;
				thisSettings.DisableParachuteHatch = Settings.PlanetaryInstallations.DisableParachuteHatch;
				thisSettings.DisablePiston = Settings.PlanetaryInstallations.DisablePiston;
				thisSettings.DisableProgrammableBlock = Settings.PlanetaryInstallations.DisableProgrammableBlock;
				thisSettings.DisableProjector = Settings.PlanetaryInstallations.DisableProjector;
				thisSettings.DisableReactor = Settings.PlanetaryInstallations.DisableReactor;
				thisSettings.DisableRefinery = Settings.PlanetaryInstallations.DisableRefinery;
				thisSettings.DisableRocketLauncher = Settings.PlanetaryInstallations.DisableRocketLauncher;
				thisSettings.DisableReloadableRocketLauncher = Settings.PlanetaryInstallations.DisableReloadableRocketLauncher;
				thisSettings.DisableRotor = Settings.PlanetaryInstallations.DisableRotor;
				thisSettings.DisableSensor = Settings.PlanetaryInstallations.DisableSensor;
				thisSettings.DisableSolarPanel = Settings.PlanetaryInstallations.DisableSolarPanel;
				thisSettings.DisableSoundBlock = Settings.PlanetaryInstallations.DisableSoundBlock;
				thisSettings.DisableSpaceBall = Settings.PlanetaryInstallations.DisableSpaceBall;
				thisSettings.DisableTimerBlock = Settings.PlanetaryInstallations.DisableTimerBlock;
				thisSettings.DisableThruster = Settings.PlanetaryInstallations.DisableThruster;
				thisSettings.DisableWelder = Settings.PlanetaryInstallations.DisableWelder;
				thisSettings.DisableUpgradeModule = Settings.PlanetaryInstallations.DisableUpgradeModule;
			}
			
			if(spawnType == "Other") {
				
				thisSettings.UseCleanupSettings = Settings.OtherNPCs.UseCleanupSettings;
				thisSettings.CleanupUseDistance = Settings.OtherNPCs.CleanupUseDistance;
				thisSettings.CleanupUseTimer = Settings.OtherNPCs.CleanupUseTimer;
				thisSettings.CleanupDistanceStartsTimer = Settings.OtherNPCs.CleanupDistanceStartsTimer;
				thisSettings.CleanupResetTimerWithinDistance = Settings.OtherNPCs.CleanupResetTimerWithinDistance;
				thisSettings.CleanupDistanceTrigger = Settings.OtherNPCs.CleanupDistanceTrigger;
				thisSettings.CleanupTimerTrigger = Settings.OtherNPCs.CleanupTimerTrigger;
				thisSettings.CleanupIncludeUnowned = Settings.OtherNPCs.CleanupIncludeUnowned;
				thisSettings.CleanupUnpoweredOverride = Settings.OtherNPCs.CleanupUnpoweredOverride;
				thisSettings.CleanupUnpoweredDistanceTrigger = Settings.OtherNPCs.CleanupUnpoweredDistanceTrigger;
				thisSettings.CleanupUnpoweredTimerTrigger = Settings.OtherNPCs.CleanupUnpoweredTimerTrigger;
				thisSettings.UseBlockDisable = Settings.OtherNPCs.UseBlockDisable;
				thisSettings.DisableAirVent = Settings.OtherNPCs.DisableAirVent;
				thisSettings.DisableAntenna = Settings.OtherNPCs.DisableAntenna;
				thisSettings.DisableArtificialMass = Settings.OtherNPCs.DisableArtificialMass;
				thisSettings.DisableAssembler = Settings.OtherNPCs.DisableAssembler;
				thisSettings.DisableBattery = Settings.OtherNPCs.DisableBattery;
				thisSettings.DisableBattery = Settings.OtherNPCs.DisableBattery;
				thisSettings.DisableCollector = Settings.OtherNPCs.DisableCollector;
				thisSettings.DisableCollector = Settings.OtherNPCs.DisableCollector;
				thisSettings.DisableCollector = Settings.OtherNPCs.DisableCollector;
				thisSettings.DisableDecoy = Settings.OtherNPCs.DisableDecoy;
				thisSettings.DisableDecoy = Settings.OtherNPCs.DisableDecoy;
				thisSettings.DisableJumpDrive = Settings.OtherNPCs.DisableJumpDrive;
				thisSettings.DisableGasGenerator = Settings.OtherNPCs.DisableGasGenerator;
				thisSettings.DisableGasGenerator = Settings.OtherNPCs.DisableGasGenerator;
				thisSettings.DisableGatlingGun = Settings.OtherNPCs.DisableGatlingGun;
				thisSettings.DisableGatlingTurret = Settings.OtherNPCs.DisableGatlingTurret;
				thisSettings.DisableGravityGenerator = Settings.OtherNPCs.DisableGravityGenerator;
				thisSettings.DisableGrinder = Settings.OtherNPCs.DisableGrinder;
				thisSettings.DisableGyro = Settings.OtherNPCs.DisableGyro;
				thisSettings.DisableInteriorTurret = Settings.OtherNPCs.DisableInteriorTurret;
				thisSettings.DisableLandingGear = Settings.OtherNPCs.DisableLandingGear;
				thisSettings.DisableLaserAntenna = Settings.OtherNPCs.DisableLaserAntenna;
				thisSettings.DisableLcdPanel = Settings.OtherNPCs.DisableLcdPanel;
				thisSettings.DisableLightBlock = Settings.OtherNPCs.DisableLightBlock;
				thisSettings.DisableMedicalRoom = Settings.OtherNPCs.DisableMedicalRoom;
				thisSettings.DisableMergeBlock = Settings.OtherNPCs.DisableMergeBlock;
				thisSettings.DisableMissileTurret = Settings.OtherNPCs.DisableMissileTurret;
				thisSettings.DisableOxygenFarm = Settings.OtherNPCs.DisableOxygenFarm;
				thisSettings.DisableParachuteHatch = Settings.OtherNPCs.DisableParachuteHatch;
				thisSettings.DisablePiston = Settings.OtherNPCs.DisablePiston;
				thisSettings.DisableProgrammableBlock = Settings.OtherNPCs.DisableProgrammableBlock;
				thisSettings.DisableProjector = Settings.OtherNPCs.DisableProjector;
				thisSettings.DisableReactor = Settings.OtherNPCs.DisableReactor;
				thisSettings.DisableRefinery = Settings.OtherNPCs.DisableRefinery;
				thisSettings.DisableRocketLauncher = Settings.OtherNPCs.DisableRocketLauncher;
				thisSettings.DisableReloadableRocketLauncher = Settings.OtherNPCs.DisableReloadableRocketLauncher;
				thisSettings.DisableRotor = Settings.OtherNPCs.DisableRotor;
				thisSettings.DisableSensor = Settings.OtherNPCs.DisableSensor;
				thisSettings.DisableSolarPanel = Settings.OtherNPCs.DisableSolarPanel;
				thisSettings.DisableSoundBlock = Settings.OtherNPCs.DisableSoundBlock;
				thisSettings.DisableSpaceBall = Settings.OtherNPCs.DisableSpaceBall;
				thisSettings.DisableTimerBlock = Settings.OtherNPCs.DisableTimerBlock;
				thisSettings.DisableThruster = Settings.OtherNPCs.DisableThruster;
				thisSettings.DisableWelder = Settings.OtherNPCs.DisableWelder;
				thisSettings.DisableUpgradeModule = Settings.OtherNPCs.DisableUpgradeModule;
				
			}
			
			return thisSettings;
			
		}
		
		public static bool IsSubgridForNormalEncounter(IMyCubeGrid cubeGrid){
			
			if(cubeGrid == null || MyAPIGateway.Entities.Exist(cubeGrid) == false){
				
				return false;
				
			}
			
			var gridGroups = MyAPIGateway.GridGroups.GetGroup(cubeGrid, GridLinkTypeEnum.Mechanical);
			
			if(gridGroups.Contains(cubeGrid) == false){
				
				gridGroups.Add(cubeGrid);
				
			}
			
			foreach(var grid in gridGroups){
				
				if(NPCWatcher.ActiveNPCs.ContainsKey(grid) == true){
					
					var spawnType = NPCWatcher.ActiveNPCs[grid].SpawnType;
					
					if(spawnType != "Other" && spawnType != "UnknownSource"){
						
						return true;
						
					}
					
				}
				
			}
			
			return false;
			
		}
		
		public static bool HasLegacyIgnoreTag(IMyCubeGrid cubeGrid){
			
			if(cubeGrid == null || MyAPIGateway.Entities.Exist(cubeGrid) == false){
				
				return false;
				
			}
			
			var gridGroups = MyAPIGateway.GridGroups.GetGroup(cubeGrid, GridLinkTypeEnum.Mechanical);
			
			if(gridGroups.Contains(cubeGrid) == false){
				
				gridGroups.Add(cubeGrid);
				
			}
			
			foreach(var grid in gridGroups){
				
				if(grid.CustomName.Contains("[NPC-IGNORE]") == true){
					
					if(NPCWatcher.ActiveNPCs.ContainsKey(grid) == true){
						
						NPCWatcher.ActiveNPCs[grid].CleanupIgnore = true;
						
					}
					
					return true;
					
				}
				
			}
			
			return false;
			
		}
		
		public static bool IsGridPowered(IMyCubeGrid cubeGrid){
			
			
			if(string.IsNullOrEmpty(MyVisualScriptLogicProvider.GetEntityName(cubeGrid.EntityId)) == true){
				
				MyVisualScriptLogicProvider.SetName(cubeGrid.EntityId, cubeGrid.EntityId.ToString());
				
			}
			
			return MyVisualScriptLogicProvider.HasPower(MyVisualScriptLogicProvider.GetEntityName(cubeGrid.EntityId));
			
		}
		
	}
	
}