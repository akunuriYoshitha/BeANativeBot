using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace BeANativeBot
{
    public class Location_ts
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Northeast_ts
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Southwest_ts
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Viewport_ts
    {
        public Northeast_ts northeast { get; set; }
        public Southwest_ts southwest { get; set; }
    }

    public class Geometry_ts
    {
        public Location_ts location { get; set; }
        public Viewport_ts viewport { get; set; }
    }

    public class OpeningHours_ts
    {
        public bool open_now { get; set; }
        public List<object> weekday_text { get; set; }
    }

    public class Photo_ts
    {
        public int height { get; set; }
        public List<string> html_attributions { get; set; }
        public string photo_reference { get; set; }
        public int width { get; set; }
    }

    public class Result_ts
    {
        public string formatted_address { get; set; }
        public Geometry_ts geometry { get; set; }
        public string icon { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public OpeningHours_ts opening_hours { get; set; }
        public List<Photo_ts> photos { get; set; }
        public string place_id { get; set; }
        public double rating { get; set; }
        public string reference { get; set; }
        public List<string> types { get; set; }
    }

    public class RootObject_ts
    {
        public List<object> html_attributions { get; set; }
        public string next_page_token { get; set; }
        public List<Result_ts> results { get; set; }
        public string status { get; set; }
    }
    public class TextSearch
    {
        public static List<string> place_details = new List<string>();
        public static List<string> references = new List<string>();
        public HttpVerb Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }

        public TextSearch()//constructor for initialization
        {
            Method = HttpVerb.GET;
            ContentType = "text/xml";
            PostData = "";
        }

        public List<Attachment> makeRequest(string placeName, string category)
        {

            List<Attachment> types = new List<Attachment>();
            string ts_url = "https://maps.googleapis.com/maps/api/place/textsearch/json?key=AIzaSyDe5LtdaLd1wLrIbZlP3Erq3hnFQplfQHo&query=" + category + "+in+" + placeName;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ts_url);//making a http web request
            request.Method = Method.ToString();
            using (HttpWebResponse res = (HttpWebResponse)request.GetResponse())
            {
                Stream res_stream = res.GetResponseStream();//creating a response stream to read the response

                StreamReader st_read = new StreamReader(res_stream);//crreating a stream reader to read from the response stream
                string places = st_read.ReadToEnd();//reading the response stream until the end and assigning to string
                                                    //the response we get is in the form of a json but converted to string when we get, this is serialization
                                                    //once we get the string form of data we convert it into jason to access internal class objects and data,this is called deserialization
                RootObject_ts places_ts = JsonConvert.DeserializeObject<RootObject_ts>(places);//Root object of city deserialization of json data
                int i = 0;
                references.Clear();
                foreach (var r1 in places_ts.results)
                {
                    string typeName = r1.name;
                    string rating = " RATING : " + r1.rating.ToString();
                    string typeAddress = " ADDRESS : " + r1.formatted_address;
                    PlaceDetails pd = new PlaceDetails();
                    pd.makeRequest(r1.place_id);
                    string url = pd.getUrl();
                    try
                    {
                        references.Add(r1.photos[0].photo_reference);
                    }
                    catch (Exception e)
                    {
                        references.Add("");
                    }
                    List<CardAction> buttons = new List<CardAction>
                    {
                        new CardAction(ActionTypes.PostBack, " View place and its reviews ", value : "Place Id : " + i + r1.place_id),
                        new CardAction(ActionTypes.OpenUrl, " Get directions to visit place ", value : url)
                    };

                    Attachment heroCard = GetHeroCard(typeName, rating, typeAddress, buttons);
                    types.Add(heroCard);
                    i++;
                    if (i == 5)
                        break;
                }
                return types;
            }
        }

        public string getReference(int index)
        {
            System.Diagnostics.Debug.WriteLine(references[index]);
            return "https://maps.googleapis.com/maps/api/place/photo?maxwidth=400&key=AIzaSyDe5LtdaLd1wLrIbZlP3Erq3hnFQplfQHo&photoreference=" + references[index];
        }

        private static Attachment GetHeroCard(string title, string subtitle, string text, List<CardAction> cardAction)
        {
            var heroCard = new HeroCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Buttons = cardAction,
            };

            return heroCard.ToAttachment();
        }

        public List<string> searchPlace(string placeName)
        {
            string ts_url = "https://maps.googleapis.com/maps/api/place/textsearch/json?key=AIzaSyDe5LtdaLd1wLrIbZlP3Erq3hnFQplfQHo&query=" + placeName;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ts_url);//making a http web request
            request.Method = Method.ToString();
            using (HttpWebResponse res = (HttpWebResponse)request.GetResponse())
            {
                Stream res_stream = res.GetResponseStream();//creating a response stream to read the response

                StreamReader st_read = new StreamReader(res_stream);//crreating a stream reader to read from the response stream
                string places = st_read.ReadToEnd();//reading the response stream until the end and assigning to string
                                                    //the response we get is in the form of a json but converted to string when we get, this is serialization
                                                    //once we get the string form of data we convert it into jason to access internal class objects and data,this is called deserialization
                RootObject_ts places_ts = JsonConvert.DeserializeObject<RootObject_ts>(places);//Root object of city deserialization of json data
                
                foreach (var r in places_ts.results)
                {
                    r.name = r.name.ToLower();
                    if (r.name.Equals(placeName.ToLower()))
                    {
                        place_details.Add(r.place_id);
                        place_details.Add(r.name);
                        place_details.Add("ADDRESS : " + r.formatted_address);
                        try
                        {
                            place_details.Add(r.photos[0].photo_reference);
                        }
                        catch (Exception e)
                        {
                            place_details.Add(" ");
                        }
                    }
                    

                }
            }
            return place_details;
        }
    }
}