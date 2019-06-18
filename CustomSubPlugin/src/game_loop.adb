with Ada.Real_Time; use Ada.Real_Time;
with Interfaces; use Interfaces;

package body Game_Loop is

   -- function OpenInteriorDoor return CSharp_Bool is
   -- begin
   --    return CSharp_Bool (custom_function_that_changes_door_status_and_returns_a_bool_for_success_status);
   -- exception
   --    when others =>
   --       return False;
   -- end OpenInteriorDoor;
   
   -- function OpenExteriorDoor return CSharp_Bool is
   -- begin
   --    return CSharp_Bool (custom_function_that_changes_door_status_and_returns_a_bool_for_success_status);
   -- exception
   --    when others =>
   --       return False;
   -- end OpenExteriorDoor;

   
   
-- All movement related functions will end up functioning the same 
-- and I suspect it's a case of carrying out the related function then returning the new sub values (seperate function)
-- compare these new values to the old ones and determine if there was any change
-- where do I find out if it was incorrect though? Is that within the SPARK files or do I need to put that check in Unity?

-- Should these all be procedures then, since they don't really need to return anything?

   procedure SubDive is
   begin
         Dive;
   exception
   when others =>
         null;
   end SubDive;

   -- as opposed to
   
   -- function SubDive return Integer is
   -- begin
   --    return CSharp_Bool (custom_function_that_runs_dive_and_returns_a_bool_for_success_status);
   -- exception
   --    when others =>
   --       return False;
   -- end SubDive;
   
   -- --------
   -- See notes above for the following 3 functions
   
   procedure SubSurface is
   begin
         Surface;
   exception
   when others =>
         null;
   end SubSurface;
   
   procedure SubGoForward is
   begin
         GoForward;
   exception
   when others =>
         null;
   end SubGoForward;
   
   procedure SubGoBack is
   begin
         GoBack;
   exception
   when others =>
         null;
   end SubGoBack;
   
   
   -- ---------------
   -- Emergency surface...
   -- procedure since it's not returning a value, right?
   
   procedure SubEmergencySurface is
   begin
		 EmergencySurface;
   exception
   when others =>
         null;
   end SubEmergencySurface;
   -- Still think I need a way to tell the game when this is happening 
   -- so that it can make it clear to the user and stop user input until complete
   
   
   
   
   -- how many functions will I need to run every frame?
   -- need all sub stats returned to update the UI PLUS need to always check for emergency surface
   -- (Are there any other features like emergency function where the sub needs to take an action on it's own on the basis of something else?)
   
   -- something like :
   
   --procedure UpdateSub is
   --begin
	--	 MySub := (Depth => MySub.Depth,
     --              Temp => MySub.Temp,
    --               Oxygen => MySub.Oxygen,
    --               FrontSpace => MySub.FrontSpace,
	--			   InnerAirlockPos => MySub.InnerAirlockPos,
	--			   InnerAirlockLock => MySub.InnerAirlockLock,
	--			   OuterAirlockPos => MySub.OuterAirlockPos,
	--			   OuterAirlockLock => MySub.OuterAirlockLock,
	--			   FiringArray => MySub.FiringArray,
	--			   AmmoSilo => MySub.AmmoSilo);
   --end UpdateSub;
   
   function GetSubStats return Sub is
   begin
      --UpdateSub;
      return MySub;
   exception
      when others =>
         return (others => <>); --What's this?
   end GetSubStats;

   function GetSubInnerAirlockPos return CSharp_Bool is
   begin
     if MySub.InnerAirlockPos = Closed then
	   return True;
	 else
	   return False;
	 end if;
   end GetSubInnerAirlockPos;
   
   function GetSubInnerAirlockLock return CSharp_Bool is
   begin
     if MySub.InnerAirlockLock = Locked then
	   return True;
	 else
	   return False;
	 end if;
   end GetSubInnerAirlockLock;
   
   function GetSubOuterAirlockPos return CSharp_Bool is
   begin
     if MySub.OuterAirlockPos = Closed then
	   return True;
	 else
	   return False;
	 end if;
   end GetSubOuterAirlockPos;
   
   function GetSubOuterAirlockLock return CSharp_Bool is
   begin
     if MySub.OuterAirlockLock = Locked then
	   return True;
	 else
	   return False;
	 end if;
   end GetSubOuterAirlockLock;
   
   --procedure SetUpForTesting is 
   --begin
	--	 CloseInner;
	--	 CloseOuter;
	--	 MySub.InnerAirlockLock := Locked;
  -- end SetUpForTesting;
   
   procedure CloseInnerDoor is
   begin
         CloseInner;
   exception
   when others =>
         null;
   end CloseInnerDoor;
   
   procedure CloseOuterDoor is
   begin
     CloseOuter;
   end CloseOuterDoor;
   
   procedure LockInnerDoor is
   begin
         MySub.InnerAirlockLock := Locked;
   exception
   when others =>
         null;
   end LockInnerDoor;
   
   procedure LockOuterDoor is
   begin
     MySub.OuterAirlockLock := Locked;
   end LockOuterDoor;
   
   procedure ResetSub is
   begin
      MySub.Depth := 0;
      MySub.Temp := 0;
      MySub.Oxygen := 100;
      MySub.FrontSpace := 100;
      MySub.InnerAirlockPos := Open;
      MySub.InnerAirlockLock := Unlocked;
      MySub.OuterAirlockPos := Closed;
      MySub.OuterAirlockLock := Locked;
      MySub.FiringArray := (others => Loaded);
      MySub.AmmoSilo := (others => Loaded);
   end ResetSub;
   -- ------------------------------------------
   
   --still to do:
   -- - procedure Push
   -- - procedure Pop
   -- - procedure Fire (n : Tube) (function TorpedoFire (n : Tube) return ... bool for success? Int for # left?)
   -- - procedure Load (n : Tube) (function TorpedoLoad (n : Tube) return ... bool for success? Int for # loaded?)
   -- in terms of a restart, can I write a procedure to change the stats back to default?
   
   

   
   
end Game_Loop;
