using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Infrastructure.Extensions;
using System.ComponentModel.DataAnnotations;
//using Infrastructure.Data;
using PrjBase.ModelBase.Attributes;
using System.ComponentModel;
using PrjBase.Data;


namespace BG.Model.Core
{
    #region BoardGame

    public partial class BoardGame
    {
        public int BoardGameID { get; set; }

        //* Name
        [Required(ErrorMessage = "Name is Required")]
        [StringLength(maximumLength: 127, ErrorMessage = "Invalid Max Length (127)")]
        public string? Name { get; set; }
        //* Year
        [Required(ErrorMessage = "Year is Required")]
        public int Year { get; set; }
        //* MinPlayers
        [Required(ErrorMessage = "MinPlayers is Required")]
        public int MinPlayers { get; set; }
        //* MaxPlayers
        [Required(ErrorMessage = "MaxPlayers is Required")]
        public int MaxPlayers { get; set; }
        //* PlayTime
        [Required(ErrorMessage = "PlayTime is Required")]
        public int PlayTime { get; set; }
        //* MinAge
        [Required(ErrorMessage = "MinAge is Required")]
        public int MinAge { get; set; }
        //* UsersRated
        [Required(ErrorMessage = "UsersRated is Required")]
        public int UsersRated { get; set; }
        //* RatingAverage
        [Required(ErrorMessage = "RatingAverage is Required")]
        public double RatingAverage { get; set; }
        //* BGGRank
        [Required(ErrorMessage = "BGGRank is Required")]
        public int BGGRank { get; set; }
        //* ComplexityAverage
        [Required(ErrorMessage = "ComplexityAverage is Required")]
        public double ComplexityAverage { get; set; }
        //* OwnedUsers
        [Required(ErrorMessage = "OwnedUsers is Required")]
        public int OwnedUsers { get; set; }

        //* DB Relationships
        public virtual List<BoardGamesDomain> BoardGamesDomains { get; set; } = [];
        public virtual List<BoardGamesMechanic> BoardGamesMechanics { get; set; } = [];

        //* Ctor
        public BoardGame() : this(0) { }
        public BoardGame(int boardGameID)
        {
            this.BoardGameID = boardGameID;
        }
    }
    public class BoardGames : List<BoardGame> { }

    #endregion

    #region Interfaces

    public partial interface IBoardGameRepository :
            ICrud<BoardGame>, ISearch<BoardGame>, IPaging<BoardGame>
            , ICount<BoardGame>
    { }

    public partial interface IBoardGameServices : IBoardGameRepository
    //ICrud<BoardGame>, ISearch<BoardGame>
    { }

    #endregion

    #region DTO

    public class BoardGame_ChangeDTO
    {
        //* BoardGameID
        [Required]
        public int BoardGameID { get; set; }

        //* Name
        [Required(ErrorMessage = "Name is Required")]
        [StringLength(maximumLength: 127, ErrorMessage = "Invalid Max Length (127)")]
        public string? Name { get; set; }

        //* Year
        [Required(ErrorMessage = "Year is Required")]
        public int? Year { get; set; }

        //* Functionality

        public BoardGame ToModel()
        {
            return new BoardGame()
            {
                BoardGameID = this.BoardGameID
                , Name = this.Name
                , Year = this.Year ?? 0
            };
        }
    }

    #endregion
}