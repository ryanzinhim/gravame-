# Gravame Manager

![Badge](https://img.shields.io/badge/status-em%20desenvolvimento-yellow)

API para consulta e gerenciamento de gravames de veículos.

## 📌 Funcionalidades
- Consultar informações de gravame de veículos
- Cadastrar novos registros
- Atualizar informações existentes
- Remover registros
- Cancelamento de apontamentos
- Integração com GraphQL para consultas avançadas
- Mapeamento automático de objetos para DTOs
- Tratamento de erros e validações
- Configuração via Docker

## 🚀 Tecnologias Utilizadas
- C#
- .NET
- Entity Framework Core
- SQL Server
- Docker
- GraphQL
- AutoMapper


## 📦 Instalação

### Pré-requisitos
- .NET SDK 7+
- Docker (caso queira rodar o banco de dados localmente)

⚙️ Mecanismos Implementados
🔄 Troca de Placa
O sistema permite a tentativa de troca de placa, garantindo que as regras de negócio sejam aplicadas corretamente antes da atualização dos dados.

🔗 Conexão com Banco de Dados
O Entity Framework Core gerencia a conexão com o banco de dados SQL Server, garantindo transações seguras e integridade dos dados.

🐳 Configuração com Docker
O projeto pode ser executado dentro de um contêiner Docker para facilitar o desenvolvimento e implantação.

🛠️ Mapeamento de Objetos
O MappingProfile.cs utiliza AutoMapper para transformar entidades em DTOs, facilitando a comunicação entre a API e os clientes.

🛡️ Tratamento de Erros e Validações
Os serviços incluem validações e tratamento de erros, assegurando que as requisições sejam processadas corretamente e evitando falhas inesperadas.

🔐 Autenticação
A API utiliza autenticação baseada em tokens. Para acessar endpoints protegidos, inclua um token de autenticação no cabeçalho da requisição.

 Principais Classes e Serviços
GravamController.cs: Controlador responsável por gerenciar as requisições relacionadas a gravames.
RepositoryBase.cs: Implementação base para repositórios de dados.
GraphQLService.cs: Serviço de integração com GraphQL.
AuthServicies.cs: Serviço de autenticação e gerenciamento de usuários.
ServiceExtensions.cs: Extensões para configuração de serviços no container de injeção de dependências.
ApontamentoServiceFactory.cs: Fábrica de serviços para apontamentos.
MappingProfile.cs: Configuração de mapeamento de objetos utilizando AutoMapper.
