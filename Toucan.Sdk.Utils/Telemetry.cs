//using System.Diagnostics;
//using System.Runtime.CompilerServices;

//namespace Toucan.Sdk.Utils;

//public static class Telemetry
//{
//    private static readonly ActivitySource Activities = new(nameof(Toucan));

//    public static Activity? Begin([CallerMemberName] string? name = null) => Activities.StartSubActivity(Guard.NotNullOrEmpty(name));
//    public static Activity? StartActivity(string name) => Activities.StartSubActivity(name);

//    public static Activity? StartSubActivity(this ActivitySource activity, string name)
//    {
//        if (Activity.Current == null)
//            return null;

//        return activity?.StartActivity(name);
//    }
//}

