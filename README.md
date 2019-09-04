# The City Info Demo API


----------


## Summary
Welcome to the City Info Demo API. Imagine that you were developing for some kind of travel site and one of the requirements was you needed to be able to ask for a complete listing of cities; ask for any given city by it's ID and, if specifically asked for, you needed to be able to provide all the "touristy" things to do for that specified city (landmarks, parks, restaurants, and so on).  

This demo RESTful API does just that. It allows consumers to make request for USA Cities and their known "Points of Interest" (tourist attractions). 

It supports and demonstrates all HTTP verbs: GET, POST, PUT, PATCH, and DELETE.


`https://city-info-api-demo.azurewebsites.net/api/v1.0/cities`

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


## Chapters
- [How To Test](#test)
- [Endpoints](#endpoints)
- [Support Media Types](#content)
- [Logging](#logging)
- [CICD](#cicd)
- [Architecture](#architecture)
- [Versions](#versions)
- [Authentication](#authentication)
- [Swagger and Documentation](#swagger)
- [Releases](#releases)
 

<a href="" id="test" name="test"></a>
## How To Test
One easy way to test this is to download Postman, a popular API development testing tool. It can be downloaded here: [https://www.getpostman.com/](https://www.getpostman.com/ "https://www.getpostman.com/") .  In this repository, you will find a Postman Collection in a folder named `Postman-Collection`.  The collection has all requests URLs pointing for local, devlopment, and product urls. 

You can `import` this collection into your Postman application for ease of testing.  Or, you can simply manually call the endpoints below in your Postman application.


<a href="" id="endpoints" name="endpoints"></a>
## Endpoints

 ![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/screenshot.PNG)


#### Get All Cities 
[https://city-info-api-demo.azurewebsites.net/api/v1.0/cities](https://city-info-api-demo.azurewebsites.net/api/v1.0/cities "https://city-info-api-demo.azurewebsites.net/api/v1.0/cities")   
`GET`   
This resource with return a collection of all cities but does not show you their associated points of interest.

#### Get City By Id  
[http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}](http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId} "http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}")   
[http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}?includepointsofinterest=false](http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}?includepointsofinterest=false "http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}?includepointsofinterest=false")   
`GET`    
Here, you can request a specific city and also provide an optional query string parameter to explicitly request the Points Of Interest along with the City data. If false, the points of interest collection will be intentionally empty (to lighten payload).  Otherwise, they will be included by default. 

#### Get Points of Interest for City  
[http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest](http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest "http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest")  
`GET`  
You can request to see a collection of the Points of Interest for any given city by Id.

#### Get Point of Interest By Id  
[http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}](http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId} "http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}")  
`GET`  
At this endpoint, you can request a specific Point of Interest for a specific City assuming you know the Ids of both resources. 


#### Create a Point of Interest  
[http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest](http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest "http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest")  
`POST`  
As a security measure, a city cannot have more than 25 Points of Interest. Assuming the city is under the limit, you can create a new Point of Interest for a valid city (by providing it's id). You will need to provide a name and a description as JSON data in the body of the POST.  Like so:

```
{
	"name": "Gino's Famous Pizza",
	"description": "Known all over the world for it's famous NY style pizza"
}
```

 If your post is successful, it will return a `201 Created Status`, and the id of the new Point of Interest.  
![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/successful-post.PNG)

Furthermore, it will return the location of this new resource in the Header of the response.
![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/successful-post-2.PNG)


#### Update a Point of Interest
[http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}](http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId} "http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}")  
`PUT`  
This the endpoint where you can update an entire Point of Interest resource. You do this through a PUT and provide the whole Point of Interest with it's new values.

If successful, it will return you a 200 Success status and the values of the updated resource.
![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/successful-put.PNG)


#### Patch a Point of Interest
[http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}](http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId} "http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}")  
`PATCH`  
Instead of updated the whole resource, you can use a patch document and only update one or more properties of the resource such as passwords, emails, and so on. With this API, you can use a standard patch document and specify what part of the resource you want to update.

```
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

#### Delete a Point of Interest
[http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}](http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId} "http://city-info-api-demo.azurewebsites.net/api/v1.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}")  
`DELETE`  
By providing a proper City id and a Point of Interest id, you can delete a resource from the data store.  This functionality would rarely make it to production like this but here is a demonstration none the less.

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/successful-delete.PNG)

If successful, it will return a 200 OK status and a message in the body.

#### City Summary Reporting Data
[http://city-info-api-demo.azurewebsites.net/api/v2.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}](http://city-info-api-demo.azurewebsites.net/api/v2.0/cities/{cityId}/pointsofinterest/{pointOfInterestId} "http://city-info-api-demo.azurewebsites.net/api/v2.0/cities/{cityId}/pointsofinterest/{pointOfInterestId}")  
`GET`  
Version **2.0** Resource.  This endpoint provides a list of all cities the count of points of interest for each city.

   
Sample Response: 
```  
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

```GET /api/v2.0/cities/reporting/summary HTTP/1.1  
Host: localhost:5000  
Authorization: Basic Q2l0eUluZm9BUEk6Q2l0eUluZm9BUElQYXNzd29yZA==  
cache-control: no-cache  
Postman-Token: 46403ff1-b551-40a3-bead-ba82d0b6ef54  
```

The base64 encoded string is made of a username of `CityInfoAPI` and a password of `CityInfoAPIPassword`.  This produces this key" `Q2l0eUluZm9BUEk6Q2l0eUluZm9BUElQYXNzd29yZA==`.  If you wish to test this endpoint manually through Swagger UI, you will need to manually provide these credentials.

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/auth-sample.PNG)

<a href="" id="content" name="content"></a>
## Support Media Types 

This demo API can return either JSON data or Xml Data (content negotiation) depending on the Accept parameter you provide in your request header. JSON is the default if nothing is provided or an invalid type is provided in the request.

**JSON**
![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/json-content-type.PNG)

**Xml**
![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/xml-content-type.PNG)

If you provide an **unsupported** media type in the request, it will, by design, give you a `406 Not Acceptable` response.  
![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/not-acceptable-error.PNG)



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

Development URI : [https://city-info-api-demo-dev.azurewebsites.net/](https://city-info-api-demo-dev.azurewebsites.net/ "https://city-info-api-demo-dev.azurewebsites.net/")  
Production URI : [https://city-info-api-demo.azurewebsites.net/](https://city-info-api-demo.azurewebsites.net/} "https://city-info-api-demo.azurewebsites.net/")

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

## Logic Layer
This is the business layer.  All requests from the Controllers go through this layer. Controllers do not directly interact with the Repositories from the Data Layer. This is where any and all business logic with reside in addition to all data mapping.

1. Request from Controller 
1. goes through Processor 
1. Processor calls Repository 
1. maps entity data to DTO 
1. returns results to Controller

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/processor-layer.PNG)

## Web API Layer
This is the layer exposed to the public. This layer contains all the typical MVC stuff - Controllers, Views (if any), Services (middleware), appSettings, and static content (in wwwroot).

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/webapi-layer.PNG)
  

<a href="" id="versions" name="versions"></a>
## Versions
This API supports versioning. All of the standard endpoints and resources are exposed for a hypothetical public-facing API: getting cities, getting a specific city, getting a city's points of interest, updating a point of interest, and so on. These all fall under Version 1.0 of this API.  

Now if this were a real API, imagine if the owners of this API needed a Version 2.0 that supported administrative functions such as reporting.  To demonstrate that scenario, another resource was created (described above) called 
`CitySummary`. This resource is only supported by the 2.0 version of this API. 

This demonstrates that an entire collection of resources can be contained in one version and an entirely different set of resources can be included in another version.


<a href="" id="authentication" name="authentication"></a>
## Authentication
A basic demonstration of authentication and security was implemented on the `City Summary Reporting Data` resource mentioned above. The concept is that in some real-world(ish) scenario, you would want to secure certain administrative resources like reporting data or POST actions.  This `CitySummary` resource in V2 demonstrates that by using Basic Authorization. 

`https://city-info-api-demo-prod.azurewebsites.net/api/v2.0/cities/reporting/summary`
![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/auth-sample.PNG)




<a href="" id="swagger" name="swagger"></a>
## Swagger and Documentation

This API is fully documented under the OpenAPI 3 standards.  It is using Swashbuckle for ASP.NET Core which includes the Swagger/API Explorer, Swagger UI, and Swagger CodeGen SDKs.


The Controllers and types in which they return are fully documented and notated per Open API 3 standards.  Doing this allow the API Explorer to "explore" this application and create specification documents ("swagger docs").

You can see all of this documentation via the Swagger UI interface.

**Local:**  
[http://localhost:5000/index.html](http://localhost:5000/index.html)


**Development:**  
[https://city-info-api-demo-dev.azurewebsites.net/index.html](https://city-info-api-demo-dev.azurewebsites.net/index.html)

Notice that you can access the various specification documents - one for each version:

[https://city-info-api-demo-dev.azurewebsites.net/swagger/CityAPISpecificationv1.0/swagger.json](https://city-info-api-demo-dev.azurewebsites.net/swagger/CityAPISpecificationv1.0/swagger.json)

[https://city-info-api-demo-dev.azurewebsites.net/swagger/CityAPISpecificationv2.0/swagger.json](https://city-info-api-demo-dev.azurewebsites.net/swagger/CityAPISpecificationv2.0/swagger.json)


<a href="" id="releases" name="releases"></a>
## Release Versions

**1.0.0**  
7.1.2019  
Initial release  

**1.1.0**  
9.4.2019  
Full integration of Swashbuckle (Swagger, Swagger UI), versioning, and Basic Authentication on new resource called City Summary - a hypothetical secured, set of resources at `https://city-info-api-demo-prod.azurewebsites.net/api/v2.0/cities/reporting/summary`.
