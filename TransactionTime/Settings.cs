using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TrainLibrary;

namespace TransactionTime
{
    class Settings
    {

        /* Set up default parameters */
        public static bool excludeListOfTrains = false;
        public static string dataFile = null;
        public static string geometryFile = null;
        
        /* Default number of simulation Categories is 3, hence, the default number of simulation files is 6, one for each direction */
        public static List<string> simulationFiles = new List<string>(new string[6]);
        public static string aggregatedDestination = null;
        
        public static analysisCategory analysisCategory = analysisCategory.TrainOperator;
        
        public static DateTime[] dateRange = new DateTime[2];

        public static double startInterpolationKm;                  /* Start km for interpoaltion data. */
        public static double endInterpolationKm;                    /* End km for interpolation data. */
        public static double interpolationInterval;                 /* Interpolation interval (metres). */
        public static double minimumJourneyDistance;                /* Minimum distance of a train journey to be considered valid. */
        public static double distanceThreshold;                     /* Minimum distance between successive data points. */
        public static double throughTrainTime;                      /* The minimum time between the through train and the train restarting. */
        public static double timethreshold;                         /* Minimum time between data points to be considered a seperate train. */
        
        public static double trainLength;                           /* The assumed train length. */
        public static double trackSpeedFactor;                      /* The percentage of the simualtion to be considered to have reached track speed. */
        public static double maxDistanceToTrackSpeed;               /* The maximum permissible distance to achieve track speed. */
        public static double stoppingSpeedThreshold;                /* The speed at which a train is considered to have stopped. */
        public static double restartSpeedThreshold;                 /* The speed at which a train is considered to have restarted. */
        public static double transactionTimeOutlierThreshold;       /* The maximum permissible transaction time to include in analysis. */

    }
}
