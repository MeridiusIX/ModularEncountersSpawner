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

namespace ModularEncountersSpawner.Templates {

    [ProtoContract]
    public class KnownPlayerLocation {

        [ProtoMember(1)]
        public string NpcFaction;

        [ProtoMember(2)]
        public Vector3D Coords;

        [ProtoMember(3)]
        public double Radius;

        [ProtoMember(4)]
        public int ExpirationTimeMinutes;

        [ProtoMember(5)]
        public int MaxSpawnedEncounters;

        [ProtoMember(6)]
        public DateTime LastSighting;

        [ProtoMember(7)]
        public int SpawnedEncounters;

        [ProtoIgnore]
        public BoundingSphereD Sphere { get { return new BoundingSphereD(Coords, Radius); } }

        [ProtoMember(8)]
        public Dictionary<string, int> CustomTracking;

        [ProtoMember(9)]
        public int MinThreatForAvoidingAbandonment;

        

        public KnownPlayerLocation() {

            this.NpcFaction = "";
            this.Coords = Vector3D.Zero;
            this.Radius = 0;
            this.ExpirationTimeMinutes = -1;
            this.MaxSpawnedEncounters = -1;
            this.LastSighting = MyAPIGateway.Session.GameDateTime;
            this.SpawnedEncounters = 0;
            this.CustomTracking = new Dictionary<string, int>();

        }

        public KnownPlayerLocation(string faction, Vector3D coords, double radius, int expiration, int maxSpawns, int minThreat) : base() {

            this.NpcFaction = faction;
            this.Coords = coords;
            this.Radius = radius;
            this.ExpirationTimeMinutes = expiration;
            this.MaxSpawnedEncounters = maxSpawns;
            this.MinThreatForAvoidingAbandonment = minThreat;

        }

    }

}
