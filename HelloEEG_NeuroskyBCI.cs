using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Threading;
using NeuroSky.ThinkGear;
using NeuroSky.ThinkGear.Algorithms;
using ZedGraph;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace BCI
{
    class Program
    {
        static event Action OnButtonPress;
        static int blinkCount = 0;
        static bool isCodeRunning = false;
        static int remainingTime;
        static int remainingTime2;
        static int remainingTime3;
        static int remainingTime4;
        static long lastBlinkTime = 0;
        static long FifthBlinkTime = 0;
        static long FourthBlinkTime = 0;
        static long ThirdBlinkTime = 0;
        static long SecondBlinkTime = 0;
        static Stopwatch stopwatch = new Stopwatch();
        static bool isExecutingCommand = false;
        static bool isUpdating = false;



        static Connector connector;
        static SerialPort port;
        static StreamWriter csvWriter;
        static ZedGraphControl zedGraphControl;
        static GraphPane myPane;
        static RollingPointPairList pointList;
        static LineItem lineItem;
        static int timen = 16000;
        static bool isWindowActive = false;
        static bool isButtonPressed = false;


        static int timeBetweenBlinks = 2000;
        static int timeToReset = 5000;
        static double newY;
        static double newX;


        //static int timeForTripleBlink = 2000; // m seconds in milliseconds

        static string csvLine;

        static bool isConnected = false;
        //static Dictionary<int, double> blinkData = new Dictionary<int, double>();
        static List<double> blinkDataDuringWindow = new List<double>();
        static int Pcount = 0;



        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Hello EEG!");

            // Initialize the CSV file and write the header
            string outputPath = "output.csv";
            csvWriter = new StreamWriter(outputPath);
            csvWriter.WriteLine("Time,BlinkStrength");

            // Initialize a new Connector and add event handlers
            connector = new Connector();
            connector.DeviceConnected += OnDeviceConnected;
            connector.DeviceConnectFail += OnDeviceFail;
            connector.DeviceValidating += OnDeviceValidating;
            // Register the OnButtonPressHandler method with the OnButtonPress event
            //OnButtonPress += OnButtonPressHandler;

            // Scan for devices across COM ports
            connector.ConnectScan("COM3");

            // Blink detection needs to be manually turned on
            connector.setBlinkDetectionEnabled(true);
            Thread.Sleep(1000);





            // Continue running the console for additional functionality
            while (!isConnected)
            {
                Console.WriteLine("Waiting for connection..."); // Add your console functionality here
                Thread.Sleep(1000); // Adjust as needed
            }
            // Use a timer to handle the 20-second window
            System.Threading.Timer windowTimer = new System.Threading.Timer(OnWindowTimerElapsed, null, 0, 1000);

            // Start the stopwatch when the timer starts
            //stopwatch.Start();
            // Start a thread for the Windows Forms application with live plotting
            Thread formsThread = new Thread(StartWindowsForms);

            formsThread.SetApartmentState(ApartmentState.STA);
            formsThread.Start();

            // Continue running the console for additional functionality
            while (!isButtonPressed)
            {
                Console.WriteLine("Waiting for button press..."); 
                Thread.Sleep(1000); 
            }

            Console.WriteLine("Start Blink! Button Pressed!");
            stopwatch.Start();
            //stopwatch.Reset();


            // Handle button press and start the window
            //StartNewWindowThread();

            // Keep the main thread alive until the timer elapses
            Thread.Sleep(Timeout.Infinite);



        }



        static void OnWindowTimerElapsed(object state)
        {
            //if (Pcount>0)
            //{
            if (isButtonPressed)
            {
                // Monitor blink count during the 15-second window
                int elapsedSeconds = (int)stopwatch.Elapsed.TotalSeconds;

                if (elapsedSeconds <= 15)
                {
                    Console.WriteLine($"Elapsed time: {elapsedSeconds} seconds");





                }
                else
                {
                    

                    ProcessBlinkData();

                    // Close the Windows Forms application gracefully
                    //Application.Exit();

                    stopwatch.Reset();


                    int els = (int)stopwatch.Elapsed.TotalSeconds;
                    if (els <= 15){
                        Console.WriteLine($"Elapsed time: {els} seconds");
                        stopwatch.Start();

                    }
                    


                }
            }
        }




        static void StartWindowsForms()
        {
            
            // Create a new form for the ZedGraphControl
            Form form = new Form();
            zedGraphControl = new ZedGraphControl();
            form.Controls.Add(zedGraphControl);
            // Set the size of the form
            form.Size = new Size(1000, 600); // Adjust width and height as needed

            // Initialize ZedGraph settings
            myPane = zedGraphControl.GraphPane;
            // Assuming zedGraphControl is your ZedGraphControl instance
            zedGraphControl.Size = new System.Drawing.Size(1000, 600); // Set the width and height as needed



            myPane.Title.Text = "Blink Strength";
            myPane.XAxis.Title.Text = "Time";
            myPane.YAxis.Title.Text = "Blink Strength";
            myPane.YAxis.Scale.Min = -10;  // Adjust the minimum value as needed
            myPane.YAxis.Scale.Max = 300; // Adjust the maximum value as needed

            myPane.YAxis.Scale.MinAuto = true; // Auto-scale the Y-axis
            myPane.YAxis.Scale.MaxAuto = true;
            myPane.XAxis.MajorGrid.IsVisible = true; // Enable vertical grid lines
            myPane.YAxis.MajorGrid.IsVisible = true; // Enable horizontal grid lines

            pointList = new RollingPointPairList(1000);

            lineItem = myPane.AddCurve("Blink Strength", pointList, Color.Blue, SymbolType.Square);
            lineItem.Line.Width = 2;  // Adjust the line width as needed
            lineItem.Symbol.Size = 2;  // Adjust the point size as needed
                                       // Start the ZedGraph timer for recording
            myPane.Rect = new RectangleF(1, 1, 900, 500);



            myPane.Margin.Left = 50;   // Set the left margin to 50 pixels
            myPane.Margin.Right = 20;  // Set the right margin to 20 pixels
            myPane.Margin.Top = 30;    // Set the top margin to 30 pixels
            myPane.Margin.Bottom = 40; // Set the bottom margin to 40 pixels





            // Display the form with ZedGraphControl
            Application.Run(form);

           
        }

     




        static void ProcessBlinkData()
        {

            if (newY > 30)
            {
                blinkCount++;

                long currentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;


       
                if (currentTime - ThirdBlinkTime <= timeBetweenBlinks && ThirdBlinkTime - SecondBlinkTime <= timeToReset && SecondBlinkTime - FourthBlinkTime <= timeBetweenBlinks && blinkCount == 4 && !isExecutingCommand)
                {
                    ThreadPool.QueueUserWorkItem(state =>
                    {

                        // Calculate the remaining time until timen
                        remainingTime = (int)(timen - stopwatch.ElapsedMilliseconds);
                        if (remainingTime > 0)
                        {
                            // Sleep for the remaining time
                            Thread.Sleep(remainingTime);



                            //Thread.Sleep(timen);
                            Console.WriteLine("Four Blinks");
                            SendCommandToArduino(1);
                        }


                    });
                    blinkCount = 0;

                }
                else if (currentTime - ThirdBlinkTime > timeBetweenBlinks && currentTime - ThirdBlinkTime <= timeToReset && blinkCount == 2 && !isExecutingCommand)
                {
                    ThreadPool.QueueUserWorkItem(state =>
                    {

                        //Thread.Sleep(timen);

                        // Calculate the remaining time until timen
                        remainingTime2 = (int)(timen - stopwatch.ElapsedMilliseconds);
                        if (remainingTime2 > 0)
                        {
                            // Sleep for the remaining time
                            Thread.Sleep(remainingTime2);


                            Console.WriteLine("Double Blinks");
                            SendCommandToArduino(2);
                        }

                    });
                    blinkCount = 0;

                }



                else
                {
                    // This is the first blink of a potential multiple blink
                    // blinkCount = 1;
                    isExecutingCommand = false; // Reset the flag as no command is being executed
                }



                FifthBlinkTime = FourthBlinkTime;
                FourthBlinkTime = SecondBlinkTime;
                SecondBlinkTime = ThirdBlinkTime;
                ThirdBlinkTime = currentTime;
                //Console.WriteLine($"FourthBlinkTime {FourthBlinkTime}SecondBlinkTime {SecondBlinkTime} ThirdBlinkTime {ThirdBlinkTime} currentTime {currentTime}");




            }


            else
            {
                long NewCurrentTime = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                if (blinkCount == 1 && NewCurrentTime - ThirdBlinkTime > timeToReset)
                {
                    remainingTime3 = (int)(timen - stopwatch.ElapsedMilliseconds);
                    if (remainingTime3 > 0)
                    {
                        // Sleep for the remaining time
                        Thread.Sleep(remainingTime3);
                    }

                    ThreadPool.QueueUserWorkItem(state =>
                    {

                        //Thread.Sleep(timen);



                        Console.WriteLine("Single Blinks");
                        SendCommandToArduino(3);


                    });


                    isExecutingCommand = true; // Set the flag to indicate that a command is being executed
                    blinkCount = 0;

                }
                else if (blinkCount == 2 && NewCurrentTime - ThirdBlinkTime > timeToReset && !isExecutingCommand)
                {
                    remainingTime4 = (int)(timen - stopwatch.ElapsedMilliseconds);
                    if (remainingTime4 > 0)
                    {
                        // Sleep for the remaining time
                        Thread.Sleep(remainingTime4);
                    }
                    ThreadPool.QueueUserWorkItem(state =>
                    {

                        //Thread.Sleep(timen);




                        Console.WriteLine("Double Blinks through 2 sec");
                        SendCommandToArduino(4);


                    });
                    blinkCount = 0;

                }
                blinkCount = 0;
                //lastBlinkTime = 0; // Reset lastBlinkTime
                // }
            }
            
        }
        

        static void SendCommandToArduino(int command)
        {



            if (command == 1)
            {
                // Send command 1 to Arduino
                port.WriteLine(1 + ";" + 0 + ";");
                Console.WriteLine(1);
                Console.WriteLine($"Sending command {command} to Arduino at {DateTime.Now}");
            }
            else if (command == 2)
            {
                // Send command 2 to Arduino
                port.WriteLine(2 + ";" + 0 + ";");
                Console.WriteLine(2);
                Console.WriteLine($"Sending command {command} to Arduino at {DateTime.Now}");
            }
            else if (command == 3)
            {
                // Send command 2 to Arduino
                port.WriteLine(3 + ";" + 0 + ";");
                Console.WriteLine(3);
                Console.WriteLine($"Sending command {command} to Arduino at {DateTime.Now}");
            }
            else if (command == 4)
            {
                // Send command 2 to Arduino
                port.WriteLine(4 + ";" + 0 + ";");
                Console.WriteLine(4);
                Console.WriteLine($"Sending command {command} to Arduino at {DateTime.Now}");
            }
            isExecutingCommand = true; // Set the flag to indicate that a command is being executed
        }



        static void OnDeviceConnected(object sender, EventArgs e)
        {
            Connector.DeviceEventArgs de = (Connector.DeviceEventArgs)e;
            Console.WriteLine("Device found on: " + de.Device.PortName);
            Console.WriteLine("Connecting to Arduino");

            port = new SerialPort("COM6", 115200, Parity.None, 8, StopBits.One);
            port.Open();
            Console.WriteLine("Connected to Arduino");

            de.Device.DataReceived += OnDataReceived;
            isConnected = true;


        }


        static void OnDeviceFail(object sender, EventArgs e)
        {
            Console.WriteLine("No devices found! :(");
        }

        static void OnDeviceValidating(object sender, EventArgs e)
        {
            Console.WriteLine("Validating: ");
        }

        static void OnDataReceived(object sender, EventArgs e)
        {


            // Inside the OnDataReceived method
            string data = port.ReadLine();

            if (data.StartsWith("ButtonPressed"))
            {

                //Console.WriteLine("Button pressed received from Arduino.");
                isButtonPressed = true;
                Pcount++;



            }

 


            Device.DataEventArgs de = (Device.DataEventArgs)e;
            DataRow[] tempDataRowArray = de.DataRowArray;

            TGParser tgParser = new TGParser();
            tgParser.Read(de.DataRowArray);

            // Simulate data for the graph (replace this with actual data)

            //if (Pcount > 0)
            //{
            for (int i = 0; i < tgParser.ParsedData.Length; i++)
            {

               

                    if (tgParser.ParsedData[i].ContainsKey("BlinkStrength"))
                    {
                        Console.WriteLine("Blink Value:" + tgParser.ParsedData[i]["BlinkStrength"]);
                        newY = tgParser.ParsedData[i]["BlinkStrength"];

                        csvLine = $"{DateTime.Now}, {tgParser.ParsedData[i]["BlinkStrength"]}";

                        ProcessBlinkData();

                    }
                    else if (!tgParser.ParsedData[i].ContainsKey("BlinkStrength"))
                    {
                        // Handle non-blink data
                        newY = 0;
                        csvWriter.WriteLine($"{DateTime.Now}, {0}");

                    }
                    newX = (double)new XDate(DateTime.Now);
                    csvWriter.WriteLine($"{DateTime.Now}, {newY}");
                    pointList.Add(newX, newY);
                    zedGraphControl.AxisChange();
                    zedGraphControl.Invalidate();



                    
                }
            csvWriter.Flush();

        }
        static void StartNewWindowThread()
        {
            // Ensure only one new window is started
            if (!isUpdating)
            {
                isUpdating = true;
                Thread newFormsThread = new Thread(StartWindowsForms);
                newFormsThread.SetApartmentState(ApartmentState.STA);
                newFormsThread.Start();
            }


        }



    }
}
