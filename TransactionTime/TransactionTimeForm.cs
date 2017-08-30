using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using IOLibrary;
using TrainLibrary;

namespace TransactionTime
{
    public partial class TransactionTimeFrom : Form
    {
        /* Timer parameters to keep track of execution time. */
        private int timeCounter = 0;
        private bool stopTheClock = false;

        /* Constant time factors. */
        public const double secPerHour = 3600;
        public const double minutesPerHour = 60;
        public const double secPerMinute = 60;


        public TransactionTimeFrom()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Select the data file
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void dataFileButton_Click(object sender, EventArgs e)
        {
            /* Select the data file. */
            Settings.dataFile = Tools.selectDataFile(caption: "Select the data file.");
            dataFilename.Text = Path.GetFileName(Settings.dataFile);
            dataFilename.ForeColor = System.Drawing.Color.Black;
        }

        /// <summary>
        /// Select the geometry file.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void geometryFileButton_Click(object sender, EventArgs e)
        {
            /* Select the geometry file. */
            Settings.geometryFile = Tools.selectDataFile(caption: "Select the geometry file.");
            geometryFilename.Text = Path.GetFileName(Settings.geometryFile);
            geometryFilename.ForeColor = System.Drawing.Color.Black;
        }

        /// <summary>
        /// Select a simulation file
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void simulationFileButton1_Click(object sender, EventArgs e)
        {
            /* Add the simualtion file to the list at the correct index. */
            setSimulationFile(simulation1Filename, 0);
        }

        /// <summary>
        /// Select a simulation file
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void simulationFileButton2_Click(object sender, EventArgs e)
        {
            /* Add the simualtion file to the list at the correct index. */
            setSimulationFile(simulation2Filename, 1);
        }

        /// <summary>
        /// Select a simulation file
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void simulationFileButton3_Click(object sender, EventArgs e)
        {
            /* Add the simualtion file to the list at the correct index. */
            setSimulationFile(simulation3Filename, 2);
        }

        /// <summary>
        /// Select a simulation file
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void simulationFileButton4_Click(object sender, EventArgs e)
        {
            /* Add the simualtion file to the list at the correct index. */
            setSimulationFile(simulation4Filename, 3);
        }

        /// <summary>
        /// Helper function to set the simulation file parameters and add the simulation file into the list in the correct index.
        /// </summary>
        /// <param name="simulationFile">Form object to populate.</param>
        /// <param name="index">Insertion index into the simulation file list.</param>
        private void setSimulationFile(TextBox simulationFile, int index)
        {
            string filename = null;
            string direction = null;
            /* Extract the simulation Category. */
            string Category = getSimulationCategory(index);

            /* Determine the direction of the simulation. */
            if ((index % 2) == 0)
                direction = "increasing";
            else
                direction = "decreasing";

            /* Create a meaningful string to help user identify the correct file. */
            string browseString = "Select the " + Category + " " + direction + " km simulation file.";

            /* Select the simulation file using the browser and insert into the simulation file list. */
            filename = Tools.selectDataFile(caption: browseString);
            Settings.simulationFiles[index] = filename;
            simulationFile.Text = Path.GetFileName(filename);
            simulationFile.ForeColor = System.Drawing.Color.Black;
        }
        
        /// <summary>
        /// Identify the simulation Category based on the index in the list.
        /// </summary>
        /// <param name="index">Index of the simulation Category</param>
        /// <returns>A string identifying the simulation Category.</returns>
        private string getSimulationCategory(int index)
        {
            /* Preliminiary function to set simulation catagories.
             * If the operator catagory object are required, the code below is kept for use.
             */

            /* Identify which simulation Category is being selected. */
            //if ((Operator1Category.SelectedItem != null || Operator2Category.SelectedItem != null || Operator3Category.SelectedItem != null) &&
            //    (Operator1Category.Text != "" || Operator2Category.Text != "" || Operator3Category.Text != ""))
            //{
            //    /* Return the appropriate operator. */
            //    if (index < 2)
            //        return Operator1Category.SelectedItem.ToString();
            //    else if (index < 4)
            //        return Operator2Category.SelectedItem.ToString();
            //    else
            //        return Operator3Category.SelectedItem.ToString();
            //}
            //else if ((Commodity1Category.SelectedItem != null || Commodity2Category.SelectedItem != null || Commodity3Category.SelectedItem != null) &&
            //    (Commodity1Category.Text != "" || Commodity2Category.Text != "" || Commodity3Category.Text != ""))
            //{
            //    /* Return the appropriate Commodity. */
            //    if (index < 2)
            //        return Commodity1Category.SelectedItem.ToString();
            //    else if (index < 4)
            //        return Commodity2Category.SelectedItem.ToString();
            //    else
            //        return Commodity3Category.SelectedItem.ToString();
            //}
            //else

            if (Settings.analysisCategory == analysisCategory.TrainOperator)
            {
                /* Return the appropriate power to weight Category. */
                if (index < 2)
                    return "Pacific National";
                else if (index < 4)
                    return "Aurizon";
                else
                    return "Alternative";
            }
            //else if (Settings.analysisCategory == analysisCategory.TrainCommodity)
            //{
            //    /* Return the appropriate power to weight Category. */
            //    if (index < 2)
            //        return "";
            //    else if (index < 4)
            //        return "";
            //    else
            //        return "";
            //}
            else if (Settings.analysisCategory == analysisCategory.TrainPowerToWeight)
            {
                /* Return the appropriate power to weight Category. */
                if (index < 2)
                    return "Underpowered";
                else if (index < 4)
                    return "Overpowered";
                else
                    return "Alternative";
            }
            else 
            {
                return "unknown";
            }
        }

        /// <summary>
        /// Select the desination folder to save all the files to.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void directoryButton_Click(object sender, EventArgs e)
        {
            /* Browse the folders for the desired desination folder. */
            Settings.aggregatedDestination = Tools.selectFolder();
            destinationDirectory.Text = Settings.aggregatedDestination;
            destinationDirectory.ForeColor = System.Drawing.Color.Black;
        }

        /// <summary>
        /// Extract the date range for the data.
        /// </summary>
        /// <returns>A 2-element array containig the start and end date to consider.</returns>
        public DateTime[] getDateRange() { return new DateTime[2] { fromDate.Value, toDate.Value }; }

        /// <summary>
        /// Extract the value of the start km for interpolation.
        /// </summary>
        /// <returns>The start km.</returns>
        public double getStartKm()
        {
            double startKm;
            if (double.TryParse(startInterpolationKm.Text, out startKm))
                return startKm;

            return 0;
        }

        /// <summary>
        /// Extract the value of the end km for interpolation.
        /// </summary>
        /// <returns>The end km.</returns>
        public double getEndKm()
        {
            double endKm;
            if (double.TryParse(endInterpolationKm.Text, out endKm))
                return endKm;

            return 0;
        }

        /// <summary>
        /// Extract the value of the interpolation interval.
        /// </summary>
        /// <returns>The interpolation interval.</returns>
        public double getInterval()
        {
            double interval;
            if (double.TryParse(interpolationInterval.Text, out interval))
                return interval;

            return 0;
        }

        /// <summary>
        /// Extract the value of the minimum journey distance threshold.
        /// </summary>
        /// <returns>The minimum distance of the train journey.</returns>
        public double getJourneydistance()
        {
            double journeyDistance;
            if (double.TryParse(minimumJourneyDistance.Text, out journeyDistance))
                return journeyDistance * 1000;

            return 0;
        }

        /// <summary>
        /// Extract the value of the time difference between succesive data points to be considered as seperate trains.
        /// </summary>
        /// <returns>The time difference in minutes.</returns>
        public double getTimeSeparation()
        {
            double timeDifference;
            if (double.TryParse(timeSeparation.Text, out timeDifference))
                return timeDifference * 60;

            return 0;
        }

        /// <summary>
        /// Extract the value of the minimum distance between successive data points.
        /// </summary>
        /// <returns>The minimum distance threshold.</returns>
        public double getDataSeparation()
        {
            double distance;
            if (double.TryParse(dataSeparation.Text, out distance))
                return distance * 1000;

            return 0;
        }

        /// <summary>
        /// Extract the value of the allowable time difference between the stopped train restarting and the through train.
        /// </summary>
        /// <returns>The time difference in minutes.</returns>
        public double getThroguhTrainTimeSeperation()
        {
            double timeDifference;
            if (double.TryParse(throughTrainTime.Text, out timeDifference))
                return timeDifference;

            return 0;
        }
        
        /// <summary>
        /// Extract the value of the assumed train length.
        /// </summary>
        /// <returns>The assumed length of the trains.</returns>
        public double getTrainLength()
        {
            double trainLength;
            if (double.TryParse(trainLengthKm.Text, out trainLength))
                return trainLength;

            return 0;
        }

        /// <summary>
        /// Extract the value of the loop speed factor for comparison to the simulation data.
        /// </summary>
        /// <returns>The comparison factor.</returns>
        public double getTrackSpeedFactor()
        {
            double trackSpeedFactor;
            if (double.TryParse(trackSpeedPercent.Text, out trackSpeedFactor))
                return trackSpeedFactor / 100.0;

            return 0;
        }

        /// <summary>
        /// Extract the value maximum permissable distance to achieve track speed.
        /// </summary>
        /// <returns>The assumed length of the trains.</returns>
        public double getMaxDistanceToTrackSpeed()
        {
            double distance;
            if (double.TryParse(maxDistanceToTrackSpeed.Text, out distance))
                return distance;

            return 0;
        }

        /// <summary>
        /// Extract the stopping speed threshold
        /// </summary>
        /// <returns>The assumed stopping speed of the train.</returns>
        public double getStoppingSpeed()
        {
            double speed;
            if (double.TryParse(stoppingThreshold.Text, out speed))
                return speed;

            return 0;
        }

        /// <summary>
        /// Extract the restart speed threshold
        /// </summary>
        /// <returns>The assumed restart speed of the train.</returns>
        public double getRestartSpeed()
        {
            double speed;
            if (double.TryParse(restartThreshold.Text, out speed))
                return speed;

            return 0;
        }

        /// <summary>
        /// Extract the maximum permissible transaction time
        /// </summary>
        /// <returns>The maximum premissible transaction time.</returns>
        public double getMaxTransactionTime()
        {
            double time;
            if (double.TryParse(transactionTimeOutlier.Text, out time))
                return time;

            return 0;
        }

        /// <summary>
        /// Validate the form parameters.
        /// </summary>
        /// <returns>True if all parameters are valid.</returns>
        public bool areParametersValid()
        {
            if (Settings.dataFile == null)
                return false;

            if (Settings.geometryFile == null)
                return false;

            if (Settings.dateRange == null ||
                Settings.dateRange[0] > DateTime.Today || Settings.dateRange[1] > DateTime.Today ||
                Settings.dateRange[0] > Settings.dateRange[1])
                return false;

            if (Settings.startInterpolationKm < 0 || Settings.startInterpolationKm > Settings.endInterpolationKm)
                return false;

            if (Settings.endInterpolationKm < 0 || Settings.endInterpolationKm < Settings.startInterpolationKm)
                return false;

            if (Settings.interpolationInterval < 0)
                return false;

            if (Settings.minimumJourneyDistance < 0)
                return false;

            if (Settings.timethreshold < 0)
                return false;

            if (Settings.throughTrainTime < 0)
                return false;

            if (Settings.distanceThreshold < 0)
                return false;

            if (Settings.trainLength < 0)
                return false;

            if (Settings.trackSpeedFactor < 0 || Settings.trackSpeedFactor >  1)
                return false;

            if (Settings.maxDistanceToTrackSpeed < 0)
                return false;

            if (Settings.stoppingSpeedThreshold < 0)
                return false;

            if (Settings.restartSpeedThreshold < 0)
                return false;

            if (Settings.transactionTimeOutlierThreshold < 0)
                return false;

            return true;
        }

        /// <summary>
        /// Assigne the form parameters to the Settings parameters.
        /// </summary>
        /// <param name="form">The current form object.</param>
        public void setFormParameters(TransactionTimeFrom form)
        {
            Settings.dateRange = form.getDateRange();
            Settings.startInterpolationKm = form.getStartKm();
            Settings.endInterpolationKm = form.getEndKm();
            Settings.interpolationInterval = form.getInterval();
            Settings.minimumJourneyDistance = form.getJourneydistance();
            Settings.timethreshold = form.getTimeSeparation();
            Settings.throughTrainTime = form.getThroguhTrainTimeSeperation();
            Settings.distanceThreshold = form.getDataSeparation();
            Settings.trainLength = form.getTrainLength();
            Settings.trackSpeedFactor = form.getTrackSpeedFactor();
            Settings.maxDistanceToTrackSpeed = form.getMaxDistanceToTrackSpeed();
            Settings.stoppingSpeedThreshold = form.getStoppingSpeed();
            Settings.restartSpeedThreshold = form.getRestartSpeed();
            Settings.transactionTimeOutlierThreshold = form.getMaxTransactionTime();
        }

        /// <summary>
        /// Begin process to calaculate the transaction time of the specified corridor.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void ProcessButton_Click(object sender, EventArgs e)
        {
            /* Create a Timer. */
            Timer timer = new Timer();
            timer.Interval = 1000;                      // Set the tick interval to 1 second.
            timer.Enabled = true;                       // Set the time to be running.
            timer.Tag = executionTime;                  // Set the timer label
            timer.Tick += new EventHandler(tickTimer);  // Event handler function.

            /* Start the timer. */
            timer.Start();

            /* Set up the background threads to run asynchronously. */
            BackgroundWorker background = new BackgroundWorker();

            background.DoWork += (backgroundSender, backgroundEvents) =>
                {
                    /* Set the form parameters */
                    setFormParameters(this);
                    if (areParametersValid())
                        Algorithm.transactionTime();
                    else
                        Tools.messageBox("Some form parameters are invalid", "Invalid Form Parameters");
                };
            
            background.RunWorkerCompleted += (backgroundSender, backgroundEvents) =>
            {

                /* When asynchronous execution complete, reset the timer counter ans stop the clock. */
                timeCounter = 0;
                stopTheClock = true;

            };

            background.RunWorkerAsync();


        }

        /// <summary>
        /// Event Handler function for the timeCounter. 
        /// This display the dynamic execution time of the program.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        void tickTimer(object sender, EventArgs e)
        {
            /* Stop the timer when stopTheClock is set to true. */
            if (stopTheClock)
            {
                ((Timer)sender).Stop();
                /* Reset the static timer properties. */
                timeCounter = 0;
                stopTheClock = false;
                return;
            }

            /* Increment the timer*/
            ++timeCounter;

            /* Convert the timeCounter to hours, minutes and seconds. */
            double hours = timeCounter / secPerHour;
            double minutes = (hours - (int)hours) * minutesPerHour;
            double seconds = (minutes - (int)minutes) * secPerMinute;

            /* Format a string for display on the form. */
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}", (int)hours, (int)minutes, (int)seconds);
            ((Label)((Timer)sender).Tag).Text = elapsedTime;

        }

        /// <summary>
        /// Set the Gunnedah basin testing parameters.
        /// </summary>
        private void setGunnedahBasinParameters()
        {
            resetDefaultParameters();

            string internalDirectory = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin";
            /* Data File */
            Settings.dataFile = internalDirectory + @"\Gunnedah Basin Data 201701-201703 - all trains.txt";
            dataFilename.Text = Path.GetFileName(Settings.dataFile);
            dataFilename.ForeColor = SystemColors.ActiveCaptionText;

            /* Geometry File */
            Settings.geometryFile = internalDirectory + @"\Gunnedah Basin Geometry.csv";
            geometryFilename.Text = Path.GetFileName(Settings.geometryFile);
            geometryFilename.ForeColor = SystemColors.ActiveCaptionText;

            /* Simulation files */
            Settings.simulationFiles[0] = internalDirectory + @"\PacificNational-Increasing.csv";
            simulation1Filename.Text = Path.GetFileName(Settings.simulationFiles[0]);
            simulation1Filename.ForeColor = SystemColors.ActiveCaptionText;

            Settings.simulationFiles[1] = internalDirectory + @"\PacificNational-Decreasing.csv";
            simulation2Filename.Text = Path.GetFileName(Settings.simulationFiles[1]);
            simulation2Filename.ForeColor = SystemColors.ActiveCaptionText;
            
            Settings.simulationFiles[2] = internalDirectory + @"\Aurizon-Increasing-60.csv";
            simulation3Filename.Text = Path.GetFileName(Settings.simulationFiles[2]);
            simulation3Filename.ForeColor = SystemColors.ActiveCaptionText;
            
            Settings.simulationFiles[3] = internalDirectory + @"\Aurizon-Decreasing.csv";
            simulation4Filename.Text = Path.GetFileName(Settings.simulationFiles[3]);
            simulation4Filename.ForeColor = SystemColors.ActiveCaptionText;
            

            /* Destination Folder */
            Settings.aggregatedDestination = internalDirectory;
            destinationDirectory.Text = Settings.aggregatedDestination;
            destinationDirectory.ForeColor = SystemColors.ActiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2017, 1, 2);  // 2017, 1, 1
            toDate.Value = new DateTime(2017, 1, 3);    // 2017, 4, 1

            startInterpolationKm.Text = "264";
            endInterpolationKm.Text = "541";
            interpolationInterval.Text = "50";

            minimumJourneyDistance.Text = "20"; // 250
            dataSeparation.Text = "400";        // 4
            timeSeparation.Text = "10";

            trainLengthKm.Text = "1.5";
            trackSpeedPercent.Text = "90";
            maxDistanceToTrackSpeed.Text = "5";

            throughTrainTime.Text = "10";
            stoppingThreshold.Text = "5";
            restartThreshold.Text = "10";

            transactionTimeOutlier.Text = "10";

            Settings.analysisCategory = analysisCategory.Unknown;

        }

        /// <summary>
        /// Function detemrines if the testing parameters for the Gunnedah basin
        /// need to be set, or resets the default settings.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void GunnedahBasinParameters_CheckedChanged(object sender, EventArgs e)
        {
            /* If Tarcoola to Kalgoorlie testing flag is checked, set the appropriate parameters. */
            if (GunnedahBasinParameters.Checked)
                setGunnedahBasinParameters();
            else
                resetDefaultParameters();
        }

        /// <summary>
        /// function resets the train performance analysis form to default settings.
        /// </summary>
        private void resetDefaultParameters()
        {

            /* Data File */
            Settings.dataFile = null;
            dataFilename.Text = "<Select data file>";
            dataFilename.ForeColor = SystemColors.InactiveCaptionText;
            
            /* Geometry File */
            Settings.geometryFile = null;
            geometryFilename.Text = "<Select geometry file>";
            geometryFilename.ForeColor = SystemColors.InactiveCaptionText;

            /* Simulation files */
            Settings.simulationFiles = new List<string>(new string[6]);
            simulation1Filename.Text = "<Select a simualtion file>";
            simulation1Filename.ForeColor = SystemColors.InactiveCaptionText;

            simulation2Filename.Text = "<Select a simualtion file>";
            simulation2Filename.ForeColor = SystemColors.InactiveCaptionText;

            simulation3Filename.Text = "<Select a simualtion file>";
            simulation3Filename.ForeColor = SystemColors.InactiveCaptionText;

            simulation4Filename.Text = "<Select a simualtion file>";
            simulation4Filename.ForeColor = SystemColors.InactiveCaptionText;


            /* Destination Folder */
            Settings.aggregatedDestination = null;
            destinationDirectory.Text = "<Select directory>";
            destinationDirectory.ForeColor = SystemColors.InactiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2016, 1, 1);
            toDate.Value = new DateTime(2016, 2, 1);

            startInterpolationKm.Text = "0";
            endInterpolationKm.Text = "100";
            interpolationInterval.Text = "50";

            minimumJourneyDistance.Text = "80";
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";
            
            trainLengthKm.Text = "1.5";
            trackSpeedPercent.Text = "90";
            maxDistanceToTrackSpeed.Text = "5";
            
            throughTrainTime.Text = "10";
            stoppingThreshold.Text = "5";
            restartThreshold.Text = "10";
            
            transactionTimeOutlier.Text = "10";

            Settings.analysisCategory = analysisCategory.Unknown;

           

        }
    }
    
}
