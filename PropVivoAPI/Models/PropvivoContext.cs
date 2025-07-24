using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PropVivoAPI.Models;

public partial class PropvivoContext : DbContext
{
    private IConfiguration _configuration;
    public PropvivoContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public PropvivoContext(DbContextOptions<PropvivoContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<BreakLogTracking> BreakLogTrackings { get; set; }

    public virtual DbSet<QueryMaster> QueryMasters { get; set; }

    public virtual DbSet<QueryResponse> QueryResponses { get; set; }

    public virtual DbSet<RoleMaster> RoleMasters { get; set; }

    public virtual DbSet<TaskAssignment> TaskAssignments { get; set; }

    public virtual DbSet<TaskMaster> TaskMasters { get; set; }

    public virtual DbSet<TaskTimeTracking> TaskTimeTrackings { get; set; }

    public virtual DbSet<UserMaster> UserMasters { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=LAPTOP-L93QS3JL\\SQLEXPRESS;Initial Catalog=PROPVIVO;Integrated Security=True;Trust Server Certificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BreakLogTracking>(entity =>
        {
            entity.HasKey(e => e.BreakLogId).HasName("PK__break_lo__5C45259E46E78A0C");

            entity.ToTable("break_log_tracking");

            entity.Property(e => e.BreakLogId).HasColumnName("break_log_id");
            entity.Property(e => e.EndedAt)
                .HasColumnType("datetime")
                .HasColumnName("ended_at");
            entity.Property(e => e.StartedAt)
                .HasColumnType("datetime")
                .HasColumnName("started_at");
            entity.Property(e => e.TaskAssignId).HasColumnName("task_assign_id");
            entity.Property(e => e.TotalTime)
                .HasColumnType("datetime")
                .HasColumnName("total_time");

            entity.HasOne(d => d.TaskAssign).WithMany(p => p.BreakLogTrackings)
                .HasForeignKey(d => d.TaskAssignId)
                .HasConstraintName("FK__break_log__task___48CFD27E");
        });

        modelBuilder.Entity<QueryMaster>(entity =>
        {
            entity.HasKey(e => e.QueryId).HasName("PK__query_ma__E793E3494BAC3F29");

            entity.ToTable("query_master");

            entity.Property(e => e.QueryId).HasColumnName("query_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.IssueAttachment)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("issue_attachment");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.Subject)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("subject");
            entity.Property(e => e.TaskAssignId).HasColumnName("task_assign_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.TaskAssign).WithMany(p => p.QueryMasters)
                .HasForeignKey(d => d.TaskAssignId)
                .HasConstraintName("FK__query_mas__task___4D94879B");
        });

        modelBuilder.Entity<QueryResponse>(entity =>
        {
            entity.HasKey(e => e.QueryResponseId).HasName("PK__query_re__0AB53E075070F446");

            entity.ToTable("query_response");

            entity.Property(e => e.QueryResponseId).HasColumnName("query_response_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Message)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("message");
            entity.Property(e => e.QueryId).HasColumnName("query_id");

            entity.HasOne(d => d.Query).WithMany(p => p.QueryResponses)
                .HasForeignKey(d => d.QueryId)
                .HasConstraintName("FK__query_res__query__52593CB8");
        });

        modelBuilder.Entity<RoleMaster>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__role_mas__760965CC2F10007B");

            entity.ToTable("role_master");

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("role_id");
            entity.Property(e => e.RoleName)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("role_name");
        });

        modelBuilder.Entity<TaskAssignment>(entity =>
        {
            entity.HasKey(e => e.TaskAssignId).HasName("PK__task_ass__8736D5683C69FB99");

            entity.ToTable("task_assignment");

            entity.Property(e => e.TaskAssignId).HasColumnName("task_assign_id");
            entity.Property(e => e.AssignedAt)
                .HasColumnType("datetime")
                .HasColumnName("assigned_at");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("status");
            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Task).WithMany(p => p.TaskAssignments)
                .HasForeignKey(d => d.TaskId)
                .HasConstraintName("FK__task_assi__task___3F466844");

            entity.HasOne(d => d.User).WithMany(p => p.TaskAssignments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__task_assi__user___3E52440B");
        });

        modelBuilder.Entity<TaskMaster>(entity =>
        {
            entity.HasKey(e => e.TaskId).HasName("PK__task_mas__0492148D37A5467C");

            entity.ToTable("task_master");

            entity.Property(e => e.TaskId).HasColumnName("task_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.Description)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(e => e.EstimatedHrs)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("estimated_hrs");
            entity.Property(e => e.TaskTitle)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("task_title");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.TaskMasters)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__task_mast__creat__398D8EEE");
        });

        modelBuilder.Entity<TaskTimeTracking>(entity =>
        {
            entity.HasKey(e => e.TaskTimeId).HasName("PK__task_tim__947B32DB4222D4EF");

            entity.ToTable("task_time_tracking");

            entity.Property(e => e.TaskTimeId).HasColumnName("task_time_id");
            entity.Property(e => e.EndedAt)
                .HasColumnType("datetime")
                .HasColumnName("ended_at");
            entity.Property(e => e.StartedAt)
                .HasColumnType("datetime")
                .HasColumnName("started_at");
            entity.Property(e => e.TaskAssignId).HasColumnName("task_assign_id");
            entity.Property(e => e.TotalTime)
                .HasColumnType("datetime")
                .HasColumnName("total_time");

            entity.HasOne(d => d.TaskAssign).WithMany(p => p.TaskTimeTrackings)
                .HasForeignKey(d => d.TaskAssignId)
                .HasConstraintName("FK__task_time__task___440B1D61");
        });

        modelBuilder.Entity<UserMaster>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__user_mas__B9BE370F32E0915F");

            entity.ToTable("user_master");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Email)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(300)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.UserMasters)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__user_mast__role___34C8D9D1");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
