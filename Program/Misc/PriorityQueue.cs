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
		public int firstPriority = int.MaxValue;
		public int lastPriority = int.MinValue;
		public int Count => nodes.Count;

		public PriorityQueue()
		{
			nodes = new List<Node>();
		}

		public void Add(RPQJob job, int priority)
		{
			
			if (Count == 0)
			{
				nodes.Add(new Node(job, priority));
				firstPriority = priority;
				lastPriority = priority;
				return;
			}
			if (Count == 1)
			{
				if (nodes[0].Priority >= priority)
				{
					nodes.Add(new Node(job, priority));
					lastPriority = priority;
				}
				else
				{
					nodes.Insert(0, new Node(job, priority));
					firstPriority = priority;
				}
				return;
			}
			
			//int backPriority = nodes.Last().Priority;
			if (lastPriority >= priority)
            {
                nodes.Add(new Node(job, priority));
				lastPriority = priority;
                return;
            }
			//int firstPriority = nodes.First().Priority;

			if(priority>firstPriority)
            {
				nodes.Insert(0, new Node(job, priority));
				firstPriority = priority;
				return;
            }

            //for (int i = 0; i < nodes.Count - 1; i++)
            //{
            //    frontPriority = nodes[i].Priority;
            //    backPriority = nodes[i + 1].Priority;
            //    if (priority > frontPriority)
            //    {
            //        nodes.Insert(i, new Node(job, priority));
            //        return;
            //    }
            //    else if (priority > backPriority)
            //    {
            //        nodes.Insert(i + 1, new Node(job, priority));
            //        return;
            //    }
            //}

			//if (index == -1)
			//	nodes.Add(new Node(job, priority));
			//else


            //if (backPriority >= priority)
            //    nodes.Add(new Node(job, priority));
			int index = nodes.FindIndex(x => x.Priority < priority);
			nodes.Insert(index, new Node(job, priority));
        }


        public RPQJob GetHighest()
		{
			return nodes.First().Job;
		}

		public RPQJob GetHighestAndRemove()
		{
			RPQJob value = nodes.First().Job;
			nodes.RemoveAt(0);
			if (nodes.Count != 0)
			{
				firstPriority = nodes[0].Priority;
			}
            else
            {
				firstPriority = int.MaxValue;
				lastPriority = int.MinValue;
            }
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
			if (nodes.Count != 0)
			{
				lastPriority = nodes.Last().Priority;
			}
			else
			{
				firstPriority = int.MaxValue;
				lastPriority = int.MinValue;
			}
			return value;
		}

		public void DivideList(RPQJob job,int priority)
        {
			int count2 = Count / 2;
			int count4 = Count / 4;
			if (nodes[count2].Priority > priority)
			{
				if(nodes[count2+count4].Priority > priority)
                {
					int index = nodes.FindIndex(count2+count4+1,Count-(count2+count4+1),x => x.Priority < priority);
					nodes.Insert(index, new Node(job, priority));
				}
                else
                {
					int index = nodes.FindIndex(count2 + 1, (count2+count4)-(count2+1)+1, x => x.Priority < priority);
					if (index == -1)
					{
						nodes.Insert((count2 + count4) - (count2 + 1) + 1, new Node(job, priority));
					}
					else
						nodes.Insert(index, new Node(job, priority));
				}
			}
			else 
			{
				if (nodes[count4].Priority > priority)
				{
					int index = nodes.FindIndex(count4 + 1, count2 - (count4+1) +1, x => x.Priority < priority);
					if (index == -1)
					{
						nodes.Insert(count2 - (count4 + 1) + 1, new Node(job, priority));
					}
					nodes.Insert(index, new Node(job, priority));
				}
				else
				{
					int index = nodes.FindIndex(0, count4+1, x => x.Priority < priority);
					if(index==-1)
                    {
						nodes.Insert(count4 + 1, new Node(job, priority));
                    }else
					nodes.Insert(index, new Node(job, priority));
				}
			}

        }
		
	}
}
