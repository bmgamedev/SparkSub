with Ada.Real_Time; use Ada.Real_Time;
with Interfaces; use Interfaces;

package body Game_Loop is

   procedure SubDive is
   begin
         Dive;
   exception
   when others =>
         null;
   end SubDive;
   
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
   
   

   
   function GetSubStats return Sub is
   begin
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
   -- Firing and reloading. Started but not working.
   
   
   function CheckTorpedoTubeN (n : Tube) return CSharp_Bool is
   begin
	 if MySub.FiringArray(n) = Loaded then
	   return True;
	 else
	   return False;
	 end if;
   exception
      when others =>
         return False;
   end CheckTorpedoTubeN;
   
   procedure FireTorpedoTubeN (n : Tube) is
   begin
	 Fire(n);
   exception
   when others =>
         null;
   end FireTorpedoTubeN;
   
   --function LoadTorpedoTubeN (n : Tube) return CSharp_Bool is
   --begin
	-- Load(n);
	-- if MySub.FiringArray(n) = Loaded then
	--   return True; --it was a success
	-- else
	--   return False;
	-- end if;
   --exception
   --   when others =>
    --     return False;
   --end LoadTorpedoTubeN;
   
   procedure LoadTorpedoTubeN (n : Tube) is
   begin
	 Load(n);
   exception
   when others =>
         null;
   end LoadTorpedoTubeN;
   
   
end Game_Loop;
