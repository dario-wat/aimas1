using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

/**
	Class that holds some static methods for creating graphs. It is
	separated into this class so it does not go in the way of GraphState
	class.

	Functions here are generally very big but have very little
	intelligent code.

	There is one intelligent and that is when visibility graph is created
	in CreateVsibilityGraph function.
*/
public class GraphFactory {

	// List for moves in 4-connect neighborhood
	private static readonly List<Vector2> movesN4 = new List<Vector2>() {
		new Vector2( 1, 0),	new Vector2(-1, 0),
		new Vector2( 0, 1), new Vector2( 0,-1)
	};

	// List for moves in 8-connect neighborhood
	private static readonly List<Vector2> movesN8 = new List<Vector2>() {
		new Vector2( 1, 0), new Vector2(-1, 0),
		new Vector2( 0, 1), new Vector2( 0,-1),
		new Vector2( 1, 1), new Vector2(-1, 1),
		new Vector2(-1,-1), new Vector2( 1,-1)
	};

	// List for moves in 16-connect neighborhood
	private static readonly List<Vector2> movesN16 = new List<Vector2>() {
		new Vector2( 1, 0), new Vector2(-1, 0),
		new Vector2( 0, 1), new Vector2( 0,-1),
		new Vector2( 1, 1), new Vector2(-1, 1),
		new Vector2(-1,-1), new Vector2( 1,-1),
		new Vector2( 2, 1), new Vector2(-2, 1),
		new Vector2( 1, 2), new Vector2( 1,-2),
		new Vector2( 2,-1), new Vector2(-2,-1),
		new Vector2( 2, 1), new Vector2(-2, 1)
	};


	// Construct discrete graph from file
	// It is returning 3 values thus out modifiers
	// Parameter neigh tells which neighborhood to use
	// Parameter obstacles is filled with vectors
	public static void CreateDiscreteFromFile(string filename, int neigh,
		out GraphState g, out IState start, out IState goal,
		List<Vector3> obstacles) {

		StreamReader sr = new StreamReader(filename);
		try {
			// Read start coordinates
			string[] sxy = sr.ReadLine().Split(' ');
			start = new DiscreteState(sxy[0], sxy[1]);

			// Read goal coordinates
			string[] gxy = sr.ReadLine().Split(' ');
			goal = new DiscreteState(gxy[0], gxy[1]);

			// Read dimensions of obstacle matrix
			string[] dim = sr.ReadLine().Split(' ');
			int ydim = int.Parse(dim[0]);
			int xdim = int.Parse(dim[1]);
			
			// Read the obstacle matrix
			int[,] A = new int[ydim, xdim];
			for (int y = 0; y < ydim; y++) {
				string[] bits = sr.ReadLine().Split(' ');
				for (int x = 0; x < xdim; x++) {
					A[y,x] = int.Parse(bits[x]);
				}
			}

			// Create all vertices, it creates full ydim by xdim
			// vertex space even though some vertices are obstacles
			// and have no connections
			List<IState> vertices = new List<IState>();
			for (int y = 0; y < ydim; y++) {
				for (int x = 0; x < xdim; x++) {
					vertices.Add(new DiscreteState(x, y));
					if (A[y,x] == 1) {	// If it is obstacle, add to list
						obstacles.Add(new Vector3(x, 0.0f, y));
					}
				}
			}

			// Picking the right neighborhood list
			List<Vector2> moves = null;
			if (neigh == 4) {
				moves = movesN4;
			} else if (neigh == 8) {
				moves = movesN8;
			} else if (neigh == 16) {
				moves = movesN16;
			} else {
				throw new ArgumentException("Neighborhood does not exist.");
			}

			// Adding edges
			g = new GraphState(vertices);
			foreach (IState vertex in vertices) {
				DiscreteState state = vertex as DiscreteState;
				int x = state.x;
				int y = state.y;
				Vector2 pos = new Vector2(x, y);

				if (A[y,x] == 1) {		// Vertex is obstacle
					continue;
				}

				// Iterate over all neighbors (moves)
				foreach (Vector2 move in moves) {
					Vector2 newPos = pos + move;
					x = (int) newPos.x;
					y = (int) newPos.y;

					// Check if edge should exist
					// Add edge with current vertex, neighbor vertex
					// and magnitude (norm) as weight
					if (x >= 0 && x < xdim && y >= 0 && y < ydim
						&& A[y,x] == 0) {
						
						g.AddEdge(
							state,
							new DiscreteState(x, y),
							move.magnitude
						);
					}
				}
			}

		} finally {
			sr.Close();		// Closing stream
		}	
	}


	// Function reads the file and creates a visibility graph from the data
	public static void CreatePolyFromFile(String filename,
		out GraphState graph, out IState start, out IState goal,
		List<Polygon> polys) {

		StreamReader sr = new StreamReader(filename);
		try {
			// Read start
			string[] sxy = sr.ReadLine().Split(' ');
			start = new ContinuousState(sxy[0], sxy[1]);

			// Read goal
			string[] gxy = sr.ReadLine().Split(' ');
			goal = new ContinuousState(gxy[0], gxy[1]);

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

			// Create visibility
			graph = CreateVisibilityGraph(count, button, vertices,
				start.ToVector2(), goal.ToVector2(), polys);

		} finally {
			sr.Close();		// Close stream
		}
	}

	// Creates visibility graph from the given data
	public static GraphState CreateVisibilityGraph(int n, int[] button,
		Vector2[] points, Vector2 start, Vector2 goal, List<Polygon> polys) {
		
		// Initialize vertices collection
		List<Vector2> vertices = new List<Vector2>(points);
		vertices.Add(start);
		vertices.Add(goal);

		// Initialize polygon and edges collection
		List<Edge> edges = new List<Edge>();
		List<Vector2> buffer = new List<Vector2>();
		for (int i = 0; i < n; i++) {
			buffer.Add(points[i]);
			if (button[i] == 3) {
				Polygon newPol = new Polygon(buffer);
				polys.Add(newPol);
				foreach (Edge e in newPol.IterEdges()) {
					edges.Add(e);
				}
				buffer.Clear();
			}
		}

		// Initialize graph with vertices
		List<IState> vert = new List<IState>();
		foreach (Vector2 v in vertices) {
			vert.Add(new ContinuousState(v.x, v.y));
		}
		GraphState g = new GraphState(vert);


		// Iterate over all vertices
		// This part is O(n^3)
		foreach (Vector2 f in vertices) {
			foreach (Vector2 s in vertices) {
				if (f.Equals(s)) {		// Same vertex
					continue;
				}

				// Create current edge
				Edge curr = new Edge(f, s);
				bool intersects = false;

				// Iterate over all edges
				// This is inner loop
				foreach (Edge e in edges) {
					if (curr.Intersect(e)) {	// Check each edge
						intersects = true;
						break;
					}
				}

				// If there is any kind of intersection with polygon
				// continue to next case
				if (intersects) {
					continue;
				}

				// Checking if midpoint is inside polygon
				Vector2 p = (f + s) / 2.0f;

				// Find which polygon edge belongs to
				Polygon pol = null;
				foreach (Polygon polTmp in polys) {
					// Only interested if edge is not actual edge of the
					// polygon, but a contact between any other 2 points
					// that do not form and edge
					if (	polTmp.ContainsVertex(f)
						&& 	polTmp.ContainsVertex(s)
						&&	!polTmp.ContainsEdge(curr)) {
						
						pol = polTmp;
						break;
					}
				}
				
				// Checking if the point is inside the polygon
				if (pol != null && pol.IsInside(p)) {
					continue;
				}

				// Otherwise, current edge is visible and is added tp the graph
				IState a = new ContinuousState(f.x, f.y);
				IState b = new ContinuousState(s.x, s.y);
				g.AddEdge(a, b, Vector2.Distance(f, s));
			}
		}

		return g;
	}

}
