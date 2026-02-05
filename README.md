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
- ✅ Listagem de usuários com foto, dados de contato e data de nascimento
- ✅ **Status de usuário (Ativo/Inativo)**:
  - Gestores podem desativar/ativar usuários que criaram
  - Indicador visual (badge) no status do usuário
  - Confirmação antes de desativar
- ✅ Página protegida por role (apenas Gestor pode criar/gerenciar usuários)

### Gestão de Tarefas

- ✅ Gestores criam tarefas com título, descrição e data/hora limite
- ✅ Atribuição de tarefas a subordinados via dropdown
- ✅ Subordinados podem marcar tarefas como concluídas
- ✅ **Exclusão de tarefas não concluídas**:
  - Apenas gestores podem deletar tarefas
  - Apenas o criador da tarefa pode deletá-la
  - Apenas tarefas pendentes podem ser deletadas
  - Confirmação antes de excluir
- ✅ Visibilidade baseada em role:
  - **Gestor**: vê tarefas que criou ou foram atribuídas a si
  - **Subordinado**: vê apenas tarefas atribuídas a si
- ✅ Filtros automáticos de tarefas por usuário e role
- ✅ Status visual (Pendente/Concluída) com labels coloridos

### Notificações por E-mail

- ✅ E-mail de boas-vindas ao criar usuário
- ✅ E-mail de notificação ao atribuir tarefa ao subordinado
- ✅ E-mail ao gestor quando subordinado conclui tarefa
- ✅ Integração com MailKit para SMTP confiável

### Interface do Usuário (UIKit)

- ✅ Framework CSS UIKit v3.21.6 integrado globalmente
- ✅ Layout responsivo com navbar
- ✅ Dashboard com cards interativos e grid responsivo
- ✅ Formulários organizados com fieldsets e ícones
- ✅ Tabelas estilizadas com striped rows e hover effects
- ✅ Labels e botões com ícones UIKit
- ✅ Alertas e mensagens de feedback visual
- ✅ Upload de foto customizado com UIKit form-custom

## 🏗️ Arquitetura

```
Desafio.Leve.sln
├── Desafio.Leve.Web             # Razor Pages, UI, controllers
├── Desafio.Leve.Domain          # Modelos de domínio (TaskItem)
└── Desafio.Leve.Infrastructure  # EF Core, Identity, serviços (Email)
```

**Nota**: A lógica de aplicação está implementada diretamente nas Razor Pages e serviços da camada Infrastructure.

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

Acesse: <http://localhost:5179>

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
- `AddIsActiveToUsers` - adiciona campo IsActive para controle de status de usuários

## 🎨 Páginas e Recursos

### Dashboard (/)

- Cards interativos com links rápidos para principais ações
- Exibição de role do usuário logado (Gestor/Subordinado)
- Grid responsivo (3 colunas em desktop, 1 em mobile)
- Ícones UIKit para cada seção

### Usuários (/Users)

- **Gestores**: visualizam apenas usuários subordinados que eles criaram

- **Index**: Tabela com foto circular, nome, email, telefones, data de nascimento e status (Ativo/Inativo)
  - Botão "Editar" para modificar dados do usuário
  - Botão "Desativar" (para usuários ativos) - com confirmação
  - Botão "Ativar" (para usuários inativos)
  - Indicadores visuais com labels coloridas (verde para Ativo, vermelho para Inativo)
- **Create**: Formulário completo organizado em 3 seções:
  - **Acesso**: Email, senha, role (obrigatório)
  - **Pessoais**: Nome, data nascimento, foto
  - **Contato**: Telefones (fixo/móvel), endereço
  - Upload de foto customizado com UIKit form-custom
  - Validação inline de todos os campos

### Tarefas (/Tasks)

- **Index**: Tabela com status visual (labels coloridas Pendente/Concluída)
  - Botão "Concluir" para tarefas pendentes (apenas para responsável)
  - Botão "Excluir" para tarefas pendentes (apenas para criador/gestor)
  - Mensagens de sucesso e erro com alertas visuais
- **Create**: Formulário com: limite (apenas data, sem hora)
  - Dropdown de subordinados para atribuição
  - Validação de campos obrigatórios
  - Validação de existência de usuário (previne FK violations)
  - Conversão automática de timezone para São Paulo (UTC-3) na exibição
  - Validação de campos obrigatórios

## 🧪 Testes Manuais Recomendados

### Fluxo Completo

1. **Login como Gestor**
   - Acesse `/Identity/Account/Login`
   - Use: `ti@leveinvestimentos.com.br` / `teste123`

2. **Criar Subordinado**
   - Acesse `/Users/Create`
   - Preencha todos os campos (incluindo foto)
   - Selecione role "Subordinado" (obrigatório)
   - Verifique role dropdown permanece após erros de validação
   - Verifique e-mail de boas-vindas

3. **Ativar/Desativar Usuário**
   - Acesse `/Users/Index`
   - Clique em "Desativar" para um usuário ativo
   - Confirme a ação
   - Verifique status muda para "Inativo" (badge vermelho)
   - Clique em "Ativar" para reativar
   - Verifique status volta para "Ativo" (badge verde)

4. **Criar Tarefa**
   - Acesse `/Tasks/Create`
   - Atribua ao subordinado criado
   - Verifique e-mail de notificação do subordinado

5. **Excluir Tarefa Pendente**
   - Acesse `/Tasks/Index`
   - Clique em "Excluir" em uma tarefa pendente
   - Confirme a ação
   - Verifique mensagem de sucesso "Tarefa excluída com sucesso"
   - Verifique tarefa foi removida da lista

6. **Concluir Tarefa como Subordinado**
   - Logout e login com o subordinado
   - Acesse `/Tasks/Index`
   - Clique em "Concluir" na tarefa
   - Verifique e-mail do gestor sobre conclusão
   - Verifique botão "Excluir" desaparece (tarefa concluída não pode ser deletada)

### Testes de Segurança

- Tente acessar `/Users/Create` como Subordinado (deve retornar Forbidden)
- Tente desativar/ativar usuário de outro gestor (deve retornar Forbidden)
- Tente deletar tarefa que não criou (deve mostrar mensagem de erro)
- Tente deletar tarefa já concluída (deve mostrar mensagem de erro)
- Upload de arquivo > 5MB (deve ser rejeitado com mensagem)
- Upload de arquivo .exe ou .pdf (deve ser rejeitado)
- Criar tarefa sem campos obrigatórios (validação inline deve impedir submit)
- Criar usuário sem selecionar função (validação deve mostrar erro)

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
  - Extensões permitidas (jpg, jpeg, png, gif, bmp, webp)
  - Limite de 5MB por arquivo
  - Geração de nome único para evitar conflitos
  - Validação ANTES de criar usuário (evita usuários órfãos)
- **Status de usuário**: Campo `IsActive` booleano com padrão true
- **Exclusão de tarefas**:
  - Apenas tarefas pendentes podem ser deletadas
  - Apenas o criador pode deletar sua tarefa
  - Apenas gestores têm permissão de delete

### E-mail e Logging

- E-mails falhos são logados com `ILogger` mas não bloqueiam operações
- Roles (Gestor/Subordinado) são criadas automaticamente na inicialização
- Seed do usuário gestor ocorre automaticamente no startup
- Notificações por e-mail: boas-vindas, atribuição de tarefa, conclusão de tarefa

### Interface

- UIKit framework via CDN para melhor performance
- Ícones UIKit integrados em toda interface
- Formulários organizados em fieldsets lógicos (Acesso, Pessoais, Contato)
- Feedback visual com labels coloridos, alertas e estados hover
- Navegação responsiva com menu mobile
- Design consistente em todas as páginas
- Validações client-side via `_ValidationScriptsPartial`

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
- Boas práticas de segurança
