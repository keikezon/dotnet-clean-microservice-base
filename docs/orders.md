# 📦 Orders Service

Responsável por gerenciar **pedidos**, incluindo:
- Criação e atualização de pedidos
- Emissão de eventos de "OrderCreated" e "OrderUpdated"
- Integração com estoque

## 🛠️ Tecnologias
- .NET 8 / C#
- MassTransit + RabbitMQ
- PostgreSQL
- Docker + Docker Compose
- OpenTelemetry + Jaeger

---

## 🔌 Endpoints
| Método | Rota            | Descrição         | Autenticação |
|--------|-----------------|------------------|--------------|
| POST    | `/api/orders`  | Criar pedido     | ✅ Bearer JWT |
| GET     | `/api/orders`  | Listar pedidos   | ✅ Bearer JWT |
| GET     | `/api/orders/{id}` | Detalhar pedido | ✅ Bearer JWT |

---

### Criar Pedido

Cria um novo pedido no sistema e retorna os detalhes do pedido criado.

**Endpoint:** POST /api/orders

**Autenticação:** `Bearer Token` obrigatório

**Request Body:**

```json
{
  "items": [
    {
      "productId": "11111111-1111-1111-1111-111111111111",
      "quantity": 1
    },
    {
      "productId": "22222222-2222-2222-2222-222222222222",
      "quantity": 3
    }
  ],
  "clientDocument": "123"
}
```

### Listar Pedidos

Lista os pedidos no sistema.

**Endpoint:** GET /api/orders

**Autenticação:** `Bearer Token` obrigatório

### Detalhar Pedido

Detalhe o pedido no sistema.

**Endpoint:** GET /api/orders/{id}

Exemplo de {id}: 11111111-1111-1111-1111-111111111111

**Autenticação:** `Bearer Token` obrigatório

---

## 📨 Eventos publicados
| Evento         | Quando disparado                             |
|----------------|----------------------------------------------|
| `OrderCreated` | Quando um novo pedido é criado               |
| `StockUpdated` | Quando um estoque de um produto é atualizado |

---

## 🧪 Como rodar localmente

```bash
dotnet run --project src/Services/Order/Order.API
```

Rodar apenas os testes desse serviço:
```bash
dotnet test src/Services/Order/Order.Tests
```