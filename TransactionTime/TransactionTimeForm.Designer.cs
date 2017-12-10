namespace TransactionTime
{
    partial class TransactionTimeFrom
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransactionTimeFrom));
            this.TransactionTimeForm = new System.Windows.Forms.TabControl();
            this.fileTab = new System.Windows.Forms.TabPage();
            this.UlanLineParameters = new System.Windows.Forms.CheckBox();
            this.GunnedahBasinParameters = new System.Windows.Forms.CheckBox();
            this.destinationDirectory = new System.Windows.Forms.TextBox();
            this.directoryButton = new System.Windows.Forms.Button();
            this.actualAveragePerformanceFile = new System.Windows.Forms.TextBox();
            this.geometryFilename = new System.Windows.Forms.TextBox();
            this.dataFilename = new System.Windows.Forms.TextBox();
            this.actualAveragePerformanceButton = new System.Windows.Forms.Button();
            this.geometryFileButton = new System.Windows.Forms.Button();
            this.dataFileButton = new System.Windows.Forms.Button();
            this.ParametersTab = new System.Windows.Forms.TabPage();
            this.executionTime = new System.Windows.Forms.Label();
            this.ExecitionTimeLabel = new System.Windows.Forms.Label();
            this.throughTrainTime = new System.Windows.Forms.TextBox();
            this.throughTrainTimeLabel = new System.Windows.Forms.Label();
            this.ProcessButton = new System.Windows.Forms.Button();
            this.transactionTimeOutlier = new System.Windows.Forms.TextBox();
            this.restartThreshold = new System.Windows.Forms.TextBox();
            this.stoppingThreshold = new System.Windows.Forms.TextBox();
            this.stopppingThresholdLabel = new System.Windows.Forms.Label();
            this.restartThresholdLabel = new System.Windows.Forms.Label();
            this.transactionTimeLabel = new System.Windows.Forms.Label();
            this.timeSeparation = new System.Windows.Forms.TextBox();
            this.maxDistanceToTrackSpeed = new System.Windows.Forms.TextBox();
            this.dataSeparation = new System.Windows.Forms.TextBox();
            this.trackSpeedPercent = new System.Windows.Forms.TextBox();
            this.minimumJourneyDistance = new System.Windows.Forms.TextBox();
            this.trainLengthKm = new System.Windows.Forms.TextBox();
            this.interpolationInterval = new System.Windows.Forms.TextBox();
            this.endInterpolationKm = new System.Windows.Forms.TextBox();
            this.startInterpolationKm = new System.Windows.Forms.TextBox();
            this.trackSpeedLabel = new System.Windows.Forms.Label();
            this.maxTrackSpeedDistance = new System.Windows.Forms.Label();
            this.startKmLabel = new System.Windows.Forms.Label();
            this.minDistanceLabel = new System.Windows.Forms.Label();
            this.dataSeparationLabel = new System.Windows.Forms.Label();
            this.timeSeparationLabel = new System.Windows.Forms.Label();
            this.endKmLabel = new System.Windows.Forms.Label();
            this.trainLengthLabel = new System.Windows.Forms.Label();
            this.interpolationLabel = new System.Windows.Forms.Label();
            this.toDate = new System.Windows.Forms.DateTimePicker();
            this.fromDate = new System.Windows.Forms.DateTimePicker();
            this.toLabel = new System.Windows.Forms.Label();
            this.fromLabel = new System.Windows.Forms.Label();
            this.DateRangeLabel = new System.Windows.Forms.Label();
            this.TransactionTimeForm.SuspendLayout();
            this.fileTab.SuspendLayout();
            this.ParametersTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // TransactionTimeForm
            // 
            this.TransactionTimeForm.Controls.Add(this.fileTab);
            this.TransactionTimeForm.Controls.Add(this.ParametersTab);
            this.TransactionTimeForm.Location = new System.Drawing.Point(12, 12);
            this.TransactionTimeForm.Name = "TransactionTimeForm";
            this.TransactionTimeForm.SelectedIndex = 0;
            this.TransactionTimeForm.Size = new System.Drawing.Size(924, 356);
            this.TransactionTimeForm.TabIndex = 6;
            // 
            // fileTab
            // 
            this.fileTab.Controls.Add(this.UlanLineParameters);
            this.fileTab.Controls.Add(this.GunnedahBasinParameters);
            this.fileTab.Controls.Add(this.destinationDirectory);
            this.fileTab.Controls.Add(this.directoryButton);
            this.fileTab.Controls.Add(this.actualAveragePerformanceFile);
            this.fileTab.Controls.Add(this.geometryFilename);
            this.fileTab.Controls.Add(this.dataFilename);
            this.fileTab.Controls.Add(this.actualAveragePerformanceButton);
            this.fileTab.Controls.Add(this.geometryFileButton);
            this.fileTab.Controls.Add(this.dataFileButton);
            this.fileTab.Location = new System.Drawing.Point(4, 22);
            this.fileTab.Name = "fileTab";
            this.fileTab.Padding = new System.Windows.Forms.Padding(3);
            this.fileTab.Size = new System.Drawing.Size(916, 330);
            this.fileTab.TabIndex = 0;
            this.fileTab.Text = "Select Files";
            this.fileTab.UseVisualStyleBackColor = true;
            // 
            // UlanLineParameters
            // 
            this.UlanLineParameters.AutoSize = true;
            this.UlanLineParameters.Location = new System.Drawing.Point(749, 56);
            this.UlanLineParameters.Name = "UlanLineParameters";
            this.UlanLineParameters.Size = new System.Drawing.Size(127, 17);
            this.UlanLineParameters.TabIndex = 15;
            this.UlanLineParameters.Text = "Ulan Line Parameters";
            this.UlanLineParameters.UseVisualStyleBackColor = true;
            this.UlanLineParameters.CheckedChanged += new System.EventHandler(this.UlanLineParameters_CheckedChanged);
            // 
            // GunnedahBasinParameters
            // 
            this.GunnedahBasinParameters.AutoSize = true;
            this.GunnedahBasinParameters.Location = new System.Drawing.Point(749, 33);
            this.GunnedahBasinParameters.Name = "GunnedahBasinParameters";
            this.GunnedahBasinParameters.Size = new System.Drawing.Size(161, 17);
            this.GunnedahBasinParameters.TabIndex = 14;
            this.GunnedahBasinParameters.Text = "Gunnedah Basin Parameters";
            this.GunnedahBasinParameters.UseVisualStyleBackColor = true;
            this.GunnedahBasinParameters.CheckedChanged += new System.EventHandler(this.GunnedahBasinParameters_CheckedChanged);
            // 
            // destinationDirectory
            // 
            this.destinationDirectory.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.destinationDirectory.Location = new System.Drawing.Point(152, 166);
            this.destinationDirectory.Name = "destinationDirectory";
            this.destinationDirectory.Size = new System.Drawing.Size(585, 20);
            this.destinationDirectory.TabIndex = 13;
            this.destinationDirectory.Text = "<Select destination directory>";
            // 
            // directoryButton
            // 
            this.directoryButton.Location = new System.Drawing.Point(15, 157);
            this.directoryButton.Name = "directoryButton";
            this.directoryButton.Size = new System.Drawing.Size(131, 37);
            this.directoryButton.TabIndex = 12;
            this.directoryButton.Text = "Select Destination Directory";
            this.directoryButton.UseVisualStyleBackColor = true;
            this.directoryButton.Click += new System.EventHandler(this.directoryButton_Click);
            // 
            // actualAveragePerformanceFile
            // 
            this.actualAveragePerformanceFile.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.actualAveragePerformanceFile.Location = new System.Drawing.Point(152, 100);
            this.actualAveragePerformanceFile.Name = "actualAveragePerformanceFile";
            this.actualAveragePerformanceFile.Size = new System.Drawing.Size(585, 20);
            this.actualAveragePerformanceFile.TabIndex = 5;
            this.actualAveragePerformanceFile.Text = "<Select actual average performance file>";
            // 
            // geometryFilename
            // 
            this.geometryFilename.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.geometryFilename.Location = new System.Drawing.Point(152, 66);
            this.geometryFilename.Name = "geometryFilename";
            this.geometryFilename.Size = new System.Drawing.Size(585, 20);
            this.geometryFilename.TabIndex = 4;
            this.geometryFilename.Text = "<Select geometry file>";
            // 
            // dataFilename
            // 
            this.dataFilename.ForeColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.dataFilename.Location = new System.Drawing.Point(152, 33);
            this.dataFilename.Name = "dataFilename";
            this.dataFilename.Size = new System.Drawing.Size(585, 20);
            this.dataFilename.TabIndex = 3;
            this.dataFilename.Text = "<Select data file>";
            // 
            // actualAveragePerformanceButton
            // 
            this.actualAveragePerformanceButton.Location = new System.Drawing.Point(15, 94);
            this.actualAveragePerformanceButton.Name = "actualAveragePerformanceButton";
            this.actualAveragePerformanceButton.Size = new System.Drawing.Size(131, 31);
            this.actualAveragePerformanceButton.TabIndex = 2;
            this.actualAveragePerformanceButton.Text = "Select Averaged Data";
            this.actualAveragePerformanceButton.UseVisualStyleBackColor = true;
            this.actualAveragePerformanceButton.Click += new System.EventHandler(this.actualAverageFileButton_Click);
            // 
            // geometryFileButton
            // 
            this.geometryFileButton.Location = new System.Drawing.Point(14, 62);
            this.geometryFileButton.Name = "geometryFileButton";
            this.geometryFileButton.Size = new System.Drawing.Size(132, 26);
            this.geometryFileButton.TabIndex = 1;
            this.geometryFileButton.Text = "Select Geometry File";
            this.geometryFileButton.UseVisualStyleBackColor = true;
            this.geometryFileButton.Click += new System.EventHandler(this.geometryFileButton_Click);
            // 
            // dataFileButton
            // 
            this.dataFileButton.Location = new System.Drawing.Point(15, 28);
            this.dataFileButton.Name = "dataFileButton";
            this.dataFileButton.Size = new System.Drawing.Size(131, 28);
            this.dataFileButton.TabIndex = 0;
            this.dataFileButton.Text = "Select Data File";
            this.dataFileButton.UseVisualStyleBackColor = true;
            this.dataFileButton.Click += new System.EventHandler(this.dataFileButton_Click);
            // 
            // ParametersTab
            // 
            this.ParametersTab.Controls.Add(this.executionTime);
            this.ParametersTab.Controls.Add(this.ExecitionTimeLabel);
            this.ParametersTab.Controls.Add(this.throughTrainTime);
            this.ParametersTab.Controls.Add(this.throughTrainTimeLabel);
            this.ParametersTab.Controls.Add(this.ProcessButton);
            this.ParametersTab.Controls.Add(this.transactionTimeOutlier);
            this.ParametersTab.Controls.Add(this.restartThreshold);
            this.ParametersTab.Controls.Add(this.stoppingThreshold);
            this.ParametersTab.Controls.Add(this.stopppingThresholdLabel);
            this.ParametersTab.Controls.Add(this.restartThresholdLabel);
            this.ParametersTab.Controls.Add(this.transactionTimeLabel);
            this.ParametersTab.Controls.Add(this.timeSeparation);
            this.ParametersTab.Controls.Add(this.maxDistanceToTrackSpeed);
            this.ParametersTab.Controls.Add(this.dataSeparation);
            this.ParametersTab.Controls.Add(this.trackSpeedPercent);
            this.ParametersTab.Controls.Add(this.minimumJourneyDistance);
            this.ParametersTab.Controls.Add(this.trainLengthKm);
            this.ParametersTab.Controls.Add(this.interpolationInterval);
            this.ParametersTab.Controls.Add(this.endInterpolationKm);
            this.ParametersTab.Controls.Add(this.startInterpolationKm);
            this.ParametersTab.Controls.Add(this.trackSpeedLabel);
            this.ParametersTab.Controls.Add(this.maxTrackSpeedDistance);
            this.ParametersTab.Controls.Add(this.startKmLabel);
            this.ParametersTab.Controls.Add(this.minDistanceLabel);
            this.ParametersTab.Controls.Add(this.dataSeparationLabel);
            this.ParametersTab.Controls.Add(this.timeSeparationLabel);
            this.ParametersTab.Controls.Add(this.endKmLabel);
            this.ParametersTab.Controls.Add(this.trainLengthLabel);
            this.ParametersTab.Controls.Add(this.interpolationLabel);
            this.ParametersTab.Controls.Add(this.toDate);
            this.ParametersTab.Controls.Add(this.fromDate);
            this.ParametersTab.Controls.Add(this.toLabel);
            this.ParametersTab.Controls.Add(this.fromLabel);
            this.ParametersTab.Controls.Add(this.DateRangeLabel);
            this.ParametersTab.Location = new System.Drawing.Point(4, 22);
            this.ParametersTab.Name = "ParametersTab";
            this.ParametersTab.Padding = new System.Windows.Forms.Padding(3);
            this.ParametersTab.Size = new System.Drawing.Size(916, 330);
            this.ParametersTab.TabIndex = 1;
            this.ParametersTab.Text = "Select Parameters";
            this.ParametersTab.UseVisualStyleBackColor = true;
            // 
            // executionTime
            // 
            this.executionTime.AutoSize = true;
            this.executionTime.Location = new System.Drawing.Point(598, 302);
            this.executionTime.Name = "executionTime";
            this.executionTime.Size = new System.Drawing.Size(26, 13);
            this.executionTime.TabIndex = 89;
            this.executionTime.Text = "time";
            // 
            // ExecitionTimeLabel
            // 
            this.ExecitionTimeLabel.AutoSize = true;
            this.ExecitionTimeLabel.Location = new System.Drawing.Point(496, 302);
            this.ExecitionTimeLabel.Name = "ExecitionTimeLabel";
            this.ExecitionTimeLabel.Size = new System.Drawing.Size(83, 13);
            this.ExecitionTimeLabel.TabIndex = 88;
            this.ExecitionTimeLabel.Text = "Execution Time:";
            // 
            // throughTrainTime
            // 
            this.throughTrainTime.Location = new System.Drawing.Point(646, 181);
            this.throughTrainTime.Name = "throughTrainTime";
            this.throughTrainTime.Size = new System.Drawing.Size(100, 20);
            this.throughTrainTime.TabIndex = 87;
            this.throughTrainTime.Text = "10";
            // 
            // throughTrainTimeLabel
            // 
            this.throughTrainTimeLabel.AutoSize = true;
            this.throughTrainTimeLabel.Location = new System.Drawing.Point(463, 184);
            this.throughTrainTimeLabel.Name = "throughTrainTimeLabel";
            this.throughTrainTimeLabel.Size = new System.Drawing.Size(179, 13);
            this.throughTrainTimeLabel.TabIndex = 86;
            this.throughTrainTimeLabel.Text = "Through Train Time Seperation (min)";
            // 
            // ProcessButton
            // 
            this.ProcessButton.Location = new System.Drawing.Point(499, 235);
            this.ProcessButton.Name = "ProcessButton";
            this.ProcessButton.Size = new System.Drawing.Size(180, 54);
            this.ProcessButton.TabIndex = 84;
            this.ProcessButton.Text = "Process";
            this.ProcessButton.UseVisualStyleBackColor = true;
            this.ProcessButton.Click += new System.EventHandler(this.ProcessButton_Click);
            // 
            // transactionTimeOutlier
            // 
            this.transactionTimeOutlier.Location = new System.Drawing.Point(272, 269);
            this.transactionTimeOutlier.Name = "transactionTimeOutlier";
            this.transactionTimeOutlier.Size = new System.Drawing.Size(100, 20);
            this.transactionTimeOutlier.TabIndex = 83;
            this.transactionTimeOutlier.Text = "10";
            // 
            // restartThreshold
            // 
            this.restartThreshold.Location = new System.Drawing.Point(272, 243);
            this.restartThreshold.Name = "restartThreshold";
            this.restartThreshold.Size = new System.Drawing.Size(100, 20);
            this.restartThreshold.TabIndex = 82;
            this.restartThreshold.Text = "10";
            // 
            // stoppingThreshold
            // 
            this.stoppingThreshold.Location = new System.Drawing.Point(272, 217);
            this.stoppingThreshold.Name = "stoppingThreshold";
            this.stoppingThreshold.Size = new System.Drawing.Size(100, 20);
            this.stoppingThreshold.TabIndex = 81;
            this.stoppingThreshold.Text = "5";
            // 
            // stopppingThresholdLabel
            // 
            this.stopppingThresholdLabel.AutoSize = true;
            this.stopppingThresholdLabel.Location = new System.Drawing.Point(16, 220);
            this.stopppingThresholdLabel.Name = "stopppingThresholdLabel";
            this.stopppingThresholdLabel.Size = new System.Drawing.Size(148, 13);
            this.stopppingThresholdLabel.TabIndex = 80;
            this.stopppingThresholdLabel.Text = "Threshold for Stopping Speed";
            // 
            // restartThresholdLabel
            // 
            this.restartThresholdLabel.AutoSize = true;
            this.restartThresholdLabel.Location = new System.Drawing.Point(16, 246);
            this.restartThresholdLabel.Name = "restartThresholdLabel";
            this.restartThresholdLabel.Size = new System.Drawing.Size(154, 13);
            this.restartThresholdLabel.TabIndex = 79;
            this.restartThresholdLabel.Text = "Threshold for Restarting Speed";
            // 
            // transactionTimeLabel
            // 
            this.transactionTimeLabel.AutoSize = true;
            this.transactionTimeLabel.Location = new System.Drawing.Point(16, 272);
            this.transactionTimeLabel.Name = "transactionTimeLabel";
            this.transactionTimeLabel.Size = new System.Drawing.Size(161, 13);
            this.transactionTimeLabel.TabIndex = 78;
            this.transactionTimeLabel.Text = "Maximum Transaction Time (min)";
            // 
            // timeSeparation
            // 
            this.timeSeparation.Location = new System.Drawing.Point(272, 181);
            this.timeSeparation.Name = "timeSeparation";
            this.timeSeparation.Size = new System.Drawing.Size(100, 20);
            this.timeSeparation.TabIndex = 77;
            this.timeSeparation.Text = "10";
            // 
            // maxDistanceToTrackSpeed
            // 
            this.maxDistanceToTrackSpeed.Location = new System.Drawing.Point(646, 155);
            this.maxDistanceToTrackSpeed.Name = "maxDistanceToTrackSpeed";
            this.maxDistanceToTrackSpeed.Size = new System.Drawing.Size(100, 20);
            this.maxDistanceToTrackSpeed.TabIndex = 76;
            this.maxDistanceToTrackSpeed.Text = "5";
            // 
            // dataSeparation
            // 
            this.dataSeparation.Location = new System.Drawing.Point(272, 155);
            this.dataSeparation.Name = "dataSeparation";
            this.dataSeparation.Size = new System.Drawing.Size(100, 20);
            this.dataSeparation.TabIndex = 75;
            this.dataSeparation.Text = "4";
            // 
            // trackSpeedPercent
            // 
            this.trackSpeedPercent.Location = new System.Drawing.Point(646, 129);
            this.trackSpeedPercent.Name = "trackSpeedPercent";
            this.trackSpeedPercent.Size = new System.Drawing.Size(100, 20);
            this.trackSpeedPercent.TabIndex = 74;
            this.trackSpeedPercent.Text = "90";
            // 
            // minimumJourneyDistance
            // 
            this.minimumJourneyDistance.Location = new System.Drawing.Point(272, 129);
            this.minimumJourneyDistance.Name = "minimumJourneyDistance";
            this.minimumJourneyDistance.Size = new System.Drawing.Size(100, 20);
            this.minimumJourneyDistance.TabIndex = 73;
            this.minimumJourneyDistance.Text = "250";
            // 
            // trainLengthKm
            // 
            this.trainLengthKm.Location = new System.Drawing.Point(646, 103);
            this.trainLengthKm.Name = "trainLengthKm";
            this.trainLengthKm.Size = new System.Drawing.Size(100, 20);
            this.trainLengthKm.TabIndex = 72;
            this.trainLengthKm.Text = "1.5";
            // 
            // interpolationInterval
            // 
            this.interpolationInterval.Location = new System.Drawing.Point(272, 103);
            this.interpolationInterval.Name = "interpolationInterval";
            this.interpolationInterval.Size = new System.Drawing.Size(100, 20);
            this.interpolationInterval.TabIndex = 71;
            this.interpolationInterval.Text = "50";
            // 
            // endInterpolationKm
            // 
            this.endInterpolationKm.Location = new System.Drawing.Point(646, 77);
            this.endInterpolationKm.Name = "endInterpolationKm";
            this.endInterpolationKm.Size = new System.Drawing.Size(100, 20);
            this.endInterpolationKm.TabIndex = 70;
            this.endInterpolationKm.Text = "541";
            // 
            // startInterpolationKm
            // 
            this.startInterpolationKm.Location = new System.Drawing.Point(272, 77);
            this.startInterpolationKm.Name = "startInterpolationKm";
            this.startInterpolationKm.Size = new System.Drawing.Size(100, 20);
            this.startInterpolationKm.TabIndex = 69;
            this.startInterpolationKm.Text = "264";
            // 
            // trackSpeedLabel
            // 
            this.trackSpeedLabel.AutoSize = true;
            this.trackSpeedLabel.Location = new System.Drawing.Point(463, 132);
            this.trackSpeedLabel.Name = "trackSpeedLabel";
            this.trackSpeedLabel.Size = new System.Drawing.Size(131, 13);
            this.trackSpeedLabel.TabIndex = 68;
            this.trackSpeedLabel.Text = "Factor of Track Speed (%)";
            // 
            // maxTrackSpeedDistance
            // 
            this.maxTrackSpeedDistance.AutoSize = true;
            this.maxTrackSpeedDistance.Location = new System.Drawing.Point(463, 158);
            this.maxTrackSpeedDistance.Name = "maxTrackSpeedDistance";
            this.maxTrackSpeedDistance.Size = new System.Drawing.Size(171, 13);
            this.maxTrackSpeedDistance.TabIndex = 67;
            this.maxTrackSpeedDistance.Text = "Maximum distance to Track Speed";
            // 
            // startKmLabel
            // 
            this.startKmLabel.AutoSize = true;
            this.startKmLabel.Location = new System.Drawing.Point(16, 80);
            this.startKmLabel.Name = "startKmLabel";
            this.startKmLabel.Size = new System.Drawing.Size(116, 13);
            this.startKmLabel.TabIndex = 66;
            this.startKmLabel.Text = "Start Kilometreage (km)";
            // 
            // minDistanceLabel
            // 
            this.minDistanceLabel.AutoSize = true;
            this.minDistanceLabel.Location = new System.Drawing.Point(16, 132);
            this.minDistanceLabel.Name = "minDistanceLabel";
            this.minDistanceLabel.Size = new System.Drawing.Size(149, 13);
            this.minDistanceLabel.TabIndex = 65;
            this.minDistanceLabel.Text = "Minimum Travel Distance (km)";
            // 
            // dataSeparationLabel
            // 
            this.dataSeparationLabel.AutoSize = true;
            this.dataSeparationLabel.Location = new System.Drawing.Point(16, 158);
            this.dataSeparationLabel.Name = "dataSeparationLabel";
            this.dataSeparationLabel.Size = new System.Drawing.Size(107, 13);
            this.dataSeparationLabel.TabIndex = 64;
            this.dataSeparationLabel.Text = "Data Separation (km)";
            // 
            // timeSeparationLabel
            // 
            this.timeSeparationLabel.AutoSize = true;
            this.timeSeparationLabel.Location = new System.Drawing.Point(16, 184);
            this.timeSeparationLabel.Name = "timeSeparationLabel";
            this.timeSeparationLabel.Size = new System.Drawing.Size(125, 13);
            this.timeSeparationLabel.TabIndex = 63;
            this.timeSeparationLabel.Text = "Data Time Separation (h)";
            // 
            // endKmLabel
            // 
            this.endKmLabel.AutoSize = true;
            this.endKmLabel.Location = new System.Drawing.Point(463, 80);
            this.endKmLabel.Name = "endKmLabel";
            this.endKmLabel.Size = new System.Drawing.Size(109, 13);
            this.endKmLabel.TabIndex = 62;
            this.endKmLabel.Text = "End interpolation (km)";
            // 
            // trainLengthLabel
            // 
            this.trainLengthLabel.AutoSize = true;
            this.trainLengthLabel.Location = new System.Drawing.Point(463, 106);
            this.trainLengthLabel.Name = "trainLengthLabel";
            this.trainLengthLabel.Size = new System.Drawing.Size(90, 13);
            this.trainLengthLabel.TabIndex = 61;
            this.trainLengthLabel.Text = "Train Length (km)";
            // 
            // interpolationLabel
            // 
            this.interpolationLabel.AutoSize = true;
            this.interpolationLabel.Location = new System.Drawing.Point(16, 106);
            this.interpolationLabel.Name = "interpolationLabel";
            this.interpolationLabel.Size = new System.Drawing.Size(120, 13);
            this.interpolationLabel.TabIndex = 60;
            this.interpolationLabel.Text = "Interpolation Interval (m)";
            // 
            // toDate
            // 
            this.toDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.toDate.Location = new System.Drawing.Point(466, 39);
            this.toDate.Name = "toDate";
            this.toDate.Size = new System.Drawing.Size(100, 20);
            this.toDate.TabIndex = 59;
            this.toDate.Value = new System.DateTime(2017, 6, 1, 0, 0, 0, 0);
            // 
            // fromDate
            // 
            this.fromDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.fromDate.Location = new System.Drawing.Point(272, 39);
            this.fromDate.Name = "fromDate";
            this.fromDate.Size = new System.Drawing.Size(100, 20);
            this.fromDate.TabIndex = 58;
            this.fromDate.Value = new System.DateTime(2017, 1, 1, 0, 0, 0, 0);
            // 
            // toLabel
            // 
            this.toLabel.AutoSize = true;
            this.toLabel.Location = new System.Drawing.Point(463, 11);
            this.toLabel.Name = "toLabel";
            this.toLabel.Size = new System.Drawing.Size(23, 13);
            this.toLabel.TabIndex = 57;
            this.toLabel.Text = "To:";
            // 
            // fromLabel
            // 
            this.fromLabel.AutoSize = true;
            this.fromLabel.Location = new System.Drawing.Point(269, 11);
            this.fromLabel.Name = "fromLabel";
            this.fromLabel.Size = new System.Drawing.Size(33, 13);
            this.fromLabel.TabIndex = 56;
            this.fromLabel.Text = "From:";
            // 
            // DateRangeLabel
            // 
            this.DateRangeLabel.AutoSize = true;
            this.DateRangeLabel.Location = new System.Drawing.Point(16, 45);
            this.DateRangeLabel.Name = "DateRangeLabel";
            this.DateRangeLabel.Size = new System.Drawing.Size(68, 13);
            this.DateRangeLabel.TabIndex = 55;
            this.DateRangeLabel.Text = "Date Range:";
            // 
            // TransactionTimeFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(948, 380);
            this.Controls.Add(this.TransactionTimeForm);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TransactionTimeFrom";
            this.Text = "Form1";
            this.TransactionTimeForm.ResumeLayout(false);
            this.fileTab.ResumeLayout(false);
            this.fileTab.PerformLayout();
            this.ParametersTab.ResumeLayout(false);
            this.ParametersTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl TransactionTimeForm;
        private System.Windows.Forms.TabPage fileTab;
        private System.Windows.Forms.TextBox actualAveragePerformanceFile;
        private System.Windows.Forms.TextBox geometryFilename;
        private System.Windows.Forms.TextBox dataFilename;
        private System.Windows.Forms.Button actualAveragePerformanceButton;
        private System.Windows.Forms.Button geometryFileButton;
        private System.Windows.Forms.Button dataFileButton;
        private System.Windows.Forms.TabPage ParametersTab;
        private System.Windows.Forms.TextBox timeSeparation;
        private System.Windows.Forms.TextBox maxDistanceToTrackSpeed;
        private System.Windows.Forms.TextBox dataSeparation;
        private System.Windows.Forms.TextBox trackSpeedPercent;
        private System.Windows.Forms.TextBox minimumJourneyDistance;
        private System.Windows.Forms.TextBox trainLengthKm;
        private System.Windows.Forms.TextBox interpolationInterval;
        private System.Windows.Forms.TextBox endInterpolationKm;
        private System.Windows.Forms.TextBox startInterpolationKm;
        private System.Windows.Forms.Label trackSpeedLabel;
        private System.Windows.Forms.Label maxTrackSpeedDistance;
        private System.Windows.Forms.Label startKmLabel;
        private System.Windows.Forms.Label minDistanceLabel;
        private System.Windows.Forms.Label dataSeparationLabel;
        private System.Windows.Forms.Label timeSeparationLabel;
        private System.Windows.Forms.Label endKmLabel;
        private System.Windows.Forms.Label trainLengthLabel;
        private System.Windows.Forms.Label interpolationLabel;
        private System.Windows.Forms.DateTimePicker toDate;
        private System.Windows.Forms.DateTimePicker fromDate;
        private System.Windows.Forms.Label toLabel;
        private System.Windows.Forms.Label fromLabel;
        private System.Windows.Forms.Label DateRangeLabel;
        private System.Windows.Forms.TextBox transactionTimeOutlier;
        private System.Windows.Forms.TextBox restartThreshold;
        private System.Windows.Forms.TextBox stoppingThreshold;
        private System.Windows.Forms.Label stopppingThresholdLabel;
        private System.Windows.Forms.Label restartThresholdLabel;
        private System.Windows.Forms.Label transactionTimeLabel;
        private System.Windows.Forms.TextBox destinationDirectory;
        private System.Windows.Forms.Button directoryButton;
        private System.Windows.Forms.Button ProcessButton;
        private System.Windows.Forms.TextBox throughTrainTime;
        private System.Windows.Forms.Label throughTrainTimeLabel;
        private System.Windows.Forms.Label executionTime;
        private System.Windows.Forms.Label ExecitionTimeLabel;
        private System.Windows.Forms.CheckBox GunnedahBasinParameters;
        private System.Windows.Forms.CheckBox UlanLineParameters;

    }
}