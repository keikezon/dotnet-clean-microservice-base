# 📦 Products Service

Responsável por gerenciar **produtos**, incluindo:
- Criação e atualização de produtos
- Atualização de estoque
- Emissão de eventos de "ProductCreated", "ProductUpdated" e "StockUpdated"

## 🛠️ Tecnologias
- .NET 8 / C#
- MassTransit + RabbitMQ
- PostgreSQL
- Docker + Docker Compose
- OpenTelemetry + Jaeger

---

## 🔌 Endpoints
| Método | Rota              | Descrição                     | Autenticação |
|--------|-------------------|-------------------------------|--------------|
| POST   | `/api/products`   | Criar produto                 | ✅ Bearer JWT |
| PUT    | `/api/products`   | Atualizar produto             | ✅ Bearer JWT |
| GET    | `/api/products`      | Listar produtos               | ✅ Bearer JWT |
| GET    | `/api/products/{id}` | Detalhar produto              | ✅ Bearer JWT |
| DELETE | `/api/products/{id}` | Deletar produto               | ✅ Bearer JWT |
| PUT    | `/api/products/increase-stock`      | Aumentar o estoque do produto | ✅ Bearer JWT |
| PUT    | `/api/products/decrease-stock`      | Diminuir o estoque do produto |              |

---

### Criar Produto

Cria um novo produto no sistema e retorna os detalhes do produto criado.

**Endpoint:** POST /api/products

**Request Body:**

```json
{
  "name": "Churo de frango",
  "description": "Patê para gatos",
  "price": 24,
  "stock": 10
}
```

### Listar Produto

Lista os produtos do sistema.

**Endpoint:** GET /api/products

**Autenticação:** `Bearer Token` obrigatório

### Detalhar Produto

Retorna os detalhes de um produto específico.

**Endpoint:** GET /api/products/{id}

Exemplo de {id}: 11111111-1111-1111-1111-111111111111

**Autenticação:** `Bearer Token` obrigatório

### Deletar Produto

Loga um novo produto no sistema.

**Endpoint:** DELETE /api/products

**Autenticação:** `Bearer Token` obrigatório

### Aumentar o estoque do produto

Aumentar o estoque do produto no sistema.

**Endpoint:** POST /api/products/increase-stock

**Autenticação:** `Bearer Token` obrigatório

**Request Body:**

```json
{
  "id": "11111111-1111-1111-1111-111111111111",
  "quantity": 10,
  "invoice": "teste"
}
```

### Diminuir o estoque do produto

Diminuir o estoque do produto no sistema.

**Endpoint:** POST /api/products/decrease-stock

**Request Body:**

```json
{
  "id": "11111111-1111-1111-1111-111111111111",
  "quantity": 1
}
```

---

## 📨 Eventos publicados
| Evento           | Quando disparado                         |
|------------------|------------------------------------------|
| `ProductCreated` | Quando um novo produto é criado          |
| `ProductUpdated` | Quando um produto é alterado         |
| `StockUpdated`   | Quando o estoque de um produto é alterado |

---

## 🧪 Como rodar localmente

```bash
dotnet run --project src/Services/Product/Product.API
```

Rodar apenas os testes desse serviço:
```bash
dotnet test src/Services/Product/Product.Tests
```