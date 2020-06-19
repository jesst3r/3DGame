﻿using GameObject;
using GameObject.AbilityLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MagicEditor
{
    public partial class MainForm : Form
    {
        ModularAbility CurrentAbility;
        private bool lockform = false;
        public List<ModularAbility> abilities = new List<ModularAbility>();
        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AbilityEffectDefinition.LoadDefinitions();

            /* DUMMY CODE DUMMY CODE DUMMY CODE
            //this will be replaced with actually loading a list
            //abilities.Add(new TestAbility());
            //abilities.Add(new TestAbility());
            //abilities.Add(new TestAbility());

            AddWithAutoname(new TestAbility(),true);
            AddWithAutoname(new TestAbility(), true);
            AddWithAutoname(new TestAbility(), true);
            //*/
            AbilityFileReader fr = new AbilityFileReader();

            abilities = fr.ReadFile();
            foreach (ModularAbility ability in abilities)
            {
                abilityselector.Items.Add(ability);
            }

            CurrentAbility = abilities[0];
            abilityselector.SelectedIndex = 0;
            EditCurrentAbility();

           
        }

        private void EditCurrentAbility()
        {
           
            ReloadList();
            SetIcon(CurrentAbility.Icon);

            this.spellname.Text = CurrentAbility.Name;
            lockform = true;
            castbase.Value =     (decimal)CurrentAbility.BaseValues["cast_time"];
            channelbase.Value =  (decimal)CurrentAbility.BaseValues["channel_time"];
            mpbase.Value =       (decimal)CurrentAbility.BaseValues["mp_cost"];
            cdbase.Value =       (decimal)CurrentAbility.BaseValues["cooldown"];
            castdelta.Value =    (decimal)CurrentAbility.GrowthValues["cast_time"];
            channeldelta.Value = (decimal)CurrentAbility.GrowthValues["channel_time"];
            mpdelta.Value =      (decimal)CurrentAbility.GrowthValues["mp_cost"];
            cddelta.Value =      (decimal)CurrentAbility.GrowthValues["cooldown"];

            UpdateDescriptionPreview();
            lockform = false;
        }

        private void UpdateBases()
        {
            CurrentAbility.BaseValues["cast_time"] =         (float)castbase.Value;
            CurrentAbility.BaseValues["channel_time"]=       (float)channelbase.Value;
            CurrentAbility.BaseValues["mp_cost"]=            (float)mpbase.Value;
            CurrentAbility.BaseValues["cooldown"]=           (float)cdbase.Value;
            CurrentAbility.GrowthValues["cast_time"]=        (float)castdelta.Value;
            CurrentAbility.GrowthValues["channel_time"]=     (float)channeldelta.Value;
            CurrentAbility.GrowthValues["mp_cost"]=          (float)mpdelta.Value;
            CurrentAbility.GrowthValues["cooldown"] =        (float)cddelta.Value;
        }

        private void SetIcon(int IconId)
        {
            iconimage.Location = new Point((IconId % 64)*-32, ((int)(IconId / 64f))*-32);
        }

        private void ReloadList()
        {
            EffectList.Items.Clear();
            foreach (ITimedEffect effect in CurrentAbility.GetModules())
            {
                AbilityEffectDefinition adef = AbilityEffectDefinition.GetDefinition(effect.EffectType);
                string[] ItemProps = new string[] { adef.FriendlyName, effect.BaseTime.ToString(), effect.BaseDuration.ToString() };
                ListViewItem line = new ListViewItem(ItemProps, adef.Icon)
                {
                    Tag = effect
                };
                EffectList.Items.Add(line);

            }

        }

        private void UpdateDescriptionPreview()
        {
            CurrentAbility.Level = (int)lvlprev.Value;
            descprev.Text = string.Join("\r\n", CurrentAbility.GetTooltip());
        }

        private string AddWithAutoname(ModularAbility ab, bool autoaccept=false)
        {
            string autoname = "";

            int degree = 0;
            foreach (ModularAbility a in abilities)
            {
                string[] autoparts = a.ID.Split('.');
                if(autoparts.Length==2)//current name is an autoname - compare to first piece only
                {
                    
                    if(autoparts[0]==ab.ID)//matching auto
                    {
                        int currentdegree = int.Parse(autoparts[1])+1;
                        if (currentdegree > degree)
                            degree = currentdegree;
                    }
                }
                else
                {
                    if (a.ID == ab.ID)
                        degree = 1;
                }
            }

            if (degree > 0)
                autoname = ab.ID + "." + degree.ToString();

            //if ab ID already exists, change the ID to autoname and add if autoaccept is true, else do nothing and just return autoname
            if (autoname!="")
            {
                if(autoaccept)
                {

                    ab.ID = autoname;
                    abilities.Add(ab);
                }
            }
            else //no collision, just append as normal
            {

                abilities.Add(ab);
            }
            //the return value is useful in determining if a collision occurred - and with autoaccept= false lets user choose to force the name or not
            return autoname;
        }

        #region GUI wireups
        private void EffectList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (EffectList.SelectedItems.Count != 1)
                return;
            ListViewItem item = EffectList.SelectedItems[0];
            EditAbilityComponent editform = new EditAbilityComponent((ITimedEffect)item.Tag);
            if (editform.ShowDialog() == DialogResult.OK)
            {

                ReloadList();
                UpdateDescriptionPreview();
            }
            //MessageBox.Show(((ITimedEffect)item.Tag).EffectType);
        }

        private void lvlprev_ValueChanged(object sender, EventArgs e)
        {
            UpdateDescriptionPreview();

        }


        private void iconimage_DoubleClick(object sender, EventArgs e)
        {
            ChooseIcon chooseform = new ChooseIcon();
            if(chooseform.ShowDialog()==DialogResult.OK)
            {
                SetIcon(chooseform.Icon);
                CurrentAbility.Icon = chooseform.Icon;
            }
        }

        private void descprev_DoubleClick(object sender, EventArgs e)
        {
            TextPrompt prompt = new TextPrompt();
            prompt.Input = CurrentAbility.DescriptionString;
            if(prompt.ShowDialog()==DialogResult.OK)
            {
                CurrentAbility.DescriptionString = prompt.Input;

                UpdateDescriptionPreview();
            }
        }

        private void spellname_DoubleClick(object sender, EventArgs e)
        {
            TextPrompt prompt = new TextPrompt();
            prompt.Input = CurrentAbility.Name;
            if (prompt.ShowDialog() == DialogResult.OK)
            {
                CurrentAbility.Name = prompt.Input;
                spellname.Text = CurrentAbility.Name;
                UpdateDescriptionPreview();
                //this refreshes the relevant string on the listbox
                abilityselector.Items[abilityselector.Items.IndexOf(CurrentAbility)] = CurrentAbility;
            }
        }

        //this handles ALL of the basic values changes
        private void cddelta_ValueChanged(object sender, EventArgs e)
        {
            if (lockform)
                return;
            UpdateBases();

            UpdateDescriptionPreview();
        }

        private void effectmenu_Opening(object sender, CancelEventArgs e)
        {
            
                effectmenu.Items[1].Enabled = EffectList.SelectedItems.Count == 1;
        }

        private void removeEffectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (EffectList.SelectedItems.Count != 1)
                return;
            ListViewItem item = EffectList.SelectedItems[0];
            ITimedEffect effect = (ITimedEffect)item.Tag;
            CurrentAbility.Effects.Remove(effect);
            EffectList.SelectedItems.Clear();
            EffectList.Items.Remove(item);
        }

        private void addEffectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PickEffectSimple box = new PickEffectSimple();
            if(box.ShowDialog()== DialogResult.OK)
            {
                string result = box.Effect;
                
                
                CurrentAbility.Effects.Add(EffectHelper.CreateEmpty(result));
                ReloadList();
            }
        }


        private void abilitymenu_Opening(object sender, CancelEventArgs e)
        {
            abilitymenu.Items[1].Enabled = abilityselector.SelectedItem != null;
        }

        private void createAbilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextPrompt prompt = new TextPrompt();
            if(prompt.ShowDialog()==DialogResult.OK)
            {
                ModularAbility a = ModularAbility.CreateEmpty(prompt.Input);
                AddWithAutoname(a, true);
                abilityselector.Items.Add(a);
            }
        }

        private void deleteAbilityToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ModularAbility removeme = (ModularAbility)abilityselector.SelectedItem;
            abilityselector.Items.Remove(removeme);
            abilities.Remove(removeme);

        }

        private void abilityselector_DoubleClick(object sender, EventArgs e)
        {
            CurrentAbility = (ModularAbility)abilityselector.SelectedItem;
            EditCurrentAbility();
        }
        #endregion

        private void saveabilities_Click(object sender, EventArgs e)
        {

            AbilityFileWriter fw = new AbilityFileWriter(abilities);
            fw.WriteFile();
        }
    }
}