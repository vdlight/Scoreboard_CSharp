using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NauComDotNet;

namespace Scoreboard_CSharp
{


    public partial class TAIFBoard : Form
    {
        penalty H1_pen;
        penalty H2_pen;
        penalty H3_pen;
        penalty G1_pen;
        penalty G2_pen;
        penalty G3_pen;

        // References found in 
        // Nautronic graphic socreboard manual
        // Appendix A, 8.2 icehockey
        // no timestamp 
        enum defs
        {
            HOME_SCORE_10 = 0,
            HOME_SCORE_1 = 1,
            HOME_SCORE_100 = 2,
            GUEST_SCORE_10 = 128,
            GUEST_SCORE_1 = 129,
            GUEST_SCORE_100 = 130,
            MATCH_PERIOD_10 = 6,
            MATCH_PERIOD_1 = 7,
            MATCH_10_MIN = 12,
            MATCH_1_MIN = 13,
            MATCH_10_SEC = 14,
            MATCH_1_SEC = 15,
            HOME_PEN1_NR_10 = 17,
            HOME_PEN1_NR_1 = 18,
            HOME_PEN1_MINS = 19,
            HOME_PEN1_10_SEC = 20,
            HOME_PEN1_1_SEC = 21,
            HOME_PEN2_NR_10 = 22,
            HOME_PEN2_NR_1 = 23,
            HOME_PEN2_MINS = 24,
            HOME_PEN2_10_SEC = 25,
            HOME_PEN2_1_SEC = 26,
            HOME_PEN3_NR_10 = 27,
            HOME_PEN3_NR_1 = 28,
            HOME_PEN3_MINS = 29,
            HOME_PEN3_10_SEC = 30,
            HOME_PEN3_1_SEC = 31,
            GUEST_PEN1_NR_10 = 145,
            GUEST_PEN1_NR_1 = 146,
            GUEST_PEN1_MINS = 147,
            GUEST_PEN1_10_SEC = 148,
            GUEST_PEN1_1_SEC = 149,
            GUEST_PEN2_NR_10 = 150,
            GUEST_PEN2_NR_1 = 151,
            GUEST_PEN2_MINS = 152,
            GUEST_PEN2_10_SEC = 153,
            GUEST_PEN2_1_SEC = 154,
            GUEST_PEN3_NR_10 = 155,
            GUEST_PEN3_NR_1 = 156,
            GUEST_PEN3_MINS = 157,
            GUEST_PEN3_10_SEC = 158,
            GUEST_PEN3_1_SEC = 159,
            HOME_SHOTS_10 = 4,
            HOME_SHOTS_1 = 5,
            GUEST_SHOTS_10 = 132,
            GUEST_SHOTS_1 = 133,

            END_OF_ENUM = 500
        }

        public class message
        {
            public message(int adress, int cnt, int data1, int data2 = 0, int data3 = 0, int data4 = 0, int data5 = 0)
            {
                _address = adress;
                _cnt = cnt;
                _data1 = data1;
                _data2 = data2;
                _data3 = data3;
                _data4 = data4;
                _data5 = data5;
            }
            public int _address;
            public int _cnt;
            public int _data1;
            public int _data2;
            public int _data3;
            public int _data4;
            public int _data5;
        }
        int index = 0;
        int cnt = 0;

        int[] values;
        int[] timeouts;

        const int communicationTimeout = 6;

        Queue<String> data;
        private NauCom NC;

        int ticker = 0;
        bool boolValue;
        int skipTime10 = 0;
        int skipPen10 = 0;
        int match1 = 0;
        int match10 = 0;
        int period = 0;
        int HScore = 0;
        int GScore = 0;
        int shots10 = 0;
        int shots1 = 0;
        int Gshots10 = 0;
        int Gshots1 = 0;
        int penNr10 = 0;
        int penNr1 = 0;
        int pensec10 = 0;
        int pensec1 = 0;
        int penmin = 0;
        int homeScore10 = 0;


        int[] homePenaltyAlive  = new int[3] { 0, 0, 0 };
        int[] homePenaltyOldVal = new int[3] { 0, 0, 0 };
        int[] guestPenaltyAlive = new int[3] { 0, 0, 0 };
        int[] guestPenaltyOldVal = new int[3] { 0, 0, 0 };

        string colon = ":";
        bool colonVisible = true;

        public TAIFBoard()
        {
            InitializeComponent();


            // placement of labels, has to be in FORM constructor
            Point p = PointToScreen(GuestTeamShots_lbl.Location);
            p = TAIF_Scoreboard.PointToClient(p);


            Time_lbl.Parent = TAIF_Scoreboard;
            Time_lbl.Location = new Point(213, 21);
            Time_lbl.BackColor = Color.Transparent;

            HomeTeamShots_lbl.Parent = TAIF_Scoreboard;
            HomeTeamShots_lbl.Location = new Point(25, 30);
            HomeTeamShots_lbl.BackColor = Color.Transparent;

            GuestTeamShots_lbl.Parent = TAIF_Scoreboard;
            GuestTeamShots_lbl.Location = new Point(498, 30);
            GuestTeamShots_lbl.BackColor = Color.Transparent;

            HomeScore_lbl.Parent = TAIF_Scoreboard;
            HomeScore_lbl.Location = new Point(152, 22);
            HomeScore_lbl.BackColor = Color.Transparent;

            HomeTeam_lbl.Parent = TAIF_Scoreboard;
            HomeTeam_lbl.Location = new Point(66, 22);
            HomeTeam_lbl.BackColor = Color.Transparent;

            GuestScore_lbl.Parent = TAIF_Scoreboard;
            GuestScore_lbl.Location = new Point(360, 22);
            GuestScore_lbl.BackColor = Color.Transparent;

            GuestTeam_lbl.Parent = TAIF_Scoreboard;
            GuestTeam_lbl.Location = new Point(402, 22);
            GuestTeam_lbl.BackColor = Color.Transparent;

            Period_lbl.Parent = TAIF_Scoreboard;
            Period_lbl.Location = new Point(252, 59);
            Period_lbl.BackColor = Color.Transparent;

            HomePen1Time_lbl.Parent = HomePen1Image_lbl;
            HomePen1Time_lbl.Location = new Point(44, 5);
            HomePen1Time_lbl.BackColor = Color.Transparent;

            HomePen1Player_lbl.Parent = HomePen1Image_lbl;
            HomePen1Player_lbl.Location = new Point(10, 5);
            HomePen1Player_lbl.BackColor = Color.Transparent;

            HomePen2Time_lbl.Parent = HomePen2Image_lbl;
            HomePen2Time_lbl.Location = new Point(44, 2);
            HomePen2Time_lbl.BackColor = Color.Transparent;

            HomePen2Player_lbl.Parent = HomePen2Image_lbl;
            HomePen2Player_lbl.Location = new Point(10, 5);
            HomePen2Player_lbl.BackColor = Color.Transparent;

            HomePen3Time_lbl.Parent = HomePen3Image_lbl;
            HomePen3Time_lbl.Location = new Point(44, 5);
            HomePen3Time_lbl.BackColor = Color.Transparent;

            HomePen3Player_lbl.Parent = HomePen3Image_lbl;
            HomePen3Player_lbl.Location = new Point(10, 5);
            HomePen3Player_lbl.BackColor = Color.Transparent;

            GuestPen1Time_lbl.Parent = GuestPen1Image_lbl;
            GuestPen1Time_lbl.Location = new Point(44, 5);
            GuestPen1Time_lbl.BackColor = Color.Transparent;

            GuestPen1Player_lbl.Parent = GuestPen1Image_lbl;
            GuestPen1Player_lbl.Location = new Point(10, 5);
            GuestPen1Player_lbl.BackColor = Color.Transparent;

            GuestPen2Time_lbl.Parent = GuestPen2Image_lbl;
            GuestPen2Time_lbl.Location = new Point(44, 5);
            GuestPen2Time_lbl.BackColor = Color.Transparent;

            GuestPen2Player_lbl.Parent = GuestPen2Image_lbl;
            GuestPen2Player_lbl.Location = new Point(10, 5);
            GuestPen2Player_lbl.BackColor = Color.Transparent;

            GuestPen3Time_lbl.Parent = GuestPen3Image_lbl;
            GuestPen3Time_lbl.Location = new Point(44, 5);
            GuestPen3Time_lbl.BackColor = Color.Transparent;

            GuestPen3Player_lbl.Parent = GuestPen3Image_lbl;
            GuestPen3Player_lbl.Location = new Point(10, 5);
            GuestPen3Player_lbl.BackColor = Color.Transparent;

            initValues();
        }

        private void initValues()
        {
            values = new int[(int)defs.END_OF_ENUM];
            timeouts = new int[(int)defs.END_OF_ENUM];

            data = new Queue<String>();
            for (int i = 0; i < (int)defs.END_OF_ENUM; i++) { 
                values[i] = 0;
                timeouts[i] = 0;
            }

            H1_pen = new penalty(HomePen1Image_lbl, HomePen1Time_lbl, HomePen1Player_lbl);
            H2_pen = new penalty(HomePen2Image_lbl, HomePen2Time_lbl, HomePen2Player_lbl);
            H3_pen = new penalty(HomePen3Image_lbl, HomePen3Time_lbl, HomePen3Player_lbl);

            G1_pen = new penalty(GuestPen1Image_lbl, GuestPen1Time_lbl, GuestPen1Player_lbl);
            G2_pen = new penalty(GuestPen2Image_lbl, GuestPen2Time_lbl, GuestPen2Player_lbl);
            G3_pen = new penalty(GuestPen3Image_lbl, GuestPen3Time_lbl, GuestPen3Player_lbl);

        }
        private void button1_Click(object sender, EventArgs e)
        {
            debugAddresses(Int32.Parse(debug_address.Text), Int32.Parse(debug_cnt.Text));
        }

        private void debug_clear_Click(object sender, EventArgs e)
        {
            debug_address.Text = "0";
            debug_cnt.Text = "0";
            debug_data0.Text = "0";
            debug_data1.Text = "0";
            debug_data2.Text = "0";
            debug_data3.Text = "0";
            debug_data4.Text = "0";
        }

        private void handleNewValue(int Adress, int Value)
        {
            if (Adress < (int)defs.END_OF_ENUM)
            {
                values[Adress] = Value;
                timeouts[Adress] = communicationTimeout;
      //          if (Adress >= Int32.Parse(filter_addr_start.Text) && Adress <= Int32.Parse(filter_addr_stop.Text)) {
             //       data.Enqueue(Adress.ToString() + " = " + values[Adress].ToString());
            //    }
            }
            else
            {
#if DEBUG
                MessageBox.Show("Address out of bounds");
#endif
            }
        }


        private void debugAddresses(int Adress, int cnt)
        {
            for (int i = 0; i < cnt; i++)
            {
                int Value = 0;

                if (i == 0)
                {
                    Value = Int32.Parse(debug_data0.Text);
                }
                if (i == 1)
                {
                    Value = Int32.Parse(debug_data1.Text);
                }
                if (i == 2)
                {
                    Value = Int32.Parse(debug_data2.Text);
                }
                if (i == 3)
                {
                    Value = Int32.Parse(debug_data3.Text);
                }
                Value = NewMethod(i, Value);

                handleNewValue(Adress, Value);
                Adress++;
            }
        }

        private int NewMethod(int i, int Value)
        {
            if (i == 4)
            {
                Value = Int32.Parse(debug_data4.Text);
            }

            return Value;
        }
        private void debug()
        {
            const int number = 26;
            // address, cnt, data1, data2, data3, data4, data5 
            List<message> messages = new List<message>();

            HomeTeam_edit.Text = "TAIF";
            GuestTeam_edit.Text = "MUP"; 

            cnt++;

            Random Seed = new Random();
            Random Rmatch10 = new Random(Seed.Next(0, 200));
            Random Rmatch1 = new Random(Seed.Next(0, 200));
            Random Rperiod = new Random(Seed.Next(0, 200));
            Random RHScore = new Random(Seed.Next(0, 200));
            Random RGScore = new Random(Seed.Next(0, 200));
            Random Rshots10 = new Random(Seed.Next(0, 200));
            Random Rshots1 = new Random(Seed.Next(0, 200));
            Random RGshots10 = new Random(Seed.Next(0, 200));
            Random RGshots1 = new Random(Seed.Next(0, 200));

            Random RpenNr10 = new Random(Seed.Next(0, 200));
            Random RpenNr1 = new Random(Seed.Next(0, 200));
          
            Random Rpensec10 = new Random(Seed.Next(0, 200));
            Random Rpensec1 = new Random(Seed.Next(0, 200));
            Random Rpenmin = new Random(Seed.Next(0, 200));
            Random RhomeScore10 = new Random(Seed.Next(0, 200));  

            cnt++;
            if((!boolValue) && (cnt > (5*4)))
            {
                period = Rperiod.Next(1, 9);
                match10 = Rmatch10.Next(0, 5);
                match1 = Rmatch1.Next(1, 9);
                HScore = RHScore.Next(0, 9);
                GScore = RGScore.Next(0, 9);
                shots10 = Rshots10.Next(0, 9);
                shots1 = Rshots1.Next(0, 9);
                Gshots10 = RGshots10.Next(0, 9);
                Gshots1 = RGshots1.Next(0, 9);
                penNr10 = RpenNr10.Next(0, 9);
                penNr1 = RpenNr1.Next(0, 9);
                penmin = Rpenmin.Next(0, 4);
                pensec10 = Rpensec10.Next(0, 5);
                pensec1 = Rpensec1.Next(0, 5);
                cnt =0;
                boolValue = true;
            }

                messages.Add(new message((int)defs.MATCH_PERIOD_10, 2, 0, period));

                if (skipTime10 == 0)
                {
                    messages.Add(new message((int)defs.MATCH_10_MIN, 2, match10, match1));
                }
                else
                {
                    skipTime10--;
                    messages.Add(new message((int)defs.MATCH_1_MIN, 1, match1));
                }
                messages.Add(new message((int)defs.MATCH_10_SEC, 2, match10, match1));

                messages.Add(new message((int)defs.HOME_SCORE_10, 3, 0, HScore, 0));

                messages.Add(new message((int)defs.GUEST_SCORE_10, 3, 0, GScore, 0));

                messages.Add(new message((int)defs.HOME_SHOTS_10, 2, shots10, shots1));
                messages.Add(new message((int)defs.GUEST_SHOTS_10, 2, Gshots10, Gshots1));

                messages.Add(new message((int)defs.HOME_PEN1_NR_10, 2, penNr10, penNr1));

                if (skipPen10 == 0)
                {
                    messages.Add(new message((int)defs.GUEST_PEN2_MINS, 3, penmin, pensec10, pensec1));
                }
                else
                {
                    skipPen10--;
                    messages.Add(new message((int)defs.GUEST_PEN2_MINS, 2, penmin, pensec10, pensec1));
            }

            textBox1.Text = timeouts[(int)defs.GUEST_PEN2_1_SEC].ToString();




            for (int index = 0; index < messages.Count; index++)
            {
                for (int i = 0; i < messages.ElementAt(index)._cnt; i++)
                {
                    switch (i)
                    {
                        case 0:
                            handleNewValue(messages.ElementAt(index)._address + i, messages.ElementAt(index)._data1);
                            break;
                        case 1:
                            handleNewValue(messages.ElementAt(index)._address + i, messages.ElementAt(index)._data2);
                            break;
                        case 2:
                            handleNewValue(messages.ElementAt(index)._address + i, messages.ElementAt(index)._data3);
                            break;
                        case 3:
                            handleNewValue(messages.ElementAt(index)._address + i, messages.ElementAt(index)._data4);
                            break;
                        case 4:
                            handleNewValue(messages.ElementAt(index)._address + i, messages.ElementAt(index)._data5);
                            break;
                    }
                }
            }
            messages.Clear();
        }

        private void timer250_Tick(object sender, EventArgs e)
        {
            //Debug mod 
            debug();

            for (int i = 0; i < (int)defs.END_OF_ENUM; i++)
            {
                if (timeouts[i] > 0)
                {
                    timeouts[i]--;
                }

                // -------------- write teams --------------
                HomeTeam_lbl.Text = HomeTeam_edit.Text;
                GuestTeam_lbl.Text = GuestTeam_edit.Text;

                colon = ":";

                //if(data.Count > 0)
                //listBox2.Items.Add(data.Dequeue());


                // -------------- update clock -------------- 
                if (timeouts[(int)defs.MATCH_10_MIN] > 0)
                { 
                    Time_lbl.Text = (values[(int)defs.MATCH_10_MIN] * 10 + values[(int)defs.MATCH_1_MIN]).ToString() +
                    colon + (values[(int)defs.MATCH_10_SEC] * 10 + values[(int)defs.MATCH_1_SEC]).ToString();
                }
                else
                {
                    Time_lbl.Text = (values[(int)defs.MATCH_1_MIN]).ToString() +
                    colon + (values[(int)defs.MATCH_10_SEC] * 10 + values[(int)defs.MATCH_1_SEC]).ToString();
                }
            }

            // -------------- update period -------------- 
            Period_lbl.Text =
                "P" + values[(int)defs.MATCH_PERIOD_1];

            // -------------- update shots -------------- 

            HomeTeamShots_lbl.Text =
                (values[(int)defs.HOME_SHOTS_10] * 10 + values[(int)defs.HOME_SHOTS_1]).ToString();

            GuestTeamShots_lbl.Text =
                (values[(int)defs.GUEST_SHOTS_10]* 10 + values[(int)defs.GUEST_SHOTS_1]).ToString();

            // -------------- update score -------------- 
            HomeScore_lbl.Text =
                (values[(int)defs.HOME_SCORE_100] * 100 + values[(int)defs.HOME_SCORE_10] * 10 + values[(int)defs.HOME_SCORE_1]).ToString();
            GuestScore_lbl.Text =
                (values[(int)defs.GUEST_SCORE_100] * 100 + values[(int)defs.GUEST_SCORE_10] * 10 + values[(int)defs.GUEST_SCORE_1]).ToString();

            // --------------  Home penalty 1 -------------- 

            if (timeouts[(int)defs.HOME_PEN1_NR_10] > 0)
            {
                HomePen1Player_lbl.Text =
               values[(int)defs.HOME_PEN1_NR_10].ToString() + values[(int)defs.HOME_PEN1_NR_1].ToString();
            }
            else
            {
                HomePen1Player_lbl.Text = values[(int)defs.HOME_PEN1_NR_1].ToString();
            }
            
            HomePen1Time_lbl.Text =
                values[(int)defs.HOME_PEN1_MINS].ToString() +
                    colon + values[(int)defs.HOME_PEN1_10_SEC].ToString() + values[(int)defs.HOME_PEN1_1_SEC].ToString();

            // -------------- Home penalty 2 -------------- 
            if (timeouts[(int)defs.HOME_PEN2_NR_10] > 0)
            {
                HomePen2Player_lbl.Text =
               values[(int)defs.HOME_PEN2_NR_10].ToString() + values[(int)defs.HOME_PEN2_NR_1].ToString();
            }
            else
            {
                HomePen2Player_lbl.Text = values[(int)defs.HOME_PEN2_NR_1].ToString();
            }
            HomePen2Time_lbl.Text =
                values[(int)defs.HOME_PEN2_MINS].ToString() +
                    colon + values[(int)defs.HOME_PEN2_10_SEC].ToString() + values[(int)defs.HOME_PEN2_1_SEC].ToString();

            // -------------- Home penalty 3 -------------- 
            HomePen3Player_lbl.Text =
                values[(int)defs.HOME_PEN3_NR_10].ToString() + values[(int)defs.HOME_PEN3_NR_1].ToString();
            HomePen3Time_lbl.Text =
                values[(int)defs.HOME_PEN3_MINS].ToString() +
                    colon + values[(int)defs.HOME_PEN3_10_SEC].ToString() + values[(int)defs.HOME_PEN3_1_SEC].ToString();

            // --------------  Guest penalty 1 -------------- 

            if (timeouts[(int)defs.GUEST_PEN1_NR_10] > 0)
            {
                GuestPen1Player_lbl.Text =
                    values[(int)defs.GUEST_PEN1_NR_10].ToString() + values[(int)defs.GUEST_PEN1_NR_1].ToString();
            }
            else
            {
                GuestPen1Player_lbl.Text = values[(int)defs.GUEST_PEN1_NR_1].ToString();
            }

            GuestPen1Time_lbl.Text =
                values[(int)defs.GUEST_PEN1_MINS].ToString() +
                    colon + values[(int)defs.GUEST_PEN1_10_SEC].ToString() + values[(int)defs.GUEST_PEN1_1_SEC].ToString();

            // -------------- Guest penalty 2 -------------- 
            if (timeouts[(int)defs.GUEST_PEN2_NR_10] > 0)
            {
                GuestPen2Player_lbl.Text =
                    values[(int)defs.GUEST_PEN2_NR_10].ToString() + values[(int)defs.GUEST_PEN2_NR_1].ToString();
            }
            else
            {
                GuestPen2Player_lbl.Text = values[(int)defs.GUEST_PEN2_NR_1].ToString();
            }


            GuestPen2Time_lbl.Text =
                values[(int)defs.GUEST_PEN2_MINS].ToString() +
                    colon + values[(int)defs.GUEST_PEN2_10_SEC].ToString() + values[(int)defs.GUEST_PEN2_1_SEC].ToString();

            // -------------- Guest penalty 3 -------------- 

            GuestPen3Player_lbl.Text =
                values[(int)defs.GUEST_PEN3_NR_10].ToString() + values[(int)defs.GUEST_PEN3_NR_1].ToString();
            GuestPen3Time_lbl.Text =
                values[(int)defs.GUEST_PEN3_MINS].ToString() +
                   colon + values[(int)defs.GUEST_PEN3_10_SEC].ToString() + values[(int)defs.GUEST_PEN3_1_SEC].ToString();

            H1_pen.setAlive(timeouts[(int)defs.HOME_PEN1_1_SEC] > 0);
            H2_pen.setAlive(timeouts[(int)defs.HOME_PEN2_1_SEC] > 0);
            G1_pen.setAlive(timeouts[(int)defs.GUEST_PEN1_1_SEC] > 0);
            G2_pen.setAlive(timeouts[(int)defs.GUEST_PEN2_1_SEC] > 0);


        }

        private void Connect_btn_Click(object sender, EventArgs e)
        {
            NC = new NauCom(Protocols.NG12, "ANY", NauBeeChannels.CHANNEL_0, true);
            NC.NauBeeDigitDotRelayUpdate += new NauCom.NauBeeDataUpdateEventHandler(NC_NauBeeDigitDotRelayUpdate);
            NC.NauBeeTextUpdate += new NauCom.NauBeeTextUpdateEventHandler(NC_NauBeeTextUpdate);
        }

        private void NC_NauBeeTextUpdate(int Adress, string Text)
        {
            Console.WriteLine(Text);
        }

        private static int OldDot253Val = 0;
        private static int OldDot254Val = 0;
        private void NC_NauBeeDigitDotRelayUpdate(EventType Type, int[] Adresses)
        {
            if (Type == EventType.Relay)
            {
                if (Adresses.Contains(0)) //Game Horn
                {
                    int Duration;
                    NC.GetRelay(0, out Duration);
                    if (Duration > 0) //Play sound!                    
                                      //  Console.WriteLine("HONK!! Remaining Duration: " + Duration + "00 msec");
                        data.Enqueue("HONK");
                }
            }
            if (Type == EventType.Digit)
            {
                string Data = "";
                for (int i = 0; i < Adresses.Length; i++)
                {
                    int Value;
                    bool Flash, ExtraVisble, ExtraFlash;
                    NC.GetDigit(Adresses[i], out Value, out Flash, out ExtraVisble, out ExtraFlash);
                    if (Value != 15) { 
                        handleNewValue(Adresses[i], Value);
                        Data += Value.ToString();
                        


                    }
                
                }
            }
            if (Type == EventType.Dot)
            {
                string Data1 = "";
                for (int i = 0; i < Adresses.Length; i++)
                {
                    int Value;
                    int Flash, ExtraVisble, ExtraFlash;
                    NC.GetDot(Adresses[i], out Value, out Flash);
                    if (Adresses[i] == 254 && Value != OldDot254Val)
                    {
                        OldDot254Val = Value;
                        Console.WriteLine(Type.ToString() + "_" + Adresses[i].ToString() + "_" + Value.ToString());
                    }
                    if (Adresses[i] == 253 && Value != OldDot253Val)
                    {
                        OldDot253Val = Value;
                        Console.WriteLine(Type.ToString() + "_" + Adresses[i].ToString() + "_" + Value.ToString());
                    }
                
                }
                data.Enqueue("DOt event");
                //Console.WriteLine(Type.ToString() + "_" + Adresses[0].ToString()+ "_" + Data.ToString());
            }
        }

        private void resetFilter_Click(object sender, EventArgs e)
        {
          
            data.Clear();
            listBox2.Items.Clear();
        }

        private void time_10_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(time_10.Text, out skipTime10);
        }

        private void pen10skip_TextChanged(object sender, EventArgs e)
        {
            Int32.TryParse(pen10skip.Text, out skipPen10);
            
        }
    }

    public class penalty
    {
        Label image_lbl;
        Label time_lbl;
        Label player_lbl;
        bool visible;

        public penalty(Label image, Label time, Label player)
        {
            visible = false;
            image_lbl = image;
            time_lbl = time;
            player_lbl = player;

            image_lbl.Visible = visible;
            time_lbl.Visible = visible;
            player_lbl.Visible = visible;
        }

        public void setAlive(bool alive)
        {
            if (alive)
            {
                image_lbl.Visible = true;
                time_lbl.Visible = true;
                player_lbl.Visible = true;

            }
            else
            {
                image_lbl.Visible = false;
                time_lbl.Visible = false;
                player_lbl.Visible = false;
            }
        }
    }
}

