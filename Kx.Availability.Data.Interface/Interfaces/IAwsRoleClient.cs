using Amazon.Runtime;

namespace Kx.Availability.Data.Interface.Interfaces;

public interface IAwsRoleClient
{
    public Task<SessionAWSCredentials> AssumeAwsRoleAsync();

    public Task<SessionAWSCredentials> AssumeAwsRoleAsync(string roleName);
}