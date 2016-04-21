using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace EdB.Interface
{
	public class ColonistBarDrawer : MonoBehaviour
	{
		public static Material SlotBackgroundMat = null;

		public static Material SlotBordersMat = null;

		public static Material SlotSelectedMat = null;

		public static Material SlotBackgroundMatLarge = null;

		public static Material SlotBordersMatLarge = null;

		public static Material SlotSelectedMatLarge = null;

		public static Material SlotBackgroundMatSmall = null;

		public static Material SlotBordersMatSmall = null;

		public static Material SlotSelectedMatSmall = null;

		public static LazyLoadTexture UnhappyTex = new LazyLoadTexture("Things/Pawn/Effects/Unhappy");

		public static LazyLoadTexture MentalBreakImminentTex = new LazyLoadTexture("Things/Pawn/Effects/MentalStateImminent");

		public static LazyLoadTexture ToggleButton = new LazyLoadTexture("EdB/Interface/ColonistBar/ToggleBar");

		public static Vector2 StartingPosition = new Vector2(640f, 16f);

		public static Vector2 SlotSize;

		public static Vector2 SlotPadding;

		public static Vector2 BackgroundSize;

		public static Vector2 BackgroundOffset;

		public static Vector2 PortraitOffset;

		public static Vector2 PortraitSize;

		public static Vector2 BodySize;

		public static Vector2 BodyOffset;

		public static Vector2 HeadSize;

		public static Vector2 HeadOffset;

		public static Vector2 MentalHealthOffset;

		public static Vector2 MentalHealthSize;

		public static Vector2 HealthOffset;

		public static Vector2 HealthSize;

		public static Vector2 MaxLabelSize;

		public static Vector2 SlotSizeLarge = new Vector2(62f, 56f);

		public static Vector2 NormalSlotPaddingLarge = new Vector2(16f, 24f);

		public static Vector2 WideSlotPaddingLarge = new Vector2(36f, 24f);

		public static Vector2 BackgroundSizeLarge = new Vector2(64f, 64f);

		public static Vector2 BackgroundOffsetLarge = new Vector2(0f, 0f);

		public static Vector2 PortraitOffsetLarge = new Vector2(5f, 5f);

		public static Vector2 PortraitSizeLarge = new Vector2(46f, 46f);

		public static Vector2 BodySizeLarge = new Vector2(76f, 76f);

		public static Vector2 BodyOffsetLarge = new Vector2(-9f, -11f);

		public static Vector2 HeadSizeLarge = new Vector2(76f, 76f);

		public static Vector2 HeadOffsetLarge = new Vector2(-9f, -26f);

		public static Vector2 MentalHealthOffsetLarge = new Vector2(-2f, -22f);

		public static Vector2 MentalHealthSizeLarge = new Vector2(76f, 76f);

		public static Vector2 HealthOffsetLarge = new Vector2(52f, 6f);

		public static Vector2 HealthSizeLarge = new Vector2(5f, 46f);

		public static Vector2 SlotSizeSmall = new Vector2(56f, 38f);

		public static Vector2 NormalSlotPaddingSmall = new Vector2(12f, 24f);

		public static Vector2 WideSlotPaddingSmall = new Vector2(36f, 24f);

		public static Vector2 BackgroundSizeSmall = new Vector2(40f, 36f);

		public static Vector2 BackgroundOffsetSmall = new Vector2(7f, 0f);

		public static Vector2 PortraitOffsetSmall = new Vector2(11f, 3f);

		public static Vector2 PortraitSizeSmall = new Vector2(28f, 28f);

		public static Vector2 BodySizeSmall = new Vector2(50f, 50f);

		public static Vector2 BodyOffsetSmall = new Vector2(1f, -7f);

		public static Vector2 HeadSizeSmall = new Vector2(50f, 50f);

		public static Vector2 HeadOffsetSmall = new Vector2(1f, -20f);

		public static Vector2 MentalHealthOffsetSmall = new Vector2(7f, -17f);

		public static Vector2 MentalHealthSizeSmall = new Vector2(52f, 52f);

		public static Vector2 HealthOffsetSmall = new Vector2(41f, 4f);

		public static Vector2 HealthSizeSmall = new Vector2(3f, 28f);

		public static Color ColorBroken = new Color(0.65f, 0.9f, 0.93f);

		public static Color ColorPsycho = new Color(0.9f, 0.2f, 0.5f);

		public static Color ColorDead = new Color(0.5f, 0.5f, 0.5f, 1f);

		public static Color ColorFrozen = new Color(0.7f, 0.7f, 0.9f, 1f);

		public static Color ColorNameUnderlay = new Color(0f, 0f, 0f, 0.6f);

		protected Mesh backgroundMesh;

		protected Mesh bodyMesh;

		protected Mesh headMesh;

		protected MaterialPropertyBlock deadPropertyBlock = new MaterialPropertyBlock();

		protected MaterialPropertyBlock cryptosleepPropertyBlock = new MaterialPropertyBlock();

		protected SelectorUtility pawnSelector = new SelectorUtility();

		protected bool smallColonistIcons = false;

		protected float doubleClickTime = -1f;

		protected new Camera camera;

		protected bool visible = true;

		protected Dictionary<Material, Material> deadMaterials = new Dictionary<Material, Material>();

		protected Dictionary<Material, Material> cryptosleepMaterials = new Dictionary<Material, Material>();

		protected List<TrackedColonist> slots;

		public List<TrackedColonist> Slots
		{
			get
			{
				return this.slots;
			}
			set
			{
				this.slots = value;
			}
		}

		public bool Visible
		{
			get
			{
				return this.visible;
			}
			set
			{
				this.visible = value;
			}
		}

		public bool SmallColonistIcons
		{
			get
			{
				return this.smallColonistIcons;
			}
		}

		public void ResetTextures()
		{
			ColonistBarDrawer.SlotBackgroundMatLarge = MaterialPool.MatFrom("EdB/Interface/ColonistBar/PortraitBackgroundLarge");
			ColonistBarDrawer.SlotBackgroundMatLarge.mainTexture.filterMode = FilterMode.Point;
			ColonistBarDrawer.SlotBordersMatLarge = MaterialPool.MatFrom("EdB/Interface/ColonistBar/PortraitBordersLarge");
			ColonistBarDrawer.SlotBordersMatLarge.mainTexture.filterMode = FilterMode.Point;
			ColonistBarDrawer.SlotSelectedMatLarge = MaterialPool.MatFrom("EdB/Interface/ColonistBar/PortraitSelectedLarge");
			ColonistBarDrawer.SlotSelectedMatLarge.mainTexture.filterMode = FilterMode.Point;
			ColonistBarDrawer.SlotBackgroundMatSmall = MaterialPool.MatFrom("EdB/Interface/ColonistBar/PortraitBackgroundSmall");
			ColonistBarDrawer.SlotBackgroundMatSmall.mainTexture.filterMode = FilterMode.Point;
			ColonistBarDrawer.SlotBordersMatSmall = MaterialPool.MatFrom("EdB/Interface/ColonistBar/PortraitBordersSmall");
			ColonistBarDrawer.SlotBordersMatSmall.mainTexture.filterMode = FilterMode.Point;
			ColonistBarDrawer.SlotSelectedMatSmall = MaterialPool.MatFrom("EdB/Interface/ColonistBar/PortraitSelectedSmall");
			ColonistBarDrawer.SlotSelectedMatSmall.mainTexture.filterMode = FilterMode.Point;
		}

		public void Draw()
		{
			if (this.visible)
			{
				this.camera.Render();
			}
		}

		public void SizeCamera(int width, int height)
		{
			float num = (float)width * 0.5f;
			float num2 = (float)height * 0.5f;
			this.camera.orthographicSize = num2;
			this.camera.transform.position = new Vector3(num, num2, 100f);
			this.camera.transform.LookAt(new Vector3(num, num2, 0f), new Vector3(0f, -1f, 0f));
			this.camera.aspect = num / num2;
		}

		public void Start()
		{
			this.camera = base.gameObject.AddComponent<Camera>();
			this.camera.orthographic = true;
			this.camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
			this.SizeCamera(Screen.width, Screen.height);
			this.camera.clearFlags = CameraClearFlags.Depth;
			this.camera.nearClipPlane = 1f;
			this.camera.farClipPlane = 200f;
			this.camera.depth = -1f;
			this.camera.enabled = false;
			this.UseLargeIcons();
			this.deadPropertyBlock = new MaterialPropertyBlock();
			this.deadPropertyBlock.Clear();
			this.deadPropertyBlock.AddColor(Shader.PropertyToID("_Color"), ColonistBarDrawer.ColorDead);
			this.cryptosleepPropertyBlock = new MaterialPropertyBlock();
			this.cryptosleepPropertyBlock.Clear();
			this.cryptosleepPropertyBlock.AddColor(Shader.PropertyToID("_Color"), ColonistBarDrawer.ColorFrozen);
		}

		public void OnGUI()
		{
			if (this.slots != null && this.slots.Count != 0)
			{
				Vector2 startingPosition = ColonistBarDrawer.StartingPosition;
				float num = (float)Screen.width;
				foreach (TrackedColonist current in this.slots)
				{
					this.RenderSlot(current, startingPosition);
					startingPosition.x += ColonistBarDrawer.SlotSize.x + ColonistBarDrawer.SlotPadding.x;
					if (startingPosition.x + ColonistBarDrawer.SlotSize.x + ColonistBarDrawer.SlotPadding.x > num)
					{
						startingPosition.y += ColonistBarDrawer.SlotSize.y + ColonistBarDrawer.SlotPadding.y;
						startingPosition.x = ColonistBarDrawer.StartingPosition.x;
					}
				}
			}
		}

		public void DrawTexturesForSlots()
		{
			if (this.visible && this.slots != null && this.slots.Count != 0)
			{
				Vector2 startingPosition = ColonistBarDrawer.StartingPosition;
				float num = (float)Screen.width;
				foreach (TrackedColonist current in this.slots)
				{
					this.DrawTextureForSlot(current, startingPosition);
					startingPosition.x += ColonistBarDrawer.SlotSize.x + ColonistBarDrawer.SlotPadding.x;
					if (startingPosition.x + ColonistBarDrawer.SlotSize.x + ColonistBarDrawer.SlotPadding.x > num)
					{
						startingPosition.y += ColonistBarDrawer.SlotSize.y + ColonistBarDrawer.SlotPadding.y;
						startingPosition.x = ColonistBarDrawer.StartingPosition.x;
					}
				}
			}
		}

		public void DrawToggleButton()
		{
			if (!this.visible)
			{
				Texture2D texture = ColonistBarDrawer.ToggleButton.Texture;
				Rect rect = new Rect((float)(Screen.width - texture.width - 16), ColonistBarDrawer.StartingPosition.y + 4f, (float)texture.width, (float)texture.height);
				ColonistBarDrawer.ToggleButton.Draw(rect);
				if (Widgets.InvisibleButton(rect))
				{
					SoundDefOf.TickTiny.PlayOneShotOnCamera();
					this.visible = true;
				}
			}
		}

		protected void RenderSlot(TrackedColonist slot, Vector2 position)
		{
			if (Event.current.type == EventType.Repaint)
			{
				Rot4 south = Rot4.South;
				Pawn pawn = slot.Pawn;
				PawnGraphicSet graphics = pawn.get_Drawer().renderer.graphics;
				if (!graphics.AllResolved)
				{
					graphics.ResolveAllGraphics();
				}
				bool flag = slot.Dead || slot.Missing;
				bool cryptosleep = slot.Cryptosleep;
				Quaternion identity = Quaternion.identity;
				Vector3 one = Vector3.one;
				Graphics.DrawMesh(this.backgroundMesh, Matrix4x4.TRS(new Vector3(position.x + ColonistBarDrawer.BackgroundOffset.x, position.y + ColonistBarDrawer.BackgroundOffset.y, 0f), identity, one), ColonistBarDrawer.SlotBackgroundMat, 1, this.camera, 0, null);
				MaterialPropertyBlock properties = null;
				if (flag)
				{
					properties = this.deadPropertyBlock;
				}
				else if (slot.Cryptosleep)
				{
					properties = this.cryptosleepPropertyBlock;
				}
				float num = 1f;
				Material material;
				foreach (Material current in graphics.MatsBodyBaseAt(south, RotDrawMode.Fresh))
				{
					material = current;
					if (flag)
					{
						material = this.GetDeadMaterial(current);
					}
					else if (cryptosleep)
					{
						material = this.GetFrozenMaterial(current);
					}
					Graphics.DrawMesh(this.bodyMesh, Matrix4x4.TRS(new Vector3(position.x + ColonistBarDrawer.PortraitOffset.x, position.y + ColonistBarDrawer.PortraitOffset.y, num), identity, one), material, 1, this.camera, 0, properties);
					num += 1f;
				}
				Material material2;
				for (int i = 0; i < graphics.apparelGraphics.Count; i++)
				{
					ApparelGraphicRecord apparelGraphicRecord = graphics.apparelGraphics[i];
					if (apparelGraphicRecord.sourceApparel.def.apparel.LastLayer == ApparelLayer.Shell)
					{
						material2 = apparelGraphicRecord.graphic.MatAt(south, null);
						material2 = graphics.flasher.GetDamagedMat(material2);
						material = material2;
						if (flag)
						{
							material = this.GetDeadMaterial(material2);
						}
						else if (cryptosleep)
						{
							material = this.GetFrozenMaterial(material2);
						}
						Graphics.DrawMesh(this.bodyMesh, Matrix4x4.TRS(new Vector3(position.x + ColonistBarDrawer.PortraitOffset.x, position.y + ColonistBarDrawer.PortraitOffset.y, num), identity, one), material, 1, this.camera, 0, properties);
						num += 1f;
					}
				}
				Graphics.DrawMesh(this.backgroundMesh, Matrix4x4.TRS(new Vector3(position.x + ColonistBarDrawer.BackgroundOffset.x, position.y + ColonistBarDrawer.BackgroundOffset.y, num), identity, one), ColonistBarDrawer.SlotBordersMat, 1, this.camera);
				num += 1f;
				if (Find.Selector.IsSelected((slot.Corpse == null) ? pawn : slot.Corpse))
				{
					Graphics.DrawMesh(this.backgroundMesh, Matrix4x4.TRS(new Vector3(position.x + ColonistBarDrawer.BackgroundOffset.x, position.y + ColonistBarDrawer.BackgroundOffset.y, num), identity, one), ColonistBarDrawer.SlotSelectedMat, 1, this.camera);
					num += 1f;
				}
				material2 = pawn.get_Drawer().renderer.graphics.HeadMatAt(south, RotDrawMode.Fresh);
				material = material2;
				if (flag)
				{
					material = this.GetDeadMaterial(material2);
				}
				else if (cryptosleep)
				{
					material = this.GetFrozenMaterial(material2);
				}
				Graphics.DrawMesh(this.headMesh, Matrix4x4.TRS(new Vector3(position.x + ColonistBarDrawer.HeadOffset.x, position.y + ColonistBarDrawer.HeadOffset.y, num), identity, one), material, 1, this.camera, 0, properties);
				num += 1f;
				bool flag2 = false;
				List<ApparelGraphicRecord> apparelGraphics = graphics.apparelGraphics;
				for (int j = 0; j < apparelGraphics.Count; j++)
				{
					if (apparelGraphics[j].sourceApparel.def.apparel.LastLayer == ApparelLayer.Overhead)
					{
						flag2 = true;
						material2 = apparelGraphics[j].graphic.MatAt(south, null);
						material2 = graphics.flasher.GetDamagedMat(material2);
						material = material2;
						if (flag)
						{
							material = this.GetDeadMaterial(material2);
						}
						else if (cryptosleep)
						{
							material = this.GetFrozenMaterial(material2);
						}
						Graphics.DrawMesh(this.headMesh, Matrix4x4.TRS(new Vector3(position.x + ColonistBarDrawer.HeadOffset.x, position.y + ColonistBarDrawer.HeadOffset.y, num), identity, one), material, 1, this.camera, 0, properties);
						num += 1f;
					}
				}
				if (!flag2)
				{
					if (slot.Pawn.story.hairDef != null)
					{
						material2 = graphics.HairMatAt(south);
						material = material2;
						if (flag)
						{
							material = this.GetDeadMaterial(material2);
						}
						else if (cryptosleep)
						{
							material = this.GetFrozenMaterial(material2);
						}
						Graphics.DrawMesh(this.headMesh, Matrix4x4.TRS(new Vector3(position.x + ColonistBarDrawer.HeadOffset.x, position.y + ColonistBarDrawer.HeadOffset.y, num), identity, one), material, 1, this.camera, 0, properties);
						num += 1f;
					}
				}
			}
		}

		protected Material GetDeadMaterial(Material material)
		{
			Material material2;
			if (!this.deadMaterials.TryGetValue(material, out material2))
			{
				material2 = new Material(material);
				this.deadMaterials[material] = material2;
			}
			return material2;
		}

		protected Material GetFrozenMaterial(Material material)
		{
			Material material2;
			if (!this.cryptosleepMaterials.TryGetValue(material, out material2))
			{
				material2 = new Material(material);
				this.cryptosleepMaterials[material] = material2;
			}
			return material2;
		}

		public void SelectAllDead()
		{
			this.pawnSelector.ClearSelection();
			foreach (TrackedColonist current in this.slots)
			{
				if (current.HealthPercent == 0f && !current.Missing)
				{
					if (current.Corpse != null)
					{
						this.pawnSelector.AddToSelection(current.Corpse);
					}
				}
			}
		}

		public void SelectAllActive()
		{
			this.pawnSelector.ClearSelection();
			foreach (TrackedColonist current in this.slots)
			{
				if (current.Controllable)
				{
					this.pawnSelector.AddToSelection(current.Pawn);
				}
			}
		}

		public void SelectAllNotSane()
		{
			this.pawnSelector.ClearSelection();
			foreach (TrackedColonist current in this.slots)
			{
				if (current.InMentalState)
				{
					this.pawnSelector.AddToSelection(current.Pawn);
				}
			}
		}

		protected bool CanSelect(Pawn pawn)
		{
			return pawn.SelectableNow();
		}

		protected void DrawTextureForSlot(TrackedColonist slot, Vector2 position)
		{
			Pawn pawn = slot.Pawn;
			if (Widgets.InvisibleButton(new Rect(position.x, position.y, ColonistBarDrawer.SlotSize.x, ColonistBarDrawer.SlotSize.y)))
			{
				int button = Event.current.button;
				if (button == 2)
				{
					Pawn carrier = slot.Carrier;
					if (carrier == null)
					{
						if (slot.InMentalState)
						{
							this.SelectAllNotSane();
						}
						else if (slot.Controllable)
						{
							this.SelectAllActive();
						}
						else
						{
							this.SelectAllDead();
						}
					}
				}
				if (button == 0)
				{
					if (Time.time - this.doubleClickTime < 0.3f)
					{
						if (!pawn.Dead)
						{
							Pawn carrier = slot.Carrier;
							if (carrier == null)
							{
								if (!slot.Missing)
								{
									Find.CameraMap.JumpTo(pawn.Position);
								}
							}
							else
							{
								Find.CameraMap.JumpTo(carrier.Position);
							}
						}
						else if (slot.Corpse != null)
						{
							Find.CameraMap.JumpTo(slot.Corpse.Position);
						}
						this.doubleClickTime = -1f;
					}
					else
					{
						if (!pawn.Dead && !ThingUtility.DestroyedOrNull(pawn))
						{
							if ((Event.current.shift || Event.current.control) && Find.Selector.IsSelected(pawn))
							{
								Find.Selector.Deselect(pawn);
							}
							else
							{
								Pawn carrier = slot.Carrier;
								if (carrier == null)
								{
									if (!Event.current.alt)
									{
										if (this.CanSelect(pawn))
										{
											this.pawnSelector.SelectThing(pawn, Event.current.shift);
										}
									}
									else if (slot.InMentalState)
									{
										this.SelectAllNotSane();
									}
									else
									{
										this.SelectAllActive();
									}
								}
							}
						}
						else
						{
							if (slot.Corpse == null || slot.Missing)
							{
								this.doubleClickTime = -1f;
								return;
							}
							if (Event.current.shift && Find.Selector.IsSelected(slot.Corpse))
							{
								Find.Selector.Deselect(slot.Corpse);
							}
							else if (Event.current.alt)
							{
								this.SelectAllDead();
							}
							else
							{
								this.pawnSelector.SelectThing(slot.Corpse, Event.current.shift);
							}
						}
						if (!Event.current.shift)
						{
							this.doubleClickTime = Time.time;
						}
					}
				}
				else
				{
					this.doubleClickTime = -1f;
				}
				if (button == 1)
				{
					List<FloatMenuOption> list = new List<FloatMenuOption>();
					if (slot.Missing || slot.Corpse != null)
					{
						string label = slot.Missing ? "EdB.ColonistBar.RemoveMissingColonist".Translate() : "EdB.ColonistBar.RemoveDeadColonist".Translate();
						list.Add(new FloatMenuOption(label, delegate
						{
							ColonistTracker.Instance.StopTrackingPawn(slot.Pawn);
						}, MenuOptionPriority.Medium, null, null));
					}
					list.Add(new FloatMenuOption("EdB.ColonistBar.HideColonistBar".Translate(), delegate
					{
						this.visible = false;
					}, MenuOptionPriority.Medium, null, null));
					FloatMenu window = new FloatMenu(list, "", false, false);
					Find.WindowStack.Add(window);
				}
			}
			if (Event.current.type == EventType.Repaint)
			{
				if (!slot.Dead)
				{
					if (slot.Incapacitated)
					{
						GUI.color = new Color(0.7843f, 0f, 0f);
					}
					else if ((double)slot.HealthPercent < 0.95)
					{
						GUI.color = new Color(0.7843f, 0.7843f, 0f);
					}
					else
					{
						GUI.color = new Color(0f, 0.7843f, 0f);
					}
					if (slot.Missing)
					{
						GUI.color = new Color(0.4824f, 0.4824f, 0.4824f);
					}
					float num = ColonistBarDrawer.HealthSize.y * slot.HealthPercent;
					GUI.DrawTexture(new Rect(position.x + ColonistBarDrawer.HealthOffset.x, position.y + ColonistBarDrawer.HealthOffset.y + ColonistBarDrawer.HealthSize.y - num, ColonistBarDrawer.HealthSize.x, num), BaseContent.WhiteTex);
				}
				Vector2 vector = Text.CalcSize(pawn.LabelBaseShort);
				if (vector.x > ColonistBarDrawer.MaxLabelSize.x)
				{
					vector.x = ColonistBarDrawer.MaxLabelSize.x;
				}
				vector.x += 4f;
				GUI.color = ColonistBarDrawer.ColorNameUnderlay;
				GUI.DrawTexture(new Rect(position.x + ColonistBarDrawer.SlotSize.x / 2f - vector.x / 2f, position.y + ColonistBarDrawer.PortraitSize.y, vector.x, 12f), BaseContent.BlackTex);
				Text.Font = GameFont.Tiny;
				GUI.skin.label.alignment = TextAnchor.UpperCenter;
				Text.Anchor = TextAnchor.UpperCenter;
				Color color = Color.white;
				MentalStateDef mentalState = slot.MentalState;
				if (mentalState != null)
				{
					color = mentalState.nameColor;
				}
				GUI.color = color;
				Widgets.Label(new Rect(position.x + ColonistBarDrawer.SlotSize.x / 2f - vector.x / 2f, position.y + ColonistBarDrawer.PortraitSize.y - 2f, vector.x, 20f), pawn.LabelBaseShort);
				if (slot.Drafted)
				{
					vector.x -= 4f;
					GUI.DrawTexture(new Rect(position.x + ColonistBarDrawer.SlotSize.x / 2f - vector.x / 2f, position.y + ColonistBarDrawer.PortraitSize.y + 11f, vector.x, 1f), BaseContent.WhiteTex);
				}
				Text.Anchor = TextAnchor.UpperLeft;
				string text = null;
				if (slot.Missing)
				{
					text = "EdB.ColonistBar.Status.MISSING".Translate();
				}
				else if (slot.Corpse != null)
				{
					text = "EdB.ColonistBar.Status.DEAD".Translate();
				}
				else if (slot.Captured)
				{
					text = "EdB.ColonistBar.Status.KIDNAPPED".Translate();
				}
				else if (slot.Cryptosleep)
				{
					text = "EdB.ColonistBar.Status.CRYPTOSLEEP".Translate();
				}
				else if (mentalState != null)
				{
					if (mentalState == MentalStateDefOf.Berserk)
					{
						text = "EdB.ColonistBar.Status.RAMPAGE".Translate();
					}
					else if (mentalState == MentalStateDefOf.BingingAlcohol)
					{
						text = "EdB.ColonistBar.Status.BINGING".Translate();
					}
					else if (mentalState == MentalStateDefOf.SocialFighting)
					{
						text = "EdB.ColonistBar.Status.FIGHT".Translate();
					}
					else if (mentalState == MentalStateDefOf.PanicFlee)
					{
						text = "EdB.ColonistBar.Status.PANIC".Translate();
					}
					else if (mentalState.defName == "ConfusedWander")
					{
						text = "EdB.ColonistBar.Status.DAZED".Translate();
					}
					else if (mentalState == MentalStateDefOf.DazedWander)
					{
						text = "EdB.ColonistBar.Status.DAZED".Translate();
					}
					else
					{
						text = "EdB.ColonistBar.Status.BROKEN".Translate();
					}
				}
				if (text != null)
				{
					Vector2 vector2 = Text.CalcSize(text);
					vector2.x += 4f;
					GUI.color = new Color(0f, 0f, 0f, 0.4f);
					GUI.DrawTexture(new Rect(position.x + ColonistBarDrawer.SlotSize.x / 2f - vector2.x / 2f, position.y + ColonistBarDrawer.PortraitSize.y + 12f, vector2.x, 13f), BaseContent.BlackTex);
					Text.Font = GameFont.Tiny;
					GUI.skin.label.alignment = TextAnchor.UpperCenter;
					Text.Anchor = TextAnchor.UpperCenter;
					GUI.color = color;
					Widgets.Label(new Rect(position.x + ColonistBarDrawer.SlotSize.x / 2f - vector2.x / 2f, position.y + ColonistBarDrawer.PortraitSize.y + 10f, vector2.x, 20f), text);
					Text.Anchor = TextAnchor.UpperLeft;
				}
				GUI.color = new Color(1f, 1f, 1f);
				if (!slot.Cryptosleep)
				{
					if (slot.MentalBreakWarningLevel == 2 && (double)Time.time % 1.2 < 0.4)
					{
						ColonistBarDrawer.MentalBreakImminentTex.Draw(new Rect(position.x + ColonistBarDrawer.PortraitOffset.x, position.y + ColonistBarDrawer.PortraitOffset.y, ColonistBarDrawer.MentalHealthSize.x, ColonistBarDrawer.MentalHealthSize.y));
					}
					else if (slot.MentalBreakWarningLevel == 1 && (double)Time.time % 1.2 < 0.4)
					{
						ColonistBarDrawer.UnhappyTex.Draw(new Rect(position.x + ColonistBarDrawer.MentalHealthOffset.x, position.y + ColonistBarDrawer.MentalHealthOffset.y, ColonistBarDrawer.MentalHealthSize.x, ColonistBarDrawer.MentalHealthSize.y));
					}
				}
			}
		}

		public void UseSmallIcons()
		{
			ColonistBarDrawer.SlotSize = ColonistBarDrawer.SlotSizeSmall;
			ColonistBarDrawer.BackgroundSize = ColonistBarDrawer.BackgroundSizeSmall;
			ColonistBarDrawer.BackgroundOffset = ColonistBarDrawer.BackgroundOffsetSmall;
			ColonistBarDrawer.SlotPadding = ColonistBarDrawer.NormalSlotPaddingSmall;
			ColonistBarDrawer.PortraitOffset = ColonistBarDrawer.PortraitOffsetSmall;
			ColonistBarDrawer.PortraitSize = ColonistBarDrawer.PortraitSizeSmall;
			ColonistBarDrawer.BodySize = ColonistBarDrawer.BodySizeSmall;
			ColonistBarDrawer.BodyOffset = ColonistBarDrawer.BodyOffsetSmall;
			ColonistBarDrawer.HeadSize = ColonistBarDrawer.HeadSizeSmall;
			ColonistBarDrawer.HeadOffset = ColonistBarDrawer.HeadOffsetSmall;
			ColonistBarDrawer.MentalHealthOffset = ColonistBarDrawer.MentalHealthOffsetSmall;
			ColonistBarDrawer.MentalHealthSize = ColonistBarDrawer.MentalHealthSizeSmall;
			ColonistBarDrawer.HealthOffset = ColonistBarDrawer.HealthOffsetSmall;
			ColonistBarDrawer.HealthSize = ColonistBarDrawer.HealthSizeSmall;
			ColonistBarDrawer.SlotBackgroundMat = ColonistBarDrawer.SlotBackgroundMatSmall;
			ColonistBarDrawer.SlotBordersMat = ColonistBarDrawer.SlotBordersMatSmall;
			ColonistBarDrawer.SlotSelectedMat = ColonistBarDrawer.SlotSelectedMatSmall;
			this.smallColonistIcons = true;
			this.ResizeMeshes();
			this.ResetMaxLabelSize();
		}

		public void UseLargeIcons()
		{
			ColonistBarDrawer.SlotSize = ColonistBarDrawer.SlotSizeLarge;
			ColonistBarDrawer.BackgroundSize = ColonistBarDrawer.BackgroundSizeLarge;
			ColonistBarDrawer.BackgroundOffset = ColonistBarDrawer.BackgroundOffsetLarge;
			ColonistBarDrawer.SlotPadding = ColonistBarDrawer.NormalSlotPaddingLarge;
			ColonistBarDrawer.PortraitOffset = ColonistBarDrawer.PortraitOffsetLarge;
			ColonistBarDrawer.PortraitSize = ColonistBarDrawer.PortraitSizeLarge;
			ColonistBarDrawer.BodySize = ColonistBarDrawer.BodySizeLarge;
			ColonistBarDrawer.BodyOffset = ColonistBarDrawer.BodyOffsetLarge;
			ColonistBarDrawer.HeadSize = ColonistBarDrawer.HeadSizeLarge;
			ColonistBarDrawer.HeadOffset = ColonistBarDrawer.HeadOffsetLarge;
			ColonistBarDrawer.MentalHealthOffset = ColonistBarDrawer.MentalHealthOffsetLarge;
			ColonistBarDrawer.MentalHealthSize = ColonistBarDrawer.MentalHealthSizeLarge;
			ColonistBarDrawer.HealthOffset = ColonistBarDrawer.HealthOffsetLarge;
			ColonistBarDrawer.HealthSize = ColonistBarDrawer.HealthSizeLarge;
			ColonistBarDrawer.SlotBackgroundMat = ColonistBarDrawer.SlotBackgroundMatLarge;
			ColonistBarDrawer.SlotBordersMat = ColonistBarDrawer.SlotBordersMatLarge;
			ColonistBarDrawer.SlotSelectedMat = ColonistBarDrawer.SlotSelectedMatLarge;
			this.smallColonistIcons = false;
			this.ResizeMeshes();
			this.ResetMaxLabelSize();
		}

		protected void ResetMaxLabelSize()
		{
			float x = ColonistBarDrawer.SlotSize.x + ColonistBarDrawer.SlotPadding.x - 8f;
			ColonistBarDrawer.MaxLabelSize = new Vector2(x, 12f);
		}

		protected void ResizeMeshes()
		{
			this.backgroundMesh = new Mesh();
			this.backgroundMesh.vertices = new Vector3[]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(ColonistBarDrawer.BackgroundSize.x, 0f, 0f),
				new Vector3(0f, ColonistBarDrawer.BackgroundSize.y, 0f),
				new Vector3(ColonistBarDrawer.BackgroundSize.x, ColonistBarDrawer.BackgroundSize.y, 0f)
			};
			this.backgroundMesh.uv = new Vector2[]
			{
				new Vector2(0f, 1f),
				new Vector2(1f, 1f),
				new Vector2(0f, 0f),
				new Vector2(1f, 0f)
			};
			this.backgroundMesh.triangles = new int[]
			{
				0,
				1,
				2,
				1,
				3,
				2
			};
			this.bodyMesh = new Mesh();
			this.bodyMesh.vertices = new Vector3[]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(ColonistBarDrawer.PortraitSize.x, 0f, 0f),
				new Vector3(0f, ColonistBarDrawer.PortraitSize.y, 0f),
				new Vector3(ColonistBarDrawer.PortraitSize.x, ColonistBarDrawer.PortraitSize.y, 0f)
			};
			Vector2 vector = new Vector2((ColonistBarDrawer.PortraitOffset.x - ColonistBarDrawer.BodyOffset.x) / ColonistBarDrawer.BodySize.x, (ColonistBarDrawer.PortraitOffset.y - ColonistBarDrawer.BodyOffset.y) / ColonistBarDrawer.BodySize.y);
			Vector2 vector2 = new Vector2((ColonistBarDrawer.PortraitOffset.x - ColonistBarDrawer.BodyOffset.x + ColonistBarDrawer.PortraitSize.x) / ColonistBarDrawer.BodySize.x, (ColonistBarDrawer.PortraitOffset.y - ColonistBarDrawer.BodyOffset.y + ColonistBarDrawer.PortraitSize.y) / ColonistBarDrawer.BodySize.y);
			this.bodyMesh.uv = new Vector2[]
			{
				new Vector2(vector.x, vector2.y),
				new Vector2(vector2.x, vector2.y),
				new Vector2(vector.x, vector.y),
				new Vector2(vector2.x, vector.y)
			};
			this.bodyMesh.triangles = new int[]
			{
				0,
				1,
				2,
				1,
				3,
				2
			};
			this.headMesh = new Mesh();
			this.headMesh.vertices = new Vector3[]
			{
				new Vector3(0f, 0f, 0f),
				new Vector3(ColonistBarDrawer.HeadSize.x, 0f, 0f),
				new Vector3(0f, ColonistBarDrawer.HeadSize.y, 0f),
				new Vector3(ColonistBarDrawer.HeadSize.x, ColonistBarDrawer.HeadSize.y, 0f)
			};
			this.headMesh.uv = new Vector2[]
			{
				new Vector2(0f, 1f),
				new Vector2(1f, 1f),
				new Vector2(0f, 0f),
				new Vector2(1f, 0f)
			};
			this.headMesh.triangles = new int[]
			{
				0,
				1,
				2,
				1,
				3,
				2
			};
		}
	}
}
