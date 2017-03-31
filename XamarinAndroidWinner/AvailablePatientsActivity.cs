using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Hackathon.NetworkUtils;
using Java.Lang;
using Newtonsoft.Json;

namespace Hackathon
{
    [Activity ( Label = "Available Patients" )]
    public class AvailablePatientsActivity : Activity, IDialogInterfaceOnDismissListener
    {
        private static List<Patient> _patients;
        private TextView tvTest;
        private ProgressBar pbLoadingPokemon;
        private static PatientsAdapter adapter;
        private Button mBtnRegisterPatient;
        private ImageButton mImgButtonPatientsToMonitor;
        List<Patient> dummyList;
        private ProgressBar mGettingPatientsProgressBar;

        protected override void OnCreate ( Bundle savedInstanceState )
        {
            base.OnCreate ( savedInstanceState );

            
            // Create your application here
            SetContentView ( Resource.Layout.available_patients_layout );

            //List<Patient> dummyList = new List<Patient> ();





            //Patient dummyPatient2 = new Patient ();
            //dummyPatient2.name = "Ethan Bradburry";
            //dummyPatient2.id = "2";

            //Patient dummyPatient3 = new Patient ();
            //dummyPatient3.name = "Mark Something";
            //dummyPatient3.id = "3";

            ////dummyList.Add ( dummyPatient );
            //dummyList.Add ( dummyPatient2 );
            //dummyList.Add ( dummyPatient3 );

            //adapter = new PatientsAdapter ( this, dummyList );

            //ListView lvPatients = FindViewById<ListView> ( Resource.Id.lv_available_patients );

            //lvPatients.Adapter = adapter;

            mGettingPatientsProgressBar = FindViewById<ProgressBar> ( Resource.Id.pb_getting_patients );
            
            getPatientsAsync ();

        }

        public override bool OnOptionsItemSelected ( IMenuItem item )
        {
            var transaction = FragmentManager.BeginTransaction ();
            var dialogFragment = new AddPatientDialog();
            dialogFragment.Show ( transaction, "dialog_fragment" );
            return true;
        }



        public override bool OnCreateOptionsMenu ( IMenu menu )
        {

            MenuInflater.Inflate ( Resource.Menu.main_menu, menu );

            return base.OnCreateOptionsMenu ( menu );
        }

        public async Task getPatientsAsync ()
        {

            mGettingPatientsProgressBar.Indeterminate = true;
            mGettingPatientsProgressBar.Visibility = ViewStates.Visible;
            // this method should get the patients from Mo's endpoint
            HttpClient client = PatientsNetworkUtils.GetClient ( PatientsNetworkUtils.CONTROLLER_BASE_ADDRESS );

            string response = await client.GetStringAsync ( "" );
            

           

            

            var patients = JsonConvert.DeserializeObject<IEnumerable<Patient>> ( response ).ToList ();
            _patients = patients;
            adapter = new PatientsAdapter ( this, _patients);

            ListView lvPatients = FindViewById<ListView> ( Resource.Id.lv_patient_list );
            lvPatients.ItemClick += ShowPatientDetails;
            lvPatients.Adapter = adapter;


        }

        private void ShowPatientDetails ( object sender, AdapterView.ItemClickEventArgs e )
        {
            Patient clickedPatient = _patients [ e.Position ];
            Intent showPatientDetails = new Intent ( this, typeof(PatientDetailsActivity) );
            string patientJson = JsonConvert.SerializeObject ( clickedPatient );
            showPatientDetails.PutExtra ( "patient", patientJson );
            StartActivity ( showPatientDetails );
        }

        public static async void notifyPatientListChanged ()
        {
            HttpClient client = PatientsNetworkUtils.GetClient ( PatientsNetworkUtils.CONTROLLER_BASE_ADDRESS );

            string response = await client.GetStringAsync ( "" );


            // dummy patient to test
            Patient dummyPatient = new Patient ();
            dummyPatient.name = "Chris Billington";
            dummyPatient.id = "1";



            var patients = JsonConvert.DeserializeObject<IEnumerable<Patient>> ( response ).ToList ();
            _patients = patients;
            
            
        }

        void IDialogInterfaceOnDismissListener.OnDismiss ( IDialogInterface dialog )
        {
            adapter.NotifyDataSetChanged();
        }

        public class PatientsAdapter : BaseAdapter
        {
            private List<Patient> _patients;
            private Activity _context;

            // constructor for the adapter takes in a list view from.
            public PatientsAdapter ( Context context, List<Patient> items )
            {
                _context = (Activity)context;
                _patients = items;

            }



            public override long GetItemId ( int position )
            {
                return position;
            }

            /**
             * 
             * This method is responsible for inflating the corrent view object. It takes in a position in the list where the data lies, the view context, and a parent viewgroup (not sure what this does?) it then creates a view object from the data and returns it.
             * 
             */
            public override View GetView ( int position, View convertView, ViewGroup parent )
            {
                Patient currentPatient = _patients [ position ];
                // inflate the list item layout.
                View patientView = _context.LayoutInflater.Inflate ( Resource.Layout.lviPatient, parent, false );

                // get a reference to the objects inside the layout (in our case just a textview).
                var patientName = patientView.FindViewById<TextView> ( Resource.Id.tv_name);
                var togglePatientIcon = patientView.FindViewById<Switch> ( Resource.Id.sw_monitor );
                var moreInfoIcon = patientView.FindViewById<ImageView> ( Resource.Id.iv_info );
                var heartBeat = patientView.FindViewById<TextView> ( Resource.Id.tv_quick_bpm );
                togglePatientIcon.Checked = currentPatient.monitor;
                //var infoIcon = patientView.FindViewById<ImageView> ( Resource.Id.iv_left_patient_info );
                //var addIcon = patientView.FindViewById<ImageView> ( Resource.Id.iv_add_patient_to_monitor );
                // set the data inside the view object from our layout.
                patientName.Text = currentPatient.name;
                


                togglePatientIcon.Click+= delegate
                {
                    if ( !currentPatient.monitor )
                    {
                        currentPatient.monitor = true;
                        MainActivity.setPatientMonitor ( currentPatient );
                        Toast.MakeText ( _context, "Patient successfully being monitored", ToastLength.Short ).Show ();
                    }

                    else
                    {
                        currentPatient.monitor = false;
                        MainActivity.unsetPatientMonitor ( currentPatient );
                        Toast.MakeText ( _context, "Patient removed from monitoring", ToastLength.Short ).Show ();
                    }

                    MainActivity.updatePatientDetails ( currentPatient );
                    
                };

                moreInfoIcon.Click += delegate
                {
                    Patient clickedPatient = currentPatient;
                    Intent showPatientDetails = new Intent ( _context, typeof ( PatientDetailsActivity ) );
                    string patientJson = JsonConvert.SerializeObject ( clickedPatient );
                    showPatientDetails.PutExtra ( "patient", patientJson );
                    _context.StartActivity ( showPatientDetails );
                    MainActivity.setPatientMonitor ( currentPatient );
                };

                return patientView;

            }

            public override Java.Lang.Object GetItem ( int position )
            {
                return null;
            }

            public override int Count => _patients.Count;


        }

        
    
    }
}