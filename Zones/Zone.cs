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
using ProtoBuf;
using VRage;
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

namespace ModularEncountersSpawner.Zones {

	public enum ZoneEnum {
	
		None,
		Static,
		Planetary,
		Dynamic
	
	}

	public class Zone {

		//Configurable
		public string Name;
		public ZoneEnum Type;

		public Vector3D Coordinates;
		public Vector3D CoreDirection;
		public double Radius;
		public List<string> PlanetList;

		public bool UseFactionOwnership;
		public string FactionOwner;
		public List<string> FriendlyFactions;

		public bool UseTimer;
		public bool PlayerInZoneResetsTime;
		public int MaxTimeMinutes;

		public bool AllowFactionContest;
		public string ContestingFaction;
		public List<string> ContestingFactionFriends;
		public int ContestVictoryPointsRequired;

		public bool UseZoneAnnouncing;
		public string ZoneEnterAnnounceMessage;
		public string ZoneEnterAnnounceSoundId;
		public string ZoneExitAnnounceMessage;
		public string ZoneExitAnnounceSoundId;

		//Non-Configurable
		public int ZoneMorale;
		public int ZoneSpawnResources;

		public int MyContestPoints;
		public int EnemyContestPoints;
		public string EnemyFaction;
		public string EnemyNewTerritory;

		public Zone() {

			//Configurable
			Name = "";
			Type = ZoneEnum.None;

			Coordinates = Vector3D.Zero;
			CoreDirection = Vector3D.Zero;
			Radius = 0;
			PlanetList = new List<string>();

			UseFactionOwnership = false;
			FactionOwner = "";
			FriendlyFactions = new List<string>();

			UseTimer = false;
			PlayerInZoneResetsTime = false;
			MaxTimeMinutes = 0;

			AllowFactionContest = false;
			ContestingFaction = "";
			ContestingFactionFriends = new List<string>();
			ContestVictoryPointsRequired = 10;

			UseZoneAnnouncing = false;
			ZoneEnterAnnounceMessage = "";
			ZoneEnterAnnounceSoundId = "";
			ZoneExitAnnounceMessage = "";
			ZoneExitAnnounceSoundId = "";

			//Non-Configurable
			ZoneMorale = 0;
			ZoneSpawnResources = 0;

		}

		public void InitTags(string tagData = "") {
		
			
		
		}

	}

}
