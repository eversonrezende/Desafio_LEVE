# Desafio LEVE - Sistema de Gestão de Tarefas

Sistema de gestão de tarefas e usuários desenvolvido com ASP.NET Core Razor Pages, Identity, Entity Framework Core, MailKit e UIKit CSS framework.

## 🚀 Funcionalidades Implementadas

### Autenticação e Usuários

- ✅ Autenticação via e-mail e senha (ASP.NET Core Identity)
- ✅ Usuário gestor inicial seed: `ti@leveinvestimentos.com.br` / senha: `teste123`
- ✅ Gestores podem criar novos usuários (Gestor ou Subordinado)
- ✅ Campos de usuário completos: nome completo, e-mail, data de nascimento, telefones (fixo/móvel), endereço, foto
- ✅ Upload de foto com validação robusta:
  - Tipos permitidos: jpg, jpeg, png, gif, bmp, webp
  - Tamanho máximo: 5MB
  - Sanitização de nomes de arquivo com GUID único
  - Proteção contra path traversal
- ✅ Listagem de usuários com foto, dados de contato e data de nascimento
- ✅ Página protegida por role (apenas Gestor pode criar/gerenciar usuários)

### Gestão de Tarefas

- ✅ Gestores criam tarefas com título, descrição e data/hora limite
- ✅ Atribuição de tarefas a subordinados via dropdown
- ✅ Subordinados podem marcar tarefas como concluídas
- ✅ Visibilidade baseada em role:
  - **Gestor**: vê tarefas que criou ou foram atribuídas a si
  - **Subordinado**: vê apenas tarefas atribuídas a si
- ✅ Filtros automáticos de tarefas por usuário e role
- ✅ Status visual (Pendente/Concluída) com labels coloridos

### Notificações por E-mail

- ✅ E-mail de boas-vindas ao criar usuário
- ✅ E-mail de notificação ao atribuir tarefa ao subordinado
- ✅ E-mail ao gestor quando subordinado conclui tarefa
- ✅ Tratamento de erros com logging (falhas de e-mail não bloqueiam operações)
- ✅ Integração com MailKit para SMTP confiável

### Interface do Usuário (UIKit)

- ✅ Framework CSS UIKit v3.21.6 integrado globalmente
- ✅ Layout responsivo com navbar moderna
- ✅ Dashboard com cards interativos e grid responsivo
- ✅ Formulários organizados com fieldsets e ícones
- ✅ Tabelas estilizadas com striped rows e hover effects
- ✅ Labels e botões com ícones UIKit
- ✅ Alertas e mensagens de feedback visual
- ✅ Upload de foto customizado com UIKit form-custom
- ✅ Design mobile-first e totalmente responsivo

## 🏗️ Arquitetura

```
Desafio.Leve.sln
├── Desafio.Leve.Web             # Razor Pages, UI, controllers
├── Desafio.Leve.Domain          # Modelos de domínio (TaskItem)
└── Desafio.Leve.Infrastructure  # EF Core, Identity, serviços (Email)
```

**Nota**: O projeto Application foi removido por não estar sendo utilizado. A lógica de aplicação está implementada diretamente nas Razor Pages e serviços da camada Infrastructure.

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

Use **User Secrets** para não expor credenciais no repositório:

```bash
cd Desafio.Leve.Web
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=desafio_leve;User ID=sa;Password=SUA_SENHA;Trusted_Connection=False;TrustServerCertificate=True;"
```

> Observação: o arquivo `appsettings.Development.json` contém apenas um placeholder (`Password=CHANGEME`).

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

- `InitialCreate` - estrutura inicial com Identity e Tasks
- `MakeUserFieldsNullable` - campos de usuário anuláveis
- `MakeFullNameNullable` - FullName anulável
- `MakeTaskUserIdsNullable` - IDs de usuário em tarefas anuláveis
- `AddCreatedByIdToUsers` - adiciona campo CreatedById para rastreamento de criação de usuários
- `AddTaskUserNavigationProperties` - adiciona índices e foreign keys para relações entre Tasks e Users

## 🎨 Páginas e Recursos

### Dashboard (/)

- Cards interativos com links rápidos para principais ações
- Exibição de role do usuário logado (Gestor/Subordinado)
- Grid responsivo (3 colunas em desktop, 1 em mobile)
- Ícones UIKit para cada seção

### Usuários (/Users)

- **Gestores**: visualizam apenas usuários subordinados que eles criaram

- **Index**: Tabela com foto circular, nome, email, telefones, data de nascimento
- **Create**: Formulário completo organizado em 3 seções:
  - **Acesso**: Email, senha, role
  - **Pessoais**: Nome, data nascimento, foto
  - **Contato**: Telefones (fixo/móvel), endereço
  - Upload de foto customizado com UIKit form-custom
  - Validação inline de todos os campos

### Tarefas (/Tasks)

- **Index**: Tabela com status visual (labels coloridas Pendente/Concluída)
  - Filtros automáticos por role
  - Botão "Concluir" para tarefas pendentes
- **Create**: Formulário com: limite (apenas data, sem hora)
  - Dropdown de subordinados para atribuição
  - Validação de campos obrigatórios
  - Conversão automática de timezone para São Paulo (UTC-3) na exibiçãoribuição
  - Validação de campos obrigatórios

## 🧪 Testes Manuais Recomendados

### Fluxo Completo

1. **Login como Gestor**
   - Acesse `/Identity/Account/Login`
   - Use: `ti@leveinvestimentos.com.br` / `teste123`

2. **Criar Subordinado**
   - Acesse `/Users/Create`
   - Preencha todos os campos (incluindo foto)
   - Selecione role "Subordinado"
   - Verifique e-mail de boas-vindas

3. **Criar Tarefa**
   - Acesse `/Tasks/Create`
   - Atribua ao subordinado criado
   - Verifique e-mail de notificação do subordinado

4. **Concluir Tarefa como Subordinado**
   - Logout e login com o subordinado
   - Acesse `/Tasks/Index`
   - Clique em "Concluir" na tarefa
   - Verifique e-mail do gestor sobre conclusão

5. **Verificar Responsividade**
   - Teste em mobile (menu hamburger)
   - Redimensione janela para ver grid adaptativo
   - Verifique tabelas responsivas

### Testes de Segurança

- Tente acessar `/Users/Create` como Subordinado (deve retornar Forbidden)
- Upload de arquivo > 5MB (deve ser rejeitado com mensagem)
- Upload de arquivo .exe ou .pdf (deve ser rejeitado)
- Criar tarefa sem campos obrigatórios (validação inline deve impedir submit)

## 🛠️ Tecnologias

- **Backend**: ASP.NET Core 8 (Razor Pages)
- **ORM**: Entity Framework Core 8
- **Autenticação**: ASP.NET Core Identity
- **Banco de Dados**: SQL Server
- **E-mail**: MailKit (SMTP)
- **Front-end**: UIKit v3.21.6 (CSS framework via CDN)
- **Validação**: Data Annotations + jQuery Validation

## 📝 Notas de Desenvolvimento

### Armazenamento e Segurança

- **Uploads**: Salvos em `wwwroot/uploads/{userId}/` com nome sanitizado (GUID + extensão)
- **Validação de upload**:
  - Whitelist de extensões permitidas (jpg, jpeg, png, gif, bmp, webp)
  - Limite de 5MB por arquivo
  - Verificação de path traversal
  - Geração de nome único para evitar conflitos

### E-mail e Logging

- E-mails falhos são logados com `ILogger` mas não bloqueiam operações
- Roles (Gestor/Subordinado) são criadas automaticamente na inicialização
- Seed do usuário gestor ocorre automaticamente no startup

### Interface e UX

- UIKit framework via CDN para melhor performance
- Ícones UIKit integrados em toda interface
- Formulários organizados em fieldsets lógicos (Acesso, Pessoais, Contato)
- Feedback visual com labels coloridos, alertas e estados hover
- Navegação responsiva com menu mobile
- Design consistente em todas as páginas
- Validações client-side via `_ValidationScriptsPartial`

## 🚧 Melhorias Futuras

- [ ] Editar perfil de usuário
- [ ] Alterar senha
- [ ] Dashboard com estatísticas de tarefas
- [ ] Filtros avançados e busca de tarefas
- [ ] Paginação nas listagens
- [ ] Comentários em tarefas
- [ ] Anexos em tarefas
- [ ] Notificações em tempo real (SignalR)
- [ ] Histórico de alterações
- [ ] Testes unitários e de integração
- [ ] Docker Compose para ambiente completo
- [ ] CI/CD pipeline

## 👨‍💻 Desenvolvimento

### Estrutura de Diretórios

```
Desafio.Leve.Web/
├── Pages/
│   ├── Index.cshtml/cs              # Dashboard
│   ├── Shared/
│   │   ├── _Layout.cshtml           # Layout global com UIKit
│   │   └── _LoginPartial.cshtml     # Menu de usuário
│   ├── Tasks/
│   │   ├── Index.cshtml/cs          # Lista de tarefas
│   │   └── Create.cshtml/cs         # Criar tarefa
│   └── Users/
│       ├── Index.cshtml/cs          # Lista de usuários
│       └── Create.cshtml/cs         # Criar usuário
├── wwwroot/
│   └── uploads/                     # Diretório de uploads
│       └── {userId}/                # Fotos por usuário
└── Program.cs                       # Configuração e startup

Desafio.Leve.Infrastructure/
├── Identity/
│   └── ApplicationUser.cs           # Modelo customizado Identity
├── Services/
│   ├── IEmailSender.cs              # Interface de e-mail
│   ├── EmailOptions.cs              # Configurações SMTP
│   └── MailKitEmailSender.cs        # Implementação MailKit
└── AppDbContext.cs                  # Contexto EF Core

Desafio.Leve.Domain/
└── Models/
    └── TaskItem.cs                  # Entidade de tarefa
```

### Comandos Úteis

```bash
# Build
dotnet build

# Executar
dotnet run --project Desafio.Leve.Web

# Nova migration
dotnet ef migrations add NomeDaMigration --project Desafio.Leve.Infrastructure --startup-project Desafio.Leve.Web

# Aplicar migrations
dotnet ef database update --project Desafio.Leve.Infrastructure --startup-project Desafio.Leve.Web

# Listar migrations
dotnet ef migrations list --project Desafio.Leve.Infrastructure --startup-project Desafio.Leve.Web

# Remover última migration (se ainda não aplicada)
dotnet ef migrations remove --project Desafio.Leve.Infrastructure --startup-project Desafio.Leve.Web
```

## 📄 Licença

Este projeto foi desenvolvido como parte de um desafio técnico para demonstração de habilidades em:

- ASP.NET Core Razor Pages
- Entity Framework Core e migrations
- ASP.NET Core Identity com roles customizadas
- Integração com serviços SMTP (MailKit)
- Upload e validação de arquivos
- UIKit CSS framework
- Autorização baseada em roles
- Boas práticas de segurança e logging
