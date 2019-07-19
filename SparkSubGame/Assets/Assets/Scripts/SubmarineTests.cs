using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEngine.SceneManagement;
//using UnityEditor.Animations;

public class SubmarineTests : MonoBehaviour
{

    #region

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
    //bool hasWarning;
    GameObject WarningBox;
    GameObject WarningText;

    Sprite DoorOpen, DoorClosed, DoorUnlocked, DoorLocked;
    GameObject[] TorpedoTubes;
    GameObject[] Torpedos;
    GameObject InDoorPos, InDoorLock, OutDoorPos, OutDoorLock;

    #endregion

    //TESTING
    //TODO remove once not needed
    bool isEmergency = false;

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
            Torpedos[i] = GameObject.Find("Torpedo" + (i + 1));
        }

        //Pause screen
        PauseScreen = GameObject.Find("PauseScreen");
        PauseScreen.GetComponent<Canvas>().sortingOrder = -1;
        //PauseScreen.SetActive(true);
        isPaused = false;

        //warning message box
        WarningText = GameObject.Find("Message");
        WarningBox = GameObject.Find("Warning");
        //WarningBox.SetActive(false);
        WarningBox.GetComponent<Canvas>().sortingOrder = 0;
        //hasWarning = false;

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
        InvokeRepeating("UpdateOxygen", 3.0f, 3.0f); 
    }

    void ResetSub()
    {
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

    void UpdateDoors()
    {
        curSub.InnerAirlockPos = Get_innerairlock_pos();
        curSub.InnerAirlockLock = Get_innerairlock_lock();
        curSub.OuterAirlockPos = Get_outerairlock_pos();
        curSub.OuterAirlockLock = Get_outerairlock_lock();
    }

    void Update()
    {
        //print("emergency sub stat: " + curSub.EmergencyStatus);

        prevSub = Get_sub_stats();

        if (!Get_innerairlock_pos() || !Get_innerairlock_lock() || !Get_outerairlock_pos() || !Get_outerairlock_lock())
        {
            isReady = false;

        }
        else
        {
            isReady = true;
        }
        animator.SetBool("isReady", isReady);
        animator.SetBool("isAlive", isAlive);

        //check for emergency surface
        if (isEmergency)
        //if (Check_emergency())
        {
            if (curSub.Temp < 100 && curSub.Oxygen > 0)
            {
                //TODO dud noise?
                StartCoroutine(ShowWarning("The emergency surface procedure has been triggered unnecessarily"));
            }

            if (transform.position.y != subPos.y)
            {
                print("pos is: " + transform.position.y + ", should be: " + subPos.y);
                isSurfacing = true;
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, subPos.y), 1.0f * Time.deltaTime);
            }
            else
            {
                Stop_emergency_surface();
                isSurfacing = false;
            }
        }
        else
        {
            if (curSub.Temp >= 100)
            {
                PlayerPrefs.SetString("PlayerDeath", "The sub didn't emergency surface and overheated.");
                StartCoroutine(TriggerExplosion());
            }
            else if (curSub.Oxygen <= 0)
            {
                PlayerPrefs.SetString("PlayerDeath", "The sub didn't emergency surface and ran out of oxygen.");
                StartCoroutine(TriggerExplosion()); //TODO update to correct death animation
            }
        }

        //Do some sub stuff
        if (!isPaused && isAlive && !isSurfacing)
        {

            //move the sub
            if (Input.GetKeyUp(KeyCode.LeftArrow)) //move sub left
            {
                //TESTING
                //Test 1: too far left:
                /*prevSub.FrontSpace = 100;
				curSub.FrontSpace = 100;
                curSub.FrontSpace++;*/

                //Test 2: Everything working
                /*prevSub.FrontSpace = 10;
				curSub.FrontSpace = 10;
                curSub.FrontSpace++;*/

                //ACTUAL FUNCTION
                Sub_go_back();
                curSub = Get_sub_stats();

                if (curSub.FrontSpace > 100)
                {
                    PlayerPrefs.SetString("PlayerDeath", "The sub went beyond the boundaries.");
                    StartCoroutine(TriggerExplosion());
                }
                else if (prevSub.FrontSpace < curSub.FrontSpace) { transform.Translate(-horizontalMovement, 0, 0); }
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow)) //move right
            {
                //TESTING
                //Test 1: too far right:
                /*prevSub.FrontSpace = 0;
                curSub.FrontSpace = 0;
                curSub.FrontSpace--;
                print(curSub.FrontSpace);*/

                //Test 2: Everything working
                /*prevSub.FrontSpace = 10;
                curSub.FrontSpace = 10;
                curSub.FrontSpace--;
                print(curSub.FrontSpace);*/

                //ACTUAL FUNCTION
                Sub_go_forward();
                curSub = Get_sub_stats();

                //FrontSpace has changed from 0 but not decreased - this should indicate it has incorrectly moved further right
                //which should result in a value of 255 since it's stored in a byte
                //But want to account for any possible incorrect value
                if (prevSub.FrontSpace == 0 && curSub.FrontSpace != prevSub.FrontSpace && curSub.FrontSpace != 1)
                {
                    PlayerPrefs.SetString("PlayerDeath", "The sub went beyond the boundaries.");
                    StartCoroutine(TriggerExplosion());
                }
                else if (prevSub.FrontSpace > curSub.FrontSpace) { transform.Translate(horizontalMovement, 0, 0); }
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow)) //dive
            {
                //TESTING
                //Test 1: too deep:
                /*curSub.Depth = 100;
                curSub.Depth++;*/

                //Test 2: Everything working
                /*prevSub.Depth = 10;
				curSub.Depth = 10;
                curSub.Depth++;*/

                //ACTUAL FUNCTION
                Sub_dive();
                curSub = Get_sub_stats();

                if (curSub.Depth > 100)
                {
                    transform.Translate(0, -verticalMovement, 0);
                    PlayerPrefs.SetString("PlayerDeath", "The sub went too deep.");
                    StartCoroutine(TriggerImplosion());
                }
                else if (prevSub.Depth < curSub.Depth) { transform.Translate(0, -verticalMovement, 0); }
            }
            else if (Input.GetKeyUp(KeyCode.UpArrow)) //surface
            {
                //TESTING
                //Test 1: too high:
                /*prevSub.Depth = 0;
                curSub.Depth = 0;
                curSub.Depth--;
                print(curSub.Depth);*/

                //Test 2: Everything working
                /*prevSub.Depth = 1;
                curSub.Depth = 1;
                curSub.Depth--;
                print(curSub.Depth);*/

                //ACTUAL FUNCTION
                Sub_surface();
                curSub = Get_sub_stats();

                //Depth has changed from 0 but not decreased - this should indicate it has incorrectly surfaced further
                //which should result in a value of 255 since it's stored in a byte
                //But want to account for any possible incorrect value
                if (prevSub.Depth == 0 && curSub.Depth != prevSub.Depth && curSub.Depth != 1)
                {
                    PlayerPrefs.SetString("PlayerDeath", "The sub left the water.");
                    StartCoroutine(TriggerExplosion()); //TODO need something different - like the sub flying away
                }
                else if (prevSub.Depth > curSub.Depth) { transform.Translate(0, verticalMovement, 0); }
            }

            //Reset the sub
            if (Input.GetKeyUp(KeyCode.Keypad0))
            {
                //print("Resetting...\n");
                ResetSub();
            }

            //Use the doors
            if (Input.GetKeyUp(KeyCode.I))
            {
                //TESTING
                //Test 1: opening a second door
                /*curSub.InnerAirlockPos = false;
                curSub.OuterAirlockPos = false;*/

                //Test 2: not toggling inner door when unlocked
                /*curSub.InnerAirlockLock = false;
                prevSub.InnerAirlockPos = true;
                curSub.InnerAirlockPos = true;*/

                //Test 3: Toggling inner door when locked
                /*curSub.InnerAirlockLock = true;
                curSub.InnerAirlockPos = true;
                prevSub.InnerAirlockPos = false;*/

                //ACTUAL FUNCTION
                //print("toggle inner door pos\n");
                Close_inner_door();
                curSub = Get_sub_stats();
                UpdateDoors();

                //if new door pos = open AND other door pos = open -> death
                if (curSub.InnerAirlockPos == false && curSub.OuterAirlockPos == false)
                {
                    audioSource.PlayOneShot(doorSFX);
                    PlayerPrefs.SetString("PlayerDeath", "Both doors have been opened, flooding the sub.");
                    StartCoroutine(TriggerImplosion());
                }
                //if prev door pos == new door pos AND the door is unlocked, toggle didn't work
                else if (curSub.InnerAirlockPos == prevSub.InnerAirlockPos && curSub.InnerAirlockLock == false)
                {
                    //TODO dud noise
                    StartCoroutine(ShowWarning("The inner door failed to open/close."));
                }
                //if prev door pos != new door pos BUT door = locked
                else if (curSub.InnerAirlockPos != prevSub.InnerAirlockPos && curSub.InnerAirlockLock == true)
                {
                    StartCoroutine(ShowWarning("The inner door lock appears to be broken."));
                }
                //Everything is fine
                else if (curSub.InnerAirlockPos != prevSub.InnerAirlockPos && curSub.InnerAirlockLock == false)
                {
                    audioSource.PlayOneShot(doorSFX);
                }
                //something other than the expected scenarios has happened - will need to be investigated
                else
                {
                    StartCoroutine(ShowWarning("Something unexpected has happened..."));
                }
            }
            if (Input.GetKeyUp(KeyCode.K))
            {
                //TESTING
                //Test 1: lock doesn't change
                /*curSub.InnerAirlockLock = false;
                prevSub.InnerAirlockLock = false;*/

                //ACTUAL FUNCTION
                //print("toggle inner door lock\n");
                Lock_inner_door();
                curSub = Get_sub_stats();
                UpdateDoors();

                //The lock hasn't changed. 
                //Can't differentiate between deliberate or erroneous so using a generic warning
                if (curSub.InnerAirlockLock == prevSub.InnerAirlockLock)
                {
                    StartCoroutine(ShowWarning("The inner lock hasn't changed"));
                    //TODO play dud sound
                }
                //everything is fine
                else if (curSub.InnerAirlockLock != prevSub.InnerAirlockLock)
                {
                    audioSource.PlayOneShot(doorSFX);
                }
                //something other than the expected scenarios has happened - will need to be investigated
                else
                {
                    StartCoroutine(ShowWarning("Something unexpected has happened..."));
                }
            }
            if (Input.GetKeyUp(KeyCode.O))
            {
                //TESTING
                //Test 1: opening a second door
                /*curSub.InnerAirlockPos = false;
                curSub.OuterAirlockPos = false;*/

                //Test 2: not toggling outer door when unlocked
                /*curSub.OuterAirlockLock = false;
                prevSub.OuterAirlockPos = true;
                curSub.OuterAirlockPos = true;*/

                //Test 3: Toggling outer door when locked
                /*curSub.OuterAirlockLock = true;
                curSub.OuterAirlockPos = true;
                prevSub.OuterAirlockPos = false;*/

                //ACTUAL FUNCTION
                //print("toggle outer door pos\n");
                Close_outer_door();
                curSub = Get_sub_stats();
                UpdateDoors();

                //if new door pos = open AND other door pos = open - death
                if (curSub.OuterAirlockPos == false && curSub.OuterAirlockPos == false)
                {
                    audioSource.PlayOneShot(doorSFX);
                    PlayerPrefs.SetString("PlayerDeath", "Both doors have been opened, flooding the sub.");
                    StartCoroutine(TriggerImplosion());
                }
                //if prev door pos == new door pos AND the door is unlocked, toggle didn't work - warning  && dud noise
                else if (curSub.OuterAirlockPos == prevSub.OuterAirlockPos && curSub.OuterAirlockLock == false)
                {
                    StartCoroutine(ShowWarning("The outer door position hasn't changed"));
                }
                //if prev door pos != new door pos BUT door = locked - warning and dud noise
                else if (curSub.OuterAirlockPos != prevSub.OuterAirlockPos && curSub.OuterAirlockLock == true)
                {
                    StartCoroutine(ShowWarning("The outer door hasn't changed"));
                }
                //Everything is fine
                else if (curSub.OuterAirlockPos != prevSub.OuterAirlockPos && curSub.OuterAirlockLock == false)
                {
                    audioSource.PlayOneShot(doorSFX);
                }
                //something other than the expected scenarios has happened - will need to be investigated
                else
                {
                    StartCoroutine(ShowWarning("Something unexpected has happened..."));
                }
            }
            if (Input.GetKeyUp(KeyCode.L))
            {
                //TESTING
                //Test 1: lock doesn't change
                /*curSub.OuterAirlockLock = false;
                prevSub.OuterAirlockLock = false;*/

                //ACTUAL FUNCTION
                //print("toggle outer door lock\n");
                Lock_outer_door();
                curSub = Get_sub_stats();
                UpdateDoors();

                //The lock hasn't changed. 
                //Can't differentiate between deliberate or erroneous so using a generic warning
                if (curSub.OuterAirlockLock == prevSub.OuterAirlockLock)
                {
                    StartCoroutine(ShowWarning("The outer lock hasn't changed"));
                    //TODO play dud sound
                    //audioSource.PlayOneShot(DudSFX);
                }
                //everything is fine
                else if (curSub.OuterAirlockLock != prevSub.OuterAirlockLock)
                {
                    audioSource.PlayOneShot(doorSFX);
                }
                //something other than the expected scenarios has happened - will need to be investigated
                else
                {
                    StartCoroutine(ShowWarning("Something unexpected has happened..."));
                }
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
        if (Input.GetKeyUp(KeyCode.T))
        {
            //Vector3 targetPos = transform.position - new Vector3(0.0f, 2.5f, 0.0f);
            //StartCoroutine(TriggerImplosion(targetPos));
            //StartCoroutine(TriggerImplosion());

            //StartCoroutine(TriggerExplosion());

            //isSurfacing = true;




            //Test: min oxy, no emergency
            //Just needed to keep the oxy counter running and ensure emergency check remained false

            //Test: max temp, no emergency
            curSub.Temp = 100;
            //And comment out the curSub update in UpdateUI

            /*// Test: unneeded emergency
            curSub.Temp = 50;
            curSub.Oxygen = 50;
            isEmergency = true;*/
        }

        //pause/unpause
        if (Input.GetKeyUp(KeyCode.P) && !isSurfacing)
        {
            if (isPaused)
            {
                //if already paused then unpause
                isPaused = false;
                //PauseScreen.SetActive(false);
                PauseScreen.GetComponent<Canvas>().sortingOrder = -1;
            }
            else
            {
                //pause the screen
                isPaused = true;
                //PauseScreen.SetActive(true);
                PauseScreen.GetComponent<Canvas>().sortingOrder = 15;
            }
        }

        UpdateUI();
    }

    IEnumerator ShowWarning(string message)
    {
        WarningText.GetComponent<Text>().text = message;
        WarningBox.GetComponent<Canvas>().sortingOrder = 1;
        yield return new WaitForSeconds(4.0f);
        WarningBox.GetComponent<Canvas>().sortingOrder = 0;
    }

    void FireTorpedo(int n)
    {
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
            //print("Tube " + n + " is empty. Please reload\n"); //TODO play dud noise
        }
        else if (output) //there's still a torpedo there
        {
            print("Torpedo was not fired. \nTube " + n + ": Loaded\n"); //TODO warning message - Fire did not work
        }
        else if (!output && tubeLoaded && curSub.FrontSpace <= safeDist) //the torpedo has fired but the front space was too small so the sub has exploded
        //if (!output && tubeLoaded && curSub.FrontSpace <= 95) //for testing
        {
            wasFired = true;
            PlayerPrefs.SetString("PlayerDeath", "A torpedo exploded too close to the sub.");
            StartCoroutine(TriggerExplosion());
        }

        if (wasFired)
        {
            Instantiate(Resources.Load("TorpedoPrefab"), transform.GetChild(0).transform.position, Quaternion.identity);
            wasFired = false;
        }
    }

    void LoadTorpedo(int n)
    {
        bool tubeStatus = Check_torpedotube_n(n); //true = the tube is currently loaded

        Load_torpedotube_n(n);

        bool output = Check_torpedotube_n(n);

        if (tubeStatus) //There is already a torpedo in there so cannot load another
        {
            //print("Tube " + n + " is already loaded. Cannot add another torpedo.\n"); //TODO play dud noise
        }
        else if (output) //there is a torpedo
        {
            audioSource.PlayOneShot(reloadSFX);
            print("Tube " + n + " is Loaded\n");
            TorpedoTubes[n - 1].SetActive(true);
            Torpedos[curTorpedo - 1].SetActive(false);
            curTorpedo--;
        }
        else if (!output) //there is no torpedo
        {
            print("Something went wrong. Tube " + n + " is Empty \n"); //TODO play dud noise
        }
    }

    IEnumerator TriggerImplosion()
    {
        Vector3 targetPos = transform.position - new Vector3(0.0f, 2.5f, 0.0f);
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

    void UpdateOxygen()
    {
        if (transform.position.y != subPos.y && !isPaused && !isSurfacing)
        {
            Update_oxygen();
        }
    }

    void UpdateUI()
    {
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
        if (curSub.Oxygen > 10)
        {
            OxygenValue.color = Color.white;
        }
        else
        {
            OxygenValue.color = Color.red;
        }
        FrontSpaceValue.text = curSub.FrontSpace.ToString();
    }

}