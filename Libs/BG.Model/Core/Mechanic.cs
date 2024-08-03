using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Extensions;
using System.ComponentModel.DataAnnotations;
using Infrastructure.Data;

                
namespace BG.Model.Core
{
    #region Mechanic
    
    public partial class Mechanic
    {
        public int MechanicID { get; set; }
            
//* MechanicDesc
[Required(ErrorMessage = "MechanicDesc is Required")]
[StringLength(maximumLength: 200, ErrorMessage = "Invalid Max Length (200)")]
              public string? MechanicDesc { get; set; }
        
//* DB Relationships
public virtual List<BoardGamesMechanic> BoardGamesMechanics { get; set; } = [];

        //* Ctor
    public Mechanic(): this(0){}
    public Mechanic(int mechanicID)
    {
        this.MechanicID = mechanicID;
    }
    }
    public class Mechanics: List<Mechanic>{}
    
    #endregion

    #region Interfaces

public partial interface IMechanicRepository : 
        ICrud<Mechanic>, ISearch<Mechanic> { }

public partial interface IMechanicServices : 
        ICrud<Mechanic>, ISearch<Mechanic> { }

#endregion
}