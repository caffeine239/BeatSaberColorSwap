using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {        
        public BeatSaberInfo songTwelveNoteInfo;

        public Form1()
        {
            InitializeComponent();
        }
        
        string saveLoc = "";

        private void button1_Click(object sender, EventArgs e)
        {
            string dir = Application.StartupPath + "/Songs/";
           
            AddMessage("Starting...", Color.DarkGreen);

            foreach (string s in Directory.GetDirectories(dir))
            {
                if (File.Exists(s + "/info.json"))
                {
                    saveLoc = s;
                    AddMessage("Found info.JSON file at " + s, Color.DarkGreen);
                    string newDir = @"" + s;
                    songTwelveNoteInfo = new BeatSaberInfo();
                    songTwelveNoteInfo = LoadsongData(newDir + "/info.json");

                    setPendingInfo();
                    
                    foreach (BeatSaberInfoDifficultyLevels Difficultyinfo in songTwelveNoteInfo.difficultyLevels)
                    {
                        AddMessage("Swapping colors for " + Difficultyinfo.difficulty + " level.", Color.DarkGreen);
                        var noteInfoRead = File.ReadAllText(s + "/" + Difficultyinfo.jsonPath);
                        BeatSaberNoteInfo noteInfo = JsonConvert.DeserializeObject<BeatSaberNoteInfo>(noteInfoRead);

                        foreach (BeatSaberNoteData noteDataInfo in noteInfo._notes)
                        {
                            if (noteDataInfo._type == Hand.red)
                                noteDataInfo._type = Hand.blue;
                            else if (noteDataInfo._type == Hand.blue)
                                noteDataInfo._type = Hand.red;                                                     
                        }

                        CreateNotesFile(noteInfo, Difficultyinfo);
                    }
                }
                
            }
        }

        BeatSaberInfo LoadsongData(string loc)
        {
            var info = File.ReadAllText(loc);
            return JsonConvert.DeserializeObject<BeatSaberInfo>(info);
        }

        private void CreateNotesFile(BeatSaberNoteInfo chart, BeatSaberInfoDifficultyLevels dif)
        {
            BeatSaberNoteInfo _data = new BeatSaberNoteInfo()
            {
                _version = chart._version,
                _beatsPerMinute = chart._beatsPerMinute,
                _beatsPerBar = chart._beatsPerBar,
                _noteJumpSpeed = chart._noteJumpSpeed,
                _shuffle = chart._shuffle,
                _shufflePeriod = chart._shufflePeriod,
                _events = chart._events,
                _notes = chart._notes,
                _obstacles = chart._obstacles
            };

            string json = JsonConvert.SerializeObject(_data, Formatting.Indented);

            if (!System.IO.File.Exists(@"" + saveLoc + "\\" + dif.jsonPath))
            {
                using (StreamWriter w = File.AppendText(@"" + saveLoc + "\\" + dif.jsonPath))
                {
                    w.WriteLine(json);
                }

                AddMessage("Swap Complete for " + dif.difficulty, Color.DarkGreen);
            }
            else
            {
                File.Delete(@"" + saveLoc + "\\" + dif.jsonPath);

                using (StreamWriter w = File.AppendText(@"" + saveLoc + "\\" + dif.jsonPath))
                {
                    w.WriteLine(json);
                }

                AddMessage("Swap Complete for " + dif.difficulty, Color.DarkGreen);
            }
        }

        void setPendingInfo()
        {
            BeatSaberInfo _data = new BeatSaberInfo()
            {
                songName = songTwelveNoteInfo.songName + " (Swapped Cube Colors)",
                songSubName = songTwelveNoteInfo.songSubName + " (Swapped Cube Colors)",
                authorName = songTwelveNoteInfo.authorName,
                beatsPerMinute = songTwelveNoteInfo.beatsPerMinute,
                previewStartTime = songTwelveNoteInfo.previewStartTime,
                previewDuration = songTwelveNoteInfo.previewDuration,
                coverImagePath = songTwelveNoteInfo.coverImagePath,
                environmentName = songTwelveNoteInfo.environmentName,
                difficultyLevels = songTwelveNoteInfo.difficultyLevels
            };

            string json = JsonConvert.SerializeObject(_data, Formatting.Indented);

            if (!System.IO.File.Exists(@"" + saveLoc + "\\info.JSON"))
            {
                using (StreamWriter w = File.AppendText(@"" + saveLoc + "\\info.JSON"))
                {
                    w.WriteLine(json);
                }

                AddMessage("Created new info.JSON file.", Color.DarkGreen);
            }
            else
            {
                File.Delete(@"" + saveLoc + "\\info.JSON");

                using (StreamWriter w = File.AppendText(@"" + saveLoc + "\\info.JSON"))
                {
                    w.WriteLine(json);
                }

                AddMessage("Created new info.JSON file.", Color.DarkGreen);
            }
        }
      
        private delegate void AddMessageCallback(string message, Color color);

        public void AddMessage(string message, Color color)
        {
            if (richTextBox1.InvokeRequired)
            {
                AddMessageCallback cb = new AddMessageCallback(AddMessageInternal);
                richTextBox1.BeginInvoke(cb, message, color);
            }
            else
            {
                AddMessageInternal(message, color);
            }
        }

        private void AddMessageInternal(string message, Color color)
        {
            string formattedMessage = String.Format("{0:G}   {1}{2}", DateTime.Now, message, Environment.NewLine);

            if (color != Color.Empty)
            {
                richTextBox1.SelectionColor = color;
            }
            richTextBox1.SelectedText = formattedMessage;

            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.ScrollToCaret();
        }

    }
}
