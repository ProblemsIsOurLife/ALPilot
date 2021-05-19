
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ALPilot
{


    public partial class Form1 : Form
    {
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter,
            string lpszClass, string lpszWindow);
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        const int WM_KEYDOWN = 0x0100;
        const int WM_KEYUP = 0x0101;
        const int WM_LBUTTONDOWN = 0x0201;
        const int WM_LBUTTONUP = 0x0202;
        const int WM_MOUSEMOVE = 0x0200;
        const int VK_A = 0x41;
        const int VK_B = 0x42;
        const int VK_C = 0x43;
        const int VK_D = 0x44;
        const int VK_E = 0x45;
        const int VK_F = 0x46;
        const int VK_G = 0x47;
        const int VK_H = 0x48;
        const int VK_I = 0x49;
        const int VK_J = 0x4A;
        const int VK_K = 0x4B;
        const int VK_L = 0x4C;
        const int VK_M = 0x4D;
        const int VK_N = 0x4E;
        const int VK_O = 0x4F;
        const int VK_P = 0x50;
        const int VK_Q = 0x51;
        const int VK_R = 0x52; // taken from http://msdn.microsoft.com/en-us/library/dd375731(v=vs.85).aspx
        const int VK_S = 0x53;
        const int VK_T = 0x54;
        const int VK_U = 0x55;
        const int VK_V = 0x56;
        const int VK_W = 0x57;
        const int VK_X = 0x58;
        const int VK_Y = 0x59;
        const int VK_Z = 0x5A;
        const int VK_0 = 0x30;
        const int VK_1 = 0x31;
        const int VK_2 = 0x32;
        const int VK_3 = 0x33;
        const int VK_4 = 0x34;
        const int VK_5 = 0x35;
        const int VK_6 = 0x36;
        const int VK_7 = 0x37;
        const int VK_8 = 0x38;
        const int VK_9 = 0x39;
        const int VK_PGUP = 0x21;
        const int VK_SPACE = 0x20;
        const int VK_OEM_MINUS = 0xBD;
        const int VK_SHIFT = 0x10;
        [DllImport("user32", SetLastError = true)]
        public static extern IntPtr SendMessage(
            IntPtr hWnd,
            uint Msg,
            int wParam,
            IntPtr lParam);
        static IntPtr MakeLParam(int x, int y) => (IntPtr)((y << 16) | (x & 0xFFFF));
        static int[] worktimer = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

   
        string[] installs = new string[] { "entergame", "mine", "flyto", "planetsrefresh", "freespace" };

        static int maxkolall = 0;
        
      
        static int[] buttons = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };


        static int[] tasks = new int[10];
        bot[] boti = new bot[10];
        public enum State
        {
            run,
            stop,
            pause
        }
        static CancellationTokenSource[] source2= new CancellationTokenSource[10];
        static CancellationTokenSource[] source3 = new CancellationTokenSource[10];
        static CancellationToken[] token2 = new CancellationToken[10];
        static CancellationToken[] token3 = new CancellationToken[10];
        static private SynchronizationContext _synchronizationContext ;
        static string[] logs = new string[10];
        static int[] localopened = new int[10];
        static int[] leaved = new int[10];
        static int leaveall = 0;
        static int leavedfirstpart=0;
        static int leavedsecondpart = 0;
        public class bot
        {
            Tesseract Tesseract1;
            Tesseract Tesseract2;
            string[] minerals = new string[10];
            IntPtr HWIDm;
            IntPtr HWIDch;
            List<string> locfarm = new List<string>();
            CancellationToken token;
            int kollocfarm;
            int tocorp;
            string locsell;
            string rudetomine;
            int gunscol;
            int threadnumber;
            int ciklzapuskov = 0;
            int numberloc = 0;
            static Form form;
            string adress = "";
            public State _state;
            int rezhims = 0;
            public Task i,jj;
            Process[] All3Proc = new Process[10];
            public bot(object obj, Form form1)
            {


                int[] temppp = (int[])obj;
                threadnumber = temppp[0];
                int tolive = temppp[1]; 
                form = form1;
                token = token2[threadnumber];
               
                string loglabel = "log" + threadnumber.ToString();
                string statusis = "status" + (threadnumber + 1).ToString();
                Task.Delay(1000).Wait();
                Tesseract1 = new Tesseract(".\\", "rus", OcrEngineMode.TesseractOnly);
                Tesseract1.SetVariable("tessedit_char_whitelist", "0123456789");
                Tesseract2 = new Tesseract(".\\", "rus", OcrEngineMode.TesseractOnly);
               
                string starts = "start" + (threadnumber + 1).ToString();
                string stops = "stop" + (threadnumber + 1).ToString();
                string pauses = "pause" + (threadnumber + 1).ToString();
                bool work = true, closedall = false;
                if (tolive == 1)
                {
                    token = token3[threadnumber];
                    jj = Task.Factory.StartNew(() => leavew(threadnumber));
                    form.Controls[starts].Invoke((MethodInvoker)(() => form.Controls[starts].Enabled = true));
                    form.Controls[pauses].Invoke((MethodInvoker)(() => form.Controls[pauses].Enabled = false));
                    form.Controls[stops].Invoke((MethodInvoker)(() => form.Controls[stops].Enabled = false));
                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                }
                else
                try
                {
                            locfarm.Add("");
                            locfarm.Add("");
                            locfarm.Add("");
                            locfarm.Add("");
                            locfarm.Add("");
                            locfarm.Add("");
                            locfarm.Add("");
                            
                            i = Task.Factory.StartNew(() => mainw(threadnumber),TaskCreationOptions.LongRunning);

                        }
                        catch (Exception)
                        {
                            if (token.IsCancellationRequested)
                            {

                            logs[threadnumber]= "stopped";
                            form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                            }
                            form.Controls[starts].Invoke((MethodInvoker)(() => form.Controls[starts].Enabled = true));
                            form.Controls[pauses].Invoke((MethodInvoker)(() => form.Controls[pauses].Enabled = false));
                            form.Controls[stops].Invoke((MethodInvoker)(() => form.Controls[stops].Enabled = false));
                            form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));

                }

            }
            static object locker = new object();
            public void leavew(int thr)
            {
                threadnumber = thr;
                string windowname = (thr + 1).ToString();
                HWIDm = FindWindow("Qt5QWindowIcon", windowname);

                if (HWIDm == IntPtr.Zero)
                {
                    return;
                }
                HWIDch = FindWindowEx(HWIDm, IntPtr.Zero, "Qt5QWindowIcon", "ScreenBoardClassWindow");
                leavegame();
            }
            public void mainw(object dat)
            {

                threadnumber = (int)dat;
               
                string shopsname = "shops" + threadnumber.ToString();
                string starts = "start" + (threadnumber + 1).ToString();
                string stops = "stop" + (threadnumber + 1).ToString();
                string pauses = "pause" + (threadnumber + 1).ToString();
                string loglabel = "log" + threadnumber.ToString();
                bool work = true, closedall = false;
                string statusis = "status" + (threadnumber + 1).ToString();
                int farmres, flytobaseres, planetres, enterres;
                Point[] center = new Point[10];
                string windowname = (threadnumber + 1).ToString();
                int configres = 0;
                configres=config();
                if (configres == -1)
                {
                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                    form.Controls[starts].Invoke((MethodInvoker)(() => form.Controls[starts].Enabled = true));
                    form.Controls[pauses].Invoke((MethodInvoker)(() => form.Controls[pauses].Enabled = false));
                    form.Controls[stops].Invoke((MethodInvoker)(() => form.Controls[stops].Enabled = false));
                    _state = State.stop;
                    return;
                }
                try { 
                    form.Controls[shopsname].Invoke((MethodInvoker)(() => adress = form.Controls[shopsname].Text));
                    HWIDm = FindWindow("Qt5QWindowIcon", windowname);

                    if (HWIDm == IntPtr.Zero)
                    {
                        MessageBox.Show("cant find nox window", "logor", MessageBoxButtons.OK);
                        form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                        form.Controls[starts].Invoke((MethodInvoker)(() => form.Controls[starts].Enabled = true));
                        form.Controls[pauses].Invoke((MethodInvoker)(() => form.Controls[pauses].Enabled = false));
                       form.Controls[stops].Invoke((MethodInvoker)(() => form.Controls[stops].Enabled = false));
                        _state = State.stop;
                        return;
                    }
                    HWIDch = FindWindowEx(HWIDm, IntPtr.Zero, "Qt5QWindowIcon", "ScreenBoardClassWindow");
                    int kolzap = 0;

                    string scriptes = "script" + (threadnumber + 1).ToString();
                    string targetedscript = "";
                    form.Controls[scriptes].Invoke((MethodInvoker)(() => targetedscript = form.Controls[scriptes].Text));
                    switch (targetedscript)
                    {

                    case "entergame":
                        {

                            form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                            enterres = enter();
                                if (enterres == -1)
                                {

                                    logs[threadnumber]="closed";
                                    _state = State.stop;

                                }
                                if (enterres == -2)
                                {

                                    logs[threadnumber]="Nox was closed";
                                    _state = State.stop;
                                }
                                if (enterres == -3)
                                {

                                    logs[threadnumber]="enemy detected";
                                    _state = State.stop;
                                }

                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));

                            form.Controls[starts].Invoke((MethodInvoker)(() => form.Controls[starts].Enabled = true));
                            form.Controls[pauses].Invoke((MethodInvoker)(() => form.Controls[pauses].Enabled = false));
                           form.Controls[stops].Invoke((MethodInvoker)(() => form.Controls[stops].Enabled = false));
                            _state = State.stop;
                            return;
                        }
                    case "mine":
                        {

                               int mainfarmres = 0;
                            while (_state != State.stop)
                            {


                                    /*var t1 = Task.Run(async () =>
                                    {
                                        await Task.Run(() =>
                                        {

                                            if (checkgame(HWIDch) == 0)
                                            {
                                                cts.Cancel();
                                            }
                                            else return;
                                        }
                                        );

                                    });*/
                                try
                                {
                                    mainfarmres= mainfarm(rezhims);
                                    //mainfarmres = farmInLoc(rezhims);
                                    if (mainfarmres == -1)
                                    {

                                        logs[threadnumber]="Nox was closed";
                                        _state = State.stop;

                                            form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));

                                        }
                                    if (mainfarmres == -2)
                                    {

                                        logs[threadnumber]="stopped";
                                        _state = State.stop;

                                            form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                        }
                                    if(mainfarmres == -3)
                                    {

                                        logs[threadnumber]="enemy detected";
                                        _state = State.stop;

                                            form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                            worktimer[threadnumber] = 1;  
                                    }
                                }
                                catch (Exception)
                                {
                                        if (token.IsCancellationRequested)
                                        {
                                            _state = State.stop;
                                            logs[threadnumber] = "stopped";
                                            form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                            return ;
                                        }
                                        Task.Delay(1000);
                                }
                               
                               
                            }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                form.Controls[starts].Invoke((MethodInvoker)(() => form.Controls[starts].Enabled = true));
                                form.Controls[pauses].Invoke((MethodInvoker)(() => form.Controls[pauses].Enabled = false));
                               form.Controls[stops].Invoke((MethodInvoker)(() => form.Controls[stops].Enabled = false));
                                return ;
                        }
                    case "flyto":
                        {
                              
                                
                                
                            form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                               
                                int flytores = 0, freespaceres = 0;
                                

                            switch (adress)
                            {
                                case "":
                                    {

                                            logs[threadnumber]="не выбран шоп";
                                            _state = State.stop;
                                        form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                        form.Controls[starts].Invoke((MethodInvoker)(() => form.Controls[starts].Enabled = true));
                                        form.Controls[pauses].Invoke((MethodInvoker)(() => form.Controls[pauses].Enabled = false));
                                       form.Controls[stops].Invoke((MethodInvoker)(() => form.Controls[stops].Enabled = false));
                                        return ;
                                    }
                                case "AMARRCORP":
                                    {
                                        adress = "AMARR";
                                        flytores = flyto( 2, adress);
                                        if (flytores == -2)
                                        {


                                                logs[threadnumber]="stopped";
                                                _state = State.stop;


                                        }
                                        _state = State.stop;
                                        form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                        form.Controls[starts].Invoke((MethodInvoker)(() => form.Controls[starts].Enabled = true));
                                        form.Controls[pauses].Invoke((MethodInvoker)(() => form.Controls[pauses].Enabled = false));
                                       form.Controls[stops].Invoke((MethodInvoker)(() => form.Controls[stops].Enabled = false));
                                        return ;
                                    }
                                case "AMARR":
                                    {
                                        adress = "AMARR";
                                        flytores = flyto( 2, adress);
                                        if (flytores == -2)
                                        {

                                                logs[threadnumber]="stopped";
                                                _state = State.stop;


                                        }
                                        _state = State.stop;
                                        form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                        form.Controls[starts].Invoke((MethodInvoker)(() => form.Controls[starts].Enabled = true));
                                        form.Controls[pauses].Invoke((MethodInvoker)(() => form.Controls[pauses].Enabled = false));
                                       form.Controls[stops].Invoke((MethodInvoker)(() => form.Controls[stops].Enabled = false));
                                        return ;
                                    }
                                case "JITA":
                                    {
                                        adress = "JITA";
                                        flytores = flyto( 2, adress);
                                        if (flytores == -2)
                                        {

                                                logs[threadnumber]="stopped";
                                                _state = State.stop;


                                        }
                                        _state = State.stop;
                                        form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                        form.Controls[starts].Invoke((MethodInvoker)(() => form.Controls[starts].Enabled = true));
                                        form.Controls[pauses].Invoke((MethodInvoker)(() => form.Controls[pauses].Enabled = false));
                                       form.Controls[stops].Invoke((MethodInvoker)(() => form.Controls[stops].Enabled = false));
                                        return ;
                                    }
                                case "Z-N9IP":
                                    {
                                        adress = "Z-N9IP";
                                        flytores = flyto( 2, adress);
                                        if (flytores == -2)
                                        {


                                                logs[threadnumber]="stopped";
                                                _state = State.stop;
                                        }
                                        form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                        form.Controls[starts].Invoke((MethodInvoker)(() => form.Controls[starts].Enabled = true));
                                        form.Controls[pauses].Invoke((MethodInvoker)(() => form.Controls[pauses].Enabled = false));
                                       form.Controls[stops].Invoke((MethodInvoker)(() => form.Controls[stops].Enabled = false));
                                        return ;
                                    }
                                default:
                                    {

                                            logs[threadnumber]="incorrect adress";
                                            form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                        form.Controls[starts].Invoke((MethodInvoker)(() => form.Controls[starts].Enabled = true));
                                        form.Controls[pauses].Invoke((MethodInvoker)(() => form.Controls[pauses].Enabled = false));
                                       form.Controls[stops].Invoke((MethodInvoker)(() => form.Controls[stops].Enabled = false));
                                        _state = State.stop;
                                        return ;
                                    }
                            }

                        }
                    case "planetsrefresh":
                        {
                            form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                            planetres = planetology();
                            if (planetres == -2)
                            {

                                    logs[threadnumber]="stopped";
                                    _state = State.stop;
                            }

                            form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));

                            form.Controls[starts].Invoke((MethodInvoker)(() => form.Controls[starts].Enabled = true));
                            form.Controls[pauses].Invoke((MethodInvoker)(() => form.Controls[pauses].Enabled = false));
                           form.Controls[stops].Invoke((MethodInvoker)(() => form.Controls[stops].Enabled = false));
                            _state = State.stop;
                            return ;
                        }
                    default:
                        {

                                logs[threadnumber]="не выбран скрипт";

                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                            form.Controls[starts].Invoke((MethodInvoker)(() => form.Controls[starts].Enabled = true));
                            form.Controls[pauses].Invoke((MethodInvoker)(() => form.Controls[pauses].Enabled = false));
                           form.Controls[stops].Invoke((MethodInvoker)(() => form.Controls[stops].Enabled = false));
                            _state = State.stop;


                            return ;
                        }
                }
                    }
                catch (Exception)
                {
                    if (token.IsCancellationRequested)
                    {
                        _state = State.stop;
                        logs[threadnumber] = "stopped";
                        form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                    }
                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                    form.Controls[starts].Invoke((MethodInvoker)(() => form.Controls[starts].Enabled = true));
                    form.Controls[pauses].Invoke((MethodInvoker)(() => form.Controls[pauses].Enabled = false));
                   form.Controls[stops].Invoke((MethodInvoker)(() => form.Controls[stops].Enabled = false));
                    _state = State.stop;
                    return;
                }
            }

            int config()
            {
               
                string loglabel = "log" + threadnumber;
                string statusis = "status" + (threadnumber + 1).ToString();

                if (!File.Exists("bot"+ (threadnumber+1).ToString() +".conf"))
                {
                    MessageBox.Show("cant find bot" + (threadnumber + 1).ToString() + ".conf file", "error", MessageBoxButtons.OK);

                    return -1;
                }
                string[] alltext = File.ReadAllLines("bot" + (threadnumber + 1).ToString() + ".conf");
                if (alltext.Length == 0)
                {
                    form.Controls[loglabel].Invoke((MethodInvoker)(() => form.Controls[loglabel].Text = "config file error"));

                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                    return -1;
                }
                if (alltext.Length < 11)
                {
                    form.Controls[loglabel].Invoke((MethodInvoker)(() => form.Controls[loglabel].Text = "config file(number strings) error"));

                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                    return -1 ;
                }
                if (alltext[0] != "<Farm:")
                {
                    form.Controls[loglabel].Invoke((MethodInvoker)(() => form.Controls[loglabel].Text = "farmblock config error"));

                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                    return -1;
                }
                int i = 1;
                int pos = 0;
                while (alltext[i] != ">")
                {
                    if (alltext[i].Length == 0)
                    {
                        form.Controls[loglabel].Invoke((MethodInvoker)(() => form.Controls[loglabel].Text = "farmblock config error"));

                        form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                        return -1;
                    }
                    if (alltext[i][0].Equals('\t')==false)
                    {
                        form.Controls[loglabel].Invoke((MethodInvoker)(() => form.Controls[loglabel].Text = "farmblock config error"));

                        form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                        return -1;
                    }
                    locfarm[pos] = alltext[i].Substring(1);
                    pos++;
                    i++;

                }
                i++;
                kollocfarm = pos;
                if (alltext[i] != "<Sell:")
                {
                    form.Controls[loglabel].Invoke((MethodInvoker)(() => form.Controls[loglabel].Text = "sellblock config error"));

                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                    return -1;
                }
                i++;
                pos = 0;
                while (alltext[i] != ">")
                {
                    if (alltext[i].Length == 0)
                    {
                        form.Controls[loglabel].Invoke((MethodInvoker)(() => form.Controls[loglabel].Text = "sellblock config error"));

                        form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                        return -1;
                    }
                    if (alltext[i][0].Equals('\t') == false)
                    {
                        form.Controls[loglabel].Invoke((MethodInvoker)(() => form.Controls[loglabel].Text = "sellblock config error"));

                        form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                        return -1;
                    }
                    locsell = alltext[i].Substring(1);

                    i++;

                }
                i++;
                if (alltext[i] != "<Conf:")
                {
                    form.Controls[loglabel].Invoke((MethodInvoker)(() => form.Controls[loglabel].Text = "sellblock config error"));

                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                    return -1;
                }
                pos = 0;
                i++;
                while (alltext[i] != ">")
                {
                    if (alltext[i].Length == 0)
                    {
                        form.Controls[loglabel].Invoke((MethodInvoker)(() => form.Controls[loglabel].Text = "sellblock config error"));

                        form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                        return -1;
                    }
                    if (alltext[i][0].Equals('\t') == false)
                    {
                        form.Controls[loglabel].Invoke((MethodInvoker)(() => form.Controls[loglabel].Text = "sellblock config error"));

                        form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                        return -1;
                    }
                    if(alltext[i].Substring(0,8) == "\ttocorp=")
                    {
                        tocorp = Convert.ToInt32(alltext[i].Substring(8));
                    }
                    i++;
                    if (alltext[i].Substring(0, 9) == "\tgunscol=")
                    {
                        gunscol = Convert.ToInt32(alltext[i].Substring(9));
                    }
                    i++;
                    if (alltext[i].Substring(0, 12) == "\trudetomine=")
                    {
                        rudetomine = alltext[i].Substring(12);
                    }
                    i++;

                }
                i++;
                if (alltext[i] != "<Mineral:")
                {
                    form.Controls[loglabel].Invoke((MethodInvoker)(() => form.Controls[loglabel].Text = "mineral config error"));

                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                    return -1;
                }
                pos = 0;
                i++;
                int k = 0;
                while (alltext[i] != ">")
                {
                    if (alltext[i].Length == 0)
                    {
                        form.Controls[loglabel].Invoke((MethodInvoker)(() => form.Controls[loglabel].Text = "mineral config error"));

                        form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                        return -1;
                    }
                    if (alltext[i][0].Equals('\t') == false)
                    {
                        form.Controls[loglabel].Invoke((MethodInvoker)(() => form.Controls[loglabel].Text = "mineral config error"));

                        form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                        return -1;
                    }
                    if (alltext[i][0] == '\t')
                    {
                        minerals[k] = alltext[i].Substring(1);
                        k++;
                    }
                    
                    i++;

                }
                return 0;

            }

            int checkgame()
            {
                Point[] center = new Point[10];
                while (center[0].X == 0)
                {
                    center = FindCountour( ref images.gameicon,"gameicon");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }


                }
                return 0;
            }


            int enter()
            {
                
                Point[] center = new Point[10];
                ciklzapuskov++;
                Point[] point = new Point[1];
                point[0].X = 512;
                point[0].Y = 284;



                center = FindCountour( ref images.gameicon,"gameicon");
                if (center[0].X == -1)
                {
                    return -1;
                }
                if (center[0].X == -2)
                {
                    return -2;
                }
                while (center[0].X == 0)
                {
                    center = FindCountour( ref images.gameicon, "gameicon");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }


                }
                click("LCL", HWIDch, center, 0);


                center = FindCountour( ref images.closequeue, "closequeue");
                if (center[0].X == -1)
                {
                    return -1;
                }
                if (center[0].X == -2)
                {
                    return -2;
                }

                while (center[0].X == 0)
                {


                    center = FindCountour( ref images.closequeue, "closequeue");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }


                    if (center[0].X == 0)
                        click("LCL", HWIDch, point, 0);
                    else
                        click("LCL", HWIDch, center, 0);
                   Task.Delay(2000).Wait();

                }
               Task.Delay(5000).Wait();
                point[0].X = 512;
                point[0].Y = 600;
                click("LCL", HWIDch, point, 0);
               Task.Delay(1000).Wait();

                bool entered = true;
                while (entered)
                {


                    center = FindCountour( ref images.venture, "venture");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }

                    while (center[0].X != 0)
                    {
                        center = FindCountour( ref images.venture,"venture");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }
                        if (center[0].X > 0)
                            click("LCL", HWIDch, center, 0);
                        return 0;
                    }

                    center = FindCountour( ref images.venture3,"venture3");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }

                    while (center[0].X != 0)
                    {
                        center = FindCountour( ref images.venture3, "venture3");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }
                        if (center[0].X > 0)
                            click("LCL", HWIDch, center, 0);
                        return 0;
                    }

                    center = FindCountour(ref images.egg, "egg");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }

                    while (center[0].X != 0)
                    {
                        center = FindCountour(ref images.egg, "egg");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }
                        if (center[0].X > 0)
                            click("LCL", HWIDch, center, 0);
                        return 0;
                    }
                    Task.Delay(2000).Wait();

                }

                return 0;

            }
            int planetology()
            {
                Point[] center = new Point[10];
                center[0].X = 48;
                center[0].Y = 14;

                click("LCL", HWIDch, center, 0);
               Task.Delay(1000).Wait();

                

                center = FindCountour( ref images.planetmenu, "planetmenu");
                if (center[0].X == -1)
                {
                    return -1;
                }
                if (center[0].X == -2)
                {
                    return -2;
                }




                if (center[0].X == 0)
                {
                    while (center[0].X == 0)
                    {

                        center = FindCountour( ref images.planetmenu, "planetmenu");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }
                       Task.Delay(1000).Wait();
                    }
                    click("LCL", HWIDch, center, 0);


                }
                else
                {
                    click("LCL", HWIDch, center, 0);


                }


               
                center = FindCountour(ref images.planetopened, "planetopened");
                if (center[0].X == -1)
                {
                    return -1;
                }
                if (center[0].X == -2)
                {
                    return -2;
                }


                if (center[0].X == 0)
                {
                    while (center[0].X == 0)
                    {

                        center = FindCountour(ref images.planetopened, "planetopened");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }


                        Task.Delay(1000).Wait();
                    }



                }
                else
                {



                }
                bool allplanets = false;
                
                while (allplanets == false)
                {



                    center = FindCountour( ref images.planetnotime, "planetnotime");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }
                    int sch = 0;
                    while (center[0].X != 0)//нету красных планет
                    {


                        click("LCL", HWIDch, center, 0);

                       Task.Delay(1000).Wait();



                        center = FindCountour( ref images.planetrenew, "planetrenew");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }

                        click("LCL", HWIDch, center, 0);


                        center = FindCountour( ref images.planetnotime, "planetnotime");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }
                       
                       Task.Delay(1000).Wait();

                    }

                    sch = 0;

                    center = FindCountour( ref images.planetlowtime, "planetlowtime");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }

                    while (center[0].X != 0)//нету желтых планет
                    {

                        click("LCL", HWIDch, center, 0);
                       Task.Delay(1000).Wait();


                        center = FindCountour( ref images.planetrenew, "planetrenew");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }


                       Task.Delay(1000).Wait();
                        click("LCL", HWIDch, center, 0);

                        center = FindCountour( ref images.planetlowtime, "planetlowtime");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }

                        
                    }
                    if (center[0].X == 0)
                    {
                        allplanets = true;
                    }



                }




                center = FindCountour( ref images.planetclose, "planetclose");
                if (center[0].X == -1)
                {
                    return -1;
                }
                if (center[0].X == -2)
                {
                    return -2;
                }

                click("LCL", HWIDch, center, 0);
               Task.Delay(1000).Wait();



                return 1;
            }

            int flyto(int rezhim, string name)
            {

                //rezhim =1 - полет до места фарма(вызывается в режиме mine), rezhim = 2 - полет до выбранного в shopcenters(просто flyto)
                int i = 0;
                bool closed = false;
                string statusis = "status" + (threadnumber + 1).ToString();
                string loglabel;
                loglabel = "log" + threadnumber.ToString();
                Point[] center = new Point[10];
                bool tocorp = false;
                Random rnd = new Random();
                Image<Bgr, byte> inputImage = null;
                int sch = 0;
               Task.Delay(2000).Wait();
                center[0].X = 48;
                center[0].Y = 14;
                click("LCL", HWIDch, center, 0);
               Task.Delay(4000).Wait();



                logs[threadnumber]="ищу карту";
                center = FindCountour( ref images.map, "map");
                if (center[0].X == -1)
                {
                    return -1;
                }
                if (center[0].X == -2)
                {
                    return -2;
                }

                while (center[0].X == 0)
                {

                 
                    center = FindCountour( ref images.map, "map");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }
                    if (center[0].X != 0)
                        break;

                    center[0].X = 48;
                    center[0].Y = 14;
                    click("LCL", HWIDch, center, 0);
                   Task.Delay(1000).Wait();


                    center = FindCountour( ref images.map, "map");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }
                    if (center[0].X != 0)
                        break;


                }

                click("LCL", HWIDch, center, 0);
               Task.Delay(4000).Wait();



                logs[threadnumber]="ищу поиск";
                center = FindCountour( ref images.search, "search");
                if (center[0].X == -1)
                {
                    return -1;
                }
                if (center[0].X == -2)
                {
                    return -2;
                }



                if (center[0].X == 0)
                {
                    while (center[0].X == 0)
                    {



                        center = FindCountour( ref images.search, "search");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }


                       Task.Delay(1000).Wait();
                    }

                    click("LCL", HWIDch, center, 0);
                }
                else
                {

                    click("LCL", HWIDch, center, 0);
                }

               Task.Delay(10000).Wait();


                logs[threadnumber]="ищу поле ввода текста";
                center = FindCountour( ref images.searchtext, "searchtext");
                if (center[0].X == -1)
                {
                    return -1;
                }
                if (center[0].X == -2)
                {
                    return -2;
                }


                if (center[0].X == 0)
                {
                    while (center[0].X == 0)
                    {


                        center = FindCountour( ref images.searchtext, "searchtext");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }



                       Task.Delay(1000).Wait();
                    }

                    click("LCL", HWIDch, center, 0);
                }
                else
                {

                    click("LCL", HWIDch, center, 0);
                }



                logs[threadnumber]="ищу кнопку подтвердить ввод";
                center = FindCountour( ref images.searchsend, "searchsend");
                if (center[0].X == -1)
                {
                    return -1;
                }
                if (center[0].X == -2)
                {
                    return -2;
                }



                if (center[0].X == 0)
                {
                    while (center[0].X == 0)
                    {


                        center = FindCountour( ref images.searchsend, "searchsend");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }

                       Task.Delay(1000).Wait();
                    }

                }
                else
                {

                }

                for (i = 0; i < name.Length; i++)
                {

                    switch (name[i])
                    {
                        case 'A':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("A", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'B':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("B", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'C':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("C", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'D':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("D", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'E':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("E", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'F':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("F", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'G':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("G", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'H':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("H", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'I':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("I", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'J':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("J", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'K':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("K", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'L':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("L", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'M':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("M", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'N':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("N", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'O':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("O", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'P':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("P", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'Q':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("Q", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'R':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("R", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'S':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("S", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'T':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("T", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'U':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("U", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'V':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("V", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'W':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("W", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'X':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("X", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'Y':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("Y", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case 'Z':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("Z", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case ' ':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click(" ", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case '-':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("-", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case '0':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("0", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case '1':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("1", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case '2':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("2", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case '3':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("3", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case '4':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("4", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case '5':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("5", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case '6':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("6", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case '7':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("7", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case '8':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("8", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }
                        case '9':
                            {
                                while (_state == State.pause)
                                {

                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                                    Task.Delay(1000).Wait();
                                }
                                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                                if (_state == State.stop)
                                {
                                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                                    return 4;
                                }
                                click("9", HWIDm, center, 0);
                               Task.Delay(rnd.Next(1, 3) * 1000).Wait();
                                break;
                            }

                    }
                }


                logs[threadnumber]="жму подтвердить ввод";
                center = FindCountour( ref images.searchsend, "searchsend");
                if (center[0].X == -1)
                {
                    return -1;
                }
                if (center[0].X == -2)
                {
                    return -2;
                }

                if (center[0].X != 0)
                {
                    while (center[0].X != 0)
                    {


                        center = FindCountour( ref images.searchsend, "searchsend");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }

                        click("LCL", HWIDch, center, 0);
                       Task.Delay(1000).Wait();
                        center = FindCountour( ref images.searchsend, "searchsend");
                    }

                }
                else
                {

                }


                logs[threadnumber]="ищу иконку места назначения";
                if (rezhim == 1)
                {


                    center = FindCountour( ref images.searchblock, "searchblock");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }

                   Task.Delay(3000).Wait();
                    int j;
                    if (center[0].X == 0)
                    {
                        while (center[0].X == 0)
                        {




                            center = FindCountour( ref images.search,"search");
                            if (center[0].X == -1)
                            {
                                return -1;
                            }
                            if (center[0].X == -2)
                            {
                                return -2;
                            }


                            center[0].Y -= 200;
                            click("LeftDown", HWIDch, center, 0);
                            for (j = center[0].Y; j > center[0].Y - 60; j = j - 4)
                            {
                                SendMessage(HWIDm, WM_MOUSEMOVE, 0x0201, MakeLParam(center[0].X, j));
                               Task.Delay(50).Wait();
                            }
                            SendMessage(HWIDm, WM_MOUSEMOVE, 0x0202, MakeLParam(center[0].X, j));
                            click("LeftUp", HWIDch, center, 0);
                           Task.Delay(2000).Wait();


                            center = FindCountour( ref images.searchblock, "searchblock");
                            if (center[0].X == -1)
                            {
                                return -1;
                            }
                            if (center[0].X == -2)
                            {
                                return -2;
                            }


                        }
                        click("LCL", HWIDch, center, 0);
                    }
                    else
                    {
                        click("LCL", HWIDch, center, 0);

                    }

                }


                if (rezhim == 2)
                {

                    center = FindCountour( ref images.shopcenter, "shopcenter");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }

                   Task.Delay(3000).Wait();
                    int j;
                    if (center[0].X == 0)
                    {
                        while (center[0].X == 0)
                        {




                            center = FindCountour( ref images.station, "station");
                            if (center[0].X == -1)
                            {
                                return -1;
                            }
                            if (center[0].X == -2)
                            {
                                return -2;
                            }


                            center[0].Y -= 200;
                            click("LeftDown", HWIDch, center, 0);
                            for (j = center[0].Y; j > center[0].Y - 60; j = j - 4)
                            {
                                SendMessage(HWIDm, WM_MOUSEMOVE, 0x0201, MakeLParam(center[0].X, j));
                               Task.Delay(50).Wait();
                            }
                            SendMessage(HWIDm, WM_MOUSEMOVE, 0x0202, MakeLParam(center[0].X, j));
                            click("LeftUp", HWIDch, center, 0);
                           Task.Delay(2000).Wait();


                            center = FindCountour( ref images.station, "station");
                            if (center[0].X == -1)
                            {
                                return -1;
                            }
                            if (center[0].X == -2)
                            {
                                return -2;
                            }


                        }
                        click("LCL", HWIDch, center, 0);
                    }
                    else
                    {
                        click("LCL", HWIDch, center, 0);

                    }

                }

                bool course = false;


                logs[threadnumber]="нажимаю полететь";
                center = FindCountour( ref images.fly, "fly");
                if (center[0].X == -1)
                {
                    return -1;
                }
                if (center[0].X == -2)
                {
                    return -2;
                }


                if (center[0].X == 0)
                {
                    while (center[0].X == 0)
                    {


                        center = FindCountour( ref images.fly, "fly");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }


                        center = FindCountour( ref images.cancelcurse, "cancelcurse");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }

                        if (center[0].X != 0)
                        {
                            course = true;
                            break;
                        }


                        center = FindCountour( ref images.fly, "fly");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }

                       Task.Delay(1000).Wait();
                    }

                    if (course == false)
                        click("LCL", HWIDch, center, 0);
                }
                else
                {

                    if (course == false)
                        click("LCL", HWIDch, center, 0);
                }


               Task.Delay(1000).Wait();



                center = FindCountour( ref images.dokfly, "dokfly");
                if (center[0].X == -1)
                {
                    return -1;
                }
                if (center[0].X == -2)
                {
                    return -2;
                }
                if (center[0].X == 0)
                {
                    while (center[0].X == 0)
                    {
                        center = FindCountour(ref images.dokfly, "dokfly");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }
                        center = FindCountour(ref images.inflight, "inflight");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }
                        if (center[0].X != 0)
                        {
                            break;
                        }

                        center = FindCountour( ref images.dokfly, "dokfly");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }
                        
                        Task.Delay(1000).Wait();
                    }

                    click("LCL", HWIDch, center, 0);
                }
                else
                {

                    click("LCL", HWIDch, center, 0);
                }

               Task.Delay(1000).Wait();
                bool poletet = true;


                center = FindCountour( ref images.flyaccept, "flyaccept");
                if (center[0].X == -1)
                {
                    return -1;
                }
                if (center[0].X == -2)
                {
                    return -2;
                }
                i = 0;
                if (center[0].X == 0)
                {
                    while (center[0].X == 0)
                    {


                        center = FindCountour( ref images.flyaccept, "flyaccept");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }

                        i++;
                        if (i == 5)
                        {
                            poletet = false;
                            break;
                        }
                       Task.Delay(500).Wait();
                    }

                    click("LCL", HWIDch, center, 0);
                   
                }
                else
                {

                    click("LCL", HWIDch, center, 0);
                }


                Task.Delay(1000).Wait();
                bool delivered = false;
                i = 0;
                int kol = 0 ;
                if(poletet == true)
                while (delivered == false)
                {



                    center = FindCountour( ref images.inflight, "inflight");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }
                    if (i == 1)
                    if(center[0].X == 0)
                    {
                        break;
                    }
                    if (center[0].X == 0)
                    {
                        while(center[0].X == 0)
                        {

                            center = FindCountour(ref images.inflight, "inflight");
                            if (center[0].X == -1)
                            {
                                return -1;
                            }
                            if (center[0].X == -2)
                            {
                                return -2;
                            }
                                kol++;
                               Task.Delay(500).Wait();
                                if (kol > 5)
                                {
                                    break;
                                }

                        }
                        i++;
                    }
                    
                    
                    Task.Delay(1000).Wait();
                }
                else
                {
                    while (delivered == false)
                    {



                        center = FindCountour(ref images.inflight, "inflight");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }
                        if (center[0].X == 0)
                            {
                                break;
                            }


                        Task.Delay(1000).Wait();
                    }
                }
               Task.Delay(1000).Wait();
                /*if(poletet == false)
                {
                    center = FindCountour(ref images.station, "station");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }
                    sch = 0;
                        while (center[0].X != 0)
                        {

                            center = FindCountour(ref images.station, "station");
                            if (center[0].X == -1)
                            {
                                return -1;
                            }
                            if (center[0].X == -2)
                            {
                                return -2;
                            }
                        Task.Delay(500).Wait();

                        sch++;
                        if (sch > 15)
                        {
                            break;
                        }
                        }


                    
                    center = FindCountour(ref images.station, "station");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }
                    
                        while (center[0].X == 0)
                        {

                            center = FindCountour(ref images.station, "station");
                            if (center[0].X == -1)
                            {
                                return -1;
                            }
                            if (center[0].X == -2)
                            {
                                return -2;
                            }
                        }


                    
                    Task.Delay(2000).Wait();
                }

               */
                closed = false;
                i = 0;
                while (closed == false)
                {



                    center = FindCountour( ref images.closemap, "closemap");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }



                    if (center[0].X == 0)
                    {
                        closed = true;
                        break;
                    }
                    /*i++;
                    if (i >= 15)
                    {
                        break;
                    }*/
                    click("LCL", HWIDch, center, 0);
                   Task.Delay(2000).Wait();

                }
                
                return 0;
            }
            int freespace()
            {
                string statusis = "status" + (threadnumber + 1).ToString();

                Image<Bgr, byte> inputImage = null;

                string loglabel = "log" + threadnumber.ToString();
                Point[] center = new Point[10];
                ciklzapuskov++;
                string starts = "start" + (threadnumber + 1).ToString();
                string stops = "stop" + (threadnumber + 1).ToString();
                string pauses = "pause" + (threadnumber + 1).ToString();
                int i = 0;
                Random rnd = new Random();
                Color pixel;
                int kol = 0;
                Bitmap bitmap;
                Point K = new Point(486, 546);
                int rezhim = 2;
                bool tocorp = false;
                string shopsname = "shops" + threadnumber.ToString();
                int tofly = 0;
                string adress = "";
                form.Controls[shopsname].Invoke((MethodInvoker)(() => adress = form.Controls[shopsname].Text));

                //скорее всего косяк
               Task.Delay(1000).Wait();
                bool closedall = false;
                while (closedall == false)
                {



                    logs[threadnumber]="закрываю все окна";
                    center = FindCountour( ref images.inventarclose, "inventarclose");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }



                    if (center[0].X != 0)
                        click("LCL", HWIDch, center, 0);

                    center = FindCountour(ref images.planetclose, "planetclose");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }



                    if (center[0].X != 0)
                        click("LCL", HWIDch, center, 0);


                    center = FindCountour( ref images.closemap, "closemap");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }


                    if (center[0].X != 0)
                        click("LCL", HWIDch, center, 0);
                    else
                    {
                        closedall = true;
                        break;
                    }
                   Task.Delay(1000).Wait();
                }
                Task.Delay(1000).Wait();
                center = FindCountour( ref images.leavedoc, "leavedoc");
                if (center[0].X == -1)
                {
                    return -1;
                }
                if (center[0].X == -2)
                {
                    return -2;
                }
                if (center[0].X == 0)
                {

                    logs[threadnumber]="лечу на базу";
                    int flytores = 0;
                    flytores = flyto(rezhim, locsell);
                    if (flytores == -1)
                    {
                        return -1;
                    }
                    if (flytores == -2)
                    {
                        return -2;
                    }
                    closedall = false;
                    while (closedall == false)
                    {



                        logs[threadnumber]="закрываю все окна";
                        center = FindCountour( ref images.inventarclose, "inventarclose");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }



                        if (center[0].X != 0)
                            click("LCL", HWIDch, center, 0);
                        center = FindCountour(ref images.planetclose, "planetclose");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }



                        if (center[0].X != 0)
                            click("LCL", HWIDch, center, 0);



                        center = FindCountour( ref images.closemap,"closemap");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }


                        if (center[0].X != 0)
                            click("LCL", HWIDch, center, 0);
                        else
                        {
                            closedall = true;
                            break;
                        }
                        Task.Delay(1000).Wait();
                    }
                }
                //выбор руды в отсеке
                Task.Delay(2000).Wait();
                closedall = false;
                while (closedall == false)
                {



                    logs[threadnumber] = "закрываю все окна";
                    center = FindCountour(ref images.inventarclose, "inventarclose");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }



                    if (center[0].X != 0)
                        click("LCL", HWIDch, center, 0);
                    center = FindCountour(ref images.planetclose, "planetclose");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }



                    if (center[0].X != 0)
                        click("LCL", HWIDch, center, 0);



                    center = FindCountour(ref images.closemap, "closemap");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }


                    if (center[0].X != 0)
                        click("LCL", HWIDch, center, 0);
                    else
                    {
                        closedall = true;
                        break;
                    }
                    Task.Delay(1000).Wait();
                }




                int sch = 0;
                center[0].X = 65;
                center[0].Y = 100;
                click("LCL", HWIDch, center, 0);
                Task.Delay(3000).Wait();

                logs[threadnumber]="открываю отсек руды";
                center = FindCountour( ref images.rudeotsek, "rudeotsek");
                if (center[0].X == -1)
                {
                    return -1;
                }
                if (center[0].X == -2)
                {
                    return -2;
                }
                Point[] tempcenter = new Point[10];
                sch = 0;
                if (center[0].X == 0)
                {
                    while (center[0].X == 0)
                    {


                        center = FindCountour( ref images.rudeotsek, "rudeotsek");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }
                        if (center[0].X != 0)
                        {
                            break;
                        }
                        sch++;
                        if (sch > 5)
                        {
                            break;
                        }
                        Task.Delay(1000).Wait();
                    }
                    if(center[0].X!=0)
                    click("LCL", HWIDch, center, 0);

                }
                else
                {
                    click("LCL", HWIDch, center, 0);

                }
                Task.Delay(5000).Wait();

                logs[threadnumber]="нажимаю выбрать все";
                center = FindCountour( ref images.pickall, "pickall");
                if (center[0].X == -1)
                {
                    return -1;
                }
                if (center[0].X == -2)
                {
                    return -2;
                }

                if (center[0].X == 0)
                {
                    while (center[0].X == 0)
                    {


                        center = FindCountour( ref images.pickall, "pickall");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }




                       Task.Delay(1000).Wait();
                    }
                }
                click("LCL", HWIDch, center, 0);
                Task.Delay(5000).Wait();

                logs[threadnumber]="нажимаю переместить в";
                center = FindCountour( ref images.movetosklad, "movetosklad");
                if (center[0].X == -1)
                {
                    return -1;
                }
                if (center[0].X == -2)
                {
                    return -2;
                }

                if (center[0].X == 0)
                {
                    while (center[0].X == 0)
                    {


                        center = FindCountour( ref images.movetosklad, "movetosklad");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }




                        Task.Delay(1000).Wait();
                    }
                }
                click("LCL", HWIDch, center, 0);
                Task.Delay(3000).Wait();
                if (rezhim == 2)
                {

                    logs[threadnumber]="нажимаю корп склад";
                    center = FindCountour( ref images.corpsklad, "corpsklad");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }

                    while (center[0].X == 0)
                    {


                        center = FindCountour( ref images.corpsklad, "corpsklad");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }
                        Task.Delay(1000).Wait();

                    }
                    sch = 0;
                    click("LCL", HWIDch, center, 0);
                    Task.Delay(1000).Wait();
                }
                if (rezhim == 1)
                {
                    center = FindCountour( ref images.insklad, "insklad");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }

                    while (center[0].X == 0)
                    {


                        center = FindCountour( ref images.insklad, "insklad");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }
                    }
                    sch = 0;
                    click("LCL", HWIDch, center, 0);
                    Task.Delay(1000).Wait();
                }



               Task.Delay(5000).Wait();



                closedall = false;
                while (closedall == false)
                {



                    logs[threadnumber]="закрываю все окна";
                    center = FindCountour( ref images.inventarclose, "inventarclose");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }



                    if (center[0].X != 0)
                        click("LCL", HWIDch, center, 0);
                    center = FindCountour(ref images.planetclose, "planetclose");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }



                    if (center[0].X != 0)
                        click("LCL", HWIDch, center, 0);



                    center = FindCountour( ref images.closemap, "closemap");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }


                    if (center[0].X != 0)
                        click("LCL", HWIDch, center, 0);
                    else
                    {
                        closedall = true;
                        break;
                    }
                    Task.Delay(1000).Wait();
                }
                click("LCL", HWIDch, center, 0);
               Task.Delay(1000).Wait();
                string counter = "cyclecounter" + threadnumber.ToString();
                string text;
                
                    text = form.Controls[counter].Text;
                    int cycelecount = Convert.ToInt32(text);
                    cycelecount++;
                    _synchronizationContext.Post((o) => form.Controls[counter].Text =  cycelecount.ToString(),null);

                return 0;
            }
            int checkmass(IntPtr HWIDch)
            {
                string loglabel = "log" + threadnumber.ToString();

                logs[threadnumber]="проверяю вес";
                Image<Bgr, byte> inputImage = null;
                Color temppixel;// r=30, g=80, b = 66
                int dsres;
                dsres = doscreen(HWIDch, ref inputImage);
                if (dsres == 4)
                {
                    return -1;
                }
                if (dsres == -2)
                {
                    return -2;
                }

                temppixel = inputImage.ToBitmap().GetPixel(75, 108);//во время фарма полоска загрузки инвентаря(зелененькая)
                inputImage.Dispose();
                if (temppixel.R >= 25 && temppixel.G >= 70 && temppixel.B >= 55)
                {
                    return 1;
                }

                return 0;
            }
            int mainfarm( int rezhim)
            {
                string loglabel = "log" + threadnumber.ToString();

                logs[threadnumber]="закрываю все";
                Point[] center = new Point[10];
               
                bool closedall = false;
                while (closedall == false)
                {
                   
                    center = FindCountour( ref images.inventarclose, "inventarclose");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }
                    if (center[0].X != 0)
                        click("LCL", HWIDch, center, 0);
                    center = FindCountour( ref images.closemap, "closemap");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }
                    if (center[0].X != 0)
                        click("LCL", HWIDch, center, 0);
                    else
                    {
                        closedall = true;
                        break;
                    }
                    Task.Delay(1000).Wait();
                }
                
                int flytores = 0;
                int farminlocres = 0;
                Point[] temppoint = new Point[10];
                temppoint[0].X = 10;
                temppoint[0].Y = 660;//клик по локалу
                int i=0;
                if(localopened[threadnumber] == 0)
                {
                    center = FindCountour(ref images.localpeople, "localpeople");
                    if (center[0].X == 0)
                    {
                        while (center[0].X == 0)
                        {
                            center = FindCountour(ref images.localpeople, "localpeople");
                            if (center[0].X == -1)
                            {
                                return -1;
                            }
                            if (center[0].X == -2)
                            {
                                return -2;
                            }
                            i++;
                            if (i % 4 == 0)
                            {
                                click("LCL", HWIDch, temppoint, 0);
                            }
                            Task.Delay(1000).Wait();
                        }
                        center = FindCountour(ref images.localminimize, "localminimize");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }
                        click("LCL", HWIDch, center, 0);
                        Task.Delay(1000).Wait();
                    }
                    localopened[threadnumber] = 1;
                }
               
                
                Task.Delay(5000).Wait();
                localpos();
                if(localniz[0].X == 0)
                {
                    center = FindCountour(ref images.localpeople, "localpeople");
                    if (center[0].X == 0)
                    {
                        while (center[0].X == 0)
                        {
                            center = FindCountour(ref images.localpeople, "localpeople");
                            if (center[0].X == -1)
                            {
                                return -1;
                            }
                            if (center[0].X == -2)
                            {
                                return -2;
                            }
                            i++;
                            if (i % 4 == 0)
                            {
                                click("LCL", HWIDch, temppoint, 0);
                            }
                           
                            Task.Delay(1000).Wait();
                        }
                        center = FindCountour(ref images.localminimize, "localminimize");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }
                        click("LCL", HWIDch, center, 0);
                        Task.Delay(1000).Wait();
                    }
                    localopened[threadnumber] = 1;
                    localpos();
                }
                int checkloc = 0;
                if ((center = FindCountour( ref images.leavedoc, "leavedoc"))[0].X == 0)
                {
                    checkloc =checklocal();
                    if(checkloc == 1)
                    {
                       leavegame();
                        return -3;
                    }

                }
                    


                
                    if (checkmass(HWIDch) == 1)
                    {
                        freespace();;
                    }


                logs[threadnumber]="вылетаю с базы";
                flytores =flyto(1, locfarm[numberloc]);
                if(flytores == -1)
                {
                    return -1;
                }
                if (flytores == -2)
                {
                    return -2;
                }
                bool farming = true;
                i = 0;
                while (farming)
                {

                    logs[threadnumber]="начинаю фармить";
                    checkloc = checklocal();
                    if (checkloc == 1)
                    {
                        leavegame();
                        return -3;
                    }

                    farminlocres = farmInLoc(rezhim);
                    if (farminlocres == -1)
                    {
                        return -1;
                    }
                    if (farminlocres == -2)
                    {
                        return -2;
                    }
                    if (farminlocres == 5)
                    {
                        checkloc = checklocal();
                        if (checkloc == 1)
                        {
                            leavegame();
                            return -3;
                        }

                        int freespaceres = 0;
                        freespaceres = freespace();
                        if (freespaceres == -1)
                        {
                            return -1;
                        }
                        if (freespaceres == -2)
                        {
                            return -2;
                        }
                       /* i++;
                        if(i%3 == 0)
                        {
                            int planetres = planetology();
                            if(planetres == -1)
                            {
                                return -1;
                            }
                            if (planetres == -2)
                            {
                                return -2;
                            }
                        }*/
                        flytores = flyto(1, locfarm[numberloc]);
                        if (flytores == -1)
                        {
                            return -1;
                        }
                        if (flytores == -2)
                        {
                            return -2;
                        }
                    }
                    if (farminlocres == 0 || farminlocres == 2)
                    {
                        checkloc = checklocal();
                        if (checkloc == 1)
                        {
                            leavegame();
                            return -3;
                        }

                        numberloc++;
                        if (numberloc >= kollocfarm-1)
                        {
                            numberloc = 0;
                        }
                        
                        flytores = flyto( 1,locfarm[numberloc]);
                        if (flytores == -1)
                        {
                            return -1;
                        }
                        if (flytores == -2)
                        {
                            return -2;
                        }
                    }
                    if(farminlocres == -3)
                    {
                        return -3;
                    }
                    if (checkmass(HWIDch) == 1)
                    {
                        int freespaceres = 0;
                        freespaceres = freespace();;
                        if (freespaceres == -1)
                        {
                            return -1;
                        }
                        if (freespaceres == -2)
                        {
                            return -2;
                        }
                        flytores =flyto(1, locfarm[numberloc]);
                        if (flytores == -1)
                        {
                            return -1;
                        }
                        if (flytores == -2)
                        {
                            return -2;
                        }
                    }

                }
               


                return 0;
            }
            int farmInLoc(int rezhim)
            {
                string loglabel = "log" + threadnumber.ToString();
                //снаружи проверяется что фармить. если режим 1 - кернит, 2 - крокит
                Point[] center = new Point[10];
                Point[] warpedto = new Point[1];
                Point[] meteors = new Point[10];
                bool nenashel = false;
                int[] meteoritsrange = new int[10];
                
                int posesheno = 0;
                int poyaskol = 0;
                bool farming = true;
                int i = 0;
                int pickedfiltr = 0;
                Random rnd = new Random();
                int kol = 0,isok=0;
                int checkloc = 0;


                // двигло 920:720
                Point[] tempcenter2 = new Point[10];
                Task.Delay(2000).Wait();


                logs[threadnumber]="ищу глазик";
                while (farming)
                {
                    checkloc =checklocal();
                    if (checkloc == 1)
                    {
                       leavegame();
                        return -3;
                    }

                    center = FindCountour( ref images.rightmenu, "rightmenu");
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }

                    if (center[0].X == 0)
                    {
                        while (center[0].X == 0)
                        {

                            checkloc = checklocal();
                            if (checkloc == 1)
                            {
                                leavegame();
                                return -3;
                            }
                            center = FindCountour( ref images.rightmenu, "rightmenu");
                            if (center[0].X == -1)
                            {
                                return -1;
                            }
                            if (center[0].X == -2)
                            {
                                return -2;
                            }



                            Task.Delay(1000).Wait();
                        }

                    }



                    if (center[0].X >= 800)
                        click("LCL", HWIDch, center, 0);
                    Task.Delay(1000).Wait();


                    checkloc = checklocal();
                    if (checkloc == 1)
                    {
                        leavegame();
                        return -3;
                    }
                    int randomed=0;

                    Task.Delay(5000).Wait();
                    int kolsch = 0;
                    int k = 0;
                    Point[] asterfiltr = new Point[1];
                    Point[] poyasfiltr = new Point[1];
                    bool poletel = false;
                    bool priletel = false;
                    Task.Delay(5000).Wait();
                    int sch = 0;
                    if (nenashel ==false)
                    {
                        center = FindCountour( ref images.filtraster, "filtraster");
                        
                        
                        if (center[0].X == 0)
                        {

                            logs[threadnumber]="фильтра астероидов нет";

                            checkloc =checklocal();
                            if (checkloc == 1)
                            {
                               leavegame();
                                return -3;
                            }

                            logs[threadnumber]="нажимаю фильтр поясов";

                            center = FindCountour( ref images.filtrpoyas, "filtrpoyas");
                            if (center[0].X == -1)
                            {
                                return -1;
                            }
                            if (center[0].X == -2)
                            {
                                return -2;
                            }
                            click("LCL", HWIDch, center, 0);
                            Task.Delay(1000).Wait();

                            center = FindCountour( ref images.poyas, "poyas");
                            while (center[0].X == 0)
                            {
                                checkloc =checklocal();
                                if (checkloc == 1)
                                {
                                   leavegame();
                                    return -3;
                                }
                                center = FindCountour( ref images.poyas, "poyas");
                                Task.Delay(1000).Wait();
                            }
                           
                            for (int j = 0; j < 10; j++)
                            {
                                if (center[j].X != 0)
                                {
                                    poyaskol++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            
                           
                            int[] wasrandomed = new int[poyaskol];

                            logs[threadnumber]="варпуюсь";
                            tempcenter2[0].X = 0;
                            while (poletel == false)
                            {
                                while(tempcenter2[0].X == 0)
                                {

                                    center = FindCountour(ref images.poyas, "poyas");
                                    poyaskol = 0;
                                    for (int j = 0; j < 10; j++)
                                    {
                                        if (center[j].X != 0)
                                        {
                                            poyaskol++;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    checkloc =checklocal();
                                if (checkloc == 1)
                                {
                                   leavegame();
                                    return -3;
                                }
                                randomed = rnd.Next(0, poyaskol);
                                wasrandomed[k] = randomed;
                                if (k != 0)
                                for(int j = 0; j < k; j++)
                                {
                                    if (wasrandomed[j] == randomed)
                                    {
                                        while(wasrandomed[j] == randomed)
                                        {
                                            randomed = rnd.Next(0, poyaskol);
                                        }
                                    }
                                            else
                                            {
                                                break;
                                            }
                                }
                                k++;
                                click("LCL", HWIDch, center, randomed);

                                logs[threadnumber]="жду кнопку варпа";
                                center = FindCountour( ref images.moveto, "moveto");
                                if (center[0].X == -1)
                                {
                                    return -1;
                                }
                                if (center[0].X == -2)
                                {
                                    return -2;
                                }
                                while (center[0].X == 0)
                                {
                                    checkloc =checklocal();
                                    if (checkloc == 1)
                                    {
                                       leavegame();
                                        return -3;
                                    }
                                    center = FindCountour( ref images.moveto, "moveto");
                                    Task.Delay(1000).Wait();
                                }
                                    tempcenter2 = FindCountour(ref images.warp, "warp");
                                }
                                Task.Delay(1000).Wait();
                                center = FindCountour( ref images.warp, "warp");
                                if (center[0].X == -1)
                                {
                                    return -1;
                                }
                                if (center[0].X == -2)
                                {
                                    return -2;
                                }
                                if (center[0].X != 0)
                                {
                                    click("LCL", HWIDch, center, 0);
                                    center = FindCountour(ref images.poyas, "poyas");
                                    warpedto[0].X = center[randomed].X;
                                    warpedto[0].Y = center[randomed].Y;
                                    break;
                                }
                                center = FindCountour( ref images.poyas, "poyas");
                                Task.Delay(1000).Wait();
                            }
                            k = 0;
                            int chkrres = 0;

                            logs[threadnumber]="жду прилета";

                            while (priletel == false)
                            {
                                checkloc =checklocal();
                                if (checkloc == 1)
                                {
                                   leavegame();
                                    return -3;
                                }
                                chkrres =checkrange(warpedto);
                                if(chkrres == -1)
                                {
                                    return -1;
                                }
                                if (chkrres == -2)
                                {
                                    return -2;
                                }
                                if(chkrres == -3)
                                {
                                    return -3;
                                }
                                if (chkrres == 0)
                                {
                                    break;
                                }
                                if (chkrres == 1)
                                {
                                    poletel = false;
                                    tempcenter2[0].X = 0;
                                    while (poletel == false)
                                    {
                                        while (tempcenter2[0].X == 0)
                                        {

                                            center = FindCountour(ref images.poyas, "poyas");
                                            poyaskol = 0;
                                            for (int j = 0; j < 10; j++)
                                            {
                                                if (center[j].X != 0)
                                                {
                                                    poyaskol++;
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                            checkloc = checklocal();
                                            if (checkloc == 1)
                                            {
                                                leavegame();
                                                return -3;
                                            }
                                            randomed = rnd.Next(0, poyaskol);
                                            wasrandomed[k] = randomed;
                                            if (k != 0)
                                                for (int j = 0; j < k; j++)
                                                {
                                                    if (wasrandomed[j] == randomed)
                                                    {
                                                        while (wasrandomed[j] == randomed)
                                                        {
                                                            randomed = rnd.Next(0, poyaskol);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        break;
                                                    }
                                                }
                                            k++;
                                            click("LCL", HWIDch, center, randomed);

                                            logs[threadnumber] = "жду кнопку варпа";
                                            center = FindCountour(ref images.moveto, "moveto");
                                            if (center[0].X == -1)
                                            {
                                                return -1;
                                            }
                                            if (center[0].X == -2)
                                            {
                                                return -2;
                                            }
                                            while (center[0].X == 0)
                                            {
                                                checkloc = checklocal();
                                                if (checkloc == 1)
                                                {
                                                    leavegame();
                                                    return -3;
                                                }
                                                center = FindCountour(ref images.moveto, "moveto");
                                                
                                                Task.Delay(1000).Wait();
                                            }
                                            tempcenter2 = FindCountour(ref images.warp, "warp");
                                        }
                                        Task.Delay(1000).Wait();
                                        center = FindCountour(ref images.warp, "warp");
                                        if (center[0].X == -1)
                                        {
                                            return -1;
                                        }
                                        if (center[0].X == -2)
                                        {
                                            return -2;
                                        }
                                        if (center[0].X != 0)
                                        {
                                            click("LCL", HWIDch, center, 0);
                                            center = FindCountour(ref images.poyas, "poyas");
                                            warpedto[0].X = center[randomed].X;
                                            warpedto[0].Y = center[randomed].Y;
                                            break;
                                        }
                                        center = FindCountour(ref images.poyas, "poyas");
                                        Task.Delay(1000).Wait();
                                    }
                                }
                                Task.Delay(1000).Wait();
                            }
                            center = FindCountour( ref images.filtraster, "filtraster");
                           click("LCL", HWIDch, center, 0);
                        }
                        else
                        {
                            click("LCL", HWIDch, center, 0);
                        }
                        kol = 0;
                        center = FindCountour(ref images.aster, "aster");
                        while (center[0].X == 0)
                        {
                            checkloc =checklocal();
                            if (checkloc == 1)
                            {
                               leavegame();
                                return -3;
                            }
                            center = FindCountour( ref images.aster, "aster");
                            if (center[0].X == -1)
                            {
                                return -1;
                            }
                            if (center[0].X == -2)
                            {
                                return -2;
                            }
                            Task.Delay(500).Wait();
                            kol++;
                            if (kol > 15)
                            {
                                break;
                            }
                        }  
                            
                        

                    }
                    else
                    {
                        poletel = false;
                        tempcenter2[0].X = 0;
                        nenashel = false;
                       
                        checkloc =checklocal();
                        if (checkloc == 1)
                        {
                           leavegame();
                            return -3;
                        }

                        logs[threadnumber] = "нажимаю фильтр поясов";

                        center = FindCountour(ref images.filtrpoyas, "filtrpoyas");
                        if (center[0].X == -1)
                        {
                            return -1;
                        }
                        if (center[0].X == -2)
                        {
                            return -2;
                        }
                        click("LCL", HWIDch, center, 0);
                        Task.Delay(1000).Wait();

                        center = FindCountour(ref images.poyas, "poyas");
                        while (center[0].X == 0)
                        {
                            checkloc = checklocal();
                            if (checkloc == 1)
                            {
                                leavegame();
                                return -3;
                            }
                            center = FindCountour(ref images.poyas, "poyas");
                            Task.Delay(1000).Wait();
                        }

                        for (int j = 0; j < 10; j++)
                        {
                            if (center[j].X != 0)
                            {
                                poyaskol++;
                            }
                            else
                            {
                                break;
                            }
                        }


                        int[] wasrandomed = new int[poyaskol];

                        logs[threadnumber] = "варпуюсь";
                        tempcenter2[0].X = 0;
                        while (poletel == false)
                        {
                            checkloc = checklocal();
                            if (checkloc == 1)
                            {
                                leavegame();
                                return -3;
                            }
                            while (tempcenter2[0].X == 0)
                            {
                                checkloc = checklocal();
                                if (checkloc == 1)
                                {
                                    leavegame();
                                    return -3;
                                }
                                center = FindCountour(ref images.poyas, "poyas");
                                poyaskol = 0;
                                for (int j = 0; j < 10; j++)
                                {
                                    if (center[j].X != 0)
                                    {
                                        poyaskol++;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                checkloc = checklocal();
                                if (checkloc == 1)
                                {
                                    leavegame();
                                    return -3;
                                }
                                randomed = rnd.Next(0, poyaskol);
                                wasrandomed[k] = randomed;
                                if (k != 0)
                                    for (int j = 0; j < k; j++)
                                    {
                                        if (wasrandomed[j] == randomed)
                                        {
                                            while (wasrandomed[j] == randomed)
                                            {
                                                randomed = rnd.Next(0, poyaskol);
                                            }
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                k++;

                                click("LCL", HWIDch, center, randomed);

                                logs[threadnumber] = "жду кнопку варпа";
                                center = FindCountour(ref images.moveto, "moveto");
                                if (center[0].X == -1)
                                {
                                    return -1;
                                }
                                if (center[0].X == -2)
                                {
                                    return -2;
                                }
                                while (center[0].X == 0)
                                {
                                    checkloc = checklocal();
                                    if (checkloc == 1)
                                    {
                                        leavegame();
                                        return -3;
                                    }
                                    center = FindCountour(ref images.moveto, "moveto");
                                    Task.Delay(1000).Wait();
                                }
                                tempcenter2 = FindCountour(ref images.warp, "warp");
                                Task.Delay(1000).Wait();
                            }
                            Task.Delay(1000).Wait();
                            center = FindCountour(ref images.warp, "warp");
                            if (center[0].X == -1)
                            {
                                return -1;
                            }
                            if (center[0].X == -2)
                            {
                                return -2;
                            }
                            if (center[0].X != 0)
                            {
                                click("LCL", HWIDch, center, 0);
                                center = FindCountour(ref images.poyas, "poyas");
                                warpedto[0].X = center[randomed].X;
                                warpedto[0].Y = center[randomed].Y;
                                break;
                            }
                            center = FindCountour(ref images.poyas, "poyas");
                        }
                        k = 0;
                        int chkrres = 0;

                        logs[threadnumber] = "жду прилета";

                        while (priletel == false)
                        {

                            checkloc = checklocal();
                            if (checkloc == 1)
                            {
                                leavegame();
                                return -3;
                            }
                            chkrres = checkrange(warpedto);
                            if (chkrres == -1)
                            {
                                return -1;
                            }
                            if (chkrres == -2)
                            {
                                return -2;
                            }
                            if (chkrres == -3)
                            {
                                return -3;
                            }
                            if (chkrres == 0)
                            {
                                break;
                            }
                            if (chkrres == 1)
                            {
                                poletel = false;
                                    tempcenter2[0].X = 0;
                                while (poletel == false)
                                {
                                    while (tempcenter2[0].X == 0)
                                    {

                                        center = FindCountour(ref images.poyas, "poyas");
                                        poyaskol = 0;
                                        for (int j = 0; j < 10; j++)
                                        {
                                            if (center[j].X != 0)
                                            {
                                                poyaskol++;
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                        checkloc = checklocal();
                                        if (checkloc == 1)
                                        {
                                            leavegame();
                                            return -3;
                                        }
                                        randomed = rnd.Next(0, poyaskol);
                                        wasrandomed[k] = randomed;
                                        if (k != 0)
                                            for (int j = 0; j < k; j++)
                                            {
                                                if (wasrandomed[j] == randomed)
                                                {
                                                    while (wasrandomed[j] == randomed)
                                                    {
                                                        randomed = rnd.Next(0, poyaskol);
                                                    }
                                                }
                                                else
                                                {
                                                    break;
                                                }
                                            }
                                        k++;
                                        click("LCL", HWIDch, center, randomed);

                                        logs[threadnumber] = "жду кнопку варпа";
                                        center = FindCountour(ref images.moveto, "moveto");
                                        if (center[0].X == -1)
                                        {
                                            return -1;
                                        }
                                        if (center[0].X == -2)
                                        {
                                            return -2;
                                        }
                                        while (center[0].X == 0)
                                        {
                                            checkloc = checklocal();
                                            if (checkloc == 1)
                                            {
                                                leavegame();
                                                return -3;
                                            }
                                            center = FindCountour(ref images.moveto, "moveto");
                                           
                                            Task.Delay(1000).Wait();
                                        }
                                        tempcenter2 = FindCountour(ref images.warp, "warp");
                                    }
                                    Task.Delay(1000).Wait();
                                    center = FindCountour(ref images.warp, "warp");
                                    if (center[0].X == -1)
                                    {
                                        return -1;
                                    }
                                    if (center[0].X == -2)
                                    {
                                        return -2;
                                    }
                                    if (center[0].X != 0)
                                    {
                                        click("LCL", HWIDch, center, 0);
                                        center = FindCountour(ref images.poyas, "poyas");
                                        warpedto[0].X = center[randomed].X;
                                        warpedto[0].Y = center[randomed].Y;
                                        break;
                                    }
                                    center = FindCountour(ref images.poyas, "poyas");
                                    Task.Delay(1000).Wait();
                                }
                            }
                            Task.Delay(1000).Wait();
                        }
                        center = FindCountour(ref images.filtraster, "filtraster");
                        click("LCL", HWIDch, center, 0);

                        Task.Delay(5000).Wait();
                        center = FindCountour( ref images.aster, "aster");
                        kol =0;
                        while (center[0].X == 0)
                        {
                            checkloc =checklocal();
                            if (checkloc == 1)
                            {
                               leavegame();
                                return -3;
                            }
                            center = FindCountour( ref images.aster, "aster");
                            if (center[0].X == -1)
                            {
                                return -1;
                            }
                            if (center[0].X == -2)
                            {
                                return -2;
                            }
                            Task.Delay(500).Wait();
                            kol++;
                            if (kol > 15)
                            {
                                break;
                            }
                        }
                    }

                    checkloc =checklocal();
                    if (checkloc == 1)
                    {
                       leavegame();
                        return -3;
                    }
                    int asterfarmres = 0;
                    int rez=0;
                    switch (rudetomine)
                    {
                        case "highsec":
                            {
                                rez = 1;
                                break;
                            }
                        case "lowsec":
                            {
                                rez = 2;
                                break;
                            }
                    }
                    Task.Delay(10000).Wait();
                    kol = 0;
                        while (asterfarmres != 5)
                        {
                        checkloc =checklocal();
                        if (checkloc == 1)
                        {
                           leavegame();
                            return -3;
                        }

                        asterfarmres =asterfarm( rez);
                            if (asterfarmres == -1)
                            {
                                return -1;
                            }
                            if (asterfarmres == -2)
                            {
                                return -2;
                            }
                            if (asterfarmres == 0)
                            {
                                nenashel = true;
                                break;
                            }
                            
                            if(asterfarmres == 5)
                            {
                               return 5;
                            }
                            if(asterfarmres == -3)
                            {
                                return -3;
                            }

                        }
                }
                return 0;
            }
            int whatmeteor(ref Image<Bgr, byte> meteor ,ref Image<Bgr, byte> targetedmeteor,out string meteortoFC,out string targetedmeteornametoFC)
            {
                Point[] center = new Point[10];
                
               
                for(int i=0;i<10;i++)
               switch (minerals[i])
                {
                        case "bistode":
                            {
                                center = FindCountour(ref images.bistode, "bistode");
                                if (center[0].X == -1)
                                {
                                    meteortoFC = "bistode";
                                    targetedmeteornametoFC = "targetedbistodefarm";
                                    return -1;
                                }
                                if (center[0].X == -2)
                                {
                                    meteortoFC = "bistode";
                                    targetedmeteornametoFC = "targetedbistodefarm";
                                    return -2;
                                }
                                if (center[0].X != 0)
                                {
                                    meteor = images.bistode;
                                    meteortoFC = "bistode";
                                    targetedmeteor = images.targetedbistodefarm;
                                    targetedmeteornametoFC = "targetedbistodefarm";
                                    return 2;
                                }
                                meteortoFC = "bistode";
                                targetedmeteornametoFC = "targetedbistodefarm";
                                break;
                            }
                    case "crokite":
                        {
                                center = FindCountour(ref images.crokite, "crokite");
                                if (center[0].X == -1)
                                {
                                    meteortoFC = "crokite";
                                    targetedmeteornametoFC = "targetedcrokitefarm";
                                    return -1;
                                }
                                if (center[0].X == -2)
                                {
                                    meteortoFC = "crokite";
                                    targetedmeteornametoFC = "targetedcrokitefarm";
                                    return -2;
                                }
                                if (center[0].X != 0)
                                {
                                    meteor = images.crokite;
                                    meteortoFC = "crokite";
                                    targetedmeteor = images.targetedcrokitefarm;
                                    targetedmeteornametoFC = "targetedcrokitefarm";
                                    return 2;
                                }
                                meteortoFC = "crokite";
                                targetedmeteornametoFC = "targetedcrokitefarm";
                                break;
                            }
                        
                        case "arkanor":
                            {
                                center = FindCountour(ref images.arcanor, "arcanor");
                                if (center[0].X == -1)
                                {
                                    meteortoFC = "arcanor";
                                    targetedmeteornametoFC = "targetedarcanorfarm";
                                    return -1;
                                }
                                if (center[0].X == -2)
                                {
                                    meteortoFC = "arcanor";
                                    targetedmeteornametoFC = "targetedarcanorfarm";
                                    return -2;
                                }
                                if (center[0].X != 0)
                                {
                                    meteor = images.arcanor;
                                    meteortoFC = "arcanor";
                                    targetedmeteor = images.targetedarcanorfarm;
                                    targetedmeteornametoFC = "targetedarcanorfarm";
                                    return 2;
                                }
                                meteortoFC = "arcanor";
                                targetedmeteornametoFC = "targetedarcanorfarm";
                                break;
                            }
                        case "gneiss":
                            {
                                center = FindCountour(ref images.gneiss, "gneiss");
                                if (center[0].X == -1)
                                {
                                    meteortoFC = "gneiss";
                                    targetedmeteornametoFC = "targetedgneissfarm";
                                    return -1;
                                }
                                if (center[0].X == -2)
                                {
                                    meteortoFC = "gneiss";
                                    targetedmeteornametoFC = "targetedgneissfarm";
                                    return -2;
                                }
                                if (center[0].X != 0)
                                {
                                    meteor = images.gneiss;
                                    meteortoFC = "gneiss";
                                    targetedmeteor = images.targetedgneissfarm;
                                    targetedmeteornametoFC = "targetedgneissfarm";
                                    return 2;
                                }
                                meteortoFC = "gneiss";
                                targetedmeteornametoFC = "targetedgneissfarm";
                                break;
                            }
                        case "hedbergite":
                            {
                                center = FindCountour(ref images.hedbergite, "hedbergite");
                                if (center[0].X == -1)
                                {
                                    meteortoFC = "hedbergite";
                                    targetedmeteornametoFC = "targetedhedbergitefarm";
                                    return -1;
                                }
                                if (center[0].X == -2)
                                {
                                    meteortoFC = "hedbergite";
                                    targetedmeteornametoFC = "targetedhedbergitefarm";
                                    return -2;
                                }
                                if (center[0].X != 0)
                                {
                                    meteor = images.hedbergite;
                                    meteortoFC = "hedbergite";
                                    targetedmeteor = images.targetedhedbergitefarm;
                                    targetedmeteornametoFC = "targetedhedbergitefarm";
                                    return 2;
                                }
                                meteortoFC = "hedbergite";
                                targetedmeteornametoFC = "targetedhedbergitefarm";
                                break;
                            }
                        case "spodumain":
                            {
                                center = FindCountour(ref images.spodumain, "spodumain");
                                if (center[0].X == -1)
                                {
                                    meteortoFC = "spodumain";
                                    targetedmeteornametoFC = "targetedspodumainfarm";
                                    return -1;
                                }
                                if (center[0].X == -2)
                                {
                                    meteortoFC = "spodumain";
                                    targetedmeteornametoFC = "targetedspodumainfarm";
                                    return -2;
                                }
                                if (center[0].X != 0)
                                {
                                    meteor = images.spodumain;
                                    meteortoFC = "spodumain";
                                    targetedmeteor = images.targetedspodumainfarm;
                                    targetedmeteornametoFC = "targetedspodumainfarm";
                                    return 2;
                                }
                                meteortoFC = "hedbergite";
                                targetedmeteornametoFC = "targetedhedbergitefarm";
                                break;
                            }
                        case "jaspet":
                            {
                                center = FindCountour(ref images.jaspet, "jaspet");
                                if (center[0].X == -1)
                                {
                                    meteortoFC = "jaspet";
                                    targetedmeteornametoFC = "targetedjaspetfarm";
                                    return -1;
                                }
                                if (center[0].X == -2)
                                {
                                    meteortoFC = "jaspet";
                                    targetedmeteornametoFC = "targetedjaspetfarm";
                                    return -2;
                                }
                                if (center[0].X != 0)
                                {
                                    meteor = images.jaspet;
                                    meteortoFC = "jaspet";
                                    targetedmeteor = images.targetedjaspetfarm;
                                    targetedmeteornametoFC = "targetedjaspetfarm";
                                    return 2;
                                }
                                meteortoFC = "jaspet";
                                targetedmeteornametoFC = "targetedjaspetfarm";
                                break;
                            }
                        case "pyroxeres":
                            {
                                center = FindCountour(ref images.pyroxeres, "pyroxeres");
                                if (center[0].X == -1)
                                {
                                    meteortoFC = "pyroxeres";
                                    targetedmeteornametoFC = "targetedpyroxeresfarm";
                                    return -1;
                                }
                                if (center[0].X == -2)
                                {
                                    meteortoFC = "pyroxeres";
                                    targetedmeteornametoFC = "targetedpyroxeresfarm";
                                    return -2;
                                }
                                if (center[0].X != 0)
                                {
                                    meteor = images.pyroxeres;
                                    meteortoFC = "pyroxeres";
                                    targetedmeteor = images.targetedpyroxeresfarm;
                                    targetedmeteornametoFC = "targetedpyroxeresfarm";
                                    return 2;
                                }
                                meteortoFC = "pyroxeres";
                                targetedmeteornametoFC = "targetedpyroxeresfarm";
                                break;
                            }

                    }
                /*
                center = FindCountour( ref images.arcanor);
                if (center[0].X == -1)
                {
                    return -1;
                }
                if (center[0].X == -2)
                {
                    return -2;
                }
                if (center[0].X != 0)
                {
                    meteor = images.arcanor;
                    targetedmeteor = images.targetedarcanorfarm;
                    return 3;
                }*/
                meteortoFC = null;
                targetedmeteornametoFC = null ;
                return 0;
            }
            int kolvoswipe = 0;
            int asterfarm(int rezhim)//rezhim == 1 - kernite,2-crokite(включая бистод и арканор). 
            {//return 0 - метеориты закончились, 5 - инвентарь фулловый, -1 - вызвано завершение, -2 - хлопнулся нокс, -3 - исчез метеорит
                Task.Delay(2000).Wait();
                Point[] center = new Point[10];
                bool near = false;
                int meteorkol = 0;
                Image<Bgr, byte> meteor=null;
                int whatmres = 0;
                Image<Bgr, byte> targetedmeteor = null;
                string loglabel = "log" + threadnumber.ToString();

                
                int kolvodist = 0;
                int kolvodisttemp = 0;
                int checkloc=0;
                string meteortoFC=null;
                string targetedmeteortoFC=null ;
                logs[threadnumber] = "ищу нужный астероид";
                checkloc =checklocal();
                if (checkloc == 1)
                {
                   leavegame();
                    return -3;
                }
                switch (rezhim)
                {
                    case 1:
                        {

                            center = FindCountour( ref images.kernite, "kernite");
                            if (center[0].X == -1)
                            {
                                return -1;
                            }
                            if (center[0].X == -2)
                            {
                                return -2;
                            }
                            for (int i = 0; i < 10; i++)
                                if (center[i].X != 0)
                                    meteorkol++;
                            break;
                        }
                    case 2:
                        {
                            
                            whatmres = whatmeteor( ref meteor, ref targetedmeteor,out meteortoFC,out targetedmeteortoFC);
                            if (whatmres == -1)
                            {
                                return -1;
                            }
                            if (whatmres == -2)
                            {
                                return -2;
                            }
                            if(whatmres== 0)
                            {
                                break;
                            }
                            center = FindCountour(ref  meteor,meteortoFC);
                            if (center[0].X == -1)
                            {
                                return -1;
                            }
                            if (center[0].X == -2)
                            {
                                return -2;
                            }
                            for (int i = 0; i < 10; i++)
                                if (center[i].X != 0)
                                    meteorkol++;
                            
                            break;
                        }
                   
                }
                checkloc = checklocal();
                if (checkloc == 1)
                {
                    leavegame();
                    return -3;
                }
                while (meteorkol == 0)
                {
                    kolvoswipe++;
                    if(kolvoswipe > 3)
                    {
                        logs[threadnumber] = "нужных астероидов нет";
                        kolvoswipe = 0;
                        return 0;
                    }
                    center = FindCountour(ref images.rightmenu, "rightmenu");
                    center[0].X += 50;
                    click("LeftDown", HWIDch, center, 0);
                    int j;
                    for (j = center[0].Y; j > center[0].Y - 80; j = j - 4)
                    {
                        SendMessage(HWIDm, WM_MOUSEMOVE, 0x0201, MakeLParam(center[0].X, j));
                        Task.Delay(50).Wait();
                    }
                    SendMessage(HWIDm, WM_MOUSEMOVE, 0x0202, MakeLParam(center[0].X, j));
                    click("LeftUp", HWIDch, center, 0);
                    Task.Delay(2000).Wait();
                    logs[threadnumber] = "ищу нужный астероид";
                    checkloc = checklocal();
                    if (checkloc == 1)
                    {
                        leavegame();
                        return -3;
                    }
                    switch (rezhim)
                    {
                        case 1:
                            {

                                center = FindCountour(ref images.kernite, "kernite");
                                if (center[0].X == -1)
                                {
                                    return -1;
                                }
                                if (center[0].X == -2)
                                {
                                    return -2;
                                }
                                for (int i = 0; i < 10; i++)
                                    if (center[i].X != 0)
                                        meteorkol++;
                                break;
                            }
                        case 2:
                            {

                                whatmres = whatmeteor(ref meteor, ref targetedmeteor, out meteortoFC, out targetedmeteortoFC);
                                if (whatmres == -1)
                                {
                                    return -1;
                                }
                                if (whatmres == -2)
                                {
                                    return -2;
                                }
                                if (whatmres == 0)
                                {
                                    break;
                                }
                                center = FindCountour(ref meteor, meteortoFC);
                                if (center[0].X == -1)
                                {
                                    return -1;
                                }
                                if (center[0].X == -2)
                                {
                                    return -2;
                                }
                                for (int i = 0; i < 10; i++)
                                    if (center[i].X != 0)
                                        meteorkol++;

                                break;
                            }

                    }
                    checkloc = checklocal();
                    if (checkloc == 1)
                    {
                        leavegame();
                        return -3;
                    }
                }
                string[] distances = new string[10];
                int nearestmeteor=-1;
                int tempmeteor=999;


                logs[threadnumber]="ищу ближайший метеорит";
                switch (rezhim)
                {
                    case 1:
                        {

                            lock (locker)
                                distances = finddistace(1, meteorkol, ref images.kernite, 0,ref meteortoFC);
                           
                            if(distances[0] == "-1")
                            {
                                return -2;
                            }

                            for(int i = 0; i < distances.Length; i++)
                            {
                                if (distances[i] == null)
                                    break;
                                if (distances[i] != "")
                                {
                                    if (distances[i].Length == 1)
                                    {
                                        nearestmeteor = i;
                                        break;
                                    }
                                }
                                else
                                {
                                    return 1;
                                }
                            }
                            if(nearestmeteor == -1)
                            {
                                nearestmeteor = 999;
                                for (int i = 0; i < distances.Length; i++)
                                {
                                    if (distances[i] == null)
                                        break;
                                    if (distances[i] != "")
                                    { 
                                        if (Convert.ToInt32(distances[i]) < tempmeteor)
                                        {
                                            tempmeteor = Convert.ToInt32(distances[i]);
                                            nearestmeteor = i;
                                        }
                                    }
                                    else
                                    {
                                        return 1;
                                    }

                            }

                               

                            }
                            break;
                        }
                    case 2:
                        {

                            lock (locker)
                                distances = finddistace(2, meteorkol,ref meteor,whatmres,ref meteortoFC);
                            if (distances[0] == "-1")
                            {
                                return -2;
                            }
                            for (int i = 0; i < meteorkol; i++)
                            {
                                if (distances[i] == null)
                                    break;
                                if (distances[i] != "")
                                {
                                    if (distances[i].Length == 1)
                                    {
                                        if (distances[i] == null)
                                            break;
                                        nearestmeteor = i;
                                        break;
                                    }
                                }
                                 else
                                 {
                                    return 1;
                                 }
                            }
                            if (nearestmeteor == -1)
                            {
                                nearestmeteor = 999;
                                for (int i = 0; i < meteorkol; i++)
                                {
                                    if (distances[i] == null)
                                        break;
                                    if (distances[i] != "")
                                    {
                                        if (Convert.ToInt32(distances[i]) < tempmeteor)
                                        {
                                            tempmeteor = Convert.ToInt32(distances[i]);
                                            nearestmeteor = i;
                                        }
                                    }
                                    else
                                    {
                                        return 1;
                                    }
                                }

                            }
                            break;
                        }
                    
                }
                if (distances[nearestmeteor] == "")
                {
                    return 1;
                }
                if (distances[nearestmeteor] == null)
                {
                    return 1;
                }
                checkloc = checklocal();
                if (checkloc == 1)
                {
                    leavegame();
                    return -3;
                }
                int distace = Convert.ToInt32(distances[nearestmeteor]);
                kolvodist = distances.Length;
                Point[] tempcenter = new Point[10];
                Point[] tempcenter2 = new Point[10];
                int sch = 0;
                int tempkolaster = 0;
                logs[threadnumber]="беру астероид в таргет";
                click("LCL", HWIDch, center, nearestmeteor);
                tempcenter = FindCountour( ref images.gettarget, "gettarget");
                sch = 0;
                if(tempcenter[0].X ==0)

                while (tempcenter[0].X == 0)
                {
                        checkloc =checklocal();
                        if (checkloc == 1)
                        {
                           leavegame();
                            return -3;
                        }
                        tempcenter = FindCountour( ref images.gettarget, "gettarget");
                        if (tempcenter[0].X != 0)
                        {
                            while(tempcenter[0].X != 0)
                            {
                                click("LCL", HWIDch, tempcenter, 0);
                                Task.Delay(1000).Wait();
                                tempcenter = FindCountour(ref images.gettarget, "gettarget");
                                if ((tempcenter2 = FindCountour(ref images.canceltarget, "canceltarget"))[0].X != 0)
                                {
                                    break;
                                }
                            }
                           
                           
                            break;
                        }
                    Task.Delay(1000).Wait();
                   
                        if((tempcenter2 = FindCountour( ref images.canceltarget, "canceltarget"))[0].X != 0)
                        {
                                break;
                        }
                        else
                        {
                            click("LCL", HWIDch, center, nearestmeteor);
                        }
                    
                }
                else
                click("LCL", HWIDch, tempcenter, 0);
                Task.Delay(2000).Wait();
                Point[] temppoint = new Point[10];
                int tempkol = 0;
                
                if (distace > 17)
                {
                    checkloc =checklocal();
                    if (checkloc == 1)
                    {
                       leavegame();
                        return -3;
                    }
                    temppoint = FindCountour(ref meteor,meteortoFC);
                    if (temppoint[0].X == -1)
                    {
                        return -1;
                    }
                    if (temppoint[0].X == -2)
                    {
                        return -2;
                    }

                    tempcenter2 = FindCountour(ref meteor, meteortoFC);
                    if (tempcenter2[0].X == -1)
                    {
                        return -1;
                    }
                    if (tempcenter2[0].X == -2)
                    {
                        return -2;
                    }
                    for (int i = 0; i < 10; i++)
                        if (tempcenter2[i].X != 0)
                        {
                            tempkolaster++;

                        }
                        else
                        {
                            break;
                        }

                    if (tempkolaster != meteorkol)
                    {
                        return 1;
                    }
                    tempkolaster = 0;

                    logs[threadnumber]="лечу к нужному астероиду";
                  
                    tempcenter[0].X = 720;
                    tempcenter[0].Y = 700;
                    click("LCL", HWIDch, tempcenter, 0);
                    Task.Delay(5000).Wait();
                    click("LCL",HWIDch,center,nearestmeteor);
                    Task.Delay(1000).Wait();
                    tempcenter = FindCountour( ref images.moveto, "moveto");
                    if (tempcenter[0].X == -1)
                    {
                        return -1;
                    }
                    if (tempcenter[0].X == 0)
                    {
                        while (tempcenter[0].X == 0)
                        {
                            tempcenter = FindCountour( ref images.moveto, "moveto");


                            Task.Delay(1000).Wait();
                            center = FindCountour(ref meteor, meteortoFC);
                            if (center[0].X == -1)
                            {
                                return -1;
                            }
                            if (center[0].X == -2)
                            {
                                return -2;
                            }
                            for (int i = 0; i < 10; i++)
                                if (center[i].X != 0)
                                {
                                    tempkolaster++;

                                }
                                else
                                {
                                    break;
                                }

                            if (tempkolaster != meteorkol)
                            {
                                return 1;
                            }
                            tempkolaster = 0;
                        }
                    }
                    click("LCL", HWIDch, tempcenter, 0);
                    Random rnd = new Random();
                    while (distace > 17)
                    {
                       
                        temppoint = FindCountour( ref meteor,meteortoFC);
                        if (temppoint[0].X == -1)
                        {
                            return -1;
                        }
                        if (temppoint[0].X == -2)
                        {
                            return -2;
                        }
                      
                        checkloc =checklocal();
                        if (checkloc == 1)
                        {
                           leavegame();
                            return -3;
                        }
                        
                        Task.Delay(1000).Wait();

                        tempcenter2 = FindCountour(ref meteor, meteortoFC);
                        if (tempcenter2[0].X == -1)
                        {
                            return -1;
                        }
                        if (tempcenter2[0].X == -2)
                        {
                            return -2;
                        }
                        for (int i = 0; i < 10; i++)
                            if (tempcenter2[i].X != 0)
                            {
                                tempkolaster++;

                            }
                            else
                            {
                                break;
                            }

                        if (tempkolaster != meteorkol)
                        {
                            return 1;
                        }
                        tempkolaster = 0;

                        switch (rezhim)
                        {
                            case 1:
                                {

                                    tempcenter = FindCountour( ref images.kernite, "kernite");
                                    if (center[nearestmeteor] != tempcenter[nearestmeteor])
                                    {
                                        return 1;
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    tempcenter = FindCountour( ref meteor,meteortoFC);
                                    if (center[nearestmeteor] != tempcenter[nearestmeteor])
                                    {
                                        return 1;
                                    }
                                    break;
                                }
                           
                               
                        }
                        switch (rezhim)
                        {
                            case 1:
                                {
                                    center = FindCountour( ref meteor,meteortoFC);
                                    if (center[0].X == -1)
                                    {
                                        return -1;
                                    }
                                    if (center[0].X == -2)
                                    {
                                        return -2;
                                    }
                                    
                                    distances = finddistace(rezhim, meteorkol, ref meteor,0,ref meteortoFC);
                                    break;
                                }
                            case 2:
                                {
                                    center = FindCountour( ref meteor, meteortoFC);
                                    if (center[0].X == -1)
                                    {
                                        return -1;
                                    }
                                    if (center[0].X == -2)
                                    {
                                        return -2;
                                    }
                                   
                                        distances = finddistace(rezhim, meteorkol, ref meteor,whatmres,ref meteortoFC);
                                    break;
                                }


                        }
                        checkloc = checklocal();
                        if (checkloc == 1)
                        {
                            leavegame();
                            return -3;
                        }
                        tempcenter2 = FindCountour(ref meteor, meteortoFC);
                        if (tempcenter2[0].X == -1)
                        {
                            return -1;
                        }
                        if (tempcenter2[0].X == -2)
                        {
                            return -2;
                        }
                        for (int i = 0; i < 10; i++)
                            if (tempcenter2[i].X != 0)
                            {
                                tempkolaster++;

                            }
                            else
                            {
                                break;
                            }

                        if (tempkolaster != meteorkol)
                        {
                            return 1;
                        }
                        tempkolaster = 0;
                        if (distances[0] == "-1")
                        {
                            return -2;
                        }
                        if(distances[nearestmeteor] == "")
                        {
                            return 1;
                        }
                        if (distances[nearestmeteor] == null)
                        {
                            return 1;
                        }
                        distace = Convert.ToInt32(distances[nearestmeteor]);
                        checkloc = checklocal();
                        if (checkloc == 1)
                        {
                            leavegame();
                            return -3;
                        }
                        Task.Delay(1000).Wait();
                    }
                    Task.Delay(2000).Wait();
                    //tempcenter[0].X = 670;
                    //tempcenter[0].Y = 730;
                    tempcenter[0].X = 720;
                    tempcenter[0].Y = 700;
                    click("LCL", HWIDch, tempcenter, 0);
                    Task.Delay(1000).Wait();
                    tempcenter = FindCountour( ref images.speed0, "speed0");
                    click("LCL", HWIDch, tempcenter, 0);
                   
                }

               
                int kolg = 0;
                tempcenter[0].X = 0;
                Task.Delay(1000).Wait();
               
                tempcenter[0].X = 0;
                sch = 0;
               
                center = FindCountour(ref meteor, meteortoFC);
                if (center[0].X == -1)
                {
                    return -1;
                }
                if (center[0].X == -2)
                {
                    return -2;
                }
                for (int i = 0; i < 10; i++)
                    if (center[i].X != 0)
                    {
                        tempkolaster++;

                    }
                    else
                    {
                        break;
                    }
                       
                if(tempkolaster < meteorkol)
                {
                    return 1;
                }
                tempkolaster = 0;
                logs[threadnumber]="жму пушки";
                while (kolg < gunscol)
                {
                    checkloc =checklocal();
                    if (checkloc == 1)
                    {
                       leavegame();
                        return -3;
                    }
                  
                    click("LCL", HWIDch, center, nearestmeteor);
                    Task.Delay(5000).Wait();
                    if (tempcenter[0].X == 0)
                    {
                       
                        while (tempcenter[0].X == 0)
                        {
                            checkloc =checklocal();
                            if (checkloc == 1)
                            {
                               leavegame();
                                return -3;
                            }
                            //изменить на проверку количества обнаруженных метеоритов, а не метеорита в таргете. если обнаруженных не совпало, то вернуть 1

                            center = FindCountour(ref meteor, meteortoFC);
                            if (center[0].X == -1)
                            {
                                return -1;
                            }
                            if (center[0].X == -2)
                            {
                                return -2;
                            }
                            for (int i = 0; i < 10; i++)
                                if (center[i].X != 0)
                                {
                                    tempkolaster++;

                                }
                                else
                                {
                                    break;
                                }

                            if (tempkolaster < meteorkol)
                            {
                                return 1;
                            }
                            tempkolaster = 0;
                            Task.Delay(1000).Wait();
                            tempcenter = FindCountour( ref images.gun1,"gun1");
                            if(tempcenter[0].X==0)
                            click("LCL", HWIDch, center, nearestmeteor);
                            Task.Delay(5000).Wait();

                        }
                        click("LCL", HWIDch, tempcenter, 0);
                    }
                    else
                        click("LCL", HWIDch, tempcenter, 0);
                    kolg++;
                    center = FindCountour(ref meteor, meteortoFC);
                    if (center[0].X == -1)
                    {
                        return -1;
                    }
                    if (center[0].X == -2)
                    {
                        return -2;
                    }
                    for (int i = 0; i < 10; i++)
                        if (center[i].X != 0)
                        {
                            tempkolaster++;

                        }
                        else
                        {
                            break;
                        }

                    if (tempkolaster < meteorkol)
                    {
                        return 1;
                    }
                    tempkolaster = 0;
                    tempcenter[0].X = 0;
                    Task.Delay(1000).Wait();
                    checkloc = checklocal();
                    if (checkloc == 1)
                    {
                        leavegame();
                        return -3;
                    }
                }
                kolg = 0;


              
                switch (rezhim)
                {
                    case 1:
                        {
                           
                            tempcenter = FindCountour( ref images.targetedkernitefarm, "targetedkernitefarm");
                            while (tempcenter[0].X !=0)
                            {

                                logs[threadnumber]="проверяю руду в таргете";
                                tempcenter = FindCountour( ref images.targetedkernitefarm, "targetedkernitefarm");
                                if (checkmass(HWIDch) == 1)
                                {
                                    return 5;
                                }
                                checkloc =checklocal();
                                if (checkloc == 1)
                                {
                                   leavegame();
                                    return -3;
                                }
                                Task.Delay(1000).Wait();
                            }
                            break;
                        }
                    case 2:
                        {

                            logs[threadnumber]="проверяю руду в таргете";
                            tempcenter = FindCountour( ref targetedmeteor, targetedmeteortoFC);
                            while (tempcenter[0].X != 0)
                            {
                                //return 5;
                                logs[threadnumber]="проверяю руду в таргете";
                                tempcenter = FindCountour( ref targetedmeteor, targetedmeteortoFC);
                                if (checkmass(HWIDch) == 1)
                                {
                                    return 5;
                                }
                                if (_state == State.stop)
                                {
                                    return -1;
                                }
                                checkloc =checklocal();
                                if (checkloc == 1)
                                {
                                   leavegame();
                                    return -3;
                                }
                                Task.Delay(1000).Wait();
                            }
                            break;
                        }
                    
                }
               

                return 3;
            }
            Point[] positionverh = new Point[10];
            Point[] localniz = new Point[10];
            int localpos()
            {
                
                positionverh = FindCountour( ref images.localpeople, "localpeople");
                localniz = FindCountour( ref images.localniz, "localniz");
                return 0;
            }
            int checklocal()
            {
                Point[] center = new Point[10];
                center = FindCountour( ref images.enemylocal, "enemylocal");
                if (center[0].X == -1)
                {
                    return -1;
                }
                if (center[0].X == -2)
                {
                    return -2;
                }
                if (center[0].X != 0)
                {
                    return 1;
                }
                
                return 0;
            }
            void leavegame()
            {
                Point[] center = new Point[10];
                Point[] tempcenter = new Point[10];
                Point[] tempcenter2 = new Point[10];
                leaved[threadnumber] = 1;
                tempcenter2 = FindCountour(ref images.searchsend, "searchsend");
                if (tempcenter2[0].X != 0)
                {
                    while(tempcenter2[0].X != 0)
                    {
                        click("LCL", HWIDch, tempcenter2, 0);
                        Task.Delay(1000).Wait();
                        tempcenter2 = FindCountour(ref images.searchsend, "searchsend");
                    }
                }
                tempcenter2 = FindCountour(ref images.gun1, "gun1");
                if (tempcenter2[0].X != 0)
                {
                    while (tempcenter2[0].X != 0)
                    {
                        click("LCL", HWIDch, tempcenter2, 0);
                        Task.Delay(1000).Wait();
                        tempcenter2 = FindCountour(ref images.gun1, "gun1");
                    }
                }
                tempcenter2 = FindCountour(ref images.moveto, "moveto");
                if (tempcenter2[0].X != 0)
                {
                    while (tempcenter2[0].X != 0)
                    {
                        click("LCL", HWIDch, tempcenter2, 0);
                        Task.Delay(1000).Wait();
                        tempcenter2 = FindCountour(ref images.moveto, "moveto");
                    }
                }
                string loglabel = "log" + threadnumber.ToString();
                tempcenter[0].X = 48;
                tempcenter[0].Y = 14;
                localopened[threadnumber] = 0;
                click("LCL", HWIDch, tempcenter, 0);//клик по лицу
                int i = 0;
                Task.Delay(1000).Wait();
                center = FindCountour( ref images.gamesettings, "gamesettings");
                Task.Delay(500).Wait();
                logs[threadnumber]="выхожу из игры";
                
                if (center[0].X == 0)
                {
                    
                    while(center[0].X == 0)
                    {
                        center = FindCountour( ref images.gamesettings, "gamesettings");
                        i++;
                        if(i%5 == 0)
                        {
                            click("LCL", HWIDch, tempcenter, 0);
                            Task.Delay(100).Wait();
                        }
                        
                        Task.Delay(500).Wait();
                    }
                    click("LCL", HWIDch, center, 0);
                }
                else
                {
                    click("LCL", HWIDch, center, 0);
                }
                Task.Delay(500);
                center = FindCountour( ref images.changepers, "changepers");
                if (center[0].X == 0)
                {
                    while (center[0].X == 0)
                    {
                        center = FindCountour( ref images.changepers, "changepers");
                        Task.Delay(500).Wait();

                        
                    }
                    click("LCL", HWIDch, center, 0);
                }
                else
                {
                    click("LCL", HWIDch, center, 0);
                }
                Task.Delay(500);
                center = FindCountour( ref images.flyaccept, "flyaccept");
                if (center[0].X == 0)
                {
                    while (center[0].X == 0)
                    {
                        center = FindCountour( ref images.flyaccept, "flyaccept");
                        Task.Delay(500);

                       
                    }
                    click("LCL", HWIDch, center, 0);
                }
                else
                {
                    click("LCL", HWIDch, center, 0);
                }
                
                Task.Delay(15000).Wait();
                for (int j = 0; j < 10; j++)
                {
                    try
                    {
                        All3Proc[j] = Process.GetProcessesByName("Nox")[j];

                    }
                    catch { All3Proc[j] = null; }
                }
                for (int k = 0; k < 10; k++)
                {
                    if (All3Proc[k].MainWindowHandle == HWIDm)
                    {
                        All3Proc[k].CloseMainWindow();
                        break;
                    }
                }
                return;

            }
            
         
            int checkrange(Point[] point)
            {
                string loglabel = "log" + threadnumber.ToString();
                int dsres = 0;
                string text="";
                Rectangle rect = new Rectangle(point[0].X + 10, point[0].Y - 20, 50, 50);
                Rectangle rect2 = new Rectangle(425, 720, 170, 20);
                Image<Bgr, byte> inputImage = null;
                
                Image<Gray, byte> imag = new Image<Gray, byte>(50, 50);
                Image<Gray, byte> imag2 = new Image<Gray, byte>(170, 20);
                int checkloc = 0;
                Task.Delay(4000).Wait();
                checkloc =checklocal();
                if (checkloc == 1)
                {
                   leavegame();
                    return -3;
                }
                int sch = 0;
                string temptext = "";
                while (true)
                {
                    checkloc =checklocal();
                    if (checkloc == 1)
                    {
                       leavegame();
                        return -3;
                    }
                    dsres = doscreen(HWIDch, ref inputImage);
                    if (dsres == 4)
                    {
                        imag.Dispose();
                        inputImage.Dispose();
                        return -1;
                    }
                    if (dsres == -2)
                    {
                        imag.Dispose();
                        inputImage.Dispose();
                        return -2;
                    }
                    Image<Gray, byte> img = inputImage.Convert<Gray, byte>();
                    img = (new Image<Gray, byte>(img.Width, img.Height, new Gray(255))).Sub(img);
                    imag = img.Copy(rect);
                    imag = Resizepic(imag);
                    //imag.Save("tef.bmp");
                    imag = Thresholdimage(imag.ToBitmap()).ToImage<Gray, byte>();
                    Tesseract2.SetImage(imag);
                    Tesseract2.Recognize();
                    text = Tesseract2.GetUTF8Text();
                    img.Dispose();
                    imag.Dispose();
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                    /*if((text.Substring(1) == "\r\nкн\r\n" || text.Substring(1) == "\r\nкм\r\n" || text.Substring(1) == "\r\nКН\r\n" ||
                        text.Substring(1) == "\r\nКМ\r\n" || text.Substring(1) == "\r\nни\r\n" || text.Substring(1) == "\r\nНИ\r\n" )&&
                         (text[0]-'0')<10
                        )
                    {
                        return 0;
                    }*/
                    Image<Gray, byte> img2 = inputImage.Convert<Gray, byte>();
                    img2 = (new Image<Gray, byte>(img2.Width, img2.Height, new Gray(255))).Sub(img2);
                    imag2 = img2.Copy(rect2);
                    imag2 = Resizepic(imag2);
                    //imag.Save("tef.bmp");
                    imag2 = Thresholdimage(imag2.ToBitmap()).ToImage<Gray, byte>();
                    Tesseract2.SetImage(imag2);
                    Tesseract2.Recognize();
                    string text2 = Tesseract2.GetUTF8Text();
                    img2.Dispose();
                    imag2.Dispose();
                    inputImage.Dispose();

                    if (text2.Substring(0,2) == "Ом" || text2.Substring(0,2) == "Пм" || text2.Substring(0, 2) == "1м" || text2.Substring(0, 2) == "2м" || text2.Substring(0, 1) == "н")
                    {
                        for(int i = 0; i < text.Length; i++)
                        {
                            if (text[i] == 'К' || text[i] == 'к' || text[i] == '2' || text[i] == '3' || text[i] == 'Н' || text[i]== 'н' || text[i] == 'З')
                            {
                                return 0;
                            }
                        }
                        sch++;
                        if (sch > 60)
                        {
                            return 1;
                        }
                    }

                    logs[threadnumber] = text.Trim('\n') +'\t'+ text2.Trim('\n');

                    Task.Delay(1000).Wait();
                }
                
            }
            string[] finddistace(int rezhim,int kolvo, ref Image<Bgr,byte> meteorite, int rezh,ref string meteoritetoFC)//rezhim =1 - кернит, 2 - крокит, 3 джаспет, 4 
            {
               
                Image<Bgr, byte> inputImage = null;
                Point[] center = new Point[10];
               
                int dsres = 0;
                string[] text = new string[10];
                string[] errstring = new string[2];
                errstring[0] = "-1";
                dsres =doscreen(HWIDch,ref inputImage);

                if (dsres == 4)
                {
                    
                    inputImage.Dispose();
                    return errstring;
                }
                if (dsres == -2)
                {
                    inputImage.Dispose();
                    return errstring;
                }
                
                Image<Gray, byte> imag = new Image<Gray, byte>(27, 15);
                if (rezh == 0)
                {
                    center = FindCountour( ref images.kernite, "kernite");

                    Image<Gray, byte> img = inputImage.Convert<Gray, byte>();
                    img = (new Image<Gray, byte>(img.Width, img.Height, new Gray(255))).Sub(img);
                    for (int i = 0; i < kolvo; i++)
                    {
                        Rectangle rect = new Rectangle(center[i].X - images.kernite.Width / 2 - 29, center[i].Y - images.kernite.Height / 2 - 13, 27, 15);

                        imag = img.Copy(rect);
                        imag = Resizepic(imag);
                        imag = Thresholdimage(imag.ToBitmap()).ToImage<Gray, byte>();
                        Tesseract1.SetImage(imag);
                        Tesseract1.Recognize();
                       
                        text[i] = Tesseract1.GetUTF8Text().Trim('\n', '\r', ' ');
                       
                    }
                    img.Dispose();
                }
                if (rezh == 1)
                {
                    center = FindCountour( ref meteorite, meteoritetoFC);
                    int k = 20;
                    Image<Gray, byte> img = inputImage.Convert<Gray, byte>();
                    img = (new Image<Gray, byte>(img.Width, img.Height, new Gray(255))).Sub(img);
                    for (int i = 0; i < kolvo; i++)
                    {
                        Rectangle rect = new Rectangle(center[i].X - meteorite.Width / 2 - 93, center[i].Y - meteorite.Height / 2, 27, 15);

                        imag = img.Copy(rect);
                        imag = Resizepic(imag);
                        imag = Thresholdimage(imag.ToBitmap()).ToImage<Gray, byte>();
                        Tesseract1.SetImage(imag);
                        Tesseract1.Recognize();
                        k++;
                        text[i] = Tesseract1.GetUTF8Text().Trim('\n', '\r', ' ');
                    }
                    img.Dispose();
                }
                if (rezh == 2)
                {
                    center = FindCountour( ref meteorite, meteoritetoFC);
                    int k = 20;
                    Image<Gray, byte> img = inputImage.Convert<Gray, byte>();
                    img = (new Image<Gray, byte>(img.Width, img.Height, new Gray(255))).Sub(img);
                    for (int i = 0; i < kolvo; i++)
                    {
                        Rectangle rect = new Rectangle(center[i].X - meteorite.Width / 2 - 29, center[i].Y - meteorite.Height / 2 - 13, 27, 15);

                        imag = img.Copy(rect);
                        imag = Resizepic(imag);
                        imag = Thresholdimage(imag.ToBitmap()).ToImage<Gray, byte>();
                        Tesseract1.SetImage(imag);
                        Tesseract1.Recognize();
                        k++;
                        text[i] = Tesseract1.GetUTF8Text().Trim('\n', '\r', ' ');
                    }
                    img.Dispose();
                }
                if (rezh == 3)
                {
                    center = FindCountour( ref meteorite, meteoritetoFC);
                    int k = 20;
                    Image<Gray, byte> img = inputImage.Convert<Gray, byte>();
                    img = (new Image<Gray, byte>(img.Width, img.Height, new Gray(255))).Sub(img);
                    for (int i = 0; i < kolvo; i++)
                    {
                        Rectangle rect = new Rectangle(center[i].X - meteorite.Width / 2 - 29, center[i].Y - meteorite.Height / 2 - 13, 27, 15);

                        imag = img.Copy(rect);
                        imag = Resizepic(imag);
                        imag = Thresholdimage(imag.ToBitmap()).ToImage<Gray, byte>();
                        Tesseract1.SetImage(imag);
                        Tesseract1.Recognize();
                        k++;
                        text[i] = Tesseract1.GetUTF8Text().Trim('\n', '\r', ' ');
                    }
                    img.Dispose();
                }

                inputImage.Dispose();
                imag.Dispose();
               
                return text;
            }
            public Image<Gray, byte> Resizepic(Image<Gray, byte> image)
            {
                int Scale = 4;
                if (Scale <= 0 || Scale == 1)
                    return image;

                image = image.Resize((int)(image.Width * Scale), (int)(image.Height * Scale), Inter.Cubic);
                return image;
            }
            public Bitmap Thresholdimage(Bitmap image)
            {
                var img = image.ToImage<Gray, byte>().Copy();
                var dst = new Mat();
                var threshold = CvInvoke.Threshold(img, dst, 0, 255, ThresholdType.Otsu);
                CvInvoke.Threshold(img, dst, threshold, 225, ThresholdType.Binary);
                
                return dst.ToBitmap();
            }
            int toleave = 0;
            int findedleave = 0;
            Point[] FindCountour(ref Image<Bgr, byte> imageToFind, string position)
            {
                Point[] errpoint = new Point[1];
                errpoint[0].X = -1;
                errpoint[0].Y = -1;
                Image<Bgr, byte> image = null;
                int dsres = 0;
                if (leaved[threadnumber] == 1)
                {
                    findedleave = 2;
                    toleave = 0;
                }
                if (leaveall == 1)
                {
                    findedleave = 1;
                    toleave = 1;
                }
                else
                if (findedleave == 0)
                {
                    if (threadnumber < 5)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            if (toleave == 0)
                            {
                                if (leaved[j] == 1 && threadnumber != j)
                                {
                                    toleave = 1;
                                    findedleave = 1;
                                }
                            }

                        }
                    }
                    else
                    {
                        for (int j = 5; j < 10; j++)
                        {
                            if (leaved[j] == 1 && threadnumber != j)
                            {
                                toleave = 1;
                                findedleave = 1;
                            }
                        }
                    }

                }
                if (toleave == 1)
                {
                    toleave = 0;
                    if (threadnumber < 5)
                    {
                        leavedfirstpart++;
                        if (leavedfirstpart == 5)
                        {
                            leavedfirstpart = 0;
                            for (int i = 0; i < 5; i++)
                            {
                                leaved[i] = 0;

                            }
                        }
                    }
                    if (threadnumber > 4)
                    {
                        leavedsecondpart++;
                        if (leavedsecondpart == 5)
                        {
                            leavedsecondpart = 0;
                            for (int i = 5; i < 10; i++)
                            {
                                leaved[i] = 0;

                            }
                        }
                    }
                    leavegame();
                    source2[threadnumber].Cancel();
                    return errpoint;
                }

                while (image == null)
                {
                    dsres = doscreen(HWIDch, ref image);

                    if (dsres == 4)
                    {
                        return errpoint;
                    }
                    if (dsres == -2)
                    {
                        errpoint[0].X = -2;
                        return errpoint;
                    }
                }

                if (_state == State.stop)
                {
                    return errpoint;
                }
                Point[] Center2 = new Point[10];
                Center2[0].X = 0;
                if (leaveall == 0 || toleave == 0)
                {
                    Image<Bgr, byte> sector2 = null;
                    Image<Bgr, byte> sector3 = null;
                    Rectangle rect = new Rectangle(500, 0, 524, 200);
                    sector2 = image.Copy(rect);

                    Image<Gray, float> result1 = sector2.MatchTemplate(images.wasattecked, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);
                    float[,,] matches1 = result1.Data;
                    for (int j = 0; j < matches1.GetLength(0); j++)
                    {
                        for (int i = 0; i < matches1.GetLength(1); i++)
                        {
                            double matchScore = matches1[j, i, 0];
                            if (matchScore > 0.8)
                            {
                                lock (locker)
                                {
                                    leaveall = 1;
                                }
                                leavegame();
                                source2[threadnumber].Cancel();
                               
                               
                                leaved[threadnumber] = 1;
                                return errpoint;

                            }
                        }

                    }
                    sector2.Dispose();


                    Rectangle rect2 = new Rectangle(0, 0, 524, 500);
                    sector3 = image.Copy(rect2);
                    Image<Gray, float> result2 = sector3.MatchTemplate(images.wasdestroyed, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);
                    float[,,] matches2 = result2.Data;
                    for (int j = 0; j < matches2.GetLength(0); j++)
                    {
                        for (int i = 0; i < matches2.GetLength(1); i++)
                        {
                            double matchScore = matches2[j, i, 0];
                            if (matchScore > 0.8)
                            {
                                lock (locker)
                                {
                                    leaveall = 1;
                                }
                                leavegame();
                                source2[threadnumber].Cancel();
                               
                                leaved[threadnumber] = 1;
                                return errpoint;
                            }
                        }

                    }
                    sector3.Dispose();


                }



                Image<Bgr, byte> sector = null;

                Rectangle sectorpos;
                switch (position)
                {
                    case "rudeotsek":
                        {
                            sectorpos = new Rectangle(0, 0, 300, 768);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "planetnotime":
                        {
                            sectorpos = new Rectangle(0, 0, 200, 768);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "planetlowtime":
                        {
                            sectorpos = new Rectangle(0, 0, 200, 768);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "palnetfulltime":
                        {
                            sectorpos = new Rectangle(0, 0, 200, 768);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "planethead":
                        {
                            sectorpos = new Rectangle(0, 0, 600, 200);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "targetedrudeotsek":
                        {
                            sectorpos = new Rectangle(0, 0, 300, 768);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "inflight":
                        {
                            sectorpos = new Rectangle(0, 0, 400, 400);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "dokfly":
                        {
                            sectorpos = new Rectangle(0, 90, 200, 90);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "searchtext":
                        {
                            sectorpos = new Rectangle(0, 100, 500, 600);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "searchblock":
                        {
                            sectorpos = new Rectangle(0, 100, 500, 600);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "shopcenter":
                        {
                            sectorpos = new Rectangle(0, 100, 500, 600);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "station":
                        {
                            sectorpos = new Rectangle(0, 100, 500, 600);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "enemylocal":
                        {
                            sectorpos = new Rectangle(positionverh[0].X - images.localpeople.Width / 2, positionverh[0].Y, images.localniz.Width, localniz[0].Y - positionverh[0].Y);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "localpeople":
                        {
                            sectorpos = new Rectangle(0, 100, 600, 668);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "localminimize":
                        {
                            sectorpos = new Rectangle(0, 100, 600, 668);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "localniz":
                        {
                            sectorpos = new Rectangle(0, 100, 600, 668);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "map":
                        {
                            sectorpos = new Rectangle(0, 300, 200, 200);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "search":
                        {
                            sectorpos = new Rectangle(0, 600, 100, 168);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "sklad":
                        {
                            sectorpos = new Rectangle(100, 200, 400, 200);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "targetedkernitefarm":
                        {
                            sectorpos = new Rectangle(300, 0, 500, 100);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "targetedbistodefarm":
                        {
                            sectorpos = new Rectangle(300, 0, 500, 100);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "targetedarcanorfarm":
                        {
                            sectorpos = new Rectangle(300, 0, 500, 100);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "changepers":
                        {
                            sectorpos = new Rectangle(320, 650, 100, 118);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "speed0":
                        {
                            sectorpos = new Rectangle(330, 580, 100, 118);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "warp":
                        {
                            sectorpos = new Rectangle(450, 0, 350, 768);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "moveto":
                        {
                            sectorpos = new Rectangle(450, 0, 350, 768);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "gettarget":
                        {
                            sectorpos = new Rectangle(450, 0, 350, 768);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "canceltarget":
                        {
                            sectorpos = new Rectangle(450, 0, 350, 768);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "gun1":
                        {
                            sectorpos = new Rectangle(480, 0, 100, 768);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "planetmenu":
                        {
                            sectorpos = new Rectangle(500, 300, 300, 300);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "gamesettings":
                        {
                            sectorpos = new Rectangle(500, 300, 300, 300);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "planetrenew":
                        {
                            sectorpos = new Rectangle(600, 0, 424, 300);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "flyaccept":
                        {
                            sectorpos = new Rectangle(600, 400, 424, 368);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "pickall":
                        {
                            sectorpos = new Rectangle(600, 600, 300, 168);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "searchsend":
                        {
                            sectorpos = new Rectangle(700, 0, 300, 100);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "rightmenu":
                        {
                            sectorpos = new Rectangle(700, 450, 324, 100);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "poyas":
                        {
                            sectorpos = new Rectangle(770, 0, 30, 768);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "aster":
                        {
                            sectorpos = new Rectangle(770, 0, 30, 768);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "leavedoc":
                        {
                            sectorpos = new Rectangle(800, 0, 224, 368);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "kernite":
                        {
                            sectorpos = new Rectangle(800, 0, 200, 538);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "crokite":
                        {
                            sectorpos = new Rectangle(800, 0, 200, 538);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "bistode":
                        {
                            sectorpos = new Rectangle(800, 0, 200, 538);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "arcanor":
                        {
                            sectorpos = new Rectangle(800, 0, 200, 538);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "hedbergite":
                        {
                            sectorpos = new Rectangle(800, 0, 200, 538);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "spodumain":
                        {
                            sectorpos = new Rectangle(800, 0, 200, 538);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "jaspet":
                        {
                            sectorpos = new Rectangle(800, 0, 200, 538);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "pyroxeres":
                        {
                            sectorpos = new Rectangle(800, 0, 200, 538);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "fly":
                        {
                            sectorpos = new Rectangle(800, 400, 224, 368);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "cancelcurse":
                        {
                            sectorpos = new Rectangle(800, 400, 224, 368);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "planetclose":
                        {
                            sectorpos = new Rectangle(900, 0, 124, 100);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "filtrpoyas":
                        {
                            sectorpos = new Rectangle(950, 20, 74, 130);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    case "filtraster":
                        {
                            sectorpos = new Rectangle(950, 20, 74, 130);
                            sector = image.Copy(sectorpos);
                            break;
                        }
                    default:
                        {
                            sectorpos = new Rectangle(0, 0, 1024, 768);
                            sector = image.Copy();
                            break;
                        }
                }
                image.Dispose();
                //sector.Save("13asdsdaas.bmp");
                int x = 0, y = 0, kol = 0;
                Point[] Center = new Point[10];
                bool dublicate = true;
                bool isdublicate = false;
                Image<Gray, float> result = sector.MatchTemplate(imageToFind, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);



                float[,,] matches = result.Data;
                for (y = 0; y < matches.GetLength(0); y++)
                {
                    for (x = 0; x < matches.GetLength(1); x++)
                    {
                        double matchScore = matches[y, x, 0];
                        if (matchScore > 0.8)
                        {
                            if (dublicate == true)
                            {
                                if (kol > 0)
                                {
                                    Center[kol].X = x + imageToFind.Width / 2 + sectorpos.X;
                                    Center[kol].Y = y + imageToFind.Height / 2 + sectorpos.Y;
                                    for (int i = 0; i < kol; i++)
                                    {
                                        if (Center[i].X > Center[kol].X - 10 &&
                                            Center[i].X < Center[kol].X + 10 &&
                                            Center[i].Y > Center[kol].Y - 10 &&
                                            Center[i].Y < Center[kol].Y + 10)
                                        {
                                            isdublicate = true;
                                            break;
                                        }

                                    }
                                    if (isdublicate == false)
                                    {
                                        Center[kol].X = x + imageToFind.Width / 2 + sectorpos.X;
                                        Center[kol].Y = y + imageToFind.Height / 2 + sectorpos.Y;
                                        kol++;
                                    }
                                    else
                                    {
                                        Center[kol].X = 0;
                                        Center[kol].Y = 0;
                                    }
                                }
                                else
                                {
                                    Center[kol].X = x + imageToFind.Width / 2 + sectorpos.X;
                                    Center[kol].Y = y + imageToFind.Height / 2 + sectorpos.Y;
                                    kol++;
                                }
                            }
                            isdublicate = false;
                            if (kol == 10)
                            {
                                sector.Dispose();
                                result.Dispose();
                                image.Dispose();
                                return Center;
                            }

                        }
                    }
                }
                sector.Dispose();
                image.Dispose();
                return Center;
            }


            /* Point[] FindCountour(IntPtr HWIDch, ref Image<Bgr, byte> imageToFind)
             {
                 Point[] errpoint = new Point[1];
                 errpoint[0].X = -1;
                 errpoint[0].Y = -1;
                 Image<Bgr, byte> image = null;
                 int dsres = 0;
                 if (token.IsCancellationRequested)
                 {
                     token.ThrowIfCancellationRequested();
                 }
                 while (image == null)
                 {
                     dsres = doscreen(HWIDch, ref image);

                     if (dsres == 4)
                     {
                         return errpoint;
                     }
                     if (dsres == -2)
                     {
                         errpoint[0].X = -2;
                         return errpoint;
                     }
                 }
                 if(_state == State.stop)
                 {
                     return errpoint;
                 }
                 Image<Gray, float> result = image.MatchTemplate(imageToFind, Emgu.CV.CvEnum.TemplateMatchingType.CcoeffNormed);



                 float[,,] matches = result.Data;
                 int x = 0, y = 0, kol = 0;
                 Point[] Center = new Point[10];
                 bool dublicate = true;
                 bool isdublicate = false;
                 for (y = 0; y < matches.GetLength(0); y++)
                 {
                     for (x = 0; x < matches.GetLength(1); x++)
                     {
                         double matchScore = matches[y, x, 0];
                         if (matchScore > 0.8)
                         {
                             if(dublicate == true)
                             {
                                 if (kol > 0)
                                 {
                                     Rectangle rect = new Rectangle(x, y, imageToFind.Width, imageToFind.Height);
                                     Center[kol] = new Point(x + imageToFind.Width / 2, y + imageToFind.Height / 2);

                                     for (int i = 0; i < kol; i++)
                                     {
                                         if (Center[i].X > Center[kol].X -10 &&
                                             Center[i].X < Center[kol].X + 10 &&
                                             Center[i].Y > Center[kol].Y - 10 &&
                                             Center[i].Y < Center[kol].Y + 10)
                                         {
                                             isdublicate = true;
                                             break;
                                         }

                                     }
                                     if (isdublicate == false)
                                     {
                                         rect = new Rectangle(x, y, imageToFind.Width, imageToFind.Height);
                                         Center[kol] = new Point(x + imageToFind.Width / 2, y + imageToFind.Height / 2);

                                         kol++;
                                     }
                                     else
                                     {
                                         Center[kol].X = 0;
                                         Center[kol].Y = 0;
                                     }
                                 }
                                 else
                                 {
                                     Rectangle rect = new Rectangle(x, y, imageToFind.Width, imageToFind.Height);
                                     Center[kol] = new Point(x + imageToFind.Width / 2, y + imageToFind.Height / 2);

                                     kol++;
                                 }
                             }

                             isdublicate = false;
                             if (kol == 10)
                             {
                                 result.Dispose();
                                 image.Dispose();
                                 return Center;
                             }
                         }

                     }

                 }

                 result.Dispose();
                 image.Dispose();
                 return Center;
             }*/



            private int doscreen(IntPtr hwidtemp, ref Image<Bgr, byte> retimage)
            {
                Image image = null;
                string statusis = "status" + (threadnumber + 1).ToString();
                ScreenShot.ScreenCapture sc = new ScreenShot.ScreenCapture();
                lock (locker)
                {
                    try
                    {
                        image = sc.CaptureWindow(hwidtemp);
                    }
                    catch (Exception)
                    {
                        form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                        return -2;
                    }

                }
                
                Bitmap bitmap = new Bitmap(image);
                while (_state == State.pause)
                {

                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = paused));
                    Task.Delay(1000).Wait();
                }
                form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = working));
                if (_state == State.stop)
                {
                    form.Controls[statusis].Invoke((MethodInvoker)(() => form.Controls[statusis].BackgroundImage = stopped));
                    return 4;
                }
                retimage = bitmap.ToImage<Bgr, byte>();
                bitmap.Dispose();
                return 0;
            }
        }
        static Image working;
        static Image paused;
        static Image stopped;
        public Form1()
        {
            InitializeComponent();
            paused = new Bitmap(@"res\paused.png");
            working = new Bitmap(@"res\working.png");
            stopped = new Bitmap(@"res\stopped.png");
            string nameaccs = "account";
            string acc;
            var file = File.ReadAllLines("accs");
           
            for (int i = 0; i < 10; i++)
            {
                if (file[i].Length < 5)
                {

                }
                else
                {
                    acc = nameaccs + i.ToString();
                    Controls[acc].Text = file[i].Substring(5);

                }
            }
           
           
          
            script1.Items.AddRange(installs);
            script2.Items.AddRange(installs);
            script3.Items.AddRange(installs);
            script4.Items.AddRange(installs);
            script5.Items.AddRange(installs);
            script6.Items.AddRange(installs);
            script7.Items.AddRange(installs);
            script8.Items.AddRange(installs);
            script9.Items.AddRange(installs);
            script10.Items.AddRange(installs);
           
          for(int j = 0; j < 10; j++)
            {
                source2[j] = new CancellationTokenSource();
                source3[j] = new CancellationTokenSource();
            }
           for(int j = 0; j < 10; j++)
            {
                token2[j] = source2[j].Token;
                       token3[j] = source3[j].Token;
            }


            _synchronizationContext = SynchronizationContext.Current;

        }

      


        private void Form1_Closed(object sender, System.EventArgs e)
        {
            for (int i = 0; i < 10; i++)
            {
                boti[i] = new bot(null,null);
                boti[i]._state = State.stop;
                source2[i].Cancel();
            }

            Application.Exit();
        }













        private static void click(string k, IntPtr tempHWID, Point[] tempK, int temppos)
        {

            switch (k)
            {
                case "shift":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_SHIFT, (IntPtr)0);
                        break;
                    }
                case "shiftup":
                    {
                        SendMessage(tempHWID, WM_KEYUP, VK_SHIFT, (IntPtr)0);
                        break;
                    }
                case " ":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_SPACE, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_SPACE, (IntPtr)0);
                        break;
                    }
                case "-":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_OEM_MINUS, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_OEM_MINUS, (IntPtr)0);
                        break;
                    }
                case "LCL":
                    {
                        SendMessage(tempHWID, WM_LBUTTONDOWN, 1, MakeLParam(tempK[temppos].X, tempK[temppos].Y));
                        SendMessage(tempHWID, WM_LBUTTONUP, 0, MakeLParam(tempK[temppos].X, tempK[temppos].Y));
                        break;
                    }
                case "PGUP":
                    {
                        PostMessage(tempHWID, WM_KEYDOWN, VK_PGUP, (IntPtr)0x1490001);
                        PostMessage(tempHWID, WM_KEYUP, VK_PGUP, (IntPtr)0xC1490001);
                        break;
                    }
                case "LeftDown":
                    {
                        PostMessage(tempHWID, WM_LBUTTONDOWN, 1, MakeLParam(tempK[temppos].X, tempK[temppos].Y));
                        break;
                    }
                case "move":
                    {
                        SendMessage(tempHWID, WM_MOUSEMOVE, 1, MakeLParam(tempK[temppos].X, tempK[temppos].Y));
                        break;
                    }
                case "LeftUp":
                    {
                        PostMessage(tempHWID, WM_LBUTTONUP, 0, MakeLParam(tempK[temppos].X, tempK[temppos].Y));
                        break;
                    }
                case "A":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_A, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_A, (IntPtr)0);
                        break;
                    }
                case "B":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_B, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_B, (IntPtr)0);
                        break;
                    }
                case "C":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_C, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_C, (IntPtr)0);
                        break;
                    }
                case "D":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_D, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_D, (IntPtr)0);
                        break;
                    }
                case "E":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_E, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_E, (IntPtr)0);
                        break;
                    }
                case "F":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_F, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_F, (IntPtr)0);
                        break;
                    }
                case "G":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_G, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_G, (IntPtr)0);
                        break;
                    }
                case "H":
                    {

                        SendMessage(tempHWID, WM_KEYDOWN, VK_H, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_H, (IntPtr)0);
                        break;
                    }
                case "I":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_I, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_I, (IntPtr)0);
                        break;
                    }
                case "J":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_J, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_J, (IntPtr)0);
                        break;
                    }
                case "K":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_K, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_K, (IntPtr)0);
                        break;
                    }
                case "L":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_L, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_L, (IntPtr)0);
                        break;
                    }
                case "M":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_M, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_M, (IntPtr)0);
                        break;
                    }
                case "N":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_N, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_N, (IntPtr)0);
                        break;
                    }
                case "O":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_O, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_O, (IntPtr)0);
                        break;
                    }
                case "P":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_P, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_P, (IntPtr)0);
                        break;
                    }
                case "Q":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_Q, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_Q, (IntPtr)0);
                        break;
                    }
                case "R":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_R, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_R, (IntPtr)0);
                        break;
                    }
                case "S":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_S, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_S, (IntPtr)0);
                        break;
                    }
                case "T":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_T, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_T, (IntPtr)0);
                        break;
                    }
                case "U":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_U, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_U, (IntPtr)0);
                        break;
                    }
                case "V":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_V, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_V, (IntPtr)0);
                        break;
                    }
                case "W":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_W, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_W, (IntPtr)0);
                        break;
                    }
                case "X":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_X, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_X, (IntPtr)0);
                        break;
                    }
                case "Y":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_Y, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_Y, (IntPtr)0);
                        break;
                    }
                case "Z":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_Z, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_Z, (IntPtr)0);
                        break;
                    }
                case "0":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_0, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_0, (IntPtr)0);
                        break;
                    }
                case "1":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_1, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_1, (IntPtr)0);
                        break;
                    }
                case "2":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_2, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_2, (IntPtr)0);
                        break;
                    }
                case "3":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_3, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_3, (IntPtr)0);
                        break;
                    }
                case "4":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_4, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_4, (IntPtr)0);
                        break;
                    }
                case "5":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_5, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_5, (IntPtr)0);
                        break;
                    }
                case "6":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_6, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_6, (IntPtr)0);
                        break;
                    }
                case "7":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_7, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_7, (IntPtr)0);
                        break;
                    }
                case "8":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_8, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_8, (IntPtr)0);
                        break;
                    }
                case "9":
                    {
                        SendMessage(tempHWID, WM_KEYDOWN, VK_9, (IntPtr)0);
                        SendMessage(tempHWID, WM_KEYUP, VK_9, (IntPtr)0);
                        break;
                    }
            }

        }





        DateTime bt0, bt1, bt2, bt3, bt4, bt5, bt6, bt7, bt8, bt9;//in
        DateTime t0, t1, t2, t3, t4, t5, t6, t7, t8, t9;//out
        DateTime s0, s1, s2, s3, s4, s5, s6, s7, s8, s9;
       private void bot0t_Tick(object sender, EventArgs e)
        {
            bt0 = DateTime.Now;
            TimeSpan ts = bt0 - t0;
            //timer0.Text = "bread";
            //if (boti[0]._state != State.stop || boti[0]._state != State.pause)
            log0.Text = logs[0];
            
            if (logs[0] == "enemy detected" || logs[0]=="closed" || logs[0] == "Nox was closed")
            {
                bot0t.Stop();
                bot0t.Enabled = false;
            }
          
           
            this.timer0.Text = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();

        }
        private void bot1t_Tick(object sender, EventArgs e)
        {
            if (logs[1] == "enemy detected" || logs[1] == "closed" || logs[1] == "Nox was closed")
            {
                bot1t.Stop();
                bot1t.Enabled = false;
            }
            bt1 = DateTime.Now;
            TimeSpan ts = bt1 - t1;
            //timer0.Text = "bread";
            //if (boti[1]._state != State.stop || boti[1]._state != State.pause)
            log1.Text = logs[1];
            this.timer1.Text = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
        }
        private void bot2t_Tick(object sender, EventArgs e)
        {
            if (logs[2] == "enemy detected" || logs[2] == "closed" || logs[2] == "Nox was closed")
            {
                bot2t.Stop();
                bot2t.Enabled = false;
            }
            bt2 = DateTime.Now;
            TimeSpan ts = bt2 - t2;
            //timer0.Text = "bread";
            // if (boti[2]._state != State.stop || boti[2]._state != State.pause)
            log2.Text = logs[2];
            this.timer2.Text = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
        }
        private void bot3t_Tick(object sender, EventArgs e)
        {
            if (logs[3] == "enemy detected" || logs[3] == "closed" || logs[3] == "Nox was closed")
            {
                bot3t.Stop();
                bot3t.Enabled = false;
            }
            bt3 = DateTime.Now;
            TimeSpan ts = bt3 - t3;
            //timer0.Text = "bread";
            // if (boti[3]._state != State.stop || boti[3]._state != State.pause)
            log3.Text = logs[3];
            this.timer3.Text = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
        }
        private void bot4t_Tick(object sender, EventArgs e)
        {
            if (logs[4] == "enemy detected" || logs[4] == "closed" || logs[4] == "Nox was closed")
            {
                bot4t.Stop();
                bot4t.Enabled = false;
            }
            bt4 = DateTime.Now;
            TimeSpan ts = bt4 - t4;
            //timer0.Text = "bread";
            // if (boti[4]._state != State.stop || boti[4]._state != State.pause)
            log4.Text = logs[4];
            this.timer4.Text = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();

        }
        private void bot5t_Tick(object sender, EventArgs e)
        {
            if (logs[5] == "enemy detected" || logs[5] == "closed" || logs[5] == "Nox was closed")
            {
                bot5t.Stop();
                bot5t.Enabled = false;
            }
            bt5 = DateTime.Now;
            TimeSpan ts = bt5 - t5;
            //timer0.Text = "bread";
            //      if (boti[5]._state != State.stop || boti[5]._state != State.pause)
            log5.Text = logs[5];
            this.timer5.Text = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
        }
        private void bot6t_Tick(object sender, EventArgs e)
        {
            if (logs[6] == "enemy detected" || logs[6] == "closed" || logs[6] == "Nox was closed")
            {
                bot6t.Stop();
                bot6t.Enabled = false;
            }
            bt6 = DateTime.Now;
            TimeSpan ts = bt6 - t6;
            //timer0.Text = "bread";
            //   if (boti[6]._state != State.stop || boti[6]._state != State.pause)
            log6.Text = logs[6];
            this.timer6.Text = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
        }
        private void bot7t_Tick(object sender, EventArgs e)
        {
            if (logs[7] == "enemy detected" || logs[7] == "closed" || logs[7] == "Nox was closed")
            {
                bot7t.Stop();
                bot7t.Enabled = false;
            }
            bt7 = DateTime.Now;
            TimeSpan ts = bt7 - t7;
            //timer0.Text = "bread";
            //    if (boti[7]._state != State.stop || boti[7]._state != State.pause)
            log7.Text = logs[7];
            this.timer7.Text = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
        }
        private void bot8t_Tick(object sender, EventArgs e)
        {
            if (logs[8] == "enemy detected" || logs[8] == "closed" || logs[8] == "Nox was closed")
            {
                bot8t.Stop();
                bot8t.Enabled = false;
            }
            bt8 = DateTime.Now;
            TimeSpan ts = bt8 - t8;
            //timer0.Text = "bread";
            //    if (boti[8]._state != State.stop || boti[8]._state != State.pause)
            log8.Text = logs[8];
            this.timer8.Text = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
        }
        private void bot9t_Tick(object sender, EventArgs e)
        {
            if (logs[9] == "enemy detected" || logs[9] == "closed" || logs[9] == "Nox was closed")
            {
                bot9t.Stop();
                bot9t.Enabled = false;
            }
            bt9 = DateTime.Now;
            TimeSpan ts = bt9 - t9;
            //timer0.Text = "bread";
            //   if (boti[9]._state != State.stop || boti[9]._state != State.pause)
            log9.Text = logs[9];
            this.timer9.Text = ts.Hours.ToString() + ":" + ts.Minutes.ToString() + ":" + ts.Seconds.ToString();
        }
        private void Start1_Click(object sender, EventArgs e)
        {
            leaveall = 0;
            source2[0].Dispose();
            source2[0] = new CancellationTokenSource();
            leaved[0] = 0;
            leaved[1] = 0;
            leaved[2] = 0;
            leaved[3] = 0;
            leaved[4] = 0;
            

            bot0t.Interval = 1000;
           




            int[] parametrs = { 0, 0, 0 };
           if (buttons[0] == 0)
            {
                start1.Enabled = false;
                pause1.Enabled = true;
                stop1.Enabled = true;
                boti[0] = new bot(parametrs, this);
                boti[0]._state = State.run;

                if (script1.Text == "mine")
                {
                    bot0t.Enabled = true;
                    t0 = DateTime.Now;
                    bot0t.Start();
                }

                buttons[0]++;
                
           }
            if (boti[0]._state == State.stop)
            {
                boti[0] = new bot(parametrs, this);
                boti[0]._state = State.run;

                if (script1.Text == "mine")
                {
                    bot0t.Enabled = true;
                    t0 = DateTime.Now;
                    bot0t.Start();
                }
                start1.Enabled = false;
                pause1.Enabled = true;
                stop1.Enabled = true;
            }
            if (boti[0]._state == State.pause)
            {
                boti[0]._state = State.run;
                start1.Enabled = false;
                pause1.Enabled = true;
                stop1.Enabled = true;
            }

        }
        private void Start2_Click(object sender, EventArgs e)
        {
            leaveall = 0;
            leaved[0] = 0;
            leaved[1] = 0;
            leaved[2] = 0;
            leaved[3] = 0;
            leaved[4] = 0;

            source2[1].Dispose();
            source2[1] = new CancellationTokenSource();
            
          
           
           
               
            int[] parametrs = { 1, 0, 0 };
            if (buttons[1] == 0)
            {
                start2.Enabled = false;
                pause2.Enabled = true;
                stop2.Enabled = true;
                boti[1] = new bot(parametrs, this);
                boti[1]._state = State.run;
                t1 = DateTime.Now;
                if (script2.Text == "mine")
                {
                    bot1t.Enabled = true;
                    bot1t.Interval = 1000;
                    bot1t.Start();
                }
                buttons[1]++;
               
            }
            if (boti[1]._state == State.stop)
            {
                boti[1]._state = State.run;
                if (script2.Text == "mine")
                {
                    bot1t.Enabled = true;
                    bot1t.Interval = 1000;
                    bot1t.Start();
                }
                boti[1] = new bot(parametrs, this);
                start2.Enabled = false;
                pause2.Enabled = true;
                stop2.Enabled = true;
            }
            if (boti[1]._state == State.pause)
            {
                boti[1]._state = State.run;
                start2.Enabled = false;
                pause2.Enabled = true;
                stop2.Enabled = true;
            }


        }
        private void Start3_Click(object sender, EventArgs e)
        {
            leaveall = 0;
            leaved[0] = 0;
            leaved[1] = 0;
            leaved[2] = 0;
            leaved[3] = 0;
            leaved[4] = 0;

            source2[2].Dispose();
            source2[2] = new CancellationTokenSource();
           
           
           
           
                

            int[] parametrs = { 2, 0, 0 };
            if (buttons[2] == 0)
            {
                start3.Enabled = false;
                pause3.Enabled = true;
                stop3.Enabled = true;
                boti[2] = new bot(parametrs, this);
                boti[2]._state = State.run;
                t2 = DateTime.Now;
                if (script3.Text == "mine")
                {
                    bot2t.Enabled = true;
                    bot2t.Interval = 1000;
                    bot2t.Start();
                }
                buttons[2]++;
               
            }
            if (boti[2]._state == State.stop)
            {
                boti[2]._state = State.run;
                if (script3.Text == "mine")
                {
                    bot2t.Enabled = true;
                    bot2t.Interval = 1000;
                    bot2t.Start();
                }
                boti[2] = new bot(parametrs, this);
                start3.Enabled = false;
                pause3.Enabled = true;
                stop3.Enabled = true;
            }
            if (boti[2]._state == State.pause)
            {
                boti[2]._state = State.run;
                start3.Enabled = false;
                pause3.Enabled = true;
                stop3.Enabled = true;
            }


        }
        private void Start4_Click(object sender, EventArgs e)
        {
            leaveall = 0;
            leaved[0] = 0;
            leaved[1] = 0;
            leaved[2] = 0;
            leaved[3] = 0;
            leaved[4] = 0;

            source2[3].Dispose();
            source2[3] = new CancellationTokenSource();
           
           
            
           
                
            int[] parametrs = { 3, 0, 0 };
            if (buttons[3] == 0)
            {
                start4.Enabled = false;
                pause4.Enabled = true;
                stop4.Enabled = true;
                boti[3] = new bot(parametrs, this);
                boti[3]._state = State.run;
               
                if (script4.Text == "mine")
                {
                    t3 = DateTime.Now;
                    bot3t.Enabled = true;
                    bot3t.Interval = 1000;
                    bot3t.Start();
                }
                buttons[3]++;
               
            }
            if (boti[3]._state == State.stop)
            {
                boti[3]._state = State.run;
                if (script4.Text == "mine")
                {
                    t3 = DateTime.Now;
                    bot3t.Enabled = true;
                    bot3t.Interval = 1000;
                    bot3t.Start();
                }
                boti[3] = new bot(parametrs, this);
                start4.Enabled = false;
                pause4.Enabled = true;
                stop4.Enabled = true;
            }
            if (boti[3]._state == State.pause)
            {
                boti[3]._state = State.run;
                start4.Enabled = false;
                pause4.Enabled = true;
                stop4.Enabled = true;
            }


        }
        private void Start5_Click(object sender, EventArgs e)
        {
            leaveall = 0;
            leaved[0] = 0;
            leaved[1] = 0;
            leaved[2] = 0;
            leaved[3] = 0;
            leaved[4] = 0;

            source2[4].Dispose();
            source2[4] = new CancellationTokenSource();


            int[] parametrs = { 4, 0, 0 };
           
          
            if (buttons[4] == 0)
            {
                start5.Enabled = false;
                pause5.Enabled = true;
                stop5.Enabled = true;
                boti[4] = new bot(parametrs, this);
                boti[4]._state = State.run;

                if (script5.Text == "mine")
                {
                    t4 = DateTime.Now;
                    bot4t.Enabled = true;
                    bot4t.Interval = 1000;
                    bot4t.Start();
                }
                buttons[4]++;
               
            }
            if (boti[4]._state == State.stop)
            {
                boti[4]._state = State.run;

                if (script5.Text == "mine")
                {
                    t4 = DateTime.Now;
                    bot4t.Enabled = true;
                    bot4t.Interval = 1000;
                    bot4t.Start();
                }
                boti[4] = new bot(parametrs, this);
                start5.Enabled = false;
                pause5.Enabled = true;
                stop5.Enabled = true;
            }
            if (boti[4]._state == State.pause)
            {
                boti[4]._state = State.run;
                start5.Enabled = false;
                pause5.Enabled = true;
                stop5.Enabled = true;
            }


        }
        private void Start6_Click(object sender, EventArgs e)
        {
            leaveall = 0;
            leaved[5] = 0;
            leaved[6] = 0;
            leaved[7] = 0;
            leaved[8] = 0;
            leaved[9] = 0;
            source2[5].Dispose();
            source2[5] = new CancellationTokenSource();

           
           
            int[] parametrs = { 5, 0, 0 };
            if (buttons[5] == 0)
            {
                start6.Enabled = false;
                pause6.Enabled = true;
                stop6.Enabled = true;
                boti[5] = new bot(parametrs, this);
                boti[5]._state = State.run;
                
                if (script6.Text == "mine")
                {
                    t5 = DateTime.Now;
                    bot5t.Enabled = true;
                    bot5t.Interval = 1000;
                    bot5t.Start();
                }
                buttons[5]++;
               
            }
            if (boti[5]._state == State.stop)
            {
                boti[5]._state = State.run;
                if (script6.Text == "mine")
                {
                    t5 = DateTime.Now;
                    bot5t.Enabled = true;
                    bot5t.Interval = 1000;
                    bot5t.Start();
                }
                boti[5] = new bot(parametrs, this);
                start6.Enabled = false;
                pause6.Enabled = true;
                stop6.Enabled = true;
            }
            if (boti[5]._state == State.pause)
            {
                boti[5]._state = State.run;
                start6.Enabled = false;
                pause6.Enabled = true;
                stop6.Enabled = true;
            }

        }
        private void Start7_Click(object sender, EventArgs e)
        {
            leaveall = 0;
            leaved[5] = 0;
            leaved[6] = 0;
            leaved[7] = 0;
            leaved[8] = 0;
            leaved[9] = 0;
            source2[6].Dispose();
            source2[6] = new CancellationTokenSource();


           
            
            // windowname = Controls[pole].Text;
            int[] parametrs = { 6, 0, 0 };
            if (buttons[6] == 0)
            {
                start7.Enabled = false;
                pause7.Enabled = true;
                stop7.Enabled = true;
                boti[6] = new bot(parametrs, this);
                boti[6]._state = State.run;
               
                if (script7.Text == "mine")
                {
                    t6 = DateTime.Now;
                    bot6t.Enabled = true;
                    bot6t.Interval = 1000;
                    bot6t.Start();
                }
                buttons[6]++;
                
            }
            if (boti[6]._state == State.stop)
            {
                boti[6]._state = State.run;
                if (script7.Text == "mine")
                {
                    t6 = DateTime.Now;
                    bot6t.Enabled = true;
                    bot6t.Interval = 1000;
                    bot6t.Start();
                }
                boti[6] = new bot(parametrs, this);
                start7.Enabled = false;
                pause7.Enabled = true;
                stop7.Enabled = true;
            }
            if (boti[6]._state == State.pause)
            {
                boti[6]._state = State.run;
                start7.Enabled = false;
                pause7.Enabled = true;
                stop7.Enabled = true;
            }



            //this.Controls[loglabel].Text = pole;
        }
        private void Start8_Click(object sender, EventArgs e)
        {
            leaveall = 0;
            leaved[5] = 0;
            leaved[6] = 0;
            leaved[7] = 0;
            leaved[8] = 0;
            leaved[9] = 0;

            source2[7].Dispose();
            source2[7] = new CancellationTokenSource();

           

            // windowname = Controls[pole].Text;
            int[] parametrs = { 7, 0, 0 };
            if (buttons[7] == 0)
            {
                start8.Enabled = false;
                pause8.Enabled = true;
                stop8.Enabled = true;
                boti[7] = new bot(parametrs, this);

                boti[7]._state = State.run;
                if (script8.Text == "mine")
                {
                    t7 = DateTime.Now;
                    bot7t.Enabled = true;
                    bot7t.Interval = 1000;
                    bot7t.Start();
                }
               
                buttons[7]++;
               
            }
            if (boti[7]._state == State.stop)
            {
                boti[7]._state = State.run;
                if (script8.Text == "mine")
                {
                    t7 = DateTime.Now;
                    bot7t.Enabled = true;
                    bot7t.Interval = 1000;
                    bot7t.Start();
                }
                boti[7] = new bot(parametrs, this);
                start8.Enabled = false;
                pause8.Enabled = true;
                stop8.Enabled = true;
            }
            if (boti[7]._state == State.pause)
            {
                boti[7]._state = State.run;
                start8.Enabled = false;
                pause8.Enabled = true;
                stop8.Enabled = true;
            }



            //this.Controls[loglabel].Text = pole;
        }
        private void Start9_Click(object sender, EventArgs e)
        {
            leaveall = 0;
            leaved[5] = 0;
            leaved[6] = 0;
            leaved[7] = 0;
            leaved[8] = 0;
            leaved[9] = 0;
            source2[8].Dispose();
            source2[8] = new CancellationTokenSource();
           
            

            int[] parametrs = { 8, 0, 0 };
            if (buttons[8] == 0)
            {
                start9.Enabled = false;
                pause9.Enabled = true;
                stop9.Enabled = true;
                boti[8] = new bot(parametrs, this);
                boti[8]._state = State.run;
               
                if (script9.Text == "mine")
                {
                    t8 = DateTime.Now;
                    bot8t.Enabled = true;
                    bot8t.Interval = 1000;
                    bot8t.Start();
                }
                buttons[8]++;
               
            }
            if (boti[8]._state == State.stop)
            {
                boti[8]._state = State.run;
                if (script9.Text == "mine")
                {
                    t8 = DateTime.Now;
                    bot8t.Enabled = true;
                    bot8t.Interval = 1000;
                    bot8t.Start();
                }
                boti[8] = new bot(parametrs, this);
                start9.Enabled = false;
                pause9.Enabled = true;
                stop9.Enabled = true;
            }
            if (boti[8]._state == State.pause)
            {
                boti[8]._state = State.run;
                start9.Enabled = false;
                pause9.Enabled = true;
                stop9.Enabled = true;
            }



            //this.Controls[loglabel].Text = pole;
        }
        private void Start10_Click(object sender, EventArgs e)
        {
            leaveall = 0;
            leaved[5] = 0;
            leaved[6] = 0;
            leaved[7] = 0;
            leaved[8] = 0;
            leaved[9] = 0;
            source2[9].Dispose();
            source2[9] = new CancellationTokenSource();
           
            

            // windowname = Controls[pole].Text;
            int[] parametrs = { 9, 0, 0 };
            if (buttons[9] == 0)
            {
                start10.Enabled = false;
                pause10.Enabled = true;
                stop10.Enabled = true;
                boti[9] = new bot(parametrs, this);
                boti[9]._state = State.run;
               
                if (script10.Text == "mine")
                {
                    t9 = DateTime.Now;
                    bot9t.Enabled = true;
                    bot9t.Interval = 1000;
                    bot9t.Start();
                }
                buttons[9]++;
                
            }
            if (boti[9]._state == State.stop)
            {
                boti[9]._state = State.run;
                if (script10.Text == "mine")
                {
                    t9 = DateTime.Now;
                    bot9t.Enabled = true;
                    bot9t.Interval = 1000;
                    bot9t.Start();
                }
                boti[9] = new bot(parametrs, this);
                start10.Enabled = false;
                pause10.Enabled = true;
                stop10.Enabled = true;
            }
            if (boti[9]._state == State.pause)
            {
                boti[9]._state = State.run;
                start10.Enabled = false;
                pause10.Enabled = true;
                stop10.Enabled = true;
            }



            //this.Controls[loglabel].Text = pole;
        }
        private void Pause1_Click(object sender, EventArgs e)
        {
           stop1.Enabled = false;
            start1.Enabled = true;
            pause1.Enabled = false;

            boti[0]._state = State.pause;
           // bot0t.Stop();
        }
        private void Pause2_Click(object sender, EventArgs e)
        {
           stop2.Enabled = false;
            start2.Enabled = true;
            pause2.Enabled = false;

            boti[1]._state = State.pause;
           // bot1t.Stop();

        }
        private void Pause3_Click(object sender, EventArgs e)
        {
            boti[2]._state = State.pause;
           stop3.Enabled = false;
            start3.Enabled = true;
            pause3.Enabled = false;
           // bot2t.Stop();
        }
        private void Pause4_Click(object sender, EventArgs e)
        {
            boti[3]._state = State.pause;
           stop4.Enabled = false;
            start4.Enabled = true;
            pause4.Enabled = false;
          //  bot3t.Stop();
        }
        private void Pause5_Click(object sender, EventArgs e)
        {
            boti[4]._state = State.pause;
           stop5.Enabled = false;
            start5.Enabled = true;
            pause5.Enabled = false;
         //   bot4t.Stop();
        }
        private void Pause6_Click(object sender, EventArgs e)
        {
            boti[5]._state = State.pause;
           stop6.Enabled = false;
            start6.Enabled = true;
            pause6.Enabled = false;
         //   bot5t.Stop();

        }
        private void Pause7_Click(object sender, EventArgs e)
        {
            boti[6]._state = State.pause;
           stop7.Enabled = false;
            start7.Enabled = true;
            pause7.Enabled = false;
      //      bot6t.Stop();

        }
        private void Pause8_Click(object sender, EventArgs e)
        {
            boti[7]._state = State.pause;
           stop8.Enabled = false;
            start8.Enabled = true;
            pause8.Enabled = false;
          //  bot7t.Stop();

        }
        private void Pause9_Click(object sender, EventArgs e)
        {
            boti[8]._state = State.pause;
           stop9.Enabled = false;
            start9.Enabled = true;
            pause9.Enabled = false;
          //  bot8t.Stop();

        }
        private void Pause10_Click(object sender, EventArgs e)
        {
            boti[9]._state = State.pause;
            stop10.Enabled = false;
            start10.Enabled = true;
            pause10.Enabled = false;
        //    bot9t.Stop();
        }
        private void Stop1_Click(object sender, EventArgs e)
        {
            source2[0].Cancel();
            stop1.Enabled = false;
            start1.Enabled = true;
            pause1.Enabled = false;
            tasks[0] = 0;
            bot0t.Enabled = false;
            timer0.Text = "";
            try
            {
                
                boti[0]._state = State.stop;
              
            }
            catch (Exception)
            {

            }
           
        }
        private void Stop2_Click(object sender, EventArgs e)
        {
            source2[1].Cancel();
            start2.Enabled = true;
           stop2.Enabled = false;
            pause2.Enabled = false;
            bot1t.Enabled = false;
            buttons[1] = 0;
            tasks[1] = 0;
            timer1.Text = "";
            try
            {
              
                boti[1]._state = State.stop;
               
            }
            catch (Exception)
            {

            }

}
        private void Stop3_Click(object sender, EventArgs e)
        {
            source2[2].Cancel();
            start3.Enabled = true;
           stop3.Enabled = false;
            pause3.Enabled = false;
            bot2t.Enabled = false;
            buttons[2] = 0;
            timer2.Text = "";
            tasks[2] = 0;
            try
            {
           
            boti[2]._state = State.stop;
           
            }
            catch (Exception)
            {

            }
}
        private void Stop4_Click(object sender, EventArgs e)
        {
            source2[3].Cancel();
            start4.Enabled = true;
            pause4.Enabled = false;
           stop4.Enabled = false;
            bot3t.Enabled = false;
            timer3.Text = "";
            buttons[3] = 0;
            tasks[3] = 0;
            try
            {
           
            boti[3]._state = State.stop;
           
            }
            catch (Exception)
            {

            }
        }
        private void Stop5_Click(object sender, EventArgs e)
        {
            source2[4].Cancel();
            start5.Enabled = true;
            pause5.Enabled = false;
           stop5.Enabled = false;
            bot4t.Enabled = false;
            timer4.Text = "";
            buttons[4] = 0;
            tasks[4] = 0;
            try
            { 
           
            boti[4]._state = State.stop;
           
            }
             catch (Exception)
            {

            }
        }
        int ischecked = 0;
        private void Checkall_Click(object sender, EventArgs e)
        {
            if (ischecked==0)
            {
                foreach (var cb in this.Controls)
                {
                    if (cb is CheckBox)
                    {
                        CheckBox c = (CheckBox)cb;
                        c.Checked = true;
                    }
                }
                ischecked = 1;
            }
            else
            {
                foreach (var cb in this.Controls)
                {
                    if (cb is CheckBox)
                    {
                        CheckBox c = (CheckBox)cb;
                        c.Checked = false;
                    }
                }
                ischecked = 0;
            }
        }

        private void Stop6_Click(object sender, EventArgs e)
        {
            source2[5].Cancel();
            start6.Enabled = true;
           stop6.Enabled = false;
            pause6.Enabled = false;
            bot5t.Enabled = false;
            timer5.Text = "";
            buttons[5] = 0;
            tasks[5] = 0;
            try
            {
              
            boti[5]._state = State.stop;
           
            }
            catch (Exception)
            {

            }
        }
        private void Stop7_Click(object sender, EventArgs e)
        {
            source2[6].Cancel();
            start7.Enabled = true;
           stop7.Enabled = false;
            pause7.Enabled = false;
            bot6t.Enabled = false;
            timer6.Text = "";

            buttons[6] = 0;
            tasks[6] = 0;
            try
            {
           
            boti[6]._state = State.stop;
           
            }
             catch (Exception)
            {

            }
        }
        private void Stop8_Click(object sender, EventArgs e)
        {
            source2[7].Cancel();
            start8.Enabled = true;
            stop8.Enabled = false;
            pause8.Enabled = false;
            bot7t.Enabled = false;
            timer7.Text = "";

            buttons[7] = 0;
            tasks[7] = 0;
            try
            {
               
                boti[7]._state = State.stop;
               
            }
            catch (Exception)
            {

            }

        }
        private void Stop9_Click(object sender, EventArgs e)
        {
            source2[8].Cancel();
            start9.Enabled = true;
           stop9.Enabled = false;
            pause9.Enabled = false;
            bot8t.Enabled = false;
            timer8.Text = "";

            buttons[8] = 0;
            tasks[8] = 0;
            try
            { 
          
            boti[8]._state = State.stop;
          
            }
            catch (Exception)
            {

            }
}
        private void Stop10_Click(object sender, EventArgs e)
        {
            source2[9].Cancel();
            start10.Enabled = true;
            stop10.Enabled = false;
            pause10.Enabled = false;
            bot9t.Enabled = false;
            timer9.Text = "";
            buttons[9] = 0;
            tasks[9] = 0;
            try
            { 
           
            boti[9]._state = State.stop;
            
            }
            catch (Exception)
            {

            }
}
        private void Accupdate_Click(object sender, EventArgs e)
        {
            string nameaccs = "account";
            string acc;
            bool first = true;
            for (int i = 0; i < 10; i++)
            {
                acc = nameaccs + i.ToString();
                if (first == true)
                {
                    if (Controls[acc].Text.Length != 0)
                    {
                        using (StreamWriter sw = new StreamWriter("accs", false, System.Text.Encoding.Default))
                        {
                            sw.WriteLine("acc" + i.ToString() + "=" + Controls[acc].Text);
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter("accs", false, System.Text.Encoding.Default))
                        {
                            sw.WriteLine("acc" + i.ToString() + "=");
                        }
                    }
                    first = false;
                }
                else
                {
                    if (Controls[acc].Text.Length != 0)
                    {
                        using (StreamWriter sw = new StreamWriter("accs", true, System.Text.Encoding.Default))
                        {
                            sw.WriteLine("acc" + i.ToString() + "=" + Controls[acc].Text);
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter("accs", true, System.Text.Encoding.Default))
                        {
                            sw.WriteLine("acc" + i.ToString() + "=");
                        }
                    }
                }



            }
            MessageBox.Show("acc file updated", "result", MessageBoxButtons.OK);

        }
        private void LeavegameTargeted_Click(object sender, EventArgs e)
        {
            int i = 0;
            foreach (var cb in this.Controls)
            {
                if (cb is CheckBox)
                {
                    CheckBox c = (CheckBox)cb;
                    if(c.Checked == true)
                    {
                        string str = c.Name.Substring(8);
                        try
                        {
                            int k = Convert.ToInt32(str) - 1;
                            boti[k]._state = State.stop;
                            source2[k].Cancel();
                          
                                
                               
                                int[] parametrs = { k, 1, 0 };
                                buttons[k] = 0;
                                Controls["start" + (k+1).ToString()].Enabled = true;
                                Controls["pause" + (k + 1).ToString()].Enabled = false;
                                Controls["stop" + (k + 1).ToString()].Enabled = false;

                                boti[k] = new bot(parametrs, this);
                            

                        }
                        catch(Exception)
                        {
                            
                        }
                    }
                    
                }
            }

        }
        private void Meteorits_Click(object sender, EventArgs e)
        {

        }
        private void Configfile1_Click(object sender, EventArgs e)
        {
            var forms = new configbot("bot1.conf");
           stop1.Enabled = false;
            start1.Enabled = true;
            pause1.Enabled = false;
            try
            {
                boti[0]._state = State.stop;
            }
            catch (Exception)
            {

            }
            bot0t.Enabled = false;
            timer0.Text = "";
            forms.Show();
        }
        private void Configfile2_Click(object sender, EventArgs e)
        {
            var forms = new configbot("bot2.conf");
           
            start2.Enabled = true;
            stop2.Enabled = false;
            pause2.Enabled = false;
            try
            {
                boti[1]._state = State.stop;
            }
            catch (Exception)
            {

            }
            bot1t.Enabled = false;
            timer1.Text = "";
            forms.Show();
        }
        private void Configfile3_Click(object sender, EventArgs e)
        {
            var forms = new configbot("bot3.conf");
           
            start3.Enabled = true;
           stop3.Enabled = false;
            pause3.Enabled = false;
            try
            {
                boti[2]._state = State.stop;
            }
            catch (Exception)
            {

            }
            bot2t.Enabled = false;
            timer2.Text = "";
            forms.Show();
        }
        private void Configfile4_Click(object sender, EventArgs e)
        {
            var forms = new configbot("bot4.conf");
           
            start4.Enabled = true;
           stop4.Enabled = false;
            pause4.Enabled = false;
            try
            {
                boti[3]._state = State.stop;
            }
            catch (Exception)
            {

            }
            bot3t.Enabled = false;
            timer3.Text = "";
            forms.Show();
        }
        private void Configfile5_Click(object sender, EventArgs e)
        {
            var forms = new configbot("bot5.conf");
           
            start5.Enabled = true;
           stop5.Enabled = false;
            pause5.Enabled = false;
            try
            {
                boti[4]._state = State.stop;
            }
            catch (Exception)
            {

            }
            bot4t.Enabled = false;
            timer4.Text = "";
            forms.Show();
        }
        private void Configfile6_Click(object sender, EventArgs e)
        {
            var forms = new configbot("bot6.conf");
            
            start6.Enabled = true;
           stop6.Enabled = false;
            pause6.Enabled = false;
            try
            {
                boti[5]._state = State.stop;
            }
            catch (Exception)
            {

            }
            bot5t.Enabled = false;
            timer5.Text = "";
            forms.Show();
        }
        private void Configfile7_Click(object sender, EventArgs e)
        {
            var forms = new configbot("bot7.conf");
           
            start7.Enabled = true;
           stop7.Enabled = false;
            pause7.Enabled = false;
            try
            {
                boti[6]._state = State.stop;
            }
            catch(Exception)
            {

            }
            bot6t.Enabled = false;
            timer6.Text = "";
            forms.Show();
        }
        private void Configfile8_Click(object sender, EventArgs e)
        {
            var forms = new configbot("bot8.conf");
            start8.Enabled = true;
           stop8.Enabled = false;
            pause8.Enabled = false;
            try
            {
                boti[7]._state = State.stop;
            }
            catch (Exception)
            {

            }
            bot7t.Enabled = false;
            timer7.Text = "";
            forms.Show();
        }
        private void Configfile9_Click(object sender, EventArgs e)
        {
            var forms = new configbot("bot9.conf");
            start9.Enabled = true;
           stop9.Enabled = false;
            pause9.Enabled = false;
            try
            {
                boti[8]._state = State.stop;
            }
            catch (Exception)
            {

            }
            bot8t.Enabled = false;
            timer8.Text = "";
            forms.Show();
        }
        private void Configfile10_Click(object sender, EventArgs e)
        {
            var forms = new configbot("bot10.conf");
            start10.Enabled = true;
            stop10.Enabled = false;
            pause10.Enabled = false;
            try
            {
                boti[9]._state = State.stop;
            }
            catch (Exception)
            {

            }
            bot9t.Enabled = false;
            timer9.Text = "";
            forms.Show();
        }






    }


}
namespace ScreenShot
{
    /// <summary>
    /// Provides functions to capture the entire screen, or a particular window, and save it to a file.
    /// </summary>
    public class ScreenCapture
    {
        /// <summary>
        /// Creates an Image object containing a screen shot of the entire desktop
        /// </summary>
        /// <returns></returns>
        public Image CaptureScreen()
        {
            return CaptureWindow(User32.GetDesktopWindow());
        }
        /// <summary>
        /// Creates an Image object containing a screen shot of a specific window
        /// </summary>
        /// <param name="handle">The handle to the window. (In windows forms, this is obtained by the Handle property)</param>
        /// <returns></returns>
        public Image CaptureImage(IntPtr handle, Point temppoint)//должен делать скрин определенной области окна, доработать на будущее
        {
            // get te hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            // create a device context we can copy to
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            // bitblt over
            GDI32.BitBlt(hdcDest, temppoint.X, temppoint.Y, 101, 44, hdcSrc, temppoint.X, temppoint.Y, GDI32.SRCCOPY);
            // restore selection
            GDI32.SelectObject(hdcDest, hOld);
            // clean up
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);
            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            GDI32.DeleteObject(hBitmap);
            return img;
        }
        public Image CaptureWindow(IntPtr handle)
        {
            // get te hDC of the target window
            IntPtr hdcSrc = User32.GetWindowDC(handle);
            // get the size
            User32.RECT windowRect = new User32.RECT();
            User32.GetWindowRect(handle, ref windowRect);
            int width = windowRect.right - windowRect.left;
            int height = windowRect.bottom - windowRect.top;
            // create a device context we can copy to
            IntPtr hdcDest = GDI32.CreateCompatibleDC(hdcSrc);
            // create a bitmap we can copy it to,
            // using GetDeviceCaps to get the width/height
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hdcSrc, width, height);
            // select the bitmap object
            IntPtr hOld = GDI32.SelectObject(hdcDest, hBitmap);
            // bitblt over
            GDI32.BitBlt(hdcDest, 0, 0, width, height, hdcSrc, 0, 0, GDI32.SRCCOPY);
            // restore selection
            GDI32.SelectObject(hdcDest, hOld);
            // clean up
            GDI32.DeleteDC(hdcDest);
            User32.ReleaseDC(handle, hdcSrc);
            // get a .NET image object for it
            Image img = Image.FromHbitmap(hBitmap);
            // free up the Bitmap object
            GDI32.DeleteObject(hBitmap);
            return img;
        }
        /// <summary>
        /// Captures a screen shot of a specific window, and saves it to a file
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public void CaptureWindowToFile(IntPtr handle, string filename, ImageFormat format)
        {
            Image img = CaptureWindow(handle);
            img.Save(filename, format);
        }
        /// <summary>
        /// Captures a screen shot of the entire desktop, and saves it to a file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="format"></param>
        public void CaptureScreenToFile(string filename, ImageFormat format)
        {
            Image img = CaptureScreen();
            img.Save(filename, format);
        }

        /// <summary>
        /// Helper class containing Gdi32 API functions
        /// </summary>
        private class GDI32
        {

            public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter
            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                int nWidth, int nHeight, IntPtr hObjectSource,
                int nXSrc, int nYSrc, int dwRop);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                int nHeight);
            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);
            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);
            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }

        /// <summary>
        /// Helper class containing User32 API functions
        /// </summary>
        private class User32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int left;
                public int top;
                public int right;
                public int bottom;
            }
            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);
            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);
        }
    }
}