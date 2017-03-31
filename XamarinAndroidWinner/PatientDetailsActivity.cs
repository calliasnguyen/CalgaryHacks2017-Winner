using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Hackathon.NetworkUtils;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace Hackathon
{
    [Activity ( Label = "PatientDetails" )]
    public class PatientDetailsActivity : Activity
    {
        //private TextView tvPatientId;
        //private decimal lastKnownHeartbeat;
        private TextView tvPatientHeartbeat;
        private Service1Connection democon;
        private Patient _patientPassed;
        private bool _binderSet = true;
        private ImageButton ibCurrentLocation;
        //private View ibLocationImageButton;
       // private ImageButton ibDeviceImageButton;
        private EditText etPatientName;
        
        private ImageButton ibEmergencyContact;
        private Switch swToggleHeartRateMonitor;
        private EditText etHighHr;
        private EditText etLowHr;
        private ImageButton ib_edit_hrrange;
        private ImageButton ib_save_hrrange;
        private EditText etPatientAge;

        protected override void OnCreate ( Bundle savedInstanceState )
        {
            base.OnCreate ( savedInstanceState );

            SetContentView(Resource.Layout.PatientDetails);

            string patientToDeserialize = Intent.GetStringExtra ( "patient" );

            _patientPassed =
                JsonConvert.DeserializeObject<Patient> ( patientToDeserialize );

            //ibDeviceImageButton = FindViewById<ImageButton> ( Resource.Id.ib_device );


          //  etPatientName = FindViewById<EditText>(Resource.Id.et_name);
            //tvPatientId = FindViewById<TextView>(Resource.Id.tv_id);
           // etPatientAge = FindViewById<EditText>(Resource.Id.et_age);
            ibEmergencyContact = FindViewById<ImageButton> ( Resource.Id.ib_emerg );
            swToggleHeartRateMonitor = FindViewById<Switch> ( Resource.Id.sw_monitor_hr );
          //  etHighHr = FindViewById<EditText> ( Resource.Id.et_highHr );
          //  etLowHr = FindViewById<EditText> ( Resource.Id.et_lowHr );
            tvPatientHeartbeat = FindViewById<TextView> ( Resource.Id.tv_current_heartbeat );
            ib_edit_hrrange = FindViewById<ImageButton> ( Resource.Id.ib_edit_hrrange );
            ib_save_hrrange = FindViewById<ImageButton> ( Resource.Id.ib_save_hrrange );
            ib_save_hrrange.Visibility = ViewStates.Gone;

            ibCurrentLocation = FindViewById<ImageButton> ( Resource.Id.ib_location );

          //  etPatientName.Text = _patientPassed.name;
          //  etPatientAge.Text = "" + _patientPassed.age;
            
         //   etHighHr.Text = "" + _patientPassed.highHr;
         //   etLowHr.Text = "" + _patientPassed.lowHr;


            //var ib_device = (ImageButton)FindViewById ( Resource.Id.ib_device );
            //var et_name = (EditText)FindViewById ( Resource.Id.et_name );
            //var et_age = (EditText)FindViewById ( Resource.Id.et_age );
            //var ib_emerg = (ImageButton)FindViewById ( Resource.Id.ib_emerg );
            //var tv_created = (TextView)FindViewById ( Resource.Id.tv_created );
            //var sw_monitor_hr = (Switch)FindViewById ( Resource.Id.sw_monitor_hr );
            //var et_highHr = (EditText)FindViewById ( Resource.Id.et_highHr );
            //var et_lowHr = (EditText)FindViewById ( Resource.Id.et_lowHr );

            ibEmergencyContact.Click += delegate
            {
                    
                    var uri = Android.Net.Uri.Parse ( "tel:" +  _patientPassed.emergPhone);
                    var intent = new Intent ( Intent.ActionDial, uri );
                    StartActivity ( intent );
                
            };

            ibCurrentLocation.Click += delegate
            {
                //var locationString = String.Format ( "geo:{lat},{lon},{lon}({message})", _patientPassed.latitude, _patientPassed.longitude, _patientPassed.name );

                string coord = _patientPassed.latitude + "," + _patientPassed.longitude;
                var geoUri = Android.Net.Uri.Parse ( "geo:" + coord + "?q=" + coord + "(" + _patientPassed.name + ")" );

                var mapIntent = new Intent ( Intent.ActionView, geoUri );
                StartActivity ( mapIntent );
            };

            //ibDeviceImageButton.Click += delegate
            //{
            //    View view = LayoutInflater.Inflate ( Resource.Layout.dialog_device, null );
            //    AlertDialog builder = new AlertDialog.Builder ( this ).Create ();
            //    builder.SetView ( view );
            //    builder.SetCanceledOnTouchOutside ( true );
            //    //TextView tv_device_id = view.FindViewById<TextView>()
            //};

            //ib_save_hrrange.Click += delegate
            //{
            //    _patientPassed.highHr = Convert.ToInt32 ( etHighHr.Text );
            //    _patientPassed.lowHr = Convert.ToInt32 ( etLowHr.Text );
            //    UpdatePatient ( _patientPassed );

            //    etHighHr.Enabled = false;
            //    etLowHr.Enabled = false;
            //    ib_save_hrrange.Visibility = ViewStates.Gone;
            //    ib_edit_hrrange.Visibility = ViewStates.Visible;
            //};

            //ib_edit_hrrange.Click += delegate
            //{
            //    ib_save_hrrange.Visibility = ViewStates.Visible;
            //    ib_edit_hrrange.Visibility = ViewStates.Gone;
            //    etHighHr.Enabled = true;
            //    etLowHr.Enabled = true;
            //};

            getVitalsAsync (_patientPassed);
        }

        private  async void UpdatePatient ( Patient _patientPassed )
        {
            var JSONpatient = JsonConvert.SerializeObject ( _patientPassed );

            HttpClient htc = new HttpClient ();
            htc.DefaultRequestHeaders.Accept.Add ( new MediaTypeWithQualityHeaderValue ( "application/json" ) );

            HttpRequestMessage hrm = new HttpRequestMessage ( HttpMethod.Post, "" );
            hrm.Content = new StringContent ( JSONpatient, Encoding.UTF8, "application/json" );

            HttpResponseMessage response = await htc.PostAsync ( "https://aa798a67.ngrok.io/api/Patients", hrm.Content );
        }

        private void setButtonTextFromService ()
        {
            if ( democon == null || democon.Binder == null )
            {
                return;
            }
            if ( _binderSet )
            {
                democon.Binder.setMonitoredPatient ( _patientPassed );
                _binderSet = false;
            }

            else
            {
                int testGetBPM = democon.Binder.GetBPM ();

                RunOnUiThread ( () =>
                {

                    tvPatientHeartbeat.Text = testGetBPM.ToString ();
                } );
            }
        }

        protected override void OnStart ()
        {
            base.OnStart ();


            democon = new Service1Connection ();
            var demoServiceIntent = new Intent ( this, typeof ( Service1 ) );
            BindService ( demoServiceIntent, democon, Bind.AutoCreate );

            

            var timer = new System.Threading.Timer (
                e => setButtonTextFromService (),
                    null,
                    TimeSpan.Zero,
                    TimeSpan.FromSeconds ( 5 ) );

        }

        public async void getVitalsAsync(Patient patient)
        {
            string patientVitalsUrl = PatientsNetworkUtils.CONTROLLER_BASE_ADDRESS + "/Readings?device=" + patient.id;

            HttpClient client = PatientsNetworkUtils.GetClient ( patientVitalsUrl );

            string response = await client.GetStringAsync ( "" );

            var vitals = JsonConvert.DeserializeObject<IEnumerable<Vital>> ( response ).ToList ();
            if (vitals.Count == 0 )
            {
                tvPatientHeartbeat.Text = "Not Found";
            }
            else
            {
                tvPatientHeartbeat.Text = vitals [ 0 ].value.ToString ();
            }
        }
    }


    public class Vital
    {
        public string meaning { get; set; }
        public int value { get; set; }
        public object received { get; set; }
    }

    
}