using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TravelInCloud.Models
{
    /// <summary>
    /// 商店用户
    /// </summary>
    public class Store : ApplicationUser
    {
        public virtual int StoreId { get; set; }

        /// <summary>
        /// 商店里的商品
        /// </summary>
        [InverseProperty(nameof(IBuyableProduct.Owner))]
        public virtual List<IBuyableProduct> Products { get; set; }
    }

    /// <summary>
    /// 商品
    /// </summary>
    public interface IBuyableProduct
    {
        /// <summary>
        /// 商品的子商品类型
        /// </summary>
        List<IBuyableItem> ProductTypes { get; set; }
        /// <summary>
        /// 商品所属的商店
        /// </summary>
        [ForeignKey(nameof(OwnerId))]
        Store Owner { get; set; }
        int OwnerId { get; set; }
    }

    /// <summary>
    /// 品型 - 即商品的一种类型
    /// </summary>
    public interface IBuyableItem
    {
        /// <summary>
        /// 品型的所属商品
        /// </summary>
        [ForeignKey(nameof(BelongingProductId))]
        IBuyableProduct BelongingProduct { get; set; }
        int BelongingProductId { get; set; }

        /// <summary>
        /// 该品型加入的所有订单
        /// </summary>
        [InverseProperty(nameof(ProductInOrder.Product))]
        List<ProductInOrder> BelongingOrders { get; set; } 
    }

    /// <summary>
    /// 景区
    /// </summary>
    public class Sight:IBuyableProduct
    {
        public virtual int SightId { get; set; }

        /// <summary>
        /// 景区所属用户
        /// </summary>
        [ForeignKey(nameof(OwnerId))]
        public Store Owner { get; set; }
        public int OwnerId { get; set; }

        /// <summary>
        /// 景区的票型
        /// </summary>
        [InverseProperty(nameof(IBuyableItem.BelongingProduct))]
        public List<IBuyableItem> ProductTypes { get; set; }

    }
    /// <summary>
    /// 票型
    /// </summary>
    public class SightType : IBuyableItem
    {
        public virtual int SightTypeId { get; set; }

        /// <summary>
        /// 票型的所属景区
        /// </summary>
        [ForeignKey(nameof(BelongingProductId))]
        public int BelongingProductId { get; set; }
        public IBuyableProduct BelongingProduct { get; set; }

        /// <summary>
        /// 该票型所加入的所有订单
        /// </summary>
        [InverseProperty(nameof(ProductInOrder.Product))]
        public List<ProductInOrder> BelongingOrders { get; set; }
    }
    /// <summary>
    /// 订单
    /// </summary>
    public class Order
    {
        public virtual int OrderId { get; set; }

        /// <summary>
        /// 订单所属用户
        /// </summary>
        [ForeignKey(nameof(OwnerId))]
        public virtual ApplicationUser Owner { get; set; }
        public virtual string OwnerId  { get; set; }

        /// <summary>
        /// 订单里的所有商品
        /// </summary>
        [InverseProperty(nameof(ProductInOrder.BelongingOrder))]
        public virtual List<ProductInOrder> Products { get; set; }
    }
    /// <summary>
    /// 订单与商品关系对象
    /// </summary>
    public class ProductInOrder
    {
        public virtual int ProductInOrderId { get; set; }

        /// <summary>
        /// PO的所属订单
        /// </summary>
        [ForeignKey(nameof(OrderId))]
        public virtual Order BelongingOrder { get; set; }
        public virtual int OrderId { get; set; }

        /// <summary>
        /// PO中的商品
        /// </summary>
        [ForeignKey(nameof(ProductId))]
        public virtual IBuyableItem Product { get; set; }
        public virtual int ProductId { get; set; }
    }
}
