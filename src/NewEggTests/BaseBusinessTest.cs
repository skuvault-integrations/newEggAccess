namespace NewEggTests
{
	public class BaseBusinessTest : BaseTest
	{
		protected override string TestSku1 
		{ 
			get 
			{ 
				return "testsku7"; 
			}
		}

		protected override string TestSku2
		{
			get
			{
				return "testserial1";
			}
		}

		protected override string WarehouseLocationCountryCode
		{
			get
			{
				return null;
			}
		}

		public BaseBusinessTest() : base("credentials_business")
		{
		}
	}
}
