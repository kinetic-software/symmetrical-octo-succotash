namespace Kx.Core.Common.Data;

public class SearchCriteria
{
    public string Key { get; set; }
    public CriteriaType Type { get; set; }
    public string Value { get; set; }

    public SearchCriteria(string key, CriteriaType type, string value)
    {
        Key = key;
        Type = type;
        Value = value;
    }
}
