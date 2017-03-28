﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PKHeX.Core;

namespace PKHeX.WinForms
{
    public partial class SAV_HallOfFame7 : Form
    {
        public SAV_HallOfFame7()
        {
            InitializeComponent();
            WinFormsUtil.TranslateInterface(this, Main.curlanguage);
            entries = new[]
            {
                CB_F1, CB_F2, CB_F3, CB_F4, CB_F5, CB_F6,
                CB_C1, CB_C2, CB_C3, CB_C4, CB_C5, CB_C6,
            };
            Setup();
        }
        private readonly SAV7 SAV = new SAV7(Main.SAV.Data);
        private readonly ComboBox[] entries;

        private void Setup()
        {
            int ofs = SAV.HoF;

            CHK_Flag.Checked = (BitConverter.ToUInt16(SAV.Data, ofs) & 1) == 1;
            NUD_Count.Value = BitConverter.ToUInt16(SAV.Data, ofs + 2);

            var specList = GameInfo.SpeciesDataSource.Where(s => s.Value <= SAV.MaxSpeciesID).ToList();
            for (int i = 0; i < entries.Length; i++)
            {
                int o = ofs + 4 + i*2;
                var cb = entries[i];
                cb.Items.Clear();

                cb.DisplayMember = "Text";
                cb.ValueMember = "Value";
                cb.DataSource = new BindingSource(specList, null);

                cb.SelectedValue = (int)BitConverter.ToUInt16(SAV.Data, o);
            }
        }

        private void B_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void B_Close_Click(object sender, EventArgs e)
        {
            int ofs = SAV.HoF;

            SAV.Data[ofs] &= 0xFE;
            SAV.Data[ofs] |= (byte)(CHK_Flag.Checked ? 1 : 0);
            BitConverter.GetBytes((ushort)NUD_Count.Value).CopyTo(SAV.Data, ofs + 2);
            for (int i = 0; i < entries.Length; i++)
            {
                int o = ofs + 4 + i * 2;
                var cb = entries[i];
                var val = WinFormsUtil.getIndex(cb);
                BitConverter.GetBytes((ushort)val).CopyTo(SAV.Data, o);
            }
            SAV.Data.CopyTo(Main.SAV.Data, 0);
            Main.SAV.Edited = true;
            Close();
        }
    }
}
