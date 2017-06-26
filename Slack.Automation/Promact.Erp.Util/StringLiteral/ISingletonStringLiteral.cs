namespace Promact.Erp.Util.StringLiteral
{
    public interface ISingletonStringLiteral
    {
        /// <summary>
        /// String Literal
        /// </summary>
        AppStringLiteral StringConstant { get; }

        /// <summary>
        /// Method to Initialize String Literal
        /// </summary>
        /// <param name="appStringLiteral">string literal</param>
        void Initialize(AppStringLiteral appStringLiteral);
    }
}
