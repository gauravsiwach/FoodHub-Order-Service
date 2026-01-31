# FoodHub Order Service

## Overview
FoodHub Order Service is a standalone microservice following Clean Architecture:
- **Domain**: Core entities, value objects, and domain rules
- **Application**: Use cases, DTOs, and repository abstractions
- **Infrastructure**: Cosmos DB persistence and external implementations
- **API**: GraphQL-first HTTP host using Hot Chocolate

## Architecture
- **Clean Architecture** with strict dependencies: Domain → Application → Infrastructure → API
- **GraphQL** API via Hot Chocolate with queries and mutations
- **Persistence**: Azure Cosmos DB document mapping
- **Observability**: Serilog logging with correlation-id middleware
- **Security**: JWT Bearer authentication and Google token validation

## Configuration
Update [src/FoodHub.Api/appsettings.json](src/FoodHub.Api/appsettings.json) with your values:
- **Cosmos**: `Endpoint`, `Key`, `DatabaseName`, `Containers.Order.Name`
- **Jwt**: `Issuer`, `Audience`, `Secret`, `ExpiryMinutes`
- **GoogleAuth**: `ClientId`, `Aud`

## Running
1. Restore packages and build:
   - `dotnet build`
2. Run the API:
   - `dotnet run --project src/FoodHub.Api/FoodHub.Api.csproj`
3. Open GraphQL endpoint:
   - `http://localhost:5000/graphql` (or the port shown in the console)

## GraphQL
### Queries
- `getOrderById(id: Guid)`
- `getOrdersByUser(userId: Guid)`

### Mutations
- `createOrder(input: CreateOrderDto)`
- `updateOrderStatus(id: Guid, status: OrderStatus)`
