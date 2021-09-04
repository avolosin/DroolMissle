using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace DroolMissle
{
    public class JsonComparer
    {
        private object _expectedJToken;
        private object _actualJToken;
        private List<TokenMatchResult> _matchResults;
        private ILookup<string, TokenMatchCriteria> _matchCriteriaByPropertyName;

        public static List<TokenMatchResult> Compare(string expectedJson, string actualJson, params TokenMatchCriteria[] matchCriteria)
        {
            var jsonComparer = new JsonComparer();
            return jsonComparer.CompareJson(expectedJson, actualJson, matchCriteria);
        }

        internal List<TokenMatchResult> CompareJson(string expectedJson, string actualJson, params TokenMatchCriteria[] matchCriteria)
        {
            _matchCriteriaByPropertyName = matchCriteria?.ToLookup(l => l.PropertyPath);


            _matchResults = new List<TokenMatchResult>(15);
            _expectedJToken = JsonConvert.DeserializeObject(expectedJson);
            _actualJToken = JsonConvert.DeserializeObject(actualJson);

            if (_expectedJToken is JArray ja)
            {
                CompareArray(ja);
            }
            if (_expectedJToken is JObject jo)
            {
                CompareObject(jo);
            }
            if (_expectedJToken is JValue jv)
            {
                CompareValues(jv);
            }

            return _matchResults;
        }

        void CompareArray(JArray ja)
        {
            foreach (var x in ja.Children())
            {
                if (x is JObject jo)
                {
                    CompareObject(jo);
                }
                else if (x is JArray cja)
                {
                    CompareArray(cja);
                }
                else if (x is JProperty jp)
                {
                    CompareProperty(jp);
                }
                else if (x is JValue jv)
                {
                    CompareValues(jv);
                }
                else
                {
                    throw new NotImplementedException($"CompareArray for value of  {JsonConvert.SerializeObject(ja)}");
                }
            }
        }

        void CompareObject(JObject jo)
        {

            foreach (var c in jo.Children())
            {
                if (c is JProperty jp)
                {
                    CompareProperty(jp);
                }
                else if (c is JObject cjo)
                {
                    CompareObject(cjo);
                }
                else if (c is JArray cja)
                {
                    CompareArray(cja);
                }
                else
                {
                    throw new NotImplementedException($"CompareObject for value of  {JsonConvert.SerializeObject(jo)}");
                }
            }
        }

        void CompareProperty(JProperty jp)
        {

            if (jp.Value is JValue jv)
            {
                CompareValues(jv);
            }
            else if (jp.Value is JObject jo)
            {
                CompareObject(jo);
            }
            else if (jp.Value is JArray ja)
            {
                CompareArray(ja);
            }
            else
            {
                throw new NotImplementedException($"CompareProperty for value of  {JsonConvert.SerializeObject(jp)}");
            }
        }

        void CompareValues(JValue jv)
        {
            JToken actualValue = "";

            if (_actualJToken is JArray actualJArray)
            {
                actualValue = actualJArray.SelectToken(jv.Path);
            }
            else
            {
                actualValue = ((JObject)_actualJToken).SelectToken(jv.Path);
            }

            var tmr = new TokenMatchResult()
            {
                Token = jv.Path,
                ExpectedJsonValue = JsonConvert.SerializeObject(jv.Value),
            };

            if (actualValue is JValue actualJValue)
            {
                tmr.ActualJsonValue = JsonConvert.SerializeObject(actualJValue.Value);

                //see if we can find one by an exact path
                if (_matchCriteriaByPropertyName.Contains(jv.Path))
                {
                    var criteria = _matchCriteriaByPropertyName[jv.Path].First();
                    tmr.IsMatch = criteria.Matches(actualJValue.Value.ToString());
                    tmr.IsCriteriaMatch = true;
                }
                else if (jv.Path.Contains("[")) //does it look like it might be an array path?
                {
                    //try to make a generic property path eg: images[0].href becomes images[*].href
                    var genericArrayPath = Regex.Replace(jv.Path, @"\[\d\].", "[*].");
                    //oh it looks like we're have a match and we care about it? perfect. replace the property path
                    if (_matchCriteriaByPropertyName.Contains(genericArrayPath))
                    {
                        var criteria = _matchCriteriaByPropertyName[genericArrayPath].First();
                        tmr.IsMatch = criteria.Matches(actualJValue.Value.ToString());
                        tmr.IsCriteriaMatch = true;
                    }
                }

                else if (jv.Path.Contains(".")) //no exact match or generic array match - check the path to see if we allow for ignoring
                {
                    var navigationProps = jv.Path.Split(".");
                    var navProp = string.Empty;

                    for (var i = 0; i < navigationProps.Length; i++)
                    {
                        navProp += navigationProps[i] + ".";
                        //syntax to ignore a property or it's children: propertyName.*-
                        if (_matchCriteriaByPropertyName.Contains($"{navProp}*-"))
                        {
                            tmr.IsMatch = true;
                            tmr.IsCriteriaMatch = true;
                            tmr.IsIgnored = true;
                            break;
                        }
                    }

                }
            }
            else
            {
                tmr.ActualJsonValue = JsonConvert.SerializeObject(actualValue);
            }

            //not yet a match? try exact matching
            if (!tmr.IsMatch)
            {
                tmr.IsMatch = tmr.ActualJsonValue == tmr.ExpectedJsonValue;
                tmr.IsCriteriaMatch = false;
            }

            _matchResults.Add(tmr);
        }
    }
}
