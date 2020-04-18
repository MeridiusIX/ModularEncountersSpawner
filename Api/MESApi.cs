using System;
using System.Collections.Generic;
using Sandbox.ModAPI;
using VRage.Game.ModAPI;
using VRage.Utils;
using VRageMath;

//Change namespace to your mod's namespace
namespace ModularEncountersSpawner.Api {
	public static class MESApi {

		public static bool MESApiReady = false;

		private static long _mesModId = 1521905890;
		private static Action<Vector3D, string, double, int, int> _addKnownPlayerLocation;
		private static Func<List<string>, MatrixD, Vector3, bool, string, string, bool> _customSpawnRequest;
		private static Func<IMyCubeGrid, Vector3D> _getDespawnCoords;
		private static Func<List<string>> _getSpawnGroupBlackList;
		private static Func<List<string>> _getNpcNameBlackList;
		private static Func<Vector3D, bool, string, bool> _isPositionInKnownPlayerLocation;
		private static Func<string, string> _convertRandomNamePatterns;
		private static Func<IMyCubeGrid, Vector3D> _getNpcStartCoordinates;
		private static Func<IMyCubeGrid, Vector3D> _getNpcEndCoordinates;
		private static Func<IMyCubeGrid, bool, bool> _setSpawnerIgnoreForDespawn;

		//Run This Method in your SessionComponent LoadData() Method
		public static void RegisterAPIListener() {

			MyAPIGateway.Utilities.RegisterMessageHandler(_mesModId, APIListener);

		}

		/// <summary>
		/// Used to Create a Known Player Location that SpawnGroups can use as a Spawn Condition
		/// If a KPL already exists within the radius of the newly created location, its timer will be reset.
		/// </summary>
		/// <param name="coords"></param>
		/// <param name="faction"></param>
		/// <param name="radius"></param>
		/// <param name="expirationMinutes"></param>
		/// <param name="maxSpawns"></param>
		public static void AddKnownPlayerLocation(Vector3D coords, string faction, double radius, int expirationMinutes, int maxSpawns) => _addKnownPlayerLocation?.Invoke(coords, faction, radius, expirationMinutes, maxSpawns);

		/// <summary>
		/// Used To Spawn A Random SpawnGroup From A Provided List At A Provided Location. The Spawn Will Not Be Categorized As A CargoShip/RandomEncounter/Etc
		/// </summary>
		/// <param name="spawnGroups">List of SpawnGroups you want to attempt spawning from</param>
		/// <param name="coords">The coordinates the Spawn will use</param>
		/// <param name="forwardDir">Forward Direction vector for the spawn</param>
		/// <param name="upDir">Up Direction Vector for the spawn</param>
		/// <param name="velocity">Velocity vector</param>
		/// <param name="factionOverride">Faction tag you want spawngroup to use, regardless of its settings</param>
		/// <param name="spawnProfileId">Identifier for your mod so MES can properly log where the spawn request originated from</param>
		public static bool CustomSpawnRequest(List<string> spawnGroups, MatrixD spawningMatrix, Vector3 velocity, bool ignoreSafetyCheck, string factionOverride, string spawnProfileId) => _customSpawnRequest?.Invoke(spawnGroups, spawningMatrix, velocity, ignoreSafetyCheck, factionOverride, spawnProfileId) ?? false;

		/// <summary>
		/// Gets the Despawn Coords that are generated from a ship spawned as either Space or Planet CargoShip.
		/// Returns Vector3D.Zero if no Coords can be found.
		/// </summary>
		/// <param name="cubeGrid">The cubegrid of the NPC you want to check Despawn Coords For</param>
		/// <returns></returns>
		public static Vector3D GetDespawnCoords(IMyCubeGrid cubeGrid) => _getDespawnCoords?.Invoke(cubeGrid) ?? Vector3D.Zero;

		/// <summary>
		/// Get a String List of all Current SpawnGroup SubtypeNames Currently in the MES Blacklist
		/// </summary>
		/// <returns>List of SpawnGroup SubtypeNames</returns>
		public static List<string> GetSpawnGroupBlackList() => _getSpawnGroupBlackList?.Invoke() ?? new List<string>();

		/// <summary>
		/// Get a String List of all Current SpawnGroup SubtypeNames Currently in the MES Blacklist
		/// </summary>
		/// <returns>List of NPC Grid Names</returns>
		public static List<string> GetNpcNameBlackList() => _getNpcNameBlackList?.Invoke() ?? new List<string>();

		/// <summary>
		/// Indicates whether a set of coordinates is within a Known Player Location.
		/// Accepts additional parameters to narrow search to faction specific Locations.
		/// </summary>
		/// <param name="coords">The coordinates you want to check</param>
		/// <param name="mustMatchFaction">Indicates if the faction match checks should be used</param>
		/// <param name="faction">Faction Tag to check against if mustMatchFaction is true</param>
		/// <returns>true if position is in a valid Known Player Location</returns>
		public static bool IsPositionInKnownPlayerLocation(Vector3D coords, bool mustMatchFaction = false, string faction = "") => _isPositionInKnownPlayerLocation?.Invoke(coords, mustMatchFaction, faction) ?? false;

		/// <summary>
		/// Allows you to provide a string that will be processed by the Random Name Generator
		/// </summary>
		/// <param name="text">The string you want to process</param>
		/// <returns>A string with all Random Name Patterns processed</returns>
		public static string ConvertRandomNamePatterns(string text) => _convertRandomNamePatterns?.Invoke(text) ?? text;

		/// <summary>
		/// Allows you to get the coordinates the NPC spawned at if it was spawned via MES
		/// </summary>
		/// <param name="cubeGrid">The cubegrid of the NPC you want to check</param>
		/// <returns>Coordinates of Start Position. Returns Vector3D.Zero if not found</returns>
		public static Vector3D GetNpcStartCoordinates(IMyCubeGrid cubeGrid) => _getNpcStartCoordinates?.Invoke(cubeGrid) ?? Vector3D.Zero;

		/// <summary>
		/// Allows you to get the coordinates the NPC will despawn at if it was spawned via MES as a Cargo Ship
		/// </summary>
		/// <param name="cubeGrid">The cubegrid of the NPC you want to check</param>
		/// <returns>Coordinates of End Position. Returns Vector3D.Zero if not found</returns>
		public static Vector3D GetNpcEndCoordinates(IMyCubeGrid cubeGrid) => _getNpcEndCoordinates?.Invoke(cubeGrid) ?? Vector3D.Zero;

		/// <summary>
		/// Allows you to set a grid to be ignored or considered by the MES Cleanup Processes
		/// </summary>
		/// <param name="cubeGrid">The cubegrid of the NPC you want to set</param>
		/// <param name="ignoreSetting">Whether or not the grid should be ignored by cleanup</param>
		/// <returns>Returns a bool indicating if the change was successful or not</returns>
		public static bool SetSpawnerIgnoreForDespawn(IMyCubeGrid cubeGrid, bool ignoreSetting) => _setSpawnerIgnoreForDespawn?.Invoke(cubeGrid, ignoreSetting) ?? false;

		//Run This Method in your SessionComponent UnloadData() Method
		public static void UnregisterListener() {

			MyAPIGateway.Utilities.UnregisterMessageHandler(_mesModId, APIListener);

		}

		public static void APIListener(object data) {

			try {

				var dict = data as Dictionary<string, Delegate>;

				if (dict == null) {

					return;

				}

				MESApiReady = true;
				_addKnownPlayerLocation = (Action<Vector3D, string, double, int, int>)dict["AddKnownPlayerLocation"];
				_customSpawnRequest = (Func<List<string>, MatrixD, Vector3, bool, string, string, bool>)dict["CustomSpawnRequest"];
				_getDespawnCoords = (Func<IMyCubeGrid, Vector3D>)dict["GetDespawnCoords"];
				_getSpawnGroupBlackList = (Func<List<string>>)dict["GetSpawnGroupBlackList"];
				_getNpcNameBlackList = (Func<List<string>>)dict["GetNpcNameBlackList"];
				_isPositionInKnownPlayerLocation = (Func<Vector3D, bool, string, bool>)dict["IsPositionInKnownPlayerLocation"];
				_convertRandomNamePatterns = (Func<string, string>)dict["ConvertRandomNamePatterns"];
				_getNpcStartCoordinates = (Func<IMyCubeGrid, Vector3D>)dict["GetNpcStartCoordinates"];
				_getNpcEndCoordinates = (Func<IMyCubeGrid, Vector3D>)dict["GetNpcEndCoordinates"];
				_setSpawnerIgnoreForDespawn = (Func<IMyCubeGrid, bool, bool>)dict["SetSpawnerIgnoreForDespawn"];


			} catch (Exception e) {

				MyLog.Default.WriteLineAndConsole("MES API Failed To Load For Client: " + MyAPIGateway.Utilities.GamePaths.ModScopeName);

			}

			
		}

	}

}
