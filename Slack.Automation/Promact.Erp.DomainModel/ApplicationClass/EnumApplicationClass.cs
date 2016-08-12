namespace Promact.Erp.DomainModel.ApplicationClass
{
    /// <summary>
    /// Leave Status
    /// </summary>
    public enum Condition
    {
        Pending,
        Approved,
        Rejected,
        Cancel
    }

    public enum SlackAction
    {
        Apply,
        List,
        Cancel,
        Status,
        Balance,
        Help
    }
}
