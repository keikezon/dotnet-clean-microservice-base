# 📦 User Service

Responsável por gerenciar **usuários**, incluindo:
- Criação e atualização de usuários
- Emissão de eventos de "UserCreated" e "UserUpdated"
- Integração com autenticação/identidade

## 🛠️ Tecnologias
- .NET 8 / C#
- MassTransit + RabbitMQ
- PostgreSQL
- Docker + Docker Compose
- OpenTelemetry + Jaeger

---

## 🔌 Endpoints
| Método | Rota              | Descrição        | Autenticação |
|--------|-------------------|------------------|--------------|
| POST   | `/api/users`      | Criar usuário    |  |
| GET    | `/api/users`      | Listar usuários  | ✅ Bearer JWT |
| GET    | `/api/users/{id}` | Detalhar usuário | ✅ Bearer JWT |
| POST   | `/api/login` | Logar usuário    |  |

---

## Usuário admin para testes (pré-cadastrado, pronto para uso)
email: admin@email.com
password: admin@123

---

### Criar Usuário

Cria um novo usuário no sistema e retorna os detalhes do usuário criado.

**Endpoint:** POST /api/users

**Request Body:**

```json
{
  "name": "Teste",
  "email": "teste@email.com",
  "password": "Teste@123"
}
```

### Listar Usuários

Lista os usuários do sistema.

**Endpoint:** GET /api/users

**Autenticação:** `Bearer Token` obrigatório

### Detalhar Usuário

Retorna os detalhes de um usuário específico.

**Endpoint:** GET /api/users/{id}

Exemplo de {id}: 11111111-1111-1111-1111-111111111111

**Autenticação:** `Bearer Token` obrigatório

### Logar Usuário

Loga um novo usuário no sistema.

**Endpoint:** POST /api/login

**Request Body:**

```json
{
  "email": "teste@email.com",
  "password": "Teste@123"
}
```
---

## 📨 Eventos publicados
| Evento         | Quando disparado                           |
|----------------|--------------------------------------------|
| `UserCreated`  |  Quando um novo usuário é criado |

---

## 🧪 Como rodar localmente

```bash
dotnet run --project src/Services/Identity/Identity.API
```

Rodar apenas os testes desse serviço:
```bash
 dotnet test src/Services/Identity/Identity.Tests
```
