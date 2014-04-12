using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClickToMove : MonoBehaviour
{
		public int maxmove;
		public Color original;
		public Color selected;
		// Use this for initialization
		void Start ()
		{
			renderer.material.color = original;
		}
	
		// Update is called once per frame
		void Update ()
		{
		
		}
		
		public void changeColor(bool isActive)
		{
			if (isActive){
				renderer.material.color = selected;
			}
			else
			{
				renderer.material.color = original;
			}
		}
		
		public int getMaxMove()
		{
			return maxmove;
		}
}
