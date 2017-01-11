using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;
using RestSharp;

namespace Nancy.Rest.Client
{
    public static class ClientFactory
    {
        public static T Create<T>(string path, T i) where T: class
        {
            dynamic exp = new ExpandoObject();
            foreach (MethodInfo m in typeof(T).GetMethods())
            {
                List<Annotations.Rest> rests = m.GetCustomAttributes<Annotations.Rest>().ToList();
                if (rests.Count > 0)
                {
                    MethodDefinition defs = new MethodDefinition();
                    defs.RestAttribute = rests[0];
                    defs.BasePath = path;
                    defs.Parameters = m.GetParameters().Select(a => new Tuple<string, Type>(a.Name, a.ParameterType)).ToArray();
                    defs.ReturnType = m.ReturnType;
                    switch (defs.Parameters.Length)
                    {
                        case 0:
                            exp[m.Name] = Return<dynamic>.Arguments(() => DoClient(defs));
                            break;
                        case 1:
                            exp[m.Name] = Return<dynamic>.Arguments<dynamic>((a) => DoClient(defs, a));
                            break;
                        case 2:
                            exp[m.Name] = Return<dynamic>.Arguments<dynamic, dynamic>((a, b) => DoClient(defs, a, b));
                            break;
                        case 3:
                            exp[m.Name] = Return<dynamic>.Arguments<dynamic, dynamic, dynamic>((a, b, c) => DoClient(defs, a, b, c));
                            break;
                        case 4:
                            exp[m.Name] = Return<dynamic>.Arguments<dynamic, dynamic, dynamic, dynamic>((a, b, c, d) => DoClient(defs, a, b, c, d));
                            break;
                        case 5:
                            exp[m.Name] = Return<dynamic>.Arguments<dynamic, dynamic, dynamic, dynamic, dynamic>((a, b, c, d, e) => DoClient(defs, a, b, c, d, e));
                            break;
                        case 6:
                            exp[m.Name] = Return<dynamic>.Arguments<dynamic, dynamic, dynamic, dynamic, dynamic, dynamic>((a, b, c, d, e, f) => DoClient(defs, a, b, c, d, e, f));
                            break;
                        case 7:
                            exp[m.Name] = Return<dynamic>.Arguments<dynamic, dynamic, dynamic, dynamic, dynamic, dynamic, dynamic>((a, b, c, d, e, f, g) => DoClient(defs, a, b, c, d, e, f, g));
                            break;
                        case 8:
                            exp[m.Name] = Return<dynamic>.Arguments<dynamic, dynamic, dynamic, dynamic, dynamic, dynamic, dynamic, dynamic>((a, b, c, d, e, f, g, h) => DoClient(defs, a, b, c, d, e, f, g, h));
                            break;
                        default:
                            throw new NotImplementedException("We only support till 8 parameters feel free to add it here :O");
                    }
                }
            }
            return Impromptu.ActLike<T>(exp);
        }

        private static dynamic DoClient(MethodDefinition def, params dynamic[] parameters)
        {
            RestClient cl = new RestClient(def.BasePath);
            RestRequest req = new RestRequest(ReplaceInPath(def.RestAttribute.Route,def,parameters));
            RestResponse<dynamic> ret = cl.ExecuteDynamic(req);
            if (ret.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Error using rest TODO");
            }
            return ret.Data;
        }


        private static string ReplaceInPath(string path, MethodDefinition def, dynamic[] parameters)
        {

            for (int x = 0; x < def.Parameters.Length; x++)
            {
                Tuple<string, Type> t = def.Parameters[x];
                dynamic param = parameters[x];
                string value = null;
                if (t.Item2 == typeof(string))
                    value = (string) param;
                else
                {
                    TypeConverter conv = TypeDescriptor.GetConverter(t.Item2);
                    if (conv.CanConvertTo(typeof(string)))
                    {
                        value = conv.ConvertTo(param, typeof(string));
                    }
                }
                if (value != null && path.Contains("{" + t.Item1 + "}"))
                {
                    path = path.Replace("{" + t.Item1 + "}", value);
                }
            }
            return path;
        }
    }
}
