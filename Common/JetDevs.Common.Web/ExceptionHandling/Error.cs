namespace JetDevs.Common.Web.ExceptionHandling
{
    /// <summary>
    /// Describes the Error
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Name of the field with error 
        /// </summary>
        public string FieldName { get; set; }
        /// <summary>
        /// Error Message
        /// </summary>
        public string Message { get; set; }
    }
}
