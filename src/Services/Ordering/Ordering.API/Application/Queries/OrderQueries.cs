namespace Microsoft.eShopOnContainers.Services.Ordering.API.Application.Queries;
    
public class OrderQueries
    : IOrderQueries
{
    private string _connectionString = string.Empty;

    public OrderQueries(string constr)
    {
        _connectionString = !string.IsNullOrWhiteSpace(constr) ? constr : throw new ArgumentNullException(nameof(constr));
    }


    public async Task<Order> GetOrderAsync(int id)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var result = await connection.QueryAsync<dynamic>(
            @"select 
                        o.[Id] as ordernumber,
                        o.OrderDate as date, 
                        o.Description as description,
                        o.Address_City as city, 
                        o.Address_Country as country, 
                        o.Address_State as state, 
                        o.Address_Street as street,
                        o.Address_ZipCode as zipcode,
                        os.Name as status, 
                        oi.ProductName as productname, 
                        oi.Units as units, 
                        oi.UnitPrice as unitprice, 
                        oi.PictureUrl as pictureurl,
                        CASE WHEN o.[Discount_Amount] IS NOT NULL AND o.[Discount_DiscountConfirmed] = 1
                            THEN o.[Discount_Amount]
                            ELSE 0
                        END as discount
                    FROM ordering.Orders o
                    LEFT JOIN ordering.Orderitems oi ON o.Id = oi.orderid 
                    LEFT JOIN ordering.orderstatus os on o.OrderStatusId = os.Id
                    WHERE o.Id=@id"
                , new { id }
            );

        if (result.AsList().Count == 0)
            throw new KeyNotFoundException();

        return MapOrderItems(result);
    }

    public async Task<IEnumerable<OrderSummary>> GetOrdersFromUserAsync(Guid userId)
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var result = await connection.QueryAsync<dynamic>(@"SELECT 
                        o.[Id] as ordernumber,
                        o.[OrderDate] as [date],
                        os.[Name] as [status], 
                        SUM(oi.units*oi.unitprice) as total,
                        CASE WHEN o.[Discount_Amount] IS NOT NULL AND o.[Discount_DiscountConfirmed] = 1
                            THEN o.[Discount_Amount]
                            ELSE 0
                        END as discount
                    FROM [ordering].[Orders] o
                    LEFT JOIN[ordering].[orderitems] oi ON  o.Id = oi.orderid 
                    LEFT JOIN[ordering].[orderstatus] os on o.OrderStatusId = os.Id                     
                    LEFT JOIN[ordering].[buyers] ob on o.BuyerId = ob.Id
                    WHERE ob.IdentityGuid = @userId
                    GROUP BY o.[Id], o.[OrderDate], os.[Name], o.[Discount_Amount], o.[Discount_DiscountConfirmed]
                    ORDER BY o.[Id]", new { userId });

        return result.Select(x => new OrderSummary()
        {
            ordernumber = x.ordernumber,
            date = x.date,
            status = x.status,
            total = x.total > x.discount ? x.total - x.discount : 1,
        });
    }

    public async Task<IEnumerable<CardType>> GetCardTypesAsync()
    {
        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        return await connection.QueryAsync<CardType>("SELECT * FROM ordering.cardtypes");
    }

    private Order MapOrderItems(dynamic result)
    {
        decimal discount = result[0].discount;
        decimal total = 0;
        var orderItems = new List<Orderitem>();

        foreach (dynamic item in result)
        {
            var orderitem = new Orderitem
            {
                productname = item.productname,
                units = item.units,
                unitprice = (double)item.unitprice,
                pictureurl = item.pictureurl
            };

            total += item.units * item.unitprice;
            orderItems.Add(orderitem);
        }
        
        var order = new Order
        {
            ordernumber = result[0].ordernumber,
            date = result[0].date,
            status = result[0].status,
            description = result[0].description,
            street = result[0].street,
            city = result[0].city,
            zipcode = result[0].zipcode,
            country = result[0].country,
            orderitems = orderItems,
            discount = result[0].discount,
            total = total,
            totalWithDiscount = total > discount ? total - discount : 1,
        };

        return order;
    }
}
