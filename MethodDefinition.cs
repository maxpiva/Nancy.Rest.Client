using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.Rest.Annotations;

namespace Nancy.Rest.Client
{
    public class MethodDefinition
    {
        public Type ReturnType { get; set; }
        public List<Tuple<string,Type>> Parameters { get; set; }

        public Rest.Annotations.Rest RestAttribute { get; set; }

        public string BasePath { get; set; }
    }
}
