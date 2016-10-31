using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TravelInCloud.Models
{
    /// <summary>
    /// 商店的类型
    /// </summary>
    public enum StoreType : int
    {
        Hotel = 1,
        Sight = 2,
        KTV = 3,
        Bath = 4,
        Restaurant = 10,
        Mall = 11,
        Car = 12,
        TravelArount = 13,
        TravelWithGroup = 14
    }

    /// <summary>
    /// 商店用户
    /// </summary>
    public class Store : ApplicationUser
    {
        public Store() { }
        public Store(ApplicationUser _user)
        {
            this.AccessFailedCount = _user.AccessFailedCount;
            this.ConcurrencyStamp = _user.ConcurrencyStamp;
            this.Email = _user.Email;
            this.EmailConfirmed = _user.EmailConfirmed;
            this.IconAddress = _user.IconAddress;
            this.Id = _user.Id;
            this.LockoutEnabled = _user.LockoutEnabled;
            this.LockoutEnd = _user.LockoutEnd;
            this.NickName = _user.NickName;
            this.NormalizedEmail = _user.NormalizedEmail;
            this.NormalizedUserName = _user.NormalizedUserName;
            this.openid = _user.openid;
            this.Orders = _user.Orders;
            this.PasswordHash = _user.PasswordHash;
            this.PhoneNumber = _user.PhoneNumber;
            this.PhoneNumberConfirmed = _user.PhoneNumberConfirmed;
            this.RegisterTime = _user.RegisterTime;
            this.SecurityStamp = _user.SecurityStamp;
            this.TwoFactorEnabled = _user.TwoFactorEnabled;
            this.UserName = _user.UserName;
        }
        public virtual StoreType StoreType { get; set; }
        public virtual string StoreOwnerName { get; set; }
        public virtual string StoreOwnerCode { get; set; }
        public virtual string StoreName { get; set; }
        public virtual string StoreDescription { get; set; }
        public virtual string StoreLocation { get; set; }
        public virtual DateTime StartStoreTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 商店里的商品
        /// </summary>
        [InverseProperty(nameof(Product.Owner))]
        public virtual List<Product> Products { get; set; }
    }

    /// <summary>
    /// 商品
    /// </summary>
    public class Product
    {
        public virtual int ProductId { get; set; }
        public virtual string ProductName { get; set; }
        public virtual string ProductDetails { get; set; }
        public virtual string ProductInfo { get; set; }
        public virtual string ProductWarnning { get; set; }
        public virtual int BuyTimes { get; set; } = 0;
        public virtual int ViewTimes { get; set; } = 0;

        /// <summary>
        /// 商品的图片列表
        /// </summary>
        [InverseProperty(nameof(ImageOfProduct.Product))]
        public List<ImageOfProduct> ImageOfProducts { get; set; }
        public string TryGetFirst()
        {
            try
            {
                return ImageOfProducts[0].ImageSrc;
            }
            catch
            {
                return "http://s.cn.bing.net/az/hprichbg/rb/GreaterKudu_ZH-CN8868031087_1920x1080.jpg";
            }
        }

        /// <summary>
        /// 商品所属用户
        /// </summary>
        [ForeignKey(nameof(OwnerId))]
        public Store Owner { get; set; }
        public string OwnerId { get; set; }

        /// <summary>
        /// 商品的品型
        /// </summary>
        [InverseProperty(nameof(ProductType.BelongingProduct))]
        public List<ProductType> ProductTypes { get; set; }
        public decimal TryGetPrice()
        {
            try
            {
                return ProductTypes.Min(t => t.Price);
            }
            catch { return 0; }
        }
    }

    /// <summary>
    /// 商品的一个图片
    /// </summary>
    public class ImageOfProduct
    {
        public virtual int ImageOfProductId { get; set; }


        [ForeignKey(nameof(ProductId))]
        public virtual Product Product { get; set; }
        public virtual int ProductId { get; set; }

        public virtual string ImageDescription { get; set; }
        public virtual string ImageSrc { get; set; }
    }

    /// <summary>
    /// 商品类型\票型
    /// </summary>
    public class ProductType
    {
        public virtual int ProductTypeId { get; set; }

        public virtual string ProductTypeName { get; set; }
        public virtual string ProductTypeDetails { get; set; }
        public virtual decimal Price { get; set; }
        public virtual decimal OldPrice { get; set; }

        /// <summary>
        /// 票型的所属商品
        /// </summary>
        [ForeignKey(nameof(BelongingProductId))]
        public int BelongingProductId { get; set; }
        public Product BelongingProduct { get; set; }

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
        public virtual string OwnerId { get; set; }

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
        [ForeignKey(nameof(ProductTypeId))]
        public virtual ProductType ProductType { get; set; }
        public virtual int ProductTypeId { get; set; }
    }
}
