using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

/**
	Rapidly Exploring Random Tree, used for more complex vehicle models.
	With K iterations tries to reach the goal with optimal path. Using more
	iterations, the path will be better.
*/
public class RRT<VState> where VState : class, IVehicleState<VState> {

	// Number of iterations
	private int K = 1000;

	// Initial and goal state
	private VState initial;
	private VState goal;

	// Generator function, generates random state in vehicle state space
	private Func<VState> generate;

	// List of polygonal obstacles, this should be modified at later stage
	// to support other types of obstacles
	private List<Polygon> obstacles;

	// Root node of the RRT, in other words, Node that wraps initial state
	private Node root;

	// Goal node, used for tracing
	private Node goalNode;

	// Path in RRT in terms of moves
	public List<Move> moves { get; private set; }


	// Constructor that initializes variables and runs RRT
	public RRT(VState initial, VState goal, Func<VState> generate,
		IEnumerable<Polygon> obstacles) {
		
		this.initial = initial;
		this.goal = goal;
		this.generate = generate;
		this.obstacles = new List<Polygon>(obstacles);
		this.root = new Node(this.initial, null, null);
		rrt();
	}

	// Runs RRT algorithm and finds "optimal" path
	private void rrt() {
		for (int i = 0; i <= K; i++) {
			// Generate random state
			VState randomState = generate();
			if (i == K) {		// In last step try to reach goal
				randomState = goal;
			}
			
			// Find closest point
			Node nearest = null;
			float minDist = float.MaxValue;
			foreach (Node n in root.Iterate()) {
				float distance = randomState.Distance(n.state);
				if (distance < minDist) {
					minDist = distance;
					nearest = n;
				}
			}

			// TODO check here if i can reach that point (not go through obstacles)
			List<Move> moves = randomState.MovesTo(nearest.state);


			// At this point, nearest cannot be null because there is always
			// at least one node in the tree whose distance is less than inf
			// nearest is also reachable
			nearest.Add(new Node(randomState, nearest, moves));
			if (randomState.Equals(goal)) {
				goalNode = nearest;
				Trace();
				return;
			}
		}
	}

	// Traces path back from the goal to the root
	private void Trace() {
		// Path does not exist
		if (goalNode == null) {
			this.moves = null;
			return;
		}

		// Trace path
		List<Node> path = new List<Node>();
		Node x = goalNode;
		while (x != null) {
			path.Add(x);
			x = x.parent;
		}
		
		// Add moves to the list
		path.Reverse();
		this.moves = new List<Move>();
		foreach (Node n in path) {
			this.moves.AddRange(n.moves);
		}
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


		// Initialize node and create empty children list
		public Node(VState state, Node parent, IEnumerable<Move> moves) {
			this.state = state;
			this.children = new List<Node>();
			this.parent = parent;
			this.moves = new List<Move>(moves);
		}

		// Constructor without moves, sets moves to empty list
		public Node(VState state, Node parent)
			: this(state, parent, new List<Move>()) {
		}

		// Add node to the list of child nodes
		// Sets the parent to this node
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
	}
}
