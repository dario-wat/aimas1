using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/**
	Basic Graph API for all graph operations.
*/
public class GraphState {

	// Default weight
	private const float DEF_WEIGHT = 1.0f;

	// Number of vertices
	public readonly int V;

	// Number of edges
	public int E { get; private set; }

	// Adjacency list with weights
	private IDictionary<IState, IDictionary<IState, float>> adj;


	// Initializes adjacency list
	public GraphState(IEnumerable<IState> vertices) {
		this.adj = new Dictionary<IState, IDictionary<IState, float>>();
		foreach (IState s in vertices) {
			adj.Add(s, new Dictionary<IState, float>());
		}
		this.V = adj.Count;
		this.E = 0;
	}

	// Adds edge to the graph
	public void AddEdge(IState v, IState w, float d) {
		if (!adj.ContainsKey(v) || !adj.ContainsKey(w)) {
			throw new ArgumentException("Vertex does no exist");
		}

		adj[v].Add(w, d);
		adj[w].Add(v, d);
		E++;
	}

	// Adds edge to the graph with predefined weight
	public void AddEdge(IState v, IState w) {
		this.AddEdge(v, w, DEF_WEIGHT);
	}

	// Returns enumerable of nodes adjacent to given vertex
	public IEnumerable<IState> Adjacent(IState v) {
		if (!adj.ContainsKey(v)) {
			throw new ArgumentException("Vertex does no exist");
		}
		return adj[v].Keys;
	}

	// Returns the weight of the edge between two given vertices
	public float Cost(IState v, IState w) {
		if (!adj.ContainsKey(v) || !adj[v].ContainsKey(w)) {
			throw new ArgumentException("Vertex does no exist");
		}
		return adj[v][w];
	}

}
