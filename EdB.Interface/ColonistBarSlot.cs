using RimWorld;
using System;
using Verse;

namespace EdB.Interface
{
	public class ColonistBarSlot
	{
		public Pawn pawn;

		public Corpse corpse = null;

		public bool missing = false;

		public bool incapacitated = false;

		public bool dead = false;

		public MentalStateDef sanity = null;

		public int psychologyLevel = 0;

		public bool kidnapped = false;

		public float health = 0f;

		public bool drafted = false;

		protected float missingTime = 0f;

		protected SelectorUtility pawnSelector = new SelectorUtility();

		protected bool remove = false;

		public Pawn Pawn
		{
			get
			{
				return this.pawn;
			}
		}

		public Corpse Corpse
		{
			get
			{
				return this.corpse;
			}
			set
			{
				this.corpse = value;
			}
		}

		public bool Missing
		{
			get
			{
				return this.missing;
			}
			set
			{
				this.missing = value;
			}
		}

		public float MissingTime
		{
			get
			{
				return this.missingTime;
			}
			set
			{
				this.missingTime = value;
			}
		}

		public bool Remove
		{
			get
			{
				return this.remove;
			}
			set
			{
				this.remove = value;
			}
		}

		public ColonistBarSlot(Pawn pawn)
		{
			this.pawn = pawn;
		}

		public void Update()
		{
			if (this.pawn != null)
			{
				this.incapacitated = false;
				if (this.pawn.health != null)
				{
					this.health = this.pawn.health.summaryHealth.SummaryHealthPercent;
					this.incapacitated = this.pawn.health.Downed;
				}
				else
				{
					this.health = 0f;
				}
				this.kidnapped = false;
				if (this.pawn.holder != null)
				{
					if (this.pawn.Destroyed)
					{
						this.missing = true;
					}
					else if (this.pawn.holder.owner != null)
					{
						Pawn_CarryTracker pawn_CarryTracker = this.pawn.holder.owner as Pawn_CarryTracker;
						if (pawn_CarryTracker != null && pawn_CarryTracker.pawn != null && pawn_CarryTracker.pawn.Faction != null)
						{
							if (pawn_CarryTracker.pawn.Faction != Faction.OfColony && pawn_CarryTracker.pawn.Faction.RelationWith(Faction.OfColony, false).hostile)
							{
								this.kidnapped = true;
							}
						}
					}
				}
				this.dead = this.pawn.Dead;
				if (this.dead)
				{
					if (this.WasReplaced(this.pawn))
					{
						this.dead = false;
					}
				}
				this.sanity = null;
				if (this.pawn.mindState != null && this.pawn.InMentalState)
				{
					this.sanity = this.pawn.MentalStateDef;
				}
				this.drafted = (!this.dead && this.pawn.Drafted);
				this.psychologyLevel = 0;
				if (this.pawn.mindState != null && this.pawn.mindState.mentalStateStarter != null && !this.pawn.Downed && !this.pawn.Dead)
				{
					if (this.pawn.mindState.mentalStateStarter.HardMentalStateImminent)
					{
						this.psychologyLevel = 2;
					}
					else if (this.pawn.mindState.mentalStateStarter.MentalStateApproaching)
					{
						this.psychologyLevel = 1;
					}
				}
			}
		}

		protected bool WasReplaced(Pawn pawn)
		{
			bool result;
			foreach (Pawn current in Find.MapPawns.FreeColonists)
			{
				if (current.GetUniqueLoadID() == pawn.GetUniqueLoadID())
				{
					result = true;
					return result;
				}
			}
			result = false;
			return result;
		}

		public Pawn FindCarrier()
		{
			Pawn result;
			if (this.pawn.holder != null && this.pawn.holder.owner != null)
			{
				Pawn_CarryTracker pawn_CarryTracker = this.pawn.holder.owner as Pawn_CarryTracker;
				if (pawn_CarryTracker != null && pawn_CarryTracker.pawn != null)
				{
					result = pawn_CarryTracker.pawn;
					return result;
				}
			}
			result = null;
			return result;
		}
	}
}
