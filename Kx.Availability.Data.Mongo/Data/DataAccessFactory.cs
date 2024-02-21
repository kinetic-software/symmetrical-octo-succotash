using Kx.Core.Common.Data;
using Kx.Core.Common.HelperClasses;
using Kx.Core.Common.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Kx.Availability.Data.Mongo.Data;

public class DataAccessFactory : IDataAccessFactory
{
    private readonly IConnectionDefinitionFactory _connectionFactory;
    private readonly ITenant _tenant;
    private readonly IConfiguration _config;
    private readonly IKxJsonSettings _jsonSettings;

    public DataAccessFactory(IConnectionDefinitionFactory connectionDefinitionFactory, ITenant tenant, IConfiguration config, IKxJsonSettings kxJsonSettings)
    {
        _jsonSettings = kxJsonSettings;
        _connectionFactory = connectionDefinitionFactory;
        _tenant = tenant;
        _config = config;
    }

    public IDataAccess GetDataAccess(KxDataType kxDataType)
    {
        return kxDataType switch
        {            
            KxDataType.AvailabilityAggregation => new AggregatedAvailabilityData(_connectionFactory, _tenant, _config, _jsonSettings),          
            _ => throw new ArgumentException("The data type provided does not have a read implementation") // DC: Feels like this will NEVER be hit? Given KxDataType only has one possible value of "AvailabilityAggregation" defined in it's enum
        };
    }

    public IDataAggregationStoreAccess<T> GetDataStoreAccess<T>() where T : class
    {
        return new DataStoreAccess<T>(_connectionFactory, _jsonSettings);
    }
}