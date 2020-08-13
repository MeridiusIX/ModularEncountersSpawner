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
using Sandbox.Game.Lights;
using Sandbox.Game.Weapons;
using Sandbox.ModAPI;
using Sandbox.ModAPI.Interfaces;
using Sandbox.ModAPI.Interfaces.Terminal;
using SpaceEngineers.Game.ModAPI;
using ProtoBuf;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.Entity;
using VRage.Game.ModAPI;
using VRage.ModAPI;
using VRage.ObjectBuilders;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Utils;
using VRageMath;


namespace ModularEncountersSpawner.BlockLogic{
	
	//Change MyObjectBuilder_LargeGatlingTurret to the matching ObjectBuilder for your block
	[MyEntityComponentDescriptor(typeof(MyObjectBuilder_Reactor), false, "ProprietarySmallBlockSmallGenerator", "ProprietarySmallBlockLargeGenerator", "ProprietaryLargeBlockSmallGenerator", "ProprietaryLargeBlockLargeGenerator")]
	 
	public class ReactorPrimingLogic : MyGameLogicComponent{
		
		IMyReactor Reactor;
		
		public override void Init(MyObjectBuilder_EntityBase objectBuilder){
			
			base.Init(objectBuilder);
			
			try{
				
				Reactor = Entity as IMyReactor;
				NeedsUpdate |= MyEntityUpdateEnum.EACH_FRAME;
				
			}catch(Exception exc){
				
				
				
			}
			
		}
		
		public override void UpdateBeforeSimulation(){
			
			if(Reactor == null){
				
				NeedsUpdate = MyEntityUpdateEnum.NONE;
				return;
				
			}
			
			if(Reactor.IsFunctional == true){
				
				if(Reactor.GetInventory().Empty() == true){
					
					var fuelId = new MyDefinitionId(typeof(MyObjectBuilder_Ingot), "UraniumB");
					var content = (MyObjectBuilder_PhysicalObject)MyObjectBuilderSerializer.CreateNewObject(fuelId);
					var fuelItem = new MyObjectBuilder_InventoryItem { Amount = 1, Content = content };
					
					if(Reactor.GetInventory().CanItemsBeAdded(100, fuelId) == true && MyAPIGateway.Multiplayer.IsServer == true){
							
						Reactor.GetInventory().AddItems(100, fuelItem.Content);
						
					}
					
				}
				
			}
			
			NeedsUpdate = MyEntityUpdateEnum.NONE;
			
		}
		
		public override void OnRemovedFromScene(){
			
			base.OnRemovedFromScene();
			
			var Block = Entity as IMyReactor;
			
			if(Block == null){
				
				return;
				
			}
			
		}
		
		public override void OnBeforeRemovedFromContainer(){
			
			base.OnBeforeRemovedFromContainer();
			
			if(Entity.InScene == true){
				
				OnRemovedFromScene();
				
			}
			
		}
		
	}
	
}