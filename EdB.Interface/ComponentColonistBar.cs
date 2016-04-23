using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace EdB.Interface
{
	public class ComponentColonistBar : MapComponent
	{
		private bool loadedTextures = false;

		private ColonistBar colonistBar;

		private ColonistBarGroup defaultGroup = new ColonistBarGroup();

		private List<ColonistBarGroup> defaultGroups = new List<ColonistBarGroup>();

		private int width;

		private int height;

		private bool initialized = false;

		public List<ColonistBarGroup> DefaultGroups
		{
			get
			{
				return this.defaultGroups;
			}
		}

		public ColonistBarGroup DefaultGroup
		{
			get
			{
				return this.defaultGroup;
			}
		}

		public string Name
		{
			get
			{
				return "ColonistBar";
			}
		}

		public ColonistBar ColonistBar
		{
			get
			{
				return this.colonistBar;
			}
		}

		public bool RenderWithScreenshots
		{
			get
			{
				return true;
			}
		}

		public override void ExposeData()
		{
		}

		public void Initialize()
		{
			ColonistTracker.Instance.Reset();
			this.defaultGroups.Add(this.defaultGroup);
			this.colonistBar = new ColonistBar();
			this.colonistBar.AddGroup(this.defaultGroup);
			this.colonistBar.CurrentGroup = this.defaultGroup;
			this.width = Screen.width;
			this.height = Screen.height;
			ColonistTracker.Instance.ColonistChanged += new ColonistNotificationHandler(this.ColonistNotificationHandler);
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				this.colonistBar.Drawer.ResetTextures();
				this.colonistBar.Drawer.enabled = true;
				this.loadedTextures = true;
			});
		}

		public override void MapComponentUpdate()
		{
			if (!this.initialized)
			{
				this.initialized = true;
				this.Initialize();
			}
			if (this.initialized && this.loadedTextures)
			{
				ColonistTracker.Instance.Update();
				if (this.width != Screen.width || this.height != Screen.height)
				{
					this.width = Screen.width;
					this.height = Screen.height;
					this.colonistBar.UpdateScreenSize(this.width, this.height);
				}
			}
		}

		public override void MapComponentOnGUI()
		{
			if (this.initialized && this.loadedTextures)
			{
				this.colonistBar.Draw();
			}
		}

		public void ColonistNotificationHandler(ColonistNotification notification)
		{
			if (notification.type == ColonistNotificationType.New)
			{
				this.defaultGroup.Add(notification.colonist);
			}
			else if (notification.type == ColonistNotificationType.Buried || notification.type == ColonistNotificationType.Lost || notification.type == ColonistNotificationType.Deleted)
			{
				this.defaultGroup.Remove(notification.colonist);
			}
		}
	}
}
