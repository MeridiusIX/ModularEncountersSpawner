using System;
using System.Collections.Generic;
using System.Text;
using VRageMath;

namespace ModularEncountersSpawner.Zones {
	public class ZoneProfile {

		public string Name;
		public string Faction;

		public bool StaticZone;
		public Vector3D Coordinates;

		public bool PlanetaryZone;
		public Vector3D Direction;
		public double SurfaceOffset;

		public bool ShellZone;
		public double MinimumShellDistance;
		public double MaximumShellDistance;

		public double Radius;

		public bool UseZoneTimeLimit;
		public int ZoneTimeLimitSeconds;
		public bool PlayerInsideZoneResetsTimeLimit;


		public void InitTags() {
		
			
		
		}

	}

}
