namespace IdentifyRequest
{
    public class Mapping<TKey>
    {       
        public TKey Key { get; set; }
        public string[] Patterns { get; set; }
        public MappingEnabledCondition Condition { get; set; }
    }
}
