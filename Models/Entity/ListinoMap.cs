using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entity
{
    public class ListinoMap : BaseMap, IMap
    {

        #region CodiceSettore

       public int CodiceSettore { get; set; }
       public int CodiceTariffa { get; set; }
       public string EtichettaTariffa { get; set; } = string.Empty;
       public string NomeTariffa { get; set; }= string.Empty;
       public decimal PrezzoTariffa { get; set; }
       

        #endregion
    }
}
