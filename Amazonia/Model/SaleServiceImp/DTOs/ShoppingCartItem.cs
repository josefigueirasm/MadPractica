﻿using System;
using System.Collections.Generic;

namespace Es.Udc.DotNet.Amazonia.Model.SaleServiceImp.DTOs
{
    [Serializable()]
    public class ShoppingCartItem
    {
        public long units { get; set; }
        public double price { get; set; }
        public bool gift { get; set; }
        public long productId { get; set; }
        public string productName { get; set; }

        public ShoppingCartItem(long units, bool gift, long productId, string productName)
        {
            this.units = units;
            this.gift = gift;
            this.productId = productId;
            this.price = 0;
            this.productName = productName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;

            ShoppingCartItem target = obj as ShoppingCartItem;

            return true
                && (this.productId == target.productId)
                ;
        }

        public override int GetHashCode()
        {
            var hashCode = -1781825586;
            hashCode = hashCode * -1521134295 + units.GetHashCode();
            hashCode = hashCode * -1521134295 + price.GetHashCode();
            hashCode = hashCode * -1521134295 + gift.GetHashCode();
            hashCode = hashCode * -1521134295 + productId.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(productName);
            return hashCode;
        }
    }
}