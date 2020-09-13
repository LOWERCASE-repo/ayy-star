#pragma warning disable 649
using UnityEngine;
using System.Collections.Generic;
using Navigation;

namespace Behaviours {
	
	class Explorer : Mover {
		
		internal NavNode node;
		internal NavNode targetNode;
		internal Rigidbody2D target;
		internal Stack<NavNode> path = new Stack<NavNode>();
		
		protected void FixedUpdate() {
			if (target == null) return;
			if (body.position.CastNav(target.position)) {
				Move(target);
				return;
			}
			if (!Validate()) return; // maybe throttle castnavs too
			if (!body.position.CastNav(node.pos)) node = null;
			if (!target.position.CastNav(targetNode.pos)) targetNode = null;
			if (!Validate()) return; // node and targetnode may have changed
			while (path.Count != 0 && body.position.CastNav(path.Peek().pos)) {
				node = path.Peek();
				path.Pop();
			}
			Move(node.pos - body.position);
		}
		
		bool Validate() {
			bool invalid = (node == null || targetNode == null);
			if (invalid) Navigator.self.Guide(this);
			return !invalid;
		}
	}
}
