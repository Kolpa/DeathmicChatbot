﻿using System;
using System.Text;
using System.Text.RegularExpressions;

using Google.GData.Client;
using HtmlAgilityPack;

namespace net_rewrite
{
	/// <summary>
	/// Description of WebsiteManager.
	/// </summary>
	public class WebsiteManager
	{
		
		private readonly Regex reg;
		
		public WebsiteManager()
		{
			//Regex by Diego Perini: https://gist.github.com/dperini/729294
			//Modifiziert um urls auch ohne Protokoll zuzulassen
			this.reg = new Regex(@"(?:(?:https?|ftp):\/\/)?(?:\S+(?::\S*)?@)?(?:(?!10(?:\.\d{1,3}){3})(?!127(?:\.\d{1,3}){3})(?!169\.254(?:\.\d{1,3}){2})(?!192\.168(?:\.\d{1,3}){2})(?!172\.(?:1[6-9]|2\d|3[0-1])(?:\.\d{1,3}){2})(?:[1-9]\d?|1\d\d|2[01]\d|22[0-3])(?:\.(?:1?\d{1,2}|2[0-4]\d|25[0-5])){2}(?:\.(?:[1-9]\d?|1\d\d|2[0-4]\d|25[0-4]))|(?:(?:[a-z\u00a1-\uffff0-9]+-?)*[a-z\u00a1-\uffff0-9]+)(?:\.(?:[a-z\u00a1-\uffff0-9]+-?)*[a-z\u00a1-\uffff0-9]+)*(?:\.(?:[a-z\u00a1-\uffff]{2,})))(?::\d{2,5})?(?:\/[^\s]*)?");
		}
		
		public string isWebpage(string txt)
		{
            Match mt = this.reg.Match(txt);
            string url=null;
            while (mt.Success)
            {
            	url += mt.Value;
            	mt = mt.NextMatch();
            }
            return url;
		}
		
		public string getPageTitle(string url)
		{
			var webGet = new HtmlWeb();
			HtmlDocument doc;
			try
			{
				doc = webGet.Load(url);
			}
			catch (System.UriFormatException)
			{
				doc = webGet.Load("http://" + url);						
			}
			if (doc != null)
			{
				HtmlNodeCollection metaTags = doc.DocumentNode.SelectNodes("//title");
			    if (metaTags != null)
			    {
			    	return HttpUtility.HtmlDecode(metaTags[0].InnerText).Replace("\r\n", "");
			    }			
			}
			return null;			
		}
	}
}
