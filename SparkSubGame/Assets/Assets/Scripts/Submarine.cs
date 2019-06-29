using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;

public class Submarine : MonoBehaviour {

    //SPARK functions
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

    //Sub specific variables
    Sub curSub, prevSub;
    private Animator animator;
    Vector3 subPos = new Vector3(-24.66f, 2.88f, 0.0f);
    int curTorpedo = 25, safeDist = 15; //TODO should come from the SPARK coursework in case it changes 
    public bool isAlive;

    //UI variables
    private float verticalMovement = 0.1f;
    private float horizontalMovement = 0.18f;
    private Text DepthValue;
    private Text TempValue;
    private Text OxygenValue;
    private Text FrontSpaceValue;
    bool isPaused;
    GameObject PauseScreen;

    Sprite DoorOpen, DoorClosed, DoorUnlocked, DoorLocked;
    GameObject[] TorpedoTubes;
    GameObject[] Torpedos;
    GameObject InDoorPos, InDoorLock, OutDoorPos, OutDoorLock;

    void Awake()
    {
        //Sprites for the door indicator GameObjects
        DoorOpen = Resources.Load<Sprite>("UI_0");
        DoorClosed = Resources.Load<Sprite>("UI_1");
        DoorUnlocked = Resources.Load<Sprite>("UI_2");
        DoorLocked = Resources.Load<Sprite>("UI_3");

        //Door indicator GameObjects
        InDoorPos = GameObject.Find("InDoorPos");
        InDoorLock = GameObject.Find("InDoorLock");
        OutDoorPos = GameObject.Find("OutDoorPos");
        OutDoorLock = GameObject.Find("OutDoorLock");

        //Text objects to show the sub stats
        DepthValue = GameObject.Find("DepthVal").GetComponent<Text>();
        TempValue = GameObject.Find("TempVal").GetComponent<Text>();
        OxygenValue = GameObject.Find("OxyVal").GetComponent<Text>();
        FrontSpaceValue = GameObject.Find("FrontVal").GetComponent<Text>();

        //The torpedo tube ammo GameObjects
        TorpedoTubes = new GameObject[4];
        for (int i = 0; i < TorpedoTubes.Length; i++)
        {
            TorpedoTubes[i] = GameObject.Find("Tube" + (i + 1) + "Ammo");
        }
        //TorpedoTubes[0] = GameObject.Find("Tube1Ammo");
        //TorpedoTubes[1] = GameObject.Find("Tube2Ammo");
        //TorpedoTubes[2] = GameObject.Find("Tube3Ammo");
        //TorpedoTubes[3] = GameObject.Find("Tube4Ammo");

        //The torpedo silo GameObjects
        //I realise this looks like it's unnecessary and horrendous but it reduces the amount of "find" calls later on so it's worth it in terms of efficiency
        Torpedos = new GameObject[25];
        for (int i = 0; i < Torpedos.Length; i++)
        {
            Torpedos[i] = GameObject.Find("Torpedo" + (i+1));
        }

        //Pause screen
        PauseScreen = GameObject.Find("PauseScreen");
        PauseScreen.SetActive(false);
        isPaused = false;

        //Set Up
        ResetSub();
        curSub = Get_sub_stats();
        UpdateDoors();
        animator = GetComponent<Animator>();
        isAlive = true;
        UpdateUI();
    }

    void ResetSub() {
        curTorpedo = 25;
        Sub_reset();
        transform.position = subPos;
        for (int i = 0; i < TorpedoTubes.Length; i++)
        {
            TorpedoTubes[i].SetActive(true);
        }
        for (int i = 0; i < Torpedos.Length; i++)
        {
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

        if (Get_innerairlock_pos() && Get_innerairlock_lock() && Get_outerairlock_pos() && Get_outerairlock_lock())
        {
            animator.Play("MovingSub");
        }
        else {
            animator.Play("IdleSub");
        }

        //Do some sub stuff
        if (!isPaused && isAlive)
        {
            //move the sub
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                //move sub left
                Sub_go_back();
                curSub = Get_sub_stats();
                if (prevSub.FrontSpace < curSub.FrontSpace) { transform.Translate(-horizontalMovement, 0, 0); }
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                //move right
                Sub_go_forward();
                curSub = Get_sub_stats();
                if (prevSub.FrontSpace > curSub.FrontSpace) { transform.Translate(horizontalMovement, 0, 0); }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                //dive
                Sub_dive();
                curSub = Get_sub_stats();
                if (prevSub.Depth < curSub.Depth) { transform.Translate(0, -verticalMovement, 0); }
            }
            else if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                //surface
                Sub_surface();
                curSub = Get_sub_stats();
                if (prevSub.Depth > curSub.Depth) { transform.Translate(0, verticalMovement, 0); }
            }

            //Reset the sub
            if (Input.GetKeyUp(KeyCode.Keypad0))
            {
                //reset
                print("Resetting...\n");
                ResetSub();
            }

            //Use the doors
            if (Input.GetKeyUp(KeyCode.I))
            {
                //print("Closing inner door...\n");
                Close_inner_door();
            }
            if (Input.GetKeyUp(KeyCode.K))
            {
                //print("Locking inner door...\n");
                Lock_inner_door();
            }
            if (Input.GetKeyUp(KeyCode.O))
            {
                //print("Closing outer door...\n");
                Close_outer_door();
            }
            if (Input.GetKeyUp(KeyCode.L))
            {
                //print("Locking outer door...\n");
                Lock_outer_door();
            }

            //Fire torpedos (tube num)
            if (Input.GetKeyUp(KeyCode.Alpha1))
            {
                FireTorpedo(1);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha2))
            {
                FireTorpedo(2);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha3))
            {
                FireTorpedo(3);
            }
            else if (Input.GetKeyUp(KeyCode.Alpha4))
            {
                FireTorpedo(4);
            }

            //load torpedos (tube num)
            if (Input.GetKeyUp(KeyCode.Q))
            {
                LoadTorpedo(1);
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                LoadTorpedo(2);
            }
            else if (Input.GetKeyUp(KeyCode.E))
            {
                LoadTorpedo(3);
            }
            else if (Input.GetKeyUp(KeyCode.R))
            {
                LoadTorpedo(4);
            }
        }

        if (Input.GetKeyUp(KeyCode.P)) {
            if (isPaused)
            {
                //if already paused then unpause
                isPaused = false;
                PauseScreen.SetActive(false);
            }
            else {
                //pause the screen
                isPaused = true;
                PauseScreen.SetActive(true);
                //TODO when the temp/oxygen is controlled automatically, don't forget to pause it.
            }
        }

        UpdateUI();
    }

    /*void PrintStats()
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
    }*/

    void FireTorpedo(int n) {
        bool tubeLoaded = Check_torpedotube_n(n); //true = torpedo to be fired is there
        bool wasFired = false;

        Fire_torpedotube_n(n);

        bool output = Check_torpedotube_n(n);
        /*if (!output && tubeLoaded && curSub.FrontSpace > safeDist) //there was a torpedo and now there isn't and there was enough safe space = success
        {
            print("Successfully fired torpedo. \nTube " + n + ": Empty\n");
            TorpedoTubes[n - 1].SetActive(false);
            wasFired = true;
        }
        else if (!tubeLoaded) //there wasn't a torpedo to start with
        {
            print("Tube " + n + " is empty. Please reload\n"); //TODO - does this curcumstance indicate an error in the coursework? i.e. should I remove this?
        }
        else if (output) //there's still a torpedo there
        {
            print("Torpedo was not fired. \nTube " + n + ": Loaded\n");
        }
        else if (!output && tubeLoaded && curSub.FrontSpace <= safeDist) //the torpedo has fired but the front space was too small so the sub has exploded
        {
            print("Ya dead"); //TODO replace this with the animation
            animator.Play("SubExplode");
            Destroy(gameObject, 1.0f);
            //game over screen
            //wasFired = true;
        }*/

        if (!output && tubeLoaded && curSub.FrontSpace <= 90) //test
        {
            isAlive = false;
            animator.transform.position = gameObject.transform.position;
            //animator.Play("SubExplode");
            animator.SetBool("isAlive", isAlive);
            Destroy(gameObject, 2.0f);
        }

        if (wasFired) {
            Instantiate(Resources.Load("TorpedoPrefab"), transform.GetChild(0).transform.position, Quaternion.identity);
            wasFired = false;
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


        //TODO what are the downsides to updating this project to c# 7? MEans I can use a ternary operator for the above...
        //bool Get_innerairlock_pos() ? InDoorPos.GetComponent<SpriteRenderer>().sprite = DoorClosed : InDoorPos.GetComponent<SpriteRenderer>().sprite = DoorOpen; 

        DepthValue.text = curSub.Depth.ToString();
        TempValue.text = curSub.Temp.ToString();
        OxygenValue.text = curSub.Oxygen.ToString();
        FrontSpaceValue.text = curSub.FrontSpace.ToString();
    }

}