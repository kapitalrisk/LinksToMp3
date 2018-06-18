using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yt2mp3.FileHandler.Models;

namespace yt2mp3.Tools
{
    public static class Serializor
    {
        public static T DeSerialize<T>(string input)
        {
            // EVOLUTION
            return JsonConvert.DeserializeObject<T>(input);
        }
    }
}
