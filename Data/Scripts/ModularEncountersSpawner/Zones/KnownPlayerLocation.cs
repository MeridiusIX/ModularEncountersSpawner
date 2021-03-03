using ProtoBuf;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Text;
using VRageMath;

namespace ModularEncountersSpawner.Zones {

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

            NpcFaction = "";
            Coords = Vector3D.Zero;
            Radius = 0;
            ExpirationTimeMinutes = -1;
            MaxSpawnedEncounters = -1;
            LastSighting = MyAPIGateway.Session.GameDateTime;
            SpawnedEncounters = 0;
            CustomCounters = new Dictionary<string, int>();
            CustomBooleans = new Dictionary<string, bool>();

        }

        public KnownPlayerLocation(string faction, Vector3D coords, double radius, int expiration, int maxSpawns, int minThreat) : base() {

            NpcFaction = faction;
            Coords = coords;
            Radius = radius;
            ExpirationTimeMinutes = expiration;
            LastSighting = MyAPIGateway.Session.GameDateTime;
            MaxSpawnedEncounters = maxSpawns;
            MinThreatForAvoidingAbandonment = minThreat;

        }

        public void IncludeCustomVariables(KnownPlayerLocation oldLocation) {

            if (oldLocation.CustomCounters == null || CustomCounters == null || oldLocation.CustomBooleans == null || CustomBooleans == null)
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
            sb.Append(" - Faction Owner:         ").Append(NpcFaction).AppendLine();
            sb.Append(" - Radius:                ").Append(Radius).AppendLine();
            sb.Append(" - Distance From Center:  ").Append(Vector3D.Distance(Coords, coords)).AppendLine();

            if (ExpirationTimeMinutes > -1) {

                var timeSpan = MyAPIGateway.Session.GameDateTime - LastSighting;
                var minutes = ExpirationTimeMinutes - timeSpan.TotalSeconds / 60;
                sb.Append(" - Time Remaining:        ").Append(minutes).Append(" / ").Append(ExpirationTimeMinutes).AppendLine();

            }

            if (MaxSpawnedEncounters > -1) {

                sb.Append(" - Encounters Spawned:    ").Append(SpawnedEncounters).Append(" / ").Append(MaxSpawnedEncounters).AppendLine();

            }

            return sb.ToString();

        }

    }

}
