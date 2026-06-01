using System.Collections.Generic;
using System.Runtime.Serialization;

[DataContract]
public class FactorSummaryResponse
{
    [DataMember]
    public int totalFactorCount { get; set; }

    [DataMember]
    public List<CategoryDto> categories { get; set; }

    [DataMember]
    public List<FactorAverageDto> aggravatingAverageFactors { get; set; }

    [DataMember]
    public List<FactorAverageDto> mitigatingAverageFactors { get; set; }
}

[DataContract]
public class CategoryDto
{
    [DataMember]
    public int id { get; set; }

    [DataMember]
    public string name { get; set; }
}

[DataContract]
public class FactorAverageDto
{
    [DataMember]
    public string factorName { get; set; }

    [DataMember]
    public decimal average { get; set; }
}