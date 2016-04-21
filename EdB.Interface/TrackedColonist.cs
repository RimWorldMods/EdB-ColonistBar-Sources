using RimWorld;
using System;
using Verse;

namespace EdB.Interface
{
	public class TrackedColonist
	{
		private Pawn pawn;

		private bool dead;

		private bool missing;

		private bool cryptosleep;

		private int missingTimestamp;

		private Corpse corpse;

		private Faction capturingFaction;

		public Pawn Pawn
		{
			get
			{
				return this.pawn;
			}
			set
			{
				this.pawn = value;
			}
		}

		public bool Dead
		{
			get
			{
				return this.dead;
			}
			set
			{
				this.dead = value;
			}
		}

		public bool Captured
		{
			get
			{
				return this.capturingFaction != null;
			}
		}

		public bool Cryptosleep
		{
			get
			{
				return this.cryptosleep;
			}
			set
			{
				this.cryptosleep = value;
			}
		}

		public Faction CapturingFaction
		{
			get
			{
				return this.capturingFaction;
			}
			set
			{
				this.capturingFaction = value;
			}
		}

		public int MissingTimestamp
		{
			get
			{
				return this.missingTimestamp;
			}
			set
			{
				this.missingTimestamp = value;
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

		public Pawn Carrier
		{
			get
			{
				Pawn result;
				if (this.pawn.holder != null && this.pawn.holder.owner != null)
				{
					Pawn_CarryTracker pawn_CarryTracker = this.pawn.holder.owner as Pawn_CarryTracker;
					if (pawn_CarryTracker != null)
					{
						result = pawn_CarryTracker.pawn;
						return result;
					}
				}
				result = null;
				return result;
			}
		}

		public bool Drafted
		{
			get
			{
				return !this.dead && this.pawn.Drafted;
			}
		}

		public MentalStateDef MentalState
		{
			get
			{
				MentalStateDef result;
				if (this.pawn.get_InMentalState())
				{
					result = this.pawn.get_MentalStateDef();
				}
				else
				{
					result = null;
				}
				return result;
			}
		}

		public bool InMentalState
		{
			get
			{
				return this.MentalState != null;
			}
		}

		public int MentalBreakWarningLevel
		{
			get
			{
				int result;
				if (this.pawn.mindState != null && this.pawn.mindState.mentalStateStarter != null && !this.pawn.Downed && !this.pawn.Dead)
				{
					if (this.pawn.mindState.mentalStateStarter.get_HardMentalStateImminent())
					{
						result = 2;
						return result;
					}
					if (this.pawn.mindState.mentalStateStarter.get_MentalStateApproaching())
					{
						result = 1;
						return result;
					}
				}
				result = 0;
				return result;
			}
		}

		public float HealthPercent
		{
			get
			{
				float result;
				if (this.pawn.health != null && this.pawn.health.summaryHealth != null)
				{
					result = this.pawn.health.summaryHealth.SummaryHealthPercent;
				}
				else
				{
					result = 0f;
				}
				return result;
			}
		}

		public bool Incapacitated
		{
			get
			{
				return this.pawn.health != null && this.pawn.health.Downed;
			}
		}

		public bool Controllable
		{
			get
			{
				return !this.Missing && !this.Dead && !this.Captured && !this.Incapacitated && !this.InMentalState && !this.Cryptosleep;
			}
		}

		public TrackedColonist()
		{
		}

		public TrackedColonist(Pawn pawn)
		{
			this.pawn = pawn;
			this.dead = false;
			this.missing = false;
			this.missingTimestamp = 0;
			this.corpse = null;
			this.capturingFaction = null;
			this.cryptosleep = false;
		}
	}
}
