using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Configuration
{
    public class Client
    {
        public string Id { get; set; }
        public string Secret { get; set; }
        public List<string> Audience { get; set; }//hangi Api a erişim izni vereceğimizi belirttiğimiz.www.myapi1.com gibi
    }
}
