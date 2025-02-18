# Blackwater Core Services

An open source collection of active & in-use services & utility functions by [Blackwater](https://clarknet.site/) written in C#.

Authentication, Encryption, and security logic / services have not been included for security reasons.

This repository is a C# class library and is meant to be used & referenced by a .NET Web API application. Some services may be dependant on the IConfiguration & IServiceCollection interfaces from .NET and may not work out the box.

## Service rundown

**HttpClientService** - A powerful and fast wrapper around the HttpClient class with automatically rotating proxies on rate limits.

**DatabaseService** - A lightweight wrapper around MongoDB.Driver to inject collections & databased on a controller level.

**ServiceCollectionExtensions** - A sentralized IServiceCollection extension to register all services & dependencies in one place.

**StringExtensions** - A lightweight extension on the "string" type to manipulate strings for efficency in large scale methods.

**Exceptions** - A list of custom defined exceptions used in Blackwater console applications & long service life applications.

**ExceptionHandlingMiddleware** - A middleware ensuring stack traces & important system information doesn't get returned during server errors.

**BsonConverter** - Lightweight class to handle JSON & BSON deserialization & serialization.

