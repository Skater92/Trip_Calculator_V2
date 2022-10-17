using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {

        /*
        * ALL OUTPUT IS APPROXIMATE AND BASED ON SIMSPEED
        * 
        * Input Example: "@TRIP_TIME SCR_0 @GPS$GPS: Skater #1:46.22:45.45:61.64:#FF75C9F1:;"
        * 
        * @TRIP_TIME - script call
        * SCR_# - Designates which screen to use on Cockpit (0-4 are available)
        * @GPS$ - GPS is to be pasted raw from clipboard here with NO SPACES (GPS: Skater #1:46.22:45.45:61.64:#FF75C9F1:)
        *  ;  MUST be present immediately after GPS
        */

        //SETTINGS

        //Enter Seat Name:

        const string ACTIVE_SEAT = "your_seat_name_here";



        //Set to True to set time scale to HOURS

        bool hourFlag = true;

        //Set to True to set Time scale to MINUTES

        bool minuteFlag = false;

        //If both false Time scale is set to DAYS



        //TOUCH NOTHING UNDER THIS

        //////////////-NO-TOUCH-NO-TOUCH-NO-TOUCH-NO-TOUCH-NO-TOUCH-\\\\\\\\\\\\\\
        //const string ACTIVE_COCKPIT;
        //float travelDistance = 0;
        double travelVelocity = 0;
        //float travelTime = 0;
        IMyTextSurfaceProvider TEST_COCKPIT;
        IMyTerminalBlock TEST_BLOCK;
        IMyShipController TEST_SEAT;
        string printVal = "NO DATA";

        public Program()
        {
            TEST_COCKPIT = GridTerminalSystem.GetBlockWithName(ACTIVE_SEAT) as IMyTextSurfaceProvider;
            TEST_BLOCK = GridTerminalSystem.GetBlockWithName(ACTIVE_SEAT);
            TEST_SEAT = GridTerminalSystem.GetBlockWithName(ACTIVE_SEAT) as IMyShipController;
            Runtime.UpdateFrequency = UpdateFrequency.Update10;
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.


        }

        public double get_velocity()
        {

            travelVelocity = TEST_SEAT.GetShipSpeed();

            Echo("Velocity m/s " + travelVelocity.ToString());

            return travelVelocity;
        }

        public Vector3D vectConvert(string input)
        {
            input = get_input();

            Vector3D dest = new Vector3D(0, 0, 0);

            int index = input.LastIndexOf("#");
            if (index >= 0)
            {
                input = input.Substring(0, index);
            }



            try
            {
                string[] args = input.Split(':');
                dest = new Vector3D(double.Parse(args[2]), double.Parse(args[3]), double.Parse(args[4]));
            }
            catch (Exception ex)
            {
                Echo(ex.Message);
            }

            return dest;
        }

        public double get_Distance(string input)
        {
            Vector3D dest = vectConvert(input);

            double retVal = 0.0;

            retVal = Vector3D.Distance(TEST_BLOCK.GetPosition(), dest);

            Echo("Distance m " + retVal.ToString());

            return retVal;
        }

        public string get_input()
        {
            string retString = "";
            string InString = TEST_BLOCK.CustomData;
            if (InString.Contains("@GPS$"))
            {
                int index = InString.LastIndexOf("$");
                int index2 = InString.IndexOf(";");

                if (index2 >= index)
                {
                    index2 -= index;
                }

                retString = InString.Substring(index, index2);
            }
            return retString;
        }

        public string ConvertFromDoubleToTime(double time)
        {
            if (hourFlag)
            {
                return TimeSpan.FromHours(time).ToString(@"hh\:mm\:ss");
            }
            else if (minuteFlag)
            {
                return TimeSpan.FromMinutes(time).ToString(@"mm\:ss\:FFF");
            }
            else
            {
                return TimeSpan.FromDays(time).ToString(@"dd\:hh\:mm");
            }

        }

        public void screenPrints()
        {
            //Take user input to choose print screen
            if (printVal == "Infinity")
            {
                printVal = "Not Enough Data";
            }


            if (TEST_BLOCK.CustomData.Contains("@TRIP_TIME"))
            {

                if (TEST_BLOCK.CustomData.Contains("SCR_0"))
                {
                    TEST_COCKPIT.GetSurface(0).WriteText("Time to Destination: " + printVal);
                }
                else if (TEST_BLOCK.CustomData.Contains("SCR_1"))
                {
                    TEST_COCKPIT.GetSurface(1).WriteText("Time to Destination: " + printVal);
                }
                else if (TEST_BLOCK.CustomData.Contains("SCR_2"))
                {
                    TEST_COCKPIT.GetSurface(2).WriteText("Time to Destination: " + printVal);
                }
                else if (TEST_BLOCK.CustomData.Contains("SCR_3"))
                {
                    TEST_COCKPIT.GetSurface(3).WriteText("Time to Destination: " + printVal);
                }
                else if (TEST_BLOCK.CustomData.Contains("SCR_4"))
                {
                    TEST_COCKPIT.GetSurface(4).WriteText("Time to Destination: " + printVal);
                }
                else
                {
                    TEST_COCKPIT.GetSurface(0).WriteText("NO INPUT");
                }
            }
        }

        public double CalculateFlightTime(double travelDistance, double travelVelocity)
        {
            double travelTime = 0;

            try
            {
                //t = d/v           

                //travelVelocity will be in meters per second 
                //travelDistance is in meters
                travelTime = travelDistance / travelVelocity;

                if (hourFlag)
                {
                    minuteFlag = false;
                    //convert travelTime to Hours
                    travelTime /= 3600;
                }
                else if (minuteFlag)
                {
                    //travelTime is in seconds, divide by 60 to convert to minutes
                    hourFlag = false;
                    travelTime /= 60;
                }
                else
                {
                    minuteFlag = false;
                    hourFlag = false;
                    travelTime /= 86400;
                }
                Echo("Travel Time: " + travelTime.ToString());


            }
            catch (Exception ex)
            {
                Echo(ex.Message);
            }

            return travelTime;
        }



        public void Main(string argument, UpdateType updateSource)
        {

            printVal = ConvertFromDoubleToTime(CalculateFlightTime(get_Distance(argument), get_velocity()));
            screenPrints();

        }


    }
}
