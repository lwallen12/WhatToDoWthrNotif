using System;
using System.Collections.Generic;
using System.Text;

namespace WhatToDoWthrNotif
{
    public class WeatherEvaluator
    {
        WeatherFull _weatherFull;
        List<double> _previousPressures;
        List<double> _previousRains;
        List<double> _previousWindDirections;
        List<double> _previousWindSpeeds;

        private int _idealTemp = 84;

        
        private List<string> _buzzKillList = new List<string>();

        public bool BuzzKill { get; set; }

        //Max would be this number * 10
        private int pressureWeight = 7;
        private int tempWeight = 8;
        private int weatherDescWeight = 8;
        private int cloudCountWeight = 3;
        private int windSpeedWeight = 7;
        private int windDirectionWeight; //see in constructor
        private int windPastWeight; //see in constructor
        private int rainFallCurrentWeight = 6;
        private int rainFallPastWeight = 5;

        //640 coastal, 440 for freshwater
        public double TotalPossible { get; set; }
        public double TotalPercent { get; set; }


        public string PressurePreviousAction { get; set; }
        public string TempAction { get; set; }
        public string WeatherDescripAction { get; set; }
        public string CloudCountAction { get; set; }
        public string WindSpeedAction { get; set; }
        public string WindCurrentDirectionAction { get; set; }
        public string WindPastAction { get; set; }
        public string RainfallCurrentAction { get; set; }
        public string RainfallPastAction { get; set; }
        

        public WeatherEvaluator(WeatherFull weatherFull, List<double> previousPressures, List<double> previousRains, List<double> previousWindDirections, List<double> previousWindSpeeds)
        {
            this._weatherFull = weatherFull;
            this._previousPressures = previousPressures;
            this._previousRains = previousRains;
            this._previousWindDirections = previousWindDirections;
            this._previousWindSpeeds = previousWindSpeeds;

            if (_weatherFull.IsCoastal == true)
            {
                windDirectionWeight = 10;
                windPastWeight = 10;
            }
            else
            {
                windDirectionWeight = 1;
                windPastWeight = 6;
            }

            TotalPossible = (pressureWeight + tempWeight + weatherDescWeight + cloudCountWeight + windSpeedWeight + windDirectionWeight + windPastWeight + rainFallCurrentWeight + rainFallPastWeight) * 10;
        }

        //TODO: Consider using case statements if more than 5 if blocks in a single method... can improve performance slightly

        public double Evaluate()
        {
            if (BuzzKill == true)
            {
                return 0.0;
            }

            int tempScore = scoreTemperature() * tempWeight;
            int pressureScore = scorePressure() * pressureWeight;
            int weatherDescripScore = scoreWeatherDescription() * weatherDescWeight;
            int cloudCountScore = scoreCloudcount() * cloudCountWeight;
            int windSpeedScore = scoreWindSpeed() * windSpeedWeight;
            int windCurrentDirectionScore = scoreCurrentWindDirection() * windDirectionWeight;
            int rainCurrentScore = scoreCurrentRainfall() * rainFallCurrentWeight;
            int rainPastScore = scorePastRainfall() * rainFallPastWeight;
            int windPastScore = scorePastWind() * windPastWeight;

            double totalPoints = tempScore + pressureScore + weatherDescripScore + cloudCountScore + windSpeedScore + windCurrentDirectionScore + rainCurrentScore + rainPastScore + windPastScore;

            //Console.WriteLine(Convert.ToString(TotalPossible) + "\n");
            return TotalPercent = totalPoints / TotalPossible;
        }

        //TODO: If any ideas... otherwise leaving blank... public void scoreOverallSituation()


        public int scorePastWind()
        {
            if (_previousWindSpeeds.Count == 0)
            {
               WindPastAction = "Time to close to score past wind.";
                return 5;
            }

            double pastOneWindSpeed = _previousWindSpeeds[0];
            double pastTwoWindSpeed = _previousWindSpeeds[1];
            double pastThreeWindSpeed = _previousWindSpeeds[2];
            double pastFourWindSpeed = _previousWindSpeeds[3];

            double avgWindSpeed = (pastOneWindSpeed + pastTwoWindSpeed + pastThreeWindSpeed + pastFourWindSpeed) / 4;

            if (_weatherFull.IsCoastal == true)
            {
                if (avgWindSpeed < 10)
                {
                    WindPastAction = $"Past Wind Speed Average: {avgWindSpeed}. Past wind speed would not indicate turbulent water, water clarity is likely okay.";
                    return 10;
                }
                else
                {
                    WindPastAction = $"Past Wind Speed Average: {avgWindSpeed}. Past wind speed may indicate turbulent water, water clarity may be murky.";
                    return 5;
                }
            }

            int goodWindCount = 0;
            int badWindCount = 0;
            int mediumWindCount = 0;

            foreach (double windDirection in _previousWindDirections)
            {
                if (windDirection <= 180 && windDirection >= 30)
                {
                    goodWindCount++;
                }
                else if (windDirection >= 180 && windDirection <= 270)
                {
                    badWindCount++;
                }
                else
                {
                    mediumWindCount++;
                }
            }

            //If this is a morning reading wind lays down at night?
            if (goodWindCount > 2 && avgWindSpeed <= 7)
            {
                WindPastAction = $"Previous wind readings are very good... Get ready for good fishin'";
                return 10;
            }
            else if (goodWindCount > 2 || avgWindSpeed <= 20)
            {
                WindPastAction = $"Previous wind readings are pretty good... Hopefully good fishin'";
                return 8;
            }
            else if (goodWindCount > 1 || avgWindSpeed <= 12)
            {
                WindPastAction = $"Previous wind readings are not bad... hopefully good fishin''";
                return 6;
            }
            else if (goodWindCount == 0 && avgWindSpeed < 10)
            {
                WindPastAction = $"Fishing is probably just alright. Bad previous wind direction, but low windspeed.";
                return 5;
            }
            else if (goodWindCount == 0 && avgWindSpeed < 15)
            {
                WindPastAction = $"Bad wind direction and high speeds.";
                return 2;
            }
            else
            {
                WindPastAction = $"Previous wind readings are pretty bad.";
                return 1;
            }



        }

        public int scorePastRainfall()
        {
            if (_previousRains.Count == 0)
            {
                RainfallPastAction = $"Time to close to gather previous rainfalls.";
                return 5;
            }

            double pastOneRainfall = _previousRains[0];
            double pastTwoRainfall = _previousRains[0];
            double pastThreeRainfall = _previousRains[0];
            double pastFourRainfall = _previousRains[0];

            double sumRainfall = pastOneRainfall + pastTwoRainfall + pastThreeRainfall + pastFourRainfall;

            if (sumRainfall < 5)
            {
                RainfallPastAction = $"Prior rainfall: {sumRainfall} Minimal. Depending on location, water clarity is likely clear";
                return 10;
            }
            else if (sumRainfall < 15)
            {
                RainfallPastAction = $"Prior rainfall: {sumRainfall} Moderate. Depending on location, water may or may not be off-color";
                return 7;
            }
            else
            {
                RainfallPastAction = $"Prior rainfall: {sumRainfall} Heavy. If location is affected by heavy rainfall, water may be very mirky. Monitor storms in tributary watersheds.";
                return 7;
            }
        }

        public int scoreCurrentRainfall()
        {
            double rainfall = _weatherFull.RainFall;

            if (rainfall < 1)
            {
                RainfallCurrentAction = $"Rainfall: {rainfall}. Little to no rainfall. Should be good to go fishing!";
                return 10;
            }

            else if (rainfall < 4)
            {
                RainfallCurrentAction = $"Rainfall: {rainfall}. Projected to be some rain, may not be good.";
                return 3;
            }

            else
            {
                RainfallCurrentAction = $"Rainfall: {rainfall}. Lots of rain. Probably stay in and watch paint dry.";
                return 1;
            }
        }

        //Maybe make a list of the last 4 wind directions?
        public int scoreCurrentWindDirection()
        {
            int windDirection = Convert.ToInt32(_weatherFull.WindDirection);

            if (_weatherFull.IsCoastal == false)
            {
                WindCurrentDirectionAction = $"Wind Direction: {windDirection}. This shouldn't actually matter. Really based on the lake itself at this point";
                return 1;
            }
            else
            {
                if (windDirection <= 180 && windDirection >= 30)
                {
                    WindCurrentDirectionAction = $"Wind Direction: Southeast/East. ({windDirection}) The gulf will be blowing in some perfect water from the gulf. Go fishing! Show them what Texas is made of!";
                    return 10;
                }
                else if (windDirection >= 180 && windDirection <= 270)
                {
                    WindCurrentDirectionAction = $"Wind Direction: SouthWest. (" + windDirection + @") Water clarity is likely murky. If the winds are strong, you need good knowledge of subsurface reefs, guts, etc. Do not chase whitecaps. Look for
                                                windward side of shores, reefs and guts where baitfish and shrimp are forced against. One upside to murky water is that predators are more aggressive. They strike
                                                first and look later. Consider more loud and obnoxious baits and lures.";
                    return 3;
                }
                else
                {
                    WindCurrentDirectionAction = $"Wind Direction: SouthWest. (" + windDirection + @") Water clarity is likely murky. If the winds are strong, you need good knowledge of subsurface reefs, guts, etc. Do not chase whitecaps. Look for
                                                windward side of shores, reefs and guts where baitfish and shrimp are forced against. One upside to murky water is that predators are more aggressive. They strike
                                                first and look later. Consider more loud and obnoxious baits and lures.";
                    return 5;
                }

            }
        }

        public int scoreWindSpeed()
        {
            double windSpeed = _weatherFull.WindSpeed;

            if (_weatherFull.IsCoastal == true)
            {
                if (windSpeed < 5)
                {
                    WindSpeedAction = $"Wind Speed: {windSpeed}.  Wind should be very light. Go fishing!";
                    return 10;
                }
                else if (windSpeed < 10)
                {
                    WindSpeedAction = $"Wind Speed: {windSpeed}. Wind may be fairly strong, but still doable.";
                    return 6;
                }
                else if (windSpeed < 15)
                {
                    WindSpeedAction = $"Wind Speed: {windSpeed}. Wind will be strong, possibly not worth it in kayaks.";
                    return 3;
                }
                else 
                {
                    WindSpeedAction = $"Wind Speed: {windSpeed}. Wind will be very, very strong. Probably not worth it.";
                    BuzzKill = true;
                    _buzzKillList.Add(WindSpeedAction);
                    return 1;
                }
            }
            else
            {
                if (windSpeed < 3)
                {
                    WindSpeedAction = $"Wind Speed: {windSpeed}. Wind should be very light. Go fishing!";
                    return 10;
                }
                if (windSpeed < 5)
                {
                    WindSpeedAction = $"Wind Speed: {windSpeed}. Wind should be pretty good. Go fishing!";
                    return 8;
                }
                else if (windSpeed < 10)
                {
                    WindSpeedAction = $"Wind Speed: {windSpeed}. Wind may be fairly strong, but still doable.";
                    return 7;
                }
                else if (windSpeed < 15)
                {
                    WindSpeedAction = $"Wind Speed: {windSpeed}. Wind will be strong, possibly not worth it in kayaks.";
                    return 5;
                }
                else if ((windSpeed < 20))
                {
                    WindSpeedAction = $"Wind Speed: {windSpeed}. Wind will be very, very strong.";
                    return 3;
                }
                else
                {
                    WindSpeedAction = $"Wind Speed: {windSpeed}. Wind will be too strong to fish most likely.";
                    BuzzKill = true;
                    _buzzKillList.Add(WindSpeedAction);
                    return 1;
                }
            }
            
        }

        public int scoreCloudcount()
        {
            int cloudCount = _weatherFull.CloudCount;

            if (cloudCount > 70)
            {
                CloudCountAction = $"Cloud Count: {cloudCount}. Expect heavy cloud cover.";
                return 10;
            }

            if (cloudCount > 25)
            {
                CloudCountAction = $"Cloud Count: {cloudCount}. Expect some cloud cover.";
                return 7;
            }

            else
            {
                CloudCountAction = $"Cloud Count: {cloudCount}. Expect mostly clear skies.";
                return 3;
            }
        }

        public int scoreWeatherDescription()
        {
            string weatherDesc = _weatherFull.WeatherDescription;

            if (weatherDesc == "overcast clouds")
            {
                WeatherDescripAction = weatherDesc;
                return 10;
            }
            else if (weatherDesc == "light rain")
            {
                WeatherDescripAction = weatherDesc;
                return 10;
            }
            else if (weatherDesc == "broken clouds")
            {
                WeatherDescripAction = weatherDesc;
                return 9;
            }
            else if (weatherDesc == "scattered clouds")
            {
                WeatherDescripAction = weatherDesc;
                return 9;
            }
            else if (weatherDesc == "moderate rain")
            {
                WeatherDescripAction = weatherDesc;
                return 5;
            }
            else if (weatherDesc == "clear sky")
            {
                WeatherDescripAction = weatherDesc;
                return 8;
            }
            else if (weatherDesc == "few clouds")
            {
                WeatherDescripAction = weatherDesc;
                return 8;
            }
            else if (weatherDesc == "heavy intensity rain")
            {
                BuzzKill = true;
                _buzzKillList.Add(weatherDesc);
                WeatherDescripAction = weatherDesc;
                return 1;
            }
            else
            {
                WeatherDescripAction = "New VALUE: " + weatherDesc;
                return 5;
            }
        }

        public int scorePressure()
        {
            double pressure = _weatherFull.Pressure;

            if (_previousPressures.Count == 0)
            {
                PressurePreviousAction = "Cannot gather previous pressures because the time is too close.";
                return 5;
            }

            double onePressureAgo = _previousPressures[0];
            double twoPressureAgo = _previousPressures[1];

            if  ((twoPressureAgo - pressure) > 3 && (onePressureAgo - pressure) > 1)
            {
                PressurePreviousAction = "Pressure is falling, fish are likely very active and feeding. Time to go!";
                return 10;
            }
            else if ((twoPressureAgo - pressure) < 3 && (onePressureAgo - pressure) < 1)
            {
                PressurePreviousAction = "Pressure is rising, this will likely make fish more sluggish";
                return 5;
            }
            else
            {
                if (pressure < 1008)
                {
                    PressurePreviousAction = "Pressure is low and stable. Pressure would indicate slower fishing.";
                    return 5;
                }
                else if (pressure < 1018)
                {
                    PressurePreviousAction = "Pressure is normal and stable. Pressure would indicate pretty good fishing";
                    return 8;
                }
                else
                {
                    PressurePreviousAction = "Pressure is high and stable. This is not bad, but it also is not great. Don't count on pressure for much of an indicator.";
                    return 6;
                }
            }
        }

        public int scoreTemperature()
        {
            double temperature = _weatherFull.Temperature;

            if (this._weatherFull.IsCoastal == false)
            {
                double absDifference = Math.Abs(temperature - _idealTemp);

                if (absDifference < 10)
                {
                    TempAction = $"Temp: {temperature}. Temperature probably feels great and should be great for fishing";
                    return 10;
                }
                else if (absDifference < 15)
                {
                    TempAction = $"Temp: {temperature}. Temperature probably feels good and should be good for fishing";
                    return 7;
                }
                else if (absDifference < 20)
                {
                    TempAction = $"Temp: {temperature}. Temperature probably feels okay and should not be too bad for fishing";
                    return 5;
                }
                else
                {
                    TempAction = $"Temp: {temperature}. Temperature is either fairly chilly or pretty hot and could affect fishing negatively";
                    return 2;
                }
            }
            else
            {
                if (temperature > _idealTemp)
                {
                    TempAction = $"Temp: {temperature}. If it is not too hot, fishing is probably pretty good!";
                    return 10;
                }

                else
                {
                    TempAction = $"Temp: {temperature}. Might be a little too chilly for Speckled Trout";
                    return 5;
                }
            }
        }
        


    }

    
}
