using ModularEncountersSpawner.Templates;
using Sandbox.Definitions;
using System;
using System.Collections.Generic;
using System.Text;
using VRageMath;

namespace ModularEncountersSpawner.Spawners {

	public class SpawnRequest{

		public string PrefabSubtypeId;
		public int TemporaryPrefabIndex;
		public string FactionOwner;

		public ImprovedSpawnGroup SpawnGroup;
		public int PrefabIndex;

		public Vector3D Coords;
		public Vector3D Forward;
		public Vector3D Up;
		public Vector3 LinearVelocity;
		public Vector3 AngularVelocity;

		public bool ReadyToSpawn;
		public bool AbandonSpawn;
		public bool SpawnFromOriginalPrefab;

		public SpawnRequest() {

			PrefabSubtypeId = "";
			TemporaryPrefabIndex = -1;
			FactionOwner = "";

			SpawnGroup = null;

			Coords = Vector3D.Zero;
			Forward = Vector3D.Zero;
			Up = Vector3D.Zero;
			LinearVelocity = Vector3.Zero;
			AngularVelocity = Vector3.Zero;

			ReadyToSpawn = false;
			AbandonSpawn = false;
			SpawnFromOriginalPrefab = false;

		}

		public bool ProcessPrefabManipulation() {

			if (SpawnGroup == null) {

				SpawnFromOriginalPrefab = true;
				return false;

			}

			return true;
		
		}

	}

	public static class NewSpawner {

		public static bool SpawnsQueued = false;
		public static int NextTemporaryPrefabIndex = 0; //Max 30
		public static List<MyPrefabDefinition> TemporaryPrefabs = new List<MyPrefabDefinition>();

		public static void SetupTemporaryPrefabs() {

			for (int i = 0; i < 30; i++) {

				TemporaryPrefabs.Add(MyDefinitionManager.Static.GetPrefabDefinition("MES-TemporaryPrefab-" + i.ToString()));

			}
		
		}

	}
}
