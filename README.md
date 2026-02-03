# Desafio LEVE - Sistema de Gestão de Tarefas

Sistema de gestão de tarefas e usuários desenvolvido com ASP.NET Core Razor Pages, Identity, Entity Framework Core e MailKit.

## 🚀 Funcionalidades Implementadas

### Autenticação e Usuários

- ✅ Autenticação via e-mail e senha (ASP.NET Core Identity)
- ✅ Usuário gestor inicial seed: `ti@leveinvestimentos.com.br` / senha: `teste123`
- ✅ Gestores podem criar novos usuários (Gestor ou Subordinado)
- ✅ Campos de usuário: nome completo, e-mail, data de nascimento, telefones (fixo/móvel), endereço, foto
- ✅ Upload de foto com validação (tipos: jpg, jpeg, png, gif, bmp, webp; tamanho máximo: 5MB)
- ✅ Proteção contra path traversal e sanitização de nomes de arquivo

### Gestão de Tarefas

- ✅ Gestores criam tarefas com título, descrição e data limite
- ✅ Atribuição de tarefas a subordinados via dropdown
- ✅ Subordinados podem marcar tarefas como concluídas
- ✅ Visibilidade por role:
  - **Gestor**: vê tarefas que criou ou atribuídas a si
  - **Subordinado**: vê apenas tarefas atribuídas a si

### Notificações por E-mail

- ✅ E-mail de boas-vindas ao criar usuário
- ✅ E-mail ao atribuir tarefa ao subordinado
- ✅ E-mail ao gestor quando subordinado conclui tarefa
- ✅ Logs de falha de envio de e-mail

## 🏗️ Arquitetura

```
Desafio.Leve.sln
├── Desafio.Leve.Web             # Razor Pages, UI, controllers
├── Desafio.Leve.Domain          # Modelos de domínio (TaskItem)
├── Desafio.Leve.Application     # Lógica de aplicação (vazio por enquanto)
└── Desafio.Leve.Infrastructure  # EF Core, Identity, serviços (Email)
```

## 📋 Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (local ou Docker)
- Conta SMTP para envio de e-mails (Gmail recomendado)

## ⚙️ Configuração e Execução

### 1. Clone o repositório

```bash
git clone <url-do-repositório>
cd Desafio_LEVE
```

### 2. Configure o banco de dados

Edite `Desafio.Leve.Web/appsettings.Development.json` e ajuste a connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=balta;User ID=sa;Password=SUA_SENHA;Trusted_Connection=False;TrustServerCertificate=True;"
  }
}
```

### 3. Configure o SMTP (E-mail)

**Gmail com App Password (recomendado):**

1. Acesse [Google App Passwords](https://myaccount.google.com/apppasswords)
2. Crie uma senha de app para "Mail"
3. Configure os secrets:

```bash
cd Desafio.Leve.Web
dotnet user-secrets init
dotnet user-secrets set "Smtp:Host" "smtp.gmail.com"
dotnet user-secrets set "Smtp:Port" "587"
dotnet user-secrets set "Smtp:UseSsl" "true"
dotnet user-secrets set "Smtp:User" "seu.email@gmail.com"
dotnet user-secrets set "Smtp:Password" "sua-senha-de-app"
dotnet user-secrets set "Smtp:From" "seu.email@gmail.com"
dotnet user-secrets set "Smtp:FromName" "Sistema LEVE"
```

### 4. Aplique as migrations

```bash
cd Desafio.Leve.Web
dotnet ef database update --project ../Desafio.Leve.Infrastructure/Desafio.Leve.Infrastructure.csproj
```

Isso criará as tabelas e aplicará o seed do usuário gestor inicial.

### 5. Execute a aplicação

```bash
dotnet run
```

Acesse: http://localhost:5179

## 🔐 Credenciais Padrão

- **E-mail**: ti@leveinvestimentos.com.br
- **Senha**: teste123
- **Role**: Gestor

## 📂 Estrutura do Banco de Dados

**Tabelas principais:**

- `AspNetUsers` - usuários (Identity + campos customizados)
- `AspNetRoles` - roles (Gestor, Subordinado)
- `AspNetUserRoles` - relação usuário-role
- `Tasks` - tarefas

**Migrations aplicadas:**

- `InitialCreate` - estrutura inicial
- `MakeUserFieldsNullable` - campos de usuário anuláveis
- `MakeFullNameNullable` - FullName anulável
- `MakeTaskUserIdsNullable` - IDs de usuário em tarefas anuláveis

## 🧪 Testes Manuais

1. **Login**: acesse `/Identity/Account/Login` com o usuário gestor seed
2. **Criar usuário**: `/Users/Create` (apenas gestor) - preencha todos os campos incluindo foto
3. **Criar tarefa**: `/Tasks/Create` e atribua a um subordinado
4. **Marcar como concluída**: login como subordinado → `/Tasks` → botão "Marcar como concluída"
5. **Verificar e-mails**: confira inbox do subordinado (criação de tarefa) e do gestor (conclusão)

## 🛠️ Tecnologias

- ASP.NET Core 8 (Razor Pages)
- Entity Framework Core 8
- ASP.NET Core Identity
- SQL Server
- MailKit (SMTP)
- UIkit (CSS framework)

## 📝 Notas de Desenvolvimento

- Uploads são salvos em `wwwroot/uploads/{userId}/`
- Validação de upload: apenas imagens até 5MB
- E-mails falhos são logados mas não bloqueiam operações
- Roles criadas dinamicamente se não existirem
- Validações client-side via `_ValidationScriptsPartial`

## 🚧 Melhorias Futuras

- [ ] Editar perfil de usuário
- [ ] Alterar senha
- [ ] Dashboard com estatísticas
- [ ] Filtros e busca de tarefas
- [ ] Paginação
- [ ] Testes unitários e integração
- [ ] Docker Compose para ambiente completo

## 📄 Licença

Este projeto foi desenvolvido como parte de um desafio técnico.

- Implementar as páginas Razor para login, cadastro/edição de usuários e gerenciamento de tarefas.
- Implementar serviço de envio de e-mails e um worker para notificações.
- Adicionar políticas de autorização (ex.: página de cadastro restrita a `Gestor`).
