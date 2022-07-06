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
    public partial class Form1
    {

        /// <summary>
        /// Read SRT File
        /// </summary>
        /// <param name="pathSrtFile"></param>
        /// <returns>List of Srt File Elements</returns>
        private List<SrtFileElements> ReadSrtFile(string pathSrtFile)
        {
            List<SrtFileElements> srtFileElementsList = new List<SrtFileElements>();

            try
            {
                using (StreamReader readerSRT = new StreamReader(pathSrtFile))
                {
                    List<string> readedLines = new List<string>();

                    while (true)
                    {
                        readedLines.Clear();

                        while (true)
                        {
                            readedLines.Add(readerSRT.ReadLine());

                            if (readedLines.LastOrDefault() == "" || readerSRT.EndOfStream)
                            {

                                if (readedLines.Count < 3) throw new Exception("Błąd w odczytywaniu danych");


                                string indexSubititleString = readedLines[0];
                                string rowWithTimes = readedLines[1];

                                string[] textArray = readedLines
                                    .Where(s => readedLines.IndexOf(s) > 1 && s != "").ToArray();

                                srtFileElementsList.Add(new SrtFileElements(indexSubititleString, rowWithTimes, textArray));
                                if (srtFileElementsList.LastOrDefault().Error) return new List<SrtFileElements>();


                                break;
                            }

                        }


                        if (readerSRT.EndOfStream)
                            break;

                    }

                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return srtFileElementsList;
        }




        /// <summary>
        /// Add Time to start and end time subtitles
        /// </summary>
        /// <param name="srtFileElementsList"></param>
        /// <param name="minutes"></param>
        /// <param name="seconds"></param>
        /// <param name="milliseconds"></param>
        private void AddTimeToSrtSubtitlesList(List<SrtFileElements> srtFileElementsList, int minutes, int seconds, int milliseconds)
        {
            for (int i = 0; i < srtFileElementsList.Count; i++)
            {
                srtFileElementsList[i].AddTimeToSrtSubtitles(minutes, seconds, milliseconds);
            }
        }



        /// <summary>
        /// Get List of Srt File Elements where start time milliseconds is zero
        /// </summary>
        /// <param name="srtFileElementsList"></param>
        /// <returns></returns>
        private List<SrtFileElements> GetSrtDataListIfStartMillisecondsIsZero(List<SrtFileElements> srtFileElementsList)
        {
            List<SrtFileElements> srtFileElementsStartZeroMillisecond = new List<SrtFileElements>();

            for (int i = 0; i < srtFileElementsList.Count; i++)
            {
                if (srtFileElementsList[i].StartTimeSubtitle.Millisecond == 0)
                {
                    srtFileElementsStartZeroMillisecond.Add(srtFileElementsList[i]);

                    srtFileElementsList.RemoveAt(i);
                    --i;

                }
            }
            return srtFileElementsStartZeroMillisecond;

        }


        private void SaveListSrtElementsToFile(List<SrtFileElements> srtFileElementsList, string path)
        {
            try
            {
                int iter = 1;
                StringBuilder finnalySrtFileSB = new StringBuilder();
                foreach (SrtFileElements srtFileElements in srtFileElementsList)
                {
                    finnalySrtFileSB.Append(iter.ToString() + "\r\n");
                    finnalySrtFileSB.Append($"{srtFileElements.StartTimeSubtitle.ToString("HH:mm:ss,fff")} --> {srtFileElements.EndTimeSubtitle.ToString("HH:mm:ss,fff")}\r\n");
                    finnalySrtFileSB.Append(string.Join("\r\n", srtFileElements.TextSubtitle));
                    finnalySrtFileSB.Append("\r\n\r\n");

                    ++iter;
                }
                //remove last \r\n
                if (finnalySrtFileSB.Length > 2) finnalySrtFileSB.Remove(finnalySrtFileSB.Length - 2, 2);

                File.WriteAllText(path, finnalySrtFileSB.ToString());
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message, "Błąd!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }


    }
}
