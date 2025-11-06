# 🌟 Luminis – Conexão e Apoio Psicológico

**Status do Projeto:** **ONLINE** – Publicação em ambiente de produção (Azure) e configuração de domínio concluídas. Funcionalidades base implementadas.

-----

## 💡 Sobre o Projeto

O **Luminis** é uma plataforma digital dedicada a **conectar pacientes a psicólogos de forma eficiente e intuitiva**. Nosso objetivo é simplificar a captação de clientes para profissionais da saúde mental e, ao mesmo tempo, oferecer um recurso acessível para que pacientes encontrem o apoio que precisam.

**Modelo Operacional:** A plataforma atua estritamente como um intermediador. O contato inicial é facilitado pelo Luminis (via link de agendamento), mas o atendimento, agendamento e a relação terapêutica são realizados diretamente entre o psicólogo e o paciente, garantindo a autonomia e o sigilo profissional.

-----

## 🚀 Funcionalidades Principais

  * **Cadastro de Profissionais:** Psicólogos podem criar perfis detalhados com validações robustas (CPF, CRP, Senha Segura).
  * **Login Seguro:** Autenticação via ASP.NET Core Identity.
  * **Gestão de Perfil:** Área logada para o psicólogo editar dados pessoais, biografia e selecionar áreas de atuação.
  * **Controle Administrativo:** Painel exclusivo para o administrador gerenciar a aprovação (`Ativo`), o destaque (`Em Destaque`) e a exclusão dos perfis.
  * **Exibição Dinâmica:** A Home e a listagem de profissionais exibem perfis aleatórios e em destaque (premium).
  * **Conexão Rápida:** Links diretos e padronizados para agendamento via WhatsApp.
  * **Design Profissional:** Layout responsivo, focado em bem-estar e com cores institucionais.
  * **Dados Dinâmicos:** Planos e perfis são gerenciados via banco de dados, permitindo a alteração de preços sem a necessidade de republicação do código.

-----

## ☁️ Status de Hospedagem (Deployment)

| Componente | Status | URL de Acesso | Observações |
| :--- | :--- | :--- | :--- |
| **Domínio Principal** | ✅ ONLINE | **https://psicologialuminis.com/** | O site está no ar, protegido por HTTPS e utilizando o domínio personalizado. |
| **Base de Dados** | ✅ Ativa | Azure SQL Database | Estrutura completa e usuários iniciados via seeding seguro. |

-----

## ⚙️ Tecnologias Utilizadas

| Categoria | Tecnologia | Detalhes |
| :--- | :--- | :--- |
| **Backend** | C\#, ASP.NET Core MVC | Lógica do servidor, Controllers e Views. |
| **Autenticação** | ASP.NET Core Identity | Gerenciamento seguro de usuários, senhas (hashing) e perfis (Admin/Psicólogo). |
| **Banco de Dados** | Azure SQL Database | Base de dados de produção. |
| **ORM** | Entity Framework Core | Migrações e acesso a dados. |
| **Design/UI** | Bootstrap 5, Poppins Font | Design responsivo, moderno e padronizado. |
| **Hospedagem** | Microsoft Azure | App Service (Básico B1) e Azure SQL. |

-----

## 📧 Contato

  * **E-mail:** [carolina.s.felix.51@gmail.com](mailto:carolina.s.felix.51@gmail.com)

-----
