using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.IO;
using System.Globalization;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using System.Diagnostics;



namespace search_nip_change_time_recruitment_task
{

    public class SrtFileElements
    {
        public int IndexSubtitle { get; set; }
        public DateTime StartTimeSubtitle { get; set; }
        public DateTime EndTimeSubtitle { get; set; }
        public List<string> TextSubtitle { get; set; }
        public bool Error { get; }

        public SrtFileElements(string indexSubtitleString, string rowWithTimes, string[] textSubtitlesArray)
        {
            try
            {
                IndexSubtitle = int.Parse(indexSubtitleString);

                //times[0]==start times[1]==end
                string[] times = rowWithTimes.Replace("-->", "©").Split('©');

                if (times.Length != 2) throw new Exception("Wystąpił błąd podczas odczytu czasu trwania napisu");


                StartTimeSubtitle = DateTime.ParseExact(times[0].Trim(), "HH:mm:ss,fff", CultureInfo.InvariantCulture);
                EndTimeSubtitle = DateTime.ParseExact(times[1].Trim(), "HH:mm:ss,fff", CultureInfo.InvariantCulture);

                TextSubtitle = new List<string>(textSubtitlesArray);

                Error = false;
            }
            catch (Exception error)
            {
                Error = true;
                MessageBox.Show(error.Message, "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }


        public void AddTimeToSrtSubtitles(int minutes, int seconds, int milliseconds)
        {
            StartTimeSubtitle = StartTimeSubtitle.AddMinutes(minutes);
            StartTimeSubtitle = StartTimeSubtitle.AddSeconds(seconds);
            StartTimeSubtitle = StartTimeSubtitle.AddMilliseconds(milliseconds);

            EndTimeSubtitle = EndTimeSubtitle.AddMinutes(minutes);
            EndTimeSubtitle = EndTimeSubtitle.AddSeconds(seconds);
            EndTimeSubtitle = EndTimeSubtitle.AddMilliseconds(milliseconds);
        }

    }

}
