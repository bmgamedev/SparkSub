-- with Ada.Real_Time; use Ada.Real_Time;
-- with Interfaces; use Interfaces;

package body Game_Loop is

   function OpenInteriorDoor (haventcheckedifineedanyargs...) return CSharp_Bool is
   begin
      return CSharp_Bool (CloseInner);
   exception
      when others =>
         return False;
   end OpenInteriorDoor;
   
   function OpenExteriorDoor (haventcheckedifineedanyargs...) return CSharp_Bool is
   begin
      return CSharp_Bool (CloseOuter);
   exception
      when others =>
         return False;
   end OpenExteriorDoor;
   
   
--Make sure all functions that alter the sub stats are called at some point to keep the sub pos etc accurate (i.e. don't forget anything)
--Can I get these functions to execute the related sub functions and return the new depth instead?   

   function SubDive (haventcheckedifineedanyargs...) return Integer is
   begin
      return CSharp_Bool (Dive);
   exception
      when others =>
         return False; --return WHAT? Cause it's not a bool
   end SubDive;
   
   function SubSurface (haventcheckedifineedanyargs...) return Integer is
   begin
      return CSharp_Bool (Surface);
   exception
      when others =>
         return False; --return WHAT? Cause it's not a bool
   end SubSurface;
   
   function SubGoForward (haventcheckedifineedanyargs...) return Integer is
   begin
      return CSharp_Bool (GoForward);
   exception
      when others =>
         return False; --return WHAT? Cause it's not a bool
   end SubGoForward;
   
   function SubGoBack (haventcheckedifineedanyargs...) return Integer is
   begin
      return CSharp_Bool (GoBack);
   exception
      when others =>
         return False; --return WHAT? Cause it's not a bool
   end SubGoBack;
   
   function SubEmergencySurface (haventcheckedifineedanyargs...) return CSharp_Bool is
   begin
      return CSharp_Bool (EmergencySurface);
   exception
      when others =>
         return False;
   end SubEmergencySurface;
   
   
   
   --still to do:
   -- - procedure Push
   -- - procedure Pop
   -- - procedure Fire (n : Tube) (function TorpedoFire (n : Tube) return ... bool for success? Int for # left?)
   -- - procedure Load (n : Tube) (function TorpedoLoad (n : Tube) return ... bool for success? Int for # loaded?)
   
   
   
end Game_Loop;
