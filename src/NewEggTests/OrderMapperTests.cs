using FluentAssertions;
using NewEggAccess.Models.Orders;
using NUnit.Framework;
using System;
using System.Linq;

namespace NewEggTests
{
	[TestFixture]
	public class OrderMapperTests
	{
		[Test]
		public void ToSVOrder()
		{
			var order = new Order()
			{
				OrderNumber = "417171",
				SellerOrderNumber = "21919191",
				OrderDate = new DateTime(2020, 03, 16, 19, 0, 0),
				OrderStatus = 0,
				CustomerName = "WY SD",
				CustomerPhoneNumber = "123-223-3223",
				CustomerEmailAddress = "gdv6l0viwo4l7j1d@marketplace.newegg.com",
				ShipToAddress1 = "149 Kenwood Dr",
				ShipToCityName = "Newcastle",
				ShipToStateCode = "WY",
				ShipToZipCode = "82701",
				ShipToCountryCode = "UNITED STATES",
				ShipToFirstName = "WY",
				ShipToLastName = "SD",
				ShipToCompany = "SkuVault",
				SalesTax = 20.0M,
				OrderTotalAmount = 120.0M,
				DiscountAmount = 10.0M,
				ItemInfoList = new OrderItem[]
					{
						new OrderItem()
						{
							SellerPartNumber = "testSku1",
							OrderedQty = 7,
							UnitPrice = 2.0M,
							ExtendShippingCharge = 1.3M,
							ExtendSalesTax = 0.5M,
							ExtendVAT = 0.15M
						},
						new OrderItem()
						{
							SellerPartNumber = "testSku2",
							OrderedQty = 2,
							UnitPrice = 1.0M,
							ExtendShippingCharge = 0.3M,
							ExtendSalesTax = 0.1M,
							ExtendVAT = 0.15M
						},
					},
				PackageInfoList = new PackageInfo[] {
						new PackageInfo()
						{
							ShipCarrier = "UPS"
						}
					}
			};

			var svOrder = order.ToSVOrder();

			svOrder.Should().NotBeNull();
			svOrder.Id.Should().Be(order.OrderNumber);
			svOrder.Number.Should().Be(order.SellerOrderNumber);
			svOrder.Status.Should().Be(NewEggOrderStatusEnum.Unshipped);
			svOrder.OrderDateUtc.Should().Be(order.OrderDate.AddHours(-8));
			svOrder.Total.Should().Be(order.OrderTotalAmount);
			svOrder.DiscountAmount.Should().Be(order.DiscountAmount);

			svOrder.ShippingInfo.Should().NotBeNull();
			svOrder.ShippingInfo.Carrier.Should().Be(order.PackageInfoList.First().ShipCarrier);
			svOrder.ShippingInfo.ShippingCharge.Should().Be(order.ShippingAmount);

			svOrder.ShippingInfo.Address.Should().NotBeNull();
			svOrder.ShippingInfo.Address.Line1.Should().Be(order.ShipToAddress1);
			svOrder.ShippingInfo.Address.City.Should().Be(order.ShipToCityName);
			svOrder.ShippingInfo.Address.State.Should().Be(order.ShipToStateCode);
			svOrder.ShippingInfo.Address.CountryCode.Should().Be(order.ShipToCountryCode);

			svOrder.ShippingInfo.ContactInfo.Should().NotBeNull();
			svOrder.ShippingInfo.ContactInfo.FirstName.Should().Be(order.ShipToFirstName);
			svOrder.ShippingInfo.ContactInfo.LastName.Should().Be(order.ShipToLastName);
			svOrder.ShippingInfo.ContactInfo.CompanyName.Should().Be(order.ShipToCompany);
			svOrder.ShippingInfo.ContactInfo.PhoneNumber.Should().Be(order.CustomerPhoneNumber);
			svOrder.ShippingInfo.ContactInfo.EmailAddress.Should().Be(order.CustomerEmailAddress);

			svOrder.Items.Count().Should().Be(2);
			svOrder.Items.First().Sku.Should().Be(order.ItemInfoList.First().SellerPartNumber);
			svOrder.Items.First().Quantity.Should().Be(order.ItemInfoList.First().OrderedQty);
			svOrder.Items.First().UnitPrice.Should().Be(order.ItemInfoList.First().UnitPrice);
			svOrder.Items.First().SalesTax.Should().Be(order.ItemInfoList.First().ExtendSalesTax);
			svOrder.Items.First().Vat.Should().Be(order.ItemInfoList.First().ExtendVAT);
			svOrder.Items.First().ShippingCharge.Should().Be(order.ItemInfoList.First().ExtendShippingCharge);
		}
	}
}