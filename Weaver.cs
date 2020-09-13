#pragma warning disable 649
using UnityEngine;
using System.Collections.Generic;
using LevelGen;

namespace Navigation {
	
	class Weaver {
		
		internal HashSet<NavNode> nodes = new HashSet<NavNode>();
		Bezite bezite;
		
		internal Weaver(Chassis chassis) {
			List<NavNode> bridgeNodes = new List<NavNode>();
			NavNode end = null;
			foreach (Bezite bridge in chassis.bridges) {
				bezite = bridge;
				List<NavNode> nodes = new List<NavNode>(new Linker<NavNode>(InitNode, Eval).result);
				if (end != null) {
					this.nodes.Remove(nodes[0]);
					nodes.RemoveAt(0);
					end.Link(nodes[0]);
				} else bridgeNodes.Add(nodes[0]);
				end = nodes[nodes.Count - 1];
				bridgeNodes.Add(end);
				for (int i = 0; i < nodes.Count - 1; i++) {
					nodes[i].Link(nodes[i + 1]);
				}
			}
			for (int i = 0; i < bridgeNodes.Count; i++) {
				WeaveBranches(bridgeNodes[i], chassis.branchSets[i]);
			}
		}
		
		void WeaveBranches(NavNode pivot, HashSet<Bezite> branches) {
			foreach (Bezite branch in branches) {
				bezite = branch;
				List<NavNode> nodes = new List<NavNode>(new Linker<NavNode>(InitNode, Eval).result);
				this.nodes.Remove(nodes[0]);
				nodes.RemoveAt(0);
				pivot.Link(nodes[0]);
				for (int i = 0; i < nodes.Count - 1; i++) {
					nodes[i].Link(nodes[i + 1]);
				}
			}
		}
		
		NavNode InitNode(float time) {
			NavNode node = new NavNode(bezite.Eval(time));
			nodes.Add(node);
			return node;
		}
		
		bool Eval(LinkedListNode<NavNode> node) {
			// return (!node.Value.pos.CastNav(node.Next.Value.pos, 0.5f));
			return (!node.Value.pos.CastNav(node.Next.Value.pos));
		}
	}
}
