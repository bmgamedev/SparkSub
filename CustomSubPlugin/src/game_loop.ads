with Submarine; use Submarine;

--NONE OF THESE FUNCTIONS ARE FINALISED
--I'm just trying to map out what I want to do...

package Game_Loop is

   -- type CSharp_Bool is new Boolean with Size => 8;

   -- function OpenInteriorDoor return CSharp_Bool
   --   with Export,
   --   Convention => C,
   --   External_Name => "sub_open_interior_door";
	 
   -- function OpenExteriorDoor return CSharp_Bool
   --   with Export,
   --   Convention => C,
   --   External_Name => "sub_open_exterior_door";
	 
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
	 
   procedure SetUpForTesting
     with Export,
     Convention => C,
     External_Name => "Sub_set_up";

end Game_Loop;