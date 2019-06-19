using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Submarine : MonoBehaviour {

    [DllImport("submarine")]
    private static extern void Sub_dive();

    [DllImport("submarine")]
    private static extern void Sub_surface();

    [DllImport("submarine")]
    private static extern void Sub_go_forward();

    [DllImport("submarine")]
    private static extern void Sub_go_back();

    //[DllImport("submarine")]
    //private static extern void Sub_set_up();

    [DllImport("submarine")]
    private static extern void Sub_reset();

    [DllImport("submarine")]
    private static extern void Close_inner_door();

    [DllImport("submarine")]
    private static extern void Close_outer_door();

    [DllImport("submarine")]
    private static extern void Lock_inner_door();

    [DllImport("submarine")]
    private static extern void Lock_outer_door();

    [DllImport("submarine")]
    private static extern bool Get_innerairlock_pos();

    [DllImport("submarine")]
    private static extern bool Get_innerairlock_lock();

    [DllImport("submarine")]
    private static extern bool Get_outerairlock_pos();

    [DllImport("submarine")]
    private static extern bool Get_outerairlock_lock();

    [DllImport("submarine")]
    private static extern void Fire_torpedotube_n(int n);
    [DllImport("submarine")]
    private static extern void Load_torpedotube_n(int n);
    [DllImport("submarine")]
    private static extern bool Check_torpedotube_n(int n);


    private struct Sub
    {
        public byte Depth;
        public byte Temp;
        public byte Oxygen;
        public byte FrontSpace;

		public bool InnerAirlockPos; 
		public bool InnerAirlockLock; 
		public bool OuterAirlockPos;
		public bool OuterAirlockLock; 

		public byte FiringArray; 
		public byte AmmoSilo; 
    }

	[DllImport("submarine")]
    private static extern Sub Get_sub_stats();

    Sub curSub;
    Sub prevSub;

    void Awake()
    {
        //If I get it working, look for the DLL...
        Sub_reset();
        curSub = Get_sub_stats();
        UpdateDoors();
        PrintStats();
    }

    void UpdateDoors() {
        curSub.InnerAirlockPos = Get_innerairlock_pos();
        curSub.InnerAirlockLock = Get_innerairlock_lock();
        curSub.OuterAirlockPos = Get_outerairlock_pos();
        curSub.OuterAirlockLock = Get_outerairlock_lock();
    }

    //called each frame
    void Update () {
        prevSub = Get_sub_stats();
        //So so so much duplication, plz fix soon...

        //Do some sub stuff
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            //move sub left
            print("Moving left\n");
            Sub_go_forward();
            PrintStats();
        } 
		else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            //move right
            print("Moving right\n");
            Sub_go_back();
            PrintStats();
        } 
		else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //dive
            print("Diving\n");
            Sub_dive();
            PrintStats();
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            //surface
            print("Surfacing\n");
            Sub_surface();
            PrintStats();
        }
        //else if (Input.GetKeyUp(KeyCode.Space))
        //{
        //    //fire torpedo
        //}

        if (Input.GetKeyUp(KeyCode.Keypad0))
        {
            //reset
            print("Resetting...\n");
            Sub_reset();
            PrintStats();
        }

        if (Input.GetKeyUp(KeyCode.I))
        {
            //reset
            print("Closing inner door...\n");
            Close_inner_door();
            PrintStats();
        }
        if (Input.GetKeyUp(KeyCode.K))
        {
            //reset
            print("Locking inner door...\n");
            Lock_inner_door();
            PrintStats();
        }
        if (Input.GetKeyUp(KeyCode.O))
        {
            //reset
            print("Closing outer door...\n");
            Close_outer_door();
            PrintStats();
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            //reset
            print("Locking outer door...\n");
            Lock_outer_door();
            PrintStats();
        }

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            //Fire tube 1
            FireTorpedo(1);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            //Fire tube 2
            FireTorpedo(2);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            //Fire tube 3
            FireTorpedo(3);
        }
        else if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            //Fire tube 4
            FireTorpedo(4);
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            //Reload tube 1
            LoadTorpedo(1);
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            //Reload tube 2
            LoadTorpedo(2);
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            //Reload tube 3
            LoadTorpedo(3);
        }
        else if (Input.GetKeyUp(KeyCode.R))
        {
            //Reload tube 4
            LoadTorpedo(4);
        }

    }

    void PrintStats()
    {

        curSub = Get_sub_stats();
        UpdateDoors();

        print("Sub stats:\n");
        print("Depth: " + curSub.Depth + "\n");
        print("Temp: " + curSub.Temp + "\n");
        print("Oxygen: " + curSub.Oxygen + "\n");
        print("Front Space: " + curSub.FrontSpace + "\n");
        print("Inner Door: " + curSub.InnerAirlockPos + "\n"); 
        print("Inner Lock: " + curSub.InnerAirlockLock + "\n"); 
        print("Outer Door: " + curSub.OuterAirlockPos + "\n");
        print("Outer Lock: " + curSub.OuterAirlockLock + "\n");
        //print("Firing Array: " + curSub.FiringArray + "\n");
        //print("Ammo Silo: " + curSub.AmmoSilo + "\n");
    }

    void FireTorpedo(int n) {
        bool tubeStatus = Check_torpedotube_n(n); //true = torpedo to be fired is there

        Fire_torpedotube_n(n);

        bool output = Check_torpedotube_n(n);
        if (!output && tubeStatus) //if there was a torpedo and now there isn't
        {
            print("Successfully fired torpedo. \nTube " + n + ": Empty\n");
        }
        else if (!tubeStatus) //if there wasn't a torpedo to start with
        {
            print("Tube " + n + " is empty. Please reload\n"); //TODO - does this curcumstance indicate an error in the coursework? i.e. should I remove this?
        }
        else if (output) //if there's still a torpedo there
        {
            print("Torpedo was not fired. \nTube " + n + ": Loaded\n");
        }
    }

    void LoadTorpedo(int n) {
        bool tubeStatus = Check_torpedotube_n(n); //true = the tube is currently loaded

        Load_torpedotube_n(n);

        bool output = Check_torpedotube_n(n);
        
        if (tubeStatus) //There is already a torpedo in there so cannot load another
        {
            print("Tube " + n + " is already loaded. Cannot add another torpedo.\n");//TODO - does this curcumstance indicate an error in the coursework? i.e. should I remove this?
        }
        else if (output) //there is a torpedo
        {
            print("Tube " + n + " is Loaded\n");
        }
        else if (!output) //there is no torpedo
        {
            print("Something went wrong. Tube " + n + " is Empty \n");
        }
    }
}
