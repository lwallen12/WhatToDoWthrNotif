﻿using Amazon;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace WhatToDoWthrNotif
{
    public class MailBuilder
    {
        List<WeatherEvaluator> _weatherEvaluators;

        public MailBuilder(List<WeatherEvaluator> weatherEvaluators)
        {
            this._weatherEvaluators = weatherEvaluators;
        }

        // Replace sender@example.com with your "From" address.
        // This address must be verified with Amazon SES.
        static readonly string senderAddress = "a.allenwill@gmail.com";

        // Replace recipient@example.com with a "To" address. If your account
        // is still in the sandbox, this address must be verified.
        //static readonly string receiverAddress = "william.allen1296@gmail.com; a.allenwill@gmail.com;";

        List<string> receiverAddresses = new List<string> { "william.allen1296@gmail.com", "a.allenwill@gmail.com" };

        // The subject line for the email.
        static readonly string subject = "Fishing Conditions Program";

        // The email body for recipients with non-HTML email clients.
        static readonly string textBody = "Amazon SES Test (.NET)\r\n"
                                        + "This email was sent through Amazon SES "
                                        + "using the AWS SDK for .NET.";

        public string buildBody()
        {

            string strBody = "";
            foreach (WeatherEvaluator weatherEvaluator in _weatherEvaluators)
            {
                string conHeading = Convert.ToString(weatherEvaluator._weatherFull.ConditionDateTime) + "---" + Convert.ToString(weatherEvaluator.TotalPercent) + "------" + weatherEvaluator._weatherFull.LocationName;
                //
                string pressureAction = weatherEvaluator.PressurePreviousAction;
                string tempAction = weatherEvaluator.TempAction;
                string weatherDescAction = weatherEvaluator.WeatherDescripAction;
                string cloudCountAction = weatherEvaluator.CloudCountAction;
                string windSpeedAction = weatherEvaluator.WindSpeedAction;
                string windCurrentDirection = weatherEvaluator.WindCurrentDirectionAction;
                string windPastAction = weatherEvaluator.WindPastAction;
                string rainFallCurrentAction = weatherEvaluator.RainfallCurrentAction;
                string rainFallPastAction = weatherEvaluator.RainfallPastAction;

                strBody = strBody +  @"
                        <hr>
                          <h1> " + conHeading + @"</h1>
                            <div>
                               <p><strong>Barometric Pressure</strong>: " + pressureAction + @"</p>
                               <p><strong>Air Temperature</strong>: " + tempAction + @"</p>
                               <p><strong>Overall Forecast</strong>: " + weatherDescAction + @"</p>
                               <p><strong>Cloud Cover</strong>: " + cloudCountAction + @"</p>
                               <p><strong>Wind Speed</strong>: " + windSpeedAction + @"</p>                               
                               <p><strong>Wind Direction</strong>: " + windCurrentDirection + @"</p>
                               <p><strong>Prior Wind</strong>: " + windPastAction + @"</p>
                               <p><strong>Current Rain</strong>: " + rainFallCurrentAction + @"</p>
                               <p><strong>Past Rainfall</strong>: " + rainFallPastAction + @"</p>
                            </div>
                            ";
            }

            return strBody;
        }
        

        public void SendEmail()
        {
            if (_weatherEvaluators.Count == 0)
            {
                return;
            }

            string mainEmailMessage = this.buildBody();
            string htmlBody = @"<html>
                    <head></head>
                    <body>
                      " + mainEmailMessage + @"
                    </body>
                    </html>";


            using (var client = new AmazonSimpleEmailServiceClient(RegionEndpoint.USEast1))
            {
                var sendRequest = new SendEmailRequest
                {
                    Source = senderAddress,
                    Destination = new Destination
                    {
                        ToAddresses = receiverAddresses
                    },
                    Message = new Message
                    {
                        Subject = new Content(subject),
                        Body = new Body
                        {
                            Html = new Content
                            {
                                Charset = "UTF-8",
                                Data = htmlBody
                            },
                            Text = new Content
                            {
                                Charset = "UTF-8",
                                Data = textBody
                            }
                        }
                    },
                    // If you are not using a configuration set, comment
                    // or remove the following line 
                    //ConfigurationSetName = configSet
                };
                try
                {
                    Console.WriteLine("Sending email using Amazon SES...");
                    var response = client.SendEmailAsync(sendRequest);
                    Console.WriteLine("The email was sent successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("The email was not sent.");
                    Console.WriteLine("Error message: " + ex.Message);

                }
            }

        }
    }
}