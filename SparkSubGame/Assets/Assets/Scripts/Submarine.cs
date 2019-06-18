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

        if (Input.GetKeyUp(KeyCode.R))
        {
            //reset
            print("Resetting...\n");
            Sub_reset();
            PrintStats();
        }

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            //reset
            print("Closing inner door...\n");
            Close_inner_door();
            PrintStats();
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            //reset
            print("Locking inner door...\n");
            Lock_inner_door();
            PrintStats();
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            //reset
            print("Closing outer door...\n");
            Close_outer_door();
            PrintStats();
        }
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            //reset
            print("Locking outer door...\n");
            Lock_outer_door();
            PrintStats();
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
        print("Firing Array: " + curSub.FiringArray + "\n");
        print("Ammo Silo: " + curSub.AmmoSilo + "\n");
    }
}
