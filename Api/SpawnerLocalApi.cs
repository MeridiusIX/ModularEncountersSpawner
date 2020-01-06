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

namespace ModularEncountersSpawner.Api {
    public static class SpawnerLocalApi {

        public static void SendApiToMods() {

            //Create a Dictionary of delegates that point to methods in the MES code.
            //Send the Dictionary To Other Mods That Registered To This ID in LoadData()
            MyAPIGateway.Utilities.SendModMessage(1521905890, GetApiDictionary());

        }

        public static Dictionary<string, Delegate> GetApiDictionary() {

            var dict = new Dictionary<string, Delegate>();
            dict.Add("AddKnownPlayerLocation", new Action<Vector3D, string, double, int, int>(KnownPlayerLocationManager.AddKnownPlayerLocation));
            dict.Add("CustomSpawnRequest", new Func<List<string>, MatrixD, Vector3, bool, bool>(CustomSpawner.CustomSpawnRequest));
            dict.Add("GetSpawnGroupBlackList", new Func<List<string>>(GetSpawnGroupBlackList));
            dict.Add("GetNpcNameBlackList", new Func<List<string>>(GetNpcNameBlackList));
            dict.Add("IsPositionInKnownPlayerLocation", new Func<Vector3D, bool, string, bool>(KnownPlayerLocationManager.IsPositionInKnownPlayerLocation));
            return dict;

        }

        public static List<string> GetSpawnGroupBlackList() {

            return new List<string>(Settings.General.NpcSpawnGroupBlacklist.ToList());

        }

        public static List<string> GetNpcNameBlackList() {

            return new List<string>(Settings.General.NpcGridNameBlacklist.ToList());

        }

    }

}
