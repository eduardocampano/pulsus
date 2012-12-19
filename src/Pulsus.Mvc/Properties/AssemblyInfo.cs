using System.Runtime.InteropServices;
using System.Web;
using Pulsus.Mvc;

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("20f583d7-2155-4442-8f44-28ef0e8e56c4")]

[assembly: PreApplicationStartMethod(typeof(AppStart), "Start")]
