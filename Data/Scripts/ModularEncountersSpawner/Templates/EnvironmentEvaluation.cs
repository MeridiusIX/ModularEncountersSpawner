using ModularEncountersSpawner.Api;
using ModularEncountersSpawner.Configuration;
using ModularEncountersSpawner.Spawners;
using Sandbox.Game;
using Sandbox.Game.Entities;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Text;
using VRage.ModAPI;
using VRageMath;

namespace ModularEncountersSpawner.Templates {

	public class EnvironmentEvaluation {

		public Vector3D Position;

		public double DistanceFromWorldCenter;
		public Vector3D DirectionFromWorldCenter;

		public List<string> InsideTerritories;
		public List<string> InsideStrictTerritories;

		public List<string> InsideKnownPlayerLocations;
		public List<string> InsideStrictKnownPlayerLocations;

		public MyPlanet NearestPlanet;
		public Water PlanetWater;
		public MyGravityProviderComponent Gravity;
		public bool IsOnPlanet;
		public string NearestPlanetName;
		public double PlanetDiameter;

		public Vector3D SurfaceCoords;
		public float OxygenAtPosition;
		public float AtmosphereAtPosition;
		public float GravityAtPosition;
		public double AltitudeAtPosition;
		public bool IsNight;
		public string WeatherAtPosition;
		public string CommonTerrainAtPosition;

		public float AirTravelViabilityRatio;

		public bool PlanetHasWater;
		public bool PositionIsUnderWater;
		public bool SurfaceIsUnderWater;
		public float WaterInSurroundingAreaRatio;

		public EnvironmentEvaluation() {

			WeatherAtPosition = "";
			CommonTerrainAtPosition = "";
			NearestPlanetName = "";

		}

		public EnvironmentEvaluation(Vector3D coords) : base() {

			//Non Planet Checks
			Position = coords;
			DistanceFromWorldCenter = Vector3D.Distance(Vector3D.Zero, coords);
			DirectionFromWorldCenter = Vector3D.Normalize(coords);

			InsideTerritories = new List<string>();
			InsideStrictTerritories = new List<string>();

			//Planet Checks
			NearestPlanet = SpawnResources.GetNearestPlanet(coords, true);

			if (NearestPlanet == null || !MyAPIGateway.Entities.Exist(NearestPlanet))
				return;

			var upDir = Vector3D.Normalize(coords - NearestPlanet.PositionComp.WorldAABB.Center);
			var downDir = upDir * -1;
			var forward = Vector3D.CalculatePerpendicularVector(upDir);
			var matrix = MatrixD.CreateWorld(coords, forward, upDir);
			var directionList = new List<Vector3D>();
			directionList.Add(matrix.Forward);
			directionList.Add(matrix.Backward);
			directionList.Add(matrix.Left);
			directionList.Add(matrix.Right);
			directionList.Add(Vector3D.Normalize(matrix.Forward + matrix.Right));
			directionList.Add(Vector3D.Normalize(matrix.Forward + matrix.Left));
			directionList.Add(Vector3D.Normalize(matrix.Backward + matrix.Right));
			directionList.Add(Vector3D.Normalize(matrix.Backward + matrix.Left));

			if (MES_SessionCore.Instance.WaterMod.Registered) {

				MES_SessionCore.Instance.WaterMod.UpdateRadius();

				if (MES_SessionCore.Instance.WaterMod.Waters != null) {

					for (int i = MES_SessionCore.Instance.WaterMod.Waters.Count - 1; i >= 0; i--) {

						if (i >= MES_SessionCore.Instance.WaterMod.Waters.Count)
							continue;

						var water = MES_SessionCore.Instance.WaterMod.Waters[i];

						if (water == null || water.planetID != NearestPlanet.EntityId)
							continue;

						PlanetWater = water;
						PlanetHasWater = true;
						PositionIsUnderWater = water.IsUnderwater(coords);
						SurfaceIsUnderWater = water.IsUnderwater(SurfaceCoords);

						int totalChecks = 0;
						int waterHits = 0;

						for (int j = 0; j < 12; j++) {

							foreach (var direction in directionList) {

								try {

									totalChecks++;
									var checkCoordsRough = direction * (j * 1000) + coords;
									var checkSurfaceCoords = NearestPlanet.GetClosestSurfacePointGlobal(checkCoordsRough);

									if (water.IsUnderwater(checkSurfaceCoords))
										waterHits++;

								} catch (Exception e) {

									Logger.AddMsg("Caught Exception Trying To Determine Water Data", true);
									Logger.AddMsg(e.ToString(), true);

								}

							}

						}

						Logger.AddMsg("Water Hits: " + waterHits.ToString(), true);
						Logger.AddMsg("Total Hits: " + totalChecks.ToString(), true);
						WaterInSurroundingAreaRatio = (float)waterHits / (float)totalChecks;

						break;

					}

				}

			}

			SurfaceCoords = SurfaceIsUnderWater ? PlanetWater.GetClosestSurfacePoint(coords) : NearestPlanet.GetClosestSurfacePointGlobal(coords);
			AltitudeAtPosition = Vector3D.Distance(SurfaceCoords, coords);
			NearestPlanetName = NearestPlanet.Generator.Id.SubtypeName;
			PlanetDiameter = NearestPlanet.AverageRadius * 2;

			var planetEntity = NearestPlanet as IMyEntity;
			Gravity = planetEntity.Components.Get<MyGravityProviderComponent>();

			if (Gravity != null) {

				if (Gravity.IsPositionInRange(coords) == true) {

					IsOnPlanet = true;

				}

			}

			if (!IsOnPlanet)
				return;

			//On Planet Checks
			GravityAtPosition = Gravity.GetGravityMultiplier(coords);
			AtmosphereAtPosition = NearestPlanet.GetAirDensity(coords);
			OxygenAtPosition = NearestPlanet.GetOxygenForPosition(coords);
			IsNight = MyVisualScriptLogicProvider.IsOnDarkSide(NearestPlanet, coords);
			WeatherAtPosition = MyVisualScriptLogicProvider.GetWeather(coords) ?? "";

			//Atmo Travel Viability
			float airTravelChecks = 0;
			float airTravelHits = 0;

			for (int i = -3; i < 4; i++) {

				for (int j = -3; j < 4; j++) {

					airTravelChecks++;
					var tempForward = matrix.Forward * (j * 500) + matrix.Translation;
					var roughCoords = matrix.Right * (i * 500) + tempForward;
					var surface = NearestPlanet.GetClosestSurfacePointGlobal(roughCoords);
					var up = Vector3D.Normalize(surface - NearestPlanet.PositionComp.WorldAABB.Center);
					var minAltitude = up * Settings.PlanetaryCargoShips.MinSpawningAltitude + surface;
					var airDensity = NearestPlanet.GetAirDensity(minAltitude);

					if (airDensity >= Settings.PlanetaryCargoShips.MinAirDensity)
						airTravelHits++;

				}
			
			}

			if(airTravelChecks > 0 && airTravelHits > 0)
				AirTravelViabilityRatio = airTravelHits / airTravelChecks;

			//Terrain Material Checks
			var terrainTypes = new Dictionary<string, int>();

			for (int i = 1; i < 12; i++) {

				foreach (var direction in directionList) {

					try {

						var checkCoordsRough = direction * (i * 15) + coords;
						var checkSurfaceCoords = NearestPlanet.GetClosestSurfacePointGlobal(checkCoordsRough);
						
						var checkMaterial = NearestPlanet.GetMaterialAt(ref checkSurfaceCoords);

						if (checkMaterial == null)
							continue;

						if (terrainTypes.ContainsKey(checkMaterial.MaterialTypeName)) {

							terrainTypes[checkMaterial.MaterialTypeName]++;

						} else {

							terrainTypes.Add(checkMaterial.MaterialTypeName, 1);

						}

					} catch (Exception e) {

						Logger.AddMsg("Caught Exception Trying To Determine Terrain Material", true);
						Logger.AddMsg(e.ToString(), true);

					}
					
				}

			}

			string highestCountName = "";
			int highestCountNumber = 0;

			foreach (var material in terrainTypes.Keys) {

				if (string.IsNullOrWhiteSpace(highestCountName) || terrainTypes[material] > highestCountNumber) {

					highestCountName = material;
					highestCountNumber = terrainTypes[material];

				}
			
			}

			if (!string.IsNullOrWhiteSpace(highestCountName)) {

				CommonTerrainAtPosition = highestCountName;

			}

		}

	}

}
