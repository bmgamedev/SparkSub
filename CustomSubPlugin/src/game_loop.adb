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
   
   function GetSubStats return Sub is
   begin
      return MySub;
   exception
      when others =>
         return (others => <>); --What's this?
   end GetSubStats;

   
   -- ------------------------------------------
   
   --still to do:
   -- - procedure Push
   -- - procedure Pop
   -- - procedure Fire (n : Tube) (function TorpedoFire (n : Tube) return ... bool for success? Int for # left?)
   -- - procedure Load (n : Tube) (function TorpedoLoad (n : Tube) return ... bool for success? Int for # loaded?)
   -- in terms of a restart, can I write a procedure to change the stats back to default?
   
   

   
   
end Game_Loop;
