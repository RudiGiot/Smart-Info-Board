using System;
using System.Globalization;
using System.Collections.Generic;
using UnityEngine;

namespace iCalAPI
{
	public class iCal
	{
        public string PRODID;
        public string VERSION;
        public string CALSCALE;
        public string METHOD;
        public string X_WR_CALNAME;
        public string X_WR_TIMEZONE;
        public List<vevent> vevent;

		public iCal(String rawData)
		{
			String[] line = rawData.Split('\n');
			int i=0;
			if (line[0].StartsWith("BEGIN:VCALENDAR", StringComparison.Ordinal)) {
				while(!line[i].StartsWith("BEGIN:VEVENT"))
				{
					i++;
					if(line[i].StartsWith("PRODID")) PRODID = line[i].Substring(7, line[i].Length-1-7);
					if(line[i].StartsWith("VERSION")) VERSION = line[i].Substring(8, line[i].Length-1-8);
					if(line[i].StartsWith("CALSCALE")) CALSCALE = line[i].Substring(9, line[i].Length-1-9);
					if(line[i].StartsWith("METHOD")) METHOD = line[i].Substring(7, line[i].Length-1-7);
					if(line[i].StartsWith("X-WR-CALNAME")) X_WR_CALNAME = line[i].Substring(13, line[i].Length-1-13);
					if(line[i].StartsWith("X-WR-TIMEZONE")) X_WR_TIMEZONE = line[i].Substring(14, line[i].Length-1-14);
				}
				vevent = new List<iCalAPI.vevent>();
                while (!line[i].StartsWith("END:VCALENDAR"))
                {
                    vevent.Add(new iCalAPI.vevent());
                    while (!line[i].StartsWith("END:VEVENT"))
                    {
                        i++;
                        if (line[i].StartsWith("DTSTART")) vevent[vevent.Count - 1].DTSTART = getDateFromString(line[i]);
                        if (line[i].StartsWith("DTEND")) vevent[vevent.Count - 1].DTEND = getDateFromString(line[i]);
                        if (line[i].StartsWith("DTSTAMP")) vevent[vevent.Count - 1].DTSTAMP = getDateFromString(line[i]);
                        if (line[i].StartsWith("UID")) vevent[vevent.Count - 1].UID = line[i].Substring(4, line[i].Length - 1 - 4);
                        if (line[i].StartsWith("CREATED")) vevent[vevent.Count - 1].CREATED = getDateFromString(line[i]);
                        if (line[i].StartsWith("DESCRIPTION")) vevent[vevent.Count - 1].DESCRIPTION = line[i].Substring(12, line[i].Length - 1 - 12);
                        if (line[i].StartsWith("LAST-MODIFIED")) vevent[vevent.Count - 1].LAST_MODIFIED = getDateFromString(line[i]);
                        if (line[i].StartsWith("LOCATION")) vevent[vevent.Count - 1].LOCATION = line[i].Substring(9, line[i].Length - 1 - 9);
                        if (line[i].StartsWith("SEQUENCE")) vevent[vevent.Count - 1].SEQUENCE = int.Parse(line[i].Substring(9, line[i].Length - 1 - 9));
                        if (line[i].StartsWith("STATUS")) vevent[vevent.Count - 1].STATUS = line[i].Substring(7, line[i].Length - 1 - 7);
                        if (line[i].StartsWith("SUMMARY")) vevent[vevent.Count - 1].SUMMARY = line[i].Substring(8, line[i].Length - 1 - 8);
                        if (line[i].StartsWith("TRANSP")) vevent[vevent.Count - 1].TRANSP = line[i].Substring(7, line[i].Length - 1 - 7);

                    }
                    // vérifier si le DTEND est <= à la daye actuelle, si pas delete le dernier élément de la liste
                    if (vevent[vevent.Count - 1].DTEND <= DateTime.Now) vevent.RemoveAt(vevent.Count-1);
                    // vérifier si c'est un rendez vous (heure dans la journée et pas minuit)
                    else if (vevent[vevent.Count - 1].DTEND.Hour != 0) vevent.RemoveAt(vevent.Count - 1);
					i++;
				}
            }
		}

		private DateTime getDateFromString(string stringDate)
		{
			DateTime dateToReturn;
			//Remove the last invisible character 
			stringDate = stringDate.Substring(0, stringDate.Length-1);
			//Split the sting at the semi-colon
			string [] stringPart = stringDate.Split(':');
			if(stringPart[1].Length==8) dateToReturn = DateTime.ParseExact(stringPart[1], "yyyyMMdd", CultureInfo.InvariantCulture);
			else dateToReturn = DateTime.ParseExact(stringPart[1], "yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture);
			return dateToReturn;
		}
	}

    public partial class vevent
    {
        public DateTime DTSTART;
		public DateTime DTEND;
		public DateTime DTSTAMP;
		public string UID;
		public DateTime CREATED;
		public string DESCRIPTION;
		public DateTime LAST_MODIFIED;
		public string LOCATION;
		public int SEQUENCE;
		public string STATUS;
		public string SUMMARY;
		public string TRANSP;
    }
}
