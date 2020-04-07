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

		public double DistanceFromWorldCenter;
		public Vector3D DirectionFromWorldCenter;

		public List<string> InsideTerritories;
		public List<string> InsideStrictTerritories;

		public List<string> InsideKnownPlayerLocations;
		public List<string> InsideStrictKnownPlayerLocations;

		public MyPlanet NearestPlanet;
		public bool IsOnPlanet;
		public string NearestPlanetName;
		public double PlanetDiameter;

		public float OxygenAtPosition;
		public float AtmosphereAtPosition;
		public float GravityAtPosition;
		public double AltitudeAtPosition;
		public bool IsNight;
		public string WeatherAtPosition;
		public string CommonTerrainAtPosition;

		public EnvironmentEvaluation(Vector3D coords) {

			//Non Planet Checks
			DistanceFromWorldCenter = Vector3D.Distance(Vector3D.Zero, coords);
			DirectionFromWorldCenter = Vector3D.Normalize(coords);

			InsideTerritories = new List<string>();
			InsideStrictTerritories = new List<string>();

			//Planet Checks
			NearestPlanet = SpawnResources.GetNearestPlanet(coords, true);

			if (NearestPlanet == null || !MyAPIGateway.Entities.Exist(NearestPlanet))
				return;

			AltitudeAtPosition = Vector3D.Distance(NearestPlanet.GetClosestSurfacePointGlobal(coords), coords);
			NearestPlanetName = NearestPlanet.Generator.Id.SubtypeName;
			PlanetDiameter = NearestPlanet.AverageRadius * 2;

			var planetEntity = NearestPlanet as IMyEntity;
			var gravityProvider = planetEntity.Components.Get<MyGravityProviderComponent>();

			if (gravityProvider != null) {

				if (gravityProvider.IsPositionInRange(coords) == true) {

					IsOnPlanet = true;

				}

			}

			if (!IsOnPlanet)
				return;

			//On Planet Checks
			GravityAtPosition = gravityProvider.GetGravityMultiplier(coords);
			AtmosphereAtPosition = NearestPlanet.GetAirDensity(coords);
			OxygenAtPosition = NearestPlanet.GetOxygenForPosition(coords);
			IsNight = MyVisualScriptLogicProvider.IsOnDarkSide(NearestPlanet, coords);
			WeatherAtPosition = MyVisualScriptLogicProvider.GetWeather(coords);

			//Terrain Material Checks
			var upDir = Vector3D.Normalize(coords - NearestPlanet.PositionComp.WorldAABB.Center);
			var downDir = upDir * -1;
			var forward = Vector3D.CalculatePerpendicularVector(upDir);
			var matrix = MatrixD.CreateWorld(coords, forward, upDir);
			var directionList = new List<Vector3D>();
			directionList.Add(matrix.Forward);
			directionList.Add(matrix.Backward);
			directionList.Add(matrix.Left);
			directionList.Add(matrix.Right);

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
