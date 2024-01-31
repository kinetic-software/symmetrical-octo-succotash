namespace Kx.Availability.Data.Interface.Interfaces;

public interface IAwsRoleSettings
{
    public string? AwsRole { get; }
    List<Uri> ServiceUrls { get; set; }
}