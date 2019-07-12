# The City Info Demo API



## Summary
Welcome to the City Info Demo API. Imagine that you were developing for some kind of travel site and one of the requirements was you needed to be able to ask for a complete listing of cities; ask for any given city by it's ID and, if specifically asked for, you needed to be able to provide all the "touristy" things to do for that specified city (landmarks, parks, restaurants, and so on).  

This demo RESTFul API does just that. It allows consumers to make request for USA Cities and their known "Points of Interest" (tourist attractions). 

It supports and demonstrates all HTTP verbs: GET, POST, PUT, PATCH, and DELETE.


`https://city-info-api-demo.azurewebsites.net/api/cities`

## Platform: 
- ASP.NET Core 2.2 API  
- Entity Framework Core
- Swashbuckle.AspNetCore 5 RC2
- AutoMapper 8.1
- NLog 4.8  
- Microsoft.AspNetCore.Mvc.Api.Analyzers 2.2.0
- Microsoft Azure cloud services   


## Chapters
- [How To Test](#test)
- [Endpoints](#endpoints)
- [Support Media Types](#content)
- [Logging](#logging)
- [CICD](#cicd)
- [Architecture](#architecture)


<a href="" id="test" name="test"></a>
## How To Test
One easy way to test this is to download Postman, a popular API development testing tool. It can be downloaded here:  `https://www.getpostman.com/`.  In this repository, you will find a Postman Collection in a folder named `Postman-Collection`.  The collection has all requests URLs pointing to `localhost:0000` but you can simply update this part of the URL to `city-info-api-demo.azurewebsites.net` if you do not wish to test locally.

You can `import` this collection into your Postman application for ease of testing.  Or, you can simply manually call the endpoints below in your Postman application.


<a href="" id="endpoints" name="endpoints"></a>
## Endpoints

 ![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/screenshot.PNG)


### Get All Cities  
`https://city-info-api-demo.azurewebsites.net/api/cities`  
`GET`   
This resource with return a collection of all cities but does not show you their associated points of interest.

### Get City By Id  
`http://city-info-api-demo.azurewebsites.net/api/cities/{cityId}`  
`http://city-info-api-demo.azurewebsites.net/api/cities/{cityId}?includepointsofinterest=false`   
`GET`    
Here, you can request a specific city and also provide an optional query string parameter to explicitly request the Points Of Interest along with the City data. If false, the points of interest collection will be intentionally empty (to lighten payload).  Otherwise, they will be included by default. 

### Get Points of Interest for City  
`http://city-info-api-demo.azurewebsites.net/api/cities/{cityId}/pointsofinterest`  
`GET`  
You can request to see a collection of the Points of Interest for any given city by Id.

### Get Point of Interest By Id  
`http://city-info-api-demo.azurewebsites.net/api/cities/{cityId}/pointsofinterest/{pointOfInterestId}`  
`GET`  
At this endpoint, you can request a specific Point of Interest for a specific City assuming you know the Ids of both resources. 


### Create a Point of Interest  
`http://city-info-api-demo.azurewebsites.net/api/cities/{cityId}/pointsofinterest`  
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


### Update a Point of Interest
`http://city-info-api-demo.azurewebsites.net/api/cities/{cityId}/pointsofinterest/{pointOfInterestId}`  
`PUT`  
This the endpoint where you can update an entire Point of Interest resource. You do this through a PUT and provide the whole Point of Interest with it's new values.

If successful, it will return you a 200 Success status and the values of the updated resource.
![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/successful-put.PNG)


### Patch a Point of Interest
`http://city-info-api-demo.azurewebsites.net/api/cities/{cityId}/pointsofinterest/{pointOfInterestId}`  
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

### Delete a Point of Interest
`http://city-info-api-demo.azurewebsites.net/api/cities/{cityId}/pointsofinterest/{pointOfInterestId}`  
`DELETE`  
By providing a proper City id and a Point of Interest id, you can delete a resource from the data store.  This functionality would rarely make it to production like this but here is a demonstration none the less.

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/successful-delete.PNG)

If successful, it will return a 200 OK status and a message in the body.

<a href="" id="content" name="content"></a>
## Content Negotiation 

This demo API can return either JSON data or Xml Data depending on the Accept parameter you provide in your request header.  JSON is the default if nothing is provided or an invalid type is provided in the request.

**JSON**
![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/add-swashbuckle/Images/json-content-type.PNG)

**Xml**
![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/add-swashbuckle/Images/xml-content-type.PNG)

If you provide a content media type in the request, it will, by design, give you a `406 Not Acceptable` response.
![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/add-swashbuckle/Images/not-acceptable-error.PNG)



<a href="" id="logging" name="logging"></a>
## Logging 
This API uses NLog (`https://nlog-project.org/`) for logging.  It is currently set up to log to local text files and to the console. As a demonstration, a log entry is created when you request a city by id that doesn't exist.

`http://localhost:5000/api/cities/123`

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/city-not-found.PNG)

The `CitiesController` logs to the logger service (NLog) and it outputs to the console:

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/console-output.PNG)

... as well as the log file:

![](https://github.com/RedBirdOBX/City-Info-API-Demo/blob/master/Images/log-file.PNG)


<a href="" id="cicd" name="cicd"></a>
## CICD
This API is built with CICD (Continuous Integration, Continuous Delivery). As new features or fixes are accepted into the `development` branch, they are automatically deployed to the development instance of this app.  As feaures and fixes are promoted up to the `master` branch, they are once again automatically delivered into the production version of this app.  No publishing or FTP-ing of files is required.  Life is good.

Development URI : `https://city-info-api-demo-dev.azurewebsites.net/api`  
Production URI : `https://city-info-api-demo.azurewebsites.net/api`

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
  

## Swagger

http://localhost:5000/index.html

http://localhost:5000/swagger/OpenAPISpecification/swagger.json