using System;
using Verse;

namespace EdB.Interface
{
	public class ColonistNotification
	{
		public TrackedColonist colonist;

		public ColonistNotificationType type;

		public Pawn relatedPawn;

		public ColonistNotification(ColonistNotificationType type, TrackedColonist colonist)
		{
			this.type = type;
			this.colonist = colonist;
			this.relatedPawn = null;
		}

		public ColonistNotification(ColonistNotificationType type, TrackedColonist colonist, Pawn relatedPawn)
		{
			this.type = type;
			this.colonist = colonist;
			this.relatedPawn = relatedPawn;
		}

		public override string ToString()
		{
			NameTriple nameTriple = this.colonist.Pawn.Name as NameTriple;
			return string.Concat(new object[]
			{
				"ColonistNotification, ",
				this.type,
				": ",
				nameTriple.Nick
			});
		}
	}
}
