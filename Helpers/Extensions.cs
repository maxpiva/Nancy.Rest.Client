using System.Reflection;
using System.Runtime.CompilerServices;
using Nancy.Rest.Annotations.Enums;
using RestSharp;

namespace Nancy.Rest.Client.Helpers
{
    internal static class Extensions
    {

        public static bool IsAsyncMethod(this MethodInfo minfo)
        {
            return (minfo.GetCustomAttribute(typeof(AsyncStateMachineAttribute)) != null);
        }
        public static Method ToMethod(this Verbs verb)
        {
            switch (verb)
            {
                case Verbs.Get:
                    return Method.GET;
                case Verbs.Delete:
                    return Method.DELETE;
                case Verbs.Head:
                    return Method.HEAD;
                case Verbs.Options:
                    return Method.OPTIONS;
                case Verbs.Patch:
                    return Method.PATCH;
                case Verbs.Post:
                    return Method.POST;
                case Verbs.Put:
                    return Method.PUT;
            }
            return Method.GET;
        }

    }
}
