using LibraryData.Models;
using System;
using System.Collections.Generic;

namespace LibraryServices
{
    /*
     * Helper methods that are useful across the service classes 
     * Keeps the services as light as possible, so seervices only contain public methods
     * Methods to return data from the db, constructing data for the other services
     */

    public class DataHelpers
    {
        public static IEnumerable<string> HumanizeBusHours(IEnumerable<BranchHours> branchHours) 
        {
            var hours = new List<string>();
            foreach (var time in branchHours)
            {
                string day = HumanizeDay(time.DayOfWeek);
                string openTime = HumanizeTime(time.OpenTime);
                string closeTime = HumanizeTime(time.CloseTime);

                string timeEntry = $"{day} {openTime} {closeTime} ";
                hours.Add(timeEntry);
            }
            return hours;
        }

        public static string HumanizeDay(int number)
        {
            return Enum.GetName(typeof(DayOfWeek), number);
        }

        public static string HumanizeTime(int time)
        {
            return TimeSpan.FromHours(time).ToString("hh':'mm'");
        }


    }


}


