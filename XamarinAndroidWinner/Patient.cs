using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace Hackathon
{
    public class Patient
    {
        
        public string id { get; set; }
        [JsonProperty ( "name" )]
        public string name { get; set; }
        public string created { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public int age { get; set; }
        public string emergPhone { get; set; }
        public int HeartBeat { get; set; }
        public bool monitor { get; set; }
        public int highHr { get; set; }
        public int lowHr { get; set; }
    }
}