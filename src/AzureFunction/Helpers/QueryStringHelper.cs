using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Web;

namespace AzureFunction.Helpers
{
    public static class QueryStringHelper
    {
        public static T GetModelFromQueryString<T>(this HttpRequest request) where T : new()
        {
            var jsonContent = request.ConvertQueryStringToJson();

            return JsonConvert.DeserializeObject<T>(jsonContent);
        }

        private static string ConvertQueryStringToJson(this HttpRequest request)
        {
            Console.WriteLine("QueryString content");
            Console.WriteLine(HttpUtility.UrlDecode(request.QueryString.Value));

            dynamic expandoModel = new ExpandoObject();

            var modelDic = (IDictionary<string, object>)expandoModel;

            // request.Query provides value as Array while HttpUtility.ParseQueryString provides value as string
            IQueryCollection queries = request.Query;

            foreach (var param in queries)
            {
                modelDic.SetModelProperty(param.Key, param.Value);
            }

            var jsonContent = JsonConvert.SerializeObject(expandoModel);

            Console.WriteLine("Serialized content");
            Console.WriteLine(jsonContent);

            return jsonContent;
        }

        private static void SetModelProperty(this IDictionary<string, object> sourceModel,
            string paramName, StringValues value)
        {
            /*
             * sample of paramName:
             *      string1
             *      list[]
             *      model2s[0][double2]
             *      model2s[0][model3][string3]
             *      model2s[0][model3s][0][string3]
             */

            var listProperties = paramName.Replace("]", string.Empty).Split("["); // do not remove empty string here

            IDictionary<string, object> currentModel = sourceModel;
            string currentPropertyName = "";
            var currentIndex = 0;

            while (currentIndex < listProperties.Length)
            {
                currentPropertyName = listProperties[currentIndex];

                // array of primitive data: list[]
                if (currentIndex < listProperties.Length - 1 && string.IsNullOrEmpty(listProperties[currentIndex + 1]))
                {
                    // create new array
                    if (!currentModel.ContainsKey(currentPropertyName))
                    {
                        currentModel[currentPropertyName] = new List<string>();
                    }

                    var list = (List<string>)currentModel[currentPropertyName];

                    // add new value
                    list.AddRange(value.ToArray());

                    // expect to stop here, there should be no more property
                    if (currentIndex != listProperties.Length - 2)
                        throw new ArgumentException($"Invalid parameter: name='{paramName}', value='{string.Join(",", value)}'");

                    return;
                }

                // array of complex data: model2s[0]
                if (currentIndex < listProperties.Length - 1 && int.TryParse(listProperties[currentIndex + 1], out int itemIndex))
                {
                    if (!currentModel.ContainsKey(currentPropertyName))
                    {
                        // create new array
                        currentModel[currentPropertyName] = new List<ExpandoObject>();
                    }

                    var list = (List<ExpandoObject>)currentModel[currentPropertyName];

                    if (list.Count <= itemIndex)
                    {
                        // create new model
                        list.Add(new ExpandoObject());
                    }

                    currentModel = list[itemIndex];

                    currentIndex += 2;
                }
                // normal property
                else
                {
                    // not yet last property => current index is a model
                    if (currentIndex < listProperties.Length - 1)
                    {
                        if (!currentModel.ContainsKey(currentPropertyName))
                        {
                            currentModel[currentPropertyName] = new ExpandoObject();
                        }

                        currentModel = (IDictionary<string, object>)currentModel[currentPropertyName];
                    }

                    currentIndex++;
                }
            }

            ValidateSingleValue(value);

            currentModel[currentPropertyName] = value[0];
        }

        private static void ValidateSingleValue(StringValues value)
        {
            if (value.Count > 1)
                throw new ArgumentException("Invalid value: value should contain 1 item only: " + string.Join(",", value));
        }
    }
}
