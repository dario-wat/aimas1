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

			// TODO in progress
			g = new GraphState(vertices);
			foreach (IState vertex in vertices) {
				DiscreteState state = vertex as DiscreteState;
				int x = state.x;
				int y = state.y;

				if (A[y,x] == 1) {
					continue;
				}
			}

			/*
			// Start adding edges, this part adds edges to the right and up
			g = new GraphState(vertices);
			for (int y = 0; y < ydim-1; y++) {
				for (int x = 0; x < xdim-1; x++) {
					if (A[y,x] == 1) {					// Obstacle
						continue;
					}
					if (A[y+1,x] == 0) {				// Add edge up
						g.AddEdge(
							new DiscreteState(x, y),
							new DiscreteState(x, y+1)
						);
					}
					if (A[y,x+1] == 0) {				// Add edge right
						g.AddEdge(
							new DiscreteState(x, y),
							new DiscreteState(x+1, y)
						);
					}
				}
			}
			
			// Add rightmost column edges up
			for (int y = 0; y < ydim-1; y++) {
				if (A[y,xdim-1] == 0 && A[y+1,xdim-1] == 0) {	// Not obstacle
					g.AddEdge(
						new DiscreteState(xdim-1, y),
						new DiscreteState(xdim-1, y+1)
					);
				}
			}

			// Add topmost row edges right
			for (int x = 0; x < xdim-1; x++) {
				if (A[ydim-1,x] == 0 && A[ydim-1,x+1] == 0) {	// Not obstacle
					g.AddEdge(
						new DiscreteState(x, ydim-1),
						new DiscreteState(x+1, ydim-1)
					);
				}
			}

			if (neigh == N8) {
				
			}
			*/

		} finally {
			sr.Close();		// Closing stream
		}	
	}

}
