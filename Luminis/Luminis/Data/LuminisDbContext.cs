using Microsoft.EntityFrameworkCore;
using Luminis.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System; 
using System.Linq;

namespace Luminis.Data
{
    public class LuminisDbContext : IdentityDbContext<IdentityUser>
    {
        public LuminisDbContext(DbContextOptions<LuminisDbContext> options) : base(options)
        {
        }

        public DbSet<Plano> Planos { get; set; }
        public DbSet<Psicologo> Psicologos { get; set; }
        public DbSet<Especialidade> Especialidades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Psicologo>()
                .Property(p => p.SenhaHash)
                .IsRequired(false);

            modelBuilder.Entity<Psicologo>()
                .HasMany(p => p.Especialidades)
                .WithMany(e => e.Psicologos)
                .UsingEntity("PsicologoEspecialidades");



            // 1. Seed Data para Abordagnes
            modelBuilder.Entity<Especialidade>().HasData(
                new Especialidade { Id = 1, Nome = "Terapia Cognitivo-Comportamental (TCC)" },
                new Especialidade { Id = 2, Nome = "Psicanálise" },
                new Especialidade { Id = 3, Nome = "Terapia Humanista" },
                new Especialidade { Id = 4, Nome = "Gestalt-terapia" },
                new Especialidade { Id = 5, Nome = "Terapia Sistêmica" },
                new Especialidade { Id = 6, Nome = "Psicologia Positiva" },
                new Especialidade { Id = 7, Nome = "Neuropsicologia" },
                new Especialidade { Id = 8, Nome = "Psicologia Organizacional" },
                new Especialidade { Id = 9, Nome = "Psicologia Escolar" },
                new Especialidade { Id = 10, Nome = "Psicologia Hospitalar" },
                new Especialidade { Id = 11, Nome = "Psicologia do Esporte" },
                new Especialidade { Id = 12, Nome = "Psicologia Jurídica" },
                new Especialidade { Id = 13, Nome = "Psicologia Social" },
                new Especialidade { Id = 14, Nome = "Orientação Profissional" },
                new Especialidade { Id = 15, Nome = "Terapia de Casal e Família" },
                new Especialidade { Id = 16, Nome = "Psicopedagogia" },
                new Especialidade { Id = 17, Nome = "Saúde Mental Perinatal" }
            );

            // 2. Seed Data para Planos
            modelBuilder.Entity<Plano>().HasData(
                new Plano { Id = 1, Nome = "Plano Mensal", Preco = 50.00m, Periodo = "mês", Descricao = "Flexibilidade para começar.", Recursos = "Perfil público completo;Busca por especialidade;Contato via WhatsApp;Suporte básico", Destaque = false },
                new Plano { Id = 2, Nome = "Plano Trimestral", Preco = 135.00m, Periodo = "trimestre", Descricao = "Economia e visibilidade aprimorada.", Recursos = "Todos os recursos do Plano Mensal;Destaque na página inicial;Desconto progressivo", Destaque = true },
                new Plano { Id = 3, Nome = "Plano Anual", Preco = 480.00m, Periodo = "ano", Descricao = "O melhor custo-benefício.", Recursos = "Todos os recursos do Plano Trimestral;Suporte prioritário;Maior desconto", Destaque = false }
            );

            modelBuilder.Entity<Psicologo>().HasData(new Psicologo
            {
                Id = 1,
                Nome = "Dr.",
                Sobrenome = "Teste",
                CRP = "01/12345",
                CPF = "000.000.000-00", // CPF de teste
                Email = "psicologo@teste.com", // Email usado para ligar ao IdentityUser
                Biografia = "Olá, sou o Dr. Teste, psicólogo com experiência em TCC.",
                FotoUrl = "https://placehold.co/400x400/87CEFA/000000?text=PSICÓLOGO+TESTE",
                WhatsApp = "5511999999999",
                WhatsAppUrl = "https://wa.me/5511999999999",
                DataNascimento = new DateTime(1985, 5, 20),
                Ativo = true,
                EmDestaque = true,
                DataCadastro = DateTime.Now
            });
        }
    }
}