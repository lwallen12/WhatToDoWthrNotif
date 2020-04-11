using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Dapper;
using MySql.Data.MySqlClient;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace WhatToDoWthrNotif
{
    public class Function
    {
        

        public void FunctionHandler(ILambdaContext context)
        {
            using (var connection = new MySqlConnection("Server=test1.ce8cn9mhhgds.us-east-1.rds.amazonaws.com;Database=whattodo;Uid=Wallen;Pwd=MyRDSdb1;Allow User Variables=True;"))
            {
                var conditions = connection.Query<WeatherFull>("SELECT * FROM Weather INNER JOIN Location ON Weather.LocationId = Location.Id ORDER BY LocationId, TimeFrameHour ASC;").ToList();

                for (int i = 0; i < conditions.Count; i++)
                {

                    //bool timeOfCondition = isDay(conditions[i].ConditionDateTime);

                    //if (!timeOfCondition)
                    //{
                    //    Console.WriteLine(Convert.ToString(conditions[i].ConditionDateTime) + ": It is night");
                    //    continue;
                    //}

                    List<double> previousPressures = new List<double>();
                    List<double> previousRains = new List<double>();
                    List<double> previousWindDirections = new List<double>();
                    List<double> previousWindSpeeds = new List<double>();
                    if (conditions[i].TimeFrameHour > 15)
                    {
                        previousPressures.Add(conditions[i - 1].Pressure);
                        previousPressures.Add(conditions[i - 2].Pressure);
                        previousPressures.Add(conditions[i - 3].Pressure);
                        previousPressures.Add(conditions[i - 4].Pressure);

                        previousRains.Add(conditions[i - 1].RainFall);
                        previousRains.Add(conditions[i - 2].RainFall);
                        previousRains.Add(conditions[i - 3].RainFall);
                        previousRains.Add(conditions[i - 4].RainFall);

                        previousWindDirections.Add(conditions[i - 1].WindDirection);
                        previousWindDirections.Add(conditions[i - 2].WindDirection);
                        previousWindDirections.Add(conditions[i - 3].WindDirection);
                        previousWindDirections.Add(conditions[i - 4].WindDirection);

                        previousWindSpeeds.Add(conditions[i - 1].WindSpeed);
                        previousWindSpeeds.Add(conditions[i - 2].WindSpeed);
                        previousWindSpeeds.Add(conditions[i - 3].WindSpeed);
                        previousWindSpeeds.Add(conditions[i - 4].WindSpeed);
                    }

                    WeatherEvaluator weatherEvaluator = new WeatherEvaluator(conditions[i], previousPressures, previousRains, previousWindDirections, previousWindSpeeds);
                    double percentCorrect = weatherEvaluator.Evaluate();

                    Console.WriteLine(Convert.ToString(percentCorrect) + "\n");       

                }
            }
        }

        public static bool isDay(DateTime conDT)
        {
            TimeSpan beginTime = new TimeSpan(5, 0, 0);
            TimeSpan endTime = new TimeSpan(20, 0, 0);

            TimeSpan conTimeOfDay = conDT.TimeOfDay;

            if (conTimeOfDay > beginTime && conTimeOfDay < endTime)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class WeatherFull
    {
        public int LocationId { get; set; }
        public int TimeFrameHour { get; set; }
        public DateTime ConditionDateTime { get; set; }
        public DateTime? LastUpdate { get; set; }
        public double Temperature { get; set; }
        public double FeelsLike { get; set; }
        public double Pressure { get; set; }
        public int Humidity { get; set; }
        public string MainDescription { get; set; }
        public string WeatherDescription { get; set; }
        public int CloudCount { get; set; }
        public double WindSpeed { get; set; }
        public double WindDirection { get; set; }
        public double RainFall { get; set; }
        public string Pod { get; set; }


        //Right here is technically a Location object, but difficult to do with Dapper since not a full ORM... will have to see what to do when tides come into play
        public int Id { get; set; }
        public string LocationName { get; set; }
        public bool IsCoastal { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string State { get; set; }

    }

    
}
