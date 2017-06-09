using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace BeANativeBot
{
    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Northeast
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Southwest
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Viewport
    {
        public Northeast northeast { get; set; }
        public Southwest southwest { get; set; }
    }

    public class Geometry
    {
        public Location location { get; set; }
        public Viewport viewport { get; set; }
    }

    public class AltId
    {
        public string place_id { get; set; }
        public string scope { get; set; }
    }

    public class Aspect
    {
        public int rating { get; set; }
        public string type { get; set; }
    }

    public class Review
    {
        public List<Aspect> aspects { get; set; }
        public string author_name { get; set; }
        public string author_url { get; set; }
        public string language { get; set; }
        public int rating { get; set; }
        public string text { get; set; }
        public object time { get; set; }
    }

    public class Result
    {
        public List<AddressComponent> address_components { get; set; }
        public string adr_address { get; set; }
        public string formatted_address { get; set; }
        public string formatted_phone_number { get; set; }
        public Geometry geometry { get; set; }
        public string icon { get; set; }
        public string id { get; set; }
        public string international_phone_number { get; set; }
        public string name { get; set; }
        public string place_id { get; set; }
        public string scope { get; set; }
        public List<AltId> alt_ids { get; set; }
        public double rating { get; set; }
        public string reference { get; set; }
        public List<Review> reviews { get; set; }
        public List<string> types { get; set; }
        public string url { get; set; }
        public string vicinity { get; set; }
        public string website { get; set; }
    }

    public class RootObject
    {
        public List<object> html_attributions { get; set; }
        public Result result { get; set; }
        public string status { get; set; }
    }
        public class DetailsPlace
    {
        public HttpVerb Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }
        public static string url = "";
        public DetailsPlace()
        {
            Method = HttpVerb.GET;
            ContentType = "text/xml";
            PostData = "";
        }
        public string MakeRequest(String placeId)//making two requests 1. lalng extraction 2.places search based on latlong
        {
            string finalRes = "Address : ";

            if (true)//to make differentiation btwn two requests
            {
                string citypoint = "https://maps.googleapis.com/maps/api/place/details/json?key=AIzaSyANeYnxWuP4GV-GZtPu1wkOmNIxJmLn2sg&placeid=" + placeId;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(citypoint);//making a http web request
                request.Method = Method.ToString();//assigning method to request

                using (HttpWebResponse res = (HttpWebResponse)request.GetResponse())//using means using internal method getting method response and assigning
                {
                    if (res.StatusCode != HttpStatusCode.OK)//checking error code
                    {
                        return finalRes;
                    }
                    Stream res_stream = res.GetResponseStream();//creating a response stream to read the response

                    StreamReader st_read = new StreamReader(res_stream);//crreating a stream reader to read from the response stream
                    string city_json = st_read.ReadToEnd();//reading the response stream until the end and assigning to string
                    //the response we get is in the form of a json but converted to string when we get, this is serialization
                    //once we get the string form of data we convert it into jason to access internal class objects and data,this is called deserialization
                    RootObject city_obj = JsonConvert.DeserializeObject<RootObject>(city_json);//Root object of city deserialization of json data
                    finalRes += Environment.NewLine + Environment.NewLine + city_obj.result.name + ", ";
                    int count = city_obj.result.address_components.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (i == count - 1)
                            finalRes += city_obj.result.address_components[i].long_name;
                        else
                            finalRes += city_obj.result.address_components[i].long_name + ", ";
                    }
                    url = city_obj.result.url;
                }
            }
            return finalRes;
        }

        public string getRouteMap()
        {
            return url;
        }
    }
}