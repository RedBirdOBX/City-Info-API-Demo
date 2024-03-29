# The City Info Demo API
  
----------
*Version 1.16.0*

## Summary
Welcome to the City Info Demo API. Imagine that you were developing for some kind of travel site and one of the requirements was you needed to be able to ask for a complete listing of cities; ask for any given city by it's ID and, if specifically asked for, you needed to be able to provide all the "touristy" things to do for that specified city (landmarks, parks, restaurants, and so on).  

This demo RESTful API does just that. It allows consumers to make request for USA Cities and their known "Points of Interest" (tourist attractions). 

It supports and demonstrates all HTTP verbs: GET, POST, PUT, PATCH, and DELETE.


`https://city-info-api-prod.azurewebsites.net/api/v1.0/cities`

## Platform: 
- ASP.NET Core 2.2 API  
- Entity Framework Core
- Swashbuckle.AspNetCore 5 RC2
- AutoMapper 8.1
- NLog 4.8  
- Microsoft.AspNetCore.Mvc.Api.Analyzers 2.2.0
- Microsoft Azure cloud services   
- Microsoft.AspNetCore.Mvc.Versioning 3.1.3
- Microsoft.AspNetCore.Mvc.Versioning.ApiExplorer 3.2.0
- AspNetCoreRateLimit 2.1.0


## Chapters
- [How To Test](#test)
- [Endpoints](#endpoints)
- [Supported Media Types](#content)
- [Logging](#logging)
- [CICD](#cicd)
- [Architecture](#architecture)
- [Versions](#versions)
- [HATEOS](#hateos)
- [Authentication](#authentication)
- [Throttling](#throttling)
- [Swagger and Documentation](#swagger)
- [Releases](#releases)
 

<a href="" id="test" name="test"></a>
## How To Test
One easy way to test this API is to download and use Postman, a popular API development testing tool. It can be downloaded here: [https://www.getpostman.com/](https://www.getpostman.com "https://www.getpostman.com") .  In this repository, you will find a Postman Collection in a folder named `Postman-Collection`.  The collection has most, if not all, test requests used by this API. It contains many GET, POST, PUT, PATCH, and DELETE requests. 

You can `import` this collection into your Postman application for ease of testing.  Or, you can simply manually call the endpoints below in your Postman application.

The Postman collection in this repo uses Postman "Global Variables" which you will notice in the URL of the request.

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/global-variable-example1.png)

You can access these variables in Postman by clicking this icon:

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/postman-global-variables-icon.png)

However, you should not need to change these.  To indicate which instance of this API you wish to test (locally running, DEV, or PROD), just change the variable name in the URL.

Valid options:  
`{{domain-local}}`  
`{{domain-dev}}`  
`{{domain-prod}}`  


<a href="" id="endpoints" name="endpoints"></a>
## Endpoints

 ![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/screenshot.PNG)

### Cities

##### Get Cities 
[https://city-info-api-prod.azurewebsites.net/api/v1.0/cities?pagenumber={n}&pagesize={n}](https://city-info-api-prod.azurewebsites.net/api/v1.0/cities?pagenumber={n}&pagesize={n} "https://city-info-api-prod.azurewebsites.net/api/v1.0/cities?pagenumber={n}&pagesize={n}")   
`GET`   
This resource with return a collection of all cities but does not show you their associated points of interest. 

**Paging:**
This endpoint implements paging and you **must** specify the page number (`pagenumber`) you are requesting **and** the number of results per page (`pagesize`) in the querystring. These two parameters are required.

Both parameters have default values should the consumer forget to provide them and minimum and maximum limits should the consumer exceed those limits.  If the limits are exceed, it will will fall back to default values.

| Parameter  | Default | Min Value | Max Value |
| ---------  |---------| --------- | ----------|
| pageNumber | 1       | 1         | n/a       |
| pageSize   | 10      | 1         | 10        |


**Optional Name Filtering**   
An optional parameter you can provide in this request is a Name filter which looks like this:
`http://city-info-api-prod.azurewebsites.net/api/v1.0/cities?pageNumber=1&pageSize=10&nameFilter=chica`

In this example, it will return up to 10 results per page for any city with a name containing "chica" such as Chicago.  It is not case-sensitive.


**Optional Name Sorting**  
Another optional parameter you can provide is `orderNameBy` and if the consumer provides `desc` as the value, it will sort the City names by descending order.  Any other value other than `desc` will result in the names being sort in ascending order. 
`http://city-info-api-prod.azurewebsites.net/api/v1.0/cities?pageNumber=1&pageSize=10&orderNameBy=desc`


**Custom Response Header:**   
The Response Header will provide the consumer helpful information in a custom item known as `X-CityParameters`.  It returns links to the next page (if applicable), previous page (if applicable), the name filter (if used), the name sorting order, and total city count.    
![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/x-cityparameters.png)




##### Get City By Id  
[http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}](http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId} "http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}")   
[http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}?includepointsofinterest=false](http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}?includepointsofinterest=false "http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}?includepointsofinterest=false")   
`GET`    
Here, you can request a specific city and also provide an optional query string parameter to explicitly request the Points Of Interest along with the City data. If false, the points of interest collection will be intentionally empty (to lighten payload).  Otherwise, they will be included by default. 

The id along with all other ids are guids.

##### Get Collection of Cities by Ids  
[http://city-info-api-prod.azurewebsites.net/api/v1.0/citycollections?cityIds={a,b,c}](http://city-info-api-prod.azurewebsites.net/api/v1.0/citycollections?cityIds={a,b,c} "http://city-info-api-prod.azurewebsites.net/api/v1.0/citycollections?cityIds={a,b,c}")     
`GET`    
Here, you can request a collection of cities by providing their Ids in the querystring.  Invalid ids and duplicates will be ignored. This uses the `CityCollections` resources.

##### Create a City
[http://city-info-api-prod.azurewebsites.net/api/v1.0/cities](http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/ "http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/")  
`POST`

Here, you can `POST` (in the body) the `json` structure of a new city.  Optionally, you can provide a list of Points of Interest (children) and they will be created as well.

Without Points of Interest

```json
{
	"name" : "Gotham",
	"description" : "Gotham city - sister city of Metropolis"
}
```

With Points of Interest

```json
{
	"name" : "Gotham",
	"description" : "Gotham city - sister city of Metropolis",
	"pointsOfInterest" : 
	[
		{
			"name": "Wayne Tower",
			"description": "Where stuff gets done."
		},
		{
			"name": "Arkham Asylum",
			"description": "You probably don't want to visit this."
		}
	]
}
```

##### Create Multiple Cities
[http://city-info-api-prod.azurewebsites.net/api/v1.0/citycollections](http://city-info-api-prod.azurewebsites.net/api/v1.0/citycollections/ "http://city-info-api-prod.azurewebsites.net/api/v1.0/citycollections/")  
`POST`

Here, you can `POST` (in the body) as `json`, an array of cities. If successful, you will receive a `201-CreatedAtRoute` response and the link to find the new cities will be in the Response Header.

```json
[
	{
		"name" : "New City B",
		"description" : "New city description B"
	},
	{
		"name" : "New City C",
		"description" : "New city description C"
	}
]
```

##### Patch a City
[http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}](http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId} "http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}")  
`PATCH`  
Instead of updated the city whole resource, you can use a patch document and only update one or more properties of the resource such as passwords, emails, and so on. With this API, you can use a standard patch document and specify what part of the resource you want to update.

```json
[
	{
		"op": "replace",
		"path": "/name",
		"value": "updated name"
	},
	{
		"op": "replace",
		"path": "/description",
		"value": "updated description 2"
	}
]
```

If successful, it will return a 200 OK status and the new updated resource in the body.


### Points Of Interest

##### Get Points of Interest for City  
[http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest](http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest "http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest")  
`GET`  
You can request to see a collection of the Points of Interest for any given city by Id.

##### Get Point of Interest By Id  
[http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}](http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId} "http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}")  
`GET`  
At this endpoint, you can request a specific Point of Interest for a specific City assuming you know the Ids of both resources. 


##### Create a Point of Interest  
[http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest](http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest "http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest")  
`POST`  
As a security measure, a city cannot have more than 25 Points of Interest. Assuming the city is under the limit, you can create a new Point of Interest for a valid city (by providing it's id). You will need to provide a name and a description as JSON data in the body of the POST.  Like so:

```json
{
	"name": "Gino's Famous Pizza",
	"description": "Known all over the world for it's famous NY style pizza"
}
```

 If your post is successful, it will return a `201 Created Status`, and the id of the new Point of Interest.  
![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/successful-post.PNG)

Furthermore, it will return the location of this new resource in the Header of the response.
![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/successful-post-2.PNG)


##### Update a Point of Interest
[http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}](http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId} "http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}")  
`PUT`  
This the endpoint where you can update an entire Point of Interest resource. You do this through a PUT and provide the whole Point of Interest with it's new values.

If successful, it will return you a 200 Success status and the values of the updated resource.
![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/successful-put.PNG)


##### Patch a Point of Interest
[http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}](http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId} "http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}")  
`PATCH`  
Instead of updated the whole resource, you can use a patch document and only update one or more properties of the resource such as passwords, emails, and so on. With this API, you can use a standard patch document and specify what part of the resource you want to update.

```json
[
	{
		"op": "replace",
		"path": "/description",
		"value": "Rico's world famous restaurant."
	}
]
```

If successful, it will return a 200 OK status and the new updated resource in the body.
![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/successful-patch.PNG)

##### Delete a Point of Interest
[http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}](http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId} "http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}")  
`DELETE`  
By providing a proper City id and a Point of Interest id, you can delete a resource from the data store.  This functionality would rarely make it to production like this but here is a demonstration none the less.

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/successful-delete.PNG)

If successful, it will return a 200 OK status and a message in the body.

##### City Summary Reporting Data
[http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/reporting/summary](http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/reporting/summary "http://city-info-api-prod.azurewebsites.net/api/v1.0/cities/reporting/summary")  
`GET`  
Version **2.0** Resource.  This endpoint provides a list of all cities the count of points of interest for each city.

   
Sample Response: 

```json  
[  
    {   
        "name": "Chicago",  
        "numberOfPointsOfInterest": 2  
    },  
    {  
        "name": "Dallas",  
        "numberOfPointsOfInterest": 3  
    },  
    {  
        "name": "Gotham",  
        "numberOfPointsOfInterest": 2  
    },
	...etc...     
]   
```

This resource requires authorization and you must pass Authorization as part of the request header.  A sample request looks something like this:

```
GET /api/v2.0/cities/reporting/summary HTTP/1.1  
Host: localhost:5000  
Authorization: Basic Q2l0eUluZm9BUEk6Q2l0eUluZm9BUElQYXNzd29yZA==  
cache-control: no-cache  
Postman-Token: 46403ff1-b551-40a3-bead-ba82d0b6ef54  
```

The base64 encoded string is made of a username of `CityInfoAPI` and a password of `CityInfoAPIPassword`.  This produces this key" `Q2l0eUluZm9BUEk6Q2l0eUluZm9BUElQYXNzd29yZA==`.  If you wish to test this endpoint manually through Swagger UI, you will need to manually provide these credentials.

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/auth-sample.PNG)

<a href="" id="content" name="content"></a>
## Support Media Types 

This demo API can return either JSON data or Xml Data (via content negotiation) depending on the Accept parameter you provide in your request header. JSON is the default if nothing is provided or an invalid type is provided in the request.

**JSON**
![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/json-content-type.PNG)

**Xml**
![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/xml-content-type.PNG)

If you provide an **unsupported** media type in the request, it will, by design, give you a `406 Not Acceptable` response.  
![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/not-acceptable-error.PNG)

Furthermore, it will also accept `Xml` and the Content-Type if specified. You can if needed, `POST` data with `Xml` instead of `JSON`.   

```xml
<CityCreateDto xmlns:i="http://www.w3.org/2001/XMLSchema-instance" xmlns="http://schemas.datacontract.org/2004/07/CityInfoAPI.Dtos.Models">
    <Description>Description for Alpha City</Description>
    <Name>New Alpha City</Name>
    <PointsOfInterest>
        <PointOfInterestCreateDto>
            <Description>Description for location 1</Description>
            <Name>Location 1</Name>
        </PointOfInterestCreateDto>
        <PointOfInterestCreateDto>
            <Description>Description for location 2</Description>
            <Name>Location 2</Name>
        </PointOfInterestCreateDto>
    </PointsOfInterest>
</CityCreateDto>
```

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/development/Images/post-with-xml.png)


<a href="" id="logging" name="logging"></a>
## Logging 
This API uses NLog ([https://nlog-project.org/](https://nlog-project.org/ "https://nlog-project.org/")) for logging.  It is currently set up to log to local text files and to the console. As a demonstration, a log entry is created when you request a city by id that doesn't exist.

`http://localhost:5000/api/cities/123`

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/city-not-found.PNG)

The `CitiesController` logs to the logger service (NLog) and it outputs to the console:

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/console-output.PNG)

... as well as the log file:

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/log-file.PNG)


<a href="" id="cicd" name="cicd"></a>
## CICD
This API is built with CICD (Continuous Integration, Continuous Delivery). As new features or fixes are accepted into the `development` branch, they are automatically deployed to the development instance of this app.  As feaures and fixes are promoted up to the `master` branch, they are once again automatically delivered into the production version of this app.  No publishing or FTP-ing of files is required.  Life is good.

Development URI*: [https://city-info-api-prod-dev.azurewebsites.net/](https://city-info-api-prod-dev.azurewebsites.net/ "https://city-info-api-prod.azurewebsites.net/")  
Production URI: [https://city-info-api-prod.azurewebsites.net/](https://city-info-api-prod.azurewebsites.net/} "https://city-info-api-prod.azurewebsites.net/")

**sometimes turned off to reduce hosting costs.*

<a href="" id="architecture" name="architecture"></a>
## Architecture  
This API is designed with what I consider to be, a sound and proper architectural design.  It has multiple layers (projects) and each layer has a single responsibility. 

**Data Layer**
This is the most protected layer.  Nothing interacts with this layer other than the Logic layer. This data is never exposed to the client/consumer site via Controllers.  This layer contains all of the Entity Framework plumbing, classes defining all of the Entities, and data Repositories - where **all** the data persistence resides.

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/data-layer.PNG)

**DTO Layer**
This layer contains all of the DTOs for this app.  These DTOs represent data that is either returned by the API or received by the consumer of the API.  Data from the data layer is never directly returned or received.  The Logic layer of this app will call the appropriate Repository and map the results to correct DTO so it can be returned to the consumer or processed by the logic layer.

Every form of data interaction (read, write) is represented by a DTO in this layer.

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/dto-layer.PNG)

**Logic Layer**
This is the business layer.  All requests from the Controllers go through this layer. Controllers do not directly interact with the Repositories from the Data Layer. This is where any and all business logic with reside in addition to all data mapping.

1. Request from Controller 
1. goes through Processor 
1. Processor calls Repository 
1. maps entity data to DTO 
1. returns results to Controller

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/processor-layer.PNG)

**Web API Layer**
This is the layer exposed to the public. This layer contains all the typical MVC stuff - Controllers, Views (if any), Services (middleware), appSettings, and static content (in wwwroot).

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/webapi-layer.PNG)
  

<a href="" id="versions" name="versions"></a>
## Versions
This API supports versioning. All of the standard endpoints and resources are exposed for a hypothetical public-facing API: getting cities, getting a specific city, getting a city's points of interest, updating a point of interest, and so on. These all fall under Version 1.0 of this API.  

Now if this were a real API, imagine if the owners of this API needed a Version 2.0 that supported administrative functions such as reporting.  To demonstrate that scenario, another resource was created (described above) called 
`CitySummary`. This resource is only supported by the 2.0 version of this API. 

This demonstrates that an entire collection of resources can be contained in one version and an entirely different set of resources can be included in another version.


<a href="" id="hateos" name="hateos"></a>
## HATEOS

Adhering to the HATEOS principles of good RESTful design, all `GET` requests include media navigational links in the response to inform the user on how else they can consume the requested resource.

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/development/Images/media-links.png)
  

<a href="" id="authentication" name="authentication"></a>
## Authentication
A basic demonstration of authentication and security was implemented on the `City Summary Reporting Data` resource mentioned above. The concept is that in some real-world(ish) scenario, you would want to secure certain administrative resources like reporting data or POST actions.  This `CitySummary` resource in V2 demonstrates that by using Basic Authorization. 

`https://city-info-api-prod.azurewebsites.net/api/v2.0/cities/reporting/summary`
![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/auth-sample.PNG)

In order to access the V2 resource which requires authentication, you must pass along an Authentication parameter in the Request Header and the value will be `Basic Q2l0eUluZm9BUEk6Q2l0eUluZm9BUElQYXNzd29yZA==`.

<a href="" id="throttling" name="throttling"></a>
## Throttling 
We have the ability to implement all kinds of throttling on this API.  
We can limit calls per minute or even per second. We can add IPs to a blacklist or 
only allow requests from a whitelist.  We can even limit requests per endpoint. 
We accomplish this by using the `AspNetCoreRateLimit` 
package ([https://github.com/stefanprodan/AspNetCoreRateLimit](https://github.com/stefanprodan/AspNetCoreRateLimit)).

Just for demonstration purposes, this API has been set up to only accept 30 total requests per minute.  
To inform the consumer how many requests are left and when the limit is reset, 
the Response Header contains a series of `X-Rate-Limit` headers which tell the consumer just that.

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/development/Images/throttling-header-items.png) 

Once the limit has been exceeded, the API will return a `429-Too many requests` status code as well 
as a `API calls quota exceeded! maximum admitted 10 per 1m.` error message in the body.  
The Response Header will also inform the consumer when they can retry (custom header item `Retry-After`) 
their request (in seconds).

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/development/Images/throttling-header-limit-exceeded.png)


<a href="" id="swagger" name="swagger"></a>
## Swagger and Documentation

This API is fully documented under the OpenAPI 3 standards.  It is using Swashbuckle for ASP.NET Core which includes the Swagger/API Explorer, Swagger UI, and Swagger CodeGen SDKs.


The Controllers and types in which they return are fully documented and notated per Open API 3 standards.  Doing this allow the API Explorer to "explore" this application and create specification documents ("swagger docs").

You can see all of this documentation via the Swagger UI interface.

**Local:**  
[http://localhost:5000/index.html](http://localhost:5000/index.html)


**Development:**  
[https://city-info-api-prod.azurewebsites.net/index.html](https://city-info-api-dev.azurewebsites.net/index.html)

Notice that you can access the various specification documents - one for each version:

[https://city-info-api-prod.azurewebsites.net/swagger/CityAPISpecificationv1.0/swagger.json](https://city-info-api-prod.azurewebsites.net/swagger/CityAPISpecificationv1.0/swagger.json)

[https://city-info-api-prod.azurewebsites.net/swagger/CityAPISpecificationv2.0/swagger.json](https://city-info-api-prod.azurewebsites.net/swagger/CityAPISpecificationv2.0/swagger.json)


<a href="" id="releases" name="releases"></a>
## Release Versions

**1.0.0**  
7.1.2019  
Initial release  

**1.1.0**  
9.4.2019  

* Full integration of Swashbuckle (Swagger, Swagger UI), versioning, and Basic Authentication on new resource called City Summary - a hypothetical secured, set of resources at `https://city-info-api-prod.azurewebsites.net/api/v2.0/cities/reporting/summary`.

**1.2.0**  
11.10.2019  

* Replaced all database record identifiers in routes with guids.  Now, regardless of where or how the data is stored, the identifiers will always remain the same. 

**1.3.0**  
11.15.2019  
* Added `try/catches` to all Controller actions.  

**1.4.0**  
12.9.2019   

Added new resources:

* Create City method was added  
* Create City and n-number of Points of Interest in a single post was added  
* Create multiple Cities with one post was added 

**1.5.0**  
12.18.2019

* Now accepts `xml` as content type for input.  As a test, a new Postman request was added to the collection.  It is labeled as 'Local - POST Create City with Xml'.  With this test case, you can post to this resource with valid xml and it will be processed just as it is with json.
* Prevents `POST` requests for cities and points of interest with provided Ids. The user should never be using POST to a city or point of interest resource with an Id in the route.  These are for `PUT` and `PATCH` only. Methods were added to the controllers to prevent this.
* If a `guid` is excluded in the body of a `POST` (create Point Of Interest) for example, it will still pass standard validation since it is invoked as an "empty guid" - a GUID type cannot be null. The `Create Point of Interest` resource was updated to check for empty guids. Prevents empty/excluded guids from being posted

**1.6.0**  
1.7.2020

* Added `CreatedOn` properties to all output Dtos.
* Created a `UpdateCity` action in CityController with uses a `PATCH` transaction. 
* Improved Model Validation code in Controllers.  Removed unnecessary code Controller actions as improvements made to the .NET Core Framework eliminate the need for this code such as checking the ModelState as one of the first actions in the method.  If the validation rules fail, it will **never** even invoke the controller action. 
* Cities cannot be created or updated with same name and description.
* Duplicate cities can no longer be added.
* Removed need for city guids in posts/puts/patches.  All guids will now be pulled from the route and are not required in any POST body as it was redundant.
* The create DTOs are now responsible for creating the GUIDs - not the database or entity. 


**1.7.0**  
1.8.2020  

* Updated Postman collection to use Global Variables to store the different domains (local, development, and production). Now, we have just one set of requests in our Postman collection instead of three.  See [#test](#test "How To Test").  

**1.8.0**  
1.17.2020  
  
* Converted all Controller, Processor, and Repository methods to be `asynchronous`. Controller actions and processor methods return `Task<T>` and calls to these services are annotated with `async` and `await`.


**1.8.1**  
1.30.2019 
 
* Added more test cities in the `in memory datastore`. More will be needed for the upcoming pagination development.
* Minor logging improvements and general clean up.
* Added pull request template.

**1.9.0**  
2.3.2020 
 
* Add paging to `GetCities` endpoint.  
* Updated Postman collection.

**1.9.1**  
2.6.2020  

* Add custom paging meta data to the response header.  Tell consumer if there is a previous page, if there is a next page, and how many total results there are. Custom Header item is known as `X-Pagination`.

**1.10.0**  
2.10.2020

* Name filtering was introduced. You can now filter on city names.

**1.11.0**  
2.12.2020
  
* If user wanted to receive cities in descending order, user can provide an optional querystring parameter to receive city names in descending order.  
* Added environmental variable check for local instances.  If the app is running locally (defined by Configuration variable), the in-memory datastore will be loaded.  If running in dev or prod environment, the sql database will be used as datastore.
* Renamed the custom response header item from `X-Pagination` to `X-CityParameters` since this serialized metadata contains more than just paging information.  Also contains total count, possible filters and possible sorting parameters.


**1.12.0**  
2.18.2020  

* Following HATEOS principles, added media links in all GET responses.  Assists consumer on how to navigate through the API. 
* All DTOs used for GET requests now inherit an abstract `LinkDto` class.  Helper utility written to populate list of links for these responses.


**1.13.0**  
2.19.2020  

* Using the 3rd party package `AspNetCoreRateLimit`, we now can limit the number of requests to any given endpoint, for any given resource, on any given interval.  For demonstration purposes, the two policies are now in place: 
 * you can only make 15 requests per minute
 * you can only make 3 requests per 5 seconds  
 
**1.14.0**   
2.26.2020

* Integrated the use of Azure Key Vault services for db connectivity. 

**1.15.0**   
7.29.2020 
* Enabled CORS for local testing. 

**1.16.0**  
5.3.2021  
Removed Azure KeyVault services and implementation.  Over-engineered.  Want to keep is simple. Removed: 
* Microsoft.Azure.Services.AppAuthentication 1.4.0
* Microsoft.Azure.KeyVault 3.0.5

