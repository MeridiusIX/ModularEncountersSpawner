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
using VRage.Game.ObjectBuilders;
using VRage.Game.ObjectBuilders.ComponentSystem;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Serialization;
using VRage.Utils;
using VRageMath;
using ModularEncountersSpawner.Templates;
using ModularEncountersSpawner.Configuration;
using ModularEncountersSpawner.Spawners;

namespace ModularEncountersSpawner{
	
	public static class GridBuilderManipulation{
		
		public struct WeaponProfile{
			
			public MyCubeBlockDefinition BlockDefinition;
			public bool IsWeaponCore;
			public bool IsWeaponStatic;
			public bool IsWeaponTurret;
			
		}
		
		public static Dictionary<string, MyCubeBlockDefinition> BlockDirectory = new Dictionary<string, MyCubeBlockDefinition>();
		public static Dictionary<string, WeaponProfile> WeaponProfiles = new Dictionary<string, WeaponProfile>();
		public static Dictionary<string, float> PowerProviderBlocks = new Dictionary<string, float>();
		public static Dictionary<string, float> BatteryMaxCapacity = new Dictionary<string, float>();
		public static List<string> ForwardGunIDs = new List<string>();
		public static List<string> TurretIDs = new List<string>();

		public static List<MyDefinitionId> AllWeaponCoreIDs = new List<MyDefinitionId>();
		public static List<MyDefinitionId> AllWeaponCoreStaticIDs = new List<MyDefinitionId>();
		public static List<MyDefinitionId> AllWeaponCoreTurretIDs = new List<MyDefinitionId>();

		public static List<string> BlacklistedWeaponSubtypes = new List<string>();
		public static List<string> WhitelistedWeaponSubtypes = new List<string>();
		public static List<string> BlacklistedWeaponTargetSubtypes = new List<string>();
		public static List<string> WhitelistedWeaponTargetSubtypes = new List<string>();
		public static Dictionary<string, float> PowerDrainingWeapons = new Dictionary<string, float>();

		public static Dictionary<MyDefinitionId, MyDefinitionId> GlobalBlockReplacements = new Dictionary<MyDefinitionId, MyDefinitionId>();

		public static Dictionary<MyDefinitionId, MyDefinitionId> HeavyArmorConvertReference = new Dictionary<MyDefinitionId, MyDefinitionId>();
		public static List<MyDefinitionId> PartialBuiltAllowedBlocks = new List<MyDefinitionId>();
		
		public static Dictionary<string, BlockReplacementProfileMES> BlockReplacementProfiles = new Dictionary<string, BlockReplacementProfileMES>();
		
		public static bool EnergyShieldModDetected = false;
		public static bool DefenseShieldModDetected = false;
		public static Dictionary<string, float> ShieldBlocksSmallGrid = new Dictionary<string, float>();
		public static Dictionary<string, float> ShieldBlocksLargeGrid = new Dictionary<string, float>();
		
		public static Random Rnd = new Random();
		public static bool SetupComplete = false;
		
		
		public static void Setup(){
			
			try{

				//Setup Power Hogging Weapons Reference
				PowerDrainingWeapons.Add("NovaTorpedoLauncher_Large", 20); //Nova Heavy Plasma Torpedo
				PowerDrainingWeapons.Add("LargeDualBeamGTFBase_Large", 1050); //GTF Large Dual Beam Laser Turret
				PowerDrainingWeapons.Add("LargeStaticLBeamGTF_Small", 787.50f); //GTF Large Heavy Beam Laser
				PowerDrainingWeapons.Add("LargeStaticLBeamGTF_Large", 787.50f); //GTF Large Heavy Beam Laser
				PowerDrainingWeapons.Add("AegisSmallBeamBase_Large", 828); //Aegis Small Multi-Laser
				PowerDrainingWeapons.Add("AegisMarauderBeamStatic_Large", 18400); //Aegis Gungnir Large Beam Cannon
				PowerDrainingWeapons.Add("MediumQuadBeamGTFBase_Large", 330); //GTF Medium Quad Beam Turret
				PowerDrainingWeapons.Add("MPulseLaserBase_Small", 225); //GTF Medium Pulse Turret
				PowerDrainingWeapons.Add("MPulseLaserBase_Large", 225); //GTF Medium Pulse Turret
				PowerDrainingWeapons.Add("AegisLargeBeamStatic_Large", 7820); //Aegis Large Static Beam Laser
				PowerDrainingWeapons.Add("AegisMediumBeamStaticS_Small", 1797.45f); //Aegis Medium Static Beam Laser
				PowerDrainingWeapons.Add("AegisMediumBeamStatic_Large", 1797.45f); //Aegis Medium Static Beam Laser
				PowerDrainingWeapons.Add("AegisSmallBeamStatic_Large", 828); //Aegis Small Static Beam Laser
				PowerDrainingWeapons.Add("SDualPlasmaBase_Large", 6); //GTF Small Dual Blaster Turret
				PowerDrainingWeapons.Add("LSDualPlasmaStatic_Small", 12); //GTF Dual Static Blaster
				PowerDrainingWeapons.Add("LSDualPlasmaStatic_Large", 12); //GTF Dual Static Blaster
				PowerDrainingWeapons.Add("MDualPlasmaBase_Large", 12); //GTF Medium Dual Blaster Turret
				PowerDrainingWeapons.Add("ThorStatic_Small", 10); //Thor Plasma Cannon
				PowerDrainingWeapons.Add("ThorStatic_Large", 10); //Thor Plasma Cannon
				PowerDrainingWeapons.Add("LPlasmaTriBlasterBase_Large", 24); //GTF Large Tri Blaster Turret
				PowerDrainingWeapons.Add("ThorTurretBase_Large", 10); //Thor Dual Plasma Cannon
				PowerDrainingWeapons.Add("XLCitadelPlasmaCannonBarrel_Large", 48); //GTF XL Citadel Plasma Cannon Turret
				PowerDrainingWeapons.Add("SmallBeamBaseGTF_Large", 15); //GTF Small Beam Turret
				PowerDrainingWeapons.Add("SSmallBeamStaticGTF_Small", 15); //
				PowerDrainingWeapons.Add("Interior_Pulse_Laser_Base_Large", 15); //GTF Pulse Laser Interior Turret
				PowerDrainingWeapons.Add("SmallPulseLaser_Base_Large", 15); //GTF Small Pulse Turret
				PowerDrainingWeapons.Add("MediumStaticLPulseGTF_Small", 241.50f); //GTF Medium Static Pulse Laser
				PowerDrainingWeapons.Add("MediumStaticLPulseGTF_Large", 241.50f); //GTF Medium Static Pulse Laser
				
				//Armor Light To Heavy
				if(BlockReplacementProfiles.ContainsKey("MES-Armor-LightToHeavy") == false){
					
					var blockReplaceLightArmor = new BlockReplacementProfileMES();
					blockReplaceLightArmor.ReplacementReferenceName = "MES-Armor-LightToHeavy";
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorBlock"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorBlock"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorSlope"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorSlope"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorCorner"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorCorner"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorCornerInv"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorCornerInv"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorBlock"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorBlock"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorSlope"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorSlope"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorCorner"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorCorner"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorCornerInv"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorCornerInv"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHalfArmorBlock"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyHalfArmorBlock"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHalfSlopeArmorBlock"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyHalfSlopeArmorBlock"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "HalfArmorBlock"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "HeavyHalfArmorBlock"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "HalfSlopeArmorBlock"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "HeavyHalfSlopeArmorBlock"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorRoundSlope"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorRoundSlope"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorRoundCorner"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorRoundCorner"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorRoundCornerInv"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorRoundCornerInv"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorRoundSlope"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorRoundSlope"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorRoundCorner"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorRoundCorner"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorRoundCornerInv"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorRoundCornerInv"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorSlope2Base"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorSlope2Base"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorSlope2Tip"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorSlope2Tip"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorCorner2Base"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorCorner2Base"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorCorner2Tip"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorCorner2Tip"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorInvCorner2Base"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorInvCorner2Base"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorInvCorner2Tip"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorInvCorner2Tip"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorSlope2Base"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorSlope2Base"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorSlope2Tip"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorSlope2Tip"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorCorner2Base"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorCorner2Base"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorCorner2Tip"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorCorner2Tip"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorInvCorner2Base"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorInvCorner2Base"));
					blockReplaceLightArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorInvCorner2Tip"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorInvCorner2Tip"));
					BlockReplacementProfiles.Add(blockReplaceLightArmor.ReplacementReferenceName, blockReplaceLightArmor);
					
				}
				
				//Armor Heavy To Light
				if(BlockReplacementProfiles.ContainsKey("MES-Armor-HeavyToLight") == false){
					
					var blockReplaceHeavyArmor = new BlockReplacementProfileMES();
					blockReplaceHeavyArmor.ReplacementReferenceName = "MES-Armor-HeavyToLight";
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorBlock"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorBlock"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorSlope"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorSlope"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorCorner"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorCorner"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorCornerInv"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorCornerInv"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorBlock"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorBlock"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorSlope"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorSlope"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorCorner"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorCorner"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorCornerInv"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorCornerInv"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyHalfArmorBlock"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHalfArmorBlock"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyHalfSlopeArmorBlock"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHalfSlopeArmorBlock"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "HeavyHalfArmorBlock"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "HalfArmorBlock"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "HeavyHalfSlopeArmorBlock"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "HalfSlopeArmorBlock"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorRoundSlope"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorRoundSlope"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorRoundCorner"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorRoundCorner"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorRoundCornerInv"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorRoundCornerInv"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorRoundSlope"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorRoundSlope"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorRoundCorner"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorRoundCorner"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorRoundCornerInv"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorRoundCornerInv"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorSlope2Base"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorSlope2Base"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorSlope2Tip"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorSlope2Tip"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorCorner2Base"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorCorner2Base"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorCorner2Tip"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorCorner2Tip"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorInvCorner2Base"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorInvCorner2Base"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorInvCorner2Tip"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorInvCorner2Tip"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorSlope2Base"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorSlope2Base"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorSlope2Tip"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorSlope2Tip"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorCorner2Base"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorCorner2Base"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorCorner2Tip"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorCorner2Tip"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorInvCorner2Base"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorInvCorner2Base"));
					blockReplaceHeavyArmor.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorInvCorner2Tip"), new SerializableDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorInvCorner2Tip"));
					BlockReplacementProfiles.Add(blockReplaceHeavyArmor.ReplacementReferenceName, blockReplaceHeavyArmor);
					
				}
				
				//Turret Gatling to Missile
				if(BlockReplacementProfiles.ContainsKey("MES-Turret-GatlingToMissile") == false){
					
					var blockReplaceGatlingTurret = new BlockReplacementProfileMES();
					blockReplaceGatlingTurret.ReplacementReferenceName = "MES-Turret-GatlingToMissile";
					blockReplaceGatlingTurret.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_LargeGatlingTurret), ""), new SerializableDefinitionId(typeof(MyObjectBuilder_LargeMissileTurret), ""));
					blockReplaceGatlingTurret.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_LargeGatlingTurret), "SmallGatlingTurret"), new SerializableDefinitionId(typeof(MyObjectBuilder_LargeMissileTurret), "SmallMissileTurret"));
					BlockReplacementProfiles.Add(blockReplaceGatlingTurret.ReplacementReferenceName, blockReplaceGatlingTurret);
					
				}
					
				//Turret Missile to Gatling
				if(BlockReplacementProfiles.ContainsKey("MES-Turret-MissileToGatling") == false){
					
					var blockReplaceMissileTurret = new BlockReplacementProfileMES();
					blockReplaceMissileTurret.ReplacementReferenceName = "MES-Turret-MissileToGatling";
					blockReplaceMissileTurret.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_LargeMissileTurret), ""), new SerializableDefinitionId(typeof(MyObjectBuilder_LargeGatlingTurret), ""));
					blockReplaceMissileTurret.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_LargeMissileTurret), "SmallMissileTurret"), new SerializableDefinitionId(typeof(MyObjectBuilder_LargeGatlingTurret), "SmallGatlingTurret"));
					BlockReplacementProfiles.Add(blockReplaceMissileTurret.ReplacementReferenceName, blockReplaceMissileTurret);
					
				}
				
				//Gun Gatling to Missile
				if(BlockReplacementProfiles.ContainsKey("MES-Gun-GatlingToMissile") == false){
					
					var blockReplaceGatlingGun = new BlockReplacementProfileMES();
					blockReplaceGatlingGun.ReplacementReferenceName = "MES-Gun-GatlingToMissile";
					blockReplaceGatlingGun.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_SmallGatlingGun), ""), new SerializableDefinitionId(typeof(MyObjectBuilder_SmallMissileLauncher), ""));
					BlockReplacementProfiles.Add(blockReplaceGatlingGun.ReplacementReferenceName, blockReplaceGatlingGun);
				
				}
				
				//Gun Missile to Gatling
				if(BlockReplacementProfiles.ContainsKey("MES-Gun-MissileToGatling") == false){
					
					var blockReplaceMissileGun = new BlockReplacementProfileMES();
					blockReplaceMissileGun.ReplacementReferenceName = "MES-Gun-MissileToGatling";
					blockReplaceMissileGun.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_SmallMissileLauncher), ""), new SerializableDefinitionId(typeof(MyObjectBuilder_SmallGatlingGun), ""));
					BlockReplacementProfiles.Add(blockReplaceMissileGun.ReplacementReferenceName, blockReplaceMissileGun);
				
				}

				if(BlockReplacementProfiles.ContainsKey("MES-ProprietaryValuableBlocks") == false) {

					var blockReplacePropBlocks = new BlockReplacementProfileMES();
					blockReplacePropBlocks.ReplacementReferenceName = "MES-ProprietaryValuableBlocks";
					blockReplacePropBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Reactor), "SmallBlockSmallGenerator"), new SerializableDefinitionId(typeof(MyObjectBuilder_Reactor), "ProprietarySmallBlockSmallGenerator"));
					blockReplacePropBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Reactor), "SmallBlockLargeGenerator"), new SerializableDefinitionId(typeof(MyObjectBuilder_Reactor), "ProprietarySmallBlockLargeGenerator"));
					blockReplacePropBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Reactor), "LargeBlockSmallGenerator"), new SerializableDefinitionId(typeof(MyObjectBuilder_Reactor), "ProprietaryLargeBlockSmallGenerator"));
					blockReplacePropBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Reactor), "LargeBlockLargeGenerator"), new SerializableDefinitionId(typeof(MyObjectBuilder_Reactor), "ProprietaryLargeBlockLargeGenerator"));
					blockReplacePropBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_GravityGenerator), ""), new SerializableDefinitionId(typeof(MyObjectBuilder_GravityGenerator), "ProprietaryGravGen"));
					blockReplacePropBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_GravityGeneratorSphere), ""), new SerializableDefinitionId(typeof(MyObjectBuilder_GravityGeneratorSphere), "ProprietaryGravGenSphere"));
					blockReplacePropBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_VirtualMass), "VirtualMassLarge"), new SerializableDefinitionId(typeof(MyObjectBuilder_VirtualMass), "ProprietaryVirtualMassLarge"));
					blockReplacePropBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_VirtualMass), "VirtualMassSmall"), new SerializableDefinitionId(typeof(MyObjectBuilder_VirtualMass), "ProprietaryVirtualMassSmall"));
					blockReplacePropBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_SpaceBall), "SpaceBallLarge"), new SerializableDefinitionId(typeof(MyObjectBuilder_SpaceBall), "ProprietarySpaceBallLarge"));
					blockReplacePropBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_SpaceBall), "SpaceBallSmall"), new SerializableDefinitionId(typeof(MyObjectBuilder_SpaceBall), "ProprietarySpaceBallSmall"));
					blockReplacePropBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "SmallBlockSmallThrust"), new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "ProprietarySmallBlockSmallThrust"));
					blockReplacePropBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "SmallBlockLargeThrust"), new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "ProprietarySmallBlockLargeThrust"));
					blockReplacePropBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "LargeBlockSmallThrust"), new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "ProprietaryLargeBlockSmallThrust"));
					blockReplacePropBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "LargeBlockLargeThrust"), new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "ProprietaryLargeBlockLargeThrust"));
					blockReplacePropBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_JumpDrive), "LargeJumpDrive"), new SerializableDefinitionId(typeof(MyObjectBuilder_JumpDrive), "ProprietaryLargeJumpDrive"));
					blockReplacePropBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_LaserAntenna), "SmallBlockLaserAntenna"), new SerializableDefinitionId(typeof(MyObjectBuilder_LaserAntenna), "ProprietarySmallBlockLaserAntenna"));
					blockReplacePropBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_LaserAntenna), "LargeBlockLaserAntenna"), new SerializableDefinitionId(typeof(MyObjectBuilder_LaserAntenna), "ProprietaryLargeBlockLaserAntenna"));
					//blockReplacePropBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(), ""), new SerializableDefinitionId(typeof(), ""));
					BlockReplacementProfiles.Add(blockReplacePropBlocks.ReplacementReferenceName, blockReplacePropBlocks);

				}

				if(BlockReplacementProfiles.ContainsKey("MES-ProprietaryCompRichBlocks") == false) {

					var blockReplaceCompRichBlocks = new BlockReplacementProfileMES();
					blockReplaceCompRichBlocks.ReplacementReferenceName = "MES-ProprietaryCompRichBlocks";
					blockReplaceCompRichBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Conveyor), "LargeBlockConveyor"), new SerializableDefinitionId(typeof(MyObjectBuilder_Conveyor), "ProprietaryLargeBlockConveyor"));
					blockReplaceCompRichBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_ConveyorConnector), "ConveyorTube"), new SerializableDefinitionId(typeof(MyObjectBuilder_ConveyorConnector), "ProprietaryConveyorTube"));
					blockReplaceCompRichBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_ConveyorConnector), "ConveyorTubeCurved"), new SerializableDefinitionId(typeof(MyObjectBuilder_ConveyorConnector), "ProprietaryConveyorTubeCurved"));
					blockReplaceCompRichBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_ConveyorSorter), "LargeBlockConveyorSorter"), new SerializableDefinitionId(typeof(MyObjectBuilder_ConveyorSorter), "ProprietaryLargeBlockConveyorSorter"));
					blockReplaceCompRichBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Gyro), "LargeBlockGyro"), new SerializableDefinitionId(typeof(MyObjectBuilder_Gyro), "ProprietaryLargeBlockGyro"));
					BlockReplacementProfiles.Add(blockReplaceCompRichBlocks.ReplacementReferenceName, blockReplaceCompRichBlocks);

				}

				if(BlockReplacementProfiles.ContainsKey("MES-DisposableNpcBeacons") == false) {

					var blockReplaceDisposableBeacons = new BlockReplacementProfileMES();
					blockReplaceDisposableBeacons.ReplacementReferenceName = "MES-DisposableNpcBeacons";
					blockReplaceDisposableBeacons.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Beacon), "SmallBlockBeacon"), new SerializableDefinitionId(typeof(MyObjectBuilder_Beacon), "DisposableNpcBeaconSmall"));
					blockReplaceDisposableBeacons.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Beacon), "LargeBlockBeacon"), new SerializableDefinitionId(typeof(MyObjectBuilder_Beacon), "DisposableNpcBeaconLarge"));
					BlockReplacementProfiles.Add(blockReplaceDisposableBeacons.ReplacementReferenceName, blockReplaceDisposableBeacons);

				}

				//Partial Block Construction Blocks
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorBlock"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorBlock"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorSlope"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorSlope"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorCorner"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorCorner"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorCornerInv"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorCornerInv"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorBlock"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorBlock"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorSlope"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorSlope"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorCorner"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorCorner"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorCornerInv"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorCornerInv"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHalfArmorBlock"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyHalfArmorBlock"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHalfSlopeArmorBlock"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyHalfSlopeArmorBlock"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "HalfArmorBlock"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "HeavyHalfArmorBlock"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "HalfSlopeArmorBlock"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "HeavyHalfSlopeArmorBlock"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorRoundSlope"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorRoundSlope"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorRoundCorner"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorRoundCorner"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorRoundCornerInv"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorRoundCornerInv"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorRoundSlope"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorRoundSlope"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorRoundCorner"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorRoundCorner"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorRoundCornerInv"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorRoundCornerInv"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorSlope2Base"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorSlope2Base"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorSlope2Tip"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorSlope2Tip"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorCorner2Base"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorCorner2Base"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorCorner2Tip"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorCorner2Tip"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorInvCorner2Base"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorInvCorner2Base"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorInvCorner2Tip"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorInvCorner2Tip"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorSlope2Base"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorSlope2Base"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorSlope2Tip"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorSlope2Tip"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorCorner2Base"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorCorner2Base"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorCorner2Tip"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorCorner2Tip"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorInvCorner2Base"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorInvCorner2Base"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorInvCorner2Tip"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorInvCorner2Tip"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "ArmorCenter"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "ArmorCorner"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "ArmorInvCorner"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "ArmorSide"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallArmorCenter"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallArmorCorner"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallArmorInvCorner"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallArmorSide"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "Window3x3FlatInv"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "Window3x3Flat"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "Window2x3FlatInv"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "Window2x3Flat"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "Window1x2Slope"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "Window1x2SideRight"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "Window1x2SideLeft"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "Window1x2Inv"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "Window1x2FlatInv"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "Window1x2Flat"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "Window1x2Face"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "Window1x1Slope"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "Window1x1Side"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "Window1x1Inv"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "Window1x1FlatInv"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "Window1x1Flat"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "Window1x1Face"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeWindowSquare"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeWindowEdge"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeWindowCen"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeSteelCatwalkPlate"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeSteelCatwalkCorner"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeSteelCatwalk2Sides"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeSteelCatwalk"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeStairs"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeRamp"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeRailStraight"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeInteriorPillar"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeCoverWallHalf"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeCoverWall"));
				PartialBuiltAllowedBlocks.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockInteriorWall"));
				
				//Shield Blocks
				ShieldBlocksSmallGrid.Add("SmallShipMicroShieldGeneratorBase", 0.2f);
				ShieldBlocksSmallGrid.Add("SmallShipSmallShieldGeneratorBase", 4.5f);
				
				ShieldBlocksLargeGrid.Add("LargeShipSmallShieldGeneratorBase", 4.5f);
				ShieldBlocksLargeGrid.Add("LargeShipLargeShieldGeneratorBase", 85);

			}catch(Exception exc){
				
				Logger.AddMsg("Caught Error Setting Up Weapon Replacer Blacklist and Power-Draining Weapon References.");
				Logger.AddMsg("Unwanted Blocks May Be Used When Replacing Weapons.");
				
			}

			

			var allDefs = MyDefinitionManager.Static.GetAllDefinitions();
			
			//Check For Energy Shield Mod
			if(MES_SessionCore.ActiveMods.Contains(484504816) == true){
				
				EnergyShieldModDetected = true;
				
			}
			
			//Check For Defense Shield Mod
			if(MES_SessionCore.ActiveMods.Contains(1365616918) == true || MES_SessionCore.ActiveMods.Contains(1492804882) == true){
				
				DefenseShieldModDetected = true;
				
			}
			
			var errorDebugging = new StringBuilder();
			
			try{
				
				foreach(MyDefinitionBase definition in allDefs.Where( x => x is MyCubeBlockDefinition)){
					
					errorDebugging.Clear();
					errorDebugging.Append("Start Weapon Profile Creation For Definition").AppendLine();
					errorDebugging.Append("Get DefinitionId").AppendLine();
					errorDebugging.Append(definition.Id.ToString()).AppendLine();
					
					if(BlockDirectory.ContainsKey(definition.Id.ToString()) == false){
						
						BlockDirectory.Add(definition.Id.ToString(), definition as MyCubeBlockDefinition);
						
					}else{
						
						Logger.AddMsg("Block Reference Setup Found Duplicate DefinitionId Detected And Skipped: " + definition.Id.ToString());
						continue;
						
					}
					
					errorDebugging.Append("Skip If Blacklisted").AppendLine();
					
					errorDebugging.Append("If Power Block, Get MaxOutput").AppendLine();
					
					if(definition as MyPowerProducerDefinition != null){
						
						var powerBlock = definition as MyPowerProducerDefinition;
						
						if(PowerProviderBlocks.ContainsKey(definition.Id.ToString()) == false){
							
							PowerProviderBlocks.Add(definition.Id.ToString(), powerBlock.MaxPowerOutput);
							
						}

						var batteryDef = powerBlock as MyBatteryBlockDefinition;

						if(batteryDef != null) {

							if(BatteryMaxCapacity.ContainsKey(batteryDef.Id.SubtypeName) == false) {

								BatteryMaxCapacity.Add(batteryDef.Id.SubtypeName, batteryDef.MaxStoredPower);

							}

						}
						
					}
					
					errorDebugging.Append("Check if Weapon Block, Continue if Not").AppendLine();

					MyCubeBlockDefinition weaponBlock = null;
					bool isWeaponCore = false;
					bool isTurret = false;
					bool isStatic = false;

					if (AllWeaponCoreIDs.Contains(definition.Id)) {

						weaponBlock = definition as MyCubeBlockDefinition;
						isWeaponCore = true;
						isStatic = AllWeaponCoreStaticIDs.Contains(definition.Id);
						isTurret = AllWeaponCoreTurretIDs.Contains(definition.Id);

					} else {

						if (definition as MyWeaponBlockDefinition != null) {

							weaponBlock = definition as MyCubeBlockDefinition;

							if (weaponBlock as MyLargeTurretBaseDefinition != null) {

								isTurret = true;

							} else {

								isStatic = true;

							}

						}
					
					}

					if (weaponBlock == null || definition.Public == false) {

						continue;

					}
					
					errorDebugging.Append("Set Defintions To WeaponProfile class object").AppendLine();
					
					WeaponProfile weaponProfile;
					weaponProfile.BlockDefinition = weaponBlock;
					weaponProfile.IsWeaponCore = isWeaponCore;
					weaponProfile.IsWeaponStatic = isStatic;
					weaponProfile.IsWeaponTurret = isTurret;

					Logger.AddMsg("Potential Weapon Profile: " + weaponBlock.Id.ToString() + " / WC: " + weaponProfile.IsWeaponCore.ToString() + " / TW: " + weaponProfile.IsWeaponTurret.ToString() + " / SW: " + weaponProfile.IsWeaponStatic.ToString(), true);

					bool goodSize = false;
					
					errorDebugging.Append("Check if weapon grid X,Y,Z size is valid").AppendLine();
					
					if(weaponProfile.IsWeaponTurret) {
						
						if(weaponBlock.Size.X == weaponBlock.Size.Z && weaponBlock.Size.X % 2 != 0){
							
							goodSize = true;
							//Logger.AddMsg("Turret Profile: " + weaponBlock.Id.ToString(), true);
							TurretIDs.Add(weaponBlock.Id.ToString());
							
						}
			
					} 

					if(weaponProfile.IsWeaponStatic){
						
						if(weaponBlock.Size.X == weaponBlock.Size.Y && weaponBlock.Size.X % 2 != 0){
							
							goodSize = true;
							//Logger.AddMsg("Static Profile: " + weaponBlock.Id.ToString(), true);
							ForwardGunIDs.Add(weaponBlock.Id.ToString());
							
						}
						
					}
										
					if(goodSize == false){
						
						continue;
						
					}
					
					errorDebugging.Append("Add weapon to profile").AppendLine();
					
					if(WeaponProfiles.ContainsKey(weaponBlock.Id.ToString()) == false){
						
						WeaponProfiles.Add(weaponBlock.Id.ToString(), weaponProfile);

						
					}else{
						
						Logger.AddMsg("Weapon Profile Already Exists And Is Being Skipped For: " + weaponBlock.Id.ToString());
						
					}
					
				}
				
			}catch(Exception exc){
				
				MyVisualScriptLogicProvider.ShowNotificationToAll("Modular Encounters Spawner + NPC Weapon Replacer Encountered An Error.", 10000, "Red");
				MyVisualScriptLogicProvider.ShowNotificationToAll("Please Submit A Copy Of The Game Log To The Mod Author.", 10000, "Red");
				Logger.AddMsg("Error While Handling Weapon Replacer Setup");
				Logger.AddMsg(errorDebugging.ToString());
				Logger.AddMsg(exc.ToString());
				
			}

		}


		public static void ProcessPrefabForManipulation(string prefabName, ImprovedSpawnGroup spawnGroup, string spawnType = "", string behavior = "") {

			Logger.AddMsg("Prefab Manipulation Started For [" + prefabName + "] in SpawnGroup [" + spawnGroup.SpawnGroupName + "]", true);

			//Run Setup
			if(SetupComplete == false) {

				SetupComplete = true;
				Setup();

			}

			Logger.AddMsg("Getting Weapon Randomizer Blacklist/Whitelist from Global Settings", true);
			//Update Lists
			BlacklistedWeaponSubtypes = Settings.General.WeaponReplacerBlacklist.ToList();
			WhitelistedWeaponSubtypes = Settings.General.WeaponReplacerWhitelist.ToList();
			BlacklistedWeaponTargetSubtypes = Settings.General.WeaponReplacerTargetBlacklist.ToList();
			WhitelistedWeaponTargetSubtypes = Settings.General.WeaponReplacerTargetWhitelist.ToList();

			Logger.AddMsg("Getting Prefab By Name", true);
			//Get Prefab
			var prefabDef = MyDefinitionManager.Static.GetPrefabDefinition(prefabName);

			if(prefabDef == null) {

				return;

			}

			Logger.AddMsg("Making Backup Prefab or Restoring From Backup", true);
			//Backup / Restore Original Prefab
			if(SpawnGroupManager.prefabBackupList.ContainsKey(prefabName) == false) {

				var backupGridList = new List<MyObjectBuilder_CubeGrid>();

				for(int j = 0;j < prefabDef.CubeGrids.Length;j++) {

					var clonedGridOb = prefabDef.CubeGrids[j].Clone();
					backupGridList.Add(clonedGridOb as MyObjectBuilder_CubeGrid);

				}

				SpawnGroupManager.prefabBackupList.Add(prefabName, backupGridList);

			} else {

				if(SpawnGroupManager.prefabBackupList[prefabName].Count == prefabDef.CubeGrids.Length) {

					for(int j = 0;j < SpawnGroupManager.prefabBackupList[prefabName].Count;j++) {

						var clonedGridOb = SpawnGroupManager.prefabBackupList[prefabName][j].Clone();
						prefabDef.CubeGrids[j] = clonedGridOb as MyObjectBuilder_CubeGrid;

					}

				}

			}

			/*
			Manipulation Order
			 - UseBlockReplacer
			 - UseBlockReplacerProfile
			 - UseRivalAi
			 - Weapon Randomization
			 - Cleanup Block Disables
				- ShiftBlockColorsHue
				- ClearAuthorship
				- OverrideBlockDamageModifier
				- Spawngroup Block Disables
				- Block Name Enable/Disable
				- Turret Settings
			 - Reduce Block States
			*/

			
			//Block Replacer Individual
			if(spawnGroup.UseBlockReplacer == true) {

				Logger.AddMsg("Running Block Replacer", true);

				foreach(var grid in prefabDef.CubeGrids) {

					ProcessBlockReplacements(grid, spawnGroup);

				}

			}

			
			//Global Block Replacer Individual
			if(Settings.General.UseGlobalBlockReplacer == true && spawnGroup.IgnoreGlobalBlockReplacer == false) {

				Logger.AddMsg("Running Global Block Replacer", true);

				GlobalBlockReplacements = Settings.General.GetReplacementReferencePairs();

				foreach(var grid in prefabDef.CubeGrids) {

					ProcessGlobalBlockReplacements(grid);

				}

			}

			if(spawnGroup.ConvertToHeavyArmor == true) {

				Logger.AddMsg("Converting To Heavy Armor", true);

				if(spawnGroup.BlockReplacerProfileNames.Contains("MES-Armor-LightToHeavy") == false) {

					spawnGroup.BlockReplacerProfileNames.Add("MES-Armor-LightToHeavy");

				}

			}

			//Block Replacer Profiles
			if(spawnGroup.UseBlockReplacerProfile == true) {

				Logger.AddMsg("Applying Block Replacement Profiles", true);

				foreach(var grid in prefabDef.CubeGrids) {

					ApplyBlockReplacementProfile(grid, spawnGroup);

				}

			}

			//Global Block Replacer Profiles
			if(Settings.General.UseGlobalBlockReplacer == true && Settings.General.GlobalBlockReplacerProfiles.Length > 0 && spawnGroup.IgnoreGlobalBlockReplacer == false) {

				Logger.AddMsg("Applying Global Block Replacement Profiles", true);

				foreach(var grid in prefabDef.CubeGrids) {

					ApplyGlobalBlockReplacementProfile(grid);

				}

			}

			//Replace RemoteControl
			if(spawnGroup.UseRivalAi == true) {

				bool primaryBehaviorSet = false;

				foreach(var grid in prefabDef.CubeGrids) {

					if (RivalAiInitialize(grid, spawnGroup, behavior, primaryBehaviorSet))
						primaryBehaviorSet = true;

				}

			}

			//Provide Shields
			if (!spawnGroup.IgnoreShieldProviderMod && spawnGroup.FactionOwner != "Nobody") {

				bool allowedShields = true;

				if (spawnGroup.PlanetaryInstallation || (spawnGroup.SpaceRandomEncounter && spawnGroup.SpawnGroup.Voxels.Count > 0))
					allowedShields = false;

				if (allowedShields) {

					foreach (var grid in prefabDef.CubeGrids) {

						if (NPCShieldManager.AddDefenseShieldsToGrid(grid, spawnGroup.AddDefenseShieldBlocks))
							break;

					}

				}

			}

			//Weapon Randomizer
			bool randomWeaponsDone = false;

			if(spawnGroup.RandomizeWeapons == true) {

				Logger.AddMsg("Randomizing Weapons Based On SpawnGroup rules", true);

				randomWeaponsDone = true;

				foreach(var grid in prefabDef.CubeGrids) {

					RandomWeaponReplacing(grid, spawnGroup);

				}

			}

			if((MES_SessionCore.NPCWeaponUpgradesModDetected == true || Settings.General.EnableGlobalNPCWeaponRandomizer == true) && spawnGroup.IgnoreWeaponRandomizerMod == false && randomWeaponsDone == false) {

				Logger.AddMsg("Randomizing Weapons Based On World Rules", true);

				foreach(var grid in prefabDef.CubeGrids) {

					RandomWeaponReplacing(grid, spawnGroup);

				}

			}

			//Disable Blocks Cleanup Settings
			var cleanup = Cleanup.GetCleaningSettingsForType(spawnType);

			if(cleanup.UseBlockDisable == true) {

				Logger.AddMsg("Applying SpawnType Cleanup Rules and Disabling Specified Blocks", true);

				foreach(var grid in prefabDef.CubeGrids) {

					ApplyBlockDisable(grid, cleanup);

				}

			}

			//Color, Block Disable, BlockName On/Off, Turret Settings
			Logger.AddMsg("Processing Common Block Operations", true);
			foreach(var grid in prefabDef.CubeGrids) {

				ProcessCommonBlockObjectBuilders(grid, spawnGroup);

			}


			//Partial Block Construction
			if(spawnGroup.ReduceBlockBuildStates == true) {

				Logger.AddMsg("Reducing Block States", true);

				foreach(var grid in prefabDef.CubeGrids) {

					PartialBlockBuildStates(grid, spawnGroup);

				}

			}

			//Random Name Generator
			if(spawnGroup.UseRandomNameGenerator == true) {

				Logger.AddMsg("Randomizing Grid Name", true);

				string newGridName = RandomNameGenerator.CreateRandomNameFromPattern(spawnGroup.RandomGridNamePattern);
				string newRandomName = spawnGroup.RandomGridNamePrefix + newGridName;

				if(prefabDef.CubeGrids.Length > 0) {

					prefabDef.CubeGrids[0].DisplayName = newRandomName;

					foreach(var grid in prefabDef.CubeGrids) {

						for(int i = 0;i < grid.CubeBlocks.Count;i++) {

							var antenna = grid.CubeBlocks[i] as MyObjectBuilder_RadioAntenna;

							if(antenna == null) {

								continue;

							}

							var antennaName = antenna.CustomName.ToUpper();
							var replaceName = spawnGroup.ReplaceAntennaNameWithRandomizedName.ToUpper();

							if(antennaName.Contains(replaceName) && string.IsNullOrWhiteSpace(replaceName) == false) {

								(grid.CubeBlocks[i] as MyObjectBuilder_TerminalBlock).CustomName = newGridName;
								break;

							}

						}

					}

				}

			}

			//Block Name Replacer
			if(spawnGroup.UseBlockNameReplacer == true) {

				Logger.AddMsg("Renaming Blocks From SpawnGroup Rules", true);

				if(prefabDef.CubeGrids.Length > 0) {

					foreach(var grid in prefabDef.CubeGrids) {

						for(int i = 0;i < grid.CubeBlocks.Count;i++) {

							var block = grid.CubeBlocks[i] as MyObjectBuilder_TerminalBlock;

							if(block == null) {

								continue;

							}

							if(string.IsNullOrWhiteSpace(block.CustomName) == true) {

								continue;

							}

							if(spawnGroup.BlockNameReplacerReference.ContainsKey(block.CustomName) == true) {

								(grid.CubeBlocks[i] as MyObjectBuilder_TerminalBlock).CustomName = spawnGroup.BlockNameReplacerReference[block.CustomName];

							}

						}

					}

				}

			}

			//AssignContainerTypesToAllCargo
			if(spawnGroup.AssignContainerTypesToAllCargo.Count > 0) {

				Logger.AddMsg("Assigning ContainerTypes to Cargo", true);

				var dlcLockers = new List<string>();
				dlcLockers.Add("LargeBlockLockerRoom");
				dlcLockers.Add("LargeBlockLockerRoomCorner");
				dlcLockers.Add("LargeBlockLockers");

				if(prefabDef.CubeGrids.Length > 0) {

					foreach(var grid in prefabDef.CubeGrids) {

						for(int i = 0;i < grid.CubeBlocks.Count;i++) {

							var block = grid.CubeBlocks[i] as MyObjectBuilder_CargoContainer;

							if(block == null || dlcLockers.Contains(grid.CubeBlocks[i].SubtypeName) == true) {

								continue;

							}

							(grid.CubeBlocks[i] as MyObjectBuilder_CargoContainer).ContainerType = spawnGroup.AssignContainerTypesToAllCargo[Rnd.Next(0, spawnGroup.AssignContainerTypesToAllCargo.Count)];

						}

					}

				}

			}

			//Container Type Assignment
			if(spawnGroup.UseContainerTypeAssignment == true) {

				Logger.AddMsg("Assigning Specific ContainerTypes to Cargo", true);

				if(prefabDef.CubeGrids.Length > 0) {

					foreach(var grid in prefabDef.CubeGrids) {

						for(int i = 0;i < grid.CubeBlocks.Count;i++) {

							var block = grid.CubeBlocks[i] as MyObjectBuilder_CargoContainer;

							if(block == null) {

								continue;

							}

							if(string.IsNullOrWhiteSpace(block.CustomName) == true) {

								continue;

							}

							if(spawnGroup.ContainerTypeAssignmentReference.ContainsKey(block.CustomName) == true) {

								(grid.CubeBlocks[i] as MyObjectBuilder_CargoContainer).ContainerType = spawnGroup.ContainerTypeAssignmentReference[block.CustomName];

							}

						}

					}

				}

			}

			//Mod Storage Attach
			if(spawnGroup.AttachModStorageComponentToGrid == true) {

				Logger.AddMsg("Assigning ModStorageComponent", true);

				foreach(var grid in prefabDef.CubeGrids) {

					ApplyCustomStorage(grid, spawnGroup);

				}

			}

			Logger.AddMsg("Prefab Manipulation For [" + prefabName + "] in SpawnGroup [" + spawnGroup.SpawnGroupName + "] Completed.", true);


		}

		public static void ApplyBlockDisable(MyObjectBuilder_CubeGrid cubeGrid, CleanupSettings cleanSettings){
			
			foreach(var block in cubeGrid.CubeBlocks){
				
				if(cleanSettings.DisableAirVent == true){
					
					if(block as MyObjectBuilder_AirVent != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableAntenna == true){
					
					if(block as MyObjectBuilder_RadioAntenna != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableArtificialMass == true){
					
					if(block as MyObjectBuilder_VirtualMass != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableAssembler == true){
					
					if(block as MyObjectBuilder_Assembler != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableBattery == true){
					
					if(block as MyObjectBuilder_BatteryBlock != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableBeacon == true){
					
					if(block as MyObjectBuilder_Beacon != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableCollector == true){
					
					if(block as MyObjectBuilder_Collector != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableConnector == true){
					
					if(block as MyObjectBuilder_ShipConnector != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableConveyorSorter == true){
					
					if(block as MyObjectBuilder_ConveyorSorter != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableDecoy == true){
					
					if(block as MyObjectBuilder_Decoy != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableDrill == true){
					
					if(block as MyObjectBuilder_Drill != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableJumpDrive == true){
					
					if(block as MyObjectBuilder_JumpDrive != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableGasGenerator == true){
					
					if(block as MyObjectBuilder_OxygenGenerator != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableGasTank == true){
					
					if(block as MyObjectBuilder_GasTank != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableGatlingGun == true){
					
					if(block as MyObjectBuilder_SmallGatlingGun != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableGatlingTurret == true){
					
					if(block as MyObjectBuilder_LargeGatlingTurret != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableGravityGenerator == true){
					
					if(block as MyObjectBuilder_GravityGenerator != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableGrinder == true){
					
					if(block as MyObjectBuilder_ShipGrinder != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableGyro == true){
					
					if(block as MyObjectBuilder_Gyro != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableInteriorTurret == true){
					
					if(block as MyObjectBuilder_InteriorTurret != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableLandingGear == true){
					
					if(block as MyObjectBuilder_LandingGear != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableLaserAntenna == true){
					
					if(block as MyObjectBuilder_LaserAntenna != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableLcdPanel == true){
					
					if(block as MyObjectBuilder_TextPanel != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableLightBlock == true){
					
					if(block as MyObjectBuilder_LightingBlock != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableMedicalRoom == true){
					
					if(block as MyObjectBuilder_MedicalRoom != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableMergeBlock == true){
					
					if(block as MyObjectBuilder_MergeBlock != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableMissileTurret == true){
					
					if(block as MyObjectBuilder_LargeMissileTurret != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableOxygenFarm == true){
					
					if(block as MyObjectBuilder_OxygenFarm != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableParachuteHatch == true){
					
					if(block as MyObjectBuilder_Parachute != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisablePiston == true){
					
					if(block as MyObjectBuilder_PistonBase != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableProgrammableBlock == true){
					
					if(block as MyObjectBuilder_MyProgrammableBlock != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableProjector == true){
					
					if(block as MyObjectBuilder_ProjectorBase != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableReactor == true){
					
					if(block as MyObjectBuilder_Reactor != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableRefinery == true){
					
					if(block as MyObjectBuilder_Refinery != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableRocketLauncher == true){
					
					if(block as MyObjectBuilder_SmallMissileLauncher != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableReloadableRocketLauncher == true){
					
					if(block as MyObjectBuilder_SmallMissileLauncherReload != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableRotor == true){
					
					if(block as MyObjectBuilder_MotorStator != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableSensor == true){
					
					if(block as MyObjectBuilder_SensorBlock != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableSolarPanel == true){
					
					if(block as MyObjectBuilder_SolarPanel != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableSoundBlock == true){
					
					if(block as MyObjectBuilder_SoundBlock != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableSpaceBall == true){
					
					if(block as MyObjectBuilder_SpaceBall != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableTimerBlock == true){
					
					if(block as MyObjectBuilder_TimerBlock != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableThruster == true){
					
					if(block as MyObjectBuilder_Thrust != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableWelder == true){
					
					if(block as MyObjectBuilder_ShipWelder != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
				if(cleanSettings.DisableUpgradeModule == true){
					
					if(block as MyObjectBuilder_UpgradeModule != null){
						
						(block as MyObjectBuilder_FunctionalBlock).Enabled = false;
						
					}
					
				}
				
			}
			
		}
		
		public static void PartialBlockBuildStates(MyObjectBuilder_CubeGrid cubeGrid, ImprovedSpawnGroup spawnGroup){
			
			if(spawnGroup.MinimumBlocksPercent >= spawnGroup.MaximumBlocksPercent){
				
				return;
				
			}
			
			if(spawnGroup.MinimumBuildPercent >= spawnGroup.MaximumBuildPercent){
				
				return;
				
			}
			
			var targetBlocks = new List<MyObjectBuilder_CubeBlock>();
			
			foreach(var block in cubeGrid.CubeBlocks.ToList()){
				
				var defIdBlock = block.GetId();

				if(PartialBuiltAllowedBlocks.Contains(defIdBlock) == true){
					
					targetBlocks.Add(block);
					
				}
				
			}
			
			var percentOfBlocks = (double)Rnd.Next(spawnGroup.MinimumBlocksPercent, spawnGroup.MaximumBlocksPercent) / 100;
			var actualPercentOfBlocks = (int)Math.Floor((double)targetBlocks.Count * percentOfBlocks);
			
			if(actualPercentOfBlocks <= 0){
				
				return;
				
			}
			
			while(targetBlocks.Count > actualPercentOfBlocks){
				
				if(targetBlocks.Count <= 1){
					
					break;
					
				}
				
				targetBlocks.RemoveAt(Rnd.Next(0, targetBlocks.Count));
				
			}
			
			foreach(var block in targetBlocks){
				
				var buildPercent = (float)Rnd.Next(spawnGroup.MinimumBuildPercent, spawnGroup.MaximumBuildPercent);
				buildPercent /= 100;
				
				if(buildPercent <= 0 || buildPercent > 1){
					
					buildPercent = 1;
					
				}
				
				block.BuildPercent = buildPercent;
				block.IntegrityPercent = buildPercent;
				
			}
			
		}
		
		public static void ProcessCommonBlockObjectBuilders(MyObjectBuilder_CubeGrid cubeGrid, ImprovedSpawnGroup spawnGroup){
			
			//Calculate Hue Shift
			
			float shiftAmount = 0;
			
			if(spawnGroup.RandomHueShift == true){
				
				var randHue = Rnd.Next(-360, 360);
				shiftAmount = (float)randHue / 360;
				
			}else{
				
				shiftAmount = (float)spawnGroup.ShiftBlockColorAmount / 360;
				
			}

			string newSkin = "";

			if(spawnGroup.AssignGridSkin.Count > 0) {

				newSkin = spawnGroup.AssignGridSkin[Rnd.Next(0, spawnGroup.AssignGridSkin.Count)];

			}

			//Get ReplaceColor Keys
			var replaceColorList = new List<Vector3>(spawnGroup.ColorReferencePairs.Keys.ToList());

			//Get ReplaceSkin Keys
			var replaceSkinList = new List<Vector3>(spawnGroup.ColorSkinReferencePairs.Keys.ToList());

			Logger.AddMsg("Check Replacable Color", true);

			Logger.AddMsg(replaceColorList.Count.ToString() + " - " + replaceSkinList.Count.ToString(), true);

			foreach(var rsl in replaceSkinList) {

				Logger.AddMsg("RSL: " + rsl.ToString(), true);

			}

			//Damage Modifier Value
			float damageModifier = 100;
			
			if(spawnGroup.BlockDamageModifier <= 0){
				
				damageModifier = 0;
				
			}else{
				
				damageModifier = (float)spawnGroup.BlockDamageModifier / 100;
				
			}

			//Editable
			cubeGrid.Editable = spawnGroup.GridsAreEditable;

			//Destructable
			cubeGrid.DestructibleBlocks = spawnGroup.GridsAreDestructable;

			foreach(var block in cubeGrid.CubeBlocks){

				//Hue Shift
				if(spawnGroup.ShiftBlockColorsHue == true){

					if(shiftAmount > 0){
	
						var newH = block.ColorMaskHSV.X + shiftAmount;
						
						if(newH > 1){
							
							newH -= 1;
							newH += -1;
							
						}
						
						block.ColorMaskHSV.X = newH;
						
					}

					if(shiftAmount < 0){
						
						var newH = block.ColorMaskHSV.X + shiftAmount;
						
						if(newH < -1){
							
							newH -= -1;
							newH += 1;
							
						}
						
						block.ColorMaskHSV.X = newH;
						
					}
					
				}

				//Random Skin
				if(newSkin != "") {

					block.SkinSubtypeId = newSkin;

				}

				
				if(spawnGroup.RecolorGrid == true) {

					var blockColor = new Vector3(block.ColorMaskHSV.X, block.ColorMaskHSV.Y, block.ColorMaskHSV.Z);

					/*
					if(block.SubtypeName.Contains("Round")) {

						Logger.AddMsg(blockColor.ToString(), true);

					}
					*/

					//Replace Colors
					if(replaceColorList.Contains(blockColor) == true) {

						block.ColorMaskHSV = spawnGroup.ColorReferencePairs[blockColor];
						blockColor = spawnGroup.ColorReferencePairs[blockColor];

					}

					//Replace Skins
					if(replaceSkinList.Contains(blockColor) == true) {

						block.SkinSubtypeId = spawnGroup.ColorSkinReferencePairs[blockColor];

					}

				}

				//Damage Modifier
				if(spawnGroup.OverrideBlockDamageModifier == true){
					
					block.BlockGeneralDamageModifier *= damageModifier;
					
				}
				
				//Remove Authorship
				if(spawnGroup.ClearAuthorship == true){
					
					block.BuiltBy = 0;
					
				}
				
				var funcBlock = block as MyObjectBuilder_FunctionalBlock;
				var termBlock = block as MyObjectBuilder_TerminalBlock;
								
				if(funcBlock == null){
					
					continue;
					
				}

				//Disable Blocks
				
				if(spawnGroup.EraseIngameScripts == true){
					
					var pbBlock = block as MyObjectBuilder_MyProgrammableBlock;
					
					if(pbBlock != null){
						
						pbBlock.Program = null;
						pbBlock.Storage = "";
						pbBlock.DefaultRunArgument = null;
						
					}
					
				}
				
				//Replenish Systems
				if(spawnGroup.ReplenishSystems == true){
					
					var tank = block as MyObjectBuilder_GasTank;
					
					if(tank != null){
						
						tank.FilledRatio = 1;
						
					}

					var battery = block as MyObjectBuilder_BatteryBlock;

					if(battery != null) {

						float maxStored = 0;
						BatteryMaxCapacity.TryGetValue(battery.SubtypeName, out maxStored);
						battery.CurrentStoredPower = maxStored;
						battery.MaxStoredPower = maxStored;

					}

				}
				
				if(spawnGroup.DisableTimerBlocks == true){

					var timer = block as MyObjectBuilder_TimerBlock;
					
					if(timer != null){
						
						timer.IsCountingDown = false;
						
					}

				}
				
				if(spawnGroup.DisableSensorBlocks == true){
					
					var sensor = block as MyObjectBuilder_SensorBlock;
					
					if(sensor != null){
						
						sensor.Enabled = false;
						
					}
					
				}
				
				if(spawnGroup.DisableWarheads == true){
					
					var warhead = block as MyObjectBuilder_Warhead;
					
					if(warhead != null){
						
						warhead.CountdownMs = 10000;
						warhead.IsArmed = false;
						warhead.IsCountingDown = false;
						
					}
					
				}
				
				if(spawnGroup.DisableThrustOverride == true){
					
					var thrust = block as MyObjectBuilder_Thrust;
					
					if(thrust != null){
						
						thrust.ThrustOverride = 0.0f;
						
					}
					
				}
				
				if(spawnGroup.DisableGyroOverride == true){
					
					var gyro = block as MyObjectBuilder_Gyro;
					
					if(gyro != null){
						
						gyro.GyroPower = 1f;
						gyro.GyroOverride = false;
						
					}
					
				}

				if (!string.IsNullOrWhiteSpace(termBlock.CustomName)) {

					//Enable Blocks By Name
					foreach (var blockName in spawnGroup.EnableBlocksWithName) {

						if (string.IsNullOrWhiteSpace(blockName) == true) {

							continue;

						}

						if (spawnGroup.AllowPartialNames == true) {

							if (termBlock.CustomName.Contains(blockName) == true) {

								funcBlock.Enabled = true;

							}

						} else if (termBlock.CustomName == blockName) {

							funcBlock.Enabled = true;

						}

					}

					//Disable Blocks By Name
					foreach (var blockName in spawnGroup.DisableBlocksWithName) {

						if (string.IsNullOrWhiteSpace(blockName)) {

							continue;

						}

						if (spawnGroup.AllowPartialNames == true) {

							if (termBlock.CustomName.Contains(blockName) == true) {

								funcBlock.Enabled = false;

							}

						} else if (termBlock.CustomName == blockName) {

							funcBlock.Enabled = false;

						}

					}

				}

				//Turret Settings
				if(spawnGroup.ChangeTurretSettings == true){
					
					var turret = block as MyObjectBuilder_TurretBase;
					
					if(turret != null){
						
						var defId = turret.GetId(); 
						var weaponBlockDef = (MyLargeTurretBaseDefinition)MyDefinitionManager.Static.GetCubeBlockDefinition(defId);
						
						if(weaponBlockDef != null){

							if(spawnGroup.TurretRange > weaponBlockDef.MaxRangeMeters){
						
								turret.Range = weaponBlockDef.MaxRangeMeters;
					
							}else{
								
								turret.Range = (float)spawnGroup.TurretRange;
								
							}
							
							turret.EnableIdleRotation = spawnGroup.TurretIdleRotation;
							turret.TargetMeteors = spawnGroup.TurretTargetMeteors;
							turret.TargetMissiles = spawnGroup.TurretTargetMissiles;
							turret.TargetCharacters = spawnGroup.TurretTargetCharacters;
							turret.TargetSmallGrids = spawnGroup.TurretTargetSmallGrids;
							turret.TargetLargeGrids = spawnGroup.TurretTargetLargeGrids;
							turret.TargetStations = spawnGroup.TurretTargetStations;
							turret.TargetNeutrals = spawnGroup.TurretTargetNeutrals;
							
						}
						
					}
					
				}
				
			}
			
		}

		public static void ProcessGlobalBlockReplacements(MyObjectBuilder_CubeGrid cubeGrid) {

			List<MyDefinitionId> UnusedDefinitions = new List<MyDefinitionId>();

			foreach(var block in cubeGrid.CubeBlocks.ToList()) {

				var defIdBlock = block.GetId(); //Get MyDefinitionId from ObjectBuilder

				if(UnusedDefinitions.Contains(defIdBlock) == true) {

					continue;

				}

				if(GlobalBlockReplacements.ContainsKey(defIdBlock) == false) {

					Logger.AddMsg("Global Block Replacement Not Found For: " + defIdBlock.ToString(), true);
					UnusedDefinitions.Add(defIdBlock);
					continue;

				}

				var targetBlockDef = MyDefinitionManager.Static.GetCubeBlockDefinition(defIdBlock);
				var newBlockDef = MyDefinitionManager.Static.GetCubeBlockDefinition(GlobalBlockReplacements[defIdBlock]);

				if(targetBlockDef == null) {

					Logger.AddMsg("GBR Target Block Definition Null: " + defIdBlock.ToString(), true);
					continue;

				}

				if(newBlockDef == null) {

					Logger.AddMsg("GBR New Block Definition Null: " + GlobalBlockReplacements[defIdBlock].ToString(), true);
					cubeGrid.CubeBlocks.Remove(block);
					continue;

				}

				if(targetBlockDef.Size != newBlockDef.Size) {

					Logger.AddMsg("GBR New Block Wrong Size: " + newBlockDef.Id.ToString(), true);
					continue;

				}

				var newBuilder = MyObjectBuilderSerializer.CreateNewObject((SerializableDefinitionId)newBlockDef.Id);
				var newBlockBuilder = newBuilder as MyObjectBuilder_CubeBlock;

				if(newBlockBuilder == null) {

					Logger.AddMsg("GBR New Block OB Null: " + newBlockDef.Id.ToString(), true);
					continue;

				}

				if(defIdBlock.TypeId == typeof(MyObjectBuilder_Beacon)) {

					(newBlockBuilder as MyObjectBuilder_TerminalBlock).CustomName = (block as MyObjectBuilder_TerminalBlock).CustomName;
					(newBlockBuilder as MyObjectBuilder_Beacon).BroadcastRadius = (block as MyObjectBuilder_Beacon).BroadcastRadius;

				}

				if(defIdBlock.TypeId == typeof(MyObjectBuilder_RadioAntenna)) {

					(newBlockBuilder as MyObjectBuilder_TerminalBlock).CustomName = (block as MyObjectBuilder_TerminalBlock).CustomName;
					(newBlockBuilder as MyObjectBuilder_RadioAntenna).BroadcastRadius = (block as MyObjectBuilder_RadioAntenna).BroadcastRadius;

				}

				newBlockBuilder.BlockOrientation = block.BlockOrientation;
				newBlockBuilder.Min = block.Min;
				newBlockBuilder.ColorMaskHSV = block.ColorMaskHSV;
				newBlockBuilder.Owner = block.Owner;

				cubeGrid.CubeBlocks.Remove(block);
				cubeGrid.CubeBlocks.Add(newBlockBuilder);

			}

		}

		public static void ProcessBlockReplacements(MyObjectBuilder_CubeGrid cubeGrid, ImprovedSpawnGroup spawnGroup){
			
			List<MyDefinitionId> UnusedDefinitions = new List<MyDefinitionId>();
			
			foreach(var block in cubeGrid.CubeBlocks.ToList()){

				var defIdBlock = block.GetId(); //Get MyDefinitionId from ObjectBuilder
				
				if(UnusedDefinitions.Contains(defIdBlock) == true){
					
					continue;
					
				}
				
				if(spawnGroup.ReplaceBlockReference.ContainsKey(defIdBlock) == false){
					
					UnusedDefinitions.Add(defIdBlock);
					continue;
					
				}
				
				var targetBlockDef = MyDefinitionManager.Static.GetCubeBlockDefinition(defIdBlock);
				var newBlockDef = MyDefinitionManager.Static.GetCubeBlockDefinition(spawnGroup.ReplaceBlockReference[defIdBlock]);
				
				if(targetBlockDef == null){
					
					continue;
					
				}

				if(newBlockDef == null){
					
					if(spawnGroup.AlwaysRemoveBlock == true){
						
						cubeGrid.CubeBlocks.Remove(block);
						
					}
					
					continue;
					
				}
				
				if(targetBlockDef.Size != newBlockDef.Size && spawnGroup.RelaxReplacedBlocksSize == false){
					
					continue;
					
				}
				
				var newBuilder = MyObjectBuilderSerializer.CreateNewObject((SerializableDefinitionId)newBlockDef.Id);
				var newBlockBuilder = newBuilder as MyObjectBuilder_CubeBlock;
				
				if(newBlockBuilder == null){
					
					continue;
					
				}

				if(defIdBlock.TypeId == typeof(MyObjectBuilder_Beacon)) {

					(newBlockBuilder as MyObjectBuilder_TerminalBlock).CustomName = (block as MyObjectBuilder_TerminalBlock).CustomName;
					(newBlockBuilder as MyObjectBuilder_Beacon).BroadcastRadius = (block as MyObjectBuilder_Beacon).BroadcastRadius;

				}

				if(defIdBlock.TypeId == typeof(MyObjectBuilder_RadioAntenna)) {

					(newBlockBuilder as MyObjectBuilder_TerminalBlock).CustomName = (block as MyObjectBuilder_TerminalBlock).CustomName;
					(newBlockBuilder as MyObjectBuilder_RadioAntenna).BroadcastRadius = (block as MyObjectBuilder_RadioAntenna).BroadcastRadius;

				}

				newBlockBuilder.BlockOrientation = block.BlockOrientation;
				newBlockBuilder.Min = block.Min;
				newBlockBuilder.ColorMaskHSV = block.ColorMaskHSV;
				newBlockBuilder.Owner = block.Owner;

				cubeGrid.CubeBlocks.Remove(block);
				cubeGrid.CubeBlocks.Add(newBlockBuilder);
				
			}
			
		}

		public static void ApplyGlobalBlockReplacementProfile(MyObjectBuilder_CubeGrid cubeGrid) {

			foreach(var name in Settings.General.GlobalBlockReplacerProfiles) {

				var replacementReference = new Dictionary<SerializableDefinitionId, SerializableDefinitionId>();

				if(BlockReplacementProfiles.ContainsKey(name) == true) {

					replacementReference = BlockReplacementProfiles[name].ReplacementReferenceDict;

				} else {

					continue;

				}

				List<MyDefinitionId> UnusedDefinitions = new List<MyDefinitionId>();

				foreach(var block in cubeGrid.CubeBlocks.ToList()) {

					var defIdBlock = block.GetId(); //Get MyDefinitionId from ObjectBuilder

					if(UnusedDefinitions.Contains(defIdBlock) == true) {

						continue;

					}

					if(replacementReference.ContainsKey((SerializableDefinitionId)defIdBlock) == false) {

						UnusedDefinitions.Add(defIdBlock);
						continue;

					}

					var targetBlockDef = MyDefinitionManager.Static.GetCubeBlockDefinition(defIdBlock);
					var newBlockDef = MyDefinitionManager.Static.GetCubeBlockDefinition((MyDefinitionId)replacementReference[(SerializableDefinitionId)defIdBlock]);

					if(targetBlockDef == null) {

						continue;

					}

					if(newBlockDef == null) {

						cubeGrid.CubeBlocks.Remove(block);
						continue;

					}

					if(targetBlockDef.Size != newBlockDef.Size) {

						continue;

					}

					var newBuilder = MyObjectBuilderSerializer.CreateNewObject((SerializableDefinitionId)newBlockDef.Id);
					var newBlockBuilder = newBuilder as MyObjectBuilder_CubeBlock;

					if(newBlockBuilder == null) {

						continue;

					}

					if(defIdBlock.TypeId == typeof(MyObjectBuilder_Beacon)) {

						(newBlockBuilder as MyObjectBuilder_TerminalBlock).CustomName = (block as MyObjectBuilder_TerminalBlock).CustomName;
						(newBlockBuilder as MyObjectBuilder_Beacon).BroadcastRadius = (block as MyObjectBuilder_Beacon).BroadcastRadius;

					}

					if(defIdBlock.TypeId == typeof(MyObjectBuilder_RadioAntenna)) {

						(newBlockBuilder as MyObjectBuilder_TerminalBlock).CustomName = (block as MyObjectBuilder_TerminalBlock).CustomName;
						(newBlockBuilder as MyObjectBuilder_RadioAntenna).BroadcastRadius = (block as MyObjectBuilder_RadioAntenna).BroadcastRadius;

					}

					newBlockBuilder.BlockOrientation = block.BlockOrientation;
					newBlockBuilder.Min = block.Min;
					newBlockBuilder.ColorMaskHSV = block.ColorMaskHSV;
					newBlockBuilder.Owner = block.Owner;

					cubeGrid.CubeBlocks.Remove(block);
					cubeGrid.CubeBlocks.Add(newBlockBuilder);

				}

			}

		}

		public static void ApplyBlockReplacementProfile(MyObjectBuilder_CubeGrid cubeGrid, ImprovedSpawnGroup spawnGroup){
			
			foreach(var name in spawnGroup.BlockReplacerProfileNames){

				var replacementReference = new Dictionary<SerializableDefinitionId, SerializableDefinitionId>();
				
				if(BlockReplacementProfiles.ContainsKey(name) == true){
					
					replacementReference = BlockReplacementProfiles[name].ReplacementReferenceDict;
					
				}else{
					
					continue;
					
				}
				
				List<MyDefinitionId> UnusedDefinitions = new List<MyDefinitionId>();
				
				foreach(var block in cubeGrid.CubeBlocks.ToList()){
					
					var defIdBlock = block.GetId(); //Get MyDefinitionId from ObjectBuilder
					
					if(UnusedDefinitions.Contains(defIdBlock) == true){
						
						continue;
						
					}
					
					if(replacementReference.ContainsKey((SerializableDefinitionId)defIdBlock) == false){
						
						UnusedDefinitions.Add(defIdBlock);
						continue;
						
					}
					
					var targetBlockDef = MyDefinitionManager.Static.GetCubeBlockDefinition(defIdBlock);
					var newBlockDef = MyDefinitionManager.Static.GetCubeBlockDefinition((MyDefinitionId)replacementReference[(SerializableDefinitionId)defIdBlock]);
					
					if(targetBlockDef == null){
						
						continue;
						
					}
					
					if(newBlockDef == null){
						
						if(spawnGroup.AlwaysRemoveBlock == true){
							
							cubeGrid.CubeBlocks.Remove(block);
							
						}
						
						continue;
						
					}
					
					if(targetBlockDef.Size != newBlockDef.Size && spawnGroup.RelaxReplacedBlocksSize == false){
						
						continue;
						
					}
					
					var newBuilder = MyObjectBuilderSerializer.CreateNewObject((SerializableDefinitionId)newBlockDef.Id);
					var newBlockBuilder = newBuilder as MyObjectBuilder_CubeBlock;
					
					if(newBlockBuilder == null){
						
						continue;
						
					}

					if(defIdBlock.TypeId == typeof(MyObjectBuilder_Beacon)) {

						(newBlockBuilder as MyObjectBuilder_TerminalBlock).CustomName = (block as MyObjectBuilder_TerminalBlock).CustomName;
						(newBlockBuilder as MyObjectBuilder_Beacon).BroadcastRadius = (block as MyObjectBuilder_Beacon).BroadcastRadius;

					}

					if(defIdBlock.TypeId == typeof(MyObjectBuilder_RadioAntenna)) {

						(newBlockBuilder as MyObjectBuilder_TerminalBlock).CustomName = (block as MyObjectBuilder_TerminalBlock).CustomName;
						(newBlockBuilder as MyObjectBuilder_RadioAntenna).BroadcastRadius = (block as MyObjectBuilder_RadioAntenna).BroadcastRadius;

					}

					newBlockBuilder.BlockOrientation = block.BlockOrientation;
					newBlockBuilder.Min = block.Min;
					newBlockBuilder.ColorMaskHSV = block.ColorMaskHSV;
					newBlockBuilder.Owner = block.Owner;

					cubeGrid.CubeBlocks.Remove(block);
					cubeGrid.CubeBlocks.Add(newBlockBuilder);
					
				}
				
			}

		}
		
		public static bool RivalAiInitialize(MyObjectBuilder_CubeGrid cubeGrid, ImprovedSpawnGroup spawnGroup, string behaviorName = null, bool primaryBehaviorAlreadySet = false){
			
			MyObjectBuilder_RemoteControl primaryRemote = null;
			MyObjectBuilder_RemoteControl rivalAiRemote = null;

			foreach(var block in cubeGrid.CubeBlocks){
				
				var thisRemote = block as MyObjectBuilder_RemoteControl;
				
				if(thisRemote == null){
					
					continue;
					
				}else{

					if(RivalAIHelper.RivalAiControlModules.Contains(thisRemote.SubtypeName) == true) {

						rivalAiRemote = thisRemote;
						break;

					}

					if(primaryRemote == null){
						
						primaryRemote = thisRemote;
						
					}
					
					if(thisRemote.IsMainRemoteControl == true){
						
						primaryRemote = thisRemote;
						
					}

				}
				
			}

			if(primaryRemote != null && rivalAiRemote == null && spawnGroup.RivalAiReplaceRemoteControl == true) {

				if(cubeGrid.GridSizeEnum == MyCubeSize.Large) {

					primaryRemote.SubtypeName = "RivalAIRemoteControlLarge";

				} else {

					primaryRemote.SubtypeName = "RivalAIRemoteControlSmall";

				}

				rivalAiRemote = primaryRemote;
				
			}

			if (primaryBehaviorAlreadySet) {

				return false;
			
			}

			if(rivalAiRemote != null && string.IsNullOrWhiteSpace(behaviorName) == false) {

				string fullBehavior = "";

				if(RivalAIHelper.RivalAiBehaviorProfiles.TryGetValue(behaviorName, out fullBehavior) == false) {

					Logger.AddMsg("RivalAI Profile Does Not Exist For: " + behaviorName, true);
					return false;

				}

				Logger.AddMsg("Attempt Attach RivalAI CustomData", true);
				if(rivalAiRemote.ComponentContainer == null) {

					Logger.AddMsg(" - Container Created", true);
					rivalAiRemote.ComponentContainer = new MyObjectBuilder_ComponentContainer();

				}

				if(rivalAiRemote.ComponentContainer.Components == null) {

					Logger.AddMsg(" - Components List Created", true);
					rivalAiRemote.ComponentContainer.Components = new List<VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer.ComponentData>();

				}

				bool foundModStorage = false;

				Logger.AddMsg(" - Check Existing Components", true);
				foreach(var component in rivalAiRemote.ComponentContainer.Components) {

					if(component.TypeId != "MyModStorageComponentBase") {

						Logger.AddMsg("   - Non ModStorage", true);
						continue;

					}

					var storage = component.Component as MyObjectBuilder_ModStorageComponent;

					if(storage == null) {

						Logger.AddMsg("   - Created Storage Null", true);
						continue;

					}

					foundModStorage = true;

					Logger.AddMsg("   - Checking If Storage Already Contains CustomData", true);
					if(storage.Storage.Dictionary.ContainsKey(new Guid("74de02b3-27f9-4960-b1c4-27351f2b06d1")) == true) {

						Logger.AddMsg("   - CustomData Exists, Updating", true);
						storage.Storage.Dictionary[new Guid("74de02b3-27f9-4960-b1c4-27351f2b06d1")] = fullBehavior;

					} else {

						Logger.AddMsg("   - CustomData Non-Exist, Creating", true);
						storage.Storage.Dictionary.Add(new Guid("74de02b3-27f9-4960-b1c4-27351f2b06d1"), fullBehavior);

					}

				}

				if(foundModStorage == false) {

					Logger.AddMsg(" - Storage Not Found, Creating Structure and Adding CustomData", true);
					var modStorage = new MyObjectBuilder_ModStorageComponent();
					var dictA = new Dictionary<Guid, string>();
					dictA.Add(new Guid("74de02b3-27f9-4960-b1c4-27351f2b06d1"), fullBehavior);
					var dictB = new SerializableDictionary<Guid, string>(dictA);
					modStorage.Storage = dictB;
					var componentData = new VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer.ComponentData();
					componentData.TypeId = "MyModStorageComponentBase";
					componentData.Component = modStorage;
					rivalAiRemote.ComponentContainer.Components.Add(componentData);

				}

				return true;

			}
			
			return false;
			
		}
		
		public static void RandomWeaponReplacing(MyObjectBuilder_CubeGrid cubeGrid, ImprovedSpawnGroup spawnGroup){
			
			var errorDebugging = new StringBuilder();
			bool allowRandomizeWeapons = true; //Backwards compatibility laziness
			Dictionary<Vector3I, MyObjectBuilder_CubeBlock> blockMap = new Dictionary<Vector3I, MyObjectBuilder_CubeBlock>();
			List<MyObjectBuilder_CubeBlock> weaponBlocks = new List<MyObjectBuilder_CubeBlock>();
			List<MyObjectBuilder_CubeBlock> replaceBlocks = new List<MyObjectBuilder_CubeBlock>();
			float availablePower = 0;
			float gridBlockCount = 0;
			bool shieldBlocksDetected = false;
			
			//Build blockMap - This is used to determine which blocks occupy cells.
			foreach(var block in cubeGrid.CubeBlocks){
				
				gridBlockCount++;
				string defIdString = block.GetId().ToString(); //Get MyDefinitionId from ObjectBuilder
				MyCubeBlockDefinition blockDefinition = null;
				
				//Check if block directory has block.
				if(BlockDirectory.ContainsKey(defIdString) == true){
					
					blockDefinition = BlockDirectory[defIdString];
					
				}else{
					
					//If Block Definition Could Not Be Found, It 
					//Likely Means The Target Grid Is Using Modded 
					//Blocks And That Mod Is Not Loaded In The Game 
					//World.
					
					//Logger("Block Definition Could Not Be Found For [" + defIdString + "]. Weapon Randomizer May Produce Unexpected Results.");
					continue;
					
				}
				
				if(PowerProviderBlocks.ContainsKey(defIdString) == true){
					
					availablePower += PowerProviderBlocks[defIdString];
					
				}
				
				//Returns a list of all cells the block occupies
				var cellList = GetBlockCells(block.Min, blockDefinition.Size, block.BlockOrientation);
				
				//Adds to map. Throws warning if a cell was already occupied, since it should not be.
				foreach(var cell in cellList){
					
					if(blockMap.ContainsKey(cell) == false){
						
						blockMap.Add(cell, block);
						
					}else{
						
						//Logger("Cell for "+ defIdString +" Already Occupied By Another Block. This May Cause Issues.");
						
					}
					
				}
				
				//If block was a weapon, add it to the list of weapons we'll be replacing
				if(block as MyObjectBuilder_UserControllableGun != null){
					
					weaponBlocks.Add(block);
					
				}
				
				//TODO: Check CustomData For MES-Replace-Block Tag
				
			}
			
			availablePower *= 0.666f; //We only want to allow 2/3 of grid power to be used by weapons - this should be ok for most NPCs
			
			//Now Process Existing Weapon Blocks
			
			if(allowRandomizeWeapons == true){
				
				foreach(var weaponBlock in weaponBlocks){
					
					//Get details of weapon block being replaced
					string defIdString = weaponBlock.GetId().ToString();
					errorDebugging.Append("Processing Grid Weapon: ").Append(defIdString).AppendLine();
					MyCubeBlockDefinition blockDefinition = BlockDirectory[defIdString];
					MyWeaponBlockDefinition targetWeaponBlockDef = (MyWeaponBlockDefinition)blockDefinition;
					
					//Do Blacklist/Whitelist Check on Target Block
					if(targetWeaponBlockDef == null){
						
						continue;
						
					}else if(IsTargetWeaponAllowed(targetWeaponBlockDef, spawnGroup) == false){
						
						continue;
						
					}

					string oldWeaponId = defIdString;
					var weaponIds = WeaponProfiles.Keys.ToList();
					bool isTurret = false;
					bool targetNeutralSetting = false;

					if (weaponBlock as MyObjectBuilder_TurretBase != null) {

						var tempTurretOb = weaponBlock as MyObjectBuilder_TurretBase;
						targetNeutralSetting = tempTurretOb.TargetNeutrals;
						isTurret = true;
						weaponIds = new List<string>(TurretIDs);

					}

					//Get Additional Details From Old Block.
					var oldBlocksCells = GetBlockCells(weaponBlock.Min, blockDefinition.Size, weaponBlock.BlockOrientation);
					var likelyMountingCell = GetLikelyBlockMountingPoint((MyWeaponBlockDefinition)blockDefinition, cubeGrid, blockMap, weaponBlock);
					var oldOrientation = (MyBlockOrientation)weaponBlock.BlockOrientation;
					var oldColor = (Vector3)weaponBlock.ColorMaskHSV;
					var oldLocalForward = GetLocalGridDirection(weaponBlock.BlockOrientation.Forward);
					var oldLocalUp = GetLocalGridDirection(weaponBlock.BlockOrientation.Up);
					
					var oldMatrix = new MatrixI(ref likelyMountingCell, ref oldLocalForward, ref oldLocalUp);
					
					//Remove The Old Block
					cubeGrid.CubeBlocks.Remove(weaponBlock);
					
					foreach(var cell in oldBlocksCells){
						
						blockMap.Remove(cell);
						
					}
					
					//Loop through weapon IDs and choose one at random each run of the loop
					while(weaponIds.Count > 0){
						
						if(weaponIds.Count == 0){
							
							errorDebugging.Append(" - No further weapons available to process.").AppendLine();
							break;
							
						}
						
						var randIndex = Rnd.Next(0, weaponIds.Count);
						var randId = weaponIds[randIndex];
						weaponIds.RemoveAt(randIndex);
						errorDebugging.Append(" - Attempting to replace with: ").Append(randId).AppendLine();
						
						if(WeaponProfiles.ContainsKey(randId) == false){
							
							errorDebugging.Append(" - No weapon profile for .").AppendLine();
							continue;
							
						}
						
						var weaponProfile = WeaponProfiles[randId];

						if (IsWeaponStaticOrTurret(isTurret, weaponProfile) == false) {

							errorDebugging.Append(" - Did not match Turret vs Static").AppendLine();
							continue;

						}

						if (IsRandomWeaponAllowed(weaponProfile.BlockDefinition, spawnGroup) == false) {

							errorDebugging.Append(" - Did not pass Blacklist/Whitelist").AppendLine();
							continue;

						}

						if (weaponProfile.BlockDefinition.CubeSize != cubeGrid.GridSizeEnum){
							
							errorDebugging.Append(" - Block not same grid size").AppendLine();
							continue;
							
						}
						
						
						bool isPowerHog = false;
						float powerDrain = 0;
						
						//Check against manually maintained list of Subtypes that draw energy for ammo generation.
						if(PowerDrainingWeapons.ContainsKey(weaponProfile.BlockDefinition.Id.SubtypeName) == true){
							
							if(PowerDrainingWeapons[weaponProfile.BlockDefinition.Id.SubtypeName] > availablePower){
								
								continue;
								
							}
							
							isPowerHog = true;
							powerDrain = PowerDrainingWeapons[weaponProfile.BlockDefinition.Id.SubtypeName];
							
						}
						
						//Calculate Min and Get Block Cells of where new weapon would be placed.
						var estimatedMin = CalculateMinPosition(weaponProfile.BlockDefinition.Size, likelyMountingCell, oldMatrix, isTurret);
						var newBlocksCells = GetBlockCells(estimatedMin, weaponProfile.BlockDefinition.Size, oldOrientation);
						bool foundOccupiedCell = false;
						
						//Check each cell against blockMap - skip weapon if a cell is occupied 
						foreach(var cell in newBlocksCells){
							
							if(blockMap.ContainsKey(cell) == true){
								
								foundOccupiedCell = true;
								break;
								
							}
							
						}
						
						if(foundOccupiedCell == true){
							
							errorDebugging.Append(" - Grid cell occupied in proposed position.").AppendLine();
							continue;
							
						}
						
						//TODO: Learn How Mount Points Work And Try To Add That Check As Well
						//Existing Method Should Work in Most Cases Though
						
						//Create Object Builder From DefinitionID
						var newBlockBuilder = MyObjectBuilderSerializer.CreateNewObject((SerializableDefinitionId)weaponProfile.BlockDefinition.Id);
						
						//Determine If Weapon Is Turret or Gun. Build Object For That Type
						if(isTurret == true && !weaponProfile.IsWeaponCore) {
							
							var turretBuilder = newBlockBuilder as MyObjectBuilder_TurretBase;
							turretBuilder.EntityId = 0;
							turretBuilder.SubtypeName = weaponProfile.BlockDefinition.Id.SubtypeName;
							turretBuilder.Min = estimatedMin;
							turretBuilder.BlockOrientation = oldOrientation;
							turretBuilder.ColorMaskHSV = oldColor;
							
							var turretDef = (MyLargeTurretBaseDefinition)weaponProfile.BlockDefinition;
							
							if(turretDef.MaxRangeMeters <= 800){
								
								turretBuilder.Range = turretDef.MaxRangeMeters;
								
								
							}else if(gridBlockCount <= 800){
								
								if(turretDef.MaxRangeMeters <= 800){
									
									turretBuilder.Range = turretDef.MaxRangeMeters;
									
								}else{
									
									turretBuilder.Range = 800;
									
								}
								
							}else{
								
								var randRange = (float)Rnd.Next(800, (int)gridBlockCount);
								
								if(randRange > turretDef.MaxRangeMeters){
									
									randRange = turretDef.MaxRangeMeters;
									
								}
								
								turretBuilder.Range = randRange;
							}

							turretBuilder.TargetMissiles = true;
							turretBuilder.TargetCharacters = true;
							turretBuilder.TargetSmallGrids = true;
							turretBuilder.TargetLargeGrids = true;
							turretBuilder.TargetStations = true;
							turretBuilder.TargetNeutrals = targetNeutralSetting;

							cubeGrid.CubeBlocks.Add(turretBuilder as MyObjectBuilder_CubeBlock);
							
						}else{
							
							var gunBuilder = newBlockBuilder as MyObjectBuilder_CubeBlock;
							gunBuilder.EntityId = 0;
							gunBuilder.SubtypeName = weaponProfile.BlockDefinition.Id.SubtypeName;
							gunBuilder.Min = estimatedMin;
							gunBuilder.BlockOrientation = oldOrientation;
							gunBuilder.ColorMaskHSV = oldColor;
							
							cubeGrid.CubeBlocks.Add(gunBuilder as MyObjectBuilder_CubeBlock);
							
						}
						
						if(isPowerHog == true){
							
							availablePower -= powerDrain;
							
						}
						
						foreach(var cell in newBlocksCells){
							
							if(blockMap.ContainsKey(cell) == false){
								
								blockMap.Add(cell, (MyObjectBuilder_CubeBlock)newBlockBuilder);
								
							}
							
						}

						Logger.AddMsg("Replaced " + oldWeaponId + " with new weapon " + weaponProfile.BlockDefinition.Id.ToString(), true);
						break;
						
					}

				}
				
			}
			
			if(Logger.LoggerDebugMode == true){
				
				Logger.AddMsg(errorDebugging.ToString(), true);
				
			}
		
		}

		public static bool IsWeaponStaticOrTurret(bool turret, WeaponProfile weapon) {

			if (weapon.IsWeaponTurret) {

				if (!turret)
					return false;
			
			} 
			
			if(weapon.IsWeaponStatic){

				if (turret)
					return false;

			}

			return true;
		
		}


		public static bool IsRandomWeaponAllowed(MyCubeBlockDefinition weaponDefinition, ImprovedSpawnGroup spawnGroup){
			
			//Check SpawnGroup First
			if(spawnGroup.WeaponRandomizerBlacklist.Count > 0){
				
				if(spawnGroup.WeaponRandomizerBlacklist.Contains(weaponDefinition.Id.SubtypeName) == true || spawnGroup.WeaponRandomizerBlacklist.Contains(weaponDefinition.Id.ToString()) == true){
				
					return false;
				
				}
				
				if(weaponDefinition.Context.ModId != "0" && string.IsNullOrEmpty(weaponDefinition.Context.ModId) == false){
					
					foreach(var item in spawnGroup.WeaponRandomizerBlacklist){
						
						if(weaponDefinition.Context.ModId.Contains(item) == true){
						
							return false;
						
						}
						
					}
					
				}
				
			}
			
			if(spawnGroup.WeaponRandomizerWhitelist.Count > 0){
				
				bool passWhitelist = false;
				
				if(spawnGroup.WeaponRandomizerWhitelist.Contains(weaponDefinition.Id.SubtypeName) == true || spawnGroup.WeaponRandomizerWhitelist.Contains(weaponDefinition.Id.ToString()) == true){
				
					passWhitelist = true;
				
				}
				
				if(weaponDefinition.Context.ModId != "0" && string.IsNullOrEmpty(weaponDefinition.Context.ModId) == false){
				
					foreach(var item in spawnGroup.WeaponRandomizerWhitelist){
						
						if(weaponDefinition.Context.ModId.Contains(item) == true){
						
							passWhitelist = true;
						
						}
						
					}
					
				}
				
				if(passWhitelist == false){
					
					return false;
					
				}
				
			}

			//Check Settings After
			if(BlacklistedWeaponSubtypes.Count > 0 && spawnGroup.IgnoreWeaponRandomizerGlobalBlacklist == false){
				
				if(BlacklistedWeaponSubtypes.Contains(weaponDefinition.Id.SubtypeName) == true || BlacklistedWeaponSubtypes.Contains(weaponDefinition.Id.ToString()) == true){
				
					return false;
				
				}
				
				if(weaponDefinition.Context.ModId != "0" && string.IsNullOrEmpty(weaponDefinition.Context.ModId) == false){
					
					foreach(var item in BlacklistedWeaponSubtypes){
						
						if(weaponDefinition.Context.ModId.Contains(item) == true){
						
							return false;
						
						}
						
					}
					
				}
				
			}
			
			if(WhitelistedWeaponSubtypes.Count > 0 && spawnGroup.IgnoreWeaponRandomizerGlobalWhitelist == false){
				
				bool passWhitelist = false;
				
				if(WhitelistedWeaponSubtypes.Contains(weaponDefinition.Id.SubtypeName) == true || WhitelistedWeaponSubtypes.Contains(weaponDefinition.Id.ToString()) == true){
				
					passWhitelist = true;
				
				}
				
				if(weaponDefinition.Context.ModId != "0" && string.IsNullOrEmpty(weaponDefinition.Context.ModId) == false){
				
					foreach(var item in WhitelistedWeaponSubtypes){
						
						if(weaponDefinition.Context.ModId.Contains(item) == true){
						
							passWhitelist = true;
						
						}
						
					}
					
				}
				
				if(passWhitelist == false){
					
					return false;
					
				}
				
			}
			
			return true;
			
		}
		
		public static bool IsTargetWeaponAllowed(MyWeaponBlockDefinition weaponDefinition, ImprovedSpawnGroup spawnGroup){
			
			//Check SpawnGroup First
			if(spawnGroup.WeaponRandomizerTargetBlacklist.Count > 0){
				
				if(spawnGroup.WeaponRandomizerTargetBlacklist.Contains(weaponDefinition.Id.SubtypeName) == true || spawnGroup.WeaponRandomizerTargetBlacklist.Contains(weaponDefinition.Id.ToString()) == true){
				
					return false;
				
				}
				
				if(weaponDefinition.Context.ModId != "0" && string.IsNullOrEmpty(weaponDefinition.Context.ModId) == false){
					
					foreach(var item in spawnGroup.WeaponRandomizerTargetBlacklist){
						
						if(weaponDefinition.Context.ModId.Contains(item) == true){
						
							return false;
						
						}
						
					}
					
				}
				
			}
			
			if(spawnGroup.WeaponRandomizerTargetWhitelist.Count > 0){
				
				bool passWhitelist = false;
				
				if(spawnGroup.WeaponRandomizerTargetWhitelist.Contains(weaponDefinition.Id.SubtypeName) == true || spawnGroup.WeaponRandomizerTargetWhitelist.Contains(weaponDefinition.Id.ToString()) == true){
				
					passWhitelist = true;
				
				}
				
				if(weaponDefinition.Context.ModId != "0" && string.IsNullOrEmpty(weaponDefinition.Context.ModId) == false){
				
					foreach(var item in spawnGroup.WeaponRandomizerTargetWhitelist){
						
						if(weaponDefinition.Context.ModId.Contains(item) == true){
						
							passWhitelist = true;
						
						}
						
					}
					
				}
				
				if(passWhitelist == false){
					
					return false;
					
				}
				
			}

			//Check Settings After
			if(BlacklistedWeaponTargetSubtypes.Count > 0 && spawnGroup.IgnoreWeaponRandomizerTargetGlobalBlacklist == false){
				
				if(BlacklistedWeaponTargetSubtypes.Contains(weaponDefinition.Id.SubtypeName) == true || BlacklistedWeaponTargetSubtypes.Contains(weaponDefinition.Id.ToString()) == true){
				
					return false;
				
				}
				
				if(weaponDefinition.Context.ModId != "0" && string.IsNullOrEmpty(weaponDefinition.Context.ModId) == false){
					
					foreach(var item in BlacklistedWeaponTargetSubtypes){
						
						if(weaponDefinition.Context.ModId.Contains(item) == true){
						
							return false;
						
						}
						
					}
					
				}
				
			}
			
			if(WhitelistedWeaponTargetSubtypes.Count > 0 && spawnGroup.IgnoreWeaponRandomizerTargetGlobalWhitelist == false){
				
				bool passWhitelist = false;
				
				if(WhitelistedWeaponTargetSubtypes.Contains(weaponDefinition.Id.SubtypeName) == true || WhitelistedWeaponTargetSubtypes.Contains(weaponDefinition.Id.ToString()) == true){
				
					passWhitelist = true;
				
				}
				
				if(weaponDefinition.Context.ModId != "0" && string.IsNullOrEmpty(weaponDefinition.Context.ModId) == false){
				
					foreach(var item in WhitelistedWeaponTargetSubtypes){
						
						if(weaponDefinition.Context.ModId.Contains(item) == true){
						
							passWhitelist = true;
						
						}
						
					}
					
				}
				
				if(passWhitelist == false){
					
					return false;
					
				}
				
			}
			
			return true;
			
		}

		public static void ApplyCustomStorage(MyObjectBuilder_CubeGrid grid, ImprovedSpawnGroup spawnGroup) {

			if(grid.ComponentContainer == null) {

				grid.ComponentContainer = new MyObjectBuilder_ComponentContainer();

			}

			if(grid.ComponentContainer.Components == null) {

				grid.ComponentContainer.Components = new List<VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer.ComponentData>();

			}

			bool foundModStorage = false;

			foreach(var component in grid.ComponentContainer.Components) {

				if(component.TypeId != "MyModStorageComponentBase") {

					continue;

				}

				var storage = component.Component as MyObjectBuilder_ModStorageComponent;

				if(storage == null) {

					continue;

				}

				foundModStorage = true;

				if(storage.Storage.Dictionary.ContainsKey(spawnGroup.StorageKey) == true) {

					storage.Storage.Dictionary[spawnGroup.StorageKey] = spawnGroup.StorageValue;

				} else {

					storage.Storage.Dictionary.Add(spawnGroup.StorageKey, spawnGroup.StorageValue);

				}

			}

			if(foundModStorage == false) {

				var modStorage = new MyObjectBuilder_ModStorageComponent();
				var dictA = new Dictionary<Guid, string>();
				dictA.Add(spawnGroup.StorageKey, spawnGroup.StorageValue);
				var dictB = new SerializableDictionary<Guid, string>(dictA);
				modStorage.Storage = dictB;
				var componentData = new VRage.Game.ObjectBuilders.ComponentSystem.MyObjectBuilder_ComponentContainer.ComponentData();
				componentData.TypeId = "MyModStorageComponentBase";
				componentData.Component = modStorage;
				grid.ComponentContainer.Components.Add(componentData);

			}

		}
		
		public static Vector3I CalculateMinPosition(Vector3I size, Vector3I mountingCell, MatrixI mountingMatrix, bool isTurret){
						
			Vector3I minPosition = Vector3I.Zero;
			
			if(isTurret == true){
				
				var cellList = new List<Vector3I>();
				
				//Move Cells Distance
				int moveCellDist = (int)Math.Floor((double)size.X / 2);
				cellList.Add(Vector3I.Transform(new Vector3I(moveCellDist, 0, moveCellDist), mountingMatrix));
				cellList.Add(Vector3I.Transform(new Vector3I(moveCellDist * -1, 0, moveCellDist), mountingMatrix));
				cellList.Add(Vector3I.Transform(new Vector3I(moveCellDist, 0, moveCellDist * -1), mountingMatrix));
				cellList.Add(Vector3I.Transform(new Vector3I(moveCellDist * -1, 0, moveCellDist * -1), mountingMatrix));
				
				cellList.Add(Vector3I.Transform(new Vector3I(moveCellDist, size.Y - 1, moveCellDist), mountingMatrix));
				cellList.Add(Vector3I.Transform(new Vector3I(moveCellDist * -1, size.Y - 1, moveCellDist), mountingMatrix));
				cellList.Add(Vector3I.Transform(new Vector3I(moveCellDist, size.Y - 1, moveCellDist * -1), mountingMatrix));
				cellList.Add(Vector3I.Transform(new Vector3I(moveCellDist * -1, size.Y - 1, moveCellDist * -1), mountingMatrix));
				
				for(int i = 0; i < cellList.Count; i++){
					
					if(i == 0){
						
						minPosition = cellList[i];
						continue;
						
					}
					
					minPosition = Vector3I.Min(minPosition, cellList[i]);
					
				}
					
				
			}else{
				
				var forwardDist = size.Z - 1;
				Vector3I otherEnd = mountingMatrix.ForwardVector * forwardDist + mountingCell;
				minPosition = Vector3I.Min(mountingCell, otherEnd);
				
			}
			
			return minPosition;
			
		}
		
		//This is used to calculate the 'center' position of the block that is used when you get 
		//VRage.Game.ModAPI.Ingame.IMySlimBlock.Position;
		//I didn't write this.. I lifted it from MySlimBlock, since that's the only place it seems
		//to exist.
		public static Vector3I ComputePositionInGrid(MatrixI localMatrix, Vector3I blockCenter, Vector3I blockSize, Vector3I min){
			
			Vector3I center = blockCenter;
			Vector3I vector3I = blockSize - 1;
			Vector3I value;
			Vector3I.TransformNormal(ref vector3I, ref localMatrix, out value);
			Vector3I a;
			Vector3I.TransformNormal(ref center, ref localMatrix, out a);
			Vector3I vector3I2 = Vector3I.Abs(value);
			Vector3I result = a + min;
			
			if (value.X != vector3I2.X){
				
				result.X += vector3I2.X;
				
			}
			
			if (value.Y != vector3I2.Y){
				
				result.Y += vector3I2.Y;
				
			}
			
			if (value.Z != vector3I2.Z){
				
				result.Z += vector3I2.Z;
				
			}
			
			return result;
			
		}
		
		//This returns a list of cells occupied by a block. Useful to get
		//blocks that occupy multiple cells.
		public static List<Vector3I> GetBlockCells(Vector3I Min, Vector3I Size, MyBlockOrientation blockOrientation){
			
			var cellList = new List<Vector3I>();
			cellList.Add(Min);
			
			var localMatrix = new MatrixI(blockOrientation);
			
			for(int i = 0; i < Size.X; i++){
				
				for(int j = 0; j < Size.Y; j++){
					
					for(int k = 0; k < Size.Z; k++){
						
						var stepSize = new Vector3I(i,j,k);
						var transformedSize = Vector3I.TransformNormal(stepSize, ref localMatrix);
						Vector3I.Abs(ref transformedSize, out transformedSize);
						var cell = Min + transformedSize;
						
						if(cellList.Contains(cell) == false){
							
							cellList.Add(cell);
							
						}
						
					}
					
				}
				
			}
			
			return cellList;
			
		}
		
		//
		public static Vector3I GetLikelyBlockMountingPoint(MyWeaponBlockDefinition blockDefinition, MyObjectBuilder_CubeGrid cubeGrid, Dictionary<Vector3I, MyObjectBuilder_CubeBlock> blockMap, MyObjectBuilder_CubeBlock block){
			
			var direction = Vector3I.Zero;
			Vector3I likelyPosition = ComputePositionInGrid(new MatrixI(block.BlockOrientation), blockDefinition.Center, blockDefinition.Size, block.Min);
			
			if(TurretIDs.Contains(blockDefinition.Id.ToString()) == true){
				
				direction = Vector3I.Down;
				
			}else{
				
				direction = Vector3I.Backward;
				
			}
			
			var blockForward = GetLocalGridDirection(block.BlockOrientation.Forward);
			var blockUp = GetLocalGridDirection(block.BlockOrientation.Up);
			var blockLocalMatrix = new MatrixI(ref likelyPosition, ref blockForward, ref blockUp);
			bool loopBreak = false;
			
			while(loopBreak == false){
				
				var checkCell = Vector3I.Transform(direction, blockLocalMatrix);
				blockLocalMatrix = new MatrixI(ref checkCell, ref blockForward, ref blockUp);
				
				if(blockMap.ContainsKey(checkCell) == true){
					
					if(blockMap[checkCell] == block){
						
						likelyPosition = checkCell;
						
					}else{
						
						break;
						
					}
					
				}else{
					
					break;
					
				}
	
			}
			
			return likelyPosition;
			
		}
		
		//Translates a Base6Directions direction into a Vector3I
		public static Vector3I GetLocalGridDirection(Base6Directions.Direction Direction){
			
			if(Direction == Base6Directions.Direction.Forward){
				
				return new Vector3I(0,0,-1);
				
			}
			
			if(Direction == Base6Directions.Direction.Backward){
				
				return new Vector3I(0,0,1);
				
			}
			
			if(Direction == Base6Directions.Direction.Up){
				
				return new Vector3I(0,1,0);
				
			}
			
			if(Direction == Base6Directions.Direction.Down){
				
				return new Vector3I(0,-1,0);
				
			}
			
			if(Direction == Base6Directions.Direction.Left){
				
				return new Vector3I(-1,0,0);
				
			}
			
			if(Direction == Base6Directions.Direction.Right){
				
				return new Vector3I(1,0,0);
				
			}
			
			return Vector3I.Zero;
			
		}
		
	}
	
}