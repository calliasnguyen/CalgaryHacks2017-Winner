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
using System.Net.Http;
using System.Net.Http.Headers;

namespace Hackathon.NetworkUtils
{
    public static class PatientsNetworkUtils
    {
        public static string CONTROLLER_BASE_ADDRESS = "https://aa798a67.ngrok.io/api/Patients";
        public static string POKEMON_TESTER = "http://pokeapi.co/api/v2/pokemon/charizard";

        public static HttpClient GetClient(string address)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(address);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            //adding authentication
            

            return client;
        }
    }
}