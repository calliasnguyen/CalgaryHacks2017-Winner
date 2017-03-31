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

namespace Hackathon.NetworkUtils
{
    public class PokemonWrapper
    {
        public List<Form> forms { get; set; }
    }

    public class Form
    {
        public string url { get; set; }
        public string name { get; set; }
    }
}