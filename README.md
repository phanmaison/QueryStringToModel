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

# Demo
The demo use jQuery with $.get function to send ajax request to server.
It should work with any other library.

# Query String
Here is the query string used for testing
```
?string1=top@string" #1&int1=101&double1=1000.01&date1=2020-09-10&listString1[]=l1&listString1[]=l2&listString1[]=l3&listInt1[]=1&listInt1[]=2&listInt1[]=3&listDate1[]=2020-09-11&listDate1[]=2020-09-12&listDate1[]=2020-09-13&model2s[0][string2]=sub-string&l1 #2&model2s[0][int2]=102&model2s[0][double2]=1000.02&model2s[0][date2]=2020-09-11&model2s[0][model3][string3]=sub string l2 #3&model2s[0][model3][int3]=103&model2s[0][model3][double3]=1000.03&model2s[0][model3][date3]=2020-09-12&model2s[0][model3s][0][string3]=sub string+l3 #4&model2s[0][model3s][0][int3]=104&model2s[0][model3s][0][double3]=1000.04&model2s[0][model3s][0][date3]=2020-09-13&model2s[0][model3s][1][string3]=sub string+l3 #5&model2s[0][model3s][1][int3]=105&model2s[0][model3s][1][double3]=1000.05&model2s[0][model3s][1][date3]=2020-09-14&model2s[1][string2]=sub-string&l1 #6&model2s[1][int2]=106&model2s[1][double2]=1000.06&model2s[1][date2]=2020-09-15&model2s[1][model3][string3]=sub string+l3 #7&model2s[1][model3][int3]=107&model2s[1][model3][double3]=1000.07&model2s[1][model3][date3]=2020-09-16&model2s[1][model3s][0][string3]=sub string+l3 #8&model2s[1][model3s][0][int3]=108&model2s[1][model3s][0][double3]=1000.08&model2s[1][model3s][0][date3]=2020-09-17&model2s[1][model3s][1][string3]=sub string+l3 #9&model2s[1][model3s][1][int3]=109&model2s[1][model3s][1][double3]=1000.09&model2s[1][model3s][1][date3]=2020-09-18
```
which can be broken down into

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