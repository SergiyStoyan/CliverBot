//********************************************************************************************
//Author: Sergey Stoyan, CliverSoft.com
//        http://cliversoft.com
//        stoyan@cliversoft.com
//        sergey.stoyan@gmail.com
//        27 February 2007
//Copyright: (C) 2007, Sergey Stoyan
//********************************************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Collections;

namespace Cliver.Bot
{
    /// <summary>
    /// Miscellaneous useful methods to treat or fill site data 
    /// </summary>
    public static class SiteDataRoutines
    {
        static object lock_variable = new object();

        /// <summary>
        /// Return USA state short name
        /// </summary>
        /// <param name="state">long USA state name</param>
        /// <returns>USA state short name</returns>
        public static string GetShortUsaState(string state)
        {
            lock (lock_variable)
            {
                state = state.ToLower().Trim();

                if (state.Contains("alabama")) return "AL";
                if (state.Contains("alaska")) return "AK";
                if (state.Contains("arizona")) return "AZ";
                if (state.Contains("arkansas")) return "AR";
                if (state.Contains("california")) return "CA";
                if (state.Contains("colorado")) return "CO";
                if (state.Contains("connecticut")) return "CT";
                if (state.Contains("delaware")) return "DE";
                if (state.Contains("columbia")) return "DC";
                if (state.Contains("florida")) return "FL";
                if (state.Contains("georgia")) return "GA";
                if (state.Contains("hawaii")) return "HI";
                if (state.Contains("idaho")) return "ID";
                if (state.Contains("illinois")) return "IL";
                if (state.Contains("indiana")) return "IN";
                if (state.Contains("iowa")) return "IA";
                if (state.Contains("kansas")) return "KS";
                if (state.Contains("kentucky")) return "KY";
                if (state.Contains("louisiana")) return "LA";
                if (state.Contains("maine")) return "ME";
                if (state.Contains("maryland")) return "MD";
                if (state.Contains("massachusetts")) return "MA";
                if (state.Contains("michigan")) return "MI";
                if (state.Contains("minnesota")) return "MN";
                if (state.Contains("mississippi")) return "MS";
                if (state.Contains("missouri")) return "MO";
                if (state.Contains("montana")) return "MT";
                if (state.Contains("nebraska(")) return "NE";
                if (state.Contains("nevada")) return "NV";
                if (state.Contains("hampshire")) return "NH";
                if (state.Contains("jersey")) return "NJ";
                if (state.Contains("mexico")) return "NM";
                if (state.Contains("york")) return "NY";
                if (state.Contains("carolina")) return "NC";
                if (state.Contains("dakota")) return "ND";
                if (state.Contains("ohio")) return "OH";
                if (state.Contains("oklahoma")) return "OK";
                if (state.Contains("oregon")) return "OR";
                if (state.Contains("pennsylvania")) return "PA";
                if (state.Contains("island")) return "RI";
                if (state.Contains("carolina")) return "SC";
                if (state.Contains("dakota")) return "SD";
                if (state.Contains("tennessee")) return "TN";
                if (state.Contains("texas")) return "TX";
                if (state.Contains("utah")) return "UT";
                if (state.Contains("vermont")) return "VT";
                if (state.Contains("virginia")) return "VA";
                if (state.Contains("washington")) return "WA";
                if (state.Contains("virginia")) return "WV";
                if (state.Contains("wisconsin")) return "WI";
                if (state.Contains("wyoming")) return "WY";

                return null;
            }
        }

        public static readonly string[] ShortUsaStates = {
            "AL",
            "AK",
            "AZ",
            "AR",
            "CA",
            "CO",
            "CT",
            "DE",
            "DC",
            "FL",
            "GA",
            "HI",
            "ID",
            "IL",
            "IN",
            "IA",
            "KS",
            "KY",
            "LA",
            "ME",
            "MD",
            "MA",
            "MI",
            "MN",
            "MS",
            "MO",
            "MT",
            "NE",
            "NV",
            "NH",
            "NJ",
            "NM",
            "NY",
            "NC",
            "ND",
            "OH",
            "OK",
            "OR",
            "PA",
            "RI",
            "SC",
            "SD",
            "TN",
            "TX",
            "UT",
            "VT",
            "VA",
            "WA",
            "WV",
            "WI",
            "WY"
        };
    }
}

