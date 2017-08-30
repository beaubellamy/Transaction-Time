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
            
            if (TrainRecords.Count() == 0)
            {
                Tools.messageBox("There are no records in the list to analyse.", "No trains available.");
                return;
            }
            List<trainOperator> operators = TrainRecords.Select(t => t.trainOperator).Distinct().ToList();
            operators.Remove(trainOperator.Unknown);
            int numberOfOperators = operators.Count();

            /* Set simulation catagories [TRAP:Set analysis catagories] */
            List<Category> simCategories = new List<Category>();
            simCategories.Add(Processing.convertTrainOperatorToCategory(trainOperator.PacificNational));
            simCategories.Add(Processing.convertTrainOperatorToCategory(trainOperator.Aurizon));
            /* TRAP: Multiple if conditions */
            /* If further development is required to use different train types (ie, operators, by commodity, etc), 
             * the neccesasry code is in the TRAP tool. 
             */

            /* Create the list of simulated trains. */
            List<Train> simulatedTrains = new List<Train>();

            /* Read in the simulation data and interpolate to the desired granularity. */
            for (int index = 0; index < simCategories.Count(); index++)
            {
                simulatedTrains.Add(FileOperations.readSimulationData(Settings.simulationFiles[index * 2], simCategories[index], direction.IncreasingKm));
                simulatedTrains.Add(FileOperations.readSimulationData(Settings.simulationFiles[index * 2 + 1], simCategories[index], direction.DecreasingKm));
            }

            /* Interpolate the simulations to the same granularity as the ICE data will be. */
            List<Train> interpolatedSimulations = new List<Train>();
            interpolatedSimulations = Processing.interpolateTrainData(simulatedTrains, trackGeometry, Settings.startInterpolationKm, Settings.endInterpolationKm, Settings.interpolationInterval);

            /* Sort the data by [trainID, locoID, Date & Time, kmPost]. */
            List<TrainRecord> OrderdTrainRecords = new List<TrainRecord>();
            OrderdTrainRecords = TrainRecords.OrderBy(t => t.trainID).ThenBy(t => t.locoID).ThenBy(t => t.dateTime).ThenBy(t => t.kmPost).ToList();

            List<Train> trainList = new List<Train>();
            trainList = Processing.CleanData(OrderdTrainRecords, trackGeometry, Settings.timethreshold, Settings.distanceThreshold, Settings.minimumJourneyDistance, Settings.analysisCategory);
            
            /* Write the raw data to file - usefull for creating train graphs */
            //FileOperations.writeRawTrainDataWithTime(trainList, Settings.aggregatedDestination);

            /* Interpolate data */
            /******** Should only be required while we are waiting for the data in the prefered format ********/
            List<Train> interpolatedTrains = new List<Train>();
            interpolatedTrains = Processing.interpolateTrainDataWithGaps(trainList, trackGeometry, Settings.startInterpolationKm, Settings.endInterpolationKm, Settings.interpolationInterval);
            
            /**************************************************************************************************/
            
            /* Write the interpolated data to file. */
            FileOperations.writeTrainDataWithTime(interpolatedTrains, Settings.startInterpolationKm, Settings.interpolationInterval, Settings.aggregatedDestination);
            
            /* Identify all the potential loop locations where a train may be forced to stop. */
            List<LoopLocation> loopLocations = findAllLoopLocations(interpolatedTrains[0], Settings.endInterpolationKm);

            /* Identify all the trains that stop at a loop. */
            List<TrainPair> trainPairs = findAllStoppingTrains(interpolatedTrains, loopLocations, Settings.stoppingSpeedThreshold, Settings.restartSpeedThreshold);
            
            /* Identify the corresponding through trains for the trains that have stopped.  */
            populateThroughTrains(trainPairs, loopLocations, interpolatedTrains, Settings.throughTrainTime);

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
                    // 30 km prior to the end of the interpolation to allw the through trains to be found.
                    if (end < endInterpolationKm - 30)
                        loopLocations.Add(new LoopLocation(start, end));

                }
            }
            /* Return the loop locations. */
            return loopLocations;
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

                                if (timeDifference > -1 && timeDifference < minDifference)
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
                if (transactionTime > 0 && distanceToTrackSpeed < maxDistanceToTrackSpeed)
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
