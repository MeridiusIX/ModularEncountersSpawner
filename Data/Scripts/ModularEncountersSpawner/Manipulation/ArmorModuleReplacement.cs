using ModularEncountersSpawner.Templates;
using System;
using System.Collections.Generic;
using System.Text;
using VRage.Game;
using VRage.ObjectBuilders;

namespace ModularEncountersSpawner.Manipulation {
	public static class ArmorModuleReplacement {

		public static List<MyDefinitionId> SmallArmor = new List<MyDefinitionId>();
		public static List<MyDefinitionId> LargeArmor = new List<MyDefinitionId>();

		private static Random _rnd = new Random();
		private static bool _setupComplete = false;

		public static void ProcessGridForModules(MyObjectBuilder_CubeGrid grid, ImprovedSpawnGroup spawnGroup) {

			if (!_setupComplete) {

				_setupComplete = true;
				SmallArmor.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallBlockArmorBlock"));
				SmallArmor.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "SmallHeavyBlockArmorBlock"));
				LargeArmor.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeBlockArmorBlock"));
				LargeArmor.Add(new MyDefinitionId(typeof(MyObjectBuilder_CubeBlock), "LargeHeavyBlockArmorBlock"));

			}

			if (grid?.CubeBlocks == null)
				return;

			var allowedArmor = grid.GridSizeEnum == MyCubeSize.Large ? LargeArmor : SmallArmor;
			var availableArmor = new List<MyObjectBuilder_CubeBlock>();

			foreach (var block in grid.CubeBlocks) {

				if (allowedArmor.Contains(block.GetId()))
					availableArmor.Add(block);

			}

			for (int i = 0; i < spawnGroup.ModulesForArmorReplacement.Count; i++) {

				if (availableArmor.Count == 0) {

					break;
				
				}

				var armorIndex = availableArmor.Count == 1 ? 0 : _rnd.Next(0, availableArmor.Count);

				if (!ReplaceArmorWithModule(grid.CubeBlocks, availableArmor[armorIndex], spawnGroup.ModulesForArmorReplacement[i])) {

					continue;

				}

				availableArmor.RemoveAt(armorIndex);

			}

			return;
		
		}

		public static bool ReplaceArmorWithModule(List<MyObjectBuilder_CubeBlock> blocks, MyObjectBuilder_CubeBlock oldBlock, SerializableDefinitionId newBlockId) {

			var newBlock = MyObjectBuilderSerializer.CreateNewObject(newBlockId) as MyObjectBuilder_CubeBlock;

			if (newBlock == null) {

				Logger.AddMsg("Could Not Add Module To Prefab, It Does Not Exist: " + newBlockId.ToString());
				return false;

			}
			
			newBlock.BlockOrientation = oldBlock.BlockOrientation;
			newBlock.Min = oldBlock.Min;
			newBlock.ColorMaskHSV = oldBlock.ColorMaskHSV;
			newBlock.Owner = oldBlock.Owner;

			blocks.Remove(oldBlock);
			blocks.Add(newBlock);

			return true;
		
		}

	}

}
