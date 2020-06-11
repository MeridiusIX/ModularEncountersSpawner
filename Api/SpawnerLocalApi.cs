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

namespace ModularEncountersSpawner.Api {
	public static class SpawnerLocalApi {

		public static void SendApiToMods() {

			//Create a Dictionary of delegates that point to methods in the MES code.
			//Send the Dictionary To Other Mods That Registered To This ID in LoadData()
			MyAPIGateway.Utilities.SendModMessage(1521905890, GetApiDictionary());

		}

		public static Dictionary<string, Delegate> GetApiDictionary() {

			var dict = new Dictionary<string, Delegate>();
			dict.Add("AddKnownPlayerLocation", new Action<Vector3D, string, double, int, int, int>(KnownPlayerLocationManager.AddKnownPlayerLocation));
			dict.Add("CustomSpawnRequest", new Func<List<string>, MatrixD, Vector3, bool, string, string, bool>(CustomSpawner.CustomSpawnRequest));
			dict.Add("GetDespawnCoords", new Func<IMyCubeGrid, Vector3D>(GetDespawnCoords));
			dict.Add("GetSpawnGroupBlackList", new Func<List<string>>(GetSpawnGroupBlackList));
			dict.Add("GetNpcNameBlackList", new Func<List<string>>(GetNpcNameBlackList));
			dict.Add("IsPositionInKnownPlayerLocation", new Func<Vector3D, bool, string, bool>(KnownPlayerLocationManager.IsPositionInKnownPlayerLocation));
			dict.Add("ConvertRandomNamePatterns", new Func<string, string>(RandomNameGenerator.CreateRandomNameFromPattern));
			dict.Add("GetNpcStartCoordinates", new Func<IMyCubeGrid, Vector3D>(GetNpcStartCoordinates));
			dict.Add("GetNpcEndCoordinates", new Func<IMyCubeGrid, Vector3D>(GetNpcEndCoordinates));
			dict.Add("RemoveKnownPlayerLocation", new Action<Vector3D, string, bool>(KnownPlayerLocationManager.RemoveLocation));
			dict.Add("SetSpawnerIgnoreForDespawn", new Func<IMyCubeGrid, bool, bool>(SetSpawnerIgnoreForDespawn));
			dict.Add("SpawnBossEncounter", new Func<Vector3D, List<string>, bool>(SpawnBossEncounter));
			dict.Add("SpawnPlanetaryCargoShip", new Func<Vector3D, List<string>, bool>(SpawnPlanetaryCargoShip));
			dict.Add("SpawnPlanetaryInstallation", new Func<Vector3D, List<string>, bool>(SpawnPlanetaryInstallation));
			dict.Add("SpawnRandomEncounter", new Func<Vector3D, List<string>, bool>(SpawnRandomEncounter));
			dict.Add("SpawnSpaceCargoShip", new Func<Vector3D, List<string>, bool>(SpawnSpaceCargoShip));

			return dict;

		}

		public static Vector3D GetDespawnCoords(IMyCubeGrid cubeGrid) {

			ActiveNPC npcData = null;

			if (!NPCWatcher.ActiveNPCs.TryGetValue(cubeGrid, out npcData))
				return Vector3D.Zero;

			if (npcData != null)
				return npcData.EndCoords;

			return Vector3D.Zero;
		
		}

		public static List<string> GetSpawnGroupBlackList() {

			return new List<string>(Settings.General.NpcSpawnGroupBlacklist.ToList());

		}

		public static List<string> GetNpcNameBlackList() {

			return new List<string>(Settings.General.NpcGridNameBlacklist.ToList());

		}

		public static Vector3D GetNpcStartCoordinates(IMyCubeGrid cubeGrid) {

			if (cubeGrid == null || !MyAPIGateway.Entities.Exist(cubeGrid))
				return Vector3D.Zero;

			ActiveNPC npc = null;

			if (NPCWatcher.ActiveNPCs.TryGetValue(cubeGrid, out npc))
				return npc.StartCoords;

			return Vector3D.Zero;

		}

		public static Vector3D GetNpcEndCoordinates(IMyCubeGrid cubeGrid) {

			if (cubeGrid == null || !MyAPIGateway.Entities.Exist(cubeGrid))
				return Vector3D.Zero;

			ActiveNPC npc = null;

			if (NPCWatcher.ActiveNPCs.TryGetValue(cubeGrid, out npc))
				return npc.EndCoords;

			return Vector3D.Zero;

		}

		public static bool SetSpawnerIgnoreForDespawn(IMyCubeGrid cubeGrid, bool ignore) {

			if (cubeGrid == null || !MyAPIGateway.Entities.Exist(cubeGrid))
				return false;

			ActiveNPC npc = null;

			if (NPCWatcher.ActiveNPCs.ContainsKey(cubeGrid)) {

				NPCWatcher.ActiveNPCs[cubeGrid].CleanupIgnore = ignore;
				return true;

			}

			return false;

		}

		//SpawnBossEncounter
		public static bool SpawnBossEncounter(Vector3D coords, List<string> spawnGroups) {

			var result = BossEncounterSpawner.AttemptSpawn(coords, spawnGroups);
			return result.StartsWith("Boss Encounter GPS Created with Spawngroup");
		
		}

		//SpawnPlanetaryCargoShip
		public static bool SpawnPlanetaryCargoShip(Vector3D coords, List<string> spawnGroups) {

			var result = PlanetaryCargoShipSpawner.AttemptSpawn(coords, spawnGroups);
			return result.StartsWith("Spawning Group - ");

		}

		//SpawnPlanetaryInstallation
		public static bool SpawnPlanetaryInstallation(Vector3D coords, List<string> spawnGroups) {

			var result = PlanetaryInstallationSpawner.AttemptSpawn(coords, null, spawnGroups);
			return result.StartsWith("Spawning Group - ");

		}

		//SpawnRandomEncounter
		public static bool SpawnRandomEncounter(Vector3D coords, List<string> spawnGroups) {

			var result = RandomEncounterSpawner.AttemptSpawn(coords, spawnGroups);
			return result.StartsWith("Spawning Group - ");

		}

		//SpawnSpaceCargoShip
		public static bool SpawnSpaceCargoShip(Vector3D coords, List<string> spawnGroups) {

			var result = SpaceCargoShipSpawner.AttemptSpawn(coords, spawnGroups);
			return result.StartsWith("Spawning Group - ");

		}

	}

}
