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
        private void actualAverageFileButton_Click(object sender, EventArgs e)
        {
            /* Select the data file. */
            Settings.actualAveragePerformanceFile = Tools.selectDataFile(caption: "Select the actual average performance file.");
            actualAveragePerformanceFile.Text = Path.GetFileName(Settings.actualAveragePerformanceFile);
            actualAveragePerformanceFile.ForeColor = System.Drawing.Color.Black;
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
        /// Assign the form parameters to the Settings parameters.
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
            GunnedahBasinParameters.Checked = true;

            string internalDirectory = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin";
            /* Data File */
            Settings.dataFile = internalDirectory + @"\Gunnedah Basin Data 201701-201706 - all trains.txt"; //Gunnedah Basin Data 2016-2017 PN+QR.txt
            dataFilename.Text = Path.GetFileName(Settings.dataFile);
            dataFilename.ForeColor = SystemColors.ActiveCaptionText;

            /* Geometry File */
            Settings.geometryFile = internalDirectory + @"\Gunnedah Basin Geometry.csv";
            geometryFilename.Text = Path.GetFileName(Settings.geometryFile);
            geometryFilename.ForeColor = SystemColors.ActiveCaptionText;

            /* Simulation files */
            Settings.actualAveragePerformanceFile = internalDirectory + @"\Gunnedah Actual Average Trains.csv"; //PacificNational-Increasing.csv";
            actualAveragePerformanceFile.Text = Path.GetFileName(Settings.actualAveragePerformanceFile);
            actualAveragePerformanceFile.ForeColor = SystemColors.ActiveCaptionText;
            
            /* Destination Folder */
            Settings.aggregatedDestination = internalDirectory;
            destinationDirectory.Text = Settings.aggregatedDestination;
            destinationDirectory.ForeColor = SystemColors.ActiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2017, 1, 3);  // 2017, 1, 1
            toDate.Value = new DateTime(2017, 1, 4);    // 2017, 4, 1

            startInterpolationKm.Text = "264";
            endInterpolationKm.Text = "541";
            interpolationInterval.Text = "50";

            minimumJourneyDistance.Text = "20"; // 250
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            trainLengthKm.Text = "1.3";         // train length is 1335 m
            trackSpeedPercent.Text = "90";
            maxDistanceToTrackSpeed.Text = "5";

            throughTrainTime.Text = "10";
            stoppingThreshold.Text = "5";
            restartThreshold.Text = "10";

            transactionTimeOutlier.Text = "10";

            Settings.analysisCategory = analysisCategory.TrainOperator;
            PacificNational.Checked = true;
            Settings.PacificNational = true;
            Aurizon.Checked = true;
            Settings.Aurizon = true;
            CombineOperators.Checked = true;
            Settings.Combined = true;

        }

        /// <summary>
        /// Set the Ulan Line testing parameters.
        /// </summary>
        private void setUlanLineParameters()
        {
            resetDefaultParameters();
            UlanLineParameters.Checked = true;

            string internalDirectory = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Ulan";
            /* Data File */
            Settings.dataFile = internalDirectory + @"\Ulan Data 2017-201709.txt"; //Ulan Data 2017-20170531-2.txt
            dataFilename.Text = Path.GetFileName(Settings.dataFile);
            dataFilename.ForeColor = SystemColors.ActiveCaptionText;

            /* Geometry File */
            Settings.geometryFile = internalDirectory + @"\Ulan Geometry.csv";
            geometryFilename.Text = Path.GetFileName(Settings.geometryFile);
            geometryFilename.ForeColor = SystemColors.ActiveCaptionText;

            /* Simulation files */
            Settings.actualAveragePerformanceFile = internalDirectory + @"\Ulan Actual Average Trains.csv";
            actualAveragePerformanceFile.Text = Path.GetFileName(Settings.actualAveragePerformanceFile);
            actualAveragePerformanceFile.ForeColor = SystemColors.ActiveCaptionText;

            /* Destination Folder */
            Settings.aggregatedDestination = internalDirectory;
            destinationDirectory.Text = Settings.aggregatedDestination;
            destinationDirectory.ForeColor = SystemColors.ActiveCaptionText;

            /* Settings */
            fromDate.Value = new DateTime(2017, 1, 2);  // 2017, 1, 1
            toDate.Value = new DateTime(2017, 1, 3);    // 2017, 4, 1

            startInterpolationKm.Text = "280";
            endInterpolationKm.Text = "460";
            interpolationInterval.Text = "50";

            minimumJourneyDistance.Text = "5"; // 250
            dataSeparation.Text = "4";
            timeSeparation.Text = "10";

            trainLengthKm.Text = "1.5";
            trackSpeedPercent.Text = "90";
            maxDistanceToTrackSpeed.Text = "5";

            throughTrainTime.Text = "10";
            stoppingThreshold.Text = "5";
            restartThreshold.Text = "10";

            transactionTimeOutlier.Text = "10";

            Settings.analysisCategory = analysisCategory.TrainOperator;
            PacificNational.Checked = true;
            Settings.PacificNational = true;
            Freightliner.Checked = true;
            Settings.Freightliner = true;
            Aurizon.Checked = true;
            Settings.Aurizon = true;
            CombineOperators.Checked = true;
            Settings.Combined = true;


        }

        /// <summary>
        /// Function determines if the testing parameters for the Gunnedah basin
        /// need to be set, or resets the default settings.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void GunnedahBasinParameters_CheckedChanged(object sender, EventArgs e)
        {
            /* If Gunnedah testing flag is checked, set the appropriate parameters. */
            if (GunnedahBasinParameters.Checked)
                setGunnedahBasinParameters();
            else
                resetDefaultParameters();
        }

        /// <summary>
        /// Function determines if the testing parameters for the Ulan line
        /// need to be set, or resets the default settings.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void UlanLineParameters_CheckedChanged(object sender, EventArgs e)
        {
            /* If Ulan testing flag is checked, set the appropriate parameters. */
            if (UlanLineParameters.Checked)
                setUlanLineParameters();
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
            Settings.actualAveragePerformanceFile = null;
            actualAveragePerformanceFile.Text = "<Select a simualtion file>";
            actualAveragePerformanceFile.ForeColor = SystemColors.InactiveCaptionText;

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
            Settings.PacificNational = false;
            Settings.Aurizon = false;
            Settings.Freightliner = false;
            Settings.CityRail = false;
            Settings.CountryLink = false;
            Settings.Qube = false;
            Settings.SCT = false;
            Settings.Combined = false;

            PacificNational.Checked = false;
            Aurizon.Checked = false;
            Freightliner.Checked = false;
            CityRail.Checked = false;
            CountryLink.Checked = false;
            Qube.Checked = false;
            SCT.Checked = false;
            CombineOperators.Checked = false;



        }

        private void PacificNational_CheckedChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// This sets the Pacific National Operator to the appropriate value in order 
        /// to dynamically include Pacific National trains in teh analysis.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void PacificNational_Click(object sender, EventArgs e)
        {
            Settings.PacificNational = PacificNational.Checked;
        }

        /// <summary>
        /// This sets the Freightliner Operator to the appropriate value in order 
        /// to dynamically include Freightliner trains in teh analysis.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void FreightLiner_Click(object sender, EventArgs e)
        {
            Settings.Freightliner = Freightliner.Checked;
        }

        /// <summary>
        /// This sets the Aurizon Operator to the appropriate value in order 
        /// to dynamically include Aurizon trains in the analysis.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void Aurizon_Click(object sender, EventArgs e)
        {
            Settings.Aurizon = Aurizon.Checked;
        }

        /// <summary>
        /// This sets the City Rail Operator to the appropriate value in order 
        /// to dynamically include City Rail trains in the analysis.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void CityRail_Click(object sender, EventArgs e)
        {
            Settings.CityRail = CityRail.Checked;
        }

        /// <summary>
        /// This sets the Country Link Operator to the appropriate value in order 
        /// to dynamically include  Country Link trains in the analysis.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void CountryLink_Click(object sender, EventArgs e)
        {
            Settings.CountryLink = CountryLink.Checked;
        }

        /// <summary>
        /// This sets the Spare Operator to the appropriate value in order 
        /// to dynamically include Spare trains in the analysis.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void Qube_Click(object sender, EventArgs e)
        {
            Settings.Qube = Qube.Checked;
        }
        
        /// <summary>
        /// This sets the Spare Operator to the appropriate value in order 
        /// to dynamically include Spare trains in the analysis.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void SCT_Click(object sender, EventArgs e)
        {
            Settings.SCT = SCT.Checked;
        }

        /// <summary>
        /// This sets the Combined Operator to the appropriate value in order 
        /// to dynamically include acombination of train operators in the analysis.
        /// </summary>
        /// <param name="sender">The object container.</param>
        /// <param name="e">The event arguments.</param>
        private void Combined_Click(object sender, EventArgs e)
        {
            Settings.Combined = CombineOperators.Checked;
        }

    }
    
}
