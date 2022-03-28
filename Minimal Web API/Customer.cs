namespace DocumentEditorCore
{
    /// <summary>
    /// This Class contains the property for Mail merge
    /// </summary>
    public class Customer
    {
        public string CustomerID { get; set; }
        public string ProductName { get; set; }
        public string Quantity { get; set; }
        public string ShipName { get; set; }
        public string UnitPrice { get; set; }
        public string Discount { get; set; }
        public string ShipAddress { get; set; }
        public string ShipCity { get; set; }
        public string OrderDate { get; set; }
        public string ShipCountry { get; set; }
        public string OrderId { get; set; }
        public string Subtotal { get; set; }
        public string Freight { get; set; }
        public string Total { get; set; }
        public string ShipPostalCode { get; set; }
        public string RequiredDate { get; set; }
        public string ShippedDate { get; set; }
        public string ExtendedPrice { get; set; }
        public Customer(string orderId, string discount, string shipAddress, string shipCity, string orderDate, string shipCountry, string productName, string quantity, string customerID, string shipName, string unitPrice, string subtotal, string freight, string total, string shipPostalCode, string extendedPrice, string requiredDate, string shippedDate)
        {
            this.CustomerID = customerID;
            this.ProductName = productName;
            this.Quantity = quantity;
            this.ShipName = shipName;
            this.UnitPrice = unitPrice;
            this.Discount = discount;
            this.ShipAddress = shipAddress;
            this.ShipCity = shipCity;
            this.OrderDate = orderDate;
            this.ShipCountry = shipCountry;
            this.OrderId = orderId;
            this.Subtotal = subtotal;
            this.Freight = freight;
            this.Total = total;
            this.ShipPostalCode = shipPostalCode;
            this.ShippedDate = shippedDate;
            this.RequiredDate = requiredDate;
            this.ExtendedPrice = extendedPrice;
        }
    }
}
