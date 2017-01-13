using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;
using Nancy.Rest.Annotations;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Extensions;

namespace Nancy.Rest.Client
{
    public static class ClientFactory
    {

        public static T Create<T>(string path, string defaultlevelqueryparametername="level", string defaultexcludtagsqueryparametername="excludetags") where T : class
        {
            return Create<T>(path, int.MaxValue, null, defaultlevelqueryparametername, defaultexcludtagsqueryparametername);
        }
        private static T Create<T>(string path, int level, IEnumerable<string> tags, string defaultlevelqueryparametername, string defaultexcludtagsqueryparametername, bool filter=true) where T: class
        {
            dynamic dexp = new ExpandoObject();

            IDictionary<string, object> exp = (IDictionary<string, object>) dexp;
            dexp.DYN_defaultlevelqueryparametername = defaultlevelqueryparametername;
            dexp.DYN_defaultexcludtagsqueryparametername = defaultexcludtagsqueryparametername;
            dexp.DYN_level = level;
            dexp.DYN_tags = tags;
            dexp.DYN_filter = filter;

            bool hasfilterinterface = (typeof(T).GetInterfaces().Any(a => a.Name == typeof(IFilter<>).Name));

            foreach (MethodInfo m in typeof(T).GetMethods())
            {
                List<Annotations.Rest> rests = m.GetCustomAttributes<Annotations.Rest>().ToList();
                if (rests.Count > 0)
                {
                    MethodDefinition defs = new MethodDefinition();
                    defs.RestAttribute = rests[0];
                    defs.BasePath = path;
                    defs.Parameters = m.GetParameters().Select(a => new Tuple<string, Type>(a.Name, a.ParameterType)).ToList();
                    defs.ReturnType = m.ReturnType;
                    if (hasfilterinterface && (m.Name == "Filter"))
                        continue;

                    if (m.IsAsyncMethod())
                    {

                        switch (defs.Parameters.Count)
                        {
                            case 0:
                                exp[m.Name] = Return<dynamic>.Arguments(() => DoAsyncClient(dexp, defs));
                                break;
                            case 1:
                                exp[m.Name] = Return<dynamic>.Arguments<dynamic>((a) => DoAsyncClient(dexp, defs, a));
                                break;
                            case 2:
                                exp[m.Name] = Return<dynamic>.Arguments<dynamic, dynamic>((a, b) => DoAsyncClient(dexp, defs, a, b));
                                break;
                            case 3:
                                exp[m.Name] = Return<dynamic>.Arguments<dynamic, dynamic, dynamic>((a, b, c) => DoAsyncClient(dexp, defs, a, b, c));
                                break;
                            case 4:
                                exp[m.Name] = Return<dynamic>.Arguments<dynamic, dynamic, dynamic, dynamic>((a, b, c, d) => DoAsyncClient(dexp, defs, a, b, c, d));
                                break;
                            case 5:
                                exp[m.Name] = Return<dynamic>.Arguments<dynamic, dynamic, dynamic, dynamic, dynamic>((a, b, c, d, e) => DoAsyncClient(dexp, defs, a, b, c, d, e));
                                break;
                            case 6:
                                exp[m.Name] = Return<dynamic>.Arguments<dynamic, dynamic, dynamic, dynamic, dynamic, dynamic>((a, b, c, d, e, f) => DoAsyncClient(dexp, defs, a, b, c, d, e, f));
                                break;
                            case 7:
                                exp[m.Name] = Return<dynamic>.Arguments<dynamic, dynamic, dynamic, dynamic, dynamic, dynamic, dynamic>((a, b, c, d, e, f, g) => DoAsyncClient(dexp, defs, a, b, c, d, e, f, g));
                                break;
                            case 8:
                                exp[m.Name] = Return<dynamic>.Arguments<dynamic, dynamic, dynamic, dynamic, dynamic, dynamic, dynamic, dynamic>((a, b, c, d, e, f, g, h) => DoAsyncClient(dexp, defs, a, b, c, d, e, f, g, h));
                                break;
                            default:
                                throw new NotImplementedException("It only support till 8 parameters feel free to add more here :O");
                        }
                    }
                    else
                    {
                        switch (defs.Parameters.Count)
                        {
                            case 0:
                                exp[m.Name] = Return<dynamic>.Arguments(() => DoSyncClient(dexp, defs));
                                break;
                            case 1:
                                exp[m.Name] = Return<dynamic>.Arguments<dynamic>((a) => DoSyncClient(dexp, defs, a));
                                break;
                            case 2:
                                exp[m.Name] = Return<dynamic>.Arguments<dynamic, dynamic>((a, b) => DoSyncClient(dexp, defs, a, b));
                                break;
                            case 3:
                                exp[m.Name] = Return<dynamic>.Arguments<dynamic, dynamic, dynamic>((a, b, c) => DoSyncClient(dexp, defs, a, b, c));
                                break;
                            case 4:
                                exp[m.Name] = Return<dynamic>.Arguments<dynamic, dynamic, dynamic, dynamic>((a, b, c, d) => DoSyncClient(dexp, defs, a, b, c, d));
                                break;
                            case 5:
                                exp[m.Name] = Return<dynamic>.Arguments<dynamic, dynamic, dynamic, dynamic, dynamic>((a, b, c, d, e) => DoSyncClient(dexp, defs, a, b, c, d, e));
                                break;
                            case 6:
                                exp[m.Name] = Return<dynamic>.Arguments<dynamic, dynamic, dynamic, dynamic, dynamic, dynamic>((a, b, c, d, e, f) => DoSyncClient(dexp, defs, a, b, c, d, e, f));
                                break;
                            case 7:
                                exp[m.Name] = Return<dynamic>.Arguments<dynamic, dynamic, dynamic, dynamic, dynamic, dynamic, dynamic>((a, b, c, d, e, f, g) => DoSyncClient(dexp, defs, a, b, c, d, e, f, g));
                                break;
                            case 8:
                                exp[m.Name] = Return<dynamic>.Arguments<dynamic, dynamic, dynamic, dynamic, dynamic, dynamic, dynamic, dynamic>((a, b, c, d, e, f, g, h) => DoSyncClient(dexp, defs, a, b, c, d, e, f, g, h));
                                break;
                            default:
                                throw new NotImplementedException("It only support till 8 parameters feel free to add more here :O");
                        }                    
                    }
                }
            }
            T inter = Impromptu.ActLike<T>(dexp);
            if (hasfilterinterface)
            {
                if (filter)
                    exp["Filter"] = Return<T>.Arguments<int, IEnumerable<string>>((a, b) => Create<T>(path, a, b, defaultlevelqueryparametername, defaultexcludtagsqueryparametername,false));
                else
                    exp["Filter"] = Return<T>.Arguments<int, IEnumerable<string>>((a, b) => inter);
            }
            return inter;
        }

        private static dynamic DoSyncClient(dynamic dexp, MethodDefinition def, params dynamic[] parameters)
        {
            string defaultlevelqueryparametername = dexp.DYN_defaultlevelqueryparametername;
            string defaultexcludtagsqueryparametername = dexp.DYN_defaultexcludtagsqueryparametername;
            int level = dexp.DYN_level;
            List<string> tags = dexp.DYN_tags;
            RestClient cl = new RestClient(def.BasePath);
            Tuple<string, object> tup = ProcessPath(def.RestAttribute.Route, def, parameters);
            RestRequest req = new RestRequest(tup.Item1,def.RestAttribute.Verb.ToMethod());
            if (level != int.MaxValue)
                req.AddQueryParameter(defaultlevelqueryparametername, level.ToString());
            if (tags != null && tags.Count > 0)
                req.AddQueryParameter(defaultexcludtagsqueryparametername, string.Join("'", tags));
            if (tup.Item2 != null)
                req.AddBody(tup.Item2);
            req.RequestFormat=DataFormat.Json;
            IRestResponse ret = cl.Execute(req);
            if (ret.ResponseStatus != ResponseStatus.Completed)
                throw new Exception("Error using rest TODO");
            return JsonConvert.DeserializeObject(ret.Content, def.ReturnType);
        }
        private static async Task<dynamic> DoAsyncClient(dynamic dexp, MethodDefinition def, params dynamic[] parameters)
        {
            string defaultlevelqueryparametername = dexp.DYN_defaultlevelqueryparametername;
            string defaultexcludtagsqueryparametername = dexp.DYN_defaultexcludtagsqueryparametername;
            int level = dexp.DYN_level;
            List<string> tags = dexp.DYN_tags;
            RestClient cl = new RestClient(def.BasePath);
            Tuple<string, object> tup = ProcessPath(def.RestAttribute.Route, def, parameters);
            RestRequest req = new RestRequest(tup.Item1, def.RestAttribute.Verb.ToMethod());
            if (level != int.MaxValue)
                req.AddQueryParameter(defaultlevelqueryparametername, level.ToString());
            if (tags != null && tags.Count > 0)
                req.AddQueryParameter(defaultexcludtagsqueryparametername, string.Join("'", tags));
            if (tup.Item2 != null)
                req.AddBody(tup.Item2);
            req.RequestFormat = DataFormat.Json;
            IRestResponse ret = await cl.ExecuteTaskAsync(req);
            if (ret.ResponseStatus != ResponseStatus.Completed)
                throw new Exception("Error using rest TODO");
            return JsonConvert.DeserializeObject(ret.Content, def.ReturnType);
        }

        private static Method ToMethod(this Verbs verb)
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
        private static Regex rpath=new Regex("\\{(.*?)\\}",RegexOptions.Compiled);
        private static Regex options = new Regex("\\((.*?)\\)", RegexOptions.Compiled);

        //TODO It only support Nancy constrains except version and optional parameters, others types should be added.

        private static Tuple<string, object> ProcessPath(string path, MethodDefinition def, dynamic[] parameters)
        {
            List<Parameter> pars=new List<Parameter>();
            MatchCollection collection = rpath.Matches(path);
            foreach (Match m in collection)
            {
                if (m.Success)
                {
                    string value = m.Groups[1].Value;
                    Parameter p = new Parameter();
                    p.Original = value;
                    bool optional = false;
                    string constraint = null;
                    string ops = null;
                    int idx = value.LastIndexOf("?");
                    if (idx > 0)
                    {
                        value = value.Substring(0, idx);
                        optional = true;
                    }
                    idx = value.LastIndexOf(':');
                    if (idx >= 0)
                    {
                        constraint = value.Substring(idx + 1);
                        Match optmatch = options.Match(constraint);
                        if (optmatch.Success)
                        {
                            ops = optmatch.Groups[1].Value;
                            constraint = constraint.Substring(0, optmatch.Groups[1].Index);
                        }
                        value = value.Substring(0, idx);
                    }
                    Tuple<string, Type> tx = def.Parameters.FirstOrDefault(a => a.Item1 == value);
                    if (tx == null)
                        throw new Exception("Unable to find parameter '" + value + "' in method with route : " + def.RestAttribute.Route);
                    p.Name = tx.Item1;
                    dynamic par = parameters[def.Parameters.IndexOf(tx)];
                    if (constraint != null)
                    {
                        ParameterType type = ParameterType.InstanceTypes.FirstOrDefault(a => a.Name == constraint);
                        ParameterResult res = type.Convert((object) par, ops, p.Name, optional);
                        if (!res.Success)
                            throw new Exception(res.Error);
                        p.Value = res.Value;
                    }
                    else
                    {
                        TypeConverter c = TypeDescriptor.GetConverter(par.GetType());
                        if (c.CanConvertTo(typeof(string)))
                            p.Value = c.ConvertToInvariantString(par);
                        else
                            throw new Exception("Unable to convert parameter '" + value + "' to string");
                    }
                    pars.Add(p);
                }
            }
            List<dynamic> bodyobjs=new List<dynamic>();
            foreach (Parameter p in pars)
            {
                path = path.Replace("{" + p.Original + "}", p.Value);
            }
            List<string> names = pars.Select(a => a.Name).ToList();
            List<int> bodyitems = def.Parameters.Where(a => !names.Contains(a.Item1)).Select(a => def.Parameters.IndexOf(a)).ToList();
            object body = null;
            if (bodyitems.Count > 1)
            {
                Dictionary<string, object> bjson=new Dictionary<string, object>();
                foreach(int p in bodyitems)
                    bjson.Add(def.Parameters[p].Item1,parameters[p]);
                body = bjson;
            }
            else if (bodyitems.Count == 1)
                body = parameters[bodyitems[0]];
            return new Tuple<string, object>(path,body);
        }

        public class Parameter
        {
            public string Name { get; set; }
            public string Original { get; set; }
            public string Value { get; set; }
        }
    }
}
