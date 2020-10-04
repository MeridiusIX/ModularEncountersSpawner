using ProtoBuf;
using Sandbox.Game.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.Models;
using VRageMath;

namespace ModularEncountersSpawner.Api {
    [ProtoContract(UseProtoMembersOnly = true)]
    public class Water {
        /// <summary>the entity ID of a planet</summary>
        [ProtoMember(5)]
        public long planetID;

        /// <summary>the average radius of the water</summary>
        [ProtoMember(10)]
        public float radius;
        /// <summary>the current radius of the water</summary>
        [ProtoMember(11)]
        public float currentRadius;

        /// <summary>the maximum height of waves</summary>
        [ProtoMember(15)]
        public float waveHeight = 1f;
        /// <summary>how fast a wave will oscillate</summary>
        [ProtoMember(16)]
        public float waveSpeed = 0.004f;
        /// <summary>timer value for syncing waves between clients</summary>
        [ProtoMember(17)]
        public double waveTimer = 0;

        /// <summary>center position of the water</summary>
        [ProtoMember(20)]
        public Vector3 position;

        //Physics properties

        /// <summary>Viscosity of the water</summary>
        [ProtoMember(25)]
        public float viscosity = 0.1f;

        /// <summary>Buoyancy multiplier of the water</summary>
        [ProtoMember(26)]
        public float buoyancy = 1f;

        /// <summary>Whether or not the water can support fish</summary>
        [ProtoMember(30)]
        public bool enableFish = true;

        /// <summary>Whether or not the water can support seagulls</summary>
        [ProtoMember(31)]
        public bool enableSeagulls = true;

        /// <summary>All entites currently under the water</summary>
        [XmlIgnore, ProtoIgnore]
        public List<MyEntity> underWaterEntities = new List<MyEntity>();

        /// <summary>The planet entity</summary>
        [XmlIgnore, ProtoIgnore]
        public MyPlanet planet;

        /// <summary>Without any arguments is used for Protobuf</summary>
        public Water() {
            planet = MyEntities.GetEntityById(planetID) as MyPlanet;
        }

        /// <summary>Provide a planet entity and it will set everything up for you</summary>
        public Water(MyPlanet planet, float radiusMultiplier = 1.02f) {
            planetID = planet.EntityId;

            position = planet.PositionComp.GetPosition();
            radius = planet.MinimumRadius * radiusMultiplier;
            currentRadius = radius;

            enableFish = planet?.Generator?.Atmosphere?.Breathable == true && planet?.Generator?.DefaultSurfaceTemperature == VRage.Game.MyTemperatureLevel.Cozy;
            enableSeagulls = enableFish;

            this.planet = planet;
        }

        /// <summary>Returns the closest point to water without regard to voxels</summary>
        public Vector3D GetClosestSurfacePoint(Vector3D position, float altitudeOffset = 0) {
            return this.position + Vector3.Normalize(position - this.position) * (currentRadius + altitudeOffset);
        }

        /// <summary>Returns true if the given position is underwater</summary>
        public bool IsUnderwater(Vector3D position, float altitudeOffset = 0) {
            return Vector3D.Distance(this.position, position) - (currentRadius + altitudeOffset) <= 0;
        }

        ///<summary>Returns true if the given position is underwater without a square root function</summary>
        public bool IsUnderwaterSquared(Vector3D position, float altitudeOffset = 0) {
            return Vector3D.DistanceSquared(this.position, position) - (currentRadius + altitudeOffset) <= 0;
        }

        /// <summary>Overwater = 0, ExitsWater = 1, EntersWater = 2, Underwater = 3</summary>
        public int Intersects(Vector3D from, Vector3D to) {
            if (IsUnderwater(from)) {
                if (IsUnderwater(to))
                    return 3; //Underwater
                else
                    return 1; //ExitsWater
            } else {
                if (IsUnderwater(to))
                    return 2; //EntersWater
                else
                    return 0; //Overwater
            }
        }

        /// <summary>Overwater = 0, ExitsWater = 1, EntersWater = 2, Underwater = 3</summary>
        public int Intersects(LineD line) {
            if (IsUnderwater(line.From)) {
                if (IsUnderwater(line.To))
                    return 3; //Underwater
                else
                    return 1; //ExitsWater
            } else {
                if (IsUnderwater(line.To))
                    return 2; //EntersWater
                else
                    return 0; //Overwater
            }
        }

        /// <summary>Returns the depth of water a position is at, negative numbers are underwater</summary>
        public float GetDepth(Vector3 position) {
            return Vector3.Distance(this.position, position) - currentRadius;
        }

        /// <summary>Returns the depth of water a position is at without a square root function, negative numbers are underwater</summary>
        public float GetDepthSquared(Vector3 position) {
            return Vector3.DistanceSquared(this.position, position) - currentRadius;
        }

        /// <summary>Returns the depth of water a position is at using sea level, negative numbers are underwater</summary>
        public float GetDepthSimple(Vector3 position) {
            return Vector3.Distance(this.position, position) - radius;
        }

        /// <summary>Returns the depth of water a position is at using sea level without a square root function, negative numbers are underwater</summary>
        public float GetDepthSimpleSquared(Vector3 position) {
            return Vector3.DistanceSquared(this.position, position) - radius;
        }



        /// <summary>Returns the up direction at a position</summary>
        public Vector3 GetUpDirection(Vector3 position) {
            return Vector3.Normalize(position - this.position);
        }
    }
}
