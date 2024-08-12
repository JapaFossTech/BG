using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using PrjBase.Data;


namespace BG.Model.Core
{
    #region BoardGamesDomain

    public partial class BoardGamesDomain
    {
        public int BoardGamesDomainID { get; set; }


        //* DB Relationships
        public virtual int BoardGameID { get; set; }
        public virtual BoardGame? BoardGame { get; set; }
        public virtual int DomainID { get; set; }
        public virtual Domain? Domain { get; set; }

        //* Ctor
        public BoardGamesDomain() : this(0) { }
        public BoardGamesDomain(int boardGamesDomainID)
        {
            this.BoardGamesDomainID = boardGamesDomainID;
        }
    }
    public class BoardGamesDomains : List<BoardGamesDomain> { }

    #endregion

    #region Interfaces

    public partial interface IBoardGamesDomainRepository :
            ICrud<BoardGamesDomain>, ISearch<BoardGamesDomain>
    { }

    public partial interface IBoardGamesDomainServices :
            ICrud<BoardGamesDomain>, ISearch<BoardGamesDomain>
    { }

    #endregion
}