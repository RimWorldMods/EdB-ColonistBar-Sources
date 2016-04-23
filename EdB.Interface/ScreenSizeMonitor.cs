using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EdB.Interface
{
	public class ScreenSizeMonitor
	{
		public delegate void ScreenSizeChangeHandler(int width, int height);

		protected int width;

		protected int height;

		public event ScreenSizeMonitor.ScreenSizeChangeHandler Changed;

		public int Width
		{
			get
			{
				return this.width;
			}
		}

		public int Height
		{
			get
			{
				return this.height;
			}
		}

		public ScreenSizeMonitor()
		{
			this.width = Screen.width;
			this.height = Screen.height;
		}

		public void Update()
		{
			int num = Screen.width;
			int num2 = Screen.height;
			if (num != this.width || num2 != this.height)
			{
				this.width = num;
				this.height = num2;
				if (this.Changed != null)
				{
					this.Changed(num, num2);
				}
			}
		}
	}
}
