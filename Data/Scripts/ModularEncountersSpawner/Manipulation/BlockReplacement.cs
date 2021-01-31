using ModularEncountersSpawner.Configuration;
using ModularEncountersSpawner.Templates;
using Sandbox.Common.ObjectBuilders;
using Sandbox.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VRage.Game;
using VRage.ObjectBuilders;

namespace ModularEncountersSpawner.Manipulation {
	public static class BlockReplacement {

		public static Dictionary<MyDefinitionId, MyDefinitionId> GlobalBlockReplacements = new Dictionary<MyDefinitionId, MyDefinitionId>();
		public static Dictionary<MyDefinitionId, MyDefinitionId> HeavyArmorConvertReference = new Dictionary<MyDefinitionId, MyDefinitionId>();
		public static Dictionary<string, BlockReplacementProfileMES> BlockReplacementProfiles = new Dictionary<string, BlockReplacementProfileMES>();

		public static void Setup() {

			try {

				if (BlockReplacementProfiles.ContainsKey("MES-Armor-LightToHeavy") == false) {

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
				if (BlockReplacementProfiles.ContainsKey("MES-Armor-HeavyToLight") == false) {

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
				if (BlockReplacementProfiles.ContainsKey("MES-Turret-GatlingToMissile") == false) {

					var blockReplaceGatlingTurret = new BlockReplacementProfileMES();
					blockReplaceGatlingTurret.ReplacementReferenceName = "MES-Turret-GatlingToMissile";
					blockReplaceGatlingTurret.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_LargeGatlingTurret), ""), new SerializableDefinitionId(typeof(MyObjectBuilder_LargeMissileTurret), ""));
					blockReplaceGatlingTurret.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_LargeGatlingTurret), "SmallGatlingTurret"), new SerializableDefinitionId(typeof(MyObjectBuilder_LargeMissileTurret), "SmallMissileTurret"));
					BlockReplacementProfiles.Add(blockReplaceGatlingTurret.ReplacementReferenceName, blockReplaceGatlingTurret);

				}

				//Turret Missile to Gatling
				if (BlockReplacementProfiles.ContainsKey("MES-Turret-MissileToGatling") == false) {

					var blockReplaceMissileTurret = new BlockReplacementProfileMES();
					blockReplaceMissileTurret.ReplacementReferenceName = "MES-Turret-MissileToGatling";
					blockReplaceMissileTurret.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_LargeMissileTurret), ""), new SerializableDefinitionId(typeof(MyObjectBuilder_LargeGatlingTurret), ""));
					blockReplaceMissileTurret.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_LargeMissileTurret), "SmallMissileTurret"), new SerializableDefinitionId(typeof(MyObjectBuilder_LargeGatlingTurret), "SmallGatlingTurret"));
					BlockReplacementProfiles.Add(blockReplaceMissileTurret.ReplacementReferenceName, blockReplaceMissileTurret);

				}

				//Gun Gatling to Missile
				if (BlockReplacementProfiles.ContainsKey("MES-Gun-GatlingToMissile") == false) {

					var blockReplaceGatlingGun = new BlockReplacementProfileMES();
					blockReplaceGatlingGun.ReplacementReferenceName = "MES-Gun-GatlingToMissile";
					blockReplaceGatlingGun.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_SmallGatlingGun), ""), new SerializableDefinitionId(typeof(MyObjectBuilder_SmallMissileLauncher), ""));
					BlockReplacementProfiles.Add(blockReplaceGatlingGun.ReplacementReferenceName, blockReplaceGatlingGun);

				}

				//Gun Missile to Gatling
				if (BlockReplacementProfiles.ContainsKey("MES-Gun-MissileToGatling") == false) {

					var blockReplaceMissileGun = new BlockReplacementProfileMES();
					blockReplaceMissileGun.ReplacementReferenceName = "MES-Gun-MissileToGatling";
					blockReplaceMissileGun.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_SmallMissileLauncher), ""), new SerializableDefinitionId(typeof(MyObjectBuilder_SmallGatlingGun), ""));
					BlockReplacementProfiles.Add(blockReplaceMissileGun.ReplacementReferenceName, blockReplaceMissileGun);

				}

				if (BlockReplacementProfiles.ContainsKey("MES-ProprietaryValuableBlocks") == false) {

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

				if (BlockReplacementProfiles.ContainsKey("MES-ProprietaryCompRichBlocks") == false) {

					var blockReplaceCompRichBlocks = new BlockReplacementProfileMES();
					blockReplaceCompRichBlocks.ReplacementReferenceName = "MES-ProprietaryCompRichBlocks";
					blockReplaceCompRichBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Conveyor), "LargeBlockConveyor"), new SerializableDefinitionId(typeof(MyObjectBuilder_Conveyor), "ProprietaryLargeBlockConveyor"));
					blockReplaceCompRichBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_ConveyorConnector), "ConveyorTube"), new SerializableDefinitionId(typeof(MyObjectBuilder_ConveyorConnector), "ProprietaryConveyorTube"));
					blockReplaceCompRichBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_ConveyorConnector), "ConveyorTubeCurved"), new SerializableDefinitionId(typeof(MyObjectBuilder_ConveyorConnector), "ProprietaryConveyorTubeCurved"));
					blockReplaceCompRichBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_ConveyorSorter), "LargeBlockConveyorSorter"), new SerializableDefinitionId(typeof(MyObjectBuilder_ConveyorSorter), "ProprietaryLargeBlockConveyorSorter"));
					blockReplaceCompRichBlocks.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Gyro), "LargeBlockGyro"), new SerializableDefinitionId(typeof(MyObjectBuilder_Gyro), "ProprietaryLargeBlockGyro"));
					BlockReplacementProfiles.Add(blockReplaceCompRichBlocks.ReplacementReferenceName, blockReplaceCompRichBlocks);

				}

				if (BlockReplacementProfiles.ContainsKey("MES-DisposableNpcBeacons") == false) {

					var blockReplaceDisposableBeacons = new BlockReplacementProfileMES();
					blockReplaceDisposableBeacons.ReplacementReferenceName = "MES-DisposableNpcBeacons";
					blockReplaceDisposableBeacons.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Beacon), "SmallBlockBeacon"), new SerializableDefinitionId(typeof(MyObjectBuilder_Beacon), "DisposableNpcBeaconSmall"));
					blockReplaceDisposableBeacons.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Beacon), "LargeBlockBeacon"), new SerializableDefinitionId(typeof(MyObjectBuilder_Beacon), "DisposableNpcBeaconLarge"));
					BlockReplacementProfiles.Add(blockReplaceDisposableBeacons.ReplacementReferenceName, blockReplaceDisposableBeacons);

				}

				if (BlockReplacementProfiles.ContainsKey("MES-NpcThrusters-Ion") == false) {

					var replacementProfile = new BlockReplacementProfileMES();
					replacementProfile.ReplacementReferenceName = "MES-NpcThrusters-Ion";

					replacementProfile.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "SmallBlockSmallThrust"),
						new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "MES-NPC-Thrust-Ion-SmallGrid-Small"));
					replacementProfile.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "SmallBlockLargeThrust"),
						new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "MES-NPC-Thrust-Ion-SmallGrid-Large"));
					replacementProfile.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "LargeBlockSmallThrust"),
						new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "MES-NPC-Thrust-Ion-LargeGrid-Small"));
					replacementProfile.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "LargeBlockLargeThrust"),
						new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "MES-NPC-Thrust-Ion-LargeGrid-Large"));

					replacementProfile.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "SmallBlockSmallThrustSciFi"),
						new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "MES-NPC-Thrust-IonSciFi-SmallGrid-Small"));
					replacementProfile.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "SmallBlockLargeThrustSciFi"),
						new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "MES-NPC-Thrust-IonSciFi-SmallGrid-Large"));
					replacementProfile.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "LargeBlockSmallThrustSciFi"),
						new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "MES-NPC-Thrust-IonSciFi-LargeGrid-Small"));
					replacementProfile.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "LargeBlockLargeThrustSciFi"),
						new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "MES-NPC-Thrust-IonSciFi-LargeGrid-Large"));

					BlockReplacementProfiles.Add(replacementProfile.ReplacementReferenceName, replacementProfile);

				}

				if (BlockReplacementProfiles.ContainsKey("MES-NpcThrusters-Atmo") == false) {

					var replacementProfile = new BlockReplacementProfileMES();
					replacementProfile.ReplacementReferenceName = "MES-NpcThrusters-Atmo";

					replacementProfile.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "SmallBlockSmallAtmosphericThrust"),
						new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "MES-NPC-Thrust-Atmo-SmallGrid-Small"));
					replacementProfile.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "SmallBlockLargeAtmosphericThrust"),
						new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "MES-NPC-Thrust-Atmo-SmallGrid-Large"));
					replacementProfile.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "LargeBlockSmallAtmosphericThrust"),
						new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "MES-NPC-Thrust-Atmo-LargeGrid-Small"));
					replacementProfile.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "LargeBlockLargeAtmosphericThrust"),
						new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "MES-NPC-Thrust-Atmo-LargeGrid-Large"));

					replacementProfile.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "SmallBlockSmallAtmosphericThrustSciFi"),
						new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "MES-NPC-Thrust-AtmoSciFi-SmallGrid-Small"));
					replacementProfile.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "SmallBlockLargeAtmosphericThrustSciFi"),
						new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "MES-NPC-Thrust-AtmoSciFi-SmallGrid-Large"));
					replacementProfile.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "LargeBlockSmallAtmosphericThrustSciFi"),
						new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "MES-NPC-Thrust-AtmoSciFi-LargeGrid-Small"));
					replacementProfile.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "LargeBlockLargeAtmosphericThrustSciFi"),
						new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "MES-NPC-Thrust-AtmoSciFi-LargeGrid-Large"));

					BlockReplacementProfiles.Add(replacementProfile.ReplacementReferenceName, replacementProfile);

				}

				if (BlockReplacementProfiles.ContainsKey("MES-NpcThrusters-Hydro") == false) {

					var replacementProfile = new BlockReplacementProfileMES();
					replacementProfile.ReplacementReferenceName = "MES-NpcThrusters-Hydro";
					replacementProfile.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "SmallBlockSmallHydrogenThrust"),
						new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "MES-NPC-Thrust-Hydro-SmallGrid-Small"));
					replacementProfile.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "SmallBlockLargeHydrogenThrust"),
						new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "MES-NPC-Thrust-Hydro-SmallGrid-Large"));
					replacementProfile.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "LargeBlockSmallHydrogenThrust"),
						new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "MES-NPC-Thrust-Hydro-LargeGrid-Small"));
					replacementProfile.ReplacementReferenceDict.Add(new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "LargeBlockLargeHydrogenThrust"),
						new SerializableDefinitionId(typeof(MyObjectBuilder_Thrust), "MES-NPC-Thrust-Hydro-LargeGrid-Large"));
					BlockReplacementProfiles.Add(replacementProfile.ReplacementReferenceName, replacementProfile);

				}

			} catch (Exception e) {
			
			
			
			}
		
		}

		public static void ProcessGlobalBlockReplacements(MyObjectBuilder_CubeGrid cubeGrid) {

			List<MyDefinitionId> UnusedDefinitions = new List<MyDefinitionId>();

			foreach (var block in cubeGrid.CubeBlocks.ToList()) {

				var defIdBlock = block.GetId(); //Get MyDefinitionId from ObjectBuilder

				if (UnusedDefinitions.Contains(defIdBlock) == true) {

					continue;

				}

				if (GlobalBlockReplacements.ContainsKey(defIdBlock) == false) {

					Logger.AddMsg("Global Block Replacement Not Found For: " + defIdBlock.ToString(), true);
					UnusedDefinitions.Add(defIdBlock);
					continue;

				}

				var targetBlockDef = MyDefinitionManager.Static.GetCubeBlockDefinition(defIdBlock);
				var newBlockDef = MyDefinitionManager.Static.GetCubeBlockDefinition(GlobalBlockReplacements[defIdBlock]);

				if (targetBlockDef == null) {

					Logger.AddMsg("GBR Target Block Definition Null: " + defIdBlock.ToString(), true);
					continue;

				}

				if (newBlockDef == null) {

					Logger.AddMsg("GBR New Block Definition Null: " + GlobalBlockReplacements[defIdBlock].ToString(), true);
					cubeGrid.CubeBlocks.Remove(block);
					continue;

				}

				if (targetBlockDef.Size != newBlockDef.Size) {

					Logger.AddMsg("GBR New Block Wrong Size: " + newBlockDef.Id.ToString(), true);
					continue;

				}

				var newBuilder = MyObjectBuilderSerializer.CreateNewObject(newBlockDef.Id);
				var newBlockBuilder = newBuilder as MyObjectBuilder_CubeBlock;

				if (newBlockBuilder == null) {

					Logger.AddMsg("GBR New Block OB Null: " + newBlockDef.Id.ToString(), true);
					continue;

				}

				if (block.GetId().TypeId == newBlockBuilder.GetId().TypeId) {

					block.SubtypeName = newBlockBuilder.SubtypeName;
					continue;

				}

				if (defIdBlock.TypeId == typeof(MyObjectBuilder_Beacon)) {

					(newBlockBuilder as MyObjectBuilder_TerminalBlock).CustomName = (block as MyObjectBuilder_TerminalBlock).CustomName;
					(newBlockBuilder as MyObjectBuilder_Beacon).BroadcastRadius = (block as MyObjectBuilder_Beacon).BroadcastRadius;

				}

				if (defIdBlock.TypeId == typeof(MyObjectBuilder_RadioAntenna)) {

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

		public static void ProcessBlockReplacements(MyObjectBuilder_CubeGrid cubeGrid, ImprovedSpawnGroup spawnGroup) {

			List<MyDefinitionId> UnusedDefinitions = new List<MyDefinitionId>();

			foreach (var block in cubeGrid.CubeBlocks.ToList()) {

				var defIdBlock = block.GetId(); //Get MyDefinitionId from ObjectBuilder

				if (UnusedDefinitions.Contains(defIdBlock) == true) {

					continue;

				}

				if (spawnGroup.ReplaceBlockReference.ContainsKey(defIdBlock) == false) {

					UnusedDefinitions.Add(defIdBlock);
					continue;

				}

				var targetBlockDef = MyDefinitionManager.Static.GetCubeBlockDefinition(defIdBlock);
				var newBlockDef = MyDefinitionManager.Static.GetCubeBlockDefinition(spawnGroup.ReplaceBlockReference[defIdBlock]);

				if (targetBlockDef == null) {

					continue;

				}

				if (newBlockDef == null) {

					if (spawnGroup.AlwaysRemoveBlock == true) {

						cubeGrid.CubeBlocks.Remove(block);

					}

					continue;

				}

				if (targetBlockDef.Size != newBlockDef.Size && spawnGroup.RelaxReplacedBlocksSize == false) {

					continue;

				}

				var newBuilder = MyObjectBuilderSerializer.CreateNewObject(newBlockDef.Id);
				var newBlockBuilder = newBuilder as MyObjectBuilder_CubeBlock;

				if (newBlockBuilder == null) {

					continue;

				}

				if (block.GetId().TypeId == newBlockBuilder.GetId().TypeId) {

					block.SubtypeName = newBlockBuilder.SubtypeName;
					continue;

				}

				if (defIdBlock.TypeId == typeof(MyObjectBuilder_Beacon)) {

					(newBlockBuilder as MyObjectBuilder_TerminalBlock).CustomName = (block as MyObjectBuilder_TerminalBlock).CustomName;
					(newBlockBuilder as MyObjectBuilder_Beacon).BroadcastRadius = (block as MyObjectBuilder_Beacon).BroadcastRadius;

				}

				if (defIdBlock.TypeId == typeof(MyObjectBuilder_RadioAntenna)) {

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

			Logger.AddMsg(" - Total Global Replacement Profiles: " + Settings.Grids.GlobalBlockReplacerProfiles.Length, true);

			foreach (var name in Settings.Grids.GlobalBlockReplacerProfiles) {

				var replacementReference = new Dictionary<SerializableDefinitionId, SerializableDefinitionId>();

				if (BlockReplacementProfiles.ContainsKey(name) == true) {

					replacementReference = BlockReplacementProfiles[name].ReplacementReferenceDict;

				} else {

					Logger.AddMsg(" - Block Replacement Profile Not Found: " + name, true);
					continue;

				}

				List<MyDefinitionId> UnusedDefinitions = new List<MyDefinitionId>();

				foreach (var block in cubeGrid.CubeBlocks.ToList()) {

					var defIdBlock = block.GetId(); //Get MyDefinitionId from ObjectBuilder

					if (UnusedDefinitions.Contains(defIdBlock) == true) {

						continue;

					}

					if (replacementReference.ContainsKey(defIdBlock) == false) {

						UnusedDefinitions.Add(defIdBlock);
						continue;

					}

					var targetBlockDef = MyDefinitionManager.Static.GetCubeBlockDefinition(defIdBlock);
					var newBlockDef = MyDefinitionManager.Static.GetCubeBlockDefinition(replacementReference[defIdBlock]);

					if (targetBlockDef == null) {

						continue;

					}

					if (newBlockDef == null) {

						cubeGrid.CubeBlocks.Remove(block);
						continue;

					}

					if (targetBlockDef.Size != newBlockDef.Size) {

						continue;

					}

					var newBuilder = MyObjectBuilderSerializer.CreateNewObject(newBlockDef.Id);
					var newBlockBuilder = newBuilder as MyObjectBuilder_CubeBlock;

					if (newBlockBuilder == null) {

						continue;

					}

					if (block.GetId().TypeId == newBlockBuilder.GetId().TypeId) {

						block.SubtypeName = newBlockBuilder.SubtypeName;
						continue;

					}

					if (defIdBlock.TypeId == typeof(MyObjectBuilder_Beacon)) {

						(newBlockBuilder as MyObjectBuilder_TerminalBlock).CustomName = (block as MyObjectBuilder_TerminalBlock).CustomName;
						(newBlockBuilder as MyObjectBuilder_Beacon).BroadcastRadius = (block as MyObjectBuilder_Beacon).BroadcastRadius;

					}

					if (defIdBlock.TypeId == typeof(MyObjectBuilder_RadioAntenna)) {

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

		public static void ApplyBlockReplacementProfile(MyObjectBuilder_CubeGrid cubeGrid, ImprovedSpawnGroup spawnGroup) {

			Logger.AddMsg(" - Total SpawnGroup Replacement Profiles: " + spawnGroup.BlockReplacerProfileNames.Count, true);

			foreach (var name in spawnGroup.BlockReplacerProfileNames) {

				var replacementReference = new Dictionary<SerializableDefinitionId, SerializableDefinitionId>();

				if (BlockReplacementProfiles.ContainsKey(name) == true) {

					replacementReference = BlockReplacementProfiles[name].ReplacementReferenceDict;

				} else {

					Logger.AddMsg(" - Block Replacement Profile Not Found: " + name, true);
					continue;

				}

				List<MyDefinitionId> UnusedDefinitions = new List<MyDefinitionId>();

				foreach (var block in cubeGrid.CubeBlocks.ToList()) {

					var defIdBlock = block.GetId(); //Get MyDefinitionId from ObjectBuilder

					if (UnusedDefinitions.Contains(defIdBlock) == true) {

						continue;

					}

					if (replacementReference.ContainsKey(defIdBlock) == false) {

						UnusedDefinitions.Add(defIdBlock);
						continue;

					}

					var targetBlockDef = MyDefinitionManager.Static.GetCubeBlockDefinition(defIdBlock);
					var newBlockDef = MyDefinitionManager.Static.GetCubeBlockDefinition(replacementReference[defIdBlock]);

					if (targetBlockDef == null) {

						continue;

					}

					if (newBlockDef == null) {

						if (spawnGroup.AlwaysRemoveBlock == true) {

							cubeGrid.CubeBlocks.Remove(block);

						}

						continue;

					}

					if (targetBlockDef.Size != newBlockDef.Size && spawnGroup.RelaxReplacedBlocksSize == false) {

						continue;

					}

					var newBuilder = MyObjectBuilderSerializer.CreateNewObject(newBlockDef.Id);
					var newBlockBuilder = newBuilder as MyObjectBuilder_CubeBlock;

					if (newBlockBuilder == null) {

						continue;

					}

					Logger.AddMsg(" - Replacing: [" + block.GetId() + "] with [" + newBlockBuilder.GetId() + "]", true);

					if (block.GetId().TypeId == newBlockBuilder.GetId().TypeId) {

						block.SubtypeName = newBlockBuilder.SubtypeName;
						continue;

					}

					if (defIdBlock.TypeId == typeof(MyObjectBuilder_Beacon)) {

						(newBlockBuilder as MyObjectBuilder_TerminalBlock).CustomName = (block as MyObjectBuilder_TerminalBlock).CustomName;
						(newBlockBuilder as MyObjectBuilder_Beacon).BroadcastRadius = (block as MyObjectBuilder_Beacon).BroadcastRadius;

					}

					if (defIdBlock.TypeId == typeof(MyObjectBuilder_RadioAntenna)) {

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

	}

}
