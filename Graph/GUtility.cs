using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/**
	This class holds some utilities for debugging purposes
	for GraphState class.
*/
public class GUtility {

	// Converts discrete graph into string, this is only used for
	// debugging purposes
	public static string DiscreteToString(int[,] A) {
		// Dimensions
		int ydim = A.GetLength(0);
		int xdim = A.GetLength(1);
		
		// Save everything into StringBuilder
		StringBuilder sb = new StringBuilder();
		for (int i = 0; i < ydim; i++) {
			for (int j = 0; j < xdim; j++) {
				if (A[ydim-i-1,j] == 1) {
					sb.Append('#');
				} else {
					sb.Append(' ');
				}
			}
			sb.Append('\n');
		}
		return sb.ToString();
	}

	// Creates a test graph, grid of 10 by 10
	public static GraphState CreateTest() {
		int n = 10;
		List<IState> vertices = new List<IState>();
		for (int i = 0; i < n; i++) {
			for (int j = 0; j < n; j++) {
				vertices.Add(new DiscreteState(i, j));
			}
		}

		GraphState g = new GraphState(vertices);
		for (int i = 0; i < n-1; i++) {
			for (int j = 0; j < n-1; j++) {
				g.AddEdge(new DiscreteState(i, j), new DiscreteState(i+1,j));
				g.AddEdge(new DiscreteState(i, j), new DiscreteState(i,j+1));
			}
		}
		for (int i = 0; i < n-1; i++) {
			g.AddEdge(new DiscreteState(i, n-1), new DiscreteState(i+1, n-1));
			g.AddEdge(new DiscreteState(n-1, i), new DiscreteState(n-1, i+1));
		}
		return g;
	}
}
