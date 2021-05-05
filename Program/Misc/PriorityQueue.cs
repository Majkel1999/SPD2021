using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPD1.Misc
{
	struct Node
	{
		public RPQJob Job;
		public int Priority;

		public Node(RPQJob job, int priority)
		{
			Job = job;
			Priority = priority;
		}
	}

	class PriorityQueue
	{
		public List<Node> nodes;

		public int Count => nodes.Count;

		public PriorityQueue()
		{
			nodes = new List<Node>();
		}

		public void Add(RPQJob job, int priority)
		{
			int frontPriority = int.MaxValue;
			int backPriority = int.MinValue;
			if (nodes.Count == 0)
			{
				nodes.Add(new Node(job, priority));

				return;
			}
			if (nodes.Count == 1)
			{
				if (nodes[0].Priority >= priority)
					nodes.Add(new Node(job, priority));
				else
					nodes.Insert(0, new Node(job, priority));

				return;
			}

			for (int i = 0; i < nodes.Count - 1; i++)
			{
				frontPriority = nodes[i].Priority;
				backPriority = nodes[i + 1].Priority;
				if (priority > frontPriority)
				{
					nodes.Insert(i, new Node(job, priority));
					return;
				}
				else if (priority > backPriority)
				{
					nodes.Insert(i + 1, new Node(job, priority));
					return;
				}
			}

			if (backPriority >= priority)
				nodes.Add(new Node(job, priority));
		}


		public RPQJob GetHighest()
		{
			return nodes.First().Job;
		}

		public RPQJob GetHighestAndRemove()
		{
			RPQJob value = nodes.First().Job;
			nodes.RemoveAt(0);
			return value;
		}

		public RPQJob GetLowest()
		{
			return nodes.Last().Job;
		}

		public RPQJob GetLowestAndRemove()
		{
			RPQJob value = nodes.Last().Job;
			nodes.RemoveAt(nodes.Count - 1);
			return value;
		}
	}
}
