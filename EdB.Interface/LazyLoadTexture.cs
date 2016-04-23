using System;
using UnityEngine;
using Verse;

namespace EdB.Interface
{
	public class LazyLoadTexture
	{
		private string resource;

		private Texture2D texture;

		private bool loaded = false;

		public Texture2D Texture
		{
			get
			{
				if (!this.loaded)
				{
					this.Load();
				}
				return this.texture;
			}
		}

		public LazyLoadTexture(string resource)
		{
			this.resource = resource;
		}

		public Texture2D Load()
		{
			this.loaded = true;
			this.texture = ContentFinder<Texture2D>.Get(this.resource, true);
			if (this.texture == null)
			{
				this.texture = BaseContent.BadTex;
			}
			return this.texture;
		}

		public void Draw(Rect rect)
		{
			Texture2D texture2D = this.Texture;
			if (texture2D != null)
			{
				GUI.DrawTexture(rect, texture2D);
			}
		}
	}
}
