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

namespace Hackathon
{
    public class Service1Binder:Binder,IGetBPM
    {

        public Service1 Service { get; private set; }
        public Service1Binder(Service1 service)
        {
            this.Service = service;
        }
        public int GetBPM()
        {
            return Service.GetBPM();
        }
        public void setMonitoredPatient (Patient patient)
        {
            Service.setMonitoredPatient (patient);
        }
    }
}