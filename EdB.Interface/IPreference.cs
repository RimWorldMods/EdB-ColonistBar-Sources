using System;

namespace EdB.Interface
{
	public interface IPreference
	{
		string Name
		{
			get;
		}

		string Group
		{
			get;
		}

		string ValueForSerialization
		{
			get;
			set;
		}

		bool DisplayInOptions
		{
			get;
		}

		void OnGUI(float positionX, ref float positionY, float width);
	}
}
