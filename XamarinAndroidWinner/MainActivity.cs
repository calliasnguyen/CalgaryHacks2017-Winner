using Android.App;
using Android.Widget;
using Android.OS;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using Hackathon.NetworkUtils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Views;

using Object = Java.Lang.Object;
using Java.Lang;
using Com.OneSignal;
using Org.Json;
using String = Java.Lang.String;
using Android.Locations;
using Android.Util;
using Android.Runtime;
using Android;
using Plugin.Geolocator;
using System.Net.Http.Headers;
using System.Text;

namespace Hackathon
{
    [Activity(Label = "Monitors", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private List<Patient> _patients;
        //private TextView tvTest;
        //private ProgressBar pbLoadingPokemon;
        //private PatientsAdapter adapter;
        //private Button mBtnRegisterPatient;
        //private ImageButton mImgButtonPatientsToMonitor;

        public static AvailablePatientsActivity.PatientsAdapter adapter;
        public static List<Patient> mMonitoredPatients;
        private Button mCheckIfDataTransferedButton;
        private Button mCallServiceMethodButton;
        public static LinearLayout mWarningNoMonitoredPatientsMessage;
        public static Service1Connection democon;
        private LocationManager _locationManager;
        private string _locationProvider;
        public static Location _currentLocation;
        readonly string [] PermissionsLocation =
        {
          Manifest.Permission.AccessCoarseLocation,
          Manifest.Permission.AccessFineLocation
        };
        public static List<Patient> getMonitoredPatients()
        {
            return mMonitoredPatients;
        }

        public static void setPatientMonitor( Patient patientToMonitor )
        {
            mMonitoredPatients.Add(patientToMonitor);
            adapter.NotifyDataSetChanged ();
            
            mWarningNoMonitoredPatientsMessage.Visibility = ViewStates.Gone;
        }

        public static async void updatePatientDetails (Patient patientToUpdate)
        {
            string patientPayload = JsonConvert.SerializeObject ( patientToUpdate );

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
            
            
            request.Content = new StringContent ( patientPayload,
                Encoding.UTF8,
                "application/json" );
            var response = await httpClient.SendAsync ( request );

            var responseMessage = await response.Content.ReadAsStringAsync ();
            //string responseMessage = response.Content.ToString();
        }

        public static void unsetPatientMonitor(Patient patientToRemove )
        {
            var foundPatientToRemove = mMonitoredPatients.Find ( p => p.id == patientToRemove.id );

            if (foundPatientToRemove != null )
            {
                mMonitoredPatients.Remove ( foundPatientToRemove );
            }

            adapter.NotifyDataSetChanged ();
        }
        protected override void OnStart ()
        {
            base.OnStart ();

            
            democon = new Service1Connection ();
            var demoServiceIntent = new Intent ( this, typeof ( Service1 ) );
            BindService ( demoServiceIntent, democon, Bind.AutoCreate );
            


        }
        public static void setPatientServiceListener ()
        {
            democon.Binder.setMonitoredPatient (mMonitoredPatients[0]);
        }

        public static async Task<Tuple<string,string>> GetUsersLocation ()
        {
            var locater = CrossGeolocator.Current;
            locater.DesiredAccuracy = 100;
            var position = await locater.GetPositionAsync ( 20000 );
            string lat = position.Latitude.ToString ();
            string lon = position.Longitude.ToString ();

            return Tuple.Create ( lat, lon );

        }
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            mMonitoredPatients = new List<Patient> ();

            OneSignal.StartInit ( "865ae748-d04d-47a0-ac8a-595a25999f5f", "560248917441" )
                .HandleNotificationOpened ( HandleNotificationOpened )
                //.InFocusDisplaying(setMessageToDialog)
                .HandleNotificationReceived(delegateReceived)
                .EndInit ();
            mCallServiceMethodButton = FindViewById<Button> ( Resource.Id.btn_test_service );
            //mCallServiceMethodButton.Click += async delegate
            //{


            //};


            //var timer = new System.Threading.Timer (
            //    e => setButtonTextFromService (),
            //        null,
            //        TimeSpan.Zero,
            //        TimeSpan.FromSeconds ( 5 ) );

            mWarningNoMonitoredPatientsMessage = FindViewById<LinearLayout> ( Resource.Id.ll_warning_message );
            //mCheckIfDataTransferedButton = FindViewById<Button> ( Resource.Id.btn_check );
            //mCheckIfDataTransferedButton.Click += showToast;
            //mBtnRegisterPatient.Click += ShowRegisterPatientDialog;
            //getPatientsAsync ();

            // if there are no monitored patients, then show a warning message telling them to add patients to be monitored.
            

            if (mMonitoredPatients.Count == 0 )
            {
                mWarningNoMonitoredPatientsMessage.Visibility = ViewStates.Visible;
                adapter = new AvailablePatientsActivity.PatientsAdapter ( this, mMonitoredPatients );

                ListView lvPatients = FindViewById<ListView> ( Resource.Id.lv_patients );

                lvPatients.Adapter = adapter;
            }
            getMonitoredPatientsFromNetwork ();
            RequestPermissions ( PermissionsLocation, 0 );
            InitializeLocationManager ();

        }

        private void delegateReceived ( OSNotification notification )
        {
            //notification.
        }

        private async void getMonitoredPatientsFromNetwork ()
        {
            HttpClient client = PatientsNetworkUtils.GetClient ( PatientsNetworkUtils.CONTROLLER_BASE_ADDRESS );

            string response = await client.GetStringAsync ( "" );

            var patients = JsonConvert.DeserializeObject<IEnumerable<Patient>> ( response ).ToList ();
            mMonitoredPatients = patients;



            adapter.NotifyDataSetChanged();
        }

        void InitializeLocationManager ()
        {
            _locationManager = (LocationManager)GetSystemService ( LocationService );
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders ( criteriaForLocationService, true );

            if ( acceptableLocationProviders.Any () )
            {
                _locationProvider = acceptableLocationProviders.First ();
            }
            else
            {
                _locationProvider = string.Empty;
            }
            
        }

        

        

        //private void setButtonTextFromService ()
        //{
        //    if(democon == null )
        //    {
        //        return;
        //    }

        //    int testGetBPM = democon.Binder.GetBPM ();

        //    RunOnUiThread ( () =>
        //      {
        //          mMonitoredPatients [ 0 ].HeartBeat = testGetBPM;
        //          adapter.NotifyDataSetChanged ();
        //      } );
            
            
        //}

        //private void showToast ( object sender, EventArgs e )
        //{
        //    var patients = getMonitoredPatients ();
        //    Toast.MakeText ( this, patients["12445"].name, ToastLength.Long ).Show();
        //}

        //protected void OnResume (Bundle bundle)
        //{
        //    Toast.MakeText ( this, mMonitoredPatients [ "12445" ].name , ToastLength.Short);
        //}

        //private void ShowRegisterPatientDialog(object sender, EventArgs e)
        //{
        //    var transaction = FragmentManager.BeginTransaction ();
        //    var dialogFragment = new AddPatientDialog();
        //    dialogFragment.Show ( transaction, "dialog_fragment" );
        //}

        private void HandleNotificationOpened(OSNotificationOpenedResult result)
        {
            OSNotificationPayload payload = result.notification.payload;

            string test = payload.body;
            payload.title = "hi";
            Dictionary<string, object> jsonData = payload.additionalData;
            String data =  (String)jsonData["id"];
            string patientId = data.ToString ();

            var foundPatient = mMonitoredPatients.Find ( p => p.id == patientId );

            Intent showPatientDetails = new Intent ( this, typeof ( PatientDetailsActivity ) );
            string patientJson = JsonConvert.SerializeObject ( foundPatient );
            showPatientDetails.PutExtra ( "patient", patientJson );
            this.StartActivity ( showPatientDetails );
            //string data = jsonObj.GetString("abc");

        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.main_menu, menu);
            
            return base.OnCreateOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            Intent showAvailablePatientsActivity = new Intent(this, typeof(AvailablePatientsActivity));
            StartActivity(showAvailablePatientsActivity);
            return true;
        }

        //public async Task getPatientsAsync()
        //{
        //    // this method should get the patients from Mo's endpoint
        //    HttpClient client = PatientsNetworkUtils.GetClient(PatientsNetworkUtils.CONTROLLER_BASE_ADDRESS);

        //    string response = await client.GetStringAsync("");


            
            
        //    var patients = JsonConvert.DeserializeObject<IEnumerable<Patient>>(response).ToList();
        //    _patients = patients;
        //    adapter = new PatientsAdapter ( this, patients );

        //    ListView lvPatients = FindViewById<ListView> ( Resource.Id.lv_patients );
            
        //    lvPatients.Adapter = adapter;
        //    lvPatients.ItemClick += ShowPatientDetails;

        //}

        private void ShowPatientDetails(object sender, AdapterView.ItemClickEventArgs e)
        {
            // got the patient that was selected
            Patient clickedPatient = _patients[ e.Position];

            // create an intent with the patient
            Intent showPatientDetailsActivity = new Intent(this, typeof(PatientDetailsActivity) );

            string patientJson = JsonConvert.SerializeObject(clickedPatient);

            showPatientDetailsActivity.PutExtra("patient", patientJson);

            StartActivity(showPatientDetailsActivity);

        }

        
        //protected override void OnResume ()
        //{
        //    if(CheckSelfPermission(permission[1]) == (int)Permission.Granted)
        //    base.OnResume ();
        //    _locationManager.RequestLocationUpdates ( _locationProvider, 0, 0, this );
        //}

        //protected override void OnPause ()
        //{
        //    base.OnPause ();
        //    _locationManager.RemoveUpdates ( this );
        //}





        //public class PatientsAdapter : BaseAdapter
        //{
        //    private List<Patient> _patients;
        //    private Activity _context;

        //    // constructor for the adapter takes in a list view from.
        //    public PatientsAdapter ( Context context, List<Patient> items )
        //    {
        //        _context = (Activity)context;
        //        _patients = items;

        //    }



        //    public override long GetItemId ( int position )
        //    {
        //        return position;
        //    }

        //    /**
        //     * 
        //     * This method is responsible for inflating the corrent view object. It takes in a position in the list where the data lies, the view context, and a parent viewgroup (not sure what this does?) it then creates a view object from the data and returns it.
        //     * 
        //     */
        //    public override View GetView ( int position, View convertView, ViewGroup parent )
        //    {
        //        Patient currentPatient = _patients[position];
        //        // inflate the list item layout.
        //        View patientView = _context.LayoutInflater.Inflate ( Resource.Layout.patient_list_item, parent, false );

        //        // get a reference to the objects inside the layout (in our case just a textview).
        //        var patientName = patientView.FindViewById<TextView> ( Resource.Id.tv_patient_name );

        //        // set the data inside the view object from our layout.
        //        patientName.Text = currentPatient.name;

        //        return patientView;

        //    }

        //    public override Object GetItem ( int position )
        //    {
        //        return null;
        //    }

        //    public override int Count => _patients.Count;


        //}

    }
}

