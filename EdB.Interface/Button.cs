using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace EdB.Interface
{
	public class Button
	{
		public static LazyLoadTexture ButtonBGAtlas = new LazyLoadTexture("EdB/Interface/TextButton");

		public static LazyLoadTexture ButtonBGAtlasMouseover = new LazyLoadTexture("UI/Widgets/ButtonBGMouseover");

		public static LazyLoadTexture ButtonBGAtlasClick = new LazyLoadTexture("UI/Widgets/ButtonBGClick");

		protected static Color InactiveButtonColor = new Color(1f, 1f, 1f, 0.5f);

		protected static readonly Color MouseoverOptionColor = Color.yellow;

		public static bool ImageButton(Rect butRect, Texture2D tex, Color baseColor, Color mouseOverColor)
		{
			if (butRect.Contains(Event.current.mousePosition))
			{
				GUI.color = mouseOverColor;
			}
			GUI.DrawTexture(butRect, tex);
			GUI.color = baseColor;
			return Widgets.InvisibleButton(butRect);
		}

		public static bool IconButton(Rect rect, Texture texture, Color baseColor, Color highlightColor, bool enabled)
		{
			bool result;
			if (texture == null)
			{
				result = false;
			}
			else
			{
				if (!enabled)
				{
					GUI.color = Button.InactiveButtonColor;
				}
				else
				{
					GUI.color = Color.white;
				}
				Texture2D texture2 = Button.ButtonBGAtlas.Texture;
				if (enabled)
				{
					if (rect.Contains(Event.current.mousePosition))
					{
						texture2 = Button.ButtonBGAtlasMouseover.Texture;
						if (Input.GetMouseButton(0))
						{
							texture2 = Button.ButtonBGAtlasClick.Texture;
						}
					}
				}
				Widgets.DrawAtlas(rect, texture2);
				Rect position = new Rect(rect.x + rect.width / 2f - (float)(texture.width / 2), rect.y + rect.height / 2f - (float)(texture.height / 2), (float)texture.width, (float)texture.height);
				if (!enabled)
				{
					GUI.color = Button.InactiveButtonColor;
				}
				else
				{
					GUI.color = baseColor;
				}
				if (enabled && rect.Contains(Event.current.mousePosition))
				{
					GUI.color = highlightColor;
				}
				GUI.DrawTexture(position, texture);
				GUI.color = Color.white;
				result = (enabled && Widgets.InvisibleButton(rect));
			}
			return result;
		}

		public static bool TextButton(Rect rect, string label, bool drawBackground, bool doMouseoverSound, bool enabled)
		{
			TextAnchor anchor = Text.Anchor;
			Color color = GUI.color;
			GUI.color = (enabled ? Color.white : Button.InactiveButtonColor);
			if (drawBackground)
			{
				Texture2D texture = Button.ButtonBGAtlas.Texture;
				if (enabled && rect.Contains(Event.current.mousePosition))
				{
					texture = Button.ButtonBGAtlasMouseover.Texture;
					if (Input.GetMouseButton(0))
					{
						texture = Button.ButtonBGAtlasClick.Texture;
					}
				}
				Widgets.DrawAtlas(rect, texture);
			}
			if (doMouseoverSound)
			{
				MouseoverSounds.DoRegion(rect);
			}
			if (!drawBackground)
			{
				if (enabled && rect.Contains(Event.current.mousePosition))
				{
					GUI.color = Button.MouseoverOptionColor;
				}
			}
			if (drawBackground)
			{
				Text.Anchor = TextAnchor.MiddleCenter;
			}
			else
			{
				Text.Anchor = TextAnchor.MiddleLeft;
			}
			Widgets.Label(rect, label);
			Text.Anchor = anchor;
			GUI.color = color;
			return enabled && Widgets.InvisibleButton(rect);
		}

		public static bool ImageButton(Rect butRect, Texture2D tex)
		{
			return Button.ImageButton(butRect, tex, GenUI.MouseoverColor);
		}

		public static bool ImageButton(Rect butRect, Texture2D tex, Color highlightColor)
		{
			Color color = GUI.color;
			if (butRect.Contains(Event.current.mousePosition))
			{
				GUI.color = highlightColor;
			}
			GUI.DrawTexture(butRect, tex);
			GUI.color = color;
			return Widgets.InvisibleButton(butRect);
		}
	}
}
