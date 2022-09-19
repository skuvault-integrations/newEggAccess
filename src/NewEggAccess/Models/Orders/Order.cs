using NewEggAccess.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NewEggAccess.Models.Orders
{
	public class NewEggOrder
	{
		public string Id { get; set; }
		public string Number { get; set; }
		public DateTime OrderDateUtc { get; set; }
		public NewEggOrderStatusEnum Status { get; set; }
		public NewEggShippingInfo ShippingInfo { get; set; }
		public NewEggOrderItem[] Items { get; set; }
		public decimal Total { get; set; }
		public decimal DiscountAmount { get; set; }
		public decimal SalesTax { get; set; }
	}

	public enum NewEggOrderStatusEnum { Unshipped, PartiallyShipped, Shipped, Invoiced, Voided }

	public class NewEggShippingInfo
	{
		public NewEggShippingContactInfo ContactInfo { get; set; }
		public NewEggShippingAddress Address { get; set; }
		public string Carrier { get; set; }
		public decimal ShippingCharge { get; set; }
	}

	public class NewEggShippingAddress
	{
		public string Line1 { get; set; }
		public string Line2 { get; set; }
		public string City { get; set; }
		public string CountryCode { get; set; }
		public string State { get; set; }
		public string PostalCode { get; set; }
	}

	public class NewEggShippingContactInfo
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string CompanyName { get; set; }
		public string PhoneNumber { get; set; }
		public string EmailAddress { get; set; }
	}

	public class NewEggOrderItem
	{
		public string Sku { get; set; }
		public int Quantity { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal ShippingCharge { get; set; }
		public decimal SalesTax { get; set; }
		public decimal Vat { get; set; }
	}

	public class Order
	{
		public string SellerId { get; set; }
		public string OrderNumber { get; set; }
		public string SellerOrderNumber { get; set; }
		public DateTime OrderDate { get; set; }
		public int OrderStatus { get; set; }
		public string OrderStatusDescription { get; set; }
		public string CustomerName { get; set; }
		public string CustomerPhoneNumber { get; set; }
		public string CustomerEmailAddress { get; set; }
		public string ShipToAddress1 { get; set; }
		public string ShipToAddress2 { get; set; }
		public string ShipToCityName { get; set; }
		public string ShipToStateCode { get; set; }
		public string ShipToZipCode { get; set; }
		public string ShipToCountryCode { get; set; }
		public string ShipService { get; set; }
		public string ShipToFirstName { get; set; }
		public string ShipToLastName { get; set; }
		public string ShipToCompany { get; set; }
		public decimal OrderItemAmount { get; set; }
		public decimal ShippingAmount { get; set; }
		public decimal DiscountAmount { get; set; }
		public decimal? SalesTax { get; set; }
		public decimal? VATTotal { get; set; }
		public decimal? DutyTotal { get; set; }
		public decimal OrderTotalAmount { get; set; }
		public int OrderQty { get; set; }
		public OrderItem[] ItemInfoList { get; set; }
		public PackageInfo[] PackageInfoList { get; set; }
	}

	public class OrderItem
	{
		public string SellerPartNumber { get; set; }
		public string NeweggItemNumber { get; set; }
		public string MfrPartNumber { get; set; }
		public string Description { get; set; }
		public int OrderedQty { get; set; }
		public int ShippedQty { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal? ExtendUnitPrice { get; set; }
		public decimal? ExtendShippingCharge { get; set; }
		public decimal? ExtendSalesTax { get; set; }
		public decimal? ExtendVAT { get; set; }
		public decimal? ExtendDuty { get; set; }
		public string StatusDescription { get; set; }
	}

	public class PackageInfo
	{
		public string PackageType { get; set; }
		public string ShipCarrier { get; set; }
		public string ShipService { get; set; }
		public string TrackingNumber { get; set; }
	}

	public static class OrderExtensions
	{
		public static NewEggOrder ToSVOrder( this Order order )
		{
			var svOrder =  new NewEggOrder()
			{
				Id = order.OrderNumber,
				Number = order.SellerOrderNumber,
				OrderDateUtc = Misc.ConvertFromPstToUtc( order.OrderDate ),
				Status = (NewEggOrderStatusEnum)order.OrderStatus,
				ShippingInfo = new NewEggShippingInfo()
				{
					Address = new NewEggShippingAddress()
					{
						Line1 = order.ShipToAddress1,
						Line2 = order.ShipToAddress2,
						City = order.ShipToCityName,
						State = order.ShipToStateCode,
						PostalCode = order.ShipToZipCode,
						CountryCode = order.ShipToCountryCode
					},
					ContactInfo = new NewEggShippingContactInfo()
					{
						FirstName = order.ShipToFirstName ?? order.CustomerName,
						LastName = order.ShipToLastName,
						CompanyName = order.ShipToCompany,
						PhoneNumber = order.CustomerPhoneNumber,
						EmailAddress = order.CustomerEmailAddress
					},
					Carrier = order.PackageInfoList?.FirstOrDefault()?.ShipCarrier,
					ShippingCharge = order.ShippingAmount,
				},
				Total = order.OrderTotalAmount,
				DiscountAmount = order.DiscountAmount,
				SalesTax = order.SalesTax ?? 0
			};

			var items = new List< NewEggOrderItem >();
			foreach( var orderItem in order.ItemInfoList )
			{
				items.Add( new NewEggOrderItem()
				{
					Sku = orderItem.SellerPartNumber,
					Quantity = orderItem.OrderedQty,
					UnitPrice = orderItem.UnitPrice,
					ShippingCharge = orderItem.ExtendShippingCharge ?? 0,
					SalesTax = orderItem.ExtendSalesTax ?? 0,
					Vat = orderItem.ExtendVAT ?? 0
				} );
			}
			svOrder.Items = items.ToArray();

			return svOrder;
		}
	}
}