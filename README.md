# Introduction

How to convert query string to model? I googled a solution but 
they seems simple and do not support nested class.

We are using Azure function where there is a download function
with complex input model.

Model binding for query string is not easy, 
we are able to work-around with POST method but there are 
possibly milions of recorded returned which may cause the browser 
hang due to memory issue (and certainly the user experience is very bad).

So, using GET method is a must. And we need to develop a solution
to convert query string to model with minimal effort to update front end code.

# Solution

## 1. Encode Json

On frontend, convert the model to json and encode it then pass to query string
like `request=[encoded json]`. On backend, simply convert the json content 
back to model. 
This is simple solution hence no need more explain :) you can see the code, so simple

## 2. Query String

The idea comes from jquery.get, jquery has nicely parse the object to query string.

Behind the scene, jquery use jquery.param to convert the model to query string.
If you are using Angular, there is a packaged named `jquery-param` which do the same.
The code base is short and simple so that you can port to any other library if necessary.

This solution is more challenging for backend to parse the query string.

# Demo
The demo use jQuery with $.get function to send ajax request to server.

## Model

This is the model used for testing
```
{
  string1: "top@string\" #1",
  int1: 101,
  double1: 1000.01,
  date1: "2020-09-10",
  listString1: ["l1", "l2", "l3"],
  listInt1: [1, 2, 3],
  listDate1: ['2020-09-11', '2020-09-12', '2020-09-13'],
  model2s: [
    {
      string2: "sub-string&l1 #2",
      int2: 102,
      double2: 1000.02,
      date2: "2020-09-11",
      model3: {
        string3: "sub string l2 #3",
        int3: 103,
        double3: 1000.03,
        date3: "2020-09-12",
      },
      model3s: [
        {
          string3: "sub string+l3 #4",
          int3: 104,
          double3: 1000.04,
          date3: "2020-09-13",
        },
        {
          string3: "sub string+l3 #5",
          int3: 105,
          double3: 1000.05,
          date3: "2020-09-14",
        }
      ]
    },
    {
      string2: "sub-string&l1 #6",
      int2: 106,
      double2: 1000.06,
      date2: "2020-09-15",
      model3: {
        string3: "sub string+l3 #7",
        int3: 107,
        double3: 1000.07,
        date3: "2020-09-16",
      },
      model3s: [
        {
          string3: "sub string+l3 #8",
          int3: 108,
          double3: 1000.08,
          date3: "2020-09-17",
        },
        {
          string3: "sub string+l3 #9",
          int3: 109,
          double3: 1000.09,
          date3: "2020-09-18",
        }
      ]
    }
  ]
}
```

# Query String

jQuery.param will generate following query string
```
?string1=top@string" #1&int1=101&double1=1000.01&date1=2020-09-10&listString1[]=l1&listString1[]=l2&listString1[]=l3&listInt1[]=1&listInt1[]=2&listInt1[]=3&listDate1[]=2020-09-11&listDate1[]=2020-09-12&listDate1[]=2020-09-13&model2s[0][string2]=sub-string&l1 #2&model2s[0][int2]=102&model2s[0][double2]=1000.02&model2s[0][date2]=2020-09-11&model2s[0][model3][string3]=sub string l2 #3&model2s[0][model3][int3]=103&model2s[0][model3][double3]=1000.03&model2s[0][model3][date3]=2020-09-12&model2s[0][model3s][0][string3]=sub string+l3 #4&model2s[0][model3s][0][int3]=104&model2s[0][model3s][0][double3]=1000.04&model2s[0][model3s][0][date3]=2020-09-13&model2s[0][model3s][1][string3]=sub string+l3 #5&model2s[0][model3s][1][int3]=105&model2s[0][model3s][1][double3]=1000.05&model2s[0][model3s][1][date3]=2020-09-14&model2s[1][string2]=sub-string&l1 #6&model2s[1][int2]=106&model2s[1][double2]=1000.06&model2s[1][date2]=2020-09-15&model2s[1][model3][string3]=sub string+l3 #7&model2s[1][model3][int3]=107&model2s[1][model3][double3]=1000.07&model2s[1][model3][date3]=2020-09-16&model2s[1][model3s][0][string3]=sub string+l3 #8&model2s[1][model3s][0][int3]=108&model2s[1][model3s][0][double3]=1000.08&model2s[1][model3s][0][date3]=2020-09-17&model2s[1][model3s][1][string3]=sub string+l3 #9&model2s[1][model3s][1][int3]=109&model2s[1][model3s][1][double3]=1000.09&model2s[1][model3s][1][date3]=2020-09-18
```

that can be broken down into

```
string1: top@string" #1
int1: 101
double1: 1000.01
date1: 2020-09-10
listString1[]: l1
listString1[]: l2
listString1[]: l3
listInt1[]: 1
listInt1[]: 2
listInt1[]: 3
listDate1[]: 2020-09-11
listDate1[]: 2020-09-12
listDate1[]: 2020-09-13
model2s[0][string2]: sub-string&l1 #2
model2s[0][int2]: 102
model2s[0][double2]: 1000.02
model2s[0][date2]: 2020-09-11
model2s[0][model3][string3]: sub string l2 #3
model2s[0][model3][int3]: 103
model2s[0][model3][double3]: 1000.03
model2s[0][model3][date3]: 2020-09-12
model2s[0][model3s][0][string3]: sub string+l3 #4
model2s[0][model3s][0][int3]: 104
model2s[0][model3s][0][double3]: 1000.04
model2s[0][model3s][0][date3]: 2020-09-13
model2s[0][model3s][1][string3]: sub string+l3 #5
model2s[0][model3s][1][int3]: 105
model2s[0][model3s][1][double3]: 1000.05
model2s[0][model3s][1][date3]: 2020-09-14
model2s[1][string2]: sub-string&l1 #6
model2s[1][int2]: 106
model2s[1][double2]: 1000.06
model2s[1][date2]: 2020-09-15
model2s[1][model3][string3]: sub string+l3 #7
model2s[1][model3][int3]: 107
model2s[1][model3][double3]: 1000.07
model2s[1][model3][date3]: 2020-09-16
model2s[1][model3s][0][string3]: sub string+l3 #8
model2s[1][model3s][0][int3]: 108
model2s[1][model3s][0][double3]: 1000.08
model2s[1][model3s][0][date3]: 2020-09-17
model2s[1][model3s][1][string3]: sub string+l3 #9
model2s[1][model3s][1][int3]: 109
model2s[1][model3s][1][double3]: 1000.09
model2s[1][model3s][1][date3]: 2020-09-18
```

# Note
- Need to port to .NET Framework 4.x
- Used `HttpRequest.Query` which is an `IQueryCollection`.
It supports parsing value to `Array` while `HttpUtility.ParseQueryString`
parse value to string (e.g.)
  - HttpRequest.Query: [1, 2, 3]
  - HttpUtility.ParseQueryString: "1, 2, 3"