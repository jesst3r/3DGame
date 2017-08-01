﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _3DGame.GameObjects.MapEntities.ActorLogic;
using Microsoft.Xna.Framework;

namespace _3DGame.GameObjects.Items
{
    public class ItemBonus : GameObjects.StatBonus
    {

        public Color LineColour;
        public string Effecttext;
        public string BonusText
        {
            get
            {
                if(this.Multiplier!=0)
                {
                    if (this.FlatValue != 0)
                        return String.Format(this.Effecttext,  Math.Round(this.FlatValue), Math.Round(this.Multiplier*100));
                    return String.Format(this.Effecttext, Math.Round(this.Multiplier*100));
                }
                else
                {
                    return String.Format(this.Effecttext, Math.Round(this.FlatValue));
                }
               
            }
        }
        public ItemBonus()
        {
            this.Order = StatOrder.Equip;
            this.LineColour= new Color(120, 100, 255);
        }
       
    }
}
