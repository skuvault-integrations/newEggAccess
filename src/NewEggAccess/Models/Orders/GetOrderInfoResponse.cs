﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NewEggAccess.Models.Orders
{
	public class GetOrderInfoResponse
	{
		public DateTime ResponseDate { get; set; }
		public string Memo { get; set; }
		public bool IsSuccess { get; set; }
		public string OperationType { get; set; }
		public string SellerId { get; set; }
		public GetOrderInfoResponseBody ResponseBody { get; set; } 
	}

	public class GetOrderInfoResponseBody
	{
		public PageInfo PageInfo { get; set; }
		public OrderInfoList OrderInfoList { get; set; }
	}

	public class OrderInfoList
	{
		public Order[] OrderInfo { get; set; }
	}

	public class PageInfo
	{
		public int TotalCount { get; set; }
		public int TotalPageCount { get; set; }
		public int PageIndex { get; set; }
		public int PageSize { get; set; }
	}
}
