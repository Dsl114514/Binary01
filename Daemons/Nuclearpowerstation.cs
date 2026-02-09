using BepInEx;
using BepInEx.Hacknet;
using Hacknet;
using Hacknet.Extensions;
using Hacknet.Gui;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pathfinder;
using Hacknet.Daemons.Helpers;
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

namespace NPS;

public class NuclearPowerStation : Pathfinder.Daemon.BaseDaemon
{
    public NuclearPowerStation(Computer computer, string serviceName, OS opSystem) : base(computer, serviceName, opSystem) { }

    public override string Identifier => "Nuclear Power Ptation";

    [XMLStorage]
    public string DisplayString = "NSP";

    public override void draw(Rectangle bounds, SpriteBatch sb)
    {
        Rectangle drawArea = Utils.InsetRectangle(new Rectangle(bounds.X, bounds.Y + Module.PANEL_HEIGHT, bounds.Width, bounds.Height - Module.PANEL_HEIGHT), 2);

        base.draw(bounds, sb);
        Hacknet.Gui.RenderedRectangle.doRectangle(bounds.Center.X - 277, bounds.Center.Y - 365, drawArea.Width, drawArea.Height + 12, new Color(0, 0, 0));
        Hacknet.Gui.TextItem.doLabel(new Vector2(bounds.X, bounds.Y), "Nuclear Power Ptation", new Color(255, 255, 255));
        bool exitButton = Hacknet.Gui.Button.doButton(123123719, bounds.Center.X + 80, bounds.Center.Y + 300, 200, 25, "返回", new Color(255, 0, 0));
        if (exitButton)
        {
            this.os.display.command = "connect";
        }
        bool RebootsystemButton = Hacknet.Gui.Button.doButton(123312, bounds.Center.X + 80, bounds.Center.Y + 250, 200, 25, "重启系统", new Color(255, 0, 0));
        if (RebootsystemButton)
        {
           // Hacknet.Gui.TextBox.BoxWasActivated;
        }
        RenderedRectangle.doRectangle(bounds.Center.X - 275, bounds.Center.Y + 150, 550, 1, Color.White);
        
     
    }

}