using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Button : MonoBehaviour, ISelectHandler, IDeselectHandler //ISelectHandler/IDeselectHandler required for onSelect/onDeselect
{
    GameObject defaultSelection;

    void Awake()
    {
        defaultSelection = GameObject.Find("Start");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            CatchMouseClicks(defaultSelection);
        }
    }


    public void CatchMouseClicks(GameObject selection)
    {
        EventSystem.current.SetSelectedGameObject(selection);
    }

    public void Restart()
    {
        SceneManager.LoadScene("Start");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void ShowControls()
    {
        SceneManager.LoadScene("Controls");
    }

    public void OnSelect(BaseEventData eventData)
    {
        gameObject.GetComponent<Text>().text = "> " + name + " <";
    }

    public void OnDeselect(BaseEventData eventData)
    {
        gameObject.GetComponent<Text>().text = name;
    }
}
