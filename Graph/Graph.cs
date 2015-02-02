using UnityEngine;
using System.Collections.Generic;
using System;

public class Graph {

	// Number of vertices
	public readonly int V;

	// Number of edges
	public int E { get; private set; }

	// Adjacency list
	private List<List<int>> adj;

	// Creates adjacency list
	public Graph(int V) {
		this.V = V;
		this.E = 0;
		this.adj = new List<List<int>>();
		for (int i = 0; i < V; i++) {
			adj[i] = new List<int>();
		}
	}

	// Adds edge to the graph
	public void addEdge(int v, int w) {
		if (v < 0 || w < 0 || v >= V || w >= V) {
			throw new ArgumentException("Vertex does no exist");
		}

		adj[v].Add(w);
		adj[w].Add(v);
		E++;
	}

	// Returns enumerable of nodes adjacent to given vertex
	public IEnumerable<int> adjacent(int v) {
		if (v < 0 || v >= V) {
			throw new ArgumentException("Vertex does no exist");
		}
		return adj[v];
	}
}
