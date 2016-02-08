using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PlayStation_App.Models.Sticker;

namespace PlayStation_App.Tools
{
    public class SizetypeConverter {
        public static List<SizeProperty> ConvertStringToSizeProperty(string sizeProperty)
        {
            var list = new List<SizeProperty>();
            var jObject = JObject.Parse(sizeProperty);
            var jsonList = (JObject)jObject["stickerImagesBySize"];
            foreach (var item in jsonList)
            {
                var newItem = JsonConvert.DeserializeObject<SizeProperty>(item.Value.ToString());
                newItem.Size = item.Key;
                list.Add(newItem);
            }
            return list;
        }
    }
}
