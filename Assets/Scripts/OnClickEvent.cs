using UnityEngine;
using System.Collections;

public class OnClickEvent : MonoBehaviour {
	private bool clicked=false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		changeColour ();
	}
	void OnMouseDown () {
		if (clicked)
			clicked = false;
		else
			clicked = true;
	}
	
	void OnMouseEnter () {
		changeColour ();
	}
	
	void changeColour() {		
		if (clicked)
			renderer.material.color = Color.red;
		else
			renderer.material.color = Color.white;
	}
}
