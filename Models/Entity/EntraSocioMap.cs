namespace Models.Entity
{
    public class EntraSocioMap : BaseMap
    {
        public SchedaMap? EntraScheda { get; set; }
        public List<SchedaContoMap>? EntraSchedaConto { get; set; }

        
    }
}
