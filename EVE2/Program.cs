using System;
using System.Windows.Forms;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Generic;


namespace ALPilot
{
   
     static class images
    {
        public static Image<Bgr, byte> inwarp=null;
        public static Image<Bgr, byte> inputImage = null;
        public static Image<Bgr, byte> search = null;
        public static Image<Bgr, byte> keyboard9 = null;
        public static Image<Bgr, byte> keybooardok = null;
        public static Image<Bgr, byte> searchtext = null;
        public static Image<Bgr, byte> searchsend = null;
        public static Image<Bgr, byte> map = null;
        public static Image<Bgr, byte> closemap = null;
        public static Image<Bgr, byte> rightmenu = null;
        public static Image<Bgr, byte> fly = null;
        public static Image<Bgr, byte> flyaccept = null;
        public static Image<Bgr, byte> searchblock = null;
        public static Image<Bgr, byte> myloc = null;
        public static Image<Bgr, byte> poyas = null;
        public static Image<Bgr, byte> leavedoc = null;
        public static Image<Bgr, byte> warp = null;
        public static Image<Bgr, byte> kernite = null;
        public static Image<Bgr, byte> inflight = null;
        public static Image<Bgr, byte> moveto = null;
        public static Image<Bgr, byte> shop = null;
        public static Image<Bgr, byte> targetedkernitefarm = null;
        public static Image<Bgr, byte> closequeue = null;
        public static Image<Bgr, byte> sklad = null;
        public static Image<Bgr, byte> flytocenteraccept = null;
        public static Image<Bgr, byte> queuefull = null;
        public static Image<Bgr, byte> rudeotsek = null;
        public static Image<Bgr, byte> movetosklad = null;
        public static Image<Bgr, byte> sell = null;
        public static Image<Bgr, byte> sell2 = null;
        public static Image<Bgr, byte> gun1 = null;
        public static Image<Bgr, byte> insklad = null;
        public static Image<Bgr, byte> dokfly = null;
        public static Image<Bgr, byte> gettarget = null;
        public static Image<Bgr, byte> speed0 = null;
        public static Image<Bgr, byte> kerniteonsklad = null;
        public static Image<Bgr, byte> tempp = null;
        public static Image<Bgr, byte> aster = null;
        public static Image<Bgr, byte> inventarclose = null;
        public static Image<Bgr, byte> planetmenu = null;
        public static Image<Bgr, byte> planetrenew = null;
        public static Image<Bgr, byte> planetnotime = null;
        public static Image<Bgr, byte> planetlowtime = null;
        public static Image<Bgr, byte> planetfulltime = null;
        public static Image<Bgr, byte> changepers = null;
        public static Image<Bgr, byte> planetopened = null;
        public static Image<Bgr, byte> planetclose = null;
        public static Image<Bgr, byte> gamesettings = null;
        public static Image<Bgr, byte> venture = null;
        public static Image<Bgr, byte> irbis = null;
        public static Image<Bgr, byte> pickall = null;
        public static Image<Bgr, byte> gameicon = null;
        public static Image<Bgr, byte> condor = null;
        public static Image<Bgr, byte> cancelcurse = null;
        public static Image<Bgr, byte> shopcenter = null;
        public static Image<Bgr, byte> corpsklad = null;
        public static Image<Bgr, byte> station = null;
        public static Image<Bgr, byte> crokite = null;
        public static Image<Bgr, byte> targetedcrokitefarm = null;
        public static Image<Bgr, byte> bistode = null;
        public static Image<Bgr, byte> targetedbistodefarm = null;
        public static Image<Bgr, byte> arcanor = null;
        public static Image<Bgr, byte> targetedarcanorfarm = null;
        public static Image<Bgr, byte> venture3 = null;
        public static Image<Bgr, byte> targetedrudeotsek = null;
        public static Image<Bgr, byte> canceltarget = null;
        public static Image<Bgr, byte> bubble = null;
        public static Image<Bgr, byte> enemylocal = null;
        public static Image<Bgr, byte> localpeople = null;
        public static Image<Bgr, byte> localminimize = null;
        public static Image<Bgr, byte> localniz = null;
        public static Image<Bgr, byte> filtrpoyas = null;
        public static Image<Bgr, byte> filtraster = null;
        public static Image<Bgr, byte> gneiss = null;
        public static Image<Bgr, byte> targetedgneissfarm = null;
        public static Image<Bgr, byte> androiderror = null;
        public static Image<Bgr, byte> egg = null;
        public static Image<Bgr, byte> wasattecked = null;
        public static Image<Bgr, byte> wasdestroyed = null;
        public static Image<Bgr, byte> hedbergite = null;
        public static Image<Bgr, byte> targetedhedbergitefarm = null;
        public static Image<Bgr, byte> spodumain =null;
        public static Image<Bgr, byte> targetedspodumainfarm = null;
        public static Image<Bgr, byte> jaspet = null;
        public static Image<Bgr, byte> targetedjaspetfarm = null;
        public static Image<Bgr, byte> pyroxeres = null;
        public static Image<Bgr, byte> targetedpyroxeresfarm = null;
    }
    static class Program
    {



        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        ///  int i = 1, k = 1, j = 0;
        // StreamReader temp = File.OpenText("config.conf");
        /* while ((locfarm = temp.ReadLine()) != null)
         {
             listBox1.Items.Add(locfarm[i]);
         }*/
       

        public static bool firststart = true;
        static public configbot enternames;




        [STAThread]
        static void Main()
        {
            images.wasattecked = new Image<Bgr, byte>("атакован.bmp");
            images.wasdestroyed = new Image<Bgr, byte>("уничтожен.bmp");
        images.search = new Image<Bgr, byte>("поиск.bmp");
        images.searchtext = new Image<Bgr, byte>("текстпоиск.bmp");
        images.searchsend = new Image<Bgr, byte>("поискок.bmp");
        images.map = new Image<Bgr, byte>("карта.bmp");
        images.closemap = new Image<Bgr, byte>("закрытькарту.bmp");
        images.rightmenu = new Image<Bgr, byte>("боковое меню.bmp");
        images.fly = new Image<Bgr, byte>("отправиться.bmp");
        images.flyaccept = new Image<Bgr, byte>("отправитьсяпринять.bmp");
        images.searchblock = new Image<Bgr, byte>("поисквыбор.bmp");
        images.poyas = new Image<Bgr, byte>("полеастероидов.bmp");
        images.leavedoc = new Image<Bgr, byte>("выйтидок.bmp");
        images.warp = new Image<Bgr, byte>("варпануться.bmp");
        images.kernite = new Image<Bgr, byte>("кернит.bmp");
        images.inflight = new Image<Bgr, byte>("вполете.bmp");
        images.moveto = new Image<Bgr, byte>("орбита.bmp");
        images.targetedkernitefarm = new Image<Bgr, byte>("взяткернитaфармится.bmp");
        images.closequeue = new Image<Bgr, byte>("очередьзакрыть.bmp");
        images.sklad = new Image<Bgr, byte>("склад.bmp");
        images.flytocenteraccept = new Image<Bgr, byte>("указатьпунктназначения.bmp");
        images.queuefull = new Image<Bgr, byte>("очередь.bmp");
        images.rudeotsek = new Image<Bgr, byte>("отсекруда.bmp");
        images.movetosklad = new Image<Bgr, byte>("переместитьв.bmp");
        images.sell = new Image<Bgr, byte>("продать.bmp");
        images.sell2 = new Image<Bgr, byte>("продать2.bmp");
        images.gun1 = new Image<Bgr, byte>("пушка.bmp");
        images.insklad = new Image<Bgr, byte>("tosklad.bmp");
        images.dokfly = new Image<Bgr, byte>("докполететь.bmp");
        images.gettarget = new Image<Bgr, byte>("взятьтаргет.bmp");
        images.speed0 = new Image<Bgr, byte>("скорость.bmp");
        images.kerniteonsklad = new Image<Bgr, byte>("кернитнаскладе.bmp");
        images.aster = new Image<Bgr, byte>("астероид.bmp");
        images.inventarclose = new Image<Bgr, byte>("закрытьинвентарь.bmp");
        images.planetmenu = new Image<Bgr, byte>("планетаркаменю.bmp");
        images.planetrenew = new Image<Bgr, byte>("планетаркапродлить.bmp");
        images.planetnotime = new Image<Bgr, byte>("планетаркаистектаймер.bmp");
        images.planetlowtime = new Image<Bgr, byte>("планетаркамалотаймер.bmp");
        images.planetfulltime = new Image<Bgr, byte>("планетаркафуллтаймер.bmp");
        images.changepers = new Image<Bgr, byte>("сменитьперса.bmp");
        images.planetopened = new Image<Bgr, byte>("планетаркаоткрылась.bmp");
        images.planetclose = new Image<Bgr, byte>("планетарказакрыть.bmp");
        images.gamesettings = new Image<Bgr, byte>("игровыенастройки.bmp");
        images.venture = new Image<Bgr, byte>("вентура.bmp");
        images.irbis = new Image<Bgr, byte>("ирбис.bmp");
        images.condor = new Image<Bgr, byte>("кондор.bmp");
        images.pickall = new Image<Bgr, byte>("выбратьвсе.bmp");
        images.gameicon = new Image<Bgr, byte>("игра.bmp");
        images.cancelcurse = new Image<Bgr, byte>("отменакурса.bmp");
        images.shopcenter = new Image<Bgr, byte>("торговыйцентркарта.bmp");
        images.station = new Image<Bgr, byte>("станция.bmp");
        images.corpsklad = new Image<Bgr, byte>("корпсклад.bmp");
            images.targetedrudeotsek = new Image<Bgr, byte>("выбранотсекруды.bmp");
            images.venture3 = new Image<Bgr, byte>("вентура3.bmp");
            images.crokite = new Image<Bgr, byte>("крокит.bmp");
            images.targetedcrokitefarm = new Image<Bgr, byte>("крокиттаргет.bmp");
            images.bistode = new Image<Bgr, byte>("бистод.bmp");
            images.targetedbistodefarm = new Image<Bgr, byte>("бистодтаргет.bmp");
            images.arcanor = new Image<Bgr, byte>("арконор.bmp");
            images.targetedarcanorfarm = new Image<Bgr, byte>("арконортаргет.bmp");
            images.canceltarget = new Image<Bgr, byte>("таргетотмена.bmp");
            images.bubble = new Image<Bgr, byte>("бубль.bmp");
            images.enemylocal = new Image<Bgr, byte>("враглокал.bmp");
            images.localpeople = new Image<Bgr, byte>("локал.bmp");
            images.localminimize = new Image<Bgr, byte>("локалминимальный.bmp");
            images.localniz = new Image<Bgr, byte>("локалсвернуть.bmp");
            images.filtrpoyas = new Image<Bgr, byte>("фильтр пояс.bmp");
            images.filtraster = new Image<Bgr, byte>("фильтр астероид.bmp");
            images.gneiss = new Image<Bgr, byte>("гнейс.bmp");
            images.targetedgneissfarm = new Image<Bgr, byte>("гнейстаргет.bmp");
            images.androiderror = new Image<Bgr, byte>("ошибкаандроид.bmp");
            images.egg = new Image<Bgr, byte>("яйцо.bmp");
            images.hedbergite = new Image<Bgr, byte>("хедбергит.bmp");
            images.targetedhedbergitefarm = new Image<Bgr, byte>("хедбергиттаргет.bmp");
            images.spodumain = new Image<Bgr, byte>("сподуман.bmp");
            images.targetedspodumainfarm = new Image<Bgr, byte>("сподумантаргет.bmp");
            images.jaspet = new Image<Bgr, byte>("джаспет.bmp");
            images.targetedjaspetfarm = new Image<Bgr, byte>("джаспеттаргет.bmp");
            images.pyroxeres = new Image<Bgr, byte>("пироксирез.bmp");
            images.targetedpyroxeresfarm = new Image<Bgr, byte>("пироксирезтаргет.bmp");
            checkconfig();

            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);


           // Form meteorites = new meteors();
           // meteorites.ShowDialog();

            Application.Run(new Form1());
        }
        static void checkconfig()
        {
            bool check = false;
            /*if (File.Exists(".wasstarted"))
            {
                firststart = false;
                if (!File.Exists("windows"))
                {
                    File.Create("windows");
                    enternames = new windownames();
                    enternames.ShowDialog();
                }
                else
                {
                    enternames = new windownames();
                    enternames.ShowDialog();
                }
            }
            else
            {
                File.Create(".wasstarted");
                if (!File.Exists("windows"))
                {
                    File.Create("windows");
                    enternames = new windownames();
                    enternames.ShowDialog();
                }
                else
                {
                    enternames = new windownames();
                    enternames.ShowDialog();
                }
            }*/
           
           
               
            
          
            


            //проверка правильности ввода файла с локациями фарма/сдачи

           

            
            //locsell = new String[temp.Length + temp2.Length + temp3.Length + temp4.Length + temp5.Length];
           

        }
       

    }


  
}
