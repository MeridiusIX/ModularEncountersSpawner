using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
	
	public static class SettingsEditor{
				
		public static string EditSettings(string receivedCommand){
			
			if(receivedCommand.StartsWith("/MES.Settings.ResetAll") == true){
				
				Settings.General = new ConfigGeneral();
				Settings.General.SaveSettings(new ConfigGeneral());
				
				Settings.SpaceCargoShips = new ConfigSpaceCargoShips();
				Settings.SpaceCargoShips.SaveSettings(new ConfigSpaceCargoShips());
				
				Settings.RandomEncounters = new ConfigRandomEncounters();
				Settings.RandomEncounters.SaveSettings(new ConfigRandomEncounters());
				
				Settings.PlanetaryCargoShips = new ConfigPlanetaryCargoShips();
				Settings.PlanetaryCargoShips.SaveSettings(new ConfigPlanetaryCargoShips());
				
				Settings.PlanetaryInstallations = new ConfigPlanetaryInstallations();
				Settings.PlanetaryInstallations.SaveSettings(new ConfigPlanetaryInstallations());
				
				Settings.OtherNPCs = new ConfigOtherNPCs();
				Settings.OtherNPCs.SaveSettings(new ConfigOtherNPCs());
				
				return "All Spawner Settings Reset To Default Values.";
				
			}
			
			////////////////////////////////////////////////////////
			//                   General
			////////////////////////////////////////////////////////
			
			if(receivedCommand.StartsWith("/MES.Settings.General.") == true){
					
				//EnableSpaceCargoShips
				if(receivedCommand.StartsWith("/MES.Settings.General.EnableSpaceCargoShips.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.EnableSpaceCargoShips.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.EnableSpaceCargoShips = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//EnablePlanetaryCargoShips
				if(receivedCommand.StartsWith("/MES.Settings.General.EnablePlanetaryCargoShips.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.EnablePlanetaryCargoShips.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.EnablePlanetaryCargoShips = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//EnableRandomEncounters
				if(receivedCommand.StartsWith("/MES.Settings.General.EnableRandomEncounters.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.EnableRandomEncounters.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.EnableRandomEncounters = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//EnablePlanetaryInstallations
				if(receivedCommand.StartsWith("/MES.Settings.General.EnablePlanetaryInstallations.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.EnablePlanetaryInstallations.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.EnablePlanetaryInstallations = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//EnableBossEncounters
				if(receivedCommand.StartsWith("/MES.Settings.General.EnableBossEncounters.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.EnableBossEncounters.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.EnableBossEncounters = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//EnableGlobalNPCWeaponRandomizer
				if(receivedCommand.StartsWith("/MES.Settings.General.EnableGlobalNPCWeaponRandomizer.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.EnableGlobalNPCWeaponRandomizer.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.EnableGlobalNPCWeaponRandomizer = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}

				//EnableGlobalNPCShieldProvider
				if (receivedCommand.StartsWith("/MES.Settings.General.EnableGlobalNPCShieldProvider.") == true) {

					var receivedValue = receivedCommand.Replace("/MES.Settings.General.EnableGlobalNPCShieldProvider.", "");
					bool result = false;

					if (bool.TryParse(receivedValue, out result) == false) {

						return "Failed To Parse Value: " + receivedValue;

					}

					Settings.General.EnableGlobalNPCShieldProvider = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;

				}

				//EnableLegacySpaceCargoShipDetection
				if (receivedCommand.StartsWith("/MES.Settings.General.EnableLegacySpaceCargoShipDetection.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.EnableLegacySpaceCargoShipDetection.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.EnableLegacySpaceCargoShipDetection = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//UseModIdSelectionForSpawning
				if(receivedCommand.StartsWith("/MES.Settings.General.UseModIdSelectionForSpawning.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.UseModIdSelectionForSpawning.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.UseModIdSelectionForSpawning = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//UseWeightedModIdSelection
				if(receivedCommand.StartsWith("/MES.Settings.General.UseWeightedModIdSelection.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.UseWeightedModIdSelection.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.UseWeightedModIdSelection = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//LowWeightModIdSpawnGroups
				if(receivedCommand.StartsWith("/MES.Settings.General.LowWeightModIdSpawnGroups.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.LowWeightModIdSpawnGroups.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.LowWeightModIdSpawnGroups = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//LowWeightModIdModifier
				if(receivedCommand.StartsWith("/MES.Settings.General.LowWeightModIdModifier.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.LowWeightModIdModifier.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.LowWeightModIdModifier = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//MediumWeightModIdSpawnGroups
				if(receivedCommand.StartsWith("/MES.Settings.General.MediumWeightModIdSpawnGroups.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.MediumWeightModIdSpawnGroups.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.MediumWeightModIdSpawnGroups = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//MediumWeightModIdModifier
				if(receivedCommand.StartsWith("/MES.Settings.General.MediumWeightModIdModifier.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.MediumWeightModIdModifier.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.MediumWeightModIdModifier = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//HighWeightModIdSpawnGroups
				if(receivedCommand.StartsWith("/MES.Settings.General.HighWeightModIdSpawnGroups.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.HighWeightModIdSpawnGroups.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.HighWeightModIdSpawnGroups = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//HighWeightModIdModifier
				if(receivedCommand.StartsWith("/MES.Settings.General.HighWeightModIdModifier.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.HighWeightModIdModifier.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.HighWeightModIdModifier = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//UseMaxNpcGrids
				if(receivedCommand.StartsWith("/MES.Settings.General.UseMaxNpcGrids.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.UseMaxNpcGrids.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.UseMaxNpcGrids = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//UseGlobalEventsTimers
				if(receivedCommand.StartsWith("/MES.Settings.General.UseGlobalEventsTimers.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.UseGlobalEventsTimers.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.UseGlobalEventsTimers = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//IgnorePlanetWhitelists
				if(receivedCommand.StartsWith("/MES.Settings.General.IgnorePlanetWhitelists.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.IgnorePlanetWhitelists.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.IgnorePlanetWhitelists = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//IgnorePlanetBlacklists
				if(receivedCommand.StartsWith("/MES.Settings.General.IgnorePlanetBlacklists.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.IgnorePlanetBlacklists.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.IgnorePlanetBlacklists = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//ThreatRefreshTimerMinimum
				if(receivedCommand.StartsWith("/MES.Settings.General.ThreatRefreshTimerMinimum.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.ThreatRefreshTimerMinimum.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.ThreatRefreshTimerMinimum = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//ThreatReductionHandicap
				if(receivedCommand.StartsWith("/MES.Settings.General.ThreatReductionHandicap.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.ThreatReductionHandicap.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.ThreatReductionHandicap = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//MaxGlobalNpcGrids
				if(receivedCommand.StartsWith("/MES.Settings.General.MaxGlobalNpcGrids.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.MaxGlobalNpcGrids.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.MaxGlobalNpcGrids = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//PlayerWatcherTimerTrigger
				if(receivedCommand.StartsWith("/MES.Settings.General.PlayerWatcherTimerTrigger.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.PlayerWatcherTimerTrigger.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.PlayerWatcherTimerTrigger = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//NpcDistanceCheckTimerTrigger
				if(receivedCommand.StartsWith("/MES.Settings.General.NpcDistanceCheckTimerTrigger.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.NpcDistanceCheckTimerTrigger.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.NpcDistanceCheckTimerTrigger = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//NpcOwnershipCheckTimerTrigger
				if(receivedCommand.StartsWith("/MES.Settings.General.NpcOwnershipCheckTimerTrigger.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.NpcOwnershipCheckTimerTrigger.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.NpcOwnershipCheckTimerTrigger = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//NpcCleanupCheckTimerTrigger
				if(receivedCommand.StartsWith("/MES.Settings.General.NpcCleanupCheckTimerTrigger.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.NpcCleanupCheckTimerTrigger.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.NpcCleanupCheckTimerTrigger = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//NpcBlacklistCheckTimerTrigger
				if(receivedCommand.StartsWith("/MES.Settings.General.NpcBlacklistCheckTimerTrigger.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.NpcBlacklistCheckTimerTrigger.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.NpcBlacklistCheckTimerTrigger = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//SpawnedVoxelCheckTimerTrigger
				if(receivedCommand.StartsWith("/MES.Settings.General.SpawnedVoxelCheckTimerTrigger.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.SpawnedVoxelCheckTimerTrigger.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.SpawnedVoxelCheckTimerTrigger = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//SpawnedVoxelMinimumGridDistance
				if(receivedCommand.StartsWith("/MES.Settings.General.SpawnedVoxelMinimumGridDistance.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.SpawnedVoxelMinimumGridDistance.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.General.SpawnedVoxelMinimumGridDistance = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//NpcGridNameBlacklist.Add
				if(receivedCommand.StartsWith("/MES.Settings.General.NpcGridNameBlacklist.Add.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.NpcGridNameBlacklist.Add.", "");
				
					
					if(string.IsNullOrEmpty(receivedValue) == true){
						
						return "No Value Provided.";
						
					}
					
					var blacklist = new List<string>(Settings.General.NpcGridNameBlacklist.ToList());
					
					if(blacklist.Contains(receivedValue) == true){
						
						return "Grid Name Blacklist Already Contains Value: " + receivedValue;
						
					}
					
					blacklist.Add(receivedValue);
					Settings.General.NpcGridNameBlacklist = blacklist.ToArray();
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//NpcGridNameBlacklist.Remove
				if(receivedCommand.StartsWith("/MES.Settings.General.NpcGridNameBlacklist.Remove.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.NpcGridNameBlacklist.Remove.", "");
				
					
					if(string.IsNullOrEmpty(receivedValue) == true){
						
						return "No Value Provided.";
						
					}
					
					var blacklist = new List<string>(Settings.General.NpcGridNameBlacklist.ToList());
					
					if(blacklist.Contains(receivedValue) == false){
						
						return "Grid Name Blacklist Does Not Contain Value: " + receivedValue;
						
					}
					
					blacklist.Remove(receivedValue);
					Settings.General.NpcGridNameBlacklist = blacklist.ToArray();
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//PlanetSpawnsDisableList.Add
				if(receivedCommand.StartsWith("/MES.Settings.General.PlanetSpawnsDisableList.Add.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.PlanetSpawnsDisableList.Add.", "");
				
					
					if(string.IsNullOrEmpty(receivedValue) == true){
						
						return "No Value Provided.";
						
					}
					
					var blacklist = new List<string>(Settings.General.PlanetSpawnsDisableList.ToList());
					
					if(blacklist.Contains(receivedValue) == true){
						
						return "Restricted Planets Already Contains Value: " + receivedValue;
						
					}
					
					blacklist.Add(receivedValue);
					Settings.General.PlanetSpawnsDisableList = blacklist.ToArray();
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//PlanetSpawnsDisableList.Remove
				if(receivedCommand.StartsWith("/MES.Settings.General.PlanetSpawnsDisableList.Remove.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.PlanetSpawnsDisableList.Remove.", "");
				
					
					if(string.IsNullOrEmpty(receivedValue) == true){
						
						return "No Value Provided.";
						
					}
					
					var blacklist = new List<string>(Settings.General.PlanetSpawnsDisableList.ToList());
					
					if(blacklist.Contains(receivedValue) == false){
						
						return "Restricted Planets Does Not Contain Value: " + receivedValue;
						
					}
					
					blacklist.Remove(receivedValue);
					Settings.General.PlanetSpawnsDisableList = blacklist.ToArray();
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//NpcSpawnGroupBlacklist.Add
				if(receivedCommand.StartsWith("/MES.Settings.General.NpcSpawnGroupBlacklist.Add.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.NpcSpawnGroupBlacklist.Add.", "");
				
					
					if(string.IsNullOrEmpty(receivedValue) == true){
						
						return "No Value Provided.";
						
					}
					
					var blacklist = new List<string>(Settings.General.NpcSpawnGroupBlacklist.ToList());
					
					if(blacklist.Contains(receivedValue) == true){
						
						return "SpawnGroup Blacklist Already Contains Value: " + receivedValue;
						
					}
					
					blacklist.Add(receivedValue);
					Settings.General.NpcSpawnGroupBlacklist = blacklist.ToArray();
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//NpcSpawnGroupBlacklist.Remove
				if(receivedCommand.StartsWith("/MES.Settings.General.NpcSpawnGroupBlacklist.Remove.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.NpcSpawnGroupBlacklist.Remove.", "");
				
					
					if(string.IsNullOrEmpty(receivedValue) == true){
						
						return "No Value Provided.";
						
					}
					
					var blacklist = new List<string>(Settings.General.NpcSpawnGroupBlacklist.ToList());
					
					if(blacklist.Contains(receivedValue) == false){
						
						return "SpawnGroup Blacklist Does Not Contain Value: " + receivedValue;
						
					}
					
					blacklist.Remove(receivedValue);
					Settings.General.NpcSpawnGroupBlacklist = blacklist.ToArray();
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//WeaponReplacerBlacklist.Add
				if(receivedCommand.StartsWith("/MES.Settings.General.WeaponReplacerBlacklist.Add.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.WeaponReplacerBlacklist.Add.", "");
				
					
					if(string.IsNullOrEmpty(receivedValue) == true){
						
						return "No Value Provided.";
						
					}
					
					var blacklist = new List<string>(Settings.General.WeaponReplacerBlacklist.ToList());
					
					if(blacklist.Contains(receivedValue) == true){
						
						return "Weapon Randomizer Blacklist Already Contains Value: " + receivedValue;
						
					}
					
					blacklist.Add(receivedValue);
					Settings.General.WeaponReplacerBlacklist = blacklist.ToArray();
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//WeaponReplacerBlacklist.Remove
				if(receivedCommand.StartsWith("/MES.Settings.General.WeaponReplacerBlacklist.Remove.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.WeaponReplacerBlacklist.Remove.", "");
				
					
					if(string.IsNullOrEmpty(receivedValue) == true){
						
						return "No Value Provided.";
						
					}
					
					var blacklist = new List<string>(Settings.General.WeaponReplacerBlacklist.ToList());
					
					if(blacklist.Contains(receivedValue) == false){
						
						return "Weapon Randomizer Blacklist Does Not Contain Value: " + receivedValue;
						
					}
					
					blacklist.Remove(receivedValue);
					Settings.General.WeaponReplacerBlacklist = blacklist.ToArray();
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//WeaponReplacerWhitelist.Add
				if(receivedCommand.StartsWith("/MES.Settings.General.WeaponReplacerWhitelist.Add.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.WeaponReplacerWhitelist.Add.", "");
				
					
					if(string.IsNullOrEmpty(receivedValue) == true){
						
						return "No Value Provided.";
						
					}
					
					var Whitelist = new List<string>(Settings.General.WeaponReplacerWhitelist.ToList());
					
					if(Whitelist.Contains(receivedValue) == true){
						
						return "Weapon Randomizer Whitelist Already Contains Value: " + receivedValue;
						
					}
					
					Whitelist.Add(receivedValue);
					Settings.General.WeaponReplacerWhitelist = Whitelist.ToArray();
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//WeaponReplacerWhitelist.Remove
				if(receivedCommand.StartsWith("/MES.Settings.General.WeaponReplacerWhitelist.Remove.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.WeaponReplacerWhitelist.Remove.", "");
				
					
					if(string.IsNullOrEmpty(receivedValue) == true){
						
						return "No Value Provided.";
						
					}
					
					var Whitelist = new List<string>(Settings.General.WeaponReplacerWhitelist.ToList());
					
					if(Whitelist.Contains(receivedValue) == false){
						
						return "Weapon Randomizer Whitelist Does Not Contain Value: " + receivedValue;
						
					}
					
					Whitelist.Remove(receivedValue);
					Settings.General.WeaponReplacerWhitelist = Whitelist.ToArray();
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//WeaponReplacerTargetBlacklist.Add
				if(receivedCommand.StartsWith("/MES.Settings.General.WeaponReplacerTargetBlacklist.Add.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.WeaponReplacerTargetBlacklist.Add.", "");
				
					
					if(string.IsNullOrEmpty(receivedValue) == true){
						
						return "No Value Provided.";
						
					}
					
					var blacklist = new List<string>(Settings.General.WeaponReplacerTargetBlacklist.ToList());
					
					if(blacklist.Contains(receivedValue) == true){
						
						return "Weapon Randomizer Target Blacklist Already Contains Value: " + receivedValue;
						
					}
					
					blacklist.Add(receivedValue);
					Settings.General.WeaponReplacerTargetBlacklist = blacklist.ToArray();
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//WeaponReplacerTargetBlacklist.Remove
				if(receivedCommand.StartsWith("/MES.Settings.General.WeaponReplacerTargetBlacklist.Remove.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.WeaponReplacerTargetBlacklist.Remove.", "");
				
					
					if(string.IsNullOrEmpty(receivedValue) == true){
						
						return "No Value Provided.";
						
					}
					
					var blacklist = new List<string>(Settings.General.WeaponReplacerTargetBlacklist.ToList());
					
					if(blacklist.Contains(receivedValue) == false){
						
						return "Weapon Randomizer Target Blacklist Does Not Contain Value: " + receivedValue;
						
					}
					
					blacklist.Remove(receivedValue);
					Settings.General.WeaponReplacerTargetBlacklist = blacklist.ToArray();
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//WeaponReplacerTargetWhitelist.Add
				if(receivedCommand.StartsWith("/MES.Settings.General.WeaponReplacerTargetWhitelist.Add.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.WeaponReplacerTargetWhitelist.Add.", "");
				
					
					if(string.IsNullOrEmpty(receivedValue) == true){
						
						return "No Value Provided.";
						
					}
					
					var Whitelist = new List<string>(Settings.General.WeaponReplacerTargetWhitelist.ToList());
					
					if(Whitelist.Contains(receivedValue) == true){
						
						return "Weapon Randomizer Target Whitelist Already Contains Value: " + receivedValue;
						
					}
					
					Whitelist.Add(receivedValue);
					Settings.General.WeaponReplacerTargetWhitelist = Whitelist.ToArray();
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}
				
				//WeaponReplacerTargetWhitelist.Remove
				if(receivedCommand.StartsWith("/MES.Settings.General.WeaponReplacerTargetWhitelist.Remove.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.General.WeaponReplacerTargetWhitelist.Remove.", "");
				
					
					if(string.IsNullOrEmpty(receivedValue) == true){
						
						return "No Value Provided.";
						
					}
					
					var Whitelist = new List<string>(Settings.General.WeaponReplacerTargetWhitelist.ToList());
					
					if(Whitelist.Contains(receivedValue) == false){
						
						return "Weapon Randomizer Target Whitelist Does Not Contain Value: " + receivedValue;
						
					}
					
					Whitelist.Remove(receivedValue);
					Settings.General.WeaponReplacerTargetWhitelist = Whitelist.ToArray();
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;
					
				}

				//UseGlobalBlockReplacer
				if(receivedCommand.StartsWith("/MES.Settings.General.UseGlobalBlockReplacer.") == true) {

					var receivedValue = receivedCommand.Replace("/MES.Settings.General.UseGlobalBlockReplacer.", "");
					bool result = false;

					if(bool.TryParse(receivedValue, out result) == false) {

						return "Failed To Parse Value: " + receivedValue;

					}

					Settings.General.UseGlobalBlockReplacer = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;

				}

				//GlobalBlockReplacerReference.Add
				if(receivedCommand.StartsWith("/MES.Settings.General.GlobalBlockReplacerReference.Add.") == true) {

					var receivedValue = receivedCommand.Replace("/MES.Settings.General.GlobalBlockReplacerReference.Add.", "");


					if(string.IsNullOrEmpty(receivedValue) == true) {

						return "No Value Provided.";

					}

					var Whitelist = new List<string>(Settings.General.GlobalBlockReplacerReference.ToList());

					if(Whitelist.Contains(receivedValue) == true) {

						return "Weapon Randomizer Target Whitelist Already Contains Value: " + receivedValue;

					}

					Whitelist.Add(receivedValue);
					Settings.General.GlobalBlockReplacerReference = Whitelist.ToArray();
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;

				}

				//GlobalBlockReplacerReference.Remove
				if(receivedCommand.StartsWith("/MES.Settings.General.GlobalBlockReplacerReference.Remove.") == true) {

					var receivedValue = receivedCommand.Replace("/MES.Settings.General.GlobalBlockReplacerReference.Remove.", "");


					if(string.IsNullOrEmpty(receivedValue) == true) {

						return "No Value Provided.";

					}

					var Whitelist = new List<string>(Settings.General.GlobalBlockReplacerReference.ToList());

					if(Whitelist.Contains(receivedValue) == false) {

						return "Weapon Randomizer Target Whitelist Does Not Contain Value: " + receivedValue;

					}

					Whitelist.Remove(receivedValue);
					Settings.General.GlobalBlockReplacerReference = Whitelist.ToArray();
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;

				}

				//GlobalBlockReplacerProfiles.Add
				if(receivedCommand.StartsWith("/MES.Settings.General.GlobalBlockReplacerProfiles.Add.") == true) {

					var receivedValue = receivedCommand.Replace("/MES.Settings.General.GlobalBlockReplacerProfiles.Add.", "");


					if(string.IsNullOrEmpty(receivedValue) == true) {

						return "No Value Provided.";

					}

					var Whitelist = new List<string>(Settings.General.GlobalBlockReplacerProfiles.ToList());

					if(Whitelist.Contains(receivedValue) == true) {

						return "Weapon Randomizer Target Whitelist Already Contains Value: " + receivedValue;

					}

					Whitelist.Add(receivedValue);
					Settings.General.GlobalBlockReplacerProfiles = Whitelist.ToArray();
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;

				}

				//GlobalBlockReplacerProfiles.Remove
				if(receivedCommand.StartsWith("/MES.Settings.General.GlobalBlockReplacerProfiles.Remove.") == true) {

					var receivedValue = receivedCommand.Replace("/MES.Settings.General.GlobalBlockReplacerProfiles.Remove.", "");


					if(string.IsNullOrEmpty(receivedValue) == true) {

						return "No Value Provided.";

					}

					var Whitelist = new List<string>(Settings.General.GlobalBlockReplacerProfiles.ToList());

					if(Whitelist.Contains(receivedValue) == false) {

						return "Weapon Randomizer Target Whitelist Does Not Contain Value: " + receivedValue;

					}

					Whitelist.Remove(receivedValue);
					Settings.General.GlobalBlockReplacerProfiles = Whitelist.ToArray();
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;

				}

				//UseNonPhysicalAmmoForNPCs
				if(receivedCommand.StartsWith("/MES.Settings.General.UseNonPhysicalAmmoForNPCs.") == true) {

					var receivedValue = receivedCommand.Replace("/MES.Settings.General.UseNonPhysicalAmmoForNPCs.", "");
					bool result = false;

					if(bool.TryParse(receivedValue, out result) == false) {

						return "Failed To Parse Value: " + receivedValue;

					}

					Settings.General.UseNonPhysicalAmmoForNPCs = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;

				}

				//RemoveContainerInventoryFromNPCs
				if(receivedCommand.StartsWith("/MES.Settings.General.RemoveContainerInventoryFromNPCs.") == true) {

					var receivedValue = receivedCommand.Replace("/MES.Settings.General.RemoveContainerInventoryFromNPCs.", "");
					bool result = false;

					if(bool.TryParse(receivedValue, out result) == false) {

						return "Failed To Parse Value: " + receivedValue;

					}

					Settings.General.RemoveContainerInventoryFromNPCs = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;

				}

				//UseEconomyBuyingReputationIncrease
				if(receivedCommand.StartsWith("/MES.Settings.General.UseEconomyBuyingReputationIncrease.") == true) {

					var receivedValue = receivedCommand.Replace("/MES.Settings.General.UseEconomyBuyingReputationIncrease.", "");
					bool result = false;

					if(bool.TryParse(receivedValue, out result) == false) {

						return "Failed To Parse Value: " + receivedValue;

					}

					Settings.General.UseEconomyBuyingReputationIncrease = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;

				}

				//EconomyBuyingReputationCostAmount
				if(receivedCommand.StartsWith("/MES.Settings.General.EconomyBuyingReputationCostAmount.") == true) {

					var receivedValue = receivedCommand.Replace("/MES.Settings.General.EconomyBuyingReputationCostAmount.", "");
					long result = 0;

					if(long.TryParse(receivedValue, out result) == false) {

						return "Failed To Parse Value: " + receivedValue;

					}

					Settings.General.EconomyBuyingReputationCostAmount = result;
					var saveSetting = Settings.General.SaveSettings(Settings.General);
					return saveSetting;

				}

			}
			////////////////////////////////////////////////////////
			//                   SpaceCargoShips
			////////////////////////////////////////////////////////

			if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.") == true){
				
				//FirstSpawnTime
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.FirstSpawnTime.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.FirstSpawnTime.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.FirstSpawnTime = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//MinSpawnTime
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.MinSpawnTime.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.MinSpawnTime.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.MinSpawnTime = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//MaxSpawnTime
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.MaxSpawnTime.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.MaxSpawnTime.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.MaxSpawnTime = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//MaxSpawnAttempts
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.MaxSpawnAttempts.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.MaxSpawnAttempts.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.MaxSpawnAttempts = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//MinPathDistanceFromPlayer
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.MinPathDistanceFromPlayer.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.MinPathDistanceFromPlayer.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.MinPathDistanceFromPlayer = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//MaxPathDistanceFromPlayer
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.MaxPathDistanceFromPlayer.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.MaxPathDistanceFromPlayer.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.MaxPathDistanceFromPlayer = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//MinLunarSpawnHeight
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.MinLunarSpawnHeight.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.MinLunarSpawnHeight.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.MinLunarSpawnHeight = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//MaxLunarSpawnHeight
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.MaxLunarSpawnHeight.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.MaxLunarSpawnHeight.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.MaxLunarSpawnHeight = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//MinSpawnDistFromEntities
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.MinSpawnDistFromEntities.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.MinSpawnDistFromEntities.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.MinSpawnDistFromEntities = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//MinPathDistance
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.MinPathDistance.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.MinPathDistance.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.MinPathDistance = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//MaxPathDistance
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.MaxPathDistance.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.MaxPathDistance.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.MaxPathDistance = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//PathCheckStep
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.PathCheckStep.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.PathCheckStep.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.PathCheckStep = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//DespawnDistanceFromEndPath
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.DespawnDistanceFromEndPath.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.DespawnDistanceFromEndPath.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.DespawnDistanceFromEndPath = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//DespawnDistanceFromPlayer
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.DespawnDistanceFromPlayer.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.DespawnDistanceFromPlayer.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.DespawnDistanceFromPlayer = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//UseMinimumSpeed
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.UseMinimumSpeed.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.UseMinimumSpeed.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.UseMinimumSpeed = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//MinimumSpeed
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.MinimumSpeed.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.MinimumSpeed.", "");
					float result = 0;
					
					if(float.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.MinimumSpeed = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//UseSpeedOverride
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.UseSpeedOverride.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.UseSpeedOverride.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.UseSpeedOverride = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//SpeedOverride
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.SpeedOverride.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.SpeedOverride.", "");
					float result = 0;
					
					if(float.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.SpeedOverride = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//UseMaxSpawnGroupFrequency
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.UseMaxSpawnGroupFrequency.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.UseMaxSpawnGroupFrequency.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.UseMaxSpawnGroupFrequency = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//MaxSpawnGroupFrequency
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.MaxSpawnGroupFrequency.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.MaxSpawnGroupFrequency.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.MaxSpawnGroupFrequency = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//EnableWaveSpawner
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.EnableWaveSpawner.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.EnableWaveSpawner.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.EnableWaveSpawner = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//UseSpecificRandomGroups.Add
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.UseSpecificRandomGroups.Add.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.UseSpecificRandomGroups.Add.", "");
				
					
					if(string.IsNullOrEmpty(receivedValue) == true){
						
						return "No Value Provided.";
						
					}
					
					var blacklist = new List<string>(Settings.SpaceCargoShips.UseSpecificRandomGroups.ToList());
					
					if(blacklist.Contains(receivedValue) == true){
						
						return "Grid Name Blacklist Already Contains Value: " + receivedValue;
						
					}
					
					blacklist.Add(receivedValue);
					Settings.SpaceCargoShips.UseSpecificRandomGroups = blacklist.ToArray();
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//UseSpecificRandomGroups.Remove
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.UseSpecificRandomGroups.Remove.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.UseSpecificRandomGroups.Remove.", "");
				
					
					if(string.IsNullOrEmpty(receivedValue) == true){
						
						return "No Value Provided.";
						
					}
					
					var blacklist = new List<string>(Settings.SpaceCargoShips.UseSpecificRandomGroups.ToList());
					
					if(blacklist.Contains(receivedValue) == false){
						
						return "Grid Name Blacklist Does Not Contain Value: " + receivedValue;
						
					}
					
					blacklist.Remove(receivedValue);
					Settings.SpaceCargoShips.UseSpecificRandomGroups = blacklist.ToArray();
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//MinWaveSpawnTime
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.MinWaveSpawnTime.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.MinWaveSpawnTime.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.MinWaveSpawnTime = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//MaxWaveSpawnTime
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.MaxWaveSpawnTime.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.MaxWaveSpawnTime.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.MaxWaveSpawnTime = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//TotalSpawnEventsPerCluster
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.TotalSpawnEventsPerCluster.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.TotalSpawnEventsPerCluster.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.TotalSpawnEventsPerCluster = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//TimeBetweenWaveSpawns
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.TimeBetweenWaveSpawns.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.TimeBetweenWaveSpawns.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.TimeBetweenWaveSpawns = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				//PlayerClusterDistance
				if(receivedCommand.StartsWith("/MES.Settings.SpaceCargoShips.PlayerClusterDistance.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.SpaceCargoShips.PlayerClusterDistance.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.SpaceCargoShips.PlayerClusterDistance = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				
			}

			////////////////////////////////////////////////////////
			//                   RandomEncounters
			////////////////////////////////////////////////////////
			
			if(receivedCommand.StartsWith("/MES.Settings.RandomEncounters.") == true){
				
				//PlayerSpawnCooldown
				if(receivedCommand.StartsWith("/MES.Settings.RandomEncounters.PlayerSpawnCooldown.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.RandomEncounters.PlayerSpawnCooldown.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.RandomEncounters.PlayerSpawnCooldown = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				//SpawnTimerTrigger
				if(receivedCommand.StartsWith("/MES.Settings.RandomEncounters.SpawnTimerTrigger.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.RandomEncounters.SpawnTimerTrigger.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.RandomEncounters.SpawnTimerTrigger = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				//PlayerTravelDistance
				if(receivedCommand.StartsWith("/MES.Settings.RandomEncounters.PlayerTravelDistance.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.RandomEncounters.PlayerTravelDistance.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.RandomEncounters.PlayerTravelDistance = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				//MaxShipsPerArea
				if(receivedCommand.StartsWith("/MES.Settings.RandomEncounters.MaxShipsPerArea.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.RandomEncounters.MaxShipsPerArea.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.RandomEncounters.MaxShipsPerArea = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				//AreaSize
				if(receivedCommand.StartsWith("/MES.Settings.RandomEncounters.AreaSize.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.RandomEncounters.AreaSize.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.RandomEncounters.AreaSize = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				//MinSpawnDistanceFromPlayer
				if(receivedCommand.StartsWith("/MES.Settings.RandomEncounters.MinSpawnDistanceFromPlayer.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.RandomEncounters.MinSpawnDistanceFromPlayer.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.RandomEncounters.MinSpawnDistanceFromPlayer = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				//MaxSpawnDistanceFromPlayer
				if(receivedCommand.StartsWith("/MES.Settings.RandomEncounters.MaxSpawnDistanceFromPlayer.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.RandomEncounters.MaxSpawnDistanceFromPlayer.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.RandomEncounters.MaxSpawnDistanceFromPlayer = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				//MinDistanceFromOtherEntities
				if(receivedCommand.StartsWith("/MES.Settings.RandomEncounters.MinDistanceFromOtherEntities.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.RandomEncounters.MinDistanceFromOtherEntities.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.RandomEncounters.MinDistanceFromOtherEntities = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				//SpawnAttempts
				if(receivedCommand.StartsWith("/MES.Settings.RandomEncounters.SpawnAttempts.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.RandomEncounters.SpawnAttempts.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.RandomEncounters.SpawnAttempts = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				//UseMaxSpawnGroupFrequency
				if(receivedCommand.StartsWith("/MES.Settings.RandomEncounters.UseMaxSpawnGroupFrequency.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.RandomEncounters.UseMaxSpawnGroupFrequency.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.RandomEncounters.UseMaxSpawnGroupFrequency = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				//MaxSpawnGroupFrequency
				if(receivedCommand.StartsWith("/MES.Settings.RandomEncounters.MaxSpawnGroupFrequency.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.RandomEncounters.MaxSpawnGroupFrequency.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.RandomEncounters.MaxSpawnGroupFrequency = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				//DespawnDistanceFromPlayer
				if(receivedCommand.StartsWith("/MES.Settings.RandomEncounters.DespawnDistanceFromPlayer.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.RandomEncounters.DespawnDistanceFromPlayer.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.RandomEncounters.DespawnDistanceFromPlayer = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
			}
			
			////////////////////////////////////////////////////////
			//                   BossEncounters
			////////////////////////////////////////////////////////
			
			if(receivedCommand.StartsWith("/MES.Settings.BossEncounters.") == true){
				
				//PlayerSpawnCooldown
				if(receivedCommand.StartsWith("/MES.Settings.BossEncounters.PlayerSpawnCooldown.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.BossEncounters.PlayerSpawnCooldown.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.BossEncounters.PlayerSpawnCooldown = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				//SpawnTimerTrigger
				if(receivedCommand.StartsWith("/MES.Settings.BossEncounters.SpawnTimerTrigger.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.BossEncounters.SpawnTimerTrigger.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.BossEncounters.SpawnTimerTrigger = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				//SignalActiveTimer
				if(receivedCommand.StartsWith("/MES.Settings.BossEncounters.SignalActiveTimer.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.BossEncounters.SignalActiveTimer.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.BossEncounters.SignalActiveTimer = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				//MaxShipsPerArea
				if(receivedCommand.StartsWith("/MES.Settings.BossEncounters.MaxShipsPerArea.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.BossEncounters.MaxShipsPerArea.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.BossEncounters.MaxShipsPerArea = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				//AreaSize
				if(receivedCommand.StartsWith("/MES.Settings.BossEncounters.AreaSize.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.BossEncounters.AreaSize.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.BossEncounters.AreaSize = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				//TriggerDistance
				if(receivedCommand.StartsWith("/MES.Settings.BossEncounters.TriggerDistance.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.BossEncounters.TriggerDistance.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.BossEncounters.TriggerDistance = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				//PathCalculationAttempts
				if(receivedCommand.StartsWith("/MES.Settings.BossEncounters.PathCalculationAttempts.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.BossEncounters.PathCalculationAttempts.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.BossEncounters.PathCalculationAttempts = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				//MinCoordsDistanceSpace
				if(receivedCommand.StartsWith("/MES.Settings.BossEncounters.MinCoordsDistanceSpace.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.BossEncounters.MinCoordsDistanceSpace.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.BossEncounters.MinCoordsDistanceSpace = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				//MaxCoordsDistanceSpace
				if(receivedCommand.StartsWith("/MES.Settings.BossEncounters.MaxCoordsDistanceSpace.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.BossEncounters.MaxCoordsDistanceSpace.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.BossEncounters.MaxCoordsDistanceSpace = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				//MinCoordsDistancePlanet
				if(receivedCommand.StartsWith("/MES.Settings.BossEncounters.MinCoordsDistancePlanet.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.BossEncounters.MinCoordsDistancePlanet.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.BossEncounters.MinCoordsDistancePlanet = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				//MaxCoordsDistancePlanet
				if(receivedCommand.StartsWith("/MES.Settings.BossEncounters.MaxCoordsDistancePlanet.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.BossEncounters.MaxCoordsDistancePlanet.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.BossEncounters.MaxCoordsDistancePlanet = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				//PlayersWithinDistance
				if(receivedCommand.StartsWith("/MES.Settings.BossEncounters.PlayersWithinDistance.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.BossEncounters.PlayersWithinDistance.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.BossEncounters.PlayersWithinDistance = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				//MinPlanetAltitude
				if(receivedCommand.StartsWith("/MES.Settings.BossEncounters.MinPlanetAltitude.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.BossEncounters.MinPlanetAltitude.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.BossEncounters.MinPlanetAltitude = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				//MinSignalDistFromOtherEntities
				if(receivedCommand.StartsWith("/MES.Settings.BossEncounters.MinSignalDistFromOtherEntities.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.BossEncounters.MinSignalDistFromOtherEntities.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.BossEncounters.MinSignalDistFromOtherEntities = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				//MinSpawnDistFromCoords
				if(receivedCommand.StartsWith("/MES.Settings.BossEncounters.MinSpawnDistFromCoords.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.BossEncounters.MinSpawnDistFromCoords.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.BossEncounters.MinSpawnDistFromCoords = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				//MaxSpawnDistFromCoords
				if(receivedCommand.StartsWith("/MES.Settings.BossEncounters.MaxSpawnDistFromCoords.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.BossEncounters.MaxSpawnDistFromCoords.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.BossEncounters.MaxSpawnDistFromCoords = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				//MinAirDensity
				if(receivedCommand.StartsWith("/MES.Settings.BossEncounters.MinAirDensity.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.BossEncounters.MinAirDensity.", "");
					float result = 0;
					
					if(float.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.BossEncounters.MinAirDensity = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				//UseMaxSpawnGroupFrequency
				if(receivedCommand.StartsWith("/MES.Settings.BossEncounters.UseMaxSpawnGroupFrequency.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.BossEncounters.UseMaxSpawnGroupFrequency.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.BossEncounters.UseMaxSpawnGroupFrequency = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				//MaxSpawnGroupFrequency
				if(receivedCommand.StartsWith("/MES.Settings.BossEncounters.MaxSpawnGroupFrequency.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.BossEncounters.MaxSpawnGroupFrequency.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.BossEncounters.MaxSpawnGroupFrequency = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				//DespawnDistanceFromPlayer
				if(receivedCommand.StartsWith("/MES.Settings.BossEncounters.DespawnDistanceFromPlayer.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.BossEncounters.DespawnDistanceFromPlayer.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.BossEncounters.DespawnDistanceFromPlayer = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}

				
			}
			
			
			////////////////////////////////////////////////////////
			//                   PlanetaryCargoShips
			////////////////////////////////////////////////////////
			
			if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.") == true){
				
				//FirstSpawnTime
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.FirstSpawnTime.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.FirstSpawnTime.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.FirstSpawnTime = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//MinSpawnTime
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.MinSpawnTime.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.MinSpawnTime.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.MinSpawnTime = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//MaxSpawnTime
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.MaxSpawnTime.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.MaxSpawnTime.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.MaxSpawnTime = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//MaxShipsPerArea
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.MaxShipsPerArea.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.MaxShipsPerArea.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.MaxShipsPerArea = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//AreaSize
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.AreaSize.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.AreaSize.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.AreaSize = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//MaxSpawnAttempts
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.MaxSpawnAttempts.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.MaxSpawnAttempts.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.MaxSpawnAttempts = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//PlayerSurfaceAltitude
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.PlayerSurfaceAltitude.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.PlayerSurfaceAltitude.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.PlayerSurfaceAltitude = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//MinPathDistanceFromPlayer
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.MinPathDistanceFromPlayer.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.MinPathDistanceFromPlayer.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.MinPathDistanceFromPlayer = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//MaxPathDistanceFromPlayer
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.MaxPathDistanceFromPlayer.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.MaxPathDistanceFromPlayer.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.MaxPathDistanceFromPlayer = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//MinSpawnFromGrids
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.MinSpawnFromGrids.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.MinSpawnFromGrids.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.MinSpawnFromGrids = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//MinAirDensity
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.MinAirDensity.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.MinAirDensity.", "");
					float result = 0;
					
					if(float.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.MinAirDensity = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//MinSpawningAltitude
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.MinSpawningAltitude.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.MinSpawningAltitude.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.MinSpawningAltitude = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//MaxSpawningAltitude
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.MaxSpawningAltitude.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.MaxSpawningAltitude.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.MaxSpawningAltitude = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//MinPathAltitude
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.MinPathAltitude.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.MinPathAltitude.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.MinPathAltitude = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//MinPathDistance
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.MinPathDistance.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.MinPathDistance.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.MinPathDistance = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//MaxPathDistance
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.MaxPathDistance.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.MaxPathDistance.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.MaxPathDistance = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//PathStepCheckDistance
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.PathStepCheckDistance.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.PathStepCheckDistance.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.PathStepCheckDistance = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//DespawnDistanceFromEndPath
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.DespawnDistanceFromEndPath.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.DespawnDistanceFromEndPath.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.DespawnDistanceFromEndPath = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//DespawnDistanceFromPlayer
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.DespawnDistanceFromPlayer.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.DespawnDistanceFromPlayer.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.DespawnDistanceFromPlayer = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//UseMinimumSpeed
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.UseMinimumSpeed.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.UseMinimumSpeed.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.UseMinimumSpeed = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//MinimumSpeed
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.MinimumSpeed.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.MinimumSpeed.", "");
					float result = 0;
					
					if(float.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.MinimumSpeed = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//UseSpeedOverride
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.UseSpeedOverride.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.UseSpeedOverride.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.UseSpeedOverride = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//SpeedOverride
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.SpeedOverride.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.SpeedOverride.", "");
					float result = 0;
					
					if(float.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.SpeedOverride = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//UseMaxSpawnGroupFrequency
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.UseMaxSpawnGroupFrequency.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.UseMaxSpawnGroupFrequency.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.UseMaxSpawnGroupFrequency = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				//MaxSpawnGroupFrequency
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryCargoShips.MaxSpawnGroupFrequency.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryCargoShips.MaxSpawnGroupFrequency.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryCargoShips.MaxSpawnGroupFrequency = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
			}
			
			////////////////////////////////////////////////////////
			//                   PlanetaryInstallations
			////////////////////////////////////////////////////////
			
			if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.") == true){
				
				//PlayerSpawnCooldown
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.PlayerSpawnCooldown.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.PlayerSpawnCooldown.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.PlayerSpawnCooldown = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//SpawnTimerTrigger
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.SpawnTimerTrigger.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.SpawnTimerTrigger.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.SpawnTimerTrigger = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//PlayerDistanceSpawnTrigger
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.PlayerDistanceSpawnTrigger.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.PlayerDistanceSpawnTrigger.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.PlayerDistanceSpawnTrigger = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//MaxShipsPerArea
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.MaxShipsPerArea.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.MaxShipsPerArea.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.MaxShipsPerArea = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//AreaSize
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.AreaSize.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.AreaSize.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.AreaSize = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//PlayerMaximumDistanceFromSurface
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.PlayerMaximumDistanceFromSurface.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.PlayerMaximumDistanceFromSurface.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.PlayerMaximumDistanceFromSurface = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//MinimumSpawnDistanceFromPlayers
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.MinimumSpawnDistanceFromPlayers.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.MinimumSpawnDistanceFromPlayers.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.MinimumSpawnDistanceFromPlayers = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//MaximumSpawnDistanceFromPlayers
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.MaximumSpawnDistanceFromPlayers.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.MaximumSpawnDistanceFromPlayers.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.MaximumSpawnDistanceFromPlayers = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//AggressivePathCheck
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.AggressivePathCheck.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.AggressivePathCheck.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.AggressivePathCheck = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//SearchPathIncrement
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.SearchPathIncrement.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.SearchPathIncrement.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.SearchPathIncrement = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//MinimumSpawnDistanceFromOtherGrids
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.MinimumSpawnDistanceFromOtherGrids.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.MinimumSpawnDistanceFromOtherGrids.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.MinimumSpawnDistanceFromOtherGrids = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//MinimumTerrainVariance
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.MinimumTerrainVariance.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.MinimumTerrainVariance.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.MinimumTerrainVariance = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//MaximumTerrainVariance
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.MaximumTerrainVariance.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.MaximumTerrainVariance.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.MaximumTerrainVariance = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//AggressiveTerrainCheck
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.AggressiveTerrainCheck.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.AggressiveTerrainCheck.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.AggressiveTerrainCheck = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//TerrainCheckIncrementDistance
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.TerrainCheckIncrementDistance.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.TerrainCheckIncrementDistance.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.TerrainCheckIncrementDistance = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//SmallTerrainCheckDistance
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.SmallTerrainCheckDistance.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.SmallTerrainCheckDistance.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.SmallTerrainCheckDistance = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//MediumSpawnChanceBaseValue
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.MediumSpawnChanceBaseValue.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.MediumSpawnChanceBaseValue.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.MediumSpawnChanceBaseValue = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//MediumSpawnChanceIncrement
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.MediumSpawnChanceIncrement.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.MediumSpawnChanceIncrement.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.MediumSpawnChanceIncrement = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//MediumSpawnDistanceIncrement
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.MediumSpawnDistanceIncrement.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.MediumSpawnDistanceIncrement.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.MediumSpawnDistanceIncrement = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//MediumTerrainCheckDistance
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.MediumTerrainCheckDistance.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.MediumTerrainCheckDistance.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.MediumTerrainCheckDistance = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//LargeSpawnChanceBaseValue
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.LargeSpawnChanceBaseValue.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.LargeSpawnChanceBaseValue.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.LargeSpawnChanceBaseValue = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//LargeSpawnChanceIncrement
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.LargeSpawnChanceIncrement.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.LargeSpawnChanceIncrement.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.LargeSpawnChanceIncrement = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//LargeSpawnDistanceIncrement
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.LargeSpawnDistanceIncrement.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.LargeSpawnDistanceIncrement.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.LargeSpawnDistanceIncrement = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//LargeTerrainCheckDistance
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.LargeTerrainCheckDistance.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.LargeTerrainCheckDistance.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.LargeTerrainCheckDistance = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//UseMaxSpawnGroupFrequency
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.UseMaxSpawnGroupFrequency.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.UseMaxSpawnGroupFrequency.", "");
					bool result = false;
					
					if(bool.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.UseMaxSpawnGroupFrequency = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//MaxSpawnGroupFrequency
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.MaxSpawnGroupFrequency.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.MaxSpawnGroupFrequency.", "");
					int result = 0;
					
					if(int.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.MaxSpawnGroupFrequency = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				//DespawnDistanceFromPlayer
				if(receivedCommand.StartsWith("/MES.Settings.PlanetaryInstallations.DespawnDistanceFromPlayer.") == true){
					
					var receivedValue = receivedCommand.Replace("/MES.Settings.PlanetaryInstallations.DespawnDistanceFromPlayer.", "");
					double result = 0;
					
					if(double.TryParse(receivedValue, out result) == false){
						
						return "Failed To Parse Value: " + receivedValue;
						
					}
					
					Settings.PlanetaryInstallations.DespawnDistanceFromPlayer = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
								
			}
			
			////////////////////////////////////////////////////////
			//                   OtherNPCs
			////////////////////////////////////////////////////////
			
			//DespawnDistanceFromPlayer
			if(receivedCommand.StartsWith("/MES.Settings.OtherNPCs.DespawnDistanceFromPlayer.") == true){
				
				var receivedValue = receivedCommand.Replace("/MES.Settings.OtherNPCs.DespawnDistanceFromPlayer.", "");
				double result = 0;
				
				if(double.TryParse(receivedValue, out result) == false){
					
					return "Failed To Parse Value: " + receivedValue;
					
				}
				
				Settings.OtherNPCs.DespawnDistanceFromPlayer = result;
				var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
				return saveSetting;
				
			}
			
			
			
			////////////////////////////////////////////////////////
			//                   CleanupSettings
			////////////////////////////////////////////////////////
			
			//UseCleanupSettings
			if(receivedCommand.Contains("UseCleanupSettings") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.UseCleanupSettings = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.UseCleanupSettings = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.UseCleanupSettings = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.UseCleanupSettings = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.UseCleanupSettings = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.UseCleanupSettings = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//CleanupUseDistance
			if(receivedCommand.Contains("CleanupUseDistance") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.CleanupUseDistance = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.CleanupUseDistance = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.CleanupUseDistance = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.CleanupUseDistance = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.CleanupUseDistance = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.CleanupUseDistance = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//CleanupUseTimer
			if(receivedCommand.Contains("CleanupUseTimer") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.CleanupUseTimer = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.CleanupUseTimer = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.CleanupUseTimer = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.CleanupUseTimer = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.CleanupUseTimer = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.CleanupUseTimer = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//CleanupUseBlockLimit
			if(receivedCommand.Contains("CleanupUseBlockLimit") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.CleanupUseBlockLimit = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.CleanupUseBlockLimit = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.CleanupUseBlockLimit = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.CleanupUseBlockLimit = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.CleanupUseBlockLimit = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.CleanupUseBlockLimit = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//CleanupDistanceStartsTimer
			if(receivedCommand.Contains("CleanupDistanceStartsTimer") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.CleanupDistanceStartsTimer = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.CleanupDistanceStartsTimer = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.CleanupDistanceStartsTimer = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.CleanupDistanceStartsTimer = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.CleanupDistanceStartsTimer = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.CleanupDistanceStartsTimer = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//CleanupResetTimerWithinDistance
			if(receivedCommand.Contains("CleanupResetTimerWithinDistance") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.CleanupResetTimerWithinDistance = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.CleanupResetTimerWithinDistance = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.CleanupResetTimerWithinDistance = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.CleanupResetTimerWithinDistance = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.CleanupResetTimerWithinDistance = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.CleanupResetTimerWithinDistance = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//CleanupDistanceTrigger
			if(receivedCommand.Contains("CleanupDistanceTrigger") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				double result = 0;
				
				if(double.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.CleanupDistanceTrigger = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.CleanupDistanceTrigger = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.CleanupDistanceTrigger = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.CleanupDistanceTrigger = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.CleanupDistanceTrigger = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.CleanupDistanceTrigger = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			//CleanupTimerTrigger
			if(receivedCommand.Contains("CleanupTimerTrigger") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				int result = 0;
				
				if(int.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.CleanupTimerTrigger = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.CleanupTimerTrigger = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.CleanupTimerTrigger = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.CleanupTimerTrigger = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.CleanupTimerTrigger = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.CleanupTimerTrigger = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//CleanupBlockLimitTrigger
			if(receivedCommand.Contains("CleanupBlockLimitTrigger") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				int result = 0;
				
				if(int.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.CleanupBlockLimitTrigger = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.CleanupBlockLimitTrigger = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.CleanupBlockLimitTrigger = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.CleanupBlockLimitTrigger = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.CleanupBlockLimitTrigger = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.CleanupBlockLimitTrigger = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//CleanupIncludeUnowned
			if(receivedCommand.Contains("CleanupIncludeUnowned") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.CleanupIncludeUnowned = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.CleanupIncludeUnowned = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.CleanupIncludeUnowned = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.CleanupIncludeUnowned = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.CleanupIncludeUnowned = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.CleanupIncludeUnowned = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//CleanupUnpoweredOverride
			if(receivedCommand.Contains("CleanupUnpoweredOverride") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.CleanupUnpoweredOverride = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.CleanupUnpoweredOverride = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.CleanupUnpoweredOverride = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.CleanupUnpoweredOverride = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.CleanupUnpoweredOverride = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.CleanupUnpoweredOverride = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//CleanupUnpoweredDistanceTrigger
			if(receivedCommand.Contains("CleanupUnpoweredDistanceTrigger") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				double result = 0;
				
				if(double.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.CleanupUnpoweredDistanceTrigger = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.CleanupUnpoweredDistanceTrigger = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.CleanupUnpoweredDistanceTrigger = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.CleanupUnpoweredDistanceTrigger = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.CleanupUnpoweredDistanceTrigger = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.CleanupUnpoweredDistanceTrigger = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//CleanupUnpoweredTimerTrigger
			if(receivedCommand.Contains("CleanupUnpoweredTimerTrigger") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				int result = 0;
				
				if(int.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.CleanupUnpoweredTimerTrigger = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.CleanupUnpoweredTimerTrigger = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.CleanupUnpoweredTimerTrigger = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.CleanupUnpoweredTimerTrigger = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.CleanupUnpoweredTimerTrigger = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.CleanupUnpoweredTimerTrigger = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			
			//UseBlockDisable
			if(receivedCommand.Contains("UseBlockDisable") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.UseBlockDisable = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.UseBlockDisable = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.UseBlockDisable = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.UseBlockDisable = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.UseBlockDisable = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.UseBlockDisable = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableAirVent
			if(receivedCommand.Contains("DisableAirVent") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableAirVent = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableAirVent = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableAirVent = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableAirVent = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableAirVent = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableAirVent = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableAntenna
			if(receivedCommand.Contains("DisableAntenna") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableAntenna = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableAntenna = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableAntenna = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableAntenna = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableAntenna = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableAntenna = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableArtificialMass
			if(receivedCommand.Contains("DisableArtificialMass") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableArtificialMass = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableArtificialMass = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableArtificialMass = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableArtificialMass = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableArtificialMass = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableArtificialMass = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableAssembler
			if(receivedCommand.Contains("DisableAssembler") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableAssembler = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableAssembler = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableAssembler = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableAssembler = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableAssembler = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableAssembler = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableBattery
			if(receivedCommand.Contains("DisableBattery") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableBattery = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableBattery = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableBattery = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableBattery = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableBattery = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableBattery = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableBeacon
			if(receivedCommand.Contains("DisableBeacon") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableBeacon = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableBeacon = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableBeacon = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableBeacon = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableBeacon = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableBeacon = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableCollector
			if(receivedCommand.Contains("DisableBeacon") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableBeacon = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableBeacon = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableBeacon = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableBeacon = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableBeacon = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableBeacon = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableConnector
			if(receivedCommand.Contains("DisableConnector") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableConnector = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableConnector = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableConnector = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableConnector = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableConnector = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableConnector = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableConveyorSorter
			if(receivedCommand.Contains("DisableConveyorSorter") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableConveyorSorter = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableConveyorSorter = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableConveyorSorter = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableConveyorSorter = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableConveyorSorter = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableConveyorSorter = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableDecoy
			if(receivedCommand.Contains("DisableDecoy") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableDecoy = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableDecoy = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableDecoy = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableDecoy = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableDecoy = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableDecoy = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableDrill
			if(receivedCommand.Contains("DisableDrill") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableDrill = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableDrill = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableDrill = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableDrill = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableDrill = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableDrill = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
				
			//DisableGasGenerator
			if(receivedCommand.Contains("DisableGasGenerator") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableGasGenerator = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableGasGenerator = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableGasGenerator = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableGasGenerator = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableGasGenerator = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableGasGenerator = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableGasTank
			if(receivedCommand.Contains("DisableGasTank") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableGasTank = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableGasTank = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableGasTank = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableGasTank = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableGasTank = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableGasTank = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableGatlingGun
			if(receivedCommand.Contains("DisableGatlingGun") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableGatlingGun = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableGatlingGun = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableGatlingGun = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableGatlingGun = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableGatlingGun = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableGatlingGun = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableGatlingTurret
			if(receivedCommand.Contains("DisableGatlingTurret") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableGatlingTurret = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableGatlingTurret = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableGatlingTurret = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableGatlingTurret = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableGatlingTurret = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableGatlingTurret = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableGravityGenerator
			if(receivedCommand.Contains("DisableGravityGenerator") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableGravityGenerator = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableGravityGenerator = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableGravityGenerator = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableGravityGenerator = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableGravityGenerator = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableGravityGenerator = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableGrinder
			if(receivedCommand.Contains("DisableGrinder") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableGrinder = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableGrinder = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableGrinder = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableGrinder = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableGrinder = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableGrinder = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableGyro
			if(receivedCommand.Contains("DisableGyro") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableGyro = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableGyro = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableGyro = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableGyro = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableGyro = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableGyro = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableInteriorTurret
			if(receivedCommand.Contains("DisableInteriorTurret") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableInteriorTurret = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableInteriorTurret = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableInteriorTurret = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableInteriorTurret = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableInteriorTurret = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableInteriorTurret = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableJumpDrive
			if(receivedCommand.Contains("DisableJumpDrive") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableJumpDrive = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableJumpDrive = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableJumpDrive = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableJumpDrive = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableJumpDrive = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableJumpDrive = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableLandingGear
			if(receivedCommand.Contains("DisableLandingGear") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableLandingGear = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableLandingGear = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableLandingGear = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableLandingGear = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableLandingGear = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableLandingGear = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableLaserAntenna
			if(receivedCommand.Contains("DisableLaserAntenna") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableLaserAntenna = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableLaserAntenna = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableLaserAntenna = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableLaserAntenna = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableLaserAntenna = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableLaserAntenna = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableLcdPanel
			if(receivedCommand.Contains("DisableLcdPanel") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableLcdPanel = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableLcdPanel = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableLcdPanel = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableLcdPanel = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableLcdPanel = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableLcdPanel = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableLightBlock
			if(receivedCommand.Contains("DisableLightBlock") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableLightBlock = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableLightBlock = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableLightBlock = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableLightBlock = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableLightBlock = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableLightBlock = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableMedicalRoom
			if(receivedCommand.Contains("DisableMedicalRoom") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableMedicalRoom = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableMedicalRoom = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableMedicalRoom = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableMedicalRoom = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableMedicalRoom = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableMedicalRoom = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableMergeBlock
			if(receivedCommand.Contains("DisableMergeBlock") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableMergeBlock = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableMergeBlock = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableMergeBlock = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableMergeBlock = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableMergeBlock = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableMergeBlock = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableMissileTurret
			if(receivedCommand.Contains("DisableMissileTurret") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableMissileTurret = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableMissileTurret = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableMissileTurret = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableMissileTurret = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableMissileTurret = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableMissileTurret = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableOxygenFarm
			if(receivedCommand.Contains("DisableOxygenFarm") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableOxygenFarm = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableOxygenFarm = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableOxygenFarm = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableOxygenFarm = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableOxygenFarm = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableOxygenFarm = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableParachuteHatch
			if(receivedCommand.Contains("DisableParachuteHatch") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableParachuteHatch = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableParachuteHatch = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableParachuteHatch = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableParachuteHatch = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableParachuteHatch = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableParachuteHatch = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisablePiston
			if(receivedCommand.Contains("DisablePiston") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisablePiston = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisablePiston = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisablePiston = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisablePiston = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisablePiston = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisablePiston = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableProgrammableBlock
			if(receivedCommand.Contains("DisableProgrammableBlock") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableProgrammableBlock = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableProgrammableBlock = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableProgrammableBlock = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableProgrammableBlock = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableProgrammableBlock = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableProgrammableBlock = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableProjector
			if(receivedCommand.Contains("DisableProjector") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableProjector = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableProjector = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableProjector = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableProjector = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableProjector = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableProjector = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableReactor
			if(receivedCommand.Contains("DisableReactor") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableReactor = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableReactor = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableReactor = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableReactor = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableReactor = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableReactor = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableRefinery
			if(receivedCommand.Contains("DisableRefinery") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableRefinery = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableRefinery = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableRefinery = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableRefinery = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableRefinery = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableRefinery = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableRocketLauncher
			if(receivedCommand.Contains("DisableRocketLauncher") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableRocketLauncher = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableRocketLauncher = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableRocketLauncher = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableRocketLauncher = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableRocketLauncher = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableRocketLauncher = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableReloadableRocketLauncher
			if(receivedCommand.Contains("DisableReloadableRocketLauncher") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableReloadableRocketLauncher = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableReloadableRocketLauncher = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableReloadableRocketLauncher = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableReloadableRocketLauncher = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableReloadableRocketLauncher = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableReloadableRocketLauncher = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableRotor
			if(receivedCommand.Contains("DisableRotor") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableRotor = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableRotor = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableRotor = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableRotor = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableRotor = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableRotor = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableSensor
			if(receivedCommand.Contains("DisableSensor") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableSensor = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableSensor = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableSensor = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableSensor = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableSensor = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableSensor = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableSolarPanel
			if(receivedCommand.Contains("DisableSolarPanel") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableSolarPanel = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableSolarPanel = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableSolarPanel = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableSolarPanel = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableSolarPanel = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableSolarPanel = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableSoundBlock
			if(receivedCommand.Contains("DisableSoundBlock") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableSoundBlock = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableSoundBlock = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableSoundBlock = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableSoundBlock = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableSoundBlock = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableSoundBlock = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableSpaceBall
			if(receivedCommand.Contains("DisableSpaceBall") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableSpaceBall = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableSpaceBall = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableSpaceBall = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableSpaceBall = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableSpaceBall = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableSpaceBall = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableTimerBlock
			if(receivedCommand.Contains("DisableTimerBlock") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableTimerBlock = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableTimerBlock = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableTimerBlock = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableTimerBlock = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableTimerBlock = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableTimerBlock = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableThruster
			if(receivedCommand.Contains("DisableThruster") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableThruster = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableThruster = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableThruster = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableThruster = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableThruster = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableThruster = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableWelder
			if(receivedCommand.Contains("DisableWelder") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableWelder = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableWelder = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableWelder = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableWelder = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableWelder = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableWelder = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			//DisableUpgradeModule
			if(receivedCommand.Contains("DisableUpgradeModule") == true){
				
				var commandSplit = receivedCommand.Split('.');
				
				if(commandSplit.Length != 5){
					
					return "Bad Command Received.";
					
				}
				
				bool result = false;
				
				if(bool.TryParse(commandSplit[4], out result) == false){
					
					return "Failed To Parse Value: " + commandSplit[4];
					
				}
				
				if(commandSplit[2] == "SpaceCargoShips"){
					
					Settings.SpaceCargoShips.DisableUpgradeModule = result;
					var saveSetting = Settings.SpaceCargoShips.SaveSettings(Settings.SpaceCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "RandomEncounters"){
					
					Settings.RandomEncounters.DisableUpgradeModule = result;
					var saveSetting = Settings.RandomEncounters.SaveSettings(Settings.RandomEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryCargoShips"){
					
					Settings.PlanetaryCargoShips.DisableUpgradeModule = result;
					var saveSetting = Settings.PlanetaryCargoShips.SaveSettings(Settings.PlanetaryCargoShips);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "PlanetaryInstallations"){
					
					Settings.PlanetaryInstallations.DisableUpgradeModule = result;
					var saveSetting = Settings.PlanetaryInstallations.SaveSettings(Settings.PlanetaryInstallations);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "BossEncounters"){
					
					Settings.BossEncounters.DisableUpgradeModule = result;
					var saveSetting = Settings.BossEncounters.SaveSettings(Settings.BossEncounters);
					return saveSetting;
					
				}
				
				if(commandSplit[2] == "OtherNPCs"){
					
					Settings.OtherNPCs.DisableUpgradeModule = result;
					var saveSetting = Settings.OtherNPCs.SaveSettings(Settings.OtherNPCs);
					return saveSetting;
					
				}
				
				return "Could Not Identify Type Of Settings To Save To";
				
			}
			
			
			return "Could Not Recognize Settings Chat Command.";
			
		}

	}
	
}