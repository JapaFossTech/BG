namespace BG.Model.Core
{
    public partial interface ICoreServices
    {
        IBoardGameServices BoardGame { get; }
        IDomainServices Domain { get; }
        IMechanicServices Mechanic { get; }
        IBoardGamesDomainServices BoardGamesDomain { get; }
        IBoardGamesMechanicServices BoardGamesMechanic { get; }
    }
}
