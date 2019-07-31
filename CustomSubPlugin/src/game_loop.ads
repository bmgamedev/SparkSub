with Submarine; use Submarine;

package Game_Loop is

   type CSharp_Bool is new Boolean with Size => 8;
	 
   procedure SubDive 
     with Export,
     Convention => C,
     External_Name => "Sub_dive";
	 
   procedure SubSurface 
     with Export,
     Convention => C,
     External_Name => "Sub_surface";
	 
   procedure SubGoForward 
     with Export,
     Convention => C,
     External_Name => "Sub_go_forward";
	 
   procedure SubGoBack 
     with Export,
     Convention => C,
     External_Name => "Sub_go_back";
	 
   function GetSubStats return Sub
     with Export,
     Convention => C,
     External_Name => "Get_sub_stats";
	 
   function GetSubInnerAirlockPos return CSharp_Bool
     with Export,
     Convention => C,
     External_Name => "Get_innerairlock_pos";
	
   function GetSubInnerAirlockLock return CSharp_Bool
     with Export,
     Convention => C,
     External_Name => "Get_innerairlock_lock";
	 
   function GetSubOuterAirlockPos return CSharp_Bool
     with Export,
     Convention => C,
     External_Name => "Get_outerairlock_pos";
	
   function GetSubOuterAirlockLock return CSharp_Bool
     with Export,
     Convention => C,
     External_Name => "Get_outerairlock_lock";
	 
   procedure ResetSub
     with Export,
     Convention => C,
     External_Name => "Sub_reset";
	 
   procedure CloseInnerDoor
     with Export,
     Convention => C,
     External_Name => "Close_inner_door";
   
   procedure CloseOuterDoor
     with Export,
     Convention => C,
     External_Name => "Close_outer_door";
   
   procedure LockInnerDoor
     with Export,
     Convention => C,
     External_Name => "Lock_inner_door";
   
   procedure LockOuterDoor
     with Export,
     Convention => C,
     External_Name => "Lock_outer_door";

   function CheckTorpedoTubeN (n : Tube) return CSharp_Bool
     with Export,
     Convention => C,
     External_Name => "Check_torpedotube_n";
	 
   procedure FireTorpedoTubeN (n : Tube) 
     with Export,
     Convention => C,
     External_Name => "Fire_torpedotube_n";
	 
   procedure LoadTorpedoTubeN (n : Tube) 
     with Export,
     Convention => C,
     External_Name => "Load_torpedotube_n";
	 
   procedure UpdateOxygen
     with Export,
	 Convention => C,
	 External_Name => "Update_oxygen";
	 
   procedure UpdateTemperature (x : TempRange)
     with Export,
	 Convention => C,
	 External_Name => "Update_temperature";
	 
   procedure StopEmergencySurface
     with Export,
	 Convention => C,
	 External_Name => "Stop_emergency_surface";
	 
   function CheckEmergency return Boolean
     with Export,
     Convention => C,
     External_Name => "Check_emergency";
	 
end Game_Loop;