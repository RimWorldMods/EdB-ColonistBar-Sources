using System;

namespace EdB.Interface
{
	public class PreferenceSmallIcons : BooleanPreference
	{
		public override string Name
		{
			get
			{
				return "EdB.ColonistBar.Prefs.SmallIcons";
			}
		}

		public override string Tooltip
		{
			get
			{
				return "EdB.ColonistBar.Prefs.SmallIcons.Tip";
			}
		}

		public override string Group
		{
			get
			{
				return "EdB.ColonistBar.Prefs";
			}
		}

		public override bool DefaultValue
		{
			get
			{
				return false;
			}
		}

		public override bool Indent
		{
			get
			{
				return false;
			}
		}
	}
}
