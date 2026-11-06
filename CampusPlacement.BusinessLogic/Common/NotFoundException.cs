namespace CampusPlacement.BusinessLogic.Common;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}