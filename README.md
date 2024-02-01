# Senior C# Backend Test Repo

## The task
This is very badly designed code that needs to be refactored. Your task is to identify the issues and come up with a better design.
Fork this repo and share your repo with us. Document, or implement your proposed changes.
Allow us to have a couple of hours before your next round interview to examine your solution.

Keep in mind that this is an incomplete service. Our future requirements include a search endpoint, and processing event driven data updates following our CQRS design.
We also know that in the future this API will connect to another database which may not be MongoDB.

Because this service relies on two other services that you do not have access to you'll have to rely on the tests to ensure that your changes haven't broken anything.

We have a multi-tenant environment that spans over multiple jurisdictions. Some of our APIs also handle PIIs, so security is important to us.

## Prerequisites
These projects target .NET 8.0 so you'll need to ensure you have the [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) installed.
The tests use create a docker container for the MongoDB database that's used in the service so you'll also need to have [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed for this container to run.
The first run of the tests will take longer as we will need to pull a docker container.

## The Process
The repository has a single endpoint `/v1/{tenantId}/bedroom-availability/reloadData` and a set of tests to test the logic behind this endpoint. This process replaces the data in a MongoDB database with newly retrieved data from two other services.

The reload data process has the following high level steps:

 1. Checks whether or not the process is already running for the specified tenant. It can only run once for a specific tenant at a time.
 2. Calls two other endpoints to retrieve location and room data.
 3. Merges this data together into a single data source and stores it in a temporary collection.
 4. Replaces the data in the production collection with that in the temporary collection.

## The Repository
The repository consists of multiple different projects the ones to focus on are:

 - Kx.Availability - The entry point for the services. This contains a single endpoint to reload the data in the cache.
 - Kx.Availability.Data - This is where the logic behind the process lives.
 - Kx.Availability.Data.Mongo - MongoDB specific access code used to read/write to the required collections.
 - Kx.Availability.Tests - A set of tests that validate that the process as running as expected.

There are also some other projects that contain supporting code for the service. You do not need to look at these, you are welcome to, but focus on the previous repositories.
