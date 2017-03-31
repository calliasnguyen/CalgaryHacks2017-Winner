using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Hackathon.NetworkUtils;
using Newtonsoft.Json;
using Android.Locations;

namespace Hackathon
{
    public class AddPatientDialog : DialogFragment
    {
        private Button mMonitorPatientButton;
        private EditText mPatientNameEditText;
        private EditText mDeviceIdEditText;
        private EditText mPatientAgeEditText;
        private EditText mPatientEmergPhoneEditText;

        public override void OnCreate ( Bundle savedInstanceState )
        {
            base.OnCreate ( savedInstanceState );

            // Create your fragment here
            
        }

        public override View OnCreateView ( LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState )
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            var view = inflater.Inflate(Resource.Layout.register_patient_layout, container, false);
            mMonitorPatientButton = view.FindViewById<Button>(Resource.Id.btn_register_patient);
            mPatientNameEditText= view.FindViewById<EditText>(Resource.Id.et_patient_name);
            
            mPatientAgeEditText = view.FindViewById<EditText> ( Resource.Id.et_age );
            mPatientEmergPhoneEditText = view.FindViewById<EditText> ( Resource.Id.et_emergPhone );

            mMonitorPatientButton.Click += MonitorPatient;

            return view;
        }
        private async void MonitorPatient ( object sender, EventArgs e )
        {
            //this method should capture what is currently in the edit text and make a post call to mo to register a patient as monitored
             //here we need to access the location manager and get the persons location.


             //get the text inside edit text
            string patientName = mPatientNameEditText.Text;
            

            var location = await MainActivity.GetUsersLocation ();
            string lat = location.Item1;
            string lon = location.Item2;
            mMonitorPatientButton.Text = lat.ToString ();
            Patient patientToSend = new Patient ();
            patientToSend.name = patientName;
            patientToSend.latitude = lat;
            patientToSend.longitude = lon;
            patientToSend.age = Convert.ToInt32(mPatientAgeEditText.Text);
            patientToSend.emergPhone = mPatientEmergPhoneEditText.Text;
            patientToSend.monitor = true;
            registerPatientAsync ( patientToSend );
        }
        public async void registerPatientAsync ( Patient patientToSend )
        {
            // creat ethe url string


            string uri = PatientsNetworkUtils.CONTROLLER_BASE_ADDRESS + "/";
            var httpClient = new HttpClient ();
            httpClient.DefaultRequestHeaders.Accept.Add (
            new MediaTypeWithQualityHeaderValue ( "application/json" ) );
            httpClient.BaseAddress = new Uri ( uri );
            /*var postContent = new FormUrlEncodedContent ( new []
            {
                new KeyValuePair<string, string>("name", patientName)
                
            } );*/

            HttpRequestMessage request = new HttpRequestMessage ( HttpMethod.Post, "" );
            string patientJson = JsonConvert.SerializeObject ( patientToSend );
            Log.Info ( "JSON", patientJson );
            request.Content = new StringContent ( patientJson,
                Encoding.UTF8,
                "application/json" );
            var response = httpClient.SendAsync ( request ).Result;



            response.EnsureSuccessStatusCode ();


            string content = await response.Content.ReadAsStringAsync ();

            AvailablePatientsActivity.notifyPatientListChanged ();
            Dismiss ();

        }

        public override void OnDismiss ( IDialogInterface dialog )
        {
            base.OnDismiss ( dialog );
            Activity activity = this.Activity;
            ( (IDialogInterfaceOnDismissListener)activity ).OnDismiss ( dialog );
        }

    }
}
