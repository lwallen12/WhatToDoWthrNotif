using System;
using System.Collections.Generic;
using System.Text;

namespace WhatToDoWthrNotif
{
    public class Evaluator
    {

        private List<WeatherCondition> _weatherConditions;
        private List<WeatherCondition> _futurePressures;

        private int _tempWeight = 7;
        private int _pressureWeight = 5;
        private int _humidityWeight = 2;
        private int _weatherDescWeight = 10;
        private int _cloudWeight = 4;
        private int _windSpeed = 8;
        private int _windDirection = 9;
        private int _rain = 6;
    
        private int _tempIdeal = 75;

        public Evaluator(List<WeatherCondition> weatherConditions, List<WeatherCondition> futurePressures)
        {
            _weatherConditions = weatherConditions;
            _futurePressures = futurePressures;
        }

        /// <summary>
        /// This method should tie together all sub-methods, and kick off email if the points are enough
        /// </summary>
        public void evaluateScenario()
        {
            scoreTemperature();
            scorePressure();
        }

        private void scoreTemperature()
        {
            var tempCondition = this._weatherConditions.Find(t => t.Name == "Temperature");
            Console.WriteLine(tempCondition.Name + "-------" + tempCondition.CurrentStatus);


        }

        private void scorePressure()
        {
            if (_futurePressures.Count > 0)
            {
                var x = _futurePressures[0].Name + "--" + _futurePressures[0].TimeFrame;
                var y = _futurePressures[1].Name + "--" + _futurePressures[1].TimeFrame;
                var z = _futurePressures[2].Name + "--" + _futurePressures[2].TimeFrame;

                Console.WriteLine(x);
                Console.WriteLine(y);
                Console.WriteLine(z);
            }
            else
            {
                Console.WriteLine("Too far out for pressure prediction");
            }
        }
    }

    public class WeatherCondition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string EvaluationType { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string State { get; set; }
        public string Location { get; set; }
        public DateTime? ConditionDateTime { get; set; }
        public string TimeFrame { get; set; }
        public string CurrentStatus { get; set; }
        public DateTime? LastUpdate { get; set; }
        public int? LocationId { get; set; }
        public int? TimeGroupId { get; set; }
    }
}
