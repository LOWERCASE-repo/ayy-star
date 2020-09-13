#pragma warning disable 649
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;

using Behaviours;

namespace Navigation {
	
	class Navigator : MonoBehaviour {
		
		internal static Navigator self;
		void Awake() => self = this;
		
		Queue<Explorer> explorers = new Queue<Explorer>();
		HashSet<Explorer> explorerSet = new HashSet<Explorer>();
		HashSet<NavNode> openNodes = new HashSet<NavNode>();
		Explorer explorer;
		NavNode current, target;
		
		internal void Guide(Explorer explorer) {
			if (explorerSet.Contains(explorer)) return;
			explorers.Enqueue(explorer);
			explorerSet.Add(explorer);
		}
		
		void FixedUpdate() { // throttles one call per physics update
			if (explorers.Count == 0) return;
			explorer = explorers.Dequeue();
			explorerSet.Remove(explorer);
			NavGraph.self.ResetNodes();
			explorer.path.Clear();
			openNodes.Clear();
			FindPath();
		}
		
		void FindPath() {
			// null nodes inaccessible
			if (explorer.node == null) explorer.node = NavGraph.self.FindNode(explorer.body.position);
			if (explorer.targetNode == null) explorer.targetNode = NavGraph.self.FindNode(explorer.target.position);
			if (explorer.node == null || explorer.targetNode == null) return;
			target = explorer.targetNode;
			current = explorer.node;
			current.Start();
			FindPathRec();
			while (current.parent != null) {
				explorer.path.Push(current);
				current = current.parent;
			}
		}
		
		void FindPathRec() {
			if (current == target) return;
			foreach (KeyValuePair<NavNode, float> link in current.links) {
				NavNode node = link.Key;
				if (node.closed) continue;
				node.Calibrate(current, target.pos);
				openNodes.Add(node);
			}
			openNodes.Remove(current);
			current.closed = true;
			current = openNodes.OrderBy(i => i.TotalDist).FirstOrDefault();
			FindPathRec();
		}
	}
}
