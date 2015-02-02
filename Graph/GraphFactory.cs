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
*/
public class GraphFactory {

	// Constants for neighborhoods
	public const int N4 = 1;
	public const int N8 = 2;
	public const int N12 = 3;

	// List for moves in 4-connect neighborhood
	private static readonly List<Vector2> movesN4 = new List<Vector2>() {
		new Vector2( 1, 0),
		new Vector2(-1, 0),
		new Vector2( 0, 1),
		new Vector2( 0,-1)
	};

	// List for moves in 8-connect neighborhood
	private static readonly List<Vector2> movesN8 = new List<Vector2>() {
		new Vector2( 1, 0),
		new Vector2(-1, 0),
		new Vector2( 0, 1),
		new Vector2( 0,-1),
		new Vector2( 1, 1),
		new Vector2(-1, 1),
		new Vector2(-1,-1),
		new Vector2( 1,-1)
	};


	// Construct discrete graph from file
	// It is returning 3 values thus out modifiers
	// Parameter neigh tells which neighborhood to use
	public static void CreateDiscreteFromFile(string filename, int neigh,
		out GraphState g, out IState start, out IState goal) {

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
				}
			}

			// Picking the right neighborhood list
			List<Vector2> moves = null;
			if (neigh == N4 || neigh == N12) {	// What is 12-neighborhood
				moves = movesN4;
			} else if (neigh == N8) {
				moves = movesN8;
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

}
