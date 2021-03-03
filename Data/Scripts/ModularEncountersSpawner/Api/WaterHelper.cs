using Sandbox.Definitions;
using Sandbox.Game;
using Sandbox.Game.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.ObjectBuilders;
using VRageMath;

namespace ModularEncountersSpawner.Api {

	public static class WaterHelper {

		public static bool Enabled { get { return MES_SessionCore.Instance.WaterMod.Registered; } }
		public static WaterModAPI WaterMod { get { return MES_SessionCore.Instance.WaterMod; } }

		public static Dictionary<long, Water> WaterData = new Dictionary<long, Water>();
		private static List<long> _planetsWithWater = new List<long>();
		public static bool RequiresUpdate = true;
		public static bool RequiresUpdateAdd = true;
		public static bool RequiresUpdateRemove = false;

		public static void Init() {

			WaterMod.WaterCreatedEvent += WaterAdded;
			WaterMod.WaterRemovedEvent += WaterRemoved;

		}

		public static void Unload() {

			WaterMod.WaterCreatedEvent -= WaterAdded;
			WaterMod.WaterRemovedEvent -= WaterRemoved;

		}

		public static Vector3D GetClosestSurface(Vector3D coords, MyPlanet planet, Water water, bool ignoreWater = false) {

			bool isWater = false;
			return GetClosestSurface(coords, planet, water, ref isWater, ignoreWater);

		}

		public static Vector3D GetClosestSurface(Vector3D coords, MyPlanet planet, Water water, ref bool isWater, bool ignoreWater = false) {

			double waterLevel = 0;
			var result = GetClosestSurface(coords, planet, water, ref waterLevel, ignoreWater);
			isWater = waterLevel < 0;
			return result;

		}

		public static Vector3D GetClosestSurface(Vector3D coords, MyPlanet planet, Water water, ref double distAboveWater, bool ignoreWater = false) {

			if (planet == null) {

				distAboveWater = 0;
				return Vector3D.Zero;

			}

			if (!Enabled || water == null || ignoreWater) {

				distAboveWater = 0;
				return planet.GetClosestSurfacePointGlobal(coords);

			} else {

				var planetSurface = planet.GetClosestSurfacePointGlobal(coords);
				var waterSurface = water.GetClosestSurfacePoint(coords);
				var planetDist = Vector3D.Distance(planetSurface, planet.PositionComp.WorldAABB.Center);
				var waterDist = Vector3D.Distance(waterSurface, planet.PositionComp.WorldAABB.Center);
				bool dryLand = planetDist > waterDist;
				distAboveWater = planetDist - waterDist;
				return dryLand ? planetSurface : waterSurface;

			}

		}

		public static bool GetDepth(Vector3D coords, Water water, ref double depth) {

			if (!Enabled || water == null) {

				depth = 0;
				return false;

			}

			depth = water.GetDepth(coords);
			return depth < 0;

		}

		public static Water GetWater(MyPlanet planet) {

			RefreshWater();

			if (!Enabled || planet == null)
				return null;

			Water water = null;
			WaterData.TryGetValue(planet.EntityId, out water);
			//Logger.MsgDebug("Got Water: " + (water != null ? true : false).ToString(), DebugTypeEnum.General);
			return water;

		}

		public static bool IsPositionUnderwater(Vector3D coords, Water water) {

			if (!Enabled || water == null)
				return false;

			return water.IsUnderwater(coords);
		
		}

		public static bool IsPositionUnderwater(Vector3D coords, MyPlanet planet) {

			if (!Enabled)
				return false;

			var water = GetWater(planet);

			if (water == null)
				return false;

			return water.IsUnderwater(coords);

		}

		public static void RefreshWater() {

			if (!Enabled)
				return;

			if (RequiresUpdate) {

				RequiresUpdate = false;
				_planetsWithWater.Clear();

				if (WaterMod.Waters != null) {

					WaterMod.UpdateRadius();

					for (int i = WaterMod.Waters.Count - 1; i >= 0; i--) {

						if (WaterMod.Waters[i] == null)
							continue;

						if (!_planetsWithWater.Contains(WaterMod.Waters[i].planetID))
							_planetsWithWater.Add(WaterMod.Waters[i].planetID);

					}

				} else {

					Logger.AddMsg("Warning: Waters List Null!", true);
				
				}

				//Logger.MsgDebug("Planets With Water Update: " + _planetsWithWater.Count, DebugTypeEnum.General);

			}

			if (RequiresUpdateAdd) {

				RequiresUpdateAdd = false;

				if (WaterMod.Waters != null) {

					for (int i = WaterMod.Waters.Count - 1; i >= 0; i--) {

						if (WaterMod.Waters[i] == null)
							continue;

						if (!WaterData.ContainsKey(WaterMod.Waters[i].planetID))
							WaterData.Add(WaterMod.Waters[i].planetID, WaterMod.Waters[i]);
						else
							WaterData[WaterMod.Waters[i].planetID] = WaterMod.Waters[i];

					}

				} else {

					Logger.AddMsg("Warning: Waters List Null!", true);

				}

			}

			if (RequiresUpdateRemove) {

				RequiresUpdateRemove = false;
				var misMatchKeys = new List<long>();
				foreach (var planet in WaterData.Keys) {

					if (!_planetsWithWater.Contains(planet))
						misMatchKeys.Add(planet);

				}

				foreach (var key in misMatchKeys)
					WaterData.Remove(key);

			}

		}

		public static bool UnderwaterAndDepthCheck(Vector3D pos, Water water, bool targetState, double minDepth, double maxDepth) {

			double depth = 0;
			var underwater = WaterHelper.GetDepth(pos, water, ref depth);

			if (!underwater && !targetState) {

				return true;

			}

			if (underwater && (minDepth > -1 || maxDepth > -1)) {

				depth = Math.Abs(depth);

				if (minDepth > -1 && depth < minDepth)
					underwater = false;

				if (maxDepth > -1 && depth > maxDepth)
					underwater = false;

			}

			return (underwater == targetState);

		}

		public static void WaterAdded() {

			RequiresUpdate = true;
			RequiresUpdateAdd = true;

		}

		public static void WaterRemoved() {

			RequiresUpdate = true;
			RequiresUpdateRemove = true;

		}

	}

}
