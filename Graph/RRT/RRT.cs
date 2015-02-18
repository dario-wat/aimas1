using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Linq;

/**
	Rapidly Exploring Random Tree, used for more complex vehicle models.
	With K iterations tries to reach the goal with optimal path. Using more
	iterations, the path will be better.
*/
public class RRT<VState> where VState : class, IVehicleState<VState> {

	// Number of iterations
	private int K = 100;

	// Neighborhood size
	private int M = 1;

	// Initial and goal state
	private VState initial;
	private VState goal;

	// Generator function, generates random state in vehicle state space
	private Func<VState> generate;

	// List of polygonal obstacles, this should be modified at later stage
	// to support other types of obstacles
	private readonly List<Polygon> obstacles;

	// Root node of the RRT, in other words, Node that wraps initial state
	private Node root;

	// Goal node, used for tracing
	private Node goalNode;

	// Cost of the whole path
	public float cost {
		get { return goalNode.cost; }
	}

	// RRT Running time
	public float runTime { get; private set; }

	// Path with nodes
	private List<Node> path;

	// Path in RRT in terms of moves
	public ReadOnlyCollection<Move> moves { get; private set; }

	// Edges of the rrt graph
	public ReadOnlyCollection<Edge> edges { get; private set; }

	// All vertices in the graph
	public ReadOnlyCollection<Vector3> vertices { get; private set; }

	// Vertices that are used in moving the vehicle
	public ReadOnlyCollection<Vector3> corners { get; private set; }

	// Edges that make up the path
	public ReadOnlyCollection<Edge> pathEdges { get; private set; }


	// Constructor that initializes variables and runs RRT
	public RRT(VState initial, VState goal, Func<VState> generate,
		IEnumerable<Polygon> obstacles, int K, int M) {
		
		this.initial = initial;
		this.goal = goal;
		this.generate = generate;
		this.K = K;
		this.M = M;
		this.obstacles = new List<Polygon>(obstacles);
		this.root = new Node(this.initial, null);
		
		float startTime = Time.realtimeSinceStartup;
		rrt();
		this.runTime = Time.realtimeSinceStartup - startTime;
	}


	// Runs RRT algorithm and finds "optimal" path
	// Fills all necessary variables with data
	// Vertices and edges
	private void rrt() {
		// Initialize some stuff
		List<Node> nodes = new List<Node>();
		nodes.Add(root);
		List<Edge> edges = new List<Edge>();
		List<Vector3> vertices = new List<Vector3>();
		vertices.Add(initial.vec3);
		
		// Iterative exploring
		int uz=0;
		for (int i = 0; i <= K; i++) {
			uz++;
			//if (uz > 100) break;
			//Debug.Log(i);
			// Generate random state
			VState randomState = generate();
			if (i >= K) {		// In last step try to reach goal
				randomState = goal;
			}


			bool goout = false;
			foreach (Polygon p in obstacles) {
				if (p.IsInside(randomState.vec2)) {
					goout = true;
					break;
				}
			}
			if (goout) {
				i--;
				continue;
			}
			
			// Find closest points
			FFFBHeap<Node> neighborhood = new FFFBHeap<Node>(M);
			foreach (Node n in nodes) {
				float distance = randomState.Distance(n.state);
				neighborhood.Insert(distance, n);
			}

			
			
			// Check if its obstructed
			Tuple<List<Move>, VState> moves = null;
			Node nearest = null;
			float minDist = float.MaxValue;
			VState newState = null;
			
			foreach (Node n in neighborhood) {
				//Debug.Log("From: " + n.state + "  To: " + randomState);
				Tuple<List<Move>, VState> tMoves = n.state.MovesTo(randomState);
				if (Move.Obstructed(tMoves._1, obstacles, n.state.vec3)) {
					continue;
				}
				float distance = tMoves._2.Distance(n.state);
				if (distance + n.cost < minDist) {
					nearest = n;
					minDist = distance + n.cost;
					newState = tMoves._2;
					moves = tMoves;
				}
			}
			//Debug.Log(i + " >= " + K);
			//Debug.Log(nearest);
			if (nearest == null) {
				if (i >= K) {
					break;
				}
				i--;		// It is obstructed, generate new point
				continue;
			}
			

			// Aggregate cost
			float moveCost = 0.0f;
			foreach (Move m in moves._1) {
				moveCost += m.t;
			}

			// TODO this is literally edge, a line with 2 points
			// Should be changed later
			edges.Add(new Edge(nearest.state.vec2, newState.vec2));

			// At this point, nearest cannot be null because there is always
			// at least one node in the tree whose distance is less than inf
			// nearest is also reachable
			vertices.Add(newState.vec3);
			nodes.Add(new Node(newState, nearest, moves._1,
				nearest.cost + moveCost));
		}

		// End, goal is last position
		/*float minD = float.MaxValue;
		Node best = null;
		foreach (Node n in nodes) {
			float d = n.state.Distance(goal);
			if (d < minD) {
				minD = d;
				best = n;
			}
		}
		Debug.Log(best + " " + minD);
		*/
		this.goalNode = nodes[nodes.Count-1];
		this.edges = edges.AsReadOnly();
		this.vertices = vertices.AsReadOnly();
		Trace();
	}

	// Traces path back from the goal to the root
	// Sets moves and vertices that are used in moves
	private void Trace() {
		// Path does not exist
		if (goalNode == null) {
			this.moves = null;
			this.corners = null;
			return;
		}

		// Trace path
		List<Vector3> corners = new List<Vector3>();
		this.path = new List<Node>();
		Node x = goalNode;
		while (x != null) {
			this.path.Add(x);
			corners.Add(x.state.vec3);
			x = x.parent;
		}
		path.Reverse();
		this.corners = corners.AsReadOnly();

		// Add edges between corners
		List<Edge> pathEdges = new List<Edge>();
		for (int i = 0; i < corners.Count-1; i++) {
			pathEdges.Add(new Edge(
				new Vector2(corners[i].x, corners[i].z),
				new Vector2(corners[i+1].x, corners[i+1].z)
			));
		}
		this.pathEdges = pathEdges.AsReadOnly();

		// Add moves to the list
		List<Move> moves = new List<Move>();
		foreach (Node n in path) {
			moves.AddRange(n.moves);
		}
		this.moves = moves.AsReadOnly();
	}


	// Helping class for the RRT
	// All states are wrapped with this class
	private class Node {

		// State that the node holds
		public readonly VState state;

		// All nodes that go from this node
		private List<Node> children;

		// Every node also holds a link to its parent for easier path tracing
		public readonly Node parent;

		// Moves that this node is holding, moves need to reach this state
		// from its parent's state
		public readonly List<Move> moves;

		// The cost to get to this node from initial nodes
		public float cost = 0.0f;


		// Initialize node and create empty children list
		public Node(VState state, Node parent,
			IEnumerable<Move> moves, float cost) {
			
			this.state = state;
			this.children = new List<Node>();	// Currently not used
			this.parent = parent;
			this.moves = new List<Move>(moves);
			this.cost = cost;
		}

		// Constructor without moves, sets moves to empty list
		public Node(VState state, Node parent)
			: this(state, parent, new List<Move>(), 0.0f) {
		}

		// Add node to the list of child nodes
		public void Add(Node n) {
			children.Add(n);
		}

		// Generator to iterate all descendants of this node
		// Traversing order is not important, includes itself
		public IEnumerable<Node> Iterate() {
			IEnumerable<Node> sequence = children.Concat(new Node[] {this});
			foreach (Node n in children) {
				sequence = Enumerable.Concat(sequence, n.Iterate());
			}
			return sequence;
		}

		public readonly static IComparer<Node> costComp = new CostComparer();

		// Comparer for inserting sorted
		private class CostComparer : IComparer<Node> {

			// Compares cost as both g and h
			public int Compare(Node o1, Node o2) {
				return (int) (o1.cost - o2.cost);
			}
		}
	}
}
