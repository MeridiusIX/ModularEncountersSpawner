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

	public class Zone {


		public string Name;


		public string ZoneProfileName;


		public ZoneProfile Profile;


		public List<string> Factions;


		public Vector3D Coordinates;


		public Vector3D Direction;


		public double Radius;


		public bool IsShell;


		public double MinShellRadius;


		public double MaxShellRadius;


		public DateTime TimeCreated;


		public int TimeToExpiration;


		public int SpawnedEncounters;


		public int MaxSpawnedEncounters;


		public bool UseAllowedSpawnGroups;


		public List<string> AllowedSpawnGroups;


		public bool UseRestrictedSpawnGroups;


		public List<string> RestrictedSpawnGroups;


		public bool UseAllowedModIDs;


		public List<string> AllowedModIDs;


		public bool UseRestrictedModIDs;


		public List<string> RestrictedModIDs;




	}

}
