using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;


public enum HttpVerb//types of request methods.choode one from these to make request
{
    GET,
    POST,
    PUT,
    DELETE
}

namespace Be_A_Native
{
    //all classes from the json file. see the response from api
    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }  
    }

    public class Geometry
    {
        public Location location { get; set; }
    }

    public class OpeningHours
    {
        public bool open_now { get; set; }
    }

    public class Photo
    {
        public int height { get; set; }
        public List<object> html_attributions { get; set; }
        public string photo_reference { get; set; }
        public int width { get; set; }
    }

    public class AltId
    {
        public string place_id { get; set; }
        public string scope { get; set; }
    }

    public class Result
    {
        public Geometry geometry { get; set; }
        public string icon { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public OpeningHours opening_hours { get; set; }
        public List<Photo> photos { get; set; }
        public string place_id { get; set; }
        public string scope { get; set; }
        public List<AltId> alt_ids { get; set; }
        public string reference { get; set; }
        public List<string> types { get; set; }
        public string vicinity { get; set; }
    }

    public class RootObject//root of places api places search
    {
        public List<object> html_attributions { get; set; }
        public List<Result> results { get; set; }
        public string status { get; set; }
    }

    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
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


    public class Result_city
    {
        public List<AddressComponent> address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public string place_id { get; set; }
        public List<string> types { get; set; }
    }

    public class RootObject_city//root object of city lat long extraction
    {
        public List<Result> results { get; set; }
        public string status { get; set; }
    }

    public class RestClient//main class starts here
    {
        public string EndPoint { get; set; }//final uri of places search api
        public string citypoint { get; set; }//final uri of city latlong api
        public HttpVerb Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }
        public static string placeId;

        public RestClient()//constructor for initialization
        {
            Method = HttpVerb.GET;
            ContentType = "text/xml";
            PostData = "";
        }

        public string MakeRequest(String cityname)//making two requests 1. lalng extraction 2.places search based on latlong
        {
            Double latitude;
            Double longitude;
            if (true)//to make differentiation btwn two requests
            {
                citypoint = "https://maps.googleapis.com/maps/api/geocode/json?address=" + cityname + "&key=AIzaSyANeYnxWuP4GV-GZtPu1wkOmNIxJmLn2sg";//city api request uri
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(citypoint);//making a http web request
                request.Method = Method.ToString();//assigning method to request

                using (HttpWebResponse res = (HttpWebResponse)request.GetResponse())//using means using internal method getting method response and assigning
                {
                    if (res.StatusCode != HttpStatusCode.OK)//checking error code
                    {
                        return "error " + res.StatusCode.ToString();
                    }
                    Stream res_stream = res.GetResponseStream();//creating a response stream to read the response

                    StreamReader st_read = new StreamReader(res_stream);//crreating a stream reader to read from the response stream
                    string city_json = st_read.ReadToEnd();//reading the response stream until the end and assigning to string
                    //the response we get is in the form of a json but converted to string when we get, this is serialization
                    //once we get the string form of data we convert it into jason to access internal class objects and data,this is called deserialization
                    RootObject_city city_obj = JsonConvert.DeserializeObject<RootObject_city>(city_json);//Root object of city deserialization of json data

                    latitude = city_obj.results[0].geometry.location.lat;//extraction of latitude from city object root obj -> results[0] -> geometry ->location -> lat see response
                    longitude = city_obj.results[0].geometry.location.lng;
                    placeId = city_obj.results[0].place_id;

                }
            }
            String res_str = "";
            String res_final = "";
            List<string> Types = new List<string> { };//types of data we want to extract from places types supported are given in place types website
            Types.Add("museum");
            Types.Add("zoo");
            Types.Add("stadium");
            Types.Add("shopping_mall");
            Types.Add("art_gallery");
            foreach (string type in Types)//make one request for each type 
            {

                EndPoint = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?" + "location=" + latitude + "," + longitude + "&radius=500&type=" + type + "&key=AIzaSyDyjc_fG7ZIBbJcE4EoTZKMDKDFENLSwdg";

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(EndPoint);
                request.Method = Method.ToString();
                using (HttpWebResponse res = (HttpWebResponse)request.GetResponse())
                {
                    if (res.StatusCode != HttpStatusCode.OK)
                    {
                        return "error " + res.StatusCode.ToString();
                    }
                    Stream res_stream = res.GetResponseStream();

                    StreamReader st_read = new StreamReader(res_stream);
                    res_str = st_read.ReadToEnd();

                    RootObject r = JsonConvert.DeserializeObject<RootObject>(res_str);//google places are already sorted in the order of popularity based on some constraints
                    int flag = 0;
                    foreach (Result o in r.results)//from the results we extract places and add to the response crating a formed output
                    {
                        if (flag == 0)//printing type of the place once 
                        {
                            res_final += (type.ToUpper() + Environment.NewLine + Environment.NewLine);
                        }
                        res_final += (o.name + Environment.NewLine + Environment.NewLine);
                        flag++;

                        if (flag == 5)//maxium or top 5 popular places are retrived
                        {
                            break;
                        }
                    }
                }
            }

            return res_final;//final output or response code
        }

        public string retPlaceID(string placeName)
        {
            MakeRequest(placeName);
            return placeId;
        }

    } // class
}//name space