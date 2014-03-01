using UnityEngine;
using System.Collections;

public class OnMouseEvent: MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	 
	}

	void OnMouseEnter () {
		renderer.material.color = Color.red;
	}

	void OnMouseExit () {
		renderer.material.color = Color.white;
	}
}
