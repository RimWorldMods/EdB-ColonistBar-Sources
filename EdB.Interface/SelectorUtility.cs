using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;

namespace EdB.Interface
{
	public class SelectorUtility
	{
		protected FieldInfo hostilePawnsField = null;

		protected FieldInfo allPawnsField = null;

		protected List<Pawn> emptyList = new List<Pawn>();

		protected List<Pawn> visitorPawns = new List<Pawn>(20);

		public int HostilePawnCount
		{
			get
			{
				Dictionary<Faction, List<Pawn>> dictionary = (Dictionary<Faction, List<Pawn>>)this.hostilePawnsField.GetValue(Find.get_MapPawns());
				int result;
				if (dictionary == null)
				{
					result = 0;
				}
				else
				{
					result = dictionary[Faction.OfColony].Count;
				}
				return result;
			}
		}

		public IEnumerable<Pawn> HostilePawns
		{
			get
			{
				Dictionary<Faction, List<Pawn>> dictionary = (Dictionary<Faction, List<Pawn>>)this.hostilePawnsField.GetValue(Find.get_MapPawns());
				IEnumerable<Pawn> result;
				if (dictionary == null)
				{
					result = this.emptyList;
				}
				else
				{
					result = from p in dictionary[Faction.OfColony]
					where !p.InContainer
					select p;
				}
				return result;
			}
		}

		public bool MoreThanOneHostilePawn
		{
			get
			{
				Dictionary<Faction, List<Pawn>> dictionary = (Dictionary<Faction, List<Pawn>>)this.hostilePawnsField.GetValue(Find.get_MapPawns());
				bool result;
				if (dictionary == null)
				{
					result = false;
				}
				else
				{
					int num = 0;
					foreach (Pawn current in from p in dictionary[Faction.OfColony]
					where !p.InContainer
					select p)
					{
						if (++num > 1)
						{
							result = true;
							return result;
						}
					}
					result = false;
				}
				return result;
			}
		}

		public bool MoreThanOneVisitorPawn
		{
			get
			{
				List<Pawn> list = (List<Pawn>)this.allPawnsField.GetValue(Find.get_MapPawns());
				bool result;
				if (list == null || list.Count < 2)
				{
					result = false;
				}
				else
				{
					int num = 0;
					foreach (Pawn current in from p in list
					where p.Faction != null && p.Faction != Faction.OfColony && !p.IsPrisonerOfColony && !p.Faction.RelationWith(Faction.OfColony, false).hostile && !p.InContainer
					select p)
					{
						if (++num > 1)
						{
							result = true;
							return result;
						}
					}
					result = false;
				}
				return result;
			}
		}

		public IEnumerable<Pawn> VisitorPawns
		{
			get
			{
				List<Pawn> list = (List<Pawn>)this.allPawnsField.GetValue(Find.get_MapPawns());
				IEnumerable<Pawn> result;
				if (list == null)
				{
					result = this.emptyList;
				}
				else
				{
					result = from p in list
					where p.Faction != null && p.Faction != Faction.OfColony && !p.IsPrisonerOfColony && !p.Faction.RelationWith(Faction.OfColony, false).hostile && !p.InContainer
					select p;
				}
				return result;
			}
		}

		public bool MoreThanOneColonyAnimal
		{
			get
			{
				int num = 0;
				bool result;
				foreach (Pawn current in from pawn in Find.get_MapPawns().PawnsInFaction(Faction.OfColony)
				where !pawn.IsColonist
				select pawn)
				{
					if (++num > 1)
					{
						result = true;
						return result;
					}
				}
				result = false;
				return result;
			}
		}

		public IEnumerable<Pawn> ColonyAnimals
		{
			get
			{
				return from pawn in Find.get_MapPawns().PawnsInFaction(Faction.OfColony)
				where !pawn.IsColonist
				select pawn;
			}
		}

		public SelectorUtility()
		{
			this.hostilePawnsField = typeof(MapPawns).GetField("pawnsHostileToFaction", BindingFlags.Instance | BindingFlags.NonPublic);
			this.allPawnsField = typeof(MapPawns).GetField("allPawns", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		public void SelectNextColonist()
		{
			Selector selector = Find.Selector;
			if (selector.SingleSelectedThing == null || !(selector.SingleSelectedThing is Pawn) || selector.SingleSelectedThing.Faction != Faction.OfColony)
			{
				this.SelectThing(Find.get_MapPawns().get_FreeColonists().FirstOrDefault<Pawn>(), false);
			}
			else
			{
				bool flag = false;
				foreach (Pawn current in Find.get_MapPawns().get_FreeColonists())
				{
					if (flag)
					{
						this.SelectThing(current, false);
						return;
					}
					if (current == selector.SingleSelectedThing)
					{
						flag = true;
					}
				}
				this.SelectThing(Find.get_MapPawns().get_FreeColonists().FirstOrDefault<Pawn>(), false);
			}
		}

		public void SelectPreviousColonist()
		{
			Selector selector = Find.Selector;
			if (selector.SingleSelectedThing == null || !(selector.SingleSelectedThing is Pawn) || selector.SingleSelectedThing.Faction != Faction.OfColony)
			{
				this.SelectThing(Find.get_MapPawns().get_FreeColonists().LastOrDefault<Pawn>(), false);
			}
			else
			{
				Pawn pawn = null;
				foreach (Pawn current in Find.get_MapPawns().get_FreeColonists())
				{
					if (selector.SingleSelectedThing == current)
					{
						if (pawn != null)
						{
							this.SelectThing(pawn, false);
							break;
						}
						this.SelectThing(Find.get_MapPawns().get_FreeColonists().LastOrDefault<Pawn>(), false);
						break;
					}
					else
					{
						pawn = current;
					}
				}
			}
		}

		public void SelectNextPrisoner()
		{
			Selector selector = Find.Selector;
			if (selector.SingleSelectedThing == null || !(selector.SingleSelectedThing is Pawn))
			{
				this.SelectThing(Find.get_MapPawns().get_PrisonersOfColony().FirstOrDefault<Pawn>(), false);
			}
			else
			{
				bool flag = false;
				foreach (Pawn current in Find.get_MapPawns().get_PrisonersOfColony())
				{
					if (flag)
					{
						this.SelectThing(current, false);
						return;
					}
					if (current == selector.SingleSelectedThing)
					{
						flag = true;
					}
				}
				this.SelectThing(Find.get_MapPawns().get_PrisonersOfColony().FirstOrDefault<Pawn>(), false);
			}
		}

		public void SelectPreviousPrisoner()
		{
			Selector selector = Find.Selector;
			if (selector.SingleSelectedThing == null || !(selector.SingleSelectedThing is Pawn))
			{
				this.SelectThing(Find.get_MapPawns().get_PrisonersOfColony().LastOrDefault<Pawn>(), false);
			}
			else
			{
				Pawn pawn = null;
				foreach (Pawn current in Find.get_MapPawns().get_PrisonersOfColony())
				{
					if (selector.SingleSelectedThing == current)
					{
						if (pawn != null)
						{
							this.SelectThing(pawn, false);
							break;
						}
						this.SelectThing(Find.get_MapPawns().get_PrisonersOfColony().LastOrDefault<Pawn>(), false);
						break;
					}
					else
					{
						pawn = current;
					}
				}
			}
		}

		public void SelectNextEnemy()
		{
			Selector selector = Find.Selector;
			if (selector.SingleSelectedThing == null || !(selector.SingleSelectedThing is Pawn) || selector.SingleSelectedThing.Faction == Faction.OfColony)
			{
				Pawn thing = this.HostilePawns.FirstOrDefault<Pawn>();
				this.SelectThing(thing, false);
			}
			else
			{
				bool flag = false;
				foreach (Pawn current in this.HostilePawns)
				{
					if (flag)
					{
						this.SelectThing(current, false);
						return;
					}
					if (current == selector.SingleSelectedThing)
					{
						flag = true;
					}
				}
				Pawn thing = this.HostilePawns.FirstOrDefault<Pawn>();
				this.SelectThing(thing, false);
			}
		}

		public void SelectPreviousEnemy()
		{
			Selector selector = Find.Selector;
			if (selector.SingleSelectedThing == null || !(selector.SingleSelectedThing is Pawn) || selector.SingleSelectedThing.Faction == Faction.OfColony)
			{
				this.SelectThing(this.HostilePawns.LastOrDefault<Pawn>(), false);
			}
			else
			{
				Pawn pawn = null;
				foreach (Pawn current in this.HostilePawns)
				{
					if (selector.SingleSelectedThing == current)
					{
						if (pawn != null)
						{
							this.SelectThing(pawn, false);
							break;
						}
						this.SelectThing(this.HostilePawns.LastOrDefault<Pawn>(), false);
						break;
					}
					else
					{
						pawn = current;
					}
				}
			}
		}

		public void SelectNextVisitor()
		{
			Selector selector = Find.Selector;
			if (selector.SingleSelectedThing == null || !(selector.SingleSelectedThing is Pawn) || selector.SingleSelectedThing.Faction == Faction.OfColony)
			{
				this.SelectThing(this.VisitorPawns.FirstOrDefault<Pawn>(), false);
			}
			else
			{
				bool flag = false;
				foreach (Pawn current in this.VisitorPawns)
				{
					if (flag)
					{
						this.SelectThing(current, false);
						return;
					}
					if (current == selector.SingleSelectedThing)
					{
						flag = true;
					}
				}
				this.SelectThing(this.VisitorPawns.FirstOrDefault<Pawn>(), false);
			}
		}

		public void SelectPreviousVisitor()
		{
			Selector selector = Find.Selector;
			if (selector.SingleSelectedThing == null || !(selector.SingleSelectedThing is Pawn) || selector.SingleSelectedThing.Faction == Faction.OfColony)
			{
				this.SelectThing(this.VisitorPawns.LastOrDefault<Pawn>(), false);
			}
			else
			{
				Pawn pawn = null;
				foreach (Pawn current in this.VisitorPawns)
				{
					if (selector.SingleSelectedThing == current)
					{
						if (pawn != null)
						{
							this.SelectThing(pawn, false);
							break;
						}
						this.SelectThing(this.VisitorPawns.LastOrDefault<Pawn>(), false);
						break;
					}
					else
					{
						pawn = current;
					}
				}
			}
		}

		public void SelectThing(Thing thing, bool addToSelection)
		{
			if (thing != null)
			{
				if (!addToSelection)
				{
					Find.Selector.ClearSelection();
				}
				Find.Selector.Select(thing, true, true);
				Find.MainTabsRoot.SetCurrentTab(MainTabDefOf.Inspect, true);
			}
		}

		public void SelectAllColonists()
		{
			Selector selector = Find.Selector;
			selector.ClearSelection();
			foreach (Pawn current in Find.get_MapPawns().get_FreeColonists())
			{
				Find.Selector.Select(current, false, false);
			}
			Find.MainTabsRoot.SetCurrentTab(MainTabDefOf.Inspect, true);
		}

		public void ClearSelection()
		{
			Find.Selector.ClearSelection();
		}

		public void AddToSelection(object o)
		{
			Find.Selector.Select(o, false, false);
		}

		public void SelectNextColonyAnimal()
		{
			Selector selector = Find.Selector;
			if (selector.SingleSelectedThing == null || !(selector.SingleSelectedThing is Pawn) || selector.SingleSelectedThing.Faction != Faction.OfColony || (selector.SingleSelectedThing as Pawn).IsColonist)
			{
				this.SelectThing(this.ColonyAnimals.FirstOrDefault<Pawn>(), false);
			}
			else
			{
				bool flag = false;
				foreach (Pawn current in this.ColonyAnimals)
				{
					if (flag)
					{
						this.SelectThing(current, false);
						return;
					}
					if (current == selector.SingleSelectedThing)
					{
						flag = true;
					}
				}
				this.SelectThing(this.ColonyAnimals.FirstOrDefault<Pawn>(), false);
			}
		}

		public void SelectPreviousColonyAnimal()
		{
			Selector selector = Find.Selector;
			if (selector.SingleSelectedThing == null || !(selector.SingleSelectedThing is Pawn) || selector.SingleSelectedThing.Faction != Faction.OfColony || (selector.SingleSelectedThing as Pawn).IsColonist)
			{
				this.SelectThing(this.ColonyAnimals.LastOrDefault<Pawn>(), false);
			}
			else
			{
				Pawn pawn = null;
				foreach (Pawn current in this.ColonyAnimals)
				{
					if (selector.SingleSelectedThing == current)
					{
						if (pawn != null)
						{
							this.SelectThing(pawn, false);
							break;
						}
						this.SelectThing(this.ColonyAnimals.LastOrDefault<Pawn>(), false);
						break;
					}
					else
					{
						pawn = current;
					}
				}
			}
		}
	}
}
