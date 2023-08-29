using AotC.Content.Items.Accessories;
using AotC.Core.GlobalInstances.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace AotC.Content.CustomHooks
{

    class PlayerTarget : HookGroup
    {
        //Drawing Player to Target. Should be safe. Excuse me if im duplicating something that alr exists :p
        public static RenderTarget2D Target;

        public static bool canUseTarget = false;

        public static int sheetSquareX;
        public static int sheetSquareY;

        private readonly List<DrawData> _drawData = new();

        private readonly List<int> _dust = new();

        private readonly List<int> _gore = new();
        public static SamplerState MountedSamplerState
        {
            get
            {
                if (!Main.drawToScreen)
                {
                    return SamplerState.AnisotropicClamp;
                }
                return SamplerState.LinearClamp;
            }
        }


        /// <summary>
        /// we use a dictionary for the Player indexes because they are not guarenteed to be 0, 1, 2 etc. the Player at index 1 leaving could result in 2 Players being numbered 0, 2
        /// but we don't want a gigantic RT with all 255 possible Players getting template space so we resize and keep track of their effective index
        /// </summary>
        private static Dictionary<int, int> PlayerIndexLookup;

        /// <summary>
        /// to keep track of Player counts as they change
        /// </summary>
        private static int prevNumPlayers;

        //stored vars so we can determine original lighting for the Player / potentially other uses
        Vector2 oldPos;
        Vector2 oldCenter;
        Vector2 oldMountedCenter;
        Vector2 oldScreen;
        Vector2 oldItemLocation;
        Vector2 positionOffset;

        public override void Load()
        {
            if (Main.dedServ)
                return;

            sheetSquareX = 400;
            sheetSquareY = 600;

            PlayerIndexLookup = new Dictionary<int, int>();
            prevNumPlayers = -1;

            Main.QueueMainThreadAction(() => Target = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight));

            On_Main.CheckMonoliths += DrawTargets;
            On_Lighting.GetColor_int_int += GetColorOverride;
            On_Lighting.GetColor_Point += GetColorOverride;
            On_Lighting.GetColor_int_int_Color += GetColorOverride;
            On_Lighting.GetColor_Point_Color += GetColorOverride;
            On_Lighting.GetColorClamped += GetColorOverride;
        }

        public override void Unload()
        {
            On_Main.CheckMonoliths -= DrawTargets;
            On_Lighting.GetColor_int_int -= GetColorOverride;
            On_Lighting.GetColor_Point -= GetColorOverride;
            On_Lighting.GetColor_int_int_Color -= GetColorOverride;
            On_Lighting.GetColor_Point_Color -= GetColorOverride;
            On_Lighting.GetColorClamped -= GetColorOverride;
        }

        private Color GetColorOverride(On_Lighting.orig_GetColorClamped orig, int x, int y, Color oldColor)
        {
            if (canUseTarget)
                return orig.Invoke(x, y, oldColor);

            return orig.Invoke(x + (int)((oldPos.X - positionOffset.X) / 16), y + (int)((oldPos.Y - positionOffset.Y) / 16), oldColor);
        }

        private Color GetColorOverride(On_Lighting.orig_GetColor_Point_Color orig, Point point, Color originalColor)
        {
            if (canUseTarget)
                return orig.Invoke(point, originalColor);

            return orig.Invoke(new Point(point.X + (int)((oldPos.X - positionOffset.X) / 16), point.Y + (int)((oldPos.Y - positionOffset.Y) / 16)), originalColor);
        }

        public Color GetColorOverride(On_Lighting.orig_GetColor_Point orig, Point point)
        {
            if (canUseTarget)
                return orig.Invoke(point);

            return orig.Invoke(new Point(point.X + (int)((oldPos.X - positionOffset.X) / 16), point.Y + (int)((oldPos.Y - positionOffset.Y) / 16)));
        }

        public Color GetColorOverride(On_Lighting.orig_GetColor_int_int orig, int x, int y)
        {
            if (canUseTarget)
                return orig.Invoke(x, y);

            return orig.Invoke(x + (int)((oldPos.X - positionOffset.X) / 16), y + (int)((oldPos.Y - positionOffset.Y) / 16));
        }

        public Color GetColorOverride(On_Lighting.orig_GetColor_int_int_Color orig, int x, int y, Color c)
        {
            if (canUseTarget)
                return orig.Invoke(x, y, c);

            return orig.Invoke(x + (int)((oldPos.X - positionOffset.X) / 16), y + (int)((oldPos.Y - positionOffset.Y) / 16), c);
        }

        public static Rectangle GetPlayerTargetSourceRectangle(int whoAmI)
        {
            if (PlayerIndexLookup.ContainsKey(whoAmI))
                return new Rectangle(PlayerIndexLookup[whoAmI] * sheetSquareX, 0, sheetSquareX, sheetSquareY);

            return Rectangle.Empty;
        }

        /// <summary>
        /// gets the whoAmI's Player's renderTarget and returns a Vector2 that represents the rendertarget's position overlapping with the Player's position in terms of screen coordinates
        /// comes preshifted for reverse gravity
        /// </summary>
        /// <param name="whoAmI"></param>
        /// <returns></returns>
        public static Vector2 GetPlayerTargetPosition(int whoAmI)
        {
            Vector2 gravPosition = Main.ReverseGravitySupport(Main.player[whoAmI].position - Main.screenPosition);
            return gravPosition - new Vector2(sheetSquareX / 2, sheetSquareY / 2);
        }

        private void DrawTargets(On_Main.orig_CheckMonoliths orig)
        {
            orig();

            if (Main.gameMenu)
                return;

            if (Main.player.Any(n => n.active))
                DrawPlayerTarget();

            if (Main.instance.tileTarget.IsDisposed)
                return;
        }

        public static Vector2 GetPositionOffset(int whoAmI)
        {
            if (PlayerIndexLookup.ContainsKey(whoAmI))
                return new Vector2(PlayerIndexLookup[whoAmI] * sheetSquareX + sheetSquareX / 2, sheetSquareY / 2);

            return Vector2.Zero;
        }

        private void DrawPlayerTarget()
        {
            int activePlayerCount = Main.player.Count(n => n.active);

            if (activePlayerCount != prevNumPlayers)
            {
                prevNumPlayers = activePlayerCount;

                Target.Dispose();
                Target = new RenderTarget2D(Main.graphics.GraphicsDevice, sheetSquareY * activePlayerCount, sheetSquareY);

                int activeCount = 0;

                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (Main.player[i].active)
                    {
                        PlayerIndexLookup[i] = activeCount;
                        activeCount++;
                        Main.player[i].GetPlot().playerTexture?.Dispose();
                        Main.player[i].GetPlot().playerTexture = new(Main.graphics.GraphicsDevice, sheetSquareY * activePlayerCount, sheetSquareY);
                    }
                }
            }

            RenderTargetBinding[] oldtargets2 = Main.graphics.GraphicsDevice.GetRenderTargets();
            canUseTarget = false;

            Main.graphics.GraphicsDevice.SetRenderTarget(Target);
            Main.graphics.GraphicsDevice.Clear(Color.Transparent);
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.EffectMatrix);
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];

                if (player.active && player.dye.Length > 0)
                {
                    oldPos = player.position;
                    oldCenter = player.Center;
                    oldMountedCenter = player.MountedCenter;
                    oldScreen = Main.screenPosition;
                    oldItemLocation = player.itemLocation;
                    int oldHeldProj = player.heldProj;

                    //temp change Player's actual position to lock into their frame
                    positionOffset = GetPositionOffset(i);
                    player.position = positionOffset;
                    player.Center = oldCenter - oldPos + positionOffset;
                    player.itemLocation = oldItemLocation - oldPos + positionOffset;
                    player.MountedCenter = oldMountedCenter - oldPos + positionOffset;
                    player.heldProj = -1;
                    Main.screenPosition = Vector2.Zero;

                    DrawScuffedPlayer(player, player.position, player.fullRotation, player.fullRotationOrigin, 0f);

                    player.position = oldPos;
                    player.Center = oldCenter;
                    Main.screenPosition = oldScreen;
                    player.itemLocation = oldItemLocation;
                    player.MountedCenter = oldMountedCenter;
                    player.heldProj = oldHeldProj;
                }
            }

            Main.spriteBatch.End();

            Main.graphics.GraphicsDevice.SetRenderTargets(oldtargets2);
            canUseTarget = true;
        }

        public void DrawScuffedPlayer(Player drawPlayer, Vector2 position, float rotation, Vector2 rotationOrigin, float shadow = 0f, float scale = 1f)
        {
            if (drawPlayer.ShouldNotDraw)
            {
                return;
            }
            PlayerDrawSet drawInfo = default;
            _drawData.Clear();
            _dust.Clear();
            _gore.Clear();
            drawInfo.BoringSetup(drawPlayer, _drawData, _dust, _gore, position, shadow, rotation, rotationOrigin);
            DrawPlayer_UseNormalLayers(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_TransformDrawData(ref drawInfo);
            if (scale != 1f)
            {
                PlayerDrawLayers.DrawPlayer_ScaleDrawData(ref drawInfo, scale);
            }
            PlayerDrawLayers.DrawPlayer_RenderAllLayers(ref drawInfo);
            if (!drawInfo.drawPlayer.mount.Active || !drawInfo.drawPlayer.UsingSuperCart)
            {
                return;
            }
            for (int i = 0; i < 1000; i++)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == drawInfo.drawPlayer.whoAmI && Main.projectile[i].type == 591)
                {
                    Main.instance.DrawProj(i);
                }
            }
        }

        private static void DrawPlayer_UseNormalLayers(ref PlayerDrawSet drawInfo)
        {
            PlayerDrawLayers.DrawPlayer_extra_TorsoPlus(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_01_2_JimsCloak(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_extra_TorsoMinus(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_02_MountBehindPlayer(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_03_Carpet(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_03_PortableStool(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_extra_TorsoPlus(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_04_ElectrifiedDebuffBack(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_05_ForbiddenSetRing(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_05_2_SafemanSun(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_06_WebbedDebuffBack(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_07_LeinforsHairShampoo(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_extra_TorsoMinus(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_08_Backpacks(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_extra_TorsoPlus(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_08_1_Tails(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_extra_TorsoMinus(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_09_Wings(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_extra_TorsoPlus(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_01_BackHair(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_10_BackAcc(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_01_3_BackHead(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_extra_TorsoMinus(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_11_Balloons(ref drawInfo);
            if (drawInfo.weaponDrawOrder == WeaponDrawOrder.BehindBackArm)
            {
                PlayerDrawLayers.DrawPlayer_27_HeldItem(ref drawInfo);
            }
            PlayerDrawLayers.DrawPlayer_12_Skin(ref drawInfo);
            if (drawInfo.drawPlayer.wearsRobe && drawInfo.drawPlayer.body != 166)
            {
                PlayerDrawLayers.DrawPlayer_14_Shoes(ref drawInfo);
                PlayerDrawLayers.DrawPlayer_13_Leggings(ref drawInfo);
            }
            else
            {
                PlayerDrawLayers.DrawPlayer_13_Leggings(ref drawInfo);
                PlayerDrawLayers.DrawPlayer_14_Shoes(ref drawInfo);
            }
            PlayerDrawLayers.DrawPlayer_extra_TorsoPlus(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_15_SkinLongCoat(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_16_ArmorLongCoat(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_17_Torso(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_18_OffhandAcc(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_19_WaistAcc(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_20_NeckAcc(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_21_Head(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_21_1_Magiluminescence(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_22_FaceAcc(ref drawInfo);
            if (drawInfo.drawFrontAccInNeckAccLayer)
            {
                PlayerDrawLayers.DrawPlayer_extra_TorsoMinus(ref drawInfo);
                PlayerDrawLayers.DrawPlayer_32_FrontAcc_FrontPart(ref drawInfo);
                PlayerDrawLayers.DrawPlayer_extra_TorsoPlus(ref drawInfo);
            }
            PlayerDrawLayers.DrawPlayer_23_MountFront(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_24_Pulley(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_JimsDroneRadio(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_32_FrontAcc_BackPart(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_25_Shield(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_extra_MountPlus(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_26_SolarShield(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_extra_MountMinus(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_28_ArmOverItem(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_29_OnhandAcc(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_30_BladedGlove(ref drawInfo);
            if (!drawInfo.drawFrontAccInNeckAccLayer)
            {
                PlayerDrawLayers.DrawPlayer_32_FrontAcc_FrontPart(ref drawInfo);
            }
            PlayerDrawLayers.DrawPlayer_extra_TorsoMinus(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_31_ProjectileOverArm(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_33_FrozenOrWebbedDebuff(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_34_ElectrifiedDebuffFront(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_35_IceBarrier(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_36_CTG(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_37_BeetleBuff(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_38_EyebrellaCloud(ref drawInfo);
            PlayerDrawLayers.DrawPlayer_MakeIntoFirstFractalAfterImage(ref drawInfo);
        }
    }
}
