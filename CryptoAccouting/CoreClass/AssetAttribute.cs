using System;
namespace CryptoAccouting
{
    public class AssetAttribute
    {

        public string Code { get; set; }
        public string Name { get; set; }
        public AssetType Type { get; set; }
        public DateTime ListedDate { get; set; }
        public DateTime LastUpdate { get; }

        public AssetAttribute()
        {
        }

    }

	public enum AssetType
	{
		CryptoCoin,
		Cash,
        Other
	}
}
