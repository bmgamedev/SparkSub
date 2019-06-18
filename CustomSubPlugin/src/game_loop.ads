with Submarine; use Submarine;

--NONE OF THESE FUNCTIONS ARE FINALISED
--I'm just trying to map out what I want to do...

package Game_Loop is

   type CSharp_Bool is new Boolean with Size => 8;
	 
   --what is pushback and what is comealongside? do I need bools or ints? 
   --And what do I need to do with Invariant and AtSea?
	 
   procedure SubDive -- Is it incremental or one and done?
     with Export,
     Convention => C,
     External_Name => "Sub_dive";
	 
   procedure SubSurface -- Is it incremental or one and done?
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
	 
   procedure SubEmergencySurface
     with Export,
     Convention => C,
     External_Name => "Sub_emergency_surface";
	 

   --still to do:
   -- - procedure Push
   -- - procedure Pop
   -- - procedure Fire (n : Tube) (function TorpedoFire (n : Tube) return ... bool for success? Int for # left?)
   -- - procedure Load (n : Tube) (function TorpedoLoad (n : Tube) return ... bool for success? Int for # loaded?)
	 
   function GetSubStats return Sub
     with Export,
     Convention => C,
     External_Name => "Get_sub_stats";
	 
   --procedure SetUpForTesting
    -- with Export,
    -- Convention => C,
    -- External_Name => "Sub_set_up";
	 
	 
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
	 
end Game_Loop;