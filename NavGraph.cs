#pragma warning disable 649
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Navigation {
	
	class NavGraph : MonoBehaviour {
		
		internal static NavGraph self;
		void Awake() => self = this;
		HashSet<NavNode> nodes = new HashSet<NavNode>();
		
		void OnDrawGizmos() {
			foreach (NavNode node in nodes) {
				foreach (NavNode other in node.links.Keys) {
					Debug.DrawLine(node.pos, other.pos, new Color(0f, 0f, 0f, 0.1f));
				}
			}
		}
		
		internal void Init(HashSet<NavNode> nodes) {
			this.nodes = nodes;
			Optimize();
		}
		
		internal NavNode FindNode(Vector2 pos) {
			NavNode node = nodes
			.Where(i => pos.CastNav(i.pos))
			.OrderBy(i => (pos - i.pos).sqrMagnitude)
			.FirstOrDefault();
			if (node == null) Debug.Log("find node null");
			return node;
		}
		
		internal void ResetNodes() {
			foreach (NavNode node in nodes) node.Reset();
		}
		
		internal void Optimize() {
			List<NavNode> nodes = this.nodes
			.OrderByDescending(i => i.links.Count)
			.Where(i => i.links.Count > 1).ToList();
			for (int i = 0; i < nodes.Count; i++) {
				NavNode node = nodes[i];
				NavNode[] links = node.links.Keys
				.OrderBy(j => (j.pos - node.pos).Angle())
				.ToArray();
				if (Removable(links)) {
					LinkAround(node, links);
					this.nodes.Remove(node);
				}
			}
			foreach (NavNode node in this.nodes) node.GenWeights();
		}
		
		bool Removable(NavNode[] nodes) {
			for (int i = 0; i < nodes.Length; i++) {
				int next = (i + 1) % nodes.Length;
				if (!nodes[i].pos.CastNav(nodes[next].pos)) return false;
			}
			return true;
		}
		
		void LinkAround(NavNode center, NavNode[] nodes) {
			for (int i = 0; i < nodes.Length; i++) {
				int next = (i + 1) % nodes.Length;
				nodes[i].Link(nodes[next]);
				nodes[i].links.Remove(center);
			}
		}
	}
}
