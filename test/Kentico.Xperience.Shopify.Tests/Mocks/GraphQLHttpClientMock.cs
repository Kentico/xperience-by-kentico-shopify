using GraphQL;
using GraphQL.Client.Abstractions;

using Kentico.Xperience.Shopify.ShoppingCart;
using Kentico.Xperience.Shopify.ShoppingCart.GraphQLModels;
using Kentico.Xperience.Shopify.Tests.Repositories;

namespace Kentico.Xperience.Shopify.Tests.Mocks
{
    internal class GraphQLHttpClientMock : IGraphQLClient
    {
        private ShoppingCartRepository CartRepository => new();
        private DiscountCodesRepository DiscountCodesRepository => new();

        public GraphQLHttpClientMock()
        {

        }

        public IObservable<GraphQLResponse<TResponse>> CreateSubscriptionStream<TResponse>(GraphQLRequest request) => throw new NotImplementedException();
        public IObservable<GraphQLResponse<TResponse>> CreateSubscriptionStream<TResponse>(GraphQLRequest request, Action<Exception> exceptionHandler) => throw new NotImplementedException();


        public Task<GraphQLResponse<TResponse>> SendMutationAsync<TResponse>(GraphQLRequest request, CancellationToken cancellationToken = default)
        {
            object? response = null;
            if (typeof(TResponse) == typeof(UpdateCartLinesResponse))
            {
                response = HandleCartLineItemUpdate(request);
            }
            else if (typeof(TResponse) == typeof(RemoveCartItemResponse))
            {
                response = HandleCartItemRemove(request);
            }
            else if (typeof(TResponse) == typeof(AddToCartResponse))
            {
                response = HandleAddToCart(request);
            }
            else if (typeof(TResponse) == typeof(CreateCartResponse))
            {
                response = HandleCreateCart(request);
            }
            else if (typeof(TResponse) == typeof(UpdateDiscountCodesResponse))
            {
                response = HandleUpdateDiscountCodes(request);
            }

            if (response == null)
            {
                throw new NotImplementedException("Given response data type is not implemented with combination of request variables type");
            }

            return Task.FromResult(WrapResult<TResponse>(response));
        }


        private UpdateDiscountCodesResponse HandleUpdateDiscountCodes(GraphQLRequest request)
        {
            string? cartId = request.GetProperty<string>("CartId");
            string[]? discountCodes = request.GetProperty<string[]>("discountCodes");

            var availableDiscountCodes = DiscountCodesRepository.DiscountCodes;
            var cart = CartRepository.Carts.FirstOrDefault(x => x.Id == cartId);

            if (cart != null && discountCodes != null)
            {
                cart.DiscountCodes = discountCodes.Select(x => new DiscountCode
                {
                    Code = x,
                    Applicable = availableDiscountCodes.Contains(x)
                });
            }

            return new UpdateDiscountCodesResponse()
            {
                CartDiscountCodesUpdate = new CartResponseBase()
                {
                    Cart = cart,
                    UserErrors = []
                }
            };
        }

        private CreateCartResponse HandleCreateCart(GraphQLRequest request)
        {
            var cart = CartRepository.Carts.First();
            var createCartParams = request.GetProperty<CreateCartParameters>("CartInput");

            if (cart.Lines?.Edges is not null && createCartParams?.Lines is not null)
            {
                cart.Lines.Edges = createCartParams.Lines.Select(x => CreateCartLineEdge(x.MerchandiseId, x.Quantity));
            }

            return new CreateCartResponse()
            {
                CartCreate = new CartResponseBase()
                {
                    Cart = cart,
                    UserErrors = []
                }
            };
        }


        public Task<GraphQLResponse<TResponse>> SendQueryAsync<TResponse>(GraphQLRequest request, CancellationToken cancellationToken = default)
        {
            object? response = null;
            if (typeof(TResponse) == typeof(GetCartResponse))
            {
                response = HandleGetCart(request);
            }

            if (response == null)
            {
                throw new NotImplementedException("Given response data type is not implemented");
            }

            return Task.FromResult(WrapResult<TResponse>(response));
        }


        private UpdateCartLinesResponse? HandleCartLineItemUpdate(GraphQLRequest request)
        {
            if (request.Variables is not UpdateCartLineParameters updateParams)
            {
                return null;
            }

            var cartResponse = new UpdateCartLinesResponse()
            {
                CartLinesUpdate = new CartResponseBase()
                {
                    UserErrors = [],
                    Cart = CartRepository.Carts.First()
                }
            };
            var cartLines = cartResponse.CartLinesUpdate.Cart.Lines?.Edges;

            var updatedLine = cartLines?.FirstOrDefault(x => x.Node.Id == updateParams.Lines.Id);
            if (updatedLine != null && updatedLine.Node != null)
            {
                if (updateParams.Lines.Quantity > 0)
                {
                    updatedLine.Node.Quantity = updateParams.Lines.Quantity;
                }
                else
                {
                    var list = cartLines!.ToList();
                    list.Remove(updatedLine);
                    cartResponse.CartLinesUpdate.Cart.Lines!.Edges = list;
                }
            }

            return cartResponse;
        }


        private GetCartResponse? HandleGetCart(GraphQLRequest request)
        {
            string? cartId = request.GetProperty("cartId")?.ToString();

            var cart = CartRepository.Carts.FirstOrDefault(x => x.Id == cartId);
            if (cart?.Id is null || cartId is null || cart.Id != cartId)
            {
                return null;
            }

            return new GetCartResponse()
            {
                Cart = cart
            };
        }

        private RemoveCartItemResponse HandleCartItemRemove(GraphQLRequest request)
        {
            string? cartId = request.GetProperty("CartId")?.ToString();
            string[]? cartLinesToRemove = (string[]?)Convert.ChangeType(request.GetProperty("LineIds"), typeof(string[]));

            var cart = CartRepository.Carts.FirstOrDefault(cart => cart.Id == cartId);

            var updatedCartLines = cart?.Lines?.Edges.Where(x => !cartLinesToRemove?.Contains(x.Node.Id) ?? false) ?? [];
            if (cart?.Lines is not null)
            {
                cart.Lines.Edges = updatedCartLines;
            }

            return new RemoveCartItemResponse()
            {
                CartLinesRemove = new CartResponseBase()
                {
                    Cart = cart,
                    UserErrors = []
                }
            };
        }


        private AddToCartResponse HandleAddToCart(GraphQLRequest request)
        {
            string? cartId = request.GetProperty("CartId")?.ToString();
            var cartLineToAdd = request.GetProperty<AddToCartLines>("Lines");

            var newLineItem = new CartLineEdge()
            {
                Node = new CartLineNode()
                {
                    Quantity = cartLineToAdd?.Quantity ?? 0,
                    Id = "TestCartItemId",
                    Cost = new CartCost()
                    {
                        TotalAmount = new PriceDto()
                        {
                            Amount = 10m,
                            CurrencyCode = ShopifySharp.GraphQL.CurrencyCode.CZK
                        },
                        SubtotalAmount = new PriceDto()
                        {
                            Amount = 10m,
                            CurrencyCode = ShopifySharp.GraphQL.CurrencyCode.CZK
                        }
                    },
                    Merchandise = new Merchandise()
                    {
                        Id = cartLineToAdd?.MerchandiseId ?? string.Empty,
                        Title = "New added variant name",
                        Product = new VariantProduct()
                        {
                            Title = "Added product name"
                        }
                    }
                }
            };

            var cart = CartRepository.Carts.FirstOrDefault(cart => cart.Id == cartId);
            var updatedCartLines = cart?.Lines?.Edges.Append(newLineItem) ?? [];
            if (cart?.Lines is not null)
            {
                cart.Lines.Edges = updatedCartLines;
            }

            return new AddToCartResponse()
            {
                CartLinesAdd = new CartResponseBase()
                {
                    Cart = cart,
                    UserErrors = []
                }
            };
        }


        private GraphQLResponse<TResponse> WrapResult<TResponse>(object result, GraphQLError[]? errors = null) => new()
        {
            Data = (TResponse)Convert.ChangeType(result, typeof(TResponse)),
            Errors = errors ?? []
        };


        private CartLineEdge CreateCartLineEdge(string? merchandiseID, int quantity) => new()
        {
            Node = new CartLineNode()
            {
                Id = "CartLine/12549687egs8sgs9g87",
                Quantity = quantity,
                Cost = new CartCost()
                {
                    TotalAmount = new PriceDto()
                    {
                        Amount = 10m,
                        CurrencyCode = ShopifySharp.GraphQL.CurrencyCode.CZK
                    }
                },
                Merchandise = new Merchandise()
                {
                    Id = merchandiseID ?? string.Empty,
                    Title = "Variant title",
                    Product = new VariantProduct()
                    {
                        Title = "product title"
                    }
                }
            }
        };
    }
}
