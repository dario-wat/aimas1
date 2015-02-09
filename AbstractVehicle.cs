using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public abstract class AbstractVehicle : MonoBehaviour {

	/** Definitions of all variables that are used for all vehicle models. **/

	// Z axis if forward-back, X axis is left-right

	// Starting coordinates
	public Vector3 start = new Vector3(0.0f, 0.0f, 0.0f);

	// Size of the cube (vehicle)
	public Vector3 size = new Vector3(1.0f, 0.5f, 2.0f);

	// Initial rotation of the vehicle
	public Vector3 rotation = new Vector3(0.0f, 0.0f, 0.0f);

	// Name of the file with obstacles
	public string obstacleFilename;

	// Material used for obstacles
	public Material material;


	/** Next up, a few private/protected variables to manage abstraction. **/

	// GUI style to use for labels
	private GUIStyle labelStyle;

	// Rectangle to use for labels
	private Rect labelRect;

	// String to print in label
	private string strCost;

	// Time taken to reach the goal
	private float totalTime = -1.0f;

	// List of polygonal obstacles
	protected List<Polygon> polys;

	// Start and goal positions
	protected Vector2 startPos;
	protected Vector2 goalPos;

	// List of moves that car has to perform
	protected abstract Queue<Move> moves { get; set; }

	// Cost of the path
	protected abstract float cost { get; set; }


	// Use this for initialization
	void Start() {
		transform.position = start;			// Sets the starting coordinates
		transform.localScale = size;		// Sets the size of the vehicle
		transform.eulerAngles = rotation;	// Sets initial rotation

		// Read file and create polygons
		ReadFile("Assets/_Data/" + obstacleFilename);
		transform.position = new Vector3(startPos.x, 0.0f, startPos.y);

		// Generate and render all obstacles
		GameObject parent = new GameObject();
		parent.name = "Polygonal Obstacles";
		foreach (Polygon pol in polys) {
			GameObject go = pol.ToGameObject(material);
			go.transform.parent = parent.transform;
		}

		// Whatever subclass has to do
		LocalStart();

		// Just to initialize and set color
		labelStyle = new GUIStyle();
		labelStyle.normal.textColor = Color.black;
		labelRect = new Rect(20, 20, 20, 20);
		strCost = "Distance/Cost/Time: " + cost.ToString("0.00");
	}
	
	// Update is called once per frame
	void Update () {
		float dt = Time.deltaTime;
		if (moves.Count > 0) {
			while (true) {
				Move move = moves.Dequeue();
				dt = move.MoveMe(transform, dt);
				if (dt == 0.0f) {
					moves.Enqueue(move);
					break;
				}
			}
		} else if (totalTime < 0.0f) {
			totalTime = Time.realtimeSinceStartup;
		}
	}

	// Prints on screen labels for cost and time
	void OnGUI() {
		string toLab = strCost
			+ "\nTime: " + Time.realtimeSinceStartup.ToString("0.00") + " s";
		if (totalTime > 0.0f) {
			toLab += "\nBest: " + totalTime.ToString("0.00") + " s";
		}
		GUI.Label(labelRect, toLab,	labelStyle);
	}

	// Whatever subclass has to initialize, this is the functin to do so
	protected abstract void LocalStart();


	// Reads file and sets list of polygons
	// Not a very smart function, just a lot of work to do
	private void ReadFile(string filename) {
		StreamReader sr = new StreamReader(filename);
		try {
			// Read start
			string[] sxy = sr.ReadLine().Split(' ');
			startPos = new Vector2(float.Parse(sxy[0]), float.Parse(sxy[1]));

			// Read goal
			string[] gxy = sr.ReadLine().Split(' ');
			goalPos = new Vector2(float.Parse(gxy[0]), float.Parse(gxy[1]));

			// Read number of vertices
			int count = int.Parse(sr.ReadLine());
			int[] button = new int[count];
			Vector2[] vertices = new Vector2[count];
			
			// Read vertices and buttons
			for (int i = 0; i < count; i++) {
				string[] line = sr.ReadLine().Split(' ');
				vertices[i] = new Vector2(
					float.Parse(line[0]), float.Parse(line[1]));
				button[i] = int.Parse(line[2]);
			}

			// Initialize polygon collection
			polys = new List<Polygon>();
			List<Vector2> buffer = new List<Vector2>();
			for (int i = 0; i < count; i++) {
				buffer.Add(vertices[i]);
				if (button[i] == 3) {
					Polygon newPol = new Polygon(buffer);
					polys.Add(newPol);
					buffer.Clear();
				}
			}

		} finally {
			sr.Close();		// Close stream
		}
	}
}
