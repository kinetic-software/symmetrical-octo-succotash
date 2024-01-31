namespace Kx.Availability.Tests.Data;

public class TestMongoDataAccessFactory : ITestDataAccessFactory
{    
    public TestMongoDataAccessFactory()
    {        
    }

    public ITestData GetDataAccess()
    {
        var data = new TestMongoData();        
        return data;
    }
}