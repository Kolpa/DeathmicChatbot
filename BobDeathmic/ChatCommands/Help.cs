﻿using BobDeathmic.ChatCommands.Setup;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BobDeathmic.ChatCommands
{
    public class Help : IfCommand
    {
        public string Trigger { get { return "!help"; } }

        public string Description { get { return "Listet Commands auf"; } }

        public string Category { get { return "General"; } }
        private string sCommandMessage { get; set; }

        public async Task<CommandEventType> EventToBeTriggered(Dictionary<string, string> args)
        {
            return CommandEventType.None;
        }

        public async Task<string> ExecuteCommandIfApplicable(Dictionary<String, String> args, IServiceScopeFactory scopeFactory)
        {
            if (args["message"].ToLower().StartsWith(Trigger))
            {
                return sCommandMessage;
            }
            return string.Empty;
        }

        public async Task<string> ExecuteWhisperCommandIfApplicable(Dictionary<string, string> args, IServiceScopeFactory scopeFactory)
        {
            return string.Empty;
        }

        public void PopulateCommandList(string provider = "discord")
        {
            //Should ever only be called once (Per Help Command init)
            if (sCommandMessage == string.Empty || sCommandMessage == null)
            {
                string currentCategory = string.Empty;
                sCommandMessage = "Viele Befehle haben einen help parameter (!befehl help)" + Environment.NewLine;

                //Manual Implementation. Would otherwise need redundant rework of Commands all holding reference to UserManager ...
                sCommandMessage += Environment.NewLine;
                sCommandMessage += Environment.NewLine;
                sCommandMessage += $"[WebInterface]{Environment.NewLine}";
                sCommandMessage += "!WebInterfaceLink (!wil) : Gibt den Link zum Webinterface zurück" + Environment.NewLine;
                foreach (IfCommand tempcommand in CommandBuilder.BuildCommands(provider, true))
                {
                    if (currentCategory != tempcommand.Category)
                    {
                        sCommandMessage += Environment.NewLine;
                        sCommandMessage += Environment.NewLine;
                        currentCategory = tempcommand.Category;
                        sCommandMessage += $"[{tempcommand.Category}]{Environment.NewLine}";
                    }
                    sCommandMessage += tempcommand.Trigger + " : " + tempcommand.Description + Environment.NewLine;
                }
            }
        }
    }
}