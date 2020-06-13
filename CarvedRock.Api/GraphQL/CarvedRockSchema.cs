using System;
using GraphQL;
using GraphQL.Types;
using GraphQL.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace CarvedRock.Api.GraphQL
{
    public class CarvedRockSchema: Schema
    {
        public CarvedRockSchema(IDependencyResolver resolver): base(resolver)
        {
            Query = resolver.Resolve<CarvedRockQuery>();
            Mutation = resolver.Resolve<CarvedRockMutation>();
            Subscription = resolver.Resolve<CarvedRockSubscription>();
        }
    }
}
