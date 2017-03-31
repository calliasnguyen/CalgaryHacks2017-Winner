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
    public class Service1Connection : Java.Lang.Object, IServiceConnection
    {
        public Service1Binder Binder { get; private set; }
        public bool IsConnected { get; private set; }
        public Service1Connection()
        {
            IsConnected = false;
            Binder = null;
        }
        public void OnServiceConnected(ComponentName name, IBinder binder)
        {
            Binder = binder as Service1Binder;
            IsConnected = Binder != null;
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            IsConnected = false;
            Binder = null;
        }
    }
}