# 🧙 UniHog — Universidade de Hogwarts

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![JavaScript](https://img.shields.io/badge/JavaScript-F7DF1E?style=for-the-badge&logo=javascript&logoColor=black)
![Status](https://img.shields.io/badge/status-em%20desenvolvimento-orange?style=for-the-badge)

> Sistema de gestão escolar completo, inspirado no universo de Harry Potter — versão de portfólio derivada de um projeto real desenvolvido voluntariamente para uma ONG.

---

## 📋 Sobre o projeto

O **UniHog** é um sistema de gestão escolar fullstack desenvolvido como versão de portfólio de um projeto **pró-bono** entregue a uma ONG. O sistema original foi construído para atender necessidades reais de gestão educacional — alunos, turmas, frequência, estoque e relatórios.

Para tornar o projeto público sem expor dados sensíveis da instituição parceira, o sistema foi **recriado com identidade visual e dados fictícios** ambientados no universo da Escola de Magia e Bruxaria de Hogwarts, preservando toda a arquitetura, lógica de negócio e complexidade técnica do sistema original.

> 💡 O projeto original (privado) demonstra experiência com entrega real de software para clientes. O UniHog demonstra a arquitetura e o nível técnico dessa entrega, de forma segura e pública.

---

## ✨ Funcionalidades

- 👨‍🎓 **Gestão de alunos** — cadastro, edição e acompanhamento de estudantes
- 📚 **Turmas e disciplinas** — organização de cursos e matérias
- ✅ **Controle de frequência** — registro e acompanhamento de presença
- 📊 **Notas e avaliações** — lançamento e consulta de resultados
- 👤 **Autenticação** — controle de acesso por perfil (administrador, professor, aluno)
- 📋 **Relatórios** — visão gerencial dos dados da instituição

---

## 🏗️ Arquitetura

O projeto segue uma **arquitetura em camadas** organizada por namespaces prefixados, o mesmo padrão adotado no sistema original entregue à ONG:

```
Z1.Model         →  Entidades e modelos de domínio
Z2.Servicos      →  Regras de negócio e casos de uso
Z3.DataAccess    →  Repositórios e acesso ao SQL Server
Z4.Bibliotecas   →  Utilitários e helpers compartilhados
UniHog/          →  Camada de apresentação (front-end web)
```

---


## 🛠️ Tecnologias utilizadas

| Tecnologia | Uso |
|-----------|-----|
| C# / .NET | Back-end e lógica de negócio |
| SQL Server | Banco de dados relacional |
| HTML / CSS / JavaScript | Front-end web |
| Arquitetura em Camadas | Separação de responsabilidades |

---

## 🗺️ Roadmap

- [ ] Dashboard com visão geral da instituição
- [ ] Relatórios exportáveis (PDF/Excel)
- [ ] Notificações e comunicados para alunos e responsáveis
- [ ] Deploy público para demonstração

---

## 👨‍💻 Autor

Feito por **[Faezito](https://github.com/Faezito)**

[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](https://linkedin.com/in/seu-perfil)
[![GitHub](https://img.shields.io/badge/GitHub-100000?style=for-the-badge&logo=github&logoColor=white)](https://github.com/Faezito)
