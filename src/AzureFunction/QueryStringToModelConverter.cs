using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Web;

namespace AzureFunction
{
    public static class QueryStringToModelConverter
    {
        public static T GetModelFromQueryString<T>(this HttpRequest req) where T : new()
        {
            var jsonContent = ConvertQueryStringToJson(req);

            return JsonConvert.DeserializeObject<T>(jsonContent);
        }

        private static string ConvertQueryStringToJson(HttpRequest req)
        {
            Console.WriteLine("QueryString content");
            Console.WriteLine(HttpUtility.UrlDecode(req.QueryString.Value));

            dynamic expandoModel = new ExpandoObject();

            var modelDic = (IDictionary<string, object>)expandoModel;

            // for other version, can use:

            var queries = HttpUtility.ParseQueryString(req.QueryString.Value);

            foreach (string param in queries.AllKeys)
            {
                // get the right model and property name for each query string param
                var (model, propertyName) = modelDic.GetModelAndProperty(param);

                // set value, note: item.Value is array
                model[propertyName] = queries[param];
            }

            var jsonContent = JsonConvert.SerializeObject(expandoModel);

            Console.WriteLine("Serialized content");
            Console.WriteLine(jsonContent);

            return jsonContent;
        }

        private static (IDictionary<string, object>, string propertyName) GetModelAndProperty(this IDictionary<string, object> sourceModel, string paramName)
        {
            /*
             * sample of paramName:
             *      string1
             *      model2s[0][double2]
             *      model2s[0][model3][string3]
             *      model2s[0][model3s][0][string3]
             */

            if (paramName.IndexOf("[") < 0) return (sourceModel, paramName);

            var listProperties = paramName.Replace("]", string.Empty).Split("[");

            IDictionary<string, object> lastModel = sourceModel;
            string propertyName = "";

            var currentIndex = 0;

            while (currentIndex < listProperties.Length)
            {
                propertyName = listProperties[currentIndex];

                if (string.IsNullOrEmpty(listProperties[currentIndex + 1]))
                {
                    // array of primitive data item[]

                    if (!lastModel.ContainsKey(propertyName))
                    {
                        // create new array
                        lastModel[propertyName] = new List<string>();
                    }

                    // TODO: need to add value here

                }
                if (currentIndex < listProperties.Length - 1 && int.TryParse(listProperties[currentIndex + 1], out int index))
                {
                    // if the property is an array such as model2s[0]

                    if (!lastModel.ContainsKey(propertyName))
                    {
                        // create new array
                        lastModel[propertyName] = new List<ExpandoObject>();
                    }

                    var propertyValue = (List<ExpandoObject>)lastModel[propertyName];

                    if (propertyValue.Count <= index)
                    {
                        // create new model
                        propertyValue.Add(new ExpandoObject());
                    }

                    lastModel = propertyValue[index];

                    currentIndex += 2;
                }
                else // normal property
                {
                    if (currentIndex < listProperties.Length - 1)
                    {
                        // not yet last property => current index is a model
                        if (!lastModel.ContainsKey(propertyName))
                        {
                            lastModel[propertyName] = new ExpandoObject();
                        }

                        lastModel = (IDictionary<string, object>)lastModel[propertyName];
                    }

                    currentIndex++;
                }
            }

            return (lastModel, propertyName);
        }

    }
}
