namespace Kx.Availability.Data.Interface.Enums;
public class SearchCriteria
{
    public string Key { get; private set; }
    public CriteriaType Type { get; private set; }
    public string Value { get; private set; }

    public SearchCriteria(string key, CriteriaType type, string value)
    {
        Key = key;
        Type = type;
        Value = value;
    }
}
