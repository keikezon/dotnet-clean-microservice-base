
# 🛒 Guia de Uso - Cadastro, Login, Produtos e Pedidos

Este guia apresenta o **fluxo completo** para utilizar o sistema de microserviços, cobrindo desde a criação de usuários, autenticação, gerenciamento de produtos e estoque, até a finalização de pedidos.

---

## 📋 Pré-requisitos

Antes de começar, certifique-se de que:

- Todos os serviços estão **em execução**:
  - Users Service
  - Products Service
  - Orders Service
  - Api Service
- Bancos de dados e mensageria (RabbitMQ/Kafka) estão **configurados e ativos**
- As **portas e URLs** estão ajustadas para seu ambiente local ou de testes

---

## 🔐 Autenticação

Após realizar o login, você precisará usar o **token JWT** em todas as chamadas que exigem autenticação.

**Cabeçalho obrigatório:**

```
Authorization: Bearer <SEU_TOKEN>
```

Para facilitar, salve o token em uma variável de ambiente no terminal:

```bash
TOKEN="<JWT>"
```

---

## **1️⃣ Cadastrar um usuário**

Cria um novo usuário no sistema.

**Endpoint:**  
```
POST /api/users
```

**Exemplo (cURL):**

```bash
curl -X POST http://localhost:8000/api/users   -H "Content-Type: application/json"   -d '{
    "name": "Teste",
    "email": "teste@email.com",
    "password": "Teste@123"
  }'
```

> 💡 **Dica:**  
> Guarde o e-mail e a senha cadastrados. Eles serão usados no login.

---

## **2️⃣ Fazer login (obter JWT)**

Autentica o usuário e retorna o token de acesso para chamadas autenticadas.

**Endpoint:**  
```
POST /api/login
```

**Exemplo (cURL):**

```bash
curl -X POST http://localhost:8000/api/login   -H "Content-Type: application/json"   -d '{
    "email": "admin@email.com",
    "password": "admin@123"
  }'
```

**Resposta esperada:**

```json
{
  "token": "<JWT>"
}
```

Salve o token retornado:

```bash
TOKEN="<JWT>"
```

Agora todas as chamadas seguintes devem conter o cabeçalho:

```
Authorization: Bearer $TOKEN
```

---

## **3️⃣ Cadastrar um produto (com estoque inicial)**

Cria um novo produto já com quantidade inicial em estoque.

**Endpoint:**  
```
POST /api/products
```

**Exemplo (cURL):**

```bash
curl -X POST http://localhost:8000/api/products   -H "Content-Type: application/json"   -H "Authorization: Bearer $TOKEN"   -d '{
    "name": "Churo de frango",
    "description": "Patê para gatos",
    "price": 24,
    "stock": 10
  }'
```

Guarde o **ID do produto retornado**:

```bash
PRODUCT_ID="11111111-1111-1111-1111-111111111111"
```

---

## **4️⃣ Aumentar o estoque (opcional)**

Caso precise aumentar o estoque de um produto já cadastrado.

**Endpoint:**  
```
PUT /api/products/{id}
```

**Exemplo (cURL):**

```bash
curl -X PUT http://localhost:8000/api/products/increase-stock   -H "Content-Type: application/json"   -H "Authorization: Bearer $TOKEN"   -d '{
    "id": "11111111-1111-1111-1111-111111111111",
    "quantity": 10,
    "invoice": "teste"
  }'
```

> ⚠️ **Atenção:**  
> Certifique-se de que o estoque seja suficiente para atender aos pedidos que serão realizados.

---

## **5️⃣ Realizar um pedido**

Efetiva um pedido utilizando o ID do produto e a quantidade desejada.  
Para mais de um item, inclua todos no array `items`.

**Endpoint:**  
```
POST /api/orders
```

**Exemplo (cURL):**

```bash
curl -X POST http://localhost:8000/api/orders   -H "Content-Type: application/json"   -H "Authorization: Bearer $TOKEN"   -d "{
    \"items\": [
      { \"productId\": \"$PRODUCT_ID\", \"quantity\": 2 }
    ],
    \"clientDocument\": \"123\"
  }"
```

Guarde o **ID do pedido retornado**:

```bash
ORDER_ID="22222222-2222-2222-2222-222222222222"
```

---

## **6️⃣ Conferir o pedido**

Consulta o pedido para verificar detalhes.

**Endpoint:**  
```
GET /api/orders/{id}
```

**Exemplo (cURL):**

```bash
curl -X GET http://localhost:8000/api/orders/$ORDER_ID   -H "Authorization: Bearer $TOKEN"
```

**Resposta esperada (exemplo):**

```json
{
  "id": "22222222-2222-2222-2222-222222222222",
  "status": "Pending",
  "items": [
    {
      "productId": "11111111-1111-1111-1111-111111111111",
      "quantity": 2,
      "price": 79.90
    }
  ],
  "total": 159.80,
  "clientDocument": "123"
}
```

---

## 💡 Dicas finais

- Sempre **valide o token JWT** antes de iniciar o fluxo completo.
- Se o pedido falhar por falta de estoque:
  1. Atualize o campo `stock` do produto.
  2. Tente criar o pedido novamente.
- Guarde os **IDs gerados** (produto, pedido, etc.) para troubleshooting e auditoria futura.

---

## 📚 Resumo dos endpoints

| Serviço   | Método | Rota                           | Descrição                   | Autenticação |
|------------|--------|--------------------------------|-----------------------------|--------------|
| Users      | POST   | `/api/users`                   | Cria novo usuário           | ❌           |
| Users       | POST   | `/api/login`              | Autentica usuário           | ❌           |
| Products   | POST   | `/api/products`                | Cria novo produto           | ✅           |
| Products   | PUT    | `/api/products/increase-stock` | Aumenta o etoque do produto | ✅           |
| Orders     | POST   | `/api/orders`                  | Cria novo pedido            | ✅           |
| Orders     | GET    | `/api/orders/{id}`             | Consulta pedido             | ✅           |

---

## 🌐 Documentação Interativa (Swagger)

Cada serviço expõe sua própria documentação via Swagger:

- **Users:** [http://localhost:8080/swagger](http://localhost:8080/swagger)
- **Products:** [http://localhost:8010/swagger](http://localhost:8010/swagger)
- **Orders:** [http://localhost:8020/swagger](http://localhost:8020/swagger)

---

## 🚀 Conclusão

Com este fluxo, você consegue:

1. Cadastrar usuários
2. Logar e obter JWT
3. Criar produtos com estoque
4. Atualizar o estoque conforme necessário
5. Efetivar pedidos
6. Consultar o pedidos

Isso cobre todo o ciclo básico de operação dos microserviços.
