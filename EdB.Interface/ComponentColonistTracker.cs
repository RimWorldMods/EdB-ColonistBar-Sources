using System;

namespace EdB.Interface
{
	public class ComponentColonistTracker
	{
		public string Name
		{
			get
			{
				return "ColonistTracker";
			}
		}

		public ComponentColonistTracker()
		{
			ColonistTracker.Instance.Reset();
		}

		public void Initialize()
		{
			ColonistTracker.Instance.InitializeWithDefaultColonists();
		}

		public void Update()
		{
			ColonistTracker.Instance.Update();
		}
	}
}
