namespace Models.Entity
{
    public class EntraSocioMap : BaseMap, IMap
    {
        public SchedaMap? EntraScheda { get; set; }
        public List<SchedaContoMap>? EntraSchedaConto { get; set; }

        
    }
}
