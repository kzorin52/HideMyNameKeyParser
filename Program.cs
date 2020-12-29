using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using xNet;

namespace HideMyNameKeyParser
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var htmls = "";
            var keys = new List<string>();
            var sites = new List<string>
            {
                "https://vk.com/hidemy_vpn_keys",
                "https://vk.com/hidemy_name_keys",
                "https://vk.com/keys_hidemy_vpn"
            };

            Console.Title = "HideMyName code parser & checker by Temnij";


            #region Parser

            foreach (var site in sites)
            {
                Log("Downloading " + site);
                htmls += new WebClient().DownloadString(site) + "\n";
                Log("\t~Downloaded~\n");

                if(sites.IndexOf(site) != 2) Thread.Sleep(3000);
            }

            foreach (Match match in Regex.Matches(htmls, "(\\d{14})"))
            {
                Log(match.Groups[1]);
                keys.Add(match.Groups[1].Value);
            }

            #endregion

            #region Checker

            Console.ForegroundColor = ConsoleColor.DarkRed;

            Log("\nChecking...\n\n");

            var req = new HttpRequest();

            Log("Чел. Откуда прокси брать будем? Принимаю только Socks4!");

            var proxies = File.ReadAllLines(Console.ReadLine());

            var x = 0;

            keys.Shuffle();

            var okKeysList = new List<string>();

            foreach (var key in keys)
            {
                await Task.Run(() =>
                {
                    check:
                    try
                    {
                        req.Proxy = ProxyClient.Parse(ProxyType.Socks4, proxies[x]);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        x = 0;
                    }

                    try
                    {
                        var request =
                            req.Get($"https://hidemy.name/api/vpn_check_code.php?code={key}").ToString();

                        if (request.Contains("PROXY"))
                        {
                            x++;
                            goto check;
                        }

                        if (request.Contains("FAST"))
                        {
                            Thread.Sleep(500);
                            x++;
                            goto check;
                        }

                        if (request.Contains("OK")) okKeysList.Add(key);

                        Log(key + " " + request);
                    }
                    catch
                    {
                        x++;
                        goto check;
                    }
                });

                x++;
            }

            //Log("Done!");

            #endregion

            var keyC = Console.ReadKey();
            switch (keyC.Key)
            {
                case ConsoleKey.S:
                    File.WriteAllLines("Keys.txt", okKeysList);
                    break;
                case ConsoleKey.E:
                    Environment.Exit(0);
                    break;
            }
            // Эта фигня почему-то не робит ;((((
        }

        private static void Log(object text)
        {
            Console.WriteLine(text);
        }
    }

    public static class Ext
    {
        private static readonly Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}