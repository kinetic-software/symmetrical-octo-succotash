Feature: AggregateAvailabilityData

  Background:
    Given I have a clean database    
    And My tenant id is "13"    
    And I have set an HttpRequestHandler for Locations
    And I have set an HttpRequestHandler for Rooms
    And The Locations API returns the following:
      """
      {
      	"totalPages":1,
      	"totalItems":1,
      	"page":1,
      	"data":[
      		{
      		"id": "location-guid-123",
      		"name": "West Wing",
      		"externalId": 2,
      		"type": "block"			
      		}
      	]		
      }		
      """
    And The Bedrooms API returns the following:
      """
      {
        "totalPages":1,
        "totalItems":1,
        "page":1,
        "data":[
        	{
		        "externalId": 1,
		        "name": "Squalid Hovel",
		        "blockId": 2, 
		        "capacity": 1, 
		        "maxCapacity": 1,
		        "externalTypeId": 1,
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
      	  "ExceptionMessage" : "Test Exception Message",      	  
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
                  "externalId": 2,
                  "entityVersion": 0
                }
              }
            ],
            "displayOrder": 0,
            "meta": {
              "externalId": 1,
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
      	  "ExceptionMessage" : "Test Exception Message",      	  
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
			          "externalId": 2,
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
			          "externalId": 2,
			          "entityVersion": 0
			        }
			      }
			    ],
			    "displayOrder": 0,
			    "meta": {
			      "externalId": 0,
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
			          "externalId": 2,
			          "entityVersion": 0
			        }
			      }
			    ],
			    "displayOrder": 0,
			    "meta": {
			      "externalId": 1,
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
      	  "ExceptionMessage" : "Test Exception Message",      	  
      	  "IsEnded" : false,
      	  "IsSuccess" : false
      	},
        {
      	  "ID" : "64a3d6dde45a7a6391c883242",
      	  "TenantId" : "13",
      	  "StartTime" : "2023-07-04T08:22:53.232+0000",
      	  "StateTime" : "2023-07-04T08:22:54.199+0000",
      	  "State" : "CycleError",      	  
      	  "ExceptionMessage" : "Test Exception Message",      	  
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
      	  "ExceptionMessage" : "Test Exception Message",      	  
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
      	  "ExceptionMessage" : "Test Exception Message",      	  
      	  "IsEnded" : false,
      	  "IsSuccess" : false
      	}
      ]
      """
    When I request Tenants Data to be loaded into the Mongodb
    Then I receive a No Content response
