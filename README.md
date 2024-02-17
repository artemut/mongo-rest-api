# Mongo.RestApi

A self-hosted API that provides HTTP endpoints for finding, inserting, updating, and deleting documents in mongo.

**Important:** no authentication is currently implemented, so use it for local development only.

## Use cases / motivation of the project

The primary motivation is to facilitate communication between client-side applications and Mongo databases through HTTP(s) calls, as MongoDB lacks a built-in REST API.

There might be reasons why you wouldn't want to construct a server-side API to abstract the database from your client applications:

1. If you're developing a new web/mobile application and anticipate frequent changes to data models and operations, it may be reasonable to defer the backend development and interact directly with MongoDB. Later, once you have stable data models and a clear understanding of your data operations, you can build your own backend APIs and refactor client applications to use them.

2. If you're creating a local website or browser extension for personal or family purposes, and security is not a concern, you may prefer to avoid building a backend API.

## Installation

### Option 1. Clone the repository (any OS)

You can clone the repository to your local machine, and then build and run the solution. Make sure your OS has [ASP.NET Core Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) installed (version 6.xx and higher).

```
dotnet build src\Mongo.RestApi\Mongo.RestApi.csproj
dotnet run --project src\Mongo.RestApi\Mongo.RestApi.csproj
```

Now, service is running on port 5600: http://localhost:5600/

### Option 2. Use MSI installer (Windows only)

You can download the installer [Mongo.RestApi_1.1.msi](installers/Mongo.RestApi/Mongo.RestApi_1.1.msi) and run it on your Windows machine. It will install a windows service **Mongo.RestApi** so that the API is always running in the background on port 5600: http://localhost:5600/


#### If port 5600 is already taken

The port 5600 is the default port that the API will use. If it's already taken, please update the settings file `C:\Program Files\artemut\Mongo.RestApi\appsettings.json` so that another free port is used instead:

```json
{
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://localhost:5600" // change the port
      }
    }
  }
}
```

Make sure to restart **Mongo.RestApi** windows service after making changes to the `appsettings.json`.

## Configuration

By default, the API is already configured with a connection string to the local mongodb deployment, so you don't have to make any additional configurations if you're going to use local mongo.

But, if needed, you can change or add connection strings to the `appsettings.json` file. Example:

```json
{
  "ConnectionStrings": {
    "local": "mongodb://127.0.0.1:27017",
    "other_host": "mongodb://remote-host:27017"
  }
}
```

If you used MSI installation, you can find `appsettings.json` at `C:\Program Files\artemut\Mongo.RestApi\appsettings.json`.
Make sure to restart **Mongo.RestApi** windows service after making changes to the `appsettings.json`.

## Endpoints

There are 4 endpoints:
- Find: `POST /{connectionName}/{databaseName}/{collectionName}/find`
- Insert: `POST /{connectionName}/{databaseName}/{collectionName}/insert`
- Update: `POST /{connectionName}/{databaseName}/{collectionName}/update`
- Delete: `POST /{connectionName}/{databaseName}/{collectionName}/delete`

Each endpoint's URL contains values of:
- `connectionName` - use **local** to issue requests to the local mongodb
    - however, it's possible to use other replica sets or dedicated mongo servers in the URL. Make sure to update `appsettings.json` with required connection strings (see **Configuration** section above for more details). Then, the `connectionName` parameter should correspond to the connection string name in the `appsettings.json`
- `databaseName` - name of the mongo database
- `collectionName` - name of the mongo collection

### Find

`POST /{clusterName}/{databaseName}/{collectionName}/find`

Request body mimics [**find**](https://www.mongodb.com/docs/manual/reference/command/find/) database command.

The difference, however, is that unlike the mongo [**find**](https://www.mongodb.com/docs/manual/reference/command/find/) command, the API **find** endpoint returns all the data that satisfies the given **filter**. It invokes mongo [**getMore**](https://www.mongodb.com/docs/manual/reference/command/getMore/) command internally to load all the remaining batches and then returns the resulting documents.

*Example:*

```json
{
  "filter": {
    "productCategory": 123,
    "$or": [
      { "availableBegin": { "$gte": { "$date": "2023-11-01T00:00:00.000Z" } } },
      { "alwaysAvailable": true }
    ]
  },
  "sort": { "price": -1 },
  "projection": { "productDetails": -1 },
  "skip": 100,
  "limit": 50
}

```

### Insert

`POST /{clusterName}/{databaseName}/{collectionName}/insert`

Request body mimics [**insert**](https://www.mongodb.com/docs/manual/reference/command/insert/) database command.

*Example:*

```json
{
  "documents": [
    {
      "_id": 123,
      "name": "test123",
      "date": { "$date": "2023-11-09T00:00:00.000Z" }
    },
    {
      "_id": 124,
      "name": "test124",
      "date": { "$date": "2023-11-10T00:00:00.000Z" },
      "details": {
        "description": "bla"
      }
    }
  ]
}
```

### Update

`POST /{clusterName}/{databaseName}/{collectionName}/update`

Request body mimics [**update**](https://www.mongodb.com/docs/manual/reference/command/update/) database command.

*Example:*

```json
{
  "updates": [{
      "q": { "_id": 1 },
      "u": { "$set": { "name": "updated_name" } }
  },{
      "q": { "_id": 2 },
      "u": { "name": "name_of_the_entirely_replaced_document" }
  },{
      "q": { "_id": 3 },
      "u": { "name": "name_of_the_new_document" },
      "upsert": true
  }]
}
```

### Delete

`POST /{clusterName}/{databaseName}/{collectionName}/delete`

Request body mimics [**delete**](https://www.mongodb.com/docs/manual/reference/command/delete/) database command.

*Example:*

```json
{
  "deletes": [{
      "q": { "_id": 123 }
  }]
}
```