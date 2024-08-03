namespace BG.Model.Core
{
    public partial interface ICoreData
    {
        IBoardGameRepository BoardGameRepository { get; }
IDomainRepository DomainRepository { get; }
IMechanicRepository MechanicRepository { get; }
IBoardGamesDomainRepository BoardGamesDomainRepository { get; }
IBoardGamesMechanicRepository BoardGamesMechanicRepository { get; }
    }
}
