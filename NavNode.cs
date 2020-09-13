#pragma warning disable 649
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Navigation {
	
	class NavNode {
		
		internal Vector2 pos;
		internal bool closed;
		internal NavNode parent;
		internal Dictionary<NavNode, float> links = new Dictionary<NavNode, float>();
		internal float TotalDist { get => seekerDist + targetDist; }
		internal void Start() => seekerDist = 0f;
		float seekerDist, targetDist;
		
		internal NavNode(Vector2 pos) {
			this.pos = pos;
			Reset();
		}
		
		internal void Reset() {
			seekerDist = Mathf.Infinity;
			targetDist = Mathf.Infinity;
			parent = null;
			closed = false;
		}
		
		internal void Link(NavNode node) { // unity's .net doesnt have tryadd lmfao
			if (!links.ContainsKey(node)) links.Add(node, 0f);
			if (!node.links.ContainsKey(this)) node.links.Add(this, 0f);
		}
		
		internal void GenWeights() {
			foreach (NavNode node in links.Keys.ToList()) {
				if (links[node] != 0f) continue;
				float dist = (node.pos - pos).magnitude;
				links[node] = dist;
				node.links[this] = dist;
			}
		}
		
		internal void Calibrate(NavNode node, Vector2 target) {
			float seekerDist = node.seekerDist + node.links[this];
			if (seekerDist < this.seekerDist) {
				this.seekerDist = seekerDist;
				parent = node;
			}
			if (targetDist == Mathf.Infinity) targetDist = (target - pos).magnitude;
		}
	}
}
