﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace CoinBalance.CoreAPI
{
    public static class Util
    {

        public static string Nonce => Convert.ToString(DateTime.UtcNow.ToUnixMs());

        public static DateTime IsoDateTimeToLocal(string isoTime)
        {
            return DateTimeOffset.Parse(isoTime, null, DateTimeStyles.RoundtripKind).LocalDateTime;
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static string Hr(int width)
        {
            return string.Concat(Enumerable.Repeat("-", width));
        }

        public static long ToUnixMs(this DateTime dt)
        {
            return (long) dt.ToUniversalTime().Subtract(
                new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            ).TotalMilliseconds;
        }

        public static string GenerateTimeStamp(DateTime dt)
        {
            var dtOffset = new DateTimeOffset(dt);
            return dtOffset.ToUnixTimeMilliseconds().ToString();
        }

        public static string GenerateNewHmac(string secret, string message)
        {
            var hmc = new HMACSHA256(Encoding.ASCII.GetBytes(secret));
            var hmres = hmc.ComputeHash(Encoding.ASCII.GetBytes(message));
            return BitConverter.ToString(hmres).Replace("-", "").ToLower();
        }

        public static string SHA512NewHmac(string secret, string message)
        {
            var hmc = new HMACSHA512(Encoding.ASCII.GetBytes(secret));
            var hmres = hmc.ComputeHash(Encoding.ASCII.GetBytes(message));
            return BitConverter.ToString(hmres).Replace("-", "").ToLower();
        }


        public static T ParseEnum<T>(string value)
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }

        public static string JwtHs256Encode(object payload, string secret)
        {
            //var secbyte = Encoding.UTF8.GetBytes(secret);
            //return JWT.Encode(payload, secbyte, JwsAlgorithm.HS256);

            byte[] headerBytes = Encoding.UTF8.GetBytes("{\"alg\":\"HS256\",\"typ\":\"JWT\"}");
            string header = Convert.ToBase64String(headerBytes);

            byte[] payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));
            string body = Convert.ToBase64String(payloadBytes);

            var header_body = header + "." + body;

            byte[] chaHash = new HMACSHA256(Encoding.UTF8.GetBytes(secret)).ComputeHash(Encoding.UTF8.GetBytes(header_body));
            string sign = Convert.ToBase64String(chaHash);

            return header_body + "." + sign;

        }

        public static IEnumerable<T> Enums<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static decimal RoundDown(decimal d, double decimalPlaces)
        {
            var power = Convert.ToDecimal(Math.Pow(10, decimalPlaces));
            return Math.Floor(d * power) / power;
        }

        public static long ToEpochSeconds(DateTime dt)
        {
            var dto = new DateTimeOffset(dt.Ticks, new TimeSpan(+09, 00, 00));
            return dto.ToUnixTimeSeconds();
        }

        public static DateTime FromEpochSeconds(long EpochSeconds)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddSeconds(EpochSeconds);
        }
    }
}