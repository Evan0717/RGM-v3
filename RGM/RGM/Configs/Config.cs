using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Interfaces;
using RGM.Modes;

namespace RGM
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        public string WebhookURL { get; set; } = "https://discord.com/api/webhooks/1281967055272153099/b4-Qw3t5V4Tliq6axCheK0Ekv7_XdZ25MhbVMbepF_7WPz8RwsXw_c-3S58Cx3c8hj24";
        public string BotAPIServer { get; set; } = "http://127.0.0.1:50000/";
    }
}
