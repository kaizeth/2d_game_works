using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ClickToMove : MonoBehaviour
{
		public int maxmove;
		//public int[] moveset;
		public float clickX, clickY;
		public Vector3 newLocation;
		public float nextFire, movePoint;
		public float moveSpeed=0.25f;
		private static string filepath;
		public static int[][] floorWeight; 
		List<Pathfinder.Square>  MoveList = new List<Pathfinder.Square> ();
		bool isMoving;

		// Use this for initialization
		void Start ()
		{
				//maxmove = 4;
				FloorSet ();
		}
	
		// Update is called once per frame
		void Update ()
		{
				//Debug.Log ("floorWeight in Update " + floorWeight.Length + " , " + floorWeight [0].Length);
				if (Input.GetButton ("Fire1") && Time.time > nextFire) { // Nextfire intentionally to prevent multiple calls in one click
						nextFire = Time.time + 1;
	
						if (isMoving){
							Debug.Log("Wait until move finishes.");
							return;
						}
			
						newLocation = Camera.main.ScreenToWorldPoint (Input.mousePosition);
						newLocation.z = 0;
						newLocation.x = Mathf.Round (newLocation.x);
						newLocation.y = Mathf.Round (newLocation.y);
						//Debug.Log (floorWeight [(int)newLocation.y] [(int)newLocation.x]);
						Debug.Log("You are at " + transform.position.x + "," + transform.position.y);
						Debug.Log ("You clicked on " + newLocation.x + "," + newLocation.y);
						
						if (floorWeight[(int)newLocation.y][(int)newLocation.x]!=0){
							Pathfinder.findPath (floorWeight, transform.position, newLocation);
							moveToIt();
						} else {
							Debug.Log("This spot is unreachable");
						}
						
				}
					
		}

		static void FloorSet ()
		{	
				filepath = Application.dataPath + "/Floor/floor.txt";
				string[][] jagged = Pathfinder.readFile (filepath);
		
				Debug.Log ("Jagged " + jagged.Length + " , " + jagged [0].Length);
				floorWeight = new int[jagged.Length][];
				// convert floor types to weight
				for (int y = 0; y < jagged.Length; y++) {
						floorWeight [y] = new int[jagged [0].Length];
			
						for (int x = 0; x < jagged[0].Length; x++) {
				
								switch (jagged [y] [x]) {
								case "0": //Grass - refer to GenerateFloor.cs
										floorWeight [y] [x] = 1;
										break;
								case "1": //Earth - refer to GenerateFloor.cs
										floorWeight [y] [x] = 0;
										break;
								case "2": //Rock - refer to GenerateFloor.cs
										floorWeight [y] [x] = 0;
										break;
								default://Incase when unrecognize value
										floorWeight [y] [x] = 0; 
										break;
								}
				
						}
				}
				Debug.Log ("floorWeight " + floorWeight.Length + " , " + floorWeight [0].Length);
				return;
		}
		
		
		void moveToIt()
		{
				Pathfinder.Square testing ;
				Debug.Log ("Length of closedLists is " + Pathfinder.closedLists.Count);
				testing = Pathfinder.closedLists[Pathfinder.closedLists.Count - 1];
				Debug.Log("End goal is " + testing.getX () + " ," + testing.getY () + ". G is " + testing.getG () + ". H is " + testing.getH ());
				
				Pathfinder.Square theParent;
				MoveList.Clear();
				int i=0;
				theParent=testing;
				do{
					MoveList.Add (theParent);		
					theParent = theParent.getParent();
				} while(theParent.getG ()!=0);
				
				isMoving = true;
				StartCoroutine("movePath");	
				
				return;
		}
		
		IEnumerator movePath (){
				do {
					for (int i=(MoveList.Count-1);i>=0;i--){
						Debug.Log ("Walk to " + MoveList[i].getX () + " ," + MoveList[i].getY ());
						yield return StartCoroutine(movingObj(new Vector3 (MoveList[i].getX (),MoveList[i].getY (),0)));	
					}
				} while (transform.position != newLocation && isMoving);
				isMoving = false;
				transform.position=newLocation;
				Debug.Log("Reached End");
		}
		
		IEnumerator movingObj (Vector3 target){
				
				while (transform.position != target){
					transform.position = Vector3.Lerp(transform.position, target, moveSpeed); // move
				yield return 0  ;
				}
		}
}
