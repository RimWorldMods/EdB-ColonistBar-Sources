using System;

namespace EdB.Interface
{
	public class PreferenceEnabled : BooleanPreference
	{
		public override string Name
		{
			get
			{
				return "EdB.ColonistBar.Prefs.Enable";
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
				return true;
			}
		}
	}
}
