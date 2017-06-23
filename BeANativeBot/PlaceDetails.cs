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
    public class AddressComponent_pd
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }
    }

    public class Location_pd
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Northeast_pd
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Southwest_pd
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }

    public class Viewport_pd
    {
        public Northeast_pd northeast { get; set; }
        public Southwest_pd southwest { get; set; }
    }

    public class Geometry_pd
    {
        public Location_pd location { get; set; }
        public Viewport_pd viewport { get; set; }
    }

    public class AltId_pd
    {
        public string place_id { get; set; }
        public string scope { get; set; }
    }

    public class Aspect_pd
    {
        public int rating { get; set; }
        public string type { get; set; }
    }

    public class Review_pd
    {
        public List<Aspect_pd> aspects { get; set; }
        public string author_name { get; set; }
        public string author_url { get; set; }
        public string language { get; set; }
        public int rating { get; set; }
        public string text { get; set; }
        public object time { get; set; }
    }

    public class Result_pd
    {
        public List<AddressComponent_pd> address_components { get; set; }
        public string adr_address { get; set; }
        public string formatted_address { get; set; }
        public string formatted_phone_number { get; set; }
        public Geometry_pd geometry { get; set; }
        public string icon { get; set; }
        public string id { get; set; }
        public string international_phone_number { get; set; }
        public string name { get; set; }
        public string place_id { get; set; }
        public string scope { get; set; }
        public List<AltId_pd> alt_ids { get; set; }
        public double rating { get; set; }
        public string reference { get; set; }
        public List<Review_pd> reviews { get; set; }
        public List<string> types { get; set; }
        public string url { get; set; }
        public string vicinity { get; set; }
        public string website { get; set; }
    }

    public class RootObject_pd
    {
        public List<object> html_attributions { get; set; }
        public Result_pd result { get; set; }
        public string status { get; set; }
    }
    public class PlaceDetails
    {
        public static List<string> keys = new List<string>
        {
            "AIzaSyCEcSwhhCMO8L-rb-nERCXw4bpC2qA_Gno",
            "AIzaSyBQo1JlbpQyB-Ip28zey6zQ0RsE7klEDqo",
            "AIzaSyAic7iByLYCk0r9IwsGhrFdQUVKABW8om0",
            "AIzaSyD2jHO-vBqhB3g34E286pQzlnmSfvfFBKo"
        };
        public static List<Review_pd> userReviews = new List<Review_pd>();
        public static string phoneNum = "Not Available";
        public static string placeName;
        public static double rating;
        public HttpVerb Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }
        public static string url;

        public PlaceDetails()//constructor for initialization
        {
            Method = HttpVerb.GET;
            ContentType = "text/xml";
            PostData = "";
        }

        public void makeRequest(string placeId)
        {
            Random rnd = new Random();
            int rk = rnd.Next(keys.Count);
            string key = keys[rk];
            string ts_url = "https://maps.googleapis.com/maps/api/place/details/json?key=" + key + "&placeid=" + placeId;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ts_url);//making a http web request
            request.Method = Method.ToString();
            using (HttpWebResponse res = (HttpWebResponse)request.GetResponse())
            {
                Stream res_stream = res.GetResponseStream();//creating a response stream to read the response

                StreamReader st_read = new StreamReader(res_stream);//crreating a stream reader to read from the response stream
                string places = st_read.ReadToEnd();//reading the response stream until the end and assigning to string
                                                    //the response we get is in the form of a json but converted to string when we get, this is serialization
                                                    //once we get the string form of data we convert it into jason to access internal class objects and data,this is called deserialization
                RootObject_pd places_pd = JsonConvert.DeserializeObject<RootObject_pd>(places);//Root object of city deserialization of json data
                url = places_pd.result.url;
                placeName = places_pd.result.name;
                rating = places_pd.result.rating;
                try { phoneNum = places_pd.result.formatted_phone_number; }
                catch (Exception e) { }
                userReviews = places_pd.result.reviews;
            }
        }

        public Attachment makeCard(string photoReference, string placeId)
        {
            makeRequest(placeId);
            string title = placeName;
            string subtitle = "CONTACT NUMBER :  " + phoneNum;
            CardImage cardImage = new CardImage(url: photoReference);
            CardAction cardButton = new CardAction(ActionTypes.OpenUrl, " Get directions to visit place ", value: url);
            Attachment detailsCard = GetHeroCard(title, subtitle, "", cardImage, cardButton);
            return detailsCard;
        }

        public string getUrl()
        {
            return url;
        }

        public string GetReviews(string placeId)
        {
            makeRequest(placeId);
            string review = "**REVIEWS**" + Environment.NewLine + Environment.NewLine;
            int i = 1;
            foreach (var rev in userReviews)
            {
                review += i.ToString() + ".  By " + rev.author_name + Environment.NewLine + Environment.NewLine;
                review += rev.text + Environment.NewLine + Environment.NewLine;
                i++;

            }
            return review;
        }

        private static Attachment GetHeroCard(string title, string subtitle, string text, CardImage cardImage, CardAction cardAction)
        {
            var heroCard = new HeroCard
            {
                Title = title,
                Subtitle = subtitle,
                Text = text,
                Images = new List<CardImage>() { cardImage },
                Buttons = new List<CardAction>() { cardAction },
            };

            return heroCard.ToAttachment();
        }

        public Attachment displayPlaceDetails(string placeName)
        {
            System.Diagnostics.Debug.WriteLine(placeName);
            Random rnd = new Random();
            int rk = rnd.Next(keys.Count);
            string key = keys[rk];
            List<string> initialDetails = new List<string>();
            TextSearch ts = new TextSearch();
            initialDetails = ts.searchPlace(placeName);
            System.Diagnostics.Debug.WriteLine(placeName);
            string placeId = initialDetails[0];
            placeName = initialDetails[1];
            System.Diagnostics.Debug.WriteLine(placeName);
            string address = initialDetails[2];
            string photoRef = "https://maps.googleapis.com/maps/api/place/photo?maxwidth=400&key=" + key +"&photoreference=" + initialDetails[3];
            makeRequest(placeId);
            CardImage cardImage = new CardImage(url: photoRef);
            CardAction cardButton = new CardAction(ActionTypes.OpenUrl, " Get directions to visit place ", value: url);
            Attachment detailsCard = GetHeroCard(placeName, "CONTACT NUMBER : " + phoneNum, address, cardImage, cardButton);
            return detailsCard;
        }
    }
}