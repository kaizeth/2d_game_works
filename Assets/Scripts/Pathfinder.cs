using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class Pathfinder : MonoBehaviour
{ 	
		// Constants/configuration - change here as needed! -------------------------
		//public static int[][][] openList;
		//public static int[][] closedList;
		public static int moveWeight = 10; // as G in F = G + H
		static int[][] walkable;
		static Square current, target;
		static List<Square>  openLists = new List<Square> ();
		public static List<Square>  closedLists = new List<Square> ();
	
		
		
		// Use this for initialization
		void Start ()
		{

		}
	
		// Update is called once per frame
		void Update ()
		{
				
		}

		public static void test (float x)
		{
				//Test for function call
				Debug.Log ("Value of X is " + x); 
		}
		
		public static void findPath (int[][] map, Vector3 start, Vector3 end)
		{	//Step 0: Clear existing static lists
				openLists.Clear ();
				closedLists.Clear ();
			
				//Step 1: set start as current, prepare openLists and closedLists
				current = new Square ((int)start.x, (int)start.y);
				target = new Square ((int)end.x, (int)end.y);
				walkable = map;
				closedLists.Add (current); // Add current into closed list.
				int loopCount = 0;
	
				//Step 1.1: Loop around current, add into openLists.
				for (int i=0; i<3; i++) {					
						for (int j=0; j<3; j++) {
								loopCount += 1;
								if (loopCount % 2 == 1) //choose to NOT skip diagonal, but double moveWeight if hit diagonal. Initial call skip center.
										continue;
							
								if (loopCount == 5)
										continue;
								//Debug.Log ("Floor " + ((current.getX () - 1 + i) + "," + (current.getY () - 1 + j) + " is " + walkable [(current.getY () - 1 + j)] [(current.getX () - 1 + i)]));
					
								if (walkable [(current.getY () - 1 + i)] [(current.getX () - 1 + j)] == 0) // Check if cell is walkable
										continue;
			
								//Debug.Log ("G of " + (current.getX () - 1 + i) + "," + (current.getY () - 1 + j) + " is " + (current.getG () + moveWeight*walkable [(current.getY () - 1 + j)] [(current.getX () - 1 + i)]));					
	
								//Step 1.2: Calculate heuristics before add into openLists
								int heuristics = 10 * (int)(Mathf.Abs (current.getX () - 1 + j - target.getX ()) + Mathf.Abs (current.getY () - 1 + i - target.getY ()));
			
								//Step 1.3: Add squares into openLists
								openLists.Add (new Square (
									(int)start.x - 1 + j,
									(int)start.y - 1 + i,
									(current.getG () + moveWeight * walkable [(current.getY () - 1 + i)] [(current.getX () - 1 + j)]),
									heuristics,
									current));
						}
				}
				
				// Loop until end is reached.
				int counting = 0;
				while (current.getX () != target.getX () || current.getY ()!=target.getY ()) {
						if (counting == 250)
								break;
						// Step 2: Loop through open list to get next current
//Debug.Log ("____________________               CalculateLists called!!!!!");
						calculateLists ();
						//Step 3: Using next current, add neighbours to open lists
						growOpenLists ();
						counting++;
				}
				Debug.Log ("openLists size is " + openLists.Count);
				return;
		}
		
		public static void calculateLists ()
		{
				int bestScore = 99999;
				int bestIndex = -1;
				int score = 0;
				for (int i=0; i<openLists.Count; i++) {
						score = openLists [i].getG () + openLists [i].getH ();
//Debug.Log ("Score of " + openLists [i].getX () + "," + openLists [i].getY () + " is " + score + ". Parent is " + openLists [i].getParent ().getX () + "," + openLists [i].getParent ().getY ());
						if (score < bestScore) {
								bestScore = score;
								bestIndex = i;
						}					
				}
//Debug.Log ("openLists size is " + openLists.Count);
			
				// Step 2.1: After finding best score, add to closedLists, use as current and remove from openLists
				closedLists.Add (openLists [bestIndex]);
				current = openLists [bestIndex];
				openLists.RemoveAt (bestIndex); 			
			
				
		}
		
		public static void growOpenLists ()
		{
				int loopCount = 0;
				bool inClosed = false;
				bool inOpen=false;
				//Step 1.1: Loop around current, add into openLists.
				for (int i=0; i<3; i++) {					
						for (int j=0; j<3; j++) {
								loopCount += 1;
								if (loopCount % 2 == 1)
										continue; //skipping diagonal movement
									
								// Check if cell is walkable
								if (walkable [(current.getY () - 1 + i)] [(current.getX () - 1 + j)] == 0) 
										continue;
									
								//Debug.Log ("Floor " + ((current.getX () - 1 + j) + "," + (current.getY () - 1 + i) + " is " + walkable [(current.getY () - 1 + i)] [(current.getX () - 1 + j)]));
									
								//Step 1.2: Calculate heuristics before add into openLists
								int heuristics = 10 * (int)(Mathf.Abs (current.getX () - 1 + j - target.getX ()) + Mathf.Abs (current.getY () - 1 + i - target.getY ()));
		
								int breakloop=0;	
								// Check if cell already in closed lists
								inClosed = false;
								
								for (int k=0; k<closedLists.Count; k++) {

										if (breakloop >= 250){
											Debug.Log ("Force break at k. closedLists.count:" + closedLists.Count);
											break;
										}
										else {
													breakloop++;
										}

										//if in closedLists, stop check and move to next coordinate.
										if (closedLists [k].getX () == (current.getX () - 1 + j) && closedLists [k].getY () == (current.getY () - 1 + i)) 
													inClosed=true;
													break;
													
								}
								
								if (inClosed)
										continue;
								
								inOpen = false;
								//if not in closedLists, check if its in openLists
								int breakloop2=0;
								for (int c=0; c<openLists.Count; c++) {
										if (breakloop2 >= 250){
											Debug.Log ("Force break at c. openLists.count:" + openLists.Count);
											break;
										}										
										else {
												breakloop2++;
										}
										
										// If in openlist, check if G in openLists is larger than G of current coordinate.
										if (openLists [c].getG () > current.getG () + moveWeight * walkable [(current.getY () - 1 + i)] [(current.getX () - 1 + j)]){
												openLists [c].setParent (new Square (// If true, then replace parent of current square in closed list with this square
							                       current.getX () - 1 + j,
							                       current.getY () - 1 + i,
							                       (current.getG () + moveWeight * walkable [(current.getY () - 1 + i)] [(current.getX () - 1 + j)]),
							                       heuristics,
							                       current));	
												inOpen=true;
												break;
										}
										//Debug.Log ("G of " + (current.getX () - 1 + i) + "," + (current.getY () - 1 + j) + " is " + (current.getG () + moveWeight*walkable [(current.getY () - 1 + j)] [(current.getX () - 1 + i)]));					
										
								}
								
								if (inOpen)
									continue;
									
								//If not in openLists, add it in!
								openLists.Add (new Square (
									current.getX () - 1 + j,
									current.getY () - 1 + i,
									(current.getG () + moveWeight * walkable [(current.getY () - 1 + j)] [(current.getX () - 1 + i)]),
									heuristics,
									current));	
						}
				}
		}		


		//Reads whole file and puts them into 2D String Array for use
		public static string[][] readFile (string file)
		{
				string text = System.IO.File.ReadAllText (file);
				string[] lines = Regex.Split (text, "\n");
				int rows = lines.Length;
		
				string[][] levelBase = new string[rows][];
				for (int i = 0; i < lines.Length; i++) {
						string[] stringsOfLine = Regex.Split (lines [i], " ");
						levelBase [lines.Length - 1 - i] = stringsOfLine;
				}
				return levelBase;
		}

		//Class constructor
		public class Square
		{
				int x, y;
				int g = 0;
				int h = 0;
				Square parent;
			
				public Square ()
				{
						x = 0;
						y = 0;
						g = 0;
						h = 0;	
				}

				public Square (int x, int y)
				{
						this.x = x;
						this.y = y;
				}

				public Square (int x, int y, int g, int h)
				{
						this.x = x;
						this.y = y;
						this.g = g;
						this.h = h;
				}
		
				public Square (int x, int y, int g, int h, Square parent)
				{
						this.x = x;
						this.y = y;
						this.g = g;
						this.h = h;
						this.parent = parent;
				}
			
				public void setParent (Square parent)
				{
						this.parent = parent;
				}
				
				public int getX ()
				{
						return x;
				}
				
				public int getY ()
				{
						return y;
				}

				public int getG ()
				{
						return g;
				}
				
				public int getH ()
				{
						return h;
				}
		
				public Square getParent ()
				{
						return parent;
				}
		}
}
