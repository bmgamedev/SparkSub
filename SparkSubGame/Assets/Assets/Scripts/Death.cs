using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Death : MonoBehaviour {

	// Use this for initialization
	void Start () {
        string death = PlayerPrefs.GetString("PlayerDeath");
        gameObject.GetComponent<Text>().text = death;
        PlayerPrefs.SetString("PlayerDeath", "");

    }
}
