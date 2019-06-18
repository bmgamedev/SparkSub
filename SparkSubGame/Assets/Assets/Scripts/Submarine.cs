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

    /*[DllImport("submarine")]
    private static extern int Get_sub_depth();

    [DllImport("submarine")]
    private static extern int Get_sub_front();

    [DllImport("submarine")]
    private static extern bool Get_inner_airlock_pos();*/

    private struct Sub
    {
        public byte Depth;
        public byte Temp;
        public byte Oxygen;
        public byte FrontSpace;

		public bool InnerAirlockPos; //Might change from bool to something else later (enum, etc)
		public bool InnerAirlockLock; //Might change from bool to something else later (enum, etc)
		public bool OuterAirlockPos; //Might change from bool to something else later (enum, etc)
		public bool OuterAirlockLock; //Might change from bool to something else later (enum, etc)

		public byte FiringArray; //Don't know if this is int or byte yet...
		public byte AmmoSilo; //Don't know if this is int or byte yet...
    }

	[DllImport("submarine")]
    private static extern Sub Get_sub_stats();

    Sub curSub;
    Sub prevSub;
    //bool isDiving;
    //int curDepth, curFront;

    //enum Door { Open, Closed };
    //Door InnerDoor, OuterDoor;
    //enum Lock { Unlocked, Locked };
    //Lock InnerLock, OuterLock;

    //void UpdateSub(Sub mySub) {
        /*mySub.Depth = Get_sub_depth();
        mySub.Temp = Get_sub_depth();
        mySub.Oxygen = Get_sub_depth();
        mySub.FrontSpace = Get_sub_front();
        mySub.InnerAirlockPos = Get_inner_airlock_pos();
        mySub.InnerAirlockLock = Get_sub_depth();
        mySub.OuterAirlockPos = Get_sub_depth();
        mySub.OuterAirlockLock = Get_sub_depth();*/
    //}

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

        /*if (Input.GetKeyDown(KeyCode.DownArrow) && !isDiving)
        {
            //dive
            isDiving = true;
            print("Diving\n");
            
            Sub_dive();
            curSub = Get_sub_stats();
            PrintStats();
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            isDiving = false;
        }*/


        //Do some sub stuff
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            //move sub left
            print("Moving left\n");
            //prevSub = Get_sub_stats();
            Sub_go_forward();
            curSub = Get_sub_stats();
            PrintStats();
        } 
		else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            //move right
            print("Moving right\n");
            //prevSub = Get_sub_stats();
            Sub_go_back();
            curSub = Get_sub_stats();
            PrintStats();
        } 
		else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //dive
            print("Diving\n");
            //prevSub = Get_sub_stats();
            Sub_dive();
            curSub = Get_sub_stats();
            PrintStats();
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            //surface
            print("Surfacing\n");
            //prevSub = Get_sub_stats();
            Sub_surface();
            curSub = Get_sub_stats();
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
            curSub = Get_sub_stats();
            PrintStats();
        }

        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            //reset
            print("Closing inner door...\n");
            Close_inner_door();
            curSub = Get_sub_stats();
            //UpdateDoors();
            PrintStats();
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            //reset
            print("Locking inner door...\n");
            Lock_inner_door();
            curSub = Get_sub_stats();
            //UpdateDoors();
            PrintStats();
        }
        if (Input.GetKeyUp(KeyCode.Alpha3))
        {
            //reset
            print("Closing outer door...\n");
            Close_outer_door();
            curSub = Get_sub_stats();
            //UpdateDoors();
            PrintStats();
        }
        if (Input.GetKeyUp(KeyCode.Alpha4))
        {
            //reset
            print("Locking outer door...\n");
            Lock_outer_door();
            curSub = Get_sub_stats();
            //UpdateDoors();
            PrintStats();
        }

    }

    /*void UpdateDoors() {
        if (!curSub.InnerAirlockPos == true) {
            InnerDoor = Door.Open;
        }
        else
        {
            InnerDoor = Door.Closed;
        }

        if (!curSub.InnerAirlockLock == true)
        {
            InnerLock = Lock.Locked;
        }
        else
        {
            InnerLock = Lock.Unlocked;
        }

        if (!curSub.OuterAirlockPos == true)
        {
            OuterDoor = Door.Open;
        }
        else
        {
            OuterDoor = Door.Closed;
        }

        if (!curSub.OuterAirlockLock == true)
        {
            OuterLock = Lock.Locked;
        }
        else
        {
            OuterLock = Lock.Unlocked;
        }
    }*/

    void PrintStats()
    {
        //curDepth = Get_sub_depth();
        //curFront = Get_sub_front();
        //print("Depth: " + curDepth + ", front space: " + curFront + "\n");
        //Debug.Log("old depth: " + prevSub.Depth + ", new depth: " + curSub.Depth + "\n" + "old front space: " + prevSub.FrontSpace + ", new front space: " + curSub.FrontSpace + "\n");
        //print("inner air lock pos: " + Get_inner_airlock_pos().ToString() + "\n");
        //print("inner air lock pos: " + InnerDoor + "\n");
        //print("temp: " + curSub.Temp + ", oxygen: " + curSub.Oxygen + "\n");

        curSub = Get_sub_stats();
        UpdateDoors();

        print("Sub stats:\n");
        print("Depth: " + curSub.Depth + "\n");
        print("Temp: " + curSub.Temp + "\n");
        print("Oxygen: " + curSub.Oxygen + "\n");
        print("Front Space: " + curSub.FrontSpace + "\n");
        print("Inner Door: " + curSub.InnerAirlockPos + "\n"); //False = , True = ???
        print("Inner Lock: " + curSub.InnerAirlockLock + "\n"); //False = , True = ???
        print("Outer Door: " + curSub.OuterAirlockPos + "\n");
        print("Outer Lock: " + curSub.OuterAirlockLock + "\n");
        print("Firing Array: " + curSub.FiringArray + "\n");
        print("Ammo Silo: " + curSub.AmmoSilo + "\n");
    }
}
