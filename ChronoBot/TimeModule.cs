using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using UnitConversion;

namespace ChronoBot
{

    public class ChronoModule : ModuleBase<SocketCommandContext>
    {
        [Command("commands")]
        [Summary("Provides a summary for all commands")]
        public async Task CommandSummary()
        {
            var commandMessage = "Valid commands:\n" +
                "- !say: repeats whatever comes after the command\n\n" +
                "- !ping: plays ping pong\n\n" +
                "- !pong: plays ping pong\n\n" +
                "- !convert time: Converts a DateTime into the other timezones in our server. Must match format exactly!\nFormat: !convert time year(4 digits) month(2 digits) day(2 digits) hour(2 digits, 24-hour scale) minute(2 digits) (timezone name)\n !timezones will give you valid timezones.\n" +
                "Example: !convert time 2021 08 10 13 30 \"Central Standard Time\"\n\n" +
                "- !convert distance: Converts a distance from one unit of measure into another. Must match format exactly!\nFormat: !convert distance (value) (original unit of measure) (target unit of measure)\n !distances will give you valid units of measure.\n" +
                "Example: !convert distance 100 metre ft\n\n" +
                "- !convert mass: Converts a mass from one unit of measure into another. Must match format exactly!\nFormat: !convert mass (value) (original unit of measure) (target unit of measure)\n !masses will give you valid units of measure.\n" +
                "Example: !convert mass 50 lb kg\n\n" +
                "- !convert temperature: Converts a temperature from one unit of measure into another. Must match format exactly!\nFormat: !convert temperature (value) (original unit of measure) (target unit of measure)\n !temperatures will give you valid units of measure.\n" +
                "Example: !convert temperature 100 fahrenheit celsius\n\n" +
                "- !convert volume: Converts a volume from one unit of measure into another. Must match format exactly!\nFormat: !convert volume (value) (original unit of measure) (target unit of measure)\n !volumes will give you valid units of measure.\n" +
                "Example: !convert volume 1 L \"imperial gallon\"";
            await Context.Channel.SendMessageAsync(commandMessage);
        }
    }

    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        [Command("timezones")]
        public async Task ListTimezonesAsync() => await ReplyAsync("The current valid timezones are: GMT Standard Time\nEastern Standard Time\nCentral Standard Time");

        [Command("distances")]
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
        public async Task ListTemperatureUOMsAsync()
        {
            await ReplyAsync("The current valid units of measurement for temperature are:\n" +
                "celcius (or Celcius, °C, °c)\n" +
                "fahrenheit (or Fahrenheit, °F, °f)\n" +
                "kelvin (or Kelvin, °K, °k)");
        }

        [Command("volumes")]
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

    public class FunModule : ModuleBase<SocketCommandContext>
    {
        [Command("say")]
        [Summary("Echoes a message.")]
        public async Task SayAsync([Remainder] [Summary("The text to echo")] string echo)
        {
            await Context.Channel.SendMessageAsync(echo);
        }

        [Command("ping")]
        [Summary("Plays ping-pong")]
        public async Task PingPongAsync() => await ReplyAsync("pong");

        [Command("pong")]
        [Summary("Plays more ping-pong")]
        public async Task PongPingAsync() => await ReplyAsync("ping");
    }

    [Group("convert")]
    public class ConversionModule : ModuleBase<SocketCommandContext>
    {
        [Command("time")]
        [Summary("Converts from one time zone to the others on the server")]
        public async Task ConvertTimeAsync(int year, int month, int day, int hour, int minute, string timeZoneName)
        {
            try
            {
                DateTime suppliedTime = new DateTime(year, month, day, hour, minute, 0);

                var ourZones = new List<string>(){
                "GMT Standard Time",
                "Eastern Standard Time",
                "Central Standard Time"
                };

                var timeMessage = String.Format("{0:t} in {1} on {0:d} is:\n", suppliedTime, timeZoneName);

                foreach (var zone in ourZones)
                {
                    if (timeZoneName != zone)
                    {
                        timeMessage += String.Format("~ {0:t} in {1}\n", TimeZoneInfo.ConvertTimeBySystemTimeZoneId(suppliedTime, timeZoneName, zone), zone);
                    }
                }

                await ReplyAsync(timeMessage);
            }
            catch (Exception ex)
            {
                await ReplyAsync("There was an issue, please check your command and try again.");
            }
        }

        [Command("distance")]
        [Summary("Converts distance from the first specified unit to the second")]
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
        [Summary("Converts mass from the first specified unit to the second")]
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
        [Summary("Converts temperature from the first specified unit to the second")]
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
        [Summary("Converts volume from the first specified unit to the second")]
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
