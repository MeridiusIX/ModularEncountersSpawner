using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Utils;

namespace ModularEncountersSpawner.Api {
    public class WaterModAPI {
        public const ushort ModHandlerID = 50271;
        public const int ModAPIVersion = 10;

        /// <summary>
        /// List of all water objects in the world, null if not registered
        /// </summary>
        public List<Water> Waters { get; private set; }

        /// <summary>
        /// Invokes when the API recieves data from the Water Mod
        /// </summary>
        public event Action RecievedData;

        /// <summary>
        /// Invokes when a water is added to the Waters list
        /// </summary>
        public event Action WaterCreatedEvent;

        /// <summary>
        /// Invokes when a water is removed from the Waters list
        /// </summary>
        public event Action WaterRemovedEvent;

        /// <summary>
        /// Invokes when the water API becomes registered and ready to work
        /// </summary>
        public event Action OnRegisteredEvent;

        /// <summary>
        /// True if the API is registered/alive
        /// </summary>
        public bool Registered { get; private set; } = false;

        /// <summary>
        /// Used to tell in chat what mod is out of date
        /// </summary>
        private string ModName = "UnknownMod";

        //Water API Guide
        //Drag WaterModAPI.cs and Water.cs into your mod
        //Create a new WaterModAPI object in your mod's script, "WaterModAPI api = new WaterModAPI();"
        //Register the api at the start of your session with api.Register()
        //Unregister the api at the end of your session with api.Unregister()
        //Run api.UpdateRadius() inside of an update method

        /// <summary>
        /// Register with a mod name so version control can recognize what mod may be out of date
        /// </summary>
        public void Register(string modName) {
            ModName = modName;
            MyAPIGateway.Utilities.RegisterMessageHandler(ModHandlerID, ModHandler);
        }

        /// <summary>
        /// Unregister to prevent odd behavior after reloading your save/game
        /// </summary>
        public void Unregister() {
            MyAPIGateway.Utilities.UnregisterMessageHandler(ModHandlerID, ModHandler);
        }

        /// <summary>
        /// Do not use, for interfacing with Water Mod
        /// </summary>
        private void ModHandler(object data) {

            //Logger.AddMsg("Water ModHandler Received Data");

            if (data == null) {

                //Logger.AddMsg("Data Received in Water ModHandler was null");
                return;

            }


            if (data is byte[]) {
                Waters = MyAPIGateway.Utilities.SerializeFromBinary<List<Water>>((byte[])data);

                if (Waters == null)
                    Waters = new List<Water>();
                else foreach (var water in Waters) {
                        MyEntity entity = MyEntities.GetEntityById(water.planetID);

                        if (entity != null)
                            water.planet = MyEntities.GetEntityById(water.planetID) as MyPlanet;
                    }

                int count = Waters.Count;
                RecievedData?.Invoke();

                if (count > Waters.Count)
                    WaterCreatedEvent?.Invoke();
                if (count < Waters.Count)
                    WaterRemovedEvent?.Invoke();

                //Logger.AddMsg("Water Count: " + count.ToString());

            } else {

                //Logger.AddMsg("Data Received in Water ModHandler was not byte[]");

            }

            if (!Registered) {
                Registered = true;
                OnRegisteredEvent?.Invoke();
            }

            if (data is int && (int)data != ModAPIVersion) {
                MyLog.Default.WriteLine("Water API V" + ModAPIVersion + " for " + ModName + " is outdated, expected V" + (int)data);
                MyAPIGateway.Utilities.ShowMessage(ModName, "Water API V" + ModAPIVersion + " is outdated, expected V" + (int)data);
            } else if (data is int) {

                //Logger.AddMsg("Water Mod Version: " + ((int)data).ToString());

            }
        }

        /// <summary>
        /// Recalculates the CurrentRadius for all waters
        /// </summary>
        public void UpdateRadius() {

            if (Waters == null)
                return;

            foreach (var water in Waters) {
                water.waveTimer += water.waveSpeed;
                water.currentRadius = (float)Math.Max(water.radius + Math.Sin(water.waveTimer * water.waveSpeed) * water.waveHeight, 0);
            }

        }

    }

}