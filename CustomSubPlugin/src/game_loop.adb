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
   
   function GetSubStats return Sub is
   begin
      return MySub;
   exception
      when others =>
         return (others => <>);
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
   
   procedure CloseInnerDoor is
   begin
         ToggleInner;
   exception
   when others =>
         null;
   end CloseInnerDoor;
   
   procedure CloseOuterDoor is
   begin
     ToggleOuter;
   exception
   when others =>
         null;
   end CloseOuterDoor;
   
   procedure LockInnerDoor is
   begin
	 if MySub.InnerAirlockLock = Unlocked then
	   MySub.InnerAirlockLock := Locked;
	 else
	   MySub.InnerAirlockLock := Unlocked;
	 end if;
   exception
   when others =>
         null;
   end LockInnerDoor;
   
   procedure LockOuterDoor is
   begin
   	 if MySub.OuterAirlockLock = Unlocked then
	   MySub.OuterAirlockLock := Locked;
	 else
	   MySub.OuterAirlockLock := Unlocked;
	 end if;
   exception
   when others =>
         null;
   end LockOuterDoor;
   
   procedure ResetSub is
   begin
      MySub.Depth := 0;
      MySub.Temp := 0;
      MySub.Oxygen := 100;
	  MySub.Emergency := False;
      MySub.FrontSpace := 100;
      MySub.InnerAirlockPos := Open;
      MySub.InnerAirlockLock := Unlocked;
      MySub.OuterAirlockPos := Closed;
      MySub.OuterAirlockLock := Locked;
      MySub.FiringArray := (others => Loaded);
      MySub.AmmoSilo := (others => Loaded);
   exception
   when others =>
         null;
   end ResetSub;
   
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
   
   procedure LoadTorpedoTubeN (n : Tube) is
   begin
	 Load(n);
   exception
   when others =>
         null;
   end LoadTorpedoTubeN;
   
   procedure UpdateOxygen is
   begin
	 MySub.Oxygen := MySub.Oxygen-1;
   exception
   when others =>
         null;
   end UpdateOxygen;
   
   procedure UpdateTemperature (x : TempRange) is
   begin
	 MySub.Temp := x;
   exception
   when others =>
         null;
   end UpdateTemperature;
   
   procedure StopEmergencySurface is
   begin
	 MySub.Emergency := False;
	 MySub.Depth := 0;
	 EmergencySurface;
   exception
   when others =>
         null;
   end StopEmergencySurface;
   
   function CheckEmergency return Boolean is
   begin
	 Emergency;
	 return MySub.Emergency;
   exception
   when others =>
         return False;
   end CheckEmergency;
   
end Game_Loop;