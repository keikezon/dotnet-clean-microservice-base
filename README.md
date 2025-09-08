# 🏗️ CleanMicroserviceBase

Este projeto é composto por vários microserviços independentes que juntos formam um sistema de controle de produto e ordem de serviço, com cadastro a autenticação de usuário (Admin/Seller).

## 📚 Arquitetura
O sistema segue uma arquitetura baseada em **microserviços** com comunicação via **RabbitMQ** e **REST APIs**.

Principais tecnologias:
- .NET 8 / C#
- MassTransit + RabbitMQ
- PostgreSQL
- Docker + Docker Compose
- OpenTelemetry + Jaeger

## 🧩 Serviços
| Serviço  | Porta | Descrição                     | Docs                                    |
|----------|-------|-------------------------------|-----------------------------------------|
| Users    | 8080  | Gerencia usuários e auth      | [users.md](./docs/users.md)      |
| Orders   | 8020  | Gerencia pedidos              | [orders.md](./docs/orders.md)    |
| Products | 8010  | Controle de produto e estoque | [products.md](./docs/products.md) |
| Api    | 8000  | Api Gateway a ser usada      |       |
| RabbitMQ | 15672 | Mensageria                    |  |
| Jaeger | 16686 | Telemetria                    |  |

## 🛒 Guia de Uso - Cadastro, Login, Produtos e Pedidos
**Fluxo para testar a api:** [Clique aqui](./docs/guide.md)

---

## 🚀 Como rodar localmente

1. **Clonar o repositório**
   ```bash
   git clone git@github.com:keikezon/dotnet-clean-microservice-base.git
   cd dotnet-clean-microservice-base
   ```

2. **Subir ambiente**
   ```bash
   docker-compose up -build
   ```

3. **Rodar os testes**

   ```bash
   dotnet test
   ```
---

[Link do postman](https://web.postman.co/workspace/My-Workspace~8b18a694-61c8-406f-8258-cc76086c3374/collection/412296-0e1db38a-3a59-49ea-b4e6-09668c3d2b6f?action=share&source=copy-link&creator=412296)

👥 Contribuidores

Keith Zonatto
- Desenvolvedora Full Stack
