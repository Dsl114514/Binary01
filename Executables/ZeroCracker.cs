using BepInEx;
using BepInEx.Hacknet;
using Hacknet;
using Hacknet.Extensions;
using Hacknet.Gui;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pathfinder;
using Pathfinder.Action;
using Pathfinder.Administrator;
using Pathfinder.Daemon;
using Pathfinder.GUI;
using Pathfinder.Mission;
using Pathfinder.Port;
using Pathfinder.Util;
using Pathfinder.Util.XML;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
namespace ZeroCracker
{
    public class ZeroCrackers : Pathfinder.Executable.BaseExecutable
    {
        public ZeroCrackers(Rectangle location, OS operatingSystem, string[] args) : base(location, operatingSystem, args)
        {
            this.ramCost = 200; // 你的内存占用
            this.IdentifierName = "Zero Cracker"; // 你的程序在ram栏显示的名称
        }

        // 用于控制钟摆摆动角度的变量
        private float pendulumAngle = 0f;

        // 创建随机数生成器
        private Random random = new Random();

        public override void LoadContent() // 首次执行后触发的内容
        {
            Computer computer = Programs.getComputer(os, os.thisComputer.ip);
            CheckProxy();
            base.LoadContent();
        }

        public override void Draw(float t) // 绘制程序窗口
        {
            base.Draw(t);
            drawTarget();
            drawOutline();
            Rectangle drawArea = Utils.InsetRectangle(new Rectangle(this.bounds.X, this.bounds.Y + Module.PANEL_HEIGHT, this.bounds.Width, this.bounds.Height - Module.PANEL_HEIGHT), 2);
            Hacknet.Gui.RenderedRectangle.doRectangle(bounds.X + 2, bounds.Y + 15, drawArea.Width, drawArea.Height, new Color(0, 0, 0));
            key();
            Hacknet.Gui.TextItem.doLabel(new Vector2(bounds.Center.X - 125, bounds.Center.Y- 15), "Key connection", new Color(255, 255, 255,50));
            
        }

        private float lifetime = 0f;
        public override void Update(float t)
        {
            base.Update(t);
            lifetime += t;
            if (lifetime > 15.5f)
            {
                isExiting = true;
                Programs.getComputer(os, targetIP).openPort(0, os.thisComputer.ip);
            }
        }

        private bool proxyBlocked = false;
        private void CheckProxy()
        {
            Computer computer = Programs.getComputer(this.os, this.targetIP);
            bool flag = computer != null && computer.proxyActive;
            if (flag)
            {
                this.proxyBlocked = true;
                this.os.write("ERROR: Proxy firewall active");
                this.needsRemoval = true;
            }
        }
        private void key()
        {
            // 计算绘制区域
            Rectangle drawArea = Utils.InsetRectangle(new Rectangle(this.bounds.X, this.bounds.Y + Module.PANEL_HEIGHT, this.bounds.Width, this.bounds.Height - Module.PANEL_HEIGHT), 4);

            int centerX = drawArea.X + drawArea.Width / 2;
            int centerY = drawArea.Y + drawArea.Height / 2;
            int maxSize = Math.Min(drawArea.Width, drawArea.Height) / 2;

            // 使用生命周期控制动画
            float animationTime = lifetime % 5f; // 5秒循环
            float pulse = 0.5f + 0.5f * (float)Math.Sin(animationTime * Math.PI * 2f);

            // 1. 绘制背景网格（参考示例风格）
            for (int x = drawArea.X + 8; x < drawArea.X + drawArea.Width - 8; x += 20)
            {
                for (int y = drawArea.Y + 8; y < drawArea.Y + drawArea.Height - 8; y += 20)
                {
                    // 计算动态大小
                    float distFactor = (Math.Abs(x - centerX) + Math.Abs(y - centerY)) / (float)(drawArea.Width + drawArea.Height);
                    float timeOffset = animationTime + distFactor * 2f;
                    float sizeFactor = 0.6f - Math.Abs((timeOffset % 2f) - 1f);

                    if (sizeFactor > 0)
                    {
                        sizeFactor = sizeFactor * sizeFactor * 0.8f;
                        int rectSize = (int)(12 * sizeFactor);
                        int rectX = x + 10 - rectSize / 2;
                        int rectY = y + 10 - rectSize / 2;

                        // 使用随机颜色
                        byte r = (byte)(100 + random.Next(100));
                        byte g = (byte)(100 + random.Next(100));
                        byte b = (byte)(150 + random.Next(100));
                        Color gridColor = new Color(r, g, b, 100);

                        // 使用RenderedRectangle绘制
                        RenderedRectangle.doRectangle(rectX, rectY, rectSize, rectSize, gridColor);
                    }
                }
            }

            // 2. 绘制钥匙主体
            // 3. 绘制旋转的光环效果
            int haloRadius = maxSize * 3 / 4;
            int pointCount = 32;

            for (int i = 0; i < pointCount; i++)
            {
                float angle = MathHelper.ToRadians(i * 15f + animationTime * 100f);
                float x = centerX + (float)Math.Cos(angle) * haloRadius;
                float y = centerY + (float)Math.Sin(angle) * haloRadius;

                int pointSize = 3;
                float alpha = 0.3f + 0.7f * (float)Math.Sin(animationTime * Math.PI * 2f + i * 0.5f);
                Color haloColor = new Color(0, 200, 255, (byte)(alpha * 255));

                RenderedRectangle.doRectangle((int)x - pointSize / 2, (int)y - pointSize / 2, pointSize, pointSize, haloColor);
            }

            // 4. 绘制连接线（从钥匙到边缘）
            for (int i = 0; i < 8; i++)
            {
                float lineAngle = MathHelper.ToRadians(i * 45f + animationTime * 30f);
                float endX = centerX + (float)Math.Cos(lineAngle) * haloRadius;
                float endY = centerY + (float)Math.Sin(lineAngle) * haloRadius;

                // 使用多个小矩形模拟线条
                int segments = 10;
                for (int j = 0; j < segments; j++)
                {
                    float t = (float)j / segments;
                    float segX = centerX + (endX - centerX) * t;
                    float segY = centerY + (endY - centerY) * t;

                    int segSize = 1 + (int)(Math.Sin(animationTime * Math.PI * 4f + j * 0.5f) * 2);
                    byte alpha = (byte)(100 + 155 * (1f - t));
                    Color lineColor = new Color(100, 200, 255, alpha);

                    RenderedRectangle.doRectangle((int)segX - segSize / 2, (int)segY - segSize / 2, segSize, segSize, lineColor);
                }
            }
        }
        private void no()
        {
     
        }
    }
}

