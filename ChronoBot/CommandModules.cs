using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ChronoBot.Models;
using ChronoBot.Services;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;
using NodaTime;
using UnitConversion;

namespace ChronoBot
{
    [Group("chrono")]

    public class ChronoModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commands;

        public ChronoModule(CommandService commands)
        {
            _commands = commands;
        }

        [Command("help")]
        [Summary("Displays a summary of available commands")]
        public async Task Help()
        {
            List<ModuleInfo> modules = _commands.Modules.ToList();
            EmbedBuilder embedBuilder = new EmbedBuilder();

            foreach (var module in modules)
            {
                foreach(var command in module.Commands)
                {
                    if (String.IsNullOrEmpty(module.Group)) {
                        // Get the command Summary attribute information
                        string embedFieldText = command.Summary ?? "No description available\n";

                        embedBuilder.AddField(command.Name, embedFieldText);
                    } 
                    else
                    {
                        //get the complete command
                        string embedFieldName = $"{module.Group} {command.Name}";
                        // Get the command Summary attribute information
                        string embedFieldText = command.Summary ?? "No description available\n";

                        embedBuilder.AddField(embedFieldName, embedFieldText);
                    }
                }
            }

            await ReplyAsync("Here's a list of commands and their description: ", false, embedBuilder.Build());
        }
    }

    public class FunModule : ModuleBase<SocketCommandContext>
    {
        private readonly HttpService _http;

        public FunModule(HttpService http)
        {
            _http = http;
        }

        [Command("ping")]
        [Summary("Plays ping-pong")]
        public async Task PingPongAsync() => await ReplyAsync("pong");

        [Command("pong")]
        [Summary("Plays more ping-pong")]
        public async Task PongPingAsync() => await ReplyAsync("ping");

        [Command("joke")]
        [Summary("Tells a random joke")]
        public async Task JokeAsync()
        {
            var response = _http.Client.GetAsync("https://official-joke-api.appspot.com/jokes/random").Result;
            string responseBody = response.Content.ReadAsStringAsync().Result;
            var joke = JsonConvert.DeserializeObject<Joke>(responseBody);

            await ReplyAsync($"{joke.Setup} {joke.Punchline}");
        }

        [Command("cat")]
        [Summary("cat")]
        public async Task GetCatAsync()
        {
            var response = _http.Client.GetAsync("https://cataas.com/cat?json=true").Result;
            var responseBody = response.Content.ReadAsStringAsync().Result;
            var cat = JsonConvert.DeserializeObject<Cat>(responseBody);

            await ReplyAsync("https://cataas.com/" + cat.Url);
        }

        [Command("dog")]
        [Summary("dog")]
        public async Task GetDogAsync()
        {
            var response = _http.Client.GetAsync("https://random.dog/woof.json").Result;
            var responseBody = response.Content.ReadAsStringAsync().Result;
            var dog = JsonConvert.DeserializeObject<Dog>(responseBody);

            await ReplyAsync(dog.Url);
        }

        [Command("fox")]
        [Summary("fox")]
        public async Task GetFoxAsync()
        {
            var response = _http.Client.GetAsync("https://randomfox.ca/floof/").Result;
            var responseBody = response.Content.ReadAsStringAsync().Result;
            var fox = JsonConvert.DeserializeObject<Fox>(responseBody);

            await ReplyAsync(fox.Image);
        }
    }

    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("timezones")]
        [Summary("Gets valid timezones")]
        public async Task ListTimezonesAsync() => await ReplyAsync("The current valid timezones are: Europe/London\nUS/Eastern\nUS/Central");

        [Command("distances")]
        [Summary("Gets valid distance units of measure")]
        public async Task ListDistanceUOMsAsync()
        {
            await ReplyAsync("The current valid units of measure for distance are:\n" +
                "m (or metre)\n" +
                "km (or kilometre)\n" +
                "cm (or centimeter)\n" +
                "mm (or milimetre)\n" +
                "ft (or foot, feet)\n" +
                "yd (or yard)\n" +
                "in (or inch)\n" +
                "The mile is not supported, for some reason.");
        }

        [Command("masses")]
        [Summary("Gets valid mass units of measure")]
        public async Task ListMassUOMsAsync()
        { 
            await ReplyAsync("The current valid mass units of measurement for mass are:\n" +
                "kg (or kilogram)\n" +
                "g (or gram)\n" +
                "lb (or lbs, pound, pounds)\n" +
                "st (or stone)\n" +
                "quintal\n" +
                "\"us ton\" (or \"short ton\", \"net ton\")\n" +
                "\"imperial ton\" (or \"long ton\", \"weight ton\", \"gross ton\")\n" +
                "\nUnits of measure with multiple words (i.e. \"imperial ton\") MUST be wrapped within quotes as displayed above.");
        }

        [Command("temperatures")]
        [Summary("Gets valid temperature units of measure")]
        public async Task ListTemperatureUOMsAsync()
        {
            await ReplyAsync("The current valid units of measurement for temperature are:\n" +
                "celcius (or Celcius, °C, °c)\n" +
                "fahrenheit (or Fahrenheit, °F, °f)\n" +
                "kelvin (or Kelvin, °K, °k)");
        }

        [Command("volumes")]
        [Summary("Gets valid volume units of measure")]
        public async Task ListVolumeUOMsAsync()
        {
            await ReplyAsync("The current valid units of measurement for volume are:\n" +
                "l, (or L, lt, ltr, liter, litre, dm³, dm3, \"cubic decimetre\"\n" +
                "m3 (or m³, \"cubic metre\")\n" +
                "cm3 (or cm³, \"cubic centimetre\"\n" +
                "mm3 (or mm³, \"cubic millimetre\")\n" +
                "ft3 (or ft³, \"cubic foot\", \"cubic feet\", \"cu ft\"\n" +
                "in3 (or in³, in3, \"cu in \", \"cubic inch\")\n" +
                "\"imperial pint\" (or \"imperial pt\", \"imperial p\")\n" +
                "\"imperial gallon\" (or \"imperial gal\")\n" +
                "\"imperial quart\" (or \"imperial qt\")\n" +
                "\"US pint\" (or \"US pt\", \"US p\")\n" +
                "\"US gallon\" (or \"US gal\")\n" +
                "\"US quart\" (or \"US qt\")\n" +
                "\nUnits of measure with multiple words (i.e. \"imperial pint\") MUST be wrapped within quotes as displayed above.");
        }
    }

    public class ReminderModule : ModuleBase<SocketCommandContext>
    {
        [Command("remindus", RunMode = RunMode.Async)]
        [Summary("Creates a series of reminders for an upcoming event.\nFormat: !remindus (event name, wrapped in \" \" if more than one word) (year) (month) (day) (hour, 24-hour scale) (minute) (timezone) \nExample: !remindus D&D 2021 08 28 13 00 US/Central")]
        public async Task SessionReminderAsync(string eventName, int year, int month, int day, int hour, int minute, string timeZoneName)
        {
            try
            {
                var tzProvider = DateTimeZoneProviders.Tzdb;

                LocalDateTime suppliedTime = new LocalDateTime(year, month, day, hour, minute);
                var zonedSuppliedTime = suppliedTime.InZoneLeniently(tzProvider[timeZoneName]);
                var twelveHoursBefore = zonedSuppliedTime.PlusMilliseconds(-43200000);

                var systemClock = SystemClock.Instance;
                var now = new ZonedDateTime(systemClock.GetCurrentInstant(), tzProvider[timeZoneName]);

                //get millisecond intervals between now and two hours before
                var timeUntilFirstReminder = twelveHoursBefore.ToInstant() - now.ToInstant();

                await ReplyAsync("Reminder set!");

                //Task.delay that long, send 12 hour message
                await Task.Delay((int)timeUntilFirstReminder.TotalMilliseconds); //this whole thing is stupid, use events/event handlers instead?
                await ReplyAsync($"Twelve hours until {eventName}!");
                //Task.delay 3600000 and send 1-hour warning
                await Task.Delay(39600000);
                await ReplyAsync($"One hour until {eventName} starts! Get HYPE!");
                //task.delay 2700000 and send 15 min warning
                await Task.Delay(2700000);
                await ReplyAsync($"15 minute warning, finish any last-minute prep!");
            } 
            catch (Exception e)
            {
                await ReplyAsync("Something broke. sadface.");
                await ReplyAsync(e.Message);
            }
        }
    }

    [Group("convert")]
    public class ConversionModule : ModuleBase<SocketCommandContext>
    {
        [Command("time")]
        [Summary("Converts from one *valid* timezone to another.\nFormat: !convert time (year) (month) (day) (hour on 24-hour scale) (minute)\nExample: !convert time 2021 08 13 15 00 US/Central")]
        public async Task ConvertTimeAsync(int year, int month, int day, int hour, int minute, string timeZoneName)
        {
            try
            {
                EmbedBuilder embedBuilder = new EmbedBuilder();

                var tzProvider = DateTimeZoneProviders.Tzdb;
                var ourZones = new List<string>(){
                    "Europe/London",
                    "US/Eastern",
                    "US/Central"
                };

                LocalDateTime suppliedTime = new LocalDateTime(year, month, day, hour, minute);
                var providedTime = suppliedTime.InZoneLeniently(tzProvider[timeZoneName]);
                var providedInstant = providedTime.ToInstant();

                foreach (var zone in ourZones)
                {
                    if (timeZoneName != zone)
                    {
                        var convertedZone = new ZonedDateTime(providedInstant, tzProvider[zone]);
                        var convertedTimeOfDay = convertedZone.Hour < 12 ? "AM" : "PM";
                        var embedMessage = String.Format("{0}:{1:00} {2} on {3}", convertedZone.ClockHourOfHalfDay, convertedZone.Minute, convertedTimeOfDay, convertedZone.Date);

                        embedBuilder.AddField(zone, embedMessage);
                    }
                }
                var suppliedTimeOfDay = suppliedTime.Hour < 12 ? "AM" : "PM";
                var titleMessage = String.Format("In the other timezones, {0}:{1:00} {2} on {3} in {4} is: ", suppliedTime.ClockHourOfHalfDay, suppliedTime.Minute, suppliedTimeOfDay, suppliedTime.Date, timeZoneName);

                await ReplyAsync(titleMessage, false, embedBuilder.Build());
            }
            catch (Exception ex)
            {
                await ReplyAsync(ex.ToString());
                await ReplyAsync("There was an issue, please check your command and try again.");
            }
        }

        [Command("distance")]
        [Summary("Converts from one *valid* distance to another.\nFormat: !convert distance (value) (original unit of measure) (target unit of measure)\nExample: !convert distance 10 metre feet")]
        public async Task ConvertDistanceAsync(double value, string originalUnit, string targetUnit)
        {
            try
            {
                var converter = new DistanceConverter(originalUnit, targetUnit);

                var message = $"{value} {originalUnit} is: {converter.LeftToRight(value)} {targetUnit}";

                await ReplyAsync(message);
            }
            catch (Exception ex)
            {
                await ReplyAsync("There was an issue, please check your command and try again.");
            }
        }

        [Command("mass")]
        [Summary("Converts from one *valid* mass to another.\nFormat: !convert mass (value) (original unit of measure) (target unit of measure)\nExample: !convert mass 25 lbs kg")]
        public async Task ConvertMassAsync(double value, string originalUnit, string targetUnit)
        {
            try
            {
                var converter = new MassConverter(originalUnit, targetUnit);

                var message = $"{value} {originalUnit} is: {converter.LeftToRight(value)} {targetUnit}";

                await ReplyAsync(message);
            }
            catch (Exception ex)
            {
                await ReplyAsync("There was an issue, please check your command and try again.");
            }
        }

        [Command("temperature")]
        [Summary("Converts from one *valid* temperature to another.\nFormat: !convert temperature (value) (original unit of measure) (target unit of measure)\nExample: !convert temperature 100 fahrenheit celsius")]
        public async Task ConvertTemperatureAsync(double value, string originalUnit, string targetUnit)
        {
            try
            {
                var converter = new TemperatureConverter(originalUnit, targetUnit);

                var message = $"{value} {originalUnit} is: {converter.LeftToRight(value)} {targetUnit}";

                await ReplyAsync(message);
            }
            catch (Exception ex)
            {
                await ReplyAsync("There was an issue, please check your command and try again.");
            }
        }

        [Command("volume")]
        [Summary("Converts from one *valid* volume to another.\nFormat: !convert volume (value) (original unit of measure) (target unit of measure)\nExample: !convert volume 20 L \"imperial gallon\"")]
        public async Task ConvertVolumeAsync(double value, string originalUnit, string targetUnit)
        {
            try
            {
                var converter = new VolumeConverter(originalUnit, targetUnit);

                var message = $"{value} {originalUnit} is: {converter.LeftToRight(value)} {targetUnit}";

                await ReplyAsync(message);
            }
            catch (Exception ex)
            {
                await ReplyAsync("There was an issue, please check your command and try again.");
            }
        }
    }
}
