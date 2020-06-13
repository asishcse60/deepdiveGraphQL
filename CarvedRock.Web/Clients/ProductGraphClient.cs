using System.Reactive.Linq;
using System.Threading.Tasks;
using CarvedRock.Web.Models;
using GraphQL;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;

namespace CarvedRock.Web.Clients
{
    public class ProductGraphClient
    {
        private readonly IGraphQLClient _client;

        public ProductGraphClient(IGraphQLClient client)
        {
            _client = client;
        }

        public async Task<ProductModel> GetProduct(int id)
        {
            var query = new GraphQLRequest
            {
                Query = @" 
                query productQuery($productId: ID!)
                { product(id: $productId) 
                    { id name price rating photoFileName description stock introducedAt 
                      reviews { title review }
                    }
                }",
                Variables = new {productId = id}
            };

            //var response = await _client.PostAsync(query);
            // var dataFieldAs = response.GetDataFieldAs<ProductModel>("product");
           // return dataFieldAs;

            var response = await _client.SendQueryAsync<ProductModel>(query);
            var result = response.Data;
            return result;
        }

        public async Task AddReview(ProductReviewModel review)
        {
            var query = new GraphQLRequest
            {
                Query = @" 
                mutation($review: reviewInput!)
                {
                    createReview(review: $review)
                    {
                        id
                    }
                }",
                Variables = new { review }
            };
            //var response = await _client.PostAsync(query);
            //var reviewReturned = response.GetDataFieldAs<ProductReviewModel>("createReview");
            var response = await _client.SendQueryAsync<ProductModel>(query);
            var result = response.Data;
           // return result;
        }

        public async Task<ProductModel> CreateOwner(ProductReviewModel ownerToCreate)
        {
            var query = new GraphQLRequest
            {
                Query = @"
                            mutation($owner: ownerInput!){
                              createOwner(owner: $owner){
                                id,
                                name,
                                address
                              }
                            }",
                Variables = new { owner = ownerToCreate }
            };

            var response = await _client.SendMutationAsync<ProductModel>(query);
            return response.Data;
        }

        public async Task<ProductModel> UpdateOwner(string id, ProductModel ownerToUpdate)
        {
            var query = new GraphQLRequest
            {
                Query = @"
                            mutation($owner: ownerInput!, $ownerId: ID!){
                              updateOwner(owner: $owner, ownerId: $ownerId){
                                id,
                                name,
                                address
                              }
                            }",
                Variables = new { owner = ownerToUpdate, ownerId = id }
            };

            var response = await _client.SendMutationAsync<ProductModel>(query);
            return response.Data;
        }

        public async Task<ProductModel> DeleteOwner(string id)
        {
            var query = new GraphQLRequest
            {
                Query = @"
                           mutation($ownerId: ID!){
                              deleteOwner(ownerId: $ownerId)
                            }",
                Variables = new { ownerId = id }
            };

            var response = await _client.SendMutationAsync<ProductModel>(query);
            return response.Data;
        }

        public async Task SubscribeToUpdates()
        {
            var queryRequest = new GraphQLRequest   
            {
                Query = @"
                           subscription{ reviewAdded { title productId } }",
            };

            var result = await _client.CreateSubscriptionStream<ProductReviewModel>(
                queryRequest);
            
           // var result = await _client.SendSubscribeAsync("subscription { reviewAdded { title productId } }");
            //result.OnReceive += Receive;
        }

        private void Receive(GraphQLResponse<ProductReviewModel> resp)
        {
            var review = resp.Data.Review;
        }
    }
}
