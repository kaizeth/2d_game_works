using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public class GenerateFloor : MonoBehaviour {
	public Transform origin;
	public Transform grass;
	public Transform earth;
	public Transform rock;
	
	public const string sgrass = "0";
	public const string searth = "1";
	public const string srock = "2";
	//public const string sstart = "S"; //not used now
	private string filepath;
	// Use this for initialization
	void Start () {
		filepath = Application.dataPath + "/Floor/floor.txt";
		string[][] jagged = readFile(filepath);
		
		// create planes based on matrix
		for (int y = 0; y < jagged.Length; y++) {

			for (int x = 0; x < jagged[0].Length; x++) {

				switch (jagged[y][x]){
				case sgrass:
					Instantiate(grass, new Vector3(origin.position.x + x, origin.position.y - y, origin.position.z), origin.rotation);
					break;
				case searth:
					Instantiate(earth, new Vector3(origin.position.x + x, origin.position.y - y, origin.position.z), origin.rotation);
					break;
				case srock:
					Instantiate(rock, new Vector3(origin.position.x + x, origin.position.y - y, origin.position.z), origin.rotation);
					break;
				}

			}

		}   

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	string[][] readFile(string file){
		string text = System.IO.File.ReadAllText(file);
		string[] lines = Regex.Split(text, "\n");
		int rows = lines.Length;
		
		string[][] levelBase = new string[rows][];
		for (int i = 0; i < lines.Length; i++)  {
			string[] stringsOfLine = Regex.Split(lines[i], " ");
			levelBase[i] = stringsOfLine;
		}
		return levelBase;
	}
}
