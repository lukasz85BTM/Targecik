using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BunifuAnimatorNS;
using Bunifu.Framework;

namespace Targecik
{
    
    public partial class Targecik : Form
    {
        //Zegarek
        Timer t = new Timer();
        Timer tt = new Timer();
        

        //Linijka kodu CIEŃ OKNA
        #region CIEŃ OKNA - KOD
        private bool Drag;
        private int MouseX;
        private int MouseY;

        private const int WM_NCHITTEST = 0x84;
        private const int HTCLIENT = 0x1;
        private const int HTCAPTION = 0x2;

        private bool m_aeroEnabled;

        private const int CS_DROPSHADOW = 0x00020000;
        private const int WM_NCPAINT = 0x0085;
        private const int WM_ACTIVATEAPP = 0x001C;

        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);
        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);
        [System.Runtime.InteropServices.DllImport("dwmapi.dll")]

        public static extern int DwmIsCompositionEnabled(ref int pfEnabled);
        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect,
            int nTopRect,
            int nRightRect,
            int nBottomRect,
            int nWidthEllipse,
            int nHeightEllipse
            );

        public struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }
        protected override CreateParams CreateParams
        {
            get
            {
                m_aeroEnabled = CheckAeroEnabled();
                CreateParams cp = base.CreateParams;
                if (!m_aeroEnabled)
                    cp.ClassStyle |= CS_DROPSHADOW; return cp;
            }
        }
        private bool CheckAeroEnabled()
        {
            if (Environment.OSVersion.Version.Major >= 6)
            {
                int enabled = 0; DwmIsCompositionEnabled(ref enabled);
                return (enabled == 1) ? true : false;
            }
            return false;
        }
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_NCPAINT:
                    if (m_aeroEnabled)
                    {
                        var v = 2;
                        DwmSetWindowAttribute(this.Handle, 2, ref v, 4);
                        MARGINS margins = new MARGINS()
                        {
                            bottomHeight = 1,
                            leftWidth = 0,
                            rightWidth = 0,
                            topHeight = 0
                        }; DwmExtendFrameIntoClientArea(this.Handle, ref margins);
                    }
                    break;
                default: break;
            }
            base.WndProc(ref m);
            if (m.Msg == WM_NCHITTEST && (int)m.Result == HTCLIENT) m.Result = (IntPtr)HTCAPTION;
        }
        private void PanelMove_MouseDown(object sender, MouseEventArgs e)
        {
            Drag = true;
            MouseX = Cursor.Position.X - this.Left;
            MouseY = Cursor.Position.Y - this.Top;
        }
        private void PanelMove_MouseMove(object sender, MouseEventArgs e)
        {
            if (Drag)
            {
                this.Top = Cursor.Position.Y - MouseY;
                this.Left = Cursor.Position.X - MouseX;
            }
        }
        private void PanelMove_MouseUp(object sender, MouseEventArgs e) { Drag = false; }
        #endregion
        //Koniec linijki kodu CIEŃ OKNA

        
        public Targecik()
        {
            InitializeComponent();
            panel_czasdost.Visible = true;
            bunifuCustomLabel1.Visible = false;
            btn_menu.Visible = false; /* UKRYTY*/
            redlabel.Visible = false;
            panel1.Visible = true;
            czas1.Start();
        }

        

        private void btn_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        int minets = 0, sekends = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            minets = 0;
            sekends = 0;
            timer3.Start();
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer3.Stop();
            button1.Enabled = false;
            button2.Enabled = true;
        }

        private void timer3_Tick(object sender, EventArgs e) /* PROGRESBAR nieużywany */
        {
            sekends++;
            if (sekends == 60)
            {
                minets++;
                sekends = 0;
            }
            label33.Text = Convert.ToString(minets);
            label34.Text = Convert.ToString(sekends);
            guna2ProgressBar1.Value = minets;
            guna2ProgressBar2.Value = sekends;
        }

        private void Targecik_Load(object sender, EventArgs e)
        {
            timer3.Interval = 1000;
            label33.Text = "0"; label34.Text = "0";
            guna2ProgressBar1.Maximum = 59;
            guna2ProgressBar2.Maximum = 59;
            button1.Enabled = true;


            btn_oblicz.PerformClick(); /*<- automatyczne potwierdzenie Buttona*/
            this.KeyPreview = true;

            //Metoda Zegara
            t.Interval = 1000; // milisekundy
            t.Tick += new EventHandler(this.t_Tick);
            t.Start();
        }
        private void t_Tick(object sender, EventArgs e)
        {
            int hh = DateTime.Now.Hour;
            int mm = DateTime.Now.Minute;
            int ss = DateTime.Now.Second;

            string time = "";

            if (hh < 10)
            {
                time += "0" + hh;
            }
            else
            {
                time += hh;
            }
            time += ":";

            if (mm < 10)
            {
                time += "0" + mm;
            }
            else
            {
                time += mm;
            }
            time += ":";

            if (ss < 10)
            {
                time += "0" + ss;
            }
            else
            {
                time += ss;
            }
            // aktualizacja labela Zegar
            lab_czasACTUAL.Text = time;

        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            this.guna2ProgressBar1.Increment(1);
        }
        private void cb_cykl_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.guna2ProgressBar1.Value = int.Parse(lab_szacowanyTarget.Text);
            if (rb_bezczyszcz.Checked == true) // BEZ CZYSZCZENIA
            {
               if (cb_real.Checked == false)
                {
                    //Narzucenie Cyklu i liczenie targetu przy konkretnym OEE
                    if (cb_cykl.Text == "3,1")
                    {
                        Cykl_zadany.Text = "3,1";
                        lab_wydajnosc.Text = "86,253";

                    }
                    if (cb_cykl.Text == "3,2")
                    {
                        Cykl_zadany.Text = "3,2";
                        lab_wydajnosc.Text = "86,415";


                    }
                    if (cb_cykl.Text == "3,4")
                    {
                        Cykl_zadany.Text = "3,4";
                        lab_wydajnosc.Text = "86,450";


                    }
                    if (cb_cykl.Text == "3,6")
                    {
                        Cykl_zadany.Text = "3,6";
                        lab_wydajnosc.Text = "86,320";


                    }
                    if (cb_cykl.Text == "3,8")
                    {
                        Cykl_zadany.Text = "3,8";
                        lab_wydajnosc.Text = "86,481";


                    }
                    if (cb_cykl.Text == "4,0")
                    {
                        Cykl_zadany.Text = "4,0";
                        lab_wydajnosc.Text = "86,390";


                    }
                    if (cb_cykl.Text == "5,0")
                    {
                        Cykl_zadany.Text = "5,0";
                        lab_wydajnosc.Text = "86,440";


                    }

                    redlabel.Visible = false;
                    label10.ForeColor = Color.DarkGray;

                }
                if (cb_real.Checked == true)
                {
                    //Narzucenie Cyklu i liczenie targetu przy 100% OEE
                    if (cb_cykl.Text == "3,1")
                    {
                        Cykl_zadany.Text = "3,1";
                        lab_wydajnosc.Text = "100";


                    }
                    if (cb_cykl.Text == "3,2")
                    {
                        Cykl_zadany.Text = "3,2";
                        lab_wydajnosc.Text = "100";


                    }
                    if (cb_cykl.Text == "3,4")
                    {
                        Cykl_zadany.Text = "3,4";
                        lab_wydajnosc.Text = "100";


                    }
                    if (cb_cykl.Text == "3,6")
                    {
                        Cykl_zadany.Text = "3,6";
                        lab_wydajnosc.Text = "100";


                    }
                    if (cb_cykl.Text == "3,8")
                    {
                        Cykl_zadany.Text = "3,8";
                        lab_wydajnosc.Text = "100";


                    }
                    if (cb_cykl.Text == "4,0")
                    {
                        Cykl_zadany.Text = "4,0";
                        lab_wydajnosc.Text = "100";


                    }
                    if (cb_cykl.Text == "5,0")
                    {
                        Cykl_zadany.Text = "5,0";
                        lab_wydajnosc.Text = "100";


                    }

                    redlabel.Visible = true;
                    label10.ForeColor = Color.LimeGreen;

                    
                }
                
                //Wyliczenie TARGETU
                double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100));
                lab_targetCEL.Text = ((float)target).ToString();
                lab_targetCEL.Text = target.ToString("0");
            }
            if (rb_czyszczenie.Checked == true) // CZYSZCZENIE FPLM
            {
                if (cb_real.Checked == false)
                {
                    //Narzucenie Cyklu i liczenie targetu przy konkretnym OEE
                    if (cb_cykl.Text == "3,1")
                    {
                        Cykl_zadany.Text = "3,1";
                        lab_wydajnosc.Text = "86,253";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 150);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "3,2")
                    {
                        Cykl_zadany.Text = "3,2";
                        lab_wydajnosc.Text = "86,415";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 161);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "3,4")
                    {
                        Cykl_zadany.Text = "3,4";
                        lab_wydajnosc.Text = "86,450";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 150);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "3,6")
                    {
                        Cykl_zadany.Text = "3,6";
                        lab_wydajnosc.Text = "86,320";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 130);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "3,8")
                    {
                        Cykl_zadany.Text = "3,8";
                        lab_wydajnosc.Text = "86,481";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 150);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "4,0")
                    {
                        Cykl_zadany.Text = "4,0";
                        lab_wydajnosc.Text = "86,390";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 130);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "5,0")
                    {
                        Cykl_zadany.Text = "5,0";
                        lab_wydajnosc.Text = "86,440";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 108);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                }
                if (cb_real.Checked == true)
                {
                    //Narzucenie Cyklu i liczenie targetu przy 100% OEE
                    if (cb_cykl.Text == "3,1")
                    {
                        Cykl_zadany.Text = "3,1";
                        lab_wydajnosc.Text = "100";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 150);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "3,2")
                    {
                        Cykl_zadany.Text = "3,2";
                        lab_wydajnosc.Text = "100";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 161);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "3,4")
                    {
                        Cykl_zadany.Text = "3,4";
                        lab_wydajnosc.Text = "100";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 150);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "3,6")
                    {
                        Cykl_zadany.Text = "3,6";
                        lab_wydajnosc.Text = "100";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 130);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "3,8")
                    {
                        Cykl_zadany.Text = "3,8";
                        lab_wydajnosc.Text = "100";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 150);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "4,0")
                    {
                        Cykl_zadany.Text = "4,0";
                        lab_wydajnosc.Text = "100";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 130);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "5,0")
                    {
                        Cykl_zadany.Text = "5,0";
                        lab_wydajnosc.Text = "100";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 108);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                }
            }
            if (rb_zakonczenie.Checked == true)  // ZAKOŃCZENIE PRODUKCJI
            {
                if (cb_real.Checked == false)
                {
                    //Narzucenie Cyklu i liczenie targetu przy konkretnym OEE
                    if (cb_cykl.Text == "3,1")
                    {
                        Cykl_zadany.Text = "3,1";
                        lab_wydajnosc.Text = "86,253";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 850);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "3,2")
                    {
                        Cykl_zadany.Text = "3,2";
                        lab_wydajnosc.Text = "86,415";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 830);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "3,4")
                    {
                        Cykl_zadany.Text = "3,4";
                        lab_wydajnosc.Text = "86,450";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 780);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "3,6")
                    {
                        Cykl_zadany.Text = "3,6";
                        lab_wydajnosc.Text = "86,320";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 700);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "3,8")
                    {
                        Cykl_zadany.Text = "3,8";
                        lab_wydajnosc.Text = "86,481";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 700);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "4,0")
                    {
                        Cykl_zadany.Text = "4,0";
                        lab_wydajnosc.Text = "86,390";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 650);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "5,0")
                    {
                        Cykl_zadany.Text = "5,0";
                        lab_wydajnosc.Text = "86,440";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 518);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                }
                if (cb_real.Checked == true)
                {
                    //Narzucenie Cyklu i liczenie targetu przy 100% OEE
                    if (cb_cykl.Text == "3,1")
                    {
                        Cykl_zadany.Text = "3,1";
                        lab_wydajnosc.Text = "100";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 850);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "3,2")
                    {
                        Cykl_zadany.Text = "3,2";
                        lab_wydajnosc.Text = "100";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 830);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "3,4")
                    {
                        Cykl_zadany.Text = "3,4";
                        lab_wydajnosc.Text = "100";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 780);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "3,6")
                    {
                        Cykl_zadany.Text = "3,6";
                        lab_wydajnosc.Text = "100";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 700);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "3,8")
                    {
                        Cykl_zadany.Text = "3,8";
                        lab_wydajnosc.Text = "100";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 700);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "4,0")
                    {
                        Cykl_zadany.Text = "4,0";
                        lab_wydajnosc.Text = "100";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 650);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                    if (cb_cykl.Text == "5,0")
                    {
                        Cykl_zadany.Text = "5,0";
                        lab_wydajnosc.Text = "100";
                        //Wyliczenie TARGETU
                        double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100) - 518);
                        lab_targetCEL.Text = ((float)target).ToString();
                        lab_targetCEL.Text = target.ToString("0");

                    }
                }
            }

            lab_wykonanie.Text = (float.Parse(tb_plm.Text) - float.Parse(tb_ng.Text)).ToString();

            //Zliczanie strat w sztukach dla tb_plm
            tb_strata.Text = (float.Parse(lab_targetCEL.Text) - float.Parse(lab_wykonanie.Text)).ToString();

            double strata;
            strata = double.Parse(tb_strata.Text);
            if (strata >= 0)
            {
                tb_strata.Text = (float.Parse(lab_targetCEL.Text) - float.Parse(lab_wykonanie.Text)).ToString();
                tb_zysk.Text = 0.ToString();
            }
            else if (strata <= 0)
            {
                tb_zysk.Text = (float.Parse(lab_wykonanie.Text) - float.Parse(lab_targetCEL.Text)).ToString();
                tb_strata.Text = 0.ToString();
            }

            #region PRZELICZNIK SZTUK na CZAS
            //PRZELICZENIE SZTUK na DANY CZAS
            lab_target_12.Text = lab_targetCEL.Text;

            //double target1 = (float.Parse(lab_target_12.Text) / 12);
            //lab_target_1.Text = ((float)target1).ToString();              /*Można USUNĄĆ*/
            //lab_target_1.Text = Math.Ceiling(target1).ToString("0");

            double target1 = Convert.ToDouble(float.Parse(lab_target_12.Text) / 12);
            lab_target_1.Text = Math.Ceiling(target1).ToString();
            lab_target_1.Text = target1.ToString("0.0");

            lab_target_2.Text = (float.Parse(lab_target_1.Text) * 2).ToString();
            lab_target_3.Text = (float.Parse(lab_target_1.Text) * 3).ToString();
            lab_target_4.Text = (float.Parse(lab_target_1.Text) * 4).ToString();
            lab_target_5.Text = (float.Parse(lab_target_1.Text) * 5).ToString();
            lab_target_6.Text = (float.Parse(lab_target_1.Text) * 6).ToString();
            lab_target_7.Text = (float.Parse(lab_target_1.Text) * 7).ToString();
            lab_target_8.Text = (float.Parse(lab_target_1.Text) * 8).ToString();
            lab_target_9.Text = (float.Parse(lab_target_1.Text) * 9).ToString();
            lab_target_10.Text = (float.Parse(lab_target_1.Text) * 10).ToString();
            lab_target_11.Text = (float.Parse(lab_target_1.Text) * 11).ToString();

            // CZAS PONIŻEJ 1H
            double target030 = (float.Parse(lab_target_1.Text) / 2);
            lab_target_030.Text = ((float)target030).ToString();
            lab_target_030.Text = target030.ToString("0.0");

            double target015 = (float.Parse(lab_target_030.Text) / 2);
            lab_target_015.Text = ((float)target015).ToString();
            lab_target_015.Text = target015.ToString("0.0");

            double target010 = (float.Parse(lab_target_030.Text) / 3);
            lab_target_010.Text = ((float)target010).ToString();
            lab_target_010.Text = target010.ToString("0.0");

            double target05 = (float.Parse(lab_target_015.Text) / 3);
            lab_target_05.Text = ((float)target05).ToString();
            lab_target_05.Text = target05.ToString("0.0");

            double target01 = (float.Parse(lab_target_05.Text) / 5);
            lab_target_01.Text = ((float)target01).ToString();
            lab_target_01.Text = target01.ToString("0.0");

            #endregion
        }

        int maxNumberOfBlinksCount = 30;
        int totalBlinkCount = 0;

        private void btn_sprawdz_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer2.Start();

            try
            {
                double OEE = (float.Parse(lab_wykonanie.Text) / (float.Parse(lab_czasDost.Text) / double.Parse(Cykl_zadany.Text)) * 100);
                lab_oee.Text = ((int)OEE).ToString();
                lab_oee.Text = OEE.ToString("0.0");

                try
                {
                    double percentage;
                    percentage = double.Parse(lab_oee.Text);
                    if (percentage >= 86.5)
                    {
                        lab_oee.ForeColor = Color.Lime;
                        tb_plm.ForeColor = Color.White;
                    }
                    else if (percentage >= 84.5)
                    {
                        lab_oee.ForeColor = Color.White;
                        tb_plm.ForeColor = Color.White;
                    }
                    else if (percentage <= 84.5)
                    {
                        lab_oee.ForeColor = Color.Red;
                        tb_plm.ForeColor = Color.PaleVioletRed;
                    }

                }
                catch
                {
                    MessageBox.Show(" Wynik procentowy OEE nie został osiągnięty ", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                } /* PROCENTOWO OEE */

            }
            catch
            {
                MessageBox.Show("Najpierw wybierz Cykl maszyny, potrzebny do wyliczenia współczynnika", "UWAGA", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            } /* WSPÓŁCZYNNIK JAKOŚCI */
            try
            {
                if (lab_target_12.Text == "")
                {
                    lab_target_12.Text = "0.0";
                }
                else if (lab_target_12.Text != "0" | lab_target_12.Text != "")
                {
                    double wykon = Convert.ToDouble(100 * (float.Parse(lab_wykonanie.Text) / float.Parse(lab_target_12.Text)) / 100 *100);
                    lab_yield.Text = Math.Round(wykon).ToString();
                    lab_yield.Text = wykon.ToString("0.00");
                }
                try /*SPRAWDZENIE CZY PRZEKROCZENIE YIELD*/
                {
                    double percentage2;
                    percentage2 = double.Parse(lab_yield.Text);
                    if (percentage2 >= 99.81)
                    {
                        lab_yield.ForeColor = Color.Lime;
                        tb_ng.ForeColor = Color.White;
                    }
                    else if (percentage2 <= 99.81)
                    {
                        lab_yield.ForeColor = Color.Red;
                        tb_ng.ForeColor = Color.PaleVioletRed;
                    }
                }
                catch
                {

                }
            }
            catch
            {
                MessageBox.Show("Ups, coś poszło nie tak, sprawdź poprawność danych prawdopodonie nie podałeś wyników !", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            } /* WYKONANIE */
            try
            {
                lab_startaSekundy.Text = (int.Parse(lab_czasDost.Text) - int.Parse(lab_wykonanie.Text) * 100 / float.Parse(lab_wydajnosc.Text) * float.Parse(Cykl_zadany.Text)).ToString();

                double czasNG = (float.Parse(lab_startaSekundy.Text) / 60);
                lab_czasNG.Text = ((int)czasNG).ToString();
                lab_czasNG.Text = czasNG.ToString("0");

                lab_godzin.Text = (int.Parse(lab_czasNG.Text) / 60).ToString();

                double minut = (float.Parse(lab_startaSekundy.Text) % 3600 / 60);
                lab_minut.Text = ((int)minut).ToString();
                lab_minut.Text = minut.ToString("0");

                if (minut >= 0)
                {
                    lab_minut.Text = ((int)minut).ToString();
                    lab_minut.Text = minut.ToString("0");

                    if (int.Parse(lab_minut.Text) < 60)
                    {
                        lab_minut.Text = ((int)minut).ToString();
                        lab_minut.Text = minut.ToString("0");
                    }
                    else if (lab_minut.Text == "60")
                    {
                        lab_minut.Text = "0";
                    }
                }
                else if (minut <= 0)
                {
                    lab_mPlus.Text = (int.Parse(lab_minut.Text) * -1).ToString();
                    lab_minut.Text = "0";
                }

                if (int.Parse(lab_godzin.Text) > 0)
                {
                    lab_godzin.Text = (int.Parse(lab_czasNG.Text) / 60).ToString();
                }
                else if (int.Parse(lab_godzin.Text) < 0)
                {
                    lab_godzin.Text = "0".ToString();
                }
               
            }
            catch
            {
                MessageBox.Show("Brak danych, sprawdź poprawność zapisu", "UWAGA", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            } /* STARTA CZASOWA W MINUTACH */
            
        }
    
        private void list_up_Click(object sender, EventArgs e)
        {
            panel_czasdost.Visible = false;
            list_down.Visible = true;
            list_up.Visible = false;
        }

        private void list_down_Click(object sender, EventArgs e)
        {
            panel_czasdost.Visible = true;
            list_up.Visible = true;
            list_down.Visible = false;
        }

        private void btn_minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btn_oblicz_Click(object sender, EventArgs e)
        {
            if (tb_czasDostepny.Text == "")
            {
                this.Godziny.Text = "Brak danych";
                this.Minuty.Text = "Brak danych";
                MessageBox.Show("Komórka nie może pozostać pusta!", "UWAGA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            try
            {
                Godziny.Text = (int.Parse(tb_czasDostepny.Text) / 60 / 60 + " godz.").ToString();
                Minuty.Text = (int.Parse(tb_czasDostepny.Text) % 3600 / 60 + " min.").ToString();

                tb_godz.Text = (int.Parse(tb_czasDostepny.Text) / 60 / 60).ToString();
                tb_min.Text = (int.Parse(tb_czasDostepny.Text) % 3600 / 60).ToString();
                lab_sekundy.Text = (tb_czasDostepny.Text) + " sek.".ToString();
                tb_minuty.Text = (int.Parse(tb_czasDostepny.Text) / 60).ToString();
                lab_godziny2.Text = (int.Parse(tb_czasDostepny.Text) / 60 / 60 + " godz.").ToString();

                //Przerzucenie Czasu dostępnego do głownego Wyświetlenia
                lab_czasDost.Text = tb_czasDostepny.Text;
            }
            catch
            {

            }
        }

        private void btn_oblicz2_Click(object sender, EventArgs e)
        {
            if (tb_godz.Text == "" || tb_min.Text == "")
            {
                this.lab_sekundy.Text = "Brak danych";
                MessageBox.Show("Komórka nie może pozostać pusta!", "UWAGA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            try
            {
                lab_sekundy.Text = (int.Parse(tb_godz.Text) * 60 * 60 + (float.Parse(tb_min.Text) * 60)).ToString();
                tb_czasDostepny.Text = int.Parse(lab_sekundy.Text).ToString();

                Godziny.Text = (int.Parse(tb_czasDostepny.Text) / 60 / 60 + " godz.").ToString();
                Minuty.Text = (int.Parse(tb_czasDostepny.Text) % 3600 / 60 + " min.").ToString();
                tb_minuty.Text = (int.Parse(tb_czasDostepny.Text) / 60).ToString();
                lab_godziny2.Text = (int.Parse(tb_minuty.Text) / 60).ToString();

                btn_oblicz.PerformClick(); /*<- automatyczne potwierdzenie Buttona*/
            }
            catch
            {

            }
        }

        private void btn_oblicz3_Click(object sender, EventArgs e)
        {
            if (tb_minuty.Text == "")
            {
                this.lab_godziny2.Text = "Brak danych";
                MessageBox.Show("Komórka nie może pozostać pusta!", "UWAGA!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            try
            {
                lab_godziny2.Text = (int.Parse(tb_minuty.Text) / 60).ToString();
                tb_czasDostepny.Text = (int.Parse(tb_minuty.Text) * 60).ToString();
                tb_godz.Text = (int.Parse(tb_minuty.Text) / 60).ToString();
                lab_sekundy.Text = (int.Parse(tb_minuty.Text) * 60).ToString();
                tb_min.Text = (float.Parse(tb_czasDostepny.Text) % 3600 / 60).ToString();

                Godziny.Text = (int.Parse(tb_czasDostepny.Text) / 60 / 60 + " godz.").ToString();
                Minuty.Text = (int.Parse(tb_czasDostepny.Text) % 3600 / 60 + " min.").ToString();

                btn_oblicz.PerformClick(); /*<- automatyczne potwierdzenie Buttona*/
            }
            catch
            {

            }
        }

        private void cb_czasPrzezbrojenie_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                #region Wyliczenie straty dostosowanej do przezbrojenia

                if (cb_czasPrzezbrojenie.Text == "" || cb_czasPrzezbrojenie.Text == "0")
                {
                    lab_przezbrojenieCzas.Text = "0 minut";
                    lab_czasDost.Text = "43200";
                    lab_infoP.Text = "Brak przezbrojenia";
                    lab_InfoPrzezbrojenie.Text = "Bez przezbrojenia";

                    //Wyliczenie TARGETU
                    double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100));
                    lab_targetCEL.Text = ((float)target).ToString();
                    lab_targetCEL.Text = target.ToString("0");
                }
                if (cb_czasPrzezbrojenie.Text == "30")
                {
                    lab_przezbrojenieCzas.Text = "30 minut";
                    lab_czasDost.Text = (float.Parse(tb_czasDostepny.Text) - float.Parse(cb_czasPrzezbrojenie.Text) * 60).ToString();
                    lab_infoP.Text = "0 godzin 30 minut";
                    lab_InfoPrzezbrojenie.Text = "Rozstaw";

                    //Wyliczenie TARGETU
                    double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100));
                    lab_targetCEL.Text = ((float)target).ToString();
                    lab_targetCEL.Text = target.ToString("0");
                }
                if (cb_czasPrzezbrojenie.Text == "50")
                {
                    lab_przezbrojenieCzas.Text = "50 minut";
                    lab_czasDost.Text = (float.Parse(tb_czasDostepny.Text) - float.Parse(cb_czasPrzezbrojenie.Text) * 60).ToString();
                    lab_infoP.Text = "0 godzin 50 minut";
                    lab_InfoPrzezbrojenie.Text = "Rozstaw + Program";

                    //Wyliczenie TARGETU
                    double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100));
                    lab_targetCEL.Text = ((float)target).ToString();
                    lab_targetCEL.Text = target.ToString("0");
                }
                if (cb_czasPrzezbrojenie.Text == "55")
                {
                    lab_przezbrojenieCzas.Text = "55 minut";
                    lab_czasDost.Text = (float.Parse(tb_czasDostepny.Text) - float.Parse(cb_czasPrzezbrojenie.Text) * 60).ToString();
                    lab_infoP.Text = "0 godzin 55 minut";
                    lab_InfoPrzezbrojenie.Text = "Rozstaw + Inny rodzaj cementu";

                    //Wyliczenie TARGETU
                    double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100));
                    lab_targetCEL.Text = ((float)target).ToString();
                    lab_targetCEL.Text = target.ToString("0");
                }
                if (cb_czasPrzezbrojenie.Text == "70")
                {
                    lab_przezbrojenieCzas.Text = "70 minut";
                    lab_czasDost.Text = (float.Parse(tb_czasDostepny.Text) - float.Parse(cb_czasPrzezbrojenie.Text) * 60).ToString();
                    lab_infoP.Text = "1 godzina 10 minut";
                    lab_InfoPrzezbrojenie.Text = "Rozstaw + Program + Inny rodzaj cementu ";

                    //Wyliczenie TARGETU
                    double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100));
                    lab_targetCEL.Text = ((float)target).ToString();
                    lab_targetCEL.Text = target.ToString("0");
                }
                if (cb_czasPrzezbrojenie.Text == "80")
                {
                    lab_przezbrojenieCzas.Text = "80 minut";
                    lab_czasDost.Text = (float.Parse(tb_czasDostepny.Text) - float.Parse(cb_czasPrzezbrojenie.Text) * 60).ToString();
                    lab_infoP.Text = "1 godzina 20 minut";
                    lab_InfoPrzezbrojenie.Text = "Rozstaw + Foremki";

                    //Wyliczenie TARGETU
                    double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100));
                    lab_targetCEL.Text = ((float)target).ToString();
                    lab_targetCEL.Text = target.ToString("0");
                }
                if (cb_czasPrzezbrojenie.Text == "90")
                {
                    lab_przezbrojenieCzas.Text = "90 minut";
                    lab_czasDost.Text = (float.Parse(tb_czasDostepny.Text) - float.Parse(cb_czasPrzezbrojenie.Text) * 60).ToString();
                    lab_infoP.Text = "1 godzina 30 minut";
                    lab_InfoPrzezbrojenie.Text = "Rozstaw + Foremki + Inny rodzaj cementu";

                    //Wyliczenie TARGETU
                    double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100));
                    lab_targetCEL.Text = ((float)target).ToString();
                    lab_targetCEL.Text = target.ToString("0");
                }
                if (cb_czasPrzezbrojenie.Text == "110")
                {
                    lab_przezbrojenieCzas.Text = "110 minut";
                    lab_czasDost.Text = (float.Parse(tb_czasDostepny.Text) - float.Parse(cb_czasPrzezbrojenie.Text) * 60).ToString();
                    lab_infoP.Text = "1 godzina 50 minut";
                    lab_InfoPrzezbrojenie.Text = "Rozstaw + Inny rodzaj cementu + Mycie instalacji";

                    //Wyliczenie TARGETU
                    double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100));
                    lab_targetCEL.Text = ((float)target).ToString();
                    lab_targetCEL.Text = target.ToString("0");
                }
                if (cb_czasPrzezbrojenie.Text == "150")
                {
                    lab_przezbrojenieCzas.Text = "150 minut";
                    lab_czasDost.Text = (float.Parse(tb_czasDostepny.Text) - float.Parse(cb_czasPrzezbrojenie.Text) * 60).ToString();
                    lab_infoP.Text = "2 godziny 30 minut";
                    lab_InfoPrzezbrojenie.Text = "Rozstaw + Foremki + Inny rodzaj cementu + Mycie instalacji";

                    //Wyliczenie TARGETU
                    double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100));
                    lab_targetCEL.Text = ((float)target).ToString();
                    lab_targetCEL.Text = target.ToString("0");
                }
                if (cb_czasPrzezbrojenie.Text == "180")
                {
                    lab_przezbrojenieCzas.Text = "180 minut";
                    lab_czasDost.Text = (float.Parse(tb_czasDostepny.Text) - float.Parse(cb_czasPrzezbrojenie.Text) * 60).ToString();
                    lab_infoP.Text = "3 godziny 0 minut";
                    lab_InfoPrzezbrojenie.Text = "Rozstaw + Foremki + Regulacje PTSM oraz PLM";

                    //Wyliczenie TARGETU
                    double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100));
                    lab_targetCEL.Text = ((float)target).ToString();
                    lab_targetCEL.Text = target.ToString("0");
                }
                if (cb_czasPrzezbrojenie.Text == "235")
                {
                    lab_przezbrojenieCzas.Text = "235 minut";
                    lab_czasDost.Text = (float.Parse(tb_czasDostepny.Text) - float.Parse(cb_czasPrzezbrojenie.Text) * 60).ToString();
                    lab_infoP.Text = "3 godziy 55 minut";
                    lab_InfoPrzezbrojenie.Text = "Rozstaw + Foremki + Inny rodzaj cementu + Mycie instalacji + Regulacje PTSM,PLM";

                    //Wyliczenie TARGETU
                    double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100));
                    lab_targetCEL.Text = ((float)target).ToString();
                    lab_targetCEL.Text = target.ToString("0");
                }
                if (cb_czasPrzezbrojenie.Text == "360")
                {
                    lab_przezbrojenieCzas.Text = "360 minut";
                    lab_czasDost.Text = (float.Parse(tb_czasDostepny.Text) - float.Parse(cb_czasPrzezbrojenie.Text) * 60).ToString();
                    lab_infoP.Text = "6 godzin 0 minut";
                    lab_InfoPrzezbrojenie.Text = "Przezbrojenie FLEX";

                    //Wyliczenie TARGETU
                    double target = (float.Parse(lab_wydajnosc.Text) * (float.Parse(lab_czasDost.Text) / float.Parse(Cykl_zadany.Text) / 100));
                    lab_targetCEL.Text = ((float)target).ToString();
                    lab_targetCEL.Text = target.ToString("0");
                }

                #endregion

                lab_wykonanie.Text = (float.Parse(tb_plm.Text) - float.Parse(tb_ng.Text)).ToString();
                //Zliczanie strat w sztukach dla tb_plm
                tb_strata.Text = (float.Parse(lab_targetCEL.Text) - float.Parse(lab_wykonanie.Text)).ToString();

                double strata;
                strata = double.Parse(tb_strata.Text);
                if (strata >= 0)
                {
                    tb_strata.Text = (float.Parse(lab_targetCEL.Text) - float.Parse(lab_wykonanie.Text)).ToString();
                    tb_zysk.Text = 0.ToString();
                }
                else if (strata <= 0)
                {
                    tb_zysk.Text = (float.Parse(lab_wykonanie.Text) - float.Parse(lab_targetCEL.Text)).ToString();
                    tb_strata.Text = 0.ToString();
                }

                if (cb_czasPrzezbrojenie.Text == "0")
                {
                    rb_bezczyszcz.Enabled = true;
                    rb_czyszczenie.Enabled = true;
                    rb_zakonczenie.Enabled = true;
                }
                else if (cb_czasPrzezbrojenie.Text != "0")
                {
                    rb_bezczyszcz.Enabled = false;
                    rb_czyszczenie.Enabled = false;
                    rb_zakonczenie.Enabled = false;
                }
                btn_sprawdz.PerformClick();
            }
            catch
            {
                MessageBox.Show("Uwaga sprawdź poprawność znaków!","Informacja",MessageBoxButtons.OK,MessageBoxIcon.Warning);
            }
 
        }

        private void tb_plm_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (tb_plm.Text == "0" | tb_plm.Text == "")
                {
                    tb_plm.Text = "0";
                }
                else
                { 
                    lab_wykonanie.Text = (float.Parse(tb_plm.Text) - float.Parse(tb_ng.Text)).ToString(); 
                
                }

                //Zliczanie strat w sztukach dla tb_plm
                tb_strata.Text = (float.Parse(lab_targetCEL.Text) - float.Parse(lab_wykonanie.Text)).ToString();                 
                
                double strata;
                strata = double.Parse(tb_strata.Text);
                if (strata >= 0)
                {
                    tb_strata.Text = (float.Parse(lab_targetCEL.Text) - float.Parse(lab_wykonanie.Text)).ToString();
                    tb_zysk.Text = 0.ToString();
                }
                else if (strata <= 0)
                {
                    tb_zysk.Text = (float.Parse(lab_wykonanie.Text) - float.Parse(lab_targetCEL.Text)).ToString();
                    tb_strata.Text = 0.ToString();
                }
                
            }
            catch
            {
                MessageBox.Show(" Nie wybrano cyklu w jakim dana maszyna ma pracować. Pole musi zawierać cyfry od 0-9, również nie może pozostać puste ponieważ program na bierząco zlicza czas ", "Informacja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void tb_ng_TextChanged(object sender, EventArgs e)
        {
            try
            {
                lab_wykonanie.Text = (float.Parse(tb_plm.Text) - float.Parse(tb_ng.Text)).ToString();

                //Zliczanie strat w sztukach dla tb_ng
                tb_strata.Text = (float.Parse(lab_targetCEL.Text) - float.Parse(lab_wykonanie.Text)).ToString();

                double strata;
                strata = double.Parse(tb_strata.Text);
                if (strata >= 0)
                {
                    tb_strata.Text = (float.Parse(lab_targetCEL.Text) - float.Parse(lab_wykonanie.Text)).ToString();
                    tb_zysk.Text = 0.ToString();
                }
                else if (strata <= 0)
                {
                    tb_zysk.Text = (float.Parse(lab_wykonanie.Text) - float.Parse(lab_targetCEL.Text)).ToString();
                    tb_strata.Text = 0.ToString();
                }
            }
            catch
            {
                MessageBox.Show("Te pole nie może pozostać puste","Uwaga",MessageBoxButtons.OK,MessageBoxIcon.Asterisk);
            }
         
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            animacja1.Show(bunifuCustomLabel1);
            bunifuCustomLabel1.Visible = true;
            totalBlinkCount++;
            
            if (totalBlinkCount == maxNumberOfBlinksCount)
            {
                animacja1.Hide(bunifuCustomLabel1);
                timer1.Stop();
                bunifuCustomLabel1.Visible = false;
                totalBlinkCount = 0;
            }
        }

        private void cb_real_CheckedChanged(object sender, EventArgs e)
        {
            if (cb_real.Checked == false)
            {
                animacja1.Hide(redlabel);
                redlabel.Visible = false;
                label7.Visible = true;
                cb_real.BackColor = Color.Transparent;
                cb_real.ForeColor = Color.Silver;
            }
            if (cb_real.Checked == true)
            {
                animacja1.Show(redlabel);
                redlabel.Visible = true;
                label7.Visible = false;
                cb_real.BackColor = Color.YellowGreen;
                cb_real.ForeColor = Color.Black;
            }
        }

        private void czas1_Tick(object sender, EventArgs e)
        {
            DateTime dateTime = DateTime.Now;
            DateTime timeStart = dateTimePicker1.Value;
            DateTime timeStop = dateTimePicker2.Value;
            timeStop = DateTime.Now;
            
            TimeSpan difference = timeStop - timeStart;
            lab_time.Text = dateTime.ToString();

            double hour = difference.TotalHours;
            lab_hour.Text = ((int)hour).ToString();  /* RZUTOWANIE DO INT */
            lab_hour.Text = Math.Round(hour).ToString();
            lab_hour.Text = hour.ToString("0");
        }

        private void panel_menu_DoubleClick(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void lab_czasACTUAL_TextChanged(object sender, EventArgs e)
        {
            try
            {
                DateTime timeStart = dateTimePicker1.Value;
                DateTime timeStop = dateTimePicker2.Value;
                timeStop = DateTime.Now;

                TimeSpan difference = timeStop - timeStart;
                lab_czasACTUAL.Text = DateTime.Now.ToString();

                double hour = difference.TotalHours;
                lab_hour.Text = ((int)hour).ToString();  /* RZUTOWANIE DO INT */
                lab_hour.Text = Math.Ceiling(hour).ToString();
                lab_hour.Text = hour.ToString("0.0");

                /* CZAS POZOSTAŁY W MINUTACH */
                lab_czasPozostaly.Text = (float.Parse(lab_czasDost.Text) - (hour * 60 * 60)).ToString();
                double CzasPozostaly = (float.Parse(lab_czasPozostaly.Text) / 60);
                lab_czasPozostaly.Text = ((int)CzasPozostaly).ToString();  /* RZUTOWANIE DO INT */
                lab_czasPozostaly.Text = CzasPozostaly.ToString("0");
                lab_czasPozostalySEK.Text = (int.Parse(lab_czasDost.Text) - (hour * 60 * 60/*int.Parse(lab_czasPozostaly.Text*/)).ToString("0");

                lab_resztaCzasu.Text = (float.Parse(lab_czasPozostaly.Text) / 60).ToString("00");
                lab_minuts2.Text = (float.Parse(lab_czasPozostalySEK.Text) % 3600 / 60).ToString("0");

                /* CZAS PRACY */
                double Szacowany = (hour * 60 * 60);
                lab_szacowanyTarget.Text = ((int)Szacowany).ToString();  /* RZUTOWANIE DO INT */
                lab_szacowanyTarget.Text = Szacowany.ToString("0");

                guna2ProgressBar3.Value = int.Parse(lab_szacowanyTarget.Text); /* TEST */

                lab_minutsP.Text = (Szacowany % 3600 / 60).ToString("0");

                double czasPracy = (Szacowany / 60);
                lab_czasPracyMIN.Text = ((int)czasPracy).ToString();
                lab_czasPracyMIN.Text = czasPracy.ToString("0");

                double praca = (float.Parse(lab_wykonanie.Text) + (float.Parse(lab_czasPozostaly.Text) - float.Parse(tb_przerwa.Text)) * float.Parse(lab_target_01.Text));
                lab_czasPracyGodz.Text = ((int)praca).ToString();  /* RZUTOWANIE DO INT */
                lab_czasPracyGodz.Text = praca.ToString("0" + " szt.");

                //Czas do końca uzyskany z przezbrojenia
                double PrzezbrojenieCzas = (float.Parse(lab_czasPozostaly.Text) - (float.Parse(cb_czasPrzezbrojenie.Text) / 60));
                lab_czasPrzezbrojenia.Text = ((int)PrzezbrojenieCzas).ToString();  /* RZUTOWANIE DO INT */
                lab_czasPrzezbrojenia.Text = Math.Round(PrzezbrojenieCzas).ToString();
                lab_czasPrzezbrojenia.Text = PrzezbrojenieCzas.ToString("0");

            }
            catch
            {

            } /* RÓŻNICA CZASÓW */
        }

        private void guna2ProgressBar2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label29_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }

        private void btn_menu_Click(object sender, EventArgs e)
        {
            if (panel1.Height == 590)
            {
                panel1.Visible = false;
                panel1.Height = 0;
                
            }
            else
            {
                panel1.Visible = true;
                panel1.Height = 590;
                
            }
        }

       
    }
}
