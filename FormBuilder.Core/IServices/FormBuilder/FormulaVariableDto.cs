public class FormulaVariableDto
{
    public int Id { get; set; }
    public int FormulaId { get; set; }
    public int FieldId { get; set; }
    public string FieldCode { get; set; }
    public string FieldName { get; set; }
    public string FieldType { get; set; }
    public string VariableName { get; set; }
}

public class FormulaFieldInfoDto
{
    public int FieldId { get; set; }
    public string FieldCode { get; set; }
    public string FieldName { get; set; }
    public string FieldType { get; set; }
    public string TabName { get; set; }
    public int FormBuilderId { get; set; }
    public string FormBuilderName { get; set; }
    public bool IsActive { get; set; }
}

public class ValidateExpressionResultDto
{
    public bool IsValid { get; set; }
    public List<string> ValidFieldCodes { get; set; } = new List<string>();
    public List<string> InvalidFieldCodes { get; set; } = new List<string>();
    public List<FormulaFieldInfoDto> FieldDetails { get; set; } = new List<FormulaFieldInfoDto>();
}