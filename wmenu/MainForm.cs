﻿using System;
using System.Collections.Generic;
using Microsoft.Win32;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace wmenu
{
    public partial class MainForm : Form
    {
        private readonly Color _backColour = Color.Azure;

        struct Program { 
            public string name; 
            public string path; 
        }

        private List<Program> _programs = new List<Program>();

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            int width = Screen.FromControl(this).Bounds.Width;
            int height = Screen.FromControl(this).Bounds.Height / 50;

            this.Width = width;
            this.Height = height;
            this.Location = new Point(0,0);
            this.Padding = Padding.Empty;
            this.BackColor = _backColour;

            inputTxtBox.Width = width / 7;
            inputTxtBox.Height = height;
            inputTxtBox.Location = new Point(0, height/4);
            inputTxtBox.Padding = Padding.Empty;
            inputTxtBox.Margin = Padding.Empty;
            inputTxtBox.BackColor = _backColour;
            inputTxtBox.BorderStyle = BorderStyle.None;

            lblPrograms.Text = string.Empty;
            lblPrograms.Width = width;
            lblPrograms.Height = height;

            LoadPrograms();
            DisplayBestMatch();
        }

        private void DisplayBestMatch(string input = "")
        {
            StringBuilder sb = new StringBuilder();
            if (string.IsNullOrWhiteSpace(input) || string.IsNullOrEmpty(input))
            {
                foreach (var p in _programs)
                    sb.Append(p.name + " ");
            }

            // Find the best matches according to the input

            lblPrograms.Text = sb.ToString();
        }

        private void inputTxtBox_TextChanged(object sender, EventArgs e)
        {
            DisplayBestMatch(inputTxtBox.Text);
        }

        private void LoadPrograms()
        {
            string uninstallKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths";

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(uninstallKey))
            {
                foreach (string skName in key.GetSubKeyNames())
                {
                    using (RegistryKey sk = key.OpenSubKey(skName))
                    {
                        string path = ((string)sk.GetValue(string.Empty))?.Trim();
                        if(!string.IsNullOrEmpty(path))
                        {
                            _programs.Add(new Program()
                            {
                                name = skName,
                                path = path
                            });
                        }
                    }
                }
            }
            _programs.Sort((x, y) => { return string.Compare(x.name, y.name); });
        }

        private void inputTxtBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }
    }
}