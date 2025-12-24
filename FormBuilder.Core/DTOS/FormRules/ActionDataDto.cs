namespace FormBuilder.Core.DTOS.FormRules
{
    /// <summary>
    /// Represents an action in a form rule (THEN/ELSE part)
    /// </summary>
    public class ActionDataDto
    {
        /// <summary>
        /// Action type: SetVisible, SetReadOnly, SetMandatory, SetDefault, ClearValue, Compute
        /// </summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// Target field code
        /// </summary>
        public string FieldCode { get; set; } = string.Empty;

        /// <summary>
        /// Value for the action (for SetVisible, SetReadOnly, SetMandatory, SetDefault)
        /// </summary>
        public object? Value { get; set; }

        /// <summary>
        /// Formula expression (for Compute action)
        /// </summary>
        public string? Expression { get; set; }
    }
}

