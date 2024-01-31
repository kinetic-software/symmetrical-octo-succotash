﻿Feature: AggregateAvailabilityData

  Background: 
    Given I have a clean database    
    And My tenant id is "13"    
    And I have set an HttpRequestHandler for Locations
    And I have set an HttpRequestHandler for Rooms   
    Then I call the locations Api and get the following locations data
      """
      {
      	"totalPages":1,
      	"totalItems":1,
      	"page":1,
      	"data":[
      		{
      		"id": "location-guid-123",
      		"name": "West Wing",
      		"germId": 2,
      		"type": "block"			
      		}
      	]		
      }		
      """
    And I call the Bedrooms Api to get the following rooms data
      """
      {
           "totalPages":1,
           "totalItems":1,
           "page":1,
           "data":[
             {
      	  "germId": 1,
      	  "name": "Squalid Hovel",
      	  "blockId": 2, 
      	  "capacity": 1, 
      	  "maxCapacity": 1,
      	  "germTypeId": 1,
      	  "inactive": true
      	}
           ]
         }		
      """    

  @clearStateData @clearMongoTestData
  Scenario: Aggregate data from the core Apis
    Given I have the following data in the state table
      """
      [
      	{
      	  "ID" : "64a3d6dde45a7a6391c8841f",
      	  "TenantId" : "13",
      	  "StartTime" : "2023-07-04T08:22:53.232+0000",
      	  "StateTime" : "2023-07-04T08:22:54.199+0000",
      	  "State" : "CycleError",
      	  "Entity" : "Site",
      	  "ExceptionMessage" : "Test Exception Message",
      	  "CallerId" : null,
      	  "IsEnded" : true,
      	  "IsSuccess" : false
      	}
      ]
      """
    When I request Tenants Data to be loaded into the Mongodb
    Then I receive a No Content response    
    And I see the following availability in my database:
      """
        [
          {
            "id": "mongo-guid-1234",
            "tenantId": "13",
            "roomId": 1,
            "locations": [
              {
                "id": "location-guid-123",
                "type": "block",
                "name": "West Wing",
                "isDirectLocation": true,
                "meta": {
                  "germId": 2,
                  "entityVersion": 0
                }
              }
            ],
            "displayOrder": 0,
            "meta": {
              "germId": 1,
              "entityVersion": 0
            }    
          }
        ]		
      """

  @clearStateData @clearMongoTestData
  Scenario: Does not affect any other tenant's data
    Given I have the following data in the state table
      """
      [
      	{
      	  "ID" : "64a3d6dde45a7a6391c8841f",
      	  "TenantId" : "13",
      	  "StartTime" : "2023-07-04T08:22:53.232+0000",
      	  "StateTime" : "2023-07-04T08:22:54.199+0000",
      	  "State" : "CycleError",
      	  "Entity" : "Site",
      	  "ExceptionMessage" : "Test Exception Message",
      	  "CallerId" : null,
      	  "IsEnded" : true,
      	  "IsSuccess" : false
      	}
      ]
      """
    And I have the following availability in my database:
      """
     [
  {
    "id": "mongo-guid-1235",
    "tenantId": "45",
    "roomId": 7,
    "locations": [
      {
        "id": "location-guid-123",
        "type": "block",
        "name": "West Wing",
        "isDirectLocation": true,
        "meta": {
          "germId": 2,
          "entityVersion": 0
        }
      }
    ],
    "displayOrder": 0    
  }
]
      """
    When I request Tenants Data to be loaded into the Mongodb
    Then I receive a No Content response
    And I see the following availability in my database:
      """
[
  {
    "id": "mongo-guid-1235",
    "tenantId": "45",
    "roomId": 7,
    "locations": [
      {
        "id": "location-guid-123",
        "type": "block",
        "name": "West Wing",
        "isDirectLocation": true,
        "meta": {
          "germId": 2,
          "entityVersion": 0
        }
      }
    ],
    "displayOrder": 0,
    "meta": {
      "germId": 0,
      "entityVersion": 0
    }
  },
  {
    "id": "mongo-guid-1234",
    "tenantId": "13",
    "roomId": 1,
    "locations": [
      {
        "id": "location-guid-123",
        "type": "block",
        "name": "West Wing",
        "isDirectLocation": true,
        "meta": {
          "germId": 2,
          "entityVersion": 0
        }
      }
    ],
    "displayOrder": 0,
    "meta": {
      "germId": 1,
      "entityVersion": 0
    }
  }
]	
      """

  @clearStateData @clearMongoTestData
  Scenario: Does allow running if a run has not completed on a different tenant
    Given I have the following data in the state table
      """
      [
      	{
      	  "ID" : "64a3d6dde45a7a6391c8841f",
      	  "TenantId" : "45",
      	  "StartTime" : "2023-07-04T08:22:53.232+0000",
      	  "StateTime" : "2023-07-04T08:22:54.199+0000",
      	  "State" : "CycleError",
      	  "Entity" : "Site",
      	  "ExceptionMessage" : "Test Exception Message",
      	  "CallerId" : null,
      	  "IsEnded" : false,
      	  "IsSuccess" : false
      	},
        {
      	  "ID" : "64a3d6dde45a7a6391c883242",
      	  "TenantId" : "13",
      	  "StartTime" : "2023-07-04T08:22:53.232+0000",
      	  "StateTime" : "2023-07-04T08:22:54.199+0000",
      	  "State" : "CycleError",
      	  "Entity" : "Site",
      	  "ExceptionMessage" : "Test Exception Message",
      	  "CallerId" : null,
      	  "IsEnded" : true,
      	  "IsSuccess" : false
      	}
      ]
      """
    When I request Tenants Data to be loaded into the Mongodb
    Then I receive a No Content response

  @clearStateData @clearMongoTestData
  Scenario: Does not allow running if a run has not completed
    Given I have the following data in the state table
      """
      [
      	{
      	  "ID" : "64a3d6dde45a7a6391c8841f",
      	  "TenantId" : "13",
      	  "StartTime" : "2023-09-18T11:05:53.232+0000",
      	  "StateTime" : "2023-09-18T11:22:54.199+0000",
      	  "State" : "CycleError",
      	  "Entity" : "Site",
      	  "ExceptionMessage" : "Test Exception Message",
      	  "CallerId" : null,
      	  "IsEnded" : false,
      	  "IsSuccess" : false
      	}
      ]
      """
    When I request Tenants Data to be loaded into the Mongodb
    Then I receive a ExpectationFailed response
    And I see the following message:
      """
      Cannot start a new run for tenant 13 Previous Run Has Not Ended.
      """

  @clearStateData @clearMongoTestData
  Scenario: Does allow running if a run has timed out
    Given I have the following data in the state table that started past the timeout
      """
      [
      	{
      	  "ID" : "64a3d6dde45a7a6391c8841f",
      	  "TenantId" : "13",			 
      	  "State" : "CycleError",
      	  "Entity" : "Site",
      	  "ExceptionMessage" : "Test Exception Message",
      	  "CallerId" : null,
      	  "IsEnded" : false,
      	  "IsSuccess" : false
      	}
      ]
      """
    When I request Tenants Data to be loaded into the Mongodb
    Then I receive a No Content response
