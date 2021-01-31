﻿using System;
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
        public Dictionary<string, int> CustomCounters;

        [ProtoMember(9)]
        public int MinThreatForAvoidingAbandonment;

        [ProtoMember(10)]
        public Dictionary<string, bool> CustomBooleans;



        public KnownPlayerLocation() {

            this.NpcFaction = "";
            this.Coords = Vector3D.Zero;
            this.Radius = 0;
            this.ExpirationTimeMinutes = -1;
            this.MaxSpawnedEncounters = -1;
            this.LastSighting = MyAPIGateway.Session.GameDateTime;
            this.SpawnedEncounters = 0;
            this.CustomCounters = new Dictionary<string, int>();
            this.CustomBooleans = new Dictionary<string, bool>();

        }

        public KnownPlayerLocation(string faction, Vector3D coords, double radius, int expiration, int maxSpawns, int minThreat) : base() {

            this.NpcFaction = faction;
            this.Coords = coords;
            this.Radius = radius;
            this.ExpirationTimeMinutes = expiration;
            this.LastSighting = MyAPIGateway.Session.GameDateTime;
            this.MaxSpawnedEncounters = maxSpawns;
            this.MinThreatForAvoidingAbandonment = minThreat;

        }

        public void IncludeCustomVariables(KnownPlayerLocation oldLocation) {

            if (oldLocation.CustomCounters == null || this.CustomCounters == null || oldLocation.CustomBooleans == null || this.CustomBooleans == null)
                return;

            foreach (var counter in oldLocation.CustomCounters.Keys) {

                int counterValue = 0;

                if (CustomCounters.TryGetValue(counter, out counterValue)) {

                    CustomCounters[counter] += counterValue;

                } else {

                    CustomCounters.Add(counter, counterValue);

                }
            
            }

            foreach (var boolean in oldLocation.CustomBooleans.Keys) {

                bool boolresult = false;

                if (CustomBooleans.TryGetValue(boolean, out boolresult)) {

                    if (oldLocation.CustomBooleans[boolean] || boolresult)
                        CustomBooleans[boolean] = true;

                } else {

                    CustomBooleans.Add(boolean, boolresult);

                }
            
            }
        
        }

        public string GetInfo(Vector3D coords) {

            var sb = new StringBuilder();
            sb.Append(" - [Known Player Location Info] ").AppendLine();
            sb.Append(" - Faction Owner:         ").Append(this.NpcFaction).AppendLine();
            sb.Append(" - Radius:                ").Append(this.Radius).AppendLine();
            sb.Append(" - Distance From Center:  ").Append(Vector3D.Distance(this.Coords, coords)).AppendLine();

            if (this.ExpirationTimeMinutes > -1) {

                var timeSpan = MyAPIGateway.Session.GameDateTime - this.LastSighting;
                var minutes = this.ExpirationTimeMinutes - (timeSpan.TotalSeconds / 60);
                sb.Append(" - Time Remaining:        ").Append(minutes).Append(" / ").Append(this.ExpirationTimeMinutes).AppendLine();

            }

            if (this.MaxSpawnedEncounters > -1) {

                sb.Append(" - Encounters Spawned:    ").Append(this.SpawnedEncounters).Append(" / ").Append(this.MaxSpawnedEncounters).AppendLine();

            }

            return sb.ToString();

        }

    }

}
