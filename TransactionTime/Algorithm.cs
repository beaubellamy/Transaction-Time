using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* Custom libaries */
using TrainLibrary;
using IOLibrary;
using Statistics;

namespace TransactionTime
{
    class Algorithm
    {
        /* The minimum gap in time between points to indicate a large gap when a train is restarting. */
        private static double fineTuneTimeGap_sec = 30;

        /// <summary>
        /// Calaculate the transaction time for each opposing cross encountered by trains in the supplied region.
        /// </summary>
        public static void transactionTime()
        {

            /* Ensure there is a empty list of trains to exclude to start. */
            List<string> excludeTrainList = new List<string> { };

            /* Read in the track geometry data. */
            List<TrackGeometry> trackGeometry = new List<TrackGeometry>();
            trackGeometry = FileOperations.readGeometryfile(Settings.geometryFile);

            /* Read the data. */
            List<TrainRecord> TrainRecords = new List<TrainRecord>();
            TrainRecords = FileOperations.readICEData(Settings.dataFile, excludeTrainList, Settings.excludeListOfTrains, Settings.dateRange);
            /* When using Gunnedah Basin Data 2016-2017 PN+QR2.txt use the readICEData2() function. */


            if (TrainRecords.Count() == 0)
            {
                Tools.messageBox("There are no records in the list to analyse.", "No trains available.");
                return;
            }
            List<trainOperator> operators = TrainRecords.Select(t => t.trainOperator).Distinct().ToList();
            operators.Remove(trainOperator.Unknown);
            int numberOfOperators = operators.Count();

            /* Set simulation categories [TRAP:Set analysis categories] */
            List<Category> simCategories = new List<Category>();
            simCategories.Add(Processing.convertTrainOperatorToCategory(trainOperator.PacificNational));
            simCategories.Add(Processing.convertTrainOperatorToCategory(trainOperator.Aurizon));
            // Add when running Ulan Line data
            //simCategories.Add(Processing.convertTrainOperatorToCategory(trainOperator.Freightliner));
            simCategories.Add(Processing.convertTrainOperatorToCategory(trainOperator.Combined));
            /* TRAP: Multiple if conditions */
            /* If further development is required to use different train types (ie, operators, by commodity, etc), 
             * the neccesasry code is in the TRAP tool. 
             */

            /* Create the list of simulated trains. */
            List<Train> simulatedTrains = new List<Train>();

            /* Read the actual averaged train performance for each category. */
            List<Train> interpolatedSimulations = new List<Train>();
            interpolatedSimulations.AddRange(FileOperations.readSimulationData(Settings.actualAveragePerformanceFile, simCategories.Count()));

            /* Sort the data by [trainID, locoID, Date & Time, kmPost]. */
            List<TrainRecord> OrderdTrainRecords = new List<TrainRecord>();
            OrderdTrainRecords = TrainRecords.OrderBy(t => t.trainID).ThenBy(t => t.locoID).ThenBy(t => t.dateTime).ThenBy(t => t.kmPost).ToList();

            /* Clean the records and create individual train journeys. */
            List<Train> trainList = new List<Train>();
            trainList = Processing.CleanData(OrderdTrainRecords, trackGeometry, Settings.timethreshold, Settings.distanceThreshold, Settings.minimumJourneyDistance, Settings.analysisCategory);
            
            /* Perform minor cleaning porocedures and create individual train journeys. */
            List<Train> rawTrainList = new List<Train>();
            rawTrainList = Processing.MakeTrains(OrderdTrainRecords, trackGeometry, Settings.timethreshold, Settings.distanceThreshold, Settings.minimumJourneyDistance, Settings.analysisCategory);
            /* Write the raw data to file - usefull for creating train graphs */
            FileOperations.writeRawTrainDataWithTime(rawTrainList, Settings.aggregatedDestination);

            /* Interpolate data */
            /******** Should only be required while we are waiting for the data in the prefered format ********/
            List<Train> interpolatedTrains = new List<Train>(); 
            List<Train> utilisationTrains = new List<Train>();
            interpolatedTrains = Processing.interpolateTrainDataWithGaps(trainList, trackGeometry, Settings.startInterpolationKm, Settings.endInterpolationKm, Settings.interpolationInterval);
            /* Interpoalte for utilisation calculations. ie interpoalte through the gaps */
            utilisationTrains = Processing.interpolateTrainData(trainList, trackGeometry, Settings.startInterpolationKm, Settings.endInterpolationKm, Settings.interpolationInterval);

            /**************************************************************************************************/

            /* Write the interpolated data to file. */
            FileOperations.writeTrainDataWithTime(interpolatedTrains, Settings.startInterpolationKm, Settings.interpolationInterval, Settings.aggregatedDestination);

            /* Identify all the potential loop locations where a train may be forced to stop. */
            List<LoopLocation> loopLocations = findAllLoopLocations(interpolatedTrains[0], Settings.endInterpolationKm);
            /* Identify the boundaries of the sections for the section utilisation. */
            List<Section> sections = findAllSections(utilisationTrains[0], Settings.endInterpolationKm);

            /* Identify all the trains that stop at a loop. */
            List<TrainPair> trainPairs = findAllStoppingTrains(interpolatedTrains, loopLocations, Settings.stoppingSpeedThreshold, Settings.restartSpeedThreshold);

            /* Identify the corresponding through trains for the trains that have stopped.  */
            populateThroughTrains(trainPairs, loopLocations, interpolatedTrains, Settings.throughTrainTime);
            List<TrainPair> allTrainPairs = trainPairs;

            /******************************************************************************************************/
            
            #region transactionTimeCalculations

            /* Ignore the train pairs where we do not have an identified through train. */
            trainPairs = trainPairs.Where(p => p.throughTrain != null).ToList();

            /* Order the remaining pairs by loop location, stopping train direction and ID. */
            trainPairs = trainPairs.OrderBy(t => t.loopLocation.loopStart).ThenBy(t => t.stoppedTrain.trainDirection).ThenBy(t => t.stoppedTrain.trainID).ToList();
            trainPairs = calculateTransactionTime(trainPairs, interpolatedSimulations, Settings.maxDistanceToTrackSpeed, Settings.trackSpeedFactor, Settings.interpolationInterval, Settings.trainLength);

            /* Remove outliers [ie transaction time greater than transaction time outlier threshold (10 min)]*/
            trainPairs = trainPairs.Where(p => p.transactionTime < Settings.transactionTimeOutlierThreshold).ToList();

            /* Generate the statistics for the list of train pairs in each loop */
            List<TrainPairStatistics> stats = new List<TrainPairStatistics>();
            foreach (LoopLocation loop in loopLocations)
            {
                /* Extract the relavant train pairs that are associated to the current loop. */
                List<TrainPair> pairs = trainPairs.Where(p => p.loopLocation == loop).ToList();
                if (pairs.Count > 0)
                    stats.Add(TrainPairStatistics.generateStats(pairs));
            }
            stats.Add(TrainPairStatistics.generateStats(trainPairs));
            stats[stats.Count() - 1].Category = "All loops";

            /* Write the train pairs to file grouped by loop location. */
            FileOperations.writeTrainPairs(trainPairs, loopLocations, Settings.aggregatedDestination);
            FileOperations.wrtieTrainPairStatistics(stats, Settings.aggregatedDestination);

            #endregion

            /******************************************************************************************************/
            
            # region utilisationCalculations

            /* Initialise the current train properties. */
            Train train = new Train();
            List<TrainJourney> currentTrainJourney = new List<TrainJourney>();
            List<TrainJourney> stopInLoopJourney = new List<TrainJourney>();

            /* Initialise the time range to search for trains in each section. */
            DateTime currentTime = Settings.dateRange[0];
            DateTime previousStartTime = new DateTime();
            DateTime timeTrainEntersSection = new DateTime();
            DateTime timeTrainStopsInLoop = Settings.dateRange[1];
            DateTime endTime = new DateTime();
            /* Flag to indicate to ignore a train due to lack of data. */
            bool ignoreTrain = false;

            /* Create a list of occupation block objects to maintain utilisation calculations. */
            List<OccupationBlock> occupationBlock = new List<OccupationBlock>();
            /* Create a list of each occupation block for each section. */
            List<List<OccupationBlock>> sectionUtilisation = new List<List<OccupationBlock>>();

            /* A sorted dictionary list of ustilisation section for rolling 24 hourly period. */
            SortedDictionary<string, List<double>> rollingUtilisation = new SortedDictionary<string, List<double>>();
            List<double> rollingSectionUtilisation = new List<double>();
            
            /* Calcualte how many 24 hour periods need to be processed. */
            int hourlyPeriods = (int)(Settings.dateRange[1] - Settings.dateRange[0]).TotalHours;

            /* Initialise boundary parameters. */
            double trainLength = Settings.trainLength;
            double stoppingSpeed = Settings.restartSpeedThreshold;

            /* Loop through each sections. */
            foreach (Section section in sections)
            {
                /* Find the first train that occupies the section. */
                train = firstTrainInSection(utilisationTrains, section, currentTime);
                currentTrainJourney = train.journey;

                /* Reset the current time to the earliest time for this train, where it was still in the section. 
                 * - This is the start time for the utilisation of the current train.
                 */
                currentTime = currentTrainJourney.Where(t => t.kilometreage > section.sectionStart && t.kilometreage < section.sectionEnd &&
                    t.speed > 0).Min(t => t.dateTime);
                
                /* Flag, to indicate that the current section block is still active. */
                bool activeOccupationBlock = true;
                
                int a = 1;

                /* Continue searching for trains in the current section until the end of the analysis period is reached. */
                while (currentTime < Settings.dateRange[1])
                {
                    /* While the section block is active, continue searching for the end of the block. */
                    while (activeOccupationBlock)
                    {
                        /* Set the current journey to the current train. */
                        currentTrainJourney = train.journey;

                        /* If there is a train in the loop waiting to occupy the section, or if there is a 
                         * train about to enter the section, continue with the new train. */
                        if (isTrainWaitingToOccupySection(train, trainPairs, section))
                        {
                            //train = findTrainWaitingInLoop(train, trainPairs, section);


                            /* Extract the part of journey of that is in the current section. */
                            if (train.trainDirection == direction.IncreasingKm)
                                currentTrainJourney = currentTrainJourney.Where(t => t.kilometreage > section.sectionStart && 
                                    t.kilometreage < section.sectionEnd + trainLength &&
                                    t.speed > stoppingSpeed && t.dateTime > endTime).ToList();

                            else
                                currentTrainJourney = currentTrainJourney.Where(t => t.kilometreage > section.sectionStart - trainLength && 
                                    t.kilometreage < section.sectionEnd &&
                                    t.speed > stoppingSpeed && t.dateTime > endTime).ToList();

                            /* Find the time the current train leaves the section. */
                            if (currentTrainJourney.Count() > 0)
                            {
                                /* remove when finished testing */
                                validateJourney(currentTrainJourney, train.trainID, train.trainDirection);
                                endTime = currentTrainJourney.Max(t => t.dateTime);
                            }
                            else
                            {
                                ignoreTrain = true;
                                activeOccupationBlock = false;
                                break;
                            }

                            //endTime = currentTrainJourney.Where(t => t.kilometreage > section.sectionStart && t.kilometreage < section.sectionEnd &&
                            //    t.speed > 0 && t.dateTime > endTime).Max(t => t.dateTime);

                            /* Find the next train in the section */
                            train = firstTrainInSection(utilisationTrains, section, endTime);
                            
                            previousStartTime = currentTime;

                            /* Find the time the next train enters the section. */
                            timeTrainEntersSection = train.journey.Where(t => t.kilometreage > section.sectionStart && t.kilometreage < section.sectionEnd &&
                                t.speed > 0 //&& t.dateTime > endTime
                                ).Min(t => t.dateTime);

                            /* IF: the train entered the section > 10 minutes after the previous train cleared, 
                             * set the end time to the time the previous train cleared the section. 
                             */
                            if ((timeTrainEntersSection - endTime).TotalMinutes > Settings.transactionTimeOutlierThreshold)
                            {
                                activeOccupationBlock = false;
                                break;
                            }
                            
                        }
                        else
                        {
                            // May not want the speed to be above restart when trying to find the train leaves the section???

                            /* Determine the time the rear of the train leaves the section. */
                            if (train.trainDirection == direction.IncreasingKm)
                            {
                                /* Find the point the train stops, if there is one */
                                stopInLoopJourney = currentTrainJourney.Where(j => j.kilometreage > section.sectionEnd &&
                                    j.kilometreage < section.sectionEnd + trainLength && 
                                    j.speed > 0 && j.speed < Settings.stoppingSpeedThreshold).ToList();
                                
                                if (stopInLoopJourney.Count > 0)
                                    timeTrainStopsInLoop = stopInLoopJourney.Min(j => j.dateTime);
                                
                                currentTrainJourney = currentTrainJourney.Where(j => j.kilometreage > section.sectionStart &&
                                    j.kilometreage < section.sectionEnd + trainLength && j.dateTime < timeTrainStopsInLoop && j.speed > stoppingSpeed).ToList();
                            }
                            else
                            {
                                /* Find the point the train stops, if there is one */
                                stopInLoopJourney = currentTrainJourney.Where(j => j.kilometreage > section.sectionStart - trainLength &&
                                    j.kilometreage < section.sectionStart &&
                                    j.speed > 0 && j.speed < Settings.stoppingSpeedThreshold).ToList();
                                
                                if (stopInLoopJourney.Count > 0)
                                    timeTrainStopsInLoop = stopInLoopJourney.Min(j => j.dateTime);
                                
                                currentTrainJourney = currentTrainJourney.Where(j => j.kilometreage > section.sectionStart - trainLength &&
                                    j.kilometreage < section.sectionEnd && j.dateTime < timeTrainStopsInLoop && j.speed > stoppingSpeed).ToList();


                            }

                            if (currentTrainJourney.Count() > 0)
                            {
                                /* remove when finished testing */
                                validateJourney(currentTrainJourney, train.trainID,train.trainDirection); // only required for testing if there is any gaps in the journey extracted.
                                endTime = currentTrainJourney.Max(j => j.dateTime);
                            }
                            else
                                /* The boundary of the train has been reached. */
                                ignoreTrain = true;

                            if (endTime < currentTime)
                                /* Need to deal with trains that appear to stop in the middle of the section. 
                                 * This is likely due to drivers not turning the loco ID off at the end of the journey. */
                                ignoreTrain = true;

                            /* Release the active section flag for the next iteration. */
                            activeOccupationBlock = false;
                        }
                    }

                    /* Add the current section block details to the list. */
                    if (!ignoreTrain)
                    {
                        occupationBlock.Add(new OccupationBlock(section, currentTime, endTime));
                        previousStartTime = currentTime;
                        
                    }
                    else
                    {
                        /* Adjsut the end time to find the next train after the train we are ignoring. */
                        ignoreTrain = false;
                        endTime = currentTime.AddMinutes(5);
                    }

                    /* Find the next train that occupies the section. */
                    train = firstTrainInSection(utilisationTrains, section, endTime);
                    if (train != null)
                    {
                        currentTime = train.journey.Where(t => t.kilometreage > section.sectionStart && t.kilometreage < section.sectionEnd &&
                            t.speed > 0 && t.dateTime > endTime).Min(t => t.dateTime);
                        

                    }
                    /* If there are no more train's in the current section, continue to the next section. */
                    else
                        break;
                    
                    /* Reset the active section flag */
                    activeOccupationBlock = true;
                    timeTrainStopsInLoop = Settings.dateRange[1];
                }
                
                /**********************************************************************************************/
                /* Maybe make this section a function. */

                /* Aggregate the time to 24 hour periods. */
                for (int periodIdx = 0; periodIdx < hourlyPeriods; periodIdx++)
                {
                    /* Define the time boundaries for the sublist. */
                    DateTime startDate = Settings.dateRange[0].AddHours(periodIdx);
                    DateTime endDate = startDate.AddDays(1);
                                        
                    /*  Extract the sublist and sum the times */
                    double totalMinutesOccupied = occupationBlock.Where(u => u.startTime >= startDate && u.endTime < endDate).Sum(u => u.minutesOccupied);

                    /* Add to the dictionary*/
                    rollingSectionUtilisation.Add(totalMinutesOccupied);

                }
                
                /**********************************************************************************************/

                /* Buld the key string for the dictionary. */
                string key = string.Format("{0:0.00} km - {1:0.00} km", section.sectionStart, section.sectionEnd);
                rollingUtilisation[key] = new List<double> (rollingSectionUtilisation);
                rollingSectionUtilisation.Clear();
                
                /* List of each utilsation block in each section. */
                sectionUtilisation.Add(new List<OccupationBlock>(occupationBlock));
                occupationBlock.Clear();

                /* Reset the search time to the start for the next section. */
                currentTime = Settings.dateRange[0];
                endTime = currentTime;


            }
            # endregion

            /* Write the occupation blocks for each section to file.  */
            FileOperations.writeSectionUtilistion(sectionUtilisation, Settings.aggregatedDestination);

            /* Write the aggregated rolling 24 hour utilisation times to file for each section. */
            FileOperations.writeRollingUtilistion(rollingUtilisation, Settings.aggregatedDestination, Settings.dateRange);

        }
        
        private static void validateJourney(List<TrainJourney> journey,string trainId, direction direction)
        {
            double difference = 0;

            for (int i = 1; i < journey.Count(); i++)
            {
                difference = journey[i].kilometreage - journey[i - 1].kilometreage;
                if (difference > 0.050001)
                    Console.WriteLine("{0}: [{1} - {2}]- {3}:: There is a difference of {4} in the current Train journey.",trainId, i,journey.Count(), direction.ToString(), difference);
            }
        }

        /// <summary>
        /// Determine if there is a train in the loop waiting to occupy the current section.
        /// </summary>
        /// <param name="train">The current train in the section.</param>
        /// <param name="trainPairs">A list of all train pairs that have been identified.</param>
        /// <param name="section">The section being occupied.</param>
        /// <returns>A flag indicating if there is a train in teh loop waiting to occupy the section.</returns>
        private static bool isTrainWaitingToOccupySection(Train train, List<TrainPair> trainPairs, Section section)
        {
            /* Find a train that is waiting in the loop. */
            Train trainInLoop = findTrainWaitingInLoop(train, trainPairs, section);

            /* If there is not train in the loop, there is no train waiting to occupy the section. */
            if (trainInLoop != null)
                return true;
            else 
                return false;

        }

        /// <summary>
        /// Find the train waiting in the loop as the current train passes it.
        /// </summary>
        /// <param name="train">The train occupying the section.</param>
        /// <param name="trainPairs">A list of all train pairs that have been identified.</param>
        /// <param name="section">The section being occupied.</param>
        /// <returns>The train waiting in the loop or null, if no train exists.</returns>
        public static Train findTrainWaitingInLoop(Train train, List<TrainPair> trainPairs, Section section)
        {
            /* Search through each train pairs */
            foreach (TrainPair pair in trainPairs)
            {
                /* If the current train is in the increasing direction, check the loop at the end of the section. */
                if (train.trainDirection == direction.IncreasingKm)
                {
                    /* The difference needs to be between 0 and 0.5 km to allow for the alignment between loops and sections. */
                    if (Math.Abs(pair.loopLocation.loopStart - section.sectionEnd) < 0.050001 &&
                        train == pair.throughTrain)
                        /* Return the stopped train. */
                        return pair.stoppedTrain;
                }
                else
                {
                    /* Check the loop at the begining of the section. */
                    if (Math.Abs(pair.loopLocation.loopEnd - section.sectionStart) < 0.050001 &&
                            train == pair.throughTrain)
                        /* Return the stopped train. */
                        return pair.stoppedTrain;
                }

            }
            return null;
        }

        /// <summary>
        /// Find the next train in the section.
        /// </summary>
        /// <param name="trains">A list of all train in the data.</param>
        /// <param name="section">The current section being occupied.</param>
        /// <param name="currentTime">The time of day to search for the next train.</param>
        /// <returns>The next train in the section, or null if none exists.</returns>
        public static Train firstTrainInSection(List<Train> trains, Section section, DateTime currentTime)
        {
            /* Define a sub journey list to define a trains journey inside the current section. */
            List<TrainJourney> sectionJourney = new List<TrainJourney>();
            /* The earliest time found ahead of the current time. */
            DateTime minimumTime = DateTime.MaxValue;
            DateTime time = new DateTime();
            /* The train object that will be the next train in section. */
            Train firstTrain = null;

            /* Loop through each train to find the next train in the section */
            foreach (Train train in trains)
            {
                /* Find the time the front of the train enterest the section. */
                if (train.trainDirection == direction.IncreasingKm)
                    sectionJourney = train.journey.Where(t => t.kilometreage > section.sectionStart && t.kilometreage < section.sectionEnd &&
                        t.speed > 0 && t.dateTime > currentTime).ToList();
                else
                    sectionJourney = train.journey.Where(t => t.kilometreage > section.sectionStart && t.kilometreage < section.sectionEnd && 
                        t.speed > 0 && t.dateTime > currentTime).ToList();

                /* If there are points in the section journey, find the earliest time, greater than the current search time. */
                if (sectionJourney.Count > 0)
                {
                    time = sectionJourney.Min(t => t.dateTime);

                    /* Find the train with the minimum time. */
                    if (time < minimumTime)
                    {
                        minimumTime = time;
                        firstTrain = train;
                    }
                }
            }

            /* Return the next train in the section. */
            return firstTrain;
        }
        
        /// <summary>
        /// Find the locations of all loops
        /// </summary>
        /// <param name="train">A typical train</param>
        /// <param name="endInterpolationKm">The end of the interpolation region.</param>
        /// <returns>A list of loop locations.</returns>
        public static List<LoopLocation> findAllLoopLocations(Train train, double endInterpolationKm)
        {
            List<TrainJourney> journey = train.journey;
            List<LoopLocation> loopLocations = new List<LoopLocation>();

            /* Loop location parameters */
            bool startLoopFound = false;
            double start = 0, end = 0;

            for (int index = 0; index < journey.Count(); index++)
            {
                /* Find the start of the loop */
                if (!startLoopFound && journey[index].isLoopHere)
                {
                    /* If the start of the loop is found, set the start parameter. */
                    startLoopFound = true;
                    start = journey[index].kilometreage;
                }

                /* Only when the start is found, look for the end of the loop. */
                if (startLoopFound && !journey[index].isLoopHere)
                {
                    /* If the end of the loop is found, set the end parameter. */
                    startLoopFound = false;
                    end = journey[index - 1].kilometreage;

                    /* Create a loop location if its not too close to the end of the data. */
                    // 30 km prior to the end of the interpolation to allow the through trains to be found.
                    /* This is somewhat of an arbitrary number based on Gunnedah basin trains. */
                    if (end < endInterpolationKm - 30)
                        loopLocations.Add(new LoopLocation(start, end));

                }
            }
            /* Return the loop locations. */
            return loopLocations;
        }
        
        /// <summary>
        /// Find the boundaries of the sections
        /// </summary>
        /// <param name="train">A typical train</param>
        /// <param name="endInterpolationKm">The end of the interpolation region.</param>
        /// <returns>A list of section boundaries.</returns>
        public static List<Section> findAllSections(Train train, double endInterpolationKm)
        {
            List<TrainJourney> journey = train.journey;
            List<Section> section = new List<Section>();

            /* Loop location parameters */
            bool sectionFound = true;
            double start = 0, end = 0;

            /* Loop through the first train for the loop locations */
            for (int index = 0; index < journey.Count(); index++)
            {
                /* Find the start of each section */
                if (!sectionFound && !journey[index].isLoopHere)
                {
                    sectionFound = true;
                    start = journey[index].kilometreage;
                }

                /* The beginning of the next loop is the end of the current section. */
                if (sectionFound && journey[index].isLoopHere)
                {
                    end = journey[index-1].kilometreage;
                    sectionFound = false;

                    if(start !=0 && end != 0)
                        section.Add(new Section(start, end));

                }


            }
            /* Return the loop locations. */
            return section;
        }
        
        /// <summary>
        /// Find the trains that stop within a loop.
        /// The lowest speed within a loop is considered to be the stoppping location.
        /// The restart location is assumed to be where the train reaches a minimum threshold.
        /// </summary>
        /// <param name="trains">A list of trains to search through.</param>
        /// <param name="loopLocations">A list of locatons a train may stop at.</param>
        /// <param name="stoppingSpeedThreshold">The minimum speed a train is travelling before its considered to stop.</param>
        /// <param name="restartSpeedThreshold">The minimum speed a train need to reach before it is considered to have restarted.</param>
        /// <returns>A list of train pairs that do not have the through trains identified.</returns>
        public static List<TrainPair> findAllStoppingTrains(List<Train> trains, List<LoopLocation> loopLocations, double stoppingSpeedThreshold, double restartSpeedThreshold)
        {
            /* Set the minimum speed. */
            double minSpeed = double.MinValue;
            List<TrainJourney> speeds = new List<TrainJourney>();
            /* Create a list of pairs to store the stopped trains and the details of the cross. */
            List<TrainPair> trainPairs = new List<TrainPair>();

            /* loop through the trains to find the trains that stop in loops. */
            foreach (Train train in trains)
            {
                /* loop through each loop location. */
                for (int loopIdx = 0; loopIdx < loopLocations.Count(); loopIdx++)
                {
                    /* Find the minimum speed within the loop. */
                    speeds = train.journey.Where(t => t.interpolate == true).
                                Where(t => t.kilometreage >= loopLocations[loopIdx].loopStart).
                                Where(t => t.kilometreage <= loopLocations[loopIdx].loopEnd).ToList();
                    /* Determine the minimum speed of the train journey items within the boundaries of the loop. */
                    if (speeds.Count() > 0)
                        minSpeed = speeds.Min(t => t.speed);
                    else
                        /* Default minimum speed. */
                        minSpeed = 0;
                    
                    /* If the minimm speed is below the threshold, it is considered to be stopping within the loop. */
                    if (minSpeed > 0 && minSpeed < stoppingSpeedThreshold)
                    {
                        /* Find the index, where the train stops. */
                        int stopIdx = train.journey.FindIndex(t => t.speed == minSpeed);

                        /* Determine the appropriate index where the train restarts. The train must be 
                         * above a specified speed threshold to be considered to have restarted.
                         */
                        int restartIdx = 0;
                        if (train.trainDirection == direction.IncreasingKm)
                            restartIdx = train.journey.FindIndex(stopIdx, t => t.speed >= restartSpeedThreshold);
                        else
                            restartIdx = train.journey.FindLastIndex(stopIdx, stopIdx - 1, t => t.speed >= restartSpeedThreshold);
    
                        /* Add the stopped train parameters to the list. */
                        if (stopIdx > 0 && restartIdx > 0)
                        {
                            /* Fine tune the restart locaton. */
                            restartIdx = fineTuneRestart(train.journey, restartIdx, train.trainDirection);
                            /* Add the stopped train and cross details to the list. */
                            trainPairs.Add(new TrainPair(train, null, loopLocations[loopIdx], train.journey[stopIdx].kilometreage, train.journey[restartIdx].kilometreage));
                        }
                    }

                }

            }
            /* Sort the train pairs in order of loops */
            trainPairs = trainPairs.OrderBy(t => t.loopLocation.loopStart).ThenBy(t => t.stoppedTrain.trainID).ToList();

            return trainPairs;

        }
        
        /// <summary>
        /// Fine tune the stopped train restart location.
        /// </summary>
        /// <param name="journey">The train journey details.</param>
        /// <param name="index">The initial restart index.</param>
        /// <param name="direction">The train direction.</param>
        /// <returns>A new index for the restart location.</returns>
        private static int fineTuneRestart(List<TrainJourney> journey, int index, direction direction)
        {
            /* Define evaluation variables. */
            double timeDiff = 0;
            int increment = 1;
            int restartIndex = index;

            /*  Determine the increment value based on the train direction. */
            if (direction == direction.DecreasingKm)
                increment = -1;
                            
            /* Fine tune the restart location by looking up to two point ahead for a large time difference. */
            timeDiff = (journey[index + increment].dateTime - journey[index].dateTime).TotalSeconds;

            /* A time gap of greater than 30 seconds is considered large. */
            /* It takes 36 sec for a train travelling at 5 kph to travel a distance of 50m. 
             * So a gap of less than 30 sec, the train will be travelling faster than 5 kph. 
             */
            if (timeDiff > fineTuneTimeGap_sec)
            {
                restartIndex = index + increment;

                timeDiff = (journey[index + 2 * increment].dateTime - journey[index + increment].dateTime).TotalSeconds;
                /* Check the next point ahead for a large gap in time. */
                if (timeDiff > fineTuneTimeGap_sec)
                    restartIndex = index + 2 * increment;
            }

            return restartIndex;
          
        }

        /// <summary>
        /// Find the through trains at each loop associated to the stopped trains.
        /// </summary>
        /// <param name="trainPairs">A list of train pairs.</param>
        /// <param name="loopLocations">A list of loop locations.</param>
        /// <param name="trains">A list of trains that may or may not include the through train.</param>
        /// <param name="timeThreshold">The maximum time difference between the stopped train and the through train.</param>
        public static void populateThroughTrains(List<TrainPair> trainPairs, List<LoopLocation> loopLocations, List<Train> trains, double timeThreshold)
        {
            double minDifference = double.MaxValue;
            Train keepThrough = new Train();

            /* Search through each loop location. */
            foreach (LoopLocation loop in loopLocations)
            {
                /* Cycle through the train pairs. */
                foreach (TrainPair pair in trainPairs)
                {
                    /* If the current train pair stopped at the current loop, search for the through train. */
                    if (pair.loopLocation == loop)
                    {
                        Train stopped = pair.stoppedTrain;
                        minDifference = double.MaxValue;
                        keepThrough = null;

                        /* Search through each train for the corresponding through train. */
                        foreach (Train throughTrain in trains)
                        {
                            /* Find the location where the stopped train restarts. */
                            int idx = throughTrain.journey.FindIndex(t => t.kilometreage == pair.restartLocation);
                            
                            if (throughTrain.trainDirection != pair.stoppedTrain.trainDirection && throughTrain.journey[idx].interpolate)
                            {
                                /* Calcualte the time difference of each train at the restart location. */
                                double timeDifference = (pair.stoppedTrain.journey[idx].dateTime - throughTrain.journey[idx].dateTime).TotalMinutes;

                                if (timeDifference > -5 && timeDifference < minDifference) // timeDifference > -1
                                {
                                    /* only keep the through train that is closest in time to the stopped train. */
                                    minDifference = timeDifference;
                                    keepThrough = throughTrain;
                                }
                                                               
                            }

                        }
                        /* Populate the through train. */
                        pair.throughTrain = keepThrough;
                    } 
                }   
            }   
        }

        /// <summary>
        /// Calculate the transaction time of all train pairs
        /// </summary>
        /// <param name="trainPairs">A list of train pairs.</param>
        /// <param name="interpolatedSimulations">A list of simualtions.</param>
        /// <param name="maxDistanceToTrackSpeed">The maximum permitted distance to achieve track speed.</param>
        /// <param name="trackSpeedFactor">The multiplication factor applied to the simualtion speed for comparison.</param>
        /// <param name="interpolationInterval">The interpolation interval.</param>
        /// <param name="trainLength">The train length to define where the train will clear the loop.</param>
        /// <returns>A list of train pairs which have the transaction time components populated.</returns>
        public static List<TrainPair> calculateTransactionTime(List<TrainPair> trainPairs, List<Train> interpolatedSimulations, double maxDistanceToTrackSpeed, double trackSpeedFactor, double interpolationInterval, double trainLength)
        {
            List<int> keepIdx = new List<int>();
            Train simulation = new Train();
            double timeToReachTrackSpeed, simulatedTrainTime, timeToClearLoop;
            double transactionTime = 0;
            double distanceToTrackSpeed = 0;

            /* Cycle through each train pair. */
            foreach (TrainPair pair in trainPairs)
            {
                
                timeToReachTrackSpeed = 0;
                simulatedTrainTime = 0;
                timeToClearLoop = 0;
                transactionTime = 0;
                distanceToTrackSpeed = 0;

                /* Extract the appropriate simulation for comparison. */
                simulation = pair.matchToSimulation(interpolatedSimulations);

                /* Calcualte the time components for the transaction time. */
                List<double> transactionTimeComponents = new List<double>();
                transactionTimeComponents = calculateTransactionTimeComponents(pair, simulation, maxDistanceToTrackSpeed, trackSpeedFactor, interpolationInterval, trainLength);

                /* Extract each individual time component */
                timeToReachTrackSpeed = transactionTimeComponents[0];
                simulatedTrainTime = transactionTimeComponents[1];
                timeToClearLoop = transactionTimeComponents[2];
                /* Extract the required distance to reach track speed. */
                distanceToTrackSpeed = transactionTimeComponents[3];

                /* Calculate the transaction time. */
                transactionTime = timeToReachTrackSpeed - simulatedTrainTime + timeToClearLoop;

                /* Identify the pairs that should be kept based on the distance required to achieve 
                 * track speed and positive transaction time.
                 */
                if (timeToClearLoop > 0 && transactionTime > 0 && distanceToTrackSpeed < maxDistanceToTrackSpeed)
                    keepIdx.Add(trainPairs.IndexOf(pair));

                /* Populate the transaction time and its componenets for each train pair. */
                pair.timeForStoppedTrainToReachTrackSpeed = timeToReachTrackSpeed;
                pair.distanceToTrackSpeed = distanceToTrackSpeed;
                pair.simulatedTrainToReachTrackSpeedLocation = simulatedTrainTime;
                pair.timeBetweenClearingLoopAndRestart = timeToClearLoop;
                pair.transactionTime = transactionTime;

            }

            /* Select only the train pairs that are valid. */
            trainPairs = keepIdx.Select(item => trainPairs[item]).ToList();

            return trainPairs;
        }

        /// <summary>
        /// Calculate the components of the transaction time. 
        /// This includes the time between the through train clearing the loop and the 
        /// stopped train to restart, the time the stopped train takes to get to track 
        /// speed and the corresponding time for the simulated train to reach the 
        /// track speed location.
        /// </summary>
        /// <param name="pair">A train pair.</param>
        /// <param name="simulation">The corresponding simualtion.</param>
        /// <param name="maxDistanceToTrackSpeed">The maximum permitted distance to achieve track speed.</param>
        /// <param name="trackSpeedFactor">The multiplication factor applied to the simualtion speed for comparison.</param>
        /// <param name="interpolationInterval">The interpolation interval.</param>
        /// <param name="trainLength">The train length to define where the train will clear the loop.</param>
        /// <returns>A list containing the transaction time components and the distance required to achieve track speed.</returns>
        public static List<double> calculateTransactionTimeComponents(TrainPair pair, Train simulation, double maxDistanceToTrackSpeed, double trackSpeedFactor, double interpolationInterval, double trainLength)
        {
            double timeToReachTrackSpeed = 0;
            double simulatedTrainTime = 0;
            double timeToClearLoop = 0;
            double distanceToTrackSpeed = 0;
            DateTime restartTime = DateTime.MinValue;
            DateTime clearanceTime = DateTime.MinValue;

            int increment = 0;
            double loopSide = 0;
            int trackSpeedLocationIdx = 0;
            int restartIdx = 0;

            /* Define the changing parameters based on the direction of the stopping train. */
            if (pair.stoppedTrain.trainDirection == direction.IncreasingKm)
            {
                increment = 1;
                loopSide = pair.loopLocation.loopEnd;

                trackSpeedLocationIdx = pair.stoppedTrain.journey.FindIndex(t => t.kilometreage == loopSide) + increment * 2;
                restartIdx = pair.stoppedTrain.journey.FindIndex(t => t.kilometreage == pair.restartLocation) + increment * 2;

                /* Get the index for the latest of the restart time and the loop end. */
                trackSpeedLocationIdx = Math.Max(trackSpeedLocationIdx, restartIdx);
            }
            else
            {
                increment = -1;
                loopSide = pair.loopLocation.loopStart;

                trackSpeedLocationIdx = pair.stoppedTrain.journey.FindIndex(t => t.kilometreage == loopSide) + increment * 2;
                restartIdx = pair.stoppedTrain.journey.FindIndex(t => t.kilometreage == pair.restartLocation) + increment * 2;

                /* Get the index for the latest of the restart time and the loop end. */
                trackSpeedLocationIdx = Math.Min(trackSpeedLocationIdx, restartIdx);
            }

            /* find the location where the train reaches 90% of simulated train speed. */
            while (pair.stoppedTrain.journey[trackSpeedLocationIdx].speed < trackSpeedFactor * simulation.journey[trackSpeedLocationIdx].speed)
            {
                /* Increment the distance travelled, and the time for the stopped train and the simulation to reach the track speed location. */
                distanceToTrackSpeed += interpolationInterval * Processing.metresToKilometers;
                timeToReachTrackSpeed += (pair.stoppedTrain.journey[trackSpeedLocationIdx].dateTime - pair.stoppedTrain.journey[trackSpeedLocationIdx - increment].dateTime).TotalMinutes;
                simulatedTrainTime += (simulation.journey[trackSpeedLocationIdx].dateTime - simulation.journey[trackSpeedLocationIdx - increment].dateTime).TotalMinutes;

                /* Check we are not trying to continue outside the bounds of the train journey. */
                if (trackSpeedLocationIdx == 1 || trackSpeedLocationIdx == pair.stoppedTrain.journey.Count() - 2)
                    break;

                /* Increment to the next index in the direction the train is travelling. */
                trackSpeedLocationIdx = trackSpeedLocationIdx + increment;

            }

            /* Add the last point */
            trackSpeedLocationIdx = trackSpeedLocationIdx + increment;
            timeToReachTrackSpeed += (pair.stoppedTrain.journey[trackSpeedLocationIdx].dateTime - pair.stoppedTrain.journey[trackSpeedLocationIdx - increment].dateTime).TotalMinutes;
            simulatedTrainTime += (simulation.journey[trackSpeedLocationIdx].dateTime - simulation.journey[trackSpeedLocationIdx - increment].dateTime).TotalMinutes;
            
            /* Time when the stopped train restarts. */
            restartTime = pair.stoppedTrain.journey[restartIdx].dateTime;
            /* Time when the through train clears the loop. */
            clearanceTime = pair.throughTrain.journey[pair.throughTrain.journey.FindIndex(t => t.kilometreage >= loopSide - increment * trainLength)].dateTime;
            /* Calculate the time between the through train clearing the loop and the stopped train to restart. */
            timeToClearLoop = (restartTime - clearanceTime).TotalMinutes;

            /* Return the time compenents as a list. */
            return new List<double>(new double[] { timeToReachTrackSpeed, simulatedTrainTime, timeToClearLoop, distanceToTrackSpeed });
        }


        
    }
}
