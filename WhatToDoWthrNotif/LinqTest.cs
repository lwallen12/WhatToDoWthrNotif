using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;

namespace WhatToDoWthrNotif
{
    public class LinqTest
    {
        //List<UserSelection> _appUsers = new List<UserSelection>
        //{
        //    new UserSelection {Email = "a.allenwill@gmail.com", LocationIds = new List<int> { 4682991, 4683416 } },
        //    new UserSelection {Email = "william.allen1296@gmail.com", LocationIds = new List<int> { 4682991, 4683416, 4692856, 5117949 } }
        //};

        List<UserSelection> _appUsers = new List<UserSelection>();



        public void printSomeStuff()
        {
            //var receiverAddresses = _appUsers.Select(u => u.Email).ToList();

            var theseUsers = _appUsers.Select(u => new { u.Email, u.LocationIds })
                .Where(u => u.Email == "william.allen1296@gmail.com")
                .ToList();

            List<UserSelection> thisEmail = new List<UserSelection>(_appUsers.FindAll(u => u.Email == "william.allen1296@gmail.com"));
            UserSelection thisUser = _appUsers.Find(u => u.Email == "a.allenwill@gmail.com");

            foreach (int id in thisUser.LocationIds)
            {
                Console.WriteLine(Convert.ToString(id) + "\n");
            }

            int hi = 434;
            if (thisEmail[0].LocationIds.Contains(hi))
            {
                Console.WriteLine("here it is");
            }
            else
            {
                Console.WriteLine("It was not in there");
            }


        }


        public void BuildUserSelection()
        {


            using (var connection = new MySqlConnection("Server=test1.ce8cn9mhhgds.us-east-1.rds.amazonaws.com;Database=whattodo;Uid=Wallen;Pwd=MyRDSdb1;Allow User Variables=True;"))
            {
                var users = connection.Query<string>("SELECT DISTINCT UserId FROM UserSelection;").ToList();

                foreach (string usr in users)
                {
                    //Console.WriteLine(usr);
                    var locs = connection.Query<int>($"SELECT DISTINCT LocationId FROM UserSelection WHERE UserId = '{usr}';").ToList();

                    UserSelection userSelection = new UserSelection(usr, locs);
                    _appUsers.Add(userSelection);

                }

            }

            Console.WriteLine(_appUsers.Count());
        }


    }




    public class UserSelection
    {
        public UserSelection()
        {

        }

        public UserSelection(string email, List<int> locationIds)
        {
            Email = email;
            LocationIds = locationIds;
        }

        public string Email { get; set; }
        public List<int> LocationIds { get; set; }

    }
}
