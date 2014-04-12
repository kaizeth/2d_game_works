using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectAction : MonoBehaviour {
float nextFire;
bool selected=false;
public GameObject hold;
public Vector3 newLocation;
public float moveSpeed=0.25f;
private static string filepath;
public static int[][] floorWeight; 
List<Pathfinder.Square>  MoveList = new List<Pathfinder.Square> ();
bool isMoving;
	// Use this for initialization
	void Start () {
		FloorSet ();	
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetButton ("Fire1") && Time.time > nextFire) { // Nextfire intentionally to prevent multiple calls in one click
				nextFire = Time.time + 0.5f;
					
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition); 
			RaycastHit hit;
			
			if (isMoving){
				Debug.Log("Wait until move finishes.");
				return;
			}		
			
			if(Physics.Raycast(ray,out hit,1000f)) //From camera, pick the frontmost object shown on screen, output object into hit
			{	//using contitionals to determine what to do with this object we just hit with our click
				if(hit.transform.tag=="Units" && selected==false){
					hold = hit.transform.gameObject;
					selected=true;
					ClickToMove ctm = (ClickToMove)hold.GetComponent("ClickToMove");
					ctm.changeColor(selected);
					Pathfinder.highlightMoveable(floorWeight, hold.transform.position, ctm.getMaxMove());
				}
				else if (hit.transform.tag=="Units" && selected==true){ //action should change, part of next stage
					selected=false;
					ClickToMove ctm = (ClickToMove)hold.GetComponent("ClickToMove");
					ctm.changeColor(selected);
					hold = null;
					Pathfinder.colourMoveable(selected);
				}
				else if (hit.transform.tag=="Floor" && selected==true && hit.transform.gameObject.renderer.material.color == Color.cyan){
					Pathfinder.colourMoveable(false);
					if (isMoving){
						Debug.Log("Wait until move finishes.");
						return;
					}
		
					newLocation = Camera.main.ScreenToWorldPoint (Input.mousePosition);
					newLocation.z = 0;
					newLocation.x = Mathf.Round (newLocation.x);
					newLocation.y = Mathf.Round (newLocation.y);
					//Debug.Log (floorWeight [(int)newLocation.y] [(int)newLocation.x]);
					Debug.Log("You are at " + hold.transform.position.x + "," + hold.transform.position.y);
					Debug.Log ("You clicked on " + newLocation.x + "," + newLocation.y);
					
					if (floorWeight[(int)newLocation.y][(int)newLocation.x]!=0){
						Pathfinder.findPath (floorWeight, hold.transform.position, newLocation);
						moveToIt();
					} else {
						Debug.Log("This spot is unreachable");
					}
				}
				else {
					selected=false;
					ClickToMove ctm = (ClickToMove)hold.GetComponent("ClickToMove");
					ctm.changeColor(selected);
					hold = null;
					Pathfinder.colourMoveable(selected);
				}
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
		} while (hold.transform.position != newLocation && isMoving);
		isMoving = false;
		selected=false;
		ClickToMove ctm = (ClickToMove)hold.GetComponent("ClickToMove");
		ctm.changeColor(selected);
		hold.transform.position=newLocation;
		Debug.Log("Reached End");
	}
	
	IEnumerator movingObj (Vector3 target){
		
		while (hold.transform.position != target){
			hold.transform.position = Vector3.Lerp(hold.transform.position, target, moveSpeed); // move
			yield return 0  ;
		}
	}
}
