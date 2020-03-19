namespace Dotnettency
{
    public class KeyTenantIdentifier<TKey> : TenantIdentifier
    {
        public KeyTenantIdentifier(TKey key) : base(new System.Uri($"key://{typeof(TKey).Name}/{key}"))
        {

        }

        public static implicit operator KeyTenantIdentifier<TKey>(TKey key)
        {
            KeyTenantIdentifier<TKey> value = new KeyTenantIdentifier<TKey>(key);
            return value;
        }


        public TenantIdentifier AsTenantIdentifier()
        {
            // syntactic sugar.
            return this;
        }
    }
}
