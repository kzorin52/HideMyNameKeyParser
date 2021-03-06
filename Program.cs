﻿using Newtonsoft.Json.Linq;
using ProxyScrapeAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using xNet;

namespace HideMyNameKeyParser
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var htmls = "";

            var sites = new List<string>
            {
                "https://api.vk.com/method/wall.get?count=3&v=5.126&access_token=6a7240856a7240856a724085736a0327f666a726a72408534d612167901c443446147c0&domain=hidemy_vpn_keys",
                "https://api.vk.com/method/wall.get?count=3&v=5.126&access_token=6a7240856a7240856a724085736a0327f666a726a72408534d612167901c443446147c0&domain=hidemy_name_keys"
            };

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls |
                                                   SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            Console.Title = "\tHideMyName code parser & checker by -=[TEMNIJ]=- v 2.0";
            var keys = new List<string>();

            Console.WriteLine("Режим:\n\t1 - Генерация & чек\n\t2 - Парсинг & чек\n");
            var mode = int.Parse(Console.ReadLine());
            if (mode == 1)
            {
                #region Generator

                for (var i = 0; i < 150; i++)
                {
                    var rnd = new Random(Environment.TickCount);
                    var l1 = rnd.Next(0, 9).ToString();
                    Thread.Sleep(5);
                    var l2 = rnd.Next(0, 9).ToString();
                    Thread.Sleep(5);
                    var l3 = rnd.Next(0, 9).ToString();
                    Thread.Sleep(5);
                    var l4 = rnd.Next(0, 9).ToString();
                    Thread.Sleep(5);
                    var l5 = rnd.Next(0, 9).ToString();
                    Thread.Sleep(5);
                    var l6 = rnd.Next(0, 9).ToString();
                    Thread.Sleep(5);
                    var l7 = rnd.Next(0, 9).ToString();
                    Thread.Sleep(5);
                    var l8 = rnd.Next(0, 9).ToString();
                    Thread.Sleep(5);
                    var l9 = rnd.Next(0, 9).ToString();
                    Thread.Sleep(5);
                    var l10 = rnd.Next(0, 9).ToString();
                    Thread.Sleep(5);
                    var l11 = rnd.Next(0, 9).ToString();
                    Thread.Sleep(5);
                    var l12 = rnd.Next(0, 9).ToString();
                    Thread.Sleep(5);
                    var l13 = rnd.Next(0, 9).ToString();
                    Thread.Sleep(5);
                    var l14 = rnd.Next(0, 9).ToString();

                    keys.Add(l1 + l2 + l3 + l4 + l5 + l6 + l7 + l8 + l9 + l10 + l11 + l12 + l13 + l14);
                }

                #endregion Generator
            }

            if (mode == 2)
            {
                #region Parser

                foreach (var site in sites)
                {
                    Log("Downloading " + site.Substring(143));
                    htmls += new WebClient().DownloadString(site) + "\n";
                    Log("\t~Downloaded~\n");
                }

                keys.AddRange(Regex.Matches(htmls, "(\\d{14})")
                    .OfType<Match>()
                    .Select(m => m.Groups[0].Value)
                    .ToArray());
                Log($"Нашёл {keys.Count} ключа(ей)!");

                #endregion
            }

            var okKeysList = new List<string>();

            var check = new Task(() =>
            {
                #region Checker

                void SaveAndGrabProxies(object obj)
                {
                    File.WriteAllLines($"Keys-{DateTime.Now:dd-MM-yyyy-HH-mm}.txt", okKeysList);
                    Log("\nСохранил\n");
                }

                var timer = new Timer(SaveAndGrabProxies, 0, 1000 * 60 * 5, 1000 * 60 * 5);

                Console.ForegroundColor = ConsoleColor.Red;

                Log("\nChecking...\n\n");

                var req = new HttpRequest();
                string[] proxies;

                req.UserAgent = "AndroidVPNGui/2.0.79 (h-RU; Android SDK: 29 (10); ru; +0600)";

                Log("Чтоб облегчить тебе труд, так уж и быть, прокси я сам скачаю");

                if (!File.Exists("socks4autoload.txt"))
                {
                    proxies = new Scraper()
                        .GetProxies(Scraper.ProxyType.Socks4, 2000)
                        .Replace("\r", "")
                        .Split('\n');

                    Log($"\tСкачал {proxies.Length} проксей \n");
                }
                else
                {
                    proxies = File.ReadAllLines("socks4autoload.txt");

                    Log($"\nПодгрузил {proxies.Length} проксей \n");
                }


                Log("Чтобы выйти, смело жми 'e', ну а чтобы сохранить все 'OK' ключи, жми 's'!\n");

                var errors = 0;
                var x = 0;

                keys.Shuffle();

                var keys1 = keys.GetRange(0, keys.Count / 4);
                var keys2 = keys.GetRange(keys.Count / 4, keys.Count / 4);
                var keys3 = keys.GetRange((keys.Count / 4) * 2, keys.Count / 4);
                var keys4 = keys.GetRange((keys.Count / 4) * 3, keys.Count / 4);

                Task.Run(() =>
                {
                    foreach (var key in keys1)
                    {
                        new Thread(() =>
                        {
                        check:
                            try
                            {
                                req.Proxy = ProxyClient.Parse(ProxyType.Socks4, proxies[x]);
                            }
                            catch (IndexOutOfRangeException)
                            {
                                x = 0;
                                goto check;
                            }
                            catch (ArgumentException)
                            {
                                x++;
                                goto check;
                            }

                            try
                            {
                                var request =
                                    req.Get($"https://hidemy.name/api/vpn_check_code.php?out=js&code={key}").ToString();

                                var code = JObject.Parse(request)["result"].ToString();

                                if (code.Contains("PROXY"))
                                {
                                    x++;
                                    goto check;
                                }

                                if (code.Contains("FAST"))
                                {
                                    Thread.Sleep(500);
                                    x++;
                                    goto check;
                                }

                                if (code.Contains("OK"))
                                {
                                    try
                                    {
                                        okKeysList.Add(key);

                                        DateTime pDate = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(double.Parse(JObject.Parse(request)["time_remaining"].ToString()));

                                        Log(key + " " + code + " " + pDate.ToString("hh:mm:ss") +
                                            " осталось");
                                    }
                                    catch(Exception ex)
                                    {
                                        throw(ex);
                                    }
                                }
                                else
                                {
                                    Log(key + " " + code);
                                }
                            }
                            catch
                            {
                                errors++;
                                x++;

                                Console.Title = "Checking... Errors: " + errors;
                                goto check;
                            }

                        }).Start();


                        x++;
                    }
                });

                Task.Run(() =>
                {
                    foreach (var key in keys2)
                    {
                        new Thread(() =>
                        {
                        check:
                            try
                            {
                                req.Proxy = ProxyClient.Parse(ProxyType.Socks4, proxies[x]);
                            }
                            catch (IndexOutOfRangeException)
                            {
                                x = 0;
                                goto check;
                            }
                            catch (ArgumentException)
                            {
                                x++;
                                goto check;
                            }

                            try
                            {
                                var request =
                                    req.Get($"https://hidemy.name/api/vpn_check_code.php?out=js&code={key}").ToString();

                                var code = JObject.Parse(request)["result"].ToString();

                                if (code.Contains("PROXY"))
                                {
                                    x++;
                                    goto check;
                                }

                                if (code.Contains("FAST"))
                                {
                                    Thread.Sleep(500);
                                    x++;
                                    goto check;
                                }

                                if (code.Contains("OK"))
                                {
                                    try
                                    {
                                        okKeysList.Add(key);

                                        DateTime pDate = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(JObject.Parse(request)["time_remaining"].ToString().ToInt());
                                        var tdate = pDate - DateTime.Now;

                                        Log(key + " " + code + " " + tdate.ToString("hh:mm:ss") +
                                            " осталось");
                                    }
                                    catch(Exception ex)
                                    {
                                        throw(ex);
                                    }
                                }
                                else
                                {
                                    Log(key + " " + code);
                                }
                            }
                            catch
                            {
                                errors++;
                                x++;

                                Console.Title = "Checking... Errors: " + errors;
                                goto check;
                            }

                        }).Start();


                        x++;
                    }
                });
                
                Task.Run(() =>
                {
                    foreach (var key in keys3)
                    {
                        new Thread(() =>
                        {
                        check:
                            try
                            {
                                req.Proxy = ProxyClient.Parse(ProxyType.Socks4, proxies[x]);
                            }
                            catch (IndexOutOfRangeException)
                            {
                                x = 0;
                                goto check;
                            }
                            catch (ArgumentException)
                            {
                                x++;
                                goto check;
                            }

                            try
                            {
                                var request =
                                    req.Get($"https://hidemy.name/api/vpn_check_code.php?out=js&code={key}").ToString();

                                var code = JObject.Parse(request)["result"].ToString();

                                if (code.Contains("PROXY"))
                                {
                                    x++;
                                    goto check;
                                }

                                if (code.Contains("FAST"))
                                {
                                    Thread.Sleep(500);
                                    x++;
                                    goto check;
                                }

                                if (code.Contains("OK"))
                                {
                                    try
                                    {
                                        okKeysList.Add(key);

                                        DateTime pDate = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(double.Parse(JObject.Parse(request)["time_remaining"].ToString()));

                                        Log(key + " " + code + " " + pDate.ToString("hh:mm:ss") +
                                            " осталось");
                                    }
                                    catch(Exception ex)
                                    {
                                        throw(ex);
                                    }
                                }
                                else
                                {
                                    Log(key + " " + code);
                                }
                            }
                            catch
                            {
                                errors++;
                                x++;

                                Console.Title = "Checking... Errors: " + errors;
                                goto check;
                            }

                        }).Start();


                        x++;
                    }
                });

                Task.Run(() =>
                {
                    foreach (var key in keys4)
                    {
                        new Thread(() =>
                        {
                        check:
                            try
                            {
                                req.Proxy = ProxyClient.Parse(ProxyType.Socks4, proxies[x]);
                            }
                            catch (IndexOutOfRangeException)
                            {
                                x = 0;
                                goto check;
                            }
                            catch (ArgumentException)
                            {
                                x++;
                                goto check;
                            }

                            try
                            {
                                var request =
                                    req.Get($"https://hidemy.name/api/vpn_check_code.php?out=js&code={key}").ToString();

                                var code = JObject.Parse(request)["result"].ToString();

                                if (code.Contains("PROXY"))
                                {
                                    x++;
                                    goto check;
                                }

                                if (code.Contains("FAST"))
                                {
                                    Thread.Sleep(500);
                                    x++;
                                    goto check;
                                }

                                if (code.Contains("OK"))
                                {
                                    try
                                    {
                                        okKeysList.Add(key);

                                        DateTime pDate = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(JObject.Parse(request)["time_remaining"].ToString().ToInt());
                                        var tdate = pDate - DateTime.Now;

                                        Log(key + " " + code + " " + tdate.ToString("hh:mm:ss") +
                                            " осталось");
                                    }
                                    catch(Exception ex)
                                    {
                                        throw(ex);
                                    }
                                }
                                else
                                {
                                    Log(key + " " + code);
                                }
                            }
                            catch
                            {
                                errors++;
                                x++;

                                Console.Title = "Checking... Errors: " + errors;
                                goto check;
                            }

                        }).Start();


                        x++;
                    }
                });

                #endregion

                #region Timer

                #endregion
            });
            check.Start();


            while (true)
            {
                if (check.IsCompleted)
                    Console.WriteLine("Done!");

                var keyC = Console.ReadKey();
                switch (keyC.Key)
                {
                    case ConsoleKey.S:
                        Save(okKeysList);
                        Log("\nСохранил");
                        break;
                    case ConsoleKey.E:
                        check.Dispose();
                        Environment.Exit(0);
                        break;
                }
            }
        }

        static void Save(List<string> keys)
        {
            List<string> okKeysList = new List<string>();

            for (int i = 0; i < keys.Count; i++)
            {
                okKeysList.Add($"{i}) 🔑 —{keys[i]}— 🔑");
            }
            okKeysList.Add("Все ключи чекались чекером ключей `HideMyName code parser & checker by -=[TEMNIJ]=-`");

            File.WriteAllLines($"Ключи HideMy {DateTime.Now:dd-MM-yyyy HH-mm}.txt", okKeysList);
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

        public static int ToInt(this string stringer)
        {
            return int.Parse(stringer);
        }
    }
}