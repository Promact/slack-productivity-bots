namespace Promact.Erp.Util.StringLiteral
{
    public interface IStringLiteral
    {
        /// <summary>
        /// Method to start file watcher
        /// </summary>
        void CreateFileWatcher();

        /// <summary>
        /// Method to create .json file of StringConstantRepository.cs and inirialize string literal
        /// </summary>
        void OnInit();
    }
}
