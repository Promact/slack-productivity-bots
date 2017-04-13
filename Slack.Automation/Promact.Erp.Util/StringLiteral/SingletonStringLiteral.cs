namespace Promact.Erp.Util.StringLiteral
{
    public class SingletonStringLiteral : ISingletonStringLiteral
    {
        #region Private Variable
        private AppStringLiteral stringLiteral { get; set; }
        #endregion

        #region Constructor
        public SingletonStringLiteral()
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Method to Initialize String Literal
        /// </summary>
        /// <param name="appStringLiteral">string literal</param>
        public void Initialize(AppStringLiteral appStringLiteral)
        {
            stringLiteral = appStringLiteral;
        }

        /// <summary>
        /// String Literal
        /// </summary>
        public AppStringLiteral StringConstant
        {
            get
            {
                return stringLiteral;
            }
        }
        #endregion
    }
}
