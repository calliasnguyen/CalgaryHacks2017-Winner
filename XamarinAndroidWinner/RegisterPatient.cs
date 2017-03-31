using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Hackathon.NetworkUtils;

namespace Hackathon
{
    [Activity ( Label = "RegisterPatient" )]
    public class RegisterPatient : Activity
    {
        private Button mMonitorPatientButton;
        private EditText mPatientNameEditText;

        protected override void OnCreate ( Bundle savedInstanceState )
        {
            base.OnCreate ( savedInstanceState );

            // Create your application here
            SetContentView(Resource.Layout.register_patient_layout);

            // get a reference to the edit text
            mPatientNameEditText = FindViewById<EditText>(Resource.Id.et_patient_name);
            
            // get a reference to the button
            mMonitorPatientButton = FindViewById<Button>(Resource.Id.btn_register_patient);

            // set event handler for the button
            mMonitorPatientButton.Click += MonitorPatient;
        }

        private void MonitorPatient(object sender, EventArgs e)
        {
            // this method should capture what is currently in the edit text and make a post call to mo to register a patient as monitored

            // get the text inside edit text
            string patientName = mPatientNameEditText.Text;

            registerPatientAsync(patientName);
        }

        public async void registerPatientAsync(string patientName)
        {
            // creat ethe url string

            
            string uri = PatientsNetworkUtils.CONTROLLER_BASE_ADDRESS + "/";
            var httpClient = new HttpClient ();
            httpClient.DefaultRequestHeaders.Accept.Add (
            new MediaTypeWithQualityHeaderValue ( "application/json" ) );
            httpClient.BaseAddress = new Uri(uri);
            /*var postContent = new FormUrlEncodedContent ( new []
            {
                new KeyValuePair<string, string>("name", patientName)
                
            } );*/

            HttpRequestMessage request = new HttpRequestMessage ( HttpMethod.Post, "" );
            request.Content = new StringContent("{\"name\":\"" + patientName + "\"}",
                Encoding.UTF8,
                "application/json");
            var response = httpClient.SendAsync(request).Result;

            

            response.EnsureSuccessStatusCode ();


            string content = await response.Content.ReadAsStringAsync ();

            Toast.MakeText(this, content, ToastLength.Short);
        }

    }
}
