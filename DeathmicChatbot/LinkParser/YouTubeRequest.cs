﻿using System;
using System.Text.RegularExpressions;
using Google.YouTube;
using DeathmicChatbot.Interfaces;

namespace DeathmicChatbot.LinkParser
{
    internal class YoutubeHandler : IURLHandler
    {
        private readonly Regex _reg;
        private readonly YouTubeRequest _request;

        public YoutubeHandler()
        {
            var settings = new YouTubeRequestSettings("Youtube Title Bot",
                                                      "AI39si6DFGChi5M0rnrX6p5dasT6STlFELYpJdbxdVXR3L1-Cj5RzNUU2nsm2LPmshlVGHuYmeaZ30zGJgqdhSSNoWQgJmEEDA");
            _request = new YouTubeRequest(settings);
            _reg =
                new Regex(
                    "^(?:https?\\:\\/\\/)?(?:www\\.)?(?:youtu\\.be\\/|youtube\\.com\\/(?:embed\\/|v\\/|watch\\?v\\=))([\\w-]{10,12})(?:[\\&\\?\\#].*?)*?(?:[\\&\\?\\#]t=([\\dhm]+s))?$");
        }

        public Video GetVideoInfo(string addr)
        {
            var videoEntryUrl =
                new Uri("http://gdata.youtube.com/feeds/api/videos/" + addr);
            Console.WriteLine(addr);
            return _request.Retrieve<Video>(videoEntryUrl);
        }

        public string IsYtLink(string txt)
        {
            var match = _reg.Match(txt);
            return match.Success ? match.Groups[1].Value : null;
        }

        public bool handleURL(string URL, IrcDotNet.IrcClient ctx)
        {
            var match = IsYtLink(URL);
            Console.WriteLine(URL);
            if (match != null)
            {
                ctx.LocalUser.SendMessage(Properties.Settings.Default.Channel,GetInfoString(GetVideoInfo(match)));
                return true;
            }
            return false;
        }

        public static string GetInfoString(Video vi) { return vi.Title + " " + Math.Round(vi.RatingAverage, 2) + "Ø"; }
    }
}