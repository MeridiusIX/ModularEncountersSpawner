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

namespace ModularEncountersSpawner.Templates {

    [ProtoContract]
    public class RivalAISpawnRequest {

        [ProtoMember(1)]
        public List<string> SpawnGroups;

        [ProtoMember(2)]
        public Vector3D Coords;

        [ProtoMember(3)]
        public Vector3D ForwardDir;

        [ProtoMember(4)]
        public Vector3D UpDir;

        [ProtoMember(5)]
        public Vector3 Velocity;

        [ProtoIgnore]
        public MatrixD Matrix;

        public RivalAISpawnRequest() {

            SpawnGroups = new List<string>();

            Coords = Vector3D.Zero;
            ForwardDir = Vector3D.Forward;
            UpDir = Vector3D.Up;
            Velocity = Vector3.Zero;
            Matrix = MatrixD.Identity;

        }

    }
}
