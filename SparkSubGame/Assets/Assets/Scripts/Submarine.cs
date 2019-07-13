using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEditor.Animations;

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

    [DllImport("submarine")]
    private static extern void Update_oxygen();

    [DllImport("submarine")]
    private static extern void Update_temperature(int x);

    [DllImport("submarine")]
    private static extern void Stop_emergency_surface();

    [DllImport("submarine")]
    private static extern bool Check_emergency();


    private struct Sub
    {
        public byte Depth;
        public byte Temp;
        public byte Oxygen;
        public byte EmergencyStatus;
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
    AudioSource audioSource;
    public AudioClip doorSFX, torpedoSFX, implosionSFX, reloadSFX;
    Vector3 subPos = new Vector3(-24.66f, 2.88f, 0.0f);
    int curTorpedo = 25, safeDist = 15; //TODO should come from the SPARK coursework in case it changes 
    public bool isAlive, isReady, isSurfacing;

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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; //This would be better in a game manager script but it's the only thing I would have in it for this game right now.

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

        //The torpedo silo GameObjects
        //I realise this looks like it's unnecessary but it reduces the amount of "find" calls later on so it's worth it in terms of efficiency
        Torpedos = new GameObject[25];
        for (int i = 0; i < Torpedos.Length; i++)
        {
            Torpedos[i] = GameObject.Find("Torpedo" + (i+1));
        }

        //Pause screen
        PauseScreen = GameObject.Find("PauseScreen");
        PauseScreen.SetActive(false);
        isPaused = false;

        //Animations
        animator = GetComponent<Animator>();

        //sounds
        audioSource = GetComponent<AudioSource>();

        //Set Up
        ResetSub();
        curSub = Get_sub_stats();
        UpdateDoors();
        isAlive = true;
        isReady = false;
        isSurfacing = false;
        UpdateUI();

        //reduce the oxygen steadily
        //StartCoroutine("UpdateOxygen");
        InvokeRepeating("UpdateOxygen", 3.0f,3.0f);

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
    }

    void UpdateDoors() {
        curSub.InnerAirlockPos = Get_innerairlock_pos();
        curSub.InnerAirlockLock = Get_innerairlock_lock();
        curSub.OuterAirlockPos = Get_outerairlock_pos();
        curSub.OuterAirlockLock = Get_outerairlock_lock();
    }

    void Update () {
        print("emergency sub stat: " + curSub.EmergencyStatus);

        prevSub = Get_sub_stats();

        if (!Get_innerairlock_pos() || !Get_innerairlock_lock() || !Get_outerairlock_pos() || !Get_outerairlock_lock())
        {
            isReady = false;
            
        }
        else {
            isReady = true;
        }
        animator.SetBool("isReady", isReady);
        animator.SetBool("isAlive", isAlive);

        //check for emergency surface
        if (Check_emergency())
        {
            if (transform.position.y != subPos.y)
            {
                print("pos is: " + transform.position.y + ", should be: " + subPos.y);
                isSurfacing = true;
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, subPos.y), 1.0f * Time.deltaTime);
            }
            else {
                Stop_emergency_surface();
                isSurfacing = false;
            }
        }

        //Do some sub stuff
        if (!isPaused && isAlive && !isSurfacing)
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
                if (curSub.Depth > 100 && prevSub.Depth < curSub.Depth) {
                    transform.Translate(0, -verticalMovement, 0);
                    Vector3 targetPos = transform.position - new Vector3(0.0f, 2.5f, 0.0f);
                    StartCoroutine(TriggerImplosion(targetPos));
                }
                else if (prevSub.Depth < curSub.Depth) { transform.Translate(0, -verticalMovement, 0); }
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
                audioSource.PlayOneShot(doorSFX);
                Close_inner_door();
            }
            if (Input.GetKeyUp(KeyCode.K))
            {
                //print("Locking inner door...\n");
                audioSource.PlayOneShot(doorSFX);
                Lock_inner_door();
            }
            if (Input.GetKeyUp(KeyCode.O))
            {
                //print("Closing outer door...\n");
                audioSource.PlayOneShot(doorSFX);
                Close_outer_door();
            }
            if (Input.GetKeyUp(KeyCode.L))
            {
                //print("Locking outer door...\n");
                audioSource.PlayOneShot(doorSFX);
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

        // FOR TESTING PURPOSES 
        if (Input.GetKeyUp(KeyCode.T)) {
            //Vector3 targetPos = transform.position - new Vector3(0.0f, 2.5f, 0.0f);
            //StartCoroutine(TriggerImplosion(targetPos));

            //StartCoroutine(TriggerExplosion());

            isSurfacing = true;
        }

        if (Input.GetKeyUp(KeyCode.P) && !isSurfacing) {
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

    void FireTorpedo(int n) {
        bool tubeLoaded = Check_torpedotube_n(n); //true = torpedo to be fired is there
        bool wasFired = false;

        Fire_torpedotube_n(n);

        bool output = Check_torpedotube_n(n);
        if (!output && tubeLoaded && curSub.FrontSpace > safeDist) //there was a torpedo and now there isn't and there was enough safe space = success
        {
            audioSource.PlayOneShot(torpedoSFX);
            print("Successfully fired torpedo. \nTube " + n + ": Empty\n");
            TorpedoTubes[n - 1].SetActive(false);
            wasFired = true;
        }
        else if (!tubeLoaded) //there wasn't a torpedo to start with
        {
            print("Tube " + n + " is empty. Please reload\n"); //TODO - does this circumstance indicate an error in the coursework? i.e. should I remove this?
        }
        else if (output) //there's still a torpedo there
        {
            print("Torpedo was not fired. \nTube " + n + ": Loaded\n");
        }
        else if (!output && tubeLoaded && curSub.FrontSpace <= safeDist) //the torpedo has fired but the front space was too small so the sub has exploded
        //if (!output && tubeLoaded && curSub.FrontSpace <= 95) //for testing
        {
            print("Ya dead");
            wasFired = true;
            PlayerPrefs.SetString("PlayerDeath", "A torpedo exploded too close to the sub.");
            StartCoroutine(TriggerExplosion());
        }

        if (wasFired) {
            Instantiate(Resources.Load("TorpedoPrefab"), transform.GetChild(0).transform.position, Quaternion.identity);
            wasFired = false;
        }
    }

    IEnumerator TriggerImplosion(Vector3 targetPos)
    {
        isAlive = false;
        animator.SetBool("hasImploded", !isAlive);
        audioSource.PlayOneShot(implosionSFX);

        yield return new WaitForSeconds(0.35f);
        float dist = Vector3.Distance(transform.position, targetPos);
        
        while (dist > 0.5f)
        {
            //print(dist + "\n");
            transform.position = Vector3.MoveTowards(transform.position, transform.position - new Vector3(0.0f, 10.0f, 0.0f), 0.07f);//speed * Time.deltaTime);
            dist = Vector3.Distance(transform.position, targetPos);
            yield return new WaitForSeconds(0.1f);
        }

        print("game over\n");
        SceneManager.LoadScene("GameOver");
    }

    IEnumerator TriggerExplosion()
    {
        yield return new WaitForSeconds(0.5f);
        isAlive = false;
        animator.SetBool("hasExploded", !isAlive);
        yield return new WaitForSeconds(2.5f);
        //print("game over\n");
        SceneManager.LoadScene("GameOver");
    }

    void UpdateOxygen() {
        if (transform.position.y != subPos.y && !isPaused && !isSurfacing)
        {
            Update_oxygen();
        }  
    }

    /*IEnumerator UpdateOxygen()
    {
        yield return new WaitForSeconds(3.0f);

        while (true)
        {
            if (transform.position.y != subPos.y) { Update_oxygen(); }
            yield return new WaitForSeconds(3.0f);
        }
    }*/

    void LoadTorpedo(int n) {
        bool tubeStatus = Check_torpedotube_n(n); //true = the tube is currently loaded

        Load_torpedotube_n(n);

        bool output = Check_torpedotube_n(n);
        
        if (tubeStatus) //There is already a torpedo in there so cannot load another
        {
            print("Tube " + n + " is already loaded. Cannot add another torpedo.\n");//TODO - does this circumstance indicate an error in the coursework? i.e. should I remove this?
        }
        else if (output) //there is a torpedo
        {
            audioSource.PlayOneShot(reloadSFX);
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

        curSub = Get_sub_stats();

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


        //TODO what are the downsides to updating this project to c# 7? Means I can use a ternary operator for the above...
        //bool Get_innerairlock_pos() ? InDoorPos.GetComponent<SpriteRenderer>().sprite = DoorClosed : InDoorPos.GetComponent<SpriteRenderer>().sprite = DoorOpen; 

        DepthValue.text = curSub.Depth.ToString();
        TempValue.text = curSub.Temp.ToString();
        if (curSub.Temp < 90)
        {
            TempValue.color = Color.white;
        }
        else
        {
            TempValue.color = Color.red;
        }
        OxygenValue.text = curSub.Oxygen.ToString();
        if (curSub.Oxygen > 10) {
            OxygenValue.color = Color.white;
        }
        else
        {
            OxygenValue.color = Color.red;
        }
        FrontSpaceValue.text = curSub.FrontSpace.ToString();
    }

}