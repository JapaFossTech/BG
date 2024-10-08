﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using PrjBase.Data;


namespace BG.Model.Core
{
    #region Domain

    public partial class Domain
    {
        public int DomainID { get; set; }

        //* DomainDesc
        [Required(ErrorMessage = "DomainDesc is Required")]
        [StringLength(maximumLength: 200, ErrorMessage = "Invalid Max Length (200)")]
        public string? DomainDesc { get; set; }

        //* DB Relationships
        public virtual List<BoardGamesDomain> BoardGamesDomains { get; set; } = [];

        //* Ctor
        public Domain() : this(0) { }
        public Domain(int domainID)
        {
            this.DomainID = domainID;
        }
    }
    public class Domains : List<Domain> { }

    #endregion

    #region Interfaces

    public partial interface IDomainRepository :
        ICrud<Domain>, ISearch<Domain>, IPaging<Domain>
        , ICount<Domain>
    { }

    public partial interface IDomainServices : IDomainRepository
    { }

    #endregion
}