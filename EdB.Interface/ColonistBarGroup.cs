using System;
using System.Collections.Generic;
using Verse;

namespace EdB.Interface
{
	public class ColonistBarGroup
	{
		private List<TrackedColonist> colonists;

		private string name;

		private bool visible;

		private string id = "";

		public string Id
		{
			get
			{
				return this.id;
			}
			set
			{
				this.id = value;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
			}
		}

		public bool Visible
		{
			get
			{
				return this.visible;
			}
			set
			{
				this.visible = value;
			}
		}

		public List<TrackedColonist> Colonists
		{
			get
			{
				return this.colonists;
			}
		}

		public int OrderHash
		{
			get
			{
				int num = 33;
				foreach (TrackedColonist current in this.colonists)
				{
					num = 17 * num + current.Pawn.GetUniqueLoadID().GetHashCode();
				}
				num = 17 * num + this.name.GetHashCode();
				return num;
			}
		}

		public ColonistBarGroup()
		{
			this.colonists = new List<TrackedColonist>();
		}

		public ColonistBarGroup(int reserve)
		{
			if (reserve > 0)
			{
				this.colonists = new List<TrackedColonist>(reserve);
			}
			else
			{
				this.colonists = new List<TrackedColonist>();
			}
		}

		public ColonistBarGroup(string name, List<TrackedColonist> colonists)
		{
			this.colonists = colonists;
			this.name = name;
		}

		public void Clear()
		{
			this.colonists.Clear();
		}

		public void Add(TrackedColonist colonist)
		{
			if (!this.colonists.Contains(colonist))
			{
				this.colonists.Add(colonist);
			}
		}

		public bool Remove(TrackedColonist colonist)
		{
			return this.colonists.Remove(colonist);
		}

		public bool Remove(Pawn pawn)
		{
			int num = this.colonists.FindIndex((TrackedColonist c) => c.Pawn == pawn);
			bool result;
			if (num != -1)
			{
				this.colonists.RemoveAt(num);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}
	}
}
