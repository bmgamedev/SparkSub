-- with Ada.Real_Time; use Ada.Real_Time;
-- with Interfaces; use Interfaces;

package body Game_Loop is

   function OpenInteriorDoor return CSharp_Bool is
   begin
      return CSharp_Bool (CloseInner);
   exception
      when others =>
         return False;
   end OpenInteriorDoor;
   
   function OpenExteriorDoor return CSharp_Bool is
   begin
      return CSharp_Bool (CloseOuter);
   exception
      when others =>
         return False;
   end OpenExteriorDoor;
   
   
-- Make sure all functions that alter the sub stats are called at some point to keep the sub pos etc accurate (i.e. don't forget anything)
-- Can I get these functions to execute the related sub functions and return the new depth instead?   
-- Do I need to turn them into procedures instead?
-- All movement related functions will end up functioning the same and I suspect it's a case of carrying out the related function then returning the new values

   procedure SubDive is
   begin
         Dive;
		 return Depth;
   exception
   when others =>
         null;
   end SubDive;

   -- or
   
   function SubDive return Integer is
   begin
      return CSharp_Bool (Dive);
   exception
      when others =>
         return False; --return WHAT? Cause it's not a bool (it's prob a procedure tbh)
   end SubDive;
   
   -- --------
   -- See notes above for the following 3 functions
   
   function SubSurface return Integer is
   begin
      return CSharp_Bool (Surface);
   exception
      when others =>
         return False; 
   end SubSurface;
   
   function SubGoForward return Integer is
   begin
      return CSharp_Bool (GoForward);
   exception
      when others =>
         return False; 
   end SubGoForward;
   
   function SubGoBack return Integer is
   begin
      return CSharp_Bool (GoBack);
   exception
      when others =>
         return False; 
   end SubGoBack;
   
   -- ---------------
   -- Emergency surface...
   
   -- Can procedures return more than one thing??
   -- Want to have emergency surface return the cur temp and oxy
   -- Use these values to update the UI regardless
   -- and then have it run the Emergency surface procedure if needed based on the values
   -- (or just run it anyway since it takes no action if not needed)
   
   function SubEmergencySurface return CSharp_Bool is
   begin
      return CSharp_Bool (EmergencySurface);
   exception
      when others =>
         return False;
   end SubEmergencySurface;
   
   -- or
   
   procedure SubEmergencySurface is
   begin
		 return Depth;
		 return Temp;
		 return Oxygen;
		 EmergencySurface;
   exception
   when others =>
         null;
   end SubEmergencySurface;
   
   -- how many functions will I need to run every frame?
   -- need all sub stats returned to update the UI PLUS need to always check for emergency surface
   -- (Are there any other features like emergency function where the sub needs to take an action on it's own on the basis of something else?)
   -- something like :
   
   procedure SubStats (Depth : out Integer; 
                       Temp : out Integer; 
                       Oxygen : out Integer; 
                       FrontSpace : out Integer; 
                       InnerAirlockPos : out CSharp_Bool; -- or some custom type in place of the bool
					   InnerAirlockLock : out CSharp_Bool; -- or some custom type in place of the bool
					   OuterAirlockPos : out CSharp_Bool; -- or some custom type in place of the bool
					   OuterAirlockLock : out CSharp_Bool; -- or some custom type in place of the bool
					   FiringArray : out Integer; -- double check if this will be an int tho
					   AmmoSilo : out Integer ) is
   begin 
		 EmergencySurface;
   exception
   when others =>
         null; -- Need something different here? Can I get away with not having it? If the coursework isn't correct, could break the game...
   end SubStats;
   
   function GetSubStats return Sub is
   begin
      return MySub;
   exception
      when others =>
         return (others => <>); --What's this?
   end GetSubStats;
   
   --"Ada doesn't have a tuple construct and does not allow returning multiple values from a subprogram 
   --(except by declaring a full-fledged record type). 
   --Hence, a way to return multiple values from a subprogram is to use out parameters."
   -- procedure Read_Int (Stream : Network_Stream; Success : out Boolean; Result : out Integer);
   -- https://learn.adacore.com/courses/intro-to-ada/chapters/subprograms.html
   -- Although it's hard to tell if this is actually correct right now...
   
   -- ------------------------------------------
   
   --still to do:
   -- - procedure Push
   -- - procedure Pop
   -- - procedure Fire (n : Tube) (function TorpedoFire (n : Tube) return ... bool for success? Int for # left?)
   -- - procedure Load (n : Tube) (function TorpedoLoad (n : Tube) return ... bool for success? Int for # loaded?)
   -- in terms of a restart, can I write a procedure to change the stats back to default?
   
   

   
   
end Game_Loop;
