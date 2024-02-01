
# Senior C# Backend Test Repo

## The task
This is very badly designed code. Your task is to refactor the solution and document your intentions.
If you can't fit in the given timeframe, we will evaluate your intentions and any WIP you check in.

Keep in mind that this is an incomplete service. Our future requirements include a search endpoint, and
processing event driven data updates following our CQRS design.

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
