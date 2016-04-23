using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace EdB.Interface
{
	public class Button
	{
		protected static Texture2D ButtonBGAtlas;

		protected static readonly Texture2D ButtonBGAtlasMouseover = ContentFinder<Texture2D>.Get("UI/Widgets/ButtonBGMouseover", true);

		protected static readonly Texture2D ButtonBGAtlasClick = ContentFinder<Texture2D>.Get("UI/Widgets/ButtonBGClick", true);

		protected static Color InactiveButtonColor = new Color(1f, 1f, 1f, 0.5f);

		protected static readonly Color MouseoverOptionColor = Color.yellow;

		public static void ResetTextures()
		{
			Button.ButtonBGAtlas = ContentFinder<Texture2D>.Get("EdB/Interface/TextButton", true);
		}

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
				Texture2D atlas = Button.ButtonBGAtlas;
				if (enabled)
				{
					if (rect.Contains(Event.current.mousePosition))
					{
						atlas = Button.ButtonBGAtlasMouseover;
						if (Input.GetMouseButton(0))
						{
							atlas = Button.ButtonBGAtlasClick;
						}
					}
				}
				Widgets.DrawAtlas(rect, atlas);
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
				Texture2D atlas = Button.ButtonBGAtlas;
				if (enabled && rect.Contains(Event.current.mousePosition))
				{
					atlas = Button.ButtonBGAtlasMouseover;
					if (Input.GetMouseButton(0))
					{
						atlas = Button.ButtonBGAtlasClick;
					}
				}
				Widgets.DrawAtlas(rect, atlas);
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
