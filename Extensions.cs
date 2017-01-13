using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Nancy.Rest.Client
{
    public static class Extensions
    {
        public static dynamic Execute(this RestClient cl, object req, Type objtype)
        {
            Func<IRestRequest, IRestResponse<dynamic>> func=cl.Execute<dynamic>;
            return (dynamic) func.Method.GetGenericMethodDefinition().MakeGenericMethod(objtype).Invoke(cl, new object[] {req});
        }
        internal static bool IsAsyncMethod(this MethodInfo minfo)
        {
            return (minfo.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null);
        }
    }
}
