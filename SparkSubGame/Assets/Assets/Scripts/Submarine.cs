using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine;


public class Submarine : MonoBehaviour {

    private float verticalMovement = 0.1f;
    private float horizontalMovement = 0.18f;
    private int safeDist;
    public Text DepthLabel;
    public Text TempLabel;
    public Text OxygenLabel;
    public Text FrontLabel;
    Animator animator;

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

    Sprite DoorOpen, DoorClosed, DoorUnlocked, DoorLocked;
    GameObject[] TorpedoTubes;
    GameObject[] Torpedos;
    //GameObject Tube1Torpedo, Tube2Torpedo, Tube3Torpedo, Tube4Torpedo; //TODO remove once array set up
    GameObject InDoorPos, InDoorLock, OutDoorPos, OutDoorLock;
    int curTorpedo;

    void Awake()
    {
        //If I get it working, look for the DLL...

        DoorOpen = Resources.Load<Sprite>("UI_0");
        DoorClosed = Resources.Load<Sprite>("UI_1");
        DoorUnlocked = Resources.Load<Sprite>("UI_2");
        DoorLocked = Resources.Load<Sprite>("UI_3");


        /*Tube1Torpedo = GameObject.Find("Tube1Ammo");
        Tube2Torpedo = GameObject.Find("Tube2Ammo");
        Tube3Torpedo = GameObject.Find("Tube3Ammo");
        Tube4Torpedo = GameObject.Find("Tube4Ammo");*/

        TorpedoTubes = new GameObject[4];
        TorpedoTubes[0] = GameObject.Find("Tube1Ammo");
        TorpedoTubes[1] = GameObject.Find("Tube2Ammo");
        TorpedoTubes[2] = GameObject.Find("Tube3Ammo");
        TorpedoTubes[3] = GameObject.Find("Tube4Ammo");

        //I realise this looks horrendous but it reduces the amount of "find" calls later on so it's worth it in terms of efficiency
        Torpedos = new GameObject[25];
        for (int i = 0; i < 25; i++) {
            Torpedos[i] = GameObject.Find("Torpedo" + (i+1));
        }
        

        InDoorPos = GameObject.Find("InDoorPos");
        InDoorLock = GameObject.Find("InDoorLock");
        OutDoorPos = GameObject.Find("OutDoorPos");
        OutDoorLock = GameObject.Find("OutDoorLock");

        curTorpedo = 25;

        ResetSub();
        curSub = Get_sub_stats();
        UpdateDoors();
        animator = GetComponent<Animator>();
        UpdateUI();
    }

    void ResetSub() {
        curTorpedo = 25;
        Sub_reset();
        transform.position = new Vector3(-7.38f, 2.88f, 0f);
        for (int i = 0; i < TorpedoTubes.Length; i++) {
            TorpedoTubes[i].SetActive(true);
        }
        for (int i = 0; i < Torpedos.Length; i++) {
            Torpedos[i].SetActive(true);
        }
        //PrintStats();
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

        if (Get_innerairlock_pos() && Get_innerairlock_lock() && Get_outerairlock_pos() && Get_outerairlock_lock())
        {
            animator.Play("MovingSub");
        }
        else {
            animator.Play("IdleSub");
        }

        //Do some sub stuff
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            //move sub left
            print("Moving left (back)\n");
            Sub_go_back();
            PrintStats();
            if (prevSub.FrontSpace < curSub.FrontSpace) { transform.Translate(-horizontalMovement, 0, 0); }
        } 
		else if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            //move right
            print("Moving right (forward)\n");
            Sub_go_forward();
            PrintStats();
            if (prevSub.FrontSpace > curSub.FrontSpace) { transform.Translate(horizontalMovement, 0, 0); }
        } 
		else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //dive
            print("Diving\n");
            Sub_dive();
            //curSub = Get_sub_stats();
            PrintStats();
            if (prevSub.Depth < curSub.Depth) { transform.Translate(0, -verticalMovement, 0); }
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            //surface
            print("Surfacing\n");
            Sub_surface();
            //curSub = Get_sub_stats();
            PrintStats();
            if (prevSub.Depth > curSub.Depth) { transform.Translate(0, verticalMovement, 0); }
            
        }
        //else if (Input.GetKeyUp(KeyCode.Space))
        //{
        //    //fire torpedo
        //}

        if (Input.GetKeyUp(KeyCode.Keypad0))
        {
            //reset
            print("Resetting...\n");
            ResetSub();
        }

        if (Input.GetKeyUp(KeyCode.I))
        {
            //
            print("Closing inner door...\n");
            Close_inner_door();
            PrintStats();
        }
        if (Input.GetKeyUp(KeyCode.K))
        {
            //
            print("Locking inner door...\n");
            Lock_inner_door();
            PrintStats();
        }
        if (Input.GetKeyUp(KeyCode.O))
        {
            //
            print("Closing outer door...\n");
            Close_outer_door();
            PrintStats();
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            //
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

        UpdateUI();
    }

    void PrintStats()
    {

        curSub = Get_sub_stats();
        UpdateDoors();

        //print("Sub stats:\n");
        print("Depth: " + curSub.Depth + "\n");
        print("Temp: " + curSub.Temp + "\n");
        print("Oxygen: " + curSub.Oxygen + "\n");
        print("Front Space: " + curSub.FrontSpace + "\n");
        //print("Inner Door: " + curSub.InnerAirlockPos + "\n"); 
        //print("Inner Lock: " + curSub.InnerAirlockLock + "\n"); 
        //print("Outer Door: " + curSub.OuterAirlockPos + "\n");
        //print("Outer Lock: " + curSub.OuterAirlockLock + "\n");
        //print("Firing Array: " + curSub.FiringArray + "\n");
        //print("Ammo Silo: " + curSub.AmmoSilo + "\n");
    }

    void FireTorpedo(int n) {
        bool tubeStatus = Check_torpedotube_n(n); //true = torpedo to be fired is there

        Fire_torpedotube_n(n);

        bool output = Check_torpedotube_n(n);
        //TODO replace 15 with a var for safeDist taken from the cwk
        if (!output && tubeStatus && curSub.FrontSpace > 15) //there was a torpedo and now there isn't and there was enough safe space = success
        {
            print("Successfully fired torpedo. \nTube " + n + ": Empty\n");
            TorpedoTubes[n - 1].SetActive(false);
            /*switch (n)
            {
                case 1:
                    Tube1Torpedo.SetActive(false);
                    break;
                case 2:
                    Tube2Torpedo.SetActive(false);
                    break;
                case 3:
                    Tube3Torpedo.SetActive(false);
                    break;
                default:
                    Tube4Torpedo.SetActive(false);
                    break;
            }*/
        }
        else if (!tubeStatus) //there wasn't a torpedo to start with
        {
            print("Tube " + n + " is empty. Please reload\n"); //TODO - does this curcumstance indicate an error in the coursework? i.e. should I remove this?
        }
        else if (output) //there's still a torpedo there
        {
            print("Torpedo was not fired. \nTube " + n + ": Loaded\n");
        }
        else if (!output && tubeStatus && curSub.FrontSpace <= 15) //the torpedo has fired but the front space was too small so the sub has exploded
        {
            //TODO replace 15 with a var for safeDist taken from the cwk
            print("Ya dead"); //TODO replace this with the animation
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
            TorpedoTubes[n - 1].SetActive(true);
            /*switch (n)
            {
                case 1:
                    Tube1Torpedo.SetActive(true);
                    break;
                case 2:
                    Tube2Torpedo.SetActive(true);
                    break;
                case 3:
                    Tube3Torpedo.SetActive(true);
                    break;
                default:
                    Tube4Torpedo.SetActive(true);
                    break;
            }*/

            //GameObject torpedo = GameObject.Find("Torpedo"+curTorpedo);
            Torpedos[curTorpedo-1].SetActive(false);
            curTorpedo--;

        }
        else if (!output) //there is no torpedo
        {
            print("Something went wrong. Tube " + n + " is Empty \n");
        }
    }

    void UpdateUI() {

        if (Get_innerairlock_pos())
        {
            InDoorPos.GetComponent<SpriteRenderer>().sprite = DoorClosed;
        }
        else
        {
            InDoorPos.GetComponent<SpriteRenderer>().sprite = DoorOpen;
        }

        if (Get_outerairlock_pos())
        {
            OutDoorPos.GetComponent<SpriteRenderer>().sprite = DoorClosed;
        }
        else
        {
            OutDoorPos.GetComponent<SpriteRenderer>().sprite = DoorOpen;
        }

        if (Get_innerairlock_lock())
        {
            InDoorLock.GetComponent<SpriteRenderer>().sprite = DoorLocked;
        }
        else
        {
            InDoorLock.GetComponent<SpriteRenderer>().sprite = DoorUnlocked;
        }

        if (Get_outerairlock_lock())
        {
            OutDoorLock.GetComponent<SpriteRenderer>().sprite = DoorLocked;
        }
        else
        {
            OutDoorLock.GetComponent<SpriteRenderer>().sprite = DoorUnlocked;
        }

        //bool Get_innerairlock_pos() ? InDoorLock = "" : InDoorLock = ""; //what are the downsides to updating this project to c# 7?

        DepthLabel.text = curSub.Depth.ToString();
        TempLabel.text = curSub.Temp.ToString();
        OxygenLabel.text = curSub.Oxygen.ToString();
        FrontLabel.text = curSub.FrontSpace.ToString();
    }

   

}