using ModularEncountersSpawner.Configuration;
using ModularEncountersSpawner.Spawners;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Text;
using VRageMath;

namespace ModularEncountersSpawner.World {
	public static class TimeoutManagement {

		public static List<TimeoutZone> Timeouts = new List<TimeoutZone>();

		public static void ApplySpawnTimeoutToZones(SpawnType spawn, Vector3D coords) {

			bool appliedToZones = false;

			for (int i = Timeouts.Count - 1; i >= 0; i--) {

				var timeout = Timeouts[i];

				if (timeout.TimeoutType != spawn)
					continue;

				if (Vector3D.Distance(coords, timeout.Coords) > timeout.Radius)
					continue;

				timeout.IncreaseSpawns();
				appliedToZones = true;

			}

			if (appliedToZones)
				return;

			Timeouts.Add(new TimeoutZone(spawn, coords, GetRadius(spawn)));
		
		}

		public static bool IsSpawnAllowed(SpawnType spawn, Vector3D coords) {

			if (!GetEnabled(spawn))
				return true;

			var currentTime = MyAPIGateway.Session.GameDateTime;
			var timeLimit = GetCooldownLimit(spawn);
			var spawnLimit = GetSpawnLimit(spawn);

			for (int i = Timeouts.Count - 1; i >= 0; i--) {

				var timeout = Timeouts[i];

				if (timeout.Remove) {

					Timeouts.RemoveAt(i);
					continue;

				}

				if (timeout.TimeoutType != spawn)
					continue;

				if (timeout.TimeoutType == SpawnType.SpaceCargoShip && MES_SessionCore.SpaceCargoShipWaveSpawner.SpawnWaves)
					continue;

				if (Vector3D.Distance(coords, timeout.Coords) > timeout.Radius)
					continue;

				if ((currentTime - timeout.LastSpawnedEncounter).TotalSeconds >= timeLimit) {

					Timeouts.RemoveAt(i);
					continue;

				}

				if (timeout.SpawnedEncounters >= spawnLimit)
					return false;
			
			}

			return true;
		
		}

		public static int GetCooldownLimit(SpawnType spawn) {

			if (spawn == SpawnType.SpaceCargoShip)
				return Settings.SpaceCargoShips.TimeoutDuration;

			if (spawn == SpawnType.RandomEncounter)
				return Settings.RandomEncounters.TimeoutDuration;

			if (spawn == SpawnType.PlanetaryCargoShip)
				return Settings.PlanetaryCargoShips.TimeoutDuration;

			if (spawn == SpawnType.PlanetaryInstallation)
				return Settings.PlanetaryInstallations.TimeoutDuration;

			if (spawn == SpawnType.BossEncounter)
				return Settings.BossEncounters.TimeoutDuration;

			if (spawn == SpawnType.OtherNPC)
				return Settings.OtherNPCs.TimeoutDuration;

			return 0;
		
		}

		public static int GetSpawnLimit(SpawnType spawn) {

			if (spawn == SpawnType.SpaceCargoShip)
				return Settings.SpaceCargoShips.TimeoutSpawnLimit;

			if (spawn == SpawnType.RandomEncounter)
				return Settings.RandomEncounters.TimeoutSpawnLimit;

			if (spawn == SpawnType.PlanetaryCargoShip)
				return Settings.PlanetaryCargoShips.TimeoutSpawnLimit;

			if (spawn == SpawnType.PlanetaryInstallation)
				return Settings.PlanetaryInstallations.TimeoutSpawnLimit;

			if (spawn == SpawnType.BossEncounter)
				return Settings.BossEncounters.TimeoutSpawnLimit;

			if (spawn == SpawnType.OtherNPC)
				return Settings.OtherNPCs.TimeoutSpawnLimit;

			return 0;

		}

		public static double GetRadius(SpawnType spawn) {

			if (spawn == SpawnType.SpaceCargoShip)
				return Settings.SpaceCargoShips.TimeoutRadius;

			if (spawn == SpawnType.RandomEncounter)
				return Settings.RandomEncounters.TimeoutRadius;

			if (spawn == SpawnType.PlanetaryCargoShip)
				return Settings.PlanetaryCargoShips.TimeoutRadius;

			if (spawn == SpawnType.PlanetaryInstallation)
				return Settings.PlanetaryInstallations.TimeoutRadius;

			if (spawn == SpawnType.BossEncounter)
				return Settings.BossEncounters.TimeoutRadius;

			if (spawn == SpawnType.OtherNPC)
				return Settings.OtherNPCs.TimeoutRadius;

			return 0;

		}

		public static bool GetEnabled(SpawnType spawn) {

			if (spawn == SpawnType.SpaceCargoShip)
				return Settings.SpaceCargoShips.UseTimeout;

			if (spawn == SpawnType.RandomEncounter)
				return Settings.RandomEncounters.UseTimeout;

			if (spawn == SpawnType.PlanetaryCargoShip)
				return Settings.PlanetaryCargoShips.UseTimeout;

			if (spawn == SpawnType.PlanetaryInstallation)
				return Settings.PlanetaryInstallations.UseTimeout;

			if (spawn == SpawnType.BossEncounter)
				return Settings.BossEncounters.UseTimeout;

			if (spawn == SpawnType.OtherNPC)
				return Settings.OtherNPCs.UseTimeout;

			return false;

		}

	}

	public enum SpawnType {
		
		None,
		SpaceCargoShip,
		RandomEncounter,
		PlanetaryCargoShip,
		PlanetaryInstallation,
		BossEncounter,
		OtherNPC
	
	}

	public class TimeoutZone {

		public SpawnType TimeoutType;
		public int SpawnedEncounters;
		public DateTime LastSpawnedEncounter;
		public Vector3D Coords;
		public double Radius;
		public bool Remove;

		public TimeoutZone() {

			TimeoutType = SpawnType.None;
			SpawnedEncounters = 0;
			LastSpawnedEncounter = MyAPIGateway.Session.GameDateTime;
			Coords = Vector3D.Zero;
			Radius = 0;
			Remove = false;

		}

		public TimeoutZone(SpawnType spawn, Vector3D coords, double radius) {

			TimeoutType = spawn;
			SpawnedEncounters = 1;
			LastSpawnedEncounter = MyAPIGateway.Session.GameDateTime;
			Coords = coords;
			Radius = radius;
			Remove = false;

		}

		public void IncreaseSpawns() {

			SpawnedEncounters++;
			LastSpawnedEncounter = MyAPIGateway.Session.GameDateTime;

		}

		public bool InsideRadius(Vector3D coords) {

			return Vector3D.Distance(coords, Coords) <= TimeoutManagement.GetRadius(TimeoutType);

		}

		public Vector2D TimeoutLength() {

			return new Vector2D((MyAPIGateway.Session.GameDateTime - LastSpawnedEncounter).TotalSeconds, TimeoutManagement.GetCooldownLimit(TimeoutType));
		
		}

		public string GetInfo(Vector3D coords) {

			var sb = new StringBuilder();
			sb.Append(" - [Timeout Zone] ").AppendLine();
			sb.Append(" - Spawn Type:         ").Append(TimeoutType.ToString()).AppendLine();
			var spawnLimit = TimeoutManagement.GetSpawnLimit(TimeoutType);
			sb.Append(" - Spawned Encounters: ").Append(SpawnedEncounters).Append(" / ").Append(spawnLimit).AppendLine();
			sb.Append(" - Restricting Spawns: ").Append(SpawnedEncounters >= spawnLimit).AppendLine();
			var time = TimeoutLength();
			sb.Append(" - Time Remaining:     ").Append((int)time.X).Append(" / ").Append((int)time.Y).AppendLine();
			sb.Append(" - Position In Radius: ").Append(InsideRadius(coords)).AppendLine();
			
			return sb.ToString();

		}

	}

}
