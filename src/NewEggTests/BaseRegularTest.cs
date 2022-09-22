namespace NewEggTests
{
	public class BaseRegularTest : BaseTest
	{
		protected override string TestSku1 
		{ 
			get 
			{ 
				return "testSku1"; 
			}
		}

		protected override string TestSku2
		{
			get
			{
				return "testSku2";
			}
		}

		protected override string WarehouseLocationCountryCode
		{
			get
			{
				return "USA";
			}
		}

		public BaseRegularTest() : base("credentials")
		{
		}
	}
}
