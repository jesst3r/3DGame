﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GUI;

namespace _3DGame.Scenes.GameplayAssets
{
    public class StatusWindow : GUI.Window
    {
        public GUI.Controls.Button OKButton;
        public GUI.Controls.RichTextDisplay Texst;
        public GameplayAssets.ItemSlot slot;
        public StatusWindow(WindowManager WM)
            {
            this.WM = WM;
            this.Width = 360;
            this.Height = 200;
            this.Title = "Status";
            this.OKButton = new GUI.Controls.Button("Test button");
            this.OKButton.Clicked += OKButton_Clicked;
            this.OKButton.Width = 128;
            this.OKButton.Height = 48;
            this.OKButton.X = 64;
            this.AddControl(this.OKButton);
            string plagueis = "Did you ever hear the tragedy of ^FF0000 Darth Plagueis ^FFFFFF The Wise? I thought not. It's not a story the Jedi would tell you. It's a Sith legend. ^00A000 Darth Plagueis was a Dark Lord of the Sith, so powerful and so wise ^FFFFFF he could use the Force to influence the midichlorians to create life... He had such a knowledge of the dark side that he could even keep the ones he cared about from dying. The dark side of the Force is a pathway to many abilities some consider to be unnatural. He became so powerful... the only thing he was afraid of was losing his power, which eventually, of course, he did. Unfortunately, he taught his apprentice everything he knew, then his apprentice killed him in his sleep. Ironic. He could save others from death, but not himself.";
            plagueis = "bepis ";
            Texst = new GUI.Controls.RichTextDisplay(plagueis, 256, 64, WM);
            Texst.X = 0;
            Texst.Y = 52;
            Texst.Flip = true;

            this.slot = new ItemSlot(GameObjects.Items.Material.MaterialTemplates.GetRandomMaterial());
            this.slot.X = 0;
            this.slot.Y = 49;
            this.Controls.Add(this.slot);
           // this.AddControl(Texst);
        }

        private void OKButton_Clicked(object sender, EventArgs e)
        {
            this.slot.Item = GameObjects.Items.Material.MaterialTemplates.GetRandomMaterial();
            this.slot.Item.SubType = GameObjects.RNG.Next(GameObjects.Items.Material.MaterialType.Max);
            this.OKButton.Title = this.slot.Item.GetName();
            List<string> ToolTip = this.slot.Item.GetTooltip();
            Console.WriteEx("New item is ^BEGINLINK " + Renderer.ColourToCode(this.slot.Item.NameColour)+ "["+this.slot.Item.GetName()+"] ^ENDLINK .^FFFFFF Click name to see more.",new List<Action> { new Action(() => {ToolTipWindow tip = new ToolTipWindow(this.WM,ToolTip, WM.MouseX, WM.MouseY, false);
                WM.Add(tip); })});
        }
    }
}
