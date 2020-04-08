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
                var conditions = connection.Query<WeatherCondition>("SELECT * FROM WeatherCondition").ToList();

                Console.WriteLine("Records in Selection: " + conditions.Count());


                //int pressCounter = 24;
                for (int i = 0; i < conditions.Count; i += 8)
                {
                    List<WeatherCondition> currentSeriesConds = new List<WeatherCondition>();
                    List<WeatherCondition> previousPressures = new List<WeatherCondition>();

                    int j = i;
                    while (j < (i + 8))
                    {
                        currentSeriesConds.Add(conditions[j]);
                        j++;
                    }

                    //I can change this data to numeric and drop the h... that is if I don't entirely restructure
                    if (conditions[i].TimeFrame == "6h" || conditions[i].TimeFrame == "9h" || conditions[i].TimeFrame == "12h" || conditions[i].TimeFrame =="15h")
                    {

                        foreach (WeatherCondition w in currentSeriesConds)
                        {
                            Console.WriteLine(w.Name + "---" + w.TimeFrame);
                        }

                    }
                    else
                    {
                        //Console.WriteLine("LETS ADD THIS" + conditions[i - 7].Name + "---" + conditions[i - 7].TimeFrame);
                        previousPressures.Add(conditions[i - 7]);
                        previousPressures.Add(conditions[i - 15]);
                        previousPressures.Add(conditions[i - 23]);
                        previousPressures.Add(conditions[i - 31]);

                    }

                    Evaluator evaluator = new Evaluator(currentSeriesConds, previousPressures);
                    evaluator.evaluateScenario();

                    //pressCounter = pressCounter - 8;
                }
            }
        }
    }
}
