using System;
using System.Collections.Generic;
using System.Linq;
using ThameJordan25SU233x;

namespace ACS_JThameM7
{
    //
    public class clsCartItem
    {
        //
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int QuantityInStock { get; set; }

        //
        public clsCartItem(int itemID, string itemName, decimal price, int quantity, int quantityInStock)
        {
            ItemID = itemID;
            ItemName = itemName;
            Price = price;
            Quantity = quantity;
            QuantityInStock = quantityInStock;
        }

        public decimal TotalPrice => Price * Quantity;
    }

    //
    public class clsCart
    {
        public int AccountID { get; set; }

        //
        public List<clsCartItem> CartItems { get; set; } = new List<clsCartItem>();

        public int OrderNumber { get; private set; }
        public decimal Subtotal { get; private set; }
        public decimal DiscountAmount { get; private set; } = 0m;
        public decimal TaxAmount { get; private set; }
        public decimal TotalDue { get; private set; }
        public decimal DiscountedSubtotal { get; private set; }

        private const decimal TAX_RATE = 0.0825m;

        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }

        public decimal DiscountPercentage { get; set; } = 0m;
        public decimal DiscountDollarAmount { get; set; } = 0m;

        //
        public List<clsItemDiscount> ItemDiscounts { get; set; } = new List<clsItemDiscount>();

        public string DiscountCode { get; set; }

        //
        public clsCart(int accountId, List<clsCartItem> cartItems)
        {
            AccountID = accountId;
            CartItems = cartItems ?? new List<clsCartItem>();
            GenerateOrderNumber();
            Recalculate(); 
        }

        // Recalculate values/amounts
        public void Recalculate()
        {
            // Variable(s)
            Subtotal = CartItems.Sum(i => i.TotalPrice);
            decimal subtotal = Subtotal;
            decimal itemDiscountTotal = 0m;

            foreach (var item in CartItems)
            {
                var match = ItemDiscounts.FirstOrDefault(d => d.ItemID == item.ItemID);
                if (match != null)
                {
                    decimal saved = 0m;

                    if (match.DiscountPercentage > 0)
                    {
                        saved = Math.Round(item.Price * item.Quantity * match.DiscountPercentage, 2);
                    }
                    else if (match.DiscountDollarAmount > 0)
                    {
                        saved = Math.Round(match.DiscountDollarAmount * item.Quantity, 2);
                    }

                    itemDiscountTotal += saved;
                }
            }

            DiscountedSubtotal = subtotal - itemDiscountTotal;

            decimal cartLevelDiscount = 0m;
            if (!string.IsNullOrEmpty(DiscountCode) && clsSQL.IsCartLevelDiscount(DiscountCode))
            {
                var info = clsSQL.GetDiscountInfo(DiscountCode);
                if (info.DiscountPercentage > 0)
                {
                    cartLevelDiscount = Math.Round(DiscountedSubtotal * info.DiscountPercentage, 2);
                }
                else if (info.DiscountDollarAmount > 0)
                {
                    cartLevelDiscount = info.DiscountDollarAmount;
                    if (cartLevelDiscount > DiscountedSubtotal)
                        cartLevelDiscount = DiscountedSubtotal;
                }

                DiscountedSubtotal -= cartLevelDiscount;
            }

            DiscountAmount = Math.Round(itemDiscountTotal + cartLevelDiscount, 2);
            TaxAmount = Math.Round(DiscountedSubtotal * TAX_RATE, 2);
            TotalDue = DiscountedSubtotal + TaxAmount;
        }

        // Generate order number
        private void GenerateOrderNumber()
        {
            Random orderIDGenerator = new Random();
            OrderNumber = orderIDGenerator.Next(10000, 99999);
        }
    }
}