using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using TrainLibrary;
using Statistics;
using IOLibrary;

using Microsoft.Office.Interop.Excel;

namespace TransactionTime
{
    ///// <summary>
    ///// A class describing the pair of trains that interact in a conflict.
    ///// Ie. one train stops and the other goes through.
    ///// </summary>
    //public class TrainPair
    //{
    //    public Train stoppedTrain;
    //    public Train throughTrain;
    //    public LoopLocation loopLocation;
    //    public double stoppedLocation;
    //    public double restartLocation;

    //    public double timeForStoppedTrainToReachTrackSpeed;
    //    public double simulatedTrainToReachTrackSpeedLocation;
    //    public double timeBetweenClearingLoopAndRestart;
    //    public double transactionTime;

    //    /// <summary>
    //    /// Default train pair constructor.
    //    /// </summary>
    //    public TrainPair()
    //    {
    //        this.stoppedTrain = null;
    //        this.throughTrain = null;
    //        this.loopLocation = null;
    //        this.stoppedLocation = 0;
    //        this.restartLocation = 0;

    //        this.timeForStoppedTrainToReachTrackSpeed = 0;
    //        this.simulatedTrainToReachTrackSpeedLocation = 0;
    //        this.timeBetweenClearingLoopAndRestart = 0;
    //        this.transactionTime = 0;
    //    }

    //    /// <summary>
    //    /// Train pair constructor for each conflit at a loop.
    //    /// </summary>
    //    /// <param name="stopped">The stopped trains.</param>
    //    /// <param name="through">The train that continues through the loop.</param>
    //    /// <param name="location">The location of the loop.</param>
    //    /// <param name="stop">The kilometreage where the train stops.</param>
    //    /// <param name="restart">The kilometreage where the train restarts.</param>
    //    public TrainPair(Train stopped, Train through, LoopLocation location, double stop, double restart)
    //    {
    //        this.stoppedTrain = stopped;
    //        this.throughTrain = through;
    //        this.loopLocation = location;
    //        this.stoppedLocation = stop;
    //        this.restartLocation = restart;

    //        this.timeForStoppedTrainToReachTrackSpeed = 0;
    //        this.simulatedTrainToReachTrackSpeedLocation = 0;
    //        this.timeBetweenClearingLoopAndRestart = 0;
    //        this.transactionTime = 0;
    //    }
        
    //    /// <summary>
    //    /// Function gets the corresponding simulation to the instance of the stoped train.
    //    /// </summary>
    //    /// <param name="simulations">A list of simulations</param>
    //    /// <returns>The simulation that corresponds to the stopped train.</returns>
    //    public Train matchToSimulation(List<Train> simulations)
    //    {
    //        /* Cycle through all the simulations */
    //        foreach (Train train in simulations)
    //        {
    //            if (train.Category == Processing.convertTrainOperatorToCategory(this.stoppedTrain.trainOperator) &&
    //                train.trainDirection == this.stoppedTrain.trainDirection)
    //                return train;
    //        }

    //        /* This section should return an weighted average simualtion.
    //         * The weighted average simualtion need to be added to the list prior to calling the fucntion.
    //         */
    //        if (this.stoppedTrain.trainDirection == direction.IncreasingKm)
    //            return simulations[0];
    //        else
    //            return simulations[1];

    //    }

    //}

    ///// <summary>
    ///// A class defining the loop location by kilometreage.
    ///// </summary>
    //public class LoopLocation
    //{
    //    public double loopStart;
    //    public double loopEnd;

    //    /// <summary>
    //    /// Default loop location constructor
    //    /// </summary>
    //    public LoopLocation()
    //    {
    //        this.loopStart = 0;
    //        this.loopEnd = 1;
    //    }

    //    /// <summary>
    //    /// Loop location constructor
    //    /// </summary>
    //    /// <param name="location">A loop location object.</param>
    //    public LoopLocation(LoopLocation location)
    //    {
    //        this.loopStart = location.loopStart;
    //        this.loopEnd = location.loopEnd;
    //    }

    //    /// <summary>
    //    /// Loop location constructor
    //    /// </summary>
    //    /// <param name="start">The smallest kilometreage of the loop ends.</param>
    //    /// <param name="end">The largest kilometreage of the loop ends.</param>
    //    public LoopLocation(double start, double end)
    //    {
    //        this.loopStart = start;
    //        this.loopEnd = end;
    //    }

    //}




    class Program
    {

        static void Main(string[] args)
        {
            /* Form parameters */
            /* Set up default parameters */
            bool excludeListOfTrains = false;
            string trainListFile = null;        /* No trains to exclude. */
            string geometryFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\Gunnedah Basin Geometry.csv";
            //string temporarySpeedRestrictionFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\Gunnedah Basin TSR.csv";
            string dataFile = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\Gunnedah Basin Data 2017-20170529.txt";
            
            List<string> simulationFiles = new List<string>(new string[6]);
            simulationFiles[0] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\PacificNational-Increasing.csv";
            simulationFiles[1] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\PacificNational-Decreasing.csv";
            simulationFiles[2] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\Aurizon-Increasing-60.csv";
            simulationFiles[3] = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin\Aurizon-Decreasing.csv";

            string aggregatedDestination = @"S:\Corporate Strategy\Infrastructure Strategies\Simulations\Train Performance Analysis\Gunnedah Basin";

            analysisCategory analysisCategory = analysisCategory.TrainOperator;

            DateTime[] dateRange = new DateTime[2];
            dateRange[0] = new DateTime(2017,1,5);
            dateRange[1] = new DateTime(2017,1,10);

            double startInterpolationKm = 264;
            double endInterpolationKm = 541;
            double interpolationInterval = 50;
            double minimumJourneyDistance = 250*1000;
            double distanceThreshold = 4*1000;
            double timethreshold = 10*60;

            double trainLength = 1.5;
            double trackSpeedFactor = 0.9;
            double maxDistanceToTrackSpeed = 7;
            double timeThreshold = 10;
            double stoppingSpeedThreshold = 5;
            double restartSpeedThreshold = 10;

            /************************************ Form parameters ********************************************************/

            /* Ensure there is a empty list of trains to exclude to start. */
            List<string> excludeTrainList = new List<string> { };

            /* Populate the exluded train list. */
            if (trainListFile != null)
                excludeTrainList = FileOperations.readTrainList(trainListFile);

            /* Read in the track geometry data. */
            List<TrackGeometry> trackGeometry = new List<TrackGeometry>();
            trackGeometry = FileOperations.readGeometryfile(geometryFile);
            
            /* Read the data. */
            List<TrainRecord> TrainRecords = new List<TrainRecord>();
            TrainRecords = FileOperations.readICEData(dataFile, excludeTrainList, excludeListOfTrains, dateRange);

            if (TrainRecords.Count() == 0)
            {
                //tool.messageBox("There are no records in the list to analyse.", "No trains available.");
                return;
            }
            
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
                simulatedTrains.Add(FileOperations.readSimulationData(simulationFiles[index * 2], simCategories[index], direction.IncreasingKm));
                simulatedTrains.Add(FileOperations.readSimulationData(simulationFiles[index * 2 + 1], simCategories[index], direction.DecreasingKm));
            }

            Console.WriteLine("Interpolating Simulation data.");

            /* Interpolate the simulations to the same granularity as the ICE data will be. */
            List<Train> interpolatedSimulations = new List<Train>();
            interpolatedSimulations = Processing.interpolateTrainData(simulatedTrains, trackGeometry, startInterpolationKm, endInterpolationKm, interpolationInterval);

            /* Sort the data by [trainID, locoID, Date & Time, kmPost]. */
            List<TrainRecord> OrderdTrainRecords = new List<TrainRecord>();
            OrderdTrainRecords = TrainRecords.OrderBy(t => t.trainID).ThenBy(t => t.locoID).ThenBy(t => t.dateTime).ThenBy(t => t.kmPost).ToList();

            Console.WriteLine("Making trains. [" + OrderdTrainRecords.Count() + "]");

            List<Train> trainList = new List<Train>();
            trainList = Processing.CleanData(OrderdTrainRecords, trackGeometry, timethreshold, distanceThreshold, minimumJourneyDistance, analysisCategory);
            //trainList = Processing.MakeTrains(OrderdTrainRecords, trackGeometry, timethreshold, distanceThreshold, minimumJourneyDistance, analysisCategory);

            Console.WriteLine("Interpolating train data. [" + trainList.Count() + "]");

            /* Interpolate data */
            /******** Should only be required while we are waiting for the data in the prefered format ********/
            List<Train> interpolatedTrains = new List<Train>();
            interpolatedTrains = Processing.interpolateTrainData(trainList, trackGeometry, startInterpolationKm, endInterpolationKm, interpolationInterval);

            /**************************************************************************************************/

            Console.WriteLine("Writing data to file.");

            /* Write the interpolated data to file. */
            FileOperations.writeTrainDataWithTime(interpolatedTrains, startInterpolationKm, interpolationInterval, aggregatedDestination);

            /* Identify all the potential loop locations where a train may be forced to stop. */
            List<LoopLocation> loopLocations = findAllLoopLocations(interpolatedTrains[0], endInterpolationKm);
            
            /* Identify all the trains that stop at a loop. */
            List<TrainPair> trainPairs = findAllStoppingTrains(interpolatedTrains, loopLocations, stoppingSpeedThreshold, restartSpeedThreshold); // new List<TrainPair>();
            
            /* Identify the corresponding through trains for the trains that have stopped.  */
            populateThroughTrains(trainPairs, loopLocations, interpolatedTrains, timeThreshold);

            /* Ignore the train pairs where we do not have an identified through train. */
            trainPairs = trainPairs.Where(p => p.throughTrain != null).ToList();
            /* Order the remaining pairs by loop location, stopping train direction and ID. */
            trainPairs = trainPairs.OrderBy(t => t.loopLocation.loopStart).ThenBy(t => t.stoppedTrain.trainDirection).ThenBy(t => t.stoppedTrain.trainID).ToList();
            /* Calculate the transaction time and the times contributing to it and populate the train pair parameters. */
            trainPairs = calculateTransactionTime(trainPairs, interpolatedSimulations, maxDistanceToTrackSpeed, trackSpeedFactor, interpolationInterval, trainLength);
                        
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
            stats[stats.Count()-1].Category = "All loops";

            /* Write the train pairs to file grouped by loop location. */
            FileOperations.writeTrainPairs(trainPairs, loopLocations, aggregatedDestination);
            FileOperations.wrtieTrainPairStatistics(stats, aggregatedDestination);
           

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

                if (!startLoopFound && journey[index].isLoopHere)
                {
                    /* If teh start of the loop is found, set the start parameter. */
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

            double minSpeed = double.MinValue;
            List<TrainPair> trainPairs = new List<TrainPair>();

            /* loop through the trains to find the trains that stop in loops. */
            foreach (Train train in trains)
            {
                /* loop through each loop location. */
                for (int loopIdx = 0; loopIdx < loopLocations.Count(); loopIdx++)
                {
                    /* Find the minimum speed within the loop. */
                    minSpeed = train.journey.Where(t => t.kilometreage >= loopLocations[loopIdx].loopStart).
                                                    Where(t => t.kilometreage <= loopLocations[loopIdx].loopEnd).Min(t => t.speed);

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
                        trainPairs.Add(new TrainPair(train, null, loopLocations[loopIdx], train.journey[stopIdx].kilometreage, train.journey[restartIdx].kilometreage));

                    }



                }



            }
            return trainPairs;
        
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
            /* Cycle through the train pairs. */
            foreach (TrainPair pair in trainPairs)
            {
                /* Search through each loop location. */
                foreach (LoopLocation loop in loopLocations)
                {
                    /* If the current train pair stopped at the current loop, search for the through train. */
                    if (pair.loopLocation == loop)
                    {
                        /* Search through each train for the corresponding through train. */
                        foreach (Train throughTrain in trains)
                        {
                            /* Find the location where the stopped train restarts. */
                            int idx = throughTrain.journey.FindIndex(t => t.kilometreage == pair.restartLocation);

                            /* Ensure the train is going in the opposite direction to the stopped train. */
                            if (throughTrain.trainDirection != pair.stoppedTrain.trainDirection) // may need to check not "invalid, or unknown"
                            {
                                /* calcualte teh time differance of each train at the restart location. */
                                double timeDifference = (pair.stoppedTrain.journey[idx].dateTime - throughTrain.journey[idx].dateTime).TotalMinutes;

                                /* Im concerned that this may capture additonal trains, but its unlikely that there will be 
                                 * multiple trains in opposing direction so close to each other.
                                 */
                                /* If the time difference is less than the threshold, then the train is considered to be the through train. */
                                if (timeDifference > 0 && timeDifference < timeThreshold)
                                    pair.throughTrain = throughTrain;

                            } 

                        }
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

                /* Identify the pairs that should be kept based on the distance required to achieve 
                 * track speed and positive time components. 
                 */
                if (timeToReachTrackSpeed > 0 && simulatedTrainTime > 0 && timeToClearLoop > 0 && distanceToTrackSpeed < maxDistanceToTrackSpeed)
                    keepIdx.Add(trainPairs.IndexOf(pair));

                /* Calculate the transaction time. */
                transactionTime = timeToReachTrackSpeed - simulatedTrainTime + timeToClearLoop;

                /* Populate the transaction time and its componenets for each train pair. */
                pair.timeForStoppedTrainToReachTrackSpeed = timeToReachTrackSpeed;
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

            /* Define the changinf parameters based on the direction of the stopping train. */
            if (pair.stoppedTrain.trainDirection == direction.IncreasingKm)
            {
                increment = 1;
                loopSide = pair.loopLocation.loopEnd;
            }
            else
            {
                loopSide = pair.loopLocation.loopStart;
                increment = -1;
            }
            
            /* Identify the index where the stopped train stops. */
            int trackSpeedLocationIdx = pair.stoppedTrain.journey.FindIndex(t => t.kilometreage == loopSide) + increment;
            
            /* find the location where the train reaches 90% of simulated train speed. */    
            while (distanceToTrackSpeed < maxDistanceToTrackSpeed && pair.stoppedTrain.journey[trackSpeedLocationIdx].speed < trackSpeedFactor * simulation.journey[trackSpeedLocationIdx].speed)
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
            restartTime = pair.stoppedTrain.journey[pair.stoppedTrain.journey.FindIndex(t => t.kilometreage == pair.restartLocation)].dateTime;
            /* Time when the through train clears the loop. */
            clearanceTime = pair.throughTrain.journey[pair.throughTrain.journey.FindIndex(t => t.kilometreage >= loopSide - increment * trainLength)].dateTime;
            /* Calculate the time between the through train clearing the loop and the stopped train to restart. */
            timeToClearLoop = (restartTime - clearanceTime).TotalMinutes;

            /* Return the time compenents as a list. */
            return new List<double>(new double[] { timeToReachTrackSpeed, simulatedTrainTime, timeToClearLoop, distanceToTrackSpeed });
        }

        
    }
}
