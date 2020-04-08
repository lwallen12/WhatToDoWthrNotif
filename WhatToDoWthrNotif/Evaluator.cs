using System;
using System.Collections.Generic;
using System.Text;

namespace WhatToDoWthrNotif
{
    public class Evaluator
    {

        private List<WeatherCondition> _weatherConditions;
        private List<WeatherCondition> _previousPressures;

        private int _tempWeight = 7;
        private int _pressureWeight = 8;
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
            _previousPressures = futurePressures;
        }

        /// <summary>
        /// This method should tie together all sub-methods, and kick off email if the points are enough
        /// </summary>
        public void evaluateScenario()
        {
            scoreTemperature();
            scorePressure();
        }

        private int scoreTemperature()
        {
            var tempCondition = this._weatherConditions.Find(t => t.Name == "Temperature");
            Console.WriteLine(tempCondition.Name + "-------" + tempCondition.CurrentStatus);

            var temperature = Convert.ToDecimal(tempCondition.CurrentStatus);
            var absTemperature = Math.Abs(temperature - _tempIdeal);

            if (tempCondition.Location != "Conroe")
            {
                if (temperature > 70)
                {
                    return 10;
                }

                else
                {
                    return 5;
                }
            }

            if (absTemperature < 10)
            {
                return 10;
            }
            else if (absTemperature < 15)
            {
                return 9;
            }
            else if (absTemperature < 20)
            {
                return 6;
            }
            else if (absTemperature < 25)
            {
                return 4;
            }

            else return 1;

        }

        private void scorePressure()
        {
            if (_previousPressures.Count > 0)
            {
                var weatherDescription = this._weatherConditions.Find(t => t.Name == "Weather Description");
                var currentPressureObject = _weatherConditions.Find(p => p.Name == "Pressure");

                Console.WriteLine(weatherDescription.CurrentStatus);

                var now = currentPressureObject.Name + "--" + currentPressureObject.TimeFrame + "--" + currentPressureObject.CurrentStatus;
                var x = _previousPressures[0].Name + "--" + _previousPressures[0].TimeFrame + "--" + _previousPressures[0].CurrentStatus;
                var y = _previousPressures[1].Name + "--" + _previousPressures[1].TimeFrame + "--" + _previousPressures[1].CurrentStatus;
                var z = _previousPressures[2].Name + "--" + _previousPressures[2].TimeFrame + "--" + _previousPressures[2].CurrentStatus;
                var a = _previousPressures[3].Name + "--" + _previousPressures[3].TimeFrame + "--" + _previousPressures[3].CurrentStatus;

                Console.WriteLine("Now: " + now); //24h
                Console.WriteLine(x); //21h
                Console.WriteLine(y); //18h
                Console.WriteLine(z); //15h
                Console.WriteLine(a); //12h

                decimal highAirPressure = 1013.2M;
                decimal lowAirPressure = 1009.2M;

                decimal targetTimePressure = Convert.ToDecimal(currentPressureObject.CurrentStatus);
                decimal threePressure = Convert.ToDecimal(_previousPressures[0].CurrentStatus);
                decimal sixPressure = Convert.ToDecimal(_previousPressures[1].CurrentStatus);
                decimal ninePressure = Convert.ToDecimal(_previousPressures[2].CurrentStatus);
                decimal twelvePressure = Convert.ToDecimal(_previousPressures[3].CurrentStatus);
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
