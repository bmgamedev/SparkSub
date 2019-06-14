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

    [DllImport("submarine")]
    private static extern void Sub_set_up();

    private struct Sub
    {
        public int Depth;
        public int Temp;
        public int Oxygen;
        public int FrontSpace;
		public bool InnerAirlockPos; //Might change from bool to something else later (enum, etc)
		public bool InnerAirlockLock; //Might change from bool to something else later (enum, etc)
		public bool OuterAirlockPos; //Might change from bool to something else later (enum, etc)
		public bool OuterAirlockLock; //Might change from bool to something else later (enum, etc)
		public byte FiringArray; //Don't know if this is int or byte yet...
		public byte AmmoSilo; //Don't know if this is int or byte yet...
    }
	
	[DllImport("submarine")]
    private static extern Sub Get_sub_stats();

    Sub curSub, prevSub;


    void Awake () {
        //If I get it working, look for the DLL...

        //curSub = get_sub_stats();
        //prevSub = get_sub_stats();

        Sub_set_up();
    }
	
	//called each frame
	void Update () {
		
		//So so so much duplication, plz fix soon...
		
		
		//Do some sub stuff
		if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            //move sub left
            print("Moving left\n");
            prevSub = Get_sub_stats();
            Sub_go_forward();
            curSub = Get_sub_stats();
            PrintStats();
        } 
		else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            //move right
            print("Moving right\n");
            prevSub = Get_sub_stats();
            Sub_go_back();
            curSub = Get_sub_stats();
            PrintStats();
        } 
		else if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            //dive
            print("Diving\n");
            prevSub = Get_sub_stats();
            Sub_dive();
            curSub = Get_sub_stats();
            PrintStats();
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            //surface
            print("Surfacing\n");
            prevSub = Get_sub_stats();
            Sub_surface();
            curSub = Get_sub_stats();
            PrintStats();
        } 
		/*else if (Input.GetKeyUp(KeyCode.Space))
        {
            //fire torpedo
        }*/

    }

    void PrintStats()
    {
        print("old depth: " + prevSub.Depth + ", new depth: " + curSub.Depth + "\n");
        print("old front space: " + prevSub.FrontSpace + ", new front space: " + curSub.FrontSpace + "\n");
    }
}
