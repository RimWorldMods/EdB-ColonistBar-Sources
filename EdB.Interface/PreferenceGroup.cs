using System;
using System.Collections.Generic;

namespace EdB.Interface
{
	public class PreferenceGroup
	{
		private string name;

		private List<IPreference> preferences = new List<IPreference>();

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

		public IEnumerable<IPreference> Preferences
		{
			get
			{
				return this.preferences;
			}
		}

		public int PreferenceCount
		{
			get
			{
				return this.preferences.Count;
			}
		}

		public PreferenceGroup(string name)
		{
			this.name = name;
		}

		public void Add(IPreference preference)
		{
			this.preferences.Add(preference);
		}
	}
}
