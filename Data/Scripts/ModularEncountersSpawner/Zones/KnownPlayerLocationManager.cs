using Sandbox.Game;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using VRage.Game.ModAPI;
using VRageMath;

namespace ModularEncountersSpawner.Zones {
    public static class KnownPlayerLocationManager {

        public static List<KnownPlayerLocation> Locations = new List<KnownPlayerLocation>();

        public static void AddKnownPlayerLocation(Vector3D coords, string faction, double radius, int duration = -1, int maxEncounters = -1, int minThreatToAvoidAbandonment = -1) {

            bool foundExistingLocation = false;
            var sphere = new BoundingSphereD(coords, radius);
            List<KnownPlayerLocation> intersectingLocations = new List<KnownPlayerLocation>();

            foreach (var location in Locations) {

                if (location.NpcFaction != faction) {

                    continue;

                }

                if (!sphere.Intersects(location.Sphere) && location.Sphere.Contains(coords) == ContainmentType.Disjoint) {

                    continue;

                }

                intersectingLocations.Add(location);
                foundExistingLocation = true;

            }

            if (foundExistingLocation == false) {

                Logger.AddMsg("Creating KnownPlayerLocation at " + coords.ToString() + " / Radius: " + radius.ToString(), true);
                Locations.Add(new KnownPlayerLocation(faction, coords, radius, duration, maxEncounters, minThreatToAvoidAbandonment));
                AlertPlayersOfNewKPL(coords, radius, faction);

            } else {

                var currentLocation = new KnownPlayerLocation(faction, coords, radius, duration, maxEncounters, minThreatToAvoidAbandonment);

                foreach (var location in intersectingLocations) {

                    var dirFromCurrentToIntersection = Vector3D.Normalize(location.Coords - currentLocation.Coords);
                    var coordsBetweenCenters = dirFromCurrentToIntersection * (Vector3D.Distance(location.Coords, currentLocation.Coords) / 2) + currentLocation.Coords;
                    var radiusToUse = location.Radius == currentLocation.Radius ? currentLocation.Radius : currentLocation.Radius > location.Radius ? currentLocation.Radius : location.Radius;
                    var outerRimCoords = -dirFromCurrentToIntersection * radiusToUse + currentLocation.Coords;
                    var finalRadius = Vector3D.Distance(outerRimCoords, coordsBetweenCenters);
                    currentLocation = new KnownPlayerLocation(faction, coordsBetweenCenters, finalRadius, duration, maxEncounters, minThreatToAvoidAbandonment);
                    currentLocation.IncludeCustomVariables(location);
                    Locations.Remove(location);

                }

                Locations.Add(currentLocation);
                AlertPlayersOfNewKPL(coords, radius, faction);
                Logger.AddMsg("Known Player Location(s) Already Exist, Merging Locations.", true);

            }

            SaveLocations();

        }

        public static void AlertPlayersOfNewKPL(Vector3D coords, double radius, string faction) {

            var playerList = new List<IMyPlayer>();
            MyAPIGateway.Players.GetPlayers(playerList);

            foreach (var player in playerList) {

                if (player.IsBot == true) {

                    continue;

                }

                if (Vector3D.Distance(player.GetPosition(), coords) > radius) {

                    continue;

                }

                if (string.IsNullOrWhiteSpace(faction)) {

                    MyVisualScriptLogicProvider.ShowNotification("This area has been identified as \"Player Occupied\"", 5000, "Red", player.IdentityId);

                } else {

                    MyVisualScriptLogicProvider.ShowNotification("This area has been identified as \"Player Occupied\" by " + faction, 5000, "Red", player.IdentityId);

                }

            }

        }

        public static void ChangeZoneSizeAtLocation(Vector3D coords, string faction = "", double size = 0, bool isMultiplicative = false) {

            foreach (var location in Locations) {

                if (IsPositionInKnownPlayerLocation(location, coords, true, faction))
                    continue;

                if (!isMultiplicative) {

                    location.Radius += size;

                } else {

                    location.Radius *= size;

                }

            }

        }

        public static void CleanExpiredLocations() {

            bool needsUpdate = false;

            for (int i = Locations.Count - 1; i >= 0; i--) {

                var location = Locations[i];
                var duration = MyAPIGateway.Session.GameDateTime - location.LastSighting;

                if (location.Radius <= 0) {

                    Logger.AddMsg(string.Format("Player Known Location At [{0}] Has Been Removed Because its Radius is 0 or Less", location.Coords), true);
                    Locations.RemoveAt(i);
                    needsUpdate = true;
                    continue;

                }

                if (location.ExpirationTimeMinutes >= 0 && duration.TotalSeconds / 60 >= location.ExpirationTimeMinutes) {

                    Logger.AddMsg(string.Format("Player Known Location At [{0}] Has Been Removed Because its Timer Expired", location.Coords), true);
                    Locations.RemoveAt(i);
                    needsUpdate = true;
                    continue;

                }

                if (location.MaxSpawnedEncounters >= 0 && location.SpawnedEncounters >= location.MaxSpawnedEncounters) {

                    Logger.AddMsg(string.Format("Player Known Location At [{0}] Has Been Removed Because it has Exceeded Spawn Count", location.Coords), true);
                    Locations.RemoveAt(i);
                    needsUpdate = true;
                    continue;

                }

            }

            if (needsUpdate == true) {

                SaveLocations();

            }

        }

        public static bool IsPositionInKnownPlayerLocation(Vector3D coords, bool matchFaction = false, string faction = "") {

            foreach (var location in Locations) {

                if (IsPositionInKnownPlayerLocation(location, coords, matchFaction, faction))
                    return true;

            }

            return false;

        }

        public static bool IsPositionInKnownPlayerLocation(KnownPlayerLocation location, Vector3D coords, bool matchFaction = false, string faction = "") {

            if (matchFaction == true && faction != location.NpcFaction) {

                return false;

            }

            if (Vector3D.Distance(coords, location.Coords) > location.Radius) {

                return false;

            }

            return true;

        }

        public static void IncreaseSpawnCountOfLocations(Vector3D coords, string faction) {

            foreach (var location in Locations) {

                if (!string.IsNullOrWhiteSpace(location.NpcFaction) && faction != location.NpcFaction)
                    continue;

                if (IsPositionInKnownPlayerLocation(location, coords, false)) {

                    location.LastSighting = MyAPIGateway.Session.GameDateTime;
                    location.SpawnedEncounters++;

                }

            }

            SaveLocations();

        }

        public static void LoadLocations() {

            try {

                string byteString = "";

                if (MyAPIGateway.Utilities.GetVariable("MES-KnownPlayerLocationData", out byteString) == true) {

                    var byteData = Convert.FromBase64String(byteString);
                    Locations = MyAPIGateway.Utilities.SerializeFromBinary<List<KnownPlayerLocation>>(byteData);

                }

            } catch (Exception ex) {

                Logger.AddMsg("Error Retrieving KnownPlayerLocations Data from Sandbox");
                Logger.AddMsg(ex.ToString());
                Locations = new List<KnownPlayerLocation>();

            }

        }

        public static void RemoveLocation(Vector3D coords, string faction = "", bool removeAllZones = false) {

            bool removedLocation = false;

            for (int i = Locations.Count - 1; i >= 0; i--) {

                if (Locations[i].Sphere.Contains(coords) != ContainmentType.Disjoint) {

                    if (removeAllZones || faction == Locations[i].NpcFaction || string.IsNullOrWhiteSpace(faction) == string.IsNullOrWhiteSpace(Locations[i].NpcFaction)) {

                        Logger.AddMsg(string.Format("Player Known Location At [{0}] Has Been Removed", Locations[i].Coords), true);
                        Locations.RemoveAt(i);
                        removedLocation = true;

                    }

                }

            }

            if (removedLocation)
                SaveLocations();

        }

        public static void SaveLocations() {

            try {

                var byteData = MyAPIGateway.Utilities.SerializeToBinary(Locations);
                var byteString = Convert.ToBase64String(byteData);
                MyAPIGateway.Utilities.SetVariable("MES-KnownPlayerLocationData", byteString);

            } catch (Exception ex) {

                Logger.AddMsg("Error Saving KnownPlayerLocations Data to Sandbox");
                Logger.AddMsg(ex.ToString());

            }

        }

    }
}
