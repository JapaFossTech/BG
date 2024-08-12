using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using PrjBase.Data;


namespace BG.Model.Core
{
    #region BoardGamesMechanic

    public partial class BoardGamesMechanic
    {
        public int BoardGamesMechanicID { get; set; }


        //* DB Relationships
        public virtual int BoardGameID { get; set; }
        public virtual BoardGame? BoardGame { get; set; }
        public virtual int MechanicID { get; set; }
        public virtual Mechanic? Mechanic { get; set; }

        //* Ctor
        public BoardGamesMechanic() : this(0) { }
        public BoardGamesMechanic(int boardGamesMechanicID)
        {
            this.BoardGamesMechanicID = boardGamesMechanicID;
        }
    }
    public class BoardGamesMechanics : List<BoardGamesMechanic> { }

    #endregion

    #region Interfaces

    public partial interface IBoardGamesMechanicRepository :
            ICrud<BoardGamesMechanic>, ISearch<BoardGamesMechanic>
    { }

    public partial interface IBoardGamesMechanicServices :
            ICrud<BoardGamesMechanic>, ISearch<BoardGamesMechanic>
    { }

    #endregion
}