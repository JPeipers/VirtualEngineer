using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;
using System.Windows;
//using System.Windows.Forms;
using System.Windows.Controls;
using System.Timers;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Diplomdarbs_v0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool Connected = false;
        bool Autolvl = false;
        bool zProbeState = false;
        bool okStop = true;
        bool readProbePosition = false;
        bool gotPosition = false;
        bool DMeasureComplete = false;
        bool diveMeasureComplete = false;
        bool probeTestMeasureComplete = false;
        bool EEPROM_ready = false;
        bool addMeasure = true;
        bool addBlank = true;
        bool randomSquareComplete = false;
        bool canReach = false;
        bool directionMarker = false;
        bool initialisePosition = false;
        bool fineDraw = false;
        bool clipEdges = true;


        int routineCounter = 0;
        int commandLoopType = -1;
        int position = -1;
        int stepType = 0; //0 - coarse, 1 - fine
        int maxDiscreteDiameter = 0;
        int measureCounter = 0;
        int uniqueMeasurePosition = 0;
        int barCount = 0;
        int barWidth = 0;
        int barPosition = 0;
        int probeMeasureCounter = 0;
        int barLabelYoffset = 0;
        int wholeGridSize = 0;
        int waitCount = 0;
        int gridProbePointsRandom = 0;
        int randomBreak = 0;
        int gridSquareRandomCount = 0;
        int gridCountXAxis = 0;
        int gridCountYAxis = 0;
        int localRandomPointCounter = 0;
        int processOrigin = -1;
        int multiplierColorScale = 0;
        int probePoints = 0;
        int gridSizeUniversal = 0;
        int printerSize = 0;
        int gcodeType = -1;
        int universalPointCounter = 0;
        int F_headPos = 0;
        int F_headPos_new = 0;
        int slipumsInfinity = 0;
        int calibratePhase = -1;
        int clearance = 15;

        float probeReach = 0f;
        float coveredArea = 0f;
        float xPosMath = 0f;
        float yPosMath = 0f;
        float areaCovered = 0f;
        float barMaxValue = 0f;
        float squareToGridScale = 1;
        float discreteRadiuss = 0f;
        float discreteRadiussCheck = 0f;
        float discreteRadiussCheckEqual = 0f;
        float localRandomSum = 0f;
        float XYoffset = 0f;
        float universalMeasureSum = 0f;
        float xGridPosition = 0f;
        float yGridPosition = 0f;
        float X_headPos = 0f;
        float Y_headPos = 0f;
        float Z_headPos = 0f;
        float E_headPos = 0f;
        float X_headPos_new = 0f;
        float Y_headPos_new = 0f;
        float Z_headPos_new = 0f;
        float E_headPos_new = 0f;
        float[,] gridArrayProcess;
        float printSize = 0f;
        float gridSize = 0f;
        float slipumaKoeficients = 0f;
        float linearaNovirze = 0f;
        float diminishingZ = 1f;
        float Zoffset = 0f;

        string z_probeHeight = "0";
        string bedDistance = "0";
        string XY1 = "0";
        string XY2 = "0";
        string XY3 = "0";
        string endstopX = "0";
        string endstopY = "0";
        string endstopZ = "0";
        string horizontalRadiuss = "0";

        string coarseStep = "0.2";
        string fineStep = "0.01";
        string stepSize = "";
        string[] positionArray = new string[7];
        string[,,] gridArray;
        //string[,,] gridArrayProcess;
        string infoText_positionCase = "Position case";
        string infoText_routineCase = "Routine case";
        string infoText_fineRoutineCase = "Fine routine case";
        string bufString;
        string positionString = "";
        string generatedFilename = "";
        string gcode_printerSize = "";
        string gcode_gridSize = "";
        string gcodeNameExport = "";
        string fileNameExport = "";



        SerialPort myPort = new SerialPort();

        List<string> linesToProcess = new List<string>();
        List<string> rawGcode = new List<string>();
        List<float> probeMeasurementsList = new List<float>();
        List<int> measurementCounterList = new List<int>();
        List<float> uniqueProbeMeasurementsList = new List<float>();
        List<string> gridMeasureGcode = new List<string>();
        List<string> disposableGridMeasureGcode = new List<string>();
        List<string> EEPROMList = new List<string>();
        List<Tuple<float, float>> gridProbePoints = new List<Tuple<float, float>>();
        List<float> gridMeasureSimpleResults = new List<float>();
        List<float> gridMeasureRandomResults = new List<float>();
        List<string> gridMeasureSimple = new List<string>();
        List<float> gridMeasureRandom = new List<float>();
        List<float> gridMeasureUniversal = new List<float>();
        List<int> XCrossList = new List<int>();
        List<int> YCrossList = new List<int>();
        List<string> tableData = new List<string>();



        List<float> disposableMeasureList = new List<float>();
        List<Tuple<float, float, float>> gridTestSimpleResults = new List<Tuple<float, float, float>>();
        List<Tuple<float, float, float>> gridTestRandomResults = new List<Tuple<float, float, float>>();
        List<Tuple<float, float, float>> gridTestRandomResultsCorrect = new List<Tuple<float, float, float>>();
        List<Tuple<float, float, float>> gridTestSimpleCompleteGrid = new List<Tuple<float, float, float>>();
        List<Tuple<float, float, float>> gridTestRandomCompleteGrid = new List<Tuple<float, float, float>>();
        List<Tuple<float, float, float>> gridTestUniversalResults = new List<Tuple<float, float, float>>();
        List<Tuple<float, float>> crossPointList = new List<Tuple<float, float>>();
        List<Tuple<float, float>> midPointList = new List<Tuple<float, float>>();
        List<Tuple<float, float>> halfStepList = new List<Tuple<float, float>>();
        List<float> extruderPosition = new List<float>();
        List<Tuple<string, string>> gridTestUniversal = new List<Tuple<string, string>>();


        System.Timers.Timer routineTimer = new System.Timers.Timer();

        System.Windows.Shapes.Rectangle barGraph;
        System.Windows.Shapes.Rectangle gridSquare;

        Random rnd = new Random();

        public MainWindow()
        {
            InitializeComponent();

            comboBox_baudrate.Items.Add("1200");
            comboBox_baudrate.Items.Add("2400");
            comboBox_baudrate.Items.Add("4800");
            comboBox_baudrate.Items.Add("9600");
            comboBox_baudrate.Items.Add("19200");
            comboBox_baudrate.Items.Add("38400");
            comboBox_baudrate.Items.Add("57600");
            comboBox_baudrate.Items.Add("115200");



            comboBox_comList.Items.Clear();
            string[] scanPorts = SerialPort.GetPortNames();
            foreach (string comport in scanPorts)
            {
                comboBox_comList.Items.Add(comport);
            }

            if (scanPorts.Length == 1)
            {
                comboBox_comList.SelectedValue = scanPorts[0];
            }
            routineTimer.Elapsed += new ElapsedEventHandler(TimedClicking);
            routineTimer.Enabled = false;

            /*   System.Timers.Timer routineTimer = new System.Timers.Timer();
               routineTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
               routineTimer.Interval = 5000;
               
           */
        }
        /*
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                textBox_console.AppendText("Timer.");
                textBox_console.AppendText(Environment.NewLine);


            });
        }
        */
        /// <summary>
        /// Pamata procedūras un atsevišķas testa funkcijas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dropDownOpened(object sender, EventArgs e)
        {
            comboBox_comList.Items.Clear();
            string[] ports = SerialPort.GetPortNames();
            foreach (string comport in ports)
            {
                comboBox_comList.Items.Add(comport);
            }
        }
        private void button_connectCom_Click(object sender, RoutedEventArgs e)
        {

            if (Connected == false)
            {
                button_send.IsEnabled = true;
                int selectedIndex = comboBox_comList.SelectedIndex;
                Object selectedCom = comboBox_comList.SelectedItem;
                Object selectedBaudrate = comboBox_baudrate.SelectedItem;
                label_debug.Content = ("Selected: " + selectedCom.ToString() + "\n" + "Index: " + selectedIndex.ToString());

                myPort.PortName = selectedCom.ToString();
                myPort.BaudRate = Convert.ToInt32(selectedBaudrate.ToString());
                myPort.DataBits = 8;
                myPort.Parity = Parity.None;
                myPort.StopBits = StopBits.One;

                myPort.Open();
                Connected = true;
                string sent = textBox_console.Text;
                textBox_console.AppendText("Connected.");
                textBox_console.AppendText(Environment.NewLine);
                myPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                button_connectCom.Content = "Disconnect";


            }
            else
            {
                myPort.Close();
                Connected = false;
                button_connectCom.Content = "Connect";
            }


        }
        public void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                SerialPort sp = (SerialPort)sender;
                string indata = sp.ReadExisting();
                textBox_console.AppendText(indata);
                //
                bufString += indata;
                if (bufString.Contains("\n"))
                {
                    List<string> lines = bufString.Split('\n').ToList();
                    for (int i = 0; i < lines.Count - 1; i++)
                    {
                        linesToProcess.Add(lines[i]);

                        if (lines[i].Contains("DM") && lines[i].Contains("X:") && lines[i].Contains("Y:") && lines[i].Contains("Z:"))
                        {

                            positionString = lines[i];
                            label_triggerPosition.Content = lines[i];
                            diveMeasureComplete = true;
                            okStop = true;
                            //routineTimer.Interval = 50;
                        }

                        if (!lines[i].Contains("DM=") && lines[i].Contains("X:") && lines[i].Contains("Y:") && lines[i].Contains("Z:") && lines[i].Contains("E:"))
                        {
                            label_PositionContent.Content = lines[i];
                            if (readProbePosition == true)
                            {
                                positionString = lines[i];
                                label_triggerPosition.Content = lines[i];
                                gotPosition = true;
                                readProbePosition = false;
                            }
                        }


                        //Z-probe state:H
                        if (lines[i].Contains("state:H"))
                        {
                            zProbeState = true;
                            routineTimer.Interval = 200;
                        }
                        if (lines[i].Contains("state:L"))
                        {
                            zProbeState = false;
                        }
                        if (lines[i].Contains("Info:"))
                        {
                            okStop = true;
                        }
                        if (linesToProcess.Count > 400)
                        {
                            while (linesToProcess.Count > 300)
                            {
                                linesToProcess.RemoveAt(0);
                            }

                        }
                        if (lines[i].Contains("ok"))
                        {
                            okStop = true;

                        }

                        if (lines[i].Contains("wait"))
                        {
                            waitCount++;
                            if (waitCount == 2)
                            {

                                if (EEPROM_ready == false)
                                {
                                    EEPROM_ready = true;
                                    okStop = true;
                                }

                                waitCount = 0;
                            }
                        }

                    }

                    if (bufString.Last() == '\n')
                    {
                        //linesToProcess.Add(lines.Last());
                        bufString = "";
                    }
                    else
                    {
                        bufString = lines.Last();
                    }

                }
            });



        }
        private void textBox_console_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (checkBox_autoscroll.IsChecked == true)
            {
                textBox_console.ScrollToEnd();
            }
        }
        public void button_send_Click(object sender, RoutedEventArgs e)
        {
            string ToSend = textBox_command.Text;
            Console.WriteLine(ToSend.Replace('\n', '$'));
            myPort.WriteLine(ToSend);
        }
        private void textBox_command_GotFocus(object sender, RoutedEventArgs e)
        {
            if (textBox_command.Text == "Command")
            {
                textBox_command.Text = "";
            }
        }
        private void button_G36Procedure_Click(object sender, RoutedEventArgs e)
        {
            /*
            int safetyG36=0; 
            int safetyG36Treshhold=100; 

            send G36 S
            
            send "relative" 
            while "checkprobe!=true"
                send "Z-0.1 F(legit)"
                send "check probe"
                safetyG36++;
                if safetyG36==safetyTreshhold then break
            
            safetyG36=0;                
            send "absolute"
            send G36 X
            
            send "relative" 
            while "checkprobe!=true"
                send "Z-0.1 F(legit)"
                send "check probe"
                safetyG36++;
                if safetyG36==safetyTreshhold then break
            
            safetyG36=0;
            send "absolute"
            send G36 Y

            send "relative" 
            while "checkprobe!=true"
                send "Z-0.1 F(legit)"
                send "check probe"
                safetyG36++;
                if safetyG36==safetyTreshhold then break
            
            safetyG36=0;
            send "absolute"
            send G36 Z

             */
        }
        public void button_populateEEPROMList_Click(object sender, RoutedEventArgs e)
        {
            textBox_debug.Clear();

            for (int i = 0; i < EEPROMList.Count - 1; i++)
            {
                textBox_debug.AppendText(EEPROMList[i] + '\n');
            }

            myPort.WriteLine("G28");
        }
        public void button_Process_Click(object sender, RoutedEventArgs e)
        {


            textBox_debug.Clear();
            List<string> lines = new List<string>(linesToProcess);
            int lineCount = lines.Count;
            textBox_debug.AppendText(String.Format("LinesReceived: {0}\n", lines.Count));
            for (int i = 0; i < lineCount - 1; i++)
            {
                textBox_debug.AppendText(lines[0] + '\n');
                if (lines[0].Contains("Printer ID"))
                {

                    for (int j = 0; j < 200; j++)
                    {
                        EEPROMList.Add(lines[j]);
                        if (lines[j].Contains("EPR") != true)
                        {
                            break;
                        }
                    }


                }
                lines.RemoveAt(0);
            }

            myPort.WriteLine("G28");

        }
        private void enterCommand(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string ToSend = textBox_command.Text;
                myPort.WriteLine(ToSend);
                textBox_command.Text = "";
                //enter key is down
            }
        }
        private void button_cancelRoutine_Click(object sender, RoutedEventArgs e)
        {
            myPort.WriteLine("G28");
            routineTimer.Enabled = false;
            button_gridTest_Universal.IsEnabled = true;
            processOrigin = -1;
            routineCounter = 0;
            if (commandLoopType == 3)
            {
                processProbeMeasurements();
            }
            commandLoopType = -1;
            textBox_console.AppendText("Routine canceled.\n");
            calibratePhase = -1;
        }
        private void tabControl_Vadiba_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (tabControl_Vadiba.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    if (DMeasureComplete == true)
                    {

                    }
                    break;
                default:
                    break;

            }

        }
        /// <summary>
        /// Testa funkcijas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_readPosition_Click(object sender, RoutedEventArgs e)
        {

            myPort.WriteLine("m114");

        }
        private void button_autoLevel_Click(object sender, RoutedEventArgs e)
        {

            if (Autolvl == false)
            {
                button_autoLevel.Content = "autolvlDisable";
                myPort.WriteLine("m320");
                Autolvl = true;
            }
            else
            {
                button_autoLevel.Content = "autolvlEnable";
                myPort.WriteLine("m321");
                Autolvl = false;
            }
        }
        private void button_G37_Click(object sender, RoutedEventArgs e)
        {

            myPort.WriteLine("g37");

        }
        /// <summary>
        /// Procedūru palaišanas un to parametri
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_g37Probe_Click(object sender, RoutedEventArgs e)
        {
            routineTimer.Interval = 2000;
            routineTimer.Enabled = true;
            processOrigin = 0;
            commandLoopType = 0;
            //initialize
            position = 0;
            routineCounter = 0;
            stepType = 0;
            stepSize = coarseStep;

        }
        private void button_G37_diveMeasure_Click(object sender, RoutedEventArgs e)
        {
            routineTimer.Interval = 2000;
            routineTimer.Enabled = true;
            processOrigin = 1;
            commandLoopType = 1;
            //initialize
            position = 0;
            routineCounter = 0;
            stepType = 0;
            stepSize = coarseStep;
            DMeasureComplete = false;
        }
        private void button_probeTest_Click(object sender, RoutedEventArgs e)
        {
            routineTimer.Interval = 2000;
            routineTimer.Enabled = true;
            processOrigin = 3;
            commandLoopType = 3;
            //initialize
            position = 0;
            routineCounter = 0;
            probeMeasureCounter = 0;
            probeMeasurementsList.Clear();
            textBox_probeMeasurements.Clear();
            canvas_probeMeasure.Children.Clear();

            label_measureCounter.Content = "0";
        }
        private void button_gridTestSimple_Click(object sender, RoutedEventArgs e)
        {
            routineTimer.Interval = 2000;
            routineTimer.Enabled = true;
            button_gridTestSimple.IsEnabled = false;
            processOrigin = 5;
            commandLoopType = 5;
            //initialize
            position = 0;
            routineCounter = 0;
            probeMeasureCounter = 0;
            disposableGridMeasureGcode = new List<string>(gridMeasureGcode);
            label_measureCounter.Content = "0";
            gridMeasureSimpleResults.Clear();
        }
        private void button_gridTestRandom_Click(object sender, RoutedEventArgs e)
        {
            routineTimer.Interval = 2000;
            routineTimer.Enabled = true;
            button_gridTestSimple.IsEnabled = false;
            processOrigin = 8;
            commandLoopType = 8;
            //initialize
            position = 0;
            routineCounter = 0;
            probeMeasureCounter = 0;
            disposableGridMeasureGcode = new List<string>(gridMeasureGcode);
            label_measureCounter.Content = "0";
            gridMeasureRandomResults.Clear();
        }
        public void button_gridTest_Universal_Click(object sender, RoutedEventArgs e)
        {
            routineTimer.Interval = 2000;
            routineTimer.Enabled = true;
            button_gridTest_Universal.IsEnabled = false;
            processOrigin = 10;
            commandLoopType = 10;
            //initialize
            position = 0;
            routineCounter = 0;
            probeMeasureCounter = 0;
            universalPointCounter = 0;
            label_measureCounter.Content = "0";
            gridMeasureUniversal.Clear();
            //gridArray = null;
        }
        public void button_planeCalibrationRoutine_Click_1(object sender, RoutedEventArgs e)
        {
            routineTimer.Interval = 2000;
            routineTimer.Enabled = true;
            button_gridTest_Universal.IsEnabled = false;
            processOrigin = 12;
            commandLoopType = 12;
            //initialize
            position = 0;
            routineCounter = 0;
            probeMeasureCounter = 0;
            universalPointCounter = 0;
            calibratePhase = 0;
            label_measureCounter.Content = "0";
            gridMeasureUniversal.Clear();
        }
        /// <summary>
        /// Procedūru taimeris
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void TimedClicking(object source, ElapsedEventArgs e)
        {

            this.Dispatcher.Invoke(() =>
            {
                textBox_console.AppendText("TimerClick.\n");
                label_commandLoopType_value.Content = Convert.ToString(commandLoopType);
                label_EEPROM_ready_value.Content = Convert.ToString(EEPROM_ready);

                if (okStop == true)
                {
                    switch (commandLoopType)
                    {
                        case 0:
                            descendTest();
                            break;
                        case 1:
                            diveMeasure();
                            break;
                        case 2:
                            processDM();
                            break;
                        case 3:
                            probeTest();
                            break;
                        case 4:
                            processProbeMeasurements();
                            break;
                        case 5:
                            gridTestSimple();
                            break;
                        case 6:
                            gridTestProcessSimple();
                            break;
                        case 7:
                            if (EEPROM_ready == true)
                            {
                                EEPROMReadValues();
                            }
                            break;
                        case 8:
                            localRandomGridTest();
                            break;
                        case 9:
                            processRandomGridMeasure();
                            break;
                        case 10:
                            universalGridMeasure();
                            break;
                        case 11:
                            processUniversalGridMeasure();
                            break;
                        case 12:
                            planeCalibration();
                            break;
                        default:
                            textBox_console.AppendText("Command type Error.\n");
                            break;
                    }
                    okStop = true;

                }


            });
        }
        /// <summary>
        /// Mehāniskā automatizācija
        /// </summary>
        public void descendTest()
        {
            label_routinePosition.Content = "Rout:" + Convert.ToString(routineCounter);
            label_positionPosition.Content = "Pos:" + Convert.ToString(position);
            textBox_console.AppendText("Descend test.\n");
            if (zProbeState == true)
            {
                if (stepType == 0)
                {
                    routineCounter = 6;
                }
                if (stepType == 1)
                {
                    routineCounter = 7;
                }


                myPort.WriteLine("m114");
                readProbePosition = true;
                zProbeState = false;
            }
            else
            {
                ///Coarse stepping
                if (stepType == 0)
                {

                    switch (routineCounter)
                    {
                        case 0:
                            myPort.WriteLine("g28");
                            textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                            routineTimer.Interval = 1000;
                            routineCounter++;
                            break;
                        case 1:
                            myPort.WriteLine("g37");
                            textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                            routineTimer.Interval = 5000;
                            routineCounter++;
                            break;
                        case 2:
                            myPort.WriteLine("m119");
                            textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                            routineTimer.Interval = 40;
                            routineCounter++;
                            break;
                        case 3:
                            myPort.WriteLine("g91");
                            textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                            routineTimer.Interval = 20;
                            routineCounter++;
                            break;
                        case 4:
                            myPort.WriteLine("G1 Z-" + stepSize + " F2000");
                            textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                            routineTimer.Interval = 20;
                            routineCounter++;
                            break;
                        case 5:
                            myPort.WriteLine("G90");
                            textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                            routineTimer.Interval = 20;
                            routineCounter = 2;
                            break;
                        case 6:
                            ///
                            descendTestPosition();

                            if (position != 6)
                            {
                                myPort.WriteLine("G37");
                            }
                            position++;
                            textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                            myPort.WriteLine("m114");
                            routineTimer.Interval = 2000;
                            routineCounter = 2;
                            break;
                        default:
                            textBox_console.AppendText("Routine Error");
                            break;
                    }
                }
                //Fine stepping
                if (stepType == 1)
                {
                    switch (routineCounter)
                    {
                        case 0:
                            position = 0;
                            myPort.WriteLine("g28");
                            textBox_console.AppendText(infoText_fineRoutineCase + ": " + Convert.ToString(routineCounter) + "\n");
                            routineTimer.Interval = 1000;
                            routineCounter++;
                            break;
                        case 1:
                            myPort.WriteLine("g37");
                            textBox_console.AppendText(infoText_fineRoutineCase + ": " + Convert.ToString(routineCounter) + "\n");
                            routineTimer.Interval = 4000;
                            routineCounter++;
                            break;
                        case 2:
                            myPort.WriteLine("G90");
                            myPort.WriteLine("G1 Z1 F2000");
                            textBox_console.AppendText(infoText_fineRoutineCase + ": " + Convert.ToString(routineCounter) + "\n");
                            routineTimer.Interval = 1000;
                            routineCounter++;
                            break;
                        case 3:
                            myPort.WriteLine("m119");
                            textBox_console.AppendText(infoText_fineRoutineCase + ": " + Convert.ToString(routineCounter) + "\n");
                            routineTimer.Interval = 40;
                            routineCounter++;
                            break;
                        case 4:
                            myPort.WriteLine("g91");
                            textBox_console.AppendText(infoText_fineRoutineCase + ": " + Convert.ToString(routineCounter) + "\n");
                            routineTimer.Interval = 20;
                            routineCounter++;
                            break;
                        case 5:
                            myPort.WriteLine("G1 Z-" + stepSize + " F2000");
                            textBox_console.AppendText(infoText_fineRoutineCase + ": " + Convert.ToString(routineCounter) + "\n");
                            routineTimer.Interval = 20;
                            routineCounter++;
                            break;
                        case 6:
                            myPort.WriteLine("G90");
                            textBox_console.AppendText(infoText_fineRoutineCase + ": " + Convert.ToString(routineCounter) + "\n");
                            routineTimer.Interval = 20;
                            routineCounter = 3;
                            break;
                        case 7:
                            ///
                            descendTestPosition();
                            if (position != 6)
                            {
                                myPort.WriteLine("G37");
                            }
                            position++;
                            textBox_console.AppendText(infoText_fineRoutineCase + ": " + Convert.ToString(routineCounter) + "\n");
                            myPort.WriteLine("m114");
                            routineTimer.Interval = 2000;
                            routineCounter = 2;
                            if (position == 7)
                            {
                                routineTimer.Enabled = false;
                                myPort.WriteLine("G28");
                                textBox_console.AppendText("Done.\n");
                            }
                            break;

                        /*

                        myPort.WriteLine("m114");
                        routineTimer.Interval = 2000;
                        routineCounter = 2;
                        break;    
                     */

                        default:
                            textBox_console.AppendText("Routine Error");
                            break;
                    }
                }
            }
        }
        public void descendTestPosition()
        {

            string[] positionLines = positionString.Split(' ');
            foreach (string stringLine in positionLines)
            {
                if (stringLine.Contains("Z:"))
                {
                    string modStringLine = stringLine.Replace("Z:", "");

                    switch (position)
                    {
                        case 0:
                            positionArray[position] = modStringLine;
                            label_position0.Content = positionArray[position];
                            textBox_console.AppendText(infoText_positionCase + Convert.ToString(position) + "\n");
                            break;
                        case 1:
                            positionArray[position] = modStringLine;
                            label_position1.Content = positionArray[position];
                            textBox_console.AppendText(infoText_positionCase + Convert.ToString(position) + "\n");
                            break;
                        case 2:
                            positionArray[position] = modStringLine;
                            label_position2.Content = positionArray[position];
                            textBox_console.AppendText(infoText_positionCase + Convert.ToString(position) + "\n");
                            break;
                        case 3:
                            positionArray[position] = modStringLine;
                            label_position3.Content = positionArray[position];
                            textBox_console.AppendText(infoText_positionCase + Convert.ToString(position) + "\n");
                            break;
                        case 4:
                            positionArray[position] = modStringLine;
                            label_position4.Content = positionArray[position];
                            textBox_console.AppendText(infoText_positionCase + Convert.ToString(position) + "\n");
                            break;
                        case 5:
                            positionArray[position] = modStringLine;
                            label_position5.Content = positionArray[position];
                            textBox_console.AppendText(infoText_positionCase + Convert.ToString(position) + "\n");
                            break;
                        case 6:
                            positionArray[position] = modStringLine;
                            label_position6.Content = positionArray[position];
                            textBox_console.AppendText(infoText_positionCase + Convert.ToString(position) + "\n");
                            break;
                        case 7:
                            textBox_console.AppendText(infoText_positionCase + Convert.ToString(position) + "\n");
                            stepType = 1;
                            stepSize = fineStep;
                            routineCounter = 0;
                            position = -1;
                            break;

                        default:
                            textBox_console.AppendText("Position Error");
                            break;
                    }
                }
            }


        }
        public void diveMeasure()
        {
            label_routinePosition.Content = "Rout:" + Convert.ToString(routineCounter);
            label_positionPosition.Content = "Pos:" + Convert.ToString(position);
            textBox_console.AppendText("Dive measure.\n");

            if (diveMeasureComplete == true)
            {
                //
                routineCounter = 4;
                diveMeasureComplete = false;
            }
            else
            {
                switch (routineCounter)
                {
                    case 0:
                        myPort.WriteLine("g28");
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                        routineTimer.Interval = 1000;
                        routineCounter++;
                        break;
                    case 1:
                        myPort.WriteLine("g37");
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                        //routineTimer.Interval = 5000;
                        routineCounter++;
                        break;
                    case 2:
                        //
                        myPort.WriteLine("G38 P5");
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                        routineTimer.Interval = 50;
                        routineCounter++;
                        break;
                    case 3:

                        break;
                    case 4:
                        ///
                        diveMeasurePosition();

                        if (position != 6)
                        {
                            myPort.WriteLine("G37");
                        }
                        else
                        {
                            myPort.WriteLine("G28");
                            myPort.WriteLine("M205");
                            EEPROM_ready = false;
                            routineTimer.Interval = 5000;
                            commandLoopType = 2;

                        }
                        position++;
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");

                        routineTimer.Interval = 2000;
                        routineCounter = 2;
                        break;
                    default:
                        textBox_console.AppendText("Routine Error");
                        break;
                }
            }

        }
        public void diveMeasurePosition()
        {
            string[] positionLines = positionString.Split(' ');
            foreach (string stringLine in positionLines)
            {
                if (stringLine.Contains("Z:"))
                {
                    string modStringLine = stringLine.Replace("Z:", "");

                    switch (position)
                    {
                        case 0:
                            positionArray[position] = modStringLine;
                            label_position0.Content = positionArray[position];
                            textBox_console.AppendText(infoText_positionCase + Convert.ToString(position) + "\n");
                            break;
                        case 1:
                            positionArray[position] = modStringLine;
                            label_position1.Content = positionArray[position];
                            textBox_console.AppendText(infoText_positionCase + Convert.ToString(position) + "\n");
                            break;
                        case 2:
                            positionArray[position] = modStringLine;
                            label_position2.Content = positionArray[position];
                            textBox_console.AppendText(infoText_positionCase + Convert.ToString(position) + "\n");
                            break;
                        case 3:
                            positionArray[position] = modStringLine;
                            label_position3.Content = positionArray[position];
                            textBox_console.AppendText(infoText_positionCase + Convert.ToString(position) + "\n");
                            break;
                        case 4:
                            positionArray[position] = modStringLine;
                            label_position4.Content = positionArray[position];
                            textBox_console.AppendText(infoText_positionCase + Convert.ToString(position) + "\n");
                            break;
                        case 5:
                            positionArray[position] = modStringLine;
                            label_position5.Content = positionArray[position];
                            textBox_console.AppendText(infoText_positionCase + Convert.ToString(position) + "\n");
                            break;
                        case 6:
                            positionArray[position] = modStringLine;
                            label_position6.Content = positionArray[position];
                            textBox_console.AppendText(infoText_positionCase + Convert.ToString(position) + "\n");
                            break;

                        default:
                            textBox_console.AppendText("Position Error");
                            break;
                    }
                }
            }

        }
        public void processDM()
        {

            List<string> lines = new List<string>(linesToProcess);
            int lineCount = lines.Count;
            textBox_debug.AppendText(String.Format("LinesReceived: {0}\n", lines.Count));
            for (int i = 0; i < lineCount - 1; i++)
            {

                textBox_debug.AppendText(lines[0] + '\n');
                if (lines[0].Contains("Printer ID"))
                {
                    for (int j = 0; j < lines.Count; j++)
                    {
                        EEPROMList.Add(lines[j]);
                        if (lines[j].Contains("EPR") != true)
                        {
                            routineTimer.Enabled = false;
                            break;
                        }
                        if (j == lines.Count)
                        {

                            break;
                        }
                        routineTimer.Enabled = false;
                    }
                }

                lines.RemoveAt(0);
            }
            textBox_debug.Clear();

            for (int i = 0; i < EEPROMList.Count - 1; i++)
            {
                textBox_debug.AppendText(EEPROMList[i] + '\n');
            }

            EEPROM_ready = false;
            if (routineTimer.Enabled == false)
            {
                myPort.WriteLine("G28");
                foreach (string EEPROMLine in EEPROMList)
                {
                    if (EEPROMLine.Contains("Z-probe height"))
                    {
                        string[] stringSplit = EEPROMLine.Split(' ');
                        label_z_probeHeight_value.Content = Convert.ToString(z_probeHeight);
                        z_probeHeight = stringSplit[2];
                    }
                    if (EEPROMLine.Contains("bed dist"))
                    {
                        string[] stringSplit = EEPROMLine.Split(' ');
                        bedDistance = stringSplit[2];
                    }
                }


                for (int i = 0; i < 7; i++)
                {
                    switch (i)
                    {
                        case 0:
                            positionArray[i] = Convert.ToString(Math.Truncate((Convert.ToSingle(bedDistance) - Convert.ToSingle(z_probeHeight) - Convert.ToSingle(positionArray[i])) * 1000) / 1000);
                            label_position0.Content = positionArray[i];
                            break;
                        case 1:
                            positionArray[i] = Convert.ToString(Math.Truncate((clearance - Convert.ToSingle(z_probeHeight) - Convert.ToSingle(positionArray[i])) * 1000) / 1000);
                            label_position1.Content = positionArray[i];
                            break;
                        case 2:
                            positionArray[i] = Convert.ToString(Math.Truncate((clearance - Convert.ToSingle(z_probeHeight) - Convert.ToSingle(positionArray[i])) * 1000) / 1000);
                            label_position2.Content = positionArray[i];
                            break;

                        default:
                            textBox_console.AppendText("EEPROM process error");
                            break;
                    }

                }
                DMeasureComplete = true;
            }

        }
        public void gridTestSimple()
        {

            label_routinePosition.Content = "Rout:" + Convert.ToString(routineCounter);
            label_positionPosition.Content = "Pos:" + Convert.ToString(position);
            textBox_console.AppendText("Grid test measure (Simple).\n");

            if (diveMeasureComplete == true)
            {
                //
                routineCounter = 4;
                diveMeasureComplete = false;
            }
            else
            {
                switch (routineCounter)
                {
                    case 0:
                        myPort.WriteLine("g28");
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                        routineTimer.Interval = 1000;
                        routineCounter++;
                        break;
                    case 1:
                        myPort.WriteLine(disposableGridMeasureGcode[0]);
                        if (disposableGridMeasureGcode.Count != 0)
                        {
                            disposableGridMeasureGcode.RemoveAt(0);
                        }

                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                        //routineTimer.Interval = 5000;
                        routineCounter++;
                        break;
                    case 2:
                        //
                        myPort.WriteLine("G38 P5");
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                        routineTimer.Interval = 50;
                        routineCounter++;
                        break;
                    case 3:

                        break;
                    case 4:
                        ///
                        gridTestMeasure();

                        if (disposableGridMeasureGcode.Count == 0)
                        {
                            myPort.WriteLine("M205");
                            myPort.WriteLine("G28");

                            EEPROM_ready = false;
                            routineTimer.Interval = 5000;
                            commandLoopType = 7;
                        }
                        position++;
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");

                        routineTimer.Interval = 200;
                        if (checkBox_measureAfterHomeSimple.IsChecked == true)
                        {
                            routineCounter = 0;
                        }
                        else
                        {
                            routineCounter = 1;
                        }

                        break;
                    default:
                        textBox_console.AppendText("Routine Error");
                        break;
                }
            }

        }
        public void gridTestMeasure()
        {
            string[] positionLines = positionString.Split(' ');
            foreach (string stringLine in positionLines)
            {
                if (stringLine.Contains("Z:"))
                {
                    string modStringLine = stringLine.Replace("Z:", "");
                    gridMeasureSimpleResults.Add(Convert.ToSingle(modStringLine));
                }
            }

        }
        public void gridTestProcessSimple()
        {
            float xPosGcode = 0;
            float yPosGcode = 0;



            routineTimer.Enabled = false;
            textBox_debugGridPointList.Clear();
            textBox_debugGridPointList.AppendText("Grid measure. " + "Count:" + Convert.ToString(gridMeasureGcode.Count) + " Z_Probe_Height:" + z_probeHeight + "\r\n");
            disposableMeasureList = new List<float>(gridMeasureSimpleResults);
            wholeGridSize = Convert.ToInt32(textBox_printingDiameter.Text) / Convert.ToInt32(textBox_gridSize.Text) - 1;

            foreach (string gcode in gridMeasureGcode)
            {

                textBox_debugGridPointList.AppendText(gcode + " Measure: " + Convert.ToString(clearance - disposableMeasureList[0] - Convert.ToSingle(z_probeHeight)) + "\n");

                if (disposableMeasureList.Count != 0)
                {
                    disposableMeasureList.RemoveAt(0);
                }

            }
            disposableMeasureList = new List<float>(gridMeasureSimpleResults);
            foreach (string gcode in gridMeasureGcode)
            {
                string[] gcodeSplit = gcode.Split(' ');
                xPosGcode = Convert.ToSingle(gcodeSplit[1].Replace("X", ""));
                yPosGcode = Convert.ToSingle(gcodeSplit[2].Replace("Y", ""));
                if (Single.IsNaN(disposableMeasureList[0]))
                {
                    gridTestSimpleResults.Add(new Tuple<float, float, float>(xPosGcode, yPosGcode, 0));
                }
                else
                {
                    if (checkBox_offsetXYCorrectionSimple.IsChecked == true)
                    {
                        gridTestSimpleResults.Add(new Tuple<float, float, float>(xPosGcode, yPosGcode, clearance - XYoffset - disposableMeasureList[0] - Convert.ToSingle(z_probeHeight)));
                    }
                    else
                    {
                        gridTestSimpleResults.Add(new Tuple<float, float, float>(xPosGcode, yPosGcode, clearance - disposableMeasureList[0] - Convert.ToSingle(z_probeHeight)));

                    }
                }

                //textBox_debugGridPointList.AppendText(gcode + " Measure: " + Convert.ToString(5 - disposableMeasureList[0] - Convert.ToSingle(z_probeHeight)) + "\n");

                if (disposableMeasureList.Count != 0)
                {
                    disposableMeasureList.RemoveAt(0);
                }

            }

            for (float x = ((Convert.ToSingle(wholeGridSize) * Convert.ToSingle(textBox_gridSize.Text)) / 2f) + (Convert.ToSingle(textBox_gridSize.Text) / 2f); x > 0f - (((Convert.ToSingle(wholeGridSize) * Convert.ToSingle(textBox_gridSize.Text)) / 2f) + Convert.ToSingle(textBox_gridSize.Text) + (Convert.ToSingle(textBox_gridSize.Text) / 2f) / 2f); x = x - Convert.ToSingle(textBox_gridSize.Text))
            {
                for (float y = ((Convert.ToSingle(wholeGridSize) * Convert.ToSingle(textBox_gridSize.Text)) / 2f) + (Convert.ToSingle(textBox_gridSize.Text) / 2f); y > 0f - (((Convert.ToSingle(wholeGridSize) * Convert.ToSingle(textBox_gridSize.Text)) / 2f) + Convert.ToSingle(textBox_gridSize.Text) + (Convert.ToSingle(textBox_gridSize.Text) / 2f) / 2f); y = y - Convert.ToSingle(textBox_gridSize.Text))
                {
                    addBlank = true;
                    foreach (Tuple<float, float, float> result in gridTestSimpleResults)
                    {
                        if (result.Item1 == x && result.Item2 == y)
                        {
                            gridTestSimpleCompleteGrid.Add(result);
                            addBlank = false;
                        }
                    }
                    if (addBlank == true)
                    {
                        gridTestSimpleCompleteGrid.Add(new Tuple<float, float, float>(x, y, 0f));
                    }
                }
            }
            textBox_debugGridPointList.Clear();
            foreach (Tuple<float, float, float> result in gridTestSimpleCompleteGrid)
            {
                textBox_debugGridPointList.AppendText("X" + Convert.ToString(result.Item1) + " " + "Y" + Convert.ToString(result.Item2) + " " + "M" + Convert.ToString(result.Item3) + "\r\n");
            }
            gridTestProcessSimpleVisualisation();
        }
        public void gridTestProcessSimpleVisualisation()
        {
            multiplierColorScale = Convert.ToInt32(textBox_multiplierColorScaleSimple.Text);
            squareToGridScale = Convert.ToSingle(canvas_gridMeasureSimple.Height / (Convert.ToDouble(textBox_printingDiameter.Text) + Convert.ToDouble(textBox_gridSize.Text)));
            canvas_gridMeasureSimple.Children.Clear();
            foreach (Tuple<float, float, float> square in gridTestSimpleCompleteGrid)
            {
                int color = Convert.ToInt32(square.Item3 * 1000f);
                gridSquare = new System.Windows.Shapes.Rectangle();
                gridSquare.Stroke = new SolidColorBrush(Colors.Black);
                gridSquare.StrokeThickness = 0;
                gridSquare.ToolTip = Convert.ToString(square.Item3);
                ////greenish
                if ((color > Convert.ToSingle(-10) && color < Convert.ToSingle(10)) || color == Convert.ToSingle(-10) || color == Convert.ToSingle(10))
                {
                    gridSquare.Fill = new SolidColorBrush(Color.FromArgb(100, 127, 255, 127));
                }
                ////redish
                if (color > 1)
                {
                    if (color * multiplierColorScale / 10 > 255 || color * multiplierColorScale / 10 == 255)
                    {
                        gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                    }
                    if (color * multiplierColorScale / 10 > 0 && color * multiplierColorScale / 10 < 255)
                    {
                        gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 255, Convert.ToByte(255 - (color * multiplierColorScale / 10)), Convert.ToByte(255 - (color * multiplierColorScale / 10))));
                    }
                }
                ////blueish
                if (color < -1)
                {
                    if (color * multiplierColorScale / 10 < -255 || color * multiplierColorScale / 10 == -255)
                    {
                        gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
                    }
                    if (color * multiplierColorScale / 10 < 0 && color * multiplierColorScale / 10 > -255)
                    {
                        gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, Convert.ToByte(255 - (color * (0 - multiplierColorScale) / 10)), Convert.ToByte(255 - (color * (0 - multiplierColorScale) / 10)), 255));
                    }
                }
                gridSquare.Width = Convert.ToDouble(textBox_gridSize.Text) * Convert.ToDouble(squareToGridScale);
                gridSquare.Height = Convert.ToDouble(textBox_gridSize.Text) * Convert.ToDouble(squareToGridScale);
                Canvas.SetLeft(gridSquare, (square.Item1 + (Convert.ToDouble(textBox_printingDiameter.Text) / 2)) * Convert.ToDouble(squareToGridScale));
                Canvas.SetTop(gridSquare, ((Convert.ToDouble(textBox_printingDiameter.Text) - (square.Item2 + (Convert.ToDouble(textBox_printingDiameter.Text) / 2))) * Convert.ToDouble(squareToGridScale)));
                canvas_gridMeasureSimple.Children.Add(gridSquare);
                if (checkBox_measureLabelsSimple.IsChecked == true)
                {
                    TextBlock textBlock_labels = new TextBlock();
                    textBlock_labels.Text = Convert.ToString(Convert.ToString(Math.Truncate(Convert.ToDouble(square.Item3) * 100) / 100));
                    textBlock_labels.Foreground = Brushes.Black;
                    textBlock_labels.FontWeight = FontWeights.UltraBold;
                    textBlock_labels.FontSize = 14;
                    Canvas.SetLeft(textBlock_labels, (square.Item1 + (Convert.ToDouble(textBox_printingDiameter.Text) / 2)) * Convert.ToDouble(squareToGridScale));
                    Canvas.SetTop(textBlock_labels, ((Convert.ToDouble(textBox_printingDiameter.Text) - (square.Item2 + (Convert.ToDouble(textBox_printingDiameter.Text) / 2))) * Convert.ToDouble(squareToGridScale)));
                    canvas_gridMeasureSimple.Children.Add(textBlock_labels);
                }

            }
            //canvas_gridMeasureSimple.Height;

        }
        public void probeTest()
        {
            label_routinePosition.Content = "Rout:" + Convert.ToString(routineCounter);
            label_positionPosition.Content = "Pos:" + Convert.ToString(position);
            textBox_console.AppendText("Probe test.\n");

            if (diveMeasureComplete == true)
            {
                //
                routineCounter = 4;
                diveMeasureComplete = false;
            }
            else
            {
                switch (routineCounter)
                {
                    case 0:
                        myPort.WriteLine("g28");
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                        routineTimer.Interval = 1000;
                        routineCounter++;
                        break;
                    case 1:
                        myPort.WriteLine("g1" + " " + "X" + textBox_probeXPosition.Text + " " + "Y" + textBox_probeYPosition.Text + "z" + clearance);
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                        //routineTimer.Interval = 5000;
                        routineCounter++;
                        break;
                    case 2:
                        //
                        myPort.WriteLine("G38 P5");
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                        routineTimer.Interval = 50;
                        routineCounter++;
                        break;
                    case 3:

                        break;
                    case 4:
                        ///
                        probeTestMeasure();

                        if (position == Convert.ToInt32(textBox_probeMeasureCount.Text) - 1)
                        {
                            myPort.WriteLine("G28");
                            commandLoopType = 4;

                        }
                        probeMeasureCounter++;
                        label_measureCounter.Content = Convert.ToString(probeMeasureCounter);
                        position++;
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                        routineCounter = 2;
                        okStop = true;
                        break;
                    default:
                        textBox_console.AppendText("Routine Error");
                        break;
                }
            }

        }
        public void probeTestMeasure()
        {
            string[] positionLines = positionString.Split(' ');
            foreach (string stringLine in positionLines)
            {
                if (stringLine.Contains("Z:"))
                {
                    string modStringLine = stringLine.Replace("Z:", "");

                    probeMeasurementsList.Add(Convert.ToSingle(modStringLine));
                    textBox_probeMeasurements.AppendText(modStringLine + "\n");

                }
            }

        }
        public void localRandomGridTest()
        {
            label_routinePosition.Content = "Rout:" + Convert.ToString(routineCounter);
            label_positionPosition.Content = "Pos:" + Convert.ToString(position);
            textBox_console.AppendText("Grid test measure (Random).\n");

            if (diveMeasureComplete == true)
            {
                //
                routineCounter = 4;
                diveMeasureComplete = false;
            }
            else
            {
                switch (routineCounter)
                {
                    case 0:
                        myPort.WriteLine("g28");
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                        routineTimer.Interval = 1000;
                        routineCounter++;
                        break;
                    case 1:
                        if (disposableGridMeasureGcode[0].Contains("GridSquareEnd"))
                        {
                            randomSquareComplete = true;
                            disposableGridMeasureGcode.RemoveAt(0);
                            foreach (float randomMeasurePoint in gridMeasureRandom)
                            {
                                localRandomPointCounter++;
                                localRandomSum = localRandomSum + randomMeasurePoint;
                            }
                            if (disposableGridMeasureGcode[0].Contains("Measure"))
                            {
                                string[] gridSquarePosition = disposableGridMeasureGcode[0].Split(' ');
                                gridTestRandomResults.Add(new Tuple<float, float, float>(Convert.ToSingle(gridSquarePosition[1]), Convert.ToSingle(gridSquarePosition[2]), localRandomSum / localRandomPointCounter));
                            }
                            disposableGridMeasureGcode.RemoveAt(0);
                            if (disposableGridMeasureGcode[0].Contains("GCodeEnd"))
                            {
                                routineCounter = 4;
                            }
                            localRandomPointCounter = 0;
                            localRandomSum = 0;
                            gridMeasureRandom.Clear();
                        }
                        else
                        {
                            myPort.WriteLine(disposableGridMeasureGcode[0]);
                            if (disposableGridMeasureGcode.Count != 0)
                            {
                                disposableGridMeasureGcode.RemoveAt(0);
                            }
                        }



                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                        //routineTimer.Interval = 5000;
                        if (randomSquareComplete == false)
                        {
                            routineCounter++;
                        }
                        else
                        {
                            routineCounter = 1;
                            randomSquareComplete = false;
                        }
                        break;
                    case 2:
                        //
                        myPort.WriteLine("G38 P5");
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                        routineTimer.Interval = 50;
                        routineCounter++;
                        break;
                    case 3:

                        break;
                    case 4:
                        ///
                        gridTestRandomMeasure();

                        if (disposableGridMeasureGcode.Count == 0)
                        {
                            myPort.WriteLine("M205");
                            myPort.WriteLine("G28");

                            EEPROM_ready = false;
                            routineTimer.Interval = 5000;
                            commandLoopType = 7;
                        }
                        position++;
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");

                        routineTimer.Interval = 200;
                        if (checkBox_measureAfterHomingRandom.IsChecked == true)
                        {
                            routineCounter = 0;
                        }
                        else
                        {
                            routineCounter = 1;
                        }
                        break;
                    default:
                        textBox_console.AppendText("Routine Error");
                        break;
                }
            }
        }
        public void gridTestRandomMeasure()
        {
            string[] positionLines = positionString.Split(' ');
            foreach (string stringLine in positionLines)
            {
                if (stringLine.Contains("Z:"))
                {
                    string modStringLine = stringLine.Replace("Z:", "");
                    gridMeasureRandom.Add(Convert.ToSingle(modStringLine));
                }
            }
        }
        public void processRandomGridMeasure()
        {
            float xPosGcode = 0;
            float yPosGcode = 0;
            gridMeasureRandomResults.Clear();
            foreach (Tuple<float, float, float> measureLineRandom in gridTestRandomResults)
            {
                gridMeasureRandomResults.Add(measureLineRandom.Item3);
            }
            routineTimer.Enabled = false;
            textBox_debugGridPointList_Random.Clear();
            gridTestRandomResultsCorrect.Clear();
            gridTestRandomCompleteGrid.Clear();
            textBox_debugGridPointList_Random.AppendText("Grid measure. " + "Count:" + Convert.ToString(gridMeasureRandomResults.Count) + " Z_Probe_Height:" + z_probeHeight + "\r\n");

            wholeGridSize = Convert.ToInt32(textBox_printingDiameterRandom.Text) / Convert.ToInt32(textBox_gridSize_Random.Text) - 1;
            /////
            foreach (Tuple<float, float, float> coordinateMeasure in gridTestRandomResults)
            {

                textBox_debugGridPointList_Random.AppendText("X " + Convert.ToString(coordinateMeasure.Item1) + " Y " + Convert.ToString(coordinateMeasure.Item2) + " Measure: " + Convert.ToString(Convert.ToSingle(clearance) - coordinateMeasure.Item3 - Convert.ToSingle(z_probeHeight)) + "\n");

            }
            foreach (Tuple<float, float, float> coordinateMeaasureCorrect in gridTestRandomResults)
            {
                if (Single.IsNaN(coordinateMeaasureCorrect.Item3))
                {
                    gridTestRandomResultsCorrect.Add(new Tuple<float, float, float>(coordinateMeaasureCorrect.Item1, coordinateMeaasureCorrect.Item2, 0f));
                }
                else
                {
                    if (checkBox_offsetXYCorrectionRandom.IsChecked == true)
                    {
                        gridTestRandomResultsCorrect.Add(new Tuple<float, float, float>(coordinateMeaasureCorrect.Item1, coordinateMeaasureCorrect.Item2, (Convert.ToSingle(clearance) - XYoffset - coordinateMeaasureCorrect.Item3 - Convert.ToSingle(z_probeHeight))));
                    }
                    else
                    {
                        gridTestRandomResultsCorrect.Add(new Tuple<float, float, float>(coordinateMeaasureCorrect.Item1, coordinateMeaasureCorrect.Item2, (Convert.ToSingle(clearance) - coordinateMeaasureCorrect.Item3 - Convert.ToSingle(z_probeHeight))));

                    }
                }
            }

            for (float x = ((Convert.ToSingle(wholeGridSize) * Convert.ToSingle(textBox_gridSize_Random.Text)) / 2f) + (Convert.ToSingle(textBox_gridSize_Random.Text) / 2f); x > 0f - (((Convert.ToSingle(wholeGridSize) * Convert.ToSingle(textBox_gridSize_Random.Text)) / 2f) + Convert.ToSingle(textBox_gridSize_Random.Text) + (Convert.ToSingle(textBox_gridSize_Random.Text) / 2f) / 2f); x = x - Convert.ToSingle(textBox_gridSize_Random.Text))
            {
                for (float y = ((Convert.ToSingle(wholeGridSize) * Convert.ToSingle(textBox_gridSize_Random.Text)) / 2f) + (Convert.ToSingle(textBox_gridSize_Random.Text) / 2f); y > 0f - (((Convert.ToSingle(wholeGridSize) * Convert.ToSingle(textBox_gridSize_Random.Text)) / 2f) + Convert.ToSingle(textBox_gridSize_Random.Text) + (Convert.ToSingle(textBox_gridSize_Random.Text) / 2f) / 2f); y = y - Convert.ToSingle(textBox_gridSize_Random.Text))
                {
                    addBlank = true;
                    foreach (Tuple<float, float, float> result in gridTestRandomResultsCorrect)
                    {
                        if (result.Item1 == x && result.Item2 == y)
                        {
                            gridTestRandomCompleteGrid.Add(result);
                            addBlank = false;
                        }
                    }
                    if (addBlank == true)
                    {
                        gridTestRandomCompleteGrid.Add(new Tuple<float, float, float>(x, y, 0f));
                    }
                }
            }
            textBox_debugGridPointList_Random.Clear();
            foreach (Tuple<float, float, float> result in gridTestRandomCompleteGrid)
            {
                textBox_debugGridPointList_Random.AppendText("X" + Convert.ToString(result.Item1) + " " + "Y" + Convert.ToString(result.Item2) + " " + "M" + Convert.ToString(result.Item3) + "\r\n");
            }
            gridTestProcessRandomVisualisation();
        }
        public void gridTestProcessRandomVisualisation()
        {
            multiplierColorScale = Convert.ToInt32(textBox_multiplierColorScaleRandom.Text);
            squareToGridScale = Convert.ToSingle(canvas_gridMeasureRandom.Height / (Convert.ToDouble(textBox_printingDiameterRandom.Text) + Convert.ToDouble(textBox_gridSize_Random.Text)));
            canvas_gridMeasureRandom.Children.Clear();
            foreach (Tuple<float, float, float> square in gridTestRandomCompleteGrid)
            {
                int color = Convert.ToInt32(square.Item3 * 1000f);
                gridSquare = new System.Windows.Shapes.Rectangle();
                gridSquare.Stroke = new SolidColorBrush(Colors.Black);
                gridSquare.StrokeThickness = 0;
                gridSquare.ToolTip = Convert.ToString(square.Item3);
                ////greenish
                if ((color > Convert.ToSingle(-10) && color < Convert.ToSingle(10)) || color == Convert.ToSingle(-10) || color == Convert.ToSingle(10))
                {
                    gridSquare.Fill = new SolidColorBrush(Color.FromArgb(100, 127, 255, 127));
                }
                ////redish
                if (color > 1)
                {
                    if (color * multiplierColorScale / 10 > 255 || color * multiplierColorScale / 10 == 255)
                    {
                        gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                    }
                    if (color * multiplierColorScale / 10 > 0 && color * multiplierColorScale / 10 < 255)
                    {
                        gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 255, Convert.ToByte(255 - (color * multiplierColorScale / 10)), Convert.ToByte(255 - (color * multiplierColorScale / 10))));
                    }
                }
                ////blueish
                if (color < -1)
                {
                    if (color * multiplierColorScale / 10 < -255 || color * multiplierColorScale / 10 == -255)
                    {
                        gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
                    }
                    if (color * multiplierColorScale / 10 < 0 && color * multiplierColorScale / 10 > -255)
                    {
                        gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, Convert.ToByte(255 - (color * (0 - multiplierColorScale) / 10)), Convert.ToByte(255 - (color * (0 - multiplierColorScale) / 10)), 255));
                    }
                }
                gridSquare.Width = Convert.ToDouble(textBox_gridSize_Random.Text) * Convert.ToDouble(squareToGridScale);
                gridSquare.Height = Convert.ToDouble(textBox_gridSize_Random.Text) * Convert.ToDouble(squareToGridScale);
                Canvas.SetLeft(gridSquare, (square.Item1 + (Convert.ToDouble(textBox_printingDiameterRandom.Text) / 2)) * Convert.ToDouble(squareToGridScale));
                Canvas.SetTop(gridSquare, ((Convert.ToDouble(textBox_printingDiameterRandom.Text) - (square.Item2 + (Convert.ToDouble(textBox_printingDiameterRandom.Text) / 2))) * Convert.ToDouble(squareToGridScale)));
                canvas_gridMeasureRandom.Children.Add(gridSquare);
                if (checkBox_measureLabelsRandom.IsChecked == true)
                {
                    TextBlock textBlock_labels = new TextBlock();
                    textBlock_labels.Text = Convert.ToString(Convert.ToString(Math.Truncate(Convert.ToDouble(square.Item3) * 100) / 100));
                    textBlock_labels.Foreground = Brushes.Black;
                    textBlock_labels.FontWeight = FontWeights.UltraBold;
                    textBlock_labels.FontSize = 14;
                    Canvas.SetLeft(textBlock_labels, (square.Item1 + (Convert.ToDouble(textBox_printingDiameterRandom.Text) / 2)) * Convert.ToDouble(squareToGridScale));
                    Canvas.SetTop(textBlock_labels, ((Convert.ToDouble(textBox_printingDiameterRandom.Text) - (square.Item2 + (Convert.ToDouble(textBox_printingDiameterRandom.Text) / 2))) * Convert.ToDouble(squareToGridScale)));
                    canvas_gridMeasureRandom.Children.Add(textBlock_labels);
                }
            }
        }
        public void universalGridMeasure()
        {
            label_routinePosition.Content = "Rout:" + Convert.ToString(routineCounter);
            label_positionPosition.Content = "Pos:" + Convert.ToString(position);
            textBox_console.AppendText("Grid test measure (Universal).\n");

            if (diveMeasureComplete == true)
            {
                //
                routineCounter = 4;
                diveMeasureComplete = false;
            }
            else
            {
                switch (routineCounter)
                {
                    case 0:
                        myPort.WriteLine("g28");
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                        routineTimer.Interval = 1000;
                        routineCounter++;
                        break;
                    case 1:
                        if (gridTestUniversal[0].Item1.Contains("Done") == false)
                        {
                            if (gridTestUniversal[0].Item1.Contains("GridEnd") == false)
                            {
                                myPort.WriteLine(gridTestUniversal[0].Item1);
                                string[] splitPositionLine = gridTestUniversal[0].Item2.Split(' ');
                                xGridPosition = Convert.ToSingle(splitPositionLine[1]);
                                yGridPosition = Convert.ToSingle(splitPositionLine[2]);

                                Ellipse punktsEllipse = new Ellipse();
                                punktsEllipse = new Ellipse();
                                punktsEllipse.Stroke = new SolidColorBrush(Colors.Cyan);
                                //gridSquare.ToolTip = Convert.ToString(square.Item3);
                                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                                mySolidColorBrush.Color = Color.FromArgb(255, 0, 0, 0);
                                punktsEllipse.Fill = mySolidColorBrush;
                                punktsEllipse.Width = 2;
                                punktsEllipse.Height = 2;
                                Canvas.SetTop(punktsEllipse, ((Convert.ToDouble(textBox_printingDiameter_Universal.Text) / 2) - Convert.ToDouble((yGridPosition * gridSizeUniversal) - (printerSize / 2))) * Convert.ToDouble(squareToGridScale));
                                Canvas.SetLeft(punktsEllipse, ((Convert.ToDouble((xGridPosition * gridSizeUniversal) - (printerSize / 2)) + (Convert.ToDouble(textBox_printingDiameter_Universal.Text) / 2))) * Convert.ToDouble(squareToGridScale));
                                //textBox_debugGridPointList_Universal.AppendText(Convert.ToString((Convert.ToDouble((yGridPosition * gridSizeUniversal) - (printerSize / 2)) + (Convert.ToDouble(textBox_printingDiameter_Universal.Text) / 2)) * Convert.ToDouble(squareToGridScale))+ " " + Convert.ToString(((Convert.ToDouble(textBox_printingDiameter_Universal.Text) - (Convert.ToDouble((xGridPosition * gridSizeUniversal) - (printerSize / 2)) + (Convert.ToDouble(textBox_printingDiameter_Universal.Text) / 2))) * Convert.ToDouble(squareToGridScale))) + "\n");


                                canvas_gridMeasure_Universal.Children.Add(punktsEllipse);

                                gridTestUniversal.RemoveAt(0);
                                routineCounter++;
                            }
                            else
                            {
                                if (universalPointCounter != 0)
                                {
                                    foreach (float measure in gridMeasureUniversal)
                                    {
                                        universalMeasureSum = universalMeasureSum + measure;
                                    }
                                    gridTestUniversalResults.Add(new Tuple<float, float, float>(xGridPosition, yGridPosition, (universalMeasureSum / Convert.ToSingle(universalPointCounter))));
                                    universalPointCounter = 0;
                                    universalMeasureSum = 0;
                                    gridMeasureUniversal.Clear();
                                }
                                else
                                {
                                    routineTimer.Interval = 10;
                                }
                                gridTestUniversal.RemoveAt(0);

                            }

                            textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");

                        }
                        else
                        {
                            myPort.WriteLine("M205");
                            myPort.WriteLine("G28");

                            EEPROM_ready = false;
                            routineTimer.Interval = 5000;
                            commandLoopType = 7;
                        }


                        break;
                    case 2:
                        //
                        myPort.WriteLine("G38 P5");
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                        routineTimer.Interval = 50;
                        routineCounter++;
                        break;
                    case 3:

                        break;
                    case 4:
                        ///
                        gridTestUniversalMeasure();

                        if (gridTestUniversal[0].Item1.Contains("Done") == true)
                        {
                            myPort.WriteLine("M205");
                            myPort.WriteLine("G28");

                            EEPROM_ready = false;
                            routineTimer.Interval = 5000;
                            commandLoopType = 7;
                        }
                        position++;
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");

                        routineTimer.Interval = 200;
                        if (checkBox_measureAfterHoming_Universal.IsChecked == true)
                        {
                            routineCounter = 0;
                        }
                        else
                        {
                            routineCounter = 1;
                        }
                        break;
                    default:
                        textBox_console.AppendText("Routine Error");
                        break;
                }
            }
        }
        public void gridTestUniversalMeasure()
        {

            string[] positionLines = positionString.Split(' ');
            foreach (string stringLine in positionLines)
            {
                if (stringLine.Contains("Z:"))
                {
                    string modStringLine = stringLine.Replace("Z:", "");
                    gridMeasureUniversal.Add(Convert.ToSingle(modStringLine));
                }
            }
            universalPointCounter++;
        }
        public void processUniversalGridMeasure()
        {
            textBox_debugGridPointList_Universal.Clear();
            routineTimer.Enabled = false;
            foreach (Tuple<float, float, float> measure in gridTestUniversalResults)
            {
                if (checkBox_offsetXYCorrection_Universal.IsChecked == true)
                {
                    gridArray[Convert.ToInt32(measure.Item1), Convert.ToInt32(measure.Item2), 2] = Convert.ToString(Convert.ToSingle(clearance) - measure.Item3 - Convert.ToSingle(z_probeHeight) - XYoffset);
                }
                else
                {
                    gridArray[Convert.ToInt32(measure.Item1), Convert.ToInt32(measure.Item2), 2] = Convert.ToString(Convert.ToSingle(clearance) - measure.Item3 - Convert.ToSingle(z_probeHeight));
                }
                //textBox_debugGridPointList_Universal.AppendText("GX" + " " + Convert.ToString(measure.Item1) + " " + "GY" + " " + Convert.ToString(measure.Item2) + " " + "M" + " " + Convert.ToString(measure.Item3) + "\n");
            }
            for (int i = 0; i < gridArray.GetLength(0); i++)
            {
                for (int j = 0; j < gridArray.GetLength(1); j++)
                {
                    textBox_debugGridPointList_Universal.AppendText("GX" + " " + gridArray[i, j, 0] + " " + "GY" + " " + gridArray[i, j, 1] + " " + "M" + " " + gridArray[i, j, 2] + "\n");
                }
            }
            gridTestProcessUniversalVisualisation();
        }
        public void gridTestProcessUniversalVisualisation()
        {
            multiplierColorScale = Convert.ToInt32(textBox_multiplierColorScale_Universal.Text);
            squareToGridScale = Convert.ToSingle(canvas_gridMeasure_Universal.Height / (Convert.ToDouble(textBox_printingDiameter_Universal.Text) + Convert.ToDouble(textBox_gridSize_Universal.Text)));
            canvas_gridMeasure_Universal.Children.Clear();
            canvas_gridMeasureScale_Universal.Children.Clear();

            for (int i = 0; i < gridArray.GetLength(0); i++)
            {
                for (int j = 0; j < gridArray.GetLength(1); j++)
                {
                    int color = 0;

                    if (gridArray[i, j, 2].Contains("N/A") == true)
                    {
                        color = 64001;
                        gridArray[i, j, 2] = "0";
                    }
                    else
                    {
                        color = Convert.ToInt32(Convert.ToSingle(gridArray[i, j, 2]) * 1000f);
                    }

                    gridSquare = new System.Windows.Shapes.Rectangle();
                    gridSquare.Stroke = new SolidColorBrush(Colors.Black);
                    gridSquare.StrokeThickness = 0;
                    gridSquare.ToolTip = gridArray[i, j, 2];

                    ////greenish
                    if ((color > Convert.ToSingle(-10) && color < Convert.ToSingle(10)) || color == Convert.ToSingle(-10) || color == Convert.ToSingle(10))
                    {
                        gridSquare.Fill = new SolidColorBrush(Color.FromArgb(100, 127, 255, 127));
                    }
                    ////redish
                    if (color > 1)
                    {
                        if (color * multiplierColorScale / 10 > 255 || color * multiplierColorScale / 10 == 255)
                        {
                            gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                        }
                        if (color * multiplierColorScale / 10 > 0 && color * multiplierColorScale / 10 < 255)
                        {
                            gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 255, Convert.ToByte(255 - (color * multiplierColorScale / 10)), Convert.ToByte(255 - (color * multiplierColorScale / 10))));
                        }
                    }
                    ////blueish
                    if (color < -1)
                    {
                        if (color * multiplierColorScale / 10 < -255 || color * multiplierColorScale / 10 == -255)
                        {
                            gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
                        }
                        if (color * multiplierColorScale / 10 < 0 && color * multiplierColorScale / 10 > -255)
                        {
                            gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, Convert.ToByte(255 - (color * (0 - multiplierColorScale) / 10)), Convert.ToByte(255 - (color * (0 - multiplierColorScale) / 10)), 255));
                        }
                    }
                    ////grayish
                    if (color == 64001)
                    {
                        gridSquare.Fill = new SolidColorBrush(Color.FromArgb(100, 100, 100, 100));
                    }
                    gridSquare.Width = Convert.ToDouble(textBox_gridSize_Universal.Text) * Convert.ToDouble(squareToGridScale);
                    gridSquare.Height = Convert.ToDouble(textBox_gridSize_Universal.Text) * Convert.ToDouble(squareToGridScale);
                    Canvas.SetLeft(gridSquare, (Convert.ToSingle(gridArray[i, j, 0]) + (Convert.ToDouble(textBox_printingDiameter_Universal.Text) / 2)) * Convert.ToDouble(squareToGridScale));
                    Canvas.SetTop(gridSquare, ((Convert.ToDouble(textBox_printingDiameter_Universal.Text) - (Convert.ToSingle(gridArray[i, j, 1]) + (Convert.ToDouble(textBox_printingDiameter_Universal.Text) / 2))) * Convert.ToDouble(squareToGridScale)));
                    canvas_gridMeasure_Universal.Children.Add(gridSquare);
                    if (checkBox_measureLabels_Universal.IsChecked == true)
                    {
                        TextBlock textBlock_labels = new TextBlock();
                        textBlock_labels.Text = Convert.ToString(Convert.ToString(Math.Truncate(Convert.ToDouble(Convert.ToSingle(gridArray[i, j, 2])) * 100) / 100));
                        textBlock_labels.Foreground = Brushes.Black;
                        textBlock_labels.FontWeight = FontWeights.UltraBold;
                        textBlock_labels.FontSize = 14;
                        Canvas.SetLeft(textBlock_labels, (Convert.ToSingle(gridArray[i, j, 0]) + (Convert.ToDouble(textBox_printingDiameter_Universal.Text) / 2)) * Convert.ToDouble(squareToGridScale));
                        Canvas.SetTop(textBlock_labels, ((Convert.ToDouble(textBox_printingDiameter_Universal.Text) - (Convert.ToSingle(gridArray[i, j, 1]) + (Convert.ToDouble(textBox_printingDiameter_Universal.Text) / 2))) * Convert.ToDouble(squareToGridScale)));
                        canvas_gridMeasure_Universal.Children.Add(textBlock_labels);
                    }

                }
            }



            ////Scale
            int scaleSegments = 23;
            int value = -300;
            float scaleSegmentWidth = Convert.ToSingle(canvas_gridMeasureScale_Universal.Width / scaleSegments);
            multiplierColorScale = Convert.ToInt32(textBox_multiplierColorScale_Universal.Text);

            for (int i = 0; i < scaleSegments; i++)
            {
                value = value + 25;
                gridSquare = new System.Windows.Shapes.Rectangle();
                gridSquare.Stroke = new SolidColorBrush(Colors.Black);
                gridSquare.StrokeThickness = 0;
                gridSquare.ToolTip = Convert.ToString(Math.Truncate((Convert.ToSingle(value) * multiplierColorScale / 10000) * 100) / 100) + " mm";

                ////greenish
                if ((value > Convert.ToSingle(-10) && value < Convert.ToSingle(10)) || value == Convert.ToSingle(-10) || value == Convert.ToSingle(10))
                {
                    gridSquare.Fill = new SolidColorBrush(Color.FromArgb(100, 127, 255, 127));
                }
                ////redish
                if (value > 1)
                {
                    if (value * multiplierColorScale / 10 > 255 || value * multiplierColorScale / 10 == 255)
                    {
                        gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                    }
                    if (value * multiplierColorScale / 10 > 0 && value * multiplierColorScale / 10 < 255)
                    {
                        gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 255, Convert.ToByte(255 - (value * multiplierColorScale / 10)), Convert.ToByte(255 - (value * multiplierColorScale / 10))));
                    }
                }
                ////blueish
                if (value < -1)
                {
                    if (value * multiplierColorScale / 10 < -255 || value * multiplierColorScale / 10 == -255)
                    {
                        gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
                    }
                    if (value * multiplierColorScale / 10 < 0 && value * multiplierColorScale / 10 > -255)
                    {
                        gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, Convert.ToByte(255 - (value * (0 - multiplierColorScale) / 10)), Convert.ToByte(255 - (value * (0 - multiplierColorScale) / 10)), 255));
                    }
                }

                gridSquare.Width = scaleSegmentWidth;
                gridSquare.Height = canvas_gridMeasureScale_Universal.Height;
                Canvas.SetLeft(gridSquare, scaleSegmentWidth * i);
                Canvas.SetTop(gridSquare, 0);
                canvas_gridMeasureScale_Universal.Children.Add(gridSquare);

                TextBlock textBlock_labels = new TextBlock();

                textBlock_labels.Text = Convert.ToString(Math.Truncate((Convert.ToSingle(value) * multiplierColorScale / 10000) * 100) / 100);
                textBlock_labels.Foreground = Brushes.Black;
                textBlock_labels.FontWeight = FontWeights.UltraBold;
                textBlock_labels.FontSize = 8;
                Canvas.SetLeft(textBlock_labels, scaleSegmentWidth * i);
                if (i % 2 == 1)
                {
                    Canvas.SetTop(textBlock_labels, 0);
                }
                else
                {
                    Canvas.SetTop(textBlock_labels, canvas_gridMeasureScale_Universal.Height / 2);
                }
                canvas_gridMeasureScale_Universal.Children.Add(textBlock_labels);

            }

            button_gridTest_Universal.IsEnabled = true;
            button_exportToFile.IsEnabled = true;
        }
        public void planeCalibration()
        {
            switch (calibratePhase)
            {
                case 0:
                    planeCalibration_g36();
                    break;
                case 1:
                    planeLevel();
                    break;
                case 2:
                    planeTest_process();
                    break;
                default:
                    break;
            }
        }
        public void planeCalibration_g36()
        {
            label_routinePosition.Content = "Rout:" + Convert.ToString(routineCounter);
            label_positionPosition.Content = "Pos:" + Convert.ToString(position);
            textBox_console.AppendText("Plane calibration.\n");

            if (diveMeasureComplete == true)
            {
                //
                routineCounter = 4;
                diveMeasureComplete = false;
            }
            else
            {
                switch (routineCounter)
                {
                    case 0:
                        myPort.WriteLine("g28");
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                        routineTimer.Interval = 1000;
                        routineCounter++;
                        break;
                    case 1:

                        myPort.WriteLine("g36 s");
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                        //routineTimer.Interval = 5000;
                        routineCounter++;
                        break;
                    case 2:
                        //Stay at trigger point
                        myPort.WriteLine("G38 P6");
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                        routineTimer.Interval = 50;
                        routineCounter++;
                        break;
                    case 3:

                        break;
                    case 4:
                        ///
                        if (position != 2)
                        {
                            switch (position)
                            {
                                case 0:
                                    myPort.WriteLine("g36 x");
                                    break;
                                case 1:
                                    myPort.WriteLine("g36 y");
                                    break;
                            }

                        }
                        else
                        {
                            myPort.WriteLine("G36 Z");

                            myPort.WriteLine("M205");
                            EEPROM_ready = false;
                            routineTimer.Interval = 5000;
                            commandLoopType = 7;

                        }
                        position++;
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");

                        routineTimer.Interval = 2000;
                        routineCounter = 2;
                        break;
                    default:
                        textBox_console.AppendText("Routine Error");
                        break;
                }
            }

        }
        public void planeLevel()
        {
            myPort.WriteLine("M206 T3 P3222 X" + Convert.ToString(Convert.ToSingle(XY1) - 0));
            myPort.WriteLine("M206 T3 P3226 X" + Convert.ToString(Convert.ToSingle(XY2) - 0));
            myPort.WriteLine("M206 T3 P3230 X" + Convert.ToString(Convert.ToSingle(XY3) - 0));
            myPort.WriteLine("G32 S2");
            calibratePhase = 2;
            position = 0;
        }
        public void planeTest_process()
        {
            label_routinePosition.Content = "Rout:" + Convert.ToString(routineCounter);
            label_positionPosition.Content = "Pos:" + Convert.ToString(position);
            textBox_console.AppendText("Plane measure.\n");

            if (diveMeasureComplete == true)
            {
                //
                routineCounter = 4;
                diveMeasureComplete = false;
            }
            else
            {
                switch (routineCounter)
                {
                    case 0:
                        myPort.WriteLine("g28");
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                        routineTimer.Interval = 1000;
                        routineCounter++;
                        break;
                    case 1:
                        myPort.WriteLine("g37");
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                        //routineTimer.Interval = 5000;
                        routineCounter++;
                        break;
                    case 2:
                        //
                        myPort.WriteLine("G38 P5");
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");
                        routineTimer.Interval = 50;
                        routineCounter++;
                        break;
                    case 3:

                        break;
                    case 4:
                        ///
                        planeTestMeasure();

                        if (position != 6)
                        {
                            myPort.WriteLine("G37");
                        }
                        else
                        {
                            myPort.WriteLine("G28");
                            routineTimer.Enabled = false;

                        }
                        position++;
                        textBox_console.AppendText(infoText_routineCase + ": " + Convert.ToString(routineCounter) + "\n");

                        routineTimer.Interval = 2000;
                        routineCounter = 2;
                        break;
                    default:
                        textBox_console.AppendText("Routine Error");
                        break;
                }
            }
        }
        public void planeTestMeasure()
        {
            string[] positionLines = positionString.Split(' ');
            foreach (string stringLine in positionLines)
            {
                if (stringLine.Contains("Z:"))
                {
                    string modStringLine = stringLine.Replace("Z:", "");

                    switch (position)
                    {
                        case 0:
                            positionArray[position] = Convert.ToString((Math.Truncate((Convert.ToSingle(modStringLine) - Convert.ToSingle(z_probeHeight) - Convert.ToSingle(bedDistance)) * 1000) / 1000));
                            label_position0.Content = Convert.ToString(positionArray[position]);
                            textBox_console.AppendText(infoText_positionCase + Convert.ToString(position) + "\n");
                            break;
                        case 1:
                            positionArray[position] = Convert.ToString((Math.Truncate((Convert.ToSingle(modStringLine) - Convert.ToSingle(z_probeHeight) - Convert.ToSingle(clearance)) * 1000) / 1000));
                            label_position1.Content = Convert.ToString(positionArray[position]);
                            textBox_console.AppendText(infoText_positionCase + Convert.ToString(position) + "\n");
                            break;
                        case 2:
                            positionArray[position] = Convert.ToString((Math.Truncate((Convert.ToSingle(modStringLine) - Convert.ToSingle(z_probeHeight) - Convert.ToSingle(clearance)) * 1000) / 1000));
                            label_position2.Content = Convert.ToString(positionArray[position]);
                            textBox_console.AppendText(infoText_positionCase + Convert.ToString(position) + "\n");
                            break;
                        case 3:
                            positionArray[position] = Convert.ToString((Math.Truncate((Convert.ToSingle(modStringLine) - Convert.ToSingle(z_probeHeight) - Convert.ToSingle(clearance)) * 1000) / 1000));
                            label_position3.Content = Convert.ToString(positionArray[position]);
                            textBox_console.AppendText(infoText_positionCase + Convert.ToString(position) + "\n");
                            break;
                        case 4:
                            positionArray[position] = Convert.ToString((Math.Truncate((Convert.ToSingle(modStringLine) - Convert.ToSingle(z_probeHeight) - Convert.ToSingle(clearance)) * 1000) / 1000));
                            label_position4.Content = Convert.ToString(positionArray[position]);
                            textBox_console.AppendText(infoText_positionCase + Convert.ToString(position) + "\n");
                            break;
                        case 5:
                            positionArray[position] = Convert.ToString((Math.Truncate((Convert.ToSingle(modStringLine) - Convert.ToSingle(z_probeHeight) - Convert.ToSingle(clearance)) * 1000) / 1000));
                            label_position5.Content = Convert.ToString(positionArray[position]);
                            textBox_console.AppendText(infoText_positionCase + Convert.ToString(position) + "\n");
                            break;
                        case 6:
                            positionArray[position] = Convert.ToString((Math.Truncate((Convert.ToSingle(modStringLine) - Convert.ToSingle(z_probeHeight) - Convert.ToSingle(clearance)) * 1000) / 1000));
                            label_position6.Content = Convert.ToString(positionArray[position]);
                            textBox_console.AppendText(infoText_positionCase + Convert.ToString(position) + "\n");
                            break;

                        default:
                            textBox_console.AppendText("Position Error");
                            break;
                    }
                }
            }

        }
        /// <summary>
        /// Rezultātu apstrāde
        /// </summary>
        public void EEPROMReadValues()
        {
            List<string> lines = new List<string>(linesToProcess);
            int lineCount = lines.Count;
            textBox_debug.AppendText(String.Format("LinesReceived: {0}\n", lines.Count));
            for (int i = 0; i < lineCount - 1; i++)
            {

                textBox_debug.AppendText(lines[0] + '\n');
                if (lines[0].Contains("Printer ID"))
                {
                    for (int j = 0; j < lines.Count; j++)
                    {
                        EEPROMList.Add(lines[j]);
                        if (lines[j].Contains("EPR") != true)
                        {
                            routineTimer.Enabled = false;
                            break;
                        }
                        if (j == lines.Count)
                        {

                            break;
                        }

                    }
                    routineTimer.Enabled = false;
                }

                lines.RemoveAt(0);
            }
            textBox_debug.Clear();

            for (int i = 0; i < EEPROMList.Count - 1; i++)
            {
                textBox_debug.AppendText(EEPROMList[i] + '\n');
            }

            EEPROM_ready = false;
            if (routineTimer.Enabled == false)
            {
                myPort.WriteLine("G28");
                foreach (string EEPROMLine in EEPROMList)
                {
                    if (EEPROMLine.Contains("Tower X"))
                    {
                        string[] stringSplit = EEPROMLine.Split(' ');
                        endstopX = stringSplit[2];
                    }
                    if (EEPROMLine.Contains("Tower Y"))
                    {
                        string[] stringSplit = EEPROMLine.Split(' ');
                        endstopY = stringSplit[2];
                    }
                    if (EEPROMLine.Contains("Tower Z"))
                    {
                        string[] stringSplit = EEPROMLine.Split(' ');
                        endstopZ = stringSplit[2];
                    }
                    if (EEPROMLine.Contains("Horizontal rod"))
                    {
                        string[] stringSplit = EEPROMLine.Split(' ');
                        horizontalRadiuss = stringSplit[2];
                    }
                    if (EEPROMLine.Contains("Z-probe height"))
                    {
                        string[] stringSplit = EEPROMLine.Split(' ');
                        z_probeHeight = stringSplit[2];
                    }
                    if (EEPROMLine.Contains("bed dist"))
                    {
                        string[] stringSplit = EEPROMLine.Split(' ');
                        bedDistance = stringSplit[2];
                    }
                    if (EEPROMLine.Contains("XY1"))
                    {
                        string[] stringSplit = EEPROMLine.Split(' ');
                        XY1 = stringSplit[2];
                    }
                    if (EEPROMLine.Contains("XY2"))
                    {
                        string[] stringSplit = EEPROMLine.Split(' ');
                        XY2 = stringSplit[2];
                    }
                    if (EEPROMLine.Contains("XY3"))
                    {
                        string[] stringSplit = EEPROMLine.Split(' ');
                        XY3 = stringSplit[2];
                    }
                    if (EEPROMLine.Contains("bed dist"))
                    {
                        string[] stringSplit = EEPROMLine.Split(' ');
                        bedDistance = stringSplit[2];
                    }
                }

                DMeasureComplete = true;
                switch (processOrigin)
                {
                    case 5:
                        //simple
                        commandLoopType = 6;
                        break;
                    case 8:
                        //random
                        commandLoopType = 9;
                        break;
                    case 10:
                        //universal
                        commandLoopType = 11;
                        break;
                    case 12:
                        commandLoopType = 12;
                        calibratePhase = 1;
                        break;
                    default:
                        break;
                }
                XYoffset = (Convert.ToSingle(XY1) + Convert.ToSingle(XY2) + Convert.ToSingle(XY3)) / 3;
                routineTimer.Enabled = true;
            }

        }
        public void button_calculateArea_Click(object sender, RoutedEventArgs e)
        {
            button_gridTestSimple.IsEnabled = true;
            gridProbePoints.Clear();
            textBox_debugGridPointList.Clear();
            gridMeasureGcode.Clear();
            gridTestSimpleResults.Clear();
            gridTestSimpleCompleteGrid.Clear();


            probeReach = 2 * Convert.ToSingle((Convert.ToDouble(textBox_printingDiameter.Text) / 2) - (Math.Sqrt(Math.Pow(Convert.ToDouble(textBox_probeOffsetX.Text), 2) + Math.Pow(Convert.ToDouble(textBox_probeOffsetY.Text), 2))));

            label_probeReach_value.Content = Convert.ToString(probeReach);

            maxDiscreteDiameter = Convert.ToInt32(textBox_gridSize.Text) * (Convert.ToInt32(probeReach) / Convert.ToInt32(textBox_gridSize.Text));

            label_maxDiscreteDiameter_value.Content = Convert.ToString(maxDiscreteDiameter);



            for (int i = 0; i < (Convert.ToInt32(textBox_printingDiameter.Text) / Convert.ToInt32(textBox_gridSize.Text)) + 1; i++)
            {

                for (int j = 0; j < (Convert.ToInt32(textBox_printingDiameter.Text) / Convert.ToInt32(textBox_gridSize.Text)) + 1; j++)
                {

                    xPosMath = Convert.ToSingle((Convert.ToSingle(textBox_printingDiameter.Text) / 2) - (Convert.ToSingle(textBox_gridSize.Text) * i));
                    yPosMath = Convert.ToSingle((Convert.ToSingle(textBox_printingDiameter.Text) / 2) - (Convert.ToSingle(textBox_gridSize.Text) * j));
                    ////diskrētas koordinātas atlasīt un pārbaudīt nosegto laukumu
                    ////pozicija i*gridSize;j*gridSize
                    if (Math.Sqrt((xPosMath * xPosMath) + (yPosMath * yPosMath)) < Convert.ToSingle(maxDiscreteDiameter) / 2 || Math.Sqrt((xPosMath * xPosMath) + (yPosMath * yPosMath)) == Convert.ToSingle(maxDiscreteDiameter) / 2)
                    {


                        gridProbePoints.Add(new Tuple<float, float>(xPosMath, yPosMath));
                        //textBox_debugGridPointList.AppendText(Convert.ToString(gridProbePoints.Last().Item1) + " " + Convert.ToString(gridProbePoints.Last().Item2) + "\n");
                        gridMeasureGcode.Add("G1 " + "X" + Convert.ToString(gridProbePoints.Last().Item1) + " " + "Y" + Convert.ToString(gridProbePoints.Last().Item2) + " " + "Z" + Convert.ToSingle(clearance));
                        textBox_debugGridPointList.AppendText(gridMeasureGcode.Last() + ";" + "\r\n");
                    }

                }

            }
            areaCovered = Convert.ToSingle(gridProbePoints.Count * Convert.ToSingle(textBox_gridSize.Text) * Convert.ToSingle(textBox_gridSize.Text));
            label_gridMeasurePointCount_value.Content = Convert.ToString(gridProbePoints.Count);
            label_coveredArea_value.Content = Convert.ToString(areaCovered / ((Convert.ToSingle(textBox_printingDiameter.Text) * Convert.ToSingle(textBox_printingDiameter.Text) * (Math.PI) / 4)));

            squareToGridScale = Convert.ToSingle(canvas_gridMeasureSimple.Height / (Convert.ToDouble(textBox_printingDiameter.Text)));
            canvas_gridMeasureSimple.Children.Clear();

            Ellipse punktsEllipse = new Ellipse();

            punktsEllipse = new Ellipse();
            punktsEllipse.Stroke = new SolidColorBrush(Colors.Black);
            punktsEllipse.Width = Convert.ToDouble(textBox_printingDiameter.Text) * Convert.ToDouble(squareToGridScale);
            punktsEllipse.Height = Convert.ToDouble(textBox_printingDiameter.Text) * Convert.ToDouble(squareToGridScale);

            Canvas.SetTop(punktsEllipse, 0);
            Canvas.SetLeft(punktsEllipse, 0);
            canvas_gridMeasureSimple.Children.Add(punktsEllipse);


            foreach (Tuple<float, float> punkts in gridProbePoints)
            {
                punktsEllipse = new Ellipse();
                punktsEllipse.Stroke = new SolidColorBrush(Colors.Black);
                //gridSquare.ToolTip = Convert.ToString(square.Item3);
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = Color.FromArgb(255, 0, 0, 0);
                punktsEllipse.Fill = mySolidColorBrush;
                punktsEllipse.Width = 2;
                punktsEllipse.Height = 2;
                Canvas.SetTop(punktsEllipse, (punkts.Item1 + (Convert.ToDouble(textBox_printingDiameterRandom.Text) / 2)) * Convert.ToDouble(squareToGridScale));
                Canvas.SetLeft(punktsEllipse, ((Convert.ToDouble(textBox_printingDiameterRandom.Text) - (punkts.Item2 + (Convert.ToDouble(textBox_printingDiameterRandom.Text) / 2))) * Convert.ToDouble(squareToGridScale)));



                canvas_gridMeasureSimple.Children.Add(punktsEllipse);
            }


        }
        public void processProbeMeasurements()
        {
            routineTimer.Enabled = false;
            textBox_probeMeasurements.Clear();
            uniqueProbeMeasurementsList.Clear();
            measurementCounterList.Clear();


            ////probeMeasurementsList.RemoveAt(0);

            foreach (float measure in probeMeasurementsList)
            {
                //textBox_probeMeasurements.AppendText(Convert.ToString(measure) + "\n");

                foreach (float compare in uniqueProbeMeasurementsList)
                {
                    if (measure == compare)
                    {
                        addMeasure = false;

                    }
                }
                if (addMeasure == true)
                {
                    uniqueProbeMeasurementsList.Add(measure);
                    //textBox_probeMeasurements.AppendText(Convert.ToString(measure) + "group \n");

                }
                addMeasure = true;

            }
            textBox_probeMeasurements.Clear();
            uniqueProbeMeasurementsList.Sort((y, x) => y.CompareTo(x));

            barMaxValue = 0;
            textBox_probeMeasurements.AppendText("Test position:" + " " + "X" + textBox_probeXPosition.Text + " " + "Y" + textBox_probeYPosition.Text + "\r\n");


            foreach (float group in uniqueProbeMeasurementsList)
            {
                measureCounter = 0;

                foreach (float measure in probeMeasurementsList)
                {
                    if (measure == group)
                    {
                        measureCounter++;
                    }
                }
                measurementCounterList.Add(measureCounter);
                textBox_probeMeasurements.AppendText("Measurement group: " + Convert.ToString(group) + " Count: " + measureCounter + " \n");
            }
            foreach (int group in measurementCounterList)
            {
                if (group > barMaxValue)
                {
                    barMaxValue = group;
                }
            }

            barPosition = 0;

            barCount = uniqueProbeMeasurementsList.Count;
            barWidth = Convert.ToInt32(canvas_probeMeasure.Width) / (2 * barCount);
            canvas_probeMeasure.Children.Clear();

            foreach (int barIndex in measurementCounterList)
            {
                barGraph = new System.Windows.Shapes.Rectangle();
                barGraph.Stroke = new SolidColorBrush(Colors.Black);
                barGraph.Fill = new SolidColorBrush(Colors.Black);
                barGraph.Width = barWidth;
                barGraph.Height = (barIndex / barMaxValue) * canvas_probeMeasure.Height;
                Canvas.SetLeft(barGraph, 0 + (barPosition * 2 * barWidth));
                Canvas.SetTop(barGraph, 0 + canvas_probeMeasure.Height - ((barIndex / barMaxValue) * canvas_probeMeasure.Height));
                canvas_probeMeasure.Children.Add(barGraph);

                barPosition++;
            }
            barPosition = 0;
            foreach (float group in uniqueProbeMeasurementsList)
            {
                TextBlock textBlock_labels = new TextBlock();
                if (barPosition % 2 == 1)
                {
                    barLabelYoffset = 10;
                }
                else
                {
                    barLabelYoffset = 0;
                }
                textBlock_labels.Text = Convert.ToString(group);
                textBlock_labels.Foreground = Brushes.Red;
                textBlock_labels.FontWeight = FontWeights.UltraBold;
                textBlock_labels.FontSize = 14;
                Canvas.SetLeft(textBlock_labels, 0 + (barPosition * 2 * barWidth));
                Canvas.SetTop(textBlock_labels, canvas_probeMeasure.Height + barLabelYoffset);
                canvas_probeMeasure.Children.Add(textBlock_labels);
                barPosition++;
            }
            barPosition = 0;
            foreach (int barIndex in measurementCounterList)
            {
                TextBlock textBlock_labels = new TextBlock();
                textBlock_labels.Text = Convert.ToString(barIndex);
                textBlock_labels.Foreground = Brushes.Blue;
                textBlock_labels.FontWeight = FontWeights.UltraBold;
                textBlock_labels.FontSize = 14;
                Canvas.SetLeft(textBlock_labels, 0 + (barPosition * 2 * barWidth) + (barWidth));
                Canvas.SetTop(textBlock_labels, 0 + canvas_probeMeasure.Height - ((barIndex / barMaxValue) * canvas_probeMeasure.Height));
                canvas_probeMeasure.Children.Add(textBlock_labels);
                barPosition++;
            }




        }
        private void textBox_probeMeasurements_TextChanged(object sender, TextChangedEventArgs e)
        {
            textBox_console.ScrollToEnd();
        }
        private void button_drawTextBlockTest_Click(object sender, RoutedEventArgs e)
        {

            for (int i = 0; i < 10; i++)
            {

                TextBlock textBlock_labels = new TextBlock();
                textBlock_labels.Text = "Text " + Convert.ToString(i);

                Canvas.SetTop(textBlock_labels, 100 + (i * 10));
                Canvas.SetLeft(textBlock_labels, 100 + (i * 10));
                canvas_probeMeasure.Children.Add(textBlock_labels);


            }
        }
        private void button_calculateParametersRandomGrid_Click(object sender, RoutedEventArgs e)
        {
            button_gridTestRandom.IsEnabled = true;

            gridProbePointsRandom = 0;
            localRandomSum = 0;
            localRandomPointCounter = 0;
            gridMeasureRandom.Clear();
            gridProbePoints.Clear();
            textBox_debugGridPointList_Random.Clear();
            gridMeasureGcode.Clear();
            gridTestSimpleResults.Clear();
            gridTestSimpleCompleteGrid.Clear();

            probeReach = 2 * Convert.ToSingle((Convert.ToDouble(textBox_printingDiameterRandom.Text) / 2) - (Math.Sqrt(Math.Pow(Convert.ToDouble(textBox_probeOffsetX_Random.Text), 2) + Math.Pow(Convert.ToDouble(textBox_probeOffsetY_Random.Text), 2))));

            label_probeReachRandom_value.Content = Convert.ToString(probeReach);

            maxDiscreteDiameter = Convert.ToInt32(textBox_gridSize_Random.Text) * (Convert.ToInt32(probeReach) / Convert.ToInt32(textBox_gridSize_Random.Text));

            label_maxDiscreteDiameterRandom_value.Content = Convert.ToString(maxDiscreteDiameter);



            ////Local Random
            localRandom();


            areaCovered = Convert.ToSingle(gridProbePointsRandom * Convert.ToSingle(textBox_gridSize_Random.Text) * Convert.ToSingle(textBox_gridSize_Random.Text));
            label_gridMeasurePointCountRandom_value.Content = Convert.ToString(gridProbePoints.Count);
            label_coveredAreaRandom_value.Content = Convert.ToString(areaCovered / ((Convert.ToSingle(textBox_printingDiameterRandom.Text) * Convert.ToSingle(textBox_printingDiameterRandom.Text) * (Math.PI) / 4)));
            label_gridSquaresRandom_value.Content = Convert.ToString(gridSquareRandomCount);

            squareToGridScale = Convert.ToSingle(canvas_gridMeasureRandom.Height / (Convert.ToDouble(textBox_printingDiameterRandom.Text)));
            canvas_gridMeasureRandom.Children.Clear();

            Ellipse punktsEllipse = new Ellipse();

            punktsEllipse = new Ellipse();
            punktsEllipse.Stroke = new SolidColorBrush(Colors.Black);
            punktsEllipse.Width = Convert.ToDouble(textBox_printingDiameterRandom.Text) * Convert.ToDouble(squareToGridScale);
            punktsEllipse.Height = Convert.ToDouble(textBox_printingDiameterRandom.Text) * Convert.ToDouble(squareToGridScale);

            Canvas.SetTop(punktsEllipse, 0);
            Canvas.SetLeft(punktsEllipse, 0);
            canvas_gridMeasureRandom.Children.Add(punktsEllipse);


            foreach (Tuple<float, float> punkts in gridProbePoints)
            {
                punktsEllipse = new Ellipse();
                punktsEllipse.Stroke = new SolidColorBrush(Colors.Black);
                //gridSquare.ToolTip = Convert.ToString(square.Item3);
                SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                mySolidColorBrush.Color = Color.FromArgb(255, 0, 0, 0);
                punktsEllipse.Fill = mySolidColorBrush;
                punktsEllipse.Width = 2;
                punktsEllipse.Height = 2;
                Canvas.SetTop(punktsEllipse, (punkts.Item1 + (Convert.ToDouble(textBox_printingDiameterRandom.Text) / 2)) * Convert.ToDouble(squareToGridScale));
                Canvas.SetLeft(punktsEllipse, ((Convert.ToDouble(textBox_printingDiameterRandom.Text) - (punkts.Item2 + (Convert.ToDouble(textBox_printingDiameterRandom.Text) / 2))) * Convert.ToDouble(squareToGridScale)));



                canvas_gridMeasureRandom.Children.Add(punktsEllipse);
            }


        }
        public void localRandom()
        {
            ////timesaving
            gridCountXAxis = (Convert.ToInt32(textBox_printingDiameterRandom.Text) / Convert.ToInt32(textBox_gridSize_Random.Text)) + 1;
            gridCountYAxis = (Convert.ToInt32(textBox_printingDiameterRandom.Text) / Convert.ToInt32(textBox_gridSize_Random.Text)) + 1;
            discreteRadiuss = Convert.ToSingle(maxDiscreteDiameter) / 2;
            discreteRadiussCheck = Convert.ToSingle(maxDiscreteDiameter + ((1) * Convert.ToInt32(textBox_gridSize_Random.Text))) / 2;
            discreteRadiussCheckEqual = Convert.ToSingle(maxDiscreteDiameter + Convert.ToSingle(maxDiscreteDiameter + ((1) * Convert.ToInt32(textBox_gridSize_Random.Text))) / 2) / 2;

            gridSquareRandomCount = 0;
            for (int i = 0; i < gridCountXAxis; i++)
            {

                for (int j = 0; j < gridCountYAxis; j++)
                {
                    int k = 0;
                    xPosMath = Convert.ToSingle((Convert.ToSingle(textBox_printingDiameterRandom.Text) / 2) - (Convert.ToSingle(textBox_gridSize_Random.Text) * i));
                    yPosMath = Convert.ToSingle((Convert.ToSingle(textBox_printingDiameterRandom.Text) / 2) - (Convert.ToSingle(textBox_gridSize_Random.Text) * j));
                    ////diskrētas koordinātas atlasīt un pārbaudīt nosegto laukumu
                    ////pozicija i*gridSize;j*gridSize
                    randomBreak = 0;
                    if (Math.Sqrt((xPosMath * xPosMath) + (yPosMath * yPosMath)) < discreteRadiussCheck || Math.Sqrt((xPosMath * xPosMath) + (yPosMath * yPosMath)) == discreteRadiussCheckEqual)
                    {
                        while (k != Convert.ToInt32(textBox_randomPointCount.Text))
                        {
                            int XRand = rnd.Next(0 - (Convert.ToInt32(textBox_gridSize_Random.Text) / 2), (Convert.ToInt32(textBox_gridSize_Random.Text) / 2) + 1);
                            int YRand = rnd.Next(0 - (Convert.ToInt32(textBox_gridSize_Random.Text) / 2), (Convert.ToInt32(textBox_gridSize_Random.Text) / 2) + 1);
                            if (Math.Sqrt(((xPosMath + XRand) * (xPosMath + XRand)) + ((yPosMath + YRand) * (yPosMath + YRand))) < discreteRadiuss || Math.Sqrt(((xPosMath + XRand) * (xPosMath + XRand)) + ((yPosMath + YRand) * (yPosMath + YRand))) == discreteRadiuss)
                            {

                                gridProbePoints.Add(new Tuple<float, float>((xPosMath + XRand), (yPosMath + YRand)));
                                //textBox_debugGridPointList.AppendText(Convert.ToString(gridProbePoints.Last().Item1) + " " + Convert.ToString(gridProbePoints.Last().Item2) + "\n");
                                gridMeasureGcode.Add("G1 " + "X" + Convert.ToString(gridProbePoints.Last().Item1) + " " + "Y" + Convert.ToString(gridProbePoints.Last().Item2) + " " + "Z" + Convert.ToSingle(clearance));
                                textBox_debugGridPointList_Random.AppendText(gridMeasureGcode.Last() + ";" + "\r\n");
                                k++;
                                randomBreak = 0;
                            }
                            randomBreak++;
                            if (randomBreak == 50)
                            {
                                break;
                            }

                        }
                        gridMeasureGcode.Add("GridSquareEnd");
                        gridMeasureGcode.Add("Measure:" + " " + Convert.ToString(xPosMath) + " " + Convert.ToString(yPosMath));
                        gridSquareRandomCount++;
                        gridProbePointsRandom++;
                        textBox_debugGridPointList_Random.AppendText("GridSquareEnd" + "\r\n");
                        textBox_debugGridPointList_Random.AppendText("Measure:" + " " + Convert.ToString(xPosMath) + " " + Convert.ToString(yPosMath) + "\r\n");
                    }

                }

            }
            gridMeasureGcode.Add("GCodeEnd");
        }

        public void button_calculateParametersGrid_Universal_Click(object sender, RoutedEventArgs e)
        {

            textBox_debugGridPointList_Universal.Clear();
            gridTestUniversal.Clear();
            probeReach = 2 * Convert.ToSingle((Convert.ToDouble(textBox_printingDiameter_Universal.Text) / 2) - (Math.Sqrt(Math.Pow(Convert.ToDouble(textBox_probeOffsetX_Universal.Text), 2) + Math.Pow(Convert.ToDouble(textBox_probeOffsetY_Universal.Text), 2))));
            printerSize = Convert.ToInt32(textBox_printingDiameter_Universal.Text);
            label_probeReach_Universal_value.Content = Convert.ToString(probeReach);

            maxDiscreteDiameter = Convert.ToInt32(textBox_gridSize_Universal.Text) * (Convert.ToInt32(probeReach) / Convert.ToInt32(textBox_gridSize_Universal.Text));

            label_maxDiscreteDiameter_Universal_value.Content = Convert.ToString(maxDiscreteDiameter);

            gridCountXAxis = (Convert.ToInt32(textBox_printingDiameter_Universal.Text) / Convert.ToInt32(textBox_gridSize_Universal.Text)) + 1;
            gridCountYAxis = (Convert.ToInt32(textBox_printingDiameter_Universal.Text) / Convert.ToInt32(textBox_gridSize_Universal.Text)) + 1;
            discreteRadiuss = Convert.ToSingle(maxDiscreteDiameter) / 2;
            discreteRadiussCheck = Convert.ToSingle(maxDiscreteDiameter + ((1) * Convert.ToInt32(textBox_gridSize_Universal.Text))) / 2;
            discreteRadiussCheckEqual = Convert.ToSingle(maxDiscreteDiameter + Convert.ToSingle(maxDiscreteDiameter + ((1) * Convert.ToInt32(textBox_gridSize_Universal.Text))) / 2) / 2;

            if (checkBox_randomMode.IsChecked == true)
            {
                probePoints = Convert.ToInt32(textBox_randomPointCount_Universal.Text);
            }
            else
            {
                probePoints = 1;
            }
            gridSizeUniversal = Convert.ToInt32(textBox_gridSize_Universal.Text);

            gridArray = new string[gridCountXAxis, gridCountYAxis, (probePoints * 3) + 3];

            universalPointGenerator();
        }

        public void universalPointGenerator()
        {

            for (int i = gridArray.GetLength(0) - 1; i != -1; i--)
            {
                for (int j = 0; j < gridArray.GetLength(1); j++)
                {
                    gridArray[i, j, 0] = Convert.ToString((i * gridSizeUniversal) - (printerSize / 2));
                    gridArray[i, j, 1] = Convert.ToString((j * gridSizeUniversal) - (printerSize / 2));
                    int k = 0;
                    xPosMath = Convert.ToSingle((i * gridSizeUniversal) - (printerSize / 2));
                    yPosMath = Convert.ToSingle((j * gridSizeUniversal) - (printerSize / 2));

                    if (checkBox_randomMode.IsChecked == true)
                    {
                        if (Math.Sqrt((xPosMath * xPosMath) + (yPosMath * yPosMath)) < discreteRadiussCheck || Math.Sqrt((xPosMath * xPosMath) + (yPosMath * yPosMath)) == discreteRadiussCheckEqual)
                        {
                            gridArray[i, j, 2] = "OK " + Convert.ToString(probePoints);
                        }
                        else
                        {
                            gridArray[i, j, 2] = "N/A";
                        }
                    }
                    else
                    {
                        if (Math.Sqrt((xPosMath * xPosMath) + (yPosMath * yPosMath)) < discreteRadiuss || Math.Sqrt((xPosMath * xPosMath) + (yPosMath * yPosMath)) == discreteRadiuss)
                        {
                            gridArray[i, j, 2] = "OK " + Convert.ToString(probePoints);
                        }
                        else
                        {
                            gridArray[i, j, 2] = "N/A";
                        }
                    }



                    if (checkBox_randomMode.IsChecked == true)
                    {
                        if (gridArray[i, j, 2] == "OK " + Convert.ToString(probePoints))
                        {
                            while (k != Convert.ToInt32(textBox_randomPointCount_Universal.Text))
                            {
                                int XRand = rnd.Next(0 - (Convert.ToInt32(textBox_gridSize_Universal.Text) / 2), (Convert.ToInt32(textBox_gridSize_Universal.Text) / 2) + 1);
                                int YRand = rnd.Next(0 - (Convert.ToInt32(textBox_gridSize_Universal.Text) / 2), (Convert.ToInt32(textBox_gridSize_Universal.Text) / 2) + 1);
                                if (Math.Sqrt(((xPosMath + XRand) * (xPosMath + XRand)) + ((yPosMath + YRand) * (yPosMath + YRand))) < discreteRadiuss || Math.Sqrt(((xPosMath + XRand) * (xPosMath + XRand)) + ((yPosMath + YRand) * (yPosMath + YRand))) == discreteRadiuss)
                                {

                                    gridArray[i, j, (k * 3) + 3] = Convert.ToString((i * gridSizeUniversal) - (printerSize / 2) + XRand);
                                    gridArray[i, j, (k * 3) + 4] = Convert.ToString((j * gridSizeUniversal) - (printerSize / 2) + YRand);
                                    gridArray[i, j, (k * 3) + 5] = "Measurement#" + Convert.ToString(k);

                                    k++;
                                    randomBreak = 0;
                                }
                                randomBreak++;
                                if (randomBreak == 50)
                                {
                                    break;
                                }

                            }
                        }



                    }
                    else
                    {
                        if (gridArray[i, j, 2] == "OK " + Convert.ToString(probePoints))
                        {
                            for (k = 0; k < probePoints; k++)
                            {
                                gridArray[i, j, (k * 3) + 3] = Convert.ToString((i * gridSizeUniversal) - (printerSize / 2));
                                gridArray[i, j, (k * 3) + 4] = Convert.ToString((j * gridSizeUniversal) - (printerSize / 2));
                                gridArray[i, j, (k * 3) + 5] = "Measurement";
                            }
                        }
                    }



                }
            }
            foreach (string text in gridArray)
            {
                textBox_debugGridPointList_Universal.AppendText(text + "\n");
            }


            squareToGridScale = Convert.ToSingle(canvas_gridMeasure_Universal.Height / (Convert.ToDouble(textBox_printingDiameter_Universal.Text)));
            canvas_gridMeasure_Universal.Children.Clear();

            Ellipse punktsEllipse = new Ellipse();

            punktsEllipse = new Ellipse();
            punktsEllipse.Stroke = new SolidColorBrush(Colors.Black);
            punktsEllipse.Width = Convert.ToDouble(textBox_printingDiameter_Universal.Text) * Convert.ToDouble(squareToGridScale);
            punktsEllipse.Height = Convert.ToDouble(textBox_printingDiameter_Universal.Text) * Convert.ToDouble(squareToGridScale);

            Canvas.SetTop(punktsEllipse, 0);
            Canvas.SetLeft(punktsEllipse, 0);
            canvas_gridMeasure_Universal.Children.Add(punktsEllipse);


            for (int i = 0; i < gridArray.GetLength(0); i++)
            {
                for (int j = 0; j < gridArray.GetLength(1); j++)
                {
                    if (gridArray[i, j, 2].Contains("OK") == true)
                    {
                        if (checkBox_randomMode.IsChecked == false)
                        {

                            punktsEllipse = new Ellipse();
                            punktsEllipse.Stroke = new SolidColorBrush(Colors.Black);
                            //gridSquare.ToolTip = Convert.ToString(square.Item3);
                            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                            mySolidColorBrush.Color = Color.FromArgb(255, 0, 0, 0);
                            punktsEllipse.Fill = mySolidColorBrush;
                            punktsEllipse.Width = 2;
                            punktsEllipse.Height = 2;
                            Canvas.SetTop(punktsEllipse, (Convert.ToDouble(gridArray[i, j, 3]) + (Convert.ToDouble(textBox_printingDiameter_Universal.Text) / 2)) * Convert.ToDouble(squareToGridScale));
                            Canvas.SetLeft(punktsEllipse, ((Convert.ToDouble(textBox_printingDiameter_Universal.Text) - (Convert.ToDouble(gridArray[i, j, 4]) + (Convert.ToDouble(textBox_printingDiameter_Universal.Text) / 2))) * Convert.ToDouble(squareToGridScale)));



                            canvas_gridMeasure_Universal.Children.Add(punktsEllipse);
                        }
                        else
                        {
                            for (int k = 0; k < probePoints; k++)
                            {
                                if (gridArray[i, j, (k * 3) + 3] != null && gridArray[i, j, (k * 3) + 4] != null && gridArray[i, j, (k * 3) + 5] != null)
                                {
                                    punktsEllipse = new Ellipse();
                                    punktsEllipse.Stroke = new SolidColorBrush(Colors.Black);
                                    //gridSquare.ToolTip = Convert.ToString(square.Item3);
                                    SolidColorBrush mySolidColorBrush = new SolidColorBrush();
                                    mySolidColorBrush.Color = Color.FromArgb(255, 0, 0, 0);
                                    punktsEllipse.Fill = mySolidColorBrush;
                                    punktsEllipse.Width = 2;
                                    punktsEllipse.Height = 2;
                                    Canvas.SetTop(punktsEllipse, (Convert.ToDouble(gridArray[i, j, (k * 3) + 3]) + (Convert.ToDouble(textBox_printingDiameter_Universal.Text) / 2)) * Convert.ToDouble(squareToGridScale));
                                    Canvas.SetLeft(punktsEllipse, ((Convert.ToDouble(textBox_printingDiameter_Universal.Text) - (Convert.ToDouble(gridArray[i, j, (k * 3) + 4]) + (Convert.ToDouble(textBox_printingDiameter_Universal.Text) / 2))) * Convert.ToDouble(squareToGridScale)));



                                    canvas_gridMeasure_Universal.Children.Add(punktsEllipse);
                                }

                            }

                        }




                    }
                }
            }

            universalGcodeGenerator();
        }

        public void universalGcodeGenerator()
        {
            directionMarker = false;
            double markerSize = 30;
            Console.WriteLine("universalGcodeGenerator START");
            ////Y scan
            if (radioButton_Y.IsChecked == true)
            {
                //front-back
                if (radioButton_L.IsChecked == true)
                {
                    for (int i = 0; i < gridArray.GetLength(0); i++)
                    {

                        //left-right
                        if (radioButton_F.IsChecked == true)
                        {
                            if (directionMarker == false)
                            {
                                Ellipse punktsEllipse = new Ellipse();

                                punktsEllipse = new Ellipse();
                                punktsEllipse.Stroke = new SolidColorBrush(Colors.Black);
                                punktsEllipse.Width = markerSize;
                                punktsEllipse.Height = markerSize;
                                punktsEllipse.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                                Canvas.SetTop(punktsEllipse, canvas_gridMeasure_Universal.Height - markerSize);
                                Canvas.SetLeft(punktsEllipse, 0);
                                canvas_gridMeasure_Universal.Children.Add(punktsEllipse);

                                directionMarker = true;
                            }
                            for (int j = 0; j < gridArray.GetLength(1); j++)
                            {
                                universalGcodeGeneratorProcess(i, j);
                            }
                        }
                        //right-left
                        if (radioButton_B.IsChecked == true)
                        {
                            if (directionMarker == false)
                            {
                                Ellipse punktsEllipse = new Ellipse();

                                punktsEllipse = new Ellipse();
                                punktsEllipse.Stroke = new SolidColorBrush(Colors.Black);
                                punktsEllipse.Width = markerSize;
                                punktsEllipse.Height = markerSize;
                                punktsEllipse.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                                Canvas.SetTop(punktsEllipse, 0);
                                Canvas.SetLeft(punktsEllipse, 0);
                                canvas_gridMeasure_Universal.Children.Add(punktsEllipse);

                                directionMarker = true;
                            }
                            for (int j = gridArray.GetLength(1) - 1; j != -1; j--)
                            {
                                universalGcodeGeneratorProcess(i, j);
                            }
                        }
                    }

                }
                //back-front
                if (radioButton_R.IsChecked == true)
                {
                    for (int i = gridArray.GetLength(0) - 1; i != -1; i--)
                    {
                        //left-right
                        if (radioButton_F.IsChecked == true)
                        {
                            if (directionMarker == false)
                            {
                                Ellipse punktsEllipse = new Ellipse();

                                punktsEllipse = new Ellipse();
                                punktsEllipse.Stroke = new SolidColorBrush(Colors.Black);
                                punktsEllipse.Width = markerSize;
                                punktsEllipse.Height = markerSize;
                                punktsEllipse.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                                Canvas.SetTop(punktsEllipse, canvas_gridMeasure_Universal.Height - markerSize);
                                Canvas.SetLeft(punktsEllipse, canvas_gridMeasure_Universal.Width - markerSize);
                                canvas_gridMeasure_Universal.Children.Add(punktsEllipse);

                                directionMarker = true;
                            }
                            for (int j = 0; j < gridArray.GetLength(1); j++)
                            {
                                universalGcodeGeneratorProcess(i, j);
                            }
                        }
                        //right-left
                        if (radioButton_B.IsChecked == true)
                        {
                            if (directionMarker == false)
                            {
                                Ellipse punktsEllipse = new Ellipse();

                                punktsEllipse = new Ellipse();
                                punktsEllipse.Stroke = new SolidColorBrush(Colors.Black);
                                punktsEllipse.Width = markerSize;
                                punktsEllipse.Height = markerSize;
                                punktsEllipse.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                                Canvas.SetTop(punktsEllipse, 0);
                                Canvas.SetLeft(punktsEllipse, canvas_gridMeasure_Universal.Width - markerSize);
                                canvas_gridMeasure_Universal.Children.Add(punktsEllipse);

                                directionMarker = true;
                            }
                            for (int j = gridArray.GetLength(1) - 1; j != -1; j--)
                            {
                                universalGcodeGeneratorProcess(i, j);
                            }
                        }
                    }
                }
            }


            ////X scan
            if (radioButton_X.IsChecked == true)
            {
                //front-back
                if (radioButton_F.IsChecked == true)
                {
                    for (int j = 0; j < gridArray.GetLength(0); j++)
                    {
                        //left-right
                        if (radioButton_L.IsChecked == true)
                        {
                            if (directionMarker == false)
                            {
                                Ellipse punktsEllipse = new Ellipse();

                                punktsEllipse = new Ellipse();
                                punktsEllipse.Stroke = new SolidColorBrush(Colors.Black);
                                punktsEllipse.Width = markerSize;
                                punktsEllipse.Height = markerSize;
                                punktsEllipse.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                                Canvas.SetTop(punktsEllipse, canvas_gridMeasure_Universal.Height - markerSize);
                                Canvas.SetLeft(punktsEllipse, 0);
                                canvas_gridMeasure_Universal.Children.Add(punktsEllipse);

                                directionMarker = true;
                            }
                            for (int i = 0; i < gridArray.GetLength(1); i++)
                            {
                                universalGcodeGeneratorProcess(i, j);
                            }
                        }
                        //right-left
                        if (radioButton_R.IsChecked == true)
                        {
                            if (directionMarker == false)
                            {
                                Ellipse punktsEllipse = new Ellipse();

                                punktsEllipse = new Ellipse();
                                punktsEllipse.Stroke = new SolidColorBrush(Colors.Black);
                                punktsEllipse.Width = markerSize;
                                punktsEllipse.Height = markerSize;
                                punktsEllipse.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                                Canvas.SetTop(punktsEllipse, canvas_gridMeasure_Universal.Height - markerSize);
                                Canvas.SetLeft(punktsEllipse, canvas_gridMeasure_Universal.Width - markerSize);
                                canvas_gridMeasure_Universal.Children.Add(punktsEllipse);

                                directionMarker = true;
                            }
                            for (int i = gridArray.GetLength(1) - 1; i != -1; i--)
                            {
                                universalGcodeGeneratorProcess(i, j);
                            }
                        }
                    }

                }
                //back-front
                if (radioButton_B.IsChecked == true)
                {
                    for (int j = gridArray.GetLength(0) - 1; j != -1; j--)
                    {
                        //left-right
                        if (radioButton_L.IsChecked == true)
                        {
                            if (directionMarker == false)
                            {
                                Ellipse punktsEllipse = new Ellipse();

                                punktsEllipse = new Ellipse();
                                punktsEllipse.Stroke = new SolidColorBrush(Colors.Black);
                                punktsEllipse.Width = markerSize;
                                punktsEllipse.Height = markerSize;
                                punktsEllipse.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                                Canvas.SetTop(punktsEllipse, 0);
                                Canvas.SetLeft(punktsEllipse, 0);
                                canvas_gridMeasure_Universal.Children.Add(punktsEllipse);

                                directionMarker = true;
                            }
                            for (int i = 0; i < gridArray.GetLength(1); i++)
                            {
                                universalGcodeGeneratorProcess(i, j);
                            }
                        }
                        //right-left
                        if (radioButton_R.IsChecked == true)
                        {
                            if (directionMarker == false)
                            {
                                Ellipse punktsEllipse = new Ellipse();

                                punktsEllipse = new Ellipse();
                                punktsEllipse.Stroke = new SolidColorBrush(Colors.Black);
                                punktsEllipse.Width = markerSize;
                                punktsEllipse.Height = markerSize;
                                punktsEllipse.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                                Canvas.SetTop(punktsEllipse, 0);
                                Canvas.SetLeft(punktsEllipse, canvas_gridMeasure_Universal.Width - markerSize);
                                canvas_gridMeasure_Universal.Children.Add(punktsEllipse);

                                directionMarker = true;
                            }
                            for (int i = gridArray.GetLength(1) - 1; i != -1; i--)
                            {
                                universalGcodeGeneratorProcess(i, j);
                            }
                        }
                    }
                }
            }

            gridTestUniversal.Add(new Tuple<string, string>("Done", "Done"));
            button_gridTest_Universal.IsEnabled = true;
        }

        public void universalGcodeGeneratorProcess(int x, int y)
        {
            //Console.WriteLine(Convert.ToString(x) + " " + Convert.ToString(y));
            if (gridArray[x, y, 2].Contains("OK") == true)
            {

                if (checkBox_randomMode.IsChecked == false)
                {
                    //*
                    gridTestUniversal.Add(new Tuple<string, string>("G1" + " " + "X" + gridArray[x, y, 0] + " " + "Y" + gridArray[x, y, 1] + " " + "Z" + Convert.ToSingle(clearance), "Simple" + " " + Convert.ToString(x) + " " + Convert.ToString(y)));
                }
                else
                {
                    for (int m = 0; m < probePoints; m++)
                    {
                        if (gridArray[x, y, (m * 3) + 3] != null && gridArray[x, y, (m * 3) + 4] != null && gridArray[x, y, (m * 3) + 5] != null)
                        {
                            //Console.WriteLine(gridArray[x, y, (m * 3) + 3] + " " + gridArray[x, y, (m * 3) + 4] + " " + gridArray[x, y, (m * 3) + 5]);
                            gridTestUniversal.Add(new Tuple<string, string>("G1" + " " + "X" + gridArray[x, y, (m * 3) + 3] + " " + "Y" + gridArray[x, y, (m * 3) + 4] + " " + "Z" + Convert.ToSingle(clearance), "Random" + " " + Convert.ToString(x) + " " + Convert.ToString(y) + " " + Convert.ToString(m)));
                        }
                    }
                }
            }
            gridTestUniversal.Add(new Tuple<string, string>("GridEnd", "GridEnd"));
        }

        public void button_showGcodeUniversal_Click(object sender, RoutedEventArgs e)
        {
            textBox_debugGridPointList_Universal.Clear();
            foreach (Tuple<string, string> gcode in gridTestUniversal)
            {
                textBox_debugGridPointList_Universal.AppendText(gcode.Item1 + " " + gcode.Item2 + "\r\n");
            }
        }
        /// <summary>
        /// Gcode pēcapstrāde
        /// </summary>
        public void filenameGenerator()
        {
            generatedFilename = "";
            generatedFilename = generatedFilename + textBox_printingDiameter_Universal.Text;
            generatedFilename = generatedFilename + "_" + textBox_gridSize_Universal.Text;
            if (checkBox_randomMode.IsChecked == true)
            {
                generatedFilename = generatedFilename + "_" + "R" + textBox_randomPointCount_Universal.Text;
            }
            else
            {
                generatedFilename = generatedFilename + "_" + "S";
            }
            if (radioButton_X.IsChecked == true)
            {
                generatedFilename = generatedFilename + "_" + "X";
            }
            else
            {
                generatedFilename = generatedFilename + "_" + "Y";
            }
            if (radioButton_R.IsChecked == true)
            {
                generatedFilename = generatedFilename + "R";
            }
            else
            {
                generatedFilename = generatedFilename + "L";
            }
            if (radioButton_F.IsChecked == true)
            {
                generatedFilename = generatedFilename + "F";
            }
            else
            {
                generatedFilename = generatedFilename + "B";
            }

        }

        public void button_exportToFile_Click(object sender, RoutedEventArgs e)
        {
            button_exportToFile.IsEnabled = false;
            filenameGenerator();
            StreamWriter sw;
            if (textBox_fileName_universal.Text.Contains("filename") == true || textBox_fileName_universal.Text == null)
            {
                sw = new StreamWriter(generatedFilename + ".txt", false); ///true - saglabat, false - dzest info
            }
            else
            {
                sw = new StreamWriter(textBox_fileName_universal.Text + ".txt", false);
            }


            sw.WriteLine("Column count {0}", gridArray.GetLength(0));
            sw.WriteLine("Row count {0}", gridArray.GetLength(1));
            sw.WriteLine(textBox_printingDiameter_Universal.Text + " " + textBox_gridSize_Universal.Text);

            for (int i = 0; i < gridArray.GetLength(0); i++)
            {
                for (int j = 0; j < gridArray.GetLength(1); j++)
                {
                    if (j != gridArray.GetLength(1) - 1)
                    {
                        sw.Write(gridArray[i, j, 0] + " " + gridArray[i, j, 1] + " " + Convert.ToString(Math.Truncate(Convert.ToDouble(gridArray[i, j, 2]) * 1000) / 1000) + "*");
                    }
                    else
                    {
                        sw.Write(gridArray[i, j, 0] + " " + gridArray[i, j, 1] + " " + Convert.ToString(Math.Truncate(Convert.ToDouble(gridArray[i, j, 2]) * 1000) / 1000));
                    }

                }
                if (i != gridArray.GetLength(0))
                {
                    sw.WriteLine();
                }

            }





            sw.Close();

        }

        public void button_openFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            try
            {
                // Get the selected file name and display in a TextBox 
                if (result == true)
                {
                    // Open document 
                    string filename = dlg.FileName;
                    textBox_openFile.Text = filename;
                    //MessageBox.Show("All is OK!");


                    StreamReader sr = new StreamReader(filename);
                    string str;
                    int columnCount = 0;
                    int rowCount = 0;
                    int lineCounter = 0;
                    int pointer = 0;
                    //gcode_gridSize = "";
                    //gcode_printerSize = "";

                    fileNameExport = System.IO.Path.GetFileNameWithoutExtension(filename);


                    while ((str = sr.ReadLine()) != null)
                    {
                        switch (lineCounter)
                        {
                            case 0:
                                string[] columnRead = str.Split(' ');
                                columnCount = Convert.ToInt32(columnRead[2]);
                                lineCounter++;
                                break;
                            case 1:
                                string[] rowRead = str.Split(' ');
                                rowCount = Convert.ToInt32(rowRead[2]);
                                lineCounter++;
                                gridArrayProcess = new float[columnCount, rowCount];
                                break;
                            case 2:
                                string[] sizeRead = str.Split(' ');
                                printSize = Convert.ToSingle(sizeRead[0]);
                                gridSize = Convert.ToSingle(sizeRead[1]);
                                lineCounter++;
                                break;
                            case 3:

                                string[] lineRead = str.Split('*');

                                for (int i = 0; i < gridArrayProcess.GetLength(0) - 1; i++)
                                {
                                    string[] measureRead = lineRead[i].Split(' ');
                                    //MessageBox.Show(measureRead[2]);
                                    gridArrayProcess[pointer, i] = Convert.ToSingle(measureRead[2]);
                                }
                                pointer++;
                                break;

                        }

                    }


                    sr.Close();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
        }

        public void button_openRawGcode_Click(object sender, RoutedEventArgs e)
        {
            rawGcode.Clear();
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".gcode";
            dlg.Filter = "gcode files (*.gcode)|*.gcode|All files (*.*)|*.*";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            try
            {
                // Get the selected file name and display in a TextBox 
                if (result == true)
                {
                    // Open document 
                    string filename = dlg.FileName;
                    textBox_openGcode.Text = filename;
                    //MessageBox.Show("All is OK!");


                    gcodeNameExport = System.IO.Path.GetFileNameWithoutExtension(filename);


                    StreamReader sr = new StreamReader(filename);
                    string str;
                    gcode_gridSize = "";
                    gcode_printerSize = "";

                    while ((str = sr.ReadLine()) != null)
                    {

                        rawGcode.Add(str);

                    }


                    sr.Close();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
        }

        public void button_gcodeCrawler_Click(object sender, RoutedEventArgs e)
        {
            float progress = 0f;
            Zoffset = Convert.ToSingle(textBox_Zoffset.Text);
            diminishingZ = 1f;
            X_headPos = 0f;
            Y_headPos = 0f;
            Z_headPos = Single.PositiveInfinity;
            E_headPos = Single.PositiveInfinity;
            F_headPos = -1;

            X_headPos_new = 0f;
            Y_headPos_new = 0f;
            Z_headPos_new = Single.PositiveInfinity;
            E_headPos_new = 0f;
            F_headPos_new = -1;
            string stepMode = "";
            string ZCorrectionMode = "";
            if (radioButton_processMode_0.IsChecked == true) { stepMode = "DSM"; }
            if (radioButton_processMode_1.IsChecked == true) { stepMode = "M2M"; }
            if (radioButton_processMode_2.IsChecked == true) { stepMode = "HSM"; }

            if (radioButton_Z_correction_discrete.IsChecked == true) { ZCorrectionMode = "DZ"; }
            if (radioButton_Z_correction_continuous.IsChecked == true) { ZCorrectionMode = "CZ"; }



            initialisePosition = false;
            StreamWriter sw;
            if (checkBox_defaultFilename.IsChecked == true)
            {
                sw = new StreamWriter("crwlr_result" + ".gcode", false); ///true - saglabat, false - dzest info
                foreach (string gcodeLine in rawGcode)
                {

                    if (checkBox_diminishingZ.IsChecked == true)
                    {
                        if (gcodeLine.Contains("; layer") == true)
                        {
                            if (gcodeLine.Contains("; layer 1,") == true)
                            {
                                //do nothing
                            }
                            else
                            {
                                if (diminishingZ > 0)
                                {
                                    diminishingZ = diminishingZ - Convert.ToSingle(textBox_diminishingZ.Text);
                                }
                                else
                                {
                                    diminishingZ = 0f;
                                }
                            }
                        }
                    }

                    if (gcodeLine.Contains("G92") == true)
                    {
                        E_headPos = 0f;
                    }

                    if (gcodeLine.Contains("G1") == false || gcodeLine[0] == ';')
                    {
                        sw.WriteLine(gcodeLine);
                    }
                    else
                    {
                        if (radioButton_processMode_0.IsChecked == true)
                        {
                            discreteStepMode(gcodeLine, sw);
                        }
                        if (radioButton_processMode_1.IsChecked == true)
                        {
                            midToMidmode(gcodeLine, sw);
                        }
                        if (radioButton_processMode_2.IsChecked == true)
                        {
                            halfStepMode(gcodeLine, sw);
                        }
                    }
                }
                sw.Close();
                label_gcodeProgress.Content = "Done!";
            }
            else
            {
                sw = new StreamWriter(gcodeNameExport + "_" + fileNameExport + "_" + stepMode + "_" + ZCorrectionMode + ".gcode", false); ///true - saglabat, false - dzest info
                foreach (string gcodeLine in rawGcode)
                {

                    if (checkBox_diminishingZ.IsChecked == true)
                    {
                        if (gcodeLine.Contains("; layer") == true)
                        {
                            if (gcodeLine.Contains("; layer 1,") == true)
                            {
                                //do nothing
                            }
                            else
                            {
                                if (diminishingZ > 0)
                                {
                                    diminishingZ = diminishingZ - Convert.ToSingle(textBox_diminishingZ.Text);
                                }
                                else
                                {
                                    diminishingZ = 0f;
                                }
                            }
                        }
                    }

                    if (gcodeLine.Contains("G92") == true)
                    {
                        E_headPos = 0f;
                    }

                    if (gcodeLine.Contains("G1") == false || gcodeLine[0] == ';')
                    {
                        sw.WriteLine(gcodeLine);
                    }
                    else
                    {
                        if (radioButton_processMode_0.IsChecked == true)
                        {
                            discreteStepMode(gcodeLine, sw);
                        }
                        if (radioButton_processMode_1.IsChecked == true)
                        {
                            midToMidmode(gcodeLine, sw);
                        }
                        if (radioButton_processMode_2.IsChecked == true)
                        {
                            halfStepMode(gcodeLine, sw);
                        }
                    }
                }
                sw.Close();
                label_gcodeProgress.Content = "Done!";
            }


        }

        public void discreteStepMode(string gcodeLine, StreamWriter sw)
        {
            //Console.WriteLine("Here!");
            ////
            string[] splitGcodeLine = gcodeLine.Split(' ');
            foreach (string splitElement in splitGcodeLine)
            {
                //Console.WriteLine(gcodeLine);
                if (splitElement.Contains("X")) { X_headPos_new = Convert.ToSingle(splitElement.Replace(";", " ").Replace("X", " ")); }
                if (splitElement.Contains("Y")) { Y_headPos_new = Convert.ToSingle(splitElement.Replace(";", " ").Replace("Y", " ")); }
                if (splitElement.Contains("Z")) { Z_headPos_new = Convert.ToSingle(splitElement.Replace(";", " ").Replace("Z", " ")); }
                if (splitElement.Contains("E")) { E_headPos_new = Convert.ToSingle(splitElement.Replace(";", " ").Replace("E", " ")); }
                if (splitElement.Contains("F")) { F_headPos_new = Convert.ToInt32(splitElement.Replace(";", " ").Replace("F", " ")); }
            }
            //inicialise
            if (Z_headPos_new == Single.PositiveInfinity || E_headPos_new == Single.PositiveInfinity || F_headPos_new == -1)
            {
                if (gcodeLine.Contains("Z") && gcodeLine.Contains("F") && !gcodeLine.Contains("X") && !gcodeLine.Contains("Y") && !gcodeLine.Contains("E"))
                {
                    PositionFunction_ZF(gcodeLine, sw);
                }
                if (gcodeLine.Contains("F") && gcodeLine.Contains("E") && !gcodeLine.Contains("X") && !gcodeLine.Contains("Y") && !gcodeLine.Contains("Z"))
                {
                    PositionFunction_EF(gcodeLine, sw);
                }
                X_headPos = X_headPos_new;
                Y_headPos = Y_headPos_new;
                Z_headPos = Z_headPos_new;
                E_headPos = E_headPos_new;
                F_headPos = F_headPos_new;
            }
            //process
            else
            {
                if (gcodeLine.Contains("Z") && gcodeLine.Contains("F") && !gcodeLine.Contains("X") && !gcodeLine.Contains("Y") && !gcodeLine.Contains("E"))
                {
                    PositionFunction_ZF(gcodeLine, sw);
                }
                if (gcodeLine.Contains("F") && gcodeLine.Contains("E") && !gcodeLine.Contains("X") && !gcodeLine.Contains("Y") && !gcodeLine.Contains("Z"))
                {
                    PositionFunction_EF(gcodeLine, sw);
                }
                ////XY lines
                if (gcodeLine.Contains("X") && gcodeLine.Contains("Y"))
                {
                    //Console.WriteLine("Contain XY.");
                    slipumaKoeficients = (Y_headPos_new - Y_headPos) / (X_headPos_new - X_headPos);

                    if (float.IsPositiveInfinity(slipumaKoeficients)) { slipumsInfinity = 1; }
                    if (float.IsNegativeInfinity(slipumaKoeficients)) { slipumsInfinity = -1; }
                    if (!float.IsPositiveInfinity(slipumaKoeficients) && !float.IsNegativeInfinity(slipumaKoeficients)) { slipumsInfinity = 0; }

                    switch (slipumsInfinity)
                    {
                        case -1:
                            linearaNovirze = 0;
                            //Console.WriteLine("Infinity -");
                            break;
                        case 0:
                            linearaNovirze = Y_headPos - (slipumaKoeficients * X_headPos);
                            //Console.WriteLine("Function");
                            break;
                        case 1:
                            linearaNovirze = 0;
                            //Console.WriteLine("Infinity +");
                            break;
                        default:
                            linearaNovirze = 0;
                            break;
                    }

                    XCrossList.Clear();
                    YCrossList.Clear();
                    crossPointList.Clear();
                    extruderPosition.Clear();
                    //// X crosslist
                    XCrossList = new List<int>(crossListGenerator(X_headPos, X_headPos_new));
                    //// Y crosslist   
                    YCrossList = new List<int>(crossListGenerator(Y_headPos, Y_headPos_new));
                    //// Crosspoints

                    if (X_headPos != X_headPos_new && Y_headPos != Y_headPos_new)
                    {
                        //Console.WriteLine("Diagonal");
                        foreach (int i in XCrossList)
                        {
                            crossPointList.Add(new Tuple<float, float>(Convert.ToSingle(i), (slipumaKoeficients * i) + linearaNovirze));
                            //Console.WriteLine(Convert.ToSingle(i) + " " + (slipumaKoeficients * i) + linearaNovirze);
                        }
                        foreach (int i in YCrossList)
                        {
                            crossPointList.Add(new Tuple<float, float>((i - linearaNovirze) / slipumaKoeficients, Convert.ToSingle(i)));
                            //Console.WriteLine((i - linearaNovirze) / slipumaKoeficients + " " + Convert.ToSingle(i));
                        }

                        crossPointList.Sort((x, y) => y.Item1.CompareTo(x.Item1));
                        if ((Y_headPos_new - Y_headPos) > 0 && (X_headPos_new - X_headPos) > 0)
                        {
                            crossPointList.Reverse();
                        }
                        if ((Y_headPos_new - Y_headPos) > 0 && (X_headPos_new - X_headPos) < 0)
                        {
                            //crossPointList.Reverse(); //VVVV
                        }
                        if ((Y_headPos_new - Y_headPos) < 0 && (X_headPos_new - X_headPos) > 0)
                        {
                            crossPointList.Reverse(); //VVVV
                        }
                        if ((Y_headPos_new - Y_headPos) < 0 && (X_headPos_new - X_headPos) < 0)
                        {
                            //crossPointList.Reverse();
                        }

                    }
                    if (X_headPos == X_headPos_new && Y_headPos != Y_headPos_new)
                    {
                        //Console.WriteLine("Horizontal");
                        foreach (int i in YCrossList)
                        {
                            crossPointList.Add(new Tuple<float, float>(X_headPos_new, Convert.ToSingle(i)));
                            //Console.WriteLine(X_headPos_new + " " + Convert.ToSingle(i));

                        }

                    }
                    if (X_headPos != X_headPos_new && Y_headPos == Y_headPos_new)
                    {
                        //Console.WriteLine("Vertical");
                        foreach (int i in XCrossList)
                        {
                            crossPointList.Add(new Tuple<float, float>(Convert.ToSingle(i), Y_headPos_new));
                            //Console.WriteLine(Convert.ToSingle(i) + " " + Y_headPos_new);

                        }

                    }
                    ////Add destination

                    //Console.WriteLine(X_headPos_new + " " + Y_headPos_new);
                    crossPointList.Add(new Tuple<float, float>(X_headPos_new, Y_headPos_new));
                    foreach (Tuple<float, float> segment in crossPointList)
                    {
                        float distance = Convert.ToSingle(Math.Sqrt(((X_headPos_new - X_headPos) * (X_headPos_new - X_headPos)) + ((Y_headPos_new - Y_headPos) * (Y_headPos_new - Y_headPos))));
                        float distanceSegment = Convert.ToSingle(Math.Sqrt(((segment.Item1 - X_headPos) * (segment.Item1 - X_headPos)) + ((segment.Item2 - Y_headPos) * (segment.Item2 - Y_headPos))));
                        if (E_headPos_new != E_headPos)
                        {
                            extruderPosition.Add(Convert.ToSingle(Math.Truncate(Convert.ToDouble(((100000 * (E_headPos + ((E_headPos_new - E_headPos) * (distanceSegment / distance)))))))) / 100000);
                        }
                        else
                        {
                            extruderPosition.Add(E_headPos);
                        }

                    }
                    if (!extruderPosition.Any())
                    {
                        extruderPosition.Add(E_headPos_new);
                    }
                    bool firstFeed = true;
                    float PreviousItem1 = Single.PositiveInfinity;
                    float PreviousItem2 = Single.PositiveInfinity;
                    float PreviousExtrusion = Single.PositiveInfinity;
                    foreach (Tuple<float, float> crossPoint in crossPointList)
                    {
                        if (crossPoint.Item1 == PreviousItem1 && crossPoint.Item2 == PreviousItem2 && PreviousExtrusion == extruderPosition[0])
                        {
                            extruderPosition.RemoveAt(0);
                        }
                        else
                        {
                            string processedGcodeLine = "G1";
                            if (firstFeed == true)
                            {
                                processedGcodeLine = processedGcodeLine + " " + "X" + Convert.ToString(crossPoint.Item1) + " " + "Y" + Convert.ToString(crossPoint.Item2) + " " + "Z" + Convert.ToString(Z_headPos + zCorrection(crossPoint.Item1, crossPoint.Item2) + " " + "E" + extruderPosition[0] + " " + "F" + F_headPos_new);
                                firstFeed = false;
                            }
                            else
                            {
                                processedGcodeLine = processedGcodeLine + " " + "X" + Convert.ToString(crossPoint.Item1) + " " + "Y" + Convert.ToString(crossPoint.Item2) + " " + "Z" + Convert.ToString(Z_headPos + zCorrection(crossPoint.Item1, crossPoint.Item2) + " " + "E" + extruderPosition[0]);
                            }
                            sw.WriteLine(processedGcodeLine + ";");
                            PreviousItem1 = crossPoint.Item1;
                            PreviousItem2 = crossPoint.Item2;
                            PreviousExtrusion = extruderPosition[0];
                            extruderPosition.RemoveAt(0);
                            //Console.WriteLine("Gcode write.");
                        }

                    }

                    sw.WriteLine(";");
                    X_headPos = X_headPos_new;
                    Y_headPos = Y_headPos_new;
                    Z_headPos = Z_headPos_new;
                    E_headPos = E_headPos_new;
                    F_headPos = F_headPos_new;

                }
            }

        }
        public void midToMidmode(string gcodeLine, StreamWriter sw)
        {
            //Console.WriteLine("Here!");
            ////
            string[] splitGcodeLine = gcodeLine.Split(' ');
            foreach (string splitElement in splitGcodeLine)
            {
                //Console.WriteLine(gcodeLine);
                if (splitElement.Contains("X")) { X_headPos_new = Convert.ToSingle(splitElement.Replace(";", " ").Replace("X", " ")); }
                if (splitElement.Contains("Y")) { Y_headPos_new = Convert.ToSingle(splitElement.Replace(";", " ").Replace("Y", " ")); }
                if (splitElement.Contains("Z")) { Z_headPos_new = Convert.ToSingle(splitElement.Replace(";", " ").Replace("Z", " ")); }
                if (splitElement.Contains("E")) { E_headPos_new = Convert.ToSingle(splitElement.Replace(";", " ").Replace("E", " ")); }
                if (splitElement.Contains("F")) { F_headPos_new = Convert.ToInt32(splitElement.Replace(";", " ").Replace("F", " ")); }
            }
            //inicialise
            if (Z_headPos_new == Single.PositiveInfinity || E_headPos_new == Single.PositiveInfinity || F_headPos_new == -1)
            {
                if (gcodeLine.Contains("Z") && gcodeLine.Contains("F") && !gcodeLine.Contains("X") && !gcodeLine.Contains("Y") && !gcodeLine.Contains("E"))
                {
                    PositionFunction_ZF(gcodeLine, sw);
                }
                if (gcodeLine.Contains("F") && gcodeLine.Contains("E") && !gcodeLine.Contains("X") && !gcodeLine.Contains("Y") && !gcodeLine.Contains("Z"))
                {
                    PositionFunction_EF(gcodeLine, sw);
                }
                X_headPos = X_headPos_new;
                Y_headPos = Y_headPos_new;
                Z_headPos = Z_headPos_new;
                E_headPos = E_headPos_new;
                F_headPos = F_headPos_new;
            }
            //process
            else
            {
                if (gcodeLine.Contains("Z") && gcodeLine.Contains("F") && !gcodeLine.Contains("X") && !gcodeLine.Contains("Y") && !gcodeLine.Contains("E"))
                {
                    PositionFunction_ZF(gcodeLine, sw);
                }
                if (gcodeLine.Contains("F") && gcodeLine.Contains("E") && !gcodeLine.Contains("X") && !gcodeLine.Contains("Y") && !gcodeLine.Contains("Z"))
                {
                    PositionFunction_EF(gcodeLine, sw);
                }
                ////XY lines
                if (gcodeLine.Contains("X") && gcodeLine.Contains("Y"))
                {
                    //Console.WriteLine("Contain XY.");
                    slipumaKoeficients = (Y_headPos_new - Y_headPos) / (X_headPos_new - X_headPos);

                    if (float.IsPositiveInfinity(slipumaKoeficients)) { slipumsInfinity = 1; }
                    if (float.IsNegativeInfinity(slipumaKoeficients)) { slipumsInfinity = -1; }
                    if (!float.IsPositiveInfinity(slipumaKoeficients) && !float.IsNegativeInfinity(slipumaKoeficients)) { slipumsInfinity = 0; }

                    switch (slipumsInfinity)
                    {
                        case -1:
                            linearaNovirze = 0;
                            //Console.WriteLine("Infinity -");
                            break;
                        case 0:
                            linearaNovirze = Y_headPos - (slipumaKoeficients * X_headPos);
                            //Console.WriteLine("Function");
                            break;
                        case 1:
                            linearaNovirze = 0;
                            //Console.WriteLine("Infinity +");
                            break;
                        default:
                            linearaNovirze = 0;
                            break;
                    }

                    XCrossList.Clear();
                    YCrossList.Clear();
                    crossPointList.Clear();
                    midPointList.Clear();
                    extruderPosition.Clear();
                    //// X crosslist
                    XCrossList = new List<int>(crossListGenerator(X_headPos, X_headPos_new));
                    //// Y crosslist   
                    YCrossList = new List<int>(crossListGenerator(Y_headPos, Y_headPos_new));
                    //// Crosspoints

                    if (X_headPos != X_headPos_new && Y_headPos != Y_headPos_new)
                    {
                        //Console.WriteLine("Diagonal");
                        foreach (int i in XCrossList)
                        {
                            crossPointList.Add(new Tuple<float, float>(Convert.ToSingle(i), (slipumaKoeficients * i) + linearaNovirze));
                            //Console.WriteLine(Convert.ToSingle(i) + " " + (slipumaKoeficients * i) + linearaNovirze);
                        }
                        foreach (int i in YCrossList)
                        {
                            crossPointList.Add(new Tuple<float, float>((i - linearaNovirze) / slipumaKoeficients, Convert.ToSingle(i)));
                            //Console.WriteLine((i - linearaNovirze) / slipumaKoeficients + " " + Convert.ToSingle(i));
                        }
                        crossPointList.Sort((x, y) => y.Item1.CompareTo(x.Item1));
                        if ((Y_headPos_new - Y_headPos) > 0 && (X_headPos_new - X_headPos) > 0)
                        {
                            crossPointList.Reverse();
                        }
                        if ((Y_headPos_new - Y_headPos) > 0 && (X_headPos_new - X_headPos) < 0)
                        {
                            //crossPointList.Reverse(); //VVVV
                        }
                        if ((Y_headPos_new - Y_headPos) < 0 && (X_headPos_new - X_headPos) > 0)
                        {
                            crossPointList.Reverse(); //VVVV
                        }
                        if ((Y_headPos_new - Y_headPos) < 0 && (X_headPos_new - X_headPos) < 0)
                        {
                            //crossPointList.Reverse();
                        }
                    }
                    if (X_headPos == X_headPos_new && Y_headPos != Y_headPos_new)
                    {
                        //Console.WriteLine("Horizontal");
                        foreach (int i in YCrossList)
                        {
                            crossPointList.Add(new Tuple<float, float>(X_headPos_new, Convert.ToSingle(i)));
                            //Console.WriteLine(X_headPos_new + " " + Convert.ToSingle(i));

                        }
                    }
                    if (X_headPos != X_headPos_new && Y_headPos == Y_headPos_new)
                    {
                        //Console.WriteLine("Vertical");
                        foreach (int i in XCrossList)
                        {
                            crossPointList.Add(new Tuple<float, float>(Convert.ToSingle(i), Y_headPos_new));
                            //Console.WriteLine(Convert.ToSingle(i) + " " + Y_headPos_new);

                        }

                    }
                    ////Add destination
                    crossPointList.Add(new Tuple<float, float>(X_headPos_new, Y_headPos_new));
                    //Console.WriteLine(X_headPos_new + " " + Y_headPos_new);
                    midPointList.Add(new Tuple<float, float>((X_headPos + crossPointList[0].Item1) / 2, (Y_headPos + crossPointList[0].Item2) / 2));

                    for (int i = 0; i < crossPointList.Count - 1; i++)
                    {
                        midPointList.Add(new Tuple<float, float>((crossPointList[i].Item1 + crossPointList[i + 1].Item1) / 2, (crossPointList[i].Item2 + crossPointList[i + 1].Item2) / 2));
                    }
                    midPointList.Add(new Tuple<float, float>(X_headPos_new, Y_headPos_new));

                    foreach (Tuple<float, float> segment in midPointList)
                    {
                        float distance = Convert.ToSingle(Math.Sqrt(((X_headPos_new - X_headPos) * (X_headPos_new - X_headPos)) + ((Y_headPos_new - Y_headPos) * (Y_headPos_new - Y_headPos))));
                        float distanceSegment = Convert.ToSingle(Math.Sqrt(((segment.Item1 - X_headPos) * (segment.Item1 - X_headPos)) + ((segment.Item2 - Y_headPos) * (segment.Item2 - Y_headPos))));
                        if (E_headPos_new != E_headPos)
                        {
                            extruderPosition.Add(Convert.ToSingle(Math.Truncate(Convert.ToDouble(((100000 * (E_headPos + ((E_headPos_new - E_headPos) * (distanceSegment / distance)))))))) / 100000);
                        }
                        else
                        {
                            extruderPosition.Add(E_headPos);
                        }

                    }
                    if (!extruderPosition.Any())
                    {
                        extruderPosition.Add(E_headPos_new);
                    }
                    bool firstFeed = true;
                    float PreviousItem1 = Single.PositiveInfinity;
                    float PreviousItem2 = Single.PositiveInfinity;
                    float PreviousExtrusion = Single.PositiveInfinity;
                    foreach (Tuple<float, float> crossPoint in midPointList)
                    {
                        if (crossPoint.Item1 == PreviousItem1 && crossPoint.Item2 == PreviousItem2 && PreviousExtrusion == extruderPosition[0])
                        {
                            extruderPosition.RemoveAt(0);
                        }
                        else
                        {
                            string processedGcodeLine = "G1";
                            if (firstFeed == true)
                            {
                                processedGcodeLine = processedGcodeLine + " " + "X" + Convert.ToString(crossPoint.Item1) + " " + "Y" + Convert.ToString(crossPoint.Item2) + " " + "Z" + Convert.ToString(Z_headPos + zCorrection(crossPoint.Item1, crossPoint.Item2) + " " + "E" + extruderPosition[0] + " " + "F" + F_headPos_new);
                                firstFeed = false;
                            }
                            else
                            {
                                processedGcodeLine = processedGcodeLine + " " + "X" + Convert.ToString(crossPoint.Item1) + " " + "Y" + Convert.ToString(crossPoint.Item2) + " " + "Z" + Convert.ToString(Z_headPos + zCorrection(crossPoint.Item1, crossPoint.Item2) + " " + "E" + extruderPosition[0]);
                            }
                            sw.WriteLine(processedGcodeLine + ";");
                            PreviousItem1 = crossPoint.Item1;
                            PreviousItem2 = crossPoint.Item2;
                            PreviousExtrusion = extruderPosition[0];
                            extruderPosition.RemoveAt(0);
                            //Console.WriteLine("Gcode write.");
                        }

                    }

                    sw.WriteLine(";");
                    X_headPos = X_headPos_new;
                    Y_headPos = Y_headPos_new;
                    Z_headPos = Z_headPos_new;
                    E_headPos = E_headPos_new;
                    F_headPos = F_headPos_new;

                }
            }

        }
        public void halfStepMode(string gcodeLine, StreamWriter sw)
        {
            //Console.WriteLine("Here!");
            ////
            string[] splitGcodeLine = gcodeLine.Split(' ');
            foreach (string splitElement in splitGcodeLine)
            {
                //Console.WriteLine(gcodeLine);
                if (splitElement.Contains("X")) { X_headPos_new = Convert.ToSingle(splitElement.Replace(";", " ").Replace("X", " ")); }
                if (splitElement.Contains("Y")) { Y_headPos_new = Convert.ToSingle(splitElement.Replace(";", " ").Replace("Y", " ")); }
                if (splitElement.Contains("Z")) { Z_headPos_new = Convert.ToSingle(splitElement.Replace(";", " ").Replace("Z", " ")); }
                if (splitElement.Contains("E")) { E_headPos_new = Convert.ToSingle(splitElement.Replace(";", " ").Replace("E", " ")); }
                if (splitElement.Contains("F")) { F_headPos_new = Convert.ToInt32(splitElement.Replace(";", " ").Replace("F", " ")); }
            }
            //inicialise
            if (Z_headPos_new == Single.PositiveInfinity || E_headPos_new == Single.PositiveInfinity || F_headPos_new == -1)
            {
                if (gcodeLine.Contains("Z") && gcodeLine.Contains("F") && !gcodeLine.Contains("X") && !gcodeLine.Contains("Y") && !gcodeLine.Contains("E"))
                {
                    PositionFunction_ZF(gcodeLine, sw);
                }
                if (gcodeLine.Contains("F") && gcodeLine.Contains("E") && !gcodeLine.Contains("X") && !gcodeLine.Contains("Y") && !gcodeLine.Contains("Z"))
                {
                    PositionFunction_EF(gcodeLine, sw);
                }
                X_headPos = X_headPos_new;
                Y_headPos = Y_headPos_new;
                Z_headPos = Z_headPos_new;
                E_headPos = E_headPos_new;
                F_headPos = F_headPos_new;
            }
            //process
            else
            {
                if (gcodeLine.Contains("Z") && gcodeLine.Contains("F") && !gcodeLine.Contains("X") && !gcodeLine.Contains("Y") && !gcodeLine.Contains("E"))
                {
                    PositionFunction_ZF(gcodeLine, sw);
                }
                if (gcodeLine.Contains("F") && gcodeLine.Contains("E") && !gcodeLine.Contains("X") && !gcodeLine.Contains("Y") && !gcodeLine.Contains("Z"))
                {
                    PositionFunction_EF(gcodeLine, sw);
                }
                ////XY lines
                if (gcodeLine.Contains("X") && gcodeLine.Contains("Y"))
                {
                    //Console.WriteLine("Contain XY.");
                    slipumaKoeficients = (Y_headPos_new - Y_headPos) / (X_headPos_new - X_headPos);

                    if (float.IsPositiveInfinity(slipumaKoeficients)) { slipumsInfinity = 1; }
                    if (float.IsNegativeInfinity(slipumaKoeficients)) { slipumsInfinity = -1; }
                    if (!float.IsPositiveInfinity(slipumaKoeficients) && !float.IsNegativeInfinity(slipumaKoeficients)) { slipumsInfinity = 0; }

                    switch (slipumsInfinity)
                    {
                        case -1:
                            linearaNovirze = 0;
                            //Console.WriteLine("Infinity -");
                            break;
                        case 0:
                            linearaNovirze = Y_headPos - (slipumaKoeficients * X_headPos);
                            //Console.WriteLine("Function");
                            break;
                        case 1:
                            linearaNovirze = 0;
                            //Console.WriteLine("Infinity +");
                            break;
                        default:
                            linearaNovirze = 0;
                            break;
                    }

                    XCrossList.Clear();
                    YCrossList.Clear();
                    crossPointList.Clear();
                    midPointList.Clear();
                    halfStepList.Clear();
                    extruderPosition.Clear();
                    //// X crosslist
                    XCrossList = new List<int>(crossListGenerator(X_headPos, X_headPos_new));
                    //// Y crosslist   
                    YCrossList = new List<int>(crossListGenerator(Y_headPos, Y_headPos_new));
                    //// Crosspoints

                    if (X_headPos != X_headPos_new && Y_headPos != Y_headPos_new)
                    {
                        //Console.WriteLine("Diagonal");
                        foreach (int i in XCrossList)
                        {
                            crossPointList.Add(new Tuple<float, float>(Convert.ToSingle(i), (slipumaKoeficients * i) + linearaNovirze));
                            //Console.WriteLine(Convert.ToSingle(i) + " " + (slipumaKoeficients * i) + linearaNovirze);
                        }
                        foreach (int i in YCrossList)
                        {
                            crossPointList.Add(new Tuple<float, float>((i - linearaNovirze) / slipumaKoeficients, Convert.ToSingle(i)));
                            //Console.WriteLine((i - linearaNovirze) / slipumaKoeficients + " " + Convert.ToSingle(i));
                        }
                        crossPointList.Sort((x, y) => y.Item1.CompareTo(x.Item1));
                        if ((Y_headPos_new - Y_headPos) > 0 && (X_headPos_new - X_headPos) > 0)
                        {
                            crossPointList.Reverse();
                        }
                        if ((Y_headPos_new - Y_headPos) > 0 && (X_headPos_new - X_headPos) < 0)
                        {
                            //crossPointList.Reverse(); //VVVV
                        }
                        if ((Y_headPos_new - Y_headPos) < 0 && (X_headPos_new - X_headPos) > 0)
                        {
                            crossPointList.Reverse(); //VVVV
                        }
                        if ((Y_headPos_new - Y_headPos) < 0 && (X_headPos_new - X_headPos) < 0)
                        {
                            //crossPointList.Reverse();
                        }
                    }
                    if (X_headPos == X_headPos_new && Y_headPos != Y_headPos_new)
                    {
                        //Console.WriteLine("Horizontal");
                        foreach (int i in YCrossList)
                        {
                            crossPointList.Add(new Tuple<float, float>(X_headPos_new, Convert.ToSingle(i)));
                            //Console.WriteLine(X_headPos_new + " " + Convert.ToSingle(i));

                        }
                    }
                    if (X_headPos != X_headPos_new && Y_headPos == Y_headPos_new)
                    {
                        //Console.WriteLine("Vertical");
                        foreach (int i in XCrossList)
                        {
                            crossPointList.Add(new Tuple<float, float>(Convert.ToSingle(i), Y_headPos_new));
                            //Console.WriteLine(Convert.ToSingle(i) + " " + Y_headPos_new);

                        }

                    }
                    ////Add destination
                    crossPointList.Add(new Tuple<float, float>(X_headPos_new, Y_headPos_new));
                    //Console.WriteLine(X_headPos_new + " " + Y_headPos_new);
                    midPointList.Add(new Tuple<float, float>((X_headPos + crossPointList[0].Item1) / 2, (Y_headPos + crossPointList[0].Item2) / 2));


                    halfStepList.Add(new Tuple<float, float>(midPointList[0].Item1, midPointList[0].Item2));

                    for (int i = 0; i < crossPointList.Count - 1; i++)
                    {
                        halfStepList.Add(new Tuple<float, float>(crossPointList[i].Item1, crossPointList[i].Item2));
                        halfStepList.Add(new Tuple<float, float>((crossPointList[i].Item1 + crossPointList[i + 1].Item1) / 2, (crossPointList[i].Item2 + crossPointList[i + 1].Item2) / 2));
                    }

                    halfStepList.Add(new Tuple<float, float>(X_headPos_new, Y_headPos_new));

                    foreach (Tuple<float, float> segment in halfStepList)
                    {
                        float distance = Convert.ToSingle(Math.Sqrt(((X_headPos_new - X_headPos) * (X_headPos_new - X_headPos)) + ((Y_headPos_new - Y_headPos) * (Y_headPos_new - Y_headPos))));
                        float distanceSegment = Convert.ToSingle(Math.Sqrt(((segment.Item1 - X_headPos) * (segment.Item1 - X_headPos)) + ((segment.Item2 - Y_headPos) * (segment.Item2 - Y_headPos))));
                        if (E_headPos_new != E_headPos)
                        {
                            extruderPosition.Add(Convert.ToSingle(Math.Truncate(Convert.ToDouble(((100000 * (E_headPos + ((E_headPos_new - E_headPos) * (distanceSegment / distance)))))))) / 100000);
                        }
                        else
                        {
                            extruderPosition.Add(E_headPos);
                        }

                    }
                    if (!extruderPosition.Any())
                    {
                        extruderPosition.Add(E_headPos_new);
                    }
                    bool firstFeed = true;
                    float PreviousItem1 = Single.PositiveInfinity;
                    float PreviousItem2 = Single.PositiveInfinity;
                    float PreviousExtrusion = Single.PositiveInfinity;
                    foreach (Tuple<float, float> crossPoint in halfStepList)
                    {
                        if (crossPoint.Item1 == PreviousItem1 && crossPoint.Item2 == PreviousItem2 && PreviousExtrusion == extruderPosition[0])
                        {
                            extruderPosition.RemoveAt(0);
                        }
                        else
                        {
                            string processedGcodeLine = "G1";
                            if (firstFeed == true)
                            {
                                processedGcodeLine = processedGcodeLine + " " + "X" + Convert.ToString(crossPoint.Item1) + " " + "Y" + Convert.ToString(crossPoint.Item2) + " " + "Z" + Convert.ToString(Z_headPos + zCorrection(crossPoint.Item1, crossPoint.Item2) + " " + "E" + extruderPosition[0] + " " + "F" + F_headPos_new);
                                firstFeed = false;
                            }
                            else
                            {
                                processedGcodeLine = processedGcodeLine + " " + "X" + Convert.ToString(crossPoint.Item1) + " " + "Y" + Convert.ToString(crossPoint.Item2) + " " + "Z" + Convert.ToString(Z_headPos + zCorrection(crossPoint.Item1, crossPoint.Item2) + " " + "E" + extruderPosition[0]);
                            }
                            sw.WriteLine(processedGcodeLine + ";");
                            PreviousItem1 = crossPoint.Item1;
                            PreviousItem2 = crossPoint.Item2;
                            PreviousExtrusion = extruderPosition[0];
                            extruderPosition.RemoveAt(0);
                            //Console.WriteLine("Gcode write.");
                        }

                    }

                    sw.WriteLine(";");
                    X_headPos = X_headPos_new;
                    Y_headPos = Y_headPos_new;
                    Z_headPos = Z_headPos_new;
                    E_headPos = E_headPos_new;
                    F_headPos = F_headPos_new;

                }
            }

        }
        public List<int> crossListGenerator(float last, float current)
        {
            List<int> crosslist = new List<int>();
            crosslist.Clear();
            //Console.WriteLine("CrossList");
            if (last < current)
            {
                //Console.WriteLine("Last > Current");
                for (int i = (0 - Convert.ToInt32(printSize / 2)); i < printSize / 2; i = i + Convert.ToInt32(gridSize))
                {
                    if ((i > last && i < current) || i == last || i == current)
                    {
                        crosslist.Add(i);
                        //Console.WriteLine("- > +");
                    }
                }
            }
            if (last > current)
            {
                //Console.WriteLine("Current > Last");
                for (int i = Convert.ToInt32(printSize / 2); i > (0 - printSize / 2); i = i - Convert.ToInt32(gridSize))
                {
                    if ((i < last && i > current) || i == last || i == current)
                    {
                        crosslist.Add(i);
                        //Console.WriteLine("+ > -");
                    }
                }
            }
            return crosslist;
        }
        public void initialisePositionFunction(string gcodeLineInitialise, StreamWriter sw)
        {
            string processedGcodeLine = "G1";
            if (gcodeLineInitialise.Contains("X"))
            {
                string[] splitGcode = gcodeLineInitialise.Split(' ');
                foreach (string partialGcodeLine in splitGcode)
                {
                    if (partialGcodeLine.Contains("X"))
                    {
                        X_headPos = Convert.ToSingle(partialGcodeLine.Replace("X", ""));
                        Console.WriteLine("X:" + X_headPos);
                        initialisePosition = true;
                        processedGcodeLine = processedGcodeLine + " " + "X" + X_headPos;
                    }
                }
            }

            if (gcodeLineInitialise.Contains("Y"))
            {
                string[] splitGcode = gcodeLineInitialise.Split(' ');
                foreach (string partialGcodeLine in splitGcode)
                {
                    if (partialGcodeLine.Contains("Y"))
                    {
                        Y_headPos = Convert.ToSingle(partialGcodeLine.Replace("Y", ""));
                        Console.WriteLine("Y:" + Y_headPos);
                        initialisePosition = true;
                        processedGcodeLine = processedGcodeLine + " " + "Y" + Y_headPos;
                    }
                }
            }

            if (gcodeLineInitialise.Contains("Z"))
            {
                string[] splitGcode = gcodeLineInitialise.Split(' ');
                foreach (string partialGcodeLine in splitGcode)
                {
                    if (partialGcodeLine.Contains("Z"))
                    {
                        Z_headPos = Convert.ToSingle(partialGcodeLine.Replace("Z", "")) + zCorrection(X_headPos, Y_headPos);
                        Console.WriteLine("Z:" + Z_headPos);
                        processedGcodeLine = processedGcodeLine + " " + "Z" + Z_headPos;
                    }
                }
            }

            if (gcodeLineInitialise.Contains("E"))
            {
                string[] splitGcode = gcodeLineInitialise.Split(' ');
                foreach (string partialGcodeLine in splitGcode)
                {
                    if (partialGcodeLine.Contains("E"))
                    {
                        E_headPos = Convert.ToSingle(partialGcodeLine.Replace("E", ""));
                        Console.WriteLine("E:" + E_headPos);
                        processedGcodeLine = processedGcodeLine + " " + "E" + E_headPos;
                    }
                }
            }

            if (gcodeLineInitialise.Contains("F"))
            {
                string[] splitGcode = gcodeLineInitialise.Split(' ');
                foreach (string partialGcodeLine in splitGcode)
                {
                    if (partialGcodeLine.Contains("F"))
                    {
                        F_headPos = Convert.ToInt32(partialGcodeLine.Replace("F", ""));
                        Console.WriteLine("F:" + F_headPos);
                        processedGcodeLine = processedGcodeLine + " " + "F" + F_headPos;
                    }
                }
            }
            sw.WriteLine(processedGcodeLine + ";");
        }
        public void PositionFunction_ZF(string gcodeLine_ZF, StreamWriter sw)
        {
            string processedGcodeLine = "G1";
            if (true)
            {
                string[] splitGcode = gcodeLine_ZF.Split(' ');
                foreach (string partialGcodeLine in splitGcode)
                {
                    if (partialGcodeLine.Contains("Z"))
                    {
                        Z_headPos = Convert.ToSingle(partialGcodeLine.Replace("Z", "")) + zCorrection(X_headPos, Y_headPos);
                        //Console.WriteLine("Z:" + Z_headPos);
                        processedGcodeLine = processedGcodeLine + " " + "Z" + Z_headPos;
                        Z_headPos = Z_headPos - zCorrection(X_headPos, Y_headPos);
                    }
                }
            }

            if (true)
            {
                string[] splitGcode = gcodeLine_ZF.Split(' ');
                foreach (string partialGcodeLine in splitGcode)
                {
                    if (partialGcodeLine.Contains("F"))
                    {
                        F_headPos = Convert.ToInt32(partialGcodeLine.Replace("F", ""));
                        //Console.WriteLine("F:" + F_headPos);
                        processedGcodeLine = processedGcodeLine + " " + "F" + F_headPos;
                    }
                }
            }
            sw.WriteLine(processedGcodeLine + ";");
        }
        public void PositionFunction_EF(string gcodeLine_EF, StreamWriter sw)
        {
            string processedGcodeLine = "G1";
            if (gcodeLine_EF.Contains("E"))
            {
                string[] splitGcode = gcodeLine_EF.Split(' ');
                foreach (string partialGcodeLine in splitGcode)
                {
                    if (partialGcodeLine.Contains("E"))
                    {
                        E_headPos = Convert.ToSingle(partialGcodeLine.Replace("E", ""));
                        //Console.WriteLine("E:" + E_headPos);
                        processedGcodeLine = processedGcodeLine + " " + "E" + E_headPos;
                    }
                }
            }

            if (gcodeLine_EF.Contains("F"))
            {
                string[] splitGcode = gcodeLine_EF.Split(' ');
                foreach (string partialGcodeLine in splitGcode)
                {
                    if (partialGcodeLine.Contains("F"))
                    {
                        F_headPos = Convert.ToInt32(partialGcodeLine.Replace("F", ""));
                        //Console.WriteLine("F:" + F_headPos);
                        processedGcodeLine = processedGcodeLine + " " + "F" + F_headPos;
                    }
                }
            }
            sw.WriteLine(processedGcodeLine + ";");
        }
        public void button_showArray_Click(object sender, RoutedEventArgs e)
        {
            textBox_array.Clear();
            //textBox_array
            for (int i = 0; i < gridArrayProcess.GetLength(0); i++)
            {
                string arrayText = "";
                for (int j = 0; j < gridArrayProcess.GetLength(1); j++)
                {
                    if (gridArrayProcess[i, j] == 0f)
                    {
                        arrayText = arrayText + " " + "0.000" + " ";
                    }
                    else
                    {
                        if (gridArrayProcess[i, j] > 0f)
                        {
                            arrayText = arrayText + " " + Convert.ToString(gridArrayProcess[i, j]) + " ";
                        }
                        else
                        {
                            arrayText = arrayText + Convert.ToString(gridArrayProcess[i, j]) + " ";
                        }

                    }
                }
                if (checkBox_tsv.IsChecked == true)
                {
                    arrayText = arrayText.Replace("  ", " ");
                    arrayText = arrayText.Replace(" ", "\t");

                }
                textBox_array.AppendText(Convert.ToString(arrayText) + "\n");
            }
        }
        public float zCorrection(float X, float Y)
        {
            try
            {
                float correction = 0f;
                for (int i = 0; ((i * gridSize)) - (printSize / 2) < (printSize / 2) + (gridSize / 2); i++)
                {
                    if (Y <= ((i * gridSize)) - (printSize / 2))
                    {
                        for (int j = 0; ((j * gridSize)) - (printSize / 2) < (printSize / 2) + (gridSize / 2); j++)
                        {
                            if (X <= ((j * gridSize)) - (printSize / 2))
                            {

                                if (gridArrayProcess.GetLength(0) < j + 1 || 0 > j - 1 || gridArrayProcess.GetLength(1) < i + 1 || 0 > i - 1)
                                {
                                    break;
                                }

                                if (Math.Pow(((i * gridSize) - (printSize / 2)), 2) + Math.Pow(((j * gridSize) - (printSize / 2)), 2) >= Math.Pow((printSize - ((2 * gridSize) * Convert.ToSingle(textBox_clipEdgesScale.Text))) / 2, 2) && clipEdges == true)
                                {
                                    return 0;
                                }

                                if (radioButton_Z_correction_discrete.IsChecked == true)
                                {
                                    correction = gridArrayProcess[j - 1, i - 1];
                                    ////Console.WriteLine("Discrete " + correction);
                                    return correction * diminishingZ + Zoffset;
                                }


                                if (radioButton_Z_correction_continuous.IsChecked == true)
                                {
                                    /////Console.WriteLine("gridsize " + gridSize);
                                    float xCorrectionPos = (((j - 1) * gridSize)) - (printSize / 2);
                                    float yCorrectionPos = (((i - 1) * gridSize)) - (printSize / 2);

                                    if (X > xCorrectionPos || X == xCorrectionPos)
                                    {
                                        if (Y > yCorrectionPos || Y == yCorrectionPos)
                                        {
                                            ////Bi-linear interpolation
                                            float current = gridArrayProcess[j - 1, i - 1];
                                            float neighbourUp = gridArrayProcess[j - 1, i];
                                            float neighbourUpRight = gridArrayProcess[j, i];
                                            float neighbourRight = gridArrayProcess[j, i - 1];

                                            float squareArea = (gridSize) * (gridSize);

                                            float CurrentC = (area(X, Y, j - 1, i - 1, gridSize, printSize)) / squareArea;
                                            float UpC = (area(X, Y, j - 1, i, gridSize, printSize)) / squareArea;
                                            float UpRightC = (area(X, Y, j, i, gridSize, printSize)) / squareArea;
                                            float RightC = (area(X, Y, j, i - 1, gridSize, printSize)) / squareArea;


                                            //Console.WriteLine("Cont " + CurrentC + " " + UpC + " " + UpRightC + " " + RightC + " " + X + " " + Y);
                                            correction = (CurrentC * neighbourUpRight) + (UpC * neighbourRight) + (UpRightC * current) + (RightC * neighbourUp);
                                            //Console.WriteLine("Corr: " + current + " " + neighbourUp + " " + neighbourUpRight + " " + neighbourRight + " C:" + correction);
                                        }
                                        else
                                        {
                                            float current = gridArrayProcess[j - 1, i - 1];
                                            float neighbourDown = gridArrayProcess[j - 1, i - 2];
                                            float neighbourDownRight = gridArrayProcess[j, i - 2];
                                            float neighbourRight = gridArrayProcess[j, i - 1];

                                            float squareArea = (gridSize) * (gridSize);

                                            float CurrentC = (area(X, Y, j - 1, i - 1, gridSize, printSize)) / squareArea;
                                            float DownC = (area(X, Y, j - 1, i - 2, gridSize, printSize)) / squareArea;
                                            float DownRightC = (area(X, Y, j, i - 2, gridSize, printSize)) / squareArea;
                                            float RightC = (area(X, Y, j, i - 1, gridSize, printSize)) / squareArea;


                                            //Console.WriteLine("Cont " + CurrentC + " " + DownC + " " + DownRightC + " " + RightC + " " + X + " " + Y);
                                            correction = (CurrentC * neighbourDownRight) + (DownC * neighbourRight) + (DownRightC * current) + (RightC * neighbourDown);
                                            //Console.WriteLine("Corr: " + current + " " + neighbourDown + " " + neighbourDownRight + " " + neighbourRight + " C:" + correction);
                                        }

                                    }
                                    else
                                    {
                                        if (Y > yCorrectionPos || Y == yCorrectionPos)
                                        {
                                            ////Bi-linear interpolation
                                            float current = gridArrayProcess[j - 1, i - 1];
                                            float neighbourUp = gridArrayProcess[j - 1, i];
                                            float neighbourUpLeft = gridArrayProcess[j - 2, i];
                                            float neighbourLeft = gridArrayProcess[j - 2, i - 1];

                                            float squareArea = (gridSize) * (gridSize);

                                            float CurrentC = (area(X, Y, j - 1, i - 1, gridSize, printSize)) / squareArea;
                                            float UpC = (area(X, Y, j - 1, i, gridSize, printSize)) / squareArea;
                                            float UpLeftC = (area(X, Y, j - 2, i, gridSize, printSize)) / squareArea;
                                            float LeftC = (area(X, Y, j - 2, i - 1, gridSize, printSize)) / squareArea;


                                            //Console.WriteLine("Cont " + CurrentC + " " + UpC + " " + UpLeftC + " " + LeftC + " " + X + " " + Y);
                                            correction = (CurrentC * neighbourUpLeft) + (UpC * neighbourLeft) + (UpLeftC * current) + (LeftC * neighbourUp);
                                            //Console.WriteLine("Corr: " + current + " " + neighbourUp + " " + neighbourUpLeft + " " + neighbourLeft + " C:" + correction);
                                        }
                                        else
                                        {
                                            float current = gridArrayProcess[j - 1, i - 1];
                                            float neighbourDown = gridArrayProcess[j - 1, i - 2];
                                            float neighbourDownLeft = gridArrayProcess[j - 2, i - 2];
                                            float neighbourLeft = gridArrayProcess[j - 2, i - 1];

                                            float squareArea = (gridSize) * (gridSize);

                                            float CurrentC = (area(X, Y, j - 1, i - 1, gridSize, printSize)) / squareArea;
                                            float DownC = (area(X, Y, j - 1, i - 2, gridSize, printSize)) / squareArea;
                                            float DownLeftC = (area(X, Y, j - 2, i - 2, gridSize, printSize)) / squareArea;
                                            float LeftC = (area(X, Y, j - 2, i - 1, gridSize, printSize)) / squareArea;


                                            ////Console.WriteLine("Cont " + CurrentC + " " + DownC + " " + DownLeftC + " " + LeftC + " " + X + " " + Y);
                                            correction = (CurrentC * neighbourDownLeft) + (DownC * neighbourLeft) + (DownLeftC * current) + (LeftC * neighbourDown);
                                            //Console.WriteLine("Corr: " + current + " " + neighbourDown + " " + neighbourDownLeft + " " + neighbourLeft + " C:" + correction);
                                        }
                                    }

                                    ////Console.WriteLine("Cont " + correction);
                                    return correction * diminishingZ + Zoffset;
                                }
                            }
                        }
                    }
                }
            }

            catch
            {

            }
            return 0;
        }
        public float area(float X, float Y, float i, float j, float size, float compensation)
        {
            try
            {
                float Xpos = ((i * size) - (compensation / 2));
                float Ypos = ((j * size) - (compensation / 2));

                float value = Math.Abs((X - Xpos) * (Y - Ypos));
                //Console.WriteLine("X " + X);
                //Console.WriteLine("Y " + Y);
                //Console.WriteLine("Xpos " + ((i * size) - (compensation / 2)));
                //Console.WriteLine("Ypos " + ((j * size) - (compensation / 2)));
                //Console.WriteLine("Value " + value);

                return value;
            }
            catch
            {

            }
            return 0;
        }
        /// <summary>
        /// Tools
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void button_gcodeFragment_Click(object sender, RoutedEventArgs e)
        {
            tableData.Clear();
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".txt";
            dlg.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            try
            {
                // Get the selected file name and display in a TextBox 
                if (result == true)
                {
                    // Open document 
                    string filename = dlg.FileName;
                    textBox_gcodeFragment.Text = filename;
                    //MessageBox.Show("All is OK!");




                    StreamReader sr = new StreamReader(filename);
                    string str;
                    while ((str = sr.ReadLine()) != null)
                    {
                        string[] splitLine = str.Split(' ');
                        string Xpos = "";
                        string Ypos = "";
                        string Zpos = "";
                        string Epos = "";

                        foreach (string pos in splitLine)
                        {
                            if (pos.Contains("X")) { Xpos = pos.Replace("X", ""); }
                            if (pos.Contains("Y")) { Ypos = pos.Replace("Y", ""); }
                            if (pos.Contains("Z")) { Zpos = pos.Replace("Z", ""); }
                        }
                        tableData.Add(Xpos + "\t" + Ypos + "\t" + Zpos);


                    }


                    sr.Close();

                }
                StreamWriter sw;
                if (checkBox_defaultFilename.IsChecked == true)
                {
                    sw = new StreamWriter("TableData" + ".txt", false); ///true - saglabat, false - dzest info
                    foreach (string dataLine in tableData)
                    {
                        sw.WriteLine(dataLine);
                    }
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
            }
        }

        private void button_saveMap_Click(object sender, RoutedEventArgs e)
        {
            UIElement element = grid_universalGridMeasure as UIElement;
            Size size = element.RenderSize;
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96, 96, PixelFormats.Pbgra32);
            element.Measure(size);
            element.Arrange(new Rect(size));
            rtb.Render(element);
            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

            //save to memory stream
            System.IO.MemoryStream ms = new System.IO.MemoryStream();

            pngEncoder.Save(ms);
            ms.Close();
            System.IO.File.WriteAllBytes(textBox_fileName_universal.Text + ".png", ms.ToArray());
            Console.WriteLine("Done");



        }
        private void visualisation(Canvas currentCanvas, Canvas currentCanvasScale, float[,] currentArray)
        {
            multiplierColorScale = Convert.ToInt32(textBox_multiplierColorScale_Universal.Text);
            squareToGridScale = Convert.ToSingle(currentCanvas.Height / (Convert.ToDouble(printSize) + Convert.ToDouble(gridSize)));

            if (fineDraw == false)
            {
                currentCanvas.Children.Clear();
                for (int i = 0; i < currentArray.GetLength(0); i++)
                {
                    for (int j = 0; j < currentArray.GetLength(1); j++)
                    {
                        int color = 0;

                        if (float.IsNaN(currentArray[i, j]))
                        {
                            color = 64001;
                            currentArray[i, j] = 0;
                        }
                        else
                        {
                            color = Convert.ToInt32(Convert.ToSingle(currentArray[i, j]) * 1000f);
                        }

                        gridSquare = new System.Windows.Shapes.Rectangle();

                        gridSquare.ToolTip = currentArray[i, j];

                        ////greenish
                        if ((color > Convert.ToSingle(-10) && color < Convert.ToSingle(10)) || color == Convert.ToSingle(-10) || color == Convert.ToSingle(10))
                        {
                            gridSquare.Fill = new SolidColorBrush(Color.FromArgb(100, 127, 255, 127));
                            gridSquare.Stroke = new SolidColorBrush(Color.FromArgb(100, 127, 255, 127));
                        }
                        ////redish
                        if (color > 1)
                        {
                            if (color * multiplierColorScale / 10 > 255 || color * multiplierColorScale / 10 == 255)
                            {
                                gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                                gridSquare.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                            }
                            if (color * multiplierColorScale / 10 > 0 && color * multiplierColorScale / 10 < 255)
                            {
                                gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 255, Convert.ToByte(255 - (color * multiplierColorScale / 10)), Convert.ToByte(255 - (color * multiplierColorScale / 10))));
                                gridSquare.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, Convert.ToByte(255 - (color * multiplierColorScale / 10)), Convert.ToByte(255 - (color * multiplierColorScale / 10))));
                            }
                        }
                        ////blueish
                        if (color < -1)
                        {
                            if (color * multiplierColorScale / 10 < -255 || color * multiplierColorScale / 10 == -255)
                            {
                                gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
                                gridSquare.Stroke = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
                            }
                            if (color * multiplierColorScale / 10 < 0 && color * multiplierColorScale / 10 > -255)
                            {
                                gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, Convert.ToByte(255 - (color * (0 - multiplierColorScale) / 10)), Convert.ToByte(255 - (color * (0 - multiplierColorScale) / 10)), 255));
                                gridSquare.Stroke = new SolidColorBrush(Color.FromArgb(255, Convert.ToByte(255 - (color * (0 - multiplierColorScale) / 10)), Convert.ToByte(255 - (color * (0 - multiplierColorScale) / 10)), 255));
                            }
                        }
                        ////grayish
                        if (color == 64001)
                        {
                            gridSquare.Fill = new SolidColorBrush(Color.FromArgb(100, 100, 100, 100));
                            gridSquare.Stroke = new SolidColorBrush(Color.FromArgb(100, 100, 100, 100));
                        }

                        gridSquare.StrokeThickness = 0;
                        gridSquare.Width = Convert.ToDouble(gridSize) * Convert.ToDouble(squareToGridScale) + 1;
                        gridSquare.Height = Convert.ToDouble(gridSize) * Convert.ToDouble(squareToGridScale) + 1;
                        Canvas.SetLeft(gridSquare, (Convert.ToSingle(i * gridSize - printSize / 2) + (Convert.ToDouble(printSize) / 2)) * Convert.ToDouble(squareToGridScale));
                        Canvas.SetTop(gridSquare, ((Convert.ToDouble(printSize) - (Convert.ToSingle(j * gridSize - printSize / 2) + (Convert.ToDouble(printSize) / 2))) * Convert.ToDouble(squareToGridScale)));
                        currentCanvas.Children.Add(gridSquare);
                        if (checkBox_measureLabels_Universal.IsChecked == true)
                        {
                            TextBlock textBlock_labels = new TextBlock();
                            textBlock_labels.Text = Convert.ToString(Convert.ToString(Math.Truncate(Convert.ToDouble(Convert.ToSingle(currentArray[i, j])) * 100) / 100));
                            textBlock_labels.Foreground = Brushes.Black;
                            textBlock_labels.FontWeight = FontWeights.UltraBold;
                            textBlock_labels.FontSize = 14;
                            Canvas.SetLeft(textBlock_labels, (Convert.ToSingle(i * gridSize - printSize / 2) + (Convert.ToDouble(printSize) / 2)) * Convert.ToDouble(squareToGridScale));
                            Canvas.SetTop(textBlock_labels, ((Convert.ToDouble(printSize) - (Convert.ToSingle(j * gridSize - printSize / 2) + (Convert.ToDouble(printSize) / 2))) * Convert.ToDouble(squareToGridScale)));
                            currentCanvas.Children.Add(textBlock_labels);
                        }

                    }
                }



                ////Scale
                int scaleSegments = 23;
                int value = -300;
                float scaleSegmentWidth = Convert.ToSingle(currentCanvasScale.Width / scaleSegments);
                multiplierColorScale = Convert.ToInt32(textBox_multiplierColorScale_Universal.Text);

                for (int i = 0; i < scaleSegments; i++)
                {
                    value = value + 25;
                    gridSquare = new System.Windows.Shapes.Rectangle();
                    gridSquare.Stroke = new SolidColorBrush(Colors.Black);
                    gridSquare.StrokeThickness = 0;
                    gridSquare.ToolTip = Convert.ToString(Math.Truncate((Convert.ToSingle(value) * multiplierColorScale / 10000) * 100) / 100) + " mm";

                    ////greenish
                    if ((value > Convert.ToSingle(-10) && value < Convert.ToSingle(10)) || value == Convert.ToSingle(-10) || value == Convert.ToSingle(10))
                    {
                        gridSquare.Fill = new SolidColorBrush(Color.FromArgb(100, 127, 255, 127));
                    }
                    ////redish
                    if (value > 1)
                    {
                        if (value * multiplierColorScale / 10 > 255 || value * multiplierColorScale / 10 == 255)
                        {
                            gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                        }
                        if (value * multiplierColorScale / 10 > 0 && value * multiplierColorScale / 10 < 255)
                        {
                            gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 255, Convert.ToByte(255 - (value * multiplierColorScale / 10)), Convert.ToByte(255 - (value * multiplierColorScale / 10))));
                        }
                    }
                    ////blueish
                    if (value < -1)
                    {
                        if (value * multiplierColorScale / 10 < -255 || value * multiplierColorScale / 10 == -255)
                        {
                            gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
                        }
                        if (value * multiplierColorScale / 10 < 0 && value * multiplierColorScale / 10 > -255)
                        {
                            gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, Convert.ToByte(255 - (value * (0 - multiplierColorScale) / 10)), Convert.ToByte(255 - (value * (0 - multiplierColorScale) / 10)), 255));
                        }
                    }

                    gridSquare.Width = scaleSegmentWidth;
                    gridSquare.Height = currentCanvasScale.Height;
                    Canvas.SetLeft(gridSquare, scaleSegmentWidth * i);
                    Canvas.SetTop(gridSquare, 0);
                    currentCanvasScale.Children.Add(gridSquare);

                    TextBlock textBlock_labels = new TextBlock();

                    textBlock_labels.Text = Convert.ToString(Math.Truncate((Convert.ToSingle(value) * multiplierColorScale / 10000) * 100) / 100);
                    textBlock_labels.Foreground = Brushes.Black;
                    textBlock_labels.FontWeight = FontWeights.UltraBold;
                    textBlock_labels.FontSize = 8;
                    Canvas.SetLeft(textBlock_labels, scaleSegmentWidth * i);
                    if (i % 2 == 1)
                    {
                        Canvas.SetTop(textBlock_labels, 0);
                    }
                    else
                    {
                        Canvas.SetTop(textBlock_labels, currentCanvasScale.Height / 2);
                    }
                    currentCanvasScale.Children.Add(textBlock_labels);

                }

            }
            else
            {
                currentCanvas.Children.Clear();
                float refineFactor = Convert.ToSingle(textBox_subGrid.Text);
                float subDivision = (printSize / gridSize) * refineFactor;
                float subGridSize = gridSize / refineFactor;
                squareToGridScale = Convert.ToSingle(currentCanvas.Height / (Convert.ToDouble(printSize + subGridSize)));
                bool skipFirst = true;

                for (int i = 0 - (Convert.ToInt32(printSize / 2)); i <= printSize / 2; i = i + Convert.ToInt32(subGridSize))
                {
                    for (int j = 0 - (Convert.ToInt32(printSize / 2)); j <= printSize / 2; j = j + Convert.ToInt32(subGridSize))
                    {
                        float zValue = 0;

                        zValue = zCorrection(i, j);

                        int color = 0;



                        color = Convert.ToInt32(Convert.ToSingle(zValue) * 1000f);


                        gridSquare = new System.Windows.Shapes.Rectangle();
                        gridSquare.Stroke = new SolidColorBrush(Colors.Black);
                        gridSquare.StrokeThickness = 0;
                        gridSquare.ToolTip = zValue;

                        ////greenish
                        if ((color > Convert.ToSingle(-10) && color < Convert.ToSingle(10)) || color == Convert.ToSingle(-10) || color == Convert.ToSingle(10))
                        {
                            gridSquare.Fill = new SolidColorBrush(Color.FromArgb(100, 127, 255, 127));
                        }
                        ////redish
                        if (color > 1)
                        {
                            if (color * multiplierColorScale / 10 > 255 || color * multiplierColorScale / 10 == 255)
                            {
                                gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                            }
                            if (color * multiplierColorScale / 10 > 0 && color * multiplierColorScale / 10 < 255)
                            {
                                gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 255, Convert.ToByte(255 - (color * multiplierColorScale / 10)), Convert.ToByte(255 - (color * multiplierColorScale / 10))));
                            }
                        }
                        ////blueish
                        if (color < -1)
                        {
                            if (color * multiplierColorScale / 10 < -255 || color * multiplierColorScale / 10 == -255)
                            {
                                gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
                            }
                            if (color * multiplierColorScale / 10 < 0 && color * multiplierColorScale / 10 > -255)
                            {
                                gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, Convert.ToByte(255 - (color * (0 - multiplierColorScale) / 10)), Convert.ToByte(255 - (color * (0 - multiplierColorScale) / 10)), 255));
                            }
                        }
                        ////grayish
                        if (color == 64001)
                        {
                            gridSquare.Fill = new SolidColorBrush(Color.FromArgb(100, 100, 100, 100));
                        }
                        gridSquare.Width = Convert.ToDouble(subGridSize) * Convert.ToDouble(squareToGridScale) + 1;
                        gridSquare.Height = Convert.ToDouble(subGridSize) * Convert.ToDouble(squareToGridScale) + 1;
                        Canvas.SetLeft(gridSquare, (Convert.ToSingle(i) + (Convert.ToDouble(printSize) / 2)) * Convert.ToDouble(squareToGridScale));
                        Canvas.SetTop(gridSquare, ((Convert.ToDouble(printSize) - (Convert.ToSingle(j + (Convert.ToDouble(printSize) / 2)))) * Convert.ToDouble(squareToGridScale)));



                        currentCanvas.Children.Add(gridSquare);
                        if (checkBox_measureLabels_HeatMap.IsChecked == true)
                        {
                            TextBlock textBlock_labels = new TextBlock();
                            textBlock_labels.Text = Convert.ToString(Convert.ToString(Math.Truncate(Convert.ToDouble(Convert.ToSingle(zValue)) * 100) / 100));
                            textBlock_labels.Foreground = Brushes.Black;
                            textBlock_labels.FontWeight = FontWeights.UltraBold;
                            textBlock_labels.FontSize = 14;
                            Canvas.SetLeft(textBlock_labels, (Convert.ToSingle(i) + (Convert.ToDouble(printSize) / 2)) * Convert.ToDouble(squareToGridScale));
                            Canvas.SetTop(textBlock_labels, ((Convert.ToDouble(printSize) - (Convert.ToSingle(j + (Convert.ToDouble(printSize) / 2)))) * Convert.ToDouble(squareToGridScale)));
                            currentCanvas.Children.Add(textBlock_labels);
                        }

                    }
                }



                ////Scale
                int scaleSegments = 23;
                int value = -300;
                float scaleSegmentWidth = Convert.ToSingle(currentCanvasScale.Width / scaleSegments);
                multiplierColorScale = Convert.ToInt32(textBox_multiplierColorScale_Universal.Text);

                for (int i = 0; i < scaleSegments; i++)
                {
                    value = value + 25;
                    gridSquare = new System.Windows.Shapes.Rectangle();
                    gridSquare.Stroke = new SolidColorBrush(Colors.Black);
                    gridSquare.StrokeThickness = 0;
                    gridSquare.ToolTip = Convert.ToString(Math.Truncate((Convert.ToSingle(value) * multiplierColorScale / 10000) * 100) / 100) + " mm";

                    ////greenish
                    if ((value > Convert.ToSingle(-10) && value < Convert.ToSingle(10)) || value == Convert.ToSingle(-10) || value == Convert.ToSingle(10))
                    {
                        gridSquare.Fill = new SolidColorBrush(Color.FromArgb(100, 127, 255, 127));
                    }
                    ////redish
                    if (value > 1)
                    {
                        if (value * multiplierColorScale / 10 > 255 || value * multiplierColorScale / 10 == 255)
                        {
                            gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
                        }
                        if (value * multiplierColorScale / 10 > 0 && value * multiplierColorScale / 10 < 255)
                        {
                            gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 255, Convert.ToByte(255 - (value * multiplierColorScale / 10)), Convert.ToByte(255 - (value * multiplierColorScale / 10))));
                        }
                    }
                    ////blueish
                    if (value < -1)
                    {
                        if (value * multiplierColorScale / 10 < -255 || value * multiplierColorScale / 10 == -255)
                        {
                            gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, 0, 0, 255));
                        }
                        if (value * multiplierColorScale / 10 < 0 && value * multiplierColorScale / 10 > -255)
                        {
                            gridSquare.Fill = new SolidColorBrush(Color.FromArgb(255, Convert.ToByte(255 - (value * (0 - multiplierColorScale) / 10)), Convert.ToByte(255 - (value * (0 - multiplierColorScale) / 10)), 255));
                        }
                    }

                    gridSquare.Width = scaleSegmentWidth;
                    gridSquare.Height = currentCanvasScale.Height;
                    Canvas.SetLeft(gridSquare, scaleSegmentWidth * i);
                    Canvas.SetTop(gridSquare, 0);
                    currentCanvasScale.Children.Add(gridSquare);

                    TextBlock textBlock_labels = new TextBlock();

                    textBlock_labels.Text = Convert.ToString(Math.Truncate((Convert.ToSingle(value) * multiplierColorScale / 10000) * 100) / 100);
                    textBlock_labels.Foreground = Brushes.Black;
                    textBlock_labels.FontWeight = FontWeights.UltraBold;
                    textBlock_labels.FontSize = 8;
                    Canvas.SetLeft(textBlock_labels, scaleSegmentWidth * i);
                    if (i % 2 == 1)
                    {
                        Canvas.SetTop(textBlock_labels, 0);
                    }
                    else
                    {
                        Canvas.SetTop(textBlock_labels, currentCanvasScale.Height / 2);
                    }
                    currentCanvasScale.Children.Add(textBlock_labels);

                }
                fineDraw = false;
            }
        }

        private void button_generateHeatmap_Click(object sender, RoutedEventArgs e)
        {
            visualisation(canvas_heatmap, canvas_heatmapScale, gridArrayProcess);
        }

        private void button_fineMap_Click(object sender, RoutedEventArgs e)
        {
            fineDraw = true;
            if (checkBox_clipEdges.IsChecked == true)
            {
                clipEdges = true;
            }
            else
            {
                clipEdges = false;
            }
            visualisation(canvas_heatmap, canvas_heatmapScale, gridArrayProcess);

        }
    }
}
